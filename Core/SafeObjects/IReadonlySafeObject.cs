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
namespace VisionNet.Core.SafeObjects
{
    /// <summary>
    /// Defines a read-only wrapper for an object of type <typeparamref name="TType"/>, providing safe access to its value and utility methods for basic operations.
    /// </summary>
    /// <typeparam name="TType">The type of the underlying data object.</typeparam>
    public interface IReadonlySafeObject<TType>
    {
        /// <summary>
        /// Gets or sets the default value used when the underlying value is null or an invalid conversion is attempted.
        /// </summary>
        object DefaultValue { get; }

        /// <summary>
        /// Gets the type of the data stored within the safe object.
        /// </summary>
        TType DataType { get; }

        /// <summary>
        /// Retrieves the value stored in the safe object as an object type.
        /// </summary>
        /// <returns>The value of the underlying data object.</returns>
        object GetValue();

        /// <summary>
        /// Retrieves the value stored in the safe object as an object type.
        /// </summary>
        /// <returns>The value of the underlying data object.</returns>
        object Value { get; }

        /// <summary>
        /// Attempts to retrieve the value stored in the safe object, casting it to the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to which the value should be cast.</typeparam>
        /// <param name="value">The variable to store the cast value if the operation succeeds.</param>
        /// <returns>True if the value was successfully retrieved and cast; otherwise, false.</returns>
        bool TryGetValue<T>(out T value);

        /// <summary>
        /// Validates whether the given value is considered valid for the underlying data type of the safe object.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>True if the value is valid; otherwise, false.</returns>
        bool IsValidValue(object value);

        /// <summary>
        /// Converts the value of the safe object to a boolean, with an optional default value if the conversion is not straightforward.
        /// </summary>
        /// <param name="defaultValue">The default boolean value to return if the conversion cannot be performed directly.</param>
        /// <returns>The boolean representation of the safe object's value, or the default value if the conversion fails.</returns>
        bool ToBool(bool defaultValue = false);

        /// <summary>
        /// Converts the value of the safe object to a double, with an optional default value if the conversion is not straightforward.
        /// </summary>
        /// <param name="defaultValue">The default double value to return if the conversion cannot be performed directly.</param>
        /// <returns>The double representation of the safe object's value, or the default value if the conversion fails.</returns>
        double ToFloat(double defaultValue = 0D);

        /// <summary>
        /// Converts the value of the safe object to a long, with an optional default value if the conversion is not straightforward.
        /// </summary>
        /// <param name="defaultValue">The default long value to return if the conversion cannot be performed directly.</param>
        /// <returns>The long representation of the safe object's value, or the default value if the conversion fails.</returns>
        long ToInt(long defaultValue = 0L);

        /// <summary>
        /// Converts the value of the safe object to a string, with an optional default value if the conversion is not straightforward.
        /// </summary>
        /// <param name="defaultValue">The default string value to return if the conversion cannot be performed directly.</param>
        /// <returns>The string representation of the safe object's value, or the default value if the conversion fails.</returns>
        string ToString(string defaultValue = "");

        /// <summary>
        /// Determines if the value of the safe object can be considered 'true' in a boolean context.
        /// </summary>
        /// <returns>True if the value represents a 'true' state; otherwise, false.</returns>
        bool IsTrue();

        /// <summary>
        /// Determines if the value of the safe object can be considered 'false' in a boolean context.
        /// </summary>
        /// <returns>True if the value represents a 'false' state; otherwise, false.</returns>
        bool IsFalse();
    }

}
