namespace NModbusAsync
{
    public interface IModbusLogger
    {
        void Log(LogLevel level, string message);
    }
}