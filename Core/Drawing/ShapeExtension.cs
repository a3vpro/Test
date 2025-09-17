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
using System.Drawing;
using System.Linq;
using System.Reflection;
using VisionNet.Core.Comparisons;
using VisionNet.Core.Maths;

namespace VisionNet.Drawing
{
    /// <summary>
    /// Provides extension methods for geometric shapes and related calculations.
    /// </summary>
    public static class ShapeExtension
    {
        /// <summary> The IsInside function determines whether a point is inside of a rectangle.</summary>
        /// <param name="source"> The point to check</param>
        /// <param name="limit"> This is the limit of the rectangle.
        /// </param>
        /// <returns> True if the point is inside the rectangle</returns>
        public static bool IsInside(this Point source, Rectangle limit)
        {
            return source.X.InRange(limit.Left, limit.Right - 1) &&
                source.Y.InRange(limit.Top, limit.Bottom - 1);
        }

        /// <summary> The Clamp function returns a point that is within the specified rectangle.</summary>
        /// <param name="source"> The source.</param>
        /// <param name="limit"> 
        /// </param>
        /// <returns> A point</returns>
        public static Point Clamp(this Point source, Rectangle limit)
        {
            return new Point
            {
                X = source.X.Clamp(limit.Left, limit.Right - 1),
                Y = source.Y.Clamp(limit.Top, limit.Bottom - 1)
            };
        }

        /// <summary>
        /// Calculates the center of mass (average position) of a collection of points.
        /// </summary>
        /// <param name="points">The collection of points to calculate the center of mass for.</param>
        /// <returns>
        /// A <see cref="Point"/> representing the center of mass of the collection.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the collection is empty.
        /// </exception>
        public static Point CenterOfMass(this IEnumerable<Point> points)
        {
            var count = points.Count();
            var sum = points.Aggregate((p1, p2) => new Point(p1.X + p2.X, p1.Y + p2.Y));
            return new Point(sum.X / count, sum.Y / count);
        }

        /// <summary> The Center function takes a collection of points and returns the center of mass point.</summary>
        /// <param name="points"> The points.
        /// </param>
        /// <returns> The center point of a list of points.</returns>
        public static PointF CenterOfMass(this IEnumerable<PointF> points)
        {
            var count = points.Count();
            var sum = points.Aggregate((p1, p2) => new PointF(p1.X + p2.X, p1.Y + p2.Y));
            return new PointF(sum.X / (float)count, sum.Y / (float)count);
        }

        /// <summary>
        /// Calculates the Euclidean distance between two <see cref="PointF"/> objects.
        /// </summary>
        /// <param name="pointA">The first <see cref="PointF"/>.</param>
        /// <param name="pointB">The second <see cref="PointF"/>.</param>
        /// <returns>The Euclidean distance between the two points.</returns>
        public static double EuclideanDistance(this PointF pointA, PointF pointB) =>
            MathHelper.EuclideanDistance(pointA.X, pointA.Y, pointB.X, pointB.Y);

        /// <summary>
        /// Calculates the Euclidean distance between a <see cref="PointF"/> and a specified point (X, Y).
        /// </summary>
        /// <param name="pointA">The first <see cref="PointF"/>.</param>
        /// <param name="x">The X coordinate of the second point.</param>
        /// <param name="y">The Y coordinate of the second point.</param>
        /// <returns>The Euclidean distance between the <see cref="PointF"/> and the specified coordinates.</returns>
        public static double EuclideanDistance(this PointF pointA, float x, float y) =>
            MathHelper.EuclideanDistance(pointA.X, pointA.Y, x, y);

        /// <summary>
        /// Scales the <see cref="Point"/> by the specified factor.
        /// </summary>
        /// <param name="point">The <see cref="Point"/> to scale.</param>
        /// <param name="scale">The scaling factor.</param>
        /// <returns>A new <see cref="Point"/> that is scaled by the specified factor.</returns>
        public static Point Scale(this Point point, double scale)
        {
            return new Point((int)(point.X * scale), (int)(point.Y * scale));
        }

        /// <summary>
        /// Scales the <see cref="PointF"/> by the specified factor.
        /// </summary>
        /// <param name="point">The <see cref="PointF"/> to scale.</param>
        /// <param name="scale">The scaling factor.</param>
        /// <returns>A new <see cref="PointF"/> that is scaled by the specified factor.</returns>
        public static PointF Scale(this PointF point, double scale)
        {
            return new PointF((float)(point.X * scale), (float)(point.Y * scale));
        }

