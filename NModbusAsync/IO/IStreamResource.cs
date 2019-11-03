using System;
using System.IO;

namespace NModbusAsync.IO
{
    internal interface IStreamResource<out TResource> : IDisposable
    {
        TResource Resource { get; }

        Stream GetStream();
    }
}