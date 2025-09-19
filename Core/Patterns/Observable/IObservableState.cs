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
using VisionNet.Core.Events;

namespace VisionNet.Core.Patterns
{
    /// <summary>
    /// Defines an interface for observing state changes.
    /// </summary>
    /// <typeparam name="TState">The type representing the states.</typeparam>
    public interface IObservableState<TState>
    {
        /// <summary>
        /// Gets the current state of the observable object.
        /// </summary>
        /// <value>
        /// The current value of the state of type <typeparamref name="TState"/>.
        /// </value>
        TState State { get; }

        /// <summary>
        /// Occurs when the state of the object changes.
        /// </summary>
        event EventHandler<StateChangedEventArgs<TState>> StateChanged;
    }
}

