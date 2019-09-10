namespace NModbusAsync.Message
{
    internal interface IModbusRequest : IModbusMessage
    {
        void ValidateResponse(IModbusMessage response);
    }
}