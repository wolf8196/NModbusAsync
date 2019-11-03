using System;
using System.Net;
using NModbusAsync.Messages.Abstractions;
using NModbusAsync.Utility;

namespace NModbusAsync.Messages
{
    internal sealed class WriteMultipleRegistersRequest : WriteMultipleRequest<WriteMultipleRegistersResponse>
    {
        private readonly ushort[] data;

        internal WriteMultipleRegistersRequest(byte slaveAddress, ushort startAddress, ushort[] data)
            : base(slaveAddress, ModbusFunctionCodes.WriteMultipleRegisters, startAddress, (ushort)data.Length)
        {
            this.data = data;
            ByteSize = (ushort)(6 + (this.data.Length * 2));
        }

        public override ushort ByteSize { get; }

        public override string ToString()
        {
            return $"Write {NumberOfPoints} holding registers starting at address {StartAddress} into slave {SlaveAddress}.";
        }

        public override void WriteTo(Memory<byte> buffer)
        {
            base.WriteTo(buffer);

            var bufferSpan = buffer.Span;

            for (int i = 0; i < data.Length; i++)
            {
                NetCoreBitConverter.TryWriteBytes(bufferSpan.Slice((i * 2) + 6, 2), IPAddress.HostToNetworkOrder((short)data[i]));
            }
        }
    }
}