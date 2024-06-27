using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.RtuOverTcp
{
    [ExcludeFromCodeCoverage]
    public class RtuOverTcpWriteMultipleCoilsTest : WriteMultipleCoilsTest
    {
        public RtuOverTcpWriteMultipleCoilsTest(ITestOutputHelper output)
        : base(RtuOverTcpMaster, output)
        {
        }
    }
}