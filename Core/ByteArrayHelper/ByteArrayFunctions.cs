using System;
using System.Collections.Generic;
using System.Text;

namespace VisionNet.Core.ByteArrayHelper
{
    /// <summary>
    /// Provides helper methods for reading and writing Siemens S7 PLC primitive data types to raw byte buffers using big-endian encoding.
    /// </summary>
    public class ByteArrayFunctions
    {
        /// <summary>
        /// Identifies a single bit operand in an S7 buffer.
        /// </summary>
        public const int TypeBit = 0x01;

        /// <summary>
        /// Identifies an 8-bit unsigned integer operand in an S7 buffer.
        /// </summary>
        public const int TypeByte = 0x02;

        /// <summary>
        /// Identifies an 8-bit character operand in an S7 buffer.
        /// </summary>
        public const int TypeChar = 0x03;

        /// <summary>
        /// Identifies a 16-bit unsigned integer operand in an S7 buffer.
        /// </summary>
        public const int TypeWord = 0x04;

        /// <summary>
        /// Identifies a 16-bit signed integer operand in an S7 buffer.
        /// </summary>
        public const int TypeInt = 0x05;

        /// <summary>
        /// Identifies a 32-bit unsigned integer operand in an S7 buffer.
        /// </summary>
        public const int TypeDWord = 0x06;

        /// <summary>
        /// Identifies a 32-bit signed integer operand in an S7 buffer.
        /// </summary>
        public const int TypeDInt = 0x07;

        /// <summary>
        /// Identifies a 32-bit IEEE 754 floating-point operand in an S7 buffer.
        /// </summary>
        public const int TypeReal = 0x08;

        /// <summary>
        /// Identifies a packed BCD counter operand in an S7 buffer.
        /// </summary>
        public const int TypeCounter = 0x1C;
        //public const int TypeTimer = 0x1D;

		#region [Help Functions]

		private static Int64 bias = 621355968000000000; // "decimicros" between 0001-01-01 00:00:00 and 1970-01-01 00:00:00

		private static int BCDtoByte(byte B)
		{
			return ((B >> 4) * 10) + (B & 0x0F);
		}

		private static byte ByteToBCD(int Value)
		{
			return (byte)(((Value / 10) << 4) | (Value % 10));
		}

		private static byte[] CopyFrom(byte[] Buffer, int Pos, int Size)
		{
			byte[] Result = new byte[Size];
			Array.Copy(Buffer, Pos, Result, 0, Size);
			return Result;
		}

                /// <summary>
                /// Calculates the number of bytes occupied by an S7 data type identified by the supplied word-length code.
                /// </summary>
                /// <param name="WordLength">S7 word-length identifier, typically one of the <see cref="TypeBit"/> through <see cref="TypeCounter"/> constants.</param>
                /// <returns>The size in bytes required to hold a single value of the specified type, or 0 when the code is unknown.</returns>
                public static int DataSizeByte(int WordLength)
                {
                        switch (WordLength)
                        {
                                case TypeBit: return 1;  // S7 sends 1 byte per bit
				case TypeByte: return 1;
				case TypeChar: return 1;
				case TypeWord: return 2;
				case TypeDWord: return 4;
				case TypeInt: return 2;
				case TypeDInt: return 4;
				case TypeReal: return 4;
				case TypeCounter: return 2;
				//case TypeTimer: return 2;
				default: return 0;
			}
		}

		#region Get/Set the bit at Pos.Bit
                /// <summary>
                /// Reads a single bit from the specified byte buffer using a bit index relative to the containing byte.
                /// </summary>
                /// <param name="Buffer">Byte buffer that contains the bit; must not be <see langword="null"/> and must include the index referenced by <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based byte offset of the byte that contains the requested bit.</param>
                /// <param name="Bit">Zero-based bit index within the addressed byte. Values are clamped to the inclusive range 0 to 7.</param>
                /// <returns><see langword="true"/> when the addressed bit is set; otherwise, <see langword="false"/>.</returns>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when <paramref name="Pos"/> exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static bool GetBitAt(byte[] Buffer, int Pos, int Bit)
                {
                        byte[] Mask = { 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80 };
                        if (Bit < 0) Bit = 0;
                        if (Bit > 7) Bit = 7;
                        return (Buffer[Pos] & Mask[Bit]) != 0;
                }
                /// <summary>
                /// Sets or clears a single bit within the specified byte buffer using a bit index relative to the containing byte.
                /// </summary>
                /// <param name="Buffer">Reference to the byte buffer to modify; must not be <see langword="null"/> and must include the index referenced by <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based byte offset of the byte that contains the bit to mutate.</param>
                /// <param name="Bit">Zero-based bit index within the addressed byte. Values outside 0 to 7 are clamped to that range.</param>
                /// <param name="Value"><see langword="true"/> to set the bit, or <see langword="false"/> to clear it.</param>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when <paramref name="Pos"/> exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static void SetBitAt(ref byte[] Buffer, int Pos, int Bit, bool Value)
                {
                        byte[] Mask = { 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80 };
                        if (Bit < 0) Bit = 0;
			if (Bit > 7) Bit = 7;

			if (Value)
				Buffer[Pos] = (byte)(Buffer[Pos] | Mask[Bit]);
			else
				Buffer[Pos] = (byte)(Buffer[Pos] & ~Mask[Bit]);
		}
		#endregion

		#region Get/Set 8 bit signed value (S7 SInt) -128..127
                /// <summary>
                /// Reads a signed 8-bit integer (S7 SInt) from the specified buffer position.
                /// </summary>
                /// <param name="Buffer">Byte buffer that stores the value; must not be <see langword="null"/> and must expose at least one byte starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the byte to convert.</param>
                /// <returns>A signed value in the range -128 to 127 representing the requested SInt.</returns>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when <paramref name="Pos"/> is outside the bounds of <paramref name="Buffer"/>.</exception>
                public static int GetSIntAt(byte[] Buffer, int Pos)
                {
                        int Value = Buffer[Pos];
                        if (Value < 128)
                                return Value;
                        else
                                return (int)(Value - 256);
                }
                /// <summary>
                /// Writes a signed 8-bit integer (S7 SInt) into the specified buffer position.
                /// </summary>
                /// <param name="Buffer">Byte buffer that will receive the value; must not be <see langword="null"/> and must expose at least one byte starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the target byte.</param>
                /// <param name="Value">Signed value to store. Values outside -128 to 127 are clamped to that range.</param>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when <paramref name="Pos"/> is outside the bounds of <paramref name="Buffer"/>.</exception>
                public static void SetSIntAt(byte[] Buffer, int Pos, int Value)
                {
                        if (Value < -128) Value = -128;
                        if (Value > 127) Value = 127;
			Buffer[Pos] = (byte)Value;
		}
		#endregion

