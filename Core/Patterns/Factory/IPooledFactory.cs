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
namespace VisionNet.Core.Patterns
{
    /// <summary>
    /// Defines the contract for a factory that creates instances of a specified type, can link to another factory, 
    /// and supports creating objects based on an index.
    /// </summary>
    /// <typeparam name="T">The type of object that the factory creates.</typeparam>
    /// <typeparam name="TIndex">The type of the index used to create the object.</typeparam>
    public interface IPooledFactory<T, TIndex> : IFactoryAppender<T>
    {
        /// <summary>
        /// Creates and returns an instance of the specified type <typeparamref name="T"/> based on the provided index.
        /// </summary>
        /// <param name="index">The index used to create the object.</param>
        /// <returns>A new instance of type <typeparamref name="T"/> created using the specified index.</returns>
        T Create(TIndex index);
    }

}
