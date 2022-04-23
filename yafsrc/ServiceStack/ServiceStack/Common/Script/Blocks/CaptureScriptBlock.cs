// ***********************************************************************
// <copyright file="CaptureScriptBlock.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace ServiceStack.Script;

/// <summary>
/// Captures the output and assigns it to the specified variable.
/// Accepts an optional Object Dictionary as scope arguments when evaluating body.
/// Usages: {{#capture output}} {{#each args}} - [{{it}}](/path?arg={{it}}) {{/each}} {{/capture}}
/// {{#capture output {nums:[1,2,3]} }} {{#each nums}} {{it}} {{/each}} {{/capture}}
/// {{#capture appendTo output {nums:[1,2,3]} }} {{#each nums}} {{it}} {{/each}} {{/capture}}
/// </summary>
public class CaptureScriptBlock : ScriptBlock
{
    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public override string Name => "capture";
    /// <summary>
    /// Parse Body using Specified Language. Uses host language if unspecified.
    /// </summary>
    /// <value>The body.</value>
    public override ScriptLanguage Body => ScriptTemplate.Language;

    /// <summary>
    /// Struct Tuple
    /// </summary>
    internal struct Tuple
    {
        /// <summary>
        /// The name
        /// </summary>
        internal string name;
        /// <summary>
        /// The scope arguments
        /// </summary>
        internal Dictionary<string, object> scopeArgs;
        /// <summary>
        /// The append to
        /// </summary>
        internal bool appendTo;
        /// <summary>
        /// Initializes a new instance of the <see cref="Tuple"/> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="scopeArgs">The scope arguments.</param>
        /// <param name="appendTo">if set to <c>true</c> [append to].</param>
        internal Tuple(string name, Dictionary<string, object> scopeArgs, bool appendTo)
        {
            this.name = name;
            this.scopeArgs = scopeArgs;
            this.appendTo = appendTo;
        }
    }

    /// <summary>
    /// Write as an asynchronous operation.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="block">The block.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public override async Task WriteAsync(ScriptScopeContext scope, PageBlockFragment block, CancellationToken token)
    {
        var tuple = Parse(scope, block);
        var name = tuple.name;

        using var ms = MemoryStreamFactory.GetStream();
        var useScope = scope.ScopeWith(tuple.scopeArgs, ms);

        await WriteBodyAsync(useScope, block, token).ConfigAwait();

        // ReSharper disable once MethodHasAsyncOverload
        var capturedOutput = ms.ReadToEnd();

        if (tuple.appendTo && scope.PageResult.Args.TryGetValue(name, out var oVar)
                           && oVar is string existingString)
        {
            scope.PageResult.Args[name] = existingString + capturedOutput;
            return;
        }

        scope.PageResult.Args[name] = capturedOutput;
    }

    //Extract usages of Span outside of async method 
    /// <summary>
    /// Parses the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="block">The block.</param>
    /// <returns>Tuple.</returns>
    /// <exception cref="System.NotSupportedException">'capture' block is missing name of variable to assign captured output to</exception>
    /// <exception cref="System.NotSupportedException">'capture' block is missing name of variable to assign captured output to</exception>
    /// <exception cref="System.NotSupportedException">Any 'capture' argument must be an Object Dictionary</exception>
    private Tuple Parse(ScriptScopeContext scope, PageBlockFragment block)
    {
        if (block.Argument.IsNullOrWhiteSpace())
            throw new NotSupportedException("'capture' block is missing name of variable to assign captured output to");

        var literal = block.Argument.AdvancePastWhitespace();
        bool appendTo = false;
        if (literal.StartsWith("appendTo "))
        {
            appendTo = true;
            literal = literal.Advance("appendTo ".Length);
        }

        literal = literal.ParseVarName(out var name);
        if (name.IsNullOrEmpty())
            throw new NotSupportedException("'capture' block is missing name of variable to assign captured output to");

        literal = literal.AdvancePastWhitespace();

        var argValue = literal.GetJsExpressionAndEvaluate(scope);

        var scopeArgs = argValue as Dictionary<string, object>;

        if (argValue != null && scopeArgs == null)
            throw new NotSupportedException("Any 'capture' argument must be an Object Dictionary");

        return new Tuple(name.ToString(), scopeArgs, appendTo);
    }
}