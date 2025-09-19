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
using VisionNet.Core.Patterns;

namespace VisionNet.Core.SafeObjects
{
    public interface ISafeObjectCollection<TKey, TValue, TType> : IWriteSafeObjectCollection<TKey, TValue, TType>, IReadonlySafeObjectCollection<TKey, TValue, TType>
        where TValue : ISafeObject<TType>
    {

        ///// <summary>
        ///// The try set value.
        ///// </summary>
        ///// <param name="key">The key.</param>
        ///// <param name="value">The value.</param>
        ///// <returns>The result.</returns>
        //bool TrySetValue(TKey key, object value);

        /// <summary>
        /// Gets or Sets the this[].
        /// </summary>
        //new object this[TKey key] { get; set; }

        ///// <summary>
        ///// Clear the collection
        ///// </summary>
        //void Clear();

        //// TODO: Faltaría poner un valor a su valor por defecto
    }
}
