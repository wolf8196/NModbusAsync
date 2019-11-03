using NModbusAsync.Messages.Abstractions;

namespace NModbusAsync.Messages
{
    internal sealed class ReadHoldingRegistersResponse : ReadRegistersResponse
    {
        public override string ToString()
        {
            return $"Read {Data.Length} holding registers from slave {SlaveAddress}.";
        }
    }
}