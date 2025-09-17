using System;
using System.Collections.Generic;
using System.Text;

namespace VisionNet.Core.Drawing
{
    /// <summary>
    /// Simple 2D point with double precision coordinates. Provides basic arithmetic
    /// operators for convenience; note that these operators are value-wise and do not
    /// mutate operands.
    /// </summary>
    public struct PointD
    {
        /// <summary>X coordinate.</summary>
        public double X;
        /// <summary>Y coordinate.</summary>
        public double Y;
        /// <summary>
        /// Constructs a point from its coordinates.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }
        /// <summary>
        /// Adds two points component-wise.
        /// </summary>
        /// <param name="p1">Left operand.</param>
        /// <param name="p2">Right operand.</param>
        /// <returns>A new <see cref="PointD"/> equal to (p1.X + p2.X, p1.Y + p2.Y).</returns>
        public static PointD operator +(PointD p1, PointD p2)
        {
            return new PointD(p1.X + p2.X, p1.Y + p2.Y);
        }
        /// <summary>
        /// Subtracts two points component-wise.
        /// </summary>
        /// <param name="p1">Left operand.</param>
        /// <param name="p2">Right operand.</param>
        /// <returns>A new <see cref="PointD"/> equal to (p1.X - p2.X, p1.Y - p2.Y).</returns>
        public static PointD operator -(PointD p1, PointD p2)
        {
            return new PointD(p1.X - p2.X, p1.Y - p2.Y);
        }
        /// <summary>
        /// Multiplies a point by a scalar.
        /// </summary>
        /// <param name="p">Point to scale.</param>
        /// <param name="scalar">Scaling factor.</param>
        /// <returns>Scaled point.</returns>
        public static PointD operator *(PointD p, double scalar)
        {
            return new PointD(p.X * scalar, p.Y * scalar);
        }
        /// <summary>
        /// Divides a point by a scalar.
        /// </summary>
        /// <param name="p">Point to scale.</param>
        /// <param name="scalar">Divisor (non-zero).</param>
        /// <returns>Scaled point.</returns>
        public static PointD operator /(PointD p, double scalar)
        {
            return new PointD(p.X / scalar, p.Y / scalar);
        }
        /// <summary>
        /// Formats the point as <c>(X, Y)</c> with default numeric formatting.
        /// </summary>
        /// <returns>Human-readable string.</returns>
        public override string ToString()
        {
            return string.Format("({0}, {1})", X, Y);
        }
    }
}
