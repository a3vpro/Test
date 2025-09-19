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
using Newtonsoft.Json;
using VisionNet.Core.Abstractions;
using VisionNet.Core.Serialization;
using VisionNet.Core.Types;
using VisionNet.IO.FilePersistence;

namespace VisionNet.Vision.Core
{
    /// <summary>
    /// Represents the result of a product inspection process.
    /// </summary>
    [Serializable]
    public class ProductResult : OptionsBase
    {
        /// <summary>
        /// Gets or sets general information about the product inspection results.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category(nameof(ProductResult))]
        [Description("General information of the result of product inspections.")]
        [DisplayName(nameof(Info))]
        public ProductInfoResult Info { get; set; } = new ProductInfoResult(); // TODO: Rename appropriately

        /// <summary>
        /// Gets or sets the parametrization details of the inspection process.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category(nameof(ProductResult))]
        [Description("Parametrization of the inspection process.")]
        [DisplayName(nameof(Features))]
        public ProductFeatures Features { get; set; } = new ProductFeatures();

        /// <summary>
        /// Gets or sets the list of images related to the product inspection.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category(nameof(ProductResult))]
        [Description("List of the images.")]
        [DisplayName(nameof(Images))]
        public List<ImageResult> Images { get; set; } = new List<ImageResult>();

        /// <summary>
        /// Gets or sets the list of inspections performed on the product.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category(nameof(ProductResult))]
        [Description("List of the inspections done.")]
        [DisplayName(nameof(Inspections))]
        public List<InspectionResult> Inspections { get; set; } = new List<InspectionResult>();

        /// <summary>
        /// Gets or sets the current status of the product processing.
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public ProductProcessStatus Status { get; set; } = ProductProcessStatus.Init;

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <see cref="ProductResult"/> object that is a copy of this instance.</returns>
        public object Clone()
        {
            return new ProductResult
            {
                //Index = Index,
                Info = Info?.Clone() as ProductInfoResult,
                Features = Features?.Clone() as ProductFeatures,
                Images = Images?.Where(img => img != null).Select(img => img.Clone() as ImageResult).ToList(),
                Inspections = Inspections?.Where(ins => ins != null).Select(ins => ins.Clone() as InspectionResult).ToList(),
                Status = Status
            };
        }

        /// <summary>
        /// Gets the default.
        /// </summary>
        [JsonIgnore]
        [Browsable(false)]
        public override IOptions Default =>
            new ProductResult
            {
                Info = new ProductInfoResult
                {
                    DateTime = DateTime.Now,
                    Enabled = true,
                    Success = true,
                    Result = true,
                    Id = Guid.NewGuid(),
                },
                Inspections = new List<InspectionResult>
                {
                    new InspectionResult
                    {
                        Name = "ForeignObjects",
                        Enabled = true,
                        Success = true,
                        PrevResult = true,
                        Result = true,
                        Measurables = new List<NamedValue>
                        {
                            new NamedValue
                            {
                                Name = "Score",
                                Type = BasicTypeCode.FloatingPointNumber,
                                Value = 1.0
}
                        }
                    },
                    new InspectionResult
                    {
                        Name = "MeatClassifier",
                        Enabled = true,
                        Success = true,
                        PrevResult = true,
                        Result = true,
                        Measurables = new List<NamedValue>
                        {
                            new NamedValue
                            {
                                Name = "Score",
                                Type = BasicTypeCode.FloatingPointNumber,
                                Value = 2.0
                            }
                        }
                    }
                }
            };

        /// <summary>
        /// Gets the default.
        /// </summary>
        public static ProductResult DefaultInstance => new ProductResult().Default as ProductResult;
    }
}
