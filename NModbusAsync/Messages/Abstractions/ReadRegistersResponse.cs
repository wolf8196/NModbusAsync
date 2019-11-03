using System;
using NModbusAsync.Utility;

namespace NModbusAsync.Messages.Abstractions
{
    internal abstract class ReadRegistersResponse : ModbusResponse
    {
        internal ushort[] Data { get; private set; }

        protected override int MinimumFrameSize => 3;

        public override void Initialize(ReadOnlySpan<byte> frame)
        {
            base.Initialize(frame);

            var dataLength = frame[2];

            if (frame.Length < MinimumFrameSize + dataLength)
            {
                throw new FormatException("Message frame does not contain enough bytes.");
            }

            Data = frame.Slice(MinimumFrameSize, dataLength).ToHostUInt16Array();
        }
    }
}