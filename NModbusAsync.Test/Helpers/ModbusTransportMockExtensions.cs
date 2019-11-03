using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using NModbusAsync.IO;

namespace NModbusAsync.Test.Helpers
{
    [ExcludeFromCodeCoverage]
    internal static class ModbusTransportMockExtensions
    {
        internal static void SetupWriteRequestAsync(this Mock<ModbusTransport> mock, params IModbusRequest[] requests)
        {
            mock.Protected()
                .As<IModbusTransportMock>()
                .Setup(x => x.WriteRequestAsync(requests[0], CancellationToken.None))
                .Returns(Task.CompletedTask);
        }

        internal static void SetupThrowsWriteRequestAsync(this Mock<ModbusTransport> mock, Exception ex)
        {
            mock.Protected()
                .As<IModbusTransportMock>()
                .Setup(x => x.WriteRequestAsync(It.IsAny<IModbusRequest>(), CancellationToken.None))
                .Throws(ex);
        }

        internal static void SetupReadResponseAsync<TResponse>(this Mock<ModbusTransport> mock, params IModbusResponse[] responses) where TResponse : IModbusResponse, new()
        {
            if (responses.Length == 1)
            {
                mock.Protected()
                    .As<IModbusTransportMock>()
                    .Setup(x => x.ReadResponseAsync<TResponse>(CancellationToken.None))
                    .ReturnsAsync(responses[0]);
                return;
            }

            var sequenceMock = mock.Protected()
                .As<IModbusTransportMock>()
                .SetupSequence(x => x.ReadResponseAsync<TResponse>(CancellationToken.None));

            foreach (var item in responses)
            {
                sequenceMock
                    .ReturnsAsync(item);
            }
        }

        internal static void SetupRetryReadResponse(this Mock<ModbusTransport> mock, IModbusRequest request, IModbusResponse response, params bool[] results)
        {
            if (results.Length == 1)
            {
                mock.Protected()
                    .As<IModbusTransportMock>()
                    .Setup(x => x.RetryReadResponse(request, response))
                    .Returns(results[0]);
                return;
            }

            var sequenceMock = mock.Protected()
                .As<IModbusTransportMock>()
                .SetupSequence(x => x.RetryReadResponse(request, response));

            foreach (var item in results)
            {
                sequenceMock
                    .Returns(item);
            }
        }

        internal static void SetupValidate(this Mock<ModbusTransport> mock, IModbusRequest request, IModbusResponse response)
        {
            mock.Protected().As<IModbusTransportMock>().Setup(x => x.Validate(request, response));
        }
    }
}