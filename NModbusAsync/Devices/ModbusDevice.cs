using System;
using NModbusAsync.Utility;

namespace NModbusAsync.Devices
{
    internal abstract class ModbusDevice : IDisposable
    {
        private IModbusTransport transport;

        internal ModbusDevice(IModbusTransport transport)
        {
            this.transport = transport;
        }

        public IModbusTransport Transport => transport;

        public void Dispose()
        {
            DisposableUtility.Dispose(ref transport);
        }
    }
}