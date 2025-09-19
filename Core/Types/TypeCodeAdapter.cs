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
using VisionNet.Core.Patterns;

namespace VisionNet.Core.Types
{
    /// <summary>
    /// Clase conversora entre tipos básicos TypeCode y Type
    /// </summary>
    public class TypeCodeAdapter : IBidirectionalAdapter<TypeCode, Type>
    {
        
        /// <summary> The Convert function converts a TypeCode value to the corresponding type.</summary>
        /// <param name="value"> The typecode value to convert.</param>
        /// <returns> The type of the value.</returns>
        public Type Convert(TypeCode value)
        {
            var result = typeof(object);
            Convert(value, ref result);
            return result;
        }

        
        /// <summary> The Convert function converts a Type to a TypeCode.</summary>
        /// <param name="value"> The type to convert</param>
        /// <returns> A typecode value that indicates the type of object passed to it.</returns>
        public TypeCode Convert(Type value)
        {
            var result = TypeCode.Empty;
            Convert(value, ref result);
            return result;
        }

        
        /// <summary> The Convert function converts a TypeCode value to the corresponding type.</summary>
        /// <param name="value"> What is this for?</param>
        /// <param name="result"> What is this used for?</param>
        /// <returns> A type object.</returns>
        public void Convert(TypeCode value, ref Type result)
        {
            switch (value)
            {
                case TypeCode.Object:
                case TypeCode.Empty:
                default:
                    result = typeof(object);
                    break;
                case TypeCode.Boolean:
                    result = typeof(bool);
                    break;
                case TypeCode.Byte:
                    result = typeof(byte);
                    break;
                case TypeCode.Char:
                    result = typeof(char);
                    break;
                case TypeCode.DateTime:
                    result = typeof(DateTime);
                    break;
                case TypeCode.DBNull:
                    result = typeof(DBNull);
                    break;
                case TypeCode.Decimal:
                    result = typeof(decimal);
                    break;
                case TypeCode.Double:
                    result = typeof(double);
                    break;
                case TypeCode.Int16:
                    result = typeof(short);
                    break;
                case TypeCode.Int32:
                    result = typeof(int);
                    break;
                case TypeCode.Int64:
                    result = typeof(long);
                    break;
                case TypeCode.SByte:
                    result = typeof(sbyte);
                    break;
                case TypeCode.Single:
                    result = typeof(float);
                    break;
                case TypeCode.String:
                    result = typeof(string);
                    break;
                case TypeCode.UInt16:
                    result = typeof(ushort);
                    break;
                case TypeCode.UInt32:
                    result = typeof(uint);
                    break;
                case TypeCode.UInt64:
                    result = typeof(ulong);
                    break;
            }
        }

        
        /// <summary> The Convert function converts a Type to a TypeCode.</summary>
        /// <param name="value"> The type of the value.</param>
        /// <param name="result"> The typecode result is a reference to the type code that will be returned. 
        /// </param>
        /// <returns> A typecode value that indicates the type of object passed to it.</returns>
        public void Convert(Type value, ref TypeCode result)
        {
            result = TypeCode.Empty;

            if (value.IsAssignableFrom(typeof(bool)))
                result = TypeCode.Boolean;
            else if (value.IsAssignableFrom(typeof(byte)))
                result = TypeCode.Byte;
            else if (value.IsAssignableFrom(typeof(char)))
                result = TypeCode.Char;
            else if (value.IsAssignableFrom(typeof(DateTime)))
                result = TypeCode.DateTime;
            else if (value.IsAssignableFrom(typeof(DBNull)))
                result = TypeCode.DBNull;
            else if (value.IsAssignableFrom(typeof(decimal)))
                result = TypeCode.Decimal;
            else if (value.IsAssignableFrom(typeof(double)))
                result = TypeCode.Double;
            else if (value.IsAssignableFrom(typeof(short)))
                result = TypeCode.Int16;
            else if (value.IsAssignableFrom(typeof(int)))
                result = TypeCode.Int32;
            else if (value.IsAssignableFrom(typeof(long)))
                result = TypeCode.Int64;
            else if (value.IsAssignableFrom(typeof(sbyte)))
                result = TypeCode.SByte;
            else if (value.IsAssignableFrom(typeof(float)))
                result = TypeCode.Single;
            else if (value.IsAssignableFrom(typeof(string)))
                result = TypeCode.String;
            else if (value.IsAssignableFrom(typeof(ushort)))
                result = TypeCode.UInt16;
            else if (value.IsAssignableFrom(typeof(uint)))
                result = TypeCode.UInt32;
            else if (value.IsAssignableFrom(typeof(ulong)))
                result = TypeCode.UInt64;
            else if (value.IsAssignableFrom(typeof(object)))
                result = TypeCode.Object;
        }
    }
}
