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

namespace VisionNet.Core.Bitwise
{
    public static partial class Bits
    {
        private static void ThrowIndexOutOfRange() => throw new IndexOutOfRangeException();

        internal static ulong ToUnsigned(long value) => unchecked((ulong)value);
        internal static uint ToUnsigned(int value) => unchecked((uint)value);
        internal static ushort ToUnsigned(short value) => unchecked((ushort)value);
        internal static byte ToUnsigned(sbyte value) => unchecked((byte)value);
    }
}
