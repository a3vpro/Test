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

namespace VisionNet.Core.Patterns
{
    /// <summary>
    /// A generic factory class that creates instances of the specified type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of object that the factory creates.</typeparam>
    public class Factory<T> : IFactory<T>
    {
        /// <summary>
        /// Creates a new instance of the type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>An instance of the type <typeparamref name="T"/>.</returns>
        public virtual T Create()
        {
            return NewInstance();
        }

        /// <summary>
        /// Creates a new instance of the generic type <typeparamref name="T"/> using reflection.
        /// </summary>
        /// <returns>An instance of the type <typeparamref name="T"/>.</returns>
        protected virtual T NewInstance()
        {
            return (T)Activator.CreateInstance(typeof(T));
        }

        /// <summary>
        /// Creates a new instance of the type <typeparamref name="T"/> through the factory.
        /// </summary>
        /// <returns>An instance of the type <typeparamref name="T"/>.</returns>
        public static T CreateNew()
        {
            return new Factory<T>().Create();
        }
    }
}
