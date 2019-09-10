using System.Net.Sockets;

namespace NModbusAsync
{
    public interface IModbusFactory
    {
        IModbusMaster CreateMaster(TcpClient client);
    }
}