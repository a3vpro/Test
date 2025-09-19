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
using VisionNet.Core.Maths;

namespace VisionNet.Core.ByteArrayHelper
{
    /// <summary>
    /// Provides utility functions for working with Siemens S7 byte arrays and various data types used in Siemens PLC systems.
    /// </summary>
    public class SiemensByteArrayFunctions
    {
        #region Constants
        /// <summary>
        /// Constant representing the type identifier for a bit.
        /// </summary>
        public const int TYPE_BIT = 0x01;

        /// <summary>
        /// Constant representing the type identifier for a byte.
        /// </summary>
        public const int TYPE_BYTE = 0x02;

        /// <summary>
        /// Constant representing the type identifier for a character.
        /// </summary>
        public const int TYPE_CHAR = 0x03;

        /// <summary>
        /// Constant representing the type identifier for a word.
        /// </summary>
        public const int TYPE_WORD = 0x04;

        /// <summary>
        /// Constant representing the type identifier for an integer.
        /// </summary>
        public const int TYPE_INT = 0x05;

        /// <summary>
        /// Constant representing the type identifier for a double word.
        /// </summary>
        public const int TYPE_DWORD = 0x06;

        /// <summary>
        /// Constant representing the type identifier for a double integer.
        /// </summary>
        public const int TYPE_DINT = 0x07;

        /// <summary>
        /// Constant representing the type identifier for a real number.
        /// </summary>
        public const int TYPE_REAL = 0x08;

        /// <summary>
        /// Constant representing the type identifier for a counter.
        /// </summary>
        public const int TYPE_COUNTER = 0x1C;

        //public const int TYPE_TIMER = 0x1D;
        #endregion


        private const Int64 BIAS = 621355968000000000; // "decimicros" between 0001-01-01 00:00:00 and 1970-01-01 00:00:00

        /// <summary>
        /// Converts a binary-coded decimal (BCD) value to an integer.
        /// </summary>
        /// <param name="b">The BCD value to convert.</param>
        /// <returns>The integer equivalent of the BCD value.</returns>
        private static int _bcdToByte(byte b)
        {
            return ((b >> 4) * 10) + (b & 0x0F);
        }

        /// <summary>
        /// Converts a byte value to binary-coded decimal (BCD).
        /// </summary>
        /// <param name="value">The integer value to convert.</param>
        /// <returns>A byte containing the BCD equivalent of the specified value.</returns>
        private static byte _byteToBcd(int value)
        {
            return (byte)(((value / 10) << 4) | (value % 10));
        }

        /// <summary>
        /// Copies a specified portion of a byte array.
        /// </summary>
        /// <param name="buffer">The byte array to copy from.</param>
        /// <param name="pos">The position in the array to start copying from.</param>
        /// <param name="size">The number of bytes to copy.</param>
        /// <returns>A new byte array containing the copied data.</returns>
        private static byte[] _copyFrom(byte[] buffer, int pos, int size)
        {
            byte[] result = new byte[size];
            Array.Copy(buffer, pos, result, 0, size);
            return result;
        }

        #region [Help Functions]
        /// <summary>
        /// Returns the size in bytes of a given data type.
        /// </summary>
        /// <param name="wordLength">The data type identifier.</param>
        /// <returns>The number of bytes required to store the specified data type.</returns>
        public static int DataSizeByte(int wordLength)
        {
            switch (wordLength)
            {
                case TYPE_BIT: return 1;  // S7 sends 1 byte per bit
                case TYPE_BYTE: return 1;
                case TYPE_CHAR: return 1;
                case TYPE_WORD: return 2;
                case TYPE_DWORD: return 4;
                case TYPE_INT: return 2;
                case TYPE_DINT: return 4;
                case TYPE_REAL: return 4;
                case TYPE_COUNTER: return 2;
                //case TYPE_TIMER: return 2;
                default: return 0;
            }
        }

        #region Get/Set the bit at pos.Bit
        /// <summary>
        /// Gets the value of a specific bit in a byte array.
        /// </summary>
        /// <param name="buffer">The byte array containing the bit.</param>
        /// <param name="pos">The position in the array to get the bit from.</param>
        /// <param name="bit">The bit position within the byte (0 to 7).</param>
        /// <returns><c>true</c> if the bit is set, otherwise <c>false</c>.</returns>
        public static bool GetBitAt(byte[] buffer, int pos, int bit)
        {
            byte[] mask = { 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80 };
            if (bit < 0) bit = 0;
            if (bit > 7) bit = 7;
            return (buffer[pos] & mask[bit]) != 0;
        }

        /// <summary>
        /// Sets the value of a specific bit in a byte array.
        /// </summary>
        /// <param name="buffer">The byte array to modify.</param>
        /// <param name="pos">The position in the array to set the bit.</param>
        /// <param name="bit">The bit position within the byte (0 to 7).</param>
        /// <param name="value">The value to set the bit to (<c>true</c> or <c>false</c>).</param>
        public static void SetBitAt(ref byte[] buffer, int pos, int bit, bool value)
        {
            byte[] mask = { 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80 };
            if (bit < 0) bit = 0;
            if (bit > 7) bit = 7;

            if (value)
                buffer[pos] = (byte)(buffer[pos] | mask[bit]);
            else
                buffer[pos] = (byte)(buffer[pos] & ~mask[bit]);
        }
        #endregion

        #region Get/Set 8 bit signed value (S7 SInt) -128..127
        /// <summary>
        /// Retrieves an 8-bit signed integer (S7 SInt) from a byte array.
        /// </summary>
        /// <param name="buffer">The byte array containing the data.</param>
        /// <param name="pos">The position in the array to read from.</param>
        /// <returns>The 8-bit signed integer value.</returns>
        public static int GetSIntAt(byte[] buffer, int pos)
        {
            int value = buffer[pos];
            if (value < 128)
                return value;
            else
                return (int)(value - 256);

        }

