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
    public class StatusValidator<T>
    {
        private DictionaryOfList<T, T> _transitions = new DictionaryOfList<T, T>();

        public void AddTransition(T source, T destiny) =>
            _transitions.Add(source, destiny);

        
        /// <summary> The IsValidTransition function checks if the source state is in the transitions dictionary, and then checks if any of those transition states are equal to the destiny state.</summary>
        /// <param name="source"> The source state</param>
        /// <param name="destiny"> The destiny state.</param>
        /// <returns> A bool value.</returns>
        public bool IsValidTransition(T source, T destiny)
        {
            return _transitions.ContainsKey(source) &&
                _transitions[source].Any(v => v.Equals(destiny));
        }

        
        /// <summary> The GoToState function is used to transition from one state to another.
        /// It will only allow the transition if it is a valid one, otherwise it will return the current state.</summary>
        /// <param name="source"> The current state.</param>
        /// <param name="destiny"> The destiny state</param>
        /// <param name="action"> The action to be executed when the state changes.</param>
        /// <returns> The destiny state. if the transition is not valid, it returns the source state.</returns>
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

        
        /// <summary> The TryGoToState function attempts to transition the state machine from one state to another.
        /// If the transition is valid, it will execute an action and raise a StatusChanged event.</summary>
        /// <param name="source"> The current state of the object.</param>
        /// <param name="destiny"> The destiny state</param>
        /// <param name="action"> What is this parameter used for?</param>
        /// <returns> A boolean value. it returns true if the transition was successful, otherwise it returns false.</returns>
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

        
        /// <summary> The RaiseStatusChanged function is a helper function that raises the StatusChanged event.
        /// It catches any exceptions thrown by the event handlers and ignores them.</summary>
        /// <param name="sender"> The object that raised the event</param>
        /// <param name="eventArgs"> What is this?</param>
        /// <returns> A void.</returns>
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
        public event EventHandler<EventArgs<T>> StatusChanged;

    }
}
