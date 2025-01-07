using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.Tcp
{
    public class TcpReadInputRegistersTest : ReadInputRegistersTest
    {
        public TcpReadInputRegistersTest(ITestOutputHelper output)
            : base(TcpMaster, output)
        {
        }
    }
}