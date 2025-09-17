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
using System.Globalization;
using VisionNet.Core.Strings;
using VisionNet.Core.Types;
using VisionNet.Core.Exceptions;
using System.Collections;
using System.Text.RegularExpressions;
using VisionNet.Core.Collections;
using VisionNet.Core.Reflection;
using VisionNet.Core.Maths;

namespace VisionNet.Core.SafeObjects
{
    public static class SafeObjectHelper
    {
        enum BooleanRepresentationMotivation
        {
            TrueFalse,
            YesNo,
        }

        private static List<(CultureInfo cultureInfo, BooleanRepresentationMotivation motivation, string True, string False)> _booleanRepresentations = new List<(CultureInfo, BooleanRepresentationMotivation, string, string)>
        {
            (new CultureInfo("en-EN"), BooleanRepresentationMotivation.YesNo, "yes", "no"),
            (new CultureInfo("es-ES"), BooleanRepresentationMotivation.TrueFalse, "verdadero", "falso"),
            (new CultureInfo("es-ES"), BooleanRepresentationMotivation.YesNo, "si", "no"),
        };


        
        /// <summary> The IsAssignableFrom function determines whether an object can be assigned to a type.</summary>
        /// <param name="value"> The object to check.</param>
        /// <returns> True if the value is not null and it is of type t or a derived type.</returns>
        public static bool IsAssignableFrom<T>(object value)
        {
            return (value != null) &&
                typeof(T).IsAssignableFrom(value.GetType());
        }

        
        /// <summary> The IsAssignableFrom function determines whether an instance of a specified type can be assigned to a variable of the current type.</summary>
        /// <param name="type"> The type to check.</param>
        /// <param name="value"> The object to be tested.</param>
        /// <returns> True if the type is assignable from the value.</returns>
        public static bool IsAssignableFrom(Type type, object value)
        {
            return (type != null) &&
                (value != null) &&
                type.IsAssignableFrom(value.GetType());
        }

        
        /// <summary> The TryChangeType function attempts to convert the value parameter to the conversionType type. If it fails, it will attempt to convert using CultureInfo.CurrentCulture.</summary>
        /// <param name="value"> The value to be converted.</param>
        /// <param name="conversionType"> The type to convert the value to.</param>
        /// <param name="result"> The result of the conversion.</param>
        /// <param name="defaultValue"> The default value to return if the conversion fails.</param>
        /// <returns> A boolean value. if the conversion is successful, the result parameter contains a converted value and true is returned. otherwise, false is returned.</returns>
        public static bool TryChangeType(this object value, Type conversionType, out object result, object defaultValue = null)
        {
            try
            {
                result = Convert.ChangeType(value, conversionType);
                return true;
            }
            catch
            {
                if (value is string)
                {
                    try
                    {
                        result = Convert.ChangeType(value, conversionType, CultureInfo.CurrentCulture);
                        return true;
                    }
                    catch
                    {
                        result = defaultValue;
                        return false;
                    }
                }
                else
                {
                    result = defaultValue;
                    return false;
                }
            }
        }
        
        /// <summary> The TryChangeType function attempts to convert the value parameter to the conversionType type. If it fails, it will attempt to convert using CultureInfo.CurrentCulture.</summary>
        /// <param name="value"> The value to be converted.</param>
        /// <param name="defaultValue"> The default value to return if the conversion fails.</param>
        /// <returns> A boolean value. if the conversion is successful, the result parameter contains a converted value and true is returned. otherwise, false is returned.</returns>
        public static T ChangeType<T>(this object value, object defaultValue = null)
        {
            TryChangeType(value, typeof(T), out var result, defaultValue);
            return (T)result;
        }

