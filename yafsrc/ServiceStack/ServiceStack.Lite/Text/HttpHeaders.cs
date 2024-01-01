// ***********************************************************************
// <copyright file="HttpHeaders.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;

namespace ServiceStack;

/// <summary>
/// Class HttpHeaders.
/// </summary>
public static class HttpHeaders
{
    /// <summary>
    /// The x tag
    /// </summary>
    public const string XTag = "X-Tag";

    /// <summary>
    /// The x location
    /// </summary>
    public const string XLocation = "X-Location";

    /// <summary>
    /// The x status
    /// </summary>
    public const string XStatus = "X-Status";

    /// <summary>
    /// The referer
    /// </summary>
    public const string Referer = "Referer";

    /// <summary>
    /// The cache control
    /// </summary>
    public const string CacheControl = "Cache-Control";

    /// <summary>
    /// If modified since
    /// </summary>
    public const string IfModifiedSince = "If-Modified-Since";

    /// <summary>
    /// If match
    /// </summary>
    public const string IfMatch = "If-Match";

    /// <summary>
    /// The last modified
    /// </summary>
    public const string LastModified = "Last-Modified";

    /// <summary>
    /// The accept
    /// </summary>
    public const string Accept = "Accept";

    /// <summary>
    /// The accept encoding
    /// </summary>
    public const string AcceptEncoding = "Accept-Encoding";

    /// <summary>
    /// The content type
    /// </summary>
    public const string ContentType = "Content-Type";

    /// <summary>
    /// The content encoding
    /// </summary>
    public const string ContentEncoding = "Content-Encoding";

    /// <summary>
    /// The content length
    /// </summary>
    public const string ContentLength = "Content-Length";

    /// <summary>
    /// The content disposition
    /// </summary>
    public const string ContentDisposition = "Content-Disposition";

    /// <summary>
    /// The location
    /// </summary>
    public const string Location = "Location";

    /// <summary>
    /// The set cookie
    /// </summary>
    public const string SetCookie = "Set-Cookie";

    /// <summary>
    /// The e tag
    /// </summary>
    public const string ETag = "ETag";

    /// <summary>
    /// The age
    /// </summary>
    public const string Age = "Age";

    /// <summary>
    /// The expires
    /// </summary>
    public const string Expires = "Expires";

    /// <summary>
    /// The vary
    /// </summary>
    public const string Vary = "Vary";

    /// <summary>
    /// The authorization
    /// </summary>
    public const string Authorization = "Authorization";

    /// <summary>
    /// The allow origin
    /// </summary>
    public const string AllowOrigin = "Access-Control-Allow-Origin";

    /// <summary>
    /// The origin
    /// </summary>
    public const string Origin = "Origin";

    /// <summary>
    /// The request method
    /// </summary>
    public const string RequestMethod = "Access-Control-Request-Method";

    /// <summary>
    /// The request headers
    /// </summary>
    public const string RequestHeaders = "Access-Control-Request-Headers";

    /// <summary>
    /// The accept ranges
    /// </summary>
    public const string AcceptRanges = "Accept-Ranges";

    /// <summary>
    /// The content range
    /// </summary>
    public const string ContentRange = "Content-Range";

    /// <summary>
    /// The range
    /// </summary>
    public const string Range = "Range";

    /// <summary>
    /// The allow
    /// </summary>
    public const string Allow = "Allow";

    /// <summary>
    /// The connection
    /// </summary>
    public const string Connection = "Connection";

    /// <summary>
    /// The cookie
    /// </summary>
    public const string Cookie = "Cookie";

    /// <summary>
    /// The content language
    /// </summary>
    public const string ContentLanguage = "Content-Language";

    /// <summary>
    /// The expect
    /// </summary>
    public const string Expect = "Expect";

    /// <summary>
    /// The pragma
    /// </summary>
    public const string Pragma = "Pragma";

    /// <summary>
    /// The proxy connection
    /// </summary>
    public const string ProxyConnection = "Proxy-Connection";

    /// <summary>
    /// The te
    /// </summary>
    public const string TE = "TE";

    /// <summary>
    /// The trailer
    /// </summary>
    public const string Trailer = "Trailer";

    /// <summary>
    /// The transfer encoding
    /// </summary>
    public const string TransferEncoding = "Transfer-Encoding";

    /// <summary>
    /// The upgrade
    /// </summary>
    public const string Upgrade = "Upgrade";

    /// <summary>
    /// The via
    /// </summary>
    public const string Via = "Via";

    /// <summary>
    /// The warning
    /// </summary>
    public const string Warning = "Warning";

    /// <summary>
    /// The date
    /// </summary>
    public const string Date = "Date";

    /// <summary>
    /// The host
    /// </summary>
    public const string Host = "Host";

    /// <summary>
    /// The user agent
    /// </summary>
    public const string UserAgent = "User-Agent";

    /// <summary>
    /// The restricted headers
    /// </summary>
    public static HashSet<string> RestrictedHeaders { get; set; } = new(StringComparer.OrdinalIgnoreCase) {
        Accept,
        Connection,
        ContentLength,
        ContentType,
        Date,
        Expect,
        Host,
        IfModifiedSince,
        Range,
        Referer,
        TransferEncoding,
        UserAgent,
        ProxyConnection,
    };
}


/// <summary>
/// Class CompressionTypes.
/// </summary>
public static class CompressionTypes
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// The default
    /// </summary>
    public const string Default = Brotli;
#else
    public const string Default = Deflate;
#endif

    /// <summary>
    /// The brotli
    /// </summary>
    public const string Brotli = "br";

    /// <summary>
    /// The deflate
    /// </summary>
    public const string Deflate = "deflate";

    /// <summary>
    /// The g zip
    /// </summary>
    public const string GZip = "gzip";

    /// <summary>
    /// Returns true if ... is valid.
    /// </summary>
    /// <param name="compressionType">Type of the compression.</param>
    /// <returns>bool.</returns>
    public static bool IsValid(string compressionType)
    {
#if NET7_0_OR_GREATER
            return compressionType is Deflate or GZip or Brotli;
#else
        return compressionType is Deflate or GZip;
#endif
    }

    /// <summary>
    /// Gets the extension.
    /// </summary>
    /// <param name="compressionType">Type of the compression.</param>
    /// <returns>string.</returns>
    /// <exception cref="NotSupportedException">Unknown compressionType: " + compressionType</exception>
    public static string GetExtension(string compressionType)
    {
        switch (compressionType)
        {
            case Brotli:
            case Deflate:
            case GZip:
                return "." + compressionType;
            default:
                throw new NotSupportedException("Unknown compressionType: " + compressionType);
        }
    }

}