namespace NModbusAsync
{
    public interface IModbusMessage
    {
        ushort TransactionId { get; set; }

        byte FunctionCode { get; }

        byte SlaveAddress { get; }
    }
}