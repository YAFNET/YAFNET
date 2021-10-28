// ***********************************************************************
// <copyright file="ScriptLanguage.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceStack.Script
{
    using ServiceStack.Text;

    /// <summary>
    /// Class ScriptLanguage.
    /// </summary>
    public abstract class ScriptLanguage
    {
        /// <summary>
        /// Gets the verbatim.
        /// </summary>
        /// <value>The verbatim.</value>
        public static ScriptLanguage Verbatim => ScriptVerbatim.Language;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the line comment.
        /// </summary>
        /// <value>The line comment.</value>
        public virtual string LineComment => null;

        /// <summary>
        /// Parses the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="body">The body.</param>
        /// <returns>List&lt;PageFragment&gt;.</returns>
        public List<PageFragment> Parse(ScriptContext context, ReadOnlyMemory<char> body) => Parse(context, body, default);

        /// <summary>
        /// Parses the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="body">The body.</param>
        /// <param name="modifiers">The modifiers.</param>
        /// <returns>List&lt;PageFragment&gt;.</returns>
        public abstract List<PageFragment> Parse(ScriptContext context, ReadOnlyMemory<char> body, ReadOnlyMemory<char> modifiers);

        /// <summary>
        /// Writes the page fragment asynchronous.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="fragment">The fragment.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<bool> WritePageFragmentAsync(ScriptScopeContext scope, PageFragment fragment, CancellationToken token) => TypeConstants.FalseTask;

        /// <summary>
        /// Writes the statement asynchronous.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="statement">The statement.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<bool> WriteStatementAsync(ScriptScopeContext scope, JsStatement statement, CancellationToken token) => TypeConstants.FalseTask;

        /// <summary>
        /// Parses the verbatim block.
        /// </summary>
        /// <param name="blockName">Name of the block.</param>
        /// <param name="argument">The argument.</param>
        /// <param name="body">The body.</param>
        /// <returns>PageBlockFragment.</returns>
        public virtual PageBlockFragment ParseVerbatimBlock(string blockName, ReadOnlyMemory<char> argument, ReadOnlyMemory<char> body)
        {
            var bodyFragment = new List<PageFragment> { new PageStringFragment(body) };
            var blockFragment = new PageBlockFragment(blockName, argument, bodyFragment);
            return blockFragment;
        }

        /// <summary>
        /// Unwraps the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Object.</returns>
        public static object UnwrapValue(object value)
        {
            if (value == null || value == JsNull.Value || value == StopExecution.Value || value == IgnoreResult.Value)
                return null;
            if (value is IRawString rs)
                return rs.ToRawString();
            return value;
        }
    }

    /// <summary>
    /// Class ScriptVerbatim. This class cannot be inherited.
    /// Implements the <see cref="ServiceStack.Script.ScriptLanguage" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptLanguage" />
    public sealed class ScriptVerbatim : ScriptLanguage
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="ScriptVerbatim"/> class from being created.
        /// </summary>
        private ScriptVerbatim() { } // force usage of singleton

        /// <summary>
        /// The language
        /// </summary>
        public static readonly ScriptLanguage Language = new ScriptVerbatim();

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name => "verbatim";

        /// <summary>
        /// Parses the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="body">The body.</param>
        /// <param name="modifiers">The modifiers.</param>
        /// <returns>List&lt;PageFragment&gt;.</returns>
        public override List<PageFragment> Parse(ScriptContext context, ReadOnlyMemory<char> body, ReadOnlyMemory<char> modifiers) =>
            new()
            {
                new PageStringFragment(body)
            };
    }
}