using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.RtuOverTcp
{
    [ExcludeFromCodeCoverage]
    public class RtuOverTcpReadCoilsTest : ReadCoilsTest
    {
        public RtuOverTcpReadCoilsTest(ITestOutputHelper output)
        : base(RtuOverTcpMaster, output)
        {
        }
    }
}