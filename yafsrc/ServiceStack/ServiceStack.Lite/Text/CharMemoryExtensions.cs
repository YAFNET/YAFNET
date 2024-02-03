// ***********************************************************************
// <copyright file="CharMemoryExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Text;

using System;
using System.Runtime.CompilerServices;

/// <summary>
/// Class CharMemoryExtensions.
/// </summary>
public static class CharMemoryExtensions
{
    /// <summary>
    /// Determines whether [is null or empty] [the specified value].
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if [is null or empty] [the specified value]; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty(this ReadOnlyMemory<char> value)
    {
        return value.IsEmpty;
    }

    /// <summary>
    /// Advances the specified to.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="to">To.</param>
    /// <returns>ReadOnlyMemory&lt;System.Char&gt;.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyMemory<char> Advance(this ReadOnlyMemory<char> text, int to)
    {
        return text[to..];
    }

    /// <summary>
    /// Rights the part.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <returns>ReadOnlyMemory&lt;System.Char&gt;.</returns>
    public static ReadOnlyMemory<char> RightPart(this ReadOnlyMemory<char> strVal, char needle)
    {
        if (strVal.IsEmpty)
        {
            return strVal;
        }

        var pos = strVal.IndexOf(needle);
        return pos == -1
                   ? strVal
                   : strVal[(pos + 1)..];
    }

    /// <summary>
    /// Rights the part.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <returns>ReadOnlyMemory&lt;System.Char&gt;.</returns>
    public static ReadOnlyMemory<char> RightPart(this ReadOnlyMemory<char> strVal, string needle)
    {
        if (strVal.IsEmpty)
        {
            return strVal;
        }

        var pos = strVal.IndexOf(needle);
        return pos == -1
                   ? strVal
                   : strVal[(pos + needle.Length)..];
    }

    /// <summary>
    /// Lasts the right part.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <returns>ReadOnlyMemory&lt;System.Char&gt;.</returns>
    public static ReadOnlyMemory<char> LastRightPart(this ReadOnlyMemory<char> strVal, char needle)
    {
        if (strVal.IsEmpty)
        {
            return strVal;
        }

        var pos = strVal.LastIndexOf(needle);
        return pos == -1
                   ? strVal
                   : strVal[(pos + 1)..];
    }

    /// <summary>
    /// Lasts the right part.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <returns>ReadOnlyMemory&lt;System.Char&gt;.</returns>
    public static ReadOnlyMemory<char> LastRightPart(this ReadOnlyMemory<char> strVal, string needle)
    {
        if (strVal.IsEmpty)
        {
            return strVal;
        }

        var pos = strVal.LastIndexOf(needle);
        return pos == -1
                   ? strVal
                   : strVal[(pos + needle.Length)..];
    }

    /// <summary>
    /// Tries the read line.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="line">The line.</param>
    /// <param name="startIndex">The start index.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool TryReadLine(this ReadOnlyMemory<char> text, out ReadOnlyMemory<char> line, ref int startIndex)
    {
        if (startIndex >= text.Length)
        {
            line = TypeConstants.NullStringMemory;
            return false;
        }

        text = text[startIndex..];

        var nextLinePos = text.Span.IndexOfAny('\r', '\n');
        if (nextLinePos == -1)
        {
            var nextLine = text[..text.Length];
            startIndex += text.Length;
            line = nextLine;
            return true;
        }
        else
        {
            var nextLine = text[..nextLinePos];

            startIndex += nextLinePos + 1;

            var span = text.Span;
            if (span[nextLinePos] == '\r' && span.Length > nextLinePos + 1 && span[nextLinePos + 1] == '\n')
            {
                startIndex += 1;
            }

            line = nextLine;
            return true;
        }
    }

    /// <summary>
    /// Tries the read part.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="needle">The needle.</param>
    /// <param name="part">The part.</param>
    /// <param name="startIndex">The start index.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool TryReadPart(this ReadOnlyMemory<char> text, ReadOnlyMemory<char> needle, out ReadOnlyMemory<char> part, ref int startIndex)
    {
        if (startIndex >= text.Length)
        {
            part = TypeConstants.NullStringMemory;
            return false;
        }

        text = text[startIndex..];
        var nextPartPos = text.Span.IndexOf(needle.Span);
        if (nextPartPos == -1)
        {
            var nextPart = text[..text.Length];
            startIndex += text.Length;
            part = nextPart;
            return true;
        }
        else
        {
            var nextPart = text[..nextPartPos];
            startIndex += nextPartPos + needle.Length;
            part = nextPart;
            return true;
        }
    }

    /// <summary>
    /// Splits the on first.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <param name="first">The first.</param>
    /// <param name="last">The last.</param>
    public static void SplitOnFirst(this ReadOnlyMemory<char> strVal, char needle, out ReadOnlyMemory<char> first, out ReadOnlyMemory<char> last)
    {
        first = default;
        last = default;
        if (strVal.IsEmpty)
        {
            return;
        }

        var pos = strVal.Span.IndexOf(needle);
        if (pos == -1)
        {
            first = strVal;
        }
        else
        {
            first = strVal[..pos];
            last = strVal[(pos + 1)..];
        }
    }

    /// <summary>
    /// Splits the on first.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <param name="first">The first.</param>
    /// <param name="last">The last.</param>
    public static void SplitOnFirst(this ReadOnlyMemory<char> strVal, ReadOnlyMemory<char> needle, out ReadOnlyMemory<char> first, out ReadOnlyMemory<char> last)
    {
        first = default;
        last = default;
        if (strVal.IsEmpty)
        {
            return;
        }

        var pos = strVal.Span.IndexOf(needle.Span);
        if (pos == -1)
        {
            first = strVal;
        }
        else
        {
            first = strVal[..pos];
            last = strVal[(pos + needle.Length)..];
        }
    }

