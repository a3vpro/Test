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
    public class LoopTask : IStartable, IExceptionObservable
    {
        private CancellationTokenSource _cancellation;
        private Task _task;
        private int _taskThreadId;

        public long MaxIterations { get; set; } = 0;

        public int DelayMs { get; set; } = 20;

        public ThreadPriority Priority { get; set; } = ThreadPriority.Normal;

        #region IStartable
        public ServiceStatus Status { get; protected set; } = ServiceStatus.Stopped;

        
        /// <summary> The Start function starts the service.</summary>
        /// <returns> A task object.</returns>
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

        
        /// <summary> The Stop function stops the service.</summary>
        /// <returns> The service status</returns>
        public void Stop()
        {
            if (Status == ServiceStatus.Started)
            {
                Status = ServiceStatus.Stopped;

                _cancellation?.Cancel();
            }
        }

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

        
        /// <summary> The LoopTask function is a task that runs in the background and loops until it is cancelled.
        /// It can be used to run tasks on a regular interval, or for an indefinite amount of time.</summary>
        /// <param name="maxIterations"> The number of iterations to perform.</param>
        /// <param name="delayMs"> The delay in milliseconds between iterations.</param>
        /// <returns> A task object.</returns>
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
        public event EventHandler<EventArgs<CancellationTokenSource>> Action;

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

        
        /// <summary> The StartNew function is a static function that creates a new LoopTask object and starts it.
        /// The action parameter is an Action delegate that takes in one argument, which is the CancellationTokenSource of the loop task.
        /// The maxIterations parameter specifies how many times to run the loop before stopping it automatically. If this value is 0, then there will be no limit on how many times to run the loop.</summary>
        /// <param name="action"> The action to be performed in the loop.</param>
        /// <param name="maxIterations"> The maximum number of iterations to run. if 0, the loop will continue until cancelled.</param>
        /// <param name="delayMs"> The delay in milliseconds between each iteration of the loop.</param>
        /// <returns> An object of type looptask</returns>
        public static LoopTask StartNew(Action<CancellationTokenSource> action, long maxIterations = 0, int delayMs = 20)
        { 
            var loopTask = new LoopTask(maxIterations, delayMs);
            loopTask.Action += (s, e) => action?.Invoke(e.Value);
            loopTask.Start();
            return loopTask;
        }
    }
}
