using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NModbusAsync.IO.Abstractions;
using NModbusAsync.Utility;

namespace NModbusAsync.IO
{
    internal sealed class PipeAdapter<TResource> : IPipeResource<TResource>
    {
        private const string TimeoutOutOfRangeExceptionMessage = "Timeout can be only be set to 'System.Threading.Timeout.Infinite' or a value > 0.";

        private readonly IStreamResource<TResource> streamResource;
        private readonly PipeWriter pipeWriter;
        private readonly PipeReader pipeReader;
        private int readTimeout;
        private int writeTimeout;

        internal PipeAdapter(IStreamResource<TResource> streamResource)
        {
            this.streamResource = streamResource;

            pipeWriter = PipeWriter.Create(streamResource.GetStream());
            pipeReader = PipeReader.Create(streamResource.GetStream());
            readTimeout = Timeout.Infinite;
            writeTimeout = Timeout.Infinite;
        }

        public TResource Resource => streamResource.Resource;

        public int ReadTimeout
        {
            get
            {
                return readTimeout;
            }

            set
            {
                ValidateTimeout(value);
                readTimeout = value;
            }
        }

        public int WriteTimeout
        {
            get
            {
                return writeTimeout;
            }

            set
            {
                ValidateTimeout(value);
                writeTimeout = value;
            }
        }

        public async Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken token)
        {
            var writeTask = pipeWriter.WriteAsync(buffer, token);

            if (!writeTask.IsCompleted && !await writeTask.AsTask().WaitAsync(WriteTimeout, token).ConfigureAwait(false))
            {
                throw new TimeoutException("Failed to write to the transport connection in specified period of time.");
            }
        }

        public async Task<ReadOnlySequence<byte>> ReadAsync(int count, CancellationToken token)
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();

                var readResult = await ReadAsync(token).ConfigureAwait(false);

                if (readResult.Buffer.Length < count)
                {
                    if (readResult.IsCompleted)
                    {
                        throw new PipeReaderCompleteException();
                    }

                    MarkExamined(readResult.Buffer);
                    continue;
                }

                return readResult.Buffer;
            }
        }

        public void MarkConsumed(ReadOnlySequence<byte> buffer)
        {
            pipeReader.AdvanceTo(buffer.End, buffer.End);
        }

        public void MarkExamined(ReadOnlySequence<byte> buffer)
        {
            pipeReader.AdvanceTo(buffer.Start, buffer.End);
        }

        public void Dispose()
        {
            streamResource.Dispose();
            pipeWriter.Complete(new Exception());
            pipeReader.Complete(new Exception());
        }

        private static void ValidateTimeout(int value)
        {
            if (value <= 0 && value != Timeout.Infinite)
            {
                throw new ArgumentOutOfRangeException(nameof(value), TimeoutOutOfRangeExceptionMessage);
            }
        }

        private async Task<ReadResult> ReadAsync(CancellationToken token)
        {
            var readTask = pipeReader.ReadAsync(token);

            if (!readTask.IsCompleted && !await readTask.AsTask().WaitAsync(ReadTimeout, token).ConfigureAwait(false))
            {
                throw new TimeoutException("Failed to read from the transport connection in specified period of time.");
            }

            return readTask.Result;
        }
    }
}