using NModbusAsync.Messages.Abstractions;

namespace NModbusAsync.Messages
{
    internal sealed class WriteSingleRegisterRequest : WriteSingleRequest<WriteSingleRegisterResponse>
    {
        internal WriteSingleRegisterRequest(byte slaveAddress, ushort startAddress, ushort register)
            : base(slaveAddress, ModbusFunctionCodes.WriteSingleRegister, startAddress, register)
        {
        }

        public override string ToString()
        {
            return $"Write single holding register {Value} at address {StartAddress} into slave {SlaveAddress}.";
        }
    }
}