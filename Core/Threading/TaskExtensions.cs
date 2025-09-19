using System;
using System.Threading;
using System.Threading.Tasks;

namespace VisionNet.Core.Threading
{
    public static class TaskExtensions
    {
        internal struct VoidTypeStruct { }  // See Footnote #1

        /// <summary>
        /// Creates a proxy task that completes when the source task finishes or faults with a <see cref="TimeoutException"/> when the timeout interval elapses.
        /// </summary>
        /// <param name="task">The task to observe for completion; must not be <see langword="null"/>.</param>
        /// <param name="millisecondsTimeout">The timeout in milliseconds; use <see cref="Timeout.Infinite"/> to wait indefinitely or <c>0</c> to time out immediately.</param>
        /// <returns>A task that transitions to the same final state (including cancellation) as <paramref name="task"/> when it completes before the timeout, or faults with <see cref="TimeoutException"/> when the timeout is exceeded.</returns>
        /// <exception cref="TimeoutException">The returned task faults with this exception when <paramref name="task"/> does not complete within <paramref name="millisecondsTimeout"/>.</exception>
        public static Task TimeoutAfter(this Task task, int millisecondsTimeout)
        {
            // Short-circuit #1: infinite timeout or task already completed
            if (task.IsCompleted || (millisecondsTimeout == Timeout.Infinite))
            {
                // Either the task has already completed or timeout will never occur.
                // No proxy necessary.
                return task;
            }

            // tcs.Task will be returned as a proxy to the caller
            TaskCompletionSource<VoidTypeStruct> tcs =
                new TaskCompletionSource<VoidTypeStruct>();

            // Short-circuit #2: zero timeout
            if (millisecondsTimeout == 0)
            {
                // We've already timed out.
                tcs.SetException(new TimeoutException());
                return tcs.Task;
            }

            // Set up a timer to complete after the specified timeout period
            Timer timer = new Timer(state =>
            {
                // Recover your state information
                var myTcs = (TaskCompletionSource<VoidTypeStruct>)state;

                // Fault our proxy with a TimeoutException
                myTcs.TrySetException(new TimeoutException());
            }, tcs, millisecondsTimeout, Timeout.Infinite);

            // Wire up the logic for what happens when source task completes
            task.ContinueWith((antecedent, state) =>
            {
                // Recover our state data
                var tuple =
                    (Tuple<Timer, TaskCompletionSource<VoidTypeStruct>>)state;

                // Cancel the Timer
                tuple.Item1.Dispose();

                // Marshal results to proxy
                MarshalTaskResults(antecedent, tuple.Item2);
            },
            Tuple.Create(timer, tcs),
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);

            return tcs.Task;
        }

        internal static void MarshalTaskResults<TResult>(
            Task source, TaskCompletionSource<TResult> proxy)
        {
            switch (source.Status)
            {
                case TaskStatus.Faulted:
                    proxy.TrySetException(source.Exception);
                    break;
                case TaskStatus.Canceled:
                    proxy.TrySetCanceled();
                    break;
                case TaskStatus.RanToCompletion:
                    Task<TResult> castedSource = source as Task<TResult>;
                    proxy.TrySetResult(
                        castedSource == null ? default(TResult) : // source is a Task
                            castedSource.Result); // source is a Task<TResult>
                    break;
            }
        }
    }
}
