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

namespace VisionNet.Core.Types
{
    [Serializable]
    /// <summary>
    /// Represents a named value with a specified type and associated value.
    /// </summary>
    public class NamedValue : ICloneable
    {
        /// <summary>
        /// Gets or sets the name of the value.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category(nameof(NamedValue))]
        [Description("Name of the value.")]
        [DisplayName(nameof(Name))]
        public string Name { get; set; } = "NoName";

        /// <summary>
        /// Gets or sets the type of the value.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category(nameof(NamedValue))]
        [Description("Type of the value.")]
        [DisplayName(nameof(Type))]
        [JsonConverter(typeof(StringEnumConverter))]
        public BasicTypeCode Type { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category(nameof(NamedValue))]
        [Description("Value.")]
        [DisplayName(nameof(Value))]
        public object Value { get; set; }

        /// <summary>
        /// Creates a deep copy of the current <see cref="NamedValue"/> instance.
        /// </summary>
        /// <returns>A new instance of <see cref="NamedValue"/> with the same data.</returns>
        public object Clone()
        {
            return new NamedValue
            {
                Name = Name,
                Type = Type,
                Value = Value is ICloneable cloneable ? cloneable.Clone() : Value
            };
        }
    }
}
