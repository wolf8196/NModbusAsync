using System;
using System.Threading;
using System.Threading.Tasks;

namespace NModbusAsync.Utility
{
    internal static class TaskExtensions
    {
        internal static async Task<bool> WaitAsync(this Task task, int milliseconds, CancellationToken token = default)
        {
            var timeoutTask = Task.Delay(milliseconds, token);

            var completedTask = await Task.WhenAny(task, timeoutTask).ConfigureAwait(false);

            if (completedTask.Status == TaskStatus.Faulted)
            {
                throw completedTask.Exception;
            }

            if (completedTask.Status == TaskStatus.Canceled)
            {
                throw new OperationCanceledException();
            }

            return completedTask == task;
        }
    }
}