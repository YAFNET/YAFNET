// ***********************************************************************
// <copyright file="ScriptScopeContext.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace ServiceStack.Script;

/// <summary>
/// Struct ScriptScopeContext
/// </summary>
public struct ScriptScopeContext
{
    /// <summary>
    /// Gets the page result.
    /// </summary>
    /// <value>The page result.</value>
    public PageResult PageResult { get; }
    /// <summary>
    /// Gets the page.
    /// </summary>
    /// <value>The page.</value>
    public SharpPage Page => PageResult.Page;
    /// <summary>
    /// Gets the code page.
    /// </summary>
    /// <value>The code page.</value>
    public SharpCodePage CodePage => PageResult.CodePage;
    /// <summary>
    /// Gets the context.
    /// </summary>
    /// <value>The context.</value>
    public ScriptContext Context => PageResult.Context;
    /// <summary>
    /// Gets the scoped parameters.
    /// </summary>
    /// <value>The scoped parameters.</value>
    public Dictionary<string, object> ScopedParams { get; internal set; }
    /// <summary>
    /// Gets the output stream.
    /// </summary>
    /// <value>The output stream.</value>
    public Stream OutputStream { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptScopeContext"/> struct.
    /// </summary>
    /// <param name="pageResult">The page result.</param>
    /// <param name="outputStream">The output stream.</param>
    /// <param name="scopedParams">The scoped parameters.</param>
    public ScriptScopeContext(PageResult pageResult, Stream outputStream, Dictionary<string, object> scopedParams)
    {
        PageResult = pageResult;
        ScopedParams = scopedParams ?? new Dictionary<string, object>();
        OutputStream = outputStream;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptScopeContext"/> struct.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="scopedParams">The scoped parameters.</param>
    public ScriptScopeContext(ScriptContext context, Dictionary<string, object> scopedParams)
    {
        PageResult = new PageResult(context.EmptyPage);
        OutputStream = null;
        ScopedParams = scopedParams;
    }

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>ScriptScopeContext.</returns>
    public ScriptScopeContext Clone()
    {
        return new ScriptScopeContext(PageResult, OutputStream, new Dictionary<string, object>(ScopedParams));
    }
}

/// <summary>
/// Class ScopeVars.
/// Implements the <see cref="object" />
/// </summary>
/// <seealso cref="object" />
public class ScopeVars : Dictionary<string, object>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScopeVars"/> class.
    /// </summary>
    public ScopeVars() { }
    /// <summary>
    /// Initializes a new instance of the <see cref="ScopeVars"/> class.
    /// </summary>
    /// <param name="dictionary">The <see cref="T:System.Collections.Generic.IDictionary`2" /> whose elements are copied to the new <see cref="T:System.Collections.Generic.Dictionary`2" />.</param>
    public ScopeVars(IDictionary<string, object> dictionary) : base(dictionary) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="ScopeVars"/> class.
    /// </summary>
    /// <param name="dictionary">The <see cref="T:System.Collections.Generic.IDictionary`2" /> whose elements are copied to the new <see cref="T:System.Collections.Generic.Dictionary`2" />.</param>
    /// <param name="comparer">The <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> implementation to use when comparing keys, or <see langword="null" /> to use the default <see cref="T:System.Collections.Generic.EqualityComparer`1" /> for the type of the key.</param>
    public ScopeVars(IDictionary<string, object> dictionary, IEqualityComparer<string> comparer) : base(dictionary, comparer) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="ScopeVars"/> class.
    /// </summary>
    /// <param name="comparer">The <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> implementation to use when comparing keys, or <see langword="null" /> to use the default <see cref="T:System.Collections.Generic.EqualityComparer`1" /> for the type of the key.</param>
    public ScopeVars(IEqualityComparer<string> comparer) : base(comparer) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="ScopeVars"/> class.
    /// </summary>
    /// <param name="capacity">The initial number of elements that the <see cref="T:System.Collections.Generic.Dictionary`2" /> can contain.</param>
    public ScopeVars(int capacity) : base(capacity) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="ScopeVars"/> class.
    /// </summary>
    /// <param name="capacity">The initial number of elements that the <see cref="T:System.Collections.Generic.Dictionary`2" /> can contain.</param>
    /// <param name="comparer">The <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> implementation to use when comparing keys, or <see langword="null" /> to use the default <see cref="T:System.Collections.Generic.EqualityComparer`1" /> for the type of the key.</param>
    public ScopeVars(int capacity, IEqualityComparer<string> comparer) : base(capacity, comparer) { }
}

/// <summary>
/// Class ScriptScopeContextUtils.
/// </summary>
public static class ScriptScopeContextUtils
{
    /// <summary>
    /// Returns the value.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="returnValue">The return value.</param>
    /// <param name="returnArgs">The return arguments.</param>
    /// <returns>StopExecution.</returns>
    public static StopExecution ReturnValue(this ScriptScopeContext scope, object returnValue, Dictionary<string, object> returnArgs = null)
    {
        scope.PageResult.ReturnValue = new ReturnValue(returnValue, returnArgs);
        scope.PageResult.HaltExecution = true;
        return StopExecution.Value;
    }

