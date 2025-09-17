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
        public BasicTypeCode Convert(Type value)
        {
            var result = BasicTypeCode.NotSupported;
            Convert(value, ref result);
            return result;
        }

        /// <summary> The Convert function converts a Type value to the corresponding type.</summary>
        /// <param name="value"> What is this for?</param>
        /// <param name="result"> What is this used for?</param>
        /// <result =s> A type object.</result =s>
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

        public Type Convert(BasicTypeCode value)
        {
            var result = typeof(DBNull);
            Convert(value, ref result);
            return result;
        }

        /// <summary> The Convert function converts a Type value to the corresponding type.</summary>
        /// <param name="value"> What is this for?</param>
        /// <param name="result"> What is this used for?</param>
        /// <result =s> A type object.</result =s>
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
