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
using VisionNet.Core.Abstractions;
using VisionNet.Core.Patterns;
using VisionNet.Core.Types;

namespace VisionNet.Vision.Core
{
    [Serializable]
    public class ParameterOptions : IEquatable<ParameterOptions>, IIndexable<string>, INamed, IDescriptible
    {
        /// <summary>
        /// Código identificativo del parámetro
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Vision")]
        [Description("Unique identifier of the parameter of the vision function")]
        [DisplayName(nameof(Index))]
        public string Index { get; set; } = "None";

        /// <summary>
        /// Código intero del parámetro en la función de vision
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Vision")]
        [Description("Reference of the parameter name of the external vision tool")]
        [DisplayName(nameof(ExternalIndex))]
        public string ExternalIndex { get; set; } = "Unkwnown";

        /// <summary>
        /// Nombre identificativo del parámetro
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Vision")]
        [Description("Name of the parameter of the vision function")]
        [DisplayName(nameof(Name))]
        public string Name { get; set; } = "None";

        /// <summary>
        /// Descripción del parámetro
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Vision")]
        [Description("Description of the parameter of the vision function")]
        [DisplayName(nameof(Description))]
        public string Description { get; set; } = "";

        /// <summary>
        /// Indica si el parámetro de la función de vision es una entrada o una salida
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Vision")]
        [Description("It indicates that the parameter is for input or output values")]
        [DisplayName(nameof(Direction))]
        public ParameterDirection Direction { get; set; } = ParameterDirection.Input;

        /// <summary>
        /// Indica si se trata de un parámetro fijo o es una variable
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Vision")]
        [Description("It indicates that the parameter is a fixed value or called at runtime")]
        [DisplayName(nameof(Source))]
        public ParameterSource Source { get; set; } = ParameterSource.Constant;

        /// <summary>
        /// Propiedad opcional que indica el valor fijo del parámetro
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Vision")]
        [Description("Indicates the fixed value of the parameter. Only used in constant source option.")]
        [DisplayName(nameof(DefaultValue))]
        public object DefaultValue { get; set; } = null;

        /// <summary>
        /// Destino del parametro de la función de vision
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Vision")]
        [Description("Indicates the destiny of the parametrer.")]
        [DisplayName(nameof(Scope))]
        public ParameterScope Scope { get; set; } = ParameterScope.Execution;

        /// <summary>
        /// Tipo de dato de la function de vision
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Vision")]
        [Description("Indicates the data type of the parametrer.")]
        [DisplayName(nameof(DataType))]
        public BasicTypeCode DataType { get; set; } = BasicTypeCode.Boolean;

        /// <summary>
        /// Se trata de un array de valores
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Vision")]
        [Description("Indicates it is vector of the data type of the parametrer.")]
        [DisplayName(nameof(IsArray))]
        public bool IsArray { get; set; } = false;

        /// <summary>
        /// Indica los parámetros adicionales de la conversión para el tipo de dato DataType
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Vision")]
        [Description("Conversion between types options")]
        [DisplayName(nameof(TypeConversionPreferences))]
        public TypeConversionPreferences TypeConversionPreferences { get; set; } = TypeConversionPreferences.None;

        /// <summary>
        /// If the parameter is Output direction, this value enable the storing of the value in the measurables list in order to save in database.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Vision")]
        [Description("If the parameter is Output direction, this value enable the storing of the value in the measurables list in order to save in database.")]
        [DisplayName(nameof(IncludeInMeasurable))]
        public bool IncludeInMeasurable { get; set; }

        /// <summary>
        /// Se guarda en resultado
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Vision")]
        [Description("Indicates if the parameter is to be saved in the result")]
        [DisplayName(nameof(SaveToResult))]
        public bool SaveToResult { get; set; } = true;


        public ParameterOptions()
        {
        }

        public ParameterOptions(
             string index = "None",
             string externalIndex = "Unkwnown",
             string name = "None",
             string description = "",
             ParameterDirection direction = ParameterDirection.Input,
             ParameterSource source = ParameterSource.Constant,
             object defaultValue = null,
             ParameterScope scope = ParameterScope.Execution,
             BasicTypeCode dataType = BasicTypeCode.Boolean,
             bool isArray = false,
             bool saveToResult = true)
        {
            Index = index;
            ExternalIndex = externalIndex;
            Name = name;
            Description = description;
            Direction = direction;
            Source = source;
            DefaultValue = defaultValue;
            Scope = scope;
            DataType = dataType;
            IsArray = isArray;

            if (direction == ParameterDirection.Input)
            {
                SaveToResult = false;
            }
            else
            {
                SaveToResult = saveToResult;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(ParameterOptions other)
        {
            if (other is null)
                return false;

            // Si se compara la misma referencia, se retorna true inmediatamente.
            if (ReferenceEquals(this, other))
                return true;

            return (other.Index == Index) &&
                   (other.Direction == Direction) &&
                   (other.Scope == Scope);
        }

        public static bool operator ==(ParameterOptions p1, ParameterOptions p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(ParameterOptions p1, ParameterOptions p2)
        {
            return !p1.Equals(p2);
        }
    }
}