    /// <summary>
    /// Resolve value from stored arguments and filters
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="name">The name.</param>
    /// <returns>System.Object.</returns>
    public static object GetValue(this ScriptScopeContext scope, string name) => scope.PageResult.GetValue(name, scope);

    /// <summary>
    /// Resolve value from stored arguments only
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="name">The name.</param>
    /// <returns>System.Object.</returns>
    public static object GetArgument(this ScriptScopeContext scope, string name) => scope.PageResult.GetArgument(name, scope);

    /// <summary>
    /// Try Resolve value from stored arguments and filters
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool TryGetValue(this ScriptScopeContext scope, string name, out object value) =>
        scope.PageResult.TryGetValue(name, scope, argsOnly: false, out value);

    /// <summary>
    /// Tries the get method.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="name">The name.</param>
    /// <param name="fnArgValuesCount">The function argument values count.</param>
    /// <param name="fn">The function.</param>
    /// <param name="scriptMethod">The script method.</param>
    /// <param name="requiresScope">if set to <c>true</c> [requires scope].</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool TryGetMethod(this ScriptScopeContext scope, string name, int fnArgValuesCount, out Delegate fn, out ScriptMethods scriptMethod, out bool requiresScope)
    {
        scriptMethod = null;
        requiresScope = false;
        var result = scope.PageResult;

        fn = scope.GetValue(name) as Delegate ?? result.GetFilterInvoker(name, fnArgValuesCount, out scriptMethod);

        if (fn == null)
        {
            fn = result.GetContextFilterInvoker(name, fnArgValuesCount + 1, out scriptMethod);
            if (fn == null)
            {
                var contextFilter = result.GetContextBlockInvoker(name, fnArgValuesCount + 1, out scriptMethod);
                if (contextFilter != null)
                {
                    // Other languages require captured output of Context Blocks
                    var filter = scriptMethod;
                    fn = (StaticMethodInvoker)(args =>
                                                      {
                                                          var ctxScope = (ScriptScopeContext)args[0];
                                                          using var ms = MemoryStreamFactory.GetStream();
                                                          args[0] = ctxScope.ScopeWithStream(ms);
                                                          var task = (Task)contextFilter(filter, args);
                                                          task.Wait();
                                                          var discard = task.GetResult();

                                                          var ret = MemoryProvider.Instance.FromUtf8(ms.GetBufferAsMemory().Span);
                                                          return ret.ToString();
                                                      });
                }
            }
            if (fn != null)
                requiresScope = true;
        }

        return fn != null;
    }

    /// <summary>
    /// Evaluates the expression.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="expr">The expr.</param>
    /// <returns>System.Object.</returns>
    public static object EvaluateExpression(this ScriptScopeContext scope, string expr) //used in test only
    {
        expr.ParseJsExpression(out var token);
        return token.Evaluate(scope);
    }

