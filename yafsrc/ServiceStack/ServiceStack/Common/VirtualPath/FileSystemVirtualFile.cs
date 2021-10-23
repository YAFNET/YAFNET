// ***********************************************************************
// <copyright file="FileSystemVirtualFile.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.IO;
using ServiceStack.IO;

namespace ServiceStack.VirtualPath
{
    /// <summary>
    /// Class FileSystemVirtualFile.
    /// Implements the <see cref="ServiceStack.VirtualPath.AbstractVirtualFileBase" />
    /// </summary>
    /// <seealso cref="ServiceStack.VirtualPath.AbstractVirtualFileBase" />
    public class FileSystemVirtualFile : AbstractVirtualFileBase
    {
        /// <summary>
        /// The backing file
        /// </summary>
        protected FileInfo BackingFile;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name => BackingFile.Name;

        /// <summary>
        /// Gets the real path.
        /// </summary>
        /// <value>The real path.</value>
        public override string RealPath => BackingFile.FullName;

        /// <summary>
        /// Gets the last modified.
        /// </summary>
        /// <value>The last modified.</value>
        public override DateTime LastModified => BackingFile.LastWriteTimeUtc;

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        public override long Length => BackingFile.Length;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemVirtualFile"/> class.
        /// </summary>
        /// <param name="owningProvider">The owning provider.</param>
        /// <param name="directory">The directory.</param>
        /// <param name="fInfo">The f information.</param>
        /// <exception cref="System.ArgumentNullException">fInfo</exception>
        public FileSystemVirtualFile(IVirtualPathProvider owningProvider, IVirtualDirectory directory, FileInfo fInfo)
            : base(owningProvider, directory)
        {
            this.BackingFile = fInfo ?? throw new ArgumentNullException(nameof(fInfo));
        }

        /// <summary>
        /// Opens the read.
        /// </summary>
        /// <returns>Stream.</returns>
        /// <exception cref="System.TimeoutException">Exceeded timeout of {VirtualPathUtils.MaxRetryOnExceptionTimeout}</exception>
        public override Stream OpenRead()
        {
            var i = 0;
            var firstAttempt = DateTime.UtcNow;
            IOException originalEx = null;

            while (DateTime.UtcNow - firstAttempt < VirtualPathUtils.MaxRetryOnExceptionTimeout)
            {
                try
                {
                    i++;
                    return BackingFile.OpenRead();
                }
                catch (IOException ex) // catch The process cannot access the file '...' because it is being used by another process.
                {
                    if (originalEx == null)
                        originalEx = ex;

                    i.SleepBackOffMultiplier();
                }
            }

            throw new TimeoutException($"Exceeded timeout of {VirtualPathUtils.MaxRetryOnExceptionTimeout}", originalEx);
        }

        /// <summary>
        /// Refresh file stats for this node if supported
        /// </summary>
        public override void Refresh()
        {
            BackingFile.Refresh();
        }
    }
}
