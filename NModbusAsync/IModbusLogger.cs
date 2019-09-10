namespace NModbusAsync
{
    public interface IModbusLogger
    {
        void Log(LoggingLevel level, string message);
    }
}