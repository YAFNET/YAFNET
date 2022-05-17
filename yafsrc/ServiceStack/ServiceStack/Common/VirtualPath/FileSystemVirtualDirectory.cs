// ***********************************************************************
// <copyright file="FileSystemVirtualDirectory.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ServiceStack.IO;
using ServiceStack.Logging;
using ServiceStack.Text;

namespace ServiceStack.VirtualPath;

/// <summary>
/// Class FileSystemVirtualDirectory.
/// Implements the <see cref="ServiceStack.VirtualPath.AbstractVirtualDirectoryBase" />
/// </summary>
/// <seealso cref="ServiceStack.VirtualPath.AbstractVirtualDirectoryBase" />
public class FileSystemVirtualDirectory : AbstractVirtualDirectoryBase
{
    /// <summary>
    /// The log
    /// </summary>
    private static ILog Log = LogManager.GetLogger(typeof(FileSystemVirtualDirectory));

    /// <summary>
    /// The backing dir information
    /// </summary>
    protected DirectoryInfo BackingDirInfo;

    /// <summary>
    /// Gets the files.
    /// </summary>
    /// <value>The files.</value>
    public override IEnumerable<IVirtualFile> Files => this.Where(n => n.IsDirectory == false).Cast<IVirtualFile>();

    /// <summary>
    /// Gets the directories.
    /// </summary>
    /// <value>The directories.</value>
    public override IEnumerable<IVirtualDirectory> Directories => this.Where(n => n.IsDirectory).Cast<IVirtualDirectory>();

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public override string Name => BackingDirInfo.Name;

    /// <summary>
    /// Gets the last modified.
    /// </summary>
    /// <value>The last modified.</value>
    public override DateTime LastModified => BackingDirInfo.LastWriteTimeUtc;

    /// <summary>
    /// Gets the real path.
    /// </summary>
    /// <value>The real path.</value>
    public override string RealPath => BackingDirInfo.FullName;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemVirtualDirectory"/> class.
    /// </summary>
    /// <param name="owningProvider">The owning provider.</param>
    /// <param name="parentDirectory">The parent directory.</param>
    /// <param name="dInfo">The d information.</param>
    /// <exception cref="System.ArgumentNullException">dInfo</exception>
    public FileSystemVirtualDirectory(IVirtualPathProvider owningProvider, IVirtualDirectory parentDirectory, DirectoryInfo dInfo)
        : base(owningProvider, parentDirectory)
    {
        this.BackingDirInfo = dInfo ?? throw new ArgumentNullException(nameof(dInfo));
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public override IEnumerator<IVirtualNode> GetEnumerator()
    {
        var directoryNodes = GetDirectories()
            .Select(dInfo => new FileSystemVirtualDirectory(VirtualPathProvider, this, dInfo))
            .Where(x => !x.ShouldSkipPath());

        var fileNodes = GetFiles()
            .Select(fInfo => new FileSystemVirtualFile(VirtualPathProvider, this, fInfo));

        return directoryNodes.Cast<IVirtualNode>()
            .Union(fileNodes.Cast<IVirtualNode>())
            .GetEnumerator();
    }

    /// <summary>
    /// Gets the files.
    /// </summary>
    /// <returns>FileInfo[].</returns>
    private FileInfo[] GetFiles()
    {
        try
        {
            return BackingDirInfo.GetFiles();
        }
        catch (Exception ex)
        {
            //Possible exception from scanning symbolic links
            Log.Warn($"Unable to GetFiles for {RealPath}", ex);
            return TypeConstants<FileInfo>.EmptyArray;
        }
    }

    /// <summary>
    /// Gets the directories.
    /// </summary>
    /// <returns>DirectoryInfo[].</returns>
    private DirectoryInfo[] GetDirectories()
    {
        try
        {
            return BackingDirInfo.GetDirectories();
        }
        catch (Exception ex)
        {
            //Possible exception from scanning symbolic links
            Log.Warn($"Unable to GetDirectories for {RealPath}", ex);
            return TypeConstants<DirectoryInfo>.EmptyArray;
        }
    }

    /// <summary>
    /// Gets the file from backing directory or default.
    /// </summary>
    /// <param name="fName">Name of the f.</param>
    /// <returns>IVirtualFile.</returns>
    protected override IVirtualFile GetFileFromBackingDirectoryOrDefault(string fName)
    {
        var fInfo = EnumerateFiles(fName).FirstOrDefault();

        return fInfo != null
                   ? new FileSystemVirtualFile(VirtualPathProvider, this, fInfo)
                   : null;
    }

    /// <summary>
    /// Gets the matching files in dir.
    /// </summary>
    /// <param name="globPattern">The glob pattern.</param>
    /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
    protected override IEnumerable<IVirtualFile> GetMatchingFilesInDir(string globPattern)
    {
        try
        {
            if (globPattern.IndexOf('/') >= 0)
            {
                var dirPath = globPattern.LastLeftPart("/");
                var fileNameSearch = globPattern.LastRightPart("/");
                var dir = GetDirectory(dirPath);

                if (dir != null)
                {
                    var matchingFilesInBackingDir = ((FileSystemVirtualDirectory)dir).EnumerateFiles(fileNameSearch)
                        .Select(fInfo => (IVirtualFile)new FileSystemVirtualFile(VirtualPathProvider, dir, fInfo));

                    return matchingFilesInBackingDir;
                }

                return TypeConstants<IVirtualFile>.EmptyArray;
            }
            else
            {
                var matchingFilesInBackingDir = EnumerateFiles(globPattern)
                    .Select(fInfo => (IVirtualFile)new FileSystemVirtualFile(VirtualPathProvider, this, fInfo));

                return matchingFilesInBackingDir;
            }
        }
        catch (Exception ex)
        {
            //Possible exception from scanning symbolic links
            Log.Warn($"Unable to scan for {globPattern} in {RealPath}", ex);
            return TypeConstants<IVirtualFile>.EmptyArray;
        }
    }

    /// <summary>
    /// Gets the directory from backing directory or default.
    /// </summary>
    /// <param name="dName">Name of the d.</param>
    /// <returns>IVirtualDirectory.</returns>
    protected override IVirtualDirectory GetDirectoryFromBackingDirectoryOrDefault(string dName)
    {
        var dInfo = EnumerateDirectories(dName)
            .FirstOrDefault();

        return dInfo != null
                   ? new FileSystemVirtualDirectory(VirtualPathProvider, this, dInfo)
                   : null;
    }

    /// <summary>
    /// Enumerates the files.
    /// </summary>
    /// <param name="pattern">The pattern.</param>
    /// <returns>IEnumerable&lt;FileInfo&gt;.</returns>
    public IEnumerable<FileInfo> EnumerateFiles(string pattern)
    {
        return BackingDirInfo.GetFiles(pattern, SearchOption.TopDirectoryOnly);
    }

    /// <summary>
    /// Enumerates the directories.
    /// </summary>
    /// <param name="dirName">Name of the dir.</param>
    /// <returns>IEnumerable&lt;DirectoryInfo&gt;.</returns>
    public IEnumerable<DirectoryInfo> EnumerateDirectories(string dirName)
    {
        if (dirName[dirName.Length - 1] == ':')
        {
            var dir = new DirectoryInfo(dirName + Path.DirectorySeparatorChar);
            var subDirs = dir.GetDirectories();
        }

        return BackingDirInfo.GetDirectories(dirName, SearchOption.TopDirectoryOnly);
    }
}