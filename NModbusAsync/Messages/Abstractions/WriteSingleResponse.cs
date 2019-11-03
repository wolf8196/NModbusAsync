using System;
using System.Net;
using NModbusAsync.Utility;

namespace NModbusAsync.Messages.Abstractions
{
    internal abstract class WriteSingleResponse : WriteResponse
    {
        internal ushort Value { get; private set; }

        public override void Initialize(ReadOnlySpan<byte> frame)
        {
            base.Initialize(frame);

            Value = (ushort)IPAddress.NetworkToHostOrder(NetCoreBitConverter.ToInt16(frame.Slice(4, 2)));
        }
    }
}