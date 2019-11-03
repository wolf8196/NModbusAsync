using NModbusAsync.Messages.Abstractions;
using NModbusAsync.Utility;

namespace NModbusAsync.Messages
{
    internal sealed class WriteSingleCoilResponse : WriteSingleResponse
    {
        public override string ToString()
        {
            return $"Wrote single coil {(Value == Constants.CoilOn ? 1 : 0)} at address {StartAddress} into slave {SlaveAddress}.";
        }
    }
}