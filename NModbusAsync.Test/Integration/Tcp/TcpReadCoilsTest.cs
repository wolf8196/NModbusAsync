using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.Tcp
{
    public class TcpReadCoilsTest : ReadCoilsTest
    {
        public TcpReadCoilsTest(ITestOutputHelper output)
            : base(TcpMaster, output)
        {
        }
    }
}