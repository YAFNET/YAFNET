// ***********************************************************************
// <copyright file="RawScriptBlock.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System.Threading;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace ServiceStack.Script
{
    /// <summary>
    /// Special block which captures the raw body as a string fragment
    /// Usages: {{#raw}}emit {{ verbatim }} body{{/raw}}
    /// {{#raw varname}}assigned to varname{{/raw}}
    /// {{#raw appendTo varname}}appended to varname{{/raw}}
    /// </summary>
    public class RawScriptBlock : ScriptBlock
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name => "raw";
        /// <summary>
        /// Parse Body using Specified Language. Uses host language if unspecified.
        /// </summary>
        /// <value>The body.</value>
        public override ScriptLanguage Body => ScriptVerbatim.Language;

        /// <summary>
        /// Write as an asynchronous operation.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="block">The block.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public override async Task WriteAsync(ScriptScopeContext scope, PageBlockFragment block, CancellationToken token)
        {
            var strFragment = (PageStringFragment)block.Body[0];

            if (!block.Argument.IsNullOrWhiteSpace())
            {
                Capture(scope, block, strFragment);
            }
            else
            {
                await scope.OutputStream.WriteAsync(strFragment.Value.Span, token).ConfigAwait();
            }
        }

        /// <summary>
        /// Captures the specified scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="block">The block.</param>
        /// <param name="strFragment">The string fragment.</param>
        private static void Capture(ScriptScopeContext scope, PageBlockFragment block, PageStringFragment strFragment)
        {
            var literal = block.Argument.Span.AdvancePastWhitespace();
            bool appendTo = false;
            if (literal.StartsWith("appendTo "))
            {
                appendTo = true;
                literal = literal.Advance("appendTo ".Length);
            }

            literal = literal.ParseVarName(out var name);
            var nameString = name.Value();
            if (appendTo && scope.PageResult.Args.TryGetValue(nameString, out var oVar)
                         && oVar is string existingString)
            {
                scope.PageResult.Args[nameString] = existingString + strFragment.Value;
                return;
            }

            scope.PageResult.Args[nameString] = strFragment.Value.ToString();
        }
    }
}