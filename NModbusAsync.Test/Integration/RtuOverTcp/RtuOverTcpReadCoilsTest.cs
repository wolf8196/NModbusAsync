using System.Diagnostics.CodeAnalysis;

namespace NModbusAsync.Test.Integration
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