		#region Get/Set 16 bit signed value (S7 int) -32768..32767
                /// <summary>
                /// Reads a 16-bit signed integer (S7 Int) from the buffer using big-endian byte order.
                /// </summary>
                /// <param name="Buffer">Byte buffer that stores the value; must not be <see langword="null"/> and must provide at least two bytes starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <returns>A signed 16-bit value decoded from the buffer.</returns>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the requested range exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static short GetIntAt(byte[] Buffer, int Pos)
                {
                        return (short)((Buffer[Pos] << 8) | Buffer[Pos + 1]);
                }
                /// <summary>
                /// Writes a 16-bit signed integer (S7 Int) to the buffer using big-endian byte order.
                /// </summary>
                /// <param name="Buffer">Byte buffer that will receive the value; must not be <see langword="null"/> and must provide at least two bytes starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <param name="Value">Signed 16-bit value to encode.</param>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the requested range exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static void SetIntAt(byte[] Buffer, int Pos, Int16 Value)
                {
                        Buffer[Pos] = (byte)(Value >> 8);
                        Buffer[Pos + 1] = (byte)(Value & 0x00FF);
		}
		#endregion

		#region Get/Set 32 bit signed value (S7 DInt) -2147483648..2147483647
                /// <summary>
                /// Reads a 32-bit signed integer (S7 DInt) from the buffer using big-endian byte order.
                /// </summary>
                /// <param name="Buffer">Byte buffer that stores the value; must not be <see langword="null"/> and must provide at least four bytes starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <returns>A signed 32-bit value decoded from the buffer.</returns>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the requested range exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static int GetDIntAt(byte[] Buffer, int Pos)
                {
                        int Result;
                        Result = Buffer[Pos]; Result <<= 8;
                        Result += Buffer[Pos + 1]; Result <<= 8;
                        Result += Buffer[Pos + 2]; Result <<= 8;
                        Result += Buffer[Pos + 3];
                        return Result;
                }
                /// <summary>
                /// Writes a 32-bit signed integer (S7 DInt) to the buffer using big-endian byte order.
                /// </summary>
                /// <param name="Buffer">Byte buffer that will receive the value; must not be <see langword="null"/> and must provide at least four bytes starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <param name="Value">Signed 32-bit value to encode.</param>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the requested range exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static void SetDIntAt(byte[] Buffer, int Pos, int Value)
                {
                        Buffer[Pos + 3] = (byte)(Value & 0xFF);
                        Buffer[Pos + 2] = (byte)((Value >> 8) & 0xFF);
			Buffer[Pos + 1] = (byte)((Value >> 16) & 0xFF);
			Buffer[Pos] = (byte)((Value >> 24) & 0xFF);
		}
		#endregion

		#region Get/Set 64 bit signed value (S7 LInt) -9223372036854775808..9223372036854775807
                /// <summary>
                /// Reads a 64-bit signed integer (S7 LInt) from the buffer using big-endian byte order.
                /// </summary>
                /// <param name="Buffer">Byte buffer that stores the value; must not be <see langword="null"/> and must provide at least eight bytes starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <returns>A signed 64-bit value decoded from the buffer.</returns>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the requested range exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static Int64 GetLIntAt(byte[] Buffer, int Pos)
                {
                        Int64 Result;
                        Result = Buffer[Pos]; Result <<= 8;
			Result += Buffer[Pos + 1]; Result <<= 8;
			Result += Buffer[Pos + 2]; Result <<= 8;
			Result += Buffer[Pos + 3]; Result <<= 8;
			Result += Buffer[Pos + 4]; Result <<= 8;
			Result += Buffer[Pos + 5]; Result <<= 8;
			Result += Buffer[Pos + 6]; Result <<= 8;
                        Result += Buffer[Pos + 7];
                        return Result;
                }
                /// <summary>
                /// Writes a 64-bit signed integer (S7 LInt) to the buffer using big-endian byte order.
                /// </summary>
                /// <param name="Buffer">Byte buffer that will receive the value; must not be <see langword="null"/> and must provide at least eight bytes starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <param name="Value">Signed 64-bit value to encode.</param>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the requested range exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static void SetLIntAt(byte[] Buffer, int Pos, Int64 Value)
                {
                        Buffer[Pos + 7] = (byte)(Value & 0xFF);
                        Buffer[Pos + 6] = (byte)((Value >> 8) & 0xFF);
			Buffer[Pos + 5] = (byte)((Value >> 16) & 0xFF);
			Buffer[Pos + 4] = (byte)((Value >> 24) & 0xFF);
			Buffer[Pos + 3] = (byte)((Value >> 32) & 0xFF);
			Buffer[Pos + 2] = (byte)((Value >> 40) & 0xFF);
			Buffer[Pos + 1] = (byte)((Value >> 48) & 0xFF);
			Buffer[Pos] = (byte)((Value >> 56) & 0xFF);
		}
		#endregion

		#region Get/Set 8 bit unsigned value (S7 USInt) 0..255
                /// <summary>
                /// Reads an unsigned 8-bit integer (S7 USInt) from the specified buffer position.
                /// </summary>
                /// <param name="Buffer">Byte buffer that stores the value; must not be <see langword="null"/> and must expose at least one byte starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the byte to read.</param>
                /// <returns>Unsigned 8-bit value stored at the requested position.</returns>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when <paramref name="Pos"/> is outside the bounds of <paramref name="Buffer"/>.</exception>
                public static byte GetUSIntAt(byte[] Buffer, int Pos)
                {
                        return Buffer[Pos];
                }
                /// <summary>
                /// Writes an unsigned 8-bit integer (S7 USInt) to the specified buffer position.
                /// </summary>
                /// <param name="Buffer">Byte buffer that will receive the value; must not be <see langword="null"/> and must expose at least one byte starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the destination byte.</param>
                /// <param name="Value">Unsigned 8-bit value to write.</param>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when <paramref name="Pos"/> is outside the bounds of <paramref name="Buffer"/>.</exception>
                public static void SetUSIntAt(byte[] Buffer, int Pos, byte Value)
                {
                        Buffer[Pos] = Value;
                }
		#endregion

