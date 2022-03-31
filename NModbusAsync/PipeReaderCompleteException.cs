using System;

namespace NModbusAsync
{
    public class PipeReaderCompleteException : Exception
    {
        public PipeReaderCompleteException()
            : base("Pipe was completed before enough data was read.")
        {
        }
    }
}