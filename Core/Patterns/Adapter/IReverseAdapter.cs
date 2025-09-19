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
    /// Defines an interface for reverse conversion, allowing conversion from <typeparamref name="TDestiny"/> to <typeparamref name="TSource"/>.
    /// </summary>
    /// <typeparam name="TSource">The source type to be converted.</typeparam>
    /// <typeparam name="TDestiny">The target type to be converted from.</typeparam>
    public interface IReverseAdapter<TSource, TDestiny>
    {
        /// <summary>
        /// Converts a value of type <typeparamref name="TDestiny"/> to <typeparamref name="TSource"/>.
        /// </summary>
        /// <param name="value">The value of type <typeparamref name="TDestiny"/> to convert.</param>
        /// <returns>A converted value of type <typeparamref name="TSource"/>.</returns>
        TSource Convert(TDestiny value);
    }

}
