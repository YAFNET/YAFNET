// ***********************************************************************
// <copyright file="MemoryProvider.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Text;

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ServiceStack.Text.Common;

/// <summary>
/// Class MemoryProvider.
/// </summary>
public abstract class MemoryProvider
{
    /// <summary>
    /// The instance
    /// </summary>
    public static MemoryProvider Instance =
#if NETCORE
            NetCoreMemory.Provider;
#else
        DefaultMemory.Provider;
#endif

    /// <summary>
    /// The bad format
    /// </summary>
    internal const string BadFormat = "Input string was not in a correct format.";
    /// <summary>
    /// The overflow message
    /// </summary>
    internal const string OverflowMessage = "Value was either too large or too small for an {0}.";

    /// <summary>
    /// Tries the parse boolean.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">if set to <c>true</c> [result].</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public abstract bool TryParseBoolean(ReadOnlySpan<char> value, out bool result);
    /// <summary>
    /// Parses the boolean.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public abstract bool ParseBoolean(ReadOnlySpan<char> value);

    /// <summary>
    /// Tries the parse decimal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public abstract bool TryParseDecimal(ReadOnlySpan<char> value, out decimal result);
    /// <summary>
    /// Parses the decimal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Decimal.</returns>
    public abstract decimal ParseDecimal(ReadOnlySpan<char> value);
    /// <summary>
    /// Parses the decimal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="allowThousands">if set to <c>true</c> [allow thousands].</param>
    /// <returns>System.Decimal.</returns>
    public abstract decimal ParseDecimal(ReadOnlySpan<char> value, bool allowThousands);

    /// <summary>
    /// Tries the parse float.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public abstract bool TryParseFloat(ReadOnlySpan<char> value, out float result);
    /// <summary>
    /// Parses the float.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Single.</returns>
    public abstract float ParseFloat(ReadOnlySpan<char> value);

    /// <summary>
    /// Tries the parse double.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public abstract bool TryParseDouble(ReadOnlySpan<char> value, out double result);
    /// <summary>
    /// Parses the double.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Double.</returns>
    public abstract double ParseDouble(ReadOnlySpan<char> value);

    /// <summary>
    /// Parses the s byte.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.SByte.</returns>
    public abstract sbyte ParseSByte(ReadOnlySpan<char> value);
    /// <summary>
    /// Parses the byte.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Byte.</returns>
    public abstract byte ParseByte(ReadOnlySpan<char> value);
    /// <summary>
    /// Parses the int16.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Int16.</returns>
    public abstract short ParseInt16(ReadOnlySpan<char> value);
    /// <summary>
    /// Parses the u int16.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.UInt16.</returns>
    public abstract ushort ParseUInt16(ReadOnlySpan<char> value);
    /// <summary>
    /// Parses the int32.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Int32.</returns>
    public abstract int ParseInt32(ReadOnlySpan<char> value);
    /// <summary>
    /// Parses the u int32.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.UInt32.</returns>
    public abstract uint ParseUInt32(ReadOnlySpan<char> value);
    /// <summary>
    /// Parses the u int32.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="style">The style.</param>
    /// <returns>System.UInt32.</returns>
    public abstract uint ParseUInt32(ReadOnlySpan<char> value, NumberStyles style);
    /// <summary>
    /// Parses the int64.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Int64.</returns>
    public abstract long ParseInt64(ReadOnlySpan<char> value);
    /// <summary>
    /// Parses the u int64.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.UInt64.</returns>
    public abstract ulong ParseUInt64(ReadOnlySpan<char> value);

    /// <summary>
    /// Parses the unique identifier.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>Guid.</returns>
    public abstract Guid ParseGuid(ReadOnlySpan<char> value);

    /// <summary>
    /// Parses the base64.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Byte[].</returns>
    public abstract byte[] ParseBase64(ReadOnlySpan<char> value);

    /// <summary>
    /// Converts to base64.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public abstract string ToBase64(ReadOnlyMemory<byte> value);

    /// <summary>
    /// Writes the specified stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="value">The value.</param>
    public abstract void Write(Stream stream, ReadOnlyMemory<char> value);
    /// <summary>
    /// Writes the specified stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="value">The value.</param>
    public abstract void Write(Stream stream, ReadOnlyMemory<byte> value);

    /// <summary>
    /// Writes the asynchronous.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="value">The value.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    public abstract Task WriteAsync(Stream stream, ReadOnlyMemory<char> value, CancellationToken token = default);
    /// <summary>
    /// Writes the asynchronous.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="value">The value.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    public abstract Task WriteAsync(Stream stream, ReadOnlyMemory<byte> value, CancellationToken token = default);

    /// <summary>
    /// Writes the asynchronous.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="value">The value.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    public abstract Task WriteAsync(Stream stream, ReadOnlySpan<char> value, CancellationToken token = default);

    /// <summary>
    /// Deserializes the specified stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="type">The type.</param>
    /// <param name="deserializer">The deserializer.</param>
    /// <returns>System.Object.</returns>
    public abstract object Deserialize(Stream stream, Type type, DeserializeStringSpanDelegate deserializer);

    /// <summary>
    /// Deserializes the asynchronous.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="type">The type.</param>
    /// <param name="deserializer">The deserializer.</param>
    /// <returns>Task&lt;System.Object&gt;.</returns>
    public abstract Task<object> DeserializeAsync(Stream stream, Type type,
                                                  DeserializeStringSpanDelegate deserializer);

    /// <summary>
    /// Appends the specified sb.
    /// </summary>
    /// <param name="sb">The sb.</param>
    /// <param name="value">The value.</param>
    /// <returns>StringBuilder.</returns>
    public abstract StringBuilder Append(StringBuilder sb, ReadOnlySpan<char> value);

    /// <summary>
    /// Gets the UTF8 character count.
    /// </summary>
    /// <param name="bytes">The bytes.</param>
    /// <returns>System.Int32.</returns>
    public abstract int GetUtf8CharCount(ReadOnlySpan<byte> bytes);
    /// <summary>
    /// Gets the UTF8 byte count.
    /// </summary>
    /// <param name="chars">The chars.</param>
    /// <returns>System.Int32.</returns>
    public abstract int GetUtf8ByteCount(ReadOnlySpan<char> chars);

    /// <summary>
    /// Converts to utf8.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>ReadOnlyMemory&lt;System.Byte&gt;.</returns>
    public abstract ReadOnlyMemory<byte> ToUtf8(ReadOnlySpan<char> source);
    /// <summary>
    /// Froms the UTF8.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>ReadOnlyMemory&lt;System.Char&gt;.</returns>
    public abstract ReadOnlyMemory<char> FromUtf8(ReadOnlySpan<byte> source);

    /// <summary>
    /// Converts to utf8.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="destination">The destination.</param>
    /// <returns>System.Int32.</returns>
    public abstract int ToUtf8(ReadOnlySpan<char> source, Span<byte> destination);
    /// <summary>
    /// Froms the UTF8.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="destination">The destination.</param>
    /// <returns>System.Int32.</returns>
    public abstract int FromUtf8(ReadOnlySpan<byte> source, Span<char> destination);

    /// <summary>
    /// Converts to utf8bytes.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>System.Byte[].</returns>
    public abstract byte[] ToUtf8Bytes(ReadOnlySpan<char> source);
    /// <summary>
    /// Froms the UTF8 bytes.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>System.String.</returns>
    public abstract string FromUtf8Bytes(ReadOnlySpan<byte> source);
    /// <summary>
    /// Converts to memorystream.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>MemoryStream.</returns>
    public abstract MemoryStream ToMemoryStream(ReadOnlySpan<byte> source);
}