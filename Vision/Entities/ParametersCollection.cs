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
using VisionNet.Core.Patterns;
using VisionNet.Core.Serialization;
using VisionNet.IO.FilePersistence;

namespace VisionNet.Vision.Core.Entities
{
    [Serializable]
    public class ParametersCollection: FileStorer<JSONSerializer>
    {
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ParametersCollection")]
        [Description("Parameters are validated.")]
        [DisplayName(nameof(ParametersValid))]
        public bool ParametersValid { get; set; }

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ParametersCollection")]
        [Description("Error description.")]
        [DisplayName(nameof(ParametersError))]
        public string ParametersError { get; set; } = "";

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ParametersCollection")]
        [Description("List of all parameters.")]
        [DisplayName(nameof(Parameters))]
        //[Editor(typeof (CollectionEditor), typeof (UITypeEditor))]
        public List<VisionValue> Parameters { get; set; } = new List<VisionValue>();

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ProductResult")]
        [Description("List of the images.")]
        [DisplayName(nameof(Images))]
        //[Editor(typeof (CollectionEditor), typeof (UITypeEditor))]
        public List<ImageInfo> Images { get; set; }
    }
}
