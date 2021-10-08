using System.Diagnostics.CodeAnalysis;

namespace NModbusAsync.Test.Integration.RtuOverTcp
{
    [ExcludeFromCodeCoverage]
    public class RtuOverTcpWriteSingleHoldingRegisterTest : WriteSingleHoldingRegisterTest
    {
        public RtuOverTcpWriteSingleHoldingRegisterTest()
            : base(RtuOverTpcMaster)
        {
        }
    }
}