using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using NModbusAsync.IO.Abstractions;
using NModbusAsync.Utility;

namespace NModbusAsync.IO
{
    internal sealed class PipeAdapter<TResource> : IPipeResource<TResource>
    {
        private const string TimeoutOutOfRangeExceptionMessage = "Timeout can be only be set to 'System.Threading.Timeout.Infinite' or a value > 0";

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
                if (value <= 0 && value != Timeout.Infinite)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), TimeoutOutOfRangeExceptionMessage);
                }

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
                if (value <= 0 && value != Timeout.Infinite)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), TimeoutOutOfRangeExceptionMessage);
                }

                writeTimeout = value;
            }
        }

        public async Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken token)
        {
            var writeTask = pipeWriter.WriteAsync(buffer, token);

            if (!await writeTask.AsTask().WaitAsync(WriteTimeout, token).ConfigureAwait(false))
            {
                throw new TimeoutException("Failed to write to the transport connection in specified period of time.");
            }
        }

        public async Task<ReadOnlySequence<byte>> ReadAsync(CancellationToken token)
        {
            var readTask = pipeReader.ReadAsync(token);

            if (!await readTask.AsTask().WaitAsync(ReadTimeout, token).ConfigureAwait(false))
            {
                throw new TimeoutException("Failed to read from the transport connection in specified period of time.");
            }

            return readTask.Result.Buffer;
        }

        public void AdvanceTo(SequencePosition consumed)
        {
            pipeReader.AdvanceTo(consumed);
        }

        public void Dispose()
        {
            streamResource.Dispose();
            pipeWriter.Complete(new Exception());
            pipeReader.Complete(new Exception());
        }
    }
}