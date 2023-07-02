// ***********************************************************************
// <copyright file="HttpMethods.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack;

using System.Collections.Generic;

/// <summary>
/// Class HttpMethods.
/// </summary>
public static class HttpMethods
{
    /// <summary>
    /// All verbs
    /// </summary>
    static readonly string[] allVerbs =
        {
            "OPTIONS", "GET", "HEAD", "POST", "PUT", "DELETE", "TRACE", "CONNECT", // RFC 2616
            "PROPFIND", "PROPPATCH", "MKCOL", "COPY", "MOVE", "LOCK", "UNLOCK", // RFC 2518
            "VERSION-CONTROL", "REPORT", "CHECKOUT", "CHECKIN", "UNCHECKOUT", "MKWORKSPACE", "UPDATE", "LABEL",
            "MERGE", "BASELINE-CONTROL", "MKACTIVITY", // RFC 3253
            "ORDERPATCH", // RFC 3648
            "ACL", // RFC 3744
            "PATCH", // https://datatracker.ietf.org/doc/draft-dusseault-http-patch/
            "SEARCH", // https://datatracker.ietf.org/doc/draft-reschke-webdav-search/
            "BCOPY", "BDELETE", "BMOVE", "BPROPFIND", "BPROPPATCH", "NOTIFY", "POLL", "SUBSCRIBE",
            "UNSUBSCRIBE" //MS Exchange WebDav: http://msdn.microsoft.com/en-us/library/aa142917.aspx
        };

    /// <summary>
    /// All verbs
    /// </summary>
    public static HashSet<string> AllVerbs = new(allVerbs);

    /// <summary>
    /// Existses the specified HTTP method.
    /// </summary>
    /// <param name="httpMethod">The HTTP method.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool Exists(string httpMethod) => AllVerbs.Contains(httpMethod.ToUpper());

    /// <summary>
    /// Determines whether the specified HTTP verb has verb.
    /// </summary>
    /// <param name="httpVerb">The HTTP verb.</param>
    /// <returns><c>true</c> if the specified HTTP verb has verb; otherwise, <c>false</c>.</returns>
    public static bool HasVerb(string httpVerb) => Exists(httpVerb);

    /// <summary>
    /// The get
    /// </summary>
    public const string Get = "GET";

    /// <summary>
    /// The put
    /// </summary>
    public const string Put = "PUT";

    /// <summary>
    /// The post
    /// </summary>
    public const string Post = "POST";

    /// <summary>
    /// The delete
    /// </summary>
    public const string Delete = "DELETE";

    /// <summary>
    /// The options
    /// </summary>
    public const string Options = "OPTIONS";

    /// <summary>
    /// The head
    /// </summary>
    public const string Head = "HEAD";

    /// <summary>
    /// The patch
    /// </summary>
    public const string Patch = "PATCH";
}