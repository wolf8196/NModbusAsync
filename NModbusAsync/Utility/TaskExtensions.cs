using System;
using System.Threading;
using System.Threading.Tasks;

namespace NModbusAsync.Utility
{
    internal static class TaskExtensions
    {
        internal static async Task<bool> WaitAsync(this Task task, int milliseconds, CancellationToken token = default)
        {
            Task completedTask;

            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(token))
            {
                var timeoutTask = Task.Delay(milliseconds, cts.Token);
                completedTask = await Task.WhenAny(task, timeoutTask).ConfigureAwait(false);
                cts.Cancel();
            }

            if (completedTask.Status == TaskStatus.Faulted)
            {
                throw completedTask.Exception;
            }

            if (completedTask.Status == TaskStatus.Canceled)
            {
                throw new OperationCanceledException(token);
            }

            return completedTask == task;
        }
    }
}