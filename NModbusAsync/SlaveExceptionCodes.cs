namespace NModbusAsync
{
    public static class SlaveExceptionCodes
    {
        public const byte IllegalFunction = 1;

        public const byte IllegalDataAddress = 2;

        public const byte IllegalDataValue = 3;

        public const byte SlaveDeviceFailure = 4;

        public const byte Acknowledge = 5;

        public const byte SlaveDeviceBusy = 6;
    }
}