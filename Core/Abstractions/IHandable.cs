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
    /// Defines an interface for objects that have a handle property.
    /// </summary>
    public interface IHandable
    {
        /// <summary>
        /// Gets the handle associated with the object.
        /// </summary>
        /// <value>
        /// The handle associated with the object, represented as an <see cref="object"/>.
        /// </value>
        object Handle { get; }
    }

}
