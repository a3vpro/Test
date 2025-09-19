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
using System.Collections.Generic;

namespace VisionNet.Core.Abstractions
{
    /// <summary>
    /// Represents an unsubscriber that removes an observer from the list of observers when disposed.
    /// </summary>
    /// <typeparam name="T">The type of the observed data.</typeparam>
    public class Unsubscriber<T> : IDisposable
    {
        private List<IObserver<T>> _observers;
        private IObserver<T> _observer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Unsubscriber{T}"/> class.
        /// </summary>
        /// <param name="observers">The list of observers to which the <paramref name="observer"/> belongs.</param>
        /// <param name="observer">The observer to be removed upon disposal.</param>
        public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        /// <summary>
        /// Removes the observer from the list of observers when disposed.
        /// </summary>
        public void Dispose()
        {
            if (_observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}
