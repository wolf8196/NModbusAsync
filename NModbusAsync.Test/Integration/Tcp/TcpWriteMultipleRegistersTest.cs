using System.Diagnostics.CodeAnalysis;

namespace NModbusAsync.Test.Integration.Tcp
{
    [ExcludeFromCodeCoverage]
    public class TcpWriteMultipleRegistersTest : WriteMultipleRegistersTest
    {
        public TcpWriteMultipleRegistersTest()
        : base(TcpMaster)
        {
        }
    }
}