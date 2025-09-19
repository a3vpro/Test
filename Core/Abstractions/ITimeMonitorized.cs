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

namespace VisionNet.Core.Abstractions
{
    /// <summary>
    /// Defines a contract for objects that are monitored in terms of time, tracking the duration of the last acquisition and the last update timestamp.
    /// </summary>
    public interface ITimeMonitorized
    {
        /// <summary>
        /// Gets the duration of the last acquisition. If it's the first acquisition, the value is 0.
        /// </summary>
        TimeSpan LastDuration { get; }

        /// <summary>
        /// Gets the timestamp of the last update.
        /// </summary>
        DateTime LastUpdate { get; }
    }

}
