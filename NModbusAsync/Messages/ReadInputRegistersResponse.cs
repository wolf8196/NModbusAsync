using NModbusAsync.Messages.Abstractions;

namespace NModbusAsync.Messages
{
    internal sealed class ReadInputRegistersResponse : ReadRegistersResponse
    {
        public override string ToString()
        {
            return $"Read {Data.Length} input registers from slave {SlaveAddress}.";
        }
    }
}