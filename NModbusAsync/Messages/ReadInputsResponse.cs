using NModbusAsync.Messages.Abstractions;

namespace NModbusAsync.Messages
{
    internal sealed class ReadInputsResponse : ReadDiscretesResponse
    {
        public override string ToString()
        {
            return $"Read {Data.Length} inputs from slave {SlaveAddress}.";
        }
    }
}