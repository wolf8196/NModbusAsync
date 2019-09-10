using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace NModbusAsync.IO
{
    internal sealed class ModbusIpTransport : ModbusTransport
    {
        private static readonly object TransactionIdLock = new object();
        private static ushort transactionId;

        internal ModbusIpTransport(IStreamResource streamResource, IModbusLogger logger)
            : base(streamResource, logger)
        {
            if (streamResource == null)
            {
                throw new ArgumentNullException(nameof(streamResource));
            }
        }

        public override byte[] BuildMessageFrame(IModbusMessage message)
        {
            var header = GetMbapHeader(message);
            var pdu = message.ProtocolDataUnit;
            var messageBody = new byte[header.Length + pdu.Length];
            Buffer.BlockCopy(header, 0, messageBody, 0, header.Length);
            Buffer.BlockCopy(pdu, 0, messageBody, header.Length, pdu.Length);
            return messageBody;
        }

        public override Task WriteAsync(IModbusMessage message)
        {
            return WriteAsync(message, default);
        }

        public override Task WriteAsync(IModbusMessage message, CancellationToken token)
        {
            message.TransactionId = GetNewTransactionId();
            byte[] frame = BuildMessageFrame(message);

            Logger.Log(LoggingLevel.Trace, $"TX: {string.Join(", ", frame)}");

            return StreamResource.WriteAsync(frame, 0, frame.Length, token);
        }

        public override Task<byte[]> ReadRequestAsync()
        {
            return ReadRequestAsync(default);
        }

        public override Task<byte[]> ReadRequestAsync(CancellationToken token)
        {
            return ReadRequestResponseAsync(StreamResource, Logger, token);
        }

        protected override Task<IModbusMessage> ReadResponseAsync<T>()
        {
            return ReadResponseAsync<T>(default);
        }

        protected override async Task<IModbusMessage> ReadResponseAsync<T>(CancellationToken token)
        {
            return CreateMessageAndInitializeTransactionId<T>(
                await ReadRequestResponseAsync(StreamResource, Logger, token).ConfigureAwait(false));
        }

        protected override bool OnShouldRetryResponse(IModbusMessage request, IModbusMessage response)
        {
            if (request.TransactionId > response.TransactionId && request.TransactionId - response.TransactionId < RetryOnOldResponseThreshold)
            {
                // This response was from a previous request
                return true;
            }

            return base.OnShouldRetryResponse(request, response);
        }

        protected override void OnValidateResponse(IModbusMessage request, IModbusMessage response)
        {
            if (request.TransactionId != response.TransactionId)
            {
                throw new IOException($"Response was not of expected transaction ID. Expected {request.TransactionId}, received {response.TransactionId}.");
            }
        }

        private static async Task<byte[]> ReadRequestResponseAsync(IStreamResource streamResource, IModbusLogger logger, CancellationToken token)
        {
            if (streamResource == null)
            {
                throw new ArgumentNullException(nameof(streamResource));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            // read header
            var mbapHeader = new byte[6];
            int numBytesRead = 0;

            while (numBytesRead != 6)
            {
                int read = await streamResource.ReadAsync(mbapHeader, numBytesRead, 6 - numBytesRead, token).ConfigureAwait(false);

                if (read == 0)
                {
                    throw new IOException("Read resulted in 0 bytes returned.");
                }

                numBytesRead += read;
            }

            logger.Log(LoggingLevel.Debug, $"MBAP header: {string.Join(", ", mbapHeader)}");
            var frameLength = (ushort)IPAddress.HostToNetworkOrder(BitConverter.ToInt16(mbapHeader, 4));
            logger.Log(LoggingLevel.Debug, $"{frameLength} bytes in PDU.");

            // read message
            var messageFrame = new byte[frameLength];
            numBytesRead = 0;

            while (numBytesRead != frameLength)
            {
                int read = await streamResource.ReadAsync(messageFrame, numBytesRead, frameLength - numBytesRead, token).ConfigureAwait(false);

                if (read == 0)
                {
                    throw new IOException("Read resulted in 0 bytes returned.");
                }

                numBytesRead += read;
            }

            logger.Log(LoggingLevel.Debug, $"PDU: {frameLength}");

            var frame = new byte[mbapHeader.Length + messageFrame.Length];
            Buffer.BlockCopy(mbapHeader, 0, frame, 0, mbapHeader.Length);
            Buffer.BlockCopy(messageFrame, 0, frame, mbapHeader.Length, messageFrame.Length);

            logger.Log(LoggingLevel.Debug, $"RX: {string.Join(", ", frame)}");

            return frame;
        }

        private static byte[] GetMbapHeader(IModbusMessage message)
        {
            byte[] transactionId = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)message.TransactionId));
            byte[] length = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)(message.ProtocolDataUnit.Length + 1)));

            var mbapHeader = new byte[7];
            Buffer.BlockCopy(transactionId, 0, mbapHeader, 0, 2);

            // leave blanks in mbapHeader for protocol identifier
            Buffer.BlockCopy(length, 0, mbapHeader, 4, 2);
            mbapHeader[6] = message.SlaveAddress;

            return mbapHeader;
        }

        private ushort GetNewTransactionId()
        {
            lock (TransactionIdLock)
            {
                transactionId = transactionId == ushort.MaxValue ? (ushort)1 : ++transactionId;
                return transactionId;
            }
        }

        private IModbusMessage CreateMessageAndInitializeTransactionId<T>(byte[] fullFrame) where T : IModbusMessage, new()
        {
            byte[] mbapHeader = new Span<byte>(fullFrame, 0, 6).ToArray();
            byte[] messageFrame = new Span<byte>(fullFrame, 6, fullFrame.Length - 6).ToArray();

            IModbusMessage response = CreateResponse<T>(messageFrame);
            response.TransactionId = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(mbapHeader, 0));

            return response;
        }
    }
}