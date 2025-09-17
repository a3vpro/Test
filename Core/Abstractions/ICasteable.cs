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
    /// Defines an interface for objects that can be cast to a specified type.
    /// </summary>
    public interface ICasteable
    {
        /// <summary>
        /// Casts the current object to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to cast the object to.</typeparam>
        /// <returns>The object cast to the specified type.</returns>
        T Cast<T>();
    }

}
