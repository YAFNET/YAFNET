// ***********************************************************************
// <copyright file="FileSystemMapping.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using ServiceStack.IO;

namespace ServiceStack.VirtualPath
{
    using ServiceStack.Text;

    /// <summary>
    /// Class FileSystemMapping.
    /// Implements the <see cref="ServiceStack.VirtualPath.AbstractVirtualPathProviderBase" />
    /// </summary>
    /// <seealso cref="ServiceStack.VirtualPath.AbstractVirtualPathProviderBase" />
    public class FileSystemMapping : AbstractVirtualPathProviderBase
    {
        /// <summary>
        /// The root dir information
        /// </summary>
        protected readonly DirectoryInfo RootDirInfo;
        /// <summary>
        /// The root dir
        /// </summary>
        protected FileSystemVirtualDirectory RootDir;
        /// <summary>
        /// Gets the alias.
        /// </summary>
        /// <value>The alias.</value>
        public string Alias { get; private set; }

        /// <summary>
        /// Gets the root directory.
        /// </summary>
        /// <value>The root directory.</value>
        public override IVirtualDirectory RootDirectory => RootDir;
        /// <summary>
        /// Gets the virtual path separator.
        /// </summary>
        /// <value>The virtual path separator.</value>
        public override string VirtualPathSeparator => "/";
        /// <summary>
        /// Gets the real path separator.
        /// </summary>
        /// <value>The real path separator.</value>
        public override string RealPathSeparator => Convert.ToString(Path.DirectorySeparatorChar);

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemMapping"/> class.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <param name="rootDirectoryPath">The root directory path.</param>
        public FileSystemMapping(string alias, string rootDirectoryPath)
            : this(alias, new DirectoryInfo(rootDirectoryPath))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemMapping"/> class.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <param name="rootDirInfo">The root dir information.</param>
        /// <exception cref="System.ArgumentNullException">alias</exception>
        /// <exception cref="System.ArgumentNullException">rootDirInfo</exception>
        /// <exception cref="System.ArgumentException">Alias '{alias}' cannot contain directory separators</exception>
        public FileSystemMapping(string alias, DirectoryInfo rootDirInfo)
        {
            if (alias == null)
                throw new ArgumentNullException(nameof(alias));

            if (alias.IndexOfAny(new[] { '/', '\\' }) >= 0)
                throw new ArgumentException($"Alias '{alias}' cannot contain directory separators");

            this.Alias = alias;
            this.RootDirInfo = rootDirInfo ?? throw new ArgumentNullException(nameof(rootDirInfo));
            Initialize();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <exception cref="System.Exception">RootDir '{RootDirInfo.FullName}' for virtual path does not exist</exception>
        protected sealed override void Initialize()
        {
            if (!RootDirInfo.Exists)
                throw new Exception($"RootDir '{RootDirInfo.FullName}' for virtual path does not exist");

            RootDir = new FileSystemVirtualDirectory(this, null, RootDirInfo);
        }

        /// <summary>
        /// Gets the real virtual path.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>System.String.</returns>
        public string GetRealVirtualPath(string virtualPath)
        {
            virtualPath = virtualPath.TrimStart('/');
            return virtualPath.StartsWith(Alias, StringComparison.OrdinalIgnoreCase)
                ? virtualPath.Substring(Alias.Length)
                : null;
        }

        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>IVirtualFile.</returns>
        public override IVirtualFile GetFile(string virtualPath)
        {
            var nodePath = GetRealVirtualPath(virtualPath);
            return !string.IsNullOrEmpty(nodePath)
                ? base.GetFile(nodePath)
                : null;
        }

        /// <summary>
        /// Gets the directory.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>IVirtualDirectory.</returns>
        public override IVirtualDirectory GetDirectory(string virtualPath)
        {
            if (virtualPath.EqualsIgnoreCase(Alias))
                return RootDir;

            var nodePath = GetRealVirtualPath(virtualPath);
            return !string.IsNullOrEmpty(nodePath)
                ? base.GetDirectory(nodePath)
                : null;
        }

        /// <summary>
        /// Gets the root directories.
        /// </summary>
        /// <returns>IEnumerable&lt;IVirtualDirectory&gt;.</returns>
        public override IEnumerable<IVirtualDirectory> GetRootDirectories()
        {
            return new[] { new InMemoryVirtualDirectory(new MemoryVirtualFiles(), Alias), };
        }

        /// <summary>
        /// Gets the root files.
        /// </summary>
        /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
        public override IEnumerable<IVirtualFile> GetRootFiles()
        {
            return new IVirtualFile[0];
        }
    }
}
