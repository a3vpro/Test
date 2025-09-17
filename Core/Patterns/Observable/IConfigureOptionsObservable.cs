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
using System.IO;
using VisionNet.Core.Events;

namespace VisionNet.Core.Patterns
{
    /// <summary>
    /// Specifies a ConfigureOptions observable instance
    /// </summary>
    public interface IConfigureOptionsObservable<T>
    {
        /// <summary>
        /// Event callback for the successful end of the configuration
        /// </summary>
        event EventHandler<EventArgs<T>> Configured;

        /// <summary>
        /// Event callback for the error in configuration
        /// </summary>
        event EventHandler<ErrorEventArgs> ConfiguredError;
    }
}
