using System.Net.Sockets;
using NModbusAsync.Devices;
using NModbusAsync.IO;
using NModbusAsync.Logging;

namespace NModbusAsync
{
    public class ModbusFactory : IModbusFactory
    {
        private readonly IModbusLogger logger;

        public ModbusFactory(IModbusLogger logger = null)
        {
            this.logger = logger ?? NullModbusLogger.Instance;
        }

        public IModbusMaster CreateMaster<T>(T tcpClient) where T : TcpClient
        {
            var adapter = new TcpClientAdapter<T>(tcpClient);

            var transport = new ModbusIpTransport(adapter, logger);

            return new ModbusIpMaster(transport);
        }
    }
}