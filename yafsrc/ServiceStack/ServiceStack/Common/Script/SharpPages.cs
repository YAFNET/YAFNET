// ***********************************************************************
// <copyright file="SharpPages.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Concurrent;
using System.Linq;
using ServiceStack.IO;

namespace ServiceStack.Script
{
    using ServiceStack.Extensions;
    using ServiceStack.Text;

    /// <summary>
    /// Interface ISharpPages
    /// </summary>
    public interface ISharpPages
    {
        /// <summary>
        /// Resolves the layout page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="layout">The layout.</param>
        /// <returns>SharpPage.</returns>
        SharpPage ResolveLayoutPage(SharpPage page, string layout);
        /// <summary>
        /// Adds the page.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <param name="file">The file.</param>
        /// <returns>SharpPage.</returns>
        SharpPage AddPage(string virtualPath, IVirtualFile file);
        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>SharpPage.</returns>
        SharpPage GetPage(string virtualPath);
        /// <summary>
        /// Tries the get page.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>SharpPage.</returns>
        SharpPage TryGetPage(string path);
        /// <summary>
        /// Called when [time page].
        /// </summary>
        /// <param name="contents">The contents.</param>
        /// <param name="ext">The ext.</param>
        /// <returns>SharpPage.</returns>
        SharpPage OneTimePage(string contents, string ext);
        /// <summary>
        /// Called when [time page].
        /// </summary>
        /// <param name="contents">The contents.</param>
        /// <param name="ext">The ext.</param>
        /// <param name="init">The initialize.</param>
        /// <returns>SharpPage.</returns>
        SharpPage OneTimePage(string contents, string ext, Action<SharpPage> init);

        /// <summary>
        /// Resolves the layout page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="layout">The layout.</param>
        /// <returns>SharpPage.</returns>
        SharpPage ResolveLayoutPage(SharpCodePage page, string layout);
        /// <summary>
        /// Gets the code page.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>SharpCodePage.</returns>
        SharpCodePage GetCodePage(string virtualPath);

        /// <summary>
        /// Gets the last modified.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>DateTime.</returns>
        DateTime GetLastModified(SharpPage page);
    }

