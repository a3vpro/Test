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
namespace VisionNet.Core.Abstractions
{
    /// <summary>
    /// Specifies a startable instance
    /// </summary>
    public interface IStartable
    {
        /// <summary>
        /// The state of execution
        /// </summary>
        ServiceStatus Status { get; }

        /// <summary>
        /// Perform the start command
        /// </summary>
        void Start();

        /// <summary>
        /// Perform the stop command
        /// </summary>
        void Stop();
    }
}
