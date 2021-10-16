using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace NModbusAsync.Utility
{
    internal static class ReadOnlySequenceExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ReadOnlySpan<byte> ToSpan(this ReadOnlySequence<byte> buffer)
        {
            if (buffer.IsSingleSegment)
            {
                return buffer.First.Span;
            }

            return buffer.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ReadOnlyMemory<byte> ToMemory(this ReadOnlySequence<byte> buffer)
        {
            if (buffer.IsSingleSegment)
            {
                return buffer.First;
            }

            return buffer.ToArray();
        }
    }
}