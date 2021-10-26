using System.Threading;
using System.Threading.Tasks;

namespace NModbusAsync.Test.Helpers
{
    internal interface IModbusTransportMock
    {
        Task WriteRequestAsync(IModbusRequest request, CancellationToken token = default);

        Task<IModbusResponse> ReadResponseAsync<TResponse>(CancellationToken token = default)
            where TResponse : IModbusResponse, new();

        bool RetryReadResponse(IModbusRequest request, IModbusResponse response);

        void Validate(IModbusRequest request, IModbusResponse response);
    }
}