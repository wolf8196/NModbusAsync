namespace NModbusAsync
{
    public interface IModbusMessage
    {
        byte FunctionCode { get; }

        byte SlaveAddress { get; }

        byte[] MessageFrame { get; }

        byte[] ProtocolDataUnit { get; }

        ushort TransactionId { get; set; }

        void Initialize(byte[] frame);
    }
}