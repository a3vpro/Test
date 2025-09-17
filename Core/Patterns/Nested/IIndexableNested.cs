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
    /// Interface for nested instances with index reference of the primeval instance
    /// </summary>
    public interface IIndexableNested<TPrimaryKey>: INested
    {
        /// <summary>
        /// Unique identifier for the primeval instance.
        /// </summary>
        TPrimaryKey PrimevalIndex { get; }
    }
}