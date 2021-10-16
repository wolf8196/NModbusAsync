using System.Diagnostics.CodeAnalysis;

namespace NModbusAsync.Test.Integration.RtuOverTcp
{
    [ExcludeFromCodeCoverage]
    public class RtuOverTcpReadCoilsTest : ReadCoilsTest
    {
        public RtuOverTcpReadCoilsTest()
        : base(RtuOverTpcMaster)
        {
        }
    }
}