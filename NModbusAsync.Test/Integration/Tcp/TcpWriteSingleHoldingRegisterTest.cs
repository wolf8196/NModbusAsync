using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.Tcp
{
    public class TcpWriteSingleHoldingRegisterTest : WriteSingleHoldingRegisterTest
    {
        public TcpWriteSingleHoldingRegisterTest(ITestOutputHelper output)
            : base(TcpMaster, output)
        {
        }
    }
}