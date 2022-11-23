// ***********************************************************************
// <copyright file="StringSpanExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Text;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
    public static string Value(this ReadOnlySpan<char> value) => value.IsEmpty
                                                                     ? null
                                                                     : value.Length == 1 && value[0] == TypeConstants.NonWidthWhiteSpace
                                                                         ? ""
                                                                         : value.ToString();

    /// <summary>
    /// Values the specified object.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static object Value(this object obj) =>
        obj is string { Length: 1 } value && value[0] == TypeConstants.NonWidthWhiteSpace
            ? ""
            : obj;

    /// <summary>
    /// Determines whether [is null or empty] [the specified value].
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if [is null or empty] [the specified value]; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty(this ReadOnlySpan<char> value) => value.IsEmpty || value.Length == 1 && value[0] == TypeConstants.NonWidthWhiteSpace;

    /// <summary>
    /// Substrings the specified position.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="pos">The position.</param>
    /// <returns>System.String.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Substring(this ReadOnlySpan<char> value, int pos) => value.Slice(pos, value.Length - pos).ToString();

    /// <summary>
    /// Substrings the specified position.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="pos">The position.</param>
    /// <param name="length">The length.</param>
    /// <returns>System.String.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Substring(this ReadOnlySpan<char> value, int pos, int length) => value.Slice(pos, length).ToString();

    /// <summary>
    /// Compares the ignore case.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="text">The text.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CompareIgnoreCase(this ReadOnlySpan<char> value, ReadOnlySpan<char> text) => value.Equals(text, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Froms the CSV field.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public static ReadOnlySpan<char> FromCsvField(this ReadOnlySpan<char> text)
    {
        //TODO replace with native Replace() when exists
        if (text.IsNullOrEmpty())
            return text;

        var delim = CsvConfig.ItemDelimiterString;
        if (delim.Length == 1)
        {
            if (text[0] != delim[0])
                return text;
        }
        else if (!text.StartsWith(delim.AsSpan(), StringComparison.Ordinal))
        {
            return text;
        }

        var ret = text.Slice(CsvConfig.ItemDelimiterString.Length, text.Length - CsvConfig.EscapedItemDelimiterString.Length)
            .ToString().Replace(CsvConfig.EscapedItemDelimiterString, CsvConfig.ItemDelimiterString);

        return ret == string.Empty ? TypeConstants.EmptyStringSpan : ret.AsSpan();
    }

    /// <summary>
    /// Parses the boolean.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ParseBoolean(this ReadOnlySpan<char> value)
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
                    return true;
                break;
            case 3:
                if (value[0] == 'o' && value[1] == 'f' && value[1] == 'f')
                    return false;
                break;
        }

        return MemoryProvider.Instance.ParseBoolean(value);
    }

    /// <summary>
    /// Tries the parse boolean.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">if set to <c>true</c> [result].</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool TryParseBoolean(this ReadOnlySpan<char> value, out bool result) =>
        MemoryProvider.Instance.TryParseBoolean(value, out result);

    /// <summary>
    /// Tries the parse decimal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseDecimal(this ReadOnlySpan<char> value, out decimal result) =>
        MemoryProvider.Instance.TryParseDecimal(value, out result);

    /// <summary>
    /// Tries the parse float.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseFloat(this ReadOnlySpan<char> value, out float result) =>
        MemoryProvider.Instance.TryParseFloat(value, out result);

    /// <summary>
    /// Tries the parse double.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseDouble(this ReadOnlySpan<char> value, out double result) =>
        MemoryProvider.Instance.TryParseDouble(value, out result);

    /// <summary>
    /// Parses the decimal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Decimal.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal ParseDecimal(this ReadOnlySpan<char> value) =>
        MemoryProvider.Instance.ParseDecimal(value);

    /// <summary>
    /// Parses the float.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Single.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ParseFloat(this ReadOnlySpan<char> value) =>
        MemoryProvider.Instance.ParseFloat(value);

    /// <summary>
    /// Parses the double.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Double.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ParseDouble(this ReadOnlySpan<char> value) =>
        MemoryProvider.Instance.ParseDouble(value);

    /// <summary>
    /// Parses the int32.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Int32.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ParseInt32(this ReadOnlySpan<char> value) =>
        SignedInteger<int>.ParseInt32(value);

    /// <summary>
    /// Parses the int64.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Int64.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ParseInt64(this ReadOnlySpan<char> value) =>
        SignedInteger<long>.ParseInt64(value);

    /// <summary>
    /// Parses the unique identifier.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>Guid.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Guid ParseGuid(this ReadOnlySpan<char> value) =>
        DefaultMemory.Provider.ParseGuid(value);

    /// <summary>
    /// Parses the signed integer.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public static object ParseSignedInteger(this ReadOnlySpan<char> value)
    {
        var longValue = ParseInt64(value);
        if (longValue is >= int.MinValue and <= int.MaxValue)
            return (int)longValue;
        return longValue;
    }

    /// <summary>
    /// Advances the specified to.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="to">To.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<char> Advance(this ReadOnlySpan<char> text, int to) => text.Slice(to, text.Length - to);

    /// <summary>
    /// Advances the past whitespace.
    /// </summary>
    /// <param name="literal">The literal.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<char> AdvancePastWhitespace(this ReadOnlySpan<char> literal)
    {
        var i = 0;
        while (i < literal.Length && char.IsWhiteSpace(literal[i]))
            i++;

        return i == 0 ? literal : literal.Slice(i < literal.Length ? i : literal.Length);
    }

    /// <summary>
    /// Advances the past character.
    /// </summary>
    /// <param name="literal">The literal.</param>
    /// <param name="delim">The delimiter.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public static ReadOnlySpan<char> AdvancePastChar(this ReadOnlySpan<char> literal, char delim)
    {
        var i = 0;
        var c = (char)0;
        while (i < literal.Length && (c = literal[i]) != delim)
            i++;

        if (c == delim)
            return literal.Slice(i + 1);

        return i == 0 ? literal : literal.Slice(i < literal.Length ? i : literal.Length);
    }

    /// <summary>
    /// Lefts the part.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public static ReadOnlySpan<char> LeftPart(this ReadOnlySpan<char> strVal, char needle)
    {
        if (strVal.IsEmpty) return strVal;
        var pos = strVal.IndexOf(needle);
        return pos == -1
                   ? strVal
                   : strVal.Slice(0, pos);
    }

    /// <summary>
    /// Lefts the part.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public static ReadOnlySpan<char> LeftPart(this ReadOnlySpan<char> strVal, string needle)
    {
        if (strVal.IsEmpty) return strVal;
        var pos = strVal.IndexOf(needle);
        return pos == -1
                   ? strVal
                   : strVal.Slice(0, pos);
    }

    /// <summary>
    /// Rights the part.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public static ReadOnlySpan<char> RightPart(this ReadOnlySpan<char> strVal, char needle)
    {
        if (strVal.IsEmpty) return strVal;
        var pos = strVal.IndexOf(needle);
        return pos == -1
                   ? strVal
                   : strVal.Slice(pos + 1);
    }

    /// <summary>
    /// Rights the part.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public static ReadOnlySpan<char> RightPart(this ReadOnlySpan<char> strVal, string needle)
    {
        if (strVal.IsEmpty) return strVal;
        var pos = strVal.IndexOf(needle);
        return pos == -1
                   ? strVal
                   : strVal.Slice(pos + needle.Length);
    }

    /// <summary>
    /// Lasts the left part.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public static ReadOnlySpan<char> LastLeftPart(this ReadOnlySpan<char> strVal, char needle)
    {
        if (strVal.IsEmpty) return strVal;
        var pos = strVal.LastIndexOf(needle);
        return pos == -1
                   ? strVal
                   : strVal.Slice(0, pos);
    }

    /// <summary>
    /// Lasts the left part.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public static ReadOnlySpan<char> LastLeftPart(this ReadOnlySpan<char> strVal, string needle)
    {
        if (strVal.IsEmpty) return strVal;
        var pos = strVal.LastIndexOf(needle);
        return pos == -1
                   ? strVal
                   : strVal.Slice(0, pos);
    }

    /// <summary>
    /// Lasts the right part.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public static ReadOnlySpan<char> LastRightPart(this ReadOnlySpan<char> strVal, char needle)
    {
        if (strVal.IsEmpty) return strVal;
        var pos = strVal.LastIndexOf(needle);
        return pos == -1
                   ? strVal
                   : strVal.Slice(pos + 1);
    }

    /// <summary>
    /// Lasts the right part.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public static ReadOnlySpan<char> LastRightPart(this ReadOnlySpan<char> strVal, string needle)
    {
        if (strVal.IsEmpty) return strVal;
        var pos = strVal.LastIndexOf(needle);
        return pos == -1
                   ? strVal
                   : strVal.Slice(pos + needle.Length);
    }

    /// <summary>
    /// Splits the on first.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <param name="first">The first.</param>
    /// <param name="last">The last.</param>
    public static void SplitOnFirst(this ReadOnlySpan<char> strVal, char needle, out ReadOnlySpan<char> first, out ReadOnlySpan<char> last)
    {
        first = default;
        last = default;
        if (strVal.IsEmpty) return;

        var pos = strVal.IndexOf(needle);
        if (pos == -1)
        {
            first = strVal;
        }
        else
        {
            first = strVal.Slice(0, pos);
            last = strVal.Slice(pos + 1);
        }
    }

    /// <summary>
    /// Splits the on first.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <param name="first">The first.</param>
    /// <param name="last">The last.</param>
    public static void SplitOnFirst(this ReadOnlySpan<char> strVal, string needle, out ReadOnlySpan<char> first, out ReadOnlySpan<char> last)
    {
        first = default;
        last = default;
        if (strVal.IsEmpty) return;

        var pos = strVal.IndexOf(needle);
        if (pos == -1)
        {
            first = strVal;
        }
        else
        {
            first = strVal.Slice(0, pos);
            last = strVal.Slice(pos + needle.Length);
        }
    }

    /// <summary>
    /// Splits the on last.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <param name="first">The first.</param>
    /// <param name="last">The last.</param>
    public static void SplitOnLast(this ReadOnlySpan<char> strVal, char needle, out ReadOnlySpan<char> first, out ReadOnlySpan<char> last)
    {
        first = default;
        last = default;
        if (strVal.IsEmpty) return;

        var pos = strVal.LastIndexOf(needle);
        if (pos == -1)
        {
            first = strVal;
        }
        else
        {
            first = strVal.Slice(0, pos);
            last = strVal.Slice(pos + 1);
        }
    }

    /// <summary>
    /// Splits the on last.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <param name="first">The first.</param>
    /// <param name="last">The last.</param>
    public static void SplitOnLast(this ReadOnlySpan<char> strVal, string needle, out ReadOnlySpan<char> first, out ReadOnlySpan<char> last)
    {
        first = default;
        last = default;
        if (strVal.IsEmpty) return;

        var pos = strVal.LastIndexOf(needle);
        if (pos == -1)
        {
            first = strVal;
        }
        else
        {
            first = strVal.Slice(0, pos);
            last = strVal.Slice(pos + needle.Length);
        }
    }

    /// <summary>
    /// Withouts the extension.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public static ReadOnlySpan<char> WithoutExtension(this ReadOnlySpan<char> filePath)
    {
        if (filePath.IsNullOrEmpty())
            return TypeConstants.NullStringSpan;

        var extPos = filePath.LastIndexOf('.');
        if (extPos == -1) return filePath;

        var dirPos = filePath.LastIndexOfAny(PclExport.DirSeps);
        return extPos > dirPos ? filePath.Slice(0, extPos) : filePath;
    }

    /// <summary>
    /// Gets the extension.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public static ReadOnlySpan<char> GetExtension(this ReadOnlySpan<char> filePath)
    {
        if (filePath.IsNullOrEmpty())
            return TypeConstants.NullStringSpan;

        var extPos = filePath.LastIndexOf('.');
        return extPos == -1 ? TypeConstants.NullStringSpan : filePath.Slice(extPos);
    }

    /// <summary>
    /// Parents the directory.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public static ReadOnlySpan<char> ParentDirectory(this ReadOnlySpan<char> filePath)
    {
        if (filePath.IsNullOrEmpty())
            return TypeConstants.NullStringSpan;

        var dirSep = filePath.IndexOf(PclExport.Instance.DirSep) != -1
                         ? PclExport.Instance.DirSep
                         : filePath.IndexOf(PclExport.Instance.AltDirSep) != -1
                             ? PclExport.Instance.AltDirSep
                             : (char)0;

        if (dirSep == 0)
            return TypeConstants.NullStringSpan;

        ReadOnlySpan<char> first;

        filePath.TrimEnd(dirSep).SplitOnLast(dirSep, out first, out _);
        return first;
    }

    /// <summary>
    /// Trims the end.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="trimChars">The trim chars.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public static ReadOnlySpan<char> TrimEnd(this ReadOnlySpan<char> value, params char[] trimChars)
    {
        if (value.IsEmpty) return TypeConstants.NullStringSpan;
        if (trimChars == null || trimChars.Length == 0)
            return value.TrimHelper(1);
        return value.TrimHelper(trimChars, 1);
    }

    /// <summary>
    /// Trims the helper.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="trimType">Type of the trim.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    private static ReadOnlySpan<char> TrimHelper(this ReadOnlySpan<char> value, int trimType)
    {
        if (value.IsEmpty) return TypeConstants.NullStringSpan;
        int end = value.Length - 1;
        int start = 0;
        if (trimType != 1)
        {
            start = 0;
            while (start < value.Length && char.IsWhiteSpace(value[start]))
                ++start;
        }
        if (trimType != 0)
        {
            end = value.Length - 1;
            while (end >= start && char.IsWhiteSpace(value[end]))
                --end;
        }
        return value.CreateTrimmedString(start, end);
    }

    /// <summary>
    /// Trims the helper.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="trimChars">The trim chars.</param>
    /// <param name="trimType">Type of the trim.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    private static ReadOnlySpan<char> TrimHelper(this ReadOnlySpan<char> value, IReadOnlyList<char> trimChars, int trimType)
    {
        if (value.IsEmpty)
        {
            return TypeConstants.NullStringSpan;
        }
        int end = value.Length - 1;
        int start = 0;
        if (trimType != 1)
        {
            for (start = 0; start < value.Length; ++start)
            {
                char ch = value[start];
                int index = 0;
                while (index < trimChars.Count && trimChars[index] != ch)
                    ++index;
                if (index == trimChars.Count)
                    break;
            }
        }
        if (trimType != 0)
        {
            for (end = value.Length - 1; end >= start; --end)
            {
                char ch = value[end];
                int index = 0;
                while (index < trimChars.Count && trimChars[index] != ch)
                    ++index;
                if (index == trimChars.Count)
                    break;
            }
        }
        return value.CreateTrimmedString(start, end);
    }

    /// <summary>
    /// Creates the trimmed string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="start">The start.</param>
    /// <param name="end">The end.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    private static ReadOnlySpan<char> CreateTrimmedString(this ReadOnlySpan<char> value, int start, int end)
    {
        if (value.IsEmpty) return TypeConstants.NullStringSpan;
        int length = end - start + 1;
        if (length == value.Length)
            return value;
        return length == 0 ? TypeConstants.NullStringSpan : value.Slice(start, length);
    }

    /// <summary>
    /// Safes the slice.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="startIndex">The start index.</param>
    /// <param name="length">The length.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public static ReadOnlySpan<char> SafeSlice(this ReadOnlySpan<char> value, int startIndex, int length)
    {
        if (value.IsEmpty) return TypeConstants.NullStringSpan;
        if (startIndex < 0) startIndex = 0;
        if (value.Length >= startIndex + length)
            return value.Slice(startIndex, length);

        return value.Length > startIndex ? value.Slice(startIndex) : TypeConstants.NullStringSpan;
    }

    /// <summary>
    /// Substrings the with ellipsis.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="startIndex">The start index.</param>
    /// <param name="length">The length.</param>
    /// <returns>System.String.</returns>
    public static string SubstringWithEllipsis(this ReadOnlySpan<char> value, int startIndex, int length)
    {
        if (value.IsEmpty) return string.Empty;
        var str = value.SafeSlice(startIndex, length);
        return str.Length == length
                   ? str.ToString() + "..."
                   : str.ToString();
    }

    /// <summary>
    /// Indexes the of.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="other">The other.</param>
    /// <returns>System.Int32.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf(this ReadOnlySpan<char> value, string other) => value.IndexOf(other.AsSpan());

    /// <summary>
    /// Indexes the of.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="needle">The needle.</param>
    /// <param name="start">The start.</param>
    /// <returns>System.Int32.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf(this ReadOnlySpan<char> value, string needle, int start)
    {
        var pos = value.Slice(start).IndexOf(needle.AsSpan());
        return pos == -1 ? -1 : start + pos;
    }

    /// <summary>
    /// Lasts the index of.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="other">The other.</param>
    /// <returns>System.Int32.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LastIndexOf(this ReadOnlySpan<char> value, string other) => value.LastIndexOf(other.AsSpan());

    /// <summary>
    /// Lasts the index of.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="needle">The needle.</param>
    /// <param name="start">The start.</param>
    /// <returns>System.Int32.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LastIndexOf(this ReadOnlySpan<char> value, string needle, int start)
    {
        var pos = value.Slice(start).LastIndexOf(needle.AsSpan());
        return pos == -1 ? -1 : start + pos;
    }

    /// <summary>
    /// Equals to.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualTo(this ReadOnlySpan<char> value, string other) => value.Equals(other.AsSpan(), StringComparison.Ordinal);

    /// <summary>
    /// Equals to.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualTo(this ReadOnlySpan<char> value, ReadOnlySpan<char> other) => value.Equals(other, StringComparison.Ordinal);

    /// <summary>
    /// Equalses the ordinal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsOrdinal(this ReadOnlySpan<char> value, string other) => value.Equals(other.AsSpan(), StringComparison.Ordinal);

    /// <summary>
    /// Startses the with.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartsWith(this ReadOnlySpan<char> value, string other) => value.StartsWith(other.AsSpan(), StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Startses the with.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="other">The other.</param>
    /// <param name="comparison">The comparison.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartsWith(this ReadOnlySpan<char> value, string other, StringComparison comparison) => value.StartsWith(other.AsSpan(), comparison);

    /// <summary>
    /// Endses the with.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="other">The other.</param>
    /// <param name="comparison">The comparison.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EndsWith(this ReadOnlySpan<char> value, string other, StringComparison comparison) => value.EndsWith(other.AsSpan(), comparison);

    /// <summary>
    /// Endses the with.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EndsWith(this ReadOnlySpan<char> value, string other) => value.EndsWith(other.AsSpan(), StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Equalses the ignore case.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsIgnoreCase(this ReadOnlySpan<char> value, ReadOnlySpan<char> other) => value.Equals(other, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Startses the with ignore case.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartsWithIgnoreCase(this ReadOnlySpan<char> value, ReadOnlySpan<char> other) => value.StartsWith(other, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Writes the asynchronous.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="value">The value.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task WriteAsync(this Stream stream, ReadOnlySpan<char> value, CancellationToken token = default) =>
        MemoryProvider.Instance.WriteAsync(stream, value, token);

    /// <summary>
    /// Safes the substring.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="startIndex">The start index.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<char> SafeSubstring(this ReadOnlySpan<char> value, int startIndex) => SafeSubstring(value, startIndex, value.Length);

    /// <summary>
    /// Safes the substring.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="startIndex">The start index.</param>
    /// <param name="length">The length.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public static ReadOnlySpan<char> SafeSubstring(this ReadOnlySpan<char> value, int startIndex, int length)
    {
        if (value.IsEmpty) return TypeConstants.NullStringSpan;
        if (startIndex < 0) startIndex = 0;
        if (value.Length >= startIndex + length)
            return value.Slice(startIndex, length);

        return value.Length > startIndex ? value.Slice(startIndex) : TypeConstants.NullStringSpan;
    }

    /// <summary>
    /// Appends the specified value.
    /// </summary>
    /// <param name="sb">The sb.</param>
    /// <param name="value">The value.</param>
    /// <returns>StringBuilder.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder Append(this StringBuilder sb, ReadOnlySpan<char> value) =>
        MemoryProvider.Instance.Append(sb, value);

    /// <summary>
    /// Parses the base64.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Byte[].</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ParseBase64(this ReadOnlySpan<char> value) => MemoryProvider.Instance.ParseBase64(value);

    /// <summary>
    /// Converts to utf8.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>ReadOnlyMemory&lt;System.Byte&gt;.</returns>
    public static ReadOnlyMemory<byte> ToUtf8(this ReadOnlySpan<char> value) =>
        MemoryProvider.Instance.ToUtf8(value);

    /// <summary>
    /// Converts to stringlist.
    /// </summary>
    /// <param name="from">From.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<string> ToStringList(this IEnumerable<ReadOnlyMemory<char>> from)
    {
        var to = new List<string>();
        if (from != null)
        {
            to.AddRange(from.Select(item => item.ToString()));
        }
        return to;
    }

    /// <summary>
    /// Counts the occurrences of.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="needle">The needle.</param>
    /// <returns>System.Int32.</returns>
    public static int CountOccurrencesOf(this ReadOnlySpan<char> value, char needle)
    {
        var count = 0;
        var length = value.Length;
        for (var n = length - 1; n >= 0; n--)
        {
            if (value[n] == needle)
                count++;
        }
        return count;
    }

    /// <summary>
    /// Withouts the bom.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public static ReadOnlySpan<char> WithoutBom(this ReadOnlySpan<char> value)
    {
        return value.Length > 0 && value[0] == 65279
                   ? value.Slice(1)
                   : value;
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