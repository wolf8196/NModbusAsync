using System.Net.Sockets;
using NModbusAsync.IO;
using Xunit;

namespace NModbusAsync.Test.Unit
{
    public class TcpClientAdapterTest
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void DisposesTcpClient()
        {
            // Arrange
            var tcpClient = new TcpClient();
            var target = new TcpClientAdapter<TcpClient>(tcpClient);

            // Act
            target.Dispose();

            // Assert
            Assert.Null(tcpClient.Client);
        }
    }
}