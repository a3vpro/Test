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
    /// Cleanable interface
    /// </summary>
    /// <summary>
    /// Defines an interface for objects that can perform a cleanup operation.
    /// </summary>
    public interface ICleanable
    {
        /// <summary>
        /// Performs the cleanup operation on the object, releasing resources or resetting states as necessary.
        /// </summary>
        void CleanUp();
    }

}
