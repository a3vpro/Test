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

namespace VisionNet.Core.Abstractions
{
    /// <summary>
    /// Specifies a reproductible instance
    /// </summary>
    public interface IPlayer
    {
        /// <summary>
        /// The state of reproduction
        /// </summary>
        ReproductionStatus Status { get; }

        /// <summary>
        /// Perform the play command
        /// </summary>
        void Play();

        /// <summary>
        /// Perform the stop command
        /// </summary>
        void Stop();

        /// <summary>
        /// This event is raised when reproduction status is changed
        /// </summary>
        event EventHandler<EventArgs<ReproductionStatus>> StatusChanged;
    }
}
