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
    /// <summary>
    /// Orchestrates ordered <see cref="SequenceItem"/> instances, honouring per-item delays until cancellation or the configured iteration limit is reached.
    /// Exceptions raised by sequence actions are published through the <see cref="ExceptionRaised"/> event.
    /// </summary>
    public class SequenceTask : IStartable, IExceptionObservable
    {
        private CancellationTokenSource _cancellation;
        private Task _task;
        private int _taskThreadId;
        protected List<SequenceItem> _sequences = new List<SequenceItem>();
        private long _accumulatedDelay = 0;

        /// <summary>
        /// Gets or sets the total number of full sequence cycles to execute before stopping automatically; a value of <c>0</c> allows the sequence to continue indefinitely.
        /// </summary>
        public long MaxIterations { get; set; } = 0;

        /// <summary>
        /// Gets the stopwatch that coordinates the timing for the running sequence.
        /// Returns <see langword="null"/> until the sequence has been started.
        /// </summary>
        public IReadonlyStopwatch Stopwatch { get; protected set; }

        /// <summary>
        /// Gets or sets the thread priority applied to the background worker that executes the sequence.
        /// </summary>
        public ThreadPriority Priority { get; set; } = ThreadPriority.Normal;


        #region IStartable
        /// <summary>
        /// Gets the lifecycle status of the sequence execution service.
        /// </summary>
        public ServiceStatus Status { get; protected set; } = ServiceStatus.Stopped;

        
        /// <summary>
        /// Starts the sequence execution service using an internally managed stopwatch.
        /// Exceptions encountered while executing sequence items are published via <see cref="ExceptionRaised"/>.
        /// </summary>
        public void Start()
        {
            Start(null);
        }

        /// <summary>
        /// Starts executing the configured sequence items, preserving their ordering and respecting configured delays.
        /// </summary>
        /// <param name="stopwatch">A stopwatch instance that synchronises execution timing; when <see langword="null"/>, a new stopwatch is created and started.</param>
        /// <remarks>Exceptions thrown by sequence actions are surfaced through the <see cref="ExceptionRaised"/> event.</remarks>
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

        
        /// <summary>
        /// Requests cancellation of the sequence execution service and transitions the status to <see cref="ServiceStatus.Stopped"/>.
        /// </summary>
        /// <remarks>The current sequence iteration completes before shutdown; any exceptions continue to be reported via <see cref="ExceptionRaised"/>.</remarks>
        public void Stop()
        {
            if (Status == ServiceStatus.Started)
            {
                Status = ServiceStatus.Stopped;
                _cancellation?.Cancel();
            }
        }

        /// <summary>
        /// Waits for the background execution thread to complete after cancellation or natural termination.
        /// </summary>
        /// <exception cref="AggregateException">Thrown when the underlying task faults with exceptions other than cancellation.</exception>
        /// <remarks>Attempting to wait from the executing thread triggers an <see cref="InvalidOperationException"/> published via <see cref="ExceptionRaised"/>.</remarks>
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
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceTask"/> class with the provided sequence items and optional iteration limit.
        /// </summary>
        /// <param name="sequences">The ordered collection of sequence items to execute sequentially, each with its configured delay and action.</param>
        /// <param name="maxIterations">The maximum number of times the entire sequence list is repeated before stopping automatically; specify <c>0</c> to repeat indefinitely.</param>
        /// <exception cref="ArgumentException"><paramref name="sequences"/> is <see langword="null"/>.</exception>
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
        
        /// <summary>
        /// Raises the <see cref="ExceptionRaised"/> event while protecting the notifier from observer exceptions.
        /// </summary>
        /// <param name="sender">The originator of the notification.</param>
        /// <param name="eventArgs">The event data describing the exception encountered.</param>
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
        /// Occurs when an exception happens during sequence execution or lifecycle operations.
        /// </summary>
        public event EventHandler<ErrorEventArgs> ExceptionRaised;
        #endregion

        
        /// <summary>
        /// Creates and immediately starts a new <see cref="SequenceTask"/> for the provided sequence items.
        /// </summary>
        /// <param name="sequences">The ordered collection of sequence items to execute sequentially.</param>
        /// <param name="maxIterations">The maximum number of completed sequence cycles before stopping automatically; use <c>0</c> to continue indefinitely.</param>
        /// <param name="stopwatch">An optional stopwatch shared with other services for timing; when omitted, a new internal stopwatch is created and started.</param>
        /// <returns>The started <see cref="SequenceTask"/> instance controlling the sequence execution.</returns>
        /// <exception cref="ArgumentException"><paramref name="sequences"/> is <see langword="null"/>.</exception>
        public static SequenceTask StartNew(List<SequenceItem> sequences, long maxIterations = 0, IReadonlyStopwatch stopwatch = null)
        {
            var sequenceTask = new SequenceTask(sequences, maxIterations);
            sequenceTask.Start(stopwatch);
            return sequenceTask;
        }
    }
}
