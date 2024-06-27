using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.RtuOverTcp
{
    [ExcludeFromCodeCoverage]
    public class RtuOverTcpWriteMultipleRegistersTest : WriteMultipleRegistersTest
    {
        public RtuOverTcpWriteMultipleRegistersTest(ITestOutputHelper output)
        : base(RtuOverTcpMaster, output)
        {
        }
    }
}