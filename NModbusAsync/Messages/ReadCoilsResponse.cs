using NModbusAsync.Messages.Abstractions;

namespace NModbusAsync.Messages
{
    internal sealed class ReadCoilsResponse : ReadDiscretesResponse
    {
        public override string ToString()
        {
            return $"Read {Data.Length} coils from slave {SlaveAddress}.";
        }
    }
}