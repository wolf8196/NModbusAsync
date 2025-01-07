using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.Tcp
{
    public class TcpReadHoldingRegistersTest : ReadHoldingRegistersTest
    {
        public TcpReadHoldingRegistersTest(ITestOutputHelper output)
            : base(TcpMaster, output)
        {
        }
    }
}