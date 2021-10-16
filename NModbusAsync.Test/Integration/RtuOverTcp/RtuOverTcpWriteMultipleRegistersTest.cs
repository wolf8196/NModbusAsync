using System.Diagnostics.CodeAnalysis;

namespace NModbusAsync.Test.Integration.RtuOverTcp
{
    [ExcludeFromCodeCoverage]
    public class RtuOverTcpWriteMultipleRegistersTest : WriteMultipleRegistersTest
    {
        public RtuOverTcpWriteMultipleRegistersTest()
        : base(RtuOverTpcMaster)
        {
        }
    }
}