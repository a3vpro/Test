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

namespace VisionNet.Vision.Core
{
    [Serializable]
    public class InspectionStatisticsOptions
    {
        /// <summary>
        /// Name of the inspection
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category(nameof(InspectionStatisticsOptions))]
        [Description("Name of the inspection.")]
        [DisplayName(nameof(Name))]
        public string Name { get; set; } = "Inspection";

        /// <summary>
        /// Global statistics configuration
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category(nameof(InspectionStatisticsOptions))]
        [Description("Global options.")]
        [DisplayName(nameof(Global))]
        public SingleStatisticsOptions Global { get; set; } = new SingleStatisticsOptions();

        /// <summary>
        /// Timed statistics configuration
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category(nameof(InspectionStatisticsOptions))]
        [Description("Timed options.")]
        [DisplayName(nameof(Timed))]
        //[Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
        
        public List<TimedStatisticsOptions> Timed { get; set; } = new List<TimedStatisticsOptions>();
    }
}