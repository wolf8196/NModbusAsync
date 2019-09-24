using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NModbusAsync.Utility;

namespace NModbusAsync.IO
{
    internal class TcpClientAdapter<TResource> : IStreamResource<TResource> where TResource : TcpClient
    {
        private TcpClient tcpClient;

        internal TcpClientAdapter(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient ?? throw new ArgumentNullException(nameof(tcpClient));
        }

        public int ReadTimeout
        {
            get => tcpClient.GetStream().ReadTimeout;
            set => tcpClient.GetStream().ReadTimeout = value;
        }

        public int WriteTimeout
        {
            get => tcpClient.GetStream().WriteTimeout;
            set => tcpClient.GetStream().WriteTimeout = value;
        }

        public TResource UnderlyingResource => tcpClient as TResource;

        public Task WriteAsync(byte[] buffer, int offset, int count)
        {
            return WriteAsync(buffer, offset, count, default);
        }

        public Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken token)
        {
            // Move back to sync I/O as there is no timeout in NetworkStream
            return Task.Run(() => tcpClient.GetStream().Write(buffer, offset, count), token);
        }

        public Task<int> ReadAsync(byte[] buffer, int offset, int count)
        {
            return ReadAsync(buffer, offset, count, default);
        }

        public Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken token)
        {
            // Move back to sync I/O as there is no timeout in NetworkStream
            return Task.Run(() => tcpClient.GetStream().Read(buffer, offset, count), token);
        }

        public Task FlushAsync()
        {
            return FlushAsync(default);
        }

        public Task FlushAsync(CancellationToken token)
        {
            return tcpClient.GetStream().FlushAsync(token);
        }

        public void Dispose()
        {
            DisposableUtility.Dispose(ref tcpClient);
        }
    }
}