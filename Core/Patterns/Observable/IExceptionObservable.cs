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

namespace VisionNet.Core.Patterns
{
    /// <summary>
    /// Specifies a exception raiser observable instance
    /// </summary>
    public interface IExceptionObservable
    {
        /// <summary>
        /// This event is raised when an exception is raised
        /// </summary>
        event EventHandler<ErrorEventArgs> ExceptionRaised;
    }
}