        /// <summary>
        /// Sets an 8-bit signed integer (S7 SInt) in a byte array.
        /// </summary>
        /// <param name="buffer">The byte array to modify.</param>
        /// <param name="pos">The position in the array to set the value.</param>
        /// <param name="value">The 8-bit signed integer to set.</param>
        public static void SetSIntAt(byte[] buffer, int pos, int value)
        {
            if (value < -128)
                value = -128;

            if (value > 127)
                value = 127;

            buffer[pos] = (byte)value;

        }
        #endregion

        #region Get/Set 16 bit signed value (S7 int) -32768..32767
        /// <summary>
        /// Retrieves a 16-bit signed integer (S7 Int) from a byte array.
        /// </summary>
        /// <param name="buffer">The byte array containing the data.</param>
        /// <param name="pos">The position in the array to read from.</param>
        /// <returns>The 16-bit signed integer value.</returns>
        public static short GetIntAt(byte[] buffer, int pos)
        {
            return (short)((buffer[pos] << 8) | buffer[pos + 1]);
        }


        /// <summary>
        /// Sets a 16-bit signed integer (S7 Int) in a byte array.
        /// </summary>
        /// <param name="buffer">The byte array to modify.</param>
        /// <param name="pos">The position in the array to set the value.</param>
        /// <param name="value">The 16-bit signed integer to set.</param>
        public static void SetIntAt(byte[] buffer, int pos, Int16 value)
        {
            buffer[pos] = (byte)(value >> 8);
            buffer[pos + 1] = (byte)(value & 0x00FF);
        }
        #endregion

        #region Get/Set 32 bit signed value (S7 DInt) -2147483648..2147483647
        /// <summary>
        /// Retrieves a 32-bit signed integer (S7 DInt) from a byte array.
        /// </summary>
        /// <param name="buffer">The byte array containing the data.</param>
        /// <param name="pos">The position in the array to start reading from.</param>
        /// <returns>The 32-bit signed integer value.</returns>
        public static int GetDIntAt(byte[] buffer, int pos)
        {
            int result;
            result = buffer[pos]; result <<= 8;
            result += buffer[pos + 1]; result <<= 8;
            result += buffer[pos + 2]; result <<= 8;
            result += buffer[pos + 3];
            return result;
        }

        /// <summary>
        /// Sets a 32-bit signed integer (S7 DInt) in a byte array.
        /// </summary>
        /// <param name="buffer">The byte array to modify.</param>
        /// <param name="pos">The position in the array to set the value.</param>
        /// <param name="value">The 32-bit signed integer value to set.</param>
        public static void SetDIntAt(byte[] buffer, int pos, int value)
        {
            buffer[pos + 3] = (byte)(value & 0xFF);
            buffer[pos + 2] = (byte)((value >> 8) & 0xFF);
            buffer[pos + 1] = (byte)((value >> 16) & 0xFF);
            buffer[pos] = (byte)((value >> 24) & 0xFF);
        }
        #endregion

        #region Get/Set 64 bit signed value (S7 LInt) -9223372036854775808..9223372036854775807
        /// <summary>
        /// Retrieves a 64-bit signed integer (S7 LInt) from a byte array.
        /// </summary>
        /// <param name="buffer">The byte array containing the data.</param>
        /// <param name="pos">The position in the array to start reading from.</param>
        /// <returns>The 64-bit signed integer value.</returns>
        public static Int64 GetLIntAt(byte[] buffer, int pos)
        {
            Int64 result;
            result = buffer[pos]; result <<= 8;
            result += buffer[pos + 1]; result <<= 8;
            result += buffer[pos + 2]; result <<= 8;
            result += buffer[pos + 3]; result <<= 8;
            result += buffer[pos + 4]; result <<= 8;
            result += buffer[pos + 5]; result <<= 8;
            result += buffer[pos + 6]; result <<= 8;
            result += buffer[pos + 7];
            return result;
        }

        /// <summary>
        /// Sets a 64-bit signed integer (S7 LInt) in a byte array.
        /// </summary>
        /// <param name="buffer">The byte array to modify.</param>
        /// <param name="pos">The position in the array to set the value.</param>
        /// <param name="value">The 64-bit signed integer value to set.</param>
        public static void SetLIntAt(byte[] buffer, int pos, Int64 value)
        {
            buffer[pos + 7] = (byte)(value & 0xFF);
            buffer[pos + 6] = (byte)((value >> 8) & 0xFF);
            buffer[pos + 5] = (byte)((value >> 16) & 0xFF);
            buffer[pos + 4] = (byte)((value >> 24) & 0xFF);
            buffer[pos + 3] = (byte)((value >> 32) & 0xFF);
            buffer[pos + 2] = (byte)((value >> 40) & 0xFF);
            buffer[pos + 1] = (byte)((value >> 48) & 0xFF);
            buffer[pos] = (byte)((value >> 56) & 0xFF);
        }
        #endregion

        #region Get/Set 8 bit unsigned value (S7 USInt) 0..255
        /// <summary>
        /// Retrieves an 8-bit unsigned integer (S7 USInt) from a byte array.
        /// </summary>
        /// <param name="buffer">The byte array containing the data.</param>
        /// <param name="pos">The position in the array to start reading from.</param>
        /// <returns>The 8-bit unsigned integer value.</returns>
        public static byte GetUSIntAt(byte[] buffer, int pos)
        {
            return buffer[pos];
        }

        /// <summary>
        /// Sets an 8-bit unsigned integer (S7 USInt) in a byte array.
        /// </summary>
        /// <param name="buffer">The byte array to modify.</param>
        /// <param name="pos">The position in the array to set the value.</param>
        /// <param name="value">The 8-bit unsigned integer value to set.</param>
        public static void SetUSIntAt(byte[] buffer, int pos, byte value)
        {
            buffer[pos] = value;
        }
        #endregion

