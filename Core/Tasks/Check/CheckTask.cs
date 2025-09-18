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
using System.Diagnostics;
using System.IO;
using VisionNet.Core.Events;
using VisionNet.Core.Patterns;
using System.Linq;
using VisionNet.Core.Exceptions;

namespace VisionNet.Core.Tasks
{
    /// <summary>
    /// Provides a long-running service that polls for a condition until it is satisfied, canceled, or times out.
    /// </summary>
    /// <remarks>
    /// The task raises <see cref="Check"/> repeatedly on a background thread until the supplied condition succeeds or the
    /// configured timeout expires. Consumers can subscribe to <see cref="ExceptionRaised"/> to be notified of unhandled
    /// exceptions occurring during polling.
    /// </remarks>
    public class CheckTask : IStartable, IExceptionObservable
    {
        private CancellationTokenSource _cancellation;
        private Task _task;
        private object _lock = new object();
        private int _taskThreadId;

        /// <summary>
        /// Gets a value indicating whether the most recent execution of the check succeeded.
        /// </summary>
        public bool Condition { get; set; }

        /// <summary>
        /// Gets or sets the current status of the polling operation, including initial, running, canceled, timeout, or completed states.
        /// </summary>
        public CheckState CheckStatus { get; set; } = CheckState.Initial;

        /// <summary>
        /// Gets or sets the interval, in milliseconds, between successive condition evaluations when accuracy mode is disabled.
        /// </summary>
        public int DelayMs { get; set; } = 20;
        /// <summary>
        /// Gets or sets a value indicating whether the task should actively wait for precise timing rather than sleeping between checks.
        /// </summary>
        public bool AccuracyMode { get; set; } = false;
        /// <summary>
        /// Gets or sets the maximum duration, in milliseconds, to continue polling before reporting a timeout.
        /// </summary>
        /// <remarks>
        /// Values less than or equal to zero cause the timeout condition to be met after the first evaluation cycle.
        /// </remarks>
        public long TimeOutMs { get; set; } = 1000;

        /// <summary>
        /// Gets or sets the priority assigned to the worker thread that performs the polling loop.
        /// </summary>
        public ThreadPriority Priority { get; set; } = ThreadPriority.Normal;


        #region IStartable
        /// <summary>
        /// Gets the current lifecycle state of the service.
        /// </summary>
        public ServiceStatus Status { get; protected set; } = ServiceStatus.Stopped;

        /// <summary>
        /// Starts the polling loop on a dedicated worker thread if the task is not already running.
        /// </summary>
        /// <remarks>
        /// The loop executes until <see cref="Condition"/> becomes <see langword="true"/>, <see cref="Stop"/> is invoked, or the
        /// elapsed time exceeds <see cref="TimeOutMs"/>. The initial delay before the first iteration is <see cref="DelayMs"/>
        /// when accuracy mode is disabled. When accuracy mode is enabled, the loop busy-waits to maintain the specified interval.
        /// </remarks>
        public void Start()
        {
            if (Status == ServiceStatus.Stopped)
            {
                Status = ServiceStatus.Started;

                _cancellation = new CancellationTokenSource();
                _task = Task.Factory.StartNew(() =>
                        _execute(_cancellation, TimeOutMs, DelayMs),
                    _cancellation.Token,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default);
            }
        }

        /// <summary>
        /// Requests cancellation of the polling loop and transitions the service to the stopped state.
        /// </summary>
        /// <remarks>
        /// The background thread exits promptly after the current iteration completes. Subsequent calls to <see cref="Start"/>
        /// can be used to restart the service.
        /// </remarks>
        public void Stop()
        {
            if (Status == ServiceStatus.Started)
            {
                Status = ServiceStatus.Stopped;

                _cancellation?.Cancel();
            }
        }
        #endregion
        
