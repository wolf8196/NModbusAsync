using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.Tcp
{
    [ExcludeFromCodeCoverage]
    public class TcpTimeoutTest : TimeoutTest
    {
        public TcpTimeoutTest(ITestOutputHelper output)
        : base(TcpMaster, output)
        {
        }
    }
}