namespace NModbusAsync
{
    public enum SlaveExceptionCode : byte
    {
        IllegalFunction = 1,
        IllegalDataAddress = 2,
        IllegalDataValue = 3,
        SlaveDeviceFailure = 4,
        Acknowledge = 5,
        SlaveDeviceBusy = 6,
        MemoryParityError = 8,
        GatewayPathUnavailable = 10,
        GatewayTargetDeviceFailedToRespond = 11
    }
}