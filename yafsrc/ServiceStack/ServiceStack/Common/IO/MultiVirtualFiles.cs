// ***********************************************************************
// <copyright file="MultiVirtualFiles.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ServiceStack.VirtualPath;

namespace ServiceStack.IO
{
    using ServiceStack.Text;

    /// <summary>
    /// Class MultiVirtualFiles.
    /// Implements the <see cref="ServiceStack.VirtualPath.AbstractVirtualPathProviderBase" />
    /// Implements the <see cref="ServiceStack.IO.IVirtualFiles" />
    /// </summary>
    /// <seealso cref="ServiceStack.VirtualPath.AbstractVirtualPathProviderBase" />
    /// <seealso cref="ServiceStack.IO.IVirtualFiles" />
    public class MultiVirtualFiles
        : AbstractVirtualPathProviderBase, IVirtualFiles
    {
        /// <summary>
        /// Gets or sets the child providers.
        /// </summary>
        /// <value>The child providers.</value>
        public List<IVirtualPathProvider> ChildProviders { get; set; }

        /// <summary>
        /// Gets the root directory.
        /// </summary>
        /// <value>The root directory.</value>
        public override IVirtualDirectory RootDirectory => ChildProviders.FirstOrDefault().RootDirectory;

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
        /// Initializes a new instance of the <see cref="MultiVirtualFiles"/> class.
        /// </summary>
        /// <param name="childProviders">The child providers.</param>
        /// <exception cref="System.ArgumentNullException">childProviders</exception>
        public MultiVirtualFiles(params IVirtualPathProvider[] childProviders)
        {
            if (childProviders == null || childProviders.Length == 0)
                throw new ArgumentNullException(nameof(childProviders));

            this.ChildProviders = new List<IVirtualPathProvider>(childProviders);
            Initialize();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected sealed override void Initialize() { }

        /// <summary>
        /// Combines the virtual path.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <param name="relativePath">The relative path.</param>
        /// <returns>System.String.</returns>
        public override string CombineVirtualPath(string basePath, string relativePath)
        {
            return basePath.CombineWith(relativePath);
        }

        /// <summary>
        /// Directories the exists.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool DirectoryExists(string virtualPath)
        {
            var hasDirectory = ChildProviders.Any(childProvider => childProvider.DirectoryExists(virtualPath));
            return hasDirectory;
        }

        /// <summary>
        /// Files the exists.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool FileExists(string virtualPath)
        {
            var hasFile = ChildProviders.Any(childProvider => childProvider.FileExists(virtualPath));
            return hasFile;
        }

        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>IVirtualFile.</returns>
        public override IVirtualFile GetFile(string virtualPath)
        {
            return ChildProviders.Select(childProvider => childProvider.GetFile(virtualPath))
                .FirstOrDefault(file => file != null);
        }

        /// <summary>
        /// Gets the directory.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>IVirtualDirectory.</returns>
        public override IVirtualDirectory GetDirectory(string virtualPath) =>
            MultiVirtualDirectory.ToVirtualDirectory(ChildProviders.Select(p => p.GetDirectory(virtualPath)).Where(dir => dir != null));

        /// <summary>
        /// Gets all matching files.
        /// </summary>
        /// <param name="globPattern">The glob pattern.</param>
        /// <param name="maxDepth">The maximum depth.</param>
        /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
        public override IEnumerable<IVirtualFile> GetAllMatchingFiles(string globPattern, int maxDepth = int.MaxValue)
        {
            return ChildProviders.SelectMany(p => p.GetAllMatchingFiles(globPattern, maxDepth))
                .Distinct();
        }

        /// <summary>
        /// Gets all files.
        /// </summary>
        /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
        public override IEnumerable<IVirtualFile> GetAllFiles()
        {
            return ChildProviders.SelectMany(x => x.GetAllFiles())
                .Distinct();
        }

        /// <summary>
        /// Gets the root files.
        /// </summary>
        /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
        public override IEnumerable<IVirtualFile> GetRootFiles()
        {
            return ChildProviders.SelectMany(x => x.GetRootFiles());
        }

        /// <summary>
        /// Gets the root directories.
        /// </summary>
        /// <returns>IEnumerable&lt;IVirtualDirectory&gt;.</returns>
        public override IEnumerable<IVirtualDirectory> GetRootDirectories()
        {
            return ChildProviders.SelectMany(x => x.GetRootDirectories());
        }

        /// <summary>
        /// Determines whether [is shared file] [the specified virtual file].
        /// </summary>
        /// <param name="virtualFile">The virtual file.</param>
        /// <returns><c>true</c> if [is shared file] [the specified virtual file]; otherwise, <c>false</c>.</returns>
        public override bool IsSharedFile(IVirtualFile virtualFile)
        {
            return virtualFile.VirtualPathProvider.IsSharedFile(virtualFile);
        }

        /// <summary>
        /// Determines whether [is view file] [the specified virtual file].
        /// </summary>
        /// <param name="virtualFile">The virtual file.</param>
        /// <returns><c>true</c> if [is view file] [the specified virtual file]; otherwise, <c>false</c>.</returns>
        public override bool IsViewFile(IVirtualFile virtualFile)
        {
            return virtualFile.VirtualPathProvider.IsViewFile(virtualFile);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            var sb = new List<string>();
            ChildProviders.Each(x => sb.Add(x.ToString()));
            return string.Join(", ", sb.ToArray());
        }

        /// <summary>
        /// Gets the child virtual files.
        /// </summary>
        /// <value>The child virtual files.</value>
        public IEnumerable<IVirtualPathProvider> ChildVirtualFiles
        {
            get { return ChildProviders.Where(x => x is IVirtualFiles); }
        }

        /// <summary>
        /// Writes the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="textContents">The text contents.</param>
        public void WriteFile(string filePath, string textContents)
        {
            ChildVirtualFiles.Each(x => x.WriteFile(filePath, textContents));
        }

        /// <summary>
        /// Writes the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="stream">The stream.</param>
        public void WriteFile(string filePath, Stream stream)
        {
            ChildVirtualFiles.Each(x => x.WriteFile(filePath, stream));
        }

        /// <summary>
        /// Writes the files.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <param name="toPath">To path.</param>
        public void WriteFiles(IEnumerable<IVirtualFile> files, Func<IVirtualFile, string> toPath = null)
        {
            ChildVirtualFiles.Each(x => x.WriteFiles(files, toPath));
        }

        /// <summary>
        /// Appends the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="textContents">The text contents.</param>
        public void AppendFile(string filePath, string textContents)
        {
            ChildVirtualFiles.Each(x => x.AppendFile(filePath, textContents));
        }

        /// <summary>
        /// Appends the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="stream">The stream.</param>
        public void AppendFile(string filePath, Stream stream)
        {
            ChildVirtualFiles.Each(x => x.AppendFile(filePath, stream));
        }

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void DeleteFile(string filePath)
        {
            ChildVirtualFiles.Each(x => x.DeleteFile(filePath));
        }

        /// <summary>
        /// Deletes the files.
        /// </summary>
        /// <param name="filePaths">The file paths.</param>
        public void DeleteFiles(IEnumerable<string> filePaths)
        {
            ChildVirtualFiles.Each(x => x.DeleteFiles(filePaths));
        }

        /// <summary>
        /// Deletes the folder.
        /// </summary>
        /// <param name="dirPath">The dir path.</param>
        public void DeleteFolder(string dirPath)
        {
            ChildVirtualFiles.Each(x => x.DeleteFolder(dirPath));
        }
    }

    /// <summary>
    /// Class MultiVirtualDirectory.
    /// Implements the <see cref="ServiceStack.IO.IVirtualDirectory" />
    /// </summary>
    /// <seealso cref="ServiceStack.IO.IVirtualDirectory" />
    public class MultiVirtualDirectory : IVirtualDirectory
    {
        /// <summary>
        /// Converts to virtualdirectory.
        /// </summary>
        /// <param name="dirs">The dirs.</param>
        /// <returns>IVirtualDirectory.</returns>
        public static IVirtualDirectory ToVirtualDirectory(IEnumerable<IVirtualDirectory> dirs)
        {
            var arr = dirs.ToArray();
            return arr.Length == 0
                ? null
                : arr.Length == 1
                    ? arr[0]
                    : new MultiVirtualDirectory(arr);
        }

        /// <summary>
        /// The dirs
        /// </summary>
        private readonly IVirtualDirectory[] dirs;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiVirtualDirectory"/> class.
        /// </summary>
        /// <param name="dirs">The dirs.</param>
        /// <exception cref="System.ArgumentNullException">dirs</exception>
        public MultiVirtualDirectory(IVirtualDirectory[] dirs)
        {
            if (dirs.Length == 0)
                throw new ArgumentNullException(nameof(dirs));

            this.dirs = dirs;
        }

        /// <summary>
        /// Gets the directory.
        /// </summary>
        /// <value>The directory.</value>
        public IVirtualDirectory Directory => this;
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name => this.First().Name;
        /// <summary>
        /// Gets the virtual path.
        /// </summary>
        /// <value>The virtual path.</value>
        public string VirtualPath => this.First().VirtualPath;
        /// <summary>
        /// Gets the real path.
        /// </summary>
        /// <value>The real path.</value>
        public string RealPath => this.First().RealPath;
        /// <summary>
        /// Gets a value indicating whether this instance is directory.
        /// </summary>
        /// <value><c>true</c> if this instance is directory; otherwise, <c>false</c>.</value>
        public bool IsDirectory => true;
        /// <summary>
        /// Gets the last modified.
        /// </summary>
        /// <value>The last modified.</value>
        public DateTime LastModified => this.First().LastModified;

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IEnumerator<IVirtualNode> GetEnumerator()
        {
            throw new NotImplementedException();
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
        /// Gets a value indicating whether this instance is root.
        /// </summary>
        /// <value><c>true</c> if this instance is root; otherwise, <c>false</c>.</value>
        public bool IsRoot => this.dirs.First().IsRoot;

        /// <summary>
        /// Gets the parent directory.
        /// </summary>
        /// <value>The parent directory.</value>
        public IVirtualDirectory ParentDirectory =>
            ToVirtualDirectory(dirs.SelectMany(x => x.ParentDirectory).Where(x => x != null).Cast<IVirtualDirectory>());

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <value>The files.</value>
        public IEnumerable<IVirtualFile> Files => dirs.SelectMany(x => x.Files);

        /// <summary>
        /// Gets the directories.
        /// </summary>
        /// <value>The directories.</value>
        public IEnumerable<IVirtualDirectory> Directories => dirs.SelectMany(x => x.Directories);

        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>IVirtualFile.</returns>
        public IVirtualFile GetFile(string virtualPath)
        {
            foreach (var dir in dirs)
            {
                var file = dir.GetFile(virtualPath);
                if (file != null)
                    return file;
            }
            return null;
        }

        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>IVirtualFile.</returns>
        public IVirtualFile GetFile(Stack<string> virtualPath)
        {
            foreach (var dir in dirs)
            {
                var file = dir.GetFile(virtualPath);
                if (file != null)
                    return file;
            }
            return null;
        }

        /// <summary>
        /// Gets the directory.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>IVirtualDirectory.</returns>
        public IVirtualDirectory GetDirectory(string virtualPath)
        {
            foreach (var dir in dirs)
            {
                var sub = dir.GetDirectory(virtualPath);
                if (sub != null)
                    return sub;
            }
            return null;
        }

        /// <summary>
        /// Gets the directory.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>IVirtualDirectory.</returns>
        public IVirtualDirectory GetDirectory(Stack<string> virtualPath)
        {
            foreach (var dir in dirs)
            {
                var sub = dir.GetDirectory(virtualPath);
                if (sub != null)
                    return sub;
            }
            return null;
        }

        /// <summary>
        /// Gets all matching files.
        /// </summary>
        /// <param name="globPattern">The glob pattern.</param>
        /// <param name="maxDepth">The maximum depth.</param>
        /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
        public IEnumerable<IVirtualFile> GetAllMatchingFiles(string globPattern, int maxDepth = Int32.MaxValue)
        {
            foreach (var dir in dirs)
            {
                var files = dir.GetAllMatchingFiles(globPattern, maxDepth);
                foreach (var file in files)
                {
                    yield return file;
                }
            }
        }
    }
}