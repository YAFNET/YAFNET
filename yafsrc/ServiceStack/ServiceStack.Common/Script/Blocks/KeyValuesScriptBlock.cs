// ***********************************************************************
// <copyright file="KeyValuesScriptBlock.cs" company="ServiceStack, Inc.">
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
    /// Parse text contents into a list of string Key/Value pairs and assign to specified identifier
    /// Usage: {{#keyvalues list}}
    /// Apples  2
    /// Oranges 3
    /// {{/keyvalues}}
    /// {{#keyvalues list ':'}}
    /// Grape Fruit:  2
    /// Rock Melon:   3
    /// {{/keyvalues}}
    /// </summary>
    public class KeyValuesScriptBlock : ScriptBlock
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name => "keyvalues";
        /// <summary>
        /// Parse Body using Specified Language. Uses host language if unspecified.
        /// </summary>
        /// <value>The body.</value>
        public override ScriptLanguage Body => ScriptVerbatim.Language;

        /// <summary>
        /// Writes the asynchronous.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="block">The block.</param>
        /// <param name="ct">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.NotSupportedException">#keyvalues expected string delimiter but was {token.DebugToken()}</exception>
        public override Task WriteAsync(ScriptScopeContext scope, PageBlockFragment block, CancellationToken ct)
        {
            var literal = block.Argument.Span.ParseVarName(out var name);

            var delimiter = " ";
            literal = literal.AdvancePastWhitespace();
            if (literal.Length > 0)
            {
                literal = literal.ParseJsToken(out var token);
                if (token is not JsLiteral litToken)
                    throw new NotSupportedException($"#keyvalues expected string delimiter but was {token.DebugToken()}");
                delimiter = litToken.Value.ToString();
            }

            var strFragment = (PageStringFragment)block.Body[0];
            var strDict = Context.DefaultMethods.parseKeyValues(strFragment.ValueString, delimiter);
            scope.PageResult.Args[name.ToString()] = strDict;

            return TypeConstants.EmptyTask;
        }
    }
}