using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using Moq;
using Xunit;

namespace NModbusAsync.Test.Integration
{
    [ExcludeFromCodeCoverage]
    public class ModbusFactoryTest
    {
        [Fact]
        [Trait("Category", "Intergration")]
        public void PassesLoggerToMasterFromConstructor()
        {
            // Arrange
            var target = new ModbusFactory(null);
            var client = new TcpClient("127.0.0.1", 502);

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => target.CreateMaster(client));
        }

        [Fact]
        [Trait("Category", "Intergration")]
        public void PassesLoggerToMasterFromParameter()
        {
            // Arrange
            var target = new ModbusFactory(Mock.Of<IModbusLogger>());
            var client = new TcpClient("127.0.0.1", 502);

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => target.CreateMaster(client, null));
        }
    }
}