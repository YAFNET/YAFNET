// ***********************************************************************
// <copyright file="SharpPage.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Script
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using ServiceStack.IO;
    using ServiceStack.Text;

#if NET48
    using ServiceStack.Text.Extensions;
#endif

    /// <summary>
    /// Class SharpPage.
    /// </summary>
    public class SharpPage
    {
        /// <summary>
        /// Whether to evaluate as Template block or code block
        /// </summary>
        /// <value>The script language.</value>
        public ScriptLanguage ScriptLanguage { get; set; }
        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <value>The file.</value>
        public IVirtualFile File { get; }
        /// <summary>
        /// Gets the file contents.
        /// </summary>
        /// <value>The file contents.</value>
        public ReadOnlyMemory<char> FileContents { get; private set; }
        /// <summary>
        /// Gets the body contents.
        /// </summary>
        /// <value>The body contents.</value>
        public ReadOnlyMemory<char> BodyContents { get; private set; }
        /// <summary>
        /// Gets or sets the arguments.
        /// </summary>
        /// <value>The arguments.</value>
        public Dictionary<string, object> Args { get; protected set; }
        /// <summary>
        /// Gets or sets the layout page.
        /// </summary>
        /// <value>The layout page.</value>
        public SharpPage LayoutPage { get; set; }
        /// <summary>
        /// Gets or sets the page fragments.
        /// </summary>
        /// <value>The page fragments.</value>
        public PageFragment[] PageFragments { get; set; }
        /// <summary>
        /// Gets or sets the last modified.
        /// </summary>
        /// <value>The last modified.</value>
        public DateTime LastModified { get; set; }
        /// <summary>
        /// Gets the last modified check.
        /// </summary>
        /// <value>The last modified check.</value>
        public DateTime LastModifiedCheck { get; private set; }
        /// <summary>
        /// Gets a value indicating whether this instance has initialize.
        /// </summary>
        /// <value><c>true</c> if this instance has initialize; otherwise, <c>false</c>.</value>
        public bool HasInit { get; private set; }
        /// <summary>
        /// Gets a value indicating whether this instance is layout.
        /// </summary>
        /// <value><c>true</c> if this instance is layout; otherwise, <c>false</c>.</value>
        public bool IsLayout { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is immutable.
        /// </summary>
        /// <value><c>true</c> if this instance is immutable; otherwise, <c>false</c>.</value>
        public bool IsImmutable { get; private set; }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        public ScriptContext Context { get; }
        /// <summary>
        /// Gets the format.
        /// </summary>
        /// <value>The format.</value>
        public PageFormat Format { get; }
        /// <summary>
        /// The semaphore
        /// </summary>
        private readonly object semaphore = new();

        /// <summary>
        /// Gets a value indicating whether this instance is temporary file.
        /// </summary>
        /// <value><c>true</c> if this instance is temporary file; otherwise, <c>false</c>.</value>
        public bool IsTempFile => File.Directory.VirtualPath == ScriptConstants.TempFilePath;
        /// <summary>
        /// Gets the virtual path.
        /// </summary>
        /// <value>The virtual path.</value>
        public string VirtualPath => IsTempFile ? "{temp file}" : File.VirtualPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharpPage"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="file">The file.</param>
        /// <param name="format">The format.</param>
        /// <exception cref="System.ArgumentNullException">context</exception>
        /// <exception cref="System.ArgumentNullException">file</exception>
        /// <exception cref="System.ArgumentException">File with extension '{File.Extension}' is not a registered PageFormat in Context.PageFormats - file</exception>
        public SharpPage(ScriptContext context, IVirtualFile file, PageFormat format = null)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            ScriptLanguage = context.DefaultScriptLanguage;
            File = file ?? throw new ArgumentNullException(nameof(file));

            Format = format ?? Context.GetFormat(File.Extension);
            if (Format == null)
                throw new ArgumentException($"File with extension '{File.Extension}' is not a registered PageFormat in Context.PageFormats", nameof(file));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharpPage"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="body">The body.</param>
        /// <exception cref="System.ArgumentNullException">context</exception>
        /// <exception cref="System.ArgumentNullException">body</exception>
        public SharpPage(ScriptContext context, PageFragment[] body)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            PageFragments = body ?? throw new ArgumentNullException(nameof(body));
            Format = Context.PageFormats[0];
            ScriptLanguage = context.DefaultScriptLanguage;
            Args = TypeConstants.EmptyObjectDictionary;
            File = context.EmptyFile;
            HasInit = true;
            IsImmutable = true;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns>SharpPage.</returns>
        public virtual async Task<SharpPage> Init()
        {
            if (IsImmutable)
                return this;

            if (HasInit)
            {
                var skipCheck = !Context.DebugMode &&
                    (Context.CheckForModifiedPagesAfter != null
                        ? DateTime.UtcNow - LastModifiedCheck < Context.CheckForModifiedPagesAfter.Value
                        : !Context.CheckForModifiedPages) &&
                    (Context.InvalidateCachesBefore == null || LastModifiedCheck > Context.InvalidateCachesBefore.Value);

                if (skipCheck)
                    return this;

                File.Refresh();
                LastModifiedCheck = DateTime.UtcNow;
                if (File.LastModified == LastModified)
                    return this;
            }

            return await Load().ConfigAwait();
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <returns>SharpPage.</returns>
        public async Task<SharpPage> Load()
        {
            if (IsImmutable)
                return this;

            string contents;
            using (var stream = File.OpenRead())
            {
                contents = await stream.ReadToEndAsync();
            }

            foreach (var preprocessor in Context.Preprocessors)
            {
                contents = preprocessor(contents);
            }

            var lastModified = File.LastModified;
            var fileContents = contents.AsMemory();
            var pageVars = new Dictionary<string, object>();

            var pos = 0;
            var bodyContents = fileContents;
            fileContents.AdvancePastWhitespace().TryReadLine(out ReadOnlyMemory<char> line, ref pos);
            var lineComment = ScriptLanguage.LineComment;
            if (line.StartsWith(Format.ArgsPrefix) || lineComment != null && line.StartsWith(lineComment + Format.ArgsPrefix))
            {
                while (fileContents.TryReadLine(out line, ref pos))
                {
                    if (line.Trim().Length == 0)
                        continue;


                    if (line.StartsWith(Format.ArgsSuffix) || lineComment != null && line.StartsWith(lineComment + Format.ArgsSuffix))
                        break;

                    if (lineComment != null && line.StartsWith(lineComment))
                        line = line.Slice(lineComment.Length).TrimStart();

                    var colonPos = line.IndexOf(':');
                    var spacePos = line.IndexOf(' ');
                    var bracePos = line.IndexOf('{');
                    var sep = colonPos >= 0 ? ':' : ' ';

                    if (bracePos > 0 && spacePos > 0 && colonPos > spacePos)
                        sep = ' ';

                    line.SplitOnFirst(sep, out var first, out var last);

                    var key = first.Trim().ToString();
                    pageVars[key] = !last.IsEmpty ? last.Trim().ToString() : "";
                }

                //When page has variables body starts from first non whitespace after variables end  
                var argsSuffixPos = line.LastIndexOf(Format.ArgsSuffix);
                if (argsSuffixPos >= 0)
                {
                    //Start back from the end of the ArgsSuffix
                    pos -= line.Length - argsSuffixPos - Format.ArgsSuffix.Length;
                }
                bodyContents = fileContents.SafeSlice(pos).AdvancePastWhitespace();
            }

            var pageFragments = pageVars.TryGetValue("ignore", out object ignore)
                    && ("page".Equals(ignore.ToString()) || "template".Equals(ignore.ToString()))
                ? new List<PageFragment> { new PageStringFragment(bodyContents) }
                : ScriptLanguage.Parse(Context, bodyContents);

            foreach (var fragment in pageFragments)
            {
                if (fragment is PageVariableFragment var && var.Binding == ScriptConstants.Page)
                {
                    IsLayout = true;
                    break;
                }
            }

            lock (semaphore)
            {
                LastModified = lastModified;
                LastModifiedCheck = DateTime.UtcNow;
                FileContents = fileContents;
                Args = pageVars;
                BodyContents = bodyContents;
                PageFragments = pageFragments.ToArray();

                HasInit = true;
                LayoutPage = Format.ResolveLayout(this);
            }

            if (LayoutPage != null)
            {
                if (!LayoutPage.HasInit)
                {
                    await LayoutPage.Load();
                }
                else
                {
                    if (Context.DebugMode || Context.CheckForModifiedPagesAfter != null &&
                        DateTime.UtcNow - LayoutPage.LastModifiedCheck >= Context.CheckForModifiedPagesAfter.Value)
                    {
                        LayoutPage.File.Refresh();
                        LayoutPage.LastModifiedCheck = DateTime.UtcNow;
                        if (LayoutPage.File.LastModified != LayoutPage.LastModified)
                            await LayoutPage.Load();
                    }
                }
            }

            return this;
        }
    }

    /// <summary>
    /// Class SharpPartialPage.
    /// Implements the <see cref="ServiceStack.Script.SharpPage" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.SharpPage" />
    public class SharpPartialPage : SharpPage
    {
        /// <summary>
        /// The temporary files
        /// </summary>
        private static readonly MemoryVirtualFiles TempFiles = new();
        /// <summary>
        /// The temporary dir
        /// </summary>
        private static readonly InMemoryVirtualDirectory TempDir = new(TempFiles, ScriptConstants.TempFilePath);

        /// <summary>
        /// Creates the file.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="format">The format.</param>
        /// <returns>IVirtualFile.</returns>
        static IVirtualFile CreateFile(string name, string format) =>
            new InMemoryVirtualFile(TempFiles, TempDir)
            {
                FilePath = name + "." + format,
                TextContents = "",
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="SharpPartialPage"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="name">The name.</param>
        /// <param name="body">The body.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public SharpPartialPage(ScriptContext context, string name, IEnumerable<PageFragment> body, string format, Dictionary<string, object> args = null)
            : base(context, CreateFile(name, format), context.GetFormat(format))
        {
            PageFragments = body.ToArray();
            Args = args ?? new Dictionary<string, object>();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns>Task&lt;SharpPage&gt;.</returns>
        public override Task<SharpPage> Init() => ((SharpPage)this).InTask();
    }
}