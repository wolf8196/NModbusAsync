using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.RtuOverTcp
{
    public class RtuOverTcpWriteMultipleRegistersTest : WriteMultipleRegistersTest
    {
        public RtuOverTcpWriteMultipleRegistersTest(ITestOutputHelper output)
        : base(RtuOverTcpMaster, output)
        {
        }
    }
}