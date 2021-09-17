// ***********************************************************************
// <copyright file="DirectoryInfoExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
#if !SL5
using System.Collections.Generic;
using System.IO;

namespace ServiceStack
{
    /// <summary>
    /// Class DirectoryInfoExtensions.
    /// </summary>
    public static class DirectoryInfoExtensions
    {
        /// <summary>
        /// Gets the matching files.
        /// </summary>
        /// <param name="rootDirPath">The root dir path.</param>
        /// <param name="fileSearchPattern">The file search pattern.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public static IEnumerable<string> GetMatchingFiles(this DirectoryInfo rootDirPath, string fileSearchPattern)
        {
            return GetMatchingFiles(rootDirPath.FullName, fileSearchPattern);
        }

        /// <summary>
        /// Gets the matching files.
        /// </summary>
        /// <param name="rootDirPath">The root dir path.</param>
        /// <param name="fileSearchPattern">The file search pattern.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public static IEnumerable<string> GetMatchingFiles(string rootDirPath, string fileSearchPattern)
        {
            var pending = new Queue<string>();
            pending.Enqueue(rootDirPath);

            while (pending.Count > 0)
            {
                rootDirPath = pending.Dequeue();
                var paths = Directory.GetFiles(rootDirPath, fileSearchPattern);
                foreach (var filePath in paths)
                {
                    yield return filePath;
                }
                paths = Directory.GetDirectories(rootDirPath);
                foreach (var dirPath in paths)
                {
                    var dirAttrs = File.GetAttributes(dirPath);
                    var isRecurseSymLink = (dirAttrs & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint;

                    if (!isRecurseSymLink)
                    {
                        pending.Enqueue(dirPath);
                    }
                }
            }
        }

    }

}
#endif