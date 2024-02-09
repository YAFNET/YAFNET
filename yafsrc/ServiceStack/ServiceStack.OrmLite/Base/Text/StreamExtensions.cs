// ***********************************************************************
// <copyright file="StreamExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Class StreamExtensions.
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    /// Writes to.
    /// </summary>
    /// <param name="inStream">The in stream.</param>
    /// <param name="outStream">The out stream.</param>
    /// <returns>long.</returns>
    public static long WriteTo(this Stream inStream, Stream outStream)
    {
        if (inStream is MemoryStream memoryStream)
        {
            memoryStream.WriteTo(outStream);
            return memoryStream.Position;
        }

        var data = new byte[4096];
        long total = 0;
        int bytesRead;

        while ((bytesRead = inStream.Read(data, 0, data.Length)) > 0)
        {
            outStream.Write(data, 0, bytesRead);
            total += bytesRead;
        }

        return total;
    }

    /// <summary>
    /// @jonskeet: Collection of utility methods which operate on streams.
    /// r285, February 26th 2009: http://www.yoda.arachsys.com/csharp/miscutil/
    /// </summary>
    public const int DefaultBufferSize = 8 * 1024;

    /// <summary>
    /// Copies all the data from one stream into another, using the given
    /// buffer for transferring data. Note that the current contents of
    /// the buffer is ignored, so the buffer needn't be cleared beforehand.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="output">The output.</param>
    /// <param name="buffer">The buffer.</param>
    /// <returns>long.</returns>
    /// <exception cref="ArgumentNullException">nameof(buffer)</exception>
    /// <exception cref="ArgumentNullException">nameof(buffer)</exception>
    /// <exception cref="ArgumentNullException">nameof(buffer)</exception>
    /// <exception cref="ArgumentNullException">nameof(buffer)</exception>
    public static long CopyTo(this Stream input, Stream output, byte[] buffer)
    {
        if (buffer == null)
            throw new ArgumentNullException(nameof(buffer));

        if (input == null)
            throw new ArgumentNullException(nameof(input));

        if (output == null)
            throw new ArgumentNullException(nameof(output));

        if (buffer.Length == 0)
            throw new ArgumentException("Buffer has length of 0");

        long total = 0;
        int read;
        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
        {
            output.Write(buffer, 0, read);
            total += read;
        }
        return total;
    }

    /// <summary>
    /// Combines the specified bytes.
    /// </summary>
    /// <param name="bytes">The bytes.</param>
    /// <param name="withBytes">The with bytes.</param>
    /// <returns>byte[].</returns>
    public static byte[] Combine(this byte[] bytes, params byte[][] withBytes)
    {
        var combinedLength = bytes.Length + withBytes.Sum(b => b.Length);
        var to = new byte[combinedLength];

        Buffer.BlockCopy(bytes, 0, to, 0, bytes.Length);
        var pos = bytes.Length;

        foreach (var b in withBytes)
        {
            Buffer.BlockCopy(b, 0, to, pos, b.Length);
            pos += b.Length;
        }

        return to;
    }

    /// <summary>
    /// Writes the asynchronous.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="value">The value.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>System.Threading.Tasks.Task.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task WriteAsync(this Stream stream, ReadOnlyMemory<byte> value, CancellationToken token = default) =>
        MemoryProvider.Instance.WriteAsync(stream, value, token);

    /// <summary>
    /// Writes the asynchronous.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="bytes">The bytes.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>System.Threading.Tasks.Task.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task WriteAsync(this Stream stream, byte[] bytes, CancellationToken token = default) =>
        MemoryProvider.Instance.WriteAsync(stream, bytes, token);

    /// <summary>
    /// Writes the asynchronous.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="text">The text.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>System.Threading.Tasks.Task.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task WriteAsync(this Stream stream, string text, CancellationToken token = default) =>
        MemoryProvider.Instance.WriteAsync(stream, text.AsMemory(), token);

    /// <summary>
    /// Reads to end.
    /// </summary>
    /// <param name="ms">The ms.</param>
    /// <returns>string.</returns>
    public static string ReadToEnd(this MemoryStream ms) => ReadToEnd(ms, JsConfig.UTF8Encoding);
    /// <summary>
    /// Reads to end.
    /// </summary>
    /// <param name="ms">The ms.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns>string.</returns>
    public static string ReadToEnd(this MemoryStream ms, Encoding encoding)
    {
        ms.Position = 0;

        try
        {
            return encoding.GetString(ms.GetBuffer(), 0, (int)ms.Length);
        }
        catch (UnauthorizedAccessException)
        {
        }

        Tracer.Instance.WriteWarning("MemoryStream wasn't created with a publiclyVisible:true byte[] buffer, falling back to slow impl");

        using var reader = new StreamReader(ms, encoding, true, DefaultBufferSize, true);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Gets the buffer as bytes.
    /// </summary>
    /// <param name="ms">The ms.</param>
    /// <returns>byte[].</returns>
    public static byte[] GetBufferAsBytes(this MemoryStream ms)
    {
        try
        {
            return ms.GetBuffer();
        }
        catch (UnauthorizedAccessException)
        {
        }

        Tracer.Instance.WriteWarning("MemoryStream in GetBufferAsBytes() wasn't created with a publiclyVisible:true byte[] buffer, falling back to slow impl");
        return ms.ToArray();
    }

    /// <summary>
    /// Reads to end.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns>string.</returns>
    public static string ReadToEnd(this Stream stream) => ReadToEnd(stream, JsConfig.UTF8Encoding);
    /// <summary>
    /// Reads to end.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns>string.</returns>
    public static string ReadToEnd(this Stream stream, Encoding encoding)
    {
        if (stream is MemoryStream ms)
            return ms.ReadToEnd();

        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        using var reader = new StreamReader(stream, encoding, true, DefaultBufferSize, true);
        return reader.ReadToEnd();
    }


    /// <summary>
    /// Copies to new memory stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns>System.IO.MemoryStream.</returns>
    public static MemoryStream CopyToNewMemoryStream(this Stream stream)
    {
        var ms = MemoryStreamFactory.GetStream();
        stream.CopyTo(ms);
        ms.Position = 0;
        return ms;
    }

    /// <summary>
    /// Copy to new memory stream as an asynchronous operation.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async static Task<MemoryStream> CopyToNewMemoryStreamAsync(this Stream stream)
    {
        var ms = MemoryStreamFactory.GetStream();
        await stream.CopyToAsync(ms).ConfigAwait();
        ms.Position = 0;
        return ms;
    }
}