    /// <summary>
    /// Class SharpPages.
    /// Implements the <see cref="ServiceStack.Script.ISharpPages" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ISharpPages" />
    public partial class SharpPages : ISharpPages
    {
        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        public ScriptContext Context { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharpPages"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public SharpPages(ScriptContext context) => this.Context = context;

        /// <summary>
        /// The layout
        /// </summary>
        public static string Layout = "layout";

        /// <summary>
        /// The page map
        /// </summary>
        readonly ConcurrentDictionary<string, SharpPage> pageMap = new();

        /// <summary>
        /// Resolves the layout page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="layout">The layout.</param>
        /// <returns>SharpPage.</returns>
        /// <exception cref="System.ArgumentNullException">page</exception>
        /// <exception cref="System.ArgumentException">Page {page.File.VirtualPath} has not been initialized</exception>
        public virtual SharpPage ResolveLayoutPage(SharpPage page, string layout)
        {
            if (page == null)
                throw new ArgumentNullException(nameof(page));

            if (!page.HasInit)
                throw new ArgumentException($"Page {page.File.VirtualPath} has not been initialized");

            if (page.IsLayout)
                return null;

            var layoutWithoutExt = (layout ?? Context.DefaultLayoutPage).LeftPart('.');

            var dir = page.File.Directory;
            do
            {
                var layoutPath = (dir.VirtualPath ?? "").CombineWith(layoutWithoutExt);

                if (pageMap.TryGetValue(layoutPath, out SharpPage layoutPage))
                    return layoutPage;

                foreach (var format in Context.PageFormats)
                {
                    var layoutFile = dir.GetFile($"{layoutWithoutExt}.{format.Extension}");
                    if (layoutFile != null)
                        return AddPage(layoutPath, layoutFile);
                }

                if (dir.IsRoot)
                    break;

                dir = dir.ParentDirectory;

            } while (dir != null);

            return null;
        }

        /// <summary>
        /// Resolves the layout page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="layout">The layout.</param>
        /// <returns>SharpPage.</returns>
        /// <exception cref="System.ArgumentNullException">page</exception>
        /// <exception cref="System.ArgumentException">Page {page.VirtualPath} has not been initialized</exception>
        public virtual SharpPage ResolveLayoutPage(SharpCodePage page, string layout)
        {
            if (page == null)
                throw new ArgumentNullException(nameof(page));

            if (!page.HasInit)
                throw new ArgumentException($"Page {page.VirtualPath} has not been initialized");

            var layoutWithoutExt = (layout ?? Context.DefaultLayoutPage).LeftPart('.');

            var lastDirPos = page.VirtualPath.LastIndexOf('/');
            var dirPath = lastDirPos >= 0
                ? page.VirtualPath.Substring(0, lastDirPos)
                : null;
            var dir = !string.IsNullOrEmpty(dirPath)
                ? Context.VirtualFiles.GetDirectory(dirPath)
                : Context.VirtualFiles.RootDirectory;
            do
            {
                var layoutPath = (dir.VirtualPath ?? "").CombineWith(layoutWithoutExt);

                if (pageMap.TryGetValue(layoutPath, out SharpPage layoutPage))
                    return layoutPage;

                foreach (var format in Context.PageFormats)
                {
                    var layoutFile = dir.GetFile($"{layoutWithoutExt}.{format.Extension}");
                    if (layoutFile != null)
                        return AddPage(layoutPath, layoutFile);
                }

                if (dir.IsRoot)
                    break;

                dir = dir.ParentDirectory;

            } while (dir != null);

            return null;
        }

        /// <summary>
        /// Gets the code page.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>SharpCodePage.</returns>
        public SharpCodePage GetCodePage(string virtualPath) => Context.GetCodePage(virtualPath)
            ?? (virtualPath?.Length > 0 && virtualPath[virtualPath.Length - 1] != '/' ? Context.GetCodePage(virtualPath + '/') : null);

        /// <summary>
        /// Adds the page.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <param name="file">The file.</param>
        /// <returns>SharpPage.</returns>
        public virtual SharpPage AddPage(string virtualPath, IVirtualFile file)
        {
            return pageMap[virtualPath] = new SharpPage(Context, file);
        }

        /// <summary>
        /// Tries the get page.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>SharpPage.</returns>
        public virtual SharpPage TryGetPage(string path)
        {
            var sanitizePath = path.Replace('\\', '/').TrimPrefixes("/").LastLeftPart('.');

            if (pageMap.TryGetValue(sanitizePath, out SharpPage page))
                return page;

            return null;
        }

        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <param name="pathInfo">The path information.</param>
        /// <returns>SharpPage.</returns>
        public virtual SharpPage GetPage(string pathInfo)
        {
            if (string.IsNullOrEmpty(pathInfo))
                return null;

            var sanitizePath = pathInfo.Replace('\\', '/').TrimPrefixes("/");
            var isDirectory = sanitizePath.Length == 0 || sanitizePath[sanitizePath.Length - 1] == '/';

            SharpPage page = null;
            var mappedPath = Context.GetPathMapping(nameof(SharpPages), sanitizePath);
            if (mappedPath != null)
            {
                page = TryGetPage(mappedPath);
                if (page != null)
                    return page;
                Context.RemovePathMapping(nameof(SharpPages), mappedPath);
            }

            var fileNameParts = sanitizePath.LastRightPart('/').SplitOnLast('.');
            var ext = fileNameParts.Length > 1 ? fileNameParts[1] : null;
            if (ext != null)
            {
                var registeredPageExt = Context.PageFormats.Any(x => x.Extension == ext);
                if (!registeredPageExt)
                    return null;
            }

            var filePath = sanitizePath.LastLeftPart('.');
            page = TryGetPage(filePath) ?? (!isDirectory ? TryGetPage(filePath + '/') : null);
            if (page != null)
                return page;

            foreach (var format in Context.PageFormats)
            {
                var file = !isDirectory
                    ? Context.VirtualFiles.GetFile(filePath + "." + format.Extension)
                    : Context.VirtualFiles.GetFile(filePath + Context.IndexPage + "." + format.Extension);

                if (file != null)
                {
                    var pageVirtualPath = file.VirtualPath.WithoutExtension();
                    Context.SetPathMapping(nameof(SharpPages), sanitizePath, pageVirtualPath);
                    return AddPage(pageVirtualPath, file);
                }
            }

            if (!isDirectory)
            {
                var tryFilePath = filePath + '/';
                foreach (var format in Context.PageFormats)
                {
                    var file = Context.VirtualFiles.GetFile(tryFilePath + Context.IndexPage + "." + format.Extension);
                    if (file != null)
                    {
                        var pageVirtualPath = file.VirtualPath.WithoutExtension();
                        Context.SetPathMapping(nameof(SharpPages), sanitizePath, pageVirtualPath);
                        return AddPage(pageVirtualPath, file);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// The temporary files
        /// </summary>
        private static MemoryVirtualFiles tempFiles;
        /// <summary>
        /// Gets the temporary files.
        /// </summary>
        /// <value>The temporary files.</value>
        internal static MemoryVirtualFiles TempFiles => tempFiles ??= new MemoryVirtualFiles();
        /// <summary>
        /// The temporary dir
        /// </summary>
        private static readonly InMemoryVirtualDirectory tempDir;
        /// <summary>
        /// The temporary dir
        /// </summary>
        internal static readonly InMemoryVirtualDirectory TempDir = tempDir ??= new InMemoryVirtualDirectory(TempFiles, ScriptConstants.TempFilePath);

        /// <summary>
        /// Called when [time page].
        /// </summary>
        /// <param name="contents">The contents.</param>
        /// <param name="ext">The ext.</param>
        /// <returns>SharpPage.</returns>
        public virtual SharpPage OneTimePage(string contents, string ext) => OneTimePage(contents, ext, init: null);

        /// <summary>
        /// Called when [time page].
        /// </summary>
        /// <param name="contents">The contents.</param>
        /// <param name="ext">The ext.</param>
        /// <param name="init">The initialize.</param>
        /// <returns>SharpPage.</returns>
        public SharpPage OneTimePage(string contents, string ext, Action<SharpPage> init)
        {
            var memFile = new InMemoryVirtualFile(TempFiles, TempDir)
            {
                FilePath = Guid.NewGuid().ToString("n") + "." + ext,
                TextContents = contents,
            };

            var page = new SharpPage(Context, memFile);

            try
            {
                init?.Invoke(page);

                page.Init().Wait(); // Safe as Memory Files are non-blocking
                return page;
            }
            catch (AggregateException e)
            {
#if DEBUG
                var logEx = e.InnerExceptions[0].GetInnerMostException();
                Logging.LogManager.GetLogger(typeof(SharpPages)).Error(logEx.Message + "\n" + logEx.StackTrace, logEx);
#endif
                throw e.UnwrapIfSingleException();
            }
        }

        /// <summary>
        /// Gets the last modified.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>DateTime.</returns>
        /// <exception cref="System.ArgumentNullException">page</exception>
        public DateTime GetLastModified(SharpPage page)
        {
            if (page == null)
                throw new ArgumentNullException(nameof(page));

            page.File.Refresh();
            var maxLastModified = page.File.LastModified;

            var layout = page.IsLayout ? null : page.LayoutPage ?? ResolveLayoutPage(page, null);
            if (layout != null)
            {
                var layoutLastModified = GetLastModifiedPage(layout);
                if (layoutLastModified > maxLastModified)
                    maxLastModified = layoutLastModified;
            }

            var pageLastModified = GetLastModifiedPage(page);
            if (pageLastModified > maxLastModified)
                maxLastModified = pageLastModified;

            return maxLastModified;
        }

        /// <summary>
        /// Gets the last modified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>DateTime.</returns>
        /// <exception cref="System.ArgumentNullException">page</exception>
        public DateTime GetLastModifiedPage(SharpPage page)
        {
            if (page == null)
                throw new ArgumentNullException(nameof(page));

            page.File.Refresh();
            var maxLastModified = page.File.LastModified;

            var varFragments = page.PageFragments.OfType<PageVariableFragment>();
            foreach (var fragment in varFragments)
            {
                var filter = fragment.FilterExpressions?.FirstOrDefault();
                if (filter?.Name == "partial")
                {
                    if (fragment.InitialValue is string partialPath)
                    {
                        Context.TryGetPage(page.VirtualPath, partialPath, out SharpPage partialPage, out _);
                        if (partialPage == null && partialPath[0] != '_')
                            Context.TryGetPage(page.VirtualPath, $"_{partialPath}-partial", out partialPage, out _);

                        maxLastModified = GetMaxLastModified(partialPage?.File, maxLastModified);

                        if (partialPage?.HasInit == true)
                        {
                            var partialLastModified = GetLastModifiedPage(partialPage);
                            if (partialLastModified > maxLastModified)
                                maxLastModified = partialLastModified;
                        }
                    }
                }
                else if (filter?.Name != null && Context.FileFilterNames.Contains(filter?.Name))
                {
                    if (fragment.InitialValue is string filePath)
                    {
                        var file = Context.ProtectedMethods.ResolveFile(Context.VirtualFiles, page.VirtualPath, filePath);
                        maxLastModified = GetMaxLastModified(file, maxLastModified);
                    }
                }

                var lastFilter = fragment.FilterExpressions?.LastOrDefault();
                if (lastFilter?.Name == "selectPartial")
                {
                    if (lastFilter.Arguments.FirstOrDefault() is JsLiteral argLiteral && argLiteral.Value is string partialArg)
                    {
                        if (!string.IsNullOrEmpty(partialArg))
                        {
                            Context.TryGetPage(page.VirtualPath, partialArg, out SharpPage partialPage, out _);
                            if (partialPage == null && partialArg[0] != '_')
                                Context.TryGetPage(page.VirtualPath, $"_{partialArg}-partial", out partialPage, out _);

                            maxLastModified = GetMaxLastModified(partialPage?.File, maxLastModified);

                            if (partialPage?.HasInit == true)
                            {
                                var partialLastModified = GetLastModifiedPage(partialPage);
                                if (partialLastModified > maxLastModified)
                                    maxLastModified = partialLastModified;
                            }
                        }
                    }
                }
            }

            return maxLastModified;
        }

        /// <summary>
        /// Gets the maximum last modified.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="maxLastModified">The maximum last modified.</param>
        /// <returns>DateTime.</returns>
        private DateTime GetMaxLastModified(IVirtualFile file, DateTime maxLastModified)
        {
            if (file == null)
                return maxLastModified;

            file.Refresh();
            return file.LastModified > maxLastModified
                ? file.LastModified
                : maxLastModified;
        }
    }
}