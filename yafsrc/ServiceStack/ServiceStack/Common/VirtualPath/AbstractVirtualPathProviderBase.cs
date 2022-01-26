// ***********************************************************************
// <copyright file="AbstractVirtualPathProviderBase.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using ServiceStack.IO;
using ServiceStack.Text;

namespace ServiceStack.VirtualPath
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Class AbstractVirtualPathProviderBase.
    /// Implements the <see cref="ServiceStack.IO.IVirtualPathProvider" />
    /// </summary>
    /// <seealso cref="ServiceStack.IO.IVirtualPathProvider" />
    public abstract class AbstractVirtualPathProviderBase : IVirtualPathProvider
    {
        /// <summary>
        /// Gets the root directory.
        /// </summary>
        /// <value>The root directory.</value>
        public abstract IVirtualDirectory RootDirectory { get; }
        /// <summary>
        /// Gets the virtual path separator.
        /// </summary>
        /// <value>The virtual path separator.</value>
        public abstract string VirtualPathSeparator { get; }
        /// <summary>
        /// Gets the real path separator.
        /// </summary>
        /// <value>The real path separator.</value>
        public abstract string RealPathSeparator { get; }

        /// <summary>
        /// Combines the virtual path.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <param name="relativePath">The relative path.</param>
        /// <returns>System.String.</returns>
        public virtual string CombineVirtualPath(string basePath, string relativePath)
        {
            return string.Concat(basePath, VirtualPathSeparator, relativePath);
        }

        /// <summary>
        /// Files the exists.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool FileExists(string virtualPath)
        {
            return GetFile(SanitizePath(virtualPath)) != null;
        }

        /// <summary>
        /// Sanitizes the path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>System.String.</returns>
        public virtual string SanitizePath(string filePath)
        {
            var sanitizedPath = string.IsNullOrEmpty(filePath)
                ? null
                : filePath[0] == '/' ? filePath.Substring(1) : filePath;

            return sanitizedPath?.Replace('\\', '/');
        }

        /// <summary>
        /// Directories the exists.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool DirectoryExists(string virtualPath)
        {
            return GetDirectory(SanitizePath(virtualPath)) != null;
        }

        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>IVirtualFile.</returns>
        public virtual IVirtualFile GetFile(string virtualPath)
        {
            var virtualFile = RootDirectory.GetFile(SanitizePath(virtualPath));
            virtualFile?.Refresh();
            return virtualFile;
        }

        /// <summary>
        /// Gets the file hash.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>System.String.</returns>
        public virtual string GetFileHash(string virtualPath)
        {
            var f = GetFile(virtualPath);
            return GetFileHash(f);
        }

        /// <summary>
        /// Gets the file hash.
        /// </summary>
        /// <param name="virtualFile">The virtual file.</param>
        /// <returns>System.String.</returns>
        public virtual string GetFileHash(IVirtualFile virtualFile)
        {
            return virtualFile == null ? string.Empty : virtualFile.GetFileHash();
        }

        /// <summary>
        /// Gets the directory.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>IVirtualDirectory.</returns>
        public virtual IVirtualDirectory GetDirectory(string virtualPath)
        {
            if (string.IsNullOrEmpty(virtualPath) || virtualPath == "/")
                return RootDirectory;

            return RootDirectory.GetDirectory(SanitizePath(virtualPath));
        }

        /// <summary>
        /// Gets all matching files.
        /// </summary>
        /// <param name="globPattern">The glob pattern.</param>
        /// <param name="maxDepth">The maximum depth.</param>
        /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
        public virtual IEnumerable<IVirtualFile> GetAllMatchingFiles(string globPattern, int maxDepth = int.MaxValue)
        {
            return RootDirectory.GetAllMatchingFiles(globPattern, maxDepth);
        }

        /// <summary>
        /// Gets all files.
        /// </summary>
        /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
        public virtual IEnumerable<IVirtualFile> GetAllFiles()
        {
            return RootDirectory.GetAllMatchingFiles("*");
        }

        /// <summary>
        /// Gets the root files.
        /// </summary>
        /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
        public virtual IEnumerable<IVirtualFile> GetRootFiles()
        {
            return RootDirectory.Files;
        }

        /// <summary>
        /// Gets the root directories.
        /// </summary>
        /// <returns>IEnumerable&lt;IVirtualDirectory&gt;.</returns>
        public virtual IEnumerable<IVirtualDirectory> GetRootDirectories()
        {
            return RootDirectory.Directories;
        }

        /// <summary>
        /// Determines whether [is shared file] [the specified virtual file].
        /// </summary>
        /// <param name="virtualFile">The virtual file.</param>
        /// <returns><c>true</c> if [is shared file] [the specified virtual file]; otherwise, <c>false</c>.</returns>
        public virtual bool IsSharedFile(IVirtualFile virtualFile)
        {
            return virtualFile.RealPath != null
                && virtualFile.RealPath.Contains($"{RealPathSeparator}Shared");
        }

        /// <summary>
        /// Determines whether [is view file] [the specified virtual file].
        /// </summary>
        /// <param name="virtualFile">The virtual file.</param>
        /// <returns><c>true</c> if [is view file] [the specified virtual file]; otherwise, <c>false</c>.</returns>
        public virtual bool IsViewFile(IVirtualFile virtualFile)
        {
            return virtualFile.RealPath != null
                && virtualFile.RealPath.Contains($"{RealPathSeparator}Views");
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() => $"[{GetType().Name}: {RootDirectory.RealPath}]";

        /// <summary>
        /// Writes the files.
        /// </summary>
        /// <param name="textFiles">The text files.</param>
        /// <exception cref="System.NotSupportedException"></exception>
        public virtual void WriteFiles(Dictionary<string, string> textFiles)
        {
            var vfs = this as IVirtualFiles;
            if (vfs == null)
                throw new NotSupportedException($"{GetType().Name} does not implement IVirtualFiles");

            foreach (var entry in textFiles)
            {
                vfs.WriteFile(entry.Key, entry.Value);
            }
        }

        /// <summary>
        /// Creates the content not supported exception.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>NotSupportedException.</returns>
        protected NotSupportedException CreateContentNotSupportedException(object value) =>
            new($"Could not write '{value?.GetType().Name ?? "null"}' value. Only string, byte[], Stream or IVirtualFile content is supported.");

        protected IVirtualFiles AssertVirtualFiles()
        {
            if (this is not IVirtualFiles vfs)
                throw new NotSupportedException($"{GetType().Name} does not implement IVirtualFiles");

            return vfs;
        }
        public virtual void WriteFile(string path, ReadOnlyMemory<char> text) => AssertVirtualFiles().WriteFile(path, text.ToString());

        public virtual void WriteFile(string path, ReadOnlyMemory<byte> bytes) => AssertVirtualFiles().WriteFile(path, ToMemoryStream(bytes));

        /// <summary>
        /// Converts to memorystream.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>MemoryStream.</returns>
        private static MemoryStream ToMemoryStream(ReadOnlyMemory<byte> bytes)
        {
            var ms = MemoryProvider.Instance.ToMemoryStream(bytes.Span);
            ms.Position = 0;
            return ms;
        }

        /// <summary>
        /// Writes the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="contents">The contents.</param>
        /// <exception cref="System.NotSupportedException"></exception>
        public virtual void WriteFile(string path, object contents)
        {
            if (contents == null)
                return;

            var vfs = AssertVirtualFiles();

            if (contents is IVirtualFile vfile)
                WriteFile(path, vfile.GetContents());
            else if (contents is string textContents)
                vfs.WriteFile(path, textContents);
            else if (contents is ReadOnlyMemory<char> romChars)
                WriteFile(path, romChars);
            else if (contents is byte[] binaryContents)
            {
                using var ms = MemoryStreamFactory.GetStream(binaryContents);
                vfs.WriteFile(path, ms);
            }
            else if (contents is ReadOnlyMemory<byte> romBytes)
                WriteFile(path, romBytes);
            else if (contents is Stream stream)
                vfs.WriteFile(path, stream);
            else
                throw CreateContentNotSupportedException(contents);
        }

        /// <summary>
        /// Appends the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <exception cref="System.NotSupportedException"></exception>
        // Can implement all async APIs here
        public virtual Task WriteFileAsync(string path, object contents, CancellationToken token = default)
        {
            WriteFile(path, contents);
            return TypeConstants.EmptyTask;
        }

        /// <summary>
        /// Appends the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <exception cref="System.NotSupportedException"></exception>
        public virtual void AppendFile(string path, ReadOnlyMemory<char> text) => AssertVirtualFiles().AppendFile(path, text.ToString());

        public virtual void AppendFile(string path, ReadOnlyMemory<byte> bytes) => AssertVirtualFiles().AppendFile(path, ToMemoryStream(bytes));

        /// <summary>
        /// Appends the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="contents">The contents.</param>
        /// <exception cref="System.NotSupportedException"></exception>
        public virtual void AppendFile(string path, object contents)
        {
            var vfs = AssertVirtualFiles();

            if (contents == null)
                return;

            if (contents is IVirtualFile vfile)
                AppendFile(path, vfile.GetContents());
            else if (contents is string textContents)
                vfs.AppendFile(path, textContents);
            else if (contents is ReadOnlyMemory<char> romChars)
                AppendFile(path, romChars);
            else if (contents is byte[] binaryContents)
            {
                using var ms = MemoryStreamFactory.GetStream(binaryContents);
                vfs.AppendFile(path, ms);
            }
            else if (contents is ReadOnlyMemory<byte> romBytes)
                AppendFile(path, romBytes);
            else if (contents is Stream stream)
                vfs.AppendFile(path, stream);
            else
                throw CreateContentNotSupportedException(contents);
        }

        /// <summary>
        /// Writes the files.
        /// </summary>
        /// <param name="files">The files.</param>
        public virtual void WriteFiles(Dictionary<string, object> files)
        {
            foreach (var entry in files)
            {
                WriteFile(entry.Key, entry.Value);
            }
        }
    }
}