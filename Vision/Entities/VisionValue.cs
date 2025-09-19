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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel;
using VisionNet.Core.Serialization;
using VisionNet.IO.FilePersistence;

namespace VisionNet.Vision.Core.Entities
{
  [Serializable]
  public class VisionValue : FileStorer<JSONSerializer>
  {
    [Browsable(true)]
    [ReadOnly(false)]
    [Category("VisionValue")]
    [Description("Name of the value.")]
    [DisplayName(nameof(Name))]
    public string Name { get; set; } = "NoName";

    [Browsable(true)]
    [ReadOnly(false)]
    [Category("ValueResult")]
    [Description("Type of the value.")]
    [DisplayName(nameof(Type))]
    [JsonConverter(typeof (StringEnumConverter))]
    public VisionValueType Type { get; set; }

    [Browsable(true)]
    [ReadOnly(false)]
    [Category("ValueResult")]
    [Description("Value.")]
    [DisplayName(nameof(Value))]
    public object Value { get; set; }
  }
}
