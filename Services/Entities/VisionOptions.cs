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
using VisionNet.Core.Serialization;
using VisionNet.IO.FilePersistence;
using Newtonsoft.Json;
using VisionNet.Vision.Core;
using VisionNet.Core.Abstractions;

namespace VisionNet.Vision.Services

{
    [Serializable]
    public class VisionOptions : OptionsBase
    {
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Vision")]
        [Description("Amount of simultaneous instances of VisionPipeline.")]
        [DisplayName(nameof(PipelinePoolSize))]
        //[Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
        public int PipelinePoolSize { get; set; } = 1;

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Vision")]
        [Description("List of all the vision functions.")]
        [DisplayName(nameof(VisionFunctions))]
        //[Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
        public List<VisionFunctionOptions> VisionFunctions { get; set; } = new List<VisionFunctionOptions>();

        [JsonIgnore]
        [Browsable(false)]
        /// <summary>
        /// Configuración por defecto de opciones de visión.
        /// </summary>
        public override IOptions Default =>
    new VisionOptions
    {
        VisionFunctions = new List<VisionFunctionOptions>
        {
            VisionFunctionOptions.DefaultInstance
        }
    };

        /// <summary>
        /// Instancia estática por defecto de opciones de visión.
        /// </summary>
        public static VisionOptions DefaultInstance => new VisionOptions().Default as VisionOptions;
    }
}