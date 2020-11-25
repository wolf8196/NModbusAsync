using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using NModbusAsync.IO;
using NModbusAsync.Messages;
using NModbusAsync.Test.Helpers;
using Xunit;

namespace NModbusAsync.Test.Unit
{
    [ExcludeFromCodeCoverage]
    public class ModbusTransportTest
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task RetriesOnSlaveExceptionCodeAcknowledge()
        {
            // Arrange
            var target = new Mock<ModbusTransport>(new Mock<IPipeResource>().Object, Mock.Of<IModbusLogger>(), Mock.Of<ITransactionIdProvider>()) { CallBase = true };
            var request = new ReadHoldingRegistersRequest(1, 1, 1);
            var response = ModbusResponseFactory.CreateResponse<ReadHoldingRegistersResponse>(
                new byte[] { 1, 3, 2, 0, 1 });
            var acknowledgeResponse = ModbusResponseFactory.CreateResponse<SlaveExceptionResponse>(
                new byte[] { 1, 129, (byte)SlaveExceptionCode.Acknowledge });

            target.SetupWriteRequestAsync(request);
            target.SetupReadResponseAsync<ReadHoldingRegistersResponse>(acknowledgeResponse, acknowledgeResponse, response);

            // Act
            var actual = await target.Object.SendAsync<ReadHoldingRegistersResponse>(request);

            // Assert
            Assert.Equal(response, actual);

            target.Protected().As<IModbusTransportMock>()
                .Verify(x => x.WriteRequestAsync(request, It.IsAny<CancellationToken>()), Times.Once());
            target.Protected().As<IModbusTransportMock>()
                .Verify(x => x.ReadResponseAsync<ReadHoldingRegistersResponse>(It.IsAny<CancellationToken>()), Times.Exactly(3));
            target.Protected().As<IModbusTransportMock>()
                .Verify(x => x.RetryReadResponse(request, response), Times.Once());
            target.Protected().As<IModbusTransportMock>()
                .Verify(x => x.Validate(request, response), Times.Once());
        }

