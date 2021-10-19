using NModbusAsync.Messages.Abstractions;

namespace NModbusAsync.Messages
{
    internal sealed class ReadInputRegistersRequest : ReadRegistersRequest<ReadInputRegistersResponse>
    {
        internal ReadInputRegistersRequest(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
            : base(slaveAddress, ModbusFunctionCodes.ReadInputRegisters, startAddress, numberOfPoints)
        {
        }

        public override string ToString()
        {
            return $"Read {NumberOfPoints} input registers starting at address {StartAddress} from slave {SlaveAddress}";
        }
    }
}