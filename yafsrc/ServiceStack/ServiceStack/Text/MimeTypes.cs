// ***********************************************************************
// <copyright file="MimeTypes.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack;

using System;
using System.Collections.Generic;

using ServiceStack.Text;

/// <summary>
/// Class MimeTypes.
/// </summary>
public static class MimeTypes
{
    /// <summary>
    /// The extension MIME types
    /// </summary>
    public static Dictionary<string, string> ExtensionMimeTypes = new();

    /// <summary>
    /// The UTF8 suffix
    /// </summary>
    public const string Utf8Suffix = "; charset=utf-8";

    /// <summary>
    /// The HTML
    /// </summary>
    public const string Html = "text/html";

    /// <summary>
    /// The HTML UTF8
    /// </summary>
    public const string HtmlUtf8 = Html + Utf8Suffix;

    /// <summary>
    /// The CSS
    /// </summary>
    public const string Css = "text/css";

    /// <summary>
    /// The XML
    /// </summary>
    public const string Xml = "application/xml";

    /// <summary>
    /// The XML text
    /// </summary>
    public const string XmlText = "text/xml";

    /// <summary>
    /// The json
    /// </summary>
    public const string Json = "application/json";

    /// <summary>
    /// The problem json
    /// </summary>
    public const string ProblemJson = "application/problem+json";

    /// <summary>
    /// The json text
    /// </summary>
    public const string JsonText = "text/json";

    /// <summary>
    /// The JSV
    /// </summary>
    public const string Jsv = "application/jsv";

    /// <summary>
    /// The JSV text
    /// </summary>
    public const string JsvText = "text/jsv";

    /// <summary>
    /// The CSV
    /// </summary>
    public const string Csv = "text/csv";

    /// <summary>
    /// The proto buf
    /// </summary>
    public const string ProtoBuf = "application/x-protobuf";

    /// <summary>
    /// The java script
    /// </summary>
    public const string JavaScript = "text/javascript";

    /// <summary>
    /// The web assembly
    /// </summary>
    public const string WebAssembly = "application/wasm";

    /// <summary>
    /// The jar
    /// </summary>
    public const string Jar = "application/java-archive";

    /// <summary>
    /// The DMG
    /// </summary>
    public const string Dmg = "application/x-apple-diskimage";

    /// <summary>
    /// The PKG
    /// </summary>
    public const string Pkg = "application/x-newton-compatible-pkg";

    /// <summary>
    /// The form URL encoded
    /// </summary>
    public const string FormUrlEncoded = "application/x-www-form-urlencoded";

    /// <summary>
    /// The multi part form data
    /// </summary>
    public const string MultiPartFormData = "multipart/form-data";

    /// <summary>
    /// The json report
    /// </summary>
    public const string JsonReport = "text/jsonreport";

    /// <summary>
    /// The soap11
    /// </summary>
    public const string Soap11 = "text/xml; charset=utf-8";

    /// <summary>
    /// The soap12
    /// </summary>
    public const string Soap12 = "application/soap+xml";

    /// <summary>
    /// The yaml
    /// </summary>
    public const string Yaml = "application/yaml";

    /// <summary>
    /// The yaml text
    /// </summary>
    public const string YamlText = "text/yaml";

    /// <summary>
    /// The plain text
    /// </summary>
    public const string PlainText = "text/plain";

    /// <summary>
    /// The markdown text
    /// </summary>
    public const string MarkdownText = "text/markdown";

    /// <summary>
    /// The MSG pack
    /// </summary>
    public const string MsgPack = "application/x-msgpack";

    /// <summary>
    /// The wire
    /// </summary>
    public const string Wire = "application/x-wire";

    /// <summary>
    /// The compressed
    /// </summary>
    public const string Compressed = "application/x-compressed";

    /// <summary>
    /// The net serializer
    /// </summary>
    public const string NetSerializer = "application/x-netserializer";

    /// <summary>
    /// The excel
    /// </summary>
    public const string Excel = "application/excel";

    /// <summary>
    /// The ms word
    /// </summary>
    public const string MsWord = "application/msword";

    /// <summary>
    /// The cert
    /// </summary>
    public const string Cert = "application/x-x509-ca-cert";

    /// <summary>
    /// The image PNG
    /// </summary>
    public const string ImagePng = "image/png";

    /// <summary>
    /// The image GIF
    /// </summary>
    public const string ImageGif = "image/gif";

    /// <summary>
    /// The image JPG
    /// </summary>
    public const string ImageJpg = "image/jpeg";

    /// <summary>
    /// The image SVG
    /// </summary>
    public const string ImageSvg = "image/svg+xml";

    /// <summary>
    /// The bson
    /// </summary>
    public const string Bson = "application/bson";

    /// <summary>
    /// The binary
    /// </summary>
    public const string Binary = "application/octet-stream";

