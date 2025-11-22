// ***********************************************************************
// <copyright file="CharMemoryExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Runtime.CompilerServices;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Class CharMemoryExtensions.
/// </summary>
public static class CharMemoryExtensions
{
    /// <param name="value">The value.</param>
    extension(ReadOnlyMemory<char> value)
    {
        /// <summary>
        /// Determines whether [is null or empty] [the specified value].
        /// </summary>
        /// <returns><c>true</c> if [is null or empty] [the specified value]; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNullOrEmpty()
        {
            return value.IsEmpty;
        }

        /// <summary>
        /// Advances the specified to.
        /// </summary>
        /// <param name="to">To.</param>
        /// <returns>ReadOnlyMemory&lt;System.Char&gt;.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlyMemory<char> Advance(int to)
        {
            return value[to..];
        }

        /// <summary>
        /// Rights the part.
        /// </summary>
        /// <param name="needle">The needle.</param>
        /// <returns>ReadOnlyMemory&lt;System.Char&gt;.</returns>
        public ReadOnlyMemory<char> RightPart(char needle)
        {
            if (value.IsEmpty)
            {
                return value;
            }

            var pos = value.IndexOf(needle);
            return pos == -1
                ? value
                : value[(pos + 1)..];
        }

        /// <summary>
        /// Rights the part.
        /// </summary>
        /// <param name="needle">The needle.</param>
        /// <returns>ReadOnlyMemory&lt;System.Char&gt;.</returns>
        public ReadOnlyMemory<char> RightPart(string needle)
        {
            if (value.IsEmpty)
            {
                return value;
            }

            var pos = value.IndexOf(needle);
            return pos == -1
                ? value
                : value[(pos + needle.Length)..];
        }

        /// <summary>
        /// Lasts the right part.
        /// </summary>
        /// <param name="needle">The needle.</param>
        /// <returns>ReadOnlyMemory&lt;System.Char&gt;.</returns>
        public ReadOnlyMemory<char> LastRightPart(char needle)
        {
            if (value.IsEmpty)
            {
                return value;
            }

            var pos = value.LastIndexOf(needle);
            return pos == -1
                ? value
                : value[(pos + 1)..];
        }

        /// <summary>
        /// Lasts the right part.
        /// </summary>
        /// <param name="needle">The needle.</param>
        /// <returns>ReadOnlyMemory&lt;System.Char&gt;.</returns>
        public ReadOnlyMemory<char> LastRightPart(string needle)
        {
            if (value.IsEmpty)
            {
                return value;
            }

            var pos = value.LastIndexOf(needle);
            return pos == -1
                ? value
                : value[(pos + needle.Length)..];
        }

