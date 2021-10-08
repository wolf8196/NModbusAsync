using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NModbusAsync.Messages;
using NModbusAsync.Utility;

namespace NModbusAsync.IO
{
    internal sealed class ModbusRtuOverTcpTransport : ModbusTransport
    {
        private const int SlaveAddressSize = sizeof(byte);
        private const int CrcSize = 2;
        private const int ResponseFrameStartSize = 3;

        public ModbusRtuOverTcpTransport(IPipeResource pipeResource, IModbusLogger logger, ITransactionIdProvider transactionIdProvider)
            : base(pipeResource, transactionIdProvider, logger)
        {
        }

        protected override async Task WriteRequestAsync(IModbusRequest request, CancellationToken token = default)
        {
            var totalSize = SlaveAddressSize + CrcSize + request.ByteSize;
            using (var memoryOwner = MemoryPool<byte>.Shared.Rent(totalSize))
            {
                var memory = memoryOwner.Memory;

                memory.Span[0] = request.SlaveAddress;
                request.WriteTo(memory.Slice(SlaveAddressSize, request.ByteSize));

                ushort crc = CrcCalculator.Calculate(memory.Slice(0, SlaveAddressSize + request.ByteSize).Span);
                NetCoreBitConverter.TryWriteBytes(memory.Slice(SlaveAddressSize + request.ByteSize, CrcSize).Span, crc);

                await PipeResource.WriteAsync(memory.Slice(0, totalSize), token).ConfigureAwait(false);
            }
        }

        protected override async Task<IModbusResponse> ReadResponseAsync<TResponse>(CancellationToken token = default)
        {
            var buffer = await PipeResource.ReadAsync(token).ConfigureAwait(false);

            while (buffer.Length < ResponseFrameStartSize)
            {
                PipeResource.AdvanceTo(buffer.Start);
                buffer = await PipeResource.ReadAsync(token).ConfigureAwait(false);
            }

            var frameDataLength = GetResponseDataSize(buffer.Slice(0, ResponseFrameStartSize).ToSpan());
            var frameTotalLength = ResponseFrameStartSize + frameDataLength + CrcSize;

            while (buffer.Length < frameTotalLength)
            {
                PipeResource.AdvanceTo(buffer.Start);
                buffer = await PipeResource.ReadAsync(token).ConfigureAwait(false);
            }

            var processedSequence = buffer.Slice(0, frameTotalLength);

            var expectedCrc = CrcCalculator.Calculate(processedSequence.Slice(0, frameTotalLength - CrcSize).ToSpan());
            var actualCrc = NetCoreBitConverter.ToUInt16(processedSequence.Slice(frameTotalLength - CrcSize, CrcSize).ToSpan());

            if (actualCrc != expectedCrc)
            {
                PipeResource.AdvanceTo(processedSequence.End);
                throw new IOException($@"Received unexpected CRC.
Expected: {expectedCrc}.
Received. {actualCrc}.");
            }

            var response = ModbusResponseFactory.CreateResponse<TResponse>(processedSequence.ToSpan());
            PipeResource.AdvanceTo(processedSequence.End);

            return response;
        }

        protected override bool RetryReadResponse(IModbusRequest request, IModbusResponse response)
        {
            return false;
        }

        protected override void Validate(IModbusRequest request, IModbusResponse response)
        {
            request.Validate(response);
        }

        private int GetResponseDataSize(ReadOnlySpan<byte> frameStart)
        {
            var functionCode = frameStart[1];

            if (functionCode > Constants.ExceptionOffset)
            {
                return 0;
            }

            switch (functionCode)
            {
                case ModbusFunctionCodes.ReadCoils:
                case ModbusFunctionCodes.ReadInputs:
                case ModbusFunctionCodes.ReadHoldingRegisters:
                case ModbusFunctionCodes.ReadInputRegisters:
                    return frameStart[2];

                case ModbusFunctionCodes.WriteSingleCoil:
                case ModbusFunctionCodes.WriteSingleRegister:
                case ModbusFunctionCodes.WriteMultipleCoils:
                case ModbusFunctionCodes.WriteMultipleRegisters:
                    return 3;

                default:
                    throw new ArgumentException($"Cannot extract data length because function code is unknown. Function code: {functionCode}.");
            }
        }
    }
}