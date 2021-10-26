using System.IO;
using System.Net.Sockets;
using NModbusAsync.IO.Abstractions;

namespace NModbusAsync.IO
{
    internal sealed class TcpClientAdapter<TResource> : IStreamResource<TResource>
        where TResource : TcpClient
    {
        internal TcpClientAdapter(TResource tcpClient)
        {
            Resource = tcpClient;
        }

        public TResource Resource { get; }

        public Stream GetStream()
        {
            return Resource.GetStream();
        }

        public void Dispose()
        {
            Resource.Dispose();
        }
    }
}