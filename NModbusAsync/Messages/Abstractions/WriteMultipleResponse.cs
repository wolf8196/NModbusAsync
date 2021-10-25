using System;
using System.Net;
using NModbusAsync.Utility;

namespace NModbusAsync.Messages.Abstractions
{
    internal class WriteMultipleResponse : WriteResponse
    {
        internal ushort NumberOfPoints { get; private set; }

        public override void Initialize(ReadOnlySpan<byte> frame)
        {
            base.Initialize(frame);

            NumberOfPoints = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame.Slice(4, 2)));
        }
    }
}