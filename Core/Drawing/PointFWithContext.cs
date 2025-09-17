using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace VisionNet.Core.Dawing
{
    /// <summary>
    /// Represents a point with associated context information of a generic type.
    /// This class allows associating a point with any type of context data.
    /// </summary>
    /// <typeparam name="T">The type of context information associated with the point.</typeparam>
    public class PointFWithContext<T>
    {
        /// <summary>
        /// Gets or sets the point associated with the context.
        /// </summary>
        public PointF Point { get; set; }

        /// <summary>
        /// Gets or sets the context information associated with the point.
        /// </summary>
        public T Context { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PointFWithContext{T}"/> class with the specified point and context.
        /// </summary>
        /// <param name="point">The point to associate with the context.</param>
        /// <param name="context">The context information to associate with the point.</param>
        public PointFWithContext(PointF point, T context)
        {
            Point = point;
            Context = context;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PointFWithContext{T}"/> class with the specified coordinates and context.
        /// </summary>
        /// <param name="x">The x-coordinate of the point.</param>
        /// <param name="y">The y-coordinate of the point.</param>
        /// <param name="context">The context information to associate with the point.</param>
        public PointFWithContext(float x, float y, T context)
        {
            Point = new PointF(x, y);
            Context = context;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="PointFWithContext{T}"/> instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns><c>true</c> if the specified object is equal to the current instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is PointFWithContext<T>))
                return false;

            PointFWithContext<T> other = (PointFWithContext<T>)obj;
            return this.Point.X == other.Point.X && this.Point.Y == other.Point.Y;
        }

        /// <summary>
        /// Returns the hash code for the current <see cref="PointFWithContext{T}"/> instance.
        /// </summary>
        /// <returns>A hash code for the current instance.</returns>
        public override int GetHashCode()
        {
            return Point.X.GetHashCode() ^ Point.Y.GetHashCode();
        }
    }
}
