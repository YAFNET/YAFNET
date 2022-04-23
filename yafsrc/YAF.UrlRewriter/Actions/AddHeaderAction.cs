// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

namespace YAF.UrlRewriter.Actions;

using System;

/// <summary>
/// Action that adds a given header.
/// </summary>
public class AddHeaderAction : IRewriteAction
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="header">The header name.</param>
    /// <param name="value">The header value.</param>
    public AddHeaderAction(string header, string value)
    {
        this.Header = header ?? throw new ArgumentNullException(nameof(header));
        this.Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// The header name.
    /// </summary>
    public string Header { get; }

    /// <summary>
    /// The header value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Executes the action.
    /// </summary>
    /// <param name="context">The rewrite context.</param>
    public RewriteProcessing Execute(IRewriteContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.ResponseHeaders.Add(this.Header, this.Value);

        return RewriteProcessing.ContinueProcessing;
    }
}