// ***********************************************************************
// <copyright file="JsonUtils.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace ServiceStack.OrmLite.Base.Text.Json;

/// <summary>
/// Class JsonUtils.
/// </summary>
public static class JsonUtils
{
    /// <summary>
    /// The maximum integer
    /// </summary>
    public const long MaxInteger = 9007199254740992;
    /// <summary>
    /// The minimum integer
    /// </summary>
    public const long MinInteger = -9007199254740992;

    /// <summary>
    /// The escape character
    /// </summary>
    public const char EscapeChar = '\\';

    /// <summary>
    /// The quote character
    /// </summary>
    public const char QuoteChar = '"';
    /// <summary>
    /// The null
    /// </summary>
    public const string Null = "null";
    /// <summary>
    /// The true
    /// </summary>
    public const string True = "true";
    /// <summary>
    /// The false
    /// </summary>
    public const string False = "false";

    /// <summary>
    /// The space character
    /// </summary>
    public const char SpaceChar = ' ';
    /// <summary>
    /// The tab character
    /// </summary>
    public const char TabChar = '\t';
    /// <summary>
    /// The carriage return character
    /// </summary>
    public const char CarriageReturnChar = '\r';
    /// <summary>
    /// The line feed character
    /// </summary>
    public const char LineFeedChar = '\n';
    /// <summary>
    /// The form feed character
    /// </summary>
    public const char FormFeedChar = '\f';
    /// <summary>
    /// The backspace character
    /// </summary>
    public const char BackspaceChar = '\b';

    /// <summary>
    /// Micro-optimization keep pre-built char arrays saving a .ToCharArray() + function call (see .net implementation of .Write(string))
    /// </summary>
    private readonly static char[] EscapedBackslash = [EscapeChar, EscapeChar];
    /// <summary>
    /// The escaped tab
    /// </summary>
    private readonly static char[] EscapedTab = [EscapeChar, 't'];
    /// <summary>
    /// The escaped carriage return
    /// </summary>
    private readonly static char[] EscapedCarriageReturn = [EscapeChar, 'r'];
    /// <summary>
    /// The escaped line feed
    /// </summary>
    private readonly static char[] EscapedLineFeed = [EscapeChar, 'n'];
    /// <summary>
    /// The escaped form feed
    /// </summary>
    private readonly static char[] EscapedFormFeed = [EscapeChar, 'f'];
    /// <summary>
    /// The escaped backspace
    /// </summary>
    private readonly static char[] EscapedBackspace = [EscapeChar, 'b'];
    /// <summary>
    /// The escaped quote
    /// </summary>
    private readonly static char[] EscapedQuote = [EscapeChar, QuoteChar];

    /// <summary>
    /// The white space chars
    /// </summary>
    public readonly static char[] WhiteSpaceChars = [' ', TabChar, CarriageReturnChar, LineFeedChar];

    /// <summary>
    /// Determines whether [is white space] [the specified c].
    /// </summary>
    /// <param name="c">The c.</param>
    /// <returns><c>true</c> if [is white space] [the specified c]; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWhiteSpace(char c)
    {
        return c == ' ' || c is >= '\x0009' and <= '\x000d' || c == '\x00a0' || c == '\x0085' || c == TypeConstants.NonWidthWhiteSpace;
    }

