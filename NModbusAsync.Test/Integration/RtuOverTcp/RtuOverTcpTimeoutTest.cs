using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.RtuOverTcp
{
    public class RtuOverTcpTimeoutTest : TimeoutTest
    {
        public RtuOverTcpTimeoutTest(ITestOutputHelper output)
        : base(RtuOverTcpMaster, output)
        {
        }
    }
}