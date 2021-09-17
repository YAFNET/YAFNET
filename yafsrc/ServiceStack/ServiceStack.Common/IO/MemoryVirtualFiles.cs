// ***********************************************************************
// <copyright file="MemoryVirtualFiles.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ServiceStack.Text;
using ServiceStack.VirtualPath;

namespace ServiceStack.IO
{
    /// <summary>
    /// Class MemoryVirtualFiles.
    /// Implements the <see cref="ServiceStack.VirtualPath.AbstractVirtualPathProviderBase" />
    /// Implements the <see cref="ServiceStack.IO.IVirtualFiles" />
    /// </summary>
    /// <seealso cref="ServiceStack.VirtualPath.AbstractVirtualPathProviderBase" />
    /// <seealso cref="ServiceStack.IO.IVirtualFiles" />
    public class MemoryVirtualFiles
        : AbstractVirtualPathProviderBase, IVirtualFiles
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryVirtualFiles"/> class.
        /// </summary>
        public MemoryVirtualFiles()
        {
            this.files = new List<InMemoryVirtualFile>();
            this.rootDirectory = new InMemoryVirtualDirectory(this, null);
        }

        /// <summary>
        /// The dir sep
        /// </summary>
        public const char DirSep = '/';

        /// <summary>
        /// The files
        /// </summary>
        private List<InMemoryVirtualFile> files;
        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <value>The files.</value>
        public List<InMemoryVirtualFile> Files => files;

        /// <summary>
        /// The root directory
        /// </summary>
        private readonly InMemoryVirtualDirectory rootDirectory;

        /// <summary>
        /// Gets the root directory.
        /// </summary>
        /// <value>The root directory.</value>
        public override IVirtualDirectory RootDirectory => rootDirectory;

        /// <summary>
        /// Gets the virtual path separator.
        /// </summary>
        /// <value>The virtual path separator.</value>
        public override string VirtualPathSeparator => "/";

        /// <summary>
        /// Gets the real path separator.
        /// </summary>
        /// <value>The real path separator.</value>
        public override string RealPathSeparator => "/";

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize() { }

        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>IVirtualFile.</returns>
        public override IVirtualFile GetFile(string virtualPath)
        {
            if (Files.Count == 0)
                return null;

            var filePath = SanitizePath(virtualPath);
            return Files.FirstOrDefault(x => x.FilePath == filePath);
        }

        /// <summary>
        /// Gets the directory.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>IVirtualDirectory.</returns>
        public override IVirtualDirectory GetDirectory(string virtualPath) => GetDirectory(virtualPath, forceDir: false);

        /// <summary>
        /// Gets the directory.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <param name="forceDir">if set to <c>true</c> [force dir].</param>
        /// <returns>IVirtualDirectory.</returns>
        public IVirtualDirectory GetDirectory(string virtualPath, bool forceDir)
        {
            var dirPath = SanitizePath(virtualPath);
            if (string.IsNullOrEmpty(dirPath))
                return rootDirectory;

            var dir = new InMemoryVirtualDirectory(this, dirPath, GetParentDirectory(dirPath));
            return forceDir || dir.HasFiles()
                ? dir
                : null;
        }

        /// <summary>
        /// Gets the parent directory.
        /// </summary>
        /// <param name="dirPath">The dir path.</param>
        /// <returns>IVirtualDirectory.</returns>
        public IVirtualDirectory GetParentDirectory(string dirPath)
        {
            if (string.IsNullOrEmpty(dirPath))
                return null;

            var lastDirPos = dirPath.LastIndexOf('/');
            if (lastDirPos >= 0)
            {
                var parentDir = dirPath.Substring(0, lastDirPos);
                if (!string.IsNullOrEmpty(parentDir))
                    return GetDirectory(parentDir, forceDir: true);
            }

            return this.rootDirectory;
        }

