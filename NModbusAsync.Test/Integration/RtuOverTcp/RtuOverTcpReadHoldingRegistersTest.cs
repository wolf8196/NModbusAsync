using System.Diagnostics.CodeAnalysis;

namespace NModbusAsync.Test.Integration.RtuOverTcp
{
    [ExcludeFromCodeCoverage]
    public class RtuOverTcpReadHoldingRegistersTest : ReadHoldingRegistersTest
    {
        public RtuOverTcpReadHoldingRegistersTest()
            : base(RtuOverTpcMaster)
        {
        }
    }
}