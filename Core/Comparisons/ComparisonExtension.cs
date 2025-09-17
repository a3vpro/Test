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

namespace VisionNet.Core.Comparisons
{
    /// <summary>
    /// Provides extension methods for comparing values in different range conditions.
    /// </summary>
    public static class ComparisonExtension
    {
        /// <summary>
        /// Determines if a value is within the range of two other values, inclusive.
        /// </summary>
        /// <typeparam name="T">The type of the value being checked. Must implement IComparable.</typeparam>
        /// <param name="value">The value to be checked.</param>
        /// <param name="lowValue">The lower bound of the range.</param>
        /// <param name="upValue">The upper bound of the range.</param>
        /// <returns>True if the value is in the range defined by lowValue and upValue, inclusive.</returns>
        public static bool InRange<T>(this T value, T lowValue, T upValue)
            where T : IComparable
        {
            return value.CompareTo(lowValue) >= 0 && value.CompareTo(upValue) <= 0;
        }

        /// <summary>
        /// Determines if a value is within the range of two other values, exclusive.
        /// </summary>
        /// <typeparam name="T">The type of the value being checked. Must implement IComparable.</typeparam>
        /// <param name="value">The value to be checked.</param>
        /// <param name="lowValue">The lower bound of the range.</param>
        /// <param name="upValue">The upper bound of the range.</param>
        /// <returns>True if the value is greater than lowValue and less than upValue, exclusive.</returns>
        public static bool InRangeExclusive<T>(this T value, T lowValue, T upValue)
        where T : IComparable
        {
            return value.CompareTo(lowValue) > 0 && value.CompareTo(upValue) < 0;
        }

        /// <summary>
        /// Determines if a value is outside the range of two other values, inclusive.
        /// </summary>
        /// <typeparam name="T">The type of the value being checked. Must implement IComparable.</typeparam>
        /// <param name="value">The value to be checked.</param>
        /// <param name="lowValue">The lower bound of the range.</param>
        /// <param name="upValue">The upper bound of the range.</param>
        /// <returns>True if the value is either less than or equal to lowValue or greater than or equal to upValue.</returns>
        public static bool OutRange<T>(this T value, T lowValue, T upValue)
            where T : IComparable
        {
            return value.CompareTo(lowValue) <= 0 || value.CompareTo(upValue) >= 0;
        }

        /// <summary>
        /// Determines if a value is outside the range of two other values, exclusive.
        /// </summary>
        /// <typeparam name="T">The type of the value being checked. Must implement IComparable.</typeparam>
        /// <param name="value">The value to be checked.</param>
        /// <param name="lowValue">The lower bound of the range.</param>
        /// <param name="upValue">The upper bound of the range.</param>
        /// <returns>True if the value is less than lowValue or greater than upValue, exclusive.</returns>
        public static bool OutRangeExclusive<T>(this T value, T lowValue, T upValue)
        where T : IComparable
        {
            return value.CompareTo(lowValue) < 0 || value.CompareTo(upValue) > 0;
        }

        /// <summary>
        /// Compares two values of the same type for equality. Handles null values and uses IComparable or Equals for comparison.
        /// </summary>
        /// <typeparam name="T">The type of the values to compare.</typeparam>
        /// <param name="value">The value to compare.</param>
        /// <param name="comparedValue">The value to compare with.</param>
        /// <returns>True if the values are equal, otherwise false.</returns>
        public static bool SafeAreEqualTo<T>(this T value, T comparedValue)
        {
            if (CanBeCompared(value, comparedValue))
            {
                if (value == null)
                    return comparedValue == null;

                return value.Equals(comparedValue);
            }
            return false;
        }

        /// <summary>
        /// Determines if two values can be compared. This method ensures that both values are non-null and of the same type.
        /// </summary>
        /// <typeparam name="T">The type of the values to compare.</typeparam>
        /// <param name="value">The first value to check.</param>
        /// <param name="comparedValue">The second value to check.</param>
        /// <returns>True if the values are of the same type and can be compared, otherwise false.</returns>
        public static bool CanBeCompared<T>(this T value, T comparedValue)
        {
            return (value == null && comparedValue == null) ||
                   (value != null && comparedValue != null && value.GetType() == comparedValue.GetType() &&
                    value is IComparable);
        }
    }
}