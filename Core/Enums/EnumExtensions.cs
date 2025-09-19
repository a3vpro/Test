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
using System;
using System.Collections.Generic;
using System.Linq;

namespace VisionNet.Core.Enums
{
    /// <summary>
    /// Provides extension methods for enumerations.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Determines if the enum value is contained within a specified set of values.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="this">The enum value to check.</param>
        /// <param name="possibles">The list of possible enum values.</param>
        /// <returns>True if the enum value is within the set of possible values; otherwise, false.</returns>
        public static bool IsIn<T>(this T @this, params T[] possibles)
            where T : struct, Enum
            => possibles?.Contains(@this) ?? false;

        /// <summary>
        /// Determines if the enum value is not contained within a specified set of values.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="this">The enum value to check.</param>
        /// <param name="possibles">The list of rejected enum values.</param>
        /// <returns>True if the enum value is not within the rejected values; otherwise, false.</returns>
        public static bool IsNotIn<T>(this T @this, params T[] possibles)
            where T : struct, Enum
            => possibles?.Contains(@this) ?? false;

        /// <summary>
        /// Converts an enum type to a list of key-value pairs, where the key is the enum value and the value is the string representation of that enum.
        /// </summary>
        /// <param name="enumType">The type of the enum to convert.</param>
        /// <returns>A collection of <see cref="KeyValuePair{Object, String}"/> representing the enum values and their string representations.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="enumType"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="enumType"/> is not an enum type.</exception>
        public static IEnumerable<KeyValuePair<object, string>> ToList(Type enumType)
        {
            if (enumType == null)
                throw new ArgumentNullException(nameof(enumType), "The enum type cannot be null.");

            if (!enumType.IsEnum)
                throw new ArgumentException("The provided type must be an enum type.", nameof(enumType));

            return from e in Enum.GetValues(enumType).OfType<object>()
                   select new KeyValuePair<object, string>(e, e.ToString());
        }

        /// <summary>
        /// Converts an enum type to a list of string representations of the enum values.
        /// </summary>
        /// <param name="enumType">The type of the enum to convert.</param>
        /// <returns>A collection of strings representing the enum values.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="enumType"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="enumType"/> is not an enum type.</exception>
        public static IEnumerable<string> ToStringList(Type enumType)
        {
            if (enumType == null)
                throw new ArgumentNullException(nameof(enumType), "The enum type cannot be null.");

            if (!enumType.IsEnum)
                throw new ArgumentException("The provided type must be an enum type.", nameof(enumType));

            return from e in Enum.GetValues(enumType).OfType<object>()
                   select e.ToString();
        }

        /// <summary>
        /// Converts an enum type to a list of the enum values as objects.
        /// </summary>
        /// <param name="enumType">The type of the enum to convert.</param>
        /// <returns>A collection of objects representing the enum values.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="enumType"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="enumType"/> is not an enum type.</exception>
        public static IEnumerable<object> ToValuesList(Type enumType)
        {
            if (enumType == null)
                throw new ArgumentNullException(nameof(enumType), "The enum type cannot be null.");

            if (!enumType.IsEnum)
                throw new ArgumentException("The provided type must be an enum type.", nameof(enumType));

            return from e in Enum.GetValues(enumType).OfType<object>()
                   select e;
        }

        /// <summary>
        /// Creates a list with all keys and values of a given enum type.
        /// </summary>
        /// <typeparam name="T">The enum type, which must be a struct.</typeparam>
        /// <returns>A list of <see cref="KeyValuePair{T, String}"/> representing the enum values and their string representations.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided type is not an enum type.</exception>
        public static IEnumerable<KeyValuePair<T, string>> ToList<T>() where T : struct
        {
            Type typeFromHandle = typeof(T);
            if (!typeFromHandle.IsEnum)
                throw new ArgumentException("The provided type must be an enum type.");

            return from e in Enum.GetValues(typeFromHandle).OfType<T>()
                   select new KeyValuePair<T, string>(e, e.ToString());
        }

        /// <summary>
        /// Creates a list with all string representations of the enum values for a given enum type.
        /// </summary>
        /// <typeparam name="T">The enum type, which must be a struct.</typeparam>
        /// <returns>A list of strings representing the enum values.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided type is not an enum type.</exception>
        public static IEnumerable<string> ToStringList<T>() where T : struct
        {
            Type typeFromHandle = typeof(T);
            if (!typeFromHandle.IsEnum)
                throw new ArgumentException("The provided type must be an enum type.");

            return from e in Enum.GetValues(typeFromHandle).OfType<T>()
                   select e.ToString();
        }

        /// <summary>
        /// Creates a list with all enum values for a given enum type.
        /// </summary>
        /// <typeparam name="T">The enum type, which must be a struct.</typeparam>
        /// <returns>A list of enum values represented as objects.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided type is not an enum type.</exception>
        public static IEnumerable<T> ToValuesList<T>() where T : struct
        {
            Type typeFromHandle = typeof(T);
            if (!typeFromHandle.IsEnum)
                throw new ArgumentException("The provided type must be an enum type.");

            return from e in Enum.GetValues(typeFromHandle).OfType<T>()
                   select e;
        }
    }
}
