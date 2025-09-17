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
using VisionNet.Core.Patterns;
using System.IO;
using System.Collections.Generic;
using VisionNet.Core.Monitoring;
using System.Linq;
using VisionNet.Core.Exceptions;

namespace VisionNet.Core.Tasks
{
    public class SequenceTask : IStartable, IExceptionObservable
    {
        private CancellationTokenSource _cancellation;
        private Task _task;
        private int _taskThreadId;
        protected List<SequenceItem> _sequences = new List<SequenceItem>();
        private long _accumulatedDelay = 0;

        public long MaxIterations { get; set; } = 0;

        public IReadonlyStopwatch Stopwatch { get; protected set; }

        public ThreadPriority Priority { get; set; } = ThreadPriority.Normal;


        #region IStartable
        public ServiceStatus Status { get; protected set; } = ServiceStatus.Stopped;

        
        /// <summary> The Start function starts the VisionNet service.</summary>
        /// <returns> The task that is created.</returns>
        public void Start()
        {
            Start(null);
        }

        /// <summary> The Start function starts the service.</summary>
        /// <param name="stopwatch">     the stopwatch to use for timing the service. if null, a new facadestopwatch will be created and used.
        /// </param>
        /// <returns> A task that is started and running. </returns>
        public void Start(IReadonlyStopwatch stopwatch)
        {
            if (Status == ServiceStatus.Stopped && _sequences.Count() > 0)
            {
                Status = ServiceStatus.Started;

                FacadeStopwatch sw = null;
                Stopwatch = stopwatch ?? (sw = new FacadeStopwatch());

                _cancellation = new CancellationTokenSource();
                _task = Task.Factory.StartNew(() =>
                    {
                        sw?.Start();
                        _execute(_cancellation, MaxIterations);
                    },
                    _cancellation.Token,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default);
            }
        }

        
        /// <summary> The Stop function stops the service.</summary>
        /// <returns> The status of the service.</returns>
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
            if (_taskThreadId != Thread.CurrentThread.ManagedThreadId &&
                _taskThreadId != 0)
            {
                // Si no es el mismo hilo, puedes esperar de forma segura la tarea.
                try
                {
                    _task?.Wait();
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
                var ex = new InvalidOperationException($"Se produjo una excepción en la clase {nameof(SequenceTask)}. No es posible esperar la finalización de la tarea en el mismo hilo de ejecución.");
                RaiseExceptionNotification(this, new ErrorEventArgs(ex));
            }
        }
        #endregion

        protected SequenceTask()
        {
        }
        
        /// <summary> The SequenceTask function is a constructor that takes in a list of sequences and the maximum number of iterations.
        /// If the maxIterations parameter is not specified, it defaults to 0.</summary>
        /// <param name="sequences"> List of sequences to be executed</param>
        /// <param name="maxIterations"> The maximum number of iterations to run the sequence. 
        /// if 0, then it will run indefinitely.</param>
        /// <returns> A list of sequences which are the result of the execution</returns>
        public SequenceTask(List<SequenceItem> sequences, long maxIterations = 0)
        {
            //if (sequences == null || sequences.Count == 0)
            if (sequences == null)
                throw new ArgumentException($"The parameter {nameof(sequences)} is empty");

            _sequences = sequences;
            MaxIterations = maxIterations;
        }

        private void _execute(CancellationTokenSource tokenSource, long maxIteration = -1)
        {
            _taskThreadId = Thread.CurrentThread.ManagedThreadId;
            if (Priority != ThreadPriority.Normal)
                Thread.CurrentThread.Priority = Priority;

            long iterationCounter = 0;
            long sequenceCounter = 0;
            bool mustCheckCounter = maxIteration > 0;
            _accumulatedDelay = 0;
            while (!tokenSource.Token.IsCancellationRequested && !(mustCheckCounter && iterationCounter >= maxIteration))
            {
                try
                {
                    int sequenceIndex = (int)(sequenceCounter % _sequences.Count);
                    var currentSequence = _sequences[sequenceIndex];

                    var delayMs = currentSequence.DelayMs < 0 ? 0 : currentSequence.DelayMs;
                    _accumulatedDelay += delayMs;
                    var currentDelay = (int)(_accumulatedDelay - Stopwatch.Elapsed.TotalMilliseconds);
                    currentDelay = currentDelay < 0 ? 0 : currentDelay;

                    if (!tokenSource.IsCancellationRequested && !(mustCheckCounter && iterationCounter >= maxIteration) && (currentDelay > 0))
                        tokenSource.Token.WaitHandle.WaitOne(currentDelay);

                    RaiseAction(currentSequence, tokenSource);

                    sequenceCounter++;
                    if (sequenceIndex == _sequences.Count - 1)
                        iterationCounter++;
                }
                catch (Exception ex)
                {
                    RaiseExceptionNotification(this, new ErrorEventArgs(ex));
                }
            }

            Status = ServiceStatus.Stopped;
        }

        
        /// <summary> The RaiseAction function is a helper function that invokes the Action delegate of a SequenceItem object.
        /// The RaiseAction function also handles any exceptions thrown by the Action delegate.</summary>
        /// <param name="sequenceItem"> What is this used for?</param>
        /// <param name="cancellationTokenSource"> What is this used for?
        /// </param>
        /// <returns> A boolean value. if the action is not null, it returns true. otherwise, it returns false.</returns>
        private void RaiseAction(SequenceItem sequenceItem, CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                sequenceItem.Action?.Invoke(sequenceItem.Parameters, cancellationTokenSource);
            }
            catch (Exception ex)
            {
                RaiseExceptionNotification(this, new ErrorEventArgs(ex));
            }
        }

        #region IExceptionObservable
        
        /// <summary> The RaiseExceptionNotification function is used to raise an exception notification event.</summary>
        /// <param name="sender"> </param>
        /// <param name="eventArgs"> What is this parameter used for?</param>
        /// <returns> The exceptionraised event.</returns>
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

        
        /// <summary> The StartNew function starts a new sequence task.</summary>
        /// <param name="sequences"> The list of sequences to be executed.</param>
        /// <param name="maxIterations"> The maximum number of iterations to run the sequence for. if 0, it will run indefinitely.</param>
        /// <param name="stopwatch"> The stopwatch is used to measure the time of each sequence item. 
        /// if no stopwatch is provided, a new one will be created and started automatically. 
        /// </param>
        /// <returns> A &lt;see cref=&quot;sequencetask&quot;/&gt; object.</returns>
        public static SequenceTask StartNew(List<SequenceItem> sequences, long maxIterations = 0, IReadonlyStopwatch stopwatch = null)
        {
            var sequenceTask = new SequenceTask(sequences, maxIterations);
            sequenceTask.Start(stopwatch);
            return sequenceTask;
        }
    }
}
