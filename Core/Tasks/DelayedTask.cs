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
using System.Diagnostics;
using System.Linq;
using VisionNet.Core.Exceptions;

namespace VisionNet.Core.Tasks
{
    /// <summary>
    /// Represents a task that executes an action after a specified delay.
    /// Provides functionality to start, stop, and wait for the task, and handle exceptions.
    /// Implements <see cref="IStartable"/> for starting and stopping the task, and <see cref="IExceptionObservable"/> for exception notifications.
    /// </summary>
    public class DelayedTask : IStartable, IExceptionObservable
    {
        #region Fields

        private CancellationTokenSource _cancellation;
        private Task _task;
        private int _taskThreadId;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the delay in milliseconds before the task is executed.
        /// Default is 20 milliseconds.
        /// </summary>
        public int DelayMs { get; set; } = 20;

        /// <summary>
        /// Gets or sets the thread priority of the task.
        /// Default is <see cref="ThreadPriority.Normal"/>.
        /// </summary>
        public ThreadPriority Priority { get; set; } = ThreadPriority.Normal;

        #region IStartable

        /// <summary>
        /// Gets the current status of the service.
        /// </summary>
        public ServiceStatus Status { get; protected set; } = ServiceStatus.Stopped;

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DelayedTask"/> class with a specified delay.
        /// </summary>
        /// <param name="delayMs">The delay in milliseconds before the task executes. Default is 20 milliseconds.</param>
        public DelayedTask(int delayMs = 20)
        {
            DelayMs = delayMs;
        }

        #endregion

        #region Public Methods

        #region IStartable

        /// <summary>
        /// Starts the delayed task. If the task is not already running, it will be started with the specified delay.
        /// </summary>
        public void Start()
        {
            if (Status == ServiceStatus.Stopped)
            {
                Status = ServiceStatus.Started;

                _cancellation = new CancellationTokenSource();
                _task = Task.Factory.StartNew(() =>
                        _execute(_cancellation, DelayMs),
                    _cancellation.Token,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default);
            }
        }

        /// <summary>
        /// Stops the delayed task if it is running.
        /// </summary>
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
        /// Waits for the task to complete, ensuring it is not being waited on by the same thread that initiated the task.
        /// If the current thread is different from the thread that started the task, it waits for the task to finish.
        /// Throws an exception if the task is awaited on the same thread that started it.
        /// </summary>
        public void Wait()
        {
            if (_taskThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                try
                {
                    _task.Wait();
                }
                catch (AggregateException ae)
                {
                    if (!ae.InnerExceptions.All(e => e is TaskCanceledException))
                        throw;
                }
            }
            else
            {
                var ex = new InvalidOperationException($"An exception occurred in the {nameof(DelayedTask)} class. Waiting for the task completion on the same thread is not allowed.");
                _raiseExceptionNotification(this, new ErrorEventArgs(ex));
            }
        }

        /// <summary>
        /// A wrapper for creating and starting a new <see cref="DelayedTask"/> instance.
        /// </summary>
        /// <param name="action">The action to be executed after the delay.</param>
        /// <param name="delayMs">The delay in milliseconds before the action is executed. Default is 20 milliseconds.</param>
        /// <returns>A new instance of <see cref="DelayedTask"/> that starts immediately.</returns>
        public static DelayedTask StartNew(Action<CancellationTokenSource> action, int delayMs = 20)
        {
            var delayTask = new DelayedTask(delayMs);
            delayTask.Action += (s, e) => action?.Invoke(e.Value);
            delayTask.Start();
            return delayTask;
        }

        #endregion

        #region Private Methods

        private void _execute(CancellationTokenSource tokenSource, int delayMs = 1)
        {
            _taskThreadId = Thread.CurrentThread.ManagedThreadId;
            if (Priority != ThreadPriority.Normal)
                Thread.CurrentThread.Priority = Priority;

            if (!tokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    if (delayMs > 0)
                    {
                        Stopwatch sw = new Stopwatch();
                        sw.Start();

                        while (sw.ElapsedMilliseconds < delayMs)
                        {
                            int newDelay = (int)System.Math.Ceiling(((double)delayMs - (double)sw.ElapsedMilliseconds) / 2);
                            if (newDelay > 0)
                                tokenSource.Token.WaitHandle.WaitOne(newDelay);
                        }
                        sw.Stop();
                    }

                    _raiseAction(this, new EventArgs<CancellationTokenSource>(tokenSource));
                }
                catch (Exception ex)
                {
                    _raiseExceptionNotification(this, new ErrorEventArgs(ex));
                }
            }

            Status = ServiceStatus.Stopped;
            Action = null;
        }

        #endregion

        #region Events

        private void _raiseAction(object sender, EventArgs<CancellationTokenSource> e)
        {
            try
            {
                Action?.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                _raiseExceptionNotification(this, new ErrorEventArgs(ex));
            }
        }

        /// <summary>
        /// Occurs when the delayed task action is executed.
        /// </summary>
        public event EventHandler<EventArgs<CancellationTokenSource>> Action;

        #region IExceptionObservable

        /// <summary>
        /// Raises an exception notification event if an exception occurs during task execution.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The event arguments containing the exception data.</param>
        protected void _raiseExceptionNotification(object sender, ErrorEventArgs eventArgs)
        {
            try
            {
                ExceptionRaised?.Invoke(sender, eventArgs);
            }
            catch (Exception ex)
            {
                ex.LogToConsole(nameof(_raiseExceptionNotification));
            }
        }

        /// <summary>
        /// Occurs when an exception is raised during task execution.
        /// </summary>
        public event EventHandler<ErrorEventArgs> ExceptionRaised;

        #endregion

        #endregion
    }
}
