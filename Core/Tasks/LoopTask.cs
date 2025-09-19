//----------------------------------------------------------------------------
// Author           : Álvaro Ibáñez del Pino
// NickName         : aibanez
// Created          : 28-08-2021
//
// Last Modified By : aibanez
// Last Modified On : 18-11-2023
// Description      : v1.7.1
//
// Copyright        : (C)  2023 by Sothis/Nunsys. All rights reserved.       
//----------------------------------------------------------------------------
using System;
using VisionNet.Core.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using VisionNet.Core.Events;
using VisionNet.Core.Patterns;
using System.IO;
using System.Linq;
using VisionNet.Core.Exceptions;

namespace VisionNet.Core.Tasks
{
    /// <summary>
    /// Provides a cancellable loop that repeatedly executes the <see cref="Action"/> handler until cancellation or the
    /// configured iteration limit is reached, forwarding runtime errors through <see cref="ExceptionRaised"/>.
    /// </summary>
    /// <remarks>
    /// The loop increments an internal counter, invokes <see cref="Action"/>, and then waits for <see cref="DelayMs"/>
    /// milliseconds unless cancellation is requested. The execution stops when <see cref="MaxIterations"/> is reached or
    /// when <see cref="Stop"/> is called.
    /// </remarks>
    public class LoopTask : IStartable, IExceptionObservable
    {
        private CancellationTokenSource _cancellation;
        private Task _task;
        private int _taskThreadId;

        /// <summary>
        /// Gets or sets the maximum number of loop iterations to execute before the task stops automatically; use
        /// <c>0</c> (default) for an unbounded loop.
        /// </summary>
        public long MaxIterations { get; set; } = 0;

        /// <summary>
        /// Gets or sets the delay, in milliseconds, between iterations. Negative values are coerced to <c>0</c>
        /// before being applied.
        /// </summary>
        public int DelayMs { get; set; } = 20;

        /// <summary>
        /// Gets or sets the thread priority applied to the worker thread that runs the loop.
        /// </summary>
        public ThreadPriority Priority { get; set; } = ThreadPriority.Normal;

        #region IStartable
        /// <summary>
        /// Gets the lifecycle state of the loop task, allowing callers to determine whether execution is active.
        /// </summary>
        public ServiceStatus Status { get; protected set; } = ServiceStatus.Stopped;


        /// <summary>
        /// Transitions the loop task to the <see cref="ServiceStatus.Started"/> state and begins executing iterations if
        /// the task is currently stopped; further calls while running are ignored.
        /// </summary>
        /// <remarks>
        /// Exceptions thrown by iteration handlers are surfaced via <see cref="ExceptionRaised"/> and do not terminate the
        /// loop unless the observer cancels execution through the provided <see cref="CancellationTokenSource"/>.
        /// </remarks>
        public void Start()
        {
            if (Status == ServiceStatus.Stopped)
            {
                Status = ServiceStatus.Started;

                _cancellation = new CancellationTokenSource();
                _task = Task.Factory.StartNew(() =>
                        _execute(_cancellation, MaxIterations, DelayMs),
                    _cancellation.Token//,
                    //TaskCreationOptions.LongRunning,
                    //TaskScheduler.Default);
                    );
            }
        }


        /// <summary>
        /// Requests cancellation of the loop and transitions the task state to <see cref="ServiceStatus.Stopped"/> if it
        /// was running.
        /// </summary>
        /// <remarks>
        /// The loop completes the current iteration before respecting the cancellation request.
        /// </remarks>
        public void Stop()
        {
            if (Status == ServiceStatus.Started)
            {
                Status = ServiceStatus.Stopped;

                _cancellation?.Cancel();
            }
        }

