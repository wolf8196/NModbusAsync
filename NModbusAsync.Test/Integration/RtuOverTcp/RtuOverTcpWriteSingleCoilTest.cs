using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.RtuOverTcp
{
    [ExcludeFromCodeCoverage]
    public class RtuOverTcpWriteSingleCoilTest : WriteSingleCoilTest
    {
        public RtuOverTcpWriteSingleCoilTest(ITestOutputHelper output)
        : base(RtuOverTcpMaster, output)
        {
        }
    }
}