using NModbusAsync.Messages.Abstractions;

namespace NModbusAsync.Messages
{
    internal sealed class WriteMultipleRegistersResponse : WriteMultipleResponse
    {
        public override string ToString()
        {
            return $"Wrote {NumberOfPoints} holding registers starting at address {StartAddress} into slave {SlaveAddress}.";
        }
    }
}