using System;
using System.Net;
using NModbusAsync.Utility;

namespace NModbusAsync.Messages.Abstractions
{
    internal abstract class WriteResponse : ModbusResponse
    {
        internal ushort StartAddress { get; private set; }

        protected override int MinimumFrameSize => 6;

        public override void Initialize(ReadOnlySpan<byte> frame)
        {
            base.Initialize(frame);

            StartAddress = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame.Slice(2, 2)));
        }
    }
}