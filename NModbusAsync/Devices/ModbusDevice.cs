using System;

namespace NModbusAsync.Devices
{
    internal abstract class ModbusDevice : IDisposable
    {
        protected ModbusDevice(IModbusTransport transport)
        {
            Transport = transport;
        }

        public IModbusTransport Transport { get; }

        public void Dispose()
        {
            Transport.Dispose();
        }
    }
}