using System.Diagnostics.CodeAnalysis;

namespace NModbusAsync.Test.Integration.Tcp
{
    [ExcludeFromCodeCoverage]
    public class TcpWriteSingleCoilTest : WriteSingleCoilTest
    {
        public TcpWriteSingleCoilTest()
        : base(TcpMaster)
        {
        }
    }
}