        #region Get/Set 16 bit unsigned value (S7 UInt) 0..65535
        /// <summary>
        /// Retrieves a 16-bit unsigned integer (S7 UInt) from a byte array.
        /// </summary>
        /// <param name="buffer">The byte array containing the data.</param>
        /// <param name="pos">The position in the array to start reading from.</param>
        /// <returns>The 16-bit unsigned integer value.</returns>
        public static UInt16 GetUIntAt(byte[] buffer, int pos)
        {
            return (UInt16)((buffer[pos] << 8) | buffer[pos + 1]);
        }

        /// <summary>
        /// Sets a 16-bit unsigned integer (S7 UInt) in a byte array.
        /// </summary>
        /// <param name="buffer">The byte array to modify.</param>
        /// <param name="pos">The position in the array to set the value.</param>
        /// <param name="value">The 16-bit unsigned integer value to set.</param>
        public static void SetUIntAt(byte[] buffer, int pos, UInt16 value)
        {
            buffer[pos] = (byte)(value >> 8);
            buffer[pos + 1] = (byte)(value & 0x00FF);
        }
        #endregion

        #region Get/Set 32 bit unsigned value (S7 UDInt) 0..4294967296
        /// <summary>
        /// Retrieves a 32-bit unsigned integer (S7 UDInt) from a byte array.
        /// </summary>
        /// <param name="buffer">The byte array containing the data.</param>
        /// <param name="pos">The position in the array to start reading from.</param>
        /// <returns>The 32-bit unsigned integer value.</returns>
        public static UInt32 GetUDIntAt(byte[] buffer, int pos)
        {
            UInt32 result;
            result = buffer[pos]; result <<= 8;
            result |= buffer[pos + 1]; result <<= 8;
            result |= buffer[pos + 2]; result <<= 8;
            result |= buffer[pos + 3];
            return result;
        }

        /// <summary>
        /// Sets a 32-bit unsigned integer (S7 UDInt) in a byte array.
        /// </summary>
        /// <param name="buffer">The byte array to modify.</param>
        /// <param name="pos">The position in the array to set the value.</param>
        /// <param name="value">The 32-bit unsigned integer value to set.</param>
        public static void SetUDIntAt(byte[] buffer, int pos, UInt32 value)
        {
            buffer[pos + 3] = (byte)(value & 0xFF);
            buffer[pos + 2] = (byte)((value >> 8) & 0xFF);
            buffer[pos + 1] = (byte)((value >> 16) & 0xFF);
            buffer[pos] = (byte)((value >> 24) & 0xFF);
        }
        #endregion

        #region Get/Set 64 bit unsigned value (S7 ULint) 0..18446744073709551616
        /// <summary>
        /// Retrieves a 64-bit unsigned integer (S7 ULint) from a byte array.
        /// </summary>
        /// <param name="buffer">The byte array containing the data.</param>
        /// <param name="pos">The position in the array to start reading from.</param>
        /// <returns>The 64-bit unsigned integer value.</returns>
        public static UInt64 GetULIntAt(byte[] buffer, int pos)
        {
            UInt64 result;
            result = buffer[pos]; result <<= 8;
            result |= buffer[pos + 1]; result <<= 8;
            result |= buffer[pos + 2]; result <<= 8;
            result |= buffer[pos + 3]; result <<= 8;
            result |= buffer[pos + 4]; result <<= 8;
            result |= buffer[pos + 5]; result <<= 8;
            result |= buffer[pos + 6]; result <<= 8;
            result |= buffer[pos + 7];
            return result;
        }

        /// <summary>
        /// Sets a 64-bit unsigned integer (S7 ULint) in a byte array.
        /// </summary>
        /// <param name="buffer">The byte array to modify.</param>
        /// <param name="pos">The position in the array to set the value.</param>
        /// <param name="value">The 64-bit unsigned integer value to set.</param>
        public static void SetULintAt(byte[] buffer, int pos, UInt64 value)
        {
            buffer[pos + 7] = (byte)(value & 0xFF);
            buffer[pos + 6] = (byte)((value >> 8) & 0xFF);
            buffer[pos + 5] = (byte)((value >> 16) & 0xFF);
            buffer[pos + 4] = (byte)((value >> 24) & 0xFF);
            buffer[pos + 3] = (byte)((value >> 32) & 0xFF);
            buffer[pos + 2] = (byte)((value >> 40) & 0xFF);
            buffer[pos + 1] = (byte)((value >> 48) & 0xFF);
            buffer[pos] = (byte)((value >> 56) & 0xFF);
        }
        #endregion

        #region Get/Set 8 bit word (S7 Byte) 16#00..16#FF
        /// <summary>
        /// Retrieves an 8-bit unsigned integer (S7 Byte) from a byte array.
        /// </summary>
        /// <param name="buffer">The byte array containing the data.</param>
        /// <param name="pos">The position in the array to start reading from.</param>
        /// <returns>The 8-bit unsigned integer value.</returns>
        public static byte GetByteAt(byte[] buffer, int pos)
        {
            return buffer[pos];
        }

        /// <summary>
        /// Sets an 8-bit unsigned integer (S7 Byte) in a byte array.
        /// </summary>
        /// <param name="buffer">The byte array to modify.</param>
        /// <param name="pos">The position in the array to set the value.</param>
        /// <param name="value">The 8-bit unsigned integer value to set.</param>
        public static void SetByteAt(byte[] buffer, int pos, byte value)
        {
            buffer[pos] = value;
        }
        #endregion

