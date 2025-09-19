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
using System.ComponentModel;
using VisionNet.Vision.Core;

namespace VisionNet.Vision.VisionPro
{
    [Serializable]
    public class VisionProFunctionOptions : VisionFunctionOptions
    {
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Vision")]
        [Description("Full path of the file of the VisionPro CogToolBlock (.vpp file)")]
        [DisplayName(nameof(ToolBlockFilePath))]
        public string ToolBlockFilePath { get; set; } = "None";
    }
}
