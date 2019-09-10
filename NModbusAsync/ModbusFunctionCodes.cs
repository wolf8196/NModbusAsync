namespace NModbusAsync
{
    public static class ModbusFunctionCodes
    {
        public const byte ReadCoils = 1;
        public const byte ReadInputs = 2;
        public const byte ReadHoldingRegisters = 3;
        public const byte ReadInputRegisters = 4;
        public const byte WriteSingleCoil = 5;
        public const byte WriteSingleRegister = 6;
        public const byte WriteMultipleCoils = 15;
        public const byte WriteMultipleRegisters = 16;
    }
}