namespace NModbusAsync.Message
{
    internal static class ModbusMessageFactory
    {
        internal static T CreateModbusMessage<T>(byte[] frame) where T : IModbusMessage, new()
        {
            var message = new T();
            message.Initialize(frame);
            return message;
        }
    }
}