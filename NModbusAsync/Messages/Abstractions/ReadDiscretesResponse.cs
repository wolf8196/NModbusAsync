using System;
using System.Buffers;
using NModbusAsync.Utility;

namespace NModbusAsync.Messages.Abstractions
{
    using static Constants;

    internal abstract class ReadDiscretesResponse : ModbusResponse, IDisposable
    {
        private IMemoryOwner<bool> memoryOwner;

        internal Memory<bool> Data { get; private set; }

        protected override int MinimumFrameSize => 3;

        public override void Initialize(ReadOnlySpan<byte> frame)
        {
            base.Initialize(frame);

            var dataLength = frame[2];

            if (frame.Length < MinimumFrameSize + dataLength)
            {
                throw new FormatException("Message frame does not contain enough bytes.");
            }

            var dataFrame = frame.Slice(MinimumFrameSize, dataLength);

            memoryOwner = MemoryPool<bool>.Shared.Rent(dataLength * BitsPerByte);
            var discretes = memoryOwner.Memory.Span;

            for (int i = 0; i < dataFrame.Length; i++)
            {
                var b = dataFrame[i];

                discretes[(i * BitsPerByte) + 0] = (b & 1) == 1;
                discretes[(i * BitsPerByte) + 1] = (b & 2) == 2;
                discretes[(i * BitsPerByte) + 2] = (b & 4) == 4;
                discretes[(i * BitsPerByte) + 3] = (b & 8) == 8;
                discretes[(i * BitsPerByte) + 4] = (b & 16) == 16;
                discretes[(i * BitsPerByte) + 5] = (b & 32) == 32;
                discretes[(i * BitsPerByte) + 6] = (b & 64) == 64;
                discretes[(i * BitsPerByte) + 7] = (b & 128) == 128;
            }

            Data = memoryOwner.Memory.Slice(0, dataLength * BitsPerByte);
        }

        public void Dispose()
        {
            memoryOwner?.Dispose();
        }
    }
}