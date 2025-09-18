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
    /// Exposes validation semantics that surface detailed failure information through a typed result payload when checks cannot be satisfied.
    /// </summary>
    /// <typeparam name="T">Structured result type emitted on failure conditions; must derive from <see cref="InvalidCheckResult"/> to convey error metadata.</typeparam>
    public interface ICheckeable<T> where T : InvalidCheckResult
    {
        /// <summary>
        /// Evaluates the current state and reports whether it satisfies the required invariants, returning detailed diagnostics when it does not.
        /// </summary>
        /// <param name="result">Outputs the contextual failure details when the check fails; undefined when the method returns <see langword="true"/>.</param>
        /// <returns><see langword="true"/> when the implementer meets all validation rules; otherwise <see langword="false"/> and <paramref name="result"/> contains the cause.</returns>
        bool TryCheck(out T result);

        /// <summary>
        /// Forces validation and throws an exception if invariants are violated, enabling consumers to rely on exceptions for control flow.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown when the implementer is in an invalid state and cannot pass the configured check.</exception>
        void Check();
    }
}