        /// <summary> The TryChangeType function attempts to convert the value parameter to the type specified by typeCode.
        /// If it fails, it will try a few other methods of conversion depending on what preferences are passed in.
        /// For example, if you pass in TypeConverstionPreferences.StringToBooleanLanguageInvariant and your value is &quot;true&quot; or &quot;false&quot;, 
        /// then this function will return true with result set to either true or false respectively.</summary>
        /// <param name="value"> The value to convert.</param>
        /// <param name="typeCode"> The type to convert the value to.</param>
        /// <param name="result"> The result of the conversion.</param>
        /// <param name="defaultValue"> Valor por defecto a devolver si no se puede convertir</param>
        /// <param name="preferences"> &lt;para&gt;typeconversionspreferences.none&lt;/para&gt;
        ///     &lt;para&gt;typeconversionspreferences.stringtobooleanlanguageinvariant&lt;/para&gt;
        ///     &lt;para&gt;typeconversionspreferences.stringtobooleanallowsnumbers&lt;/para&gt;
        /// </param>
        /// <returns> True if the conversion was successful, otherwise false.</returns>
        public static bool TryChangeType(this object value, TypeCode typeCode, out object result, object defaultValue = null, TypeConversionPreferences preferences = TypeConversionPreferences.None)
        {
            result = defaultValue;
            var canConvert = false;

            try
            {
                // Conversión de null a string
                if (typeCode == TypeCode.String && value == null)
                    result = string.Empty;
                else
                    // Intento básico de conversión
                    result = Convert.ChangeType(value, typeCode);
                canConvert = true;
            }
            catch
            {
                if (typeCode == TypeCode.Boolean && value is string strValue)
                {
                    if (preferences.HasFlag(TypeConversionPreferences.StringToBooleanLanguageInvariant))
                        // Intento de conversión de la representación del idioma en booleano
                        foreach (var booleanRepresentation in _booleanRepresentations)
                            if (strValue.EqualsEx(booleanRepresentation.True, StringComparisonEx.InvariantCultureIgnoreCase))
                            {
                                result = true;
                                canConvert = true;
                            }
                            else if (strValue.EqualsEx(booleanRepresentation.False, StringComparisonEx.InvariantCultureIgnoreCase))
                            {
                                result = false;
                                canConvert = true;
                            }
                    if (preferences.HasFlag(TypeConversionPreferences.StringToBooleanAllowsNumbers))
                        // Intento de conversión a número y posteriormente a booleano
                        try
                        {
                            var tmpResult = Convert.ChangeType(strValue, TypeCode.Int32);
                            result = Convert.ChangeType(tmpResult, typeCode);
                            canConvert = true;
                        }
                        catch (Exception ex)
                        {
                            ex.LogToConsole(nameof(TryChangeType));
                        }
                }
                else if (typeCode.IsNumeric())
                {
                    if (preferences.HasFlag(TypeConversionPreferences.UseRegexIfNecessary) && (value is string))
                    {
                        string cleanedInput = Regex.Replace(value.ToString(), @"[^\d,.-]", "").Trim(); // Elimina caracteres no deseados y prepara la cadena para la conversión
                        canConvert = double.TryParse(cleanedInput, NumberStyles.Any, CultureInfo.CurrentCulture, out var doubleValue); // Intenta convertir la cadena limpia a double
                        result = value = Convert.ChangeType(doubleValue, typeCode);
                    }
                    if (preferences.HasFlag(TypeConversionPreferences.NumberClamp) && (value.GetType().IsNumeric() || value is string))
                    {
                        double doubleValue = value is double ? (double)value : Convert.ToDouble(value);
                        var destMax = typeCode.MaxValue();
                        var destMin = typeCode.MinValue();
                        if (doubleValue > destMax || doubleValue < destMin)
                        {
                            var clampValue = MathHelper.Clamp(doubleValue, destMin, destMax);
                            result = Convert.ChangeType(clampValue, typeCode);
                            canConvert = true;
                        }
                    }
                }
            }

            return canConvert;
        }

