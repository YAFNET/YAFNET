// ***********************************************************************
// <copyright file="ScriptContext.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ServiceStack.Configuration;
using ServiceStack.IO;
using ServiceStack.Logging;
using ServiceStack.Text;

namespace ServiceStack.Script
{
    using ServiceStack.Extensions;

    /// <summary>
    /// Interface IConfigureScriptContext
    /// </summary>
    public interface IConfigureScriptContext
    {
        /// <summary>
        /// Configures the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        void Configure(ScriptContext context);
    }

    /// <summary>
    /// Interface IConfigurePageResult
    /// </summary>
    public interface IConfigurePageResult
    {
        /// <summary>
        /// Configures the specified page result.
        /// </summary>
        /// <param name="pageResult">The page result.</param>
        void Configure(PageResult pageResult);
    }

    /// <summary>
    /// Class ScriptContext.
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public partial class ScriptContext : IDisposable
    {
        /// <summary>
        /// Gets or sets the page formats.
        /// </summary>
        /// <value>The page formats.</value>
        public List<PageFormat> PageFormats { get; set; } = new List<PageFormat>();

        /// <summary>
        /// Gets or sets the index page.
        /// </summary>
        /// <value>The index page.</value>
        public string IndexPage { get; set; } = "index";

        /// <summary>
        /// Gets or sets the default layout page.
        /// </summary>
        /// <value>The default layout page.</value>
        public string DefaultLayoutPage { get; set; } = "_layout";

        /// <summary>
        /// Gets or sets the pages.
        /// </summary>
        /// <value>The pages.</value>
        public ISharpPages Pages { get; set; }

        /// <summary>
        /// Gets or sets the virtual files.
        /// </summary>
        /// <value>The virtual files.</value>
        public IVirtualPathProvider VirtualFiles { get; set; } = new MemoryVirtualFiles();

        /// <summary>
        /// Where to store cached files, if unspecified falls back to configured VirtualFiles if it implements IVirtualFiles (i.e. writable)
        /// </summary>
        /// <value>The cache files.</value>
        public IVirtualFiles CacheFiles { get; set; }

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        /// <value>The arguments.</value>
        public Dictionary<string, object> Args { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets a value indicating whether [debug mode].
        /// </summary>
        /// <value><c>true</c> if [debug mode]; otherwise, <c>false</c>.</value>
        public bool DebugMode { get; set; } = true;

        /// <summary>
        /// Gets the format.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns>PageFormat.</returns>
        public PageFormat GetFormat(string extension) => PageFormats.FirstOrDefault(x => x.Extension == extension);

        /// <summary>
        /// Scan Types and auto-register any Script Methods, Blocks and Code Pages
        /// </summary>
        /// <value>The scan types.</value>
        public List<Type> ScanTypes { get; set; } = new List<Type>();

        /// <summary>
        /// Scan Assemblies and auto-register any Script Methods, Blocks and Code Pages
        /// </summary>
        /// <value>The scan assemblies.</value>
        public List<Assembly> ScanAssemblies { get; set; } = new List<Assembly>();

        /// <summary>
        /// Allow scripting of Types from specified Assemblies
        /// </summary>
        /// <value>The script assemblies.</value>
        public List<Assembly> ScriptAssemblies { get; set; } = new List<Assembly>();

        /// <summary>
        /// Allow scripting of the specified Types
        /// </summary>
        /// <value>The script types.</value>
        public List<Type> ScriptTypes { get; set; } = new List<Type>();

        /// <summary>
        /// Lookup Namespaces for resolving Types in Scripts
        /// </summary>
        /// <value>The script namespaces.</value>
        public List<string> ScriptNamespaces { get; set; } = new List<string>();

        /// <summary>
        /// Allow scripting of all Types in loaded Assemblies
        /// </summary>
        /// <value><c>true</c> if [allow scripting of all types]; otherwise, <c>false</c>.</value>
        public bool AllowScriptingOfAllTypes { get; set; }

        /// <summary>
        /// Register short Type name accessible from scripts. (Advanced, use ScriptAssemblies/ScriptTypes first)
        /// </summary>
        /// <value>The script type name map.</value>
        public Dictionary<string, Type> ScriptTypeNameMap { get; } = new Dictionary<string, Type>();
        /// <summary>
        /// Register long qualified Type name accessible from scripts. (Advanced, use ScriptAssemblies/ScriptTypes first)
        /// </summary>
        /// <value>The script type qualified name map.</value>
        public Dictionary<string, Type> ScriptTypeQualifiedNameMap { get; } = new Dictionary<string, Type>();

        /// <summary>
        /// Gets or sets the container.
        /// </summary>
        /// <value>The container.</value>
        public IContainer Container { get; set; } = new SimpleContainer();

        /// <summary>
        /// Gets or sets the application settings.
        /// </summary>
        /// <value>The application settings.</value>
        public IAppSettings AppSettings { get; set; } = new SimpleAppSettings();

        /// <summary>
        /// Gets the preprocessors.
        /// </summary>
        /// <value>The preprocessors.</value>
        public List<Func<string, string>> Preprocessors { get; } = new List<Func<string, string>>();

        /// <summary>
        /// Gets or sets the default script language.
        /// </summary>
        /// <value>The default script language.</value>
        public ScriptLanguage DefaultScriptLanguage { get; set; }

        /// <summary>
        /// Gets the script languages.
        /// </summary>
        /// <value>The script languages.</value>
        public List<ScriptLanguage> ScriptLanguages { get; } = new List<ScriptLanguage>();

        /// <summary>
        /// Gets the script languages array.
        /// </summary>
        /// <value>The script languages array.</value>
        internal ScriptLanguage[] ScriptLanguagesArray { get; private set; }

        /// <summary>
        /// Gets the script methods.
        /// </summary>
        /// <value>The script methods.</value>
        public List<ScriptMethods> ScriptMethods { get; } = new List<ScriptMethods>();

        /// <summary>
        /// Insert additional Methods at the start so they have priority over default Script Methods
        /// </summary>
        /// <value>The insert script methods.</value>
        public List<ScriptMethods> InsertScriptMethods { get; } = new List<ScriptMethods>();

        /// <summary>
        /// Gets the script blocks.
        /// </summary>
        /// <value>The script blocks.</value>
        public List<ScriptBlock> ScriptBlocks { get; } = new List<ScriptBlock>();

        /// <summary>
        /// Insert additional Blocks at the start so they have priority over default Script Blocks
        /// </summary>
        /// <value>The insert script blocks.</value>
        public List<ScriptBlock> InsertScriptBlocks { get; } = new List<ScriptBlock>();

        /// <summary>
        /// Gets the code pages.
        /// </summary>
        /// <value>The code pages.</value>
        public Dictionary<string, Type> CodePages { get; } = new Dictionary<string, Type>();

        /// <summary>
        /// Gets the exclude filters named.
        /// </summary>
        /// <value>The exclude filters named.</value>
        public HashSet<string> ExcludeFiltersNamed { get; } = new HashSet<string>();

        /// <summary>
        /// The script languages map
        /// </summary>
        private readonly Dictionary<string, ScriptLanguage> scriptLanguagesMap = new Dictionary<string, ScriptLanguage>();
        /// <summary>
        /// Gets the script language.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>ScriptLanguage.</returns>
        public ScriptLanguage GetScriptLanguage(string name) => scriptLanguagesMap.TryGetValue(name, out var block) ? block : null;

        /// <summary>
        /// The blocks map
        /// </summary>
        private readonly Dictionary<string, ScriptBlock> blocksMap = new Dictionary<string, ScriptBlock>();
        /// <summary>
        /// Gets the block.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>ScriptBlock.</returns>
        public ScriptBlock GetBlock(string name) => blocksMap.TryGetValue(name, out var block) ? block : null;

        /// <summary>
        /// Gets the cache.
        /// </summary>
        /// <value>The cache.</value>
        public ConcurrentDictionary<string, object> Cache { get; } = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// Gets the cache memory.
        /// </summary>
        /// <value>The cache memory.</value>
        public ConcurrentDictionary<ReadOnlyMemory<char>, object> CacheMemory { get; } = new ConcurrentDictionary<ReadOnlyMemory<char>, object>();

        /// <summary>
        /// Gets the expiring cache.
        /// </summary>
        /// <value>The expiring cache.</value>
        public ConcurrentDictionary<string, Tuple<DateTime, object>> ExpiringCache { get; } = new ConcurrentDictionary<string, Tuple<DateTime, object>>();

        /// <summary>
        /// Gets the js token cache.
        /// </summary>
        /// <value>The js token cache.</value>
        public ConcurrentDictionary<ReadOnlyMemory<char>, JsToken> JsTokenCache { get; } = new ConcurrentDictionary<ReadOnlyMemory<char>, JsToken>();

        /// <summary>
        /// Gets the assign expression cache.
        /// </summary>
        /// <value>The assign expression cache.</value>
        public ConcurrentDictionary<string, Action<ScriptScopeContext, object, object>> AssignExpressionCache { get; } = new ConcurrentDictionary<string, Action<ScriptScopeContext, object, object>>();

        /// <summary>
        /// Gets the code page invokers.
        /// </summary>
        /// <value>The code page invokers.</value>
        public ConcurrentDictionary<Type, Tuple<MethodInfo, MethodInvoker>> CodePageInvokers { get; } = new ConcurrentDictionary<Type, Tuple<MethodInfo, MethodInvoker>>();

        /// <summary>
        /// Gets the path mappings.
        /// </summary>
        /// <value>The path mappings.</value>
        public ConcurrentDictionary<string, string> PathMappings { get; } = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Gets the plugins.
        /// </summary>
        /// <value>The plugins.</value>
        public List<IScriptPlugin> Plugins { get; } = new List<IScriptPlugin>();

        /// <summary>
        /// Insert plugins at the start of Plugins so they're registered first
        /// </summary>
        /// <value>The insert plugins.</value>
        public List<IScriptPlugin> InsertPlugins { get; } = new List<IScriptPlugin>();

        /// <summary>
        /// Gets the file filter names.
        /// </summary>
        /// <value>The file filter names.</value>
        public HashSet<string> FileFilterNames { get; } = new HashSet<string> { "includeFile", "fileContents" };

        /// <summary>
        /// Available transformers that can transform context filter stream outputs
        /// </summary>
        /// <value>The filter transformers.</value>
        public Dictionary<string, Func<Stream, Task<Stream>>> FilterTransformers { get; set; } = new Dictionary<string, Func<Stream, Task<Stream>>>();

        /// <summary>
        /// Whether to check for modified pages by default when not in DebugMode
        /// </summary>
        /// <value><c>true</c> if [check for modified pages]; otherwise, <c>false</c>.</value>
        public bool CheckForModifiedPages { get; set; } = false;

        /// <summary>
        /// How long in between checking for modified pages
        /// </summary>
        /// <value>The check for modified pages after.</value>
        public TimeSpan? CheckForModifiedPagesAfter { get; set; }

        /// <summary>
        /// Existing caches and pages created prior to specified date should be invalidated
        /// </summary>
        /// <value>The invalidate caches before.</value>
        public DateTime? InvalidateCachesBefore { get; set; }

        /// <summary>
        /// Render render filter exceptions in-line where filter is located
        /// </summary>
        /// <value><c>true</c> if [render expression exceptions]; otherwise, <c>false</c>.</value>
        public bool RenderExpressionExceptions { get; set; }

        /// <summary>
        /// What argument to assign Exceptions to
        /// </summary>
        /// <value>The assign exceptions to.</value>
        public string AssignExceptionsTo { get; set; }

        /// <summary>
        /// Whether to skip executing expressions if an Exception was thrown
        /// </summary>
        /// <value><c>true</c> if [skip executing filters if error]; otherwise, <c>false</c>.</value>
        public bool SkipExecutingFiltersIfError { get; set; }

        /// <summary>
        /// Limit Max Iterations for Heavy Operations like rendering a Script Block (default 10K)
        /// </summary>
        /// <value>The maximum quota.</value>
        public int MaxQuota { get; set; } = 10000;

        /// <summary>
        /// Limit Max number for micro ops like evaluating an AST instruction (default 1M)
        /// </summary>
        /// <value>The maximum evaluations.</value>
        public long MaxEvaluations { get; set; } = 1000000;

        /// <summary>
        /// Limit Recursion Max StackDepth (default 25)
        /// </summary>
        /// <value>The maximum stack depth.</value>
        public int MaxStackDepth { get; set; } = 25;

        /// <summary>
        /// The log
        /// </summary>
        private ILog log;
        /// <summary>
        /// Gets the log.
        /// </summary>
        /// <value>The log.</value>
        public ILog Log => log ??= LogManager.GetLogger(GetType());

        /// <summary>
        /// Gets or sets the remove new line after filters named.
        /// </summary>
        /// <value>The remove new line after filters named.</value>
        public HashSet<string> RemoveNewLineAfterFiltersNamed { get; set; } = new HashSet<string>();
        /// <summary>
        /// Gets or sets the only evaluate filters when skipping page filter execution.
        /// </summary>
        /// <value>The only evaluate filters when skipping page filter execution.</value>
        public HashSet<string> OnlyEvaluateFiltersWhenSkippingPageFilterExecution { get; set; } = new HashSet<string>();

        /// <summary>
        /// Gets or sets the parse as language.
        /// </summary>
        /// <value>The parse as language.</value>
        public Dictionary<string, ScriptLanguage> ParseAsLanguage { get; set; } = new Dictionary<string, ScriptLanguage>();

        /// <summary>
        /// Gets or sets the on unhandled expression.
        /// </summary>
        /// <value>The on unhandled expression.</value>
        public Func<PageVariableFragment, ReadOnlyMemory<byte>> OnUnhandledExpression { get; set; }
        /// <summary>
        /// Gets or sets the on render exception.
        /// </summary>
        /// <value>The on render exception.</value>
        public Action<PageResult, Exception> OnRenderException { get; set; }

        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>SharpPage.</returns>
        /// <exception cref="System.IO.FileNotFoundException">Page at path was not found: '{virtualPath}'</exception>
        public SharpPage GetPage(string virtualPath)
        {
            var page = Pages.GetPage(virtualPath);
            if (page == null)
                throw new FileNotFoundException($"Page at path was not found: '{virtualPath}'");

            return page;
        }

        /// <summary>
        /// Gets the default methods.
        /// </summary>
        /// <value>The default methods.</value>
        public DefaultScripts DefaultMethods => ScriptMethods.FirstOrDefault(x => x is DefaultScripts) as DefaultScripts;
        /// <summary>
        /// Gets the protected methods.
        /// </summary>
        /// <value>The protected methods.</value>
        public ProtectedScripts ProtectedMethods => ScriptMethods.FirstOrDefault(x => x is ProtectedScripts) as ProtectedScripts;

        /// <summary>
        /// Asserts the protected methods.
        /// </summary>
        /// <returns>ProtectedScripts.</returns>
        /// <exception cref="System.NotSupportedException">ScriptContext is not configured with ProtectedScripts</exception>
        public ProtectedScripts AssertProtectedMethods() => ProtectedMethods ??
            throw new NotSupportedException("ScriptContext is not configured with ProtectedScripts");

        /// <summary>
        /// Gets the HTML methods.
        /// </summary>
        /// <value>The HTML methods.</value>
        public HtmlScripts HtmlMethods => ScriptMethods.FirstOrDefault(x => x is HtmlScripts) as HtmlScripts;

        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <param name="fromVirtualPath">From virtual path.</param>
        /// <param name="virtualPath">The virtual path.</param>
        /// <param name="page">The page.</param>
        /// <param name="codePage">The code page.</param>
        /// <exception cref="System.IO.FileNotFoundException">Page at path was not found: '{virtualPath}'</exception>
        public void GetPage(string fromVirtualPath, string virtualPath, out SharpPage page, out SharpCodePage codePage)
        {
            if (!TryGetPage(fromVirtualPath, virtualPath, out page, out codePage))
                throw new FileNotFoundException($"Page at path was not found: '{virtualPath}'");
        }

        /// <summary>
        /// Tries the get page.
        /// </summary>
        /// <param name="fromVirtualPath">From virtual path.</param>
        /// <param name="virtualPath">The virtual path.</param>
        /// <param name="page">The page.</param>
        /// <param name="codePage">The code page.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool TryGetPage(string fromVirtualPath, string virtualPath, out SharpPage page, out SharpCodePage codePage)
        {
            var pathMapKey = nameof(GetPage) + ">" + fromVirtualPath;
            var mappedPath = GetPathMapping(pathMapKey, virtualPath);
            if (mappedPath != null)
            {
                var mappedPage = Pages.GetPage(mappedPath);
                if (mappedPage != null)
                {
                    page = mappedPage;
                    codePage = null;
                    return true;
                }
                RemovePathMapping(pathMapKey, mappedPath);
            }

            var tryExactMatch = virtualPath.IndexOf('/') >= 0; //if nested path specified, look for an exact match first
            if (tryExactMatch)
            {
                var cp = GetCodePage(virtualPath);
                if (cp != null)
                {
                    codePage = cp;
                    page = null;
                    return true;
                }

                var p = Pages.GetPage(virtualPath);
                if (p != null)
                {
                    page = p;
                    codePage = null;
                    return true;
                }
            }

            //otherwise find closest match from page.VirtualPath
            var parentPath = fromVirtualPath.IndexOf('/') >= 0
                ? fromVirtualPath.LastLeftPart('/')
                : "";
            do
            {
                var seekPath = parentPath.CombineWith(virtualPath);
                var cp = GetCodePage(seekPath);
                if (cp != null)
                {
                    codePage = cp;
                    page = null;
                    return true;
                }

                var p = Pages.GetPage(seekPath);
                if (p != null)
                {
                    page = p;
                    codePage = null;
                    SetPathMapping(pathMapKey, virtualPath, seekPath);
                    return true;
                }

                if (parentPath == "")
                    break;

                parentPath = parentPath.IndexOf('/') >= 0
                    ? parentPath.LastLeftPart('/')
                    : "";

            } while (true);

            page = null;
            codePage = null;
            return false;
        }

        /// <summary>
        /// The empty page
        /// </summary>
        private SharpPage emptyPage;
        /// <summary>
        /// Gets the empty page.
        /// </summary>
        /// <value>The empty page.</value>
        public SharpPage EmptyPage => emptyPage ??= OneTimePage("");


        /// <summary>
        /// The empty file
        /// </summary>
        private static InMemoryVirtualFile emptyFile;
        /// <summary>
        /// Gets the empty file.
        /// </summary>
        /// <value>The empty file.</value>
        public InMemoryVirtualFile EmptyFile =>
            emptyFile ??= new InMemoryVirtualFile(SharpPages.TempFiles, SharpPages.TempDir)
            {
                FilePath = "empty", TextContents = ""
            };

        /// <summary>
        /// Called when [time page].
        /// </summary>
        /// <param name="contents">The contents.</param>
        /// <param name="ext">The ext.</param>
        /// <returns>SharpPage.</returns>
        public SharpPage OneTimePage(string contents, string ext = null)
            => Pages.OneTimePage(contents, ext ?? PageFormats.First().Extension);

        /// <summary>
        /// Gets the code page.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>SharpCodePage.</returns>
        public SharpCodePage GetCodePage(string virtualPath)
        {
            var sanitizePath = virtualPath.Replace('\\', '/').TrimPrefixes("/").LastLeftPart('.');

            var isIndexPage = sanitizePath == string.Empty || sanitizePath.EndsWith("/");
            var lookupPath = !isIndexPage
                ? sanitizePath
                : sanitizePath + IndexPage;

            if (!CodePages.TryGetValue(lookupPath, out Type type))
                return null;

            var instance = (SharpCodePage)Container.Resolve(type);
            instance.Init();
            return instance;
        }

        /// <summary>
        /// Sets the path mapping.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="mapPath">The map path.</param>
        /// <param name="toPath">To path.</param>
        /// <returns>System.String.</returns>
        public string SetPathMapping(string prefix, string mapPath, string toPath)
        {
            if (!DebugMode && toPath != null && mapPath != toPath)
                PathMappings[prefix + ">" + mapPath] = toPath;

            return toPath;
        }

        /// <summary>
        /// Removes the path mapping.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="mapPath">The map path.</param>
        public void RemovePathMapping(string prefix, string mapPath)
        {
            if (DebugMode)
                return;

            if (mapPath != null)
                PathMappings.TryRemove(prefix + ">" + mapPath, out _);
        }

        /// <summary>
        /// Gets the path mapping.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public string GetPathMapping(string prefix, string key)
        {
            if (DebugMode)
                return null;

            if (PathMappings.TryGetValue(prefix + ">" + key, out string mappedPath))
                return mappedPath;

            return null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptContext"/> class.
        /// </summary>
        public ScriptContext()
        {
            Pages = new SharpPages(this);
            PageFormats.Add(new HtmlPageFormat());
            ScriptMethods.Add(new DefaultScripts());
            ScriptMethods.Add(new HtmlScripts());
            Plugins.Add(new DefaultScriptBlocks());
            Plugins.Add(new HtmlScriptBlocks());
            FilterTransformers[ScriptConstants.HtmlEncode] = HtmlPageFormat.HtmlEncodeTransformer;
            FilterTransformers["end"] = stream => (TypeConstants.EmptyByteArray.InMemoryStream() as Stream).InTask();
            FilterTransformers["buffer"] = stream => stream.InTask();

            DefaultScriptLanguage = SharpScript.Language;
            ScriptLanguages.Add(ScriptTemplate.Language);
            ScriptLanguages.Add(ScriptCode.Language);

            Args[nameof(ScriptConfig.DefaultCulture)] = ScriptConfig.CreateCulture();
            Args[nameof(ScriptConfig.DefaultDateFormat)] = ScriptConfig.DefaultDateFormat;
            Args[nameof(ScriptConfig.DefaultDateTimeFormat)] = ScriptConfig.DefaultDateTimeFormat;
            Args[nameof(ScriptConfig.DefaultTimeFormat)] = ScriptConfig.DefaultTimeFormat;
            Args[nameof(ScriptConfig.DefaultFileCacheExpiry)] = ScriptConfig.DefaultFileCacheExpiry;
            Args[nameof(ScriptConfig.DefaultUrlCacheExpiry)] = ScriptConfig.DefaultUrlCacheExpiry;
            Args[nameof(ScriptConfig.DefaultIndent)] = ScriptConfig.DefaultIndent;
            Args[nameof(ScriptConfig.DefaultNewLine)] = ScriptConfig.DefaultNewLine;
            Args[nameof(ScriptConfig.DefaultJsConfig)] = ScriptConfig.DefaultJsConfig;
            Args[nameof(ScriptConfig.DefaultStringComparison)] = ScriptConfig.DefaultStringComparison;
            Args[nameof(ScriptConfig.DefaultTableClassName)] = ScriptConfig.DefaultTableClassName;
            Args[nameof(ScriptConfig.DefaultErrorClassName)] = ScriptConfig.DefaultErrorClassName;
        }

        /// <summary>
        /// Removes the filters.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>ScriptContext.</returns>
        public ScriptContext RemoveFilters(Predicate<ScriptMethods> match)
        {
            ScriptMethods.RemoveAll(match);
            return this;
        }

        /// <summary>
        /// Removes the blocks.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>ScriptContext.</returns>
        public ScriptContext RemoveBlocks(Predicate<ScriptBlock> match)
        {
            ScriptBlocks.RemoveAll(match);
            return this;
        }

        /// <summary>
        /// Removes the plugins.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>ScriptContext.</returns>
        public ScriptContext RemovePlugins(Predicate<IScriptPlugin> match)
        {
            Plugins.RemoveAll(match);
            return this;
        }

        /// <summary>
        /// Gets or sets the on after plugins.
        /// </summary>
        /// <value>The on after plugins.</value>
        public Action<ScriptContext> OnAfterPlugins { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has initialize.
        /// </summary>
        /// <value><c>true</c> if this instance has initialize; otherwise, <c>false</c>.</value>
        public bool HasInit { get; private set; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns>ScriptContext.</returns>
        public ScriptContext Init()
        {
            if (HasInit)
                return this;
            HasInit = true;

            if (InsertScriptMethods.Count > 0)
                ScriptMethods.InsertRange(0, InsertScriptMethods);
            if (InsertScriptBlocks.Count > 0)
                ScriptBlocks.InsertRange(0, InsertScriptBlocks);
            if (InsertPlugins.Count > 0)
                Plugins.InsertRange(0, InsertPlugins);

            foreach (var assembly in ScanAssemblies.Safe())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(IScriptPlugin).IsAssignableFrom(type))
                    {
                        if (Plugins.All(x => x.GetType() != type))
                        {
                            Container.AddSingleton(type);
                            var plugin = (IScriptPlugin)Container.Resolve(type);
                            Plugins.Add(plugin);
                        }
                    }
                }
            }

            Args[ScriptConstants.Debug] = DebugMode;

            Container.AddSingleton(() => this);
            Container.AddSingleton(() => Pages);

            ScriptLanguagesArray = ScriptLanguages.Distinct().ToArray();
            foreach (var scriptLanguage in ScriptLanguagesArray)
            {
                scriptLanguagesMap[scriptLanguage.Name] = scriptLanguage;

                if (scriptLanguage is IConfigureScriptContext init)
                    init.Configure(this);
            }

            var beforePlugins = Plugins.OfType<IScriptPluginBefore>();
            foreach (var plugin in beforePlugins)
            {
                plugin.BeforePluginsLoaded(this);
            }
            foreach (var plugin in Plugins)
            {
                plugin.Register(this);
            }

            OnAfterPlugins?.Invoke(this);

            foreach (var type in ScanTypes)
            {
                ScanType(type);
            }

            foreach (var assembly in ScanAssemblies.Safe())
            {
                foreach (var type in assembly.GetTypes())
                {
                    ScanType(type);
                }
            }

            foreach (var method in ScriptMethods)
            {
                InitMethod(method);
            }

            foreach (var block in ScriptBlocks)
            {
                InitBlock(block);
                blocksMap[block.Name] = block;
            }

            ScriptNamespaces = ScriptNamespaces.Distinct().ToList();

            var allTypes = new List<Type>(ScriptTypes);
            foreach (var asm in ScriptAssemblies)
            {
                allTypes.AddRange(asm.GetTypes());
            }

            foreach (var type in allTypes)
            {
                if (!ScriptTypeNameMap.ContainsKey(type.Name))
                    ScriptTypeNameMap[type.Name] = type;

                var qualifiedName = ProtectedMethods.typeQualifiedName(type);
                if (!ScriptTypeQualifiedNameMap.ContainsKey(qualifiedName))
                    ScriptTypeQualifiedNameMap[qualifiedName] = type;
            }

            var afterPlugins = Plugins.OfType<IScriptPluginAfter>();
            foreach (var plugin in afterPlugins)
            {
                plugin.AfterPluginsLoaded(this);
            }

            return this;
        }

        /// <summary>
        /// Initializes the method.
        /// </summary>
        /// <param name="method">The method.</param>
        internal void InitMethod(ScriptMethods method)
        {
            if (method == null) return;
            if (method.Context == null)
                method.Context = this;
            if (method.Pages == null)
                method.Pages = Pages;

            if (method is IConfigureScriptContext init)
                init.Configure(this);
        }

        /// <summary>
        /// Initializes the block.
        /// </summary>
        /// <param name="block">The block.</param>
        internal void InitBlock(ScriptBlock block)
        {
            if (block == null) return;
            if (block.Context == null)
                block.Context = this;
            if (block.Pages == null)
                block.Pages = Pages;

            if (block is IConfigureScriptContext init)
                init.Configure(this);
        }

        /// <summary>
        /// Scans the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>ScriptContext.</returns>
        public ScriptContext ScanType(Type type)
        {
            if (!type.IsAbstract)
            {
                if (typeof(ScriptMethods).IsAssignableFrom(type))
                {
                    if (ScriptMethods.All(x => x?.GetType() != type))
                    {
                        Container.AddSingleton(type);
                        var method = (ScriptMethods)Container.Resolve(type);
                        ScriptMethods.Add(method);
                    }
                }
                else if (typeof(ScriptBlock).IsAssignableFrom(type))
                {
                    if (ScriptBlocks.All(x => x?.GetType() != type))
                    {
                        Container.AddSingleton(type);
                        var block = (ScriptBlock)Container.Resolve(type);
                        ScriptBlocks.Add(block);
                    }
                }
                else if (typeof(SharpCodePage).IsAssignableFrom(type))
                {
                    if (CodePages.Values.All(x => x != type))
                    {
                        Container.AddTransient(type);
                        var pageAttr = type.FirstAttribute<PageAttribute>();
                        if (pageAttr?.VirtualPath != null)
                        {
                            CodePages[pageAttr.VirtualPath] = type;
                        }
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Gets the assign expression.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>Action&lt;ScriptScopeContext, System.Object, System.Object&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">targetType</exception>
        /// <exception cref="System.ArgumentNullException">expression</exception>
        public Action<ScriptScopeContext, object, object> GetAssignExpression(Type targetType, ReadOnlyMemory<char> expression)
        {
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));
            if (expression.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(expression));

            var key = targetType.FullName + ':' + expression;

            if (AssignExpressionCache.TryGetValue(key, out var fn))
                return fn;

            AssignExpressionCache[key] = fn = ScriptTemplateUtils.CompileAssign(targetType, expression);

            return fn;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            using (Container as IDisposable) { }
        }
    }

    /// <summary>
    /// Class ReturnValue.
    /// </summary>
    public class ReturnValue
    {
        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>The result.</value>
        public object Result { get; }
        /// <summary>
        /// Gets the arguments.
        /// </summary>
        /// <value>The arguments.</value>
        public Dictionary<string, object> Args { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnValue"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="args">The arguments.</param>
        public ReturnValue(object result, Dictionary<string, object> args)
        {
            Result = result;
            Args = args;
        }
    }

    /// <summary>
    /// Class ScriptContextUtils.
    /// </summary>
    public static class ScriptContextUtils
    {
        /// <summary>
        /// The error no return
        /// </summary>
        public static string ErrorNoReturn = "Script did not return a value. Use EvaluateScript() to return script output instead";

        /// <summary>
        /// Throws the no return.
        /// </summary>
        /// <exception cref="System.NotSupportedException">Script did not return a value</exception>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowNoReturn() => throw new NotSupportedException("Script did not return a value");

        /// <summary>
        /// Shoulds the rethrow.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool ShouldRethrow(Exception e) =>
            e is ScriptException;

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="pageResult">The page result.</param>
        /// <returns>Exception.</returns>
        public static Exception HandleException(Exception e, PageResult pageResult)
        {
            var underlyingEx = e.UnwrapIfSingleException();
            if (underlyingEx is StopFilterExecutionException se)
                underlyingEx = se.InnerException;
            if (underlyingEx is TargetInvocationException te)
                underlyingEx = te.InnerException;

#if DEBUG
            var logEx = underlyingEx.GetInnerMostException();
            Logging.LogManager.GetLogger(typeof(ScriptContextUtils)).Error(logEx.Message + "\n" + logEx.StackTrace, logEx);
#endif

            if (underlyingEx is ScriptException)
                return underlyingEx;

            pageResult.LastFilterError = underlyingEx;
            return new ScriptException(pageResult);
        }

        /// <summary>
        /// Evaluates the result.
        /// </summary>
        /// <param name="pageResult">The page result.</param>
        /// <param name="returnValue">The return value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="ServiceStack.Script.ScriptException"></exception>
        public static bool EvaluateResult(this PageResult pageResult, out object returnValue)
        {
            try
            {
                pageResult.WriteToAsync(Stream.Null).Wait();
                if (pageResult.LastFilterError != null)
                    throw new ScriptException(pageResult);

                returnValue = pageResult.ReturnValue?.Result;
                return pageResult.ReturnValue != null;
            }
            catch (Exception e)
            {
                if (ShouldRethrow(e))
                    throw;
                throw HandleException(e, pageResult);
            }
        }

        /// <summary>
        /// Evaluate result as an asynchronous operation.
        /// </summary>
        /// <param name="pageResult">The page result.</param>
        /// <returns>A Task&lt;Tuple`2&gt; representing the asynchronous operation.</returns>
        /// <exception cref="ServiceStack.Script.ScriptException"></exception>
        public static async Task<Tuple<bool, object>> EvaluateResultAsync(this PageResult pageResult)
        {
            try
            {
                await pageResult.WriteToAsync(Stream.Null);
                if (pageResult.LastFilterError != null)
                    throw new ScriptException(pageResult);

                return new Tuple<bool, object>(pageResult.ReturnValue != null, pageResult.ReturnValue?.Result);
            }
            catch (Exception e)
            {
                if (ShouldRethrow(e))
                    throw;
                throw HandleException(e, pageResult);
            }
        }

        /// <summary>
        /// Render as an asynchronous operation.
        /// </summary>
        /// <param name="pageResult">The page result.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <exception cref="ServiceStack.Script.ScriptException"></exception>
        public static async Task RenderAsync(this PageResult pageResult, Stream stream, CancellationToken token = default)
        {
            if (pageResult.ResultOutput != null)
            {
                await stream.WriteAsync(MemoryProvider.Instance.ToUtf8Bytes(pageResult.ResultOutput.AsSpan()), token: token);
                return;
            }

            await pageResult.Init();
            await pageResult.WriteToAsync(stream, token);
            if (pageResult.LastFilterError != null)
                throw new ScriptException(pageResult);
        }

        /// <summary>
        /// Renders to stream.
        /// </summary>
        /// <param name="pageResult">The page result.</param>
        /// <param name="stream">The stream.</param>
        /// <exception cref="ServiceStack.Script.ScriptException"></exception>
        /// <exception cref="ServiceStack.Script.ScriptException"></exception>
        public static void RenderToStream(this PageResult pageResult, Stream stream)
        {
            try
            {
                try
                {
                    if (pageResult.ResultOutput != null)
                    {
                        if (pageResult.LastFilterError != null)
                            throw new ScriptException(pageResult);

#if NET5_0_OR_GREATER
                        stream.WriteAsync(MemoryProvider.Instance.ToUtf8Bytes(pageResult.ResultOutput.AsSpan()));
#else
                        stream.WriteAsync(MemoryProvider.Instance.ToUtf8Bytes(pageResult.ResultOutput.AsSpan())).Wait();
#endif
                        return;
                    }

                    pageResult.Init().Wait();
                    pageResult.WriteToAsync(stream).Wait();
                    if (pageResult.LastFilterError != null)
                        throw new ScriptException(pageResult);
                }
                catch (AggregateException e)
                {
                    var ex = e.UnwrapIfSingleException();
                    throw ex;
                }
            }
            catch (Exception e)
            {
                if (ShouldRethrow(e))
                    throw;
                throw HandleException(e, pageResult);
            }
        }

        /// <summary>
        /// Render to stream as an asynchronous operation.
        /// </summary>
        /// <param name="pageResult">The page result.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <exception cref="ServiceStack.Script.ScriptException"></exception>
        /// <exception cref="ServiceStack.Script.ScriptException"></exception>
        public static async Task RenderToStreamAsync(this PageResult pageResult, Stream stream)
        {
            try
            {
                if (pageResult.ResultOutput != null)
                {
                    if (pageResult.LastFilterError != null)
                        throw new ScriptException(pageResult);

                    await stream.WriteAsync(MemoryProvider.Instance.ToUtf8Bytes(pageResult.ResultOutput.AsSpan()));
                    return;
                }

                await pageResult.Init();
                await pageResult.WriteToAsync(stream);
                if (pageResult.LastFilterError != null)
                    throw new ScriptException(pageResult);
            }
            catch (Exception e)
            {
                if (ShouldRethrow(e))
                    throw;
                throw HandleException(e, pageResult);
            }
        }

        /// <summary>
        /// Renders the script.
        /// </summary>
        /// <param name="pageResult">The page result.</param>
        /// <returns>System.String.</returns>
        public static string RenderScript(this PageResult pageResult)
        {
            try
            {
                using var ms = MemoryStreamFactory.GetStream();
                pageResult.RenderToStream(ms);
                var output = ms.ReadToEnd();
                return output;
            }
            catch (Exception e)
            {
                if (ShouldRethrow(e))
                    throw;
                throw HandleException(e, pageResult);
            }
        }

        /// <summary>
        /// Render script as an asynchronous operation.
        /// </summary>
        /// <param name="pageResult">The page result.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task&lt;System.String&gt; representing the asynchronous operation.</returns>
        public static async Task<string> RenderScriptAsync(this PageResult pageResult, CancellationToken token = default)
        {
            try
            {
                using var ms = MemoryStreamFactory.GetStream();
                await RenderAsync(pageResult, ms, token);
                var output = await ms.ReadToEndAsync();
                return output;
            }
            catch (Exception e)
            {
                if (ShouldRethrow(e))
                    throw;
                throw HandleException(e, pageResult);
            }
        }

        /// <summary>
        /// Creates the scope.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="functions">The functions.</param>
        /// <param name="blocks">The blocks.</param>
        /// <returns>ScriptScopeContext.</returns>
        public static ScriptScopeContext CreateScope(this ScriptContext context, Dictionary<string, object> args = null,
            ScriptMethods functions = null, ScriptBlock blocks = null)
        {
            var pageContext = new PageResult(context.EmptyPage);
            if (functions != null)
                pageContext.ScriptMethods.Insert(0, functions);
            if (blocks != null)
                pageContext.ScriptBlocks.Insert(0, blocks);

            return new ScriptScopeContext(pageContext, null, args);
        }
    }
}
