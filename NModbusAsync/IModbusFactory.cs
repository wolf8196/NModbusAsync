using System.Net.Sockets;

namespace NModbusAsync
{
    public interface IModbusFactory
    {
        IModbusMaster CreateMaster<T>(T tcpClient) where T : TcpClient;
    }
}