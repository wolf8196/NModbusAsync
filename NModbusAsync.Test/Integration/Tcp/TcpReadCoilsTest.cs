using System.Diagnostics.CodeAnalysis;

namespace NModbusAsync.Test.Integration.Tcp
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