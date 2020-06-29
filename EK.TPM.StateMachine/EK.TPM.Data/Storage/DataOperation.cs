// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataOperation.cs" company="E&amp;K Automation GmbH">
//   Copyright (c) E&amp;K Automation GmbH. All rights reserved.
// </copyright>
// <summary>
//   Defines the staic class DataOperation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EK.TPM.Data.Storage
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Mail;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Defines the static class DataOperation.
    /// </summary>
    [Serializable]
    public static class DataOperation
    {
        #region fields

        /// <summary>
        /// The _hex digits.
        /// </summary>
        private static readonly char[] LookupTableHexDigits = "0123456789abcdef".ToCharArray();
        #endregion

        #region Enums

        /// <summary>
        /// Defines the enumeration ByteFormat.
        /// </summary>
        [Serializable]
        public enum ByteFormat
        {
            /// <summary>
            /// Byte format Intel.
            /// </summary>
            Intel,

            /// <summary>
            /// Byte format Motorolla.
            /// </summary>
            Motorolla,
        }

        #endregion

        #region Methods

        /// <summary>
        /// Calculates the week of year.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// Week of year of date.
        /// </returns>
        public static int CalcualteWeekOfYear(DateTime date)
        {
            return (date.DayOfYear / 7) + 1 >= 53 ? 1 : (date.DayOfYear / 7) + 1;
        }

        /// <summary>
        /// Converts the byte values highByte and lowByte to a unsigned short value.
        /// </summary>
        /// <param name="highByte">
        /// The high byte.
        /// </param>
        /// <param name="lowByte">
        /// The low byte.
        /// </param>
        /// <returns>
        /// The unsigned short value.
        /// </returns>
        public static ushort ToUshort(byte highByte, byte lowByte)
        {
            return (ushort)((highByte * 256) + lowByte);
        }

        /// <summary>
        /// Converts the values included in high byte, third byte, second byte and low byte to a unsigned integer value.
        /// </summary>
        /// <param name="highByte">
        /// The high byte.
        /// </param>
        /// <param name="thirdByte">
        /// The third byte.
        /// </param>
        /// <param name="secondByte">
        /// The second byte.
        /// </param>
        /// <param name="lowByte">
        /// The low byte.
        /// </param>
        /// <returns>
        /// The unsigned integer value.
        /// </returns>
        public static uint ToUint(byte highByte, byte thirdByte, byte secondByte, byte lowByte)
        {
            return (uint)((highByte * 16777216) + (thirdByte * 65536) + (secondByte * 256) + lowByte);
        }

        /// <summary>
        /// Converts the values included in high byte, third byte, second byte and low byte to a integer value.
        /// </summary>
        /// <param name="highByte">
        /// The high byte.
        /// </param>
        /// <param name="thirdByte">
        /// The third byte.
        /// </param>
        /// <param name="secondByte">
        /// The second byte.
        /// </param>
        /// <param name="lowByte">
        /// The low byte.
        /// </param>
        /// <returns>
        /// The integer value.
        /// </returns>
        public static int ToInt(byte highByte, byte thirdByte, byte secondByte, byte lowByte)
        {
            return (highByte * 16777216) + (thirdByte * 65536) + (secondByte * 256) + lowByte;
        }

        /// <summary>
        /// Converts the values included in high byte, seventh byte, sixth byte, fifth byte,
        /// fourth byte, third byte, second byte and low byte to a long value.
        /// </summary>
        /// <param name="highByte">
        /// The high byte.
        /// </param>
        /// <param name="seventhByte">
        /// The seventh byte.
        /// </param>
        /// <param name="sixthByte">
        /// The sixth byte.
        /// </param>
        /// <param name="fifthByte">
        /// The fifth byte.
        /// </param>
        /// <param name="fourthByte">
        /// The fourth byte.
        /// </param>
        /// <param name="thirdByte">
        /// The third byte.
        /// </param>
        /// <param name="secondByte">
        /// The second byte.
        /// </param>
        /// <param name="lowByte">
        /// The low byte.
        /// </param>
        /// <returns>
        /// The long value.
        /// </returns>
        public static long ToLong(
            byte highByte,
            byte seventhByte,
            byte sixthByte,
            byte fifthByte,
            byte fourthByte,
            byte thirdByte,
            byte secondByte,
            byte lowByte)
        {
            long retVal = 0;
            retVal = highByte;
            retVal = retVal << 8;
            retVal += seventhByte;
            retVal = retVal << 8;
            retVal += sixthByte;
            retVal = retVal << 8;
            retVal += fifthByte;
            retVal = retVal << 8;
            retVal += fourthByte;
            retVal = retVal << 8;
            retVal += thirdByte;
            retVal = retVal << 8;
            retVal += secondByte;
            retVal = retVal << 8;
            retVal += lowByte;
            return retVal;
        }

        /// <summary>
        /// Converts a string including hexadecimal values to a unsigned short value.
        /// </summary>
        /// <param name="hexString">
        /// The hex string.
        /// </param>
        /// <returns>
        /// The unsigned short value.
        /// </returns>
        public static ushort ToUshort(string hexString)
        {
            ushort dec = 0;
            dec = ushort.Parse(hexString, NumberStyles.HexNumber);
            return dec;
        }

        /// <summary>
        /// Converts the byte values included in highByte and lowByte to a short value.
        /// </summary>
        /// <param name="highByte">
        /// The high byte.
        /// </param>
        /// <param name="lowByte">
        /// The low byte.
        /// </param>
        /// <returns>
        /// The short value.
        /// </returns>
        public static short ToShort(byte highByte, byte lowByte)
        {
            return (short)((highByte * 256) + lowByte);
        }

        /// <summary>
        /// Converts the values included in high byte, third Byte, second Byte and low Byte to a float value by using 'Little Endian'.
        /// </summary>
        /// <param name="highByte">
        /// The high byte.
        /// </param>
        /// <param name="thirdByte">
        /// The third byte.
        /// </param>
        /// <param name="secondByte">
        /// The second byte.
        /// </param>
        /// <param name="lowByte">
        /// The low byte.
        /// </param>
        /// <returns>
        /// The float value.
        /// </returns>
        public static float ToFloat(byte highByte, byte thirdByte, byte secondByte, byte lowByte)
        {
            return ToFloat(highByte, thirdByte, secondByte, lowByte, true);
        }

        /// <summary>
        /// Converts the values included in high byte, third Byte, second Byte and low Byte to a float value by using 'Little Endian', if little Endian is true,
        /// or 'Big Endian', if little Endian is false.
        /// </summary>
        /// <param name="highByte">
        /// The high byte.
        /// </param>
        /// <param name="thirdByte">
        /// The third byte.
        /// </param>
        /// <param name="secondByte">
        /// The second byte.
        /// </param>
        /// <param name="lowByte">
        /// The low byte.
        /// </param>
        /// <param name="littleEndian">
        /// If set to <c>true</c> [little endian].
        /// </param>
        /// <returns>
        /// The float value.
        /// </returns>
        public static float ToFloat(byte highByte, byte thirdByte, byte secondByte, byte lowByte, bool littleEndian)
        {
            if (!littleEndian)
            {
                return BitConverter.ToSingle(new[] { lowByte, secondByte, thirdByte, highByte }, 0);
            }

            return BitConverter.ToSingle(new[] { highByte, thirdByte, secondByte, lowByte }, 0);
        }

        /// <summary>
        /// Converts a unsigned short value to a byte array including 4 hexadecimal values.
        /// </summary>
        /// <param name="value">
        /// The unsigned short value.
        /// </param>
        /// <returns>
        /// The byte array.
        /// </returns>
        public static byte[] To_HEX(ushort value)
        {
            byte[] ret = new byte[4];
            ret[0] = (byte)(value & 0x000F);
            ret[1] = (byte)((value / 16) & 0x000F);
            ret[2] = (byte)((value / 256) & 0x000F);
            ret[3] = (byte)((value / 4096) & 0x000F);
            return ret;
        }

        /// <summary>
        /// Converts the boolean array boolean into a byte array including the boolean values in the bits of byte array.
        /// </summary>
        /// <param name="bools">
        /// The booleans array.
        /// </param>
        /// <returns>
        /// A byte array including boolean values.
        /// </returns>
        public static byte[] ToByte(bool[] bools)
        {
            int bytes = bools.Length / 8;
            if ((bools.Length % 8) != 0)
            {
                bytes++;
            }

            byte[] arr = new byte[bytes];
            int bitIndex = 0, byteIndex = 0;
            for (int i = 0; i < bools.Length; i++)
            {
                if (bools[i])
                {
                    arr[byteIndex] |= (byte)Math.Pow(2, bitIndex);
                }

                bitIndex++;
                if (bitIndex == 8)
                {
                    bitIndex = 0;
                    byteIndex++;
                }
            }

            return arr;
        }

        /// <summary>
        /// Converts a unsigned short value to a byte array by using 'Big-Endian' (Motorola-Format).
        /// </summary>
        /// <param name="value">
        /// The unsigned short value.
        /// </param>
        /// <returns>
        /// A byte array including the unsigned short value.
        /// </returns>
        public static byte[] ToByte(ushort value)
        {
            return ToByte(value, true);
        }

        /// <summary>
        /// Converts a unsigned short value to a byte array by using 'Big-Endian' (Motorola-Format), if bigEndian is true,
        /// or 'Little-Endian', if bigEndian is false.
        /// </summary>
        /// <param name="value">
        /// The unsigned short value.
        /// </param>
        /// <param name="bigEndian">
        /// If set to <c>true</c> [big endian].
        /// </param>
        /// <returns>
        /// A byte array including the unsigned short value.
        /// </returns>
        public static byte[] ToByte(ushort value, bool bigEndian)
        {
            byte[] result = new byte[2];
            if (bigEndian)
            {
                result[1] = (byte)(value % 256);
                result[0] = (byte)((value >> 8) % 256);
            }
            else
            {
                result[0] = (byte)(value % 256);
                result[1] = (byte)((value >> 8) % 256);
            }

            return result;
        }

        /// <summary>
        /// Converts a short value to a byte array by using 'Little-Endian' (Intel-Format).
        /// </summary>
        /// <param name="value">
        /// The short value.
        /// </param>
        /// <returns>
        /// A byte array including the short value.
        /// </returns>
        public static byte[] ToByte(short value)
        {
            return ToByte(value, true);
        }

        /// <summary>
        /// Converts a short value to a byte array by using 'Little-Endian' (Intel-Format), if littleEndian is true,
        /// or 'Big-Endian', if littleEndian is false.
        /// </summary>
        /// <param name="value">
        /// The short value.
        /// </param>
        /// <param name="littleEndian">
        /// If set to <c>true</c> [little endian].
        /// </param>
        /// <returns>
        /// A byte array including the short value.
        /// </returns>
        public static byte[] ToByte(short value, bool littleEndian)
        {
            byte[] result = new byte[2];
            if (littleEndian)
            {
                result[0] = (byte)(value / 256);
                result[1] = (byte)(value % 256);
            }
            else
            {
                result[1] = (byte)(value / 256);
                result[0] = (byte)(value % 256);
            }

            return result;
        }

        /// <summary>
        /// Converts a unsigned integer value to a byte array by using 'Little Endian' (Intel-Format).
        /// </summary>
        /// <param name="value">
        /// The unsigned integer value.
        /// </param>
        /// <returns>
        /// A byte array including the unsigned integer value.
        /// </returns>
        public static byte[] ToByte(uint value)
        {
            return ToByte(value, true);
        }

        /// <summary>
        /// Converts a unsigned integer value to a byte array by using 'Little-Endian' (Intel-Format), if littleEndian is true,
        /// or 'Big-Endian', if littleEndian is false.
        /// </summary>
        /// <param name="value">
        /// The unsigned integer value.
        /// </param>
        /// <param name="littleEndian">
        /// If set to <c>true</c> [little endian].
        /// </param>
        /// <returns>
        /// A byte array including the unsigned integer value.
        /// </returns>
        public static byte[] ToByte(uint value, bool littleEndian)
        {
            byte[] result = new byte[4];
            if (littleEndian)
            {
                result[3] = (byte)(value % 256);
                result[2] = (byte)((value >> 8) % 256);
                result[1] = (byte)((value >> 16) % 256);
                result[0] = (byte)((value >> 24) % 256);
            }
            else
            {
                result[0] = (byte)(value % 256);
                result[1] = (byte)((value >> 8) % 256);
                result[2] = (byte)((value >> 16) % 256);
                result[3] = (byte)((value >> 24) % 256);
            }

            return result;
        }

        /// <summary>
        /// Converts a integer value to a byte array by using 'Little Endian' (Intel-Format).
        /// </summary>
        /// <param name="value">
        /// The integer value.
        /// </param>
        /// <returns>
        /// A byte array including the integer value.
        /// </returns>
        public static byte[] ToByte(int value)
        {
            return ToByte(value, true);
        }

        /// <summary>
        /// Converts a integer value to a byte array by using 'Little-Endian' (Intel-Format), if littleEndian is true,
        /// or 'Big-Endian', if littleEndian is false.
        /// </summary>
        /// <param name="value">
        /// The integer value.
        /// </param>
        /// <param name="littleEndian">
        /// If set to <c>true</c> [little endian].
        /// </param>
        /// <returns>
        /// A byte array including the integer value.
        /// </returns>
        public static byte[] ToByte(int value, bool littleEndian)
        {
            byte[] result = new byte[4];
            if (littleEndian)
            {
                result[3] = (byte)(value % 256);
                result[2] = (byte)((value >> 8) % 256);
                result[1] = (byte)((value >> 16) % 256);
                result[0] = (byte)((value >> 24) % 256);
            }
            else
            {
                result[0] = (byte)(value % 256);
                result[1] = (byte)((value >> 8) % 256);
                result[2] = (byte)((value >> 16) % 256);
                result[3] = (byte)((value >> 24) % 256);
            }

            return result;
        }

        /// <summary>
        /// Converts a unsigned long value to a byte array by using 'Little-Endian' (Intel-Format).
        /// </summary>
        /// <param name="value">
        /// The unsigned long value.
        /// </param>
        /// <returns>
        /// A byte array including the unsigned long value.
        /// </returns>
        public static byte[] ToByte(ulong value)
        {
            return ToByte(value, true);
        }

        /// <summary>
        /// Converts a unsigned long value to a byte array by using 'Little Endian' (Intel-Format), if little endian is true,
        /// or 'Big Endian', if little endian is false.
        /// </summary>
        /// <param name="value">
        /// The unsigned long value.
        /// </param>
        /// <param name="littleEndian">
        /// If set to <c>true</c> [little endian].
        /// </param>
        /// <returns>
        /// A byte array including the unsigned long value.
        /// </returns>
        public static byte[] ToByte(ulong value, bool littleEndian)
        {
            byte[] result = new byte[8];
            if (littleEndian)
            {
                result[7] = (byte)(value % 256);
                result[6] = (byte)((value >> 8) % 256);
                result[5] = (byte)((value >> 16) % 256);
                result[4] = (byte)((value >> 24) % 256);
                result[3] = (byte)((value >> 32) % 256);
                result[2] = (byte)((value >> 40) % 256);
                result[1] = (byte)((value >> 48) % 256);
                result[0] = (byte)((value >> 56) % 256);
            }
            else
            {
                result[0] = (byte)(value % 256);
                result[1] = (byte)((value >> 8) % 256);
                result[2] = (byte)((value >> 16) % 256);
                result[3] = (byte)((value >> 24) % 256);
                result[4] = (byte)((value >> 32) % 256);
                result[5] = (byte)((value >> 40) % 256);
                result[6] = (byte)((value >> 48) % 256);
                result[7] = (byte)((value >> 56) % 256);
            }

            return result;
        }

        /// <summary>
        /// Converts a long value to a byte array by using 'Little-Endian' (Intel-Format).
        /// </summary>
        /// <param name="value">
        /// The long value.
        /// </param>
        /// <returns>
        /// A byte array including the long value.
        /// </returns>
        public static byte[] ToByte(long value)
        {
            return ToByte(value, true);
        }

        /// <summary>
        /// Converts a long value to a byte array by using 'Little-Endian' (Intel-Format), if littleEndian is true,
        /// or 'Big-Endian', if littleEndian is false.
        /// </summary>
        /// <param name="value">
        /// The long value.
        /// </param>
        /// <param name="littleEndian">
        /// If set to <c>true</c> [little endian].
        /// </param>
        /// <returns>
        /// A byte array including the long value.
        /// </returns>
        public static byte[] ToByte(long value, bool littleEndian)
        {
            byte[] result = new byte[8];
            if (littleEndian)
            {
                result[7] = (byte)(value % 256);
                result[6] = (byte)((value >> 8) % 256);
                result[5] = (byte)((value >> 16) % 256);
                result[4] = (byte)((value >> 24) % 256);
                result[3] = (byte)((value >> 32) % 256);
                result[2] = (byte)((value >> 40) % 256);
                result[1] = (byte)((value >> 48) % 256);
                result[0] = (byte)((value >> 56) % 256);
            }
            else
            {
                result[0] = (byte)(value % 256);
                result[1] = (byte)((value >> 8) % 256);
                result[2] = (byte)((value >> 16) % 256);
                result[3] = (byte)((value >> 24) % 256);
                result[4] = (byte)((value >> 32) % 256);
                result[5] = (byte)((value >> 40) % 256);
                result[6] = (byte)((value >> 48) % 256);
                result[7] = (byte)((value >> 56) % 256);
            }

            return result;
        }

        /// <summary>
        /// Converts a float value to a byte array by using 'Little-Endian' (Intel-Format).
        /// </summary>
        /// <param name="value">
        /// The float value.
        /// </param>
        /// <returns>
        /// A byte array including the float value.
        /// </returns>
        public static byte[] ToByte(float value)
        {
            return ToByte(value, true);
        }

        /// <summary>
        /// Converts a float value to a byte array by using 'Little-Endian' (Intel-Format), if littleEndian is true,
        /// or 'Big-Endian', if littleEndian is false.
        /// </summary>
        /// <param name="value">
        /// The float value.
        /// </param>
        /// <param name="littleEndian">
        /// If set to <c>true</c> [little endian].
        /// </param>
        /// <returns>
        /// A byte array including the float value.
        /// </returns>
        public static byte[] ToByte(float value, bool littleEndian)
        {
            byte[] result = BitConverter.GetBytes(value);
            if (!littleEndian)
            {
                byte[] result2 = new byte[result.Length];
                for (int i = 0; i < result.Length; i++)
                {
                    result2[i] = result[result.Length - 1 - i];
                }

                return result2;
            }

            return result;
        }

        /// <summary>
        /// Converts a double value to a byte array by using 'Little-Endian' (Intel-Format).
        /// </summary>
        /// <param name="value">
        /// The double value.
        /// </param>
        /// <returns>
        /// A byte array including the double value.
        /// </returns>
        public static byte[] ToByte(double value)
        {
            return ToByte(value, true);
        }

        /// <summary>
        /// Converts a double value to a byte array by using 'Little-Endian' (Intel-Format), if littleEndian is true,
        /// or 'Big-Endian', if littleEndian is false.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="littleEndian">
        /// If set to <c>true</c> [little endian].
        /// </param>
        /// <returns>
        /// A byte array including the decimal value.
        /// </returns>
        public static byte[] ToByte(double value, bool littleEndian)
        {
            byte[] result = BitConverter.GetBytes(value);
            if (!littleEndian)
            {
                byte[] result2 = new byte[result.Length];
                for (int i = 0; i < result.Length; i++)
                {
                    result2[i] = result[result.Length - 1 - i];
                }

                return result2;
            }

            return result;
        }

        /// <summary>
        /// Converts a decimal value to a byte array by using 'Little-Endian' (Intel-Format).
        /// </summary>
        /// <param name="value">
        /// The decimal value.
        /// </param>
        /// <returns>
        /// A byte array including the decimal value.
        /// </returns>
        public static byte[] ToByte(decimal value)
        {
            return ToByte(value, true);
        }

        /// <summary>
        /// Converts a decimal value to a byte array by using 'Little-Endian' (Intel-Format), if littleEndian is true,
        /// or 'Big-Endian', if littleEndian is false.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="littleEndian">
        /// If set to <c>true</c> [little endian].
        /// </param>
        /// <returns>
        /// A byte array including the decimal value.
        /// </returns>
        public static byte[] ToByte(decimal value, bool littleEndian)
        {
            byte[] result = null;
            MemoryStream stream = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(value);
                result = stream.ToArray();
            }

            if (!littleEndian)
            {
                byte[] result2 = new byte[result.Length];
                for (int i = 0; i < result.Length; i++)
                {
                    result2[i] = result[result.Length - 1 - i];
                }

                return result2;
            }

            return result;
        }

        /// <summary>
        /// Converts the word array to byte array.
        /// </summary>
        /// <param name="words">
        /// The array words.
        /// </param>
        /// <param name="swapBytes">
        /// If set to <c>true</c> [swap bytes].
        /// </param>
        /// <returns>
        /// The byte array.
        /// </returns>
        public static byte[] ConvertWordArrayToByteArray(ushort[] words, bool swapBytes)
        {
            byte[] bytes = new byte[2 * words.Length];
            if (swapBytes == false)
            {
                for (int i = 0; i < words.Length; i++)
                {
                    bytes[2 * i] = (byte)(words[i] / 256);
                    bytes[1 + (2 * i)] = (byte)(words[i] % 256);
                }
            }
            else
            {
                for (int i = 0; i < words.Length; i++)
                {
                    bytes[1 + (2 * i)] = (byte)(words[i] / 256);
                    bytes[2 * i] = (byte)(words[i] % 256);
                }
            }

            return bytes;
        }

        /// <summary>
        /// Converts the word array to byte array.
        /// </summary>
        /// <param name="words">
        /// The array words.
        /// </param>
        /// <param name="swapBytes">
        /// If set to <c>true</c> [swap bytes].
        /// </param>
        /// <returns>
        /// The byte array.
        /// </returns>
        public static byte[] ConvertWordArrayToByteArray(short[] words, bool swapBytes)
        {
            byte[] bytes = new byte[2 * words.Length];
            if (swapBytes == false)
            {
                for (int i = 0; i < words.Length; i++)
                {
                    bytes[2 * i] = (byte)((ushort)words[i] / 256);
                    bytes[1 + (2 * i)] = (byte)((ushort)words[i] % 256);
                }
            }
            else
            {
                for (int i = 0; i < words.Length; i++)
                {
                    bytes[1 + (2 * i)] = (byte)(words[i] / 256);
                    bytes[2 * i] = (byte)(words[i] % 256);
                }
            }

            return bytes;
        }

        /// <summary>
        /// Bytes the array to unsigned word array.
        /// </summary>
        /// <param name="bytes">
        /// The bytes.
        /// </param>
        /// <param name="sequenceHighByteLowByte">
        /// If set to <c>true</c> [sequence high byte low byte].
        /// </param>
        /// <returns>
        /// The unsigned short array of data.
        /// </returns>
        public static ushort[] ByteArrayToUnsignedWordArray(byte[] bytes, bool sequenceHighByteLowByte)
        {
            if ((bytes.Length % 2) == 0)
            {
                ushort[] words = new ushort[bytes.Length / 2];
                if (sequenceHighByteLowByte)
                {
                    for (int i = 0; i < words.Length; i++)
                    {
                        words[i] = (ushort)((bytes[2 * i] * 256) + bytes[1 + (2 * i)]);
                    }
                }
                else
                {
                    for (int i = 0; i < words.Length; i++)
                    {
                        words[i] = (ushort)((bytes[1 + (2 * i)] * 256) + bytes[2 * i]);
                    }
                }

                return words;
            }

            throw new Exception("Odd number of bytes!");
        }

        /// <summary>
        /// Bytes the array to signed word array.
        /// </summary>
        /// <param name="bytes">
        /// The bytes.
        /// </param>
        /// <param name="sequenceHighByteLowByte">
        /// If set to <c>true</c> [sequence high byte low byte].
        /// </param>
        /// <returns>
        /// The short array of data.
        /// </returns>
        public static short[] ByteArrayToSignedWordArray(byte[] bytes, bool sequenceHighByteLowByte)
        {
            if ((bytes.Length % 2) == 0)
            {
                short[] words = new short[bytes.Length / 2];
                if (sequenceHighByteLowByte)
                {
                    for (int i = 0; i < words.Length; i++)
                    {
                        words[i] = (short)((bytes[2 * i] * 256) + bytes[1 + (2 * i)]);
                    }
                }
                else
                {
                    for (int i = 0; i < words.Length; i++)
                    {
                        words[i] = (short)((bytes[1 + (2 * i)] * 256) + bytes[2 * i]);
                    }
                }

                return words;
            }

            throw new Exception("Odd number of bytes!");
        }

        /// <summary>
        /// Convert a byte array into boolean array including values for each bit in each byte of argument bytes..
        /// </summary>
        /// <param name="bytes">
        /// The byte array bytes.
        /// </param>
        /// <returns>
        /// A boolean array including values for each bit in each byte of bytes.
        /// </returns>
        public static bool[] ToBool(byte[] bytes)
        {
            int bools = bytes.Length * 8;
            bool[] arr = new bool[bools];
            int boolsIndex = 0;

            for (int byteIndex = 0; byteIndex < bytes.Length; byteIndex++)
            {
                for (int bitIndex = 0; bitIndex < 8; bitIndex++)
                {
                    if ((bytes[byteIndex] & (byte)Math.Pow(2, bitIndex)) != 0)
                    {
                        arr[boolsIndex] = true;
                    }

                    if ((bytes[byteIndex] & (byte)Math.Pow(2, bitIndex)) == 0)
                    {
                        arr[boolsIndex] = false;
                    }

                    boolsIndex++;
                }
            }

            return arr;
        }

        /// <summary>
        /// Bytes the array to string.
        /// </summary>
        /// <param name="bytes">
        /// The bytes.
        /// </param>
        /// <returns>
        /// The byte array as string.
        /// </returns>
        public static string ByteArrayToString(byte[] bytes)
        {
            Encoding encoding = System.Text.Encoding.Default;
            return encoding.GetString(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Gets the sub array.
        /// </summary>
        /// <typeparam name="T">A abstract type.</typeparam>
        /// <param name="array">The array to build from the sub array.</param>
        /// <param name="firstIndex">The first index.</param>
        /// <param name="length">The length.</param>
        /// <returns>
        /// An array of the abstract type.
        /// </returns>
        public static T[] GetSubArray<T>(T[] array, int firstIndex, int length)
        {
            if (firstIndex + length > array.Length)
            {
                length = array.Length - firstIndex;
            }

            if (length <= 0)
            {
                return new T[0];
            }

            T[] tmp = new T[length];
            for (int i = 0; i < length; i++)
            {
                tmp[i] = array[i + firstIndex];
            }

            return tmp;
        }

        /// <summary>
        /// Finds the values in array.
        /// </summary>
        /// <typeparam name="T">
        /// An abstract type.
        /// </typeparam>
        /// <param name="searchIn">
        /// The search in.
        /// </param>
        /// <param name="searchValues">
        /// The search values.
        /// </param>
        /// <param name="startIndex">
        /// The start index.
        /// </param>
        /// <returns>
        /// The Index of searchValues if found, else -1.
        /// </returns>
        public static int FindValuesInArray<T>(T[] searchIn, T[] searchValues, int startIndex)
        {
            for (int i = startIndex, k = 0; i < searchIn.Length; i++)
            {
                if (searchIn[i].Equals(searchValues[k]))
                {
                    k++;
                }
                else
                {
                    k = 0;
                }

                if (k == searchValues.Length)
                {
                    return i - (searchValues.Length - 1);
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets the decimal ASCII string.
        /// </summary>
        /// <param name="data">
        /// The byte array data.
        /// </param>
        /// <returns>
        /// A string including the decimal value of each byte of data in [].
        /// </returns>
        public static string GetDezASCIIstring(byte[] data)
        {
            string text = string.Empty;
            for (int i = 0; i < data.Length; i++)
            {
                text += "[" + data[i] + "]";
            }

            return text;
        }

        /// <summary>
        /// Gets the hexadecimal ascii string.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>A string where each value of the byte data is represented as hexadecimal.</returns>
        public static string GetHexASCIIstring(this byte[] bytes)
        {
            char[] digits = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                int d1;
                d1 = Math.DivRem(bytes[i], 16, out int d2);
                int ix = 2 * i;
                digits[ix] = LookupTableHexDigits[d1];
                digits[ix + 1] = LookupTableHexDigits[d2];
            }

            return new string(digits);
        }

        /// <summary>
        /// Compares the arrays data1 and data2.
        /// </summary>
        /// <param name="data1">
        /// The array data1.
        /// </param>
        /// <param name="data2">
        /// The array data2.
        /// </param>
        /// <returns>
        /// True if arrays are equal, else false.
        /// </returns>
        public static bool CompareArrays(Array data1, Array data2)
        {
            if (data1 == null && data2 == null)
            {
                // If both are null, they're equal
                return true;
            }

            if (data1 == null || data2 == null)
            {
                // If either but not both are null, they're not equal
                return false;
            }

            if (data1.Length != data2.Length)
            {
                return false;
            }

            for (int i = 0; i < data1.Length; i++)
            {
                if (data1.GetValue(i).ToString() != data2.GetValue(i).ToString())
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Converts the bits of each byte of array bytes to a boolean array.
        /// </summary>
        /// <param name="bytes">
        /// The array bytes.
        /// </param>
        /// <returns>
        /// A boolean array.
        /// </returns>
        public static bool[] ToBoolArray(byte[] bytes)
        {
            int bools = bytes.Length * 8;
            bool[] arr = new bool[bools];
            int boolsIndex = 0;

            for (int byteIndex = 0; byteIndex < bytes.Length; byteIndex++)
            {
                for (int bitIndex = 0; bitIndex < 8; bitIndex++)
                {
                    if ((bytes[byteIndex] & (byte)Math.Pow(2, bitIndex)) != 0)
                    {
                        arr[boolsIndex] = true;
                    }

                    if ((bytes[byteIndex] & (byte)Math.Pow(2, bitIndex)) == 0)
                    {
                        arr[boolsIndex] = false;
                    }

                    boolsIndex++;
                }
            }

            return arr;
        }

        /// <summary>
        /// Converts the bits of a byte value to a boolean array.
        /// </summary>
        /// <param name="value">
        /// The byte value.
        /// </param>
        /// <returns>
        /// A boolean array.
        /// </returns>
        public static bool[] ToBoolArray(byte value)
        {
            bool[] returnvalue = new bool[8];

            for (int i = 0; i < 8; i++)
            {
                returnvalue[i] = (value & (byte)Math.Pow(2, i)) > 0;
            }

            return returnvalue;
        }

        /// <summary>
        /// Converts an unsigned short into a boolean array including a boolean value for each bit in the unsigned short.
        /// </summary>
        /// <param name="value">
        /// The unsigned short value.
        /// </param>
        /// <returns>
        /// A boolean array.
        /// </returns>
        public static bool[] ToBoolArray(ushort value)
        {
            bool[] returnvalue = new bool[16];

            for (int i = 0; i < 16; i++)
            {
                returnvalue[i] = (value & (ushort)Math.Pow(2, i)) > 0;
            }

            return returnvalue;
        }

        /// <summary>
        /// Converts the bits of a unsigned integer value to a boolean array.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// A boolean array.
        /// </returns>
        public static bool[] ToBoolArray(uint value)
        {
            bool[] returnvalue = new bool[32];

            for (int i = 0; i < 32; i++)
            {
                returnvalue[i] = (value & (uint)Math.Pow(2, i)) > 0;
            }

            return returnvalue;
        }

        /// <summary>
        /// Converts the bits of a unsigned long value to a boolean array.
        /// </summary>
        /// <param name="value">
        /// The unsigned long value.
        /// </param>
        /// <returns>
        /// A boolean array.
        /// </returns>
        public static bool[] ToBoolArray(ulong value)
        {
            bool[] returnvalue = new bool[64];

            for (ulong i = 0; i < 64; i++)
            {
                returnvalue[i] = (value & (ulong)Math.Pow(2, i)) > 0;
            }

            return returnvalue;
        }

        /// <summary>
        /// Converts the boolean array values to a byte value.
        /// </summary>
        /// <param name="values">
        /// The boolean array values.
        /// </param>
        /// <returns>
        /// A byte including the boolean values in the bits.
        /// </returns>
        public static byte FromBoolArrayToByte(bool[] values)
        {
            if (values.Length != 8)
            {
                throw new Exception("Unsufficiant number of Bool");
            }

            byte returnvalue = 0;

            for (int i = 0; i < 8; i++)
            {
                if (values[i])
                {
                    returnvalue |= (byte)Math.Pow(2, i);
                }
            }

            return returnvalue;
        }

        /// <summary>
        /// Converts the boolean array values to a byte value.
        /// </summary>
        /// <param name="values">
        /// The boolean array values.
        /// </param>
        /// <returns>
        /// A byte array including the boolean values in the bits.
        /// </returns>
        public static byte[] FromBoolArrayToByteArray(bool[] values)
        {
            int amoutofbytes = values.Length / 8;
            if (values.Length % 8 != 0)
            {
                amoutofbytes++;
            }

            byte[] returnvalue = new byte[amoutofbytes];

            for (int j = 0; j < amoutofbytes; j++)
            {
                byte tmpbyte = 0;
                for (int i = 0; i < 8 && ((j * 8) + i) < values.Length; i++)
                {
                    if (values[(j * 8) + i])
                    {
                        tmpbyte |= (byte)Math.Pow(2, i);
                    }
                }

                returnvalue[j] = tmpbyte;
            }

            return returnvalue;
        }

        /// <summary>
        /// Converts the boolean array to unsigned integer 16.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// A unsigned short including the boolean values in the bits.
        /// </returns>
        public static ushort FromBoolArrayToUInt16(bool[] value)
        {
            if (value.Length != 16)
            {
                throw new Exception("Unsufficiant number of Bool");
            }

            ushort returnvalue = 0;

            for (int i = 0; i < 16; i++)
            {
                if (value[i])
                {
                    returnvalue |= (ushort)Math.Pow(2, i);
                }
            }

            return returnvalue;
        }

        /// <summary>
        /// Converts the boolean array to unsigned integer 32.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// A unsigned integer including the boolean values in the bits.
        /// </returns>
        public static uint FromBoolArrayToUInt32(bool[] value)
        {
            if (value.Length != 32)
            {
                throw new Exception("Unsufficiant number of Bool");
            }

            uint returnvalue = 0;

            for (int i = 0; i < 32; i++)
            {
                if (value[i])
                {
                    returnvalue |= (uint)Math.Pow(2, i);
                }
            }

            return returnvalue;
        }

        /// <summary>
        /// Converts the boolean array to unsigned integer 64.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// A unsigned long including the boolean values in the bits.
        /// </returns>
        public static ulong FromBoolArrayToUInt64(bool[] value)
        {
            if (value.Length != 64)
            {
                throw new Exception("Unsufficiant number of Bool");
            }

            ulong returnvalue = 0;

            for (int i = 0; i < 64; i++)
            {
                if (value[i])
                {
                    returnvalue |= (ulong)Math.Pow(2, i);
                }
            }

            return returnvalue;
        }

        /// <summary>
        /// From String to integer 32.
        /// </summary>
        /// <param name="text">
        /// The string text.
        /// </param>
        /// <param name="startPos">
        /// The start position.
        /// </param>
        /// <param name="byteFormat">
        /// The byte format.
        /// </param>
        /// <returns>
        /// A integer including 4 characters from text beginning with start Position.
        /// </returns>
        public static int FromStringToInt32(string text, int startPos, ByteFormat byteFormat)
        {
            char[] tmp = text.Substring(startPos, 4).ToCharArray();

            if (byteFormat == ByteFormat.Intel)
            {
                return ToInt((byte)tmp[0], (byte)tmp[1], (byte)tmp[2], (byte)tmp[3]);
            }

            return ToInt((byte)tmp[3], (byte)tmp[2], (byte)tmp[1], (byte)tmp[0]);
        }

        /// <summary>
        /// From strings to integer 16.
        /// </summary>
        /// <param name="text">
        /// The text including 2 characters to convert into short.
        /// </param>
        /// <param name="startPos">
        /// The start position.
        /// </param>
        /// <param name="byteFormat">
        /// The byte format.
        /// </param>
        /// <returns>
        /// A short including 2 characters from text beginning with start Position.
        /// </returns>
        public static short FromStringToInt16(string text, int startPos, ByteFormat byteFormat)
        {
            char[] tmp = text.Substring(startPos, 2).ToCharArray();
            if (byteFormat == ByteFormat.Intel)
            {
                return ToShort((byte)tmp[0], (byte)tmp[1]);
            }

            return ToShort((byte)tmp[1], (byte)tmp[0]);
        }

        /// <summary>
        /// From the string to unsigned integer 32.
        /// </summary>
        /// <param name="text">
        /// The text including 4 characters to convert into unsigned integer.
        /// </param>
        /// <param name="startPos">
        /// The start position.
        /// </param>
        /// <param name="byteFormat">
        /// The byte format intel or motorolla.
        /// </param>
        /// <returns>
        /// A unsigned integer including 4 characters from text beginning with start Position.
        /// </returns>
        public static uint FromStringToUInt32(string text, int startPos, ByteFormat byteFormat)
        {
            char[] tmp = text.Substring(startPos, 4).ToCharArray();
            if (byteFormat == ByteFormat.Intel)
            {
                return ToUint((byte)tmp[0], (byte)tmp[1], (byte)tmp[2], (byte)tmp[3]);
            }

            return ToUint((byte)tmp[3], (byte)tmp[2], (byte)tmp[1], (byte)tmp[0]);
        }

        /// <summary>
        /// From the string to unsigned integer 16.
        /// </summary>
        /// <param name="text">
        /// The text including 2 characters to convert into unsigned short.
        /// </param>
        /// <param name="startPos">
        /// The start position.
        /// </param>
        /// <param name="byteFormat">
        /// The byte format intel or motorolla.
        /// </param>
        /// <returns>
        /// A unsigned short including 2 characters from text beginning with start position.
        /// </returns>
        public static ushort FromStringToUInt16(string text, int startPos, ByteFormat byteFormat)
        {
            char[] tmp = text.Substring(startPos, 2).ToCharArray();
            if (byteFormat == ByteFormat.Intel)
            {
                return ToUshort((byte)tmp[0], (byte)tmp[1]);
            }

            return ToUshort((byte)tmp[1], (byte)tmp[0]);
        }

        /// <summary>
        /// Dates the time to BCD.
        /// </summary>
        /// <param name="dateTime">
        /// The dateTime.
        /// </param>
        /// <returns>
        /// The byte array of dateTime.
        /// </returns>
        public static byte[] DateTimeToBCD(DateTime dateTime)
        {
            byte[] data = new byte[8];
            if (dateTime.Year < 1990 || dateTime.Year > 2089)
            {
                throw new Exception("Year must be between 1990 & 2089. Conversion impossible!");
            }

            if (dateTime.Year < 2000)
            {
                data[0] = (byte)(dateTime.Year - 1900);
            }
            else
            {
                data[0] = (byte)(dateTime.Year - 2000);
            }

            data[0] = (byte)((data[0] / 10 * 16) + (data[0] % 10));
            data[1] = (byte)dateTime.Month;
            data[1] = (byte)((data[1] / 10 * 16) + (data[1] % 10));
            data[2] = (byte)dateTime.Day;
            data[2] = (byte)((data[2] / 10 * 16) + (data[2] % 10));
            data[3] = (byte)dateTime.Hour;
            data[3] = (byte)((data[3] / 10 * 16) + (data[3] % 10));
            data[4] = (byte)dateTime.Minute;
            data[4] = (byte)((data[4] / 10 * 16) + (data[4] % 10));
            data[5] = (byte)dateTime.Second;
            data[5] = (byte)((data[5] / 10 * 16) + (data[5] % 10));
            data[6] = (byte)(dateTime.Millisecond / 10); // 2 Hoeherwertige Stellen
            data[6] = (byte)((data[6] / 10 * 16) + (data[6] % 10));
            data[7] = (byte)((dateTime.Millisecond % 10) << 4); // Niederwertige Stelle + Wochentag (1=Sonntag)
            if (dateTime.DayOfWeek == DayOfWeek.Sunday)
            {
                data[7] += 1;
            }

            if (dateTime.DayOfWeek == DayOfWeek.Monday)
            {
                data[7] += 2;
            }

            if (dateTime.DayOfWeek == DayOfWeek.Tuesday)
            {
                data[7] += 3;
            }

            if (dateTime.DayOfWeek == DayOfWeek.Wednesday)
            {
                data[7] += 4;
            }

            if (dateTime.DayOfWeek == DayOfWeek.Thursday)
            {
                data[7] += 5;
            }

            if (dateTime.DayOfWeek == DayOfWeek.Friday)
            {
                data[7] += 6;
            }

            if (dateTime.DayOfWeek == DayOfWeek.Saturday)
            {
                data[7] += 7;
            }

            return data;
        }

        /// <summary>
        /// BCDs to date time.
        /// </summary>
        /// <param name="data">
        /// The data array.
        /// </param>
        /// <returns>
        /// The DateTime value of data.
        /// </returns>
        public static DateTime BCDToDateTime(byte[] data)
        {
            if ((((data[0] / 16) * 10) + (data[0] % 16)) > 99)
            {
                throw new Exception("Invalide value for year! Must be between 0 and 99 (1990 = 90 -- 2089 = 89");
            }

            int year = (((data[0] / 16) * 10) + (data[0] % 16)) >= 90
                           ? (((data[0] / 16) * 10) + (data[0] % 16)) + 1900
                           : (((data[0] / 16) * 10) + (data[0] % 16)) + 2000;
            int month = ((data[1] / 16) * 10) + (data[1] % 16);
            int day = ((data[2] / 16) * 10) + (data[2] % 16);
            int hour = ((data[3] / 16) * 10) + (data[3] % 16);
            int minute = ((data[4] / 16) * 10) + (data[4] % 16);
            int second = ((data[5] / 16) * 10) + (data[5] % 16);
            int millisecond = ((((data[6] / 16) * 10) + (data[6] % 16)) * 10)
                              + ((((data[7] >> 4) / 16) * 10) + ((data[7] >> 4) % 16));
            DateTime d = new DateTime(year, month, day, hour, minute, second, millisecond);
            return d;
        }

        /// <summary>
        /// Encrypts the message.
        /// </summary>
        /// <param name="plainMessage">
        /// The plain message.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// The encrypted password.
        /// </returns>
        public static string EncryptMessage(string plainMessage, string password)
        {
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.IV = new byte[8];
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, new byte[0]);
            des.Key = pdb.CryptDeriveKey("RC2", "MD5", 128, new byte[8]);
            MemoryStream ms = new MemoryStream(plainMessage.Length * 2);
            CryptoStream encStream = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainMessage);
            encStream.Write(plainBytes, 0, plainBytes.Length);
            encStream.FlushFinalBlock();
            byte[] encryptedBytes = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(encryptedBytes, 0, (int)ms.Length);
            encStream.Close();
            return Convert.ToBase64String(encryptedBytes);
        }

        /// <summary>
        /// Decrypts the message.
        /// </summary>
        /// <param name="encryptedBase64">
        /// The encrypted base64.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// The decrypted password.
        /// </returns>
        public static string DecryptMessage(string encryptedBase64, string password)
        {
            if (encryptedBase64 == string.Empty)
            {
                return string.Empty;
            }

            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.IV = new byte[8];
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, new byte[0]);
            des.Key = pdb.CryptDeriveKey("RC2", "MD5", 128, new byte[8]);
            byte[] encryptedBytes = Convert.FromBase64String(encryptedBase64);
            MemoryStream ms = new MemoryStream(encryptedBase64.Length);
            CryptoStream decStream = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            decStream.Write(encryptedBytes, 0, encryptedBytes.Length);
            decStream.FlushFinalBlock();
            byte[] plainBytes = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(plainBytes, 0, (int)ms.Length);
            decStream.Close();

            return Encoding.UTF8.GetString(plainBytes);
        }

        #endregion

        /// <summary>
        /// Finds the next date whose day of the week equals the specified day of the week.
        /// </summary>
        /// <param name="startDate">
        /// The date to begin the search.
        /// </param>
        /// <param name="desiredDay">
        /// The desired day of the week whose date will be returned.
        /// </param>
        /// <returns>
        /// The returned date occurs on the given date's week.
        ///     If the given day occurs before given date, the date for the
        ///     following week's desired day is returned.
        /// </returns>
        public static DateTime GetNextDateForDay(DateTime startDate, DayOfWeek desiredDay)
        {
            // Given a date and day of week,
            // find the next date whose day of the week equals the specified day of the week.
            return startDate.AddDays(DaysToDayOfWeek(startDate.DayOfWeek, desiredDay));
        }

        /// <summary>
        /// Calculates the number of days to add to the given day of
        /// the week in order to return the next occurrence of the
        /// desired day of the week.
        /// </summary>
        /// <param name="current">
        /// The starting day of the week.
        /// </param>
        /// <param name="desired">
        /// The desired day of the week.
        /// </param>
        /// <returns>
        /// The number of days to add to <var>current</var> day of week
        ///     in order to achieve the next <var>desired</var> day of week.
        /// </returns>
        public static int DaysToDayOfWeek(this DayOfWeek current, DayOfWeek desired)
        {
            // f( c, d ) = [7 - (c - d)] mod 7
            // where 0 <= c < 7 and 0 <= d < 7
            int c = (int)current;
            int d = (int)desired;
            int n = 7 - c + d;
            return n % 7;
        }

        /// <summary>
        /// Hexadecimal strings to byte array.
        /// </summary>
        /// <param name="str">The hex string. </param>
        /// <returns>returns a byte array.</returns>
        public static byte[] StrHexToByteArray(string str)
        {
            Dictionary<string, byte> hexindex = new Dictionary<string, byte>();
            for (byte i = 0; i < 255; i++)
            {
                hexindex.Add(i.ToString("X2"), i);
            }

            List<byte> hexres = new List<byte>();
            for (int i = 0; i < str.Length; i += 2)
            {
                hexres.Add(hexindex[str.Substring(i, 2)]);
            }

            return hexres.ToArray();
        }

        /// <summary>
        /// Try Parse using Reflection.
        /// </summary>
        /// <typeparam name="T">The expected type the string should be converted to. </typeparam>
        /// <param name="stringValue">The string value.</param>
        /// <param name="convertedValue">The converted value.</param>
        /// <returns>true, if the conversion was successful.</returns>
        public static bool TryConvertValue<T>(string stringValue, out T convertedValue)
        {
            Type targetType = typeof(T);

            if (targetType == typeof(string))
            {
                convertedValue = (T)Convert.ChangeType(stringValue, typeof(T));
                return true;
            }

            if (targetType.IsEnum)
            {
                try
                {
                    convertedValue = (T)Enum.Parse(targetType, stringValue);
                    return true;
                }
                catch
                {
                    convertedValue = default(T);
                    return false;
                }
            }

            if (targetType.FullName == typeof(MailAddress).FullName)
            {
                try
                {
                    convertedValue = (T)Convert.ChangeType(new MailAddress(stringValue), typeof(MailAddress));
                    return true;
                }
                catch
                {
                    convertedValue = default(T);
                    return false;
                }
            }

            if (targetType.FullName == typeof(IPAddress).FullName)
            {
                System.Net.IPAddress test = System.Net.IPAddress.None;
                if (System.Net.IPAddress.TryParse(stringValue, out test) == true)
                {
                    convertedValue = (T)Convert.ChangeType(test, typeof(IPAddress));
                    return true;
                }
                else
                {
                    try
                    {
                        System.Net.IPHostEntry entry = System.Net.Dns.GetHostEntry(stringValue);
                        if (entry != null && entry.AddressList.Length > 0)
                        {
                            convertedValue = (T)Convert.ChangeType(entry.AddressList[0], typeof(IPAddress));
                            return true;
                        }
                        else
                        {
                            convertedValue = default(T);
                            return false;
                        }
                    }
                    catch
                    {
                        convertedValue = default(T);
                        return false;
                    }
                }
            }

            var nullableType = targetType.IsGenericType &&
                               targetType.GetGenericTypeDefinition() == typeof(Nullable<>);
            if (nullableType)
            {
                if (string.IsNullOrEmpty(stringValue))
                {
                    convertedValue = default(T);
                    return true;
                }

                targetType = new NullableConverter(targetType).UnderlyingType;
            }

            Type[] argTypes = { typeof(string), targetType.MakeByRefType() };
            var tryParseMethodInfo = targetType.GetMethod("TryParse", argTypes);
            if (tryParseMethodInfo == null)
            {
                convertedValue = default(T);
                return false;
            }

            object[] args = { stringValue, null };
            var successfulParse = (bool)tryParseMethodInfo.Invoke(null, args);
            if (!successfulParse)
            {
                convertedValue = default(T);
                return false;
            }

            convertedValue = (T)args[1];
            return true;
        }

        /// <summary>
        /// Gets all complete sequences between initiator and terminator.
        /// Also provides an index to the first byte that should be preserved (see <paramref name="nextIndex"/>).
        /// </summary>
        /// <param name="data">The the array that contains the data. It is not modified.</param>
        /// <param name="initiator">The initiator byte. Must be different from. <paramref name="terminator"/></param>
        /// <param name="terminator">The terminator byte. Must be different from. <paramref name="initiator"/></param>
        /// <param name="nextIndex">Index of the next byte. It will point to:
        /// - the index of the last initiator of the last unterminated sequence
        /// - the index of the first byte after the last terminated sequence
        /// - a (virtual) index that equals the length of <paramref name="data"/> in case
        ///   + <paramref name="data"/> ends with a terminator
        ///   + OR there are no started sequences at all.
        /// </param>
        /// <returns>A List of byte[] representing single telegrams.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="data"/> is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="initiator"/> and <paramref name="terminator"/> are equal.</exception>
        public static List<byte[]> GetCompleteSequences(byte[] data, byte initiator, byte terminator, out int nextIndex)
        {
            // initialize out value
            nextIndex = 0;

            if (data == null)
            {
                throw new ArgumentNullException();
            }

            if (initiator == terminator)
            {
                throw new ArgumentException("Start and end marker must be different.");
            }

            List<byte[]> result = new List<byte[]>();

            // stores the indices of the last found markers
            int lastStartIndex = -1;
            int lastEndIndex = -1;

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == initiator)
                {
                    lastStartIndex = i;
                }
                else if (data[i] == terminator)
                {
                    // end marker is only relevant after a start marker
                    if (lastStartIndex >= 0 && lastStartIndex < i)
                    {
                        lastEndIndex = i;
                        int length = lastEndIndex - lastStartIndex - 1;
                        if (length > 0)
                        {
                            // store the found sequence in return list (omitting the markers)
                            byte[] foundSequence = new byte[length];
                            Array.Copy(data, lastStartIndex + 1, foundSequence, 0, length);
                            result.Add(foundSequence);
                        }

                        // invalidate indices to indicate that the current sequence has been terminated
                        lastStartIndex = -1;
                        lastEndIndex = -1;
                    }
                }
            }

            if (lastStartIndex < 0)
            {
                // no start was found or last sequence was terminated -> entire array can be discarded
                nextIndex = data.Length;
            }
            else if (lastEndIndex < 0)
            {
                // a start marker without corresponding end marker was found at the end of the data -> return index of last start
                nextIndex = lastStartIndex;
            }
            else if (lastStartIndex < lastEndIndex)
            {
                // last sequence was terminated
                nextIndex = data.Length;
            }
            else if (lastStartIndex > lastEndIndex)
            {
                // last sequence was not terminated, preserve from last start index on
                nextIndex = lastStartIndex;
            }

            return result;
        }
    }
}
