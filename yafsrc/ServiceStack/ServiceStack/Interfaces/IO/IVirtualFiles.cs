// ***********************************************************************
// <copyright file="IVirtualFiles.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;

namespace ServiceStack.IO
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface IVirtualFiles
    /// Implements the <see cref="ServiceStack.IO.IVirtualPathProvider" />
    /// </summary>
    /// <seealso cref="ServiceStack.IO.IVirtualPathProvider" />
    public interface IVirtualFiles : IVirtualPathProvider
    {
        Task WriteFileAsync(string filePath, object contents, CancellationToken token = default);

        /// <summary>
        /// Writes the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="textContents">The text contents.</param>
        void WriteFile(string filePath, string textContents);

        /// <summary>
        /// Writes the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="stream">The stream.</param>
        void WriteFile(string filePath, Stream stream);

        /// <summary>
        /// Contents can be either:
        /// string, ReadOnlyMemory&lt;char&gt;, byte[], `ReadOnlyMemory&lt;byte&gt;, Stream or IVirtualFile
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="contents">The contents.</param>
        void WriteFile(string filePath, object contents);

        /// <summary>
        /// Writes the files.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <param name="toPath">To path.</param>
        void WriteFiles(IEnumerable<IVirtualFile> files, Func<IVirtualFile, string> toPath = null);

        /// <summary>
        /// Writes the files.
        /// </summary>
        /// <param name="textFiles">The text files.</param>
        void WriteFiles(Dictionary<string, string> textFiles);
        /// <summary>
        /// Writes the files.
        /// </summary>
        /// <param name="files">The files.</param>
        void WriteFiles(Dictionary<string, object> files);

        /// <summary>
        /// Appends the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="textContents">The text contents.</param>
        void AppendFile(string filePath, string textContents);

        /// <summary>
        /// Appends the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="stream">The stream.</param>
        void AppendFile(string filePath, Stream stream);

        /// <summary>
        /// Contents can be either:
        /// string, ReadOnlyMemory&lt;char&gt;, byte[], `ReadOnlyMemory&lt;byte&gt;, Stream or IVirtualFile
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="contents">The contents.</param>
        void AppendFile(string filePath, object contents);

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        void DeleteFile(string filePath);

        /// <summary>
        /// Deletes the files.
        /// </summary>
        /// <param name="filePaths">The file paths.</param>
        void DeleteFiles(IEnumerable<string> filePaths);

        /// <summary>
        /// Deletes the folder.
        /// </summary>
        /// <param name="dirPath">The dir path.</param>
        void DeleteFolder(string dirPath);
    }
}