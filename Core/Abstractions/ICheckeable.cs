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
    /// Defines a contract for objects that can be checked for validity.
    /// </summary>
    /// <typeparam name="T">The type of the result that is returned from the check, which must derive from <see cref="InvalidCheckResult"/>.</typeparam>
    public interface ICheckeable<T> where T : InvalidCheckResult
    {
        /// <summary>
        /// Attempts to perform a check and outputs the result.
        /// </summary>
        /// <param name="result">The result of the check, which is of type <typeparamref name="T"/>.</param>
        /// <returns>True if the check was successful, otherwise false.</returns>
        bool TryCheck(out T result);

        /// <summary>
        /// Performs a check without returning the result. 
        /// </summary>
        void Check();
    }
}
