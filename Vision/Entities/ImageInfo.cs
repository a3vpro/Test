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
using System.ComponentModel;
using Newtonsoft.Json;
using VisionNet.Core.Abstractions;
using VisionNet.Core.Serialization;
using VisionNet.Image;
using VisionNet.IO.FilePersistence;

namespace VisionNet.Vision.Core.Entities
{
    [Serializable]
    public class ImageInfo : FileStorer<JSONSerializer>, INamed
    {
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ImageInfo")]
        [Description("Name of the image.")]
        [DisplayName(nameof(Name))]
        public string Name { get; set; } = "NoName";

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ImageInfo")]
        [Description("File name of the image.")]
        [DisplayName(nameof(FileName))]
        public string FileName { get; set; }

        [Browsable(false)]
        [JsonIgnore]
        public IImage Image { get; set; }

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ImageInfo")]
        [Description("Source of the image.")]
        [DisplayName(nameof(Source))]
        public string Source { get; set; }

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ImageInfo")]
        [Description("Acquisition moment.")]
        [DisplayName(nameof(AcquisitionMoment))]
        public DateTime AcquisitionMoment { get; set; }
    }
}