        /// <summary>
        /// Directories the exists.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool DirectoryExists(string virtualPath)
        {
            return GetDirectory(virtualPath) != null;
        }

        /// <summary>
        /// Creates the directory.
        /// </summary>
        /// <param name="dirPath">The dir path.</param>
        /// <returns>IVirtualDirectory.</returns>
        private IVirtualDirectory CreateDirectory(string dirPath)
        {
            return new InMemoryVirtualDirectory(this, dirPath, GetParentDirectory(dirPath));
        }

        /// <summary>
        /// Writes the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="textContents">The text contents.</param>
        public void WriteFile(string filePath, string textContents)
        {
            filePath = SanitizePath(filePath);
            AddFile(new InMemoryVirtualFile(this, CreateDirectory(GetDirPath(filePath)))
            {
                FilePath = filePath,
                TextContents = textContents,
                FileLastModified = DateTime.UtcNow,
            });
        }

        /// <summary>
        /// Writes the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="stream">The stream.</param>
        public void WriteFile(string filePath, Stream stream)
        {
            filePath = SanitizePath(filePath);
            AddFile(new InMemoryVirtualFile(this, CreateDirectory(GetDirPath(filePath)))
            {
                FilePath = filePath,
                ByteContents = stream.ReadFully(),
                FileLastModified = DateTime.UtcNow,
            });
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
            filePath = SanitizePath(filePath);

            var existingFile = GetFile(filePath);
            var text = existingFile != null
                ? existingFile.ReadAllText() + textContents
                : textContents;

            DeleteFile(filePath);

            AddFile(new InMemoryVirtualFile(this, CreateDirectory(GetDirPath(filePath)))
            {
                FilePath = filePath,
                TextContents = text,
                FileLastModified = DateTime.UtcNow,
            });
        }

        /// <summary>
        /// Appends the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="stream">The stream.</param>
        public void AppendFile(string filePath, Stream stream)
        {
            filePath = SanitizePath(filePath);

            var existingFile = GetFile(filePath);
            var bytes = existingFile != null
                ? existingFile.ReadAllBytes().Combine(stream.ReadFully())
                : stream.ReadFully();

            DeleteFile(filePath);

            AddFile(new InMemoryVirtualFile(this, CreateDirectory(GetDirPath(filePath)))
            {
                FilePath = filePath,
                ByteContents = bytes,
                FileLastModified = DateTime.UtcNow,
            });
        }

