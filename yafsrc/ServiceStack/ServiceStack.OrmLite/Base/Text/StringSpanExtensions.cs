// ***********************************************************************
// <copyright file="StringSpanExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Helpful extensions on ReadOnlySpan&lt;char&gt;
/// Previous extensions on StringSegment available from: https://gist.github.com/mythz/9825689f0db7464d1d541cb62954614c
/// </summary>
public static class StringSpanExtensions
{
    /// <summary>
    /// Returns null if Length == 0, string.Empty if value[0] == NonWidthWhitespace, otherwise returns value.ToString()
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Value(this ReadOnlySpan<char> value)
    {
        return value.IsEmpty
            ? null
            : value.Length == 1 && value[0] == TypeConstants.NonWidthWhiteSpace
                ? ""
                : value.ToString();
    }

    /// <summary>
    /// Values the specified object.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static internal object Value(this object obj)
    {
        return obj is string { Length: 1 } value && value[0] == TypeConstants.NonWidthWhiteSpace
            ? ""
            : obj;
    }

    /// <param name="value">The value.</param>
    extension(ReadOnlySpan<char> value)
    {
        /// <summary>
        /// Determines whether [is null or empty] [the specified value].
        /// </summary>
        /// <returns><c>true</c> if [is null or empty] [the specified value]; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNullOrEmpty()
        {
            return value.IsEmpty || value.Length == 1 && value[0] == TypeConstants.NonWidthWhiteSpace;
        }

        /// <summary>
        /// Substrings the specified position.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <returns>System.String.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string Substring(int pos)
        {
            return value.Slice(pos, value.Length - pos).ToString();
        }

        /// <summary>
        /// Substrings the specified position.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="length">The length.</param>
        /// <returns>System.String.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string Substring(int pos, int length)
        {
            return value.Slice(pos, length).ToString();
        }

