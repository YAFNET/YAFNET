// ***********************************************************************
// <copyright file="VirtualFilesExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;

using ServiceStack.Text;
using ServiceStack.VirtualPath;

namespace ServiceStack.IO;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Class VirtualFilesExtensions.
/// </summary>
public static class VirtualFilesExtensions
{
    /// <summary>
    /// The error not writable
    /// </summary>
    private const string ErrorNotWritable = "{0} does not implement IVirtualFiles";

    /// <summary>
    /// Determines whether the specified file path is file.
    /// </summary>
    /// <param name="pathProvider">The path provider.</param>
    /// <param name="filePath">The file path.</param>
    /// <returns><c>true</c> if the specified file path is file; otherwise, <c>false</c>.</returns>
    public static bool IsFile(this IVirtualPathProvider pathProvider, string filePath)
    {
        return pathProvider.FileExists(filePath);
    }

    /// <summary>
    /// Determines whether the specified file path is directory.
    /// </summary>
    /// <param name="pathProvider">The path provider.</param>
    /// <param name="filePath">The file path.</param>
    /// <returns><c>true</c> if the specified file path is directory; otherwise, <c>false</c>.</returns>
    public static bool IsDirectory(this IVirtualPathProvider pathProvider, string filePath)
    {
        return pathProvider.DirectoryExists(filePath);
    }

    /// <summary>
    /// Writes the file.
    /// </summary>
    /// <param name="pathProvider">The path provider.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="textContents">The text contents.</param>
    /// <exception cref="System.InvalidOperationException"></exception>
    public static void WriteFile(this IVirtualPathProvider pathProvider, string filePath, string textContents)
    {
        if (pathProvider is not IVirtualFiles writableFs)
            throw new InvalidOperationException(ErrorNotWritable.Fmt(pathProvider.GetType().Name));

        writableFs.WriteFile(filePath, textContents);
    }

    /// <summary>
    /// Writes the file.
    /// </summary>
    /// <param name="pathProvider">The path provider.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="stream">The stream.</param>
    /// <exception cref="System.InvalidOperationException"></exception>
    public static void WriteFile(this IVirtualPathProvider pathProvider, string filePath, Stream stream)
    {
        if (pathProvider is not IVirtualFiles writableFs)
            throw new InvalidOperationException(ErrorNotWritable.Fmt(pathProvider.GetType().Name));

        writableFs.WriteFile(filePath, stream);
    }

    /// <summary>
    /// Writes the file.
    /// </summary>
    /// <param name="pathProvider">The path provider.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="bytes">The bytes.</param>
    /// <exception cref="System.InvalidOperationException"></exception>
    public static void WriteFile(this IVirtualPathProvider pathProvider, string filePath, byte[] bytes)
    {
        if (pathProvider is not IVirtualFiles writableFs)
            throw new InvalidOperationException(ErrorNotWritable.Fmt(pathProvider.GetType().Name));

        using var ms = MemoryStreamFactory.GetStream(bytes);
        writableFs.WriteFile(filePath, ms);
    }

    /// <summary>
    /// Writes the file.
    /// </summary>
    /// <param name="pathProvider">The path provider.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="text">The text.</param>
    /// <exception cref="System.InvalidOperationException"></exception>
    public static void WriteFile(this IVirtualPathProvider pathProvider, string filePath, ReadOnlyMemory<char> text)
    {
        if (pathProvider is not AbstractVirtualPathProviderBase writableFs)
            throw new InvalidOperationException(ErrorNotWritable.Fmt(pathProvider.GetType().Name));

        writableFs.WriteFile(filePath, text);
    }

    /// <summary>
    /// Writes the file.
    /// </summary>
    /// <param name="pathProvider">The path provider.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="bytes">The bytes.</param>
    /// <exception cref="System.InvalidOperationException"></exception>
    public static void WriteFile(this IVirtualPathProvider pathProvider, string filePath, ReadOnlyMemory<byte> bytes)
    {
        if (pathProvider is not AbstractVirtualPathProviderBase writableFs)
            throw new InvalidOperationException(ErrorNotWritable.Fmt(pathProvider.GetType().Name));

        writableFs.WriteFile(filePath, bytes);
    }

