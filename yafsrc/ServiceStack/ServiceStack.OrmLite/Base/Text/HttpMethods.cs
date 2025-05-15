// ***********************************************************************
// <copyright file="HttpMethods.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System.Collections.Generic;

namespace ServiceStack.OrmLite.Base.Text;

public static class HttpMethods
{
    readonly static string[] allVerbs =
    [
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
    ];

    public static HashSet<string> AllVerbs = [..allVerbs];

    public static bool Exists(string httpMethod) => AllVerbs.Contains(httpMethod.ToUpper());

    public const string Get = "GET";

    public const string Put = "PUT";

    public const string Post = "POST";

    public const string Delete = "DELETE";

    public const string Options = "OPTIONS";

    public const string Head = "HEAD";

    public const string Patch = "PATCH";
}