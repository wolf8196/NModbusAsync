using System;
using System.Net;
using System.Runtime.InteropServices;

namespace NModbusAsync.Utility
{
    internal static class SpanExtensions
    {
        internal static ushort[] ToHostUInt16Array(this ReadOnlySpan<byte> networkBytes)
        {
            var result = MemoryMarshal.Cast<byte, ushort>(networkBytes).ToArray();

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (ushort)IPAddress.NetworkToHostOrder((short)result[i]);
            }

            return result;
        }
    }
}