// ***********************************************************************
// <copyright file="NetCoreMemory.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Buffers.Text;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ServiceStack.OrmLite.Base.Text.Common;
using ServiceStack.OrmLite.Base.Text.Pools;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Class NetCoreMemory. This class cannot be inherited.
/// Implements the <see cref="MemoryProvider" />
/// </summary>
/// <seealso cref="MemoryProvider" />
public sealed class NetCoreMemory : MemoryProvider
{
    /// <summary>
    /// Gets the provider.
    /// </summary>
    /// <value>The provider.</value>
    public static NetCoreMemory Provider => field ??= new NetCoreMemory();

    /// <summary>
    /// Prevents a default instance of the <see cref="NetCoreMemory"/> class from being created.
    /// </summary>
    private NetCoreMemory()
    {
    }

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
    public override bool ParseBoolean(ReadOnlySpan<char> value)
    {
        return bool.Parse(value);
    }

    /// <summary>
    /// Tries the parse boolean.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">if set to <c>true</c> [result].</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public override bool TryParseBoolean(ReadOnlySpan<char> value, out bool result)
    {
        return bool.TryParse(value, out result);
    }

    /// <summary>
    /// Tries the parse decimal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public override bool TryParseDecimal(ReadOnlySpan<char> value, out decimal result)
    {
        return decimal.TryParse(
            value,
            NumberStyles.Float | NumberStyles.AllowThousands,
            CultureInfo.InvariantCulture,
            out result);
    }

    /// <summary>
    /// Parses the decimal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="allowThousands">if set to <c>true</c> [allow thousands].</param>
    /// <returns>System.Decimal.</returns>
    public override decimal ParseDecimal(ReadOnlySpan<char> value, bool allowThousands)
    {
        return decimal.Parse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
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
            value,
            NumberStyles.Float | NumberStyles.AllowThousands,
            CultureInfo.InvariantCulture,
            out result);
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
            value,
            NumberStyles.Float | NumberStyles.AllowThousands,
            CultureInfo.InvariantCulture,
            out result);
    }

    /// <summary>
    /// Parses the decimal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Decimal.</returns>
    public override decimal ParseDecimal(ReadOnlySpan<char> value)
    {
        return decimal.Parse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Parses the float.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Single.</returns>
    public override float ParseFloat(ReadOnlySpan<char> value)
    {
        return float.Parse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Parses the double.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Double.</returns>
    public override double ParseDouble(ReadOnlySpan<char> value)
    {
        return double.Parse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Parses the s byte.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.SByte.</returns>
    public override sbyte ParseSByte(ReadOnlySpan<char> value)
    {
        return sbyte.Parse(value);
    }

    /// <summary>
    /// Parses the byte.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Byte.</returns>
    public override byte ParseByte(ReadOnlySpan<char> value)
    {
        return byte.Parse(value);
    }

    /// <summary>
    /// Parses the int32.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Int32.</returns>
    public override int ParseInt32(ReadOnlySpan<char> value)
    {
        return int.Parse(value);
    }

    /// <summary>
    /// Parses the u int32.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="style">The style.</param>
    /// <returns>System.UInt32.</returns>
    public override uint ParseUInt32(ReadOnlySpan<char> value, NumberStyles style)
    {
        return uint.Parse(value.ToString(), NumberStyles.HexNumber);
    }

    /// <summary>
    /// Parses the base64.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Byte[].</returns>
    public override byte[] ParseBase64(ReadOnlySpan<char> value)
    {
        var bytes = BufferPool.GetBuffer(Base64.GetMaxDecodedFromUtf8Length(value.Length));
        try
        {
            if (Convert.TryFromBase64Chars(value, bytes, out var bytesWritten))
            {
                var ret = new byte[bytesWritten];
                Buffer.BlockCopy(bytes, 0, ret, 0, bytesWritten);
                return ret;
            }
            else
            {
                var chars = value.ToArray();
                return Convert.FromBase64CharArray(chars, 0, chars.Length);
            }
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
    public override void Write(Stream stream, ReadOnlyMemory<char> value)
    {
        var utf8 = this.ToUtf8(value.Span);
        if (stream is MemoryStream ms)
        {
            ms.Write(utf8.Span);
        }
        else
        {
            stream.Write(utf8.Span);
        }
    }

    /// <summary>
    /// Writes the specified stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="value">The value.</param>
    public override void Write(Stream stream, ReadOnlyMemory<byte> value)
    {
        if (stream is MemoryStream ms)
        {
            ms.Write(value.Span);
        }
        else
        {
            stream.Write(value.Span);
        }
    }

    /// <summary>
    /// Write as an asynchronous operation.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="value">The value.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async override Task WriteAsync(Stream stream, ReadOnlyMemory<char> value, CancellationToken token = default)
    {
        var utf8 = this.ToUtf8(value.Span);
        if (stream is MemoryStream ms)
        {
            ms.Write(utf8.Span);
        }
        else
        {
            await stream.WriteAsync(utf8, token).ConfigAwait();
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
        var utf8 = this.ToUtf8(value);
        if (stream is MemoryStream ms)
        {
            ms.Write(utf8.Span);
            return Task.CompletedTask;
        }

        return stream.WriteAsync(utf8, token).AsTask();
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
        if (stream is MemoryStream ms)
        {
            ms.Write(value.Span);
        }
        else
        {
            await stream.WriteAsync(value, token).ConfigAwait();
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
    public async override Task<object> DeserializeAsync(
        Stream stream,
        Type type,
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
    /// Deserializes the specified memory stream.
    /// </summary>
    /// <param name="memoryStream">The memory stream.</param>
    /// <param name="fromPool">if set to <c>true</c> [from pool].</param>
    /// <param name="type">The type.</param>
    /// <param name="deserializer">The deserializer.</param>
    /// <returns>System.Object.</returns>
    private static object Deserialize(
        MemoryStream memoryStream,
        bool fromPool,
        Type type,
        DeserializeStringSpanDelegate deserializer)
    {
        var bytes = memoryStream.GetBufferAsSpan().WithoutBom();
        var chars = CharPool.GetBuffer(Encoding.UTF8.GetCharCount(bytes));
        try
        {
            var charsWritten = Encoding.UTF8.GetChars(bytes, chars);
            ReadOnlySpan<char> charsSpan = chars;
            var ret = deserializer(type, charsSpan.Slice(0, charsWritten));
            return ret;
        }
        finally
        {
            CharPool.ReleaseBufferToPool(ref chars);

            if (fromPool)
            {
                memoryStream.Dispose();
            }
        }
    }

    /// <summary>
    /// Appends the specified sb.
    /// </summary>
    /// <param name="sb">The sb.</param>
    /// <param name="value">The value.</param>
    /// <returns>StringBuilder.</returns>
    public override StringBuilder Append(StringBuilder sb, ReadOnlySpan<char> value)
    {
        return sb.Append(value);
    }

    /// <summary>
    /// Converts to utf8.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>ReadOnlyMemory&lt;System.Byte&gt;.</returns>
    public ReadOnlyMemory<byte> ToUtf8(ReadOnlySpan<char> source)
    {
        Memory<byte> bytes = new byte[Encoding.UTF8.GetByteCount(source)];
        var bytesWritten = Encoding.UTF8.GetBytes(source, bytes.Span);
        return bytes.Slice(0, bytesWritten);
    }
}