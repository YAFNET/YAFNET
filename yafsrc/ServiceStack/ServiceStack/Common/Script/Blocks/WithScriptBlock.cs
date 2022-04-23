// ***********************************************************************
// <copyright file="WithScriptBlock.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceStack.Script;

using ServiceStack.Text;

/// <summary>
/// Handlebars.js like with block
/// Usages: {{#with person}} Hi {{name}}, I'm {{age}} years old{{/with}}
/// {{#with person}} Hi {{name}}, I'm {{age}} years old {{else}} no person {{/with}}
/// </summary>
public class WithScriptBlock : ScriptBlock
{
    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public override string Name => "with";

    /// <summary>
    /// Write as an asynchronous operation.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="block">The block.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public override async Task WriteAsync(ScriptScopeContext scope, PageBlockFragment block, CancellationToken token)
    {
        var result = await block.Argument.GetJsExpressionAndEvaluateAsync(scope,
                         ifNone: () => throw new NotSupportedException("'with' block does not have a valid expression"));

        if (result != null)
        {
            var resultAsMap = result.ToObjectDictionary();

            var withScope = scope.ScopeWithParams(resultAsMap);

            await WriteBodyAsync(withScope, block, token);
        }
        else
        {
            await WriteElseAsync(scope, block.ElseBlocks, token);
        }
    }
}