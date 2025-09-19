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
using System.Linq;
using VisionNet.Core.Comparisons;

namespace VisionNet.Core.Maths
{
    /// <summary>
    /// Provides utility mathematical operations and range helpers so callers can perform
    /// common calculations, conversions, and index manipulations without duplicating logic.
    /// All members are deterministic and thread-safe because they avoid shared state.
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Retrieves the smallest element from the provided values using the default comparer for the type.
        /// </summary>
        /// <typeparam name="T">The comparable element type stored in <paramref name="values"/>.</typeparam>
        /// <param name="values">Ordered or unordered values to evaluate. The array must not be null or empty.</param>
        /// <returns>The minimum element within <paramref name="values"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="values"/> is empty.</exception>
        public static T Min<T>(params T[] values) => values.Min();

        /// <summary>
        /// Retrieves the largest element from the provided values using the default comparer for the type.
        /// </summary>
        /// <typeparam name="T">The comparable element type stored in <paramref name="values"/>.</typeparam>
        /// <param name="values">Ordered or unordered values to evaluate. The array must not be null or empty.</param>
        /// <returns>The maximum element within <paramref name="values"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="values"/> is empty.</exception>
        public static T Max<T>(params T[] values) => values.Max();

        /// <summary>
        /// Constrains a value so that it remains between the supplied inclusive bounds.
        /// </summary>
        /// <typeparam name="T">The value type implementing <see cref="IComparable{T}"/>.</typeparam>
        /// <param name="val">The value to clamp. If it already lies within the range it is returned unchanged.</param>
        /// <param name="min">The inclusive lower bound allowed for <paramref name="val"/>.</param>
        /// <param name="max">The inclusive upper bound allowed for <paramref name="val"/>.</param>
        /// <returns><paramref name="min"/> when <paramref name="val"/> is lower than the bound, <paramref name="max"/> when it exceeds the bound, or the original value when it lies within the range.</returns>
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        /// <summary>
        /// Increments an integer within an inclusive range, wrapping to the lower bound when the upper bound is exceeded.
        /// </summary>
        /// <param name="val">The value to advance. Values outside the range are clamped before incrementing.</param>
        /// <param name="minValue">The inclusive lower bound of the cyclic range.</param>
        /// <param name="maxValue">The inclusive upper bound of the cyclic range.</param>
        /// <returns>The incremented value, wrapped to <paramref name="minValue"/> when the range end is crossed.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="maxValue"/> is lower than <paramref name="minValue"/>.</exception>
        public static int IncInRange(this int val, int minValue, int maxValue)
        {
            // Validate range
            if (maxValue < minValue)
                throw new ArgumentException("maxValue must be greater than or equal to minValue.");

            // Clamp the initial value
            val = Clamp(val, minValue, maxValue);

            // Increment and wrap around if necessary
            val++;
            if (val > maxValue)
                val = minValue;

            return val;
        }

        /// <summary>
        /// Decrements an integer within an inclusive range, wrapping to the upper bound when the lower bound is crossed.
        /// </summary>
        /// <param name="val">The value to reduce. Values outside the range are clamped before decrementing.</param>
        /// <param name="minValue">The inclusive lower bound of the cyclic range.</param>
        /// <param name="maxValue">The inclusive upper bound of the cyclic range.</param>
        /// <returns>The decremented value, wrapped to <paramref name="maxValue"/> when the range start is crossed.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="maxValue"/> is lower than <paramref name="minValue"/>.</exception>
        public static int DecInRange(this int val, int minValue, int maxValue)
        {
            // Validate range
            if (maxValue < minValue)
                throw new ArgumentException("maxValue must be greater than or equal to minValue.");

            // Clamp the initial value
            val = Clamp(val, minValue, maxValue);

            // Decrement and wrap around if necessary
            val--;
            if (val < minValue)
                val = maxValue;

            return val;
        }

        /// <summary>
        /// Calculates the dimensions of the smallest square-like matrix that can contain the requested number of elements.
        /// </summary>
        /// <param name="elements">The number of items that must fit within the matrix. Must be greater than zero.</param>
        /// <param name="cols">Outputs the recommended column count.</param>
        /// <param name="rows">Outputs the recommended row count.</param>
        /// <param name="columnPriority">When an exact square is not possible, <see langword="true"/> increases the column count first; otherwise the row count takes precedence.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="elements"/> is less than or equal to zero.</exception>
        public static void CalculateUpperSquaredMatrixSize(int elements, out int cols, out int rows, bool columnPriority = true)
        {
            if (elements <= 0)
                throw new ArgumentException($"The number of elements must be greater than 0.");

            var root = System.Math.Sqrt(elements);
            cols = columnPriority ? (int)System.Math.Ceiling(root): (int)System.Math.Round(root);
            rows = columnPriority ? (int)System.Math.Round(root): (int)System.Math.Ceiling(root);
        }