    /// <summary>
    /// Writes the file.
    /// </summary>
    /// <param name="pathProvider">The path provider.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="contents">The contents.</param>
    /// <exception cref="System.InvalidOperationException"></exception>
    public static void WriteFile(this IVirtualPathProvider pathProvider, string filePath, object contents)
    {
        if (pathProvider is not IVirtualFiles writableFs)
            throw new InvalidOperationException(ErrorNotWritable.Fmt(pathProvider.GetType().Name));

        writableFs.WriteFile(filePath, contents);
    }

    /// <summary>
    /// Appends the file.
    /// </summary>
    /// <param name="pathProvider">The path provider.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="textContents">The text contents.</param>
    /// <exception cref="System.InvalidOperationException"></exception>
    public static void AppendFile(this IVirtualPathProvider pathProvider, string filePath, string textContents)
    {
        if (pathProvider is not IVirtualFiles writableFs)
            throw new InvalidOperationException(ErrorNotWritable.Fmt(pathProvider.GetType().Name));

        writableFs.AppendFile(filePath, textContents);
    }

    /// <summary>
    /// Appends the file.
    /// </summary>
    /// <param name="pathProvider">The path provider.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="stream">The stream.</param>
    /// <exception cref="System.InvalidOperationException"></exception>
    public static void AppendFile(this IVirtualPathProvider pathProvider, string filePath, Stream stream)
    {
        if (pathProvider is not IVirtualFiles writableFs)
            throw new InvalidOperationException(ErrorNotWritable.Fmt(pathProvider.GetType().Name));

        writableFs.AppendFile(filePath, stream);
    }

    /// <summary>
    /// Appends the file.
    /// </summary>
    /// <param name="pathProvider">The path provider.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="bytes">The bytes.</param>
    /// <exception cref="System.InvalidOperationException"></exception>
    public static void AppendFile(this IVirtualPathProvider pathProvider, string filePath, byte[] bytes)
    {
        if (pathProvider is not IVirtualFiles writableFs)
            throw new InvalidOperationException(ErrorNotWritable.Fmt(pathProvider.GetType().Name));

        using var ms = MemoryStreamFactory.GetStream(bytes);
        writableFs.AppendFile(filePath, ms);
    }

    /// <summary>
    /// Appends the file.
    /// </summary>
    /// <param name="pathProvider">The path provider.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="contents">The contents.</param>
    /// <exception cref="System.InvalidOperationException"></exception>
    public static void AppendFile(this IVirtualPathProvider pathProvider, string filePath, object contents)
    {
        if (pathProvider is not IVirtualFiles writableFs)
            throw new InvalidOperationException(ErrorNotWritable.Fmt(pathProvider.GetType().Name));

        writableFs.AppendFile(filePath, contents);
    }

    /// <summary>
    /// Appends the file.
    /// </summary>
    /// <param name="pathProvider">The path provider.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="text">The text.</param>
    /// <exception cref="System.InvalidOperationException"></exception>
    public static void AppendFile(this IVirtualPathProvider pathProvider, string filePath, ReadOnlyMemory<char> text)
    {
        if (pathProvider is not AbstractVirtualPathProviderBase writableFs)
            throw new InvalidOperationException(ErrorNotWritable.Fmt(pathProvider.GetType().Name));

        writableFs.AppendFile(filePath, text);
    }

    /// <summary>
    /// Appends the file.
    /// </summary>
    /// <param name="pathProvider">The path provider.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="bytes">The bytes.</param>
    /// <exception cref="System.InvalidOperationException"></exception>
    public static void AppendFile(this IVirtualPathProvider pathProvider, string filePath, ReadOnlyMemory<byte> bytes)
    {
        if (pathProvider is not AbstractVirtualPathProviderBase writableFs)
            throw new InvalidOperationException(ErrorNotWritable.Fmt(pathProvider.GetType().Name));

        writableFs.AppendFile(filePath, bytes);
    }

    /// <summary>
    /// Writes the file.
    /// </summary>
    /// <param name="pathProvider">The path provider.</param>
    /// <param name="file">The file.</param>
    /// <param name="filePath">The file path.</param>
    /// <exception cref="System.InvalidOperationException"></exception>
    public static void WriteFile(this IVirtualPathProvider pathProvider, IVirtualFile file, string filePath = null)
    {
        if (pathProvider is not IVirtualFiles writableFs)
            throw new InvalidOperationException(ErrorNotWritable.Fmt(pathProvider.GetType().Name));

        using var stream = file.OpenRead();
        writableFs.WriteFile(filePath ?? file.VirtualPath, stream);
    }

