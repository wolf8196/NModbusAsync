using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.RtuOverTcp
{
    public class RtuOverTcpWriteSingleCoilTest : WriteSingleCoilTest
    {
        public RtuOverTcpWriteSingleCoilTest(ITestOutputHelper output)
        : base(RtuOverTcpMaster, output)
        {
        }
    }
}