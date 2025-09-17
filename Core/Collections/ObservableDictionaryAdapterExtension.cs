using System.Collections.Generic;
using VisionNet.Core.Patterns;

namespace VisionNet.Core.Collections
{
    /// <summary>
    /// Provides extension methods to convert between <see cref="Dictionary{TKey, TValue}"/> and <see cref="ObservableDictionary{TKey, TValue}"/> using <see cref="ObservableDictionaryAdapter{TKey, TValue}"/>.
    /// </summary>
    public static class ObservableDictionaryAdapterExtension
    {
        /// <summary>
        /// Converts a Dictionary to an ObservableDictionary.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="dictionary">The source Dictionary.</param>
        /// <returns>An ObservableDictionary containing the dictionary's elements.</returns>
        public static ObservableDictionary<TKey, TValue> ToObservableDictionary<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            var adapter = new ObservableDictionaryAdapter<TKey, TValue>();
            return adapter.Convert(dictionary);
        }

        /// <summary>
        /// Converts an ObservableDictionary to a Dictionary.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="observable">The source ObservableDictionary.</param>
        /// <returns>A Dictionary containing the observable dictionary's elements.</returns>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this ObservableDictionary<TKey, TValue> observable)
        {
            var adapter = new ObservableDictionaryAdapter<TKey, TValue>();
            return adapter.Convert(observable);
        }
    }
}
