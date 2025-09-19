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
using System.Diagnostics;

namespace VisionNet.Core.Monitoring
{
    /// <summary>
    /// Provides extension methods for the <see cref="Stopwatch"/> class.
    /// </summary>
    public static class StopwatchExtensions
    {
        /// <summary>
        /// Converts a <see cref="Stopwatch"/> instance to a <see cref="IReadonlyStopwatch"/> instance.
        /// This is useful when you want to pass a stopwatch around but prevent the recipient from modifying it.
        /// </summary>
        /// <param name="sw">The <see cref="Stopwatch"/> instance to be converted.</param>
        /// <returns>A new <see cref="IReadonlyStopwatch"/> instance wrapping the provided <see cref="Stopwatch"/>.</returns>
        public static IReadonlyStopwatch ToReadOnly(this Stopwatch sw)
        {
            return new FacadeStopwatch(sw);
        }
    }
}

