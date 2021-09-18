// ***********************************************************************
// <copyright file="IVirtualFile.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System.IO;

namespace ServiceStack.IO
{
    /// <summary>
    /// Interface IVirtualFile
    /// Implements the <see cref="ServiceStack.IO.IVirtualNode" />
    /// </summary>
    /// <seealso cref="ServiceStack.IO.IVirtualNode" />
    public interface IVirtualFile : IVirtualNode
    {
        /// <summary>
        /// Gets the virtual path provider.
        /// </summary>
        /// <value>The virtual path provider.</value>
        IVirtualPathProvider VirtualPathProvider { get; }

        /// <summary>
        /// The file extension without '.' prefix
        /// </summary>
        /// <value>The extension.</value>
        string Extension { get; }

        /// <summary>
        /// Gets the file hash.
        /// </summary>
        /// <returns>System.String.</returns>
        string GetFileHash();

        /// <summary>
        /// Opens the read.
        /// </summary>
        /// <returns>Stream.</returns>
        Stream OpenRead();
        /// <summary>
        /// Opens the text.
        /// </summary>
        /// <returns>StreamReader.</returns>
        StreamReader OpenText();
        /// <summary>
        /// Reads all text.
        /// </summary>
        /// <returns>System.String.</returns>
        string ReadAllText();

        /// <summary>
        /// Returns ReadOnlyMemory&lt;byte&gt; for binary files or
        /// ReadOnlyMemory&lt;char&gt; for text files
        /// </summary>
        /// <returns>System.Object.</returns>
        object GetContents();

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        long Length { get; }

        /// <summary>
        /// Refresh file stats for this node if supported
        /// </summary>
        void Refresh();
    }
}
