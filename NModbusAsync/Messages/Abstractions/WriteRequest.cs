using System.IO;

namespace NModbusAsync.Messages.Abstractions
{
    internal abstract class WriteRequest<TResponse> : ModbusRequest
        where TResponse : WriteResponse
    {
        protected WriteRequest(byte slaveAddress, byte functionCode, ushort startAddress)
            : base(slaveAddress, functionCode)
        {
            StartAddress = startAddress;
        }

        internal ushort StartAddress { get; }

        public override void Validate(IModbusResponse response)
        {
            base.Validate(response);
            var typedResponse = (TResponse)response;

            if (StartAddress != typedResponse.StartAddress)
            {
                throw new IOException($"Received unexpected start address. Expected: {StartAddress}. Received: {typedResponse.StartAddress}.");
            }
        }
    }
}