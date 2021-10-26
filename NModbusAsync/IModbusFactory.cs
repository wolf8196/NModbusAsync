using System.Net.Sockets;
using Microsoft.Extensions.Logging;

namespace NModbusAsync
{
    public interface IModbusFactory
    {
        IModbusMaster CreateRtuOverTcpMaster<TResource>(TResource tcpClient)
            where TResource : TcpClient;

        IModbusMaster CreateRtuOverTcpMaster<TResource>(TResource tcpClient, ILogger<IModbusMaster> logger)
            where TResource : TcpClient;

        IModbusMaster CreateTcpMaster<T>(T tcpClient)
            where T : TcpClient;

        IModbusMaster CreateTcpMaster<TResource>(TResource tcpClient, ILogger<IModbusMaster> logger)
            where TResource : TcpClient;
    }
}