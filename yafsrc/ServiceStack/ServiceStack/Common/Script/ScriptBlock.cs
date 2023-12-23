// ***********************************************************************
// <copyright file="ScriptBlock.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

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
    /// Writes the asynchronous.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="block">The block.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    public abstract Task WriteAsync(ScriptScopeContext scope, PageBlockFragment block, CancellationToken token);
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
                                                                new CsvScriptBlock()
                                                            });
    }
}