        /// <summary>
        /// Tries the read line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool TryReadLine(out ReadOnlyMemory<char> line, ref int startIndex)
        {
            if (startIndex >= value.Length)
            {
                line = TypeConstants.NullStringMemory;
                return false;
            }

            value = value[startIndex..];

            var nextLinePos = value.Span.IndexOfAny('\r', '\n');
            if (nextLinePos == -1)
            {
                var nextLine = value[..value.Length];
                startIndex += value.Length;
                line = nextLine;
                return true;
            }
            else
            {
                var nextLine = value[..nextLinePos];

                startIndex += nextLinePos + 1;

                var span = value.Span;
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
        /// <param name="needle">The needle.</param>
        /// <param name="part">The part.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool TryReadPart(ReadOnlyMemory<char> needle, out ReadOnlyMemory<char> part, ref int startIndex)
        {
            if (startIndex >= value.Length)
            {
                part = TypeConstants.NullStringMemory;
                return false;
            }

            value = value[startIndex..];
            var nextPartPos = value.Span.IndexOf(needle.Span);
            if (nextPartPos == -1)
            {
                var nextPart = value[..value.Length];
                startIndex += value.Length;
                part = nextPart;
                return true;
            }
            else
            {
                var nextPart = value[..nextPartPos];
                startIndex += nextPartPos + needle.Length;
                part = nextPart;
                return true;
            }
        }

        /// <summary>
        /// Splits the on first.
        /// </summary>
        /// <param name="needle">The needle.</param>
        /// <param name="first">The first.</param>
        /// <param name="last">The last.</param>
        public void SplitOnFirst(char needle, out ReadOnlyMemory<char> first, out ReadOnlyMemory<char> last)
        {
            first = default;
            last = default;
            if (value.IsEmpty)
            {
                return;
            }

            var pos = value.Span.IndexOf(needle);
            if (pos == -1)
            {
                first = value;
            }
            else
            {
                first = value[..pos];
                last = value[(pos + 1)..];
            }
        }

        /// <summary>
        /// Splits the on first.
        /// </summary>
        /// <param name="needle">The needle.</param>
        /// <param name="first">The first.</param>
        /// <param name="last">The last.</param>
        public void SplitOnFirst(ReadOnlyMemory<char> needle, out ReadOnlyMemory<char> first, out ReadOnlyMemory<char> last)
        {
            first = default;
            last = default;
            if (value.IsEmpty)
            {
                return;
            }

            var pos = value.Span.IndexOf(needle.Span);
            if (pos == -1)
            {
                first = value;
            }
            else
            {
                first = value[..pos];
                last = value[(pos + needle.Length)..];
            }
        }

        /// <summary>
        /// Splits the on last.
        /// </summary>
        /// <param name="needle">The needle.</param>
        /// <param name="first">The first.</param>
        /// <param name="last">The last.</param>
        public void SplitOnLast(char needle, out ReadOnlyMemory<char> first, out ReadOnlyMemory<char> last)
        {
            first = default;
            last = default;
            if (value.IsEmpty)
            {
                return;
            }

            var pos = value.Span.LastIndexOf(needle);
            if (pos == -1)
            {
                first = value;
            }
            else
            {
                first = value[..pos];
                last = value[(pos + 1)..];
            }
        }

        /// <summary>
        /// Splits the on last.
        /// </summary>
        /// <param name="needle">The needle.</param>
        /// <param name="first">The first.</param>
        /// <param name="last">The last.</param>
        public void SplitOnLast(ReadOnlyMemory<char> needle, out ReadOnlyMemory<char> first, out ReadOnlyMemory<char> last)
        {
            first = default;
            last = default;
            if (value.IsEmpty)
            {
                return;
            }

            var pos = value.Span.LastIndexOf(needle.Span);
            if (pos == -1)
            {
                first = value;
            }
            else
            {
                first = value[..pos];
                last = value[(pos + needle.Length)..];
            }
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="needle">The needle.</param>
        /// <returns>System.Int32.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(char needle)
        {
            return value.Span.IndexOf(needle);
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="needle">The needle.</param>
        /// <returns>System.Int32.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(string needle)
        {
            return value.Span.IndexOf(needle.AsSpan());
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="needle">The needle.</param>
        /// <param name="start">The start.</param>
        /// <returns>System.Int32.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(char needle, int start)
        {
            var pos = value[start..].Span.IndexOf(needle);
            return pos == -1 ? -1 : start + pos;
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="needle">The needle.</param>
        /// <param name="start">The start.</param>
        /// <returns>System.Int32.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(string needle, int start)
        {
            var pos = value[start..].Span.IndexOf(needle.AsSpan());
            return pos == -1 ? -1 : start + pos;
        }

        /// <summary>
        /// Lasts the index of.
        /// </summary>
        /// <param name="needle">The needle.</param>
        /// <returns>System.Int32.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(char needle)
        {
            return value.Span.LastIndexOf(needle);
        }

        /// <summary>
        /// Lasts the index of.
        /// </summary>
        /// <param name="needle">The needle.</param>
        /// <returns>System.Int32.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(string needle)
        {
            return value.Span.LastIndexOf(needle.AsSpan());
        }

        /// <summary>
        /// Lasts the index of.
        /// </summary>
        /// <param name="needle">The needle.</param>
        /// <param name="start">The start.</param>
        /// <returns>System.Int32.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(char needle, int start)
        {
            var pos = value[start..].Span.LastIndexOf(needle);
            return pos == -1 ? -1 : start + pos;
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
            var pos = value[start..].Span.LastIndexOf(needle.AsSpan());
            return pos == -1 ? -1 : start + pos;
        }

        /// <summary>
        /// Startses the with.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool StartsWith(string other)
        {
            return value.Span.StartsWith(other.AsSpan(), StringComparison.OrdinalIgnoreCase);
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
            return value.Span.StartsWith(other.AsSpan(), comparison);
        }

        /// <summary>
        /// Endses the with.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EndsWith(string other)
        {
            return value.Span.EndsWith(other.AsSpan(), StringComparison.OrdinalIgnoreCase);
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
            return value.Span.EndsWith(other.AsSpan(), comparison);
        }

        /// <summary>
        /// Equalses the ordinal.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EqualsOrdinal(string other)
        {
            return value.Span.Equals(other.AsSpan(), StringComparison.Ordinal);
        }

        /// <summary>
        /// Equalses the ordinal.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EqualsOrdinal(ReadOnlyMemory<char> other)
        {
            return value.Span.Equals(other.Span, StringComparison.Ordinal);
        }

        /// <summary>
        /// Safes the slice.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The length.</param>
        /// <returns>ReadOnlyMemory&lt;System.Char&gt;.</returns>
        public ReadOnlyMemory<char> SafeSlice(int startIndex, int length)
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
}