		#region Get/Set 16 bit unsigned value (S7 UInt) 0..65535
                /// <summary>
                /// Reads a 16-bit unsigned integer (S7 UInt) from the buffer using big-endian byte order.
                /// </summary>
                /// <param name="Buffer">Byte buffer that stores the value; must not be <see langword="null"/> and must provide at least two bytes starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <returns>An unsigned 16-bit value decoded from the buffer.</returns>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the requested range exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static UInt16 GetUIntAt(byte[] Buffer, int Pos)
                {
                        return (UInt16)((Buffer[Pos] << 8) | Buffer[Pos + 1]);
                }
                /// <summary>
                /// Writes a 16-bit unsigned integer (S7 UInt) to the buffer using big-endian byte order.
                /// </summary>
                /// <param name="Buffer">Byte buffer that will receive the value; must not be <see langword="null"/> and must provide at least two bytes starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <param name="Value">Unsigned 16-bit value to encode.</param>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the requested range exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static void SetUIntAt(byte[] Buffer, int Pos, UInt16 Value)
                {
                        Buffer[Pos] = (byte)(Value >> 8);
                        Buffer[Pos + 1] = (byte)(Value & 0x00FF);
		}
		#endregion

		#region Get/Set 32 bit unsigned value (S7 UDInt) 0..4294967296
                /// <summary>
                /// Reads a 32-bit unsigned integer (S7 UDInt) from the buffer using big-endian byte order.
                /// </summary>
                /// <param name="Buffer">Byte buffer that stores the value; must not be <see langword="null"/> and must provide at least four bytes starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <returns>An unsigned 32-bit value decoded from the buffer.</returns>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the requested range exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static UInt32 GetUDIntAt(byte[] Buffer, int Pos)
                {
                        UInt32 Result;
                        Result = Buffer[Pos]; Result <<= 8;
                        Result |= Buffer[Pos + 1]; Result <<= 8;
                        Result |= Buffer[Pos + 2]; Result <<= 8;
                        Result |= Buffer[Pos + 3];
                        return Result;
                }
                /// <summary>
                /// Writes a 32-bit unsigned integer (S7 UDInt) to the buffer using big-endian byte order.
                /// </summary>
                /// <param name="Buffer">Byte buffer that will receive the value; must not be <see langword="null"/> and must provide at least four bytes starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <param name="Value">Unsigned 32-bit value to encode.</param>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the requested range exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static void SetUDIntAt(byte[] Buffer, int Pos, UInt32 Value)
                {
                        Buffer[Pos + 3] = (byte)(Value & 0xFF);
                        Buffer[Pos + 2] = (byte)((Value >> 8) & 0xFF);
			Buffer[Pos + 1] = (byte)((Value >> 16) & 0xFF);
			Buffer[Pos] = (byte)((Value >> 24) & 0xFF);
		}
		#endregion

		#region Get/Set 64 bit unsigned value (S7 ULint) 0..18446744073709551616
                /// <summary>
                /// Reads a 64-bit unsigned integer (S7 ULInt) from the buffer using big-endian byte order.
                /// </summary>
                /// <param name="Buffer">Byte buffer that stores the value; must not be <see langword="null"/> and must provide at least eight bytes starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <returns>An unsigned 64-bit value decoded from the buffer.</returns>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the requested range exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static UInt64 GetULIntAt(byte[] Buffer, int Pos)
                {
                        UInt64 Result;
                        Result = Buffer[Pos]; Result <<= 8;
			Result |= Buffer[Pos + 1]; Result <<= 8;
			Result |= Buffer[Pos + 2]; Result <<= 8;
			Result |= Buffer[Pos + 3]; Result <<= 8;
			Result |= Buffer[Pos + 4]; Result <<= 8;
			Result |= Buffer[Pos + 5]; Result <<= 8;
			Result |= Buffer[Pos + 6]; Result <<= 8;
                        Result |= Buffer[Pos + 7];
                        return Result;
                }
                /// <summary>
                /// Writes a 64-bit unsigned integer (S7 ULInt) to the buffer using big-endian byte order.
                /// </summary>
                /// <param name="Buffer">Byte buffer that will receive the value; must not be <see langword="null"/> and must provide at least eight bytes starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <param name="Value">Unsigned 64-bit value to encode.</param>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the requested range exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static void SetULintAt(byte[] Buffer, int Pos, UInt64 Value)
                {
                        Buffer[Pos + 7] = (byte)(Value & 0xFF);
                        Buffer[Pos + 6] = (byte)((Value >> 8) & 0xFF);
			Buffer[Pos + 5] = (byte)((Value >> 16) & 0xFF);
			Buffer[Pos + 4] = (byte)((Value >> 24) & 0xFF);
			Buffer[Pos + 3] = (byte)((Value >> 32) & 0xFF);
			Buffer[Pos + 2] = (byte)((Value >> 40) & 0xFF);
			Buffer[Pos + 1] = (byte)((Value >> 48) & 0xFF);
			Buffer[Pos] = (byte)((Value >> 56) & 0xFF);
		}
		#endregion

		#region Get/Set 8 bit word (S7 Byte) 16#00..16#FF
                /// <summary>
                /// Reads an 8-bit byte (S7 Byte) from the specified buffer position without conversion.
                /// </summary>
                /// <param name="Buffer">Byte buffer that stores the value; must not be <see langword="null"/> and must expose at least one byte starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the byte to read.</param>
                /// <returns>The raw byte value at the requested position.</returns>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when <paramref name="Pos"/> is outside the bounds of <paramref name="Buffer"/>.</exception>
                public static byte GetByteAt(byte[] Buffer, int Pos)
                {
                        return Buffer[Pos];
                }
                /// <summary>
                /// Writes an 8-bit byte (S7 Byte) to the specified buffer position without conversion.
                /// </summary>
                /// <param name="Buffer">Byte buffer that will receive the value; must not be <see langword="null"/> and must expose at least one byte starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the destination byte.</param>
                /// <param name="Value">Raw byte value to write.</param>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when <paramref name="Pos"/> is outside the bounds of <paramref name="Buffer"/>.</exception>
                public static void SetByteAt(byte[] Buffer, int Pos, byte Value)
                {
                        Buffer[Pos] = Value;
                }
		#endregion

		#region Get/Set 16 bit word (S7 Word) 16#0000..16#FFFF
                /// <summary>
                /// Reads a 16-bit unsigned word (S7 Word) from the buffer using big-endian byte order.
                /// </summary>
                /// <param name="Buffer">Byte buffer that stores the word; must not be <see langword="null"/> and must provide at least two bytes starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <returns>The unsigned 16-bit word value.</returns>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the requested range exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static UInt16 GetWordAt(byte[] Buffer, int Pos)
                {
                        return GetUIntAt(Buffer, Pos);
                }
                /// <summary>
                /// Writes a 16-bit unsigned word (S7 Word) to the buffer using big-endian byte order.
                /// </summary>
                /// <param name="Buffer">Byte buffer that will receive the word; must not be <see langword="null"/> and must provide at least two bytes starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <param name="Value">Unsigned 16-bit word value to encode.</param>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the requested range exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static void SetWordAt(byte[] Buffer, int Pos, UInt16 Value)
                {
                        SetUIntAt(Buffer, Pos, Value);
                }
		#endregion

