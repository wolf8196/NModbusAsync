using System;
using System.Threading;
using System.Threading.Tasks;

namespace NModbusAsync
{
    public interface IStreamResource : IDisposable
    {
        int ReadTimeout { get; set; }

        int WriteTimeout { get; set; }

        Task FlushAsync();

        Task FlushAsync(CancellationToken token);

        Task<int> ReadAsync(byte[] buffer, int offset, int count);

        Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken token);

        Task WriteAsync(byte[] buffer, int offset, int count);

        Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken token);
    }
}