        #region Get/Set 16 bit word (S7 Word) 16#0000..16#FFFF
        /// <summary>
        /// Retrieves a 16-bit unsigned integer (S7 Word) from a byte array.
        /// </summary>
        /// <param name="buffer">The byte array containing the data.</param>
        /// <param name="pos">The position in the array to start reading from.</param>
        /// <returns>The 16-bit unsigned integer value.</returns>
        public static UInt16 GetWordAt(byte[] buffer, int pos)
        {
            return GetUIntAt(buffer, pos);
        }
        /// <summary>
        /// Sets a 16-bit unsigned integer (S7 Word) in a byte array.
        /// </summary>
        /// <param name="buffer">The byte array to modify.</param>
        /// <param name="pos">The position in the array to set the value.</param>
        /// <param name="value">The 16-bit unsigned integer value to set.</param>
        public static void SetWordAt(byte[] buffer, int pos, UInt16 value)
        {
            SetUIntAt(buffer, pos, value);
        }
        #endregion

        #region Get/Set 32 bit word (S7 DWord) 16#00000000..16#FFFFFFFF
        /// <summary>
        /// Retrieves a 32-bit unsigned integer (S7 DWord) from a byte array.
        /// </summary>
        /// <param name="buffer">The byte array containing the data.</param>
        /// <param name="pos">The position in the array to start reading from.</param>
        /// <returns>The 32-bit unsigned integer value.</returns>   
        public static UInt32 GetDWordAt(byte[] buffer, int pos)
        {
            return GetUDIntAt(buffer, pos);
        }
        /// <summary>
        /// Sets a 32-bit unsigned integer (DWORD) at a specified position in the byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array where the DWORD will be set.</param>
        /// <param name="pos">The position in the byte array where the DWORD should be placed.</param>
        /// <param name="value">The 32-bit unsigned integer (DWORD) value to set in the buffer.</param>
        public static void SetDWordAt(byte[] buffer, int pos, UInt32 value)

        {
            SetUDIntAt(buffer, pos, value);
        }
        #endregion

        #region Get/Set 64 bit word (S7 LWord) 16#0000000000000000..16#FFFFFFFFFFFFFFFF
        /// <summary>
        /// Retrieves a 64-bit unsigned integer (LWORD) from a specified position in the byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array from which the LWORD will be retrieved.</param>
        /// <param name="pos">The position in the byte array from which the LWORD should be read.</param>
        /// <returns>A 64-bit unsigned integer (LWORD) value retrieved from the buffer.</returns>
        public static UInt64 GetLWordAt(byte[] buffer, int pos)

        {
            return GetULIntAt(buffer, pos);
        }
        /// <summary>
        /// Sets a 64-bit unsigned integer (LWORD) at a specified position in the byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array where the LWORD will be set.</param>
        /// <param name="pos">The position in the byte array where the LWORD should be placed.</param>
        /// <param name="value">The 64-bit unsigned integer (LWORD) value to set in the buffer.</param>
        public static void SetLWordAt(byte[] buffer, int pos, UInt64 value)

        {
            SetULintAt(buffer, pos, value);
        }
        #endregion

        #region Get/Set 32 bit floating point number (S7 Real) (Range of Single)
        /// <summary>
        /// Retrieves a single-precision floating-point value (Real) from a specified position in the byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array from which the real value will be retrieved.</param>
        /// <param name="pos">The position in the byte array from which the real value should be read.</param>
        /// <returns>A single-precision floating-point value (Real) retrieved from the buffer.</returns>
        public static Single GetRealAt(byte[] buffer, int pos)

        {
            UInt32 value = GetUDIntAt(buffer, pos);
            byte[] bytes = BitConverter.GetBytes(value);
            return BitConverter.ToSingle(bytes, 0);
        }
        /// <summary>
        /// Sets a single-precision floating-point value (Real) at a specified position in the byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array where the real value will be set.</param>
        /// <param name="pos">The position in the byte array where the real value should be placed.</param>
        /// <param name="value">The single-precision floating-point value (Real) to set in the buffer.</param>
        public static void SetRealAt(byte[] buffer, int pos, Single value)

        {
            byte[] floatArray = BitConverter.GetBytes(value);
            buffer[pos] = floatArray[3];
            buffer[pos + 1] = floatArray[2];
            buffer[pos + 2] = floatArray[1];
            buffer[pos + 3] = floatArray[0];
        }
        #endregion

        #region Get/Set 64 bit floating point number (S7 LReal) (Range of Double)
        /// <summary>
        /// Retrieves a double-precision floating-point value (LReal) from a specified position in the byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array from which the LReal value will be retrieved.</param>
        /// <param name="pos">The position in the byte array from which the LReal value should be read.</param>
        /// <returns>A double-precision floating-point value (LReal) retrieved from the buffer.</returns>
        public static Double GetLRealAt(byte[] buffer, int pos)

        {
            UInt64 value = GetULIntAt(buffer, pos);
            byte[] bytes = BitConverter.GetBytes(value);
            return BitConverter.ToDouble(bytes, 0);
        }
        /// <summary>
        /// Sets a double-precision floating-point value (LReal) at a specified position in the byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array where the LReal value will be set.</param>
        /// <param name="pos">The position in the byte array where the LReal value should be placed.</param>
        /// <param name="value">The double-precision floating-point value (LReal) to set in the buffer.</param>
        public static void SetLRealAt(byte[] buffer, int pos, Double value)

        {
            byte[] floatArray = BitConverter.GetBytes(value);
            buffer[pos] = floatArray[7];
            buffer[pos + 1] = floatArray[6];
            buffer[pos + 2] = floatArray[5];
            buffer[pos + 3] = floatArray[4];
            buffer[pos + 4] = floatArray[3];
            buffer[pos + 5] = floatArray[2];
            buffer[pos + 6] = floatArray[1];
            buffer[pos + 7] = floatArray[0];
        }
        #endregion

