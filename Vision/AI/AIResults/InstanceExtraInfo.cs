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
using System.Drawing;
using Newtonsoft.Json;

namespace VisionNet.Vision.AI
{
    [Serializable]
    public class InstanceExtraInfo
    {
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Vision")]
        [Description("Description of the class")]
        [DisplayName(nameof(Description))]
        public string Description { get; set; }

        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Vision")]
        [Description("Alias of the class")]
        [DisplayName(nameof(Alias))]
        public string Alias { get; set; }

        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Vision")]
        [Description("Agrupation of the class")]
        [DisplayName(nameof(Group))]
        public string Group { get; set; }

        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Vision")]
        [Description("Icon identifier")]
        [DisplayName(nameof(Icon))]
        public string Icon { get; set; }

        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Vision")]
        [Description("Color name")]
        [DisplayName(nameof(Color))]
        public string Color { get; set; }

        [JsonIgnore]
        protected static InstanceExtraInfo Default
        {
            get
            {
                return new InstanceExtraInfo
                {
                    Alias = "",
                    Group = "",
                    Icon = "",
                    Color = "Red",
                };
            }
        }
    }
}
