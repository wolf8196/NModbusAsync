using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.RtuOverTcp
{
    [ExcludeFromCodeCoverage]
    public class RtuOverTcpTimeoutTest : TimeoutTest
    {
        public RtuOverTcpTimeoutTest(ITestOutputHelper output)
        : base(RtuOverTcpMaster, output)
        {
        }
    }
}