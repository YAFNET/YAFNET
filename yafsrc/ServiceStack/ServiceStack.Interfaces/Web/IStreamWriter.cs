// ***********************************************************************
// <copyright file="IStreamWriter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceStack.Web
{
    /// <summary>
    /// Interface IStreamWriter
    /// </summary>
    [Obsolete("Use IStreamWriterAsync")]
    public interface IStreamWriter
    {
        /// <summary>
        /// Writes to.
        /// </summary>
        /// <param name="responseStream">The response stream.</param>
        void WriteTo(Stream responseStream);
    }

    /// <summary>
    /// Interface IStreamWriterAsync
    /// </summary>
    public interface IStreamWriterAsync
    {
        /// <summary>
        /// Writes to asynchronous.
        /// </summary>
        /// <param name="responseStream">The response stream.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        Task WriteToAsync(Stream responseStream, CancellationToken token = default);
    }
}