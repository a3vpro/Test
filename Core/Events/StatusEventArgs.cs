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

namespace VisionNet.Core.Events
{
    /// <summary>
    /// A generical EventArgs instance using generics
    /// </summary>
    /// <typeparam name="T">The type of arguments</typeparam>
    /// <typeparam name="S">The type of the status</typeparam>
    public class EventArgs<T, S> : EventArgs
    {
        /// <summary>
        /// The value of the EventArgs
        /// </summary>
        public T Value { get; private set; }
        /// <summary>
        /// The status of the value
        /// </summary>
        public S Status { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventArgs"/> class with the specified value and status.
        /// </summary>
        /// <param name="value">The value associated with the event.</param>
        /// <param name="status">The status associated with the event.</param>
        public EventArgs(T value, S status)
        {
            Value = value;
            Status = status;
        }
    }
}