        /// <summary>
        /// Converts a <see cref="PointF"/> to a <see cref="Point"/> by truncating the coordinates to integers.
        /// </summary>
        /// <param name="point">The <see cref="PointF"/> to convert.</param>
        /// <returns>A new <see cref="Point"/> with integer values derived from the <see cref="PointF"/>.</returns>
        public static Point ToInt(this PointF point) =>
            new Point((int)point.X, (int)point.Y);

        /// <summary>
        /// Converts a <see cref="Point"/> to a <see cref="PointF"/> by converting the integer coordinates to floats.
        /// </summary>
        /// <param name="point">The <see cref="Point"/> to convert.</param>
        /// <returns>A new <see cref="PointF"/> with floating-point values derived from the <see cref="Point"/>.</returns>
        public static PointF ToFloat(this Point point) =>
            new PointF((float)point.X, (float)point.Y);

        /// <summary>
        /// Translates a point by the specified translation.
        /// </summary>
        /// <param name="point">The point to translate.</param>
        /// <param name="translation">The translation to apply.</param>
        /// <returns>A new point translated by the given translation.</returns>
        public static Point TranslateTo(this Point point, Point translation)
        {
            return new Point(
                point.X - translation.X,
                point.Y - translation.Y);
        }

        /// <summary>
        /// Translates a point by the given translation.
        /// </summary>
        /// <param name="point">The point to translate.</param>
        /// <param name="translation">The translation to apply.</param>
        /// <returns>A new point with the x and y values of the point parameter added to the x and y values of translation.</returns>
        public static Point TranslateFrom(this Point point, Point translation)
        {
            return new Point(
                point.X + translation.X,
                point.Y + translation.Y);
        }

        /// <summary>
        /// Determines whether the source rectangle is inside of the limit rectangle.
        /// </summary>
        /// <param name="source">The rectangle to check.</param>
        /// <param name="limit">The rectangle that defines the limit.</param>
        /// <returns>True if the source rectangle is inside or equal to the limit rectangle; otherwise, false.</returns>
        public static bool IsInside(this Rectangle source, Rectangle limit)
        {
            return source.Location.IsInside(limit) &&
                new Point(source.Right - 1, source.Bottom - 1).IsInside(limit);
        }

        /// <summary>
        /// Returns a rectangle that is the intersection of the source and limit rectangles.
        /// </summary>
        /// <param name="source">The source rectangle.</param>
        /// <param name="limit">The rectangle that defines the limit.</param>
        /// <returns>A rectangle that is within the bounds of the specified limit.</returns>
        public static Rectangle Clamp(this Rectangle source, Rectangle limit)
        {
            var leftTop = source.Location.Clamp(limit);
            var rightBottom = new Point(source.Right, source.Bottom).Clamp(limit);

            return Rectangle.FromLTRB(leftTop.X, leftTop.Y, rightBottom.X, rightBottom.Y);
        }

        /// <summary>
        /// The <see cref="Rectangle"/> extension method that calculates the area of the rectangle.
        /// </summary>
        /// <param name="source">The rectangle to calculate the area of.</param>
        /// <returns>The area of the rectangle as a long value.</returns>
        public static long Area(this Rectangle source)
        {
            return source.Width * source.Height;
        }

        /// <summary>
        /// The <see cref="Rectangle"/> extension method that determines if the rectangle is larger than the given size.
        /// </summary>
        /// <param name="source">The rectangle to compare.</param>
        /// <param name="comparison">The size to compare the rectangle against.</param>
        /// <returns>True if the rectangle is larger in both width and height, otherwise false.</returns>
        public static bool IsBiggerThan(this Rectangle source, Size comparison)
        {
            return source.Width > comparison.Width && source.Height > comparison.Height;
        }

        /// <summary>
        /// The <see cref="Rectangle"/> extension method that determines if the rectangle is larger than or equal to the given size.
        /// </summary>
        /// <param name="source">The rectangle to compare.</param>
        /// <param name="comparison">The size to compare the rectangle against.</param>
        /// <returns>True if the rectangle is larger than or equal to the given size, otherwise false.</returns>
        public static bool IsBiggerOrEqualThan(this Rectangle source, Size comparison)
        {
            return source.Width >= comparison.Width && source.Height >= comparison.Height;
        }

