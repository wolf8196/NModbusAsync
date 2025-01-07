using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.Tcp
{
    public class TcpTimeoutTest : TimeoutTest
    {
        public TcpTimeoutTest(ITestOutputHelper output)
        : base(TcpMaster, output)
        {
        }
    }
}