		#region Get/Set 32 bit word (S7 DWord) 16#00000000..16#FFFFFFFF
                /// <summary>
                /// Reads a 32-bit unsigned double word (S7 DWord) from the buffer using big-endian byte order.
                /// </summary>
                /// <param name="Buffer">Byte buffer that stores the word; must not be <see langword="null"/> and must provide at least four bytes starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <returns>The unsigned 32-bit double word value.</returns>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the requested range exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static UInt32 GetDWordAt(byte[] Buffer, int Pos)
                {
                        return GetUDIntAt(Buffer, Pos);
                }
                /// <summary>
                /// Writes a 32-bit unsigned double word (S7 DWord) to the buffer using big-endian byte order.
                /// </summary>
                /// <param name="Buffer">Byte buffer that will receive the word; must not be <see langword="null"/> and must provide at least four bytes starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <param name="Value">Unsigned 32-bit double word value to encode.</param>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the requested range exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static void SetDWordAt(byte[] Buffer, int Pos, UInt32 Value)
                {
                        SetUDIntAt(Buffer, Pos, Value);
                }
		#endregion

		#region Get/Set 64 bit word (S7 LWord) 16#0000000000000000..16#FFFFFFFFFFFFFFFF
                /// <summary>
                /// Reads a 64-bit unsigned long word (S7 LWord) from the buffer using big-endian byte order.
                /// </summary>
                /// <param name="Buffer">Byte buffer that stores the word; must not be <see langword="null"/> and must provide at least eight bytes starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <returns>The unsigned 64-bit long word value.</returns>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the requested range exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static UInt64 GetLWordAt(byte[] Buffer, int Pos)
                {
                        return GetULIntAt(Buffer, Pos);
                }
                /// <summary>
                /// Writes a 64-bit unsigned long word (S7 LWord) to the buffer using big-endian byte order.
                /// </summary>
                /// <param name="Buffer">Byte buffer that will receive the word; must not be <see langword="null"/> and must provide at least eight bytes starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <param name="Value">Unsigned 64-bit long word value to encode.</param>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the requested range exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static void SetLWordAt(byte[] Buffer, int Pos, UInt64 Value)
                {
                        SetULintAt(Buffer, Pos, Value);
                }
		#endregion

		#region Get/Set 32 bit floating point number (S7 Real) (Range of Single)
                /// <summary>
                /// Reads a 32-bit IEEE 754 floating-point number (S7 Real) from the buffer using big-endian byte order.
                /// </summary>
                /// <param name="Buffer">Byte buffer that stores the value; must not be <see langword="null"/> and must provide at least four bytes starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <returns>A <see cref="Single"/> representing the decoded floating-point value.</returns>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the requested range exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static Single GetRealAt(byte[] Buffer, int Pos)
                {
                        UInt32 Value = GetUDIntAt(Buffer, Pos);
                        byte[] bytes = BitConverter.GetBytes(Value);
                        return BitConverter.ToSingle(bytes, 0);
                }
                /// <summary>
                /// Writes a 32-bit IEEE 754 floating-point number (S7 Real) to the buffer using big-endian byte order.
                /// </summary>
                /// <param name="Buffer">Byte buffer that will receive the value; must not be <see langword="null"/> and must provide at least four bytes starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <param name="Value">Floating-point value to encode.</param>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the requested range exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static void SetRealAt(byte[] Buffer, int Pos, Single Value)
                {
                        byte[] FloatArray = BitConverter.GetBytes(Value);
                        Buffer[Pos] = FloatArray[3];
			Buffer[Pos + 1] = FloatArray[2];
			Buffer[Pos + 2] = FloatArray[1];
			Buffer[Pos + 3] = FloatArray[0];
		}
		#endregion

		#region Get/Set 64 bit floating point number (S7 LReal) (Range of Double)
                /// <summary>
                /// Reads a 64-bit IEEE 754 floating-point number (S7 LReal) from the buffer using big-endian byte order.
                /// </summary>
                /// <param name="Buffer">Byte buffer that stores the value; must not be <see langword="null"/> and must provide at least eight bytes starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <returns>A <see cref="Double"/> representing the decoded floating-point value.</returns>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the requested range exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static Double GetLRealAt(byte[] Buffer, int Pos)
                {
                        UInt64 Value = GetULIntAt(Buffer, Pos);
                        byte[] bytes = BitConverter.GetBytes(Value);
                        return BitConverter.ToDouble(bytes, 0);
                }
                /// <summary>
                /// Writes a 64-bit IEEE 754 floating-point number (S7 LReal) to the buffer using big-endian byte order.
                /// </summary>
                /// <param name="Buffer">Byte buffer that will receive the value; must not be <see langword="null"/> and must provide at least eight bytes starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <param name="Value">Floating-point value to encode.</param>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the requested range exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static void SetLRealAt(byte[] Buffer, int Pos, Double Value)
                {
                        byte[] FloatArray = BitConverter.GetBytes(Value);
                        Buffer[Pos] = FloatArray[7];
			Buffer[Pos + 1] = FloatArray[6];
			Buffer[Pos + 2] = FloatArray[5];
			Buffer[Pos + 3] = FloatArray[4];
			Buffer[Pos + 4] = FloatArray[3];
			Buffer[Pos + 5] = FloatArray[2];
			Buffer[Pos + 6] = FloatArray[1];
			Buffer[Pos + 7] = FloatArray[0];
		}
		#endregion

