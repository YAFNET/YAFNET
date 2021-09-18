// ***********************************************************************
// <copyright file="ResourceVirtualFile.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.IO;
using System.Reflection;
using ServiceStack.IO;

namespace ServiceStack.VirtualPath
{
    /// <summary>
    /// Class ResourceVirtualFile.
    /// Implements the <see cref="ServiceStack.VirtualPath.AbstractVirtualFileBase" />
    /// </summary>
    /// <seealso cref="ServiceStack.VirtualPath.AbstractVirtualFileBase" />
    public class ResourceVirtualFile : AbstractVirtualFileBase
    {
        /// <summary>
        /// The backing assembly
        /// </summary>
        protected readonly Assembly BackingAssembly;
        /// <summary>
        /// The file name
        /// </summary>
        protected readonly string FileName;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name => FileName;

        /// <summary>
        /// Gets the virtual path.
        /// </summary>
        /// <value>The virtual path.</value>
        public override string VirtualPath => GetVirtualPathToRoot();

        /// <summary>
        /// Gets the real path.
        /// </summary>
        /// <value>The real path.</value>
        public override string RealPath => GetRealPathToRoot();

        /// <summary>
        /// Gets the last modified.
        /// </summary>
        /// <value>The last modified.</value>
        public override DateTime LastModified => Directory.LastModified;

        /// <summary>
        /// The length
        /// </summary>
        private long? length;
        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        public override long Length
        {
            get
            {
                if (length == null)
                {
                    using var s = OpenRead();
                    length = s.Length;
                }
                return length.Value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceVirtualFile"/> class.
        /// </summary>
        /// <param name="owningProvider">The owning provider.</param>
        /// <param name="directory">The directory.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <exception cref="System.ArgumentNullException">fileName</exception>
        /// <exception cref="System.ArgumentNullException">directory</exception>
        public ResourceVirtualFile(IVirtualPathProvider owningProvider, ResourceVirtualDirectory directory, string fileName)
            : base(owningProvider, directory)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));

            this.FileName = fileName;
            this.BackingAssembly = directory.BackingAssembly ?? throw new ArgumentNullException(nameof(directory));
        }

        /// <summary>
        /// Opens the read.
        /// </summary>
        /// <returns>Stream.</returns>
        public override Stream OpenRead()
        {
            var fullName = RealPath;
            return BackingAssembly.GetManifestResourceStream(fullName);
        }
    }
}
