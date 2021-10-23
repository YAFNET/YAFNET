// ***********************************************************************
// <copyright file="GistVirtualFiles.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ServiceStack.Text;
using ServiceStack.VirtualPath;

namespace ServiceStack.IO
{
    /// <summary>
    /// Class GistVirtualFiles.
    /// Implements the <see cref="ServiceStack.VirtualPath.AbstractVirtualPathProviderBase" />
    /// Implements the <see cref="ServiceStack.IO.IVirtualFiles" />
    /// </summary>
    /// <seealso cref="ServiceStack.VirtualPath.AbstractVirtualPathProviderBase" />
    /// <seealso cref="ServiceStack.IO.IVirtualFiles" />
    public partial class GistVirtualFiles : AbstractVirtualPathProviderBase, IVirtualFiles
    {
        /// <summary>
        /// Gets the gateway.
        /// </summary>
        /// <value>The gateway.</value>
        public IGistGateway Gateway { get; }
        /// <summary>
        /// Gets the gist identifier.
        /// </summary>
        /// <value>The gist identifier.</value>
        public string GistId { get; private set; }

        /// <summary>
        /// The root directory
        /// </summary>
        private readonly GistVirtualDirectory rootDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="GistVirtualFiles"/> class.
        /// </summary>
        /// <param name="gistId">The gist identifier.</param>
        public GistVirtualFiles(string gistId) : this(gistId, new GitHubGateway()) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="GistVirtualFiles"/> class.
        /// </summary>
        /// <param name="gistId">The gist identifier.</param>
        /// <param name="accessToken">The access token.</param>
        public GistVirtualFiles(string gistId, string accessToken) : this(gistId, new GitHubGateway(accessToken)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GistVirtualFiles"/> class.
        /// </summary>
        /// <param name="gistId">The gist identifier.</param>
        /// <param name="gateway">The gateway.</param>
        public GistVirtualFiles(string gistId, IGistGateway gateway)
        {
            this.Gateway = gateway;
            this.GistId = gistId;
            this.rootDirectory = new GistVirtualDirectory(this, null, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GistVirtualFiles"/> class.
        /// </summary>
        /// <param name="gist">The gist.</param>
        public GistVirtualFiles(Gist gist) : this(gist.Id) => InitGist(gist);

        /// <summary>
        /// Initializes a new instance of the <see cref="GistVirtualFiles"/> class.
        /// </summary>
        /// <param name="gist">The gist.</param>
        /// <param name="accessToken">The access token.</param>
        public GistVirtualFiles(Gist gist, string accessToken) : this(gist.Id, accessToken) => InitGist(gist);
        /// <summary>
        /// Initializes a new instance of the <see cref="GistVirtualFiles"/> class.
        /// </summary>
        /// <param name="gist">The gist.</param>
        /// <param name="gateway">The gateway.</param>
        public GistVirtualFiles(Gist gist, IGistGateway gateway) : this(gist.Id, gateway) => InitGist(gist);

        /// <summary>
        /// Initializes the gist.
        /// </summary>
        /// <param name="gist">The gist.</param>
        private void InitGist(Gist gist)
        {
            gistCache = gist;
            LastRefresh = gist.Updated_At.GetValueOrDefault(DateTime.UtcNow);
        }

        /// <summary>
        /// Gets the last refresh.
        /// </summary>
        /// <value>The last refresh.</value>
        public DateTime LastRefresh { get; private set; }

        /// <summary>
        /// Gets or sets the refresh after.
        /// </summary>
        /// <value>The refresh after.</value>
        public TimeSpan RefreshAfter { get; set; } = TimeSpan.MaxValue;

        /// <summary>
        /// The dir sep
        /// </summary>
        public const char DirSep = '\\';

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
        public override string RealPathSeparator => "\\";

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize() { }

        /// <summary>
        /// Determines whether [is dir sep] [the specified c].
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns><c>true</c> if [is dir sep] [the specified c]; otherwise, <c>false</c>.</returns>
        public static bool IsDirSep(char c) => c == '\\' || c == '/';

        /// <summary>
        /// The base64 modifier
        /// </summary>
        public const string Base64Modifier = "|base64";

        /// <summary>
        /// Froms the base64 string.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="base64String">The base64 string.</param>
        /// <returns>System.Byte[].</returns>
        /// <exception cref="System.Exception">Could not convert Base 64 contents of '{path}', length: {base64String.Length}, starting with: {base64String.SafeSubstring(50)}</exception>
        private static byte[] FromBase64String(string path, string base64String)
        {
            try
            {
                if (string.IsNullOrEmpty(base64String))
                    return TypeConstants.EmptyByteArray;

                return Convert.FromBase64String(base64String);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Could not convert Base 64 contents of '{path}', length: {base64String.Length}, starting with: {base64String.SafeSubstring(50)}",
                    ex);
            }
        }

        /// <summary>
        /// Gets the gist text contents.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="gist">The gist.</param>
        /// <param name="text">The text.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool GetGistTextContents(string filePath, Gist gist, out string text)
        {
            if (GetGistContents(filePath, gist, out text, out var bytesContent))
            {
                if (text == null)
                    text = MemoryProvider.Instance.FromUtf8(bytesContent.GetBufferAsMemory().Span).ToString();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the gist contents.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="gist">The gist.</param>
        /// <param name="text">The text.</param>
        /// <param name="stream">The stream.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool GetGistContents(string filePath, Gist gist, out string text, out MemoryStream stream)
        {
            var base64FilePath = filePath + Base64Modifier;
            foreach (var entry in gist.Files)
            {
                var file = entry.Value;
                var isMatch = entry.Key == filePath || entry.Key == base64FilePath;
                if (!isMatch)
                    continue;

                // GitHub can truncate Gist and return partial content
                if ((string.IsNullOrEmpty(file.Content) || file.Content.Length < file.Size) && file.Truncated)
                {
                    file.Content = file.Raw_Url.GetStringFromUrl(
                        requestFilter: req => req.UserAgent = nameof(GitHubGateway));
                }

                text = file.Content;
                if (entry.Key == filePath)
                {
                    if (filePath.EndsWith(Base64Modifier))
                    {
                        stream = MemoryStreamFactory.GetStream(FromBase64String(entry.Key, text));
                        text = null;
                    }
                    else
                    {
                        var bytesMemory = MemoryProvider.Instance.ToUtf8(text.AsSpan());
                        stream = MemoryProvider.Instance.ToMemoryStream(bytesMemory.Span);
                    }
                    return true;
                }

                if (entry.Key == base64FilePath)
                {
                    stream = MemoryStreamFactory.GetStream(FromBase64String(entry.Key, text));
                    text = null;
                    return true;
                }
            }

            text = null;
            stream = null;
            return false;
        }

        /// <summary>
        /// The gist cache
        /// </summary>
        private Gist gistCache;

        /// <summary>
        /// Gets the gist.
        /// </summary>
        /// <param name="refresh">if set to <c>true</c> [refresh].</param>
        /// <returns>Gist.</returns>
        public Gist GetGist(bool refresh = false)
        {
            if (gistCache != null && !refresh)
                return gistCache;

            LastRefresh = DateTime.UtcNow;
            return gistCache = Gateway.GetGist(GistId);
        }

        /// <summary>
        /// Get gist as an asynchronous operation.
        /// </summary>
        /// <param name="refresh">if set to <c>true</c> [refresh].</param>
        /// <returns>A Task&lt;Gist&gt; representing the asynchronous operation.</returns>
        public async Task<Gist> GetGistAsync(bool refresh = false)
        {
            if (gistCache != null && !refresh)
                return gistCache;

            LastRefresh = DateTime.UtcNow;
            return gistCache = await Gateway.GetGistAsync(GistId).ConfigAwait();
        }

        /// <summary>
        /// Load all truncated files as an asynchronous operation.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task LoadAllTruncatedFilesAsync()
        {
            var gist = await GetGistAsync().ConfigAwait();

            var files = gist.Files.Where(x =>
                (string.IsNullOrEmpty(x.Value.Content) || x.Value.Content.Length < x.Value.Size) && x.Value.Truncated);

            var tasks = files.Select(async x =>
            {
                x.Value.Content = await x.Value.Raw_Url.GetStringFromUrlAsync().ConfigAwait();
            });

            await Task.WhenAll(tasks).ConfigAwait();
        }

        /// <summary>
        /// Clears the gist.
        /// </summary>
        public void ClearGist() => gistCache = null;

        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>IVirtualFile.</returns>
        public override IVirtualFile GetFile(string virtualPath)
        {
            if (string.IsNullOrEmpty(virtualPath))
                return null;

            var filePath = SanitizePath(virtualPath);
            var gist = GetGist();

            if (!GetGistContents(filePath, gist, out var text, out var stream))
                return null;

            var dirPath = GetDirPath(filePath);
            return new GistVirtualFile(this, new GistVirtualDirectory(this, dirPath, GetParentDirectory(dirPath)))
                .Init(filePath, gist.Updated_At ?? gist.Created_At, text, stream);
        }

        /// <summary>
        /// Gets the parent directory.
        /// </summary>
        /// <param name="dirPath">The dir path.</param>
        /// <returns>GistVirtualDirectory.</returns>
        private GistVirtualDirectory GetParentDirectory(string dirPath)
        {
            if (string.IsNullOrEmpty(dirPath))
                return null;

            var parentDir = GetDirPath(dirPath.TrimEnd(DirSep));
            return parentDir != null
                ? new GistVirtualDirectory(this, parentDir, GetParentDirectory(parentDir))
                : (GistVirtualDirectory)RootDirectory;
        }

        /// <summary>
        /// Gets the directory.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>IVirtualDirectory.</returns>
        public override IVirtualDirectory GetDirectory(string virtualPath)
        {
            if (virtualPath == null)
                return null;

            var dirPath = SanitizePath(virtualPath);
            if (string.IsNullOrEmpty(dirPath))
                return RootDirectory;

            var seekPath = dirPath[dirPath.Length - 1] != DirSep
                ? dirPath + DirSep
                : dirPath;

            var gist = GetGist();
            foreach (var entry in gist.Files)
            {
                if (entry.Key.StartsWith(seekPath))
                    return new GistVirtualDirectory(this, dirPath, GetParentDirectory(dirPath));
            }

            return null;
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
        /// Files the exists.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool FileExists(string virtualPath)
        {
            return GetFile(virtualPath) != null;
        }

        /// <summary>
        /// Writes the files.
        /// </summary>
        /// <param name="textFiles">The text files.</param>
        public override void WriteFiles(Dictionary<string, string> textFiles)
        {
            var gistFiles = new Dictionary<string, string>();
            foreach (var entry in textFiles)
            {
                var filePath = SanitizePath(entry.Key);
                gistFiles[filePath] = entry.Value;
            }

            Gateway.WriteGistFiles(GistId, gistFiles);
            ClearGist();
        }

        /// <summary>
        /// Writes the files.
        /// </summary>
        /// <param name="files">The files.</param>
        public override void WriteFiles(Dictionary<string, object> files)
        {
            Gateway.WriteGistFiles(GistId, files);
            ClearGist();
        }

        /// <summary>
        /// Writes the file.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <param name="contents">The contents.</param>
        public void WriteFile(string virtualPath, string contents)
        {
            var filePath = SanitizePath(virtualPath);
            Gateway.WriteGistFile(GistId, filePath, contents);
            ClearGist();
        }

        /// <summary>
        /// Writes the file.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <param name="stream">The stream.</param>
        public void WriteFile(string virtualPath, Stream stream)
        {
            var base64 = ToBase64(stream);
            var filePath = SanitizePath(virtualPath) + Base64Modifier;
            Gateway.WriteGistFile(GistId, filePath, base64);
            ClearGist();
        }

        /// <summary>
        /// Converts to base64.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>System.String.</returns>
        public static string ToBase64(Stream stream)
        {
            var base64 = stream is MemoryStream ms
                ? Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length)
                : Convert.ToBase64String(stream.ReadFully());
            return base64;
        }

        /// <summary>
        /// Converts to base64.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>System.String.</returns>
        public static string ToBase64(byte[] bytes) => Convert.ToBase64String(bytes);

        /// <summary>
        /// Writes the files.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <param name="toPath">To path.</param>
        public void WriteFiles(IEnumerable<IVirtualFile> files, Func<IVirtualFile, string> toPath = null)
        {
            this.CopyFrom(files, toPath);
            ClearGist();
        }

        /// <summary>
        /// Appends the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="textContents">The text contents.</param>
        /// <exception cref="System.NotImplementedException">Gists doesn't support appending to files</exception>
        public void AppendFile(string filePath, string textContents)
        {
            throw new NotImplementedException("Gists doesn't support appending to files");
        }

        /// <summary>
        /// Appends the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="stream">The stream.</param>
        /// <exception cref="System.NotImplementedException">Gists doesn't support appending to files</exception>
        public void AppendFile(string filePath, Stream stream)
        {
            throw new NotImplementedException("Gists doesn't support appending to files");
        }

        /// <summary>
        /// Resolves the name of the gist file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>System.String.</returns>
        public string ResolveGistFileName(string filePath)
        {
            var gist = GetGist();
            var baseFilePath = filePath + Base64Modifier;
            foreach (var entry in gist.Files)
            {
                if (entry.Key == filePath || entry.Key == baseFilePath)
                    return entry.Key;
            }

            return null;
        }

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void DeleteFile(string filePath)
        {
            filePath = SanitizePath(filePath);
            filePath = ResolveGistFileName(filePath) ?? filePath;
            Gateway.DeleteGistFiles(GistId, filePath);
            ClearGist();
        }

        /// <summary>
        /// Deletes the files.
        /// </summary>
        /// <param name="virtualFilePaths">The virtual file paths.</param>
        public void DeleteFiles(IEnumerable<string> virtualFilePaths)
        {
            var filePaths = virtualFilePaths.Map(x =>
            {
                var filePath = SanitizePath(x);
                return ResolveGistFileName(filePath) ?? filePath;
            });
            Gateway.DeleteGistFiles(GistId, filePaths.ToArray());
            ClearGist();
        }

        /// <summary>
        /// Deletes the folder.
        /// </summary>
        /// <param name="dirPath">The dir path.</param>
        public void DeleteFolder(string dirPath)
        {
            dirPath = SanitizePath(dirPath);
            var nestedFiles = EnumerateFiles(dirPath).Map(x => x.FilePath);
            DeleteFiles(nestedFiles);
        }

        /// <summary>
        /// Enumerates the files.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns>IEnumerable&lt;GistVirtualFile&gt;.</returns>
        public IEnumerable<GistVirtualFile> EnumerateFiles(string prefix = null)
        {
            var gist = GetGist();

            foreach (var entry in gist.Files)
            {
                if (!GetGistContents(entry.Key, gist, out var text, out var stream))
                    continue;

                var filePath = SanitizePath(entry.Key);
                var dirPath = GetDirPath(filePath);

                if (prefix != null && (dirPath == null || !dirPath.StartsWith(prefix)))
                    continue;

                yield return new GistVirtualFile(this,
                        new GistVirtualDirectory(this, dirPath, GetParentDirectory(dirPath)))
                    .Init(filePath, gist.Updated_At ?? gist.Created_At, text, stream);
            }
        }

        /// <summary>
        /// Gets all files.
        /// </summary>
        /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
        public override IEnumerable<IVirtualFile> GetAllFiles()
        {
            return EnumerateFiles();
        }

        /// <summary>
        /// Gets the immediate directories.
        /// </summary>
        /// <param name="fromDirPath">From dir path.</param>
        /// <returns>IEnumerable&lt;GistVirtualDirectory&gt;.</returns>
        public IEnumerable<GistVirtualDirectory> GetImmediateDirectories(string fromDirPath)
        {
            var dirPaths = EnumerateFiles(fromDirPath)
                .Map(x => x.DirPath)
                .Distinct()
                .Map(x => GetImmediateSubDirPath(fromDirPath, x))
                .Where(x => x != null)
                .Distinct();

            var parentDir = GetParentDirectory(fromDirPath);
            return dirPaths.Map(x => new GistVirtualDirectory(this, x, parentDir));
        }

        /// <summary>
        /// Gets the immediate files.
        /// </summary>
        /// <param name="fromDirPath">From dir path.</param>
        /// <returns>IEnumerable&lt;GistVirtualFile&gt;.</returns>
        public IEnumerable<GistVirtualFile> GetImmediateFiles(string fromDirPath)
        {
            return EnumerateFiles(fromDirPath)
                .Where(x => x.DirPath == fromDirPath);
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
        /// Sanitizes the path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>System.String.</returns>
        public override string SanitizePath(string filePath)
        {
            var sanitizedPath = string.IsNullOrEmpty(filePath)
                ? null
                : IsDirSep(filePath[0]) ? filePath.Substring(1) : filePath;

            return sanitizedPath?.Replace('/', DirSep);
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>System.String.</returns>
        public static string GetFileName(string filePath) => filePath.LastRightPart(DirSep);
    }

    /// <summary>
    /// Class GistVirtualFile.
    /// Implements the <see cref="ServiceStack.VirtualPath.AbstractVirtualFileBase" />
    /// </summary>
    /// <seealso cref="ServiceStack.VirtualPath.AbstractVirtualFileBase" />
    public class GistVirtualFile : AbstractVirtualFileBase
    {
        /// <summary>
        /// Gets or sets the path provider.
        /// </summary>
        /// <value>The path provider.</value>
        private GistVirtualFiles PathProvider { get; set; }

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <value>The client.</value>
        public IGistGateway Client => PathProvider.Gateway;

        /// <summary>
        /// Gets the gist identifier.
        /// </summary>
        /// <value>The gist identifier.</value>
        public string GistId => PathProvider.GistId;

        /// <summary>
        /// Gets the extension.
        /// </summary>
        /// <value>The extension.</value>
        public override string Extension => Name.LastRightPart('.').LeftPart('|');

        /// <summary>
        /// Initializes a new instance of the <see cref="GistVirtualFile"/> class.
        /// </summary>
        /// <param name="pathProvider">The path provider.</param>
        /// <param name="directory">The directory.</param>
        public GistVirtualFile(GistVirtualFiles pathProvider, IVirtualDirectory directory)
            : base(pathProvider, directory)
        {
            this.PathProvider = pathProvider;
        }

        /// <summary>
        /// Gets the dir path.
        /// </summary>
        /// <value>The dir path.</value>
        public string DirPath => ((GistVirtualDirectory)base.Directory).DirPath;

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>The file path.</value>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>The type of the content.</value>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name => GistVirtualFiles.GetFileName(FilePath);

        /// <summary>
        /// Gets the virtual path.
        /// </summary>
        /// <value>The virtual path.</value>
        public override string VirtualPath => FilePath.Replace('\\', '/');

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
        public override long Length => ContentLength;

        /// <summary>
        /// Gets or sets the length of the content.
        /// </summary>
        /// <value>The length of the content.</value>
        public long ContentLength { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; set; } // Empty for Binary Files
        /// <summary>
        /// Gets or sets the stream.
        /// </summary>
        /// <value>The stream.</value>
        public Stream Stream { get; set; }

        /// <summary>
        /// Initializes the specified file path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="lastModified">The last modified.</param>
        /// <param name="text">The text.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>GistVirtualFile.</returns>
        public GistVirtualFile Init(string filePath, DateTime lastModified, string text, MemoryStream stream)
        {
            FilePath = filePath;
            ContentType = MimeTypes.GetMimeType(filePath);
            FileLastModified = lastModified;
            ContentLength = stream.Length;
            Text = text;
            Stream = stream;
            return this;
        }

        /// <summary>
        /// Opens the read.
        /// </summary>
        /// <returns>Stream.</returns>
        public override Stream OpenRead()
        {
            Stream.Position = 0;
            return Stream.CopyToNewMemoryStream();
        }

        /// <summary>
        /// Gets the contents.
        /// </summary>
        /// <returns>System.Object.</returns>
        public override object GetContents()
        {
            return Text != null
                ? (object)Text.AsMemory()
                : Stream is MemoryStream ms
                    ? ms.GetBufferAsMemory()
                    : Stream?.CopyToNewMemoryStream().GetBufferAsMemory();
        }

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        /// <exception cref="System.IO.FileNotFoundException">Gist File no longer exists</exception>
        /// <exception cref="System.IO.FileNotFoundException">Gist no longer exists</exception>
        public override void Refresh()
        {
            var elapsed = DateTime.UtcNow - PathProvider.LastRefresh;
            var shouldRefresh = elapsed > PathProvider.RefreshAfter;
            var gist = PathProvider.GetGist(refresh: shouldRefresh);
            if (gist != null)
            {
                if (!GistVirtualFiles.GetGistContents(FilePath, gist, out var text, out var stream))
                    throw new FileNotFoundException("Gist File no longer exists", FilePath);

                Init(FilePath, gist.Updated_At ?? gist.Created_At, text, stream);
                return;
            }

            throw new FileNotFoundException("Gist no longer exists", GistId);
        }
    }

    /// <summary>
    /// Class GistVirtualDirectory.
    /// Implements the <see cref="ServiceStack.VirtualPath.AbstractVirtualDirectoryBase" />
    /// </summary>
    /// <seealso cref="ServiceStack.VirtualPath.AbstractVirtualDirectoryBase" />
    public class GistVirtualDirectory : AbstractVirtualDirectoryBase
    {
        /// <summary>
        /// Gets the path provider.
        /// </summary>
        /// <value>The path provider.</value>
        internal GistVirtualFiles PathProvider { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GistVirtualDirectory"/> class.
        /// </summary>
        /// <param name="pathProvider">The path provider.</param>
        /// <param name="dirPath">The dir path.</param>
        /// <param name="parentDir">The parent dir.</param>
        public GistVirtualDirectory(GistVirtualFiles pathProvider, string dirPath, IVirtualDirectory parentDir)
            : base(pathProvider, parentDir)
        {
            this.PathProvider = pathProvider;
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
        public override IEnumerable<IVirtualFile> Files => PathProvider.GetImmediateFiles(DirPath);

        /// <summary>
        /// Gets the directories.
        /// </summary>
        /// <value>The directories.</value>
        public override IEnumerable<IVirtualDirectory> Directories => PathProvider.GetImmediateDirectories(DirPath);

        /// <summary>
        /// Gets the gateway.
        /// </summary>
        /// <value>The gateway.</value>
        public IGistGateway Gateway => PathProvider.Gateway;

        /// <summary>
        /// Gets the gist identifier.
        /// </summary>
        /// <value>The gist identifier.</value>
        public string GistId => PathProvider.GistId;

        /// <summary>
        /// Gets or sets the dir path.
        /// </summary>
        /// <value>The dir path.</value>
        public string DirPath { get; set; }

        /// <summary>
        /// Gets the virtual path.
        /// </summary>
        /// <value>The virtual path.</value>
        public override string VirtualPath => DirPath?.Replace('\\', '/');

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name => DirPath?.LastRightPart(GistVirtualFiles.DirSep);

        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>IVirtualFile.</returns>
        public override IVirtualFile GetFile(string virtualPath)
        {
            return VirtualPathProvider.GetFile(DirPath.CombineWith(virtualPath));
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
            var matchingFilesInBackingDir = EnumerateFiles(globPattern);
            return matchingFilesInBackingDir;
        }

        /// <summary>
        /// Enumerates the files.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns>IEnumerable&lt;GistVirtualFile&gt;.</returns>
        public IEnumerable<GistVirtualFile> EnumerateFiles(string pattern)
        {
            foreach (var file in PathProvider.GetImmediateFiles(DirPath).Where(f => f.Name.Glob(pattern)))
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
            return new GistVirtualDirectory(PathProvider, PathProvider.SanitizePath(DirPath.CombineWith(directoryName)),
                this);
        }

        /// <summary>
        /// Adds the file.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <param name="contents">The contents.</param>
        public void AddFile(string virtualPath, string contents)
        {
            VirtualPathProvider.WriteFile(DirPath.CombineWith(virtualPath), contents);
        }

        /// <summary>
        /// Adds the file.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <param name="stream">The stream.</param>
        public void AddFile(string virtualPath, Stream stream)
        {
            VirtualPathProvider.WriteFile(DirPath.CombineWith(virtualPath), stream);
        }

        /// <summary>
        /// Strips the dir separator prefix.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>System.String.</returns>
        private static string StripDirSeparatorPrefix(string filePath)
        {
            return string.IsNullOrEmpty(filePath)
                ? filePath
                : filePath[0] == GistVirtualFiles.DirSep ? filePath.Substring(1) : filePath;
        }

        /// <summary>
        /// Gets all matching files.
        /// </summary>
        /// <param name="globPattern">The glob pattern.</param>
        /// <param name="maxDepth">The maximum depth.</param>
        /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
        public override IEnumerable<IVirtualFile> GetAllMatchingFiles(string globPattern, int maxDepth = int.MaxValue)
        {
            if (IsRoot)
            {
                return PathProvider.EnumerateFiles().Where(x =>
                    (x.DirPath == null || x.DirPath.CountOccurrencesOf(GistVirtualFiles.DirSep) < maxDepth - 1)
                    && x.Name.Glob(globPattern));
            }

            return PathProvider.EnumerateFiles(DirPath).Where(x =>
                x.DirPath != null
                && x.DirPath.CountOccurrencesOf(GistVirtualFiles.DirSep) < maxDepth - 1
                && x.DirPath.StartsWith(DirPath)
                && x.Name.Glob(globPattern));
        }
    }
}