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
    /// Defines an adapter interface for converting objects from one type to another.
    /// </summary>
    /// <typeparam name="TSource">The source type to be converted.</typeparam>
    /// <typeparam name="TDestiny">The target type after conversion.</typeparam>
    public interface IUpdatingAdapter<TSource, TDestiny>
    {
        /// <summary>
        /// Converts a value of type <typeparamref name="TSource"/> to <typeparamref name="TDestiny"/>.
        /// </summary>
        /// <param name="value">The value of type <typeparamref name="TSource"/> to convert.</param>
        /// <param name="destiny">The object of type <typeparamref name="TSource"/> to update.</param>
        /// <returns>A converted value of type <typeparamref name="TDestiny"/>.</returns>
        TDestiny Convert(TSource value, TDestiny destiny);
    }
}
