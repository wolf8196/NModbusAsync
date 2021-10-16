using System.Net.Sockets;

namespace NModbusAsync
{
    public interface IModbusFactory
    {
        IModbusMaster CreateRtuOverTcpMaster<TResource>(TResource tcpClient) where TResource : TcpClient;

        IModbusMaster CreateRtuOverTcpMaster<TResource>(TResource tcpClient, IModbusLogger logger) where TResource : TcpClient;

        IModbusMaster CreateTcpMaster<T>(T tcpClient) where T : TcpClient;

        IModbusMaster CreateTcpMaster<TResource>(TResource tcpClient, IModbusLogger logger) where TResource : TcpClient;
    }
}