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
    public class InspectionResult : InspectionInfo
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
        [Description("Result of the inspection.")]
        [DisplayName(nameof(Result))]
        public bool Result { get; set; }

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("InspectionResult")]
        [Description("Inspecction executed successfull.")]
        [DisplayName(nameof(Success))]
        public bool Success { get; set; }

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("InspectionResult")]
        [Description("Previous result of the inspection.")]
        [DisplayName(nameof(PrevResult))]
        public bool PrevResult { get; set; }

        [Browsable(false)]
        public ResultStatus Status => ResultStatusHelper.Create(Result, Enabled, Success, PrevResult);

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("InspectionResult")]
        [Description("Duration of the execution in milliseconds.")]
        [DisplayName(nameof(ProcessTime))]
        public double ProcessTime { get; set; }

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("InspectionResult")]
        [Description("Error description.")]
        [DisplayName(nameof(Error))]
        public string Error { get; set; } = "";

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("InspectionResult")]
        [Description("List of all measurables.")]
        [DisplayName(nameof(Measurables))]
        //[Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
        public List<VisionValue> Measurables { get; set; } = new List<VisionValue>();
    }
}