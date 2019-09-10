using System;
using System.Linq;
using NModbusAsync.Data;
using NModbusAsync.Utility;

namespace NModbusAsync.Message
{
    internal class ReadHoldingInputRegistersResponse : AbstractModbusMessageWithData<RegisterCollection>, IModbusMessage
    {
        public ReadHoldingInputRegistersResponse()
        {
        }

        internal ReadHoldingInputRegistersResponse(byte functionCode, byte slaveAddress, RegisterCollection data)
            : base(slaveAddress, functionCode)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            ByteCount = data.ByteCount;
            Data = data;
        }

        internal byte ByteCount
        {
            get => MessageImpl.ByteCount.Value;
            private set => MessageImpl.ByteCount = value;
        }

        protected override int MinimumFrameSize => 3;

        public override string ToString()
        {
            string msg = $"Read {Data.Count} {(FunctionCode == ModbusFunctionCodes.ReadHoldingRegisters ? "holding" : "input")} registers.";
            return msg;
        }

        protected override void InitializeUnique(byte[] frame)
        {
            if (frame.Length < MinimumFrameSize + frame[2])
            {
                throw new FormatException("Message frame does not contain enough bytes.");
            }

            ByteCount = frame[2];
            Data = new RegisterCollection(new Span<byte>(frame, 3, ByteCount).ToArray());
        }
    }
}