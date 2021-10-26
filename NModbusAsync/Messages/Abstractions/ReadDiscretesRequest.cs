using System.IO;

namespace NModbusAsync.Messages.Abstractions
{
    internal abstract class ReadDiscretesRequest<TResponse> : ReadRequest
        where TResponse : ReadDiscretesResponse
    {
        protected ReadDiscretesRequest(byte slaveAddress, byte functionCode, ushort startAddress, ushort numberOfPoints)
            : base(slaveAddress, functionCode, startAddress, numberOfPoints)
        {
        }

        public override void Validate(IModbusResponse response)
        {
            base.Validate(response);
            var typedResponse = (TResponse)response;

            if (NumberOfPoints > typedResponse.Data.Length)
            {
                throw new IOException($"Received less discretes than expected. Expected: {NumberOfPoints}. Received: {typedResponse.Data.Length}.");
            }
        }
    }
}