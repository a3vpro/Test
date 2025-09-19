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
using VisionNet.Core.Abstractions;
using VisionNet.Core.Serialization;
using VisionNet.IO.FilePersistence;

namespace VisionNet.Vision.Core.Entities
{
    [Serializable]
    public class InspectionInfo : ParametersCollection, INamed, IDisableable
    {
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("InspectionResult")]
        [Description("Name of the inspection.")]
        [DisplayName(nameof(Name))]
        public string Name { get; set; } = "NoName";

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("InspectionResult")]
        [Description("The inspection is enabled / disabled.")]
        [DisplayName(nameof(Enabled))]
        public bool Enabled { get; set; }

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("InspectionResult")]
        [Description("Previous result of the inspection.")]
        [DisplayName(nameof(PrevResult))]
        public bool PrevResult { get; set; }
    }
}