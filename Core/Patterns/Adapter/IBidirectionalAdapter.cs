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
    /// Defines an interface for bidirectional adapters, enabling conversion between two types in both directions.
    /// Inherits from both <see cref="IAdapter{TSource, TDestiny}"/> and <see cref="IReverseAdapter{TSource, TDestiny}"/>.
    /// </summary>
    /// <typeparam name="T1">The first type to be converted.</typeparam>
    /// <typeparam name="T2">The second type to be converted.</typeparam>
    public interface IBidirectionalAdapter<T1, T2> : IAdapter<T1, T2>, IReverseAdapter<T1, T2>
    {
    }

}