        /// <summary>
        /// Compares the ignore case.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CompareIgnoreCase(ReadOnlySpan<char> text)
        {
            return value.Equals(text, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Froms the CSV field.
        /// </summary>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        public ReadOnlySpan<char> FromCsvField()
        {
            //TODO replace with native Replace() when exists
            if (value.IsNullOrEmpty())
            {
                return value;
            }

            var delim = CsvConfig.ItemDelimiterString;
            if (delim.Length == 1)
            {
                if (value[0] != delim[0])
                {
                    return value;
                }
            }
            else if (!value.StartsWith(delim.AsSpan(), StringComparison.Ordinal))
            {
                return value;
            }

            var ret = value.Slice(CsvConfig.ItemDelimiterString.Length, value.Length - CsvConfig.EscapedItemDelimiterString.Length)
                .ToString().Replace(CsvConfig.EscapedItemDelimiterString, CsvConfig.ItemDelimiterString);

            return ret == string.Empty ? TypeConstants.EmptyStringSpan : ret.AsSpan();
        }

        /// <summary>
        /// Parses the boolean.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ParseBoolean()
        {
            //Lots of kids like to use '1', HTML checkboxes use 'on' as a soft convention
            switch (value.Length)
            {
                case 0:
                    return false;
                case 1:
                    switch (value[0])
                    {
                        case '1':
                        case 't':
                        case 'T':
                        case 'y':
                        case 'Y':
                            return true;
                        case '0':
                        case 'f':
                        case 'F':
                        case 'n':
                        case 'N':
                            return false;
                    }
                    break;
                case 2:
                    if (value[0] == 'o' && value[1] == 'n')
                    {
                        return true;
                    }

                    break;
                case 3:
                    if (value[0] == 'o' && value[1] == 'f' && value[1] == 'f')
                    {
                        return false;
                    }

                    break;
            }

            return MemoryProvider.Instance.ParseBoolean(value);
        }

        /// <summary>
        /// Tries the parse boolean.
        /// </summary>
        /// <param name="result">if set to <c>true</c> [result].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool TryParseBoolean(out bool result)
        {
            return MemoryProvider.Instance.TryParseBoolean(value, out result);
        }

        /// <summary>
        /// Tries the parse decimal.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryParseDecimal(out decimal result)
        {
            return MemoryProvider.Instance.TryParseDecimal(value, out result);
        }

        /// <summary>
        /// Tries the parse float.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryParseFloat(out float result)
        {
            return MemoryProvider.Instance.TryParseFloat(value, out result);
        }

        /// <summary>
        /// Tries the parse double.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryParseDouble(out double result)
        {
            return MemoryProvider.Instance.TryParseDouble(value, out result);
        }

        /// <summary>
        /// Parses the decimal.
        /// </summary>
        /// <returns>System.Decimal.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public decimal ParseDecimal()
        {
            return MemoryProvider.Instance.ParseDecimal(value);
        }

        /// <summary>
        /// Parses the float.
        /// </summary>
        /// <returns>System.Single.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ParseFloat()
        {
            return MemoryProvider.Instance.ParseFloat(value);
        }

        /// <summary>
        /// Parses the double.
        /// </summary>
        /// <returns>System.Double.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ParseDouble()
        {
            return MemoryProvider.Instance.ParseDouble(value);
        }

        /// <summary>
        /// Parses the int32.
        /// </summary>
        /// <returns>System.Int32.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ParseInt32()
        {
            return SignedInteger<int>.ParseInt32(value);
        }

        /// <summary>
        /// Parses the unique identifier.
        /// </summary>
        /// <returns>Guid.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Guid ParseGuid()
        {
            return DefaultMemory.Provider.ParseGuid(value);
        }

        /// <summary>
        /// Advances the past whitespace.
        /// </summary>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<char> AdvancePastWhitespace()
        {
            var i = 0;
            while (i < value.Length && char.IsWhiteSpace(value[i]))
            {
                i++;
            }

            return i == 0 ? value : value.Slice(i < value.Length ? i : value.Length);
        }

        /// <summary>
        /// Rights the part.
        /// </summary>
        /// <param name="needle">The needle.</param>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        public ReadOnlySpan<char> RightPart(char needle)
        {
            if (value.IsEmpty)
            {
                return value;
            }

            var pos = value.IndexOf(needle);
            return pos == -1
                ? value
                : value.Slice(pos + 1);
        }

        /// <summary>
        /// Rights the part.
        /// </summary>
        /// <param name="needle">The needle.</param>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        public ReadOnlySpan<char> RightPart(string needle)
        {
            if (value.IsEmpty)
            {
                return value;
            }

            var pos = value.IndexOf(needle);
            return pos == -1
                ? value
                : value.Slice(pos + needle.Length);
        }

        /// <summary>
        /// Lasts the right part.
        /// </summary>
        /// <param name="needle">The needle.</param>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        public ReadOnlySpan<char> LastRightPart(char needle)
        {
            if (value.IsEmpty)
            {
                return value;
            }

            var pos = value.LastIndexOf(needle);
            return pos == -1
                ? value
                : value.Slice(pos + 1);
        }

        /// <summary>
        /// Lasts the right part.
        /// </summary>
        /// <param name="needle">The needle.</param>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        public ReadOnlySpan<char> LastRightPart(string needle)
        {
            if (value.IsEmpty)
            {
                return value;
            }

            var pos = value.LastIndexOf(needle);
            return pos == -1
                ? value
                : value.Slice(pos + needle.Length);
        }

        /// <summary>
        /// Splits the on first.
        /// </summary>
        /// <param name="needle">The needle.</param>
        /// <param name="first">The first.</param>
        /// <param name="last">The last.</param>
        public void SplitOnFirst(char needle, out ReadOnlySpan<char> first, out ReadOnlySpan<char> last)
        {
            first = default;
            last = default;
            if (value.IsEmpty)
            {
                return;
            }

            var pos = value.IndexOf(needle);
            if (pos == -1)
            {
                first = value;
            }
            else
            {
                first = value.Slice(0, pos);
                last = value.Slice(pos + 1);
            }
        }

        /// <summary>
        /// Splits the on first.
        /// </summary>
        /// <param name="needle">The needle.</param>
        /// <param name="first">The first.</param>
        /// <param name="last">The last.</param>
        public void SplitOnFirst(string needle, out ReadOnlySpan<char> first, out ReadOnlySpan<char> last)
        {
            first = default;
            last = default;
            if (value.IsEmpty)
            {
                return;
            }

            var pos = value.IndexOf(needle);
            if (pos == -1)
            {
                first = value;
            }
            else
            {
                first = value.Slice(0, pos);
                last = value.Slice(pos + needle.Length);
            }
        }

        /// <summary>
        /// Splits the on last.
        /// </summary>
        /// <param name="needle">The needle.</param>
        /// <param name="first">The first.</param>
        /// <param name="last">The last.</param>
        public void SplitOnLast(char needle, out ReadOnlySpan<char> first, out ReadOnlySpan<char> last)
        {
            first = default;
            last = default;
            if (value.IsEmpty)
            {
                return;
            }

            var pos = value.LastIndexOf(needle);
            if (pos == -1)
            {
                first = value;
            }
            else
            {
                first = value.Slice(0, pos);
                last = value.Slice(pos + 1);
            }
        }

        /// <summary>
        /// Splits the on last.
        /// </summary>
        /// <param name="needle">The needle.</param>
        /// <param name="first">The first.</param>
        /// <param name="last">The last.</param>
        public void SplitOnLast(string needle, out ReadOnlySpan<char> first, out ReadOnlySpan<char> last)
        {
            first = default;
            last = default;
            if (value.IsEmpty)
            {
                return;
            }

            var pos = value.LastIndexOf(needle);
            if (pos == -1)
            {
                first = value;
            }
            else
            {
                first = value.Slice(0, pos);
                last = value.Slice(pos + needle.Length);
            }
        }

        /// <summary>
        /// Withouts the extension.
        /// </summary>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        public ReadOnlySpan<char> WithoutExtension()
        {
            if (value.IsNullOrEmpty())
            {
                return TypeConstants.NullStringSpan;
            }

            var extPos = value.LastIndexOf('.');
            if (extPos == -1)
            {
                return value;
            }

            var dirPos = value.LastIndexOfAny(PclExport.DirSeps);
            return extPos > dirPos ? value.Slice(0, extPos) : value;
        }

        /// <summary>
        /// Gets the extension.
        /// </summary>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        public ReadOnlySpan<char> GetExtension()
        {
            if (value.IsNullOrEmpty())
            {
                return TypeConstants.NullStringSpan;
            }

            var extPos = value.LastIndexOf('.');
            return extPos == -1 ? TypeConstants.NullStringSpan : value.Slice(extPos);
        }

        /// <summary>
        /// Parents the directory.
        /// </summary>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        public ReadOnlySpan<char> ParentDirectory()
        {
            if (value.IsNullOrEmpty())
            {
                return TypeConstants.NullStringSpan;
            }

            var dirSep = value.IndexOf(PclExport.Instance.DirSep) != -1
                ? PclExport.Instance.DirSep
                : value.IndexOf(PclExport.Instance.AltDirSep) != -1
                    ? PclExport.Instance.AltDirSep
                    : (char)0;

            if (dirSep == 0)
            {
                return TypeConstants.NullStringSpan;
            }

            ReadOnlySpan<char> first;
            value.TrimEnd(dirSep).SplitOnLast(dirSep, out first, out _);

            return first;
        }

        /// <summary>
        /// Trims the end.
        /// </summary>
        /// <param name="trimChars">The trim chars.</param>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        public ReadOnlySpan<char> TrimEnd(params char[] trimChars)
        {
            if (value.IsEmpty)
            {
                return TypeConstants.NullStringSpan;
            }

            if (trimChars == null || trimChars.Length == 0)
            {
                return value.TrimHelper(1);
            }

            return value.TrimHelper(trimChars, 1);
        }

        /// <summary>
        /// Trims the helper.
        /// </summary>
        /// <param name="trimType">Type of the trim.</param>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        private ReadOnlySpan<char> TrimHelper(int trimType)
        {
            if (value.IsEmpty)
            {
                return TypeConstants.NullStringSpan;
            }

            var end = value.Length - 1;
            var start = 0;
            if (trimType != 1)
            {
                start = 0;
                while (start < value.Length && char.IsWhiteSpace(value[start]))
                {
                    ++start;
                }
            }
            if (trimType != 0)
            {
                end = value.Length - 1;
                while (end >= start && char.IsWhiteSpace(value[end]))
                {
                    --end;
                }
            }
            return value.CreateTrimmedString(start, end);
        }

        /// <summary>
        /// Trims the helper.
        /// </summary>
        /// <param name="trimChars">The trim chars.</param>
        /// <param name="trimType">Type of the trim.</param>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        private ReadOnlySpan<char> TrimHelper(IReadOnlyList<char> trimChars, int trimType)
        {
            if (value.IsEmpty)
            {
                return TypeConstants.NullStringSpan;
            }
            var end = value.Length - 1;
            var start = 0;
            if (trimType != 1)
            {
                for (start = 0; start < value.Length; ++start)
                {
                    var ch = value[start];
                    var index = 0;
                    while (index < trimChars.Count && trimChars[index] != ch)
                    {
                        ++index;
                    }

                    if (index == trimChars.Count)
                    {
                        break;
                    }
                }
            }
            if (trimType != 0)
            {
                for (end = value.Length - 1; end >= start; --end)
                {
                    var ch = value[end];
                    var index = 0;
                    while (index < trimChars.Count && trimChars[index] != ch)
                    {
                        ++index;
                    }

                    if (index == trimChars.Count)
                    {
                        break;
                    }
                }
            }
            return value.CreateTrimmedString(start, end);
        }

        /// <summary>
        /// Creates the trimmed string.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        private ReadOnlySpan<char> CreateTrimmedString(int start, int end)
        {
            if (value.IsEmpty)
            {
                return TypeConstants.NullStringSpan;
            }

            var length = end - start + 1;
            if (length == value.Length)
            {
                return value;
            }

            return length == 0 ? TypeConstants.NullStringSpan : value.Slice(start, length);
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>System.Int32.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(string other)
        {
            return value.IndexOf(other.AsSpan());
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="needle">The needle.</param>
        /// <param name="start">The start.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <returns>int.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(string needle, int start, StringComparison comparisonType = StringComparison.Ordinal)
        {
            var pos = value[start..].IndexOf(needle.AsSpan(), comparisonType);
            return pos == -1 ? -1 : start + pos;
        }

        /// <summary>
        /// Lasts the index of.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>System.Int32.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(string other)
        {
            return value.LastIndexOf(other.AsSpan());
        }

        /// <summary>
        /// Lasts the index of.
        /// </summary>
        /// <param name="needle">The needle.</param>
        /// <param name="start">The start.</param>
        /// <returns>System.Int32.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(string needle, int start)
        {
            var pos = value.Slice(start).LastIndexOf(needle.AsSpan());
            return pos == -1 ? -1 : start + pos;
        }

        /// <summary>
        /// Equals to.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EqualTo(string other)
        {
            return value.Equals(other.AsSpan(), StringComparison.Ordinal);
        }

        /// <summary>
        /// Equals to.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EqualTo(ReadOnlySpan<char> other)
        {
            return value.Equals(other, StringComparison.Ordinal);
        }

        /// <summary>
        /// Equalses the ordinal.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EqualsOrdinal(string other)
        {
            return value.Equals(other.AsSpan(), StringComparison.Ordinal);
        }

        /// <summary>
        /// Startses the with.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool StartsWith(string other)
        {
            return value.StartsWith(other.AsSpan(), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Startses the with.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <param name="comparison">The comparison.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool StartsWith(string other, StringComparison comparison)
        {
            return value.StartsWith(other.AsSpan(), comparison);
        }

        /// <summary>
        /// Endses the with.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <param name="comparison">The comparison.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EndsWith(string other, StringComparison comparison)
        {
            return value.EndsWith(other.AsSpan(), comparison);
        }

        /// <summary>
        /// Endses the with.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EndsWith(string other)
        {
            return value.EndsWith(other.AsSpan(), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Equalses the ignore case.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EqualsIgnoreCase(ReadOnlySpan<char> other)
        {
            return value.Equals(other, StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// Writes the asynchronous.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="value">The value.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task WriteAsync(this Stream stream, ReadOnlySpan<char> value, CancellationToken token = default)
    {
        return MemoryProvider.Instance.WriteAsync(stream, value, token);
    }

    /// <param name="value">The value.</param>
    extension(ReadOnlySpan<char> value)
    {
        /// <summary>
        /// Safes the substring.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<char> SafeSubstring(int startIndex)
        {
            return SafeSubstring(value, startIndex, value.Length);
        }

        /// <summary>
        /// Safes the substring.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The length.</param>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        public ReadOnlySpan<char> SafeSubstring(int startIndex, int length)
        {
            if (value.IsEmpty)
            {
                return TypeConstants.NullStringSpan;
            }

            if (startIndex < 0)
            {
                startIndex = 0;
            }

            if (value.Length >= startIndex + length)
            {
                return value.Slice(startIndex, length);
            }

            return value.Length > startIndex ? value.Slice(startIndex) : TypeConstants.NullStringSpan;
        }
    }

    /// <summary>
    /// Appends the specified value.
    /// </summary>
    /// <param name="sb">The sb.</param>
    /// <param name="value">The value.</param>
    /// <returns>StringBuilder.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder Append(this StringBuilder sb, ReadOnlySpan<char> value)
    {
        return MemoryProvider.Instance.Append(sb, value);
    }

    /// <param name="value">The value.</param>
    extension(ReadOnlySpan<char> value)
    {
        /// <summary>
        /// Parses the base64.
        /// </summary>
        /// <returns>System.Byte[].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ParseBase64()
        {
            return MemoryProvider.Instance.ParseBase64(value);
        }

        /// <summary>
        /// Withouts the bom.
        /// </summary>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        public ReadOnlySpan<char> WithoutBom()
        {
            return value.Length > 0 && value[0] == 65279
                ? value.Slice(1)
                : value;
        }
    }

    /// <summary>
    /// Withouts the bom.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>ReadOnlySpan&lt;System.Byte&gt;.</returns>
    public static ReadOnlySpan<byte> WithoutBom(this ReadOnlySpan<byte> value)
    {
        return value.Length > 3 && value[0] == 0xEF && value[1] == 0xBB && value[2] == 0xBF
                   ? value.Slice(3)
                   : value;
    }

}