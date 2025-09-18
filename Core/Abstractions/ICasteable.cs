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
    /// Provides a strongly-typed conversion contract that allows the implementing instance to expose its value in another compatible representation.
    /// </summary>
    public interface ICasteable
    {
        /// <summary>
        /// Projects the current object into the requested type, typically by returning either the instance itself or an adapted view.
        /// </summary>
        /// <typeparam name="T">Target type expected by the caller; must be supported by the implementing instance.</typeparam>
        /// <returns>The current instance represented as <typeparamref name="T"/>, either via casting or wrapping as defined by the implementation.</returns>
        /// <exception cref="InvalidCastException">Thrown when the requested cast cannot be satisfied due to an incompatible type.</exception>
        T Cast<T>();
    }

}
