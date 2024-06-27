using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.RtuOverTcp
{
    [ExcludeFromCodeCoverage]
    public class RtuOverTcpReadHoldingRegistersTest : ReadHoldingRegistersTest
    {
        public RtuOverTcpReadHoldingRegistersTest(ITestOutputHelper output)
            : base(RtuOverTcpMaster, output)
        {
        }
    }
}