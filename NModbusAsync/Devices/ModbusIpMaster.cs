namespace NModbusAsync.Devices
{
    internal class ModbusIpMaster : ModbusMaster
    {
        internal ModbusIpMaster(IModbusTransport transport)
            : base(transport)
        {
        }
    }
}