        #region Get/Set DateTime (S7 DATE_AND_TIME)
        /// <summary>
        /// Retrieves a DateTime value from a specified position in the byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array from which the DateTime value will be retrieved.</param>
        /// <param name="pos">The position in the byte array from which the DateTime value should be read.</param>
        /// <returns>A DateTime value retrieved from the buffer.</returns>
        public static DateTime GetDateTimeAt(byte[] buffer, int pos)
        {
            int year, month, day, hour, minute, second, msec;

            year = _bcdToByte(buffer[pos]);
            if (year < 90)
                year += 2000;
            else
                year += 1900;

            month = _bcdToByte(buffer[pos + 1]);
            day = _bcdToByte(buffer[pos + 2]);
            hour = _bcdToByte(buffer[pos + 3]);
            minute = _bcdToByte(buffer[pos + 4]);
            second = _bcdToByte(buffer[pos + 5]);
            msec = (_bcdToByte(buffer[pos + 6]) * 10) + (_bcdToByte(buffer[pos + 7]) / 10);

            try
            {
                return new DateTime(year, month, day, hour, minute, second, msec);
            }
            catch (ArgumentOutOfRangeException)
            {
                return new DateTime(0);
            }
        }

        /// <summary>
        /// Sets a DateTime value at a specified position in the byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array where the DateTime value will be set.</param>
        /// <param name="pos">The position in the byte array where the DateTime value should be placed.</param>
        /// <param name="value">The DateTime value to set in the buffer.</param>
        public static void SetDateTimeAt(byte[] buffer, int pos, DateTime value)
        {
            int year = value.Year;
            int month = value.Month;
            int day = value.Day;
            int hour = value.Hour;
            int minute = value.Minute;
            int second = value.Second;
            int dayOfWeek = (int)value.DayOfWeek + 1;

            // MSecH = First two digits of milliseconds
            int msecH = value.Millisecond / 10;
            // MSecL = Last digit of milliseconds
            int msecL = value.Millisecond % 10;

            if (year > 1999)
                year -= 2000;

            buffer[pos] = _byteToBcd(year);
            buffer[pos + 1] = _byteToBcd(month);
            buffer[pos + 2] = _byteToBcd(day);
            buffer[pos + 3] = _byteToBcd(hour);
            buffer[pos + 4] = _byteToBcd(minute);
            buffer[pos + 5] = _byteToBcd(second);
            buffer[pos + 6] = _byteToBcd(msecH);
            buffer[pos + 7] = _byteToBcd(msecL * 10 + dayOfWeek);
        }

        #endregion

        #region Get/Set DATE (S7 DATE) 
        private static readonly DateTime BASE_DATE = new DateTime(1990, 1, 1);



        /// <summary>
        /// Retrieves a <see cref="DateTime"/> from the specified byte array at the given position.
        /// The method interprets the value at the given position as the number of days 
        /// since a predefined base date and adds it to that base date.
        /// </summary>
        /// <param name="buffer">The byte array containing the data.</param>
        /// <param name="pos">The position in the byte array where the day count is stored.</param>
        /// <returns>A <see cref="DateTime"/> calculated by adding the day count to the base date. 
        /// If an error occurs, a <see cref="DateTime"/> representing the Unix epoch (January 1, 0001) is returned.</returns>
        public static DateTime GetDateAt(byte[] buffer, int pos)
        {
            try
            {
                return BASE_DATE.AddDays(GetIntAt(buffer, pos));
            }
            catch (System.ArgumentOutOfRangeException)
            {
                return new DateTime(0);
            }
        }

        /// <summary>
        /// Sets a <see cref="DateTime"/> value at the specified position in the byte array.
        /// The method converts the <see cref="DateTime"/> value into the number of days 
        /// since a predefined base date and stores it as a 16-bit signed integer.
        /// </summary>
        /// <param name="buffer">The byte array to which the date will be written.</param>
        /// <param name="pos">The position in the byte array where the date value should be stored.</param>
        /// <param name="value">The <see cref="DateTime"/> value to be stored.</param>
        public static void SetDateAt(byte[] buffer, int pos, DateTime value)
        {
            SetIntAt(buffer, pos, (Int16)(value - BASE_DATE).Days);
        }
        /// <summary>
        /// Converts a DateTime to a S7 DATE value.
        /// </summary>
        /// <param name="value">The DateTime to convert.</param>
        /// <returns>The corresponding S7 DATE value.</returns>
        public static ushort SetDate(DateTime value)
        {
            var difference = value - BASE_DATE;
            return (ushort)difference.Days;
        }
        #endregion

        #region Get/Set TOD (S7 TIME_OF_DAY)
        /// <summary>
        /// Retrieves a Time-of-Day (TOD) value from a specified position in the byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array from which the TOD value will be retrieved.</param>
        /// <param name="pos">The position in the byte array from which the TOD value should be read.</param>
        /// <returns>A DateTime representing the Time-of-Day (TOD) value retrieved from the buffer.</returns>
        public static DateTime GetTODAt(byte[] buffer, int pos)

        {
            try
            {
                return new DateTime(0).AddMilliseconds(GetDIntAt(buffer, pos));
            }
            catch (System.ArgumentOutOfRangeException)
            {
                return new DateTime(0);
            }
        }
        /// <summary>
        /// Sets a Time-of-Day (TOD) value at a specified position in the byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array where the TOD value will be set.</param>
        /// <param name="pos">The position in the byte array where the TOD value should be placed.</param>
        /// <param name="value">The DateTime representing the Time-of-Day (TOD) value to set in the buffer.</param>
        public static void SetTODAt(byte[] buffer, int pos, DateTime value)

        {
            TimeSpan time = value.TimeOfDay;
            SetDIntAt(buffer, pos, (Int32)System.Math.Round(time.TotalMilliseconds));
        }
        #endregion

