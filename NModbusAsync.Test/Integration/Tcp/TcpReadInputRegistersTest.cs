using System.Diagnostics.CodeAnalysis;

namespace NModbusAsync.Test.Integration.Tcp
{
    [ExcludeFromCodeCoverage]
    public class TcpReadInputRegistersTest : ReadInputRegistersTest
    {
        public TcpReadInputRegistersTest()
            : base(TcpMaster)
        {
        }
    }
}