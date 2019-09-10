namespace NModbusAsync.Logging
{
    internal class NullModbusLogger : IModbusLogger
    {
        internal static readonly NullModbusLogger Instance = new NullModbusLogger();

        private NullModbusLogger()
        {
        }

        public void Log(LoggingLevel level, string message)
        {
        }
    }
}