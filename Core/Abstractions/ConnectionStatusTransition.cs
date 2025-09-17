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
using VisionNet.Core.States;

namespace VisionNet.Core.Abstractions
{
    /// <summary>
    /// Represents a transition for a connection status, ensuring it adheres to validation rules defined by the base class.
    /// </summary>
    public class ConnectionStatusTransition : StatusValidator<ConnectionStatus>
    {
        // Add the rest of the class members with documentation if necessary.
        /// <summary> The ConnectionStatusTransition function is a constructor that creates the possible transitions between states for the ConnectionStatus state machine.</summary>
        /// <returns> The connectionstatustransition function.</returns>
        public ConnectionStatusTransition()
    {
        AddTransition(ConnectionStatus.Initial, ConnectionStatus.ReadyToConnect);

        AddTransition(ConnectionStatus.ReadyToConnect, ConnectionStatus.Connecting);

        AddTransition(ConnectionStatus.Connecting, ConnectionStatus.Connected);
        AddTransition(ConnectionStatus.Connecting, ConnectionStatus.Error);

        AddTransition(ConnectionStatus.Connected, ConnectionStatus.Disconnected);
        AddTransition(ConnectionStatus.Connected, ConnectionStatus.Error);

        AddTransition(ConnectionStatus.Disconnected, ConnectionStatus.Connecting);
        AddTransition(ConnectionStatus.Disconnected, ConnectionStatus.ReadyToConnect);

        AddTransition(ConnectionStatus.Error, ConnectionStatus.ReadyToConnect);
    }
    }
}