    /// <summary>
    /// The server sent events
    /// </summary>
    public const string ServerSentEvents = "text/event-stream";

    /// <summary>
    /// Gets the extension.
    /// </summary>
    /// <param name="mimeType">Type of the MIME.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="System.NotSupportedException">Unknown mimeType: " + mimeType</exception>
    public static string GetExtension(string mimeType)
    {
        switch (mimeType)
        {
            case ProtoBuf:
                return ".pbuf";
        }

        var parts = mimeType.Split('/');
        if (parts.Length == 1) return "." + parts[0].LeftPart('+').LeftPart(';');
        if (parts.Length == 2) return "." + parts[1].LeftPart('+').LeftPart(';');

        throw new NotSupportedException("Unknown mimeType: " + mimeType);
    }

    //Lower cases and trims left part of content-type prior ';'
    /// <summary>
    /// Gets the type of the real content.
    /// </summary>
    /// <param name="contentType">Type of the content.</param>
    /// <returns>System.String.</returns>
    public static string GetRealContentType(string contentType)
    {
        if (contentType == null)
            return null;

        int start = -1, end = -1;

        for (int i = 0; i < contentType.Length; i++)
        {
            if (!char.IsWhiteSpace(contentType[i]))
            {
                if (contentType[i] == ';')
                    break;
                if (start == -1)
                {
                    start = i;
                }

                end = i;
            }
        }

        return start != -1 ? contentType.Substring(start, end - start + 1).ToLowerInvariant() : null;
    }

    /// <summary>
    /// Case-insensitive, trimmed compare of two content types from start to ';', i.e. without charset suffix
    /// </summary>
    /// <param name="contentType">Type of the content.</param>
    /// <param name="matchesContentType">Type of the matches content.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool MatchesContentType(string contentType, string matchesContentType)
    {
        if (contentType == null || matchesContentType == null)
            return false;

        int start = -1, matchStart = -1, matchEnd = -1;

        for (var i = 0; i < contentType.Length; i++)
        {
            if (char.IsWhiteSpace(contentType[i]))
                continue;
            start = i;
            break;
        }

        for (var i = 0; i < matchesContentType.Length; i++)
        {
            if (char.IsWhiteSpace(matchesContentType[i]))
                continue;
            if (matchesContentType[i] == ';')
                break;
            if (matchStart == -1)
                matchStart = i;
            matchEnd = i;
        }

        return start != -1 && matchStart != -1 && matchEnd != -1 && string.Compare(
                   contentType,
                   start,
                   matchesContentType,
                   matchStart,
                   matchEnd - matchStart + 1,
                   StringComparison.OrdinalIgnoreCase) == 0;
    }

    /// <summary>
    /// Gets or sets the is binary filter.
    /// </summary>
    /// <value>The is binary filter.</value>
    public static Func<string, bool?> IsBinaryFilter { get; set; }

    /// <summary>
    /// Determines whether the specified content type is binary.
    /// </summary>
    /// <param name="contentType">Type of the content.</param>
    /// <returns><c>true</c> if the specified content type is binary; otherwise, <c>false</c>.</returns>
    public static bool IsBinary(string contentType)
    {
        var userFilter = IsBinaryFilter?.Invoke(contentType);
        if (userFilter != null)
            return userFilter.Value;

        var realContentType = GetRealContentType(contentType);
        switch (realContentType)
        {
            case ProtoBuf:
            case MsgPack:
            case Binary:
            case Bson:
            case Wire:
            case Cert:
            case Excel:
            case MsWord:
            case Compressed:
            case WebAssembly:
            case Jar:
            case Dmg:
            case Pkg:
                return true;
        }

        // Text format exceptions to below heuristics
        switch (realContentType)
        {
            case ImageSvg:
                return false;
        }

        var primaryType = realContentType.LeftPart('/');
        var secondaryType = realContentType.RightPart('/');
        switch (primaryType)
        {
            case "image":
            case "audio":
            case "video":
                return true;
        }

        if (secondaryType.StartsWith("pkc") || secondaryType.StartsWith("x-pkc") || secondaryType.StartsWith("font")
            || secondaryType.StartsWith("vnd.ms-"))
            return true;

        return false;
    }

