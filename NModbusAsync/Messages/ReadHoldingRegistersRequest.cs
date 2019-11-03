using NModbusAsync.Messages.Abstractions;

namespace NModbusAsync.Messages
{
    internal sealed class ReadHoldingRegistersRequest : ReadRegistersRequest<ReadHoldingRegistersResponse>
    {
        internal ReadHoldingRegistersRequest(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
            : base(slaveAddress, ModbusFunctionCodes.ReadHoldingRegisters, startAddress, numberOfPoints)
        {
        }

        public override string ToString()
        {
            return $"Read {NumberOfPoints} holding registers starting at address {StartAddress} from slave {SlaveAddress}.";
        }
    }
}