    /// <summary>
    /// Creates the scoped context.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="template">The template.</param>
    /// <param name="scopeParams">The scope parameters.</param>
    /// <param name="cachePage">if set to <c>true</c> [cache page].</param>
    /// <returns>ScriptScopeContext.</returns>
    public static ScriptScopeContext CreateScopedContext(this ScriptScopeContext scope, string template, Dictionary<string, object> scopeParams = null, bool cachePage = true)
    {
        SharpPage dynamicPage = null;

        if (cachePage)
        {
            scope.Context.Cache.TryGetValue(template, out object value);
            dynamicPage = value as SharpPage;
        }

        if (dynamicPage == null)
        {
            dynamicPage = scope.Context.OneTimePage(template);

            if (cachePage)
            {
                scope.Context.Cache[template] = dynamicPage;
            }
        }

        var newScopeParams = new Dictionary<string, object>(scope.ScopedParams);
        scopeParams.Each((key, val) => newScopeParams[key] = val);

        var pageResult = scope.PageResult.Clone(dynamicPage).Init().Result;
        var itemScope = new ScriptScopeContext(pageResult, scope.OutputStream, newScopeParams);

        return itemScope;
    }

    /// <summary>
    /// Writes the page asynchronous.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>Task.</returns>
    public static Task WritePageAsync(this ScriptScopeContext scope) => scope.PageResult.WritePageAsync(scope.Page, scope);

    /// <summary>
    /// Scopes the with parameters.
    /// </summary>
    /// <param name="parentContext">The parent context.</param>
    /// <param name="scopedParams">The scoped parameters.</param>
    /// <returns>ScriptScopeContext.</returns>
    public static ScriptScopeContext ScopeWithParams(this ScriptScopeContext parentContext, Dictionary<string, object> scopedParams)
        => ScopeWith(parentContext, scopedParams, parentContext.OutputStream);

    /// <summary>
    /// Scopes the with.
    /// </summary>
    /// <param name="parentContext">The parent context.</param>
    /// <param name="scopedParams">The scoped parameters.</param>
    /// <param name="outputStream">The output stream.</param>
    /// <returns>ScriptScopeContext.</returns>
    public static ScriptScopeContext ScopeWith(this ScriptScopeContext parentContext, Dictionary<string, object> scopedParams = null, Stream outputStream = null)
    {
        if (scopedParams == null && outputStream == null)
            return parentContext;

        scopedParams ??= parentContext.ScopedParams;

        outputStream ??= parentContext.OutputStream;

        if (parentContext.ScopedParams.Count == 0)
            return new ScriptScopeContext(parentContext.PageResult, outputStream, scopedParams);

        var to = new Dictionary<string, object>();
        foreach (var entry in parentContext.ScopedParams)
        {
            to[entry.Key] = entry.Value;
        }
        foreach (var entry in scopedParams)
        {
            to[entry.Key] = entry.Value;
        }
        return new ScriptScopeContext(parentContext.PageResult, outputStream, to);
    }

    /// <summary>
    /// Scopes the with stream.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="stream">The stream.</param>
    /// <returns>ScriptScopeContext.</returns>
    public static ScriptScopeContext ScopeWithStream(this ScriptScopeContext scope, Stream stream) =>
        new(scope.PageResult, stream, scope.ScopedParams);

    /// <summary>
    /// Write page as an asynchronous operation.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="page">The page.</param>
    /// <param name="codePage">The code page.</param>
    /// <param name="pageParams">The page parameters.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public static async Task WritePageAsync(this ScriptScopeContext scope, SharpPage page, SharpCodePage codePage, Dictionary<string, object> pageParams, CancellationToken token = default)
    {
        await scope.PageResult.WritePageAsync(page, codePage, scope.ScopeWithParams(pageParams), token).ConfigAwait();
    }

    /// <summary>
    /// Invokes the assign expression.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="assignExpr">The assign expr.</param>
    /// <param name="target">The target.</param>
    /// <param name="value">The value.</param>
    /// <exception cref="ServiceStack.Script.BindingExpressionException">Could not evaluate assign expression '{assignExpr}' - null</exception>
    public static void InvokeAssignExpression(this ScriptScopeContext scope, string assignExpr, object target, object value)
    {
        var fn = scope.Context.GetAssignExpression(target.GetType(), assignExpr.AsMemory());

        try
        {
            fn(scope, target, value);
        }
        catch (Exception ex)
        {
            throw new BindingExpressionException($"Could not evaluate assign expression '{assignExpr}'", null, assignExpr, ex);
        }
    }
}