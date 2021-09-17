// ***********************************************************************
// <copyright file="AbstractVirtualFileBase.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using ServiceStack.IO;
using ServiceStack.Text;
using ServiceStack.VirtualPath;

namespace ServiceStack.VirtualPath
{
    /// <summary>
    /// Class AbstractVirtualFileBase.
    /// Implements the <see cref="ServiceStack.IO.IVirtualFile" />
    /// </summary>
    /// <seealso cref="ServiceStack.IO.IVirtualFile" />
    public abstract class AbstractVirtualFileBase : IVirtualFile
    {
        /// <summary>
        /// Gets or sets the scan skip paths.
        /// </summary>
        /// <value>The scan skip paths.</value>
        public static List<string> ScanSkipPaths { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the virtual path provider.
        /// </summary>
        /// <value>The virtual path provider.</value>
        public IVirtualPathProvider VirtualPathProvider { get; set; }

        /// <summary>
        /// The file extension without '.' prefix
        /// </summary>
        /// <value>The extension.</value>
        public virtual string Extension => Name.LastRightPart('.');

        /// <summary>
        /// Gets or sets the directory.
        /// </summary>
        /// <value>The directory.</value>
        public IVirtualDirectory Directory { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public abstract string Name { get; }
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
        public virtual bool IsDirectory => false;
        /// <summary>
        /// Gets the last modified.
        /// </summary>
        /// <value>The last modified.</value>
        public abstract DateTime LastModified { get; }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        public abstract long Length { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractVirtualFileBase"/> class.
        /// </summary>
        /// <param name="owningProvider">The owning provider.</param>
        /// <param name="directory">The directory.</param>
        /// <exception cref="System.ArgumentNullException">owningProvider</exception>
        /// <exception cref="System.ArgumentNullException">directory</exception>
        protected AbstractVirtualFileBase(IVirtualPathProvider owningProvider, IVirtualDirectory directory)
        {
            this.VirtualPathProvider = owningProvider ?? throw new ArgumentNullException(nameof(owningProvider));
            this.Directory = directory ?? throw new ArgumentNullException(nameof(directory));
        }

        /// <summary>
        /// Gets the file hash.
        /// </summary>
        /// <returns>System.String.</returns>
        public virtual string GetFileHash()
        {
            using var stream = OpenRead();
            return stream.ToMd5Hash();
        }

        /// <summary>
        /// Opens the text.
        /// </summary>
        /// <returns>StreamReader.</returns>
        public virtual StreamReader OpenText()
        {
            return new StreamReader(OpenRead());
        }

        /// <summary>
        /// Reads all text.
        /// </summary>
        /// <returns>System.String.</returns>
        public virtual string ReadAllText()
        {
            using var reader = OpenText();
            var text = reader.ReadToEnd();
            return text;
        }

        /// <summary>
        /// Reads all bytes.
        /// </summary>
        /// <returns>System.Byte[].</returns>
        public virtual byte[] ReadAllBytes()
        {
            using var stream = OpenRead();
            return stream.ReadFully();
        }

        /// <summary>
        /// Opens the read.
        /// </summary>
        /// <returns>Stream.</returns>
        public abstract Stream OpenRead();

        /// <summary>
        /// Returns ReadOnlyMemory&lt;byte&gt; for binary files or
        /// ReadOnlyMemory&lt;char&gt; for text files
        /// </summary>
        /// <returns>System.Object.</returns>
        public virtual object GetContents()
        {
            using var stream = OpenRead();
            var romBytes = stream.ReadFullyAsMemory();
            if (MimeTypes.IsBinary(MimeTypes.GetMimeType(Extension)))
                return romBytes;

            return MemoryProvider.Instance.FromUtf8(romBytes.Span);
        }

        /// <summary>
        /// Gets the virtual path to root.
        /// </summary>
        /// <returns>System.String.</returns>
        protected virtual string GetVirtualPathToRoot()
        {
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
            var parentPath = Directory != null ? pathSel(Directory) : string.Empty;
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
            if (obj is not AbstractVirtualFileBase other)
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
        /// Refresh file stats for this node if supported
        /// </summary>
        public virtual void Refresh()
        {
        }
    }
}

namespace ServiceStack
{
    /// <summary>
    /// Class VirtualFileExtensions.
    /// </summary>
    public static class VirtualFileExtensions
    {
        /// <summary>
        /// Shoulds the skip path.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool ShouldSkipPath(this IVirtualNode node)
        {
            foreach (var skipPath in AbstractVirtualFileBase.ScanSkipPaths)
            {
                if (node.VirtualPath.StartsWith(skipPath.TrimStart('/'), StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets all root directories.
        /// </summary>
        /// <param name="vfs">The VFS.</param>
        /// <returns>IVirtualDirectory[].</returns>
        public static IVirtualDirectory[] GetAllRootDirectories(this IVirtualPathProvider vfs) => vfs is MultiVirtualFiles mvfs
            ? mvfs.ChildProviders.Select(x => x.RootDirectory).ToArray()
            : new[] { vfs.RootDirectory };

        /// <summary>
        /// Gets the virtual file source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vfs">The VFS.</param>
        /// <returns>T.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetVirtualFileSource<T>(this IVirtualPathProvider vfs) where T : class => vfs as T ??
            (vfs is MultiVirtualFiles mvfs ? mvfs.ChildProviders.FirstOrDefault(x => x is T) as T : null);

        /// <summary>
        /// Gets the memory virtual files.
        /// </summary>
        /// <param name="vfs">The VFS.</param>
        /// <returns>MemoryVirtualFiles.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MemoryVirtualFiles GetMemoryVirtualFiles(this IVirtualPathProvider vfs) =>
            vfs.GetVirtualFileSource<MemoryVirtualFiles>();

        /// <summary>
        /// Gets the file system virtual files.
        /// </summary>
        /// <param name="vfs">The VFS.</param>
        /// <returns>FileSystemVirtualFiles.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FileSystemVirtualFiles GetFileSystemVirtualFiles(this IVirtualPathProvider vfs) =>
            vfs.GetVirtualFileSource<FileSystemVirtualFiles>();

        /// <summary>
        /// Gets the gist virtual files.
        /// </summary>
        /// <param name="vfs">The VFS.</param>
        /// <returns>GistVirtualFiles.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GistVirtualFiles GetGistVirtualFiles(this IVirtualPathProvider vfs) =>
            vfs.GetVirtualFileSource<GistVirtualFiles>();

        /// <summary>
        /// Gets the resource virtual files.
        /// </summary>
        /// <param name="vfs">The VFS.</param>
        /// <returns>ResourceVirtualFiles.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResourceVirtualFiles GetResourceVirtualFiles(this IVirtualPathProvider vfs) =>
            vfs.GetVirtualFileSource<ResourceVirtualFiles>();

        /// <summary>
        /// Gets the text contents as memory.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>ReadOnlyMemory&lt;System.Char&gt;.</returns>
        public static ReadOnlyMemory<char> GetTextContentsAsMemory(this IVirtualFile file)
        {
            var contents = file.GetContents();
            var span = contents is ReadOnlyMemory<char> rom
                ? rom
                : contents is string s
                    ? s.AsMemory()
                    : file.ReadAllText().AsMemory();
            return span;
        }

        /// <summary>
        /// Gets the bytes contents as memory.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>ReadOnlyMemory&lt;System.Byte&gt;.</returns>
        public static ReadOnlyMemory<byte> GetBytesContentsAsMemory(this IVirtualFile file)
        {
            var contents = file.GetContents();
            var span = contents is ReadOnlyMemory<byte> rom
                ? rom
                : contents is ReadOnlyMemory<char> romChars
                    ? MemoryProvider.Instance.ToUtf8(romChars.Span)
                    : contents is string s
                        ? MemoryProvider.Instance.ToUtf8(s.AsSpan())
                        : file.ReadAllBytes().AsMemory();
            return span;
        }

        /// <summary>
        /// Gets the bytes contents as bytes.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] GetBytesContentsAsBytes(this IVirtualFile file)
        {
            if (file is InMemoryVirtualFile m)
                return m.ByteContents ?? MemoryProvider.Instance.ToUtf8Bytes(m.TextContents.AsSpan());
            if (file is GistVirtualFile g && g.Stream != null)
                return ((MemoryStream)g.Stream).GetBufferAsBytes();

            var contents = file.GetContents();
            var bytes = contents is ReadOnlyMemory<byte> rom
                ? rom.ToArray()
                : contents is ReadOnlyMemory<char> romChars
                    ? MemoryProvider.Instance.ToUtf8(romChars.Span).ToArray()
                    : contents is string s
                        ? MemoryProvider.Instance.ToUtf8(s.AsSpan()).ToArray()
                        : file.ReadAllBytes();
            return bytes;
        }

    }

}