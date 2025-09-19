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
  public class ProductResult : ProductInfo
    {
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ProductInfoResult")]
        [Description("Identification number of product.")]
        [DisplayName(nameof(Index))]
        public long Index { get; set; }

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ProductInfoResult")]
        [Description("Source of the inspections.")]
        [DisplayName(nameof(Source))]
        public string Source { get; set; } = "NoLine";

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ProductInfoResult")]
        [Description("The inspections are enabled / disabled.")]
        [DisplayName(nameof(Enabled))]
        public bool Enabled { get; set; }

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ProductInfoResult")]
        [Description("Result of the inspections.")]
        [DisplayName(nameof(Result))]
        public bool Result { get; set; }

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ProductInfoResult")]
        [Description("Inspecctions executed successfull.")]
        [DisplayName(nameof(Success))]
        public bool Success { get; set; }

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ProductInfoResult")]
        [Description("Duration of the execution in milliseconds.")]
        [DisplayName(nameof(ProcessTime))]
        public double ProcessTime { get; set; }

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ProductInfoResult")]
        [Description("Error description.")]
        [DisplayName(nameof(Error))]
        public string Error { get; set; } = "";

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ProductInfoResult")]
        [Description("Time of the execution.")]
        [DisplayName(nameof(DateTime))]
        public DateTime DateTime { get; set; } = DateTime.Now;

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ProductInfoResult")]
        [Description("Bypass mode.")]
        [DisplayName(nameof(ByPass))]
        public bool ByPass { get; set; }

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ProductInfoResult")]
        [Description("Some inspecion has a forced value.")]
        [DisplayName(nameof(Forced))]
        public bool Forced { get; set; }

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ProductResult")]
        [Description("List of the images.")]
        [DisplayName(nameof(Images))]
        //[Editor(typeof (CollectionEditor), typeof (UITypeEditor))]
        public List<ImageInfo> Images { get; set; }

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ProductResult")]
        [Description("List of the inspections done.")]
        [DisplayName(nameof(Inspections))]
        //[Editor(typeof (CollectionEditor), typeof (UITypeEditor))]
        public List<InspectionResult> Inspections { get; set; }
    }
}
