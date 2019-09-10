using System;

namespace NModbusAsync.Utility
{
    internal static class DisposableUtility
    {
        internal static void Dispose<T>(ref T item) where T : class, IDisposable
        {
            if (item == null)
            {
                return;
            }

            item.Dispose();
            item = default;
        }
    }
}