		#region Get/Set DateTime (S7 DATE_AND_TIME)
                /// <summary>
                /// Decodes an S7 <c>DATE_AND_TIME</c> value stored in packed BCD format from the specified buffer.
                /// </summary>
                /// <param name="Buffer">Byte buffer containing eight consecutive bytes that represent the value; must not be <see langword="null"/>.</param>
                /// <param name="Pos">Zero-based offset of the first BCD-encoded byte.</param>
                /// <returns>A <see cref="DateTime"/> combining the decoded calendar and time components, or <c>new DateTime(0)</c> when the encoded value is invalid.</returns>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when fewer than eight bytes are available starting at <paramref name="Pos"/>.</exception>
                public static DateTime GetDateTimeAt(byte[] Buffer, int Pos)
                {
                        int Year, Month, Day, Hour, Min, Sec, MSec;

                        Year = BCDtoByte(Buffer[Pos]);
			if (Year < 90)
				Year += 2000;
			else
				Year += 1900;

			Month = BCDtoByte(Buffer[Pos + 1]);
			Day = BCDtoByte(Buffer[Pos + 2]);
			Hour = BCDtoByte(Buffer[Pos + 3]);
			Min = BCDtoByte(Buffer[Pos + 4]);
			Sec = BCDtoByte(Buffer[Pos + 5]);
			MSec = (BCDtoByte(Buffer[Pos + 6]) * 10) + (BCDtoByte(Buffer[Pos + 7]) / 10);
			try
			{
				return new DateTime(Year, Month, Day, Hour, Min, Sec, MSec);
			}
			catch (System.ArgumentOutOfRangeException)
			{
				return new DateTime(0);
			}
		}
                /// <summary>
                /// Encodes a <see cref="DateTime"/> as an S7 <c>DATE_AND_TIME</c> value in packed BCD format.
                /// </summary>
                /// <param name="Buffer">Byte buffer that will receive eight bytes of packed BCD data; must not be <see langword="null"/>.</param>
                /// <param name="Pos">Zero-based offset at which to write the first BCD-encoded byte.</param>
                /// <param name="Value">Timestamp to encode. Years greater than or equal to 2000 are stored as offset from 2000; years below 2000 are stored as offset from 1900.</param>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when fewer than eight bytes are available starting at <paramref name="Pos"/>.</exception>
                public static void SetDateTimeAt(byte[] Buffer, int Pos, DateTime Value)
                {
                        int Year = Value.Year;
                        int Month = Value.Month;
			int Day = Value.Day;
			int Hour = Value.Hour;
			int Min = Value.Minute;
			int Sec = Value.Second;
			int Dow = (int)Value.DayOfWeek + 1;
			// MSecH = First two digits of miliseconds 
			int MsecH = Value.Millisecond / 10;
			// MSecL = Last digit of miliseconds
			int MsecL = Value.Millisecond % 10;
			if (Year > 1999)
				Year -= 2000;

			Buffer[Pos] = ByteToBCD(Year);
			Buffer[Pos + 1] = ByteToBCD(Month);
			Buffer[Pos + 2] = ByteToBCD(Day);
			Buffer[Pos + 3] = ByteToBCD(Hour);
			Buffer[Pos + 4] = ByteToBCD(Min);
			Buffer[Pos + 5] = ByteToBCD(Sec);
			Buffer[Pos + 6] = ByteToBCD(MsecH);
			Buffer[Pos + 7] = ByteToBCD(MsecL * 10 + Dow);
		}
		#endregion

		#region Get/Set DATE (S7 DATE) 
                /// <summary>
                /// Reads an S7 <c>DATE</c> value representing the number of days since 1 January 1990.
                /// </summary>
                /// <param name="Buffer">Byte buffer containing at least two bytes at <paramref name="Pos"/>; must not be <see langword="null"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <returns>A <see cref="DateTime"/> calculated by adding the encoded day offset to 1 January 1990, or <c>new DateTime(0)</c> when the resulting date is invalid.</returns>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when fewer than two bytes are available starting at <paramref name="Pos"/>.</exception>
                public static DateTime GetDateAt(byte[] Buffer, int Pos)
                {
                        try
                        {
                                return new DateTime(1990, 1, 1).AddDays(GetIntAt(Buffer, Pos));
			}
			catch (System.ArgumentOutOfRangeException)
			{
				return new DateTime(0);
			}
		}
                /// <summary>
                /// Writes an S7 <c>DATE</c> value that stores the number of days since 1 January 1990.
                /// </summary>
                /// <param name="Buffer">Byte buffer that will receive the encoded date; must not be <see langword="null"/> and must provide at least two bytes starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <param name="Value">Date to encode. The number of days between this value and 1 January 1990 is stored.</param>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when fewer than two bytes are available starting at <paramref name="Pos"/>.</exception>
                public static void SetDateAt(byte[] Buffer, int Pos, DateTime Value)
                {
                        SetIntAt(Buffer, Pos, (Int16)(Value - new DateTime(1990, 1, 1)).Days);
                }

		#endregion

		#region Get/Set TOD (S7 TIME_OF_DAY)
                /// <summary>
                /// Reads an S7 <c>TIME_OF_DAY</c> value representing milliseconds elapsed since midnight.
                /// </summary>
                /// <param name="Buffer">Byte buffer containing at least four bytes at <paramref name="Pos"/>; must not be <see langword="null"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <returns>A <see cref="DateTime"/> based on <c>new DateTime(0)</c> advanced by the encoded milliseconds, or <c>new DateTime(0)</c> when the value is invalid.</returns>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when fewer than four bytes are available starting at <paramref name="Pos"/>.</exception>
                public static DateTime GetTODAt(byte[] Buffer, int Pos)
                {
                        try
                        {
                                return new DateTime(0).AddMilliseconds(GetDIntAt(Buffer, Pos));
			}
			catch (System.ArgumentOutOfRangeException)
			{
				return new DateTime(0);
			}
		}
                /// <summary>
                /// Writes an S7 <c>TIME_OF_DAY</c> value that stores milliseconds elapsed since midnight.
                /// </summary>
                /// <param name="Buffer">Byte buffer that will receive the encoded time; must not be <see langword="null"/> and must provide at least four bytes starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <param name="Value">Time whose time-of-day component will be encoded to milliseconds.</param>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when fewer than four bytes are available starting at <paramref name="Pos"/>.</exception>
                public static void SetTODAt(byte[] Buffer, int Pos, DateTime Value)
                {
                        TimeSpan Time = Value.TimeOfDay;
                        SetDIntAt(Buffer, Pos, (Int32)System.Math.Round(Time.TotalMilliseconds));
		}
		#endregion

