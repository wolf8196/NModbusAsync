using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.Tcp
{
    [ExcludeFromCodeCoverage]
    public class TcpWriteMultipleCoilsTest : WriteMultipleCoilsTest
    {
        public TcpWriteMultipleCoilsTest(ITestOutputHelper output)
        : base(TcpMaster, output)
        {
        }
    }
}