        #region Get/Set LTOD (S7 1500 LONG TIME_OF_DAY)
        /// <summary>
        /// Retrieves a Local Time-of-Day (LTOD) value from a specified position in the byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array from which the LTOD value will be retrieved.</param>
        /// <param name="pos">The position in the byte array from which the LTOD value should be read.</param>
        /// <returns>A DateTime representing the Local Time-of-Day (LTOD) value retrieved from the buffer.</returns>
        public static DateTime GetLTODAt(byte[] buffer, int pos)

        {
            // .NET Tick = 100 ns, S71500 Tick = 1 ns
            try
            {
                return new DateTime(System.Math.Abs(GetLIntAt(buffer, pos) / 100));
            }
            catch (System.ArgumentOutOfRangeException)
            {
                return new DateTime(0);
            }
        }
        /// <summary>
        /// Sets a Local Time-of-Day (LTOD) value at a specified position in the byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array where the LTOD value will be set.</param>
        /// <param name="pos">The position in the byte array where the LTOD value should be placed.</param>
        /// <param name="value">The DateTime representing the Local Time-of-Day (LTOD) value to set in the buffer.</param>
        public static void SetLTODAt(byte[] buffer, int pos, DateTime value)

        {
            TimeSpan time = value.TimeOfDay;
            SetLIntAt(buffer, pos, (Int64)time.Ticks * 100);
        }
        #endregion

        #region GET/SET LDT (S7 1500 Long Date and Time)
        /// <summary>
        /// Retrieves a Local Date-Time (LDT) value from a specified position in the byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array from which the LDT value will be retrieved.</param>
        /// <param name="pos">The position in the byte array from which the LDT value should be read.</param>
        /// <returns>A DateTime representing the Local Date-Time (LDT) value retrieved from the buffer.</returns>
        public static DateTime GetLDTAt(byte[] buffer, int pos)

        {
            try
            {
                return new DateTime((GetLIntAt(buffer, pos) / 100) + BIAS);
            }
            catch (System.ArgumentOutOfRangeException)
            {
                return new DateTime(0);
            }
        }
        /// <summary>
        /// Sets a Local Date-Time (LDT) value at a specified position in the byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array where the LDT value will be set.</param>
        /// <param name="pos">The position in the byte array where the LDT value should be placed.</param>
        /// <param name="value">The DateTime representing the Local Date-Time (LDT) value to set in the buffer.</param>
        public static void SetLDTAt(byte[] buffer, int pos, DateTime value)

        {
            SetLIntAt(buffer, pos, (value.Ticks - BIAS) * 100);
        }
        #endregion

        #region Get/Set DTL (S71200/1500 Date and Time)
        /// <summary>
        /// Retrieves a Date-Time Local (DTL) value from a specified position in the byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array from which the DTL value will be retrieved.</param>
        /// <param name="pos">The position in the byte array from which the DTL value should be read.</param>
        /// <returns>A DateTime representing the Date-Time Local (DTL) value retrieved from the buffer.</returns>
        public static DateTime GetDTLAt(byte[] buffer, int pos)

        {
            int Year, Month, Day, Hour, min, Sec, MSec;

            Year = buffer[pos] * 256 + buffer[pos + 1];
            Month = buffer[pos + 2];
            Day = buffer[pos + 3];
            Hour = buffer[pos + 5];
            min = buffer[pos + 6];
            Sec = buffer[pos + 7];
            MSec = (int)GetUDIntAt(buffer, pos + 8) / 1000000;

            try
            {
                return new DateTime(Year, Month, Day, Hour, min, Sec, MSec);
            }
            catch (System.ArgumentOutOfRangeException)
            {
                return new DateTime(0);
            }
        }

        /// <summary>
        /// Retrieves a Date-Time Local (DTL) value from the beginning of the byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array from which the DTL value will be retrieved.</param>
        /// <returns>A DateTime representing the Date-Time Local (DTL) value retrieved from the buffer.</returns>
        public static DateTime GetDTL(byte[] buffer)

        {
            return GetDTLAt(buffer, 0);
        }

        /// <summary>
        /// Sets a Date-Time Local (DTL) value at a specified position in the byte array.
        /// </summary>
        /// <param name="buffer">The byte array where the Date-Time Local (DTL) value will be set.</param>
        /// <param name="pos">The position in the byte array where the Date-Time Local (DTL) value should be placed.</param>
        /// <param name="value">The DateTime value to set as a Date-Time Local (DTL) in the byte array.</param>
        public static void SetDTLAt(byte[] buffer, int pos, DateTime value)

        {
            short Year = (short)value.Year;
            byte Month = (byte)value.Month;
            byte Day = (byte)value.Day;
            byte Hour = (byte)value.Hour;
            byte min = (byte)value.Minute;
            byte Sec = (byte)value.Second;
            byte Dow = (byte)(value.DayOfWeek + 1);

            Int32 NanoSecs = value.Millisecond * 1000000;

            var bytes_short = BitConverter.GetBytes(Year);

            buffer[pos] = bytes_short[1];
            buffer[pos + 1] = bytes_short[0];
            buffer[pos + 2] = Month;
            buffer[pos + 3] = Day;
            buffer[pos + 4] = Dow;
            buffer[pos + 5] = Hour;
            buffer[pos + 6] = min;
            buffer[pos + 7] = Sec;
            SetDIntAt(buffer, pos + 8, NanoSecs);
        }

        /// <summary>
        /// Converts a DateTime value into a byte array representation of a Date-Time Local (DTL) value.
        /// </summary>
        /// <param name="value">The DateTime value to convert into a byte array.</param>
        /// <returns>A byte array representing the Date-Time Local (DTL) value.</returns>
        public static byte[] SetDTL(DateTime value)

        {
            var buffer = new byte[12];

            SetDTLAt(buffer, 0, value);

            return buffer;
        }
        #endregion

