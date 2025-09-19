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

namespace VisionNet.Core
{
    /// <summary>
    /// Provides extension methods for working with <see cref="DateTime"/> objects.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Truncates the specified <see cref="DateTime"/> to the nearest multiple of the specified <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime"/> instance to truncate.</param>
        /// <param name="timeSpan">The <see cref="TimeSpan"/> representing the interval to which the <see cref="DateTime"/> should be truncated.</param>
        /// <returns>A <see cref="DateTime"/> object truncated to the nearest multiple of the specified <see cref="TimeSpan"/>.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="timeSpan"/> is <see cref="TimeSpan.Zero"/>.</exception>
        ///
        /// <remarks>
        /// This method truncates the <paramref name="dateTime"/> to the nearest multiple of the <paramref name="timeSpan"/>.
        /// The <paramref name="dateTime"/> will be adjusted so that its time component matches the largest whole multiple
        /// of <paramref name="timeSpan"/> that is less than or equal to the original value.
        /// 
        /// If <paramref name="timeSpan"/> is <see cref="TimeSpan.Zero"/>, an exception will be thrown. A <see cref="DateTime"/>
        /// value of <see cref="DateTime.MinValue"/> or <see cref="DateTime.MaxValue"/> will be returned unchanged,
        /// as these are typically used as guard values and should not be modified.
        /// 
        /// The method uses the <see cref="DateTime.Ticks"/> property for truncation, and the <paramref name="dateTime"/>
        /// will be adjusted by subtracting the remainder when the ticks are divided by the ticks of the <paramref name="timeSpan"/>.
        /// </remarks>
        public static DateTime Truncate(this DateTime dateTime, TimeSpan timeSpan)
        {
            // If the timeSpan is zero, return the original dateTime (or throw an ArgumentException if preferred).
            if (timeSpan == TimeSpan.Zero) return dateTime; // Or could throw an ArgumentException

            // Avoid modifying the "guard" values DateTime.MinValue and DateTime.MaxValue.
            // These values are often used to represent indefinite or extreme dates, and truncating them could result in unexpected behavior.
            if (dateTime == DateTime.MinValue || dateTime == DateTime.MaxValue) return dateTime; // do not modify "guard" values

            // Subtract the remainder of ticks when divided by the ticks of the provided timeSpan, effectively truncating the dateTime.
            return dateTime.AddTicks(-(dateTime.Ticks % timeSpan.Ticks));
        }
    }

}
