using System;
using System.IO;
using System.Net;
using NModbusAsync.Utility;

namespace NModbusAsync.Messages.Abstractions
{
    internal abstract class WriteMultipleRequest<TResponse> : WriteRequest<TResponse> where TResponse : WriteMultipleResponse
    {
        protected WriteMultipleRequest(byte slaveAddress, byte functionCode, ushort startAddress, ushort numberOfPoints)
            : base(slaveAddress, functionCode, startAddress)
        {
            NumberOfPoints = numberOfPoints;
        }

        internal ushort NumberOfPoints { get; }

        public override void Validate(IModbusResponse response)
        {
            base.Validate(response);
            var typedResponse = (TResponse)response;

            if (NumberOfPoints != typedResponse.NumberOfPoints)
            {
                throw new IOException($"Received unexpected number of points. Expected: {NumberOfPoints}. Received: {typedResponse.NumberOfPoints}.");
            }
        }

        public override void WriteTo(Memory<byte> buffer)
        {
            base.WriteTo(buffer);

            var bufferSpan = buffer.Span;

            NetCoreBitConverter.TryWriteBytes(bufferSpan.Slice(1, 2), IPAddress.HostToNetworkOrder((short)StartAddress));
            NetCoreBitConverter.TryWriteBytes(bufferSpan.Slice(3, 2), IPAddress.HostToNetworkOrder((short)NumberOfPoints));
            bufferSpan[5] = (byte)(ByteSize - 6);
        }
    }
}