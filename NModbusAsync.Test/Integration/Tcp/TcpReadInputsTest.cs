using System.Diagnostics.CodeAnalysis;

namespace NModbusAsync.Test.Integration.Tcp
{
    [ExcludeFromCodeCoverage]
    public class TcpReadInputsTest : ReadInputsTest
    {
        public TcpReadInputsTest()
        : base(TcpMaster)
        {
        }
    }
}