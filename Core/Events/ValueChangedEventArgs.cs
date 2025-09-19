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

namespace VisionNet.Core.Events
{
    /// <summary>Event arguments used when a value change event is raised.</summary>
    /// <remarks>
    /// <para>
    /// To avoid additional read operations you should use the properties passed in this class.
    /// </para>
    /// </remarks>
    public class ValueChangedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Information about the caller
        /// </summary>
        public CallerInformation CallerInformation { get; private set; }

        /// <summary>
        /// The previous value of the feature.
        /// </summary>
        public T PreviousValue { get; private set; }

        /// <summary>
        /// The current value of the feature.
        /// </summary>
        public T CurrentValue { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueChangedEventArgs{T}"/> class.
        /// </summary>
        /// <param name="oldValue">The value before the change.</param>
        /// <param name="newValue">The value after the change.</param>
        /// <param name="callerInformation">Information about the caller that triggered the event.</param>
        public ValueChangedEventArgs(T oldValue, T newValue, CallerInformation callerInformation) : base()
        {
            PreviousValue = oldValue;
            CurrentValue = newValue;
            CallerInformation = callerInformation;
        }

    }
}
