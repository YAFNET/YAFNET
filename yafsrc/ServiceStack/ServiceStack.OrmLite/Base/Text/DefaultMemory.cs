// ***********************************************************************
// <copyright file="DefaultMemory.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ServiceStack.OrmLite.Base.Text.Common;
using ServiceStack.OrmLite.Base.Text.Json;
using ServiceStack.OrmLite.Base.Text.Pools;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Class DefaultMemory. This class cannot be inherited.
/// Implements the <see cref="MemoryProvider" />
/// </summary>
/// <seealso cref="MemoryProvider" />
public sealed class DefaultMemory : MemoryProvider
{
    /// <summary>
    /// Gets the provider.
    /// </summary>
    /// <value>The provider.</value>
    public static DefaultMemory Provider => field ??= new DefaultMemory();
    /// <summary>
    /// Prevents a default instance of the <see cref="DefaultMemory" /> class from being created.
    /// </summary>
    private DefaultMemory() { }

    /// <summary>
    /// Configures this instance.
    /// </summary>
    public static void Configure()
    {
        Instance = Provider;
    }

    /// <summary>
    /// Parses the boolean.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="System.FormatException"></exception>
    public override bool ParseBoolean(ReadOnlySpan<char> value)
    {
        if (!value.TryParseBoolean(out var result))
        {
            throw new FormatException(BadFormat);
        }

        return result;
    }

    /// <summary>
    /// Tries the parse boolean.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">if set to <c>true</c> [result].</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public override bool TryParseBoolean(ReadOnlySpan<char> value, out bool result)
    {
        result = false;

        if (value.CompareIgnoreCase(bool.TrueString.AsSpan()))
        {
            result = true;
            return true;
        }

        return value.CompareIgnoreCase(bool.FalseString.AsSpan());
    }

    /// <summary>
    /// Tries the parse decimal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public override bool TryParseDecimal(ReadOnlySpan<char> value, out decimal result)
    {
        return TryParseDecimal(value, true, out result);
    }

    /// <summary>
    /// Parses the decimal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Decimal.</returns>
    public override decimal ParseDecimal(ReadOnlySpan<char> value)
    {
        return this.ParseDecimal(value, true);
    }

    /// <summary>
    /// Parses the decimal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="allowThousands">if set to <c>true</c> [allow thousands].</param>
    /// <returns>System.Decimal.</returns>
    /// <exception cref="System.FormatException"></exception>
    public override decimal ParseDecimal(ReadOnlySpan<char> value, bool allowThousands)
    {
        if (!TryParseDecimal(value, allowThousands, out var result))
        {
            throw new FormatException(BadFormat);
        }

        return result;
    }

    /// <summary>
    /// Tries the parse float.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public override bool TryParseFloat(ReadOnlySpan<char> value, out float result)
    {
        return float.TryParse(
            value.ToString(), NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture,
            out result);
    }

