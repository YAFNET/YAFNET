// ***********************************************************************
// <copyright file="IfScriptBlock.cs" company="ServiceStack, Inc.">
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
/// Handlebars.js like if block
/// Usages: {{#if a &gt; b}} max {{a}} {{/if}}
/// {{#if a &gt; b}} max {{a}} {{else}} max {{b}} {{/if}}
/// {{#if a &gt; b}} max {{a}} {{else if b &gt; c}} max {{b}} {{else}} max {{c}} {{/if}}
/// </summary>
public class IfScriptBlock : ScriptBlock
{
    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public override string Name => "if";

    /// <summary>
    /// Write as an asynchronous operation.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="block">The block.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public override async Task WriteAsync(ScriptScopeContext scope, PageBlockFragment block, CancellationToken token)
    {
        var result = await block.Argument.GetJsExpressionAndEvaluateToBoolAsync(scope,
                         ifNone: () => throw new NotSupportedException("'if' block does not have a valid expression")).ConfigAwait();

        if (result)
        {
            await WriteBodyAsync(scope, block, token).ConfigAwait();
        }
        else
        {
            await WriteElseAsync(scope, block.ElseBlocks, token).ConfigAwait();
        }
    }
}