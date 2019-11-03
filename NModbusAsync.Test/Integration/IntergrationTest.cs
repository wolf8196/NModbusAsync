using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;

namespace NModbusAsync.Test.Integration
{
    [ExcludeFromCodeCoverage]
    public abstract class IntergrationTest
    {
        protected IntergrationTest(byte slaveId)
        {
            SlaveId = slaveId;
            var tcpClient = new TcpClient();
            tcpClient.Connect("127.0.0.1", 502);
            Target = new ModbusFactory().CreateMaster(tcpClient);
        }

        protected IModbusMaster Target { get; }

        protected byte SlaveId { get; }
    }
}