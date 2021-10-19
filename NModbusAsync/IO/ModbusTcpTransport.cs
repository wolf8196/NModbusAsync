using System.Buffers;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NModbusAsync.IO.Abstractions;
using NModbusAsync.Messages;
using NModbusAsync.Utility;

namespace NModbusAsync.IO
{
    internal sealed class ModbusTcpTransport : ModbusTransport
    {
        private const int MbapHeaderSizeOnRequest = 7;
        private const int MbapHeaderSizeOnResponse = 6;

        internal ModbusTcpTransport(IPipeResource pipeResource, ITransactionIdProvider transactionIdProvider, ILogger<IModbusMaster> logger)
            : base(pipeResource, transactionIdProvider, logger)
        {
        }

        protected override async Task WriteRequestAsync(IModbusRequest request, CancellationToken token = default)
        {
            using (var memoryOwner = MemoryPool<byte>.Shared.Rent(MbapHeaderSizeOnRequest + request.ByteSize))
            {
                var memory = memoryOwner.Memory;

                NetCoreBitConverter.TryWriteBytes(memory.Slice(0, 2).Span, IPAddress.HostToNetworkOrder((short)request.TransactionId));
                memory.Span[2] = 0;
                memory.Span[3] = 0;
                NetCoreBitConverter.TryWriteBytes(memory.Slice(4, 2).Span, IPAddress.HostToNetworkOrder((short)(request.ByteSize + 1)));
                memory.Span[6] = request.SlaveAddress;

                request.WriteTo(memory.Slice(MbapHeaderSizeOnRequest, request.ByteSize));

                await PipeResource.WriteAsync(memory.Slice(0, MbapHeaderSizeOnRequest + request.ByteSize), token).ConfigureAwait(false);
            }
        }

        protected override async Task<IModbusResponse> ReadResponseAsync<TResponse>(CancellationToken token = default)
        {
            var buffer = await PipeResource.ReadAsync(token).ConfigureAwait(false);

            while (buffer.Length < MbapHeaderSizeOnResponse)
            {
                PipeResource.AdvanceTo(buffer.Start);
                buffer = await PipeResource.ReadAsync(token).ConfigureAwait(false);
            }

            var frameLength = (ushort)IPAddress.HostToNetworkOrder(NetCoreBitConverter.ToInt16(buffer.Slice(4, 2).ToSpan()));

            while (buffer.Length < MbapHeaderSizeOnResponse + frameLength)
            {
                PipeResource.AdvanceTo(buffer.Start);
                buffer = await PipeResource.ReadAsync(token).ConfigureAwait(false);
            }

            var processedSequence = buffer.Slice(0, MbapHeaderSizeOnResponse + frameLength);

            var response = ModbusResponseFactory.CreateResponse<TResponse>(
                processedSequence.Slice(MbapHeaderSizeOnResponse, processedSequence.Length - MbapHeaderSizeOnResponse).ToSpan());

            response.TransactionId = (ushort)IPAddress.NetworkToHostOrder(NetCoreBitConverter.ToInt16(buffer.Slice(0, 2).ToSpan()));

            PipeResource.AdvanceTo(processedSequence.End);

            return response;
        }

        protected override bool RetryReadResponse(IModbusRequest request, IModbusResponse response)
        {
            // Do not retry for these on invalid function code or slave address
            return request.FunctionCode == response.FunctionCode
                && request.SlaveAddress == response.SlaveAddress
                && request.TransactionId > response.TransactionId
                && request.TransactionId - response.TransactionId < RetryOnOldResponseThreshold;
        }

        protected override void Validate(IModbusRequest request, IModbusResponse response)
        {
            request.Validate(response);

            if (request.TransactionId != response.TransactionId)
            {
                throw new IOException($@"Received unexpected transaction Id.
Expected: {request.TransactionId}.
Received: {response.TransactionId}.");
            }
        }
    }
}