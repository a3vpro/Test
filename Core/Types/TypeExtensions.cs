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
using System.Runtime.InteropServices;

namespace VisionNet.Core.Types
{
    /// <summary>
    /// Proporciona métodos de extensión para la clase System.Type y System.TypeCode.
    /// Estos métodos incluyen la obtención del tamaño de un tipo, la comprobación de si un tipo es numérico,
    /// la obtención del valor mínimo y máximo de un tipo numérico, y la conversión entre System.Type y System.TypeCode.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// A dictionary mapping .NET types to their C# type aliases.
        /// </summary>
        private static readonly Dictionary<Type, string> CSharpTypeAliases = new Dictionary<Type, string>
        {
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(char), "char" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(float), "float" },
            { typeof(int), "int" },
            { typeof(uint), "uint" },
            { typeof(long), "long" },
            { typeof(ulong), "ulong" },
            { typeof(short), "short" },
            { typeof(ushort), "ushort" },
            { typeof(object), "object" },
            { typeof(string), "string" }
        };

        /// <summary>
        /// Gets the C# alias of the specified type if it exists; otherwise, returns the type's name.
        /// </summary>
        /// <param name="type">The type for which to get the C# alias.</param>
        /// <returns>The C# alias of the type, or the type's name if no alias exists.</returns>
        public static string GetCSharpAlias(this Type type)
        {
            if (CSharpTypeAliases.TryGetValue(type, out string alias))
            {
                return alias;
            }
            else
            {
                return type.Name;
            }
        }

        /// <summary>
        /// Comprueba si el tipo especificado es numérico.
        /// </summary>
        /// <param name="type">El tipo a comprobar.</param>
        /// <returns>Verdadero si el tipo es numérico, falso en caso contrario.</returns>
        public static bool IsNumeric(this Type type)
        {
            if (type == null)
                return false;

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        return IsNumeric(Nullable.GetUnderlyingType(type));
                    return false;
            }
            return false;
        }

        /// <summary>
        /// Comprueba si el tipo especificado es numérico.
        /// </summary>
        /// <param name="type">El tipo a comprobar.</param>
        /// <returns>Verdadero si el tipo es numérico, falso en caso contrario.</returns>
        public static bool IsNumeric(this TypeCode type)
        {
            switch (type)
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Comprueba si el tipo especificado es numérico.
        /// </summary>
        /// <param name="type">El tipo a comprobar.</param>
        /// <returns>Verdadero si el tipo es numérico, falso en caso contrario.</returns>
        public static bool IsNumeric(this BasicTypeCode type)
        {
            switch (type)
            {
                case BasicTypeCode.IntegerNumber:
                case BasicTypeCode.FloatingPointNumber:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Convierte el tipo especificado a su correspondiente código de tipo.
        /// </summary>
        /// <param name="value">El tipo a convertir.</param>
        /// <param name="result">El código de tipo correspondiente al tipo.</param>
        public static void ToType(this TypeCode value, ref Type result)
        {
            new TypeCodeAdapter().Convert(value, ref result);
        }

        /// <summary>
        /// Convierte el tipo especificado a su correspondiente código de tipo.
        /// </summary>
        /// <param name="value">El tipo a convertir.</param>
        /// <param name="result">El código de tipo correspondiente al tipo.</param>
        public static void ToType(this BasicTypeCode value, ref Type result)
        {
            new TypeToBasicAdapter().Convert(value, ref result);
        }

        /// <summary>
        /// Convierte el tipo especificado a su correspondiente código de tipo.
        /// </summary>
        /// <param name="value">El tipo a convertir.</param>
        /// <param name="result">El código de tipo correspondiente al tipo.</param>
        public static void ToTypeCode(this Type value, ref TypeCode result)
        {
            new TypeCodeAdapter().Convert(value, ref result);
        }

        /// <summary>
        /// Convierte el tipo especificado a su correspondiente código de tipo básico.
        /// </summary>
        /// <param name="value">El tipo a convertir.</param>
        /// <param name="result">El código de tipo correspondiente al tipo básico.</param>
        public static void ToBasicType(this TypeCode value, ref BasicTypeCode result)
        {
            new TypeCodeToBasicAdapter().Convert(value, ref result);
        }

        /// <summary>
        /// Convierte el tipo especificado a su correspondiente código de tipo.
        /// </summary>
        /// <param name="value">El tipo a convertir.</param>
        /// <param name="result">El código de tipo correspondiente al tipo.</param>
        public static void ToTypeCode(this BasicTypeCode value, ref TypeCode result)
        {
            new TypeCodeToBasicAdapter().Convert(value, ref result);
        }

        /// <summary>
        /// Devuelve el tamaño en bytes del tipo especificado.
        /// </summary>
        /// <param name="type">El tipo del que obtener el tamaño.</param>
        /// <returns>El tamaño en bytes del tipo.</returns>
        public static int SizeOf(this TypeCode type)
        {
            switch (type)
            {
                case TypeCode.Boolean: return sizeof(Boolean);
                case TypeCode.Byte: return sizeof(Byte);
                case TypeCode.Char: return sizeof(Char);
                case TypeCode.DateTime: return Marshal.SizeOf(typeof(DateTime));
                case TypeCode.DBNull: return 0;  // DBNull is not a data sutdotabase null vale
                case TypeCode.Decimal: return sizeof(Decimal);
                case TypeCode.Double: return sizeof(Double);
                case TypeCode.Empty: return 0;  // Empty is not a data type
                case TypeCode.Int16: return sizeof(Int16);
                case TypeCode.Int32: return sizeof(Int32);
                case TypeCode.Int64: return sizeof(Int64);
                case TypeCode.Object: return -1; // Size of object cannot be determined
                case TypeCode.SByte: return sizeof(SByte);
                case TypeCode.Single: return sizeof(Single);
                case TypeCode.String: return -1;  // Size of string cannot be determined as it varies
                case TypeCode.UInt16: return sizeof(UInt16);
                case TypeCode.UInt32: return sizeof(UInt32);
                case TypeCode.UInt64: return sizeof(UInt64);
                default: return -1;  // Unknown TypeCode value
            }
        }

        /// <summary>
        /// Devuelve el tamaño en bytes del tipo especificado.
        /// </summary>
        /// <param name="type">El tipo del que obtener el tamaño.</param>
        /// <returns>El tamaño en bytes del tipo.</returns>        
        public static int SizeOf(this Type type)
        {
            if (type == null) return 0;
            else if (type == typeof(bool)) return sizeof(bool);
            else if (type == typeof(byte)) return sizeof(byte);
            else if (type == typeof(char)) return sizeof(char);
            else if (type == typeof(DateTime)) return Marshal.SizeOf(typeof(DateTime));
            else if (type == typeof(decimal)) return sizeof(decimal);
            else if (type == typeof(double)) return sizeof(double);
            else if (type == typeof(float)) return sizeof(float);
            else if (type == typeof(int)) return sizeof(int);
            else if (type == typeof(long)) return sizeof(long);
            else if (type == typeof(sbyte)) return sizeof(sbyte);
            else if (type == typeof(short)) return sizeof(short);
            else if (type == typeof(uint)) return sizeof(uint);
            else if (type == typeof(ulong)) return sizeof(ulong);
            else if (type == typeof(ushort)) return sizeof(ushort);
            else return -1;
        }

        /// <summary>
        /// Devuelve el valor mínimo del tipo numérico especificado.
        /// </summary>
        /// <param name="type">El tipo numérico del que obtener el valor mínimo.</param>
        /// <returns>El valor mínimo del tipo numérico.</returns>
        public static double MinValue(this Type type)
        {
            if (type == typeof(int)) return int.MinValue;
            else if (type == typeof(long)) return long.MinValue;
            else if (type == typeof(double)) return double.MinValue;
            else if (type == typeof(float)) return float.MinValue;
            else if (type == typeof(decimal)) return Convert.ToDouble(decimal.MinValue);
            else if (type == typeof(short)) return short.MinValue;
            else if (type == typeof(byte)) return byte.MinValue;
            else if (type == typeof(uint)) return uint.MinValue;
            else if (type == typeof(ulong)) return ulong.MinValue;
            else if (type == typeof(ushort)) return ushort.MinValue;
            else if (type == typeof(sbyte)) return sbyte.MinValue;
            else throw new ArgumentException("MinValue is not supported for this type", nameof(type));
        }

        /// <summary>
        /// Devuelve el valor mínimo del tipo numérico especificado.
        /// </summary>
        /// <param name="type">El tipo numérico del que obtener el valor mínimo.</param>
        /// <returns>El valor mínimo del tipo numérico.</returns>
        public static double MinValue(this TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Byte: return byte.MinValue;
                case TypeCode.Int16: return short.MinValue;
                case TypeCode.Int32: return int.MinValue;
                case TypeCode.Int64: return long.MinValue;
                case TypeCode.SByte: return sbyte.MinValue;
                case TypeCode.UInt16: return ushort.MinValue;
                case TypeCode.UInt32: return uint.MinValue;
                case TypeCode.UInt64: return ulong.MinValue;
                case TypeCode.Single: return float.MinValue;
                case TypeCode.Double: return double.MinValue;
                case TypeCode.Decimal: return Convert.ToDouble(decimal.MinValue);
                default: throw new ArgumentException("MinValue is not supported for this TypeCode", nameof(typeCode));
            }
        }

        /// <summary>
        /// Devuelve el valor máximo del tipo numérico especificado.
        /// </summary>
        /// <param name="type">El tipo numérico del que obtener el valor máximo.</param>
        /// <returns>El valor máximo del tipo numérico.</returns>
        public static double MaxValue(this Type type)
        {
            if (type == typeof(int)) return int.MaxValue;
            else if (type == typeof(long)) return long.MaxValue;
            else if (type == typeof(double)) return double.MaxValue;
            else if (type == typeof(float)) return float.MaxValue;
            else if (type == typeof(decimal)) return Convert.ToDouble(decimal.MaxValue);
            else if (type == typeof(short)) return short.MaxValue;
            else if (type == typeof(byte)) return byte.MaxValue;
            else if (type == typeof(uint)) return uint.MaxValue;
            else if (type == typeof(ulong)) return ulong.MaxValue;
            else if (type == typeof(ushort)) return ushort.MaxValue;
            else if (type == typeof(sbyte)) return sbyte.MaxValue;
            else throw new ArgumentException("MaxValue is not supported for this type", nameof(type));
        }

        /// <summary>
        /// Devuelve el valor máximo del tipo numérico especificado.
        /// </summary>
        /// <param name="typeCode">El tipo numérico del que obtener el valor máximo.</param>
        /// <returns>El valor máximo del tipo numérico.</returns>
        public static double MaxValue(this TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Byte: return byte.MaxValue;
                case TypeCode.Int16: return short.MaxValue;
                case TypeCode.Int32: return int.MaxValue;
                case TypeCode.Int64: return long.MaxValue;
                case TypeCode.SByte: return sbyte.MaxValue;
                case TypeCode.UInt16: return ushort.MaxValue;
                case TypeCode.UInt32: return uint.MaxValue;
                case TypeCode.UInt64: return ulong.MaxValue;
                case TypeCode.Single: return float.MaxValue;
                case TypeCode.Double: return double.MaxValue;
                case TypeCode.Decimal: return Convert.ToDouble(decimal.MaxValue);
                default: throw new ArgumentException("MaxValue is not supported for this TypeCode", nameof(typeCode));
            }
        }

        /// <summary>
        /// Devuelve el valor por defecto del tipo numérico especificado.
        /// </summary>
        /// <param name="typeCode">El tipo del que obtener el valor por defecto.</param>
        /// <returns>El valor por defecto del tipo especificado.</returns>
        public static object DefaultValue(this TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Object: return default(object);
                case TypeCode.Empty: return null;
                case TypeCode.DBNull: return null;
                case TypeCode.Boolean: return default(bool);
                case TypeCode.Char: return default(char);
                case TypeCode.String: return default(string);
                case TypeCode.DateTime: return default(DateTime);
                case TypeCode.Byte: return default(byte);
                case TypeCode.Int16: return default(short);
                case TypeCode.Int32: return default(int);
                case TypeCode.Int64: return default(long);
                case TypeCode.SByte: return default(sbyte);
                case TypeCode.UInt16: return default(ushort);
                case TypeCode.UInt32: return default(uint);
                case TypeCode.UInt64: return default(ulong);
                case TypeCode.Single: return default(float);
                case TypeCode.Double: return default(double);
                case TypeCode.Decimal: return default(decimal);
                default: throw new ArgumentException("Default is not supported for this TypeCode", nameof(typeCode));
            }
        }

        /// <summary>
        /// Devuelve el valor mínimo del tipo numérico especificado.
        /// </summary>
        /// <param name="type">El tipo numérico del que obtener el valor mínimo.</param>
        /// <returns>El valor mínimo del tipo numérico.</returns>
        public static double MinValue(this BasicTypeCode typeCode)
        {
            switch (typeCode)
            {
                case BasicTypeCode.IntegerNumber: return long.MinValue;
                case BasicTypeCode.FloatingPointNumber: return double.MinValue;
                case BasicTypeCode.String: 
                case BasicTypeCode.Boolean: 
                case BasicTypeCode.DateTime: 
                case BasicTypeCode.Object: 
                case BasicTypeCode.Image: 
                case BasicTypeCode.Graphic: 
                case BasicTypeCode.NotSupported: 
                default: throw new ArgumentException("MinValue is not supported for this TypeCode", nameof(typeCode));
            }
        }

        /// <summary>
        /// Devuelve el valor máximo del tipo numérico especificado.
        /// </summary>
        /// <param name="typeCode">El tipo numérico del que obtener el valor máximo.</param>
        /// <returns>El valor máximo del tipo numérico.</returns>
        public static double MaxValue(this BasicTypeCode typeCode)
        {
            switch (typeCode)
            {
                case BasicTypeCode.IntegerNumber: return long.MaxValue;
                case BasicTypeCode.FloatingPointNumber: return double.MaxValue;
                case BasicTypeCode.String:
                case BasicTypeCode.Boolean:
                case BasicTypeCode.DateTime:
                case BasicTypeCode.Object:
                case BasicTypeCode.Image:
                case BasicTypeCode.Graphic:
                case BasicTypeCode.NotSupported:
                default: throw new ArgumentException("MinValue is not supported for this TypeCode", nameof(typeCode));
            }
        }

        /// <summary>
        /// Devuelve el valor por defecto del tipo numérico especificado.
        /// </summary>
        /// <param name="typeCode">El tipo del que obtener el valor por defecto.</param>
        /// <returns>El valor por defecto del tipo especificado.</returns>
        public static object DefaultValue(this BasicTypeCode typeCode, bool isArray = false)
        {
            if (isArray)
                return new List<object>();
            else
                switch (typeCode)
                {
                    case BasicTypeCode.IntegerNumber: return default(long);
                    case BasicTypeCode.FloatingPointNumber: return default(double);
                    case BasicTypeCode.String: return default(string);
                    case BasicTypeCode.Boolean: return default(bool);
                    case BasicTypeCode.DateTime: return default(DateTime);
                    case BasicTypeCode.Object: return default(object);
                    case BasicTypeCode.Image: return default(object);
                    case BasicTypeCode.Graphic: return default(object);
                    case BasicTypeCode.NotSupported:
                    default: throw new ArgumentException("TypeCode not supported", nameof(typeCode));
                }
        }
    }
}
