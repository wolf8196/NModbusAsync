using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.RtuOverTcp
{
    public class RtuOverTcpWriteSingleHoldingRegisterTest : WriteSingleHoldingRegisterTest
    {
        public RtuOverTcpWriteSingleHoldingRegisterTest(ITestOutputHelper output)
            : base(RtuOverTcpMaster, output)
        {
        }
    }
}