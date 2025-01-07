using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.Tcp
{
    public class TcpWriteMultipleCoilsTest : WriteMultipleCoilsTest
    {
        public TcpWriteMultipleCoilsTest(ITestOutputHelper output)
        : base(TcpMaster, output)
        {
        }
    }
}