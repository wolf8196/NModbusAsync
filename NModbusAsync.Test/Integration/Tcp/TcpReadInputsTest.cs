using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.Tcp
{
    public class TcpReadInputsTest : ReadInputsTest
    {
        public TcpReadInputsTest(ITestOutputHelper output)
        : base(TcpMaster, output)
        {
        }
    }
}