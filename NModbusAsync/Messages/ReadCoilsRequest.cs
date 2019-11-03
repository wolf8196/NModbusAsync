using NModbusAsync.Messages.Abstractions;

namespace NModbusAsync.Messages
{
    internal sealed class ReadCoilsRequest : ReadDiscretesRequest<ReadCoilsResponse>
    {
        internal ReadCoilsRequest(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
            : base(slaveAddress, ModbusFunctionCodes.ReadCoils, startAddress, numberOfPoints)
        {
        }

        public override string ToString()
        {
            return $"Read {NumberOfPoints} coils starting at address {StartAddress} from slave {SlaveAddress}.";
        }
    }
}