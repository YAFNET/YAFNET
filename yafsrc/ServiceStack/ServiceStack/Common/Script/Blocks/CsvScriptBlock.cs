// ***********************************************************************
// <copyright file="CsvScriptBlock.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace ServiceStack.Script;

/// <summary>
/// Parse csv contents into a string dictionary List and assign to identifier
/// Usage: {{#csv list}}
/// Item,Qty
/// Apples,2
/// Oranges,3
/// {{/csv}}
/// </summary>
public class CsvScriptBlock : ScriptBlock
{
    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public override string Name => "csv";
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
    public override Task WriteAsync(ScriptScopeContext scope, PageBlockFragment block, CancellationToken ct)
    {
        var literal = block.Argument.ParseVarName(out var name);

        var strFragment = (PageStringFragment)block.Body[0];
        var csvList = Context.DefaultMethods.parseCsv(strFragment.ValueString);
        scope.PageResult.Args[name.ToString()] = csvList;

        return TypeConstants.EmptyTask;
    }
}