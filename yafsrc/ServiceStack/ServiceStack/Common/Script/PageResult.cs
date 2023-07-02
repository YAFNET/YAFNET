// ***********************************************************************
// <copyright file="PageResult.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using ServiceStack.Text;
using ServiceStack.Web;

namespace ServiceStack.Script;

/// <summary>
/// Interface IPageResult
/// </summary>
public interface IPageResult { }

// Render a Template Page to the Response OutputStream
/// <summary>
/// Class PageResult.
/// Implements the <see cref="ServiceStack.Script.IPageResult" />
/// Implements the <see cref="ServiceStack.Web.IStreamWriterAsync" />
/// Implements the <see cref="ServiceStack.Web.IHasOptions" />
/// Implements the <see cref="System.IDisposable" />
/// </summary>
/// <seealso cref="ServiceStack.Script.IPageResult" />
/// <seealso cref="ServiceStack.Web.IStreamWriterAsync" />
/// <seealso cref="ServiceStack.Web.IHasOptions" />
/// <seealso cref="System.IDisposable" />
public class PageResult : IPageResult, IStreamWriterAsync, IHasOptions, IDisposable
{
    /// <summary>
    /// The Page to Render
    /// </summary>
    /// <value>The page.</value>
    public SharpPage Page { get; }

    /// <summary>
    /// The Code Page to Render
    /// </summary>
    /// <value>The code page.</value>
    public SharpCodePage CodePage { get; }

    /// <summary>
    /// Use specified Layout
    /// </summary>
    /// <value>The layout page.</value>
    public SharpPage LayoutPage { get; set; }

    /// <summary>
    /// Use Layout with specified name
    /// </summary>
    /// <value>The layout.</value>
    public string Layout { get; set; }

    /// <summary>
    /// Render without any Layout
    /// </summary>
    /// <value><c>true</c> if [no layout]; otherwise, <c>false</c>.</value>
    public bool NoLayout { get; set; }

    /// <summary>
    /// Extract Model Properties into Scope Args
    /// </summary>
    /// <value>The model.</value>
    public object Model { get; set; }

    /// <summary>
    /// Add additional Args available to all pages
    /// </summary>
    /// <value>The arguments.</value>
    public Dictionary<string, object> Args { get; set; }

    /// <summary>
    /// Add additional script methods available to all pages
    /// </summary>
    /// <value>The script methods.</value>
    public List<ScriptMethods> ScriptMethods { get; set; }

    /// <summary>
    /// Add additional script blocks available to all pages
    /// </summary>
    /// <value>The script blocks.</value>
    public List<ScriptBlock> ScriptBlocks { get; set; }

    /// <summary>
    /// Add additional partials available to all pages
    /// </summary>
    /// <value>The partials.</value>
    public Dictionary<string, SharpPage> Partials { get; set; }

    /// <summary>
    /// Return additional HTTP Headers in HTTP Requests
    /// </summary>
    /// <value>The options.</value>
    public IDictionary<string, string> Options { get; set; }

    /// <summary>
    /// Specify the Content-Type of the Response
    /// </summary>
    /// <value>The type of the content.</value>
    public string ContentType
    {
        get => Options.TryGetValue(HttpHeaders.ContentType, out string contentType) ? contentType : null;
        set => Options[HttpHeaders.ContentType] = value;
    }

    /// <summary>
    /// Transform the Page output using a chain of stream transformers
    /// </summary>
    /// <value>The page transformers.</value>
    public List<Func<Stream, Task<Stream>>> PageTransformers { get; set; }

    /// <summary>
    /// Transform the entire output using a chain of stream transformers
    /// </summary>
    /// <value>The output transformers.</value>
    public List<Func<Stream, Task<Stream>>> OutputTransformers { get; set; }

    /// <summary>
    /// Available transformers that can transform context filter stream outputs
    /// </summary>
    /// <value>The filter transformers.</value>
    public Dictionary<string, Func<Stream, Task<Stream>>> FilterTransformers { get; set; }

    /// <summary>
    /// Don't allow access to specified filters
    /// </summary>
    /// <value>The exclude filters named.</value>
    public HashSet<string> ExcludeFiltersNamed { get; } = new();

    /// <summary>
    /// The last error thrown by a filter
    /// </summary>
    /// <value>The last filter error.</value>
    public Exception LastFilterError { get; set; }

    /// <summary>
    /// The StackTrace where the Last Error Occured
    /// </summary>
    /// <value>The last filter stack trace.</value>
    public string[] LastFilterStackTrace { get; set; }

    /// <summary>
    /// What argument errors should be binded to
    /// </summary>
    /// <value>The assign exceptions to.</value>
    public string AssignExceptionsTo { get; set; }

    /// <summary>
    /// What argument captured errors should be binded to
    /// </summary>
    /// <value>The catch exceptions in.</value>
    public string CatchExceptionsIn { get; set; }

    /// <summary>
    /// Whether to skip execution of all page filters and just write page string fragments
    /// </summary>
    /// <value><c>true</c> if [skip filter execution]; otherwise, <c>false</c>.</value>
    public bool SkipFilterExecution { get; set; }

    /// <summary>
    /// Overrides Context to specify whether to Ignore or Continue executing filters on error
    /// </summary>
    /// <value><c>null</c> if [skip executing filters if error] contains no value, <c>true</c> if [skip executing filters if error]; otherwise, <c>false</c>.</value>
    public bool? SkipExecutingFiltersIfError { get; set; }

    /// <summary>
    /// Whether to always rethrow Exceptions
    /// </summary>
    /// <value><c>true</c> if [rethrow exceptions]; otherwise, <c>false</c>.</value>
    public bool RethrowExceptions { get; set; }

    /// <summary>
    /// Immediately halt execution of the page
    /// </summary>
    /// <value><c>true</c> if [halt execution]; otherwise, <c>false</c>.</value>
    public bool HaltExecution { get; set; }

    /// <summary>
    /// Whether to disable buffering output and render directly to OutputStream
    /// </summary>
    /// <value><c>true</c> if [disable buffering]; otherwise, <c>false</c>.</value>
    public bool DisableBuffering { get; set; }

    /// <summary>
    /// The Return value of the page (if any)
    /// </summary>
    /// <value>The return value.</value>
    public ReturnValue ReturnValue { get; set; }

