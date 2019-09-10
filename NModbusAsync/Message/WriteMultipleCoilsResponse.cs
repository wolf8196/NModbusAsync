using System;
using System.Net;

namespace NModbusAsync.Message
{
    internal class WriteMultipleCoilsResponse : AbstractModbusMessage, IModbusMessage
    {
        public WriteMultipleCoilsResponse()
        {
        }

        internal WriteMultipleCoilsResponse(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
            : base(slaveAddress, ModbusFunctionCodes.WriteMultipleCoils)
        {
            StartAddress = startAddress;
            NumberOfPoints = numberOfPoints;
        }

        internal ushort NumberOfPoints
        {
            get => MessageImpl.NumberOfPoints.Value;

            private set
            {
                if (value > Constants.MaximumDiscreteRequestResponseSize)
                {
                    string msg = $"Maximum amount of data {Constants.MaximumDiscreteRequestResponseSize} coils.";
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
            string msg = $"Wrote {NumberOfPoints} coils starting at address {StartAddress}.";
            return msg;
        }

        protected override void InitializeUnique(byte[] frame)
        {
            StartAddress = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 2));
            NumberOfPoints = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 4));
        }
    }
}