    /// <summary>
    /// Deletes the file.
    /// </summary>
    /// <param name="pathProvider">The path provider.</param>
    /// <param name="filePath">The file path.</param>
    /// <exception cref="System.InvalidOperationException"></exception>
    public static void DeleteFile(this IVirtualPathProvider pathProvider, string filePath)
    {
        if (pathProvider is not IVirtualFiles writableFs)
            throw new InvalidOperationException(ErrorNotWritable.Fmt(pathProvider.GetType().Name));

        writableFs.DeleteFile(filePath);
    }

    /// <summary>
    /// Deletes the file.
    /// </summary>
    /// <param name="pathProvider">The path provider.</param>
    /// <param name="file">The file.</param>
    public static void DeleteFile(this IVirtualPathProvider pathProvider, IVirtualFile file)
    {
        pathProvider.DeleteFile(file.VirtualPath);
    }

    /// <summary>
    /// Deletes the files.
    /// </summary>
    /// <param name="pathProvider">The path provider.</param>
    /// <param name="filePaths">The file paths.</param>
    /// <exception cref="System.InvalidOperationException"></exception>
    public static void DeleteFiles(this IVirtualPathProvider pathProvider, IEnumerable<string> filePaths)
    {
        if (pathProvider is not IVirtualFiles writableFs)
            throw new InvalidOperationException(ErrorNotWritable.Fmt(pathProvider.GetType().Name));

        writableFs.DeleteFiles(filePaths);
    }

    /// <summary>
    /// Deletes the files.
    /// </summary>
    /// <param name="pathProvider">The path provider.</param>
    /// <param name="files">The files.</param>
    /// <exception cref="System.InvalidOperationException"></exception>
    public static void DeleteFiles(this IVirtualPathProvider pathProvider, IEnumerable<IVirtualFile> files)
    {
        if (pathProvider is not IVirtualFiles writableFs)
            throw new InvalidOperationException(ErrorNotWritable.Fmt(pathProvider.GetType().Name));

        writableFs.DeleteFiles(files.Map(x => x.VirtualPath));
    }

    /// <summary>
    /// Deletes the folder.
    /// </summary>
    /// <param name="pathProvider">The path provider.</param>
    /// <param name="dirPath">The dir path.</param>
    /// <exception cref="System.InvalidOperationException"></exception>
    public static void DeleteFolder(this IVirtualPathProvider pathProvider, string dirPath)
    {
        if (pathProvider is not IVirtualFiles writableFs)
            throw new InvalidOperationException(ErrorNotWritable.Fmt(pathProvider.GetType().Name));

        writableFs.DeleteFolder(dirPath);
    }

    /// <summary>
    /// Writes the files.
    /// </summary>
    /// <param name="pathProvider">The path provider.</param>
    /// <param name="textFiles">The text files.</param>
    /// <exception cref="System.InvalidOperationException"></exception>
    public static void WriteFiles(this IVirtualPathProvider pathProvider, Dictionary<string, string> textFiles)
    {
        if (pathProvider is not IVirtualFiles writableFs)
            throw new InvalidOperationException(ErrorNotWritable.Fmt(pathProvider.GetType().Name));

        writableFs.WriteFiles(textFiles);
    }

    /// <summary>
    /// Writes the files.
    /// </summary>
    /// <param name="pathProvider">The path provider.</param>
    /// <param name="files">The files.</param>
    /// <exception cref="System.InvalidOperationException"></exception>
    public static void WriteFiles(this IVirtualPathProvider pathProvider, Dictionary<string, object> files)
    {
        if (pathProvider is not IVirtualFiles writableFs)
            throw new InvalidOperationException(ErrorNotWritable.Fmt(pathProvider.GetType().Name));

        writableFs.WriteFiles(files);
    }

    /// <summary>
    /// Writes the files.
    /// </summary>
    /// <param name="pathProvider">The path provider.</param>
    /// <param name="srcFiles">The source files.</param>
    /// <param name="toPath">To path.</param>
    /// <exception cref="System.InvalidOperationException"></exception>
    public static void WriteFiles(this IVirtualPathProvider pathProvider, IEnumerable<IVirtualFile> srcFiles, Func<IVirtualFile, string> toPath = null)
    {
        if (pathProvider is not IVirtualFiles writableFs)
            throw new InvalidOperationException(ErrorNotWritable.Fmt(pathProvider.GetType().Name));

        writableFs.WriteFiles(srcFiles, toPath);
    }

