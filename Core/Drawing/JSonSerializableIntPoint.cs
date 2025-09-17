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
    /// Represents a point with integer values for the X and Y coordinates.
    /// This class is serializable and intended to be used for JSON serialization.
    /// </summary>
    [Serializable]
    public class JSonSerializableIntPoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JSonSerializableIntPoint"/> class with default values.
        /// </summary>
        public JSonSerializableIntPoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSonSerializableIntPoint"/> class with the specified X and Y coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate of the point.</param>
        /// <param name="y">The y-coordinate of the point.</param>
        /// <returns> A jsonserializableintpoint</returns>
        public JSonSerializableIntPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Gets or sets the x-coordinate of the point.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Point coordinates")]
        [Description("Horizontal coordinates")]
        [DisplayName(nameof(X))]
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the y-coordinate of the point.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Point coordinates")]
        [Description("Vertical coordinates")]
        [DisplayName(nameof(Y))]
        public int Y { get; set; }
    }
}
