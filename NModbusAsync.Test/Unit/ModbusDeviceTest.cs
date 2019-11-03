using System.Diagnostics.CodeAnalysis;
using Moq;
using NModbusAsync.Devices;
using Xunit;

namespace NModbusAsync.Test.Unit
{
    [ExcludeFromCodeCoverage]
    public class ModbusDeviceTest
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void DisposesTransport()
        {
            // Arrange
            var transportMock = new Mock<IModbusTransport>();
            transportMock.Setup(x => x.Dispose());
            var master = new ModbusMaster(transportMock.Object);

            // Act
            master.Dispose();

            // Assert
            transportMock.Verify(x => x.Dispose(), Times.Once());
        }
    }
}