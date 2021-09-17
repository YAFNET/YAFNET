// ***********************************************************************
// <copyright file="IVirtualPathProvider.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace ServiceStack.IO
{
    /// <summary>
    /// Interface IVirtualPathProvider
    /// </summary>
    public interface IVirtualPathProvider
    {
        /// <summary>
        /// Gets the root directory.
        /// </summary>
        /// <value>The root directory.</value>
        IVirtualDirectory RootDirectory { get; }
        /// <summary>
        /// Gets the virtual path separator.
        /// </summary>
        /// <value>The virtual path separator.</value>
        string VirtualPathSeparator { get; }
        /// <summary>
        /// Gets the real path separator.
        /// </summary>
        /// <value>The real path separator.</value>
        string RealPathSeparator { get; }

        /// <summary>
        /// Combines the virtual path.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <param name="relativePath">The relative path.</param>
        /// <returns>System.String.</returns>
        string CombineVirtualPath(string basePath, string relativePath);

        /// <summary>
        /// Files the exists.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool FileExists(string virtualPath);
        /// <summary>
        /// Directories the exists.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool DirectoryExists(string virtualPath);

        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>IVirtualFile.</returns>
        IVirtualFile GetFile(string virtualPath);
        /// <summary>
        /// Gets the file hash.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>System.String.</returns>
        string GetFileHash(string virtualPath);
        /// <summary>
        /// Gets the file hash.
        /// </summary>
        /// <param name="virtualFile">The virtual file.</param>
        /// <returns>System.String.</returns>
        string GetFileHash(IVirtualFile virtualFile);

        /// <summary>
        /// Gets the directory.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>IVirtualDirectory.</returns>
        IVirtualDirectory GetDirectory(string virtualPath);

        /// <summary>
        /// Gets all matching files.
        /// </summary>
        /// <param name="globPattern">The glob pattern.</param>
        /// <param name="maxDepth">The maximum depth.</param>
        /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
        IEnumerable<IVirtualFile> GetAllMatchingFiles(string globPattern, int maxDepth = Int32.MaxValue);

        /// <summary>
        /// Gets all files.
        /// </summary>
        /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
        IEnumerable<IVirtualFile> GetAllFiles();
        /// <summary>
        /// Gets the root files.
        /// </summary>
        /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
        IEnumerable<IVirtualFile> GetRootFiles();
        /// <summary>
        /// Gets the root directories.
        /// </summary>
        /// <returns>IEnumerable&lt;IVirtualDirectory&gt;.</returns>
        IEnumerable<IVirtualDirectory> GetRootDirectories();

        /// <summary>
        /// Determines whether [is shared file] [the specified virtual file].
        /// </summary>
        /// <param name="virtualFile">The virtual file.</param>
        /// <returns><c>true</c> if [is shared file] [the specified virtual file]; otherwise, <c>false</c>.</returns>
        bool IsSharedFile(IVirtualFile virtualFile);
        /// <summary>
        /// Determines whether [is view file] [the specified virtual file].
        /// </summary>
        /// <param name="virtualFile">The virtual file.</param>
        /// <returns><c>true</c> if [is view file] [the specified virtual file]; otherwise, <c>false</c>.</returns>
        bool IsViewFile(IVirtualFile virtualFile);
    }
}
