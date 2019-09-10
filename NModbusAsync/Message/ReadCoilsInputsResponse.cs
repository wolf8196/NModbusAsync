using System;
using System.Linq;
using NModbusAsync.Data;
using NModbusAsync.Utility;

namespace NModbusAsync.Message
{
    internal class ReadCoilsInputsResponse : AbstractModbusMessageWithData<DiscreteCollection>, IModbusMessage
    {
        public ReadCoilsInputsResponse()
        {
        }

        internal ReadCoilsInputsResponse(byte functionCode, byte slaveAddress, byte byteCount, DiscreteCollection data)
            : base(slaveAddress, functionCode)
        {
            ByteCount = byteCount;
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
            string msg = $"Read {Data.Count} {(FunctionCode == ModbusFunctionCodes.ReadInputs ? "inputs" : "coils")} - {Data}.";
            return msg;
        }

        protected override void InitializeUnique(byte[] frame)
        {
            if (frame.Length < 3 + frame[2])
            {
                throw new FormatException("Message frame data segment does not contain enough bytes.");
            }

            ByteCount = frame[2];
            Data = new DiscreteCollection(new Span<byte>(frame, 3, ByteCount).ToArray());
        }
    }
}