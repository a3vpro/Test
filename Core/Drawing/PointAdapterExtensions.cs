using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using VisionNet.Core.Patterns;

namespace VisionNet.Core.Drawing
{
    /// <summary>
    /// Conversion class between 
    /// </summary>
    public static class PointAdapterExtensions
    {
        /// <summary>
        /// Converts a <see cref="System.Drawing.PointF"/> to a <see cref="PointD"/>.
        /// </summary>
        /// <param name="value">The single-precision point.</param>
        /// <returns>The double-precision <see cref="PointD"/>.</returns>

        public static PointD ToPointD(this PointF value)
        {
            var adapter = new PointAdapter();
            return adapter.Convert(value);
        }

        /// <summary>
        /// Converts a <see cref="PointD"/> to a <see cref="System.Drawing.PointF"/>.
        /// </summary>
        /// <param name="value">The double-precision point.</param>
        /// <returns>The single-precision <see cref="System.Drawing.PointF"/>.</returns>
        public static PointF ToPointF(this PointD value)
        {
            var adapter = new PointAdapter();
            return adapter.Convert(value);
        }

        /// <summary>
        /// Converts a <see cref="System.Drawing.PointF"/> to a <see cref="PointD"/>.
        /// </summary>
        /// <param name="value">The single-precision point.</param>
        /// <returns>The double-precision <see cref="PointD"/>.</returns>

        public static List<PointD> ToPointD(this List<PointF> value)
        {
            var adapter = new PointAdapter();
            return value.Select(v => adapter.Convert(v)).ToList();
        }

        /// <summary>
        /// Converts a <see cref="PointD"/> to a <see cref="System.Drawing.PointF"/>.
        /// </summary>
        /// <param name="value">The double-precision point.</param>
        /// <returns>The single-precision <see cref="System.Drawing.PointF"/>.</returns>
        public static List<PointF> ToPointF(this List<PointD> value)
        {
            var adapter = new PointAdapter();
            return value.Select(v => adapter.Convert(v)).ToList();
        }
    }
}
