// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

namespace YAF.UrlRewriter.Actions;

using System.Net;

/// <summary>
/// Returns a 404 Not Found HTTP status code.
/// </summary>
public sealed class NotFoundAction : SetStatusAction
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public NotFoundAction()
        : base(HttpStatusCode.NotFound)
    {
    }
}