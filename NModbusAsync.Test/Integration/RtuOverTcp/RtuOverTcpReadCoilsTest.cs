using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.RtuOverTcp
{
    public class RtuOverTcpReadCoilsTest : ReadCoilsTest
    {
        public RtuOverTcpReadCoilsTest(ITestOutputHelper output)
        : base(RtuOverTcpMaster, output)
        {
        }
    }
}