using NModbusAsync.Messages.Abstractions;

namespace NModbusAsync.Messages
{
    internal sealed class ReadInputsRequest : ReadDiscretesRequest<ReadInputsResponse>
    {
        internal ReadInputsRequest(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
            : base(slaveAddress, ModbusFunctionCodes.ReadInputs, startAddress, numberOfPoints)
        {
        }

        public override string ToString()
        {
            return $"Read {NumberOfPoints} inputs starting at address {StartAddress} from slave {SlaveAddress}";
        }
    }
}