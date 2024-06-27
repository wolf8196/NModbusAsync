using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.Tcp
{
    [ExcludeFromCodeCoverage]
    public class TcpReadCoilsTest : ReadCoilsTest
    {
        public TcpReadCoilsTest(ITestOutputHelper output)
            : base(TcpMaster, output)
        {
        }
    }
}