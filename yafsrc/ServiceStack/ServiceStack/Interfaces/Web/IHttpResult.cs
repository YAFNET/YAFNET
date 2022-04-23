// ***********************************************************************
// <copyright file="IHttpResult.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Net;

namespace ServiceStack.Web;

/// <summary>
/// Interface IHttpResult
/// Implements the <see cref="ServiceStack.Web.IHasOptions" />
/// </summary>
/// <seealso cref="ServiceStack.Web.IHasOptions" />
public interface IHttpResult : IHasOptions
{
    /// <summary>
    /// The HTTP Response Status
    /// </summary>
    /// <value>The status.</value>
    int Status { get; set; }

    /// <summary>
    /// The HTTP Response Status Code
    /// </summary>
    /// <value>The status code.</value>
    HttpStatusCode StatusCode { get; set; }

    /// <summary>
    /// The HTTP Status Description
    /// </summary>
    /// <value>The status description.</value>
    string StatusDescription { get; set; }

    /// <summary>
    /// The HTTP Response ContentType
    /// </summary>
    /// <value>The type of the content.</value>
    string ContentType { get; set; }

    /// <summary>
    /// Additional HTTP Headers
    /// </summary>
    /// <value>The headers.</value>
    Dictionary<string, string> Headers { get; }

    /// <summary>
    /// Additional HTTP Cookies
    /// </summary>
    /// <value>The cookies.</value>
    List<System.Net.Cookie> Cookies { get; }

    /// <summary>
    /// Response DTO
    /// </summary>
    /// <value>The response.</value>
    object Response { get; set; }

    /// <summary>
    /// Holds the request call context
    /// </summary>
    /// <value>The request context.</value>
    IRequest RequestContext { get; set; }

    /// <summary>
    /// The padding length written with the body, to be added to ContentLength of body
    /// </summary>
    /// <value>The length of the padding.</value>
    int PaddingLength { get; set; }

    /// <summary>
    /// Serialize the Response within the specified scope
    /// </summary>
    /// <value>The result scope.</value>
    Func<IDisposable> ResultScope { get; set; }
}