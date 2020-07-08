using System.Net.Sockets;

namespace NModbusAsync
{
    public interface IModbusFactory
    {
        IModbusMaster CreateMaster<T>(T tcpClient) where T : TcpClient;

        IModbusMaster CreateMaster<TResource>(TResource tcpClient, IModbusLogger logger) where TResource : TcpClient;
    }
}