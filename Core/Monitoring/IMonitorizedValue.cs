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
using VisionNet.Core.Patterns;
using VisionNet.Core.Abstractions;

namespace VisionNet.Core.Monitoring
{
    /// <summary>
    /// Represents a value that can be monitored, with extended functionalities for delayed updates, notifications, and state management.
    /// </summary>
    /// <remarks>
    /// This interface combines multiple aspects, including the ability to delay setting values, manage descriptive data, monitor state over time,
    /// handle exceptions, and observe changes. Implementations of this interface should be able to set values with a delay and update observers accordingly.
    /// </remarks>
    public interface IMonitorizedValue : IEntity<string>, INamed, IDescriptible, IObservableValueExtended, IDisableable, ITimeMonitorized, IExceptionObservable
    {
        /// <summary>
        /// Attempts to set a value with a specified delay.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <param name="delayMs">The delay in milliseconds before setting the value.</param>
        /// <param name="forceUpdate">A boolean indicating whether the value should be updated even if it's the same as the current value.</param>
        /// <param name="callerIdentifier">An optional identifier for the caller initiating the update. Used to track or differentiate callers.</param>
        /// <param name="description">An optional description associated with the value update. Can provide context or additional information.</param>
        /// <returns>
        /// <c>true</c> if the value was successfully set, <c>false</c> otherwise.
        /// </returns>
        bool TrySetValueDelayed(object value, int delayMs, bool forceUpdate = false, object callerIdentifier = null, string description = "");
    }

}
