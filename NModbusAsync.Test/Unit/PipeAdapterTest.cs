using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NModbusAsync.IO;
using NModbusAsync.IO.Abstractions;
using Xunit;

namespace NModbusAsync.Test.Unit
{
    public class PipeAdapterTest
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task ReadsOnceIfEnoughData()
        {
            // Arrange
            var streamMock = new Mock<Stream> { CallBase = false };
            streamMock.Setup(x => x.ReadAsync(It.IsAny<Memory<byte>>(), It.IsAny<CancellationToken>())).ReturnsAsync(10);
            var streamResourceMock = new Mock<IStreamResource<TcpClient>>();
            streamResourceMock.Setup(x => x.GetStream()).Returns(streamMock.Object);
            var pipeAdapter = new PipeAdapter<TcpClient>(streamResourceMock.Object);

            // Act
            var actual = await pipeAdapter.ReadAsync(10, CancellationToken.None);

            // Assert
            Assert.Equal(10, actual.Length);
            streamMock.Verify(x => x.ReadAsync(It.IsAny<Memory<byte>>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ReadsUntilEnoughData()
        {
            // Arrange
            var streamMock = new Mock<Stream> { CallBase = false };
            streamMock
                .SetupSequence(x => x.ReadAsync(It.IsAny<Memory<byte>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(3)
                .ReturnsAsync(3)
                .ReturnsAsync(4);

            var streamResourceMock = new Mock<IStreamResource<TcpClient>>();
            streamResourceMock.Setup(x => x.GetStream()).Returns(streamMock.Object);
            var pipeAdapter = new PipeAdapter<TcpClient>(streamResourceMock.Object);

            // Act
            var actual = await pipeAdapter.ReadAsync(10, CancellationToken.None);

            // Assert
            Assert.Equal(10, actual.Length);
            streamMock.Verify(x => x.ReadAsync(It.IsAny<Memory<byte>>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(250)]
        [Trait("Category", "Unit")]
        [SuppressMessage("Style", "IDE0017:Simplify object initialization", Justification = "To better separate Act part")]
        public void SetsReadTimeoutSuccessfully(int timeout)
        {
            // Arrange
            var streamResourceMock = new Mock<IStreamResource<TcpClient>>();
            streamResourceMock.Setup(x => x.GetStream()).Returns(new MemoryStream());
            var pipeAdapter = new PipeAdapter<TcpClient>(streamResourceMock.Object);

            // Act
            pipeAdapter.ReadTimeout = timeout;

            // Assert
            Assert.Equal(timeout, pipeAdapter.ReadTimeout);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(250)]
        [Trait("Category", "Unit")]
        [SuppressMessage("Style", "IDE0017:Simplify object initialization", Justification = "To better separate Act part")]
        public void SetsWriteTimeoutSuccessfully(int timeout)
        {
            // Arrange
            var streamResourceMock = new Mock<IStreamResource<TcpClient>>();
            streamResourceMock.Setup(x => x.GetStream()).Returns(new MemoryStream());
            var pipeAdapter = new PipeAdapter<TcpClient>(streamResourceMock.Object);

            // Act
            pipeAdapter.WriteTimeout = timeout;

            // Assert
            Assert.Equal(timeout, pipeAdapter.WriteTimeout);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void DisposesStreamResource()
        {
            // Arrange
            var streamResourceMock = new Mock<IStreamResource<TcpClient>>();
            streamResourceMock.Setup(x => x.GetStream()).Returns(new MemoryStream());
            var pipeAdapter = new PipeAdapter<TcpClient>(streamResourceMock.Object);

            // Act
            pipeAdapter.Dispose();

            // Assert
            streamResourceMock.Verify(x => x.Dispose(), Times.Once());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ExposesUnderlyingResource()
        {
            // Arrange
            var tcpClient = new TcpClient();
            var streamResourceMock = new Mock<IStreamResource<TcpClient>>();
            streamResourceMock.Setup(x => x.GetStream()).Returns(new MemoryStream());
            streamResourceMock.Setup(x => x.Resource).Returns(tcpClient);
            var pipeAdapter = new PipeAdapter<TcpClient>(streamResourceMock.Object);

            // Act
            var actual = pipeAdapter.Resource;

            // Assert
            Assert.Equal(tcpClient, actual);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task TimesOutOnWriteAsync()
        {
            // Arrange
            var streamMock = new Mock<Stream>() { CallBase = false };
            streamMock
                .Setup(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()))
                .Returns(new ValueTask(Task.Delay(1000)));

            var streamResourceMock = new Mock<IStreamResource<TcpClient>>();
            streamResourceMock.Setup(x => x.GetStream()).Returns(streamMock.Object);
            var pipeAdapter = new PipeAdapter<TcpClient>(streamResourceMock.Object)
            {
                WriteTimeout = 500
            };

            // Act/Assert
            await Assert.ThrowsAsync<TimeoutException>(() => pipeAdapter.WriteAsync(new byte[] { 1, 2, 3 }, CancellationToken.None));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task TimesOutOnReadAsync()
        {
            // Arrange
            var streamMock = new Mock<Stream>() { CallBase = false };
            streamMock
                .Setup(x => x.ReadAsync(It.IsAny<Memory<byte>>(), It.IsAny<CancellationToken>()))
                .Returns(async () =>
                {
                    await Task.Delay(1000);
                    return 1;
                });

            var streamResourceMock = new Mock<IStreamResource<TcpClient>>();
            streamResourceMock.Setup(x => x.GetStream()).Returns(streamMock.Object);
            var pipeAdapter = new PipeAdapter<TcpClient>(streamResourceMock.Object)
            {
                ReadTimeout = 500
            };

            // Act/Assert
            await Assert.ThrowsAsync<TimeoutException>(() => pipeAdapter.ReadAsync(10, CancellationToken.None));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ThrowsIfCompletedBeforeEnoughData()
        {
            // Arrange
            var streamMock = new Mock<Stream> { CallBase = false };
            streamMock
                .SetupSequence(x => x.ReadAsync(It.IsAny<Memory<byte>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(3)
                .ReturnsAsync(3)
                .ReturnsAsync(0);
            var streamResourceMock = new Mock<IStreamResource<TcpClient>>();
            streamResourceMock.Setup(x => x.GetStream()).Returns(streamMock.Object);
            var pipeAdapter = new PipeAdapter<TcpClient>(streamResourceMock.Object);

            // Act / Assert
            await Assert.ThrowsAsync<PipeReaderCompleteException>(() => pipeAdapter.ReadAsync(10, CancellationToken.None));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ThrowsIfCancelledByTokenBeforeEnoughData()
        {
            // Arrange
            using var cts = new CancellationTokenSource();
            var streamMock = new Mock<Stream> { CallBase = false };
            streamMock
                .SetupSequence(x => x.ReadAsync(It.IsAny<Memory<byte>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(3)
                .ReturnsAsync(3)
                .ReturnsAsync(() =>
                {
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                    return 0;
                })
                .ReturnsAsync(1);
            var streamResourceMock = new Mock<IStreamResource<TcpClient>>();
            streamResourceMock.Setup(x => x.GetStream()).Returns(streamMock.Object);
            var pipeAdapter = new PipeAdapter<TcpClient>(streamResourceMock.Object);

            // Act / Assert
            var actual = await Assert.ThrowsAsync<OperationCanceledException>(() => pipeAdapter.ReadAsync(10, cts.Token));
            Assert.Equal(cts.Token, actual.CancellationToken);
        }
    }
}