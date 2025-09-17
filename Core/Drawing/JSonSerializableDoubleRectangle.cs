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
    /// Represents a rectangle with double precision values for the X, Y coordinates, Width, and Height properties.
    /// This class is serializable and intended to be used for JSON serialization.
    /// </summary>
    [Serializable]
    public class JSonSerializableDoubleRectangle
    {
        /// <summary> The JSonSerializableDoubleRectangle function is a constructor that creates an instance of the JSonSerializableDoubleRectangle class.</summary>
        /// <returns> A jsonserializabledoublerectangle.</returns>
        public JSonSerializableDoubleRectangle()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSonSerializableDoubleRectangle"/> class with the specified X, Y, Width, and Height values.
        /// </summary>
        /// <param name="x">The x-coordinate of the top-left corner of the rectangle.</param>
        /// <param name="y">The y-coordinate of the top-left corner of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <returns> A jsonserializabledoublerectangle.</returns>
        public JSonSerializableDoubleRectangle(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Gets or sets the x-coordinate of the top-left corner of the rectangle.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Rectangle coordinates")]
        [Description("Horizontal coordinates")]
        [DisplayName(nameof(X))]
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the y-coordinate of the top-left corner of the rectangle.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Rectangle coordinates")]
        [Description("Vertical coordinates")]
        [DisplayName(nameof(Y))]
        public double Y { get; set; }

        /// <summary>
        /// Gets or sets the width of the rectangle.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Rectangle coordinates")]
        [Description("Width size")]
        [DisplayName(nameof(Width))]
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the rectangle.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Rectangle coordinates")]
        [Description("Height size")]
        [DisplayName(nameof(Height))]
        public double Height { get; set; }
    }
}
