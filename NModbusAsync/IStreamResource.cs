using System;
using System.Threading;
using System.Threading.Tasks;

namespace NModbusAsync
{
    public interface IStreamResource : IDisposable
    {
        int ReadTimeout { get; set; }

        int WriteTimeout { get; set; }

        Task FlushAsync(CancellationToken token = default);

        Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken token = default);

        Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken token = default);
    }
}