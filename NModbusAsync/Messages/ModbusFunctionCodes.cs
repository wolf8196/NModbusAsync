namespace NModbusAsync.Messages
{
    internal static class ModbusFunctionCodes
    {
        internal const byte ReadCoils = 1;
        internal const byte ReadInputs = 2;
        internal const byte ReadHoldingRegisters = 3;
        internal const byte ReadInputRegisters = 4;
        internal const byte WriteSingleCoil = 5;
        internal const byte WriteSingleRegister = 6;
        internal const byte WriteMultipleCoils = 15;
        internal const byte WriteMultipleRegisters = 16;
    }
}