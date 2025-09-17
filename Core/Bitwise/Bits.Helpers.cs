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
    /// <summary>
    /// Provides helper routines used by <see cref="Bits"/> partial implementations when performing unchecked bitwise conversions.
    /// </summary>
    public static partial class Bits
    {
        /// <summary>
        /// Throws a standardized <see cref="IndexOutOfRangeException"/> when a bit index exceeds the supported range.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">Always thrown to signal that a requested bit index is outside the valid range.</exception>
        private static void ThrowIndexOutOfRange() => throw new IndexOutOfRangeException();

        /// <summary>
        /// Reinterprets a signed 64-bit integer as its unsigned counterpart without performing overflow checks.
        /// </summary>
        /// <param name="value">Signed 64-bit integer value to convert to an unsigned representation.</param>
        /// <returns>The bitwise equivalent unsigned 64-bit integer.</returns>
        internal static ulong ToUnsigned(long value) => unchecked((ulong)value);

        /// <summary>
        /// Reinterprets a signed 32-bit integer as its unsigned counterpart without performing overflow checks.
        /// </summary>
        /// <param name="value">Signed 32-bit integer value to convert to an unsigned representation.</param>
        /// <returns>The bitwise equivalent unsigned 32-bit integer.</returns>
        internal static uint ToUnsigned(int value) => unchecked((uint)value);

        /// <summary>
        /// Reinterprets a signed 16-bit integer as its unsigned counterpart without performing overflow checks.
        /// </summary>
        /// <param name="value">Signed 16-bit integer value to convert to an unsigned representation.</param>
        /// <returns>The bitwise equivalent unsigned 16-bit integer.</returns>
        internal static ushort ToUnsigned(short value) => unchecked((ushort)value);

        /// <summary>
        /// Reinterprets a signed 8-bit integer as its unsigned counterpart without performing overflow checks.
        /// </summary>
        /// <param name="value">Signed 8-bit integer value to convert to an unsigned representation.</param>
        /// <returns>The bitwise equivalent unsigned 8-bit integer.</returns>
        internal static byte ToUnsigned(sbyte value) => unchecked((byte)value);
    }
}