        /// <summary>
        /// Blocks the current thread until the internal worker task completes, unless invoked from the same thread that
        /// runs the loop, in which case an <see cref="InvalidOperationException"/> is reported via
        /// <see cref="ExceptionRaised"/>.
        /// </summary>
        /// <remarks>
        /// If the worker faulted, waiting may rethrow the first non-cancellation exception encountered.
        /// </remarks>
        /// <exception cref="AggregateException">Thrown if the loop completed with an unhandled non-cancellation error.</exception>
        public void Wait()
        {
            // Verifica si el hilo actual es el mismo que inició la tarea.
            //if (_task.Status == TaskStatus.Running)
                if (_taskThreadId != Thread.CurrentThread.ManagedThreadId)
                {
                // Si no es el mismo hilo, puedes esperar de forma segura la tarea.
                try
                {
                    _task.Wait();
                }
                catch (AggregateException ae)
                {
                    // Verifica si todas las excepciones son TaskCanceledException
                    if (!ae.InnerExceptions.All(e => e is TaskCanceledException))
                        throw;
                }
            }
                else
                {
                    // No puedes esperar la tarea en el mismo hilo que la inició.
                    var ex = new InvalidOperationException($"Se produjo una excepción en la clase {nameof(LoopTask)}. No es posible esperar la finalización de la tarea en el mismo hilo de ejecución.");
                    RaiseExceptionNotification(this, new ErrorEventArgs(ex));
                }
        }
        #endregion

        
        /// <summary>
        /// Initializes a new instance of the <see cref="LoopTask"/> class with optional iteration and delay
        /// configuration.
        /// </summary>
        /// <param name="maxIterations">Maximum iterations to run before the loop stops automatically; <c>0</c> means no limit.</param>
        /// <param name="delayMs">Delay in milliseconds between iterations; negative values are treated as <c>0</c>.</param>
        public LoopTask(long maxIterations = 0, int delayMs = 20)
        {
            MaxIterations = maxIterations;
            DelayMs = delayMs;
        }

        private void _execute(CancellationTokenSource tokenSource, long maxIteration = -1, int delayMs = 1)
        {
            delayMs = delayMs < 0 ? 0 : delayMs;
            _taskThreadId = Thread.CurrentThread.ManagedThreadId;
            if (Priority != ThreadPriority.Normal)
               Thread.CurrentThread.Priority = Priority;

            long counter = 0;
            bool mustCheckCounter = maxIteration > 0;
            while (!tokenSource.Token.IsCancellationRequested && !(mustCheckCounter && counter >= maxIteration))
            {
                try
                {
                    counter++;

                    RaiseAction(this, new EventArgs<CancellationTokenSource>(tokenSource));

                    if (!tokenSource.IsCancellationRequested && !(mustCheckCounter && counter >= maxIteration))
                        tokenSource.Token.WaitHandle.WaitOne(delayMs);
                }
                catch (Exception ex)
                {
                    RaiseExceptionNotification(this, new ErrorEventArgs(ex));
                }
            }

            Status = ServiceStatus.Stopped;
            Action = null;
        }

        private void RaiseAction(object sender, EventArgs<CancellationTokenSource> e)
        {
            try
            {
                Action?.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                RaiseExceptionNotification(this, new ErrorEventArgs(ex));
            }
        }
        /// <summary>
        /// Occurs before each delay interval, providing observers access to the shared <see cref="CancellationTokenSource"/>
        /// so they can perform work and request cancellation when required; handler exceptions are reported via
        /// <see cref="ExceptionRaised"/>.
        /// </summary>
        public event EventHandler<EventArgs<CancellationTokenSource>> Action;

        #region IExceptionObservable
        /// <summary>
        /// Raises an exception notification event, forwarding operational faults such as iteration handler errors or invalid
        /// wait attempts to observers.
        /// </summary>
        /// <param name="sender">The originator of the exception.</param>
        /// <param name="eventArgs">The event arguments that encapsulate the exception instance.</param>
        protected void RaiseExceptionNotification(object sender, ErrorEventArgs eventArgs)
        {
            try
            {
                ExceptionRaised?.Invoke(sender, eventArgs);
            }
            catch (Exception ex)
            {
                ex.LogToConsole(nameof(RaiseExceptionNotification));
            }
        }

        /// <summary>
        /// Occurs when the loop encounters an exception, including handler failures and illegal wait operations, allowing
        /// subscribers to react or log errors.
        /// </summary>
        public event EventHandler<ErrorEventArgs> ExceptionRaised;
        #endregion


        /// <summary>
        /// Creates, configures, and starts a new <see cref="LoopTask"/> that invokes the specified action on each iteration
        /// until cancellation or the iteration limit is reached; any handler exceptions are surfaced via
        /// <see cref="ExceptionRaised"/>.
        /// </summary>
        /// <param name="action">Action executed on each iteration with access to the loop's <see cref="CancellationTokenSource"/>.</param>
        /// <param name="maxIterations">Maximum iterations to run before stopping automatically; <c>0</c> means no limit.</param>
        /// <param name="delayMs">Delay in milliseconds between iterations; negative values are coerced to <c>0</c>.</param>
        /// <returns>The started loop task instance.</returns>
        public static LoopTask StartNew(Action<CancellationTokenSource> action, long maxIterations = 0, int delayMs = 20)
        {
            var loopTask = new LoopTask(maxIterations, delayMs);
            loopTask.Action += (s, e) => action?.Invoke(e.Value);
            loopTask.Start();
            return loopTask;
        }
    }
}