        /// <summary> The TryChangeType function attempts to convert the value parameter to the type specified by typeCode.
        /// If it fails, it will try a few other methods of conversion depending on what preferences are passed in.
        /// For example, if you pass in TypeConverstionPreferences.StringToBooleanLanguageInvariant and your value is &quot;true&quot; or &quot;false&quot;, 
        /// then this function will return true with result set to either true or false respectively.</summary>
        /// <param name="value"> The value to convert.</param>
        /// <param name="typeCode"> The type to convert the value to.</param>
        /// <param name="result"> The result of the conversion.</param>
        /// <param name="defaultValue"> Valor por defecto a devolver si no se puede convertir</param>
        /// <param name="preferences"> &lt;para&gt;typeconversionspreferences.none&lt;/para&gt;
        ///     &lt;para&gt;typeconversionspreferences.stringtobooleanlanguageinvariant&lt;/para&gt;
        ///     &lt;para&gt;typeconversionspreferences.stringtobooleanallowsnumbers&lt;/para&gt;
        /// </param>
        /// <returns> True if the conversion was successful, otherwise false.</returns>
        public static bool TryChangeType(this object value, BasicTypeCode typeCode, bool isArray, out object result, object defaultValue = null, TypeConversionPreferences preferences = TypeConversionPreferences.None)
        {
            result = defaultValue;
            var canConvert = false;

            if (isArray && !(value is IList))
            {
                result = null;
                canConvert = true;
            }
            else if (isArray && value is IList)
            {
                // Get the target type
                Type desiredType = typeof(object);
                TypeCode desiredTypeCode = TypeCode.Object;
                typeCode.ToType(ref desiredType);
                typeCode.ToTypeCode(ref desiredTypeCode);

                // Get the list value
                var collection = value as IList;
                var length = collection?.Count ?? 0;

                // List to fill
                IList newCollection = value.IsListOf(desiredType)
                    ? value as IList
                    : ReflectionHelper.CreateListOfType(desiredType, length, desiredTypeCode.DefaultValue());

                canConvert = true;
                for (int i = 0; i < length; i++)
                {
                    object oldValue = collection[i];
                    var itemCanConvert = TryChangeType(oldValue, typeCode, out var newValue, defaultValue, preferences);
                    if (itemCanConvert)
                        newCollection[i] = newValue;
                    canConvert &= itemCanConvert;
                }
                result = newCollection;
            }
            else
                canConvert = TryChangeType(value, typeCode, out result, defaultValue, preferences);

            return canConvert;
        }

        public static bool TryChangeType(this object value, BasicTypeCode typeCode, out object result, object defaultValue = null, TypeConversionPreferences preferences = TypeConversionPreferences.None)
        {
            result = defaultValue;
            var canConvert = false;

            var internalTypeCode = TypeCode.Empty;
            typeCode.ToTypeCode(ref internalTypeCode);

            try
            {
                // Conversión de null a string
                if (typeCode == BasicTypeCode.String && value == null)
                {
                    canConvert = true;
                    result = string.Empty;
                }
                else if (internalTypeCode == TypeCode.Object && !(value is IConvertible) && value is Object)
                {
                    result = value;
                    canConvert = true;
                }
                else
                {
                    // Intento básico de conversión
                    result = Convert.ChangeType(value, internalTypeCode);
                }
                canConvert = true;
            }
            catch
            {

                if (typeCode == BasicTypeCode.Boolean && value is string strValue)
                {
                    if (preferences.HasFlag(TypeConversionPreferences.StringToBooleanLanguageInvariant))
                        // Intento de conversión de la representación del idioma en booleano
                        foreach (var booleanRepresentation in _booleanRepresentations)
                            if (strValue.EqualsEx(booleanRepresentation.True, StringComparisonEx.InvariantCultureIgnoreCase))
                            {
                                result = true;
                                canConvert = true;
                            }
                            else if (strValue.EqualsEx(booleanRepresentation.False, StringComparisonEx.InvariantCultureIgnoreCase))
                            {
                                result = false;
                                canConvert = true;
                            }
                    if (preferences.HasFlag(TypeConversionPreferences.StringToBooleanAllowsNumbers))
                        // Intento de conversión a número y posteriormente a booleano
                        try
                        {
                            var tmpResult = Convert.ChangeType(strValue, TypeCode.Int32);
                            result = Convert.ChangeType(tmpResult, internalTypeCode);
                            canConvert = true;
                        }
                        catch (Exception ex)
                        {
                            ex.LogToConsole(nameof(TryChangeType));
                        }
                }
                else if (preferences.HasFlag(TypeConversionPreferences.NumberClamp) && typeCode.IsNumeric() && (value.GetType().IsNumeric() || value is string))
                {
                    double doubleValue = value is double ? (double)value : Convert.ToDouble(value);
                    var destMax = typeCode.MaxValue();
                    var destMin = typeCode.MinValue();
                    if (doubleValue > destMax || doubleValue < destMin)
                    {
                        var clampValue = MathHelper.Clamp(doubleValue, destMin, destMax);
                        result = Convert.ChangeType(clampValue, internalTypeCode);
                        canConvert = true;
                    }
                }
            }

            return canConvert;
        }
    }
}