using System;

namespace NModbusAsync.IO.Abstractions
{
    internal interface ICrcCalculator
    {
        ushort Calculate(ReadOnlySpan<byte> data);

        ushort Calculate(ReadOnlyMemory<byte> data);
    }
}