        #region Get/Set String (S7 String)
        /// <summary>
        /// Retrieves a string value from a specified position in the byte array.
        /// </summary>
        /// <param name="buffer">The byte array from which the string value will be retrieved.</param>
        /// <param name="pos">The position in the byte array from which the string value should be read.</param>
        /// <returns>A string representing the value retrieved from the byte array at the specified position.</returns>
        public static string GetStringAt(byte[] buffer, int pos)

        {
            int size = System.Math.Min((int)buffer[pos + 1], buffer.Length - 2);
            return System.Text.Encoding.UTF8.GetString(buffer, pos + 2, size);
        }

        /// <summary>
        /// Retrieves a string value from the beginning of the byte array.
        /// </summary>
        /// <param name="buffer">The byte array from which the string value will be retrieved.</param>
        /// <returns>A string representing the value retrieved from the byte array.</returns>
        public static string GetString(byte[] buffer)

        {
            return GetStringAt(buffer, 0);
        }

        /// <summary>
        /// Sets a string value at a specified position in the byte array, ensuring the string does not exceed the maximum length.
        /// </summary>
        /// <param name="buffer">The byte array where the string value will be set.</param>
        /// <param name="pos">The position in the byte array where the string value should be placed.</param>
        /// <param name="maxLen">The maximum length of the string to set in the buffer.</param>
        /// <param name="value">The string value to set in the byte array.</param>
        public static void SetStringAt(byte[] buffer, int pos, int maxLen, string value)

        {
            maxLen = maxLen.Clamp(0, 254);
            int size = System.Math.Min(value.Length, maxLen);

            buffer[pos] = (byte)maxLen;
            buffer[pos + 1] = (byte)size;
            System.Text.Encoding.UTF8.GetBytes(value, 0, size, buffer, pos + 2);
        }

        /// <summary>
        /// Converts a string value into a byte array, ensuring the resulting array does not exceed the specified maximum length.
        /// </summary>
        /// <param name="maxLen">The maximum length of the resulting byte array.</param>
        /// <param name="value">The string value to convert into a byte array.</param>
        /// <returns>A byte array representing the string value, truncated to the specified maximum length if necessary.</returns>
        public static byte[] SetString(int maxLen, string value)

        {
            var buffer = new byte[value.Length + 2];
            SetStringAt(buffer, 0, maxLen, value);
            return buffer;
        }

        /// <summary>
        /// Converts a string value into a byte array.
        /// </summary>
        /// <param name="value">The string value to convert into a byte array.</param>
        /// <returns>A byte array representing the string value.</returns>
        public static byte[] SetString(string value)

        {
            return SetString(254, value);
        }
        #endregion

        #region Get/Set WString (S7-1500 String)
        /// <summary>
        /// Retrieves a wide string (S7 WString) from a byte array.
        /// </summary>
        /// <param name="buffer">The byte array containing the data.</param>
        /// <param name="pos">The position in the array to start reading from.</param>
        /// <returns>The string value.</returns>
        public static string GetWStringAt(byte[] buffer, int pos)
        {
            //WString size = n characters + 2 Words (first for max length, second for real length)
            //Get the real length in Words
            int size = GetIntAt(buffer, pos + 2);
            //Extract string in UTF-16 unicode Big Endian.
            return System.Text.Encoding.BigEndianUnicode.GetString(buffer, pos + 4, size * 2);
        }

        /// <summary>
        /// Sets a wide string (S7 WString) in a byte array.
        /// </summary>
        /// <param name="buffer">The byte array to modify.</param>
        /// <param name="pos">The position in the array to set the value.</param>
        /// <param name="maxCharNb">The maximum number of characters.</param>
        /// <param name="value">The string value to set.</param>
        public static void SetWStringAt(byte[] buffer, int pos, int maxCharNb, string value)
        {
            //Get the length in words from number of characters
            int size = value.Length;
            //Set the max length in Words 
            SetIntAt(buffer, pos, (short)maxCharNb);
            //Set the real length in words
            SetIntAt(buffer, pos + 2, (short)size);
            //Set the UTF-16 unicode Big endian String (after max length and length)
            System.Text.Encoding.BigEndianUnicode.GetBytes(value, 0, size, buffer, pos + 4);
        }
        #endregion

        #region Get/Set Array of char (S7 ARRAY OF CHARS)
        /// <summary>
        /// Retrieves a string value (sequence of characters) from a specified position in the byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array from which the string value will be retrieved.</param>
        /// <param name="pos">The position in the byte array from which the string value should be read.</param>
        /// <param name="size">The number of bytes to read from the buffer, corresponding to the string's length.</param>
        /// <returns>A string representing the characters retrieved from the buffer.</returns>
        public static string GetCharsAt(byte[] buffer, int pos, int size)

        {
            return System.Text.Encoding.UTF8.GetString(buffer, pos, size);
        }

        /// <summary>
        /// Sets a string value (sequence of characters) at a specified position in the byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array where the string value will be set.</param>
        /// <param name="pos">The position in the byte array where the string value should be placed.</param>
        /// <param name="value">The string value (sequence of characters) to set in the buffer.</param>
        public static void SetCharsAt(byte[] buffer, int pos, string value)

        {
            int maxLen = buffer.Length - pos;
            // Truncs the string if there's no room enough        
            if (maxLen > value.Length) maxLen = value.Length;
            System.Text.Encoding.UTF8.GetBytes(value, 0, maxLen, buffer, pos);
        }

        #endregion

        #region Get/Set Array of WChar (S7-1500 ARRAY OF CHARS)

        /// <summary>
        /// Retrieves a string value (sequence of characters) from a specified position in the byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array from which the string value will be retrieved.</param>
        /// <param name="pos">The position in the byte array from which the string value should be read.</param>
        /// <param name="sizeInCharNb">The number of characters to read from the buffer.</param>
        /// <returns>A string representing the characters retrieved from the buffer.</returns>
        public static String GetWCharsAt(byte[] buffer, int pos, int sizeInCharNb)

