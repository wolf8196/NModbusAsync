using System;
using System.IO;

namespace NModbusAsync.Messages.Abstractions
{
    internal abstract class ModbusRequest : IModbusRequest
    {
        protected ModbusRequest(byte slaveAddress, byte functionCode)
        {
            SlaveAddress = slaveAddress;
            FunctionCode = functionCode;
        }

        public ushort TransactionId { get; set; }

        public byte SlaveAddress { get; }

        public byte FunctionCode { get; }

        public abstract ushort ByteSize { get; }

        public virtual void Validate(IModbusResponse response)
        {
            if (FunctionCode != response.FunctionCode)
            {
                throw new IOException($@"Received unexpected function code.
Expected {FunctionCode}.
Received {response.FunctionCode}.");
            }

            if (SlaveAddress != response.SlaveAddress)
            {
                throw new IOException($@"Received unexpected slave address.
Expected {SlaveAddress}.
Received {response.SlaveAddress}.");
            }
        }

        public virtual void WriteTo(Memory<byte> buffer)
        {
            if (buffer.Length < ByteSize)
            {
                throw new ArgumentOutOfRangeException(nameof(buffer), "The length of the buffer is less than the required byte size");
            }

            var bufferSpan = buffer.Span;
            bufferSpan[0] = FunctionCode;
        }
    }
}