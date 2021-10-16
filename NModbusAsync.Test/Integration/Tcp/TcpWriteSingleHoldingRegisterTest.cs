using System.Diagnostics.CodeAnalysis;

namespace NModbusAsync.Test.Integration.Tcp
{
    [ExcludeFromCodeCoverage]
    public class TcpWriteSingleHoldingRegisterTest : WriteSingleHoldingRegisterTest
    {
        public TcpWriteSingleHoldingRegisterTest()
            : base(TcpMaster)
        {
        }
    }
}