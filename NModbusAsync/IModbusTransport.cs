using System;
using System.Threading;
using System.Threading.Tasks;

namespace NModbusAsync
{
    public interface IModbusTransport : IDisposable
    {
        int Retries { get; set; }

        uint RetryOnOldResponseThreshold { get; set; }

        bool SlaveBusyUsesRetryCount { get; set; }

        int WaitToRetryMilliseconds { get; set; }

        int ReadTimeout { get; set; }

        int WriteTimeout { get; set; }

        IPipeResource PipeResource { get; }

        Task<TResponse> SendAsync<TResponse>(IModbusRequest request, CancellationToken token = default) where TResponse : IModbusResponse, new();
    }
}