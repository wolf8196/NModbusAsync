using System.Diagnostics.CodeAnalysis;

namespace NModbusAsync.Test.Integration.RtuOverTcp
{
    [ExcludeFromCodeCoverage]
    public class RtuOverTcpReadInputRegistersTest : ReadInputRegistersTest
    {
        public RtuOverTcpReadInputRegistersTest()
            : base(RtuOverTpcMaster)
        {
        }
    }
}