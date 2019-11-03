using NModbusAsync.Messages.Abstractions;
using NModbusAsync.Utility;

namespace NModbusAsync.Messages
{
    internal sealed class WriteSingleCoilRequest : WriteSingleRequest<WriteSingleCoilResponse>
    {
        internal WriteSingleCoilRequest(byte slaveAddress, ushort startAddress, bool coil)
            : base(slaveAddress, ModbusFunctionCodes.WriteSingleCoil, startAddress, coil ? Constants.CoilOn : Constants.CoilOff)
        {
        }

        public override string ToString()
        {
            return $"Write single coil {(Value == Constants.CoilOn ? 1 : 0)} at address {StartAddress} into slave {SlaveAddress}.";
        }
    }
}