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
using System.ComponentModel;

namespace VisionNet.Drawing
{
    /// <summary>
    /// Represents a point in a two-dimensional space with serializable double precision coordinates.
    /// </summary>
    [Serializable]
    public class JSonSerializableDoublePoint
    {
        /// <summary>
        /// Gets or sets the horizontal coordinate of the point.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Point coordinates")]
        [Description("Horizontal coordinates")]
        [DisplayName(nameof(X))]
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the vertical coordinate of the point.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Point coordinates")]
        [Description("Vertical coordinates")]
        [DisplayName(nameof(Y))]
        public double Y { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSonSerializableDoublePoint"/> class with default values.
        /// </summary>
        /// <returns>A new instance of <see cref="JSonSerializableDoublePoint"/> with X and Y set to zero.</returns>
        public JSonSerializableDoublePoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSonSerializableDoublePoint"/> class with the specified coordinates.
        /// </summary>
        /// <param name="x">The horizontal coordinate of the point.</param>
        /// <param name="y">The vertical coordinate of the point.</param>
        /// <returns>A new instance of <see cref="JSonSerializableDoublePoint"/> with X and Y set to the specified values.</returns>
        public JSonSerializableDoublePoint(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
