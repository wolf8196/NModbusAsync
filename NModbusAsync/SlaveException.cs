using System;
using NModbusAsync.Messages;

namespace NModbusAsync
{
    public class SlaveException : Exception
    {
        internal SlaveException(SlaveExceptionResponse slaveExceptionResponse)
        {
            if (slaveExceptionResponse is null)
            {
                throw new ArgumentNullException(nameof(slaveExceptionResponse));
            }

            FunctionCode = slaveExceptionResponse.FunctionCode;
            SlaveExceptionCode = slaveExceptionResponse.SlaveExceptionCode;
            SlaveAddress = slaveExceptionResponse.SlaveAddress;
            Message = string.Concat(base.Message, Environment.NewLine, slaveExceptionResponse);
        }

        public override string Message { get; }

        public byte FunctionCode { get; }

        public SlaveExceptionCode SlaveExceptionCode { get; }

        public byte SlaveAddress { get; }
    }
}