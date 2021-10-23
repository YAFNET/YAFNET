// ***********************************************************************
// <copyright file="SiteUtils.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace ServiceStack
{
    /// <summary>
    /// Class SiteUtils.
    /// </summary>
    public static class SiteUtils
    {
        /// <summary>
        /// The URL character checks
        /// </summary>
        private static char[] UrlCharChecks = { ':', '/', '(' };

        /// <summary>
        /// Allow slugs to capture URLs, Examples:
        /// techstacks.io                  =&gt; https://techstacks.io
        /// http:techstacks.io             =&gt; http://techstacks.io
        /// techstacks.io:1000             =&gt; https://techstacks.io:1000
        /// techstacks.io:1000:site1:site2 =&gt; https://techstacks.io:1000/site1/site2
        /// techstacks.io:site1%7Csite2    =&gt; https://techstacks.io/site1|site2
        /// techstacks.io:1000:site1:site2(a:1,b:"c,d",e:f) =&gt; https://techstacks.io:1000/site1/site2(a:1,b:"c,d",e:f)
        /// </summary>
        /// <param name="slug">The slug.</param>
        /// <returns>System.String.</returns>
        public static string UrlFromSlug(string slug)
        {
            var url = slug;
            var isUrl = url.StartsWith("https://") || url.StartsWith("http://");
            var scheme = !isUrl && (url.StartsWith("http:") || url.StartsWith("https:"))
                ? url.LeftPart(':')
                : null;
            if (scheme != null)
                url = url.RightPart(':');

            var firstPos = url.IndexOf(':');
            if (!isUrl && firstPos >= 0)
            {
                var isColonPos = url.IndexOfAny(UrlCharChecks);
                if (isColonPos >= 0 && url[isColonPos] == ':')
                {
                    var atPort = url.RightPart(':');
                    if (atPort.Length > 0)
                    {
                        var delim1Pos = atPort.IndexOf(':');
                        var delim2Pos = atPort.IndexOf('/');
                        var endPos = delim1Pos >= 0 && delim2Pos >= 0
                            ? Math.Min(delim1Pos, delim2Pos)
                            : Math.Max(delim1Pos, delim2Pos);
                        var testPort = endPos >= 0
                            ? atPort.Substring(0, endPos)
                            : atPort.Substring(0, atPort.Length - 1);
                        url = int.TryParse(testPort, out _)
                            ? url.LeftPart(':') + ':' + UnSlash(atPort)
                            : url.LeftPart(':') + '/' + UnSlash(atPort);
                    }
                    else
                    {
                        url = url.LeftPart(':') + '/' + UnSlash(atPort);
                    }
                }
            }
            url = url.UrlDecode();
            if (!isUrl)
            {
                url = scheme != null
                    ? scheme + "://" + url
                    : "https://" + url;
            }
            return url;
        }

        /// <summary>
        /// Uns the slash.
        /// </summary>
        /// <param name="urlComponent">The URL component.</param>
        /// <returns>System.String.</returns>
        private static string UnSlash(string urlComponent)
        {
            // don't replace ':' after '('...)
            if (urlComponent.IndexOf('(') >= 0)
            {
                var target = urlComponent.LeftPart('(');
                var suffix = urlComponent.RightPart('(');
                return target.Replace(':', '/') + '(' + suffix;
            }
            return urlComponent.Replace(':', '/');
        }

        /// <summary>
        /// Convert URL to URL-friendly slugs, Examples:
        /// https://techstacks.io                  =&gt; techstacks.io
        /// http://techstacks.io                   =&gt; http:techstacks.io
        /// https://techstacks.io:1000             =&gt; techstacks.io:1000
        /// https://techstacks.io:1000/site1/site2 =&gt; techstacks.io:1000:site1:site2
        /// https://techstacks.io/site1|site2      =&gt; techstacks.io:site|site2
        /// https://techstacks.io:1000/site1/site2(a:1,b:"c,d",e:f) =&gt; techstacks.io:1000:site1:site2(a:1,b:"c,d",e:f)
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>System.String.</returns>
        public static string UrlToSlug(string url)
        {
            var slug = url;
            if (slug.StartsWith("https://"))
                slug = slug.Substring("https://".Length);
            else if (slug.StartsWith("http://"))
                slug = "http:" + slug.Substring("http://".Length);
            slug = slug.Replace('/', ':');
            return slug;
        }

        /// <summary>
        /// Converts to urlencoded.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.ArgumentException">Invalid odd number of arguments, expected [key1,value1,key2,value2,...] - args</exception>
        public static string ToUrlEncoded(List<string> args)
        {
            if (!args.IsEmpty())
            {
                if (args.Count % 2 != 0)
                    throw new ArgumentException(@"Invalid odd number of arguments, expected [key1,value1,key2,value2,...]", nameof(args));

                var sb = StringBuilderCache.Allocate();
                for (var i = 0; i < args.Count; i += 2)
                {
                    if (sb.Length > 0)
                        sb.Append('&');

                    var key = args[i];
                    var val = args[i + 1];
                    val = val?.Replace((char)31, ','); // 31 1F US (unit separator) 
                    sb.Append(key).Append('=').Append(val.UrlEncode());
                }
                return StringBuilderCache.ReturnAndFree(sb);
            }
            return string.Empty;
        }

        /// <summary>
        /// Get application metadata as an asynchronous operation.
        /// </summary>
        /// <param name="baseUrl">The base URL.</param>
        /// <returns>A Task&lt;AppMetadata&gt; representing the asynchronous operation.</returns>
        /// <exception cref="System.Exception">Not a remote ServiceStack Instance</exception>
        /// <exception cref="System.Exception">Not a remote ServiceStack Instance</exception>
        /// <exception cref="System.Exception">Not a remote ServiceStack Instance</exception>
        /// <exception cref="System.Exception">ServiceStack Instance v5.10 or higher required</exception>
        /// <exception cref="System.Exception">Could not read AppMetadata, try upgrading this App or remote ServiceStack Instance</exception>
        public static async Task<AppMetadata> GetAppMetadataAsync(this string baseUrl)
        {
            string appResponseJson = null;
            try
            {
                appResponseJson = await baseUrl.CombineWith("/metadata/app.json")
                    .GetJsonFromUrlAsync();

                if (!appResponseJson.Trim().StartsWith("{"))
                    throw new Exception("Not a remote ServiceStack Instance");
            }
            catch (Exception appEx)
            {
                string ssMetadata;
                try
                {
                    ssMetadata = await baseUrl.CombineWith("/metadata")
                        .GetStringFromUrlAsync(requestFilter: req => req.UserAgent = "ServiceStack");
                }
                catch (Exception ssEx)
                {
                    throw new Exception("Not a remote ServiceStack Instance", ssEx);
                }

                if (ssMetadata.IndexOf("https://servicestack.net", StringComparison.Ordinal) == -1)
                    throw new Exception("Not a remote ServiceStack Instance");

                throw new Exception("ServiceStack Instance v5.10 or higher required", appEx);
            }

            AppMetadata appMetadata;
            try
            {
                appMetadata = appResponseJson.FromJson<AppMetadata>();
            }
            catch (Exception e)
            {
                throw new Exception("Could not read AppMetadata, try upgrading this App or remote ServiceStack Instance", e);
            }

            return appMetadata;
        }
    }
}