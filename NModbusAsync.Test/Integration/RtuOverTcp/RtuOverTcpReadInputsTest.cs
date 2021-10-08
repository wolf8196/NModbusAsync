using System.Diagnostics.CodeAnalysis;

namespace NModbusAsync.Test.Integration.RtuOverTcp
{
    [ExcludeFromCodeCoverage]
    public class RtuOverTcpReadInputsTest : ReadInputsTest
    {
        public RtuOverTcpReadInputsTest()
        : base(RtuOverTpcMaster)
        {
        }
    }
}