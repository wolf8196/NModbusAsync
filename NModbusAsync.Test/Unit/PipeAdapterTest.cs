using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NModbusAsync.IO;
using Xunit;

namespace NModbusAsync.Test.Unit
{
    [ExcludeFromCodeCoverage]
    public class PipeAdapterTest
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(250)]
        [Trait("Category", "Unit")]
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
            var streamMock = new StreamMock
            {
                WriteTimeout = 1000
            };
            var streamResourceMock = new Mock<IStreamResource<TcpClient>>();
            streamResourceMock.Setup(x => x.GetStream()).Returns(streamMock);
            var pipeAdapter = new PipeAdapter<TcpClient>(streamResourceMock.Object)
            {
                WriteTimeout = 500
            };

            // Act/Assert
            await Assert.ThrowsAsync<TimeoutException>(() => pipeAdapter.WriteAsync(new byte[] { 1, 2, 3 }, CancellationToken.None));
        }

        private class StreamMock : Stream
        {
            public override int WriteTimeout { get; set; }

            public override bool CanWrite => true;

            #region Not implemented

            public override bool CanRead => throw new System.NotImplementedException();

            public override bool CanSeek => throw new System.NotImplementedException();

            public override long Length => throw new System.NotImplementedException();

            public override long Position { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

            public override int Read(byte[] buffer, int offset, int count) => throw new System.NotImplementedException();

            public override long Seek(long offset, SeekOrigin origin) => throw new System.NotImplementedException();

            public override void SetLength(long value) => throw new System.NotImplementedException();

            #endregion Not implemented

            public override void Write(byte[] buffer, int offset, int count)
            {
                Thread.Sleep(WriteTimeout);
            }

            public override void Flush()
            {
            }
        }
    }
}