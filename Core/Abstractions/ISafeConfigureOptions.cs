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
using Microsoft.Extensions.Options;

namespace VisionNet.Core.Abstractions
{
    /// <summary>
    /// Defines a contract for configuring options in a safe manner, allowing for the possibility of a failed configuration attempt.
    /// </summary>
    /// <typeparam name="TOptions">The type of options to be configured, which must be a reference type (class).</typeparam>
    public interface ISafeConfigureOptions<in TOptions> : IConfigureOptions<TOptions>
        where TOptions : class
    {
        /// <summary>
        /// Attempts to configure the given options. If configuration fails, it returns false.
        /// </summary>
        /// <param name="options">The options to configure.</param>
        /// <returns>True if the configuration was successful, otherwise false.</returns>
        bool TryConfigure(TOptions options);
    }
}
