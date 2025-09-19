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

namespace VisionNet.Core.Monitoring
{
    /// <summary>
    /// Defines a read-only stopwatch interface that provides the elapsed time measurements
    /// in terms of <see cref="TimeSpan"/>, milliseconds, and ticks. It also exposes whether
    /// the stopwatch is currently running or not. This interface is useful for scenarios 
    /// where you need to access timing information without being able to modify the stopwatch itself.
    /// </summary>
    public interface IReadonlyStopwatch
    {
        /// <summary>
        /// Gets the total elapsed time measured by the stopwatch.
        /// </summary>
        /// <value>
        /// A <see cref="TimeSpan"/> representing the elapsed time.
        /// </value>
        TimeSpan Elapsed { get; }

        /// <summary>
        /// Gets the total elapsed time in milliseconds.
        /// </summary>
        /// <value>
        /// The elapsed time, in milliseconds, as a <see cref="long"/> value.
        /// </value>
        long ElapsedMilliseconds { get; }

        /// <summary>
        /// Gets the total elapsed time in timer ticks.
        /// </summary>
        /// <value>
        /// The elapsed time in ticks, represented as a <see cref="long"/>.
        /// </value>
        long ElapsedTicks { get; }

        /// <summary>
        /// Gets a value indicating whether the stopwatch is currently running.
        /// </summary>
        /// <value>
        /// <c>true</c> if the stopwatch is running; otherwise, <c>false</c>.
        /// </value>
        bool IsRunning { get; }
    }

}
