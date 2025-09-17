using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using VisionNet.Core.Patterns;

namespace VisionNet.Core.Drawing
{
    /// <summary>
    /// Adapter helpers to convert between <see cref="PointD"/> and <see cref="System.Drawing.PointF"/>.
    /// </summary>
    public class PointAdapter: IBidirectionalAdapter<PointF, PointD>
    {
        /// <summary>
        /// Converts a <see cref="PointD"/> to a <see cref="System.Drawing.PointF"/>.
        /// </summary>
        /// <param name="pointD">The double-precision point.</param>
        /// <returns>The single-precision <see cref="System.Drawing.PointF"/>.</returns>
        public PointF Convert(PointD pointD)
        {
            return new PointF((float)pointD.X, (float)pointD.Y);
        }

        /// <summary>
        /// Converts a <see cref="System.Drawing.PointF"/> to a <see cref="PointD"/>.
        /// </summary>
        /// <param name="pointF">The single-precision point.</param>
        /// <returns>The double-precision <see cref="PointD"/>.</returns>
        public PointD Convert(PointF pointF)
        {
            return new PointD((double)pointF.X, (double)pointF.Y);
        }


    }
}
