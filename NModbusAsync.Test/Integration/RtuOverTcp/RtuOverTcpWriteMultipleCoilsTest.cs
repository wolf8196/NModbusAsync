using System.Diagnostics.CodeAnalysis;

namespace NModbusAsync.Test.Integration.RtuOverTcp
{
    [ExcludeFromCodeCoverage]
    public class RtuOverTcpWriteMultipleCoilsTest : WriteMultipleCoilsTest
    {
        public RtuOverTcpWriteMultipleCoilsTest()
        : base(RtuOverTpcMaster)
        {
        }
    }
}