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

namespace VisionNet.Core.Abstractions
{
    /// <summary>
    /// Represents a connectable instance that can connect and disconnect from a server.
    /// </summary>
    public interface IConnectable
    {
        /// <summary>
        /// Gets the timeout duration for connecting to a server, expressed in milliseconds.
        /// </summary>
        TimeSpan ConnectionTimeout { get; }

        /// <summary>
        /// Gets the current status of the connection.
        /// </summary>
        /// <value>
        /// The current connection status, represented by an instance of <see cref="ConnectionStatus"/>.
        /// </value>
        ConnectionStatus ConnectionStatus { get; }

        /// <summary>
        /// Initiates a connection to the server.
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnects from the server.
        /// If already disconnected, this method does nothing.
        /// </summary>
        void Disconnect();
    }
}