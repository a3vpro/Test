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
    /// Describes the contract for components that establish and terminate external connections while tracking state transitions and respecting configured timeouts.
    /// </summary>
    public interface IConnectable
    {
        /// <summary>
        /// Gets the maximum amount of time the implementation allows for the connection handshake before aborting the attempt.
        /// </summary>
        TimeSpan ConnectionTimeout { get; }

        /// <summary>
        /// Gets the current lifecycle status of the connection, allowing callers to inspect whether the endpoint is reachable, transitioning, or faulted.
        /// </summary>
        ConnectionStatus ConnectionStatus { get; }

        /// <summary>
        /// Initiates the connection workflow to the underlying endpoint, performing any necessary negotiation or authentication defined by the implementation.
        /// </summary>
        /// <exception cref="TimeoutException">Thrown when the connection attempt exceeds <see cref="ConnectionTimeout"/> without completing.</exception>
        /// <exception cref="InvalidOperationException">Thrown when a connection attempt is issued while the implementation is already connected or otherwise in a state that disallows reconnection.</exception>
        void Connect();

        /// <summary>
        /// Gracefully terminates the active connection and releases any associated resources while updating the connection status accordingly.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when a disconnect is attempted while the connection is already closed and the implementation forbids redundant calls.</exception>
        void Disconnect();
    }
}