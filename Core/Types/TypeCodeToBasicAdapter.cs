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
    public class TypeCodeToBasicAdapter : IBidirectionalAdapter<TypeCode, BasicTypeCode>
    {
        /// <summary>
        /// Converts the provided <see cref="TypeCode"/> to the corresponding <see cref="BasicTypeCode"/> supported by the adapter.
        /// </summary>
        /// <param name="value">The .NET <see cref="TypeCode"/> to convert. Integral, floating-point, string, boolean, date/time, and object type codes are recognized.</param>
        /// <returns>The matching <see cref="BasicTypeCode"/> enumeration value; returns <see cref="BasicTypeCode.NotSupported"/> when the supplied type code is not recognized.</returns>
        /// <remarks>This method does not throw exceptions.</remarks>
        public BasicTypeCode Convert(TypeCode value)
        {
            var result = BasicTypeCode.NotSupported;
            Convert(value, ref result);
            return result;
        }

        /// <summary> The Convert function converts a TypeCode value to the corresponding type.</summary>
        /// <param name="value"> What is this for?</param>
        /// <param name="result"> What is this used for?</param>
        /// <result =s> A type object.</result =s>
        public void Convert(TypeCode value, ref BasicTypeCode result)
        {
            switch (value)
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    result = BasicTypeCode.IntegerNumber;
                    break;
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    result = BasicTypeCode.FloatingPointNumber;
                    break;
                case TypeCode.String:
                    result = BasicTypeCode.String;
                    break;
                case TypeCode.Boolean:
                    result = BasicTypeCode.Boolean;
                    break;
                case TypeCode.DateTime:
                    result = BasicTypeCode.DateTime;
                    break;
                case TypeCode.Object:
                    result = BasicTypeCode.Object;
                    break;
                default:
                    result = BasicTypeCode.NotSupported;
                    break;
            }
        }

        /// <summary>
        /// Converts the provided <see cref="BasicTypeCode"/> to the corresponding <see cref="TypeCode"/> supported by the adapter.
        /// </summary>
        /// <param name="value">The <see cref="BasicTypeCode"/> to convert. IntegerNumber, FloatingPointNumber, String, Boolean, DateTime, Object, Image, and Graphic values are supported.</param>
        /// <returns>The matching <see cref="TypeCode"/> enumeration value; returns <see cref="TypeCode.Empty"/> when the supplied type is not recognized or represents <see cref="BasicTypeCode.NotSupported"/>.</returns>
        /// <remarks>This method does not throw exceptions.</remarks>
        public TypeCode Convert(BasicTypeCode value)
        {
            var result = TypeCode.Empty;
            Convert(value, ref result);
            return result;
        }

        /// <summary> The Convert function converts a TypeCode value to the corresponding type.</summary>
        /// <param name="value"> What is this for?</param>
        /// <param name="result"> What is this used for?</param>
        /// <result =s> A type object.</result =s>
        public void Convert(BasicTypeCode value, ref TypeCode result)
        {
            switch (value)
            {
                case BasicTypeCode.IntegerNumber:
                    result = TypeCode.Int64;
                    break;
                case BasicTypeCode.FloatingPointNumber:
                    result = TypeCode.Double;
                    break;
                case BasicTypeCode.String:
                    result = TypeCode.String;
                    break;
                case BasicTypeCode.Boolean:
                    result = TypeCode.Boolean;
                    break;
                case BasicTypeCode.DateTime:
                    result = TypeCode.DateTime;
                    break;
                case BasicTypeCode.Object:
                case BasicTypeCode.Image:
                case BasicTypeCode.Graphic:
                    result = TypeCode.Object;
                    break;
                case BasicTypeCode.NotSupported:
                default:
                    result = TypeCode.Empty;
                    break;
            }
        }
    }
}
