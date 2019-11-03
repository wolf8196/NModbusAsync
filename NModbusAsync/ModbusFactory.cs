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

        public IModbusMaster CreateMaster<TResource>(TResource tcpClient) where TResource : TcpClient
        {
            return new ModbusMaster(
                new ModbusIpTransport(
                    new PipeAdapter<TResource>(
                        new TcpClientAdapter<TResource>(tcpClient)),
                    logger,
                    new TransactionIdProvider()));
        }
    }
}