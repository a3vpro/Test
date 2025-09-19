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
    /// Defines a generic adapter interface for converting objects of any type to another object of any type.
    /// Inherits from <see cref="IAdapter{TSource, TDestiny}"/> with <typeparamref name="TSource"/> and <typeparamref name="TDestiny"/> as <see cref="object"/>.
    /// </summary>
    public interface IGenericAdapter : IAdapter<object, object>
    {
    }
}