        /// <summary>
        /// Determines whether the source rectangle is smaller than the comparison size.
        /// </summary>
        /// <param name="source">The source rectangle to compare.</param>
        /// <param name="comparison">The size to compare with.</param>
        /// <returns>True if the source rectangle is smaller than the comparison size, otherwise false.</returns>
        public static bool IsSmallerThan(this Rectangle source, Size comparison)
        {
            return source.Width < comparison.Width && source.Height < comparison.Height;
        }

        /// <summary>
        /// Determines whether the source rectangle is smaller or equal to the comparison size.
        /// </summary>
        /// <param name="source">The rectangle to compare.</param>
        /// <param name="comparison">The size to compare the rectangle with.</param>
        /// <returns>True if the source rectangle is smaller or equal to the comparison size, otherwise false.</returns>
        public static bool IsSmallerOrEqualThan(this Rectangle source, Size comparison)
        {
            return source.Width <= comparison.Width && source.Height <= comparison.Height;
        }

        /// <summary>
        /// Calculates the area of intersection between two rectangles.
        /// </summary>
        /// <param name="outsider">The rectangle being tested for intersection.</param>
        /// <param name="insider">The rectangle to be compared with the outsider rectangle.</param>
        /// <returns>The area of the intersection between the two rectangles, or 0 if no intersection exists.</returns>
        public static long CalcIntersectedArea(this Rectangle outsider, Rectangle insider)
        {
            var intersected = Rectangle.Intersect(outsider, insider);
            return intersected.Area();
        }

        /// <summary>
        /// Returns the center point of the specified rectangle.
        /// </summary>
        /// <param name="source">The rectangle from which the center is calculated.</param>
        /// <returns>A <see cref="Point"/> that represents the center of the rectangle.</returns>
        public static Point Center(this Rectangle source)
        {
            return new Point(source.Left + (int)(source.Width / 2), source.Top + (int)(source.Height / 2));
        }

        /// <summary>
        /// Converts a <see cref="RectangleF"/> to a <see cref="Rectangle"/> by truncating the coordinates and dimensions to integers.
        /// </summary>
        /// <param name="rectangle">The <see cref="RectangleF"/> to convert.</param>
        /// <returns>A new <see cref="Rectangle"/> with integer values derived from the <see cref="RectangleF"/>.</returns>
        public static Rectangle ToInt(this RectangleF rectangle) =>
            new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);

        /// <summary>
        /// Converts a <see cref="Rectangle"/> to a <see cref="RectangleF"/> by converting the coordinates and dimensions to floats.
        /// </summary>
        /// <param name="rectangle">The <see cref="Rectangle"/> to convert.</param>
        /// <returns>A new <see cref="RectangleF"/> with floating-point values derived from the <see cref="Rectangle"/>.</returns>
        public static RectangleF ToFloat(this Rectangle rectangle) =>
            new RectangleF((float)rectangle.X, (float)rectangle.Y, (float)rectangle.Width, (float)rectangle.Height);

        /// <summary>
        /// Translates a rectangle by the specified translation vector, moving it to a new position.
        /// </summary>
        /// <param name="rectangle">The rectangle to translate.</param>
        /// <param name="translation">The translation vector to apply to the rectangle.</param>
        /// <returns>A new <see cref="Rectangle"/> that is translated by the specified vector.</returns>
        public static Rectangle TranslateTo(this Rectangle rectangle, Point translation)
        {
            return new Rectangle(
                rectangle.X - translation.X,
                rectangle.Y - translation.Y,
                rectangle.Width,
                rectangle.Height);
        }

        /// <summary>
        /// Translates a rectangle by the specified translation vector.
        /// </summary>
        /// <param name="rectangle">The rectangle to translate.</param>
        /// <param name="translation">The translation vector that specifies how much to move the rectangle.</param>
        /// <returns>A new <see cref="Rectangle"/> that is translated by the specified translation.</returns>
        public static Rectangle TranslateFrom(this Rectangle rectangle, Point translation)
        {
            return new Rectangle(
                rectangle.X + translation.X,
                rectangle.Y + translation.Y,
                rectangle.Width,
                rectangle.Height);
        }
    }
}
