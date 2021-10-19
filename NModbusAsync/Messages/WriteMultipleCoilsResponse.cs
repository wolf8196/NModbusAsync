using NModbusAsync.Messages.Abstractions;

namespace NModbusAsync.Messages
{
    internal sealed class WriteMultipleCoilsResponse : WriteMultipleResponse
    {
        public override string ToString()
        {
            return $"Wrote {NumberOfPoints} coils starting at address {StartAddress} into slave {SlaveAddress}";
        }
    }
}