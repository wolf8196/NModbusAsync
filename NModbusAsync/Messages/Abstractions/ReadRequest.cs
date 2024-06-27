using System;
using System.Net;

namespace NModbusAsync.Messages.Abstractions
{
    internal abstract class ReadRequest : ModbusRequest
    {
        protected ReadRequest(byte slaveAddress, byte functionCode, ushort startAddress, ushort numberOfPoints)
            : base(slaveAddress, functionCode)
        {
            StartAddress = startAddress;
            NumberOfPoints = numberOfPoints;
        }

        public override ushort ByteSize => 5;

        internal ushort StartAddress { get; }

        internal ushort NumberOfPoints { get; }

        public override void WriteTo(Memory<byte> buffer)
        {
            base.WriteTo(buffer);

            var bufferSpan = buffer.Span;

            BitConverter.TryWriteBytes(bufferSpan.Slice(1, 2), IPAddress.HostToNetworkOrder((short)StartAddress));
            BitConverter.TryWriteBytes(bufferSpan.Slice(3, 2), IPAddress.HostToNetworkOrder((short)NumberOfPoints));
        }
    }
}