    /// <summary>
    /// The Current StackDepth
    /// </summary>
    /// <value>The stack depth.</value>
    public int StackDepth { get; internal set; }

    /// <summary>
    /// The Current StackDepth of rendering partials
    /// </summary>
    /// <value>The partial stack depth.</value>
    public int PartialStackDepth { get; internal set; }

    /// <summary>
    /// Can be used to track number of Evaluations
    /// </summary>
    /// <value>The evaluations.</value>
    public long Evaluations { get; private set; }

    /// <summary>
    /// Can be used to track number of Evaluations
    /// </summary>
    /// <value><c>true</c> if [page processed]; otherwise, <c>false</c>.</value>
    internal bool PageProcessed { get; set; }

    /// <summary>
    /// Asserts the next evaluation.
    /// </summary>
    /// <exception cref="System.NotSupportedException">Exceeded Max Evaluations of {Context.MaxEvaluations}. \nMaxEvaluations can be changed in `ScriptContext.MaxEvaluations`.</exception>
    public void AssertNextEvaluation()
    {
        if (Evaluations++ >= Context.MaxEvaluations)
            throw new NotSupportedException($"Exceeded Max Evaluations of {Context.MaxEvaluations}. \nMaxEvaluations can be changed in `ScriptContext.MaxEvaluations`.");
    }

    /// <summary>
    /// Asserts the next partial.
    /// </summary>
    /// <exception cref="System.NotSupportedException">Exceeded Max Partial StackDepth of {Context.MaxStackDepth}. \nMaxStackDepth can be changed in `ScriptContext.MaxStackDepth`.</exception>
    public void AssertNextPartial()
    {
        if (PartialStackDepth++ >= Context.MaxStackDepth)
            throw new NotSupportedException($"Exceeded Max Partial StackDepth of {Context.MaxStackDepth}. \nMaxStackDepth can be changed in `ScriptContext.MaxStackDepth`.");
    }

    /// <summary>
    /// Resets the iterations.
    /// </summary>
    public void ResetIterations() => Evaluations = 0;

