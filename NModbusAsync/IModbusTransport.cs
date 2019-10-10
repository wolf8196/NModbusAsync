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

        IStreamResource StreamResource { get; }

        Task<T> UnicastMessageAsync<T>(IModbusMessage message, CancellationToken token = default) where T : IModbusMessage, new();

        Task<byte[]> ReadRequestAsync(CancellationToken token = default);

        byte[] BuildMessageFrame(IModbusMessage message);

        Task WriteAsync(IModbusMessage message, CancellationToken token = default);
    }
}