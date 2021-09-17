// ***********************************************************************
// <copyright file="NoopScriptBlock.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System.Threading;
using System.Threading.Tasks;

namespace ServiceStack.Script
{
    /// <summary>
    /// Handlebars.js like noop block
    /// Usage: Remove {{#noop}} contents in here {{/noop}}
    /// </summary>
    public class NoopScriptBlock : ScriptBlock
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name => "noop";
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
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        public override Task WriteAsync(ScriptScopeContext scope, PageBlockFragment block, CancellationToken token) =>
            TypeConstants.EmptyTask;
    }
}