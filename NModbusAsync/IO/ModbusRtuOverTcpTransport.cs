using System;
using System.Threading;
using System.Threading.Tasks;

namespace NModbusAsync.IO
{
    internal sealed class ModbusRtuOverTcpTransport : ModbusTransport
    {
        public ModbusRtuOverTcpTransport(IPipeResource pipeResource, IModbusLogger logger, ITransactionIdProvider transactionIdProvider)
            : base(pipeResource, transactionIdProvider, logger)
        {
        }

        protected override Task<IModbusResponse> ReadResponseAsync<TResponse>(CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        protected override bool RetryReadResponse(IModbusRequest request, IModbusResponse response)
        {
            throw new NotImplementedException();
        }

        protected override void Validate(IModbusRequest request, IModbusResponse response)
        {
            throw new NotImplementedException();
        }

        protected override Task WriteRequestAsync(IModbusRequest request, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }
}