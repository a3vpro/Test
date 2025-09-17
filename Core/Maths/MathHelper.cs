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
    /// Mathematical helper class with aditional methods
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Gets the minimum value of the provided list
        /// </summary>
        /// <typeparam name="T">Associated type</typeparam>
        /// <param name="values">Provided array of values</param>
        /// <returns>Minimum value</returns>
        public static T Min<T>(params T[] values) => values.Min();

        /// <summary>
        /// Gets the maximium value of the provided list
        /// </summary>
        /// <typeparam name="T">Associated type</typeparam>
        /// <param name="values">Provided array of values</param>
        /// <returns>Maximium value</returns>
        public static T Max<T>(params T[] values) => values.Max();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        /// <summary>
        /// Increments an integer value within a specified range. If the value exceeds the maximum, it wraps around to the minimum.
        /// </summary>
        /// <param name="val">The current value to increment.</param>
        /// <param name="minValue">The minimum value of the range.</param>
        /// <param name="maxValue">The maximum value of the range.</param>
        /// <returns>The incremented value within the range.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="maxValue"/> is less than <paramref name="minValue"/>.</exception>
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
        /// Decrements an integer value within a specified range. If the value goes below the minimum, it wraps around to the maximum.
        /// </summary>
        /// <param name="val">The current value to decrement.</param>
        /// <param name="minValue">The minimum value of the range.</param>
        /// <param name="maxValue">The maximum value of the range.</param>
        /// <returns>The decremented value within the range.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="maxValue"/> is less than <paramref name="minValue"/>.</exception>
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
        /// Calculates the most similiar squared matrix (columns and rows)
        /// where the elements can fit into.
        /// </summary>
        /// <param name="elements">Number of element I want to put in the matrix</param>
        /// <param name="cols">Result columns of the matrix</param>
        /// <param name="rows">Result rows of the matrix</param>
        /// <param name="columnPriority">In case of it can not be a squared matrix, 
        /// indicate the preference to increase one column or row</param>
        public static void CalculateUpperSquaredMatrixSize(int elements, out int cols, out int rows, bool columnPriority = true)
        {
            if (elements <= 0)
                throw new ArgumentException($"The number of elements must be greater than 0.");

            var root = System.Math.Sqrt(elements);
            cols = columnPriority ? (int)System.Math.Ceiling(root): (int)System.Math.Round(root);
            rows = columnPriority ? (int)System.Math.Round(root): (int)System.Math.Ceiling(root);
        }

        /// <summary>
        /// Calculate the column and row of the position in the matrix defined.
        /// </summary>
        /// <param name="position">Order in the matrix in LeftToRight and TopToBottom priority</param>
        /// <param name="cols">Number of columns in the matrix</param>
        /// <param name="rows">Number of rows in the matrix</param>
        /// <param name="col">Calculated column position</param>
        /// <param name="row">Calculated row position</param>
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
        /// Converts an angle from degrees to radians.
        /// </summary>
        /// <param name="degrees">Angle in degrees.</param>
        /// <returns>Angle in radians.</returns>
        public static double DegreesToRadians(double degrees)
        {
            return degrees * System.Math.PI / 180.0;
        }

        /// <summary>
        /// Converts an angle from radians to degrees.
        /// </summary>
        /// <param name="radians">Angle in radians.</param>
        /// <returns>Angle in degrees.</returns>
        public static double RadiansToDegrees(double radians)
        {
            return radians * 180.0 / System.Math.PI;
        }

        /// <summary>
        /// Calculates the Euclidean distance between two specified points (X, Y).
        /// </summary>
        /// <param name="x1">The X coordinate of the first point.</param>
        /// <param name="y1">The Y coordinate of the first point.</param>
        /// <param name="x2">The X coordinate of the second point.</param>
        /// <param name="y2">The Y coordinate of the second point.</param>
        /// <returns>The Euclidean distance between two specified points (X, Y).</returns>
        public static double EuclideanDistance(float x1, float y1, float x2, float y2) =>
            System.Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));

        /// <summary>
        /// Calculates the Euclidean distance between two specified points (X, Y).
        /// </summary>
        /// <param name="x1">The X coordinate of the first point.</param>
        /// <param name="y1">The Y coordinate of the first point.</param>
        /// <param name="x2">The X coordinate of the second point.</param>
        /// <param name="y2">The Y coordinate of the second point.</param>
        /// <returns>The Euclidean distance between two specified points (X, Y).</returns>
        public static double EuclideanDistance(double x1, double y1, double x2, double y2) =>
            System.Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));

        /// <summary>
        /// Calculates the module of a vector.
        /// </summary>
        /// <param name="vX">The X component of the vector.</param>
        /// <param name="vY">The Y component of the vector.</param>
        /// <returns>The module of the vector.</returns>
        public static double Module(float vX, float vY) =>
            System.Math.Sqrt(vX * vX + vY * vY);

        /// <summary>
        /// Calculates the module of a vector.
        /// </summary>
        /// <param name="vX">The X component of the vector.</param>
        /// <param name="vY">The Y component of the vector.</param>
        /// <returns>The module of the vector.</returns>
        public static double Module(double x, double y) =>
            System.Math.Sqrt(x * x + y * y);

    }
}
