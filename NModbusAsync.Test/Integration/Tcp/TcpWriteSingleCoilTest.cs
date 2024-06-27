using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.Tcp
{
    [ExcludeFromCodeCoverage]
    public class TcpWriteSingleCoilTest : WriteSingleCoilTest
    {
        public TcpWriteSingleCoilTest(ITestOutputHelper output)
        : base(TcpMaster, output)
        {
        }
    }
}