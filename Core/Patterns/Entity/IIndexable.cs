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
    /// Defines an interface for objects that can be indexed by a unique identifier.
    /// </summary>
    /// <typeparam name="TPrimaryKey">The type of the unique identifier used to index the object.</typeparam>
    public interface IIndexable<TPrimaryKey>
    {
        /// <summary>
        /// Gets the unique identifier for this instance.
        /// </summary>
        TPrimaryKey Index { get; }
    }
}

