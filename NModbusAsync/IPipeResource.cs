using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace NModbusAsync
{
    public interface IPipeResource : IDisposable
    {
        int ReadTimeout { get; set; }

        int WriteTimeout { get; set; }

        Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken token);

        Task<ReadOnlySequence<byte>> ReadAsync(CancellationToken token);

        Task<ReadOnlySequence<byte>> ReadAsync(int count, CancellationToken token);

        void MarkConsumed(ReadOnlySequence<byte> buffer);

        void MarkExamined(ReadOnlySequence<byte> buffer);
    }
}