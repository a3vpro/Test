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
    /// Defines an indexable interface that allows access to elements by index.
    /// This interface inherits from a generic version of <see cref="IIndexable{T}"/> with a specific type of <see cref="object"/>.
    /// </summary>
    public interface IIndexable : IIndexable<object> { }
}

