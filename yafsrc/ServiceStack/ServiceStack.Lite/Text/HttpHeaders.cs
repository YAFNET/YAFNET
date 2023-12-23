// ***********************************************************************
// <copyright file="HttpHeaders.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;

namespace ServiceStack;

public static class HttpHeaders
{
    public const string XTag = "X-Tag";

    public const string XTrigger = "X-Trigger"; // Trigger Events on UserAgent

    public const string XLocation = "X-Location";

    public const string XStatus = "X-Status";

    public const string Referer = "Referer";

    public const string CacheControl = "Cache-Control";

    public const string IfMatch = "If-Match";

    public const string Accept = "Accept";

    public const string AcceptEncoding = "Accept-Encoding";

    public const string ContentType = "Content-Type";

    public const string ContentEncoding = "Content-Encoding";

    public const string ContentLength = "Content-Length";

    public const string ContentDisposition = "Content-Disposition";

    public const string Location = "Location";

    public const string ETag = "ETag";

    public const string Age = "Age";

    public const string Expires = "Expires";

    public const string Vary = "Vary";

    public const string Authorization = "Authorization";

    public const string AllowOrigin = "Access-Control-Allow-Origin";

    public const string AllowHeaders = "Access-Control-Allow-Headers";

    public const string Origin = "Origin";

    public const string RequestMethod = "Access-Control-Request-Method";

    public const string AcceptRanges = "Accept-Ranges";

    public const string ContentRange = "Content-Range";

    public const string Range = "Range";

    public const string Allow = "Allow";

    public const string AcceptCharset = "Accept-Charset";

    public const string Connection = "Connection";

    public const string Cookie = "Cookie";

    public const string ContentLanguage = "Content-Language";

    public const string Expect = "Expect";

    public const string Pragma = "Pragma";

    public const string TE = "TE";

    public const string Trailer = "Trailer";

    public const string TransferEncoding = "Transfer-Encoding";

    public const string Upgrade = "Upgrade";

    public const string Via = "Via";

    public const string Warning = "Warning";

    public const string Date = "Date";

    public const string Host = "Host";

    public const string UserAgent = "User-Agent";
}


public static class CompressionTypes
{
    public const string Default = Deflate;

    public const string Brotli = "br";

    public const string Deflate = "deflate";

    public const string GZip = "gzip";

    public static bool IsValid(string compressionType)
    {
        return compressionType is Deflate or GZip;
    }

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