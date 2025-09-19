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
using System.ComponentModel.Design;
using System.Drawing.Design;
using VisionNet.Core.Serialization;
using VisionNet.IO.FilePersistence;

namespace VisionNet.Vision.Core.Entities
{
  [Serializable]
  public class ProductInfo : ParametersCollection
  {
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Product")]
        [Description("Identification number of product.")]
        [DisplayName(nameof(Index))]
        public long Index { get; set; }

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Product")]
        [Description("Source of the inspections.")]
        [DisplayName(nameof(Source))]
        public string Source { get; set; } = "NoLine";

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ProductInfoReProductsult")]
        [Description("The inspections are enabled / disabled.")]
        [DisplayName(nameof(Enabled))]
        public bool Enabled { get; set; }

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Product")]
        [Description("Bypass mode.")]
        [DisplayName(nameof(ByPass))]
        public bool ByPass { get; set; }

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Product")]
        [Description("Some inspecion has a forced value.")]
        [DisplayName(nameof(Forced))]
        public bool Forced { get; set; }
    }
}
