using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.Tcp
{
    [ExcludeFromCodeCoverage]
    public class TcpReadHoldingRegistersTest : ReadHoldingRegistersTest
    {
        public TcpReadHoldingRegistersTest(ITestOutputHelper output)
            : base(TcpMaster, output)
        {
        }
    }
}