    /// <summary>
    /// Gets the type of the MIME.
    /// </summary>
    /// <param name="fileNameOrExt">The file name or ext.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="System.ArgumentNullException">fileNameOrExt</exception>
    public static string GetMimeType(string fileNameOrExt)
    {
        if (string.IsNullOrEmpty(fileNameOrExt))
            throw new ArgumentNullException(nameof(fileNameOrExt));

        var fileExt = fileNameOrExt.LastRightPart('.').ToLower();
        if (ExtensionMimeTypes.TryGetValue(fileExt, out var mimeType))
        {
            return mimeType;
        }

        switch (fileExt)
        {
            case "jpeg":
                return "image/jpeg";
            case "gif":
                return "image/gif";
            case "png":
                return "image/png";
            case "tiff":
                return "image/tiff";
            case "bmp":
                return "image/bmp";
            case "webp":
                return "image/webp";

            case "jpg":
                return "image/jpeg";

            case "tif":
                return "image/tiff";

            case "svg":
                return ImageSvg;

            case "ico":
                return "image/x-icon";

            case "htm":
            case "html":
            case "shtml":
                return "text/html";

            case "js":
                return "text/javascript";
            case "ts":
                return "text/typescript";
            case "jsx":
                return "text/jsx";

            case "csv":
                return Csv;
            case "css":
                return Css;

            case "cs":
                return "text/x-csharp";
            case "fs":
                return "text/x-fsharp";
            case "vb":
                return "text/x-vb";
            case "dart":
                return "application/dart";
            case "go":
                return "text/x-go";
            case "kt":
            case "kts":
                return "text/x-kotlin";
            case "java":
                return "text/x-java";
            case "py":
                return "text/x-python";
            case "groovy":
            case "gradle":
                return "text/x-groovy";

            case "yml":
            case "yaml":
                return YamlText;

            case "sh":
                return "text/x-sh";
            case "bat":
            case "cmd":
                return "application/bat";

            case "xml":
            case "csproj":
            case "fsproj":
            case "vbproj":
                return "text/xml";

            case "txt":
            case "ps1":
                return "text/plain";

            case "sgml":
                return "text/sgml";

            case "mp3":
                return "audio/mpeg3";

            case "au":
            case "snd":
                return "audio/basic";

            case "aac":
            case "ac3":
            case "aiff":
            case "m4a":
            case "m4b":
            case "m4p":
            case "mid":
            case "midi":
            case "wav":
                return "audio/" + fileExt;

            case "qt":
            case "mov":
                return "video/quicktime";

            case "mpg":
                return "video/mpeg";

            case "ogv":
                return "video/ogg";

            case "3gpp":
            case "avi":
            case "dv":
            case "divx":
            case "ogg":
            case "mp4":
            case "webm":
                return "video/" + fileExt;

            case "rtf":
                return "application/" + fileExt;

            case "xls":
            case "xlt":
            case "xla":
                return Excel;

            case "xlsx":
                return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            case "xltx":
                return "application/vnd.openxmlformats-officedocument.spreadsheetml.template";

            case "doc":
            case "dot":
                return MsWord;

            case "docx":
                return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            case "dotx":
                return "application/vnd.openxmlformats-officedocument.wordprocessingml.template";

            case "ppt":
            case "oit":
            case "pps":
            case "ppa":
                return "application/vnd.ms-powerpoint";

            case "pptx":
                return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
            case "potx":
                return "application/vnd.openxmlformats-officedocument.presentationml.template";
            case "ppsx":
                return "application/vnd.openxmlformats-officedocument.presentationml.slideshow";

            case "mdb":
                return "application/vnd.ms-access";

            case "cer":
            case "crt":
            case "der":
                return Cert;

            case "p10":
                return "application/pkcs10";
            case "p12":
                return "application/x-pkcs12";
            case "p7b":
            case "spc":
                return "application/x-pkcs7-certificates";
            case "p7c":
            case "p7m":
                return "application/pkcs7-mime";
            case "p7r":
                return "application/x-pkcs7-certreqresp";
            case "p7s":
                return "application/pkcs7-signature";
            case "sst":
                return "application/vnd.ms-pki.certstore";

            case "gz":
            case "tgz":
            case "zip":
            case "rar":
            case "lzh":
            case "z":
                return Compressed;

            case "eot":
                return "application/vnd.ms-fontobject";

            case "ttf":
                return "application/octet-stream";

            case "woff":
                return "application/font-woff";
            case "woff2":
                return "application/font-woff2";

            case "jar":
                return Jar;

            case "aaf":
            case "aca":
            case "asd":
            case "bin":
            case "cab":
            case "chm":
            case "class":
            case "cur":
            case "db":
            case "dat":
            case "deploy":
            case "dll":
            case "dsp":
            case "exe":
            case "fla":
            case "ics":
            case "inf":
            case "mix":
            case "msi":
            case "mso":
            case "obj":
            case "ocx":
            case "prm":
            case "prx":
            case "psd":
            case "psp":
            case "qxd":
            case "sea":
            case "snp":
            case "so":
            case "sqlite":
            case "toc":
            case "u32":
            case "xmp":
            case "xsn":
            case "xtp":
                return Binary;

            case "wasm":
                return WebAssembly;

            case "dmg":
                return Dmg;
            case "pkg":
                return Pkg;

            default:
                return "application/" + fileExt;
        }
    }
}