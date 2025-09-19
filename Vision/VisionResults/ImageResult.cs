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
using System.Linq;
using Newtonsoft.Json;
using VisionNet.Core.Types;
using VisionNet.Image;

namespace VisionNet.Vision.Core
{
    /// <summary>
    /// Represents the result of an image acquisition process, including metadata and inspection steps.
    /// </summary>
    [Serializable]
    public class ImageResult : ICloneable
    {
        /// <summary>
        /// Gets or sets the name of the image result.
        /// </summary>
        public string Name { get; set; } = "NoName";

        /// <summary>
        /// Gets or sets the file name associated with the image result.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the source type of the image result.
        /// </summary>
        public ImageResultSourceType SourceType { get; set; } = ImageResultSourceType.AcquisitionDevice;

        /// <summary>
        /// Gets or sets the name of the image source.
        /// </summary>
        public string SourceName { get; set; }

        /// <summary>
        /// Gets or sets the moment when the image was acquired.
        /// </summary>
        public DateTime AcquisitionMoment { get; set; }

        /// <summary>
        /// Gets or sets the saving status of the image result.
        /// </summary>
        public ImageSavingStatus SavingStatus { get; set; } = ImageSavingStatus.Initial;

        /// <summary>
        /// Gets or sets the image associated with this result. This property is ignored during JSON serialization.
        /// </summary>
        [JsonIgnore]
        public IImage Image { get; set; }

        /// <summary>
        /// Gets or sets the list of inspection steps performed on the image result. This property is ignored during JSON serialization.
        /// </summary>
        [JsonIgnore]
        public List<NamedValue> InspectionStep { get; set; } = new List<NamedValue>();

        /// <summary>
        /// Creates a deep copy of the current <see cref="ImageResult"/> instance.
        /// </summary>
        /// <returns>A new instance of <see cref="ImageResult"/> with the same data.</returns>
        public object Clone()
        {
            return new ImageResult
            {
                Name = Name,
                FileName = FileName,
                SourceType = SourceType,
                SourceName = SourceName,
                AcquisitionMoment = AcquisitionMoment,
                SavingStatus = SavingStatus,
                Image = Image?.Clone() as IImage, // Clones the image if not null
                InspectionStep = InspectionStep?.Where(p => p != null).Select(p => p.Clone() as NamedValue).ToList()
            };
        }
    }
}