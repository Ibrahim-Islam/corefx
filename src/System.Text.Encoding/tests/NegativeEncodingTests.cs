﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Text.Tests
{
    public static class NegativeEncodingTests
    {
        public static IEnumerable<object[]> Encodings_TestData()
        {
            yield return new object[] { new UnicodeEncoding(false, false) };
            yield return new object[] { new UnicodeEncoding(true, false) };
            yield return new object[] { new UnicodeEncoding(true, true) };
            yield return new object[] { new UnicodeEncoding(true, true) };
            yield return new object[] { new UTF7Encoding() };
            yield return new object[] { new UTF8Encoding(true) };
            yield return new object[] { new UTF8Encoding(false) };
            yield return new object[] { new ASCIIEncoding() };
        }
        
        [Theory]
        [MemberData(nameof(Encodings_TestData))]
        public static unsafe void GetByteCount_Invalid(Encoding encoding)
        {
            // Chars is null
            Assert.Throws<ArgumentNullException>(encoding is ASCIIEncoding ? "chars" : "s", () => encoding.GetByteCount((string)null));
            Assert.Throws<ArgumentNullException>("chars", () => encoding.GetByteCount((char[])null));
            Assert.Throws<ArgumentNullException>("chars", () => encoding.GetByteCount(null, 0, 0));

            // Index or count < 0
            Assert.Throws<ArgumentOutOfRangeException>("index", () => encoding.GetByteCount(new char[3], -1, 0));
            Assert.Throws<ArgumentOutOfRangeException>("count", () => encoding.GetByteCount(new char[3], 0, -1));

            // Index + count > chars.Length
            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoding.GetByteCount(new char[3], 0, 4));
            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoding.GetByteCount(new char[3], 1, 3));
            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoding.GetByteCount(new char[3], 2, 2));
            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoding.GetByteCount(new char[3], 3, 1));
            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoding.GetByteCount(new char[3], 4, 0));

            char[] chars = new char[3];
            fixed (char* pChars = chars)
            {
                char* pCharsLocal = pChars;
                Assert.Throws<ArgumentNullException>("chars", () => encoding.GetByteCount(null, 0));
                Assert.Throws<ArgumentOutOfRangeException>("count", () => encoding.GetByteCount(pCharsLocal, -1));
            }
        }
        
        [Theory]
        [MemberData(nameof(Encodings_TestData))]
        public static unsafe void GetBytes_Invalid(Encoding encoding)
        {
            string expectedStringParamName = encoding is ASCIIEncoding ? "chars" : "s";

            // Source is null
            Assert.Throws<ArgumentNullException>("s", () => encoding.GetBytes((string)null));
            Assert.Throws<ArgumentNullException>("chars", () => encoding.GetBytes((char[])null));
            Assert.Throws<ArgumentNullException>("chars", () => encoding.GetBytes(null, 0, 0));
            Assert.Throws<ArgumentNullException>(expectedStringParamName, () => encoding.GetBytes((string)null, 0, 0, new byte[1], 0));
            Assert.Throws<ArgumentNullException>("chars", () => encoding.GetBytes((char[])null, 0, 0, new byte[1], 0));

            // Char index < 0
            Assert.Throws<ArgumentOutOfRangeException>("index", () => encoding.GetBytes(new char[1], -1, 0));
            Assert.Throws<ArgumentOutOfRangeException>("charIndex", () => encoding.GetBytes("a", -1, 0, new byte[1], 0));
            Assert.Throws<ArgumentOutOfRangeException>("charIndex", () => encoding.GetBytes(new char[1], -1, 0, new byte[1], 0));

            // Char count < 0
            Assert.Throws<ArgumentOutOfRangeException>("count", () => encoding.GetBytes(new char[1], 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>("charCount", () => encoding.GetBytes("a", 0, -1, new byte[1], 0));
            Assert.Throws<ArgumentOutOfRangeException>("charCount", () => encoding.GetBytes(new char[1], 0, -1, new byte[1], 0));

            // Char index + count > source.Length
            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoding.GetBytes(new char[1], 2, 0));
            Assert.Throws<ArgumentOutOfRangeException>(expectedStringParamName, () => encoding.GetBytes("a", 2, 0, new byte[1], 0));
            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoding.GetBytes(new char[1], 2, 0, new byte[1], 0));

            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoding.GetBytes(new char[1], 1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(expectedStringParamName, () => encoding.GetBytes("a", 1, 1, new byte[1], 0));
            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoding.GetBytes(new char[1], 1, 1, new byte[1], 0));

            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoding.GetBytes(new char[1], 0, 2));
            Assert.Throws<ArgumentOutOfRangeException>(expectedStringParamName, () => encoding.GetBytes("a", 0, 2, new byte[1], 0));
            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoding.GetBytes(new char[1], 0, 2, new byte[1], 0));

            // Byte index < 0
            Assert.Throws<ArgumentOutOfRangeException>("byteIndex", () => encoding.GetBytes("a", 0, 1, new byte[1], -1));
            Assert.Throws<ArgumentOutOfRangeException>("byteIndex", () => encoding.GetBytes(new char[1], 0, 1, new byte[1], -1));

            // Byte index > bytes.Length
            Assert.Throws<ArgumentOutOfRangeException>("byteIndex", () => encoding.GetBytes("a", 0, 1, new byte[1], 2));
            Assert.Throws<ArgumentOutOfRangeException>("byteIndex", () => encoding.GetBytes(new char[1], 0, 1, new byte[1], 2));

            // Bytes does not have enough capacity to accomodate result
            Assert.Throws<ArgumentException>("bytes", () => encoding.GetBytes("abc", 0, 3, new byte[1], 0));
            Assert.Throws<ArgumentException>("bytes", () => encoding.GetBytes(new char[3], 0, 3, new byte[1], 0));

            char[] chars = new char[3];
            byte[] bytes = new byte[3];
            byte[] smallBytes = new byte[1];
            fixed (char* pChars = chars)
            fixed (byte* pBytes = bytes)
            fixed (byte* pSmallBytes = smallBytes)
            {
                char* pCharsLocal = pChars;
                byte* pBytesLocal = pBytes;
                byte* pSmallBytesLocal = pSmallBytes;

                // Bytes or chars is null
                Assert.Throws<ArgumentNullException>("chars", () => encoding.GetBytes((char*)null, 0, pBytesLocal, bytes.Length));
                Assert.Throws<ArgumentNullException>("bytes", () => encoding.GetBytes(pCharsLocal, chars.Length, (byte*)null, bytes.Length));

                // CharCount or byteCount is negative
                Assert.Throws<ArgumentOutOfRangeException>("charCount", () => encoding.GetBytes(pCharsLocal, -1, pBytesLocal, bytes.Length));
                Assert.Throws<ArgumentOutOfRangeException>("byteCount", () => encoding.GetBytes(pCharsLocal, chars.Length, pBytesLocal, -1));

                // Bytes does not have enough capacity to accomodate result
                Assert.Throws<ArgumentException>("bytes", () => encoding.GetBytes(pCharsLocal, chars.Length, pSmallBytesLocal, smallBytes.Length));
            }
        }

        [Theory]
        [MemberData(nameof(Encodings_TestData))]
        public static unsafe void GetCharCount_Invalid(Encoding encoding)
        {
            // Bytes is null
            Assert.Throws<ArgumentNullException>("bytes", () => encoding.GetCharCount(null));
            Assert.Throws<ArgumentNullException>("bytes", () => encoding.GetCharCount(null, 0, 0));

            // Index or count < 0
            Assert.Throws<ArgumentOutOfRangeException>("index", () => encoding.GetCharCount(new byte[4], -1, 0));
            Assert.Throws<ArgumentOutOfRangeException>("count", () => encoding.GetCharCount(new byte[4], 0, -1));

            // Index + count > bytes.Length
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetCharCount(new byte[4], 5, 0));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetCharCount(new byte[4], 4, 1));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetCharCount(new byte[4], 3, 2));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetCharCount(new byte[4], 2, 3));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetCharCount(new byte[4], 1, 4));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetCharCount(new byte[4], 0, 5));

            byte[] bytes = new byte[4];
            fixed (byte* pBytes = bytes)
            {
                byte* pBytesLocal = pBytes;
                Assert.Throws<ArgumentNullException>("bytes", () => encoding.GetCharCount(null, 0));
                Assert.Throws<ArgumentOutOfRangeException>("count", () => encoding.GetCharCount(pBytesLocal, -1));
            }
        }

        [Theory]
        [MemberData(nameof(Encodings_TestData))]
        public static unsafe void GetChars_Invalid(Encoding encoding)
        {
            // Bytes is null
            Assert.Throws<ArgumentNullException>("bytes", () => encoding.GetChars(null));
            Assert.Throws<ArgumentNullException>("bytes", () => encoding.GetChars(null, 0, 0));
            Assert.Throws<ArgumentNullException>("bytes", () => encoding.GetChars(null, 0, 0, new char[0], 0));

            // Chars is null
            Assert.Throws<ArgumentNullException>("chars", () => encoding.GetChars(new byte[4], 0, 4, null, 0));

            // Index < 0
            Assert.Throws<ArgumentOutOfRangeException>("index", () => encoding.GetChars(new byte[4], -1, 4));
            Assert.Throws<ArgumentOutOfRangeException>("byteIndex", () => encoding.GetChars(new byte[4], -1, 4, new char[1], 0));

            // Count < 0
            Assert.Throws<ArgumentOutOfRangeException>("count", () => encoding.GetChars(new byte[4], 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>("byteCount", () => encoding.GetChars(new byte[4], 0, -1, new char[1], 0));

            // Count > bytes.Length
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetChars(new byte[4], 0, 5));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetChars(new byte[4], 0, 5, new char[1], 0));

            // Index + count > bytes.Length
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetChars(new byte[4], 5, 0));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetChars(new byte[4], 5, 0, new char[1], 0));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetChars(new byte[4], 4, 1));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetChars(new byte[4], 4, 1, new char[1], 0));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetChars(new byte[4], 3, 2));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetChars(new byte[4], 3, 2, new char[1], 0));

            // CharIndex < 0 or >= chars.Length
            Assert.Throws<ArgumentOutOfRangeException>("charIndex", () => encoding.GetChars(new byte[4], 0, 4, new char[1], -1));
            Assert.Throws<ArgumentOutOfRangeException>("charIndex", () => encoding.GetChars(new byte[4], 0, 4, new char[1], 2));

            // Chars does not have enough capacity to accomodate result
            Assert.Throws<ArgumentException>("chars", () => encoding.GetChars(new byte[4], 0, 4, new char[1], 1));

            byte[] bytes = new byte[4];
            char[] chars = new char[4];
            char[] smallChars = new char[1];
            fixed (byte* pBytes = bytes)
            fixed (char* pChars = chars)
            fixed (char* pSmallChars = smallChars)
            {
                byte* pBytesLocal = pBytes;
                char* pCharsLocal = pChars;
                char* pSmallCharsLocal = pSmallChars;

                // Bytes or chars is null
                Assert.Throws<ArgumentNullException>("bytes", () => encoding.GetChars((byte*)null, 0, pCharsLocal, chars.Length));
                Assert.Throws<ArgumentNullException>("chars", () => encoding.GetChars(pBytesLocal, bytes.Length, (char*)null, chars.Length));

                // ByteCount or charCount is negative
                Assert.Throws<ArgumentOutOfRangeException>("byteCount", () => encoding.GetChars(pBytesLocal, -1, pCharsLocal, chars.Length));
                Assert.Throws<ArgumentOutOfRangeException>("charCount", () => encoding.GetChars(pBytesLocal, bytes.Length, pCharsLocal, -1));

                // Chars does not have enough capacity to accomodate result
                Assert.Throws<ArgumentException>("chars", () => encoding.GetChars(pBytesLocal, bytes.Length, pSmallCharsLocal, smallChars.Length));
            }
        }

        [Theory]
        [MemberData(nameof(Encodings_TestData))]
        public static void GetMaxByteCount_Invalid(Encoding encoding)
        {
            Assert.Throws<ArgumentOutOfRangeException>("charCount", () => encoding.GetMaxByteCount(-1));
            if (!(encoding is ASCIIEncoding))
            {
                Assert.Throws<ArgumentOutOfRangeException>("charCount", () => encoding.GetMaxByteCount(int.MaxValue / 2));
            }
            Assert.Throws<ArgumentOutOfRangeException>("charCount", () => encoding.GetMaxByteCount(int.MaxValue));
        }

        [Theory]
        [MemberData(nameof(Encodings_TestData))]
        public static void GetMaxCharCount_Invalid(Encoding encoding)
        {
            Assert.Throws<ArgumentOutOfRangeException>("byteCount", () => encoding.GetMaxCharCount(-1));

            // TODO: find a more generic way to find what byteCount is invalid
            if (encoding is UTF8Encoding)
            {
                Assert.Throws<ArgumentOutOfRangeException>("byteCount", () => encoding.GetMaxCharCount(int.MaxValue));
            }
        }

        [Theory]
        [MemberData(nameof(Encodings_TestData))]
        public static void GetString_Invalid(Encoding encoding)
        {
            // Bytes is null
            Assert.Throws<ArgumentNullException>("bytes", () => encoding.GetString(null));
            Assert.Throws<ArgumentNullException>("bytes", () => encoding.GetString(null, 0, 0));

            // Index or count < 0
            Assert.Throws<ArgumentOutOfRangeException>(encoding is ASCIIEncoding ? "byteIndex" : "index", () => encoding.GetString(new byte[1], -1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(encoding is ASCIIEncoding ? "byteCount" : "count", () => encoding.GetString(new byte[1], 0, -1));

            // Index + count > bytes.Length
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetString(new byte[1], 2, 0));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetString(new byte[1], 1, 1));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetString(new byte[1], 0, 2));
        }
    }
}
