using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.RtuOverTcp
{
    public class RtuOverTcpReadInputsTest : ReadInputsTest
    {
        public RtuOverTcpReadInputsTest(ITestOutputHelper output)
        : base(RtuOverTcpMaster, output)
        {
        }
    }
}