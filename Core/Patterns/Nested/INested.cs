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
    /// Represents a simple nested pattern, where an object can contain an inner instance of the same type.
    /// </summary>
    public interface INested
    {
        /// <summary>
        /// Gets the inner nested instance of the same type.
        /// </summary>
        INested Inner { get; }
    }
}
