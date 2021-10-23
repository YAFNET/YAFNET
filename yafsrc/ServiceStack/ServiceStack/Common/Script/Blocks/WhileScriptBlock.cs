// ***********************************************************************
// <copyright file="WhileScriptBlock.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Threading;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace ServiceStack.Script
{
    /// <summary>
    /// while block
    /// Usages: {{#while times &gt; 0}} {{times}}. {{times - 1 | to =&gt; times}} {{/while}}
    /// {{#while b}} {{ false | to =&gt; b }} {{else}} {{b}} was false {{/while}}
    /// Max Iterations = Context.Args[ScriptConstants.MaxQuota]
    /// </summary>
    public class WhileScriptBlock : ScriptBlock
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name => "while";

        /// <summary>
        /// Write as an asynchronous operation.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="block">The block.</param>
        /// <param name="ct">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public override async Task WriteAsync(ScriptScopeContext scope, PageBlockFragment block, CancellationToken ct)
        {
            var result = await block.Argument.GetJsExpressionAndEvaluateToBoolAsync(scope,
                ifNone: () => throw new NotSupportedException("'while' block does not have a valid expression")).ConfigAwait();

            var iterations = 0;

            if (result)
            {
                do
                {
                    await WriteBodyAsync(scope, block, ct);

                    result = await block.Argument.GetJsExpressionAndEvaluateToBoolAsync(scope,
                        ifNone: () => throw new NotSupportedException("'while' block does not have a valid expression"));

                    Context.DefaultMethods.AssertWithinMaxQuota(iterations++);

                } while (result);
            }
            else
            {
                await WriteElseAsync(scope, block.ElseBlocks, ct);
            }
        }
    }
}