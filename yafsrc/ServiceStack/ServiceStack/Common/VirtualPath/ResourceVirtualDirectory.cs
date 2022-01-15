// ***********************************************************************
// <copyright file="ResourceVirtualDirectory.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.VirtualPath
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using ServiceStack.IO;
    using ServiceStack.Logging;
    using ServiceStack.Text;

    /// <summary>
    /// Class ResourceVirtualDirectory.
    /// Implements the <see cref="ServiceStack.VirtualPath.AbstractVirtualDirectoryBase" />
    /// </summary>
    /// <seealso cref="ServiceStack.VirtualPath.AbstractVirtualDirectoryBase" />
    public class ResourceVirtualDirectory : AbstractVirtualDirectoryBase
    {
        /// <summary>
        /// Gets or sets the embedded resource treat as files.
        /// </summary>
        /// <value>The embedded resource treat as files.</value>
        public static HashSet<string> EmbeddedResourceTreatAsFiles { get; set; } = new ();

        /// <summary>
        /// The log
        /// </summary>
        private static ILog Log = LogManager.GetLogger(typeof(ResourceVirtualDirectory));

        /// <summary>
        /// The backing assembly
        /// </summary>
        protected Assembly backingAssembly;
        /// <summary>
        /// Gets or sets the root namespace.
        /// </summary>
        /// <value>The root namespace.</value>
        public string rootNamespace { get; set; }

        /// <summary>
        /// The sub directories
        /// </summary>
        protected List<ResourceVirtualDirectory> SubDirectories;
        /// <summary>
        /// The sub files
        /// </summary>
        protected List<ResourceVirtualFile> SubFiles;

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <value>The files.</value>
        public override IEnumerable<IVirtualFile> Files => SubFiles;

        /// <summary>
        /// Gets the directories.
        /// </summary>
        /// <value>The directories.</value>
        public override IEnumerable<IVirtualDirectory> Directories => SubDirectories;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name => DirectoryName;

        /// <summary>
        /// Gets or sets the name of the directory.
        /// </summary>
        /// <value>The name of the directory.</value>
        public string DirectoryName { get; set; }

        /// <summary>
        /// Gets the last modified.
        /// </summary>
        /// <value>The last modified.</value>
        public override DateTime LastModified { get; }

        /// <summary>
        /// Gets the backing assembly.
        /// </summary>
        /// <value>The backing assembly.</value>
        internal Assembly BackingAssembly => backingAssembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceVirtualDirectory"/> class.
        /// </summary>
        /// <param name="owningProvider">The owning provider.</param>
        /// <param name="parentDir">The parent dir.</param>
        /// <param name="backingAsm">The backing asm.</param>
        /// <param name="lastModified">The last modified.</param>
        /// <param name="rootNamespace">The root namespace.</param>
        public ResourceVirtualDirectory(IVirtualPathProvider owningProvider,
            IVirtualDirectory parentDir,
            Assembly backingAsm,
            DateTime lastModified,
            string rootNamespace)
        : this(owningProvider,
            parentDir,
            backingAsm,
            lastModified,
            rootNamespace,
            rootNamespace,
            GetResourceNames(backingAsm, rootNamespace))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceVirtualDirectory"/> class.
        /// </summary>
        /// <param name="owningProvider">The owning provider.</param>
        /// <param name="parentDir">The parent dir.</param>
        /// <param name="backingAsm">The backing asm.</param>
        /// <param name="lastModified">The last modified.</param>
        /// <param name="rootNamespace">The root namespace.</param>
        /// <param name="directoryName">Name of the directory.</param>
        /// <param name="manifestResourceNames">The manifest resource names.</param>
        /// <exception cref="System.ArgumentNullException">directoryName</exception>
        /// <exception cref="System.ArgumentNullException">backingAsm</exception>
        public ResourceVirtualDirectory(IVirtualPathProvider owningProvider,
            IVirtualDirectory parentDir,
            Assembly backingAsm,
            DateTime lastModified,
            string rootNamespace,
            string directoryName,
            List<string> manifestResourceNames)
            : base(owningProvider, parentDir)
        {
            if (string.IsNullOrEmpty(directoryName))
                throw new ArgumentNullException(nameof(directoryName));

            this.backingAssembly = backingAsm ?? throw new ArgumentNullException(nameof(backingAsm));
            this.LastModified = lastModified;
            this.rootNamespace = rootNamespace;
            this.DirectoryName = directoryName;

            InitializeDirectoryStructure(manifestResourceNames);
        }

        /// <summary>
        /// Gets the resource names.
        /// </summary>
        /// <param name="asm">The asm.</param>
        /// <param name="basePath">The base path.</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        public static List<string> GetResourceNames(Assembly asm, string basePath)
        {
            return asm.GetManifestResourceNames()
                .Where(x => x.StartsWith(basePath))
                .Map(x => x.Substring(basePath.Length).TrimStart('.'));
        }

        /// <summary>
        /// Initializes the directory structure.
        /// </summary>
        /// <param name="manifestResourceNames">The manifest resource names.</param>
        protected void InitializeDirectoryStructure(List<string> manifestResourceNames)
        {
            SubDirectories = new List<ResourceVirtualDirectory>();
            SubFiles = new List<ResourceVirtualFile>();

            SubFiles.AddRange(manifestResourceNames
                .Where(n => n.Count(c => c == '.') <= 1 || EmbeddedResourceTreatAsFiles.Contains(n))
                .Select(CreateVirtualFile)
                .Where(f => f != null)
                .OrderBy(f => f.Name));

            SubDirectories.AddRange(manifestResourceNames
                .Where(n => n.Count(c => c == '.') > 1)
                .GroupByFirstToken(pathSeparator: '.')
                .Select(CreateVirtualDirectory)
                .OrderBy(d => d.Name));
        }

        /// <summary>
        /// Creates the virtual directory.
        /// </summary>
        /// <param name="subResources">The sub resources.</param>
        /// <returns>ResourceVirtualDirectory.</returns>
        protected virtual ResourceVirtualDirectory CreateVirtualDirectory(IGrouping<string, string[]> subResources)
        {
            var remainingResourceNames = subResources.Select(g => g[1]);
            var subDir = new ResourceVirtualDirectory(
                VirtualPathProvider, this, backingAssembly, LastModified, rootNamespace, subResources.Key, remainingResourceNames.ToList());

            return subDir;
        }

        /// <summary>
        /// Creates the virtual file.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>ResourceVirtualFile.</returns>
        protected virtual ResourceVirtualFile CreateVirtualFile(string resourceName)
        {
            try
            {
                var fullResourceName = string.Concat(RealPath, VirtualPathProvider.RealPathSeparator, resourceName);

                var resourceNames = new[]
                {
                    fullResourceName,
                    fullResourceName.Replace(VirtualPathProvider.RealPathSeparator, ".").Trim('.')
                };

                var mrInfo = resourceNames.FirstOrDefault(x => backingAssembly.GetManifestResourceInfo(x) != null);
                if (mrInfo == null)
                {
                    Log.Warn("Virtual file not found: " + fullResourceName);
                    return null;
                }

                return new ResourceVirtualFile(VirtualPathProvider, this, resourceName);
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message, ex);
                return null;
            }
        }

        /// <summary>
        /// Consumes the tokens for virtual dir.
        /// </summary>
        /// <param name="resourceTokens">The resource tokens.</param>
        /// <returns>ResourceVirtualDirectory.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected virtual ResourceVirtualDirectory ConsumeTokensForVirtualDir(Stack<string> resourceTokens)
        {
            var subDirName = resourceTokens.Pop();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public override IEnumerator<IVirtualNode> GetEnumerator()
        {
            return Directories.Cast<IVirtualNode>().Union(Files.Cast<IVirtualNode>()).GetEnumerator();
        }

        /// <summary>
        /// Gets the file from backing directory or default.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>IVirtualFile.</returns>
        protected override IVirtualFile GetFileFromBackingDirectoryOrDefault(string fileName)
        {
            var file = Files.FirstOrDefault(f => f.Name.EqualsIgnoreCase(fileName));
            if (file != null)
                return file;

            //ResourceDir reads /path/to/a.min.js as path.to.min.js and lays out as /path/to/a/min.js
            var parts = fileName.SplitOnFirst('.');
            if (parts.Length > 1)
            {
                if (GetDirectoryFromBackingDirectoryOrDefault(parts[0]) is ResourceVirtualDirectory dir)
                {
                    return dir.GetFileFromBackingDirectoryOrDefault(parts[1]);
                }
            }

            return null;
        }

        /// <summary>
        /// The translate path.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string TranslatePath(string path) => path.Replace('-', '_');

        /// <summary>
        /// Gets the matching files in dir.
        /// </summary>
        /// <param name="globPattern">The glob pattern.</param>
        /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
        protected override IEnumerable<IVirtualFile> GetMatchingFilesInDir(string globPattern)
        {
            var useGlob = globPattern.TrimStart('/');
            var useGlobTranslate = this.TranslatePath(useGlob);

            return this.Files.Where(
                f => useGlob.IndexOf('/') >= 0
                         ? f.VirtualPath.Glob(useGlob) || f.VirtualPath.Glob(useGlobTranslate)
                         : f.Name.Glob(useGlob) || f.Name.Glob(useGlobTranslate));
        }

        /// <summary>
        /// Gets the directory from backing directory or default.
        /// </summary>
        /// <param name="directoryName">Name of the directory.</param>
        /// <returns>IVirtualDirectory.</returns>
        protected override IVirtualDirectory GetDirectoryFromBackingDirectoryOrDefault(string directoryName)
        {
            return this.Directories.FirstOrDefault(d => d.Name.EqualsIgnoreCase(directoryName))
                   ?? this.Directories.FirstOrDefault(d => d.Name.EqualsIgnoreCase(TranslatePath(directoryName ?? "")));
        }

        /// <summary>
        /// Gets the real path to root.
        /// </summary>
        /// <returns>System.String.</returns>
        protected override string GetRealPathToRoot()
        {
            var path = base.GetRealPathToRoot();
            return path.TrimStart('.');
        }
    }
}