        /// <summary>
        /// Determines the row and column within a matrix for the specified linear position using left-to-right, top-to-bottom ordering.
        /// </summary>
        /// <param name="position">Zero-based index in traversal order whose coordinates are required.</param>
        /// <param name="cols">The total number of columns in the matrix. Must be greater than zero.</param>
        /// <param name="rows">The total number of rows in the matrix. Must be greater than zero.</param>
        /// <param name="col">Outputs the zero-based column coordinate corresponding to <paramref name="position"/>.</param>
        /// <param name="row">Outputs the zero-based row coordinate corresponding to <paramref name="position"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="cols"/> or <paramref name="rows"/> is less than or equal to zero.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="position"/> does not map to a valid cell within the matrix.</exception>
        public static void GetMatrixPosition(int position, int cols, int rows, out int col, out int row)
        {
            if (cols <= 0)
                throw new ArgumentException($"The number of columns must be greater than 0.");

            if (rows <= 0)
                throw new ArgumentException($"The number of rows must be greater than 0.");

            if (!position.InRange(0, (cols * rows)-1))
                throw new ArgumentException($"The position {position} does not fit in the matrix {cols}x{rows}.");

            row = position / cols;
            col = position % cols;
        }

        /// <summary>
        /// Converts an angle measured in degrees to radians using the constant π.
        /// </summary>
        /// <param name="degrees">The angle expressed in degrees. Any finite value is permitted.</param>
        /// <returns>The angle converted to radians.</returns>
        public static double DegreesToRadians(double degrees)
        {
            return degrees * System.Math.PI / 180.0;
        }

        /// <summary>
        /// Converts an angle measured in radians to degrees using the constant π.
        /// </summary>
        /// <param name="radians">The angle expressed in radians. Any finite value is permitted.</param>
        /// <returns>The angle converted to degrees.</returns>
        public static double RadiansToDegrees(double radians)
        {
            return radians * 180.0 / System.Math.PI;
        }

        /// <summary>
        /// Calculates the Euclidean distance between two points in the Cartesian plane using single-precision inputs.
        /// </summary>
        /// <param name="x1">The x-coordinate of the first point.</param>
        /// <param name="y1">The y-coordinate of the first point.</param>
        /// <param name="x2">The x-coordinate of the second point.</param>
        /// <param name="y2">The y-coordinate of the second point.</param>
        /// <returns>The straight-line distance between the two points, returned as a <see cref="double"/>.</returns>
        public static double EuclideanDistance(float x1, float y1, float x2, float y2) =>
            System.Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));

        /// <summary>
        /// Calculates the Euclidean distance between two points in the Cartesian plane using double-precision inputs.
        /// </summary>
        /// <param name="x1">The x-coordinate of the first point.</param>
        /// <param name="y1">The y-coordinate of the first point.</param>
        /// <param name="x2">The x-coordinate of the second point.</param>
        /// <param name="y2">The y-coordinate of the second point.</param>
        /// <returns>The straight-line distance between the two points, returned as a <see cref="double"/>.</returns>
        public static double EuclideanDistance(double x1, double y1, double x2, double y2) =>
            System.Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));

        /// <summary>
        /// Computes the magnitude of a two-dimensional vector using single-precision inputs.
        /// </summary>
        /// <param name="vX">The horizontal component of the vector.</param>
        /// <param name="vY">The vertical component of the vector.</param>
        /// <returns>The vector magnitude, returned as a <see cref="double"/>.</returns>
        public static double Module(float vX, float vY) =>
            System.Math.Sqrt(vX * vX + vY * vY);

        /// <summary>
        /// Computes the magnitude of a two-dimensional vector using double-precision inputs.
        /// </summary>
        /// <param name="x">The horizontal component of the vector.</param>
        /// <param name="y">The vertical component of the vector.</param>
        /// <returns>The vector magnitude, returned as a <see cref="double"/>.</returns>
        public static double Module(double x, double y) =>
            System.Math.Sqrt(x * x + y * y);

    }
}