		#region Get/Set LTOD (S7 1500 LONG TIME_OF_DAY)
                /// <summary>
                /// Reads an S7-1500 <c>LTIME_OF_DAY</c> value representing nanoseconds elapsed since midnight.
                /// </summary>
                /// <param name="Buffer">Byte buffer containing at least eight bytes at <paramref name="Pos"/>; must not be <see langword="null"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <returns>A <see cref="DateTime"/> derived from the decoded nanosecond count divided by 100 to convert to .NET ticks, or <c>new DateTime(0)</c> when the value is invalid.</returns>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when fewer than eight bytes are available starting at <paramref name="Pos"/>.</exception>
                public static DateTime GetLTODAt(byte[] Buffer, int Pos)
                {
                        // .NET Tick = 100 ns, S71500 Tick = 1 ns
                        try
                        {
                                return new DateTime(System.Math.Abs(GetLIntAt(Buffer, Pos) / 100));
			}
			catch (System.ArgumentOutOfRangeException)
			{
				return new DateTime(0);
			}
		}
                /// <summary>
                /// Writes an S7-1500 <c>LTIME_OF_DAY</c> value expressed as nanoseconds since midnight.
                /// </summary>
                /// <param name="Buffer">Byte buffer that will receive the encoded value; must not be <see langword="null"/> and must provide at least eight bytes starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <param name="Value">Time whose time-of-day component will be converted to nanoseconds.</param>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when fewer than eight bytes are available starting at <paramref name="Pos"/>.</exception>
                public static void SetLTODAt(byte[] Buffer, int Pos, DateTime Value)
                {
                        TimeSpan Time = Value.TimeOfDay;
                        SetLIntAt(Buffer, Pos, (Int64)Time.Ticks * 100);
		}
		#endregion

		#region GET/SET LDT (S7 1500 Long Date and Time)
                /// <summary>
                /// Reads an S7-1500 long date and time (<c>LDT</c>) value encoded as nanoseconds offset from 1 January 1970 UTC.
                /// </summary>
                /// <param name="Buffer">Byte buffer containing at least eight bytes at <paramref name="Pos"/>; must not be <see langword="null"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <returns>A <see cref="DateTime"/> constructed by converting the encoded nanoseconds to .NET ticks, or <c>new DateTime(0)</c> when decoding fails.</returns>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when fewer than eight bytes are available starting at <paramref name="Pos"/>.</exception>
                public static DateTime GetLDTAt(byte[] Buffer, int Pos)
                {
                        try
                        {
                                return new DateTime((GetLIntAt(Buffer, Pos) / 100) + bias);
			}
			catch (System.ArgumentOutOfRangeException)
			{
				return new DateTime(0);
			}
		}
                /// <summary>
                /// Writes an S7-1500 long date and time (<c>LDT</c>) value encoded as nanoseconds offset from 1 January 1970 UTC.
                /// </summary>
                /// <param name="Buffer">Byte buffer that will receive the encoded value; must not be <see langword="null"/> and must provide at least eight bytes starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <param name="Value">Timestamp to encode. Its tick count is converted relative to the internal bias.</param>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when fewer than eight bytes are available starting at <paramref name="Pos"/>.</exception>
                public static void SetLDTAt(byte[] Buffer, int Pos, DateTime Value)
                {
                        SetLIntAt(Buffer, Pos, (Value.Ticks - bias) * 100);
                }
		#endregion

		#region Get/Set DTL (S71200/1500 Date and Time)
		// Thanks to Johan Cardoen for GetDTLAt
                /// <summary>
                /// Reads an S7-1200/1500 <c>DTL</c> (date and time) structure from the specified buffer.
                /// </summary>
                /// <param name="Buffer">Byte buffer containing the 12-byte <c>DTL</c> representation; must not be <see langword="null"/>.</param>
                /// <param name="Pos">Zero-based offset of the first structure byte.</param>
                /// <returns>A <see cref="DateTime"/> built from the decoded year, month, day, time-of-day, and millisecond components, or <c>new DateTime(0)</c> when decoding fails.</returns>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when fewer than twelve bytes are available starting at <paramref name="Pos"/>.</exception>
                public static DateTime GetDTLAt(byte[] Buffer, int Pos)
                {
                        int Year, Month, Day, Hour, Min, Sec, MSec;

			Year = Buffer[Pos] * 256 + Buffer[Pos + 1];
			Month = Buffer[Pos + 2];
			Day = Buffer[Pos + 3];
			Hour = Buffer[Pos + 5];
			Min = Buffer[Pos + 6];
			Sec = Buffer[Pos + 7];
			MSec = (int)GetUDIntAt(Buffer, Pos + 8) / 1000000;

			try
			{
				return new DateTime(Year, Month, Day, Hour, Min, Sec, MSec);
			}
			catch (System.ArgumentOutOfRangeException)
			{
				return new DateTime(0);
			}
		}
                /// <summary>
                /// Writes an S7-1200/1500 <c>DTL</c> (date and time) structure to the specified buffer.
                /// </summary>
                /// <param name="Buffer">Byte buffer that will receive the 12-byte <c>DTL</c> structure; must not be <see langword="null"/>.</param>
                /// <param name="Pos">Zero-based offset of the first structure byte.</param>
                /// <param name="Value">Timestamp to encode. The millisecond component is converted to nanoseconds for storage.</param>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when fewer than twelve bytes are available starting at <paramref name="Pos"/>.</exception>
                public static void SetDTLAt(byte[] Buffer, int Pos, DateTime Value)
                {
                        short Year = (short)Value.Year;
                        byte Month = (byte)Value.Month;
			byte Day = (byte)Value.Day;
			byte Hour = (byte)Value.Hour;
			byte Min = (byte)Value.Minute;
			byte Sec = (byte)Value.Second;
			byte Dow = (byte)(Value.DayOfWeek + 1);

			Int32 NanoSecs = Value.Millisecond * 1000000;

			var bytes_short = BitConverter.GetBytes(Year);

			Buffer[Pos] = bytes_short[1];
			Buffer[Pos + 1] = bytes_short[0];
			Buffer[Pos + 2] = Month;
			Buffer[Pos + 3] = Day;
			Buffer[Pos + 4] = Dow;
			Buffer[Pos + 5] = Hour;
			Buffer[Pos + 6] = Min;
			Buffer[Pos + 7] = Sec;
			SetDIntAt(Buffer, Pos + 8, NanoSecs);
		}

		#endregion

		#region Get/Set String (S7 String)
		// Thanks to Pablo Agirre 
                /// <summary>
                /// Reads an S7 <c>STRING</c> value whose first byte stores the maximum length and second byte stores the actual length.
                /// </summary>
                /// <param name="Buffer">Byte buffer containing the string control bytes and payload; must not be <see langword="null"/>.</param>
                /// <param name="Pos">Zero-based offset of the maximum-length control byte.</param>
                /// <returns>The decoded UTF-8 string with the number of characters indicated by the length byte.</returns>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the buffer does not contain the declared number of bytes.</exception>
                public static string GetStringAt(byte[] Buffer, int Pos)
                {
                        int size = (int)Buffer[Pos + 1];
                        return System.Text.Encoding.UTF8.GetString(Buffer, Pos + 2, size);
                }

