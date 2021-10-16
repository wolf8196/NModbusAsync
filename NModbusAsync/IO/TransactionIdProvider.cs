using System.Threading;
using NModbusAsync.IO.Abstractions;

namespace NModbusAsync.IO
{
    internal sealed class TransactionIdProvider : ITransactionIdProvider
    {
        private int transactionId;

        public TransactionIdProvider()
        {
            transactionId = -1;
        }

        public ushort NewId()
        {
            var newId = Interlocked.Increment(ref transactionId);
            return (ushort)(newId % ushort.MaxValue);
        }
    }
}