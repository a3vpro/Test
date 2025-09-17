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
using VisionNet.Core.Monitoring;
using VisionNet.Core.SafeObjects;

namespace VisionNet.Core.Patterns
{
    /// <summary>
    /// Defines an extended interface for observing and managing changes to a value, with additional functionality 
    /// for safe operations and tracking changes from different callers.
    /// </summary>
    public interface IObservableValueExtended : IObservableValue, ISafeObject<TypeCode>
    {
        /// <summary>
        /// Attempts to set a new value for the observable object.
        /// Optionally forces the update and allows tracking changes with a caller identifier.
        /// </summary>
        /// <param name="value">The new value to set.</param>
        /// <param name="forceUpdate">Indicates whether to force the update even if the value has not changed. Default is false.</param>
        /// <param name="callerIdentifier">An optional identifier for the caller initiating the change. Default is null.</param>
        /// <param name="description">An optional description of the change. Default is an empty string.</param>
        /// <returns>True if the value was successfully set, otherwise false.</returns>
        bool TrySetValue(object value, bool forceUpdate = false, object callerIdentifier = null, string description = "");

        /// <summary>
        /// Gets the information about the caller that last changed the value.
        /// </summary>
        /// <value>
        /// A <see cref="CallerInformation"/> object containing details about the caller.
        /// </value>
        CallerInformation CallerInformation { get; }

        /// <summary>
        /// Determines whether the value has been changed by a specific caller.
        /// </summary>
        /// <param name="callerIdentifier">The identifier of the caller to check.</param>
        /// <returns>True if the value has been changed by the specified caller, otherwise false.</returns>
        bool IsChanged(object callerIdentifier);
    }

}