                /// <summary>
                /// Writes an S7 <c>STRING</c> value including the maximum-length and actual-length control bytes.
                /// </summary>
                /// <param name="Buffer">Byte buffer that will receive the control bytes and payload; must not be <see langword="null"/> and must provide at least <paramref name="MaxLen"/> + 2 bytes starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the maximum-length control byte.</param>
                /// <param name="MaxLen">Maximum number of characters allowed in the string, stored in the first control byte.</param>
                /// <param name="Value">UTF-8 string to encode. Callers must ensure the encoded length does not exceed <paramref name="MaxLen"/> or the available buffer space because no truncation is performed.</param>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the buffer does not contain enough space for the control bytes and content.</exception>
                public static void SetStringAt(byte[] Buffer, int Pos, int MaxLen, string Value)
                {
                        int size = Value.Length;
                        Buffer[Pos] = (byte)MaxLen;
			Buffer[Pos + 1] = (byte)size;
			System.Text.Encoding.UTF8.GetBytes(Value, 0, size, Buffer, Pos + 2);
		}

		#endregion

		#region Get/Set WString (S7-1500 String)
                /// <summary>
                /// Reads an S7-1500 <c>WSTRING</c> value that stores the maximum and actual length as two 16-bit words followed by UTF-16 big-endian characters.
                /// </summary>
                /// <param name="Buffer">Byte buffer containing the control words and payload; must not be <see langword="null"/>.</param>
                /// <param name="Pos">Zero-based offset of the maximum-length word.</param>
                /// <returns>The decoded UTF-16 string containing the number of characters indicated by the length word.</returns>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the buffer does not contain the declared number of bytes.</exception>
                public static string GetWStringAt(byte[] Buffer, int Pos)
                {
                        //WString size = n characters + 2 Words (first for max length, second for real length)
                        //Get the real length in Words
                        int size = GetIntAt(Buffer, Pos + 2);
                        //Extract string in UTF-16 unicode Big Endian.
                        return System.Text.Encoding.BigEndianUnicode.GetString(Buffer, Pos + 4, size * 2);
                }

                /// <summary>
                /// Writes an S7-1500 <c>WSTRING</c> value with two 16-bit length words followed by UTF-16 big-endian characters.
                /// </summary>
                /// <param name="Buffer">Byte buffer that will receive the control words and payload; must not be <see langword="null"/> and must provide sufficient space for the encoded characters.</param>
                /// <param name="Pos">Zero-based offset of the maximum-length word.</param>
                /// <param name="MaxCharNb">Maximum number of characters that the string is allowed to contain, stored in the first word.</param>
                /// <param name="Value">UTF-16 string to encode. Callers must ensure the encoded length does not exceed <paramref name="MaxCharNb"/> or the available buffer space because no truncation is performed.</param>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the buffer does not contain enough space for the control words and payload.</exception>
                public static void SetWStringAt(byte[] Buffer, int Pos, int MaxCharNb, string Value)
                {
                        //Get the length in words from number of characters
                        int size = Value.Length;
			//Set the Max length in Words 
			SetIntAt(Buffer, Pos, (short)MaxCharNb);
			//Set the real length in words
			SetIntAt(Buffer, Pos + 2, (short)size);
			//Set the UTF-16 unicode Big endian String (after max length and length)
			System.Text.Encoding.BigEndianUnicode.GetBytes(Value, 0, size, Buffer, Pos + 4);
		}

		#endregion

		#region Get/Set Array of char (S7 ARRAY OF CHARS)
                /// <summary>
                /// Reads a fixed-length sequence of UTF-8 characters from the specified buffer.
                /// </summary>
                /// <param name="Buffer">Byte buffer that stores the characters; must not be <see langword="null"/> and must expose at least <paramref name="Size"/> bytes starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the first byte to decode.</param>
                /// <param name="Size">Number of bytes to interpret as UTF-8 data.</param>
                /// <returns>A string produced from the requested range using UTF-8 decoding.</returns>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the requested range exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static string GetCharsAt(byte[] Buffer, int Pos, int Size)
                {
                        return System.Text.Encoding.UTF8.GetString(Buffer, Pos, Size);
                }

                /// <summary>
                /// Writes a UTF-8 string into the specified buffer without control bytes, truncating when insufficient space is available.
                /// </summary>
                /// <param name="Buffer">Byte buffer that will receive the characters; must not be <see langword="null"/>.</param>
                /// <param name="Pos">Zero-based offset of the first byte to write.</param>
                /// <param name="Value">UTF-8 string whose bytes will be copied. Content is truncated to the available buffer size.</param>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when <paramref name="Pos"/> exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static void SetCharsAt(byte[] Buffer, int Pos, string Value)
                {
                        int MaxLen = Buffer.Length - Pos;
                        // Truncs the string if there's no room enough
			if (MaxLen > Value.Length) MaxLen = Value.Length;
			System.Text.Encoding.UTF8.GetBytes(Value, 0, MaxLen, Buffer, Pos);
		}

		#endregion

		#region Get/Set Array of WChar (S7-1500 ARRAY OF CHARS)

                /// <summary>
                /// Reads a fixed-length sequence of UTF-16 big-endian characters (S7 WChar array) from the specified buffer.
                /// </summary>
                /// <param name="Buffer">Byte buffer that stores the characters; must not be <see langword="null"/> and must expose at least <paramref name="SizeInCharNb"/> × 2 bytes starting at <paramref name="Pos"/>.</param>
                /// <param name="Pos">Zero-based offset of the first byte to decode.</param>
                /// <param name="SizeInCharNb">Number of UTF-16 characters to decode.</param>
                /// <returns>A string produced from the requested range using big-endian UTF-16 decoding.</returns>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the requested range exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static String GetWCharsAt(byte[] Buffer, int Pos, int SizeInCharNb)
                {
                        //Extract Unicode UTF-16 Big-Endian character from the buffer. To use with WChar Datatype.
                        //Size to read is in byte. Be careful, 1 char = 2 bytes
                        return System.Text.Encoding.BigEndianUnicode.GetString(Buffer, Pos, SizeInCharNb * 2);
                }

                /// <summary>
                /// Writes a UTF-16 big-endian string into the specified buffer without control words, truncating to the available capacity.
                /// </summary>
                /// <param name="Buffer">Byte buffer that will receive the characters; must not be <see langword="null"/>.</param>
                /// <param name="Pos">Zero-based offset of the first byte to write.</param>
                /// <param name="Value">String whose characters will be encoded as UTF-16 big-endian values. Content is truncated to the available buffer capacity.</param>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when <paramref name="Pos"/> exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static void SetWCharsAt(byte[] Buffer, int Pos, string Value)
                {
                        //Compute Max length in char number
                        int MaxLen = (Buffer.Length - Pos) / 2;
			// Truncs the string if there's no room enough        
			if (MaxLen > Value.Length) MaxLen = Value.Length;
			System.Text.Encoding.BigEndianUnicode.GetBytes(Value, 0, MaxLen, Buffer, Pos);
		}

