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
    /// Specifies a execution observable instance
    /// </summary>
    public interface IExecutionObservable<T, S>
    {
        /// <summary>
        /// Event callback for the successful end of the execution
        /// </summary>
        event EventHandler<EventArgs<T, S>> ExecutionCompleted;

        /// <summary>
        /// Event callback for the error
        /// </summary>
        event EventHandler<ErrorEventArgs> ExecutionError;
    }
}
