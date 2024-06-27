using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.RtuOverTcp
{
    [ExcludeFromCodeCoverage]
    public class RtuOverTcpReadInputsTest : ReadInputsTest
    {
        public RtuOverTcpReadInputsTest(ITestOutputHelper output)
        : base(RtuOverTcpMaster, output)
        {
        }
    }
}