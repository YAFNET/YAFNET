// ***********************************************************************
// <copyright file="HttpUtils.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Text
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Class HttpUtils.
    /// </summary>
    public static class HttpUtils
    {
        /// <summary>
        /// The user agent
        /// </summary>
        public static string UserAgent = "ServiceStack.Text";

        /// <summary>
        /// Gets or sets the use encoding.
        /// </summary>
        /// <value>The use encoding.</value>
        public static Encoding UseEncoding { get; set; } = PclExport.Instance.GetUTF8Encoding();

        /// <summary>
        /// The results filter
        /// </summary>
        [ThreadStatic]
        public static IHttpResultsFilter ResultsFilter;

        /// <summary>
        /// Adds the query parameter.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="key">The key.</param>
        /// <param name="val">The value.</param>
        /// <param name="encode">if set to <c>true</c> [encode].</param>
        /// <returns>System.String.</returns>
        public static string AddQueryParam(this string url, string key, object val, bool encode = true)
        {
            return url.AddQueryParam(key, val?.ToString(), encode);
        }

        /// <summary>
        /// Adds the query parameter.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="key">The key.</param>
        /// <param name="val">The value.</param>
        /// <param name="encode">if set to <c>true</c> [encode].</param>
        /// <returns>System.String.</returns>
        public static string AddQueryParam(this string url, string key, string val, bool encode = true)
        {
            if (url == null)
                url = "";

            if (key == null || val == null)
                return url;

            var prefix = string.Empty;
            if (!url.EndsWith("?") && !url.EndsWith("&"))
            {
                prefix = url.IndexOf('?') == -1 ? "?" : "&";
            }
            return url + prefix + key + "=" + (encode ? val.UrlEncode() : val);
        }

        /// <summary>
        /// Sets the query parameter.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="key">The key.</param>
        /// <param name="val">The value.</param>
        /// <returns>System.String.</returns>
        public static string SetQueryParam(this string url, string key, string val)
        {
            if (url == null)
                url = "";

            if (key == null || val == null)
                return url;

            var qsPos = url.IndexOf('?');
            if (qsPos != -1)
            {
                var existingKeyPos = qsPos + 1 == url.IndexOf(key + "=", qsPos, StringComparison.Ordinal)
                    ? qsPos
                    : url.IndexOf("&" + key, qsPos, StringComparison.Ordinal);

                if (existingKeyPos != -1)
                {
                    var endPos = url.IndexOf('&', existingKeyPos + 1);
                    if (endPos == -1)
                        endPos = url.Length;

                    var newUrl = url.Substring(0, existingKeyPos + key.Length + 1)
                                 + "="
                                 + val.UrlEncode()
                                 + url.Substring(endPos);
                    return newUrl;
                }
            }
            var prefix = qsPos == -1 ? "?" : "&";
            return url + prefix + key + "=" + val.UrlEncode();
        }

        /// <summary>
        /// Adds the hash parameter.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="key">The key.</param>
        /// <param name="val">The value.</param>
        /// <returns>System.String.</returns>
        public static string AddHashParam(this string url, string key, object val)
        {
            return url.AddHashParam(key, val?.ToString());
        }

        /// <summary>
        /// Adds the hash parameter.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="key">The key.</param>
        /// <param name="val">The value.</param>
        /// <returns>System.String.</returns>
        public static string AddHashParam(this string url, string key, string val)
        {
            if (url == null)
                url = "";

            if (key == null || val == null)
                return url;

            var prefix = url.IndexOf('#') == -1 ? "#" : "/";
            return url + prefix + key + "=" + val.UrlEncode();
        }

        /// <summary>
        /// Sets the hash parameter.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="key">The key.</param>
        /// <param name="val">The value.</param>
        /// <returns>System.String.</returns>
        public static string SetHashParam(this string url, string key, string val)
        {
            if (url == null)
                url = "";

            if (key == null || val == null)
                return url;

            var hPos = url.IndexOf('#');
            if (hPos != -1)
            {
                var existingKeyPos = hPos + 1 == url.IndexOf(key + "=", hPos, PclExport.Instance.InvariantComparison)
                    ? hPos
                    : url.IndexOf("/" + key, hPos, PclExport.Instance.InvariantComparison);

                if (existingKeyPos != -1)
                {
                    var endPos = url.IndexOf('/', existingKeyPos + 1);
                    if (endPos == -1)
                        endPos = url.Length;

                    var newUrl = url.Substring(0, existingKeyPos + key.Length + 1)
                                 + "="
                                 + val.UrlEncode()
                                 + url.Substring(endPos);
                    return newUrl;
                }
            }
            var prefix = url.IndexOf('#') == -1 ? "#" : "/";
            return url + prefix + key + "=" + val.UrlEncode();
        }

        /// <summary>
        /// Determines whether [has request body] [the specified HTTP method].
        /// </summary>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <returns><c>true</c> if [has request body] [the specified HTTP method]; otherwise, <c>false</c>.</returns>
        public static bool HasRequestBody(string httpMethod)
        {
            switch (httpMethod)
            {
                case HttpMethods.Get:
                case HttpMethods.Delete:
                case HttpMethods.Head:
                case HttpMethods.Options:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the json from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string GetJsonFromUrl(this string url,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return url.GetStringFromUrl(MimeTypes.Json, requestFilter, responseFilter);
        }

        /// <summary>
        /// Gets the json from URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static Task<string> GetJsonFromUrlAsync(this string url,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            return url.GetStringFromUrlAsync(MimeTypes.Json, requestFilter, responseFilter, token);
        }

        /// <summary>
        /// Gets the string from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string GetStringFromUrl(this string url, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Gets the string from URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static Task<string> GetStringFromUrlAsync(this string url, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            return SendStringToUrlAsync(url, accept: accept, requestFilter: requestFilter, responseFilter: responseFilter, token: token);
        }

        /// <summary>
        /// Posts the json to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="json">The json.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PostJsonToUrl(this string url, string json,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, "POST", json, MimeTypes.Json, MimeTypes.Json,
                requestFilter, responseFilter);
        }

        /// <summary>
        /// Patches the json to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="json">The json.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PatchJsonToUrl(this string url, string json,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, "PATCH", json, MimeTypes.Json, MimeTypes.Json,
                requestFilter, responseFilter);
        }

        /// <summary>
        /// Sends the string to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="requestBody">The request body.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string SendStringToUrl(this string url, string method = null,
            string requestBody = null, string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            var webReq = (HttpWebRequest)WebRequest.Create(url);
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
            responseFilter?.Invoke((HttpWebResponse)webRes);
            return stream.ReadToEnd(UseEncoding);
        }

        /// <summary>
        /// Send string to URL as an asynchronous operation.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="requestBody">The request body.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task&lt;System.String&gt; representing the asynchronous operation.</returns>
        public static async Task<string> SendStringToUrlAsync(this string url, string method = null, string requestBody = null,
            string contentType = null, string accept = "*/*", Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            var webReq = (HttpWebRequest)WebRequest.Create(url);
            if (method != null)
                webReq.Method = method;
            if (contentType != null)
                webReq.ContentType = contentType;

            webReq.Accept = accept;
            PclExport.Instance.AddCompression(webReq);

            requestFilter?.Invoke(webReq);

            if (ResultsFilter != null)
            {
                var result = ResultsFilter.GetString(webReq, requestBody);
                return result;
            }

            if (requestBody != null)
            {
                using var reqStream = PclExport.Instance.GetRequestStream(webReq);
                using var writer = new StreamWriter(reqStream, UseEncoding);
                await writer.WriteAsync(requestBody).ConfigAwait();
            }

            using var webRes = await webReq.GetResponseAsync().ConfigAwait();
            responseFilter?.Invoke((HttpWebResponse)webRes);
            using var stream = webRes.GetResponseStream();
            return await stream.ReadToEndAsync().ConfigAwait();
        }

        /// <summary>
        /// Puts the bytes to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="requestBody">The request body.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] PutBytesToUrl(this string url, byte[] requestBody = null, string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendBytesToUrl(url, "PUT",
                contentType: contentType, requestBody: requestBody,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Sends the bytes to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="requestBody">The request body.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] SendBytesToUrl(this string url, string method = null,
            byte[] requestBody = null, string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            var webReq = (HttpWebRequest)WebRequest.Create(url);
            if (method != null)
                webReq.Method = method;

            if (contentType != null)
                webReq.ContentType = contentType;

            webReq.Accept = accept;
            PclExport.Instance.AddCompression(webReq);

            requestFilter?.Invoke(webReq);

            if (ResultsFilter != null)
            {
                return ResultsFilter.GetBytes(webReq, requestBody);
            }

            if (requestBody != null)
            {
                using var req = PclExport.Instance.GetRequestStream(webReq);
                req.Write(requestBody, 0, requestBody.Length);
            }

            using var webRes = PclExport.Instance.GetResponse(webReq);
            responseFilter?.Invoke((HttpWebResponse)webRes);

            using var stream = webRes.GetResponseStream();
            return stream.ReadFully();
        }

        /// <summary>
        /// Determines whether [is not found] [the specified ex].
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns><c>true</c> if [is not found] [the specified ex]; otherwise, <c>false</c>.</returns>
        public static bool IsNotFound(this Exception ex)
        {
            return GetStatus(ex) == HttpStatusCode.NotFound;
        }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns>System.Nullable&lt;HttpStatusCode&gt;.</returns>
        public static HttpStatusCode? GetStatus(this Exception ex)
        {
            if (ex == null)
                return null;

            if (ex is WebException webEx)
                return GetStatus(webEx);

            if (ex is IHasStatusCode hasStatus)
                return (HttpStatusCode)hasStatus.StatusCode;

            return null;
        }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <param name="webEx">The web ex.</param>
        /// <returns>System.Nullable&lt;HttpStatusCode&gt;.</returns>
        public static HttpStatusCode? GetStatus(this WebException webEx)
        {
            var httpRes = webEx?.Response as HttpWebResponse;
            return httpRes?.StatusCode;
        }

        /// <summary>
        /// Reads to end.
        /// </summary>
        /// <param name="webRes">The web resource.</param>
        /// <returns>System.String.</returns>
        public static string ReadToEnd(this WebResponse webRes)
        {
            using var stream = webRes.GetResponseStream();
            return stream.ReadToEnd(UseEncoding);
        }

        /// <summary>
        /// Reads to end asynchronous.
        /// </summary>
        /// <param name="webRes">The web resource.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static Task<string> ReadToEndAsync(this WebResponse webRes)
        {
            using var stream = webRes.GetResponseStream();
            return stream.ReadToEndAsync(UseEncoding);
        }
    }

    //Allow Exceptions to Customize HTTP StatusCode and StatusDescription returned
    /// <summary>
    /// Interface IHasStatusCode
    /// </summary>
    public interface IHasStatusCode
    {
        /// <summary>
        /// Gets the status code.
        /// </summary>
        /// <value>The status code.</value>
        int StatusCode { get; }
    }

    /// <summary>
    /// Interface IHttpResultsFilter
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IHttpResultsFilter : IDisposable
    {
        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="webReq">The web req.</param>
        /// <param name="reqBody">The req body.</param>
        /// <returns>System.String.</returns>
        string GetString(HttpWebRequest webReq, string reqBody);
        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <param name="webReq">The web req.</param>
        /// <param name="reqBody">The req body.</param>
        /// <returns>System.Byte[].</returns>
        byte[] GetBytes(HttpWebRequest webReq, byte[] reqBody);
        /// <summary>
        /// Uploads the stream.
        /// </summary>
        /// <param name="webRequest">The web request.</param>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="fileName">Name of the file.</param>
        void UploadStream(HttpWebRequest webRequest, Stream fileStream, string fileName);
    }

    /// <summary>
    /// Class HttpResultsFilter.
    /// Implements the <see cref="IHttpResultsFilter" />
    /// </summary>
    /// <seealso cref="IHttpResultsFilter" />
    public class HttpResultsFilter : IHttpResultsFilter
    {
        /// <summary>
        /// The previous filter
        /// </summary>
        private readonly IHttpResultsFilter previousFilter;

        /// <summary>
        /// Gets or sets the string result.
        /// </summary>
        /// <value>The string result.</value>
        public string StringResult { get; set; }
        /// <summary>
        /// Gets or sets the bytes result.
        /// </summary>
        /// <value>The bytes result.</value>
        public byte[] BytesResult { get; set; }

        /// <summary>
        /// Gets or sets the string result function.
        /// </summary>
        /// <value>The string result function.</value>
        public Func<HttpWebRequest, string, string> StringResultFn { get; set; }
        /// <summary>
        /// Gets or sets the bytes result function.
        /// </summary>
        /// <value>The bytes result function.</value>
        public Func<HttpWebRequest, byte[], byte[]> BytesResultFn { get; set; }
        /// <summary>
        /// Gets or sets the upload file function.
        /// </summary>
        /// <value>The upload file function.</value>
        public Action<HttpWebRequest, Stream, string> UploadFileFn { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResultsFilter"/> class.
        /// </summary>
        /// <param name="stringResult">The string result.</param>
        /// <param name="bytesResult">The bytes result.</param>
        public HttpResultsFilter(string stringResult = null, byte[] bytesResult = null)
        {
            StringResult = stringResult;
            BytesResult = bytesResult;

            previousFilter = HttpUtils.ResultsFilter;
            HttpUtils.ResultsFilter = this;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            HttpUtils.ResultsFilter = previousFilter;
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="webReq">The web req.</param>
        /// <param name="reqBody">The req body.</param>
        /// <returns>System.String.</returns>
        public string GetString(HttpWebRequest webReq, string reqBody)
        {
            return StringResultFn != null
                ? StringResultFn(webReq, reqBody)
                : StringResult;
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <param name="webReq">The web req.</param>
        /// <param name="reqBody">The req body.</param>
        /// <returns>System.Byte[].</returns>
        public byte[] GetBytes(HttpWebRequest webReq, byte[] reqBody)
        {
            return BytesResultFn != null
                ? BytesResultFn(webReq, reqBody)
                : BytesResult;
        }

        /// <summary>
        /// Uploads the stream.
        /// </summary>
        /// <param name="webRequest">The web request.</param>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="fileName">Name of the file.</param>
        public void UploadStream(HttpWebRequest webRequest, Stream fileStream, string fileName)
        {
            UploadFileFn?.Invoke(webRequest, fileStream, fileName);
        }
    }

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
            if (parts.Length == 1) return "." + parts[0];
            if (parts.Length == 2) return "." + parts[1];

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

            return start != -1
                ? contentType.Substring(start, end - start + 1).ToLowerInvariant()
                : null;
        }

        //Compares two string from start to ';' char, case-insensitive,
        //ignoring (trimming) spaces at start and end
        /// <summary>
        /// Matcheses the type of the content.
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

            return start != -1 && matchStart != -1 && matchEnd != -1
                   && string.Compare(contentType, start,
                       matchesContentType, matchStart, matchEnd - matchStart + 1,
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

            return secondaryType.StartsWith("pkc")
                   || secondaryType.StartsWith("x-pkc")
                   || secondaryType.StartsWith("font")
                   || secondaryType.StartsWith("vnd.ms-");
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

            var fileExt = fileNameOrExt.LastRightPart('.');
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

    /// <summary>
    /// Class HttpHeaders.
    /// </summary>
    public static class HttpHeaders
    {
        /// <summary>
        /// The x parameter override prefix
        /// </summary>
        public const string XParamOverridePrefix = "X-Param-Override-";

        /// <summary>
        /// The x HTTP method override
        /// </summary>
        public const string XHttpMethodOverride = "X-Http-Method-Override";

        /// <summary>
        /// The x automatic batch completed
        /// </summary>
        public const string XAutoBatchCompleted = "X-AutoBatch-Completed"; // How many requests were completed before first failure

        /// <summary>
        /// The x tag
        /// </summary>
        public const string XTag = "X-Tag";

        /// <summary>
        /// The x user authentication identifier
        /// </summary>
        public const string XUserAuthId = "X-UAId";

        /// <summary>
        /// The x trigger
        /// </summary>
        public const string XTrigger = "X-Trigger"; // Trigger Events on UserAgent

        /// <summary>
        /// The x forwarded for
        /// </summary>
        public const string XForwardedFor = "X-Forwarded-For"; // IP Address

        /// <summary>
        /// The x forwarded port
        /// </summary>
        public const string XForwardedPort = "X-Forwarded-Port";  // 80

        /// <summary>
        /// The x forwarded protocol
        /// </summary>
        public const string XForwardedProtocol = "X-Forwarded-Proto"; // http or https

        /// <summary>
        /// The x real ip
        /// </summary>
        public const string XRealIp = "X-Real-IP";

        /// <summary>
        /// The x location
        /// </summary>
        public const string XLocation = "X-Location";

        /// <summary>
        /// The x status
        /// </summary>
        public const string XStatus = "X-Status";

        /// <summary>
        /// The x powered by
        /// </summary>
        public const string XPoweredBy = "X-Powered-By";

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
        /// If unmodified since
        /// </summary>
        public const string IfUnmodifiedSince = "If-Unmodified-Since";

        /// <summary>
        /// If none match
        /// </summary>
        public const string IfNoneMatch = "If-None-Match";

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
        /// The WWW authenticate
        /// </summary>
        public const string WwwAuthenticate = "WWW-Authenticate";

        /// <summary>
        /// The allow origin
        /// </summary>
        public const string AllowOrigin = "Access-Control-Allow-Origin";

        /// <summary>
        /// The allow methods
        /// </summary>
        public const string AllowMethods = "Access-Control-Allow-Methods";

        /// <summary>
        /// The allow headers
        /// </summary>
        public const string AllowHeaders = "Access-Control-Allow-Headers";

        /// <summary>
        /// The allow credentials
        /// </summary>
        public const string AllowCredentials = "Access-Control-Allow-Credentials";

        /// <summary>
        /// The expose headers
        /// </summary>
        public const string ExposeHeaders = "Access-Control-Expose-Headers";

        /// <summary>
        /// The access control maximum age
        /// </summary>
        public const string AccessControlMaxAge = "Access-Control-Max-Age";

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
        /// The SOAP action
        /// </summary>
        public const string SOAPAction = "SOAPAction";

        /// <summary>
        /// The allow
        /// </summary>
        public const string Allow = "Allow";

        /// <summary>
        /// The accept charset
        /// </summary>
        public const string AcceptCharset = "Accept-Charset";

        /// <summary>
        /// The accept language
        /// </summary>
        public const string AcceptLanguage = "Accept-Language";

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
        /// The proxy authenticate
        /// </summary>
        public const string ProxyAuthenticate = "Proxy-Authenticate";

        /// <summary>
        /// The proxy authorization
        /// </summary>
        public const string ProxyAuthorization = "Proxy-Authorization";

        /// <summary>
        /// The proxy connection
        /// </summary>
        public const string ProxyConnection = "Proxy-Connection";

        /// <summary>
        /// The set cookie2
        /// </summary>
        public const string SetCookie2 = "Set-Cookie2";

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
        public static HashSet<string> RestrictedHeaders = new(StringComparer.OrdinalIgnoreCase)
        {
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
    /// Class HttpMethods.
    /// </summary>
    public static class HttpMethods
    {
        /// <summary>
        /// All verbs
        /// </summary>
        static readonly string[] allVerbs = {
            "OPTIONS", "GET", "HEAD", "POST", "PUT", "DELETE", "TRACE", "CONNECT", // RFC 2616
            "PROPFIND", "PROPPATCH", "MKCOL", "COPY", "MOVE", "LOCK", "UNLOCK",    // RFC 2518
            "VERSION-CONTROL", "REPORT", "CHECKOUT", "CHECKIN", "UNCHECKOUT",
            "MKWORKSPACE", "UPDATE", "LABEL", "MERGE", "BASELINE-CONTROL", "MKACTIVITY",  // RFC 3253
            "ORDERPATCH", // RFC 3648
            "ACL",        // RFC 3744
            "PATCH",      // https://datatracker.ietf.org/doc/draft-dusseault-http-patch/
            "SEARCH",     // https://datatracker.ietf.org/doc/draft-reschke-webdav-search/
            "BCOPY", "BDELETE", "BMOVE", "BPROPFIND", "BPROPPATCH", "NOTIFY",
            "POLL",  "SUBSCRIBE", "UNSUBSCRIBE" //MS Exchange WebDav: http://msdn.microsoft.com/en-us/library/aa142917.aspx
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
}