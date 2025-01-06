using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.Tcp
{
    public class TcpWriteMultipleRegistersTest : WriteMultipleRegistersTest
    {
        public TcpWriteMultipleRegistersTest(ITestOutputHelper output)
        : base(TcpMaster, output)
        {
        }
    }
}