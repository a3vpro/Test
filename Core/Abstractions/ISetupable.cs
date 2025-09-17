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
    /// Defines an interface for objects that can be set up or initialized.
    /// </summary>
    public interface ISetupable
    {
        /// <summary>
        /// Performs the setup or initialization of the object.
        /// </summary>
        void Setup();
    }

    /// <summary>
    /// Defines a generic interface for objects that can be set up or initialized and return a result.
    /// </summary>
    /// <typeparam name="T">The type of the result returned after setup.</typeparam>
    public interface ISetupable<T>
    {
        /// <summary>
        /// Performs the setup or initialization of the object and returns a result of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>The result of the setup operation, of type <typeparamref name="T"/>.</returns>
        T Setup();
    }

}
