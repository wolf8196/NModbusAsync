using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.Tcp
{
    [ExcludeFromCodeCoverage]
    public class TcpWriteMultipleRegistersTest : WriteMultipleRegistersTest
    {
        public TcpWriteMultipleRegistersTest(ITestOutputHelper output)
        : base(TcpMaster, output)
        {
        }
    }
}