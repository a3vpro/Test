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
    /// Defines an interface for observing progress changes of a specific state.
    /// </summary>
    /// <typeparam name="TState">The type representing the state of the progress.</typeparam>
    public interface IProgressStateObservable<TState>
    {
        /// <summary>
        /// Occurs when the progress of the observable object changes.
        /// </summary>
        event EventHandler<EventArgs<TState>> ProgressChanged;
    }

}
