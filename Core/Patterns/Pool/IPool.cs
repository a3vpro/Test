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
    /// Represents a simple pool pattern for managing a collection of reusable objects of type <typeparamref name="T"/>.
    /// </summary>
    public interface IPool<T>
    {
        /// <summary>
        /// Gets the maximum number of objects the pool can hold.
        /// </summary>
        int MaxSize { get; }

        /// <summary>
        /// Gets the current number of objects in the pool.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Retrieves an object from the pool. If the pool is empty, it may create a new object.
        /// </summary>
        /// <returns>An object of type <typeparamref name="T"/> from the pool.</returns>
        T Get();

        /// <summary>
        /// Puts an object back into the pool for reuse.
        /// </summary>
        /// <param name="obj">The object to be returned to the pool.</param>
        void Release(T obj);
    }

}
