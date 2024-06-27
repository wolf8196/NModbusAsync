using System;
using System.Net;

namespace NModbusAsync.Messages.Abstractions
{
    internal abstract class WriteSingleResponse : WriteResponse
    {
        internal ushort Value { get; private set; }

        public override void Initialize(ReadOnlySpan<byte> frame)
        {
            base.Initialize(frame);

            Value = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame.Slice(4, 2)));
        }
    }
}