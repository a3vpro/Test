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
using System.Diagnostics;
using VisionNet.Core.Patterns;

namespace VisionNet.Core.Monitoring
{
    /// <summary>
    /// The FacadeStopwatch class provides a simplified interface for commonly used methods of the <see cref="Stopwatch"/> class.
    /// It implements the <see cref="IReadonlyStopwatch"/> and <see cref="IDecorator{Stopwatch}"/> interfaces and follows the Facade design pattern.
    /// </summary>
    public class FacadeStopwatch : IReadonlyStopwatch, IDecorator<Stopwatch>
    {
        /// <summary>
        /// Gets the underlying <see cref="Stopwatch"/> instance that this class is wrapping.
        /// </summary>
        public Stopwatch Target { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FacadeStopwatch"/> class.
        /// If no target stopwatch is provided, a new <see cref="Stopwatch"/> instance is created.
        /// </summary>
        /// <param name="target">The target <see cref="Stopwatch"/> instance to be wrapped by this facade.</param>
        /// <returns>A new instance of the <see cref="FacadeStopwatch"/> class.</returns>
        public FacadeStopwatch(Stopwatch target = null)
        {
            Target = target ?? new Stopwatch();
        }

        /// <summary>
        /// Gets the elapsed time of the stopwatch.
        /// </summary>
        public TimeSpan Elapsed => Target.Elapsed;

        /// <summary>
        /// Gets the elapsed time in milliseconds.
        /// </summary>
        public long ElapsedMilliseconds => Target.ElapsedMilliseconds;

        /// <summary>
        /// Gets the elapsed time in ticks.
        /// </summary>
        public long ElapsedTicks => Target.ElapsedTicks;

        /// <summary>
        /// Gets a value indicating whether the stopwatch is currently running.
        /// </summary>
        public bool IsRunning => Target.IsRunning;

        /// <summary>
        /// Resets the stopwatch to its initial state.
        /// </summary>
        public void Reset() => Target.Reset();

        /// <summary>
        /// Restarts the stopwatch, resetting its elapsed time to zero.
        /// </summary>
        public void Restart() => Target.Restart();

        /// <summary>
        /// Starts the stopwatch if it is not already running.
        /// </summary>
        public void Start() => Target.Start();

        /// <summary>
        /// Stops the stopwatch if it is currently running.
        /// </summary>
        public void Stop() => Target.Stop();
    }
}
