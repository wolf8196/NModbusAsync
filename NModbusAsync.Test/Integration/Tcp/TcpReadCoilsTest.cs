using System.Diagnostics.CodeAnalysis;

namespace NModbusAsync.Test.Integration
{
    [ExcludeFromCodeCoverage]
    public class TcpReadCoilsTest : ReadCoilsTest
    {
        public TcpReadCoilsTest()
        : base(TcpMaster)
        {
        }
    }
}