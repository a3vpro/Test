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
    /// Defines an interface for observing changes to a value.
    /// </summary>
    public interface IObservableValue
    {
        /// <summary>
        /// Occurs when the value of the observable object changes.
        /// </summary>
        event EventHandler<ValueChangedEventArgs<object>> ValueChanged;
    }

}