    /// <summary>
    /// Parses the float.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Single.</returns>
    public override float ParseFloat(ReadOnlySpan<char> value)
    {
        return float.Parse(value.ToString(),
            NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Tries the parse double.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public override bool TryParseDouble(ReadOnlySpan<char> value, out double result)
    {
        return double.TryParse(
            value.ToString(), NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture,
            out result);
    }

    /// <summary>
    /// Parses the double.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Double.</returns>
    public override double ParseDouble(ReadOnlySpan<char> value)
    {
        return double.Parse(value.ToString(),
            NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Parses the s byte.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.SByte.</returns>
    public override sbyte ParseSByte(ReadOnlySpan<char> value)
    {
        return SignedInteger<sbyte>.ParseSByte(value);
    }

    /// <summary>
    /// Parses the byte.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Byte.</returns>
    public override byte ParseByte(ReadOnlySpan<char> value)
    {
        return UnsignedInteger<byte>.ParseByte(value);
    }

    /// <summary>
    /// Parses the int32.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Int32.</returns>
    public override int ParseInt32(ReadOnlySpan<char> value)
    {
        return SignedInteger<int>.ParseInt32(value);
    }

    /// <summary>
    /// Parses the u int32.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="style">The style.</param>
    /// <returns>System.UInt32.</returns>
    public override uint ParseUInt32(ReadOnlySpan<char> value, NumberStyles style)
    {
        return uint.Parse(value.ToString(), style);
    }

    /// <summary>
    /// Creates the overflow exception.
    /// </summary>
    /// <param name="maxValue">The maximum value.</param>
    /// <returns>Exception.</returns>
    static internal Exception CreateOverflowException(long maxValue)
    {
        return new OverflowException(string.Format(OverflowMessage, SignedMaxValueToIntType(maxValue)));
    }

    /// <summary>
    /// Creates the overflow exception.
    /// </summary>
    /// <param name="maxValue">The maximum value.</param>
    /// <returns>Exception.</returns>
    static internal Exception CreateOverflowException(ulong maxValue)
    {
        return new OverflowException(string.Format(OverflowMessage, UnsignedMaxValueToIntType(maxValue)));
    }

    /// <summary>
    /// Signeds the maximum type of the value to int.
    /// </summary>
    /// <param name="maxValue">The maximum value.</param>
    /// <returns>System.String.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string SignedMaxValueToIntType(long maxValue)
    {
        return maxValue switch {
                sbyte.MaxValue => nameof(SByte),
                short.MaxValue => nameof(Int16),
                int.MaxValue => nameof(Int32),
                long.MaxValue => nameof(Int64),
                _ => "Unknown"
            };
    }

    /// <summary>
    /// Unsigneds the maximum type of the value to int.
    /// </summary>
    /// <param name="maxValue">The maximum value.</param>
    /// <returns>System.String.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string UnsignedMaxValueToIntType(ulong maxValue)
    {
        return maxValue switch {
                byte.MaxValue => nameof(Byte),
                ushort.MaxValue => nameof(UInt16),
                uint.MaxValue => nameof(UInt32),
                ulong.MaxValue => nameof(UInt64),
                _ => "Unknown"
            };
    }

    /// <summary>
    /// Tries the parse decimal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="allowThousands">if set to <c>true</c> [allow thousands].</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool TryParseDecimal(ReadOnlySpan<char> value, bool allowThousands, out decimal result)
    {
        result = 0;

        if (value.Length == 0)
        {
            return false;
        }

        ulong preResult = 0;
        var isLargeNumber = false;
        var i = 0;
        var end = value.Length;
        var state = ParseState.LeadingWhite;
        var negative = false;
        var noIntegerPart = false;
        sbyte scale = 0;

        while (i < end)
        {
            var c = value[i++];

            switch (state)
            {
                case ParseState.LeadingWhite:
                    if (JsonUtils.IsWhiteSpace(c))
                    {
                        break;
                    }

                    switch (c)
                    {
                        case '-':
                            negative = true;
                            state = ParseState.Sign;
                            break;
                        case '.':
                            {
                                noIntegerPart = true;
                                state = ParseState.FractionNumber;

                                if (i == end)
                                {
                                    return false;
                                }

                                break;
                            }
                        case '0':
                            state = ParseState.DecimalPoint;
                            break;
                        case > '0' and <= '9':
                            preResult = (ulong)(c - '0');
                            state = ParseState.Number;
                            break;
                        default:
                            return false;
                    }

                    break;
                case ParseState.Sign:
                    switch (c)
                    {
                        case '.':
                            {
                                noIntegerPart = true;
                                state = ParseState.FractionNumber;

                                if (i == end)
                                {
                                    return false;
                                }

                                break;
                            }
                        case '0':
                            state = ParseState.DecimalPoint;
                            break;
                        case > '0' and <= '9':
                            preResult = (ulong)(c - '0');
                            state = ParseState.Number;
                            break;
                        default:
                            return false;
                    }

                    break;
                case ParseState.Number:
                    switch (c)
                    {
                        case '.':
                            state = ParseState.FractionNumber;
                            break;
                        case >= '0' and <= '9' when isLargeNumber:
                            checked
                            {
                                result = 10 * result + (c - '0');
                            }

                            break;
                        case >= '0' and <= '9':
                            {
                                preResult = 10 * preResult + (ulong)(c - '0');
                                if (preResult > ulong.MaxValue / 10 - 10)
                                {
                                    isLargeNumber = true;
                                    result = preResult;
                                }

                                break;
                            }
                        default:
                            {
                                if (JsonUtils.IsWhiteSpace(c))
                                {
                                    state = ParseState.TrailingWhite;
                                }
                                else if (allowThousands && c == ',') { }
                                else
                                {
                                    return false;
                                }

                                break;
                            }
                    }

                    break;
                case ParseState.DecimalPoint:
                    if (c == '.')
                    {
                        state = ParseState.FractionNumber;
                    }
                    else
                    {
                        return false;
                    }

                    break;
                case ParseState.FractionNumber:
                    if (JsonUtils.IsWhiteSpace(c))
                    {
                        if (noIntegerPart)
                        {
                            return false;
                        }

                        state = ParseState.TrailingWhite;
                    }
                    else
                    {
                        switch (c)
                        {
                            case 'e':
                            case 'E':
                            {
                                if (noIntegerPart && scale == 0)
                                {
                                    return false;
                                }

                                state = ParseState.Exponent;
                                break;
                            }
                            case >= '0' and <= '9':
                            {
                                if (isLargeNumber)
                                {
                                    checked
                                    {
                                        result = 10 * result + (c - '0');
                                    }
                                }
                                else
                                {
                                    preResult = 10 * preResult + (ulong)(c - '0');
                                    if (preResult > ulong.MaxValue / 10 - 10)
                                    {
                                        isLargeNumber = true;
                                        result = preResult;
                                    }
                                }

                                scale++;
                                break;
                            }
                            default:
                                return false;
                        }
                    }

                    break;
                case ParseState.Exponent:
                    var expNegative = false;
                    switch (c)
                    {
                        case '-' when i == end:
                            return false;
                        case '-':
                            expNegative = true;
                            c = value[i++];
                            break;
                        case '+' when i == end:
                            return false;
                        case '+':
                            c = value[i++];
                            break;
                    }

                    //skip leading zeroes
                    while (c == '0' && i < end)
                    {
                        c = value[i++];
                    }

                    if (c is > '0' and <= '9')
                    {
                        var exp = SignedInteger<long>.ParseInt64(value.Slice(i - 1, end - i + 1));
                        if (exp < sbyte.MinValue || exp > sbyte.MaxValue)
                        {
                            return false;
                        }

                        if (!expNegative)
                        {
                            exp = (sbyte)-exp;
                        }

                        if (exp >= 0 || scale > -exp)
                        {
                            scale += (sbyte)exp;
                        }
                        else
                        {
                            for (var j = 0; j < -exp - scale; j++)
                            {
                                if (isLargeNumber)
                                {
                                    checked
                                    {
                                        result = 10 * result;
                                    }
                                }
                                else
                                {
                                    preResult = 10 * preResult;
                                    if (preResult > ulong.MaxValue / 10)
                                    {
                                        isLargeNumber = true;
                                        result = preResult;
                                    }
                                }
                            }

                            scale = 0;
                        }

                        //set i to end of string, because ParseInt16 eats number and all trailing whites
                        i = end;
                    }
                    else
                    {
                        return false;
                    }

                    break;
                case ParseState.TrailingWhite:
                    if (!JsonUtils.IsWhiteSpace(c))
                    {
                        return false;
                    }

                    break;
            }
        }

        if (!isLargeNumber)
        {
            var mid = (int)(preResult >> 32);
            var lo = (int)(preResult & 0xffffffff);
            result = new decimal(lo, mid, 0, negative, (byte)scale);
        }
        else
        {
            var bits = decimal.GetBits(result);
            result = new decimal(bits[0], bits[1], bits[2], negative, (byte)scale);
        }

        return true;
    }

    /// <summary>
    /// The lo16
    /// </summary>
    private readonly static byte[] lo16 = [
        255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
                                                  255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
                                                  255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
                                                  255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
                                                  255, 255, 255, 255, 255, 255, 255, 255, 0, 1,
                                                  2, 3, 4, 5, 6, 7, 8, 9, 255, 255,
                                                  255, 255, 255, 255, 255, 10, 11, 12, 13, 14,
                                                  15, 255, 255, 255, 255, 255, 255, 255, 255, 255,
                                                  255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
                                                  255, 255, 255, 255, 255, 255, 255, 10, 11, 12,
                                                  13, 14, 15
    ];

    /// <summary>
    /// The hi16
    /// </summary>
    private readonly static byte[] hi16 = [
        255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
                                                  255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
                                                  255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
                                                  255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
                                                  255, 255, 255, 255, 255, 255, 255, 255, 0, 16,
                                                  32, 48, 64, 80, 96, 112, 128, 144, 255, 255,
                                                  255, 255, 255, 255, 255, 160, 176, 192, 208, 224,
                                                  240, 255, 255, 255, 255, 255, 255, 255, 255, 255,
                                                  255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
                                                  255, 255, 255, 255, 255, 255, 255, 160, 176, 192,
                                                  208, 224, 240
    ];

    /// <summary>
    /// Parses the unique identifier.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>Guid.</returns>
    /// <exception cref="System.FormatException"></exception>
    public Guid ParseGuid(ReadOnlySpan<char> value)
    {
        if (value.IsEmpty)
        {
            throw new FormatException(BadFormat);
        }

        //Guid can be in one of 3 forms:
        //1. General `{dddddddd-dddd-dddd-dddd-dddddddddddd}` or `(dddddddd-dddd-dddd-dddd-dddddddddddd)` 8-4-4-4-12 chars
        //2. Hex `{0xdddddddd,0xdddd,0xdddd,{0xdd,0xdd,0xdd,0xdd,0xdd,0xdd,0xdd,0xdd}}`  8-4-4-8x2 chars
        //3. No style `dddddddddddddddddddddddddddddddd` 32 chars

        var i = 0;
        var end = value.Length;
        while (i < end && JsonUtils.IsWhiteSpace(value[i]))
        {
            i++;
        }

        if (i == end)
        {
            throw new FormatException(BadFormat);
        }

        var result = ParseGeneralStyleGuid(value.Slice(i, end - i), out var guidLen);
        i += guidLen;

        while (i < end && JsonUtils.IsWhiteSpace(value[i]))
        {
            i++;
        }

        if (i < end)
        {
            throw new FormatException(BadFormat);
        }

        return result;
    }

    /// <summary>
    /// Writes the specified stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="value">The value.</param>
    public override void Write(Stream stream, ReadOnlyMemory<char> value)
    {
        var bytes = BufferPool.GetBuffer(Encoding.UTF8.GetMaxByteCount(value.Length));
        try
        {
            var chars = value.ToArray();
            var bytesCount = Encoding.UTF8.GetBytes(chars, 0, chars.Length, bytes, 0);
            stream.Write(bytes, 0, bytesCount);
        }
        finally
        {
            BufferPool.ReleaseBufferToPool(ref bytes);
        }
    }

    /// <summary>
    /// Writes the specified stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="value">The value.</param>
    public override void Write(Stream stream, ReadOnlyMemory<byte> value)
    {
        if (MemoryMarshal.TryGetArray(value, out var segment) && segment.Array != null)
        {
            var bytes = BufferPool.GetBuffer(segment.Count);
            try
            {
                stream.Write(segment.Array, 0, segment.Count);
            }
            finally
            {
                BufferPool.ReleaseBufferToPool(ref bytes);
            }
        }
        else
        {
            var bytes = value.ToArray();
            stream.Write(bytes, 0, value.Length);
        }
    }

    /// <summary>
    /// Writes the asynchronous.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="value">The value.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    public override Task WriteAsync(Stream stream, ReadOnlySpan<char> value, CancellationToken token = default)
    {
        // encode the span into a buffer; this should never fail, so if it does: something
        // is very very ill; don't stress about returning to the pool
        var bytes = BufferPool.GetBuffer(Encoding.UTF8.GetMaxByteCount(value.Length));
        var chars = value.ToArray();
        var bytesCount = Encoding.UTF8.GetBytes(chars, 0, chars.Length, bytes, 0);
        // now do the write async - this returns to the pool
        return WriteAsyncAndReturnAsync(stream, bytes, 0, bytesCount, token);
    }

    /// <summary>
    /// Writes the asynchronous and return.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="bytes">The bytes.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="count">The count.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    private async static Task WriteAsyncAndReturnAsync(Stream stream, byte[] bytes, int offset, int count, CancellationToken token)
    {
        try
        {
            await stream.WriteAsync(bytes, offset, count, token).ConfigAwait();
        }
        finally
        {
            BufferPool.ReleaseBufferToPool(ref bytes);
        }
    }

    /// <summary>
    /// Writes the asynchronous.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="value">The value.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    public override Task WriteAsync(Stream stream, ReadOnlyMemory<char> value, CancellationToken token = default)
    {
        return this.WriteAsync(stream, value.Span, token);
    }

    /// <summary>
    /// Write as an asynchronous operation.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="value">The value.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async override Task WriteAsync(Stream stream, ReadOnlyMemory<byte> value, CancellationToken token = default)
    {
        var bytes = BufferPool.GetBuffer(value.Length);
        try
        {
            value.CopyTo(bytes);
            if (stream is MemoryStream ms)
            {
                // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                ms.Write(bytes, 0, value.Length);
            }
            else
            {
                await stream.WriteAsync(bytes, 0, value.Length, token).ConfigAwait();
            }
        }
        finally
        {
            BufferPool.ReleaseBufferToPool(ref bytes);
        }
    }

    /// <summary>
    /// Deserializes the specified stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="type">The type.</param>
    /// <param name="deserializer">The deserializer.</param>
    /// <returns>System.Object.</returns>
    public override object Deserialize(Stream stream, Type type, DeserializeStringSpanDelegate deserializer)
    {
        var fromPool = false;

        if (stream is not MemoryStream ms)
        {
            fromPool = true;

            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            ms = stream.CopyToNewMemoryStream();
        }

        return Deserialize(ms, fromPool, type, deserializer);
    }

    /// <summary>
    /// Deserialize as an asynchronous operation.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="type">The type.</param>
    /// <param name="deserializer">The deserializer.</param>
    /// <returns>A Task&lt;System.Object&gt; representing the asynchronous operation.</returns>
    public async override Task<object> DeserializeAsync(Stream stream, Type type,
                                                        DeserializeStringSpanDelegate deserializer)
    {
        var fromPool = false;

        if (stream is not MemoryStream ms)
        {
            fromPool = true;

            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            ms = await stream.CopyToNewMemoryStreamAsync().ConfigAwait();
        }

        return Deserialize(ms, fromPool, type, deserializer);
    }

    /// <summary>
    /// Deserializes the specified ms.
    /// </summary>
    /// <param name="ms">The ms.</param>
    /// <param name="fromPool">if set to <c>true</c> [from pool].</param>
    /// <param name="type">The type.</param>
    /// <param name="deserializer">The deserializer.</param>
    /// <returns>System.Object.</returns>
    private static object Deserialize(MemoryStream ms, bool fromPool, Type type,
                                      DeserializeStringSpanDelegate deserializer)
    {
        var bytes = ms.GetBufferAsBytes();
        var utf8 = CharPool.GetBuffer(Encoding.UTF8.GetCharCount(bytes, 0, (int)ms.Length));
        try
        {
            var charsWritten = Encoding.UTF8.GetChars(bytes, 0, (int)ms.Length, utf8, 0);
            var ret = deserializer(type, new ReadOnlySpan<char>(utf8, 0, charsWritten).WithoutBom());
            return ret;
        }
        finally
        {
            CharPool.ReleaseBufferToPool(ref utf8);

            if (fromPool)
            {
                ms.Dispose();
            }
        }
    }

    /// <summary>
    /// Parses the base64.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Byte[].</returns>
    public override byte[] ParseBase64(ReadOnlySpan<char> value)
    {
        return Convert.FromBase64String(value.ToString());
    }

    /// <summary>
    /// Appends the specified sb.
    /// </summary>
    /// <param name="sb">The sb.</param>
    /// <param name="value">The value.</param>
    /// <returns>StringBuilder.</returns>
    public override StringBuilder Append(StringBuilder sb, ReadOnlySpan<char> value)
    {
        return sb.Append(value.ToArray());
    }

    /// <summary>
    /// Parses the general style unique identifier.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="len">The length.</param>
    /// <returns>Guid.</returns>
    /// <exception cref="System.FormatException"></exception>
    private static Guid ParseGeneralStyleGuid(ReadOnlySpan<char> value, out int len)
    {
        var buf = value;
        var n = 0;

        var dash = 0;
        len = 32;
        var hasParenthesis = false;

        if (value.Length < len)
        {
            throw new FormatException(BadFormat);
        }

        var cs = value[0];
        if (cs == '{' || cs == '(')
        {
            n++;
            len += 2;
            hasParenthesis = true;

            if (buf[8 + n] != '-')
            {
                throw new FormatException(BadFormat);
            }
        }

        if (buf[8 + n] == '-')
        {
            if (buf[13 + n] != '-'
                || buf[18 + n] != '-'
                || buf[23 + n] != '-')
            {
                throw new FormatException(BadFormat);
            }

            len += 4;
            dash = 1;
        }

        if (value.Length < len)
        {
            throw new FormatException(BadFormat);
        }

        if (hasParenthesis)
        {
            var ce = buf[len - 1];

            if ((cs != '{' || ce != '}') && (cs != '(' || ce != ')'))
            {
                throw new FormatException(BadFormat);
            }
        }

        var a1 = ParseHexByte(buf[n], buf[n + 1]);
        n += 2;
        var a2 = ParseHexByte(buf[n], buf[n + 1]);
        n += 2;
        var a3 = ParseHexByte(buf[n], buf[n + 1]);
        n += 2;
        var a4 = ParseHexByte(buf[n], buf[n + 1]);
        var a = (a1 << 24) + (a2 << 16) + (a3 << 8) + a4;
        n += 2 + dash;

        var b1 = ParseHexByte(buf[n], buf[n + 1]);
        n += 2;
        var b2 = ParseHexByte(buf[n], buf[n + 1]);
        var b = (short)((b1 << 8) + b2);
        n += 2 + dash;

        var c1 = ParseHexByte(buf[n], buf[n + 1]);
        n += 2;
        var c2 = ParseHexByte(buf[n], buf[n + 1]);
        var c = (short)((c1 << 8) + c2);
        n += 2 + dash;

        var d = ParseHexByte(buf[n], buf[n + 1]);
        n += 2;
        var e = ParseHexByte(buf[n], buf[n + 1]);
        n += 2 + dash;

        var f = ParseHexByte(buf[n], buf[n + 1]);
        n += 2;
        var g = ParseHexByte(buf[n], buf[n + 1]);
        n += 2;
        var h = ParseHexByte(buf[n], buf[n + 1]);
        n += 2;
        var i = ParseHexByte(buf[n], buf[n + 1]);
        n += 2;
        var j = ParseHexByte(buf[n], buf[n + 1]);
        n += 2;
        var k = ParseHexByte(buf[n], buf[n + 1]);

        return new Guid(a, b, c, d, e, f, g, h, i, j, k);
    }

    /// <summary>
    /// Parses the hexadecimal byte.
    /// </summary>
    /// <param name="c1">The c1.</param>
    /// <param name="c2">The c2.</param>
    /// <returns>System.Byte.</returns>
    /// <exception cref="System.FormatException"></exception>
    private static byte ParseHexByte(char c1, char c2)
    {
        try
        {
            var lo = lo16[c2];
            var hi = hi16[c1];

            if (lo == 255 || hi == 255)
            {
                throw new FormatException(BadFormat);
            }

            return (byte)(hi + lo);
        }
        catch (IndexOutOfRangeException)
        {
            throw new FormatException(BadFormat);
        }
    }
}

/// <summary>
/// Enum ParseState
/// </summary>
enum ParseState
{
    /// <summary>
    /// The leading white
    /// </summary>
    LeadingWhite,
    /// <summary>
    /// The sign
    /// </summary>
    Sign,
    /// <summary>
    /// The number
    /// </summary>
    Number,
    /// <summary>
    /// The decimal point
    /// </summary>
    DecimalPoint,
    /// <summary>
    /// The fraction number
    /// </summary>
    FractionNumber,
    /// <summary>
    /// The exponent
    /// </summary>
    Exponent,
    /// <summary>
    /// The exponent sign
    /// </summary>
    ExponentSign,
    /// <summary>
    /// The trailing white
    /// </summary>
    TrailingWhite
}

/// <summary>
/// Class SignedInteger.
/// </summary>
/// <typeparam name="T"></typeparam>
static internal class SignedInteger<T> where T : struct, IComparable<T>, IEquatable<T>, IConvertible
{
    /// <summary>
    /// The type code
    /// </summary>
    private readonly static TypeCode typeCode;
    /// <summary>
    /// The minimum value
    /// </summary>
    private readonly static long minValue;
    /// <summary>
    /// The maximum value
    /// </summary>
    private readonly static long maxValue;

    /// <summary>
    /// Initializes static members of the <see cref="SignedInteger{T}" /> class.
    /// </summary>
    /// <exception cref="System.NotSupportedException"></exception>
    static SignedInteger()
    {
        typeCode = Type.GetTypeCode(typeof(T));

        switch (typeCode)
        {
            case TypeCode.SByte:
                minValue = sbyte.MinValue;
                maxValue = sbyte.MaxValue;
                break;
            case TypeCode.Int16:
                minValue = short.MinValue;
                maxValue = short.MaxValue;
                break;
            case TypeCode.Int32:
                minValue = int.MinValue;
                maxValue = int.MaxValue;
                break;
            case TypeCode.Int64:
                minValue = long.MinValue;
                maxValue = long.MaxValue;
                break;
            default:
                throw new NotSupportedException($"{typeof(T).Name} is not a signed integer");
        }
    }

    /// <summary>
    /// Parses the nullable object.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    static internal object ParseNullableObject(ReadOnlySpan<char> value)
    {
        return value.IsNullOrEmpty() ? null : ParseObject(value);
    }

    /// <summary>
    /// Parses the object.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    static internal object ParseObject(ReadOnlySpan<char> value)
    {
        var result = ParseInt64(value);

        /*return typeCode switch {
            TypeCode.SByte => result,
            TypeCode.Int16 => result,
            TypeCode.Int32 => result,
            _ => result
        };*/

        return result;
    }

    /// <summary>
    /// Parses the s byte.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.SByte.</returns>
    public static sbyte ParseSByte(ReadOnlySpan<char> value)
    {
        return (sbyte)ParseInt64(value);
    }

    /// <summary>
    /// Parses the int32.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Int32.</returns>
    public static int ParseInt32(ReadOnlySpan<char> value)
    {
        return (int)ParseInt64(value);
    }

    /// <summary>
    /// Parses the int64.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Int64.</returns>
    /// <exception cref="System.FormatException"></exception>
    public static long ParseInt64(ReadOnlySpan<char> value)
    {
        if (value.IsEmpty)
        {
            throw new FormatException(MemoryProvider.BadFormat);
        }

        long result = 0;
        var i = 0;
        var end = value.Length;
        var state = ParseState.LeadingWhite;
        var negative = false;

        //skip leading whitespaces
        while (i < end && JsonUtils.IsWhiteSpace(value[i]))
        {
            i++;
        }

        if (i == end)
        {
            throw new FormatException(MemoryProvider.BadFormat);
        }

        //skip leading zeros
        while (i < end && value[i] == '0')
        {
            state = ParseState.Number;
            i++;
        }

        while (i < end)
        {
            var c = value[i++];

            switch (state)
            {
                case ParseState.LeadingWhite:
                    switch (c)
                    {
                        case '-':
                            negative = true;
                            state = ParseState.Sign;
                            break;
                        case '0':
                            state = ParseState.TrailingWhite;
                            break;
                        case > '0' and <= '9':
                            result = -(c - '0');
                            state = ParseState.Number;
                            break;
                        default:
                            throw new FormatException(MemoryProvider.BadFormat);
                    }

                    break;
                case ParseState.Sign:
                    switch (c)
                    {
                        case '0':
                            state = ParseState.TrailingWhite;
                            break;
                        case > '0' and <= '9':
                            result = -(c - '0');
                            state = ParseState.Number;
                            break;
                        default:
                            throw new FormatException(MemoryProvider.BadFormat);
                    }

                    break;
                case ParseState.Number:
                    if (c is >= '0' and <= '9')
                    {
                        checked
                        {
                            result = 10 * result - (c - '0');
                        }

                        if (result < minValue
                           ) //check only minvalue, because in absolute value it's greater than maxvalue
                        {
                            throw DefaultMemory.CreateOverflowException(maxValue);
                        }
                    }
                    else if (JsonUtils.IsWhiteSpace(c))
                    {
                        state = ParseState.TrailingWhite;
                    }
                    else
                    {
                        throw new FormatException(MemoryProvider.BadFormat);
                    }

                    break;
                case ParseState.TrailingWhite:
                    if (JsonUtils.IsWhiteSpace(c))
                    {
                        state = ParseState.TrailingWhite;
                    }
                    else
                    {
                        throw new FormatException(MemoryProvider.BadFormat);
                    }

                    break;
            }
        }

        if (state != ParseState.Number && state != ParseState.TrailingWhite)
        {
            throw new FormatException(MemoryProvider.BadFormat);
        }

        if (negative)
        {
            return result;
        }

        checked
        {
            result = -result;
        }

        if (result > maxValue)
        {
            throw DefaultMemory.CreateOverflowException(maxValue);
        }

        return result;
    }
}

/// <summary>
/// Class UnsignedInteger.
/// </summary>
/// <typeparam name="T"></typeparam>
static internal class UnsignedInteger<T> where T : struct, IComparable<T>, IEquatable<T>, IConvertible
{
    /// <summary>
    /// The type code
    /// </summary>
    private readonly static TypeCode typeCode;
    /// <summary>
    /// The maximum value
    /// </summary>
    private readonly static ulong maxValue;

    /// <summary>
    /// Initializes static members of the <see cref="UnsignedInteger{T}" /> class.
    /// </summary>
    /// <exception cref="System.NotSupportedException"></exception>
    static UnsignedInteger()
    {
        typeCode = Type.GetTypeCode(typeof(T));

        maxValue = typeCode switch {
                TypeCode.Byte => byte.MaxValue,
                TypeCode.UInt16 => ushort.MaxValue,
                TypeCode.UInt32 => uint.MaxValue,
                TypeCode.UInt64 => ulong.MaxValue,
                _ => throw new NotSupportedException($"{typeof(T).Name} is not a signed integer")
            };
    }

    /// <summary>
    /// Parses the nullable object.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    static internal object ParseNullableObject(ReadOnlySpan<char> value)
    {
        return value.IsNullOrEmpty() ? null : ParseObject(value);
    }

    /// <summary>
    /// Parses the object.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    static internal object ParseObject(ReadOnlySpan<char> value)
    {
        var result = ParseUInt64(value);

        /*return typeCode switch {
            TypeCode.Byte => result,
            TypeCode.UInt16 => result,
            TypeCode.UInt32 => result,
            _ => result
        };*/

        return result;
    }

    /// <summary>
    /// Parses the byte.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Byte.</returns>
    public static byte ParseByte(ReadOnlySpan<char> value)
    {
        return (byte)ParseUInt64(value);
    }

    /// <summary>
    /// Parses the u int64.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.UInt64.</returns>
    /// <exception cref="System.FormatException"></exception>
    static internal ulong ParseUInt64(ReadOnlySpan<char> value)
    {
        if (value.IsEmpty)
        {
            throw new FormatException(MemoryProvider.BadFormat);
        }

        ulong result = 0;
        var i = 0;
        var end = value.Length;
        var state = ParseState.LeadingWhite;

        //skip leading whitespaces
        while (i < end && JsonUtils.IsWhiteSpace(value[i]))
        {
            i++;
        }

        if (i == end)
        {
            throw new FormatException(MemoryProvider.BadFormat);
        }

        //skip leading zeros
        while (i < end && value[i] == '0')
        {
            state = ParseState.Number;
            i++;
        }

        while (i < end)
        {
            var c = value[i++];

            switch (state)
            {
                case ParseState.LeadingWhite:
                    if (JsonUtils.IsWhiteSpace(c))
                    {
                        break;
                    }

                    switch (c)
                    {
                        case '0':
                            state = ParseState.TrailingWhite;
                            break;
                        case > '0' and <= '9':
                            result = (ulong)(c - '0');
                            state = ParseState.Number;
                            break;
                        default:
                            throw new FormatException(MemoryProvider.BadFormat);
                    }


                    break;
                case ParseState.Number:
                    if (c is >= '0' and <= '9')
                    {
                        checked
                        {
                            result = 10 * result + (ulong)(c - '0');
                        }

                        if (result > maxValue
                           ) //check only minvalue, because in absolute value it's greater than maxvalue
                        {
                            throw DefaultMemory.CreateOverflowException(maxValue);
                        }
                    }
                    else if (JsonUtils.IsWhiteSpace(c))
                    {
                        state = ParseState.TrailingWhite;
                    }
                    else
                    {
                        throw new FormatException(MemoryProvider.BadFormat);
                    }

                    break;
                case ParseState.TrailingWhite:
                    if (JsonUtils.IsWhiteSpace(c))
                    {
                        state = ParseState.TrailingWhite;
                    }
                    else
                    {
                        throw new FormatException(MemoryProvider.BadFormat);
                    }

                    break;
            }
        }

        if (state != ParseState.Number && state != ParseState.TrailingWhite)
        {
            throw new FormatException(MemoryProvider.BadFormat);
        }

        return result;
    }
}