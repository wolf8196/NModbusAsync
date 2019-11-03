using System;

namespace NModbusAsync
{
    public interface IModbusRequest : IModbusMessage
    {
        ushort ByteSize { get; }

        void WriteTo(Memory<byte> buffer);

        void Validate(IModbusResponse response);
    }
}