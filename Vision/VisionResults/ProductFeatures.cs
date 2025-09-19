//----------------------------------------------------------------------------
// Author           : Álvaro Ibáñez del Pino
// NickName         : aibanez
// Created          : 28-08-2021
//
// Last Modified By : aibanez
// Last Modified On : 23-06-2023
// Description      : v1.7.0
//
// Copyright        : (C)  2023 by Sothis/Nunsys. All rights reserved.            
//-----------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using VisionNet.Core.Types;

namespace VisionNet.Vision.Core
{
    /// <summary>
    /// All the input information of the product or piece to process or processed
    /// </summary>
    [Serializable]
    /// <summary>
    /// Represents the features of a product, including its identification, execution parameters, and error information.
    /// </summary>
    public class ProductFeatures : ICloneable
    {
        /// <summary>
        /// Identification of the location of the system (conveyor, cell, factory...)
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category(nameof(ProductFeatures))]
        [Description("Identification of the location of the system (conveyor, cell, factory...).")]
        [DisplayName(nameof(Source))]
        public string Source { get; set; } = "NoLine";

        /// <summary>
        /// Identification of the piece or product. (PLC index).
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ProductResult")]
        [Description("Identification of the piece or product. (PLC index).")]
        [DisplayName(nameof(ExternalIndex))]
        public long ExternalIndex { get; set; }

        /// <summary>
        /// Start time of execution.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category(nameof(ProductFeatures))]
        [Description("Start time of execution.")]
        [DisplayName(nameof(DateTime))]
        public DateTime DateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Indicates whether the parameters are valid.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ParametersResult")]
        [Description("Parameters are valid.")]
        [DisplayName(nameof(Valid))]
        public bool Valid { get; set; }

        /// <summary>
        /// Description of any error that occurred.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ParametersResult")]
        [Description("Error description.")]
        [DisplayName(nameof(Error))]
        public string Error { get; set; } = "";

        /// <summary>
        /// List of all the features of the product. It can be used to modify the behavior of the execution.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ParametersResult")]
        [Description("List of all the features of the product (It can be used to modify the behaviour of the execution).")]
        [DisplayName(nameof(Parameters))]
        public List<NamedValue> Parameters { get; set; } = new List<NamedValue>();

        /// <summary>
        /// Creates a deep copy of the current <see cref="ProductFeatures"/> instance.
        /// </summary>
        /// <returns>A new instance of <see cref="ProductFeatures"/> with the same data.</returns>
        public object Clone()
        {
            return new ProductFeatures
            {
                Source = Source,
                ExternalIndex = ExternalIndex,
                DateTime = DateTime,
                Valid = Valid,
                Error = Error,
                Parameters = Parameters?.Where(p => p != null).Select(p => p.Clone() as NamedValue).ToList()
            };
        }
    }
}
