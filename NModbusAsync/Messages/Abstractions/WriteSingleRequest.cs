using System;
using System.IO;
using System.Net;
using NModbusAsync.Utility;

namespace NModbusAsync.Messages.Abstractions
{
    internal abstract class WriteSingleRequest<TResponse> : WriteRequest<TResponse>
        where TResponse : WriteSingleResponse
    {
        protected WriteSingleRequest(byte slaveAddress, byte functionCode, ushort startAddress, ushort value)
            : base(slaveAddress, functionCode, startAddress)
        {
            Value = value;
        }

        public override ushort ByteSize => 5;

        internal ushort Value { get; }

        public override void Validate(IModbusResponse response)
        {
            base.Validate(response);
            var typedResponse = (TResponse)response;

            if (Value != typedResponse.Value)
            {
                throw new IOException($"Received unexpected value. Expected: {Value}. Received: {typedResponse.Value}.");
            }
        }

        public override void WriteTo(Memory<byte> buffer)
        {
            base.WriteTo(buffer);

            var bufferSpan = buffer.Span;

            BitConverter.TryWriteBytes(bufferSpan.Slice(1, 2), IPAddress.HostToNetworkOrder((short)StartAddress));
            BitConverter.TryWriteBytes(bufferSpan.Slice(3, 2), IPAddress.HostToNetworkOrder((short)Value));
        }
    }
}