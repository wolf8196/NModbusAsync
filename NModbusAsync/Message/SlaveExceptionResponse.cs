using System;
using System.Collections.Generic;
using System.Globalization;

namespace NModbusAsync.Message
{
    internal class SlaveExceptionResponse : AbstractModbusMessage, IModbusMessage
    {
        private static readonly Dictionary<byte, string> ExceptionMessages = CreateExceptionMessages();

        public SlaveExceptionResponse()
        {
        }

        internal SlaveExceptionResponse(byte slaveAddress, byte functionCode, byte exceptionCode)
            : base(slaveAddress, functionCode)
        {
            SlaveExceptionCode = exceptionCode;
        }

        internal byte SlaveExceptionCode
        {
            get => MessageImpl.ExceptionCode.Value;
            private set => MessageImpl.ExceptionCode = value;
        }

        protected override int MinimumFrameSize => 3;

        public override string ToString()
        {
            string msg = ExceptionMessages.ContainsKey(SlaveExceptionCode)
                ? ExceptionMessages[SlaveExceptionCode]
                : Constants.Unknown;

            return string.Format(
                CultureInfo.InvariantCulture,
                Constants.SlaveExceptionResponseFormat,
                Environment.NewLine,
                FunctionCode,
                SlaveExceptionCode,
                msg);
        }

        protected override void InitializeUnique(byte[] frame)
        {
            if (FunctionCode <= Constants.ExceptionOffset)
            {
                throw new FormatException(Constants.SlaveExceptionResponseInvalidFunctionCode);
            }

            SlaveExceptionCode = frame[2];
        }

        private static Dictionary<byte, string> CreateExceptionMessages()
        {
            return new Dictionary<byte, string>(9)
            {
                { 1, Constants.IllegalFunction },
                { 2, Constants.IllegalDataAddress },
                { 3, Constants.IllegalDataValue },
                { 4, Constants.SlaveDeviceFailure },
                { 5, Constants.Acknowledge },
                { 6, Constants.SlaveDeviceBusy },
                { 8, Constants.MemoryParityError },
                { 10, Constants.GatewayPathUnavailable },
                { 11, Constants.GatewayTargetDeviceFailedToRespond }
            };
        }
    }
}