namespace NModbusAsync.Utility
{
    internal static class Constants
    {
        internal const byte ExceptionOffset = 128;
        internal const ushort CoilOn = 0xFF00;
        internal const ushort CoilOff = 0x0000;
        internal const int BitsPerByte = 8;
    }
}