// ***********************************************************************
// <copyright file="FileSystemVirtualFiles.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using ServiceStack.VirtualPath;

namespace ServiceStack.IO
{
    using ServiceStack.Text;

    /// <summary>
    /// Class FileSystemVirtualFiles.
    /// Implements the <see cref="ServiceStack.VirtualPath.AbstractVirtualPathProviderBase" />
    /// Implements the <see cref="ServiceStack.IO.IVirtualFiles" />
    /// </summary>
    /// <seealso cref="ServiceStack.VirtualPath.AbstractVirtualPathProviderBase" />
    /// <seealso cref="ServiceStack.IO.IVirtualFiles" />
    public class FileSystemVirtualFiles
        : AbstractVirtualPathProviderBase, IVirtualFiles
    {
        /// <summary>
        /// The root dir information
        /// </summary>
        protected DirectoryInfo RootDirInfo;
        /// <summary>
        /// The root dir
        /// </summary>
        protected FileSystemVirtualDirectory RootDir;

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
        /// Initializes a new instance of the <see cref="FileSystemVirtualFiles"/> class.
        /// </summary>
        /// <param name="rootDirectoryPath">The root directory path.</param>
        public FileSystemVirtualFiles(string rootDirectoryPath)
            : this(new DirectoryInfo(rootDirectoryPath))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemVirtualFiles"/> class.
        /// </summary>
        /// <param name="rootDirInfo">The root dir information.</param>
        /// <exception cref="System.ArgumentNullException">rootDirInfo</exception>
        public FileSystemVirtualFiles(DirectoryInfo rootDirInfo)
        {
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
        /// Directories the exists.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool DirectoryExists(string virtualPath)
        {
            var isDirectory = Directory.Exists(RootDirectory.RealPath.CombineWith(SanitizePath(virtualPath)));
            return isDirectory;
        }

        /// <summary>
        /// Files the exists.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool FileExists(string virtualPath)
        {
            var isFile = File.Exists(RootDirectory.RealPath.CombineWith(SanitizePath(virtualPath)));
            return isFile;
        }

        /// <summary>
        /// Ensures the directory.
        /// </summary>
        /// <param name="dirPath">The dir path.</param>
        /// <returns>System.String.</returns>
        public string EnsureDirectory(string dirPath)
        {
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            return dirPath;
        }

        /// <summary>
        /// Writes the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="textContents">The text contents.</param>
        public void WriteFile(string filePath, string textContents)
        {
            var realFilePath = RootDir.RealPath.CombineWith(filePath);
            EnsureDirectory(Path.GetDirectoryName(realFilePath));
            File.WriteAllText(realFilePath, textContents);
        }

        /// <summary>
        /// Writes the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="stream">The stream.</param>
        public void WriteFile(string filePath, Stream stream)
        {
            var realFilePath = RootDir.RealPath.CombineWith(filePath);
            EnsureDirectory(Path.GetDirectoryName(realFilePath));
            using (var fs = File.Open(realFilePath, FileMode.Create, FileAccess.Write))
            {
                stream.WriteTo(fs);
            }
        }

        /// <summary>
        /// Writes the files.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <param name="toPath">To path.</param>
        public void WriteFiles(IEnumerable<IVirtualFile> files, Func<IVirtualFile, string> toPath = null)
        {
            this.CopyFrom(files, toPath);
        }

        /// <summary>
        /// Appends the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="textContents">The text contents.</param>
        public void AppendFile(string filePath, string textContents)
        {
            var realFilePath = RootDir.RealPath.CombineWith(filePath);
            EnsureDirectory(Path.GetDirectoryName(realFilePath));
            File.AppendAllText(realFilePath, textContents);
        }

        /// <summary>
        /// Appends the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="stream">The stream.</param>
        public void AppendFile(string filePath, Stream stream)
        {
            var realFilePath = RootDir.RealPath.CombineWith(filePath);
            EnsureDirectory(Path.GetDirectoryName(realFilePath));
            using (var fs = new FileStream(realFilePath, FileMode.Append))
            {
                stream.WriteTo(fs);
            }
        }

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void DeleteFile(string filePath)
        {
            var realFilePath = RootDir.RealPath.CombineWith(filePath);
            try
            {
                File.Delete(realFilePath);
            }
            catch (Exception /*ignore*/)
            {
            }
        }

        /// <summary>
        /// Deletes the files.
        /// </summary>
        /// <param name="filePaths">The file paths.</param>
        public void DeleteFiles(IEnumerable<string> filePaths)
        {
            filePaths.Each(DeleteFile);
        }

        /// <summary>
        /// Deletes the folder.
        /// </summary>
        /// <param name="dirPath">The dir path.</param>
        public void DeleteFolder(string dirPath)
        {
            var realPath = RootDir.RealPath.CombineWith(dirPath);
#if NET5_0_OR_GREATER
            // Doesn't properly recursively delete nested dirs/files on .NET Core (win at least)
            if (Directory.Exists(realPath))
                DeleteDirectoryRecursive(realPath);
#else
            if (Directory.Exists(realPath))
                Directory.Delete(realPath, recursive: true);
#endif
        }

        /// <summary>
        /// Deletes the directory recursive.
        /// </summary>
        /// <param name="path">The path.</param>
        public static void DeleteDirectoryRecursive(string path)
        {
            //modified from https://stackoverflow.com/a/1703799/85785
            foreach (var directory in Directory.GetDirectories(path))
            {
                var files = Directory.GetFiles(directory);
                foreach (var file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                }

                DeleteDirectoryRecursive(directory);
            }

            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException)
            {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(path, true);
            }
        }

        /// <summary>
        /// Asserts the directory.
        /// </summary>
        /// <param name="dirPath">The dir path.</param>
        /// <param name="timeoutMs">The timeout ms.</param>
        /// <returns>System.String.</returns>
        public static string AssertDirectory(string dirPath, int timeoutMs = 1000)
        {
            if (string.IsNullOrEmpty(dirPath))
                return null;

            try
            {
                ExecUtils.RetryOnException(() =>
                {
                    if (!Directory.Exists(dirPath))
                        Directory.CreateDirectory(dirPath);
                }, TimeSpan.FromMilliseconds(timeoutMs));
                return dirPath;
            }
            catch (TimeoutException e)
            {
                throw e.InnerException ?? e;
            }
        }

    }
}