        {
            //Extract Unicode UTF-16 Big-Endian character from the buffer. To use with WChar Datatype.
            //Size to read is in byte. Be careful, 1 char = 2 bytes
            return System.Text.Encoding.BigEndianUnicode.GetString(buffer, pos, sizeInCharNb * 2);
        }

        /// <summary>
        /// Sets a string value (as a sequence of characters) at a specified position in the byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array where the string value will be set.</param>
        /// <param name="pos">The position in the byte array where the string value should be placed.</param>
        /// <param name="value">The string value (sequence of characters) to set in the buffer.</param>
        public static void SetWCharsAt(byte[] buffer, int pos, string value)

        {
            //Compute max length in char number
            int maxLen = (buffer.Length - pos) / 2;
            // Truncs the string if there's no room enough        
            if (maxLen > value.Length) maxLen = value.Length;
            System.Text.Encoding.BigEndianUnicode.GetBytes(value, 0, maxLen, buffer, pos);
        }
        #endregion

        #region Get/Set Counter
        /// <summary>
        /// Converts a ushort counter value to an integer representation.
        /// </summary>
        /// <param name="value">The ushort counter value to convert.</param>
        /// <returns>An integer representing the equivalent value of the counter.</returns>
        public static int GetCounter(ushort value)

        {
            return _bcdToByte((byte)value) * 100 + _bcdToByte((byte)(value >> 8));
        }

        /// <summary>
        /// Retrieves a counter value from a specified position in the ushort array.
        /// </summary>
        /// <param name="buffer">The ushort array from which the counter value will be retrieved.</param>
        /// <param name="index">The position in the array from which the counter value should be read.</param>
        /// <returns>An integer representing the counter value retrieved from the array.</returns>
        public static int GetCounterAt(ushort[] buffer, int index)

        {
            return GetCounter(buffer[index]);
        }

        /// <summary>
        /// Converts an integer value to a counter value represented as a ushort.
        /// </summary>
        /// <param name="value">The integer value to convert to a counter.</param>
        /// <returns>A ushort representing the counter value equivalent to the input integer.</returns>
        public static ushort ToCounter(int value)

        {
            return (ushort)(_byteToBcd(value / 100) + (_byteToBcd(value % 100) << 8));
        }

        /// <summary>
        /// Sets a counter value at a specified position in the ushort array.
        /// </summary>
        /// <param name="buffer">The ushort array where the counter value will be set.</param>
        /// <param name="pos">The position in the array where the counter value should be placed.</param>
        /// <param name="value">The counter value to set in the array.</param>
        public static void SetCounterAt(ushort[] buffer, int pos, int value)

        {
            buffer[pos] = ToCounter(value);
        }
        #endregion

        #region Get/Set Timer
        /// <summary>
        /// Sets a S7 Timespan value at a specified position in the byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array where the S7 Timespan value will be set.</param>
        /// <param name="pos">The position in the byte array where the S7 Timespan value should be placed.</param>
        /// <param name="value">The TimeSpan value to set in the buffer.</param>
        public static void SetS7TimespanAt(byte[] buffer, int pos, TimeSpan value)

        {
            SetDIntAt(buffer, pos, (Int32)value.TotalMilliseconds);
        }

        /// <summary>
        /// Retrieves a <see cref="TimeSpan"/> value from the specified position in the byte array.
        /// The method reads 4 bytes starting from the given position, interprets them as a 32-bit integer 
        /// representing the number of milliseconds, and converts this value into a <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="buffer">The byte array containing the data.</param>
        /// <param name="pos">The position in the byte array where the time span data starts.</param>
        /// <returns>A <see cref="TimeSpan"/> corresponding to the value represented by the 4 bytes in the array.
        /// If the buffer length is insufficient, an empty <see cref="TimeSpan"/> is returned.</returns>
        public static TimeSpan GetS7TimespanAt(byte[] buffer, int pos)
        {
            if (buffer.Length < pos + 4)
            {
                return new TimeSpan();
            }

            Int32 a;
            a = buffer[pos + 0]; a <<= 8;
            a += buffer[pos + 1]; a <<= 8;
            a += buffer[pos + 2]; a <<= 8;
            a += buffer[pos + 3];
            TimeSpan sp = new TimeSpan(0, 0, 0, 0, a);

            return sp;
        }

        /// <summary>
        /// Retrieves a Local Time (LTime) value from a specified position in the byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array from which the LTime value will be retrieved.</param>
        /// <param name="pos">The position in the byte array from which the LTime value should be read.</param>
        /// <returns>A TimeSpan representing the Local Time (LTime) value retrieved from the buffer.</returns>
        public static TimeSpan GetLTimeAt(byte[] buffer, int pos)

        {
            //LTime size : 64 bits (8 octets)
            //Case if the buffer is too small
            if (buffer.Length < pos + 8) return new TimeSpan();

            try
            {
                // Extract and Convert number of nanoseconds to tick (1 tick = 100 nanoseconds)
                return TimeSpan.FromTicks(GetLIntAt(buffer, pos) / 100);
            }
            catch (Exception)
            {
                return new TimeSpan();
            }
        }

        /// <summary>
        /// Sets a Local Time (LTime) value at a specified position in the byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array where the LTime value will be set.</param>
        /// <param name="pos">The position in the byte array where the LTime value should be placed.</param>
        /// <param name="value">The TimeSpan representing the Local Time (LTime) value to set in the buffer.</param>
        public static void SetLTimeAt(byte[] buffer, int pos, TimeSpan value)

        {
            SetLIntAt(buffer, pos, (long)(value.Ticks * 100));
        }
        #endregion

        /// <summary>
        /// Generates a byte array of size 1024.
        /// </summary>
        /// <returns>A new byte array of size 1024.</returns>
        public byte[] GenerateByteArray()
        {
            byte[] result = new byte[1024];

            return result;
        }
        #endregion [Help Functions]
    }
}
