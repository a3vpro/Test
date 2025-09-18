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

namespace VisionNet.Core.Abstractions
{
    /// <summary>
    /// Extends cloning semantics by allowing implementations to duplicate their state directly into an existing destination instance.
    /// </summary>
    public interface ICopiable : ICloneable
    {
        /// <summary>
        /// Copies the full state of the current object into the provided destination reference, overwriting any existing values to ensure both instances remain equivalent.
        /// </summary>
        /// <param name="destiny">Reference to the target object that should receive the cloned data; must be compatible with the implementer's concrete type.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="destiny"/> references a null target.</exception>
        /// <exception cref="InvalidCastException">Thrown when the supplied destination cannot be cast to the implementer's concrete type for copying.</exception>
        void CloneTo(ref object destiny);
    }

}
