using System;
using NModbusAsync.Message;

namespace NModbusAsync
{
    public class SlaveException : Exception
    {
        private readonly SlaveExceptionResponse slaveExceptionResponse;

        internal SlaveException(SlaveExceptionResponse slaveExceptionResponse)
        {
            this.slaveExceptionResponse = slaveExceptionResponse;
        }

        public override string Message
        {
            get
            {
                var responseString = slaveExceptionResponse != null ? string.Concat(Environment.NewLine, slaveExceptionResponse) : string.Empty;
                return string.Concat(base.Message, responseString);
            }
        }

        public byte FunctionCode => slaveExceptionResponse?.FunctionCode ?? default;

        public byte SlaveExceptionCode => slaveExceptionResponse?.SlaveExceptionCode ?? default;

        public byte SlaveAddress => slaveExceptionResponse?.SlaveAddress ?? default;
    }
}