using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using VisionNet.Core.Types;

namespace VisionNet.Vision.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ParameterAttribute: Attribute
    {
        public ParameterOptions ParameterOptions { get; set; }

        public ParameterAttribute(string index,
                                 string externalIndex = null,
                                 string name = null,
                                 string description = null,
                                 ParameterDirection direction = ParameterDirection.Input,
                                 ParameterSource source = ParameterSource.Constant,
                                 object defaultValue = null,
                                 ParameterScope scope = ParameterScope.Execution,
                                 BasicTypeCode dataType = BasicTypeCode.Boolean,
                                 bool isArray = false,
                                 TypeConversionPreferences typeConversionPreferences = TypeConversionPreferences.None,
                                 bool includeInMeasurable = false,
                                 bool saveToResult = false)
        {
            externalIndex = externalIndex ?? index;
            name = name ?? index;
            description = description ?? index;
            ParameterOptions = new ParameterOptions
            {
                Index = index,
                ExternalIndex = externalIndex,
                Name = name,
                Description = description,
                Direction = direction,
                Source = source,
                DefaultValue = defaultValue,
                Scope = scope,
                DataType = dataType,
                IsArray = isArray,
                TypeConversionPreferences = typeConversionPreferences,
                IncludeInMeasurable = includeInMeasurable,
                SaveToResult = saveToResult
            };
        }
    }
}
