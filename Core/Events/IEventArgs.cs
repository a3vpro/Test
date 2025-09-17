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
namespace VisionNet.Core.Events
{
    /// <summary>
    /// Defines an interface for event arguments that encapsulate a value of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value encapsulated by the event arguments.</typeparam>
    public interface IEventArgs<out T>
    {
        /// <summary>
        /// Gets the value of the event arguments.
        /// </summary>
        T Value { get; }
    }

}