    /// <summary>
    /// Copies from.
    /// </summary>
    /// <param name="pathProvider">The path provider.</param>
    /// <param name="srcFiles">The source files.</param>
    /// <param name="toPath">To path.</param>
    public static void CopyFrom(this IVirtualPathProvider pathProvider, IEnumerable<IVirtualFile> srcFiles, Func<IVirtualFile, string> toPath = null)
    {
        foreach (var file in srcFiles)
        {
            using var stream = file.OpenRead();
            var dstPath = toPath != null ? toPath(file) : file.VirtualPath;
            if (dstPath == null)
                continue;

            pathProvider.WriteFile(dstPath, stream);
        }
    }
}

/// <summary>
/// Class VirtualDirectoryExtensions.
/// </summary>
public static class VirtualDirectoryExtensions
{
    /// <summary>
    /// Get only files in this directory
    /// </summary>
    /// <param name="dir">The dir.</param>
    /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
    public static IEnumerable<IVirtualFile> GetFiles(this IVirtualDirectory dir)
    {
        return dir.Files;
    }

    /// <summary>
    /// Get only sub directories in this directory
    /// </summary>
    /// <param name="dir">The dir.</param>
    /// <returns>IEnumerable&lt;IVirtualDirectory&gt;.</returns>
    public static IEnumerable<IVirtualDirectory> GetDirectories(this IVirtualDirectory dir)
    {
        return dir.Directories;
    }

    /// <summary>
    /// Get All Files in current and all sub directories
    /// </summary>
    /// <param name="dir">The dir.</param>
    /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
    public static IEnumerable<IVirtualFile> GetAllFiles(this IVirtualDirectory dir)
    {
        if (dir != null)
        {
            foreach (var subDir in dir.GetDirectories())
            {
                foreach (var file in subDir.GetAllFiles())
                {
                    yield return file;
                }
            }

            foreach (var file in dir.Files)
            {
                yield return file;
            }
        }
    }

    // VFS Async providers only need implement, which all async APIs are routed to:
    // Task WriteFileAsync(string filePath, object contents, CancellationToken token=default);
    // E.g. see FileSystemVirtualFiles.WriteFileAsync()

    /// <summary>
    /// Write file as an asynchronous operation.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="file">The file.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public static async Task WriteFileAsync(this IVirtualFiles vfs, string filePath, IVirtualFile file, CancellationToken token = default) =>
        await vfs.WriteFileAsync(filePath, file, token).ConfigAwait();
    /// <summary>
    /// Write file as an asynchronous operation.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="textContents">The text contents.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public static async Task WriteFileAsync(this IVirtualFiles vfs, string filePath, string textContents, CancellationToken token = default) =>
        await vfs.WriteFileAsync(filePath, textContents, token).ConfigAwait();
    /// <summary>
    /// Write file as an asynchronous operation.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="textContents">The text contents.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public static async Task WriteFileAsync(this IVirtualFiles vfs, string filePath, ReadOnlyMemory<char> textContents, CancellationToken token = default) =>
        await vfs.WriteFileAsync(filePath, textContents, token).ConfigAwait();
    /// <summary>
    /// Write file as an asynchronous operation.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="binaryContents">The binary contents.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public static async Task WriteFileAsync(this IVirtualFiles vfs, string filePath, byte[] binaryContents, CancellationToken token = default) =>
        await vfs.WriteFileAsync(filePath, binaryContents, token).ConfigAwait();
    /// <summary>
    /// Write file as an asynchronous operation.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="romBytes">The rom bytes.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public static async Task WriteFileAsync(this IVirtualFiles vfs, string filePath, ReadOnlyMemory<byte> romBytes, CancellationToken token = default) =>
        await vfs.WriteFileAsync(filePath, romBytes, token).ConfigAwait();
    /// <summary>
    /// Write file as an asynchronous operation.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="stream">The stream.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public static async Task WriteFileAsync(this IVirtualFiles vfs, string filePath, Stream stream, CancellationToken token = default) =>
        await vfs.WriteFileAsync(filePath, stream, token).ConfigAwait();
}