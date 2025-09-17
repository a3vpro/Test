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
    public class CheckTask : IStartable, IExceptionObservable
    {
        private CancellationTokenSource _cancellation;
        private Task _task;
        private object _lock = new object();
        private int _taskThreadId;

        public bool Condition { get; set; }

        public CheckState CheckStatus { get; set; } = CheckState.Initial;

        public int DelayMs { get; set; } = 20;
        public bool AccuracyMode { get; set; } = false;
        public long TimeOutMs { get; set; } = 1000;

        public ThreadPriority Priority { get; set; } = ThreadPriority.Normal;


        #region IStartable
        public ServiceStatus Status { get; protected set; } = ServiceStatus.Stopped;
        
        /// <summary> The Start function starts the service.</summary>
        /// <returns> A task object that represents the asynchronous operation.</returns>
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

        /// <summary> The Stop function stops the service.</summary>
        /// <returns> A task object</returns>
        public void Stop()
        {
            if (Status == ServiceStatus.Started)
            {
                Status = ServiceStatus.Stopped;

                _cancellation?.Cancel();
            }
        }
        #endregion
        
        /// <summary> The Wait function waits for the task to complete execution.
        /// This function can only be called from a different thread than the one that started the task.</summary>
        /// <returns> The taskstatus of the task.</returns>
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
        
        /// <summary> The CheckTask function is used to check the status of a task.
        /// &lt;para&gt;The CheckTask function takes three parameters: timeOutMs, delayMs, and accuracyMode.&lt;/para&gt;
        /// &lt;list type=&quot;bullet&quot;&gt;
        /// &lt;item&gt;&lt;description&gt;timeOutMs - The amount of time in milliseconds that the task should be allowed to run before it is considered timed out.&lt;/description&gt;&lt;/item&gt;
        /// &lt;item&gt;&lt;description&gt;delayMs - The amount of time in milliseconds between each check for whether or not the task has completed.&lt;/description&gt;&lt;/item&gt; 
        /// &lt;/list&gt;</summary>
        /// <param name="timeOutMs"> The timeout in milliseconds</param>
        /// <param name="delayMs"> The delay between each check in milliseconds</param>
        /// <param name="accuracyMode"> If true, the task will wait for the specified time out period before returning false. 
        /// if false, it will return as soon as possible after checking and finding that the condition is not met. 
        /// </param>
        /// <returns> True if the task is completed, otherwise false.</returns>
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

        /// <summary> The RaiseCheck function is a helper function that raises the Check event.
        /// It also catches any exceptions thrown by the event handler and passes them to RaiseExceptionNotification.</summary>
        /// <param name="sender"> </param>
        /// <param name="e"> What is this parameter used for?</param>
        /// <returns> The check event.</returns>
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
        public event EventHandler<ErrorEventArgs> ExceptionRaised;

        #endregion
        /// <summary> The StartNew function is a function that takes in a Func&lt;bool&gt; action, long timeOutMs = 0, int delayMs = 20, bool accuracyMode = false.
        /// The StartNew function returns an object of type CheckTask. 
        /// The StartNew function creates a new instance of the CheckTask class and assigns it to the variable checkTask. 
        /// Then it adds an event handler for the Check event on checkTask which sets e.Condition to whatever value is returned by invoking action (which was passed into this method as an argument). 
        /// Finally, it calls Start() on checkTask.</summary>
        /// <param name="action"> The action to be executed.</param>
        /// <param name="timeOutMs"> The time out in milliseconds. if the condition is not met within this time, the task will be cancelled.</param>
        /// <param name="delayMs"> Delay between each check in milliseconds</param>
        /// <param name="accuracyMode"> If true, the task will be executed every &lt;paramref name=&quot;delayms&quot;/&gt; milliseconds.
        /// otherwise it will be executed after &lt;paramref name=&quot;delayms&quot;/&gt; milliseconds.
        /// </param>
        /// <returns> A &lt;see cref=&quot;checktask&quot;/&gt; object.</returns>
        public static CheckTask StartNew(Func<bool> action, long timeOutMs = 0, int delayMs = 20, bool accuracyMode = false)
        {
            var checkTask = new CheckTask(timeOutMs, delayMs, accuracyMode);
            checkTask.Check += (s, e) => e.Condition = action?.Invoke() ?? false;
            checkTask.Start();
            return checkTask;
        }
    }
}
