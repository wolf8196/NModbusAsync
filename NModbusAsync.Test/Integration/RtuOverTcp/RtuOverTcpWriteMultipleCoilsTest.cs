using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.RtuOverTcp
{
    public class RtuOverTcpWriteMultipleCoilsTest : WriteMultipleCoilsTest
    {
        public RtuOverTcpWriteMultipleCoilsTest(ITestOutputHelper output)
        : base(RtuOverTcpMaster, output)
        {
        }
    }
}