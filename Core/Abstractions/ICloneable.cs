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
    /// Clonable interface
    /// </summary>
    public interface ICloneable<T>
    {
        /// <summary>
        /// Creates a new instance of the object <typeparamref name="T"/> that is a deep copy of the current instance.
        /// </summary>
        /// <returns>A new object <typeparamref name="T"/> that is a copy of this instance.</returns>
        T Clone();
    }
}