        /// <summary>
        /// Adds the file.
        /// </summary>
        /// <param name="file">The file.</param>
        public void AddFile(InMemoryVirtualFile file)
        {
            List<InMemoryVirtualFile> snapshot, newFiles;
            do
            {
                snapshot = files;
                newFiles = new List<InMemoryVirtualFile>(files.Where(x => x.FilePath != file.FilePath)) {
                    file
                };
            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref files, newFiles, snapshot), snapshot));
        }

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void DeleteFile(string filePath) => DeleteFiles(new[] { filePath });

        /// <summary>
        /// Deletes the files.
        /// </summary>
        /// <param name="filePaths">The file paths.</param>
        public void DeleteFiles(IEnumerable<string> filePaths)
        {
            var sanitizedFilePaths = filePaths.Select(SanitizePath).ToSet();

            List<InMemoryVirtualFile> snapshot, newFiles;
            do
            {
                snapshot = files;
                newFiles = files.Where(x => !sanitizedFilePaths.Contains(x.FilePath)).ToList();
            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref files, newFiles, snapshot), snapshot));
        }

        /// <summary>
        /// Deletes the folder.
        /// </summary>
        /// <param name="dirPath">The dir path.</param>
        public void DeleteFolder(string dirPath)
        {
            var subFiles = Files.Where(x => x.DirPath.StartsWith(dirPath));
            DeleteFiles(subFiles.Map(x => x.VirtualPath));
        }

        /// <summary>
        /// Gets the immediate directories.
        /// </summary>
        /// <param name="fromDirPath">From dir path.</param>
        /// <returns>IEnumerable&lt;InMemoryVirtualDirectory&gt;.</returns>
        public IEnumerable<InMemoryVirtualDirectory> GetImmediateDirectories(string fromDirPath)
        {
            if (Files.Count == 0)
                return TypeConstants<InMemoryVirtualDirectory>.EmptyArray;

            var dirPaths = Files
                .Map(x => x.DirPath)
                .Distinct()
                .Map(x => GetImmediateSubDirPath(fromDirPath, x))
                .Where(x => x != null)
                .Distinct();

            return dirPaths.Map(x => new InMemoryVirtualDirectory(this, x, GetParentDirectory(x)));
        }

        /// <summary>
        /// Gets the immediate files.
        /// </summary>
        /// <param name="fromDirPath">From dir path.</param>
        /// <returns>IEnumerable&lt;InMemoryVirtualFile&gt;.</returns>
        public IEnumerable<InMemoryVirtualFile> GetImmediateFiles(string fromDirPath)
        {
            if (Files.Count == 0)
                return TypeConstants<InMemoryVirtualFile>.EmptyArray;

            return Files.Where(x => x.DirPath == fromDirPath);
        }

        /// <summary>
        /// Gets all files.
        /// </summary>
        /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
        public override IEnumerable<IVirtualFile> GetAllFiles()
        {
            return Files;
        }

        /// <summary>
        /// Gets the dir path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>System.String.</returns>
        public string GetDirPath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            var lastDirPos = filePath.LastIndexOf(DirSep);
            return lastDirPos >= 0
                ? filePath.Substring(0, lastDirPos)
                : null;
        }

        /// <summary>
        /// Gets the immediate sub dir path.
        /// </summary>
        /// <param name="fromDirPath">From dir path.</param>
        /// <param name="subDirPath">The sub dir path.</param>
        /// <returns>System.String.</returns>
        public string GetImmediateSubDirPath(string fromDirPath, string subDirPath)
        {
            if (string.IsNullOrEmpty(subDirPath))
                return null;

            if (fromDirPath == null)
            {
                return subDirPath.CountOccurrencesOf(DirSep) == 0
                    ? subDirPath
                    : subDirPath.LeftPart(DirSep);
            }

            if (!subDirPath.StartsWith(fromDirPath))
                return null;

            return fromDirPath.CountOccurrencesOf(DirSep) == subDirPath.CountOccurrencesOf(DirSep) - 1
                ? subDirPath
                : null;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear() => Files.Clear();
    }

    /// <summary>
    /// Class InMemoryVirtualDirectory.
    /// Implements the <see cref="ServiceStack.VirtualPath.AbstractVirtualDirectoryBase" />
    /// </summary>
    /// <seealso cref="ServiceStack.VirtualPath.AbstractVirtualDirectoryBase" />
    public class InMemoryVirtualDirectory : AbstractVirtualDirectoryBase
    {
        /// <summary>
        /// The path provider
        /// </summary>
        private readonly MemoryVirtualFiles pathProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryVirtualDirectory"/> class.
        /// </summary>
        /// <param name="pathProvider">The path provider.</param>
        /// <param name="dirPath">The dir path.</param>
        /// <param name="parentDir">The parent dir.</param>
        public InMemoryVirtualDirectory(MemoryVirtualFiles pathProvider, string dirPath, IVirtualDirectory parentDir = null)
            : base(pathProvider, parentDir)
        {
            this.pathProvider = pathProvider;
            this.DirPath = dirPath;
        }

        /// <summary>
        /// Gets or sets the dir last modified.
        /// </summary>
        /// <value>The dir last modified.</value>
        public DateTime DirLastModified { get; set; }
        /// <summary>
        /// Gets the last modified.
        /// </summary>
        /// <value>The last modified.</value>
        public override DateTime LastModified => DirLastModified;

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <value>The files.</value>
        public override IEnumerable<IVirtualFile> Files => pathProvider.GetImmediateFiles(DirPath);

        /// <summary>
        /// Gets the directories.
        /// </summary>
        /// <value>The directories.</value>
        public override IEnumerable<IVirtualDirectory> Directories => pathProvider.GetImmediateDirectories(DirPath);

        /// <summary>
        /// Gets or sets the dir path.
        /// </summary>
        /// <value>The dir path.</value>
        public string DirPath { get; set; }

        /// <summary>
        /// Gets the virtual path.
        /// </summary>
        /// <value>The virtual path.</value>
        public override string VirtualPath => DirPath;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name => DirPath?.LastRightPart(MemoryVirtualFiles.DirSep);

        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>IVirtualFile.</returns>
        public override IVirtualFile GetFile(string virtualPath)
        {
            return pathProvider.GetFile(DirPath.CombineWith(virtualPath));
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>IEnumerator&lt;IVirtualNode&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override IEnumerator<IVirtualNode> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the file from backing directory or default.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>IVirtualFile.</returns>
        protected override IVirtualFile GetFileFromBackingDirectoryOrDefault(string fileName)
        {
            return GetFile(fileName);
        }

        /// <summary>
        /// Gets the matching files in dir.
        /// </summary>
        /// <param name="globPattern">The glob pattern.</param>
        /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
        protected override IEnumerable<IVirtualFile> GetMatchingFilesInDir(string globPattern)
        {
            if (pathProvider.Files.Count == 0)
                return TypeConstants<IVirtualFile>.EmptyArray;

            var matchingFilesInBackingDir = EnumerateFiles(globPattern);
            return matchingFilesInBackingDir;
        }

        /// <summary>
        /// Enumerates the files.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns>IEnumerable&lt;InMemoryVirtualFile&gt;.</returns>
        public IEnumerable<InMemoryVirtualFile> EnumerateFiles(string pattern)
        {
            foreach (var file in pathProvider.GetImmediateFiles(DirPath).Where(f => f.Name.Glob(pattern)))
            {
                yield return file;
            }
        }

        /// <summary>
        /// Gets the directory from backing directory or default.
        /// </summary>
        /// <param name="directoryName">Name of the directory.</param>
        /// <returns>IVirtualDirectory.</returns>
        protected override IVirtualDirectory GetDirectoryFromBackingDirectoryOrDefault(string directoryName)
        {
            var subDir = DirPath.CombineWith(directoryName);
            return new InMemoryVirtualDirectory(pathProvider, subDir, this);
        }

        /// <summary>
        /// Adds the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="contents">The contents.</param>
        public void AddFile(string filePath, string contents)
        {
            pathProvider.WriteFile(DirPath.CombineWith(filePath), contents);
        }

        /// <summary>
        /// Adds the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="stream">The stream.</param>
        public void AddFile(string filePath, Stream stream)
        {
            pathProvider.WriteFile(DirPath.CombineWith(filePath), stream);
        }

        /// <summary>
        /// Determines whether this instance has files.
        /// </summary>
        /// <returns><c>true</c> if this instance has files; otherwise, <c>false</c>.</returns>
        public bool HasFiles()
        {
            if (pathProvider.Files.Count == 0)
                return false;

            if (IsRoot)
                return pathProvider.Files.Count > 0;

            var ret = pathProvider.Files.Any(x => x.DirPath != null && x.DirPath.StartsWith(DirPath));
            return ret;
        }

        /// <summary>
        /// Gets all matching files.
        /// </summary>
        /// <param name="globPattern">The glob pattern.</param>
        /// <param name="maxDepth">The maximum depth.</param>
        /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
        public override IEnumerable<IVirtualFile> GetAllMatchingFiles(string globPattern, int maxDepth = int.MaxValue)
        {
            if (pathProvider.Files.Count == 0)
                return TypeConstants<IVirtualFile>.EmptyArray;

            if (IsRoot)
                return pathProvider.Files.Where(x =>
                    (x.DirPath == null || x.DirPath.CountOccurrencesOf('/') < maxDepth - 1)
                    && x.Name.Glob(globPattern));

            return pathProvider.Files.Where(x =>
                x.DirPath != null
                && x.DirPath.CountOccurrencesOf('/') < maxDepth - 1
                && x.DirPath.StartsWith(DirPath)
                && x.Name.Glob(globPattern));
        }

    }

    /// <summary>
    /// Class InMemoryVirtualFile.
    /// Implements the <see cref="ServiceStack.VirtualPath.AbstractVirtualFileBase" />
    /// </summary>
    /// <seealso cref="ServiceStack.VirtualPath.AbstractVirtualFileBase" />
    public class InMemoryVirtualFile : AbstractVirtualFileBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryVirtualFile"/> class.
        /// </summary>
        /// <param name="owningProvider">The owning provider.</param>
        /// <param name="directory">The directory.</param>
        public InMemoryVirtualFile(IVirtualPathProvider owningProvider, IVirtualDirectory directory)
            : base(owningProvider, directory)
        {
            this.FileLastModified = DateTime.MinValue;
        }

        /// <summary>
        /// Gets the dir path.
        /// </summary>
        /// <value>The dir path.</value>
        public string DirPath => base.Directory.VirtualPath;

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>The file path.</value>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name => FilePath.LastRightPart(MemoryVirtualFiles.DirSep);

        /// <summary>
        /// Gets the virtual path.
        /// </summary>
        /// <value>The virtual path.</value>
        public override string VirtualPath => FilePath;

        /// <summary>
        /// Gets or sets the file last modified.
        /// </summary>
        /// <value>The file last modified.</value>
        public DateTime FileLastModified { get; set; }
        /// <summary>
        /// Gets the last modified.
        /// </summary>
        /// <value>The last modified.</value>
        public override DateTime LastModified => FileLastModified;

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        public override long Length => ByteContents?.Length ?? 0;

        /// <summary>
        /// The text contents
        /// </summary>
        private string textContents;
        /// <summary>
        /// Gets or sets the text contents.
        /// </summary>
        /// <value>The text contents.</value>
        public string TextContents
        {
            get => textContents;
            set
            {
                textContents = value;
                ByteContents = value?.ToUtf8Bytes();
            }
        }

        /// <summary>
        /// Gets or sets the byte contents.
        /// </summary>
        /// <value>The byte contents.</value>
        public byte[] ByteContents { get; set; }

        /// <summary>
        /// Sets the contents.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="bytes">The bytes.</param>
        public void SetContents(string text, byte[] bytes)
        {
            if (bytes != null)
            {
                this.textContents = null;
                this.ByteContents = bytes;
            }
            else
            {
                this.ByteContents = null;
                this.TextContents = text;
            }
        }

        /// <summary>
        /// Opens the read.
        /// </summary>
        /// <returns>Stream.</returns>
        /// <exception cref="System.ArgumentNullException">ByteContents</exception>
        public override Stream OpenRead()
        {
            if (ByteContents == null)
                throw new ArgumentNullException(nameof(ByteContents));
            return MemoryStreamFactory.GetStream(ByteContents);
        }

        /// <summary>
        /// Gets the contents.
        /// </summary>
        /// <returns>System.Object.</returns>
        public override object GetContents()
        {
            return TextContents != null
                ? (object)TextContents.AsMemory()
                : ByteContents != null ? new ReadOnlyMemory<byte>(ByteContents) : null;
        }

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        public override void Refresh()
        {
            if (base.VirtualPathProvider.GetFile(VirtualPath) is InMemoryVirtualFile file && !ReferenceEquals(file, this))
            {
                this.FilePath = file.FilePath;
                this.FileLastModified = file.FileLastModified;
                SetContents(file.TextContents, file.ByteContents);
            }
        }
    }
}