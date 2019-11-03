namespace NModbusAsync.Logging
{
    internal sealed class NullModbusLogger : IModbusLogger
    {
        internal static readonly NullModbusLogger Instance = new NullModbusLogger();

        private NullModbusLogger()
        {
        }

        public void Log(LogLevel level, string message)
        {
        }
    }
}