		#endregion

		#region Get/Set Counter
                /// <summary>
                /// Converts an S7 counter value stored as packed BCD into its integer representation.
                /// </summary>
                /// <param name="Value">Packed BCD counter value as transmitted by the PLC.</param>
                /// <returns>The integer counter value in the range 0 to 9999.</returns>
                public static int GetCounter(ushort Value)
                {
                        return BCDtoByte((byte)Value) * 100 + BCDtoByte((byte)(Value >> 8));
                }

                /// <summary>
                /// Reads an S7 counter value from an array of packed BCD words.
                /// </summary>
                /// <param name="Buffer">Array of packed BCD counters; must not be <see langword="null"/>.</param>
                /// <param name="Index">Zero-based index of the counter to decode.</param>
                /// <returns>The integer counter value in the range 0 to 9999.</returns>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when <paramref name="Index"/> is outside the bounds of <paramref name="Buffer"/>.</exception>
                public static int GetCounterAt(ushort[] Buffer, int Index)
                {
                        return GetCounter(Buffer[Index]);
                }

                /// <summary>
                /// Encodes an integer value as an S7 packed BCD counter.
                /// </summary>
                /// <param name="Value">Counter value to encode. Callers should constrain the value to the range 0 to 9999 to avoid invalid packed BCD digits.</param>
                /// <returns>A packed BCD representation suitable for storage in the PLC.</returns>
                public static ushort ToCounter(int Value)
                {
                        return (ushort)(ByteToBCD(Value / 100) + (ByteToBCD(Value % 100) << 8));
                }

                /// <summary>
                /// Writes an S7 packed BCD counter value into the specified array position.
                /// </summary>
                /// <param name="Buffer">Array that will receive the encoded counter; must not be <see langword="null"/>.</param>
                /// <param name="Pos">Zero-based index of the array element to overwrite.</param>
                /// <param name="Value">Counter value to encode as packed BCD.</param>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when <paramref name="Pos"/> is outside the bounds of <paramref name="Buffer"/>.</exception>
                public static void SetCounterAt(ushort[] Buffer, int Pos, int Value)
                {
                        Buffer[Pos] = ToCounter(Value);
                }
		#endregion

		#region Get/Set Timer

                /// <summary>
                /// Writes an S7 <c>TIME</c> value representing the total milliseconds of the specified <see cref="TimeSpan"/>.
                /// </summary>
                /// <param name="Buffer">Byte buffer that will receive four bytes of big-endian data; must not be <see langword="null"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <param name="Value"><see cref="TimeSpan"/> to encode. Its total milliseconds are truncated toward zero to fit into a 32-bit signed integer.</param>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when fewer than four bytes are available starting at <paramref name="Pos"/>.</exception>
                public static void SetS7TimespanAt(byte[] Buffer, int Pos, TimeSpan Value)
                {
                        SetDIntAt(Buffer, Pos, (Int32)Value.TotalMilliseconds);
                }

                /// <summary>
                /// Reads an S7 <c>TIME</c> value that stores total milliseconds into a <see cref="TimeSpan"/>.
                /// </summary>
                /// <param name="Buffer">Byte buffer containing at least four bytes at <paramref name="pos"/>; must not be <see langword="null"/>.</param>
                /// <param name="pos">Zero-based offset of the high-order byte.</param>
                /// <returns>A <see cref="TimeSpan"/> representing the encoded milliseconds, or <see cref="TimeSpan.Zero"/> when the buffer is too small.</returns>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the requested range exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static TimeSpan GetS7TimespanAt(byte[] Buffer, int pos)
                {
                        if (Buffer.Length < pos + 4)
                        {
                                return new TimeSpan();
                        }

                        Int32 a;
                        a = Buffer[pos + 0]; a <<= 8;
			a += Buffer[pos + 1]; a <<= 8;
			a += Buffer[pos + 2]; a <<= 8;
			a += Buffer[pos + 3];
			TimeSpan sp = new TimeSpan(0, 0, 0, 0, a);

			return sp;
		}

                /// <summary>
                /// Reads an S7 <c>LTIME</c> value that stores nanoseconds into a <see cref="TimeSpan"/>.
                /// </summary>
                /// <param name="Buffer">Byte buffer containing at least eight bytes at <paramref name="pos"/>; must not be <see langword="null"/>.</param>
                /// <param name="pos">Zero-based offset of the high-order byte.</param>
                /// <returns>A <see cref="TimeSpan"/> converted from the encoded nanoseconds, or <see cref="TimeSpan.Zero"/> when decoding fails.</returns>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when the requested range exceeds the bounds of <paramref name="Buffer"/>.</exception>
                public static TimeSpan GetLTimeAt(byte[] Buffer, int pos)
                {
                        //LTime size : 64 bits (8 octets)
                        //Case if the buffer is too small
                        if (Buffer.Length < pos + 8) return new TimeSpan();

			try
			{
				// Extract and Convert number of nanoseconds to tick (1 tick = 100 nanoseconds)
				return TimeSpan.FromTicks(GetLIntAt(Buffer, pos) / 100);
			}
			catch (Exception)
			{
				return new TimeSpan();
			}
		}

                /// <summary>
                /// Writes an S7 <c>LTIME</c> value representing the total nanoseconds of the specified <see cref="TimeSpan"/>.
                /// </summary>
                /// <param name="Buffer">Byte buffer that will receive eight bytes of big-endian data; must not be <see langword="null"/>.</param>
                /// <param name="Pos">Zero-based offset of the high-order byte.</param>
                /// <param name="Value"><see cref="TimeSpan"/> to encode. Its ticks are multiplied by 100 to convert to nanoseconds.</param>
                /// <exception cref="NullReferenceException">Thrown when <paramref name="Buffer"/> is <see langword="null"/>.</exception>
                /// <exception cref="IndexOutOfRangeException">Thrown when fewer than eight bytes are available starting at <paramref name="Pos"/>.</exception>
                public static void SetLTimeAt(byte[] Buffer, int Pos, TimeSpan Value)
                {
                        SetLIntAt(Buffer, Pos, (long)(Value.Ticks * 100));
                }

		#endregion

                /// <summary>
                /// Creates a new byte buffer initialized to 1024 zero bytes.
                /// </summary>
                /// <returns>A newly allocated 1 KB buffer suitable for use with the helper functions.</returns>
                public byte[] GenerateByteArray()
        {
                        byte[] Result = new byte[1024];

                        return Result;
        }

		#endregion [Help Functions]


	}
}