    /// <summary>
    /// Splits the on last.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <param name="first">The first.</param>
    /// <param name="last">The last.</param>
    public static void SplitOnLast(this ReadOnlyMemory<char> strVal, char needle, out ReadOnlyMemory<char> first, out ReadOnlyMemory<char> last)
    {
        first = default;
        last = default;
        if (strVal.IsEmpty)
        {
            return;
        }

        var pos = strVal.Span.LastIndexOf(needle);
        if (pos == -1)
        {
            first = strVal;
        }
        else
        {
            first = strVal[..pos];
            last = strVal[(pos + 1)..];
        }
    }

    /// <summary>
    /// Splits the on last.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <param name="first">The first.</param>
    /// <param name="last">The last.</param>
    public static void SplitOnLast(this ReadOnlyMemory<char> strVal, ReadOnlyMemory<char> needle, out ReadOnlyMemory<char> first, out ReadOnlyMemory<char> last)
    {
        first = default;
        last = default;
        if (strVal.IsEmpty)
        {
            return;
        }

        var pos = strVal.Span.LastIndexOf(needle.Span);
        if (pos == -1)
        {
            first = strVal;
        }
        else
        {
            first = strVal[..pos];
            last = strVal[(pos + needle.Length)..];
        }
    }

    /// <summary>
    /// Indexes the of.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="needle">The needle.</param>
    /// <returns>System.Int32.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf(this ReadOnlyMemory<char> value, char needle)
    {
        return value.Span.IndexOf(needle);
    }

    /// <summary>
    /// Indexes the of.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="needle">The needle.</param>
    /// <returns>System.Int32.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf(this ReadOnlyMemory<char> value, string needle)
    {
        return value.Span.IndexOf(needle.AsSpan());
    }

    /// <summary>
    /// Indexes the of.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="needle">The needle.</param>
    /// <param name="start">The start.</param>
    /// <returns>System.Int32.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf(this ReadOnlyMemory<char> value, char needle, int start)
    {
        var pos = value[start..].Span.IndexOf(needle);
        return pos == -1 ? -1 : start + pos;
    }

    /// <summary>
    /// Indexes the of.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="needle">The needle.</param>
    /// <param name="start">The start.</param>
    /// <returns>System.Int32.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf(this ReadOnlyMemory<char> value, string needle, int start)
    {
        var pos = value[start..].Span.IndexOf(needle.AsSpan());
        return pos == -1 ? -1 : start + pos;
    }

    /// <summary>
    /// Lasts the index of.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="needle">The needle.</param>
    /// <returns>System.Int32.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LastIndexOf(this ReadOnlyMemory<char> value, char needle)
    {
        return value.Span.LastIndexOf(needle);
    }

    /// <summary>
    /// Lasts the index of.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="needle">The needle.</param>
    /// <returns>System.Int32.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LastIndexOf(this ReadOnlyMemory<char> value, string needle)
    {
        return value.Span.LastIndexOf(needle.AsSpan());
    }

    /// <summary>
    /// Lasts the index of.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="needle">The needle.</param>
    /// <param name="start">The start.</param>
    /// <returns>System.Int32.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LastIndexOf(this ReadOnlyMemory<char> value, char needle, int start)
    {
        var pos = value[start..].Span.LastIndexOf(needle);
        return pos == -1 ? -1 : start + pos;
    }

    /// <summary>
    /// Lasts the index of.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="needle">The needle.</param>
    /// <param name="start">The start.</param>
    /// <returns>System.Int32.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LastIndexOf(this ReadOnlyMemory<char> value, string needle, int start)
    {
        var pos = value[start..].Span.LastIndexOf(needle.AsSpan());
        return pos == -1 ? -1 : start + pos;
    }

    /// <summary>
    /// Startses the with.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartsWith(this ReadOnlyMemory<char> value, string other)
    {
        return value.Span.StartsWith(other.AsSpan(), StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Startses the with.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="other">The other.</param>
    /// <param name="comparison">The comparison.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartsWith(this ReadOnlyMemory<char> value, string other, StringComparison comparison)
    {
        return value.Span.StartsWith(other.AsSpan(), comparison);
    }

    /// <summary>
    /// Endses the with.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EndsWith(this ReadOnlyMemory<char> value, string other)
    {
        return value.Span.EndsWith(other.AsSpan(), StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Endses the with.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="other">The other.</param>
    /// <param name="comparison">The comparison.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EndsWith(this ReadOnlyMemory<char> value, string other, StringComparison comparison)
    {
        return value.Span.EndsWith(other.AsSpan(), comparison);
    }

    /// <summary>
    /// Equalses the ordinal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsOrdinal(this ReadOnlyMemory<char> value, string other)
    {
        return value.Span.Equals(other.AsSpan(), StringComparison.Ordinal);
    }

    /// <summary>
    /// Equalses the ordinal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsOrdinal(this ReadOnlyMemory<char> value, ReadOnlyMemory<char> other)
    {
        return value.Span.Equals(other.Span, StringComparison.Ordinal);
    }

    /// <summary>
    /// Safes the slice.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="startIndex">The start index.</param>
    /// <param name="length">The length.</param>
    /// <returns>ReadOnlyMemory&lt;System.Char&gt;.</returns>
    public static ReadOnlyMemory<char> SafeSlice(this ReadOnlyMemory<char> value, int startIndex, int length)
    {
        if (value.IsEmpty)
        {
            return TypeConstants.NullStringMemory;
        }

        if (startIndex < 0)
        {
            startIndex = 0;
        }

        if (value.Length >= startIndex + length)
        {
            return value.Slice(startIndex, length);
        }

        return value.Length > startIndex ? value[startIndex..] : TypeConstants.NullStringMemory;
    }
}