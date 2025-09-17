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

namespace VisionNet.Core.Events
{
    /// <summary>
    /// A generic EventArgs instance that is used to represent a state change event.
    /// </summary>
    /// <typeparam name="TState">The type of the state associated with the event.</typeparam>
    public class StateChangedEventArgs<TState> : EventArgs
    {
        /// <summary>
        /// The previous state of the EventArgs
        /// </summary>
        public TState PreviousState { get; private set; }

        /// <summary>
        /// The current state of the EventArgs
        /// </summary>
        public TState CurrentState { get; private set; }

        /// <summary>
        /// Identification of the state machine
        /// </summary>
        public string StateMachineIndex { get; private set; }

        /// <summary>
        /// Period of time that the previous state was activated
        /// </summary>
        public TimeSpan Duration { get; private set; }

        /// <summary>
        /// Create a generic EventArgs with the specific value
        /// </summary>
        /// <param name="currentState">The current state value</param>
        /// <param name="previousState">The previous state value</param>
        /// <param name="stateMachineIndex">Identification of the state machine</param>
        /// <param name="duration">Period of time that the previous state was activated</param>
        public StateChangedEventArgs(TState currentState, TState previousState, string stateMachineIndex, TimeSpan duration)
        {
            CurrentState = currentState;
            PreviousState = previousState;
            StateMachineIndex = stateMachineIndex;
            Duration = duration;
        }
    }
}
