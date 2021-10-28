// ***********************************************************************
// <copyright file="ResourceVirtualFiles.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Reflection;
using ServiceStack.DataAnnotations;
using ServiceStack.VirtualPath;

namespace ServiceStack.IO
{
    using ServiceStack.Text;

    /// <summary>
    /// Class ResourceVirtualFiles.
    /// Implements the <see cref="ServiceStack.VirtualPath.AbstractVirtualPathProviderBase" />
    /// </summary>
    /// <seealso cref="ServiceStack.VirtualPath.AbstractVirtualPathProviderBase" />
    public class ResourceVirtualFiles
        : AbstractVirtualPathProviderBase
    {
        /// <summary>
        /// The root dir
        /// </summary>
        protected ResourceVirtualDirectory RootDir;
        /// <summary>
        /// The backing assembly
        /// </summary>
        protected readonly Assembly BackingAssembly;
        /// <summary>
        /// The root namespace
        /// </summary>
        protected readonly string RootNamespace;

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
        public override string RealPathSeparator => ".";

        /// <summary>
        /// Gets or sets the last modified.
        /// </summary>
        /// <value>The last modified.</value>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceVirtualFiles"/> class.
        /// </summary>
        /// <param name="baseTypeInAssembly">The base type in assembly.</param>
        public ResourceVirtualFiles(Type baseTypeInAssembly)
            : this(baseTypeInAssembly.Assembly, GetNamespace(baseTypeInAssembly)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceVirtualFiles"/> class.
        /// </summary>
        /// <param name="backingAssembly">The backing assembly.</param>
        /// <param name="rootNamespace">The root namespace.</param>
        /// <exception cref="System.ArgumentNullException">backingAssembly</exception>
        public ResourceVirtualFiles(Assembly backingAssembly, string rootNamespace = null)
        {
            this.BackingAssembly = backingAssembly ?? throw new ArgumentNullException(nameof(backingAssembly));
            this.RootNamespace = rootNamespace ?? backingAssembly.GetName().Name;

            Initialize();
        }

        //https://docs.microsoft.com/en-us/dotnet/api/system.resources.tools.stronglytypedresourcebuilder.verifyresourcename?redirectedfrom=MSDN&view=netframework-4.8#remarks
        /// <summary>
        /// The namespace special chars
        /// </summary>
        static readonly char[] NamespaceSpecialChars = { ' ', '\u00A0', ',', ';', '|', '~', '@', '#', '%', '^', '&',
            '*', '+', '-', /*'/', '\\',*/ '<', '>', '?', '[', ']', '(', ')', '{',
            '}', '\"', '\'', '!'};

        /// <summary>
        /// Cleans the chars.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        private static string CleanChars(string name)
        {
            var newChars = new char[name.Length];
            var nameChars = name.AsSpan();
            for (var i = 0; i < nameChars.Length; i++)
            {
                newChars[i] = nameChars[i];
                foreach (var c in NamespaceSpecialChars)
                {
                    if (nameChars[i] == c)
                    {
                        newChars[i] = '_';
                        break;
                    }
                }
            }
            return new string(newChars);
        }

        /// <summary>
        /// Gets or sets the partial file names.
        /// </summary>
        /// <value>The partial file names.</value>
        public static HashSet<string> PartialFileNames { get; set; } = new()
        {
            "min.js",
            "min.css",
        };

        /// <summary>
        /// Cleans the path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>System.String.</returns>
        public string CleanPath(string filePath)
        {
            var sanitizedPath = base.SanitizePath(filePath);
            if (sanitizedPath == null)
                return null;
            var lastDirPos = sanitizedPath.LastIndexOf('/');
            if (lastDirPos >= 0)
            {
                var dirPath = sanitizedPath.Substring(0, lastDirPos);
                var fileName = sanitizedPath.Substring(lastDirPos + 1);
                if (PartialFileNames.Contains(fileName))
                {
                    var partialName = dirPath.LastRightPart('/');
                    dirPath = dirPath.LastLeftPart('/');
                    fileName = partialName + '.' + fileName;
                }

                var cleanDir = CleanChars(dirPath); //only dirs are replaced 
                var cleanPath = cleanDir + '/' + fileName;
                return cleanPath;
            }
            return sanitizedPath;
        }

        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>IVirtualFile.</returns>
        public override IVirtualFile GetFile(string virtualPath)
        {
            var virtualFile = RootDirectory.GetFile(CleanPath(virtualPath));
            virtualFile?.Refresh();
            return virtualFile;
        }

        /// <summary>
        /// Gets the namespace.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.String.</returns>
        private static string GetNamespace(Type type)
        {
            var attr = type.FirstAttribute<SchemaAttribute>();
            return attr != null ? attr.Name : type.Namespace;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected sealed override void Initialize()
        {
            var asm = BackingAssembly;
            RootDir = new ResourceVirtualDirectory(this, null, asm, LastModified, RootNamespace);
        }

        /// <summary>
        /// Combines the virtual path.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <param name="relativePath">The relative path.</param>
        /// <returns>System.String.</returns>
        public override string CombineVirtualPath(string basePath, string relativePath)
        {
            return string.Concat(basePath, VirtualPathSeparator, relativePath);
        }
    }
}