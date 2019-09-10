namespace NModbusAsync.Data
{
    internal interface IModbusMessageDataCollection
    {
        byte[] NetworkBytes { get; }
    }
}