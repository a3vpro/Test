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
    /// State of the connection
    /// </summary>
    public enum ConnectionStatus
    {
        /// <summary>
        /// Initial state. Unprepared
        /// </summary>
        Initial,
        /// <summary>
        /// The device is ready to connect
        /// </summary>
        ReadyToConnect,
        /// <summary>
        /// Connecting to the device
        /// </summary>
        Connecting,
        /// <summary>
        /// The device is connected
        /// </summary>
        Connected,
        /// <summary>
        /// The device is abnormaly disconnected
        /// </summary>
        Error,
        /// <summary>
        /// The device is unconnected successfully
        /// </summary>
        Disconnected
    }
}
