// ***********************************************************************
// <copyright file="IRequest.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Web;

using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;

using ServiceStack.Configuration;

/// <summary>
/// A thin wrapper around each host's Request e.g: ASP.NET, HttpListener, MQ, etc
/// </summary>
public interface IRequest : IResolver
{
    /// <summary>
    /// The Response API for this Request
    /// </summary>
    /// <value>The response.</value>
    IResponse Response { get; }

    /// <summary>
    /// The Verb / HttpMethod or Action for this request
    /// </summary>
    /// <value>The verb.</value>
    string Verb { get; }

    /// <summary>
    /// A Dictionary of HTTP Cookies sent with this Request
    /// </summary>
    /// <value>The cookies.</value>
    IDictionary<string, System.Net.Cookie> Cookies { get; }

    /// <summary>
    /// Attach any data to this request that all filters and services can access.
    /// </summary>
    /// <value>The items.</value>
    Dictionary<string, object> Items { get; }

    /// <summary>
    /// The HTTP Headers in a NameValueCollection
    /// </summary>
    /// <value>The headers.</value>
    NameValueCollection Headers { get; }

    /// <summary>
    /// The ?query=string in a NameValueCollection
    /// </summary>
    /// <value>The query string.</value>
    NameValueCollection QueryString { get; }

    /// <summary>
    /// The HTTP POST'ed Form Data in a NameValueCollection
    /// </summary>
    /// <value>The form data.</value>
    NameValueCollection FormData { get; }
    /// <summary>
    /// Buffer the Request InputStream so it can be re-read
    /// </summary>
    /// <value><c>true</c> if [use buffered stream]; otherwise, <c>false</c>.</value>
    bool UseBufferedStream { get; set; }

    /// <summary>
    /// The entire string contents of Request.InputStream
    /// </summary>
    /// <returns>System.String.</returns>
    string GetRawBody();

    /// <summary>
    /// The entire string contents of Request.InputStream async
    /// </summary>
    /// <returns>Task&lt;System.String&gt;.</returns>
    Task<string> GetRawBodyAsync();

    /// <summary>
    /// The Absolute URL for the request
    /// </summary>
    /// <value>The absolute URI.</value>
    string AbsoluteUri { get; }

    /// <summary>
    /// The Request Body Input Stream
    /// </summary>
    /// <value>The input stream.</value>
    Stream InputStream { get; }
}