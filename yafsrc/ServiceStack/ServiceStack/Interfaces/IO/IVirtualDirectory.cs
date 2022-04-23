// ***********************************************************************
// <copyright file="IVirtualDirectory.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.IO;

using System.Collections.Generic;

/// <summary>
/// Interface IVirtualDirectory
/// Implements the <see cref="ServiceStack.IO.IVirtualNode" />
/// Implements the <see cref="IVirtualNode" />
/// </summary>
/// <seealso cref="ServiceStack.IO.IVirtualNode" />
/// <seealso cref="IVirtualNode" />
public interface IVirtualDirectory : IVirtualNode, IEnumerable<IVirtualNode>
{
    /// <summary>
    /// Gets a value indicating whether this instance is root.
    /// </summary>
    /// <value><c>true</c> if this instance is root; otherwise, <c>false</c>.</value>
    bool IsRoot { get; }

    /// <summary>
    /// Gets the parent directory.
    /// </summary>
    /// <value>The parent directory.</value>
    IVirtualDirectory ParentDirectory { get; }

    /// <summary>
    /// Gets the files.
    /// </summary>
    /// <value>The files.</value>
    IEnumerable<IVirtualFile> Files { get; }

    /// <summary>
    /// Gets the directories.
    /// </summary>
    /// <value>The directories.</value>
    IEnumerable<IVirtualDirectory> Directories { get; }

    /// <summary>
    /// Gets the file.
    /// </summary>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns>IVirtualFile.</returns>
    IVirtualFile GetFile(string virtualPath);

    /// <summary>
    /// Gets the file.
    /// </summary>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns>IVirtualFile.</returns>
    IVirtualFile GetFile(Stack<string> virtualPath);

    /// <summary>
    /// Gets the directory.
    /// </summary>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns>IVirtualDirectory.</returns>
    IVirtualDirectory GetDirectory(string virtualPath);

    /// <summary>
    /// Gets the directory.
    /// </summary>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns>IVirtualDirectory.</returns>
    IVirtualDirectory GetDirectory(Stack<string> virtualPath);

    /// <summary>
    /// Gets all matching files.
    /// </summary>
    /// <param name="globPattern">The glob pattern.</param>
    /// <param name="maxDepth">The maximum depth.</param>
    /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
    IEnumerable<IVirtualFile> GetAllMatchingFiles(string globPattern, int maxDepth = int.MaxValue);
}