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

namespace VisionNet.Core.Types
{
    /// <summary>
    /// Provides a fluent type-based dispatcher that maps types to <see cref="Func{TResult}"/> delegates
    /// and executes the matching delegate to produce a value of type <typeparamref name="O"/>.
    /// </summary>
    /// <typeparam name="O">Type returned by each registered delegate and by the switch evaluation.</typeparam>
    /// <remarks>
    /// Use <see cref="Case{T}(Func{O})"/> to register handlers for specific types, optionally add a
    /// <see cref="Default(Func{O})"/> handler, and invoke <see cref="Switch(Type)"/> or
    /// <see cref="Switch{T}()"/> to execute the associated delegate.
    /// </remarks>
    public class TypeSwitch<O>
    {
        Dictionary<Type, Func<O>> matches = new Dictionary<Type, Func<O>>();

        /// <summary>
        /// Registers a delegate to execute when the specified type <typeparamref name="T"/> is selected.
        /// </summary>
        /// <typeparam name="T">Type key that triggers the supplied delegate.</typeparam>
        /// <param name="func">Delegate invoked to produce the switch result for type <typeparamref name="T"/>.</param>
        /// <returns>The current <see cref="TypeSwitch{O}"/> instance to continue configuration.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when a delegate has already been registered for type <typeparamref name="T"/>.
        /// </exception>
        public TypeSwitch<O> Case<T>(Func<O> func)
        {
            matches.Add(typeof(T), () => func());
            return this;
        }

        /// <summary>
        /// Registers a delegate to execute when no other registered type matches the switch request.
        /// </summary>
        /// <param name="func">Delegate invoked when no specific type registration is available.</param>
        /// <returns>The current <see cref="TypeSwitch{O}"/> instance to continue configuration.</returns>
        /// <exception cref="ArgumentException">Thrown when a default delegate has already been registered.</exception>
        public TypeSwitch<O> Default(Func<O> func)
        {
            matches.Add(typeof(object), () => func());
            return this;
        }

        /// <summary>
        /// Executes the delegate associated with the provided type and returns its result.
        /// </summary>
        /// <param name="t">Type key whose associated delegate should be executed.</param>
        /// <returns>The value produced by the delegate registered for the provided type.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="t"/> is <see langword="null"/>.</exception>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when no delegate has been registered for the provided <paramref name="t"/> and no default exists.
        /// </exception>
        public O Switch(Type t)
        {
            return matches[t]();
        }


        /// <summary>
        /// Executes the delegate associated with the generic type argument and returns its result.
        /// </summary>
        /// <typeparam name="T">Type key whose registered delegate should be executed.</typeparam>
        /// <returns>The value produced by the delegate registered for type <typeparamref name="T"/>.</returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when no delegate has been registered for type <typeparamref name="T"/> and no default exists.
        /// </exception>
        public O Switch<T>()
        {
            return matches[typeof(T)]();
        }
    }
}