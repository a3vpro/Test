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
    /// Defines the contract for a factory that creates instances of a specified type.
    /// </summary>
    /// <typeparam name="T">The type of object that the factory creates.</typeparam>
    public interface IFactory<T>
    {
        /// <summary>
        /// Creates and returns a new instance of the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>A new instance of type <typeparamref name="T"/>.</returns>
        T Create();
    }

}
