using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NModbusAsync.IO;
using NModbusAsync.IO.Abstractions;
using NModbusAsync.Messages;
using Xunit;

namespace NModbusAsync.Test.Unit
{
    public class ModbusRtuOverTcpTransportTest
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task ReadsPipeOnceIfEnoughtData()
        {
            // Arrange
            var request = new ReadHoldingRegistersRequest(1, 1, 1);
            var response = new byte[] { 1, 3, 2, 0, 0, 0, 0 };
            var responseSequence = new ReadOnlySequence<byte>(response.AsMemory());
            var pipeAdapterMock = new Mock<IPipeResource>();
            pipeAdapterMock.Setup(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            pipeAdapterMock.Setup(x => x.ReadAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(responseSequence);
            var transactionIdProviderMock = new Mock<ITransactionIdProvider>();
            transactionIdProviderMock.Setup(x => x.NewId()).Returns(1);
            var crcCalculator = new Mock<ICrcCalculator>();
            crcCalculator.Setup(x => x.Calculate(It.IsAny<ReadOnlyMemory<byte>>())).Returns(0);

            var target = new ModbusRtuOverTcpTransport(pipeAdapterMock.Object, transactionIdProviderMock.Object, crcCalculator.Object, Mock.Of<ILogger<IModbusMaster>>());

            // Act
            var actual = await target.SendAsync<ReadHoldingRegistersResponse>(request, It.IsAny<CancellationToken>());

            // Assert
            pipeAdapterMock.Verify(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()), Times.Once());
            pipeAdapterMock.Verify(x => x.ReadAsync(3, It.IsAny<CancellationToken>()), Times.Once());
            pipeAdapterMock.Verify(x => x.MarkConsumed(responseSequence), Times.Once());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ReadsPipeAgainIfNotEnoughtData()
        {
            // Arrange
            var request = new ReadHoldingRegistersRequest(1, 1, 1);
            var response = new byte[] { 1, 3, 2, 0, 0, 0, 0 };
            var responseSequence = new ReadOnlySequence<byte>(response.AsMemory());
            var pipeAdapterMock = new Mock<IPipeResource>();
            pipeAdapterMock.Setup(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            pipeAdapterMock.Setup(x => x.ReadAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(responseSequence.Slice(0, 3));
            pipeAdapterMock.Setup(x => x.ReadAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync(responseSequence);
            var transactionIdProviderMock = new Mock<ITransactionIdProvider>();
            transactionIdProviderMock.Setup(x => x.NewId()).Returns(1);
            var crcCalculator = new Mock<ICrcCalculator>();
            crcCalculator.Setup(x => x.Calculate(It.IsAny<ReadOnlyMemory<byte>>())).Returns(0);

            var target = new ModbusRtuOverTcpTransport(pipeAdapterMock.Object, transactionIdProviderMock.Object, crcCalculator.Object, Mock.Of<ILogger<IModbusMaster>>());

            // Act
            var actual = await target.SendAsync<ReadHoldingRegistersResponse>(request, It.IsAny<CancellationToken>());

            // Assert
            pipeAdapterMock.Verify(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()), Times.Once());
            pipeAdapterMock.Verify(x => x.ReadAsync(3, It.IsAny<CancellationToken>()), Times.Once());
            pipeAdapterMock.Verify(x => x.ReadAsync(7, It.IsAny<CancellationToken>()), Times.Once());
            pipeAdapterMock.Verify(x => x.MarkExamined(responseSequence.Slice(0, 3)), Times.Once());
            pipeAdapterMock.Verify(x => x.MarkConsumed(responseSequence), Times.Once());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ThrowsOnInvalidCrc()
        {
            // Arrange
            var request = new ReadHoldingRegistersRequest(1, 1, 1);
            var response = new byte[] { 1, 3, 2, 0, 0, 1, 1 };
            var responseSequence = new ReadOnlySequence<byte>(response.AsMemory());
            var pipeAdapterMock = new Mock<IPipeResource>();
            pipeAdapterMock.Setup(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            pipeAdapterMock.Setup(x => x.ReadAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(responseSequence);
            var transactionIdProviderMock = new Mock<ITransactionIdProvider>();
            transactionIdProviderMock.Setup(x => x.NewId()).Returns(1);
            var crcCalculator = new Mock<ICrcCalculator>();
            crcCalculator.Setup(x => x.Calculate(It.IsAny<ReadOnlyMemory<byte>>())).Returns(0);

            var target = new ModbusRtuOverTcpTransport(pipeAdapterMock.Object, transactionIdProviderMock.Object, crcCalculator.Object, Mock.Of<ILogger<IModbusMaster>>());

            // Act
            var ex = await Assert.ThrowsAsync<IOException>(() => target.SendAsync<ReadHoldingRegistersResponse>(request, It.IsAny<CancellationToken>()));

            // Assert
            pipeAdapterMock.Verify(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()), Times.Once());
            pipeAdapterMock.Verify(x => x.ReadAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once());
            pipeAdapterMock.Verify(x => x.MarkConsumed(responseSequence), Times.Once());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task HandlesReadOnSlaveExceptionFunctionCode()
        {
            // Arrange
            var request = new ReadHoldingRegistersRequest(1, 1, 1);
            var response = new byte[] { 1, 129, 1, 0, 0 };
            var responseSequence = new ReadOnlySequence<byte>(response.AsMemory());
            var pipeAdapterMock = new Mock<IPipeResource>();
            pipeAdapterMock.Setup(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            pipeAdapterMock.Setup(x => x.ReadAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(responseSequence);
            var transactionIdProviderMock = new Mock<ITransactionIdProvider>();
            transactionIdProviderMock.Setup(x => x.NewId()).Returns(1);
            var crcCalculator = new Mock<ICrcCalculator>();
            crcCalculator.Setup(x => x.Calculate(It.IsAny<ReadOnlyMemory<byte>>())).Returns(0);

            var target = new ModbusRtuOverTcpTransport(pipeAdapterMock.Object, transactionIdProviderMock.Object, crcCalculator.Object, Mock.Of<ILogger<IModbusMaster>>());

            // Act
            var ex = await Assert.ThrowsAsync<SlaveException>(() => target.SendAsync<ReadHoldingRegistersResponse>(request, It.IsAny<CancellationToken>()));

            // Assert
            pipeAdapterMock.Verify(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()), Times.Once());
            pipeAdapterMock.Verify(x => x.ReadAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once());
            pipeAdapterMock.Verify(x => x.MarkConsumed(responseSequence), Times.Once());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ThrowsOnInvalidFunctionCode()
        {
            // Arrange
            var request = new ReadHoldingRegistersRequest(1, 1, 1);
            var response = new byte[5];
            var responseSequence = new ReadOnlySequence<byte>(response.AsMemory());
            var pipeAdapterMock = new Mock<IPipeResource>();
            pipeAdapterMock.Setup(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            pipeAdapterMock.Setup(x => x.ReadAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(responseSequence);
            var transactionIdProviderMock = new Mock<ITransactionIdProvider>();
            transactionIdProviderMock.Setup(x => x.NewId()).Returns(1);
            var crcCalculator = new Mock<ICrcCalculator>();
            crcCalculator.Setup(x => x.Calculate(It.IsAny<ReadOnlyMemory<byte>>())).Returns(0);

            var target = new ModbusRtuOverTcpTransport(pipeAdapterMock.Object, transactionIdProviderMock.Object, crcCalculator.Object, Mock.Of<ILogger<IModbusMaster>>());

            // Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => target.SendAsync<ReadHoldingRegistersResponse>(request, It.IsAny<CancellationToken>()));

            // Assert
            pipeAdapterMock.Verify(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()), Times.Once());
            pipeAdapterMock.Verify(x => x.ReadAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}