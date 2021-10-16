using System.Diagnostics.CodeAnalysis;

namespace NModbusAsync.Test.Integration.Tcp
{
    [ExcludeFromCodeCoverage]
    public class TcpWriteMultipleCoilsTest : WriteMultipleCoilsTest
    {
        public TcpWriteMultipleCoilsTest()
        : base(TcpMaster)
        {
        }
    }
}