using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.Tcp
{
    [ExcludeFromCodeCoverage]
    public class TcpReadInputRegistersTest : ReadInputRegistersTest
    {
        public TcpReadInputRegistersTest(ITestOutputHelper output)
            : base(TcpMaster, output)
        {
        }
    }
}