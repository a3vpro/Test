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
    public interface IReadonlySafeObjectCollection<TKey, TValue, TType> : IReadOnlyRepository<TValue, TKey>
        where TValue : IReadonlySafeObject<TType>
    {
        /// <summary>
        /// The try get value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>The result.</returns>
        bool TryGetValue(TKey key, out object value);

        /// <summary>
        /// Tries to retrieve a value of type <typeparamref name="TType"/> from the safe object identified by the given key.
        /// </summary>
        /// <param name="key">The key of the safe object to retrieve the value from.</param>
        /// <param name="value">The value of type <typeparamref name="TType"/> retrieved from the safe object, if found. The default value of <typeparamref name="TType"/> if the retrieval fails.</param>
        /// <returns>True if the value of type <typeparamref name="TType"/> was successfully retrieved, otherwise false.</returns>
        bool TryGetValue<T>(TKey key, out T value);

        /// <summary>
        /// Gets the this[].
        /// </summary>
        //object this[TKey key] { get; }

        /// <summary>
        /// Validates whether the given value is considered valid for the underlying data type of the safe object.
        /// </summary>
        /// <param name="key">The index to do the action.</param>
        /// <param name="value">The value to validate.</param>
        /// <returns>True if the value is valid; otherwise, false.</returns>
        bool IsValidValue(TKey key, object value);

        /// <summary>
        /// Converts the value of the safe object to a boolean, with an optional default value if the conversion is not straightforward.
        /// </summary>
        /// <param name="key">The index to do the action.</param>
        /// <param name="defaultValue">The default boolean value to return if the conversion cannot be performed directly.</param>
        /// <returns>The boolean representation of the safe object's value, or the default value if the conversion fails.</returns>
        bool ToBool(TKey key, bool defaultValue = false);

        /// <summary>
        /// Converts the value of the safe object to a double, with an optional default value if the conversion is not straightforward.
        /// </summary>
        /// <param name="key">The index to do the action.</param>
        /// <param name="defaultValue">The default double value to return if the conversion cannot be performed directly.</param>
        /// <returns>The double representation of the safe object's value, or the default value if the conversion fails.</returns>
        double ToFloat(TKey key, double defaultValue = 0D);

        /// <summary>
        /// Converts the value of the safe object to a long, with an optional default value if the conversion is not straightforward.
        /// </summary>
        /// <param name="key">The index to do the action.</param>
        /// <param name="defaultValue">The default long value to return if the conversion cannot be performed directly.</param>
        /// <returns>The long representation of the safe object's value, or the default value if the conversion fails.</returns>
        long ToInt(TKey key, long defaultValue = 0L);

        /// <summary>
        /// Converts the value of the safe object to a string, with an optional default value if the conversion is not straightforward.
        /// </summary>
        /// <param name="key">The index to do the action.</param>
        /// <param name="defaultValue">The default string value to return if the conversion cannot be performed directly.</param>
        /// <returns>The string representation of the safe object's value, or the default value if the conversion fails.</returns>
        string ToString(TKey key, string defaultValue = "");

        /// <summary>
        /// Determines if the value of the safe object can be considered 'true' in a boolean context.
        /// </summary>
        /// <param name="key">The index to do the action.</param>
        /// <returns>True if the value represents a 'true' state; otherwise, false.</returns>
        bool IsTrue(TKey key);

        /// <summary>
        /// Determines if the value of the safe object can be considered 'false' in a boolean context.
        /// </summary>
        /// <param name="key">The index to do the action.</param>
        /// <returns>True if the value represents a 'false' state; otherwise, false.</returns>
        bool IsFalse(TKey key);
    }
}
