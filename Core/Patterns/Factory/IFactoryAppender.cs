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
    /// Defines the contract for a factory that can create instances of a specified type and link to another factory.
    /// </summary>
    /// <typeparam name="T">The type of object that the factory creates.</typeparam>
    public interface IFactoryAppender<T> : IFactory<T>
    {
        /// <summary>
        /// Links the current factory to another factory, enabling a chain of factories to create objects.
        /// </summary>
        /// <param name="factory">The factory to link to.</param>
        void LinkTo(IFactory<T> factory);
    }

}
