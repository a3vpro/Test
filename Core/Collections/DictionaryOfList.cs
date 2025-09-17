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
using System.Collections.Generic;

namespace VisionNet.Core.Collections
{
    /// <summary>
    /// Represents a dictionary where each key is associated with a list of values.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the list associated with each key.</typeparam>
    public class DictionaryOfList<TKey, TValue> : Dictionary<TKey, IList<TValue>>
    {
        /// <summary>
        /// Adds a key and value to the dictionary. If the key already exists, the value is added to the existing list of values.
        /// </summary>
        /// <param name="key">The key to which the value will be added.</param>
        /// <param name="value">The value to add to the list of values associated with the specified key.</param>
        /// <returns>The list of values associated with the specified key.</returns>
        public void Add(TKey key, TValue value)
        {
            if (!ContainsKey(key))
                base.Add(key, new List<TValue>() { value });
            else
            {
                var list = this[key];
                if (!list.Contains(value))
                    list.Add(value);
            }
        }
    }
}

