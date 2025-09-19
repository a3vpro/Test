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
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VisionNet.Core;
using VisionNet.Core.Abstractions;
using VisionNet.Core.Patterns;
using VisionNet.Core.Serialization;
using VisionNet.Core.Types;
using VisionNet.Image;
using VisionNet.IO.FilePersistence;

namespace VisionNet.Vision.Core
{
    [Serializable]
    public class VisionFunctionOptions : OptionsBase, IRuntimeOptions
    {
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Vision")]
        [Description("Unique identifier of the vision function")]
        [DisplayName(nameof(Index))]
        public string Index { get; set; } = "None";

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Vision")]
        [Description("Name of the vision function")]
        [DisplayName(nameof(Name))]
        public string Name { get; set; } = "None";

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Vision")]
        [Description("Description of the vision function")]
        [DisplayName(nameof(Description))]
        public string Description { get; set; } = "Without description";

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Vision")]
        [Description("Indicates the type of vision function")]
        [DisplayName(nameof(VisionFunctionType))]
        [JsonConverter(typeof(StringEnumConverter))]
        public VisionFunctionType VisionFunctionType { get; set; } = VisionFunctionType.Custom;

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Vision")]
        [Description("Enables or disables the execution of the vision function")]
        [DisplayName(nameof(Enabled))]
        public bool Enabled { get; set; } = true;

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Vision")]
        [Description("Enables or disables the inclusion of the vision function results in the general result")]
        [DisplayName(nameof(IncludeInResult))]
        public bool IncludeInResult { get; set; } = true;

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("InspectionResult")]
        [Description("The inspection is include in statistics.")]
        [DisplayName(nameof(IncludeInStats))]
        public bool IncludeInStats { get; set; } = true;

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("InspectionResult")]
        [Description("The inspection is separated by the class type result. Usual in DL segmentation results")]
        [DisplayName(nameof(IsSegmentationInspection))]
        public bool IsSegmentationInspection { get; set; } = false;

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Vision")]
        [Description("Defines the number of simultaneous functions to be exectued simultaneously")]
        [DisplayName(nameof(MaxDegreeOfParallelism))]
        public int MaxDegreeOfParallelism { get; set; } = 1;

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Vision")]
        [Description("Defines the number of queued execution calls")]
        [DisplayName(nameof(BoundedCapacity))]
        public int BoundedCapacity { get; set; } = 1;

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Vision")]
        [Description("List of parameters used by the vision function")]
        [DisplayName(nameof(Parameters))]
        public List<ParameterOptions> Parameters { get; set; } = new List<ParameterOptions>();

        [JsonIgnore]
        [Browsable(false)]
        /// <summary>
        /// Configuración por defecto de una función de visión.
        /// </summary>
        public override IOptions Default =>
    new VisionFunctionOptions
    {
        Index = "TestProcedure",
        Name = "TestProcedure",
        Description = "TestProcedure",
        Enabled = true,
        Parameters = new List<ParameterOptions>
        {
            DefaultParameterOptionsBasic("Enabled", BasicTypeCode.Boolean, false, ParameterSource.Constant, ParameterDirection.Input, true),
            DefaultParameterOptionsBasic("PrevResult", BasicTypeCode.Boolean, false, ParameterSource.Constant, ParameterDirection.Input, true),
            DefaultParameterOptionsBasic("enteroIn", BasicTypeCode.IntegerNumber, false, ParameterSource.Runtime, ParameterDirection.Input, 0),
            DefaultParameterOptionsBasic("realIn", BasicTypeCode.FloatingPointNumber, false, ParameterSource.Runtime, ParameterDirection.Input, 0d),
            DefaultParameterOptionsBasic("stringIn", BasicTypeCode.String, false, ParameterSource.Runtime, ParameterDirection.Input, string.Empty),
            DefaultParameterOptionsBasic("vectorIn", BasicTypeCode.IntegerNumber, true, ParameterSource.Runtime, ParameterDirection.Input, new List<int>()),
            DefaultParameterOptionsBasic("imagenIn", BasicTypeCode.Image, false, ParameterSource.Runtime, ParameterDirection.Input, null),
            DefaultParameterOptionsBasic("vectorImagenIn", BasicTypeCode.Image, true, ParameterSource.Runtime, ParameterDirection.Input, new List<IImage>()),

            DefaultParameterOptionsBasic("Enabled", BasicTypeCode.Boolean, false, ParameterSource.Constant, ParameterDirection.Output, true),
            DefaultParameterOptionsBasic("PrevResult", BasicTypeCode.Boolean, false, ParameterSource.Constant, ParameterDirection.Output, true),
            DefaultParameterOptionsBasic("Result", BasicTypeCode.Boolean, false, ParameterSource.Constant, ParameterDirection.Output, true),
            DefaultParameterOptionsBasic("Success", BasicTypeCode.Boolean, false, ParameterSource.Constant, ParameterDirection.Output, true),
            DefaultParameterOptionsBasic("ProcessTime", BasicTypeCode.FloatingPointNumber, false, ParameterSource.Constant, ParameterDirection.Output, 0d),
            DefaultParameterOptionsBasic("Error", BasicTypeCode.String, false, ParameterSource.Constant, ParameterDirection.Output, string.Empty),
            DefaultParameterOptionsBasic("enteroOut", BasicTypeCode.IntegerNumber, false, ParameterSource.Runtime, ParameterDirection.Output, 0),
            DefaultParameterOptionsBasic("realOut", BasicTypeCode.FloatingPointNumber, false, ParameterSource.Runtime, ParameterDirection.Output, 0d),
            DefaultParameterOptionsBasic("stringOut", BasicTypeCode.String, false, ParameterSource.Runtime, ParameterDirection.Output, string.Empty),
            DefaultParameterOptionsBasic("vectorOut", BasicTypeCode.IntegerNumber, true, ParameterSource.Runtime, ParameterDirection.Output, new List<int>()),
            DefaultParameterOptionsBasic("imagenOut", BasicTypeCode.Image, false, ParameterSource.Runtime, ParameterDirection.Output, null),
            DefaultParameterOptionsBasic("vectorImagenOut", BasicTypeCode.Image, true, ParameterSource.Runtime, ParameterDirection.Output, new List<IImage>()),
        }
    };

        /// <summary>
        /// Instancia estática por defecto de una función de visión.
        /// </summary>
        public static VisionFunctionOptions DefaultInstance => new VisionFunctionOptions().Default as VisionFunctionOptions;

        protected static List<ParameterOptions> DefaultParameterOptions()
        {
            return new List<ParameterOptions>
            {
                DefaultParameterOptionsBasic(nameof(InputParametersCollection.Enabled), BasicTypeCode.Boolean, false, ParameterSource.Constant, ParameterDirection.Input, true),
                DefaultParameterOptionsBasic(nameof(InputParametersCollection.PrevResult), BasicTypeCode.Boolean, false, ParameterSource.Constant, ParameterDirection.Input, false),

                DefaultParameterOptionsBasic(nameof(OutputParametersCollection.Enabled), BasicTypeCode.Boolean, false, ParameterSource.Constant, ParameterDirection.Output, true),
                DefaultParameterOptionsBasic(nameof(OutputParametersCollection.PrevResult), BasicTypeCode.Boolean, false, ParameterSource.Constant, ParameterDirection.Output, false),
                DefaultParameterOptionsBasic(nameof(OutputParametersCollection.Result), BasicTypeCode.Boolean, false, ParameterSource.Constant, ParameterDirection.Output, true),
                DefaultParameterOptionsBasic(nameof(OutputParametersCollection.Success), BasicTypeCode.Boolean, false, ParameterSource.Constant, ParameterDirection.Output, false),
                DefaultParameterOptionsBasic(nameof(OutputParametersCollection.ProcessTime), BasicTypeCode.FloatingPointNumber, false, ParameterSource.Constant, ParameterDirection.Output, 0d),
                DefaultParameterOptionsBasic(nameof(OutputParametersCollection.Error), BasicTypeCode.String, false, ParameterSource.Constant, ParameterDirection.Output, string.Empty),
            };
        }

        protected static ParameterOptions DefaultParameterOptionsBasic(string index, BasicTypeCode type, bool isArray, ParameterSource parameterSource, ParameterDirection direction, object defaultValue, ParameterScope scope = ParameterScope.Execution)
        {
            return new ParameterOptions
            {
                Index = index,
                ExternalIndex = index,
                Name = index,
                Description = index,
                DataType = type,
                DefaultValue = defaultValue,
                IsArray = isArray,
                Scope = scope,
                Direction = direction,
                Source = parameterSource,
                TypeConversionPreferences = TypeConversionPreferences.None,
            };
        }
    }
}
