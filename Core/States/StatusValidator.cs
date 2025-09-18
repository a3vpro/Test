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
using System.Linq;
using VisionNet.Core.Collections;
using VisionNet.Core.Events;

namespace VisionNet.Core.States
{
    /// <summary>
    /// Provides validation for state transitions by registering allowed transitions and emitting notifications when a transition occurs.
    /// </summary>
    /// <typeparam name="T">Type that represents the state identifier. It must provide value equality semantics.</typeparam>
    public class StatusValidator<T>
    {
        private DictionaryOfList<T, T> _transitions = new DictionaryOfList<T, T>();

        /// <summary>
        /// Registers an allowed transition from a source state to a destination state.
        /// </summary>
        /// <param name="source">State that acts as the origin of the transition. Must be a key recognizable by the validator.</param>
        /// <param name="destiny">State that can be reached from the specified source state.</param>
        public void AddTransition(T source, T destiny) =>
            _transitions.Add(source, destiny);


        /// <summary>
        /// Determines whether the specified destination state is a valid transition target for the provided source state.
        /// </summary>
        /// <param name="source">State currently assigned to the entity being validated.</param>
        /// <param name="destiny">State requested as the next state.</param>
        /// <returns><see langword="true"/> when the destination state has been registered as reachable from the source state; otherwise, <see langword="false"/>.</returns>
        public bool IsValidTransition(T source, T destiny)
        {
            return _transitions.ContainsKey(source) &&
                _transitions[source].Any(v => v.Equals(destiny));
        }


        /// <summary>
        /// Attempts to move from the specified source state to the destination state and invokes an optional action when the transition is valid.
        /// </summary>
        /// <param name="source">Current state prior to evaluating the transition.</param>
        /// <param name="destiny">Target state requested for the transition.</param>
        /// <param name="action">Optional callback executed after a successful transition and before raising <see cref="StatusChanged"/>.</param>
        /// <returns>The destination state when the transition is valid; otherwise, returns the original source state.</returns>
        public T GoToState(T source, T destiny, Action action = null)
        {
            if (IsValidTransition(source, destiny))
            {
                action?.Invoke();
                RaiseStatusChanged(this, new EventArgs<T>(destiny));
                return destiny;
            }
            else
                return source;
        }


        /// <summary>
        /// Tries to perform a transition by updating the referenced source state when the transition is valid and emitting the corresponding notifications.
        /// </summary>
        /// <param name="source">Reference to the variable that stores the current state. It is overwritten when the transition succeeds.</param>
        /// <param name="destiny">State requested as the next state for the referenced source.</param>
        /// <param name="action">Optional callback executed after a successful transition and before raising <see cref="StatusChanged"/>.</param>
        /// <returns><see langword="true"/> when the state was updated to the destination state; otherwise, <see langword="false"/>.</returns>
        public bool TryGoToState(ref T source, T destiny, Action action = null)
        {
            var result = IsValidTransition(source, destiny);

            if (result)
            {
                action?.Invoke();
                source = destiny;
                RaiseStatusChanged(this, new EventArgs<T>(destiny));
            }

            return result;
        }


        protected void RaiseStatusChanged(object sender, EventArgs<T> eventArgs)
        {
            try
            {
                StatusChanged?.Invoke(sender, eventArgs);
            }
            catch
            {
            }
        }
        /// <summary>
        /// Occurs after a successful transition, providing the destination state to subscribers.
        /// </summary>
        public event EventHandler<EventArgs<T>> StatusChanged;

    }
}
