using System;

namespace NModbusAsync.Messages.Abstractions
{
    internal abstract class ModbusResponse : IModbusResponse
    {
        public ushort TransactionId { get; set; }

        public byte SlaveAddress { get; private set; }

        public byte FunctionCode { get; private set; }

        protected abstract int MinimumFrameSize { get; }

        public virtual void Initialize(ReadOnlySpan<byte> frame)
        {
            if (frame.Length < MinimumFrameSize)
            {
                throw new FormatException($"Frame length must contain at least {MinimumFrameSize} bytes of data.");
            }

            SlaveAddress = frame[0];
            FunctionCode = frame[1];
        }
    }
}