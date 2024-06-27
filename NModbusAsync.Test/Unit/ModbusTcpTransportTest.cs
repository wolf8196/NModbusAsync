﻿using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
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
    [ExcludeFromCodeCoverage]
    public class ModbusTcpTransportTest
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task ReadsPipeOnceIfEnoughtData()
        {
            // Arrange
            var request = new ReadHoldingRegistersRequest(1, 1, 1);
            var response = new byte[] { 0, 1, 0, 0, 0, 5, 1, 3, 2, 0, 0 };
            var responseSequence = new ReadOnlySequence<byte>(response.AsMemory());
            var pipeAdapterMock = new Mock<IPipeResource>();
            pipeAdapterMock.Setup(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            pipeAdapterMock.Setup(x => x.ReadAsync(6, It.IsAny<CancellationToken>())).ReturnsAsync(responseSequence);
            var transactionIdProviderMock = new Mock<ITransactionIdProvider>();
            transactionIdProviderMock.Setup(x => x.NewId()).Returns(1);

            var target = new ModbusTcpTransport(pipeAdapterMock.Object, transactionIdProviderMock.Object, Mock.Of<ILogger<IModbusMaster>>());

            // Act
            var actual = await target.SendAsync<ReadHoldingRegistersResponse>(request, It.IsAny<CancellationToken>());

            // Assert
            pipeAdapterMock.Verify(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()), Times.Once());
            pipeAdapterMock.Verify(x => x.ReadAsync(6, It.IsAny<CancellationToken>()), Times.Once());
            pipeAdapterMock.Verify(x => x.MarkConsumed(responseSequence), Times.Once());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ReadsPipeAgainIfNotEnoughtData()
        {
            // Arrange
            var request = new ReadHoldingRegistersRequest(1, 1, 1);
            var response = new byte[] { 0, 1, 0, 0, 0, 5, 1, 3, 2, 0, 0 };
            var responseSequence = new ReadOnlySequence<byte>(response.AsMemory());
            var pipeAdapterMock = new Mock<IPipeResource>();
            pipeAdapterMock.Setup(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            pipeAdapterMock.Setup(x => x.ReadAsync(6, It.IsAny<CancellationToken>())).ReturnsAsync(responseSequence.Slice(0, 6));
            pipeAdapterMock.Setup(x => x.ReadAsync(11, It.IsAny<CancellationToken>())).ReturnsAsync(responseSequence);
            var transactionIdProviderMock = new Mock<ITransactionIdProvider>();
            transactionIdProviderMock.Setup(x => x.NewId()).Returns(1);

            var target = new ModbusTcpTransport(pipeAdapterMock.Object, transactionIdProviderMock.Object, Mock.Of<ILogger<IModbusMaster>>());

            // Act
            var actual = await target.SendAsync<ReadHoldingRegistersResponse>(request, It.IsAny<CancellationToken>());

            // Assert
            pipeAdapterMock.Verify(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()), Times.Once());
            pipeAdapterMock.Verify(x => x.ReadAsync(6, It.IsAny<CancellationToken>()), Times.Once());
            pipeAdapterMock.Verify(x => x.ReadAsync(6, It.IsAny<CancellationToken>()), Times.Once());
            pipeAdapterMock.Verify(x => x.MarkExamined(responseSequence.Slice(0, 6)), Times.Once());
            pipeAdapterMock.Verify(x => x.MarkConsumed(responseSequence), Times.Once());
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
            pipeAdapterMock.SetupSequence(x => x.ReadAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(oldResponse)
                .ReturnsAsync(oldResponse)
                .ReturnsAsync(newResponse);
            var transactionIdProviderMock = new Mock<ITransactionIdProvider>();
            transactionIdProviderMock.Setup(x => x.NewId()).Returns(2);
            var target = new ModbusTcpTransport(pipeAdapterMock.Object, transactionIdProviderMock.Object, Mock.Of<ILogger<IModbusMaster>>())
            {
                Retries = 2
            };

            // Act
            var actual = await target.SendAsync<ReadHoldingRegistersResponse>(request);

            // Assert
            pipeAdapterMock.Verify(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
            pipeAdapterMock.Verify(x => x.ReadAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
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
            pipeAdapterMock.SetupSequence(x => x.ReadAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(oldResponse)
                .ReturnsAsync(oldResponse)
                .ReturnsAsync(oldResponse)
                .ReturnsAsync(newResponse);
            var transactionIdProviderMock = new Mock<ITransactionIdProvider>();
            transactionIdProviderMock.Setup(x => x.NewId()).Returns(2);
            var target = new ModbusTcpTransport(pipeAdapterMock.Object, transactionIdProviderMock.Object, Mock.Of<ILogger<IModbusMaster>>())
            {
                Retries = 2
            };

            // Act
            await Assert.ThrowsAsync<IOException>(() => target.SendAsync<ReadHoldingRegistersResponse>(request));

            // Assert
            pipeAdapterMock.Verify(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
            pipeAdapterMock.Verify(x => x.ReadAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
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
            pipeAdapterMock.SetupSequence(x => x.ReadAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(oldResponse)
                .ReturnsAsync(newResponse);
            var transactionIdProviderMock = new Mock<ITransactionIdProvider>();
            transactionIdProviderMock.Setup(x => x.NewId()).Returns(5);
            var target = new ModbusTcpTransport(pipeAdapterMock.Object, transactionIdProviderMock.Object, Mock.Of<ILogger<IModbusMaster>>())
            {
                Retries = 0,
                RetryOnOldTransactionIdThreshold = 3
            };

            // Act
            var actual = await target.SendAsync<ReadHoldingRegistersResponse>(request);

            // Assert
            pipeAdapterMock.Verify(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()), Times.Once());
            pipeAdapterMock.Verify(x => x.ReadAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
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
            pipeAdapterMock.SetupSequence(x => x.ReadAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(oldResponse)
                .ReturnsAsync(newResponse);
            var transactionIdProviderMock = new Mock<ITransactionIdProvider>();
            transactionIdProviderMock.Setup(x => x.NewId()).Returns(2);
            var target = new ModbusTcpTransport(pipeAdapterMock.Object, transactionIdProviderMock.Object, Mock.Of<ILogger<IModbusMaster>>())
            {
                Retries = 0,
                RetryOnOldTransactionIdThreshold = 1
            };

            // Act
            await Assert.ThrowsAsync<IOException>(() => target.SendAsync<ReadHoldingRegistersResponse>(request));

            // Assert
            pipeAdapterMock.Verify(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()), Times.Once());
            pipeAdapterMock.Verify(x => x.ReadAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once());
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
            pipeAdapterMock.SetupSequence(x => x.ReadAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(oldResponse)
                .ReturnsAsync(newResponse);
            var transactionIdProviderMock = new Mock<ITransactionIdProvider>();
            transactionIdProviderMock.Setup(x => x.NewId()).Returns(2);
            var target = new ModbusTcpTransport(pipeAdapterMock.Object, transactionIdProviderMock.Object, Mock.Of<ILogger<IModbusMaster>>())
            {
                Retries = 0,
                RetryOnOldTransactionIdThreshold = 2
            };

            // Act
            await Assert.ThrowsAsync<IOException>(() => target.SendAsync<ReadHoldingRegistersResponse>(request));

            // Assert
            pipeAdapterMock.Verify(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()), Times.Once());
            pipeAdapterMock.Verify(x => x.ReadAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}