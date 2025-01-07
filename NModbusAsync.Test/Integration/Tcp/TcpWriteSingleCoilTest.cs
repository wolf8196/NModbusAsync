using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.Tcp
{
    public class TcpWriteSingleCoilTest : WriteSingleCoilTest
    {
        public TcpWriteSingleCoilTest(ITestOutputHelper output)
        : base(TcpMaster, output)
        {
        }
    }
}