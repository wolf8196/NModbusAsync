using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NModbusAsync.IO.Abstractions;
using NModbusAsync.Messages;
using NModbusAsync.Utility;

namespace NModbusAsync.IO
{
    internal sealed class ModbusRtuOverTcpTransport : ModbusTransport
    {
        private const int SlaveAddressSize = sizeof(byte);
        private const int CrcSize = 2;
        private const int ResponseFrameStartSize = 3;

        private readonly ICrcCalculator crcCalculator;

        public ModbusRtuOverTcpTransport(IPipeResource pipeResource, ITransactionIdProvider transactionIdProvider, ICrcCalculator crcCalculator, ILogger<IModbusMaster> logger)
            : base(pipeResource, transactionIdProvider, logger)
        {
            this.crcCalculator = crcCalculator;
        }

        protected override async Task WriteRequestAsync(IModbusRequest request, CancellationToken token = default)
        {
            var totalSize = SlaveAddressSize + CrcSize + request.ByteSize;
            using var memoryOwner = MemoryPool<byte>.Shared.Rent(totalSize);
            var memory = memoryOwner.Memory;

            memory.Span[0] = request.SlaveAddress;
            request.WriteTo(memory.Slice(SlaveAddressSize, request.ByteSize));

            ushort crc = crcCalculator.Calculate(memory.Slice(0, SlaveAddressSize + request.ByteSize));
            BitConverter.TryWriteBytes(memory.Slice(SlaveAddressSize + request.ByteSize, CrcSize).Span, crc);

            await PipeResource.WriteAsync(memory.Slice(0, totalSize), token).ConfigureAwait(false);
        }

        protected override async Task<IModbusResponse> ReadResponseAsync<TResponse>(CancellationToken token = default)
        {
            ReadOnlySequence<byte> processedSequence = ReadOnlySequence<byte>.Empty;
            try
            {
                var buffer = await PipeResource.ReadAsync(token).ConfigureAwait(false);
                processedSequence = buffer;

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

                processedSequence = buffer.Slice(0, frameTotalLength);

                var expectedCrc = crcCalculator.Calculate(processedSequence.Slice(0, frameTotalLength - CrcSize).ToMemory());
                var actualCrc = BitConverter.ToUInt16(processedSequence.Slice(frameTotalLength - CrcSize, CrcSize).ToSpan());

                if (actualCrc != expectedCrc)
                {
                    throw new IOException($"Received unexpected CRC. Expected: {expectedCrc}. Received: {actualCrc}.");
                }

                var response = ModbusResponseFactory.CreateResponse<TResponse>(processedSequence.ToSpan());

                return response;
            }
            finally
            {
                PipeResource.AdvanceTo(processedSequence.End);
            }
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