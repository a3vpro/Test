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
using VisionNet.Core.Abstractions;
using VisionNet.Core.Events;

namespace VisionNet.Core.Patterns
{
    /// <summary>
    /// Specifies a Connectable observable instance
    /// </summary>
    public interface IStartableObservable
    {
        /// <summary>
        /// This event is raised when connection status is changed
        /// </summary>
        event EventHandler<EventArgs<ServiceStatus>> StatusChanged;
    }
}
