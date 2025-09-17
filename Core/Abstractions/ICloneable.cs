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
namespace VisionNet.Core.Abstractions
{
    /// <summary>
    /// Defines the contract required to expose a deep clone operation capable of returning an independent copy of the current instance, ensuring that mutations on the clone do not impact the source object.
    /// </summary>
    public interface ICloneable<T>
    {
        /// <summary>
        /// Creates a new instance of the object <typeparamref name="T"/> that is a deep copy of the current instance, including any nested structures, so callers can safely mutate the resulting value without affecting the original.
        /// </summary>
        /// <returns>A fully detached copy of the current instance that mirrors the complete state of the source object.</returns>
        T Clone();
    }
}