    /// <summary>
    /// Writes the string.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    public static void WriteString(TextWriter writer, string value)
    {
        if (value == null)
        {
            writer.Write(Null);
            return;
        }

        var config = JsConfig.GetConfig();
        var escapeHtmlChars = config.EscapeHtmlChars;
        var escapeUnicode = config.EscapeUnicode;

        if (!HasAnyEscapeChars(value, escapeHtmlChars))
        {
            writer.Write(QuoteChar);
            writer.Write(value);
            writer.Write(QuoteChar);
            return;
        }

        var hexSeqBuffer = new char[4];
        writer.Write(QuoteChar);

        var len = value.Length;
        for (var i = 0; i < len; i++)
        {
            var c = value[i];

            switch (c)
            {
                case LineFeedChar:
                    writer.Write(EscapedLineFeed);
                    continue;

                case CarriageReturnChar:
                    writer.Write(EscapedCarriageReturn);
                    continue;

                case TabChar:
                    writer.Write(EscapedTab);
                    continue;

                case QuoteChar:
                    writer.Write(EscapedQuote);
                    continue;

                case EscapeChar:
                    writer.Write(EscapedBackslash);
                    continue;

                case FormFeedChar:
                    writer.Write(EscapedFormFeed);
                    continue;

                case BackspaceChar:
                    writer.Write(EscapedBackspace);
                    continue;
            }

            if (escapeHtmlChars)
            {
                switch (c)
                {
                    case '<':
                        writer.Write("\\u003c");
                        continue;
                    case '>':
                        writer.Write("\\u003e");
                        continue;
                    case '&':
                        writer.Write("\\u0026");
                        continue;
                    case '=':
                        writer.Write("\\u003d");
                        continue;
                    case '\'':
                        writer.Write("\\u0027");
                        continue;
                }
            }

            if (c.IsPrintable())
            {
                writer.Write(c);
                continue;
            }

            // http://json.org/ spec requires any control char to be escaped
            if (escapeUnicode || char.IsControl(c))
            {
                // Default, turn into a \uXXXX sequence
                IntToHex(c, hexSeqBuffer);
                writer.Write("\\u");
                writer.Write(hexSeqBuffer);
            }
            else
            {
                writer.Write(c);
            }
        }

        writer.Write(QuoteChar);
    }

    /// <summary>
    /// Determines whether the specified c is printable.
    /// </summary>
    /// <param name="c">The c.</param>
    /// <returns><c>true</c> if the specified c is printable; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsPrintable(this char c)
    {
        return c >= 32 && c <= 126;
    }

    /// <summary>
    /// Searches the string for one or more non-printable characters.
    /// </summary>
    /// <param name="value">The string to search.</param>
    /// <param name="escapeHtmlChars">if set to <c>true</c> [escape HTML chars].</param>
    /// <returns>True if there are any characters that require escaping. False if the value can be written verbatim.</returns>
    /// <remarks>Micro optimizations: since quote and backslash are the only printable characters requiring escaping, removed previous optimization
    /// (using flags instead of value.IndexOfAny(EscapeChars)) in favor of two equality operations saving both memory and CPU time.
    /// Also slightly reduced code size by re-arranging conditions.
    /// TODO: Possible Linq-only solution requires profiling: return value.Any(c =&gt; !c.IsPrintable() || c == QuoteChar || c == EscapeChar);</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool HasAnyEscapeChars(string value, bool escapeHtmlChars)
    {
        var len = value.Length;
        for (var i = 0; i < len; i++)
        {
            var c = value[i];

            // c is not printable
            // OR c is a printable that requires escaping (quote and backslash).
            if (!c.IsPrintable() || c == QuoteChar || c == EscapeChar)
            {
                return true;
            }

            if (escapeHtmlChars && (c == '<' || c == '>' || c == '&' || c == '=' || c == '\\'))
            {
                return true;
            }
        }
        return false;
    }

    // Micro optimized
    /// <summary>
    /// Ints to hexadecimal.
    /// </summary>
    /// <param name="intValue">The int value.</param>
    /// <param name="hex">The hexadecimal.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IntToHex(int intValue, char[] hex)
    {
        // TODO: test if unrolling loop is faster
        for (var i = 3; i >= 0; i--)
        {
            var num = intValue & 0xF; // intValue % 16

            // 0x30 + num == '0' + num
            // 0x37 + num == 'A' + (num - 10)
            hex[i] = (char)((num < 10 ? 0x30 : 0x37) + num);

            intValue >>= 4;
        }
    }

    /// <summary>
    /// Determines whether [is js object] [the specified value].
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if [is js object] [the specified value]; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsJsObject(ReadOnlySpan<char> value)
    {
        return !value.IsNullOrEmpty()
               && value[0] == '{'
               && value[^1] == '}';
    }

    /// <summary>
    /// Determines whether [is js array] [the specified value].
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if [is js array] [the specified value]; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsJsArray(ReadOnlySpan<char> value)
    {
        return !value.IsNullOrEmpty()
               && value[0] == '['
               && value[^1] == ']';
    }

}