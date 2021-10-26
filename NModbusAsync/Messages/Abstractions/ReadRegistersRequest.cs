using System.IO;

namespace NModbusAsync.Messages.Abstractions
{
    internal abstract class ReadRegistersRequest<TResponse> : ReadRequest
        where TResponse : ReadRegistersResponse
    {
        protected ReadRegistersRequest(byte slaveAddress, byte functionCode, ushort startAddress, ushort numberOfPoints)
            : base(slaveAddress, functionCode, startAddress, numberOfPoints)
        {
        }

        public override void Validate(IModbusResponse response)
        {
            base.Validate(response);
            var typedResponse = (TResponse)response;

            if (NumberOfPoints != typedResponse.Data.Length)
            {
                throw new IOException($"Received unexpected number of registers. Expected: {NumberOfPoints}. Received: {typedResponse.Data.Length}.");
            }
        }
    }
}