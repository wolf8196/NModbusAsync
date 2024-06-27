using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NModbusAsync.IO;
using Xunit;

namespace NModbusAsync.Test.Integration
{
    [ExcludeFromCodeCoverage]
    public class PipeAdapterTest
    {
        [Fact]
        [Trait("Category", "Integration")]
        public async Task ReadsOnceIfEnoughData()
        {
            // Arrange
            using var cts = new CancellationTokenSource();
            var port = StartDevice(
                async (client, stream) =>
                {
                    await stream.WriteAsync(Enumerable.Range(1, 10).Select(x => (byte)x).ToArray());
                    client.Close();
                },
                cts.Token);

            var client = new TcpClient();
            client.Connect(IPAddress.Loopback, port);
            var adapter = new TcpClientAdapter<TcpClient>(client);
            var pipe = new PipeAdapter<TcpClient>(adapter);

            // Act
            var actual = await pipe.ReadAsync(10, CancellationToken.None);

            // Assert
            Assert.Equal(10, actual.Length);

            cts.Cancel();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task ReadsUntilEnoughData()
        {
            // Arrange
            using var cts = new CancellationTokenSource();
            var port = StartDevice(
                async (client, stream) =>
                {
                    await stream.WriteAsync(new byte[] { 1, 2, 3 });
                    await stream.WriteAsync(new byte[] { 4, 5, 6 });
                    await stream.WriteAsync(new byte[] { 7, 8, 9 });
                    await stream.WriteAsync(new byte[] { 10 });
                    client.Close();
                },
                cts.Token);

            var client = new TcpClient();
            client.Connect(IPAddress.Loopback, port);
            var adapter = new TcpClientAdapter<TcpClient>(client);
            var pipe = new PipeAdapter<TcpClient>(adapter);

            // Act
            var actual = await pipe.ReadAsync(10, CancellationToken.None);

            // Assert
            Assert.Equal(10, actual.Length);

            cts.Cancel();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task ThrowsIfSocketClosedWithoutSendingAnyData()
        {
            // Arrange
            using var cts = new CancellationTokenSource();
            var port = StartDevice(
                (client, stream) =>
                {
                    client.Close();
                    return Task.CompletedTask;
                },
                cts.Token);

            var client = new TcpClient();
            client.Connect(IPAddress.Loopback, port);
            var adapter = new TcpClientAdapter<TcpClient>(client);
            var pipe = new PipeAdapter<TcpClient>(adapter);

            // Act / Assert
            await Assert.ThrowsAsync<PipeReaderCompleteException>(() => pipe.ReadAsync(10, CancellationToken.None));

            cts.Cancel();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task ThrowsIfSocketClosedBeforeEnoughData()
        {
            // Arrange
            using var cts = new CancellationTokenSource();
            var port = StartDevice(
                async (client, stream) =>
                {
                    await stream.WriteAsync(new byte[] { 1, 2, 3 });

                    client.Close();
                },
                cts.Token);

            var client = new TcpClient();
            client.Connect(IPAddress.Loopback, port);
            var adapter = new TcpClientAdapter<TcpClient>(client);
            var pipe = new PipeAdapter<TcpClient>(adapter);

            // Act / Assert
            await Assert.ThrowsAsync<PipeReaderCompleteException>(() => pipe.ReadAsync(10, CancellationToken.None));

            cts.Cancel();
        }

        private static int StartDevice(Func<TcpClient, NetworkStream, Task> action, CancellationToken token)
        {
            var device = new TcpListener(IPAddress.Loopback, 0);
            device.Start();

            _ = Task.Run(
                async () =>
                {
                    try
                    {
                        while (!token.IsCancellationRequested)
                        {
                            var client = await device.AcceptTcpClientAsync();
                            var stream = client.GetStream();

                            await action(client, stream);
                        }
                    }
                    finally
                    {
                        device.Stop();
                    }
                },
                token);

            return ((IPEndPoint)device.LocalEndpoint).Port;
        }
    }
}