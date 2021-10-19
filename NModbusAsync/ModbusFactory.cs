using System;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NModbusAsync.Devices;
using NModbusAsync.IO;

namespace NModbusAsync
{
    public class ModbusFactory : IModbusFactory
    {
        private readonly ILogger<IModbusMaster> logger;

        public ModbusFactory()
        {
            logger = NullLogger<IModbusMaster>.Instance;
        }

        public ModbusFactory(ILogger<IModbusMaster> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IModbusMaster CreateTcpMaster<TResource>(TResource tcpClient) where TResource : TcpClient
        {
            return CreateTcpMaster(tcpClient, logger);
        }

        public IModbusMaster CreateTcpMaster<TResource>(TResource tcpClient, ILogger<IModbusMaster> logger) where TResource : TcpClient
        {
            if (tcpClient == null)
            {
                throw new ArgumentNullException(nameof(tcpClient));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            return new ModbusMaster(
                new ModbusTcpTransport(
                    new PipeAdapter<TResource>(
                        new TcpClientAdapter<TResource>(tcpClient)),
                    new TransactionIdProvider(),
                    logger));
        }

        public IModbusMaster CreateRtuOverTcpMaster<TResource>(TResource tcpClient) where TResource : TcpClient
        {
            return CreateRtuOverTcpMaster(tcpClient, logger);
        }

        public IModbusMaster CreateRtuOverTcpMaster<TResource>(TResource tcpClient, ILogger<IModbusMaster> logger) where TResource : TcpClient
        {
            if (tcpClient == null)
            {
                throw new ArgumentNullException(nameof(tcpClient));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            return new ModbusMaster(
                new ModbusRtuOverTcpTransport(
                    new PipeAdapter<TResource>(
                        new TcpClientAdapter<TResource>(tcpClient)),
                    new TransactionIdProvider(),
                    new CrcCalculator(),
                    logger));
        }
    }
}