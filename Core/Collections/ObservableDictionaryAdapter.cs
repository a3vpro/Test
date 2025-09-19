using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using VisionNet.Core.Patterns;

namespace VisionNet.Core.Collections
{
    /// <summary>
    /// Adapter that converts between <see cref="Dictionary{TKey, TValue}"/> and <see cref="ObservableDictionary{TKey, TValue}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of dictionary keys.</typeparam>
    /// <typeparam name="TValue">The type of dictionary values.</typeparam>
    public class ObservableDictionaryAdapter<TKey, TValue> : IBidirectionalAdapter<Dictionary<TKey, TValue>, ObservableDictionary<TKey, TValue>>
    {
        /// <summary>
        /// Converts a Dictionary to an ObservableDictionary.
        /// </summary>
        /// <param name="source">The source Dictionary.</param>
        /// <returns>A new ObservableDictionary containing the items from the Dictionary.</returns>
        public ObservableDictionary<TKey, TValue> Convert(Dictionary<TKey, TValue> source)
        {
            var result = new ObservableDictionary<TKey, TValue>();
            if (source == null)
                return result;

            foreach (var kvp in source)
            {
                result.Add(kvp.Key, kvp.Value);
            }
            return result;
        }

        /// <summary>
        /// Converts an ObservableDictionary to a Dictionary.
        /// </summary>
        /// <param name="source">The source ObservableDictionary.</param>
        /// <returns>A new Dictionary containing the items from the ObservableDictionary.</returns>
        public Dictionary<TKey, TValue> Convert(ObservableDictionary<TKey, TValue> source)
        {
            var result = new Dictionary<TKey, TValue>();
            if (source == null)
                return result;

            foreach (var kvp in source)
            {
                result[kvp.Key] = kvp.Value;
            }
            return result;
        }
    }
}