// ***********************************************************************
// <copyright file="StreamExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Text;

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Class StreamExtensions.
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    /// @jonskeet: Collection of utility methods which operate on streams.
    /// r285, February 26th 2009: http://www.yoda.arachsys.com/csharp/miscutil/
    /// </summary>
    public const int DefaultBufferSize = 8 * 1024;

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

        if (ms.TryGetBuffer(out var buffer))
        {
            return encoding.GetString(buffer.Array, buffer.Offset, buffer.Count);
        }

        Tracer.Instance.WriteWarning("MemoryStream wasn't created with a publiclyVisible:true byte[] buffer, falling back to slow impl");

        using var reader = new StreamReader(ms, encoding, true, DefaultBufferSize, true);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Gets the buffer as span.
    /// </summary>
    /// <param name="ms">The ms.</param>
    /// <returns>ReadOnlySpan&lt;System.Byte&gt;.</returns>
    public static ReadOnlySpan<byte> GetBufferAsSpan(this MemoryStream ms)
    {
        if (ms.TryGetBuffer(out var buffer))
        {
            return new ReadOnlySpan<byte>(buffer.Array, buffer.Offset, buffer.Count);
        }

        Tracer.Instance.WriteWarning("MemoryStream in GetBufferAsSpan() wasn't created with a publiclyVisible:true byte[] buffer, falling back to slow impl");
        return new ReadOnlySpan<byte>(ms.ToArray());
    }

    /// <summary>
    /// Gets the buffer as bytes.
    /// </summary>
    /// <param name="ms">The ms.</param>
    /// <returns>byte[].</returns>
    public static byte[] GetBufferAsBytes(this MemoryStream ms)
    {
        if (ms.TryGetBuffer(out var buffer))
        {
            return buffer.Array;
        }

        Tracer.Instance.WriteWarning("MemoryStream in GetBufferAsBytes() wasn't created with a publiclyVisible:true byte[] buffer, falling back to slow impl");
        return ms.ToArray();
    }

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