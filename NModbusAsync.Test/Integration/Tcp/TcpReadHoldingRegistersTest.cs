using System.Diagnostics.CodeAnalysis;

namespace NModbusAsync.Test.Integration.Tcp
{
    [ExcludeFromCodeCoverage]
    public class TcpReadHoldingRegistersTest : ReadHoldingRegistersTest
    {
        public TcpReadHoldingRegistersTest()
            : base(TcpMaster)
        {
        }
    }
}