using System;
using System.Net;

namespace NModbusAsync.Message
{
    internal class WriteMultipleRegistersResponse : AbstractModbusMessage, IModbusMessage
    {
        public WriteMultipleRegistersResponse()
        {
        }

        internal WriteMultipleRegistersResponse(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
            : base(slaveAddress, ModbusFunctionCodes.WriteMultipleRegisters)
        {
            StartAddress = startAddress;
            NumberOfPoints = numberOfPoints;
        }

        internal ushort NumberOfPoints
        {
            get => MessageImpl.NumberOfPoints.Value;

            private set
            {
                if (value > Constants.MaximumRegisterRequestResponseSize)
                {
                    string msg = $"Maximum amount of data {Constants.MaximumRegisterRequestResponseSize} registers.";
                    throw new ArgumentOutOfRangeException(nameof(NumberOfPoints), msg);
                }

                MessageImpl.NumberOfPoints = value;
            }
        }

        internal ushort StartAddress
        {
            get => MessageImpl.StartAddress.Value;
            private set => MessageImpl.StartAddress = value;
        }

        protected override int MinimumFrameSize => 6;

        public override string ToString()
        {
            string msg = $"Wrote {NumberOfPoints} holding registers starting at address {StartAddress}.";
            return msg;
        }

        protected override void InitializeUnique(byte[] frame)
        {
            StartAddress = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 2));
            NumberOfPoints = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 4));
        }
    }
}