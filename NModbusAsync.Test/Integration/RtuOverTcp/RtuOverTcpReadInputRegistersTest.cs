using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.RtuOverTcp
{
    [ExcludeFromCodeCoverage]
    public class RtuOverTcpReadInputRegistersTest : ReadInputRegistersTest
    {
        public RtuOverTcpReadInputRegistersTest(ITestOutputHelper output)
            : base(RtuOverTcpMaster, output)
        {
        }
    }
}