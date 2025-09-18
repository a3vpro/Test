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
    public class TypeToBasicAdapter : IBidirectionalAdapter<Type, BasicTypeCode>
    {
        /// <summary>
        /// Converts the provided runtime type to its corresponding <see cref="BasicTypeCode"/> according to the
        /// adapter's predefined mappings for Boolean, numeric, string, date, object, and <see cref="DBNull"/> types.
        /// </summary>
        /// <param name="value">
        /// A non-null runtime <see cref="Type"/> assignable from the supported primitive types (bool, integral
        /// numbers, floating-point numbers, char), <see cref="string"/>, <see cref="DateTime"/>, <see cref="object"/>,
        /// or <see cref="DBNull"/>.
        /// </param>
        /// <returns>
        /// The mapped <see cref="BasicTypeCode"/> when the input type matches one of the supported categories;
        /// otherwise <see cref="BasicTypeCode.NotSupported"/>.
        /// </returns>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
        public BasicTypeCode Convert(Type value)
        {
            var result = BasicTypeCode.NotSupported;
            Convert(value, ref result);
            return result;
        }

        /// <summary>
        /// Converts the provided runtime type to its corresponding <see cref="BasicTypeCode"/> and stores the result
        /// in the supplied reference parameter.
        /// </summary>
        /// <param name="value">
        /// A non-null runtime <see cref="Type"/> assignable from the supported primitive types (bool, integral
        /// numbers, floating-point numbers, char), <see cref="string"/>, <see cref="DateTime"/>, <see cref="object"/>,
        /// or <see cref="DBNull"/>.
        /// </param>
        /// <param name="result">
        /// Receives the mapped <see cref="BasicTypeCode"/> for the provided type; set to
        /// <see cref="BasicTypeCode.NotSupported"/> when the type is not recognized.
        /// </param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
        public void Convert(Type value, ref BasicTypeCode result)
        {
            result = BasicTypeCode.NotSupported;

            if (value.IsAssignableFrom(typeof(bool)))
                result = BasicTypeCode.Boolean;
            else if (value.IsAssignableFrom(typeof(byte)))
                result = BasicTypeCode.IntegerNumber;
            else if (value.IsAssignableFrom(typeof(char)))
                result = BasicTypeCode.IntegerNumber;
            else if (value.IsAssignableFrom(typeof(DateTime)))
                result = BasicTypeCode.DateTime;
            else if (value.IsAssignableFrom(typeof(DBNull)))
                result = BasicTypeCode.NotSupported;
            else if (value.IsAssignableFrom(typeof(decimal)))
                result = BasicTypeCode.FloatingPointNumber;
            else if (value.IsAssignableFrom(typeof(double)))
                result = BasicTypeCode.FloatingPointNumber;
            else if (value.IsAssignableFrom(typeof(short)))
                result = BasicTypeCode.IntegerNumber;
            else if (value.IsAssignableFrom(typeof(int)))
                result = BasicTypeCode.IntegerNumber;
            else if (value.IsAssignableFrom(typeof(long)))
                result = BasicTypeCode.IntegerNumber;
            else if (value.IsAssignableFrom(typeof(sbyte)))
                result = BasicTypeCode.IntegerNumber;
            else if (value.IsAssignableFrom(typeof(float)))
                result = BasicTypeCode.FloatingPointNumber;
            else if (value.IsAssignableFrom(typeof(string)))
                result = BasicTypeCode.String;
            else if (value.IsAssignableFrom(typeof(ushort)))
                result = BasicTypeCode.IntegerNumber;
            else if (value.IsAssignableFrom(typeof(uint)))
                result = BasicTypeCode.IntegerNumber;
            else if (value.IsAssignableFrom(typeof(ulong)))
                result = BasicTypeCode.IntegerNumber;
            else if (value.IsAssignableFrom(typeof(object)))
                result = BasicTypeCode.Object;
        }

        /// <summary>
        /// Converts the provided <see cref="BasicTypeCode"/> to the representative <see cref="Type"/> defined by the
        /// adapter for Boolean, numeric, string, date, object, and unsupported values.
        /// </summary>
        /// <param name="value">
        /// A <see cref="BasicTypeCode"/> value that must correspond to one of the supported categories such as
        /// <see cref="BasicTypeCode.IntegerNumber"/>, <see cref="BasicTypeCode.FloatingPointNumber"/>,
        /// <see cref="BasicTypeCode.String"/>, <see cref="BasicTypeCode.Boolean"/>, <see cref="BasicTypeCode.DateTime"/>,
        /// <see cref="BasicTypeCode.Object"/>, <see cref="BasicTypeCode.Image"/>, or <see cref="BasicTypeCode.Graphic"/>.
        /// </param>
        /// <returns>
        /// The representative <see cref="Type"/> mapped from the provided basic type code; returns the
        /// <see cref="Type"/> object for <see cref="DBNull"/> when the value is <see cref="BasicTypeCode.NotSupported"/>.
        /// </returns>
        public Type Convert(BasicTypeCode value)
        {
            var result = typeof(DBNull);
            Convert(value, ref result);
            return result;
        }

        /// <summary>
        /// Converts the provided <see cref="BasicTypeCode"/> to the representative <see cref="Type"/> and stores it in
        /// the supplied reference parameter.
        /// </summary>
        /// <param name="value">
        /// A <see cref="BasicTypeCode"/> value that must correspond to one of the supported categories such as
        /// <see cref="BasicTypeCode.IntegerNumber"/>, <see cref="BasicTypeCode.FloatingPointNumber"/>,
        /// <see cref="BasicTypeCode.String"/>, <see cref="BasicTypeCode.Boolean"/>, <see cref="BasicTypeCode.DateTime"/>,
        /// <see cref="BasicTypeCode.Object"/>, <see cref="BasicTypeCode.Image"/>, or <see cref="BasicTypeCode.Graphic"/>.
        /// </param>
        /// <param name="result">
        /// Receives the representative <see cref="Type"/> mapped from the provided basic type code; set to
        /// <see cref="DBNull"/> when the value is <see cref="BasicTypeCode.NotSupported"/>.
        /// </param>
        public void Convert(BasicTypeCode value, ref Type result)
        {
            switch (value)
            {
                case BasicTypeCode.IntegerNumber:
                    result = typeof(long);
                    break;
                case BasicTypeCode.FloatingPointNumber:
                    result = typeof(double);
                    break;
                case BasicTypeCode.String:
                    result = typeof(string);
                    break;
                case BasicTypeCode.Boolean:
                    result = typeof(bool);
                    break;
                case BasicTypeCode.DateTime:
                    result = typeof(DateTime);
                    break;
                case BasicTypeCode.Object:
                case BasicTypeCode.Image:
                case BasicTypeCode.Graphic:
                    result = typeof(object);
                    break;
            case BasicTypeCode.NotSupported:
                result = typeof(DBNull);
                    break;
        }
    }
    }
}
