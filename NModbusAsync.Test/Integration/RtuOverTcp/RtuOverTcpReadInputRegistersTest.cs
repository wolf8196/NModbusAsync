using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.RtuOverTcp
{
    public class RtuOverTcpReadInputRegistersTest : ReadInputRegistersTest
    {
        public RtuOverTcpReadInputRegistersTest(ITestOutputHelper output)
            : base(RtuOverTcpMaster, output)
        {
        }
    }
}