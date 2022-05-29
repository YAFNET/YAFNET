// ***********************************************************************
// <copyright file="ScriptBlock.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Threading;
using System.Threading.Tasks;

using ServiceStack.Text;

namespace ServiceStack.Script;

/// <summary>
/// Class ScriptBlock.
/// Implements the <see cref="ServiceStack.Script.IConfigureScriptContext" />
/// </summary>
/// <seealso cref="ServiceStack.Script.IConfigureScriptContext" />
public abstract class ScriptBlock : IConfigureScriptContext
{
    /// <summary>
    /// Parse Body using Specified Language. Uses host language if unspecified.
    /// </summary>
    /// <value>The body.</value>
    public virtual ScriptLanguage Body { get; }

    /// <summary>
    /// Configures the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    public void Configure(ScriptContext context)
    {
        if (Body != null)
            context.ParseAsLanguage[Name] = Body;
    }

    /// <summary>
    /// Gets or sets the context.
    /// </summary>
    /// <value>The context.</value>
    public ScriptContext Context { get; set; }
    /// <summary>
    /// Gets or sets the pages.
    /// </summary>
    /// <value>The pages.</value>
    public ISharpPages Pages { get; set; }
    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public abstract string Name { get; }

    /// <summary>
    /// Gets the call trace.
    /// </summary>
    /// <param name="fragment">The fragment.</param>
    /// <returns>System.String.</returns>
    protected virtual string GetCallTrace(PageBlockFragment fragment) => "Block: " + Name +
                                                                         (fragment.Argument.IsNullOrEmpty() ? "" : " (" + fragment.Argument + ")");

    /// <summary>
    /// Gets the else call trace.
    /// </summary>
    /// <param name="fragment">The fragment.</param>
    /// <returns>System.String.</returns>
    protected virtual string GetElseCallTrace(PageElseBlock fragment) => "Block: " + Name + " > Else" +
                                                                         (fragment.Argument.IsNullOrEmpty() ? "" : " (" + fragment.Argument + ")");

    /// <summary>
    /// Writes the asynchronous.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="block">The block.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    public abstract Task WriteAsync(ScriptScopeContext scope, PageBlockFragment block, CancellationToken token);

    /// <summary>
    /// Write as an asynchronous operation.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="body">The body.</param>
    /// <param name="callTrace">The call trace.</param>
    /// <param name="cancel">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    protected virtual async Task WriteAsync(ScriptScopeContext scope, PageFragment[] body, string callTrace, CancellationToken cancel)
    {
        await scope.PageResult.WriteFragmentsAsync(scope, body, callTrace, cancel).ConfigAwait();
    }

    /// <summary>
    /// Write as an asynchronous operation.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="body">The body.</param>
    /// <param name="callTrace">The call trace.</param>
    /// <param name="cancel">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    protected virtual async Task WriteAsync(ScriptScopeContext scope, JsStatement[] body, string callTrace, CancellationToken cancel)
    {
        await scope.PageResult.WriteStatementsAsync(scope, body, callTrace, cancel).ConfigAwait();
    }

    /// <summary>
    /// Write body as an asynchronous operation.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="fragment">The fragment.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    protected virtual async Task WriteBodyAsync(ScriptScopeContext scope, PageBlockFragment fragment, CancellationToken token)
    {
        await WriteAsync(scope, fragment.Body, GetCallTrace(fragment), token).ConfigAwait();
    }

    /// <summary>
    /// Write else as an asynchronous operation.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="fragment">The fragment.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    protected virtual async Task WriteElseAsync(ScriptScopeContext scope, PageElseBlock fragment, CancellationToken token)
    {
        await WriteAsync(scope, fragment.Body, GetElseCallTrace(fragment), token).ConfigAwait();
    }

    /// <summary>
    /// Write else as an asynchronous operation.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="elseBlocks">The else blocks.</param>
    /// <param name="cancel">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    protected async Task WriteElseAsync(ScriptScopeContext scope, PageElseBlock[] elseBlocks, CancellationToken cancel)
    {
        foreach (var elseBlock in elseBlocks)
        {
            if (elseBlock.Argument.IsNullOrEmpty())
            {
                await WriteElseAsync(scope, elseBlock, cancel).ConfigAwait();
                return;
            }

            var argument = elseBlock.Argument;
            if (argument.StartsWith("if "))
                argument = argument.Advance(3);

            var result = await argument.GetJsExpressionAndEvaluateToBoolAsync(scope,
                             ifNone: () => throw new NotSupportedException("'else if' block does not have a valid expression")).ConfigAwait();
            if (result)
            {
                await WriteElseAsync(scope, elseBlock, cancel).ConfigAwait();
                return;
            }
        }
    }

    /// <summary>
    /// Determines whether this instance [can export scope arguments] the specified element.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><c>true</c> if this instance [can export scope arguments] the specified element; otherwise, <c>false</c>.</returns>
    protected bool CanExportScopeArgs(object element) =>
        element != null && element is not string && (element.GetType().IsClass || element.GetType().Name == "KeyValuePair`2");

    /// <summary>
    /// Asserts the within maximum quota.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Int32.</returns>
    protected int AssertWithinMaxQuota(int value) => Context.DefaultMethods.AssertWithinMaxQuota(value);

}

/// <summary>
/// Class DefaultScriptBlocks.
/// Implements the <see cref="ServiceStack.Script.IScriptPlugin" />
/// </summary>
/// <seealso cref="ServiceStack.Script.IScriptPlugin" />
public class DefaultScriptBlocks : IScriptPlugin
{
    /// <summary>
    /// Registers the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    public void Register(ScriptContext context)
    {
        context.ScriptBlocks.AddRange(new ScriptBlock[] {
                                                                new IfScriptBlock(),
                                                                new EachScriptBlock(),
                                                                new RawScriptBlock(),
                                                                new CaptureScriptBlock(),
                                                                new PartialScriptBlock(),
                                                                new WithScriptBlock(),
                                                                new NoopScriptBlock(),
                                                                new KeyValuesScriptBlock(),
                                                                new CsvScriptBlock(),
                                                                new FunctionScriptBlock(),
                                                                new WhileScriptBlock(),
                                                            });
    }
}

/// <summary>
/// Class ProtectedScriptBlocks.
/// Implements the <see cref="ServiceStack.Script.IScriptPlugin" />
/// </summary>
/// <seealso cref="ServiceStack.Script.IScriptPlugin" />
public class ProtectedScriptBlocks : IScriptPlugin
{
    /// <summary>
    /// Registers the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    public void Register(ScriptContext context)
    {
        context.ScriptBlocks.AddRange(new ScriptBlock[] {
                                                                new EvalScriptBlock(), // evalScript has same functionality and is registered by default 
                                                            });
    }
}