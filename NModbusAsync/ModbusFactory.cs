using System.Net.Sockets;
using NModbusAsync.Devices;
using NModbusAsync.IO;
using NModbusAsync.Logging;

namespace NModbusAsync
{
    public class ModbusFactory : IModbusFactory
    {
        private readonly IModbusLogger logger;

        public ModbusFactory()
        {
            logger = NullModbusLogger.Instance;
        }

        public ModbusFactory(IModbusLogger logger)
        {
            this.logger = logger;
        }

        public IModbusMaster CreateTcpMaster<TResource>(TResource tcpClient) where TResource : TcpClient
        {
            return CreateTcpMaster(tcpClient, logger);
        }

        public IModbusMaster CreateTcpMaster<TResource>(TResource tcpClient, IModbusLogger logger) where TResource : TcpClient
        {
            return new ModbusMaster(
                new ModbusTcpTransport(
                    new PipeAdapter<TResource>(
                        new TcpClientAdapter<TResource>(tcpClient)),
                    logger,
                    new TransactionIdProvider()));
        }

        public IModbusMaster CreateRtuOverTcpMaster<TResource>(TResource tcpClient) where TResource : TcpClient
        {
            return CreateRtuOverTcpMaster(tcpClient, logger);
        }

        public IModbusMaster CreateRtuOverTcpMaster<TResource>(TResource tcpClient, IModbusLogger logger) where TResource : TcpClient
        {
            return new ModbusMaster(
                new ModbusRtuOverTcpTransport(
                    new PipeAdapter<TResource>(
                        new TcpClientAdapter<TResource>(tcpClient)),
                    logger,
                    new TransactionIdProvider()));
        }
    }
}