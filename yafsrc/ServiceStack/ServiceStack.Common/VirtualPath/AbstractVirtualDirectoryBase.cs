// ***********************************************************************
// <copyright file="AbstractVirtualDirectoryBase.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using ServiceStack.IO;

namespace ServiceStack.VirtualPath
{
    /// <summary>
    /// Class AbstractVirtualDirectoryBase.
    /// Implements the <see cref="ServiceStack.IO.IVirtualDirectory" />
    /// </summary>
    /// <seealso cref="ServiceStack.IO.IVirtualDirectory" />
    public abstract class AbstractVirtualDirectoryBase : IVirtualDirectory
    {
        /// <summary>
        /// The virtual path provider
        /// </summary>
        protected IVirtualPathProvider VirtualPathProvider;
        /// <summary>
        /// Gets or sets the parent directory.
        /// </summary>
        /// <value>The parent directory.</value>
        public IVirtualDirectory ParentDirectory { get; set; }
        /// <summary>
        /// Gets the directory.
        /// </summary>
        /// <value>The directory.</value>
        public IVirtualDirectory Directory => this;

        /// <summary>
        /// Gets the last modified.
        /// </summary>
        /// <value>The last modified.</value>
        public abstract DateTime LastModified { get; }
        /// <summary>
        /// Gets the virtual path.
        /// </summary>
        /// <value>The virtual path.</value>
        public virtual string VirtualPath => GetVirtualPathToRoot();
        /// <summary>
        /// Gets the real path.
        /// </summary>
        /// <value>The real path.</value>
        public virtual string RealPath => GetRealPathToRoot();

        /// <summary>
        /// Gets a value indicating whether this instance is directory.
        /// </summary>
        /// <value><c>true</c> if this instance is directory; otherwise, <c>false</c>.</value>
        public virtual bool IsDirectory => true;
        /// <summary>
        /// Gets a value indicating whether this instance is root.
        /// </summary>
        /// <value><c>true</c> if this instance is root; otherwise, <c>false</c>.</value>
        public virtual bool IsRoot => ParentDirectory == null;

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <value>The files.</value>
        public abstract IEnumerable<IVirtualFile> Files { get; }
        /// <summary>
        /// Gets the directories.
        /// </summary>
        /// <value>The directories.</value>
        public abstract IEnumerable<IVirtualDirectory> Directories { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public abstract string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractVirtualDirectoryBase"/> class.
        /// </summary>
        /// <param name="owningProvider">The owning provider.</param>
        protected AbstractVirtualDirectoryBase(IVirtualPathProvider owningProvider)
            : this(owningProvider, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractVirtualDirectoryBase"/> class.
        /// </summary>
        /// <param name="owningProvider">The owning provider.</param>
        /// <param name="parentDirectory">The parent directory.</param>
        /// <exception cref="System.ArgumentNullException">owningProvider</exception>
        protected AbstractVirtualDirectoryBase(IVirtualPathProvider owningProvider, IVirtualDirectory parentDirectory)
        {
            this.VirtualPathProvider = owningProvider ?? throw new ArgumentNullException(nameof(owningProvider));
            this.ParentDirectory = parentDirectory;
        }

        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>IVirtualFile.</returns>
        public virtual IVirtualFile GetFile(string virtualPath)
        {
            var tokens = virtualPath.TokenizeVirtualPath(VirtualPathProvider);
            return GetFile(tokens);
        }

        /// <summary>
        /// Gets the directory.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>IVirtualDirectory.</returns>
        public virtual IVirtualDirectory GetDirectory(string virtualPath)
        {
            var tokens = virtualPath.TokenizeVirtualPath(VirtualPathProvider);
            return GetDirectory(tokens);
        }

        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>IVirtualFile.</returns>
        public virtual IVirtualFile GetFile(Stack<string> virtualPath)
        {
            if (virtualPath.Count == 0)
                return null;

            var pathToken = virtualPath.Pop();
            if (virtualPath.Count == 0)
                return GetFileFromBackingDirectoryOrDefault(pathToken);

            var virtDir = GetDirectoryFromBackingDirectoryOrDefault(pathToken);
            return virtDir?.GetFile(virtualPath);
        }

        /// <summary>
        /// Gets the directory.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>IVirtualDirectory.</returns>
        public virtual IVirtualDirectory GetDirectory(Stack<string> virtualPath)
        {
            if (virtualPath.Count == 0)
                return null;

            var pathToken = virtualPath.Pop();

            var virtDir = GetDirectoryFromBackingDirectoryOrDefault(pathToken);
            if (virtDir == null)
                return null;

            return virtualPath.Count == 0
                ? virtDir
                : virtDir.GetDirectory(virtualPath);
        }

        /// <summary>
        /// Gets all matching files.
        /// </summary>
        /// <param name="globPattern">The glob pattern.</param>
        /// <param name="maxDepth">The maximum depth.</param>
        /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
        public virtual IEnumerable<IVirtualFile> GetAllMatchingFiles(string globPattern, int maxDepth = Int32.MaxValue)
        {
            if (maxDepth == 0)
                yield break;

            foreach (var f in GetMatchingFilesInDir(globPattern))
                yield return f;

            foreach (var childDir in Directories)
            {
                var matchingFilesInChildDir = childDir.GetAllMatchingFiles(globPattern, maxDepth - 1);
                foreach (var f in matchingFilesInChildDir)
                    yield return f;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets the virtual path to root.
        /// </summary>
        /// <returns>System.String.</returns>
        protected virtual string GetVirtualPathToRoot()
        {
            if (IsRoot)
                return null;

            return GetPathToRoot(VirtualPathProvider.VirtualPathSeparator, p => p.VirtualPath);
        }

        /// <summary>
        /// Gets the real path to root.
        /// </summary>
        /// <returns>System.String.</returns>
        protected virtual string GetRealPathToRoot()
        {
            return GetPathToRoot(VirtualPathProvider.RealPathSeparator, p => p.RealPath);
        }

        /// <summary>
        /// Gets the path to root.
        /// </summary>
        /// <param name="separator">The separator.</param>
        /// <param name="pathSel">The path sel.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetPathToRoot(string separator, Func<IVirtualDirectory, string> pathSel)
        {
            var parentPath = ParentDirectory != null ? pathSel(ParentDirectory) : string.Empty;
            if (parentPath == separator)
                parentPath = string.Empty;

            return parentPath == null
                ? Name
                : string.Concat(parentPath, separator, Name);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is not AbstractVirtualDirectoryBase other)
                return false;

            return other.VirtualPath == this.VirtualPath;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return VirtualPath.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return $"{RealPath} -> {VirtualPath}";
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public abstract IEnumerator<IVirtualNode> GetEnumerator();

        /// <summary>
        /// Gets the file from backing directory or default.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>IVirtualFile.</returns>
        protected abstract IVirtualFile GetFileFromBackingDirectoryOrDefault(string fileName);
        /// <summary>
        /// Gets the matching files in dir.
        /// </summary>
        /// <param name="globPattern">The glob pattern.</param>
        /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
        protected abstract IEnumerable<IVirtualFile> GetMatchingFilesInDir(string globPattern);
        /// <summary>
        /// Gets the directory from backing directory or default.
        /// </summary>
        /// <param name="directoryName">Name of the directory.</param>
        /// <returns>IVirtualDirectory.</returns>
        protected abstract IVirtualDirectory GetDirectoryFromBackingDirectoryOrDefault(string directoryName);
    }
}