using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.RtuOverTcp
{
    [ExcludeFromCodeCoverage]
    public class RtuOverTcpWriteSingleHoldingRegisterTest : WriteSingleHoldingRegisterTest
    {
        public RtuOverTcpWriteSingleHoldingRegisterTest(ITestOutputHelper output)
            : base(RtuOverTcpMaster, output)
        {
        }
    }
}