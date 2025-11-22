// ***********************************************************************
// <copyright file="StreamExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ServiceStack.OrmLite.Base.Text;

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

    /// <param name="ms">The ms.</param>
    extension(MemoryStream ms)
    {
        /// <summary>
        /// Reads to end.
        /// </summary>
        /// <returns>string.</returns>
        public string ReadToEnd()
        {
            return ReadToEnd(ms, JsConfig.UTF8Encoding);
        }

        /// <summary>
        /// Reads to end.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        /// <returns>string.</returns>
        public string ReadToEnd(Encoding encoding)
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
        /// <returns>ReadOnlySpan&lt;System.Byte&gt;.</returns>
        public ReadOnlySpan<byte> GetBufferAsSpan()
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
        /// <returns>byte[].</returns>
        public byte[] GetBufferAsBytes()
        {
            if (ms.TryGetBuffer(out var buffer))
            {
                return buffer.Array;
            }

            Tracer.Instance.WriteWarning("MemoryStream in GetBufferAsBytes() wasn't created with a publiclyVisible:true byte[] buffer, falling back to slow impl");
            return ms.ToArray();
        }
    }

    /// <param name="stream">The stream.</param>
    extension(Stream stream)
    {
        /// <summary>
        /// Reads to end.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        /// <returns>string.</returns>
        public string ReadToEnd(Encoding encoding)
        {
            if (stream is MemoryStream ms)
            {
                return ms.ReadToEnd();
            }

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
        /// <returns>System.IO.MemoryStream.</returns>
        public MemoryStream CopyToNewMemoryStream()
        {
            var ms = MemoryStreamFactory.GetStream();
            stream.CopyTo(ms);
            ms.Position = 0;
            return ms;
        }

        /// <summary>
        /// Copy to new memory stream as an asynchronous operation.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task<MemoryStream> CopyToNewMemoryStreamAsync()
        {
            var ms = MemoryStreamFactory.GetStream();
            await stream.CopyToAsync(ms).ConfigAwait();
            ms.Position = 0;
            return ms;
        }
    }
}