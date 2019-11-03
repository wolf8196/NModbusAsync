using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Moq;
using NModbusAsync.Devices;
using NModbusAsync.IO;
using NModbusAsync.Messages;
using NModbusAsync.Utility;
using Xunit;

namespace NModbusAsync.Test.Unit
{
    [ExcludeFromCodeCoverage]
    public class ArgumentValidationTest
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void ModbusMasterThrowsOnNullTransport()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>("transport", () => new ModbusMaster(null));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ModbusTransportThrowsOnNullPipeResource()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>("pipeResource", () => new ModbusIpTransport(null, null, null));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ModbusTransportThrowsOnNullLogger()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>("logger", () => new ModbusIpTransport(new Mock<IPipeResource>().Object, null, null));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ModbusTransportThrowsOnNullTransactionIdProvider()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>("transactionIdProvider", () => new ModbusIpTransport(new Mock<IPipeResource>().Object, new Mock<IModbusLogger>().Object, null));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void PipeAdapterThrowsOnNullStreamResource()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>("streamResource", () => new PipeAdapter<TcpClient>(null));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void TcpClientAdapterThrowsOnNullStreamResource()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>("tcpClient", () => new TcpClientAdapter<TcpClient>(null));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void SlaveExceptionThrowsOnNullSlaveExceptionResponse()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>("slaveExceptionResponse", () => new SlaveException(null));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ModbusRequestThrowsOnInvalidBufferSize()
        {
            // Arrange
            var request = new ReadHoldingRegistersRequest(1, 1, 1);
            var memory = new Memory<byte>(new byte[4]);

            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>("buffer", () => request.WriteTo(memory));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void NetCoreBitConverterToInt16ThrowsOnInvalidSpanSize()
        {
            // Arrange
            var array = new byte[1];

            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>("value", () => NetCoreBitConverter.ToInt16(array));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void NetCoreBitConverterToInt16ReturnsFalseOnInvalidSpanSize()
        {
            // Arrange
            var array = new byte[1];

            // Act/Assert
            Assert.False(NetCoreBitConverter.TryWriteBytes(array, short.MaxValue));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2001)]
        [InlineData(3000)]
        [Trait("Category", "Unit")]
        public async Task ReadCoilsAsyncThrowsOnInvalidNumberOfPoints(ushort numberOfPoints)
        {
            // Arrange
            var target = new ModbusMaster(new Mock<IModbusTransport>().Object);

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                "numberOfPoints",
                () => target.ReadCoilsAsync(1, 0, numberOfPoints));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2001)]
        [InlineData(3000)]
        [Trait("Category", "Unit")]
        public async Task ReadInputsAsyncThrowsOnInvalidNumberOfPoints(ushort numberOfPoints)
        {
            // Arrange
            var target = new ModbusMaster(new Mock<IModbusTransport>().Object);

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                "numberOfPoints",
                () => target.ReadInputsAsync(1, 0, numberOfPoints));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(126)]
        [InlineData(1000)]
        [Trait("Category", "Unit")]
        public async Task ReadHoldingRegistersAsyncThrowsOnInvalidNumberOfPoints(ushort numberOfPoints)
        {
            // Arrange
            var target = new ModbusMaster(new Mock<IModbusTransport>().Object);

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                "numberOfPoints",
                () => target.ReadHoldingRegistersAsync(1, 0, numberOfPoints));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(126)]
        [InlineData(1000)]
        [Trait("Category", "Unit")]
        public async Task ReadInputRegistersAsyncThrowsOnInvalidNumberOfPoints(ushort numberOfPoints)
        {
            // Arrange
            var target = new ModbusMaster(new Mock<IModbusTransport>().Object);

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                "numberOfPoints",
                () => target.ReadInputRegistersAsync(1, 0, numberOfPoints));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task WriteMultipleCoilsAsyncThrowsOnNullData()
        {
            // Arrange
            var target = new ModbusMaster(new Mock<IModbusTransport>().Object);

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                "data",
                () => target.WriteMultipleCoilsAsync(1, 0, null));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1969)]
        [InlineData(1970)]
        [InlineData(2000)]
        [Trait("Category", "Unit")]
        public async Task WriteMultipleCoilsAsyncThrowsOnInvalidData(ushort numberOfPoints)
        {
            // Arrange
            var target = new ModbusMaster(new Mock<IModbusTransport>().Object);

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                "data",
                () => target.WriteMultipleCoilsAsync(1, 0, Enumerable.Repeat(true, numberOfPoints).ToArray()));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task WriteMultipleRegistersAsyncThrowsOnNullData()
        {
            // Arrange
            var target = new ModbusMaster(new Mock<IModbusTransport>().Object);

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                "data",
                async () => await target.WriteMultipleRegistersAsync(1, 0, null));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(124)]
        [InlineData(200)]
        [Trait("Category", "Unit")]
        public async Task WriteMultipleRegistersAsyncThrowsOnInvalidData(ushort numberOfPoints)
        {
            // Arrange
            var target = new ModbusMaster(new Mock<IModbusTransport>().Object);

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                "data",
                async () => await target.WriteMultipleRegistersAsync(1, 0, Enumerable.Repeat((ushort)1, numberOfPoints).ToArray()));
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(-100)]
        [Trait("Category", "Unit")]
        public void ModbusTransportThrowsOnInvalidWaitToRetryMilliseconds(int milliseconds)
        {
            // Arrange
            var transport = new ModbusIpTransport(new Mock<IPipeResource>().Object, new Mock<IModbusLogger>().Object, new Mock<ITransactionIdProvider>().Object);

            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => transport.WaitToRetryMilliseconds = milliseconds);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-2)]
        [Trait("Category", "Unit")]
        public void PipeAdapterThrowsOnInvalidReadTimeout(int timeout)
        {
            // Arrange
            var streamResourceMock = new Mock<IStreamResource<TcpClient>>();
            streamResourceMock.Setup(x => x.GetStream()).Returns(new MemoryStream());
            var pipeAdapter = new PipeAdapter<TcpClient>(streamResourceMock.Object);

            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => pipeAdapter.ReadTimeout = timeout);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-2)]
        [Trait("Category", "Unit")]
        public void PipeAdapterThrowsOnInvalidWriteTimeout(int timeout)
        {
            // Arrange
            var streamResourceMock = new Mock<IStreamResource<TcpClient>>();
            streamResourceMock.Setup(x => x.GetStream()).Returns(new MemoryStream());
            var pipeAdapter = new PipeAdapter<TcpClient>(streamResourceMock.Object);

            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => pipeAdapter.WriteTimeout = timeout);
        }
    }
}