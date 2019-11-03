using System;
using NModbusAsync.Messages.Abstractions;
using NModbusAsync.Utility;

namespace NModbusAsync.Messages
{
    using static Constants;

    internal sealed class WriteMultipleCoilsRequest : WriteMultipleRequest<WriteMultipleCoilsResponse>
    {
        private const int DataOffset = 6;
        private readonly bool[] data;

        internal WriteMultipleCoilsRequest(byte slaveAddress, ushort startAddress, bool[] data)
            : base(slaveAddress, ModbusFunctionCodes.WriteMultipleCoils, startAddress, (ushort)data.Length)
        {
            this.data = data;
            ByteSize = (ushort)(DataOffset + (byte)((this.data.Length + 7) / BitsPerByte));
        }

        public override ushort ByteSize { get; }

        public override string ToString()
        {
            return $"Write {NumberOfPoints} coils starting at address {StartAddress} into slave {SlaveAddress}.";
        }

        public override void WriteTo(Memory<byte> buffer)
        {
            base.WriteTo(buffer);

            var bufferSpan = buffer.Span;

            // zero out data bytes
            for (int i = DataOffset; i < bufferSpan.Length; i++)
            {
                bufferSpan[i] = 0;
            }

            for (int index = 0; index < data.Length; index++)
            {
                if (data[index])
                {
                    bufferSpan[(index / BitsPerByte) + DataOffset] |= (byte)(1 << (index % BitsPerByte));
                }
            }
        }
    }
}