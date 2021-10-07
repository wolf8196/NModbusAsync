using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NModbusAsync.IO;
using NModbusAsync.Messages;
using Xunit;

namespace NModbusAsync.Test.Unit
{
    [ExcludeFromCodeCoverage]
    public class ModbusIpTransportTest
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task ReadsPipeUntilEnoughtData()
        {
            // Arrange
            var request = new ReadHoldingRegistersRequest(1, 1, 1);
            var response = new byte[] { 0, 1, 0, 0, 0, 5, 1, 3, 2, 0, 0 };
            var responseSequence = new ReadOnlySequence<byte>(response.AsMemory(0, 11));
            var pipeAdapterMock = new Mock<IPipeResource>();
            pipeAdapterMock.Setup(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            pipeAdapterMock.SetupSequence(x => x.ReadAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReadOnlySequence<byte>(response.AsMemory(0, 1)))
                .ReturnsAsync(new ReadOnlySequence<byte>(response.AsMemory(0, 3)))
                .ReturnsAsync(new ReadOnlySequence<byte>(response.AsMemory(0, 5)))
                .ReturnsAsync(new ReadOnlySequence<byte>(response.AsMemory(0, 7)))
                .ReturnsAsync(new ReadOnlySequence<byte>(response.AsMemory(0, 9)))
                .ReturnsAsync(responseSequence);
            var transactionIdProviderMock = new Mock<ITransactionIdProvider>();
            transactionIdProviderMock.Setup(x => x.NewId()).Returns(1);

            var target = new ModbusTcpTransport(pipeAdapterMock.Object, Mock.Of<IModbusLogger>(), transactionIdProviderMock.Object);

            // Act
            var actual = await target.SendAsync<ReadHoldingRegistersResponse>(request, It.IsAny<CancellationToken>());

            // Assert
            pipeAdapterMock.Verify(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()), Times.Once());
            pipeAdapterMock.Verify(x => x.ReadAsync(It.IsAny<CancellationToken>()), Times.Exactly(6));
            pipeAdapterMock.Verify(x => x.AdvanceTo(responseSequence.Start), Times.Exactly(5));
            pipeAdapterMock.Verify(x => x.AdvanceTo(responseSequence.End), Times.Once());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task RetriesOnOldTransactionUsingRetryCount()
        {
            // Arrange
            var request = new ReadHoldingRegistersRequest(1, 1, 1);
            var oldResponse = new ReadOnlySequence<byte>(new byte[] { 0, 1, 0, 0, 0, 5, 1, 3, 2, 0, 0 }.AsMemory());
            var newResponse = new ReadOnlySequence<byte>(new byte[] { 0, 2, 0, 0, 0, 5, 1, 3, 2, 0, 0 }.AsMemory());
            var pipeAdapterMock = new Mock<IPipeResource>();
            pipeAdapterMock.Setup(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            pipeAdapterMock.SetupSequence(x => x.ReadAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(oldResponse)
                .ReturnsAsync(oldResponse)
                .ReturnsAsync(newResponse);
            var transactionIdProviderMock = new Mock<ITransactionIdProvider>();
            transactionIdProviderMock.Setup(x => x.NewId()).Returns(2);
            var target = new ModbusTcpTransport(pipeAdapterMock.Object, Mock.Of<IModbusLogger>(), transactionIdProviderMock.Object)
            {
                Retries = 2
            };

            // Act
            var actual = await target.SendAsync<ReadHoldingRegistersResponse>(request);

            // Assert
            pipeAdapterMock.Verify(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
            pipeAdapterMock.Verify(x => x.ReadAsync(It.IsAny<CancellationToken>()), Times.Exactly(3));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ThrowsOnOldTransactionIfRetryCountExceeded()
        {
            // Arrange
            var request = new ReadHoldingRegistersRequest(1, 1, 1);
            var oldResponse = new ReadOnlySequence<byte>(new byte[] { 0, 1, 0, 0, 0, 5, 1, 3, 2, 0, 0 }.AsMemory());
            var newResponse = new ReadOnlySequence<byte>(new byte[] { 0, 2, 0, 0, 0, 5, 1, 3, 2, 0, 0 }.AsMemory());
            var pipeAdapterMock = new Mock<IPipeResource>();
            pipeAdapterMock.Setup(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            pipeAdapterMock.SetupSequence(x => x.ReadAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(oldResponse)
                .ReturnsAsync(oldResponse)
                .ReturnsAsync(oldResponse)
                .ReturnsAsync(newResponse);
            var transactionIdProviderMock = new Mock<ITransactionIdProvider>();
            transactionIdProviderMock.Setup(x => x.NewId()).Returns(2);
            var target = new ModbusTcpTransport(pipeAdapterMock.Object, Mock.Of<IModbusLogger>(), transactionIdProviderMock.Object)
            {
                Retries = 2
            };

            // Act
            await Assert.ThrowsAsync<IOException>(() => target.SendAsync<ReadHoldingRegistersResponse>(request));

            // Assert
            pipeAdapterMock.Verify(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
            pipeAdapterMock.Verify(x => x.ReadAsync(It.IsAny<CancellationToken>()), Times.Exactly(3));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task RetriesOnOldTransactionIfDifferenceIsAboveThreshold()
        {
            // Arrange
            var request = new ReadHoldingRegistersRequest(1, 1, 1);
            var oldResponse = new ReadOnlySequence<byte>(new byte[] { 0, 3, 0, 0, 0, 5, 1, 3, 2, 0, 0 }.AsMemory());
            var newResponse = new ReadOnlySequence<byte>(new byte[] { 0, 5, 0, 0, 0, 5, 1, 3, 2, 0, 0 }.AsMemory());
            var pipeAdapterMock = new Mock<IPipeResource>();
            pipeAdapterMock.Setup(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            pipeAdapterMock.SetupSequence(x => x.ReadAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(oldResponse)
                .ReturnsAsync(newResponse);
            var transactionIdProviderMock = new Mock<ITransactionIdProvider>();
            transactionIdProviderMock.Setup(x => x.NewId()).Returns(5);
            var target = new ModbusTcpTransport(pipeAdapterMock.Object, Mock.Of<IModbusLogger>(), transactionIdProviderMock.Object)
            {
                Retries = 0,
                RetryOnOldResponseThreshold = 3
            };

            // Act
            var actual = await target.SendAsync<ReadHoldingRegistersResponse>(request);

            // Assert
            pipeAdapterMock.Verify(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()), Times.Once());
            pipeAdapterMock.Verify(x => x.ReadAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ThrowsOnOldTransactionIfDifferenceIsBelowThreshold()
        {
            // Arrange
            var request = new ReadHoldingRegistersRequest(1, 1, 1);
            var oldResponse = new ReadOnlySequence<byte>(new byte[] { 0, 3, 0, 0, 0, 5, 1, 3, 2, 0, 0 }.AsMemory());
            var newResponse = new ReadOnlySequence<byte>(new byte[] { 0, 5, 0, 0, 0, 5, 1, 3, 2, 0, 0 }.AsMemory());
            var pipeAdapterMock = new Mock<IPipeResource>();
            pipeAdapterMock.Setup(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            pipeAdapterMock.SetupSequence(x => x.ReadAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(oldResponse)
                .ReturnsAsync(newResponse);
            var transactionIdProviderMock = new Mock<ITransactionIdProvider>();
            transactionIdProviderMock.Setup(x => x.NewId()).Returns(2);
            var target = new ModbusTcpTransport(pipeAdapterMock.Object, Mock.Of<IModbusLogger>(), transactionIdProviderMock.Object)
            {
                Retries = 0,
                RetryOnOldResponseThreshold = 1
            };

            // Act
            await Assert.ThrowsAsync<IOException>(() => target.SendAsync<ReadHoldingRegistersResponse>(request));

            // Assert
            pipeAdapterMock.Verify(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()), Times.Once());
            pipeAdapterMock.Verify(x => x.ReadAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ThrowsOnOldTransactionIfDifferenceIsEqualToThreshold()
        {
            // Arrange
            var request = new ReadHoldingRegistersRequest(1, 1, 1);
            var oldResponse = new ReadOnlySequence<byte>(new byte[] { 0, 3, 0, 0, 0, 5, 1, 3, 2, 0, 0 }.AsMemory());
            var newResponse = new ReadOnlySequence<byte>(new byte[] { 0, 5, 0, 0, 0, 5, 1, 3, 2, 0, 0 }.AsMemory());
            var pipeAdapterMock = new Mock<IPipeResource>();
            pipeAdapterMock.Setup(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            pipeAdapterMock.SetupSequence(x => x.ReadAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(oldResponse)
                .ReturnsAsync(newResponse);
            var transactionIdProviderMock = new Mock<ITransactionIdProvider>();
            transactionIdProviderMock.Setup(x => x.NewId()).Returns(2);
            var target = new ModbusTcpTransport(pipeAdapterMock.Object, Mock.Of<IModbusLogger>(), transactionIdProviderMock.Object)
            {
                Retries = 0,
                RetryOnOldResponseThreshold = 2
            };

            // Act
            await Assert.ThrowsAsync<IOException>(() => target.SendAsync<ReadHoldingRegistersResponse>(request));

            // Assert
            pipeAdapterMock.Verify(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()), Times.Once());
            pipeAdapterMock.Verify(x => x.ReadAsync(It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}