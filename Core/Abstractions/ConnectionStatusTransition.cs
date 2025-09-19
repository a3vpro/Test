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
    /// Configures the allowed connection state transitions for <see cref="ConnectionStatus"/> state machines, leveraging <see cref="StatusValidator{TStatus}"/> infrastructure to enforce valid progressions.
    /// </summary>
    public class ConnectionStatusTransition : StatusValidator<ConnectionStatus>
    {
        // Define the happy-path and recovery transitions so consumers cannot move to unsupported states accidentally.
        /// <summary>
        /// Initializes the validator with the set of allowed transitions between connection states so that only legal moves are accepted by the state machine.
        /// </summary>
        /// <exception cref="System.ArgumentException">Propagated when the underlying <see cref="StatusValidator{TStatus}.AddTransition(TStatus, TStatus)"/> method rejects duplicate or contradictory transition definitions.</exception>
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
