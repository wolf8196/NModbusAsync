namespace NModbusAsync.IO
{
    internal interface ITransactionIdProvider
    {
        ushort NewId();
    }
}