    /// <summary>
    /// The stack trace
    /// </summary>
    private readonly Stack<string> stackTrace = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="PageResult" /> class.
    /// </summary>
    /// <param name="format">The format.</param>
    private PageResult(PageFormat format)
    {
        Args = new Dictionary<string, object>();
        ScriptMethods = new List<ScriptMethods>();
        ScriptBlocks = new List<ScriptBlock>();
        Partials = new Dictionary<string, SharpPage>();
        PageTransformers = new List<Func<Stream, Task<Stream>>>();
        OutputTransformers = new List<Func<Stream, Task<Stream>>>();
        FilterTransformers = new Dictionary<string, Func<Stream, Task<Stream>>>();
        Options = new Dictionary<string, string>
                      {
                          {HttpHeaders.ContentType, format?.ContentType},
                      };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PageResult" /> class.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <exception cref="System.ArgumentNullException">page</exception>
    public PageResult(SharpPage page) : this(page?.Format)
    {
        Page = page ?? throw new ArgumentNullException(nameof(page));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PageResult" /> class.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <exception cref="System.ArgumentNullException">page</exception>
    public PageResult(SharpCodePage page) : this(page?.Format)
    {
        CodePage = page ?? throw new ArgumentNullException(nameof(page));

        var hasRequest = (CodePage as IRequiresRequest)?.Request;
        if (hasRequest != null)
            Args[ScriptConstants.Request] = hasRequest;
    }

    /// <summary>
    /// Assigns the arguments.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>PageResult.</returns>
    public PageResult AssignArgs(Dictionary<string, object> args)
    {
        if (args != null)
        {
            foreach (var entry in args)
            {
                Args[entry.Key] = entry.Value;
            }
        }
        return this;
    }

    //entry point
    /// <summary>
    /// Write to as an asynchronous operation.
    /// </summary>
    /// <param name="responseStream">The response stream.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task WriteToAsync(Stream responseStream, CancellationToken token = default)
    {
        if (OutputTransformers.Count == 0)
        {
            var bufferOutput = !DisableBuffering && responseStream is not MemoryStream;
            if (bufferOutput)
            {
                using var ms = MemoryStreamFactory.GetStream();
                await WriteToAsyncInternal(ms, token).ConfigAwait();
                ms.Position = 0;
                await ms.WriteToAsync(responseStream, token).ConfigAwait();
            }
            else
            {
                await WriteToAsyncInternal(responseStream, token).ConfigAwait();
            }
            return;
        }

        //If PageResult has any OutputFilters Buffer and chain stream responses to each
        using (var ms = MemoryStreamFactory.GetStream())
        {
            stackTrace.Push("OutputTransformer");

            await WriteToAsyncInternal(ms, token).ConfigAwait();
            Stream stream = ms;

            foreach (var transformer in OutputTransformers)
            {
                stream.Position = 0;
                stream = await transformer(stream).ConfigAwait();
            }

            using (stream)
            {
                stream.Position = 0;
                await stream.WriteToAsync(responseStream, token).ConfigAwait();
            }

            stackTrace.Pop();
        }
    }

    /// <summary>
    /// Writes to asynchronous internal.
    /// </summary>
    /// <param name="outputStream">The output stream.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    internal async Task WriteToAsyncInternal(Stream outputStream, CancellationToken token)
    {
        await Init().ConfigAwait();

        if (!NoLayout)
        {
            if (LayoutPage != null)
            {
                await LayoutPage.Init().ConfigAwait();

                if (CodePage != null)
                    InitIfNewPage(CodePage);

                if (Page != null)
                    await InitIfNewPage(Page).ConfigAwait();
            }
            else
            {
                if (Page != null)
                {
                    await InitIfNewPage(Page).ConfigAwait();
                    if (Page.LayoutPage != null)
                    {
                        LayoutPage = Page.LayoutPage;
                        await LayoutPage.Init().ConfigAwait();
                    }
                }
                else if (CodePage != null)
                {
                    InitIfNewPage(CodePage);
                    if (CodePage.LayoutPage != null)
                    {
                        LayoutPage = CodePage.LayoutPage;
                        await LayoutPage.Init().ConfigAwait();
                    }
                }
            }
        }
        else
        {
            if (Page != null)
            {
                await InitIfNewPage(Page).ConfigAwait();
            }
            else if (CodePage != null)
            {
                InitIfNewPage(CodePage);
            }
        }

        token.ThrowIfCancellationRequested();

        var pageScope = CreatePageContext(null, outputStream);

        if (!NoLayout && LayoutPage != null)
        {
            // sync impl with WriteFragmentsAsync
            stackTrace.Push("Layout: " + LayoutPage.VirtualPath);

            foreach (var fragment in LayoutPage.PageFragments)
            {
                if (HaltExecution)
                    break;

                await WritePageFragmentAsync(pageScope, fragment, token).ConfigAwait();
            }

            stackTrace.Pop();
        }
        else
        {
            await WritePageAsync(Page, CodePage, pageScope, token).ConfigAwait();
        }
    }

    /// <summary>
    /// Write fragments as an asynchronous operation.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="fragments">The fragments.</param>
    /// <param name="callTrace">The call trace.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    internal async Task WriteFragmentsAsync(ScriptScopeContext scope, IEnumerable<PageFragment> fragments, string callTrace, CancellationToken token)
    {
        stackTrace.Push(callTrace);

        foreach (var fragment in fragments)
        {
            if (HaltExecution)
                return;

            await WritePageFragmentAsync(scope, fragment, token).ConfigAwait();
        }

        stackTrace.Pop();
    }

    /// <summary>
    /// Write page fragment as an asynchronous operation.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="fragment">The fragment.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task WritePageFragmentAsync(ScriptScopeContext scope, PageFragment fragment, CancellationToken token)
    {
        foreach (var scriptLanguage in Context.ScriptLanguagesArray)
        {
            if (ShouldSkipFilterExecution(fragment))
                return;

            if (await scriptLanguage.WritePageFragmentAsync(scope, fragment, token).ConfigAwait())
                break;
        }
    }

    /// <summary>
    /// Writes the statements asynchronous.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="blockStatements">The block statements.</param>
    /// <param name="callTrace">The call trace.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    public Task WriteStatementsAsync(ScriptScopeContext scope, IEnumerable<JsStatement> blockStatements, string callTrace, CancellationToken token)
    {
        try
        {
            stackTrace.Push(callTrace);
            return WriteStatementsAsync(scope, blockStatements, token);
        }
        finally
        {

            stackTrace.Pop();
        }
    }

    /// <summary>
    /// Write statements as an asynchronous operation.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="blockStatements">The block statements.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task WriteStatementsAsync(ScriptScopeContext scope, IEnumerable<JsStatement> blockStatements, CancellationToken token)
    {
        foreach (var statement in blockStatements)
        {
            foreach (var scriptLanguage in Context.ScriptLanguagesArray)
            {
                if (HaltExecution || ShouldSkipFilterExecution(statement))
                    return;

                if (await scriptLanguage.WriteStatementAsync(scope, statement, token).ConfigAwait())
                    break;
            }
        }
    }

    /// <summary>
    /// Shoulds the skip filter execution.
    /// </summary>
    /// <param name="var">The variable.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool ShouldSkipFilterExecution(PageVariableFragment var)
    {
        return HaltExecution || SkipFilterExecution && (var.Binding != null
                                                            ? !Context.OnlyEvaluateFiltersWhenSkippingPageFilterExecution.Contains(var.Binding)
                                                            : var.InitialExpression?.Name == null ||
                                                              !Context.OnlyEvaluateFiltersWhenSkippingPageFilterExecution.Contains(var.InitialExpression.Name));
    }

    /// <summary>
    /// Shoulds the skip filter execution.
    /// </summary>
    /// <param name="fragment">The fragment.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool ShouldSkipFilterExecution(PageFragment fragment) => fragment is not PageStringFragment
                                                                    && (fragment is PageVariableFragment var
                                                                            ? ShouldSkipFilterExecution(var)
                                                                            : HaltExecution || SkipFilterExecution);

    /// <summary>
    /// Shoulds the skip filter execution.
    /// </summary>
    /// <param name="statement">The statement.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool ShouldSkipFilterExecution(JsStatement statement) => HaltExecution || SkipFilterExecution;

    /// <summary>
    /// Gets the context.
    /// </summary>
    /// <value>The context.</value>
    public ScriptContext Context => Page?.Context ?? CodePage.Context;
    /// <summary>
    /// Gets the format.
    /// </summary>
    /// <value>The format.</value>
    public PageFormat Format => Page?.Format ?? CodePage.Format;
    /// <summary>
    /// Gets the virtual path.
    /// </summary>
    /// <value>The virtual path.</value>
    public string VirtualPath => Page?.VirtualPath ?? CodePage.VirtualPath;

    /// <summary>
    /// The has initialize
    /// </summary>
    private bool hasInit;
    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <returns>PageResult.</returns>
    /// <exception cref="System.NotSupportedException"></exception>
    public async Task<PageResult> Init()
    {
        if (hasInit)
            return this;

        if (!Context.HasInit)
            throw new NotSupportedException($"{Context.GetType().Name} has not been initialized. Call 'Init()' to initialize Script Context.");

        if (Model != null)
        {
            var explodeModel = Model.ToObjectDictionary();
            foreach (var entry in explodeModel)
            {
                Args[entry.Key] = entry.Value ?? JsNull.Value;
            }
        }
        Args[ScriptConstants.Model] = Model ?? JsNull.Value;

        foreach (var scriptLanguage in Context.ScriptLanguages)
        {
            if (scriptLanguage is IConfigurePageResult configurePageResult)
            {
                configurePageResult.Configure(this);
            }
        }

        foreach (var filter in ScriptMethods)
        {
            Context.InitMethod(filter);
        }

        foreach (var block in ScriptBlocks)
        {
            Context.InitBlock(block);
            blocksMap[block.Name] = block;
        }

        if (Page != null)
        {
            await Page.Init().ConfigAwait();
            InitPageArgs(Page.Args);
        }
        else
        {
            CodePage.Init();
            InitPageArgs(CodePage.Args);
        }

        if (Layout != null && !NoLayout)
        {
            LayoutPage = Page != null
                             ? Context.Pages.ResolveLayoutPage(Page, Layout)
                             : Context.Pages.ResolveLayoutPage(CodePage, Layout);
        }

        hasInit = true;

        return this;
    }

    /// <summary>
    /// Initializes the page arguments.
    /// </summary>
    /// <param name="pageArgs">The page arguments.</param>
    private void InitPageArgs(Dictionary<string, object> pageArgs)
    {
        if (pageArgs?.Count > 0)
        {
            NoLayout = pageArgs.TryGetValue("ignore", out object ignore) && "template".Equals(ignore?.ToString()) ||
                       pageArgs.TryGetValue("layout", out object layout) && "none".Equals(layout?.ToString());
        }
    }

    /// <summary>
    /// Initializes if new page.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <returns>Task.</returns>
    private Task InitIfNewPage(SharpPage page) => page != Page
                                                      ? page.Init()
                                                      : TypeConstants.EmptyTask;

    /// <summary>
    /// Initializes if new page.
    /// </summary>
    /// <param name="page">The page.</param>
    private void InitIfNewPage(SharpCodePage page)
    {
        if (page != CodePage)
            page.Init();
    }

    /// <summary>
    /// Asserts the initialize.
    /// </summary>
    /// <exception cref="System.NotSupportedException">PageResult.Init() required for this operation.</exception>
    private void AssertInit()
    {
        if (!hasInit)
            throw new NotSupportedException("PageResult.Init() required for this operation.");
    }

    /// <summary>
    /// Writes the page asynchronous.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <param name="codePage">The code page.</param>
    /// <param name="scope">The scope.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    public Task WritePageAsync(SharpPage page, SharpCodePage codePage,
                               ScriptScopeContext scope, CancellationToken token = default)
    {
        try
        {
            AssertNextPartial();

            if (page != null)
                return WritePageAsync(page, scope, token);

            return WriteCodePageAsync(codePage, scope, token);
        }
        finally
        {
            PartialStackDepth--;
        }
    }

    /// <summary>
    /// Write page as an asynchronous operation.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <param name="scope">The scope.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task WritePageAsync(SharpPage page, ScriptScopeContext scope, CancellationToken token = default)
    {
        if (PageTransformers.Count == 0)
        {
            await WritePageAsyncInternal(page, scope, token).ConfigAwait();
            return;
        }

        //If PageResult has any PageFilters Buffer and chain stream responses to each
        using var ms = MemoryStreamFactory.GetStream();
        stackTrace.Push("PageTransformer");

        await WritePageAsyncInternal(page, new ScriptScopeContext(this, ms, scope.ScopedParams), token).ConfigAwait();
        Stream stream = ms;

        foreach (var transformer in PageTransformers)
        {
            stream.Position = 0;
            stream = await transformer(stream).ConfigAwait();
        }

        using (stream)
        {
            stream.Position = 0;
            await stream.WriteToAsync(scope.OutputStream, token).ConfigAwait();
        }

        stackTrace.Pop();
    }

    /// <summary>
    /// Writes the page asynchronous internal.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <param name="scope">The scope.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    internal async Task WritePageAsyncInternal(SharpPage page, ScriptScopeContext scope, CancellationToken token = default)
    {
        await page.Init().ConfigAwait(); //reload modified changes if needed

        await WriteFragmentsAsync(scope, page.PageFragments, "Page: " + page.VirtualPath, token).ConfigAwait();
    }

    /// <summary>
    /// Write code page as an asynchronous operation.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <param name="scope">The scope.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task WriteCodePageAsync(SharpCodePage page, ScriptScopeContext scope, CancellationToken token = default)
    {
        if (PageTransformers.Count == 0)
        {
            await WriteCodePageAsyncInternal(page, scope, token).ConfigAwait();
            return;
        }

        //If PageResult has any PageFilters Buffer and chain stream responses to each
        using var ms = MemoryStreamFactory.GetStream();
        await WriteCodePageAsyncInternal(page, new ScriptScopeContext(this, ms, scope.ScopedParams), token).ConfigAwait();
        Stream stream = ms;

        foreach (var transformer in PageTransformers)
        {
            stream.Position = 0;
            stream = await transformer(stream).ConfigAwait();
        }

        using (stream)
        {
            stream.Position = 0;
            await stream.WriteToAsync(scope.OutputStream, token).ConfigAwait();
        }
    }

    /// <summary>
    /// Writes the code page asynchronous internal.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <param name="scope">The scope.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    internal Task WriteCodePageAsyncInternal(SharpCodePage page, ScriptScopeContext scope, CancellationToken token = default)
    {
        page.Scope = scope;

        if (!page.HasInit)
            page.Init();

        return page.WriteAsync(scope);
    }

    /// <summary>
    /// To the debug string.
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <returns>System.String.</returns>
    private string toDebugString(object instance)
    {
        using (JsConfig.With(new Config
                                 {
                                     ExcludeTypeInfo = true,
                                     IncludeTypeInfo = false,
                                 }))
        {
            if (instance is Dictionary<string, object> d)
                return d.ToJsv();
            if (instance is List<object> l)
                return l.ToJsv();
            if (instance is string s)
                return '"' + s.Replace("\"", "\\\"") + '"';
            return instance.ToJsv();
        }
    }

    /// <summary>
    /// Write variable as an asynchronous operation.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="var">The variable.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task WriteVarAsync(ScriptScopeContext scope, PageVariableFragment var, CancellationToken token)
    {
        if (var.Binding != null)
            stackTrace.Push($"Expression (binding): {var.Binding}");
        else if (var.InitialExpression?.Name != null)
            stackTrace.Push($"Expression (filter): {var.InitialExpression.Name}");
        else if (var.InitialValue != null)
            stackTrace.Push(
                $"Expression ({var.InitialValue.GetType().Name}): {toDebugString(var.InitialValue).SubstringWithEllipsis(0, 200)}");
        else
            stackTrace.Push(
                $"{var.Expression.GetType().Name}: {var.Expression.ToRawString().SubstringWithEllipsis(0, 200)}");

        var value = await EvaluateAsync(var, scope, token).ConfigAwait();
        if (value != IgnoreResult.Value)
        {
            if (value != null)
            {
                var bytes = Format.EncodeValue(value).ToUtf8Bytes();
                await scope.OutputStream.WriteAsync(bytes, token).ConfigAwait();
            }
            else
            {
                if (Context.OnUnhandledExpression != null)
                {
                    var bytes = Context.OnUnhandledExpression(var);
                    if (bytes.Length > 0)
                        await scope.OutputStream.WriteAsync(bytes, token).ConfigAwait();
                }
            }
        }

        stackTrace.Pop();
    }

    /// <summary>
    /// Gets the filter transformer.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>Func&lt;Stream, Task&lt;Stream&gt;&gt;.</returns>
    private Func<Stream, Task<Stream>> GetFilterTransformer(string name)
    {
        return FilterTransformers.TryGetValue(name, out Func<Stream, Task<Stream>> fn)
                   ? fn
                   : Context.FilterTransformers.TryGetValue(name, out fn)
                       ? fn
                       : null;
    }

    /// <summary>
    /// Gets the page parameters.
    /// </summary>
    /// <param name="var">The variable.</param>
    /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
    private static Dictionary<string, object> GetPageParams(PageVariableFragment var)
    {
        Dictionary<string, object> scopedParams = null;
        if (var != null && var.FilterExpressions.Length > 0)
        {
            if (var.FilterExpressions[0].Arguments.Length > 0)
            {
                var token = var.FilterExpressions[0].Arguments[0];
                scopedParams = token.Evaluate(JS.CreateScope()) as Dictionary<string, object>;
            }
        }
        return scopedParams;
    }

    /// <summary>
    /// Creates the page context.
    /// </summary>
    /// <param name="var">The variable.</param>
    /// <param name="outputStream">The output stream.</param>
    /// <returns>ScriptScopeContext.</returns>
    private ScriptScopeContext CreatePageContext(PageVariableFragment var, Stream outputStream) => new(this, outputStream, GetPageParams(var));

    /// <summary>
    /// Evaluate as an asynchronous operation.
    /// </summary>
    /// <param name="var">The variable.</param>
    /// <param name="scope">The scope.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.Object&gt; representing the asynchronous operation.</returns>
    /// <exception cref="System.NotSupportedException"></exception>
    /// <exception cref="System.NotSupportedException">Could not find FilterTransformer '{var.FilterExpressions[exprIndex].Name}' in page '{Page.VirtualPath}'</exception>
    /// <exception cref="System.Reflection.TargetInvocationException">Failed to invoke script method '{expr.GetDisplayName()}': {ex.Message}</exception>
    /// <exception cref="System.Reflection.TargetInvocationException">Failed to invoke script method '{filterName}': {useEx.Message}</exception>
    private async Task<object> EvaluateAsync(PageVariableFragment var, ScriptScopeContext scope, CancellationToken token = default)
    {
        scope.ScopedParams[nameof(PageVariableFragment)] = var;

        var value = var.Evaluate(scope);
        if (value == null)
        {
            var handlesUnknownValue = Context.OnUnhandledExpression == null &&
                                      var.FilterExpressions.Length > 0;

            if (!handlesUnknownValue)
            {
                if (var.Expression is JsMemberExpression memberExpr)
                {
                    //allow nested null bindings from an existing target to evaluate to an empty string
                    var targetValue = memberExpr.Object.Evaluate(scope);
                    if (targetValue != null)
                        return string.Empty;
                }

                if (var.Binding == null)
                    return null;

                var hasFilterAsBinding = GetFilterAsBinding(var.Binding, out ScriptMethods filter);
                if (hasFilterAsBinding != null)
                {
                    value = InvokeFilter(hasFilterAsBinding, filter, Array.Empty<object>(), var.Binding);
                }
                else
                {
                    var hasContextFilterAsBinding = GetContextFilterAsBinding(var.Binding, out filter);
                    if (hasContextFilterAsBinding != null)
                    {
                        value = InvokeFilter(hasContextFilterAsBinding, filter, new object[] { scope }, var.Binding);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        if (value == JsNull.Value)
            value = null;

        value = EvaluateIfToken(value, scope);

        for (var i = 0; i < var.FilterExpressions.Length; i++)
        {
            if (HaltExecution || value == StopExecution.Value)
                break;

            var expr = var.FilterExpressions[i];

            try
            {
                var filterName = expr.Name;

                var fnArgValues = JsCallExpression.EvaluateArgumentValues(scope, expr.Arguments);
                var fnArgsLength = fnArgValues.Count;

                var invoker = GetFilterInvoker(filterName, 1 + fnArgsLength, out ScriptMethods filter);
                var contextFilterInvoker = invoker == null
                                               ? GetContextFilterInvoker(filterName, 2 + fnArgsLength, out filter)
                                               : null;
                var contextBlockInvoker = invoker == null && contextFilterInvoker == null
                                              ? GetContextBlockInvoker(filterName, 2 + fnArgsLength, out filter)
                                              : null;
                var delegateInvoker = invoker == null && contextFilterInvoker == null && contextBlockInvoker == null
                                          ? GetValue(filterName, scope) as Delegate
                                          : null;

                if (invoker == null && contextFilterInvoker == null && contextBlockInvoker == null && delegateInvoker == null)
                {
                    if (i == 0)
                        return null; // ignore on server (i.e. assume it's on client) if first filter is missing

                    var errorMsg = CreateMissingFilterErrorMessage(filterName);
                    throw new NotSupportedException(errorMsg);
                }

                if (value is Task<object> valueObjectTask)
                    value = await valueObjectTask.ConfigAwait();

                if (delegateInvoker != null)
                {
                    value = JsCallExpression.InvokeDelegate(delegateInvoker, value, true, fnArgValues);
                }
                else if (invoker != null)
                {
                    fnArgValues.Insert(0, value);
                    var args = fnArgValues.ToArray();

                    value = InvokeFilter(invoker, filter, args, expr.Name);
                }
                else if (contextFilterInvoker != null)
                {
                    fnArgValues.Insert(0, scope);
                    fnArgValues.Insert(1, value); // filter target
                    var args = fnArgValues.ToArray();

                    value = InvokeFilter(contextFilterInvoker, filter, args, expr.Name);
                }
                else
                {
                    var hasFilterTransformers = var.FilterExpressions.Length + i > 1;

                    var useScope = hasFilterTransformers
                                       ? scope.ScopeWithStream(MemoryStreamFactory.GetStream())
                                       : scope;

                    fnArgValues.Insert(0, useScope);
                    fnArgValues.Insert(1, value); // filter target
                    var args = fnArgValues.ToArray();

                    try
                    {
                        var taskResponse = (Task)contextBlockInvoker(filter, args);
                        await taskResponse.ConfigAwait();

                        if (hasFilterTransformers)
                        {
                            using (useScope.OutputStream)
                            {
                                var stream = useScope.OutputStream;

                                //If Context Filter has any Filter Transformers Buffer and chain stream responses to each
                                for (var exprIndex = i + 1; exprIndex < var.FilterExpressions.Length; exprIndex++)
                                {
                                    stream.Position = 0;

                                    contextBlockInvoker = GetContextBlockInvoker(var.FilterExpressions[exprIndex].Name, 1 + var.FilterExpressions[exprIndex].Arguments.Length, out filter);
                                    if (contextBlockInvoker != null)
                                    {
                                        args[0] = useScope;
                                        for (var cmdIndex = 0; cmdIndex < var.FilterExpressions[exprIndex].Arguments.Length; cmdIndex++)
                                        {
                                            var arg = var.FilterExpressions[exprIndex].Arguments[cmdIndex];
                                            var varValue = arg.Evaluate(scope);
                                            args[1 + cmdIndex] = varValue;
                                        }

                                        await ((Task)contextBlockInvoker(filter, args)).ConfigAwait();
                                    }
                                    else
                                    {
                                        var transformer = GetFilterTransformer(var.FilterExpressions[exprIndex].Name);
                                        if (transformer == null)
                                            throw new NotSupportedException($"Could not find FilterTransformer '{var.FilterExpressions[exprIndex].Name}' in page '{Page.VirtualPath}'");

                                        stream = await transformer(stream).ConfigAwait();
                                        useScope = useScope.ScopeWithStream(stream);
                                    }
                                }

                                if (stream.CanRead)
                                {
                                    stream.Position = 0;
                                    await stream.WriteToAsync(scope.OutputStream, token).ConfigAwait();
                                }
                            }
                        }
                    }
                    catch (StopFilterExecutionException) { throw; }
                    catch (Exception ex)
                    {
                        var rethrow = ScriptConfig.FatalExceptions.Contains(ex.GetType());

                        var exResult = Format.OnExpressionException(this, ex);
                        if (exResult != null)
                            await scope.OutputStream.WriteAsync(Format.EncodeValue(exResult).ToUtf8Bytes(), token).ConfigAwait();
                        else if (rethrow)
                            throw;

                        throw new TargetInvocationException($"Failed to invoke script method '{expr.GetDisplayName()}': {ex.Message}", ex);
                    }

                    return IgnoreResult.Value;
                }

                if (value is Task<object> valueTask)
                    value = await valueTask.ConfigAwait();
            }
            catch (Exception ex)
            {
                var stopEx = ex as StopFilterExecutionException;
                var useEx = stopEx?.InnerException ?? ex;

                LastFilterError = useEx;
                LastFilterStackTrace = stackTrace.ToArray();

                Context.OnRenderException?.Invoke(this, ex);

                if (RethrowExceptions)
                    throw useEx;

                var skipExecutingFilters = SkipExecutingFiltersIfError.GetValueOrDefault(Context.SkipExecutingFiltersIfError);
                if (skipExecutingFilters)
                    this.SkipFilterExecution = true;

                var rethrow = ScriptConfig.FatalExceptions.Contains(useEx.GetType());
                if (!rethrow)
                {
                    string errorBinding = null;

                    if (stopEx?.Options is Dictionary<string, object> filterParams)
                    {
                        if (filterParams.TryGetValue(ScriptConstants.AssignError, out object assignError))
                        {
                            errorBinding = assignError as string;
                        }
                        else if (filterParams.TryGetValue(ScriptConstants.CatchError, out object catchError))
                        {
                            errorBinding = catchError as string;
                            ResetError();
                        }
                        if (filterParams.TryGetValue(ScriptConstants.IfErrorReturn, out object ifErrorReturn))
                        {
                            ResetError();
                            return ifErrorReturn;
                        }
                    }

                    errorBinding ??= AssignExceptionsTo ?? CatchExceptionsIn ?? Context.AssignExceptionsTo;

                    if (!string.IsNullOrEmpty(errorBinding))
                    {
                        if (CatchExceptionsIn != null)
                            ResetError();

                        scope.ScopedParams[errorBinding] = useEx;
                        scope.ScopedParams[errorBinding + "StackTrace"] = stackTrace.Map(x => "   at " + x).Join(Environment.NewLine);
                        return string.Empty;
                    }
                }

                //continueExecutingFiltersOnError == false / skipExecutingFiltersOnError == true
                if (SkipExecutingFiltersIfError.HasValue || Context.SkipExecutingFiltersIfError)
                    return string.Empty;

                // rethrow exceptions which aren't handled
                var exResult = Format.OnExpressionException(this, ex);
                if (exResult != null)
                    await scope.OutputStream.WriteAsync(Format.EncodeValue(exResult).ToUtf8Bytes(), token);
                else if (rethrow || useEx is TargetInvocationException)
                    throw useEx;

                var filterName = expr.GetDisplayName();
                if (filterName.StartsWith("throw"))
                    throw useEx;

                throw new TargetInvocationException($"Failed to invoke script method '{filterName}': {useEx.Message}", useEx);
            }
        }

        return UnwrapValue(value);
    }

    /// <summary>
    /// Resets the error.
    /// </summary>
    private void ResetError()
    {
        SkipFilterExecution = false;
        LastFilterError = null;
        LastFilterStackTrace = null;
    }

    /// <summary>
    /// Unwraps the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    private static object UnwrapValue(object value)
    {
        if (value == null || value == JsNull.Value || value == StopExecution.Value)
            return string.Empty; // treat as empty value if evaluated to null

        return value;
    }

    /// <summary>
    /// Creates the missing filter error message.
    /// </summary>
    /// <param name="filterName">Name of the filter.</param>
    /// <returns>System.String.</returns>
    internal string CreateMissingFilterErrorMessage(string filterName)
    {
        var registeredMethods = ScriptMethods.Union(Context.ScriptMethods).ToList();
        var similarNonMatchingFilters = registeredMethods
            .SelectMany(x => x.QueryFilters(filterName))
            .Where(x => !(Context.ExcludeFiltersNamed.Contains(x.Name) || ExcludeFiltersNamed.Contains(x.Name)))
            .ToList();

        var sb = StringBuilderCache.Allocate()
            .AppendLine($"Filter in '{VirtualPath}' named '{filterName}' was not found.");

        if (similarNonMatchingFilters.Count > 0)
        {
            sb.Append("Check for correct usage in similar (but non-matching) filters:").AppendLine();
            var normalFilters = similarNonMatchingFilters
                .OrderBy(x => x.GetParameters().Length + (x.ReturnType == typeof(Task) ? 10 : 1))
                .ToArray();

            foreach (var mi in normalFilters)
            {
                var argsTypesWithoutContext = mi.GetParameters()
                    .Where(x => x.ParameterType != typeof(ScriptScopeContext))
                    .ToList();

                sb.Append("{{ ");

                if (argsTypesWithoutContext.Count == 0)
                {
                    sb.Append($"{mi.Name} => {mi.ReturnType.Name}");
                }
                else
                {
                    sb.Append($"{argsTypesWithoutContext[0].ParameterType.Name} |> {mi.Name}(");
                    var piCount = 0;
                    foreach (var pi in argsTypesWithoutContext.Skip(1))
                    {
                        if (piCount++ > 0)
                            sb.Append(", ");

                        sb.Append(pi.ParameterType.Name);
                    }

                    var returnType = mi.ReturnType == typeof(Task)
                                         ? "(Stream)"
                                         : mi.ReturnType.Name;

                    sb.Append($") => {returnType}");
                }

                sb.AppendLine(" }}");
            }
        }
        else
        {
            var registeredFilterNames = registeredMethods.Map(x => $"'{x.GetType().Name}'").Join(", ");
            sb.Append($"No similar filters named '{filterName}' were found in registered filter(s): {registeredFilterNames}.");
        }

        return StringBuilderCache.ReturnAndFree(sb);
    }

    // Filters with no args can be used in-place of bindings
    /// <summary>
    /// Gets the filter as binding.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="filter">The filter.</param>
    /// <returns>MethodInvoker.</returns>
    private MethodInvoker GetFilterAsBinding(string name, out ScriptMethods filter) => GetFilterInvoker(name, 0, out filter);
    /// <summary>
    /// Gets the context filter as binding.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="filter">The filter.</param>
    /// <returns>MethodInvoker.</returns>
    private MethodInvoker GetContextFilterAsBinding(string name, out ScriptMethods filter) => GetContextFilterInvoker(name, 1, out filter);

    /// <summary>
    /// Invokes the filter.
    /// </summary>
    /// <param name="invoker">The invoker.</param>
    /// <param name="filter">The filter.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="binding">The binding.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="System.NotSupportedException"></exception>
    /// <exception cref="System.Reflection.TargetInvocationException">Failed to invoke script method '{binding}': {ex.Message}</exception>
    internal object InvokeFilter(MethodInvoker invoker, ScriptMethods filter, object[] args, string binding)
    {
        if (invoker == null)
            throw new NotSupportedException(CreateMissingFilterErrorMessage(binding.LeftPart('(')));

        try
        {
            return invoker(filter, args);
        }
        catch (StopFilterExecutionException) { throw; }
        catch (Exception ex)
        {
            var exResult = Format.OnExpressionException(this, ex);
            if (exResult != null)
                return exResult;

            if (binding.StartsWith("throw"))
                throw;

            throw new TargetInvocationException($"Failed to invoke script method '{binding}': {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Parses the js expression.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="literal">The literal.</param>
    /// <param name="token">The token.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    /// <exception cref="ServiceStack.DiagnosticEvent.Exception">Invalid literal: {literal.ToString()} in '{var.OriginalText}'</exception>
    /// <exception cref="System.Exception">Invalid literal: {literal.ToString()} in '{var.OriginalText}'</exception>
    public ReadOnlySpan<char> ParseJsExpression(ScriptScopeContext scope, ReadOnlySpan<char> literal, out JsToken token)
    {
        try
        {
            return literal.ParseJsExpression(out token);
        }
        catch (ArgumentException e)
        {
            if (scope.ScopedParams.TryGetValue(nameof(PageVariableFragment), out var oVar)
                && oVar is PageVariableFragment var && !var.OriginalText.IsNullOrEmpty())
            {
                throw new Exception($"Invalid literal: {literal.ToString()} in '{var.OriginalText}'", e);
            }

            throw;
        }
    }

    /// <summary>
    /// The blocks map
    /// </summary>
    private readonly Dictionary<string, ScriptBlock> blocksMap = new();

    /// <summary>
    /// Tries the get block.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>ScriptBlock.</returns>
    public ScriptBlock TryGetBlock(string name) => blocksMap.TryGetValue(name, out var block) ? block : Context.GetBlock(name);
    /// <summary>
    /// Gets the block.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>ScriptBlock.</returns>
    /// <exception cref="System.NotSupportedException">Block in '{VirtualPath}' named '{name}' was not found.</exception>
    public ScriptBlock GetBlock(string name)
    {
        var block = TryGetBlock(name);
        if (block == null)
            throw new NotSupportedException($"Block in '{VirtualPath}' named '{name}' was not found.");

        return block;
    }

    /// <summary>
    /// Creates the scope.
    /// </summary>
    /// <param name="outputStream">The output stream.</param>
    /// <returns>ScriptScopeContext.</returns>
    public ScriptScopeContext CreateScope(Stream outputStream = null) =>
        new(this, outputStream ?? MemoryStreamFactory.GetStream(), null);

    /// <summary>
    /// Gets the filter invoker.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="argsCount">The arguments count.</param>
    /// <param name="filter">The filter.</param>
    /// <returns>MethodInvoker.</returns>
    internal MethodInvoker GetFilterInvoker(string name, int argsCount, out ScriptMethods filter) => GetInvoker(name, argsCount, InvokerType.Filter, out filter);
    /// <summary>
    /// Gets the context filter invoker.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="argsCount">The arguments count.</param>
    /// <param name="filter">The filter.</param>
    /// <returns>MethodInvoker.</returns>
    internal MethodInvoker GetContextFilterInvoker(string name, int argsCount, out ScriptMethods filter) => GetInvoker(name, argsCount, InvokerType.ContextFilter, out filter);
    /// <summary>
    /// Gets the context block invoker.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="argsCount">The arguments count.</param>
    /// <param name="filter">The filter.</param>
    /// <returns>MethodInvoker.</returns>
    internal MethodInvoker GetContextBlockInvoker(string name, int argsCount, out ScriptMethods filter) => GetInvoker(name, argsCount, InvokerType.ContextBlock, out filter);

    /// <summary>
    /// Gets the invoker.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="argsCount">The arguments count.</param>
    /// <param name="invokerType">Type of the invoker.</param>
    /// <param name="filter">The filter.</param>
    /// <returns>MethodInvoker.</returns>
    private MethodInvoker GetInvoker(string name, int argsCount, InvokerType invokerType, out ScriptMethods filter)
    {
        if (!Context.ExcludeFiltersNamed.Contains(name) && !ExcludeFiltersNamed.Contains(name))
        {
            foreach (var tplFilter in ScriptMethods)
            {
                var invoker = tplFilter?.GetInvoker(name, argsCount, invokerType);
                if (invoker != null)
                {
                    filter = tplFilter;
                    return invoker;
                }
            }

            foreach (var tplFilter in Context.ScriptMethods)
            {
                var invoker = tplFilter?.GetInvoker(name, argsCount, invokerType);
                if (invoker != null)
                {
                    filter = tplFilter;
                    return invoker;
                }
            }
        }

        filter = null;
        return null;
    }

    /// <summary>
    /// Evaluates if token.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="scope">The scope.</param>
    /// <returns>System.Object.</returns>
    public object EvaluateIfToken(object value, ScriptScopeContext scope)
    {
        if (value is JsToken token)
            return token.Evaluate(scope);

        return value;
    }

    /// <summary>
    /// Tries the get value.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="scope">The scope.</param>
    /// <param name="argsOnly">if set to <c>true</c> [arguments only].</param>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="System.ArgumentNullException">name</exception>
    internal bool TryGetValue(string name, ScriptScopeContext scope, bool argsOnly, out object value)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));

        MethodInvoker invoker;
        const bool ret = true;

        value = scope.ScopedParams != null && scope.ScopedParams.TryGetValue(name, out object obj)
                    ? obj
                    : name == ScriptConstants.Global
                        ? Args
                        : Args.TryGetValue(name, out obj)
                            ? obj
                            : Page != null && Page.Args.TryGetValue(name, out obj)
                                ? obj
                                : CodePage != null && CodePage.Args.TryGetValue(name, out obj)
                                    ? obj
                                    : LayoutPage != null && LayoutPage.Args.TryGetValue(name, out obj)
                                        ? obj
                                        : Context.Args.TryGetValue(name, out obj)
                                            ? obj
                                            : argsOnly
                                                ? null
                                                : (invoker = GetFilterAsBinding(name, out var filter)) != null
                                                    ? InvokeFilter(invoker, filter, Array.Empty<object>(), name)
                                                    : (invoker = GetContextFilterAsBinding(name, out filter)) != null
                                                        ? InvokeFilter(invoker, filter, new object[] { scope }, name)
                                                        : null;
        return ret;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="scope">The scope.</param>
    /// <returns>System.Object.</returns>
    internal object GetValue(string name, ScriptScopeContext scope)
    {
        TryGetValue(name, scope, argsOnly: false, out var value);
        return value;
    }

    /// <summary>
    /// Gets the argument.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="scope">The scope.</param>
    /// <returns>System.Object.</returns>
    internal object GetArgument(string name, ScriptScopeContext scope)
    {
        TryGetValue(name, scope, argsOnly: true, out var value);
        return value;
    }

    /// <summary>
    /// Gets the result output.
    /// </summary>
    /// <value>The result output.</value>
    public string ResultOutput => resultOutput;

    /// <summary>
    /// The result output
    /// </summary>
    private string resultOutput;
    /// <summary>
    /// Gets the result.
    /// </summary>
    /// <value>The result.</value>
    public string Result
    {
        get
        {
            try
            {
                if (resultOutput != null)
                    return resultOutput;

                Init().Wait();
                resultOutput = this.RenderToStringAsync().Result;
                return resultOutput;
            }
            catch (AggregateException e)
            {
                var ex = e.UnwrapIfSingleException();
                throw ex;
            }
        }
    }

    /// <summary>
    /// Executes this instance.
    /// </summary>
    /// <returns>PageResult.</returns>
    public PageResult Execute()
    {
        var render = Result;
        return this;
    }

    /// <summary>
    /// Clones the specified page.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <returns>PageResult.</returns>
    public PageResult Clone(SharpPage page)
    {
        return new PageResult(page)
                   {
                       Args = Args,
                       ScriptMethods = ScriptMethods,
                       ScriptBlocks = ScriptBlocks,
                       FilterTransformers = FilterTransformers,
                   };
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        CodePage?.Dispose();
    }
}

/// <summary>
/// Class BindingExpressionException.
/// Implements the <see cref="System.Exception" />
/// </summary>
/// <seealso cref="System.Exception" />
public class BindingExpressionException : Exception
{
    /// <summary>
    /// Gets the expression.
    /// </summary>
    /// <value>The expression.</value>
    public string Expression { get; }
    /// <summary>
    /// Gets the member.
    /// </summary>
    /// <value>The member.</value>
    public string Member { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BindingExpressionException" /> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="member">The member.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="inner">The inner.</param>
    public BindingExpressionException(string message, string member, string expression, Exception inner = null)
        : base(message, inner)
    {
        Expression = expression;
        Member = member;
    }
}

/// <summary>
/// Class SyntaxErrorException.
/// Implements the <see cref="System.ArgumentException" />
/// </summary>
/// <seealso cref="System.ArgumentException" />
public class SyntaxErrorException : ArgumentException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SyntaxErrorException" /> class.
    /// </summary>
    public SyntaxErrorException() { }
    /// <summary>
    /// Initializes a new instance of the <see cref="SyntaxErrorException" /> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public SyntaxErrorException(string message) : base(message) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="SyntaxErrorException" /> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not a null reference, the current exception is raised in a <see langword="catch" /> block that handles the inner exception.</param>
    public SyntaxErrorException(string message, Exception innerException) : base(message, innerException) { }
}