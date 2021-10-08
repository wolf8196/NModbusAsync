using System.Diagnostics.CodeAnalysis;

namespace NModbusAsync.Test.Integration.RtuOverTcp
{
    [ExcludeFromCodeCoverage]
    public class RtuOverTcpWriteSingleCoilTest : WriteSingleCoilTest
    {
        public RtuOverTcpWriteSingleCoilTest()
        : base(RtuOverTpcMaster)
        {
        }
    }
}