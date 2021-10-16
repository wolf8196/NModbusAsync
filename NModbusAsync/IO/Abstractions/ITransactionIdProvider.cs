namespace NModbusAsync.IO.Abstractions
{
    internal interface ITransactionIdProvider
    {
        ushort NewId();
    }
}