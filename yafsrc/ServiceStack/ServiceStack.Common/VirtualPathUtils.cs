// ***********************************************************************
// <copyright file="VirtualPathUtils.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ServiceStack.IO;

namespace ServiceStack
{
    /// <summary>
    /// Class VirtualPathUtils.
    /// </summary>
    public static class VirtualPathUtils
    {
        /// <summary>
        /// Tokenizes the virtual path.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="pathProvider">The path provider.</param>
        /// <returns>Stack&lt;System.String&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">pathProvider</exception>
        public static Stack<string> TokenizeVirtualPath(this string str, IVirtualPathProvider pathProvider)
        {
            if (pathProvider == null)
                throw new ArgumentNullException(nameof(pathProvider));

            return TokenizeVirtualPath(str, pathProvider.VirtualPathSeparator);
        }

        /// <summary>
        /// Tokenizes the virtual path.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="virtualPathSeparator">The virtual path separator.</param>
        /// <returns>Stack&lt;System.String&gt;.</returns>
        public static Stack<string> TokenizeVirtualPath(this string str, string virtualPathSeparator)
        {
            if (string.IsNullOrEmpty(str))
                return new Stack<string>();

            var tokens = str.Split(new[] { virtualPathSeparator }, StringSplitOptions.RemoveEmptyEntries);
            return new Stack<string>(tokens.Reverse());
        }

        /// <summary>
        /// Tokenizes the resource path.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="pathSeparator">The path separator.</param>
        /// <returns>Stack&lt;System.String&gt;.</returns>
        public static Stack<string> TokenizeResourcePath(this string str, char pathSeparator = '.')
        {
            if (string.IsNullOrEmpty(str))
                return new Stack<string>();

            var n = str.Count(c => c == pathSeparator);
            var tokens = str.Split(new[] { pathSeparator }, n);

            return new Stack<string>(tokens.Reverse());
        }

        /// <summary>
        /// Groups the by first token.
        /// </summary>
        /// <param name="resourceNames">The resource names.</param>
        /// <param name="pathSeparator">The path separator.</param>
        /// <returns>IEnumerable&lt;IGrouping&lt;System.String, System.String[]&gt;&gt;.</returns>
        public static IEnumerable<IGrouping<string, string[]>> GroupByFirstToken(this IEnumerable<string> resourceNames, char pathSeparator = '.')
        {
            return resourceNames.Select(n => n.Split(new[] { pathSeparator }, 2))
                .GroupBy(t => t[0]);
        }

        /// <summary>
        /// Reads all bytes.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] ReadAllBytes(this IVirtualFile file)
        {
            using var stream = file.OpenRead();
            var bytes = stream.ReadFully();
            return bytes;
        }

        /// <summary>
        /// Existses the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool Exists(this IVirtualNode node)
        {
            return node != null;
        }

        /// <summary>
        /// Determines whether the specified node is file.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns><c>true</c> if the specified node is file; otherwise, <c>false</c>.</returns>
        public static bool IsFile(this IVirtualNode node)
        {
            return node is IVirtualFile;
        }

        /// <summary>
        /// Determines whether the specified node is directory.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns><c>true</c> if the specified node is directory; otherwise, <c>false</c>.</returns>
        public static bool IsDirectory(this IVirtualNode node)
        {
            return node is IVirtualDirectory;
        }

        /// <summary>
        /// Gets the virtual node.
        /// </summary>
        /// <param name="pathProvider">The path provider.</param>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>IVirtualNode.</returns>
        public static IVirtualNode GetVirtualNode(this IVirtualPathProvider pathProvider, string virtualPath)
        {
            return (IVirtualNode)pathProvider.GetFile(virtualPath)
                ?? pathProvider.GetDirectory(virtualPath);
        }

        /// <summary>
        /// Gets the default document.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <param name="defaultDocuments">The default documents.</param>
        /// <returns>IVirtualFile.</returns>
        public static IVirtualFile GetDefaultDocument(this IVirtualDirectory dir, List<string> defaultDocuments)
        {
            foreach (var defaultDoc in defaultDocuments)
            {
                var defaultFile = dir.GetFile(defaultDoc);
                if (defaultFile == null) continue;

                return defaultFile;
            }

            return null;
        }

        /// <summary>
        /// Gets the maximum retry on exception timeout.
        /// </summary>
        /// <value>The maximum retry on exception timeout.</value>
        public static TimeSpan MaxRetryOnExceptionTimeout { get; } = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Sleeps the back off multiplier.
        /// </summary>
        /// <param name="i">The i.</param>
        internal static void SleepBackOffMultiplier(this int i)
        {
            var nextTryMs = (2 ^ i) * 50;
#if NET5_0_OR_GREATER
            System.Threading.Tasks.Task.Delay(nextTryMs).Wait();
#elif NET48
            System.Threading.Thread.Sleep(nextTryMs);
#endif
        }

        /// <summary>
        /// Safes the name of the file.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>System.String.</returns>
        public static string SafeFileName(string uri)
        {
            var invalidFileNameChars = new HashSet<char>(Path.GetInvalidFileNameChars()) { ':' };
            var safeFileName = new string(uri.Where(c => !invalidFileNameChars.Contains(c)).ToArray());
            return safeFileName;
        }
    }
}