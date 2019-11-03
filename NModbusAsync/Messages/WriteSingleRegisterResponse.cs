using NModbusAsync.Messages.Abstractions;

namespace NModbusAsync.Messages
{
    internal sealed class WriteSingleRegisterResponse : WriteSingleResponse
    {
        public override string ToString()
        {
            return $"Wrote single holding register {Value} at address {StartAddress} into slave {SlaveAddress}.";
        }
    }
}