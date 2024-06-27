using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.Tcp
{
    [ExcludeFromCodeCoverage]
    public class TcpWriteSingleHoldingRegisterTest : WriteSingleHoldingRegisterTest
    {
        public TcpWriteSingleHoldingRegisterTest(ITestOutputHelper output)
            : base(TcpMaster, output)
        {
        }
    }
}