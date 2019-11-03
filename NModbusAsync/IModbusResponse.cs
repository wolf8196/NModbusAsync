using System;

namespace NModbusAsync
{
    public interface IModbusResponse : IModbusMessage
    {
        void Initialize(ReadOnlySpan<byte> frame);
    }
}