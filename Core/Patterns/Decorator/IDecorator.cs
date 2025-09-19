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
    /// Defines the contract for the decorator pattern, allowing a target object to be decorated with additional functionality.
    /// </summary>
    /// <typeparam name="T">The type of the target object that is being decorated.</typeparam>
    public interface IDecorator<out T>
    {
        /// <summary>
        /// Gets the target object that is being decorated.
        /// </summary>
        T Target { get; }
    }

}