        [Theory]
        [InlineData(SlaveExceptionCode.IllegalFunction)]
        [InlineData(SlaveExceptionCode.IllegalDataAddress)]
        [InlineData(SlaveExceptionCode.IllegalDataValue)]
        [InlineData(SlaveExceptionCode.SlaveDeviceFailure)]
        [InlineData(SlaveExceptionCode.MemoryParityError)]
        [InlineData(SlaveExceptionCode.GatewayPathUnavailable)]
        [InlineData(SlaveExceptionCode.GatewayTargetDeviceFailedToRespond)]
        [Trait("Category", "Unit")]
        public async Task ThrowsOnSlaveExceptionResponse(SlaveExceptionCode exceptionCode)
        {
            // Arrange
            var target = new Mock<ModbusTransport>(new Mock<IPipeResource>().Object, Mock.Of<IModbusLogger>(), Mock.Of<ITransactionIdProvider>()) { CallBase = true };
            var request = new ReadHoldingRegistersRequest(1, 1, 1);
            var response = ModbusResponseFactory.CreateResponse<SlaveExceptionResponse>(
                new byte[] { 1, 129, (byte)exceptionCode });

            target.SetupWriteRequestAsync(request);
            target.SetupReadResponseAsync<ReadHoldingRegistersResponse>(response);

            // Act
            var ex = await Assert.ThrowsAsync<SlaveException>(() => target.Object.SendAsync<ReadHoldingRegistersResponse>(request));

            // Assert
            Assert.Equal(1, ex.SlaveAddress);
            Assert.Equal(129, ex.FunctionCode);
            Assert.Equal(exceptionCode, ex.SlaveExceptionCode);

            target.Protected().As<IModbusTransportMock>()
                .Verify(x => x.WriteRequestAsync(request, It.IsAny<CancellationToken>()), Times.Once());
            target.Protected().As<IModbusTransportMock>()
                .Verify(x => x.ReadResponseAsync<ReadHoldingRegistersResponse>(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task RetriesOnSlaveExceptionCodeSlaveDeviceBusyIgnoringRetryCount()
        {
            // Arrange
            var target = new Mock<ModbusTransport>(new Mock<IPipeResource>().Object, Mock.Of<IModbusLogger>(), Mock.Of<ITransactionIdProvider>()) { CallBase = true };
            target.Object.Retries = 3;
            target.Object.SlaveBusyUsesRetryCount = false;
            var request = new ReadHoldingRegistersRequest(1, 1, 1);
            var response = ModbusResponseFactory.CreateResponse<ReadHoldingRegistersResponse>(
                new byte[] { 1, 3, 2, 0, 1 });
            var busyResponse = ModbusResponseFactory.CreateResponse<SlaveExceptionResponse>(
                new byte[] { 1, 129, (byte)SlaveExceptionCode.SlaveDeviceBusy });

            target.SetupWriteRequestAsync(request);
            target.SetupReadResponseAsync<ReadHoldingRegistersResponse>(busyResponse, busyResponse, busyResponse, busyResponse, response);

            // Act
            var actual = await target.Object.SendAsync<ReadHoldingRegistersResponse>(request);

            // Assert
            Assert.Equal(response, actual);

            target.Protected().As<IModbusTransportMock>()
                .Verify(x => x.WriteRequestAsync(request, It.IsAny<CancellationToken>()), Times.Exactly(5));
            target.Protected().As<IModbusTransportMock>()
                .Verify(x => x.ReadResponseAsync<ReadHoldingRegistersResponse>(It.IsAny<CancellationToken>()), Times.Exactly(5));
            target.Protected().As<IModbusTransportMock>()
                .Verify(x => x.RetryReadResponse(request, response), Times.Once());
            target.Protected().As<IModbusTransportMock>()
                .Verify(x => x.Validate(request, response), Times.Once());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ThrowsOnSlaveExceptionCodeSlaveDeviceBusyWhenExceedesRetryCount()
        {
            // Arrange
            var target = new Mock<ModbusTransport>(new Mock<IPipeResource>().Object, Mock.Of<IModbusLogger>(), Mock.Of<ITransactionIdProvider>()) { CallBase = true };
            target.Object.Retries = 2;
            target.Object.SlaveBusyUsesRetryCount = true;
            var request = new ReadHoldingRegistersRequest(1, 1, 1);
            var busyResponse = ModbusResponseFactory.CreateResponse<SlaveExceptionResponse>(
                new byte[] { 1, 129, (byte)SlaveExceptionCode.SlaveDeviceBusy });

            target.SetupWriteRequestAsync(request);
            target.SetupReadResponseAsync<ReadHoldingRegistersResponse>(busyResponse);

            // Act
            var ex = await Assert.ThrowsAsync<SlaveException>(() => target.Object.SendAsync<ReadHoldingRegistersResponse>(request));

            // Assert
            Assert.Equal(1, ex.SlaveAddress);
            Assert.Equal(129, ex.FunctionCode);
            Assert.Equal(SlaveExceptionCode.SlaveDeviceBusy, ex.SlaveExceptionCode);

            target.Protected().As<IModbusTransportMock>()
                .Verify(x => x.WriteRequestAsync(request, It.IsAny<CancellationToken>()), Times.Exactly(3));
            target.Protected().As<IModbusTransportMock>()
                .Verify(x => x.ReadResponseAsync<ReadHoldingRegistersResponse>(It.IsAny<CancellationToken>()), Times.Exactly(3));
        }

        [Theory]
        [MemberData(nameof(GetSocketExceptions))]
        [Trait("Category", "Unit")]
        public async Task ThrowsOnSocketExceptions(Exception exception)
        {
            // Arrange
            var target = new Mock<ModbusTransport>(new Mock<IPipeResource>().Object, Mock.Of<IModbusLogger>(), Mock.Of<ITransactionIdProvider>()) { CallBase = true };
            target.SetupThrowsWriteRequestAsync(exception);

            // Act
            var ex = await Assert.ThrowsAnyAsync<Exception>(() => target.Object.SendAsync<ReadHoldingRegistersResponse>(
                new ReadHoldingRegistersRequest(1, 1, 1)));

            // Assert
            Assert.Equal(exception, ex);

            target.Protected().As<IModbusTransportMock>()
                .Verify(x => x.WriteRequestAsync(It.IsAny<IModbusRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [Theory]
        [MemberData(nameof(GetRetriableExceptions))]
        [Trait("Category", "Unit")]
        public async Task RetriesOnSpecificExceptions(Exception exception)
        {
            // Arrange
            var target = new Mock<ModbusTransport>(new Mock<IPipeResource>().Object, Mock.Of<IModbusLogger>(), Mock.Of<ITransactionIdProvider>()) { CallBase = true };
            target.Object.Retries = 2;

            target.SetupThrowsWriteRequestAsync(exception);

            // Act
            var ex = await Assert.ThrowsAnyAsync<Exception>(
                () => target.Object.SendAsync<ReadHoldingRegistersResponse>(
                    new ReadHoldingRegistersRequest(1, 1, 1)));

            // Assert
            Assert.Equal(exception, ex);

            target.Protected().As<IModbusTransportMock>()
                .Verify(x => x.WriteRequestAsync(It.IsAny<IModbusRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ThrowsOnUnexpectedException()
        {
            // Arrange
            var target = new Mock<ModbusTransport>(new Mock<IPipeResource>().Object, Mock.Of<IModbusLogger>(), Mock.Of<ITransactionIdProvider>()) { CallBase = true };
            var exception = new Exception();
            target.Object.Retries = 2;

            target.SetupThrowsWriteRequestAsync(exception);

            // Act
            var ex = await Assert.ThrowsAnyAsync<Exception>(() => target.Object.SendAsync<ReadHoldingRegistersResponse>(
                new ReadHoldingRegistersRequest(1, 1, 1)));

            // Assert
            Assert.Equal(exception, ex);

            target.Protected().As<IModbusTransportMock>()
                .Verify(x => x.WriteRequestAsync(It.IsAny<IModbusRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task RetriesToReadResponse()
        {
            // Arrange
            var target = new Mock<ModbusTransport>(new Mock<IPipeResource>().Object, Mock.Of<IModbusLogger>(), Mock.Of<ITransactionIdProvider>()) { CallBase = true };
            var request = new WriteMultipleCoilsRequest(1, 1, new bool[] { true });
            var response = new WriteMultipleCoilsResponse();

            target.SetupWriteRequestAsync(request);
            target.SetupReadResponseAsync<WriteMultipleCoilsResponse>(response);
            target.SetupRetryReadResponse(request, response, true, false);

            // Act
            var _ = await target.Object.SendAsync<WriteMultipleCoilsResponse>(request);

            // Assert
            target.Protected().As<IModbusTransportMock>()
                .Verify(x => x.WriteRequestAsync(request, It.IsAny<CancellationToken>()), Times.Exactly(1));
            target.Protected().As<IModbusTransportMock>()
                .Verify(x => x.ReadResponseAsync<WriteMultipleCoilsResponse>(It.IsAny<CancellationToken>()), Times.Exactly(2));
            target.Protected().As<IModbusTransportMock>()
                .Verify(x => x.RetryReadResponse(request, It.IsAny<IModbusResponse>()), Times.Exactly(2));
            target.Protected().As<IModbusTransportMock>()
                .Verify(x => x.Validate(request, response), Times.Once());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void DisposesPipeResource()
        {
            // Arrange
            var pipeResource = new Mock<IPipeResource>();
            pipeResource.Setup(x => x.Dispose());
            var target = new Mock<ModbusTransport>(pipeResource.Object, Mock.Of<IModbusLogger>(), Mock.Of<ITransactionIdProvider>()) { CallBase = true };

            // Act
            target.Object.Dispose();

            // Assert
            pipeResource.Verify(x => x.Dispose(), Times.Once());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void SetsReadTimeout()
        {
            // Arrange
            var pipeAdapterMock = new Mock<IPipeResource>();
            var target = new Mock<ModbusTransport>(pipeAdapterMock.Object, Mock.Of<IModbusLogger>(), Mock.Of<ITransactionIdProvider>()) { CallBase = true };

            target.Object.ReadTimeout = 1000;

            pipeAdapterMock.VerifySet(x => x.ReadTimeout = 1000, Times.Once());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void GetsReadTimeout()
        {
            // Arrange
            var pipeAdapterMock = new Mock<IPipeResource>();
            pipeAdapterMock.Setup(x => x.ReadTimeout).Returns(1000);
            var target = new Mock<ModbusTransport>(pipeAdapterMock.Object, Mock.Of<IModbusLogger>(), Mock.Of<ITransactionIdProvider>()) { CallBase = true };

            Assert.Equal(1000, target.Object.ReadTimeout);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void SetsWriteTimeout()
        {
            // Arrange
            var pipeAdapterMock = new Mock<IPipeResource>();
            var target = new Mock<ModbusTransport>(pipeAdapterMock.Object, Mock.Of<IModbusLogger>(), Mock.Of<ITransactionIdProvider>()) { CallBase = true };

            target.Object.WriteTimeout = 1000;

            pipeAdapterMock.VerifySet(x => x.WriteTimeout = 1000, Times.Once());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void GetsWriteTimeout()
        {
            // Arrange
            var pipeAdapterMock = new Mock<IPipeResource>();
            pipeAdapterMock.Setup(x => x.WriteTimeout).Returns(1000);
            var target = new Mock<ModbusTransport>(pipeAdapterMock.Object, Mock.Of<IModbusLogger>(), Mock.Of<ITransactionIdProvider>()) { CallBase = true };

            Assert.Equal(1000, target.Object.WriteTimeout);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void SetsWaitToRetryMilliseconds()
        {
            // Arrange
            var target = new Mock<ModbusTransport>(new Mock<IPipeResource>().Object, Mock.Of<IModbusLogger>(), Mock.Of<ITransactionIdProvider>()) { CallBase = true };

            target.Object.WaitToRetryMilliseconds = 1000;

            Assert.Equal(1000, target.Object.WaitToRetryMilliseconds);
        }

        public static TheoryData<Exception> GetSocketExceptions() => new TheoryData<Exception>
        {
            new SocketException(),
            new Exception("Test", new SocketException())
        };

        public static TheoryData<Exception> GetRetriableExceptions() => new TheoryData<Exception>
        {
            new FormatException(),
            new IOException(),
            new TimeoutException(),
        };
    }
}