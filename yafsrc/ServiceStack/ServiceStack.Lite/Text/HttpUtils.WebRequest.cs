// ***********************************************************************
// <copyright file="HttpUtils.WebRequest.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#if !NET6_0_OR_GREATER

namespace ServiceStack
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using ServiceStack.Text;

    public static partial class HttpUtils
    {
        [ThreadStatic]
        public static IHttpResultsFilter ResultsFilter;

        public static string GetJsonFromUrl(
            this string url,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            return url.GetStringFromUrl(MimeTypes.Json, requestFilter, responseFilter);
        }

        public static string GetStringFromUrl(
            this string url,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string SendStringToUrl(
            this string url,
            string method = null,
            string requestBody = null,
            string contentType = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            var webReq = (HttpWebRequest) WebRequest.Create(url);
            if (method != null)
                webReq.Method = method;
            if (contentType != null)
                webReq.ContentType = contentType;

            webReq.Accept = accept;
            PclExport.Instance.AddCompression(webReq);

            requestFilter?.Invoke(webReq);

            if (ResultsFilter != null)
            {
                return ResultsFilter.GetString(webReq, requestBody);
            }

            if (requestBody != null)
            {
                using var reqStream = PclExport.Instance.GetRequestStream(webReq);
                using var writer = new StreamWriter(reqStream, UseEncoding);
                writer.Write(requestBody);
            }
            else if (method != null && HasRequestBody(method))
            {
                webReq.ContentLength = 0;
            }

            using var webRes = webReq.GetResponse();
            using var stream = webRes.GetResponseStream();
            responseFilter?.Invoke((HttpWebResponse) webRes);
            return stream.ReadToEnd(UseEncoding);
        }

        public static void SetRange(this HttpWebRequest request, long from, long? to)
        {
            if (to != null)
                request.AddRange(from, to.Value);
            else
                request.AddRange(from);
        }

        public static string GetHeader(this HttpWebRequest res, string name) => res.Headers.Get(name);

        public static string GetHeader(this HttpWebResponse res, string name) => res.Headers.Get(name);

        /// <summary>
        /// Populate HttpRequestMessage with a simpler, untyped API
        /// Syntax compatible with HttpClient's HttpRequestMessage
        /// </summary>
        public static HttpWebRequest With(this HttpWebRequest httpReq, Action<HttpRequestConfig> configure)
        {
            var config = new HttpRequestConfig();
            configure(config);

            if (config.Accept != null)
                httpReq.Accept = config.Accept;

            if (config.UserAgent != null)
                httpReq.UserAgent = config.UserAgent;

            if (config.ContentType != null)
                httpReq.ContentType = config.ContentType;

            if (config.Referer != null)
                httpReq.Referer = config.Referer;

            if (config.Authorization != null)
                httpReq.Headers[HttpHeaders.Authorization] =
                    config.Authorization.Name + " " + config.Authorization.Value;

            if (config.Range != null)
                httpReq.SetRange(config.Range.From, config.Range.To);

            if (config.Expect != null)
                httpReq.Expect = config.Expect;

            foreach (var entry in config.Headers)
            {
                httpReq.Headers[entry.Name] = entry.Value;
            }

            return httpReq;
        }
    }

    public interface IHttpResultsFilter : IDisposable
    {
        string GetString(HttpWebRequest webReq, string reqBody);

        byte[] GetBytes(HttpWebRequest webReq, byte[] reqBody);

        void UploadStream(HttpWebRequest webRequest, Stream fileStream, string fileName);
    }

    public class HttpResultsFilter : IHttpResultsFilter
    {
        private readonly IHttpResultsFilter previousFilter;

        public string StringResult { get; set; }

        public byte[] BytesResult { get; set; }

        public Func<HttpWebRequest, string, string> StringResultFn { get; set; }

        public Func<HttpWebRequest, byte[], byte[]> BytesResultFn { get; set; }

        public Action<HttpWebRequest, Stream, string> UploadFileFn { get; set; }

        public HttpResultsFilter(string stringResult = null, byte[] bytesResult = null)
        {
            StringResult = stringResult;
            BytesResult = bytesResult;

            previousFilter = HttpUtils.ResultsFilter;
            HttpUtils.ResultsFilter = this;
        }

        public void Dispose()
        {
            HttpUtils.ResultsFilter = previousFilter;
        }

        public string GetString(HttpWebRequest webReq, string reqBody)
        {
            return StringResultFn != null ? StringResultFn(webReq, reqBody) : StringResult;
        }

        public byte[] GetBytes(HttpWebRequest webReq, byte[] reqBody)
        {
            return BytesResultFn != null ? BytesResultFn(webReq, reqBody) : BytesResult;
        }

        public void UploadStream(HttpWebRequest webRequest, Stream fileStream, string fileName)
        {
            UploadFileFn?.Invoke(webRequest, fileStream, fileName);
        }
    }

    public static class HttpClientExt
    {
        /// <summary>
        /// Case-insensitive, trimmed compare of two content types from start to ';', i.e. without charset suffix 
        /// </summary>
        public static bool MatchesContentType(this HttpWebResponse res, string matchesContentType) =>
            MimeTypes.MatchesContentType(res.Headers[HttpHeaders.ContentType], matchesContentType);

        /// <summary>
        /// Returns null for unknown Content-length
        /// Syntax + Behavior compatible with HttpClient HttpResponseMessage 
        /// </summary>
        public static long? GetContentLength(this HttpWebResponse res) =>
            res.ContentLength == -1 ? null : res.ContentLength;
    }
}

#endif
