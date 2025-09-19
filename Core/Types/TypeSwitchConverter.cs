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
    /// Provides type-based dispatch to convert runtime values into a common target type.
    /// Each registered case associates a specific runtime <see cref="Type"/> with a conversion delegate.
    /// </summary>
    /// <typeparam name="O">The conversion target type returned for every successful match.</typeparam>
    public class TypeSwitchConverter<O>
    {
        Dictionary<Type, Func<object, O>> matches = new Dictionary<Type, Func<object, O>>();

        /// <summary>
        /// Registers a conversion delegate that handles values whose runtime type exactly matches <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The runtime type that will trigger the provided conversion delegate.</typeparam>
        /// <param name="func">Conversion delegate executed when an input instance of type <typeparamref name="T"/> is processed.</param>
        /// <returns>The current converter to allow fluent configuration.</returns>
        /// <exception cref="ArgumentException">Thrown when a conversion for <typeparamref name="T"/> has already been registered.</exception>
        public TypeSwitchConverter<O> Case<T>(Func<object, O> func)
        {
            matches.Add(typeof(T), (x) => func(x));
            return this;
        }

        /// <summary>
        /// Registers a conversion delegate intended to act as a fallback when no other explicit type matches are available.
        /// </summary>
        /// <param name="func">Conversion delegate used when the runtime type does not have a dedicated registration.</param>
        /// <returns>The current converter to allow fluent configuration.</returns>
        /// <exception cref="ArgumentException">Thrown when a default conversion has already been registered.</exception>
        public TypeSwitchConverter<O> Default(Func<object, O> func)
        {
            matches.Add(typeof(object), (x) => func(x));
            return this;
        }

        /// <summary>
        /// Executes the conversion delegate registered for the exact runtime type of the supplied value and returns its result.
        /// </summary>
        /// <param name="x">Value to convert into the target type <typeparamref name="O"/>.</param>
        /// <returns>The conversion result produced by the matching delegate.</returns>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="x"/> is <see langword="null"/>.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when no conversion delegate has been registered for <paramref name="x"/>'s runtime type.</exception>
        public O Switch(object x)
        {
            return matches[x.GetType()](x);
        }
    }
}