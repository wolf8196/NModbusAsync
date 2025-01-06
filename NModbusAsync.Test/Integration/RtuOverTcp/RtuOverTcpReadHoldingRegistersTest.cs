using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.RtuOverTcp
{
    public class RtuOverTcpReadHoldingRegistersTest : ReadHoldingRegistersTest
    {
        public RtuOverTcpReadHoldingRegistersTest(ITestOutputHelper output)
            : base(RtuOverTcpMaster, output)
        {
        }
    }
}