        /// <summary>
        /// Blocks the calling thread until the background polling task completes.
        /// </summary>
        /// <remarks>
        /// This method must be called from a thread other than the worker thread that performs the polling loop. If invoked on the
        /// worker thread, the method raises <see cref="ExceptionRaised"/> with an <see cref="InvalidOperationException"/>.
        /// </remarks>
        /// <exception cref="AggregateException">
        /// Thrown when the background task faults with one or more exceptions other than <see cref="TaskCanceledException"/>.
        /// </exception>
        public void Wait()
        {
            // Verifica si el hilo actual es el mismo que inició la tarea.
            //if (_task.Status == TaskStatus.Running)
            if (_taskThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                // Si no es el mismo hilo, puedes esperar de forma segura la tarea.
                try
                {
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
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckTask"/> class with the specified timing settings.
        /// </summary>
        /// <param name="timeOutMs">The maximum duration, in milliseconds, to continue polling before reporting a timeout. Values less than or equal to zero cause the timeout condition to be met after the first evaluation cycle.</param>
        /// <param name="delayMs">The delay, in milliseconds, between evaluations when accuracy mode is disabled.</param>
        /// <param name="accuracyMode">Indicates whether to perform busy waiting for precise timing (<see langword="true"/>) or use timed waits (<see langword="false"/>).</param>
        public CheckTask(long timeOutMs = 0, int delayMs = 20, bool accuracyMode = false)
        {
            TimeOutMs = timeOutMs;
            DelayMs = delayMs;
            AccuracyMode = accuracyMode;
        }

        private void _execute(CancellationTokenSource tokenSource, long timeOutMs = -1, int delayMs = 1, bool accuracyMode = false)
        {
            try
            {
                _taskThreadId = Thread.CurrentThread.ManagedThreadId;
                CheckStatus = CheckState.Checking;
                Condition = false;
                if (Priority != ThreadPriority.Normal)
                    Thread.CurrentThread.Priority = Priority;

                delayMs = delayMs < 0 ? 0 : delayMs;

                var stopwatch = new Stopwatch();
                var accuracyModeStopwatch = new Stopwatch();
                stopwatch.Start();
                bool continueLoop = true;
                do
                {
                    try
                    {
                        ConditionEventArgs eventArgs = new ConditionEventArgs();

                        RaiseCheck(this, eventArgs);

                        if (eventArgs.Condition)
                        {
                            CheckStatus = CheckState.Checked;
                            Condition = true;
                            continueLoop = false;
                        }
                        else
                        {
                            if (tokenSource.Token.IsCancellationRequested)
                            {
                                CheckStatus = CheckState.Canceled;
                                continueLoop = false;
                            }
                            else if (stopwatch.Elapsed.TotalMilliseconds > timeOutMs)
                            {
                                CheckStatus = CheckState.TimeOut;
                                continueLoop = false;
                            }
                            else if (!accuracyMode)
                                tokenSource.Token.WaitHandle.WaitOne(delayMs);
                            else
                            {
                                accuracyModeStopwatch.Reset();
                                accuracyModeStopwatch.Start();
                                while (accuracyModeStopwatch.ElapsedMilliseconds < delayMs)
                                    accuracyModeStopwatch.Stop();
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        RaiseExceptionNotification(this, new ErrorEventArgs(ex));
                    }
                } while (continueLoop);

                Status = ServiceStatus.Stopped;
                Check = null;
            }
            catch (Exception ex)
            {
                RaiseExceptionNotification(this, new ErrorEventArgs(ex));
            }
        }

        /// <summary>
        /// Invokes the <see cref="Check"/> event and routes handler exceptions through <see cref="RaiseExceptionNotification"/>.
        /// </summary>
        /// <param name="sender">The source of the invocation.</param>
        /// <param name="e">The event data containing the condition state.</param>
        private void RaiseCheck(object sender, ConditionEventArgs e)
        {
            try
            {
                Check?.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                RaiseExceptionNotification(this, new ErrorEventArgs(ex));
            }
        }
        /// <summary>
        /// Occurs repeatedly while the task is running to evaluate whether the condition has been satisfied.
        /// </summary>
        public event EventHandler<ConditionEventArgs> Check;

        #region IExceptionObservable
        /// <summary>
        /// Raises an exception notification event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The event arguments containing the exception data.</param>
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
        /// Occurs when the task captures an exception that should be surfaced to subscribers.
        /// </summary>
        public event EventHandler<ErrorEventArgs> ExceptionRaised;

        #endregion
        /// <summary>
        /// Creates, configures, and starts a new <see cref="CheckTask"/> that polls a delegate until it returns <see langword="true"/>.
        /// </summary>
        /// <param name="action">The delegate that evaluates the condition. Returning <see langword="true"/> ends the polling loop.</param>
        /// <param name="timeOutMs">The maximum polling duration, in milliseconds, before the task reports a timeout. Values less than or equal to zero cause the timeout condition to be met after the first evaluation cycle.</param>
        /// <param name="delayMs">The delay, in milliseconds, between evaluations when accuracy mode is disabled.</param>
        /// <param name="accuracyMode">Indicates whether to perform busy waiting for precise timing (<see langword="true"/>) or use timed waits (<see langword="false"/>).</param>
        /// <returns>The started <see cref="CheckTask"/> instance configured to run the supplied action.</returns>
        public static CheckTask StartNew(Func<bool> action, long timeOutMs = 0, int delayMs = 20, bool accuracyMode = false)
        {
            var checkTask = new CheckTask(timeOutMs, delayMs, accuracyMode);
            checkTask.Check += (s, e) => e.Condition = action?.Invoke() ?? false;
            checkTask.Start();
            return checkTask;
        }
    }
}
