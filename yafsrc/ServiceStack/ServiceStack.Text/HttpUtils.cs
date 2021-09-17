// ***********************************************************************
// <copyright file="HttpUtils.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace ServiceStack
{
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
        public static Encoding UseEncoding { get; set; } = PclExport.Instance.GetUTF8Encoding(false);

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
        public static string AddQueryParam(this string url, object key, string val, bool encode = true)
        {
            return AddQueryParam(url, key?.ToString(), val, encode);
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
            return url.GetStringFromUrlAsync(MimeTypes.Json, requestFilter, responseFilter, token: token);
        }

        /// <summary>
        /// Gets the XML from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string GetXmlFromUrl(this string url,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return url.GetStringFromUrl(MimeTypes.Xml, requestFilter, responseFilter);
        }

        /// <summary>
        /// Gets the XML from URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static Task<string> GetXmlFromUrlAsync(this string url,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            return url.GetStringFromUrlAsync(MimeTypes.Xml, requestFilter, responseFilter, token: token);
        }

        /// <summary>
        /// Gets the CSV from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string GetCsvFromUrl(this string url,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return url.GetStringFromUrl(MimeTypes.Csv, requestFilter, responseFilter);
        }

        /// <summary>
        /// Gets the CSV from URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static Task<string> GetCsvFromUrlAsync(this string url,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            return url.GetStringFromUrlAsync(MimeTypes.Csv, requestFilter, responseFilter, token: token);
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
        /// Posts the string to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="requestBody">The request body.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PostStringToUrl(this string url, string requestBody = null,
            string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "POST",
                requestBody: requestBody, contentType: contentType,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Posts the string to URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="requestBody">The request body.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static Task<string> PostStringToUrlAsync(this string url, string requestBody = null,
            string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            return SendStringToUrlAsync(url, method: "POST",
                requestBody: requestBody, contentType: contentType,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter, token: token);
        }

        /// <summary>
        /// Posts to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="formData">The form data.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PostToUrl(this string url, string formData = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "POST",
                contentType: MimeTypes.FormUrlEncoded, requestBody: formData,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Posts to URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="formData">The form data.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static Task<string> PostToUrlAsync(this string url, string formData = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            return SendStringToUrlAsync(url, method: "POST",
                contentType: MimeTypes.FormUrlEncoded, requestBody: formData,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter, token: token);
        }

        /// <summary>
        /// Posts to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="formData">The form data.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PostToUrl(this string url, object formData = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            string postFormData = formData != null ? QueryStringSerializer.SerializeToString(formData) : null;

            return SendStringToUrl(url, method: "POST",
                contentType: MimeTypes.FormUrlEncoded, requestBody: postFormData,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Posts to URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="formData">The form data.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static Task<string> PostToUrlAsync(this string url, object formData = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            string postFormData = formData != null ? QueryStringSerializer.SerializeToString(formData) : null;

            return SendStringToUrlAsync(url, method: "POST",
                contentType: MimeTypes.FormUrlEncoded, requestBody: postFormData,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter, token: token);
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
            return SendStringToUrl(url, method: "POST", requestBody: json, contentType: MimeTypes.Json, accept: MimeTypes.Json,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Posts the json to URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="json">The json.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static Task<string> PostJsonToUrlAsync(this string url, string json,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            return SendStringToUrlAsync(url, method: "POST", requestBody: json, contentType: MimeTypes.Json, accept: MimeTypes.Json,
                requestFilter: requestFilter, responseFilter: responseFilter, token: token);
        }

        /// <summary>
        /// Posts the json to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="data">The data.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PostJsonToUrl(this string url, object data,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "POST", requestBody: data.ToJson(), contentType: MimeTypes.Json, accept: MimeTypes.Json,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Posts the json to URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="data">The data.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static Task<string> PostJsonToUrlAsync(this string url, object data,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            return SendStringToUrlAsync(url, method: "POST", requestBody: data.ToJson(), contentType: MimeTypes.Json, accept: MimeTypes.Json,
                requestFilter: requestFilter, responseFilter: responseFilter, token: token);
        }

        /// <summary>
        /// Posts the XML to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="xml">The XML.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PostXmlToUrl(this string url, string xml,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "POST", requestBody: xml, contentType: MimeTypes.Xml, accept: MimeTypes.Xml,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Posts the XML to URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="xml">The XML.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static Task<string> PostXmlToUrlAsync(this string url, string xml,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            return SendStringToUrlAsync(url, method: "POST", requestBody: xml, contentType: MimeTypes.Xml, accept: MimeTypes.Xml,
                requestFilter: requestFilter, responseFilter: responseFilter, token: token);
        }

        /// <summary>
        /// Posts the CSV to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="csv">The CSV.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PostCsvToUrl(this string url, string csv,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "POST", requestBody: csv, contentType: MimeTypes.Csv, accept: MimeTypes.Csv,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Posts the CSV to URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="csv">The CSV.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static Task<string> PostCsvToUrlAsync(this string url, string csv,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            return SendStringToUrlAsync(url, method: "POST", requestBody: csv, contentType: MimeTypes.Csv, accept: MimeTypes.Csv,
                requestFilter: requestFilter, responseFilter: responseFilter, token: token);
        }

        /// <summary>
        /// Puts the string to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="requestBody">The request body.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PutStringToUrl(this string url, string requestBody = null,
            string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "PUT",
                requestBody: requestBody, contentType: contentType,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Puts the string to URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="requestBody">The request body.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static Task<string> PutStringToUrlAsync(this string url, string requestBody = null,
            string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            return SendStringToUrlAsync(url, method: "PUT",
                requestBody: requestBody, contentType: contentType,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter, token: token);
        }

        /// <summary>
        /// Puts to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="formData">The form data.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PutToUrl(this string url, string formData = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "PUT",
                contentType: MimeTypes.FormUrlEncoded, requestBody: formData,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Puts to URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="formData">The form data.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static Task<string> PutToUrlAsync(this string url, string formData = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            return SendStringToUrlAsync(url, method: "PUT",
                contentType: MimeTypes.FormUrlEncoded, requestBody: formData,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter, token: token);
        }

        /// <summary>
        /// Puts to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="formData">The form data.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PutToUrl(this string url, object formData = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            string postFormData = formData != null ? QueryStringSerializer.SerializeToString(formData) : null;

            return SendStringToUrl(url, method: "PUT",
                contentType: MimeTypes.FormUrlEncoded, requestBody: postFormData,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Puts to URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="formData">The form data.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static Task<string> PutToUrlAsync(this string url, object formData = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            string postFormData = formData != null ? QueryStringSerializer.SerializeToString(formData) : null;

            return SendStringToUrlAsync(url, method: "PUT",
                contentType: MimeTypes.FormUrlEncoded, requestBody: postFormData,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter, token: token);
        }

        /// <summary>
        /// Puts the json to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="json">The json.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PutJsonToUrl(this string url, string json,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "PUT", requestBody: json, contentType: MimeTypes.Json, accept: MimeTypes.Json,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Puts the json to URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="json">The json.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static Task<string> PutJsonToUrlAsync(this string url, string json,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            return SendStringToUrlAsync(url, method: "PUT", requestBody: json, contentType: MimeTypes.Json, accept: MimeTypes.Json,
                requestFilter: requestFilter, responseFilter: responseFilter, token: token);
        }

        /// <summary>
        /// Puts the json to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="data">The data.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PutJsonToUrl(this string url, object data,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "PUT", requestBody: data.ToJson(), contentType: MimeTypes.Json, accept: MimeTypes.Json,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Puts the json to URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="data">The data.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static Task<string> PutJsonToUrlAsync(this string url, object data,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            return SendStringToUrlAsync(url, method: "PUT", requestBody: data.ToJson(), contentType: MimeTypes.Json, accept: MimeTypes.Json,
                requestFilter: requestFilter, responseFilter: responseFilter, token: token);
        }

        /// <summary>
        /// Puts the XML to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="xml">The XML.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PutXmlToUrl(this string url, string xml,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "PUT", requestBody: xml, contentType: MimeTypes.Xml, accept: MimeTypes.Xml,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Puts the XML to URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="xml">The XML.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static Task<string> PutXmlToUrlAsync(this string url, string xml,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            return SendStringToUrlAsync(url, method: "PUT", requestBody: xml, contentType: MimeTypes.Xml, accept: MimeTypes.Xml,
                requestFilter: requestFilter, responseFilter: responseFilter, token: token);
        }

        /// <summary>
        /// Puts the CSV to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="csv">The CSV.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PutCsvToUrl(this string url, string csv,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "PUT", requestBody: csv, contentType: MimeTypes.Csv, accept: MimeTypes.Csv,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Puts the CSV to URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="csv">The CSV.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static Task<string> PutCsvToUrlAsync(this string url, string csv,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            return SendStringToUrlAsync(url, method: "PUT", requestBody: csv, contentType: MimeTypes.Csv, accept: MimeTypes.Csv,
                requestFilter: requestFilter, responseFilter: responseFilter, token: token);
        }

        /// <summary>
        /// Patches the string to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="requestBody">The request body.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PatchStringToUrl(this string url, string requestBody = null,
            string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "PATCH",
                requestBody: requestBody, contentType: contentType,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Patches the string to URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="requestBody">The request body.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static Task<string> PatchStringToUrlAsync(this string url, string requestBody = null,
            string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            return SendStringToUrlAsync(url, method: "PATCH",
                requestBody: requestBody, contentType: contentType,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter, token: token);
        }

        /// <summary>
        /// Patches to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="formData">The form data.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PatchToUrl(this string url, string formData = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "PATCH",
                contentType: MimeTypes.FormUrlEncoded, requestBody: formData,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Patches to URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="formData">The form data.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static Task<string> PatchToUrlAsync(this string url, string formData = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            return SendStringToUrlAsync(url, method: "PATCH",
                contentType: MimeTypes.FormUrlEncoded, requestBody: formData,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter, token: token);
        }

        /// <summary>
        /// Patches to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="formData">The form data.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PatchToUrl(this string url, object formData = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            string postFormData = formData != null ? QueryStringSerializer.SerializeToString(formData) : null;

            return SendStringToUrl(url, method: "PATCH",
                contentType: MimeTypes.FormUrlEncoded, requestBody: postFormData,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Patches to URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="formData">The form data.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static Task<string> PatchToUrlAsync(this string url, object formData = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            string postFormData = formData != null ? QueryStringSerializer.SerializeToString(formData) : null;

            return SendStringToUrlAsync(url, method: "PATCH",
                contentType: MimeTypes.FormUrlEncoded, requestBody: postFormData,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter, token: token);
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
            return SendStringToUrl(url, method: "PATCH", requestBody: json, contentType: MimeTypes.Json, accept: MimeTypes.Json,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Patches the json to URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="json">The json.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static Task<string> PatchJsonToUrlAsync(this string url, string json,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            return SendStringToUrlAsync(url, method: "PATCH", requestBody: json, contentType: MimeTypes.Json, accept: MimeTypes.Json,
                requestFilter: requestFilter, responseFilter: responseFilter, token: token);
        }

        /// <summary>
        /// Patches the json to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="data">The data.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PatchJsonToUrl(this string url, object data,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "PATCH", requestBody: data.ToJson(), contentType: MimeTypes.Json, accept: MimeTypes.Json,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Patches the json to URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="data">The data.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static Task<string> PatchJsonToUrlAsync(this string url, object data,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            return SendStringToUrlAsync(url, method: "PATCH", requestBody: data.ToJson(), contentType: MimeTypes.Json, accept: MimeTypes.Json,
                requestFilter: requestFilter, responseFilter: responseFilter, token: token);
        }

        /// <summary>
        /// Deletes from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string DeleteFromUrl(this string url, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "DELETE", accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Deletes from URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static Task<string> DeleteFromUrlAsync(this string url, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            return SendStringToUrlAsync(url, method: "DELETE", accept: accept, requestFilter: requestFilter, responseFilter: responseFilter, token: token);
        }

        /// <summary>
        /// Optionses from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string OptionsFromUrl(this string url, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "OPTIONS", accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Optionses from URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static Task<string> OptionsFromUrlAsync(this string url, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            return SendStringToUrlAsync(url, method: "OPTIONS", accept: accept, requestFilter: requestFilter, responseFilter: responseFilter, token: token);
        }

        /// <summary>
        /// Heads from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string HeadFromUrl(this string url, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "HEAD", accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Heads from URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static Task<string> HeadFromUrlAsync(this string url, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            return SendStringToUrlAsync(url, method: "HEAD", accept: accept, requestFilter: requestFilter, responseFilter: responseFilter, token: token);
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
        /// Gets the bytes from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] GetBytesFromUrl(this string url, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return url.SendBytesToUrl(accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Gets the bytes from URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Byte[]&gt;.</returns>
        public static Task<byte[]> GetBytesFromUrlAsync(this string url, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            return url.SendBytesToUrlAsync(accept: accept, requestFilter: requestFilter, responseFilter: responseFilter, token: token);
        }

        /// <summary>
        /// Posts the bytes to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="requestBody">The request body.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] PostBytesToUrl(this string url, byte[] requestBody = null, string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendBytesToUrl(url, method: "POST",
                contentType: contentType, requestBody: requestBody,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Posts the bytes to URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="requestBody">The request body.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Byte[]&gt;.</returns>
        public static Task<byte[]> PostBytesToUrlAsync(this string url, byte[] requestBody = null, string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            return SendBytesToUrlAsync(url, method: "POST",
                contentType: contentType, requestBody: requestBody,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter, token: token);
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
            return SendBytesToUrl(url, method: "PUT",
                contentType: contentType, requestBody: requestBody,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Puts the bytes to URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="requestBody">The request body.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Byte[]&gt;.</returns>
        public static Task<byte[]> PutBytesToUrlAsync(this string url, byte[] requestBody = null, string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            return SendBytesToUrlAsync(url, method: "PUT",
                contentType: contentType, requestBody: requestBody,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter, token: token);
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
        /// Send bytes to URL as an asynchronous operation.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="requestBody">The request body.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task&lt;System.Byte[]&gt; representing the asynchronous operation.</returns>
        public static async Task<byte[]> SendBytesToUrlAsync(this string url, string method = null,
            byte[] requestBody = null, string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
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
                var result = ResultsFilter.GetBytes(webReq, requestBody);
                return result;
            }

            if (requestBody != null)
            {
                using var req = PclExport.Instance.GetRequestStream(webReq);
                await req.WriteAsync(requestBody, 0, requestBody.Length, token).ConfigAwait();
            }

            var webRes = await webReq.GetResponseAsync().ConfigAwait();
            responseFilter?.Invoke((HttpWebResponse)webRes);

            using var stream = webRes.GetResponseStream();
            return await stream.ReadFullyAsync(token).ConfigAwait();
        }

        /// <summary>
        /// Gets the stream from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>Stream.</returns>
        public static Stream GetStreamFromUrl(this string url, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return url.SendStreamToUrl(accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Gets the stream from URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;Stream&gt;.</returns>
        public static Task<Stream> GetStreamFromUrlAsync(this string url, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            return url.SendStreamToUrlAsync(accept: accept, requestFilter: requestFilter, responseFilter: responseFilter, token: token);
        }

        /// <summary>
        /// Posts the stream to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="requestBody">The request body.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>Stream.</returns>
        public static Stream PostStreamToUrl(this string url, Stream requestBody = null, string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStreamToUrl(url, method: "POST",
                contentType: contentType, requestBody: requestBody,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Posts the stream to URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="requestBody">The request body.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;Stream&gt;.</returns>
        public static Task<Stream> PostStreamToUrlAsync(this string url, Stream requestBody = null, string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            return SendStreamToUrlAsync(url, method: "POST",
                contentType: contentType, requestBody: requestBody,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter, token: token);
        }

        /// <summary>
        /// Puts the stream to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="requestBody">The request body.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>Stream.</returns>
        public static Stream PutStreamToUrl(this string url, Stream requestBody = null, string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStreamToUrl(url, method: "PUT",
                contentType: contentType, requestBody: requestBody,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Puts the stream to URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="requestBody">The request body.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;Stream&gt;.</returns>
        public static Task<Stream> PutStreamToUrlAsync(this string url, Stream requestBody = null, string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
        {
            return SendStreamToUrlAsync(url, method: "PUT",
                contentType: contentType, requestBody: requestBody,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter, token: token);
        }

        /// <summary>
        /// Returns HttpWebResponse Stream which must be disposed
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="requestBody">The request body.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>Stream.</returns>
        public static Stream SendStreamToUrl(this string url, string method = null,
            Stream requestBody = null, string contentType = null, string accept = "*/*",
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
                return new MemoryStream(ResultsFilter.GetBytes(webReq, requestBody.ReadFully()));
            }

            if (requestBody != null)
            {
                using (var req = PclExport.Instance.GetRequestStream(webReq))
                {
                    requestBody.CopyTo(req);
                }
            }

            var webRes = PclExport.Instance.GetResponse(webReq);
            responseFilter?.Invoke((HttpWebResponse)webRes);

            var stream = webRes.GetResponseStream();
            return stream;
        }

        /// <summary>
        /// Returns HttpWebResponse Stream which must be disposed
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="requestBody">The request body.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task&lt;Stream&gt; representing the asynchronous operation.</returns>
        public static async Task<Stream> SendStreamToUrlAsync(this string url, string method = null,
            Stream requestBody = null, string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null, CancellationToken token = default)
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
                return new MemoryStream(ResultsFilter.GetBytes(webReq, await requestBody.ReadFullyAsync(token).ConfigAwait()));
            }

            if (requestBody != null)
            {
                using var req = PclExport.Instance.GetRequestStream(webReq);
                await requestBody.CopyToAsync(req, token).ConfigAwait();
            }

            var webRes = await webReq.GetResponseAsync().ConfigAwait();
            responseFilter?.Invoke((HttpWebResponse)webRes);

            var stream = webRes.GetResponseStream();
            return stream;
        }

        /// <summary>
        /// Determines whether the specified ex is any300.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns><c>true</c> if the specified ex is any300; otherwise, <c>false</c>.</returns>
        public static bool IsAny300(this Exception ex)
        {
            var status = ex.GetStatus();
            return status >= HttpStatusCode.MultipleChoices && status < HttpStatusCode.BadRequest;
        }

        /// <summary>
        /// Determines whether the specified ex is any400.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns><c>true</c> if the specified ex is any400; otherwise, <c>false</c>.</returns>
        public static bool IsAny400(this Exception ex)
        {
            var status = ex.GetStatus();
            return status >= HttpStatusCode.BadRequest && status < HttpStatusCode.InternalServerError;
        }

        /// <summary>
        /// Determines whether the specified ex is any500.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns><c>true</c> if the specified ex is any500; otherwise, <c>false</c>.</returns>
        public static bool IsAny500(this Exception ex)
        {
            var status = ex.GetStatus();
            return status >= HttpStatusCode.InternalServerError && (int)status < 600;
        }

        /// <summary>
        /// Determines whether [is not modified] [the specified ex].
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns><c>true</c> if [is not modified] [the specified ex]; otherwise, <c>false</c>.</returns>
        public static bool IsNotModified(this Exception ex)
        {
            return GetStatus(ex) == HttpStatusCode.NotModified;
        }

        /// <summary>
        /// Determines whether [is bad request] [the specified ex].
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns><c>true</c> if [is bad request] [the specified ex]; otherwise, <c>false</c>.</returns>
        public static bool IsBadRequest(this Exception ex)
        {
            return GetStatus(ex) == HttpStatusCode.BadRequest;
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
        /// Determines whether the specified ex is unauthorized.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns><c>true</c> if the specified ex is unauthorized; otherwise, <c>false</c>.</returns>
        public static bool IsUnauthorized(this Exception ex)
        {
            return GetStatus(ex) == HttpStatusCode.Unauthorized;
        }

        /// <summary>
        /// Determines whether the specified ex is forbidden.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns><c>true</c> if the specified ex is forbidden; otherwise, <c>false</c>.</returns>
        public static bool IsForbidden(this Exception ex)
        {
            return GetStatus(ex) == HttpStatusCode.Forbidden;
        }

        /// <summary>
        /// Determines whether [is internal server error] [the specified ex].
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns><c>true</c> if [is internal server error] [the specified ex]; otherwise, <c>false</c>.</returns>
        public static bool IsInternalServerError(this Exception ex)
        {
            return GetStatus(ex) == HttpStatusCode.InternalServerError;
        }

        /// <summary>
        /// Gets the response status.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>System.Nullable&lt;HttpStatusCode&gt;.</returns>
        public static HttpStatusCode? GetResponseStatus(this string url)
        {
            try
            {
                var webReq = (HttpWebRequest)WebRequest.Create(url);
                using (var webRes = PclExport.Instance.GetResponse(webReq))
                {
                    var httpRes = webRes as HttpWebResponse;
                    return httpRes?.StatusCode;
                }
            }
            catch (Exception ex)
            {
                return ex.GetStatus();
            }
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
        /// Determines whether the specified status code has status.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="statusCode">The status code.</param>
        /// <returns><c>true</c> if the specified status code has status; otherwise, <c>false</c>.</returns>
        public static bool HasStatus(this Exception ex, HttpStatusCode statusCode)
        {
            return GetStatus(ex) == statusCode;
        }

        /// <summary>
        /// Gets the response body.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns>System.String.</returns>
        public static string GetResponseBody(this Exception ex)
        {
            if (!(ex is WebException webEx) || webEx.Response == null || webEx.Status != WebExceptionStatus.ProtocolError)
                return null;

            var errorResponse = (HttpWebResponse)webEx.Response;
            using var responseStream = errorResponse.GetResponseStream();
            return responseStream.ReadToEnd(UseEncoding);
        }

        /// <summary>
        /// Get response body as an asynchronous operation.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task&lt;System.String&gt; representing the asynchronous operation.</returns>
        public static async Task<string> GetResponseBodyAsync(this Exception ex, CancellationToken token = default)
        {
            if (!(ex is WebException webEx) || webEx.Response == null || webEx.Status != WebExceptionStatus.ProtocolError)
                return null;

            var errorResponse = (HttpWebResponse)webEx.Response;
            using var responseStream = errorResponse.GetResponseStream();
            return await responseStream.ReadToEndAsync(UseEncoding).ConfigAwait();
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

        /// <summary>
        /// Reads the lines.
        /// </summary>
        /// <param name="webRes">The web resource.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public static IEnumerable<string> ReadLines(this WebResponse webRes)
        {
            using var stream = webRes.GetResponseStream();
            using var reader = new StreamReader(stream, UseEncoding, true, 1024, leaveOpen: true);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }

        /// <summary>
        /// Gets the error response.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>HttpWebResponse.</returns>
        public static HttpWebResponse GetErrorResponse(this string url)
        {
            try
            {
                var webReq = WebRequest.Create(url);
                using var webRes = PclExport.Instance.GetResponse(webReq);
                webRes.ReadToEnd();
                return null;
            }
            catch (WebException webEx)
            {
                return (HttpWebResponse)webEx.Response;
            }
        }

        /// <summary>
        /// Get error response as an asynchronous operation.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>A Task&lt;HttpWebResponse&gt; representing the asynchronous operation.</returns>
        public static async Task<HttpWebResponse> GetErrorResponseAsync(this string url)
        {
            try
            {
                var webReq = WebRequest.Create(url);
                using var webRes = await webReq.GetResponseAsync().ConfigAwait();
                await webRes.ReadToEndAsync().ConfigAwait();
                return null;
            }
            catch (WebException webEx)
            {
                return (HttpWebResponse)webEx.Response;
            }
        }

        /// <summary>
        /// Gets the request stream asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Task&lt;Stream&gt;.</returns>
        public static Task<Stream> GetRequestStreamAsync(this WebRequest request)
        {
            return GetRequestStreamAsync((HttpWebRequest)request);
        }

        /// <summary>
        /// Gets the request stream asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Task&lt;Stream&gt;.</returns>
        public static Task<Stream> GetRequestStreamAsync(this HttpWebRequest request)
        {
            var tcs = new TaskCompletionSource<Stream>();

            try
            {
                request.BeginGetRequestStream(iar =>
                {
                    try
                    {
                        var response = request.EndGetRequestStream(iar);
                        tcs.SetResult(response);
                    }
                    catch (Exception exc)
                    {
                        tcs.SetException(exc);
                    }
                }, null);
            }
            catch (Exception exc)
            {
                tcs.SetException(exc);
            }

            return tcs.Task;
        }

        /// <summary>
        /// Converts to.
        /// </summary>
        /// <typeparam name="TDerived">The type of the t derived.</typeparam>
        /// <typeparam name="TBase">The type of the t base.</typeparam>
        /// <param name="task">The task.</param>
        /// <returns>Task&lt;TBase&gt;.</returns>
        public static Task<TBase> ConvertTo<TDerived, TBase>(this Task<TDerived> task) where TDerived : TBase
        {
            var tcs = new TaskCompletionSource<TBase>();
            task.ContinueWith(t => tcs.SetResult(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion);
            task.ContinueWith(t => tcs.SetException(t.Exception.InnerExceptions), TaskContinuationOptions.OnlyOnFaulted);
            task.ContinueWith(t => tcs.SetCanceled(), TaskContinuationOptions.OnlyOnCanceled);
            return tcs.Task;
        }

        /// <summary>
        /// Gets the response asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Task&lt;WebResponse&gt;.</returns>
        public static Task<WebResponse> GetResponseAsync(this WebRequest request)
        {
            return GetResponseAsync((HttpWebRequest)request).ConvertTo<HttpWebResponse, WebResponse>();
        }

        /// <summary>
        /// Gets the response asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Task&lt;HttpWebResponse&gt;.</returns>
        public static Task<HttpWebResponse> GetResponseAsync(this HttpWebRequest request)
        {
            var tcs = new TaskCompletionSource<HttpWebResponse>();

            try
            {
                request.BeginGetResponse(iar =>
                {
                    try
                    {
                        var response = (HttpWebResponse)request.EndGetResponse(iar);
                        tcs.SetResult(response);
                    }
                    catch (Exception exc)
                    {
                        tcs.SetException(exc);
                    }
                }, null);
            }
            catch (Exception exc)
            {
                tcs.SetException(exc);
            }

            return tcs.Task;
        }

        /// <summary>
        /// Gets the header bytes.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <param name="field">The field.</param>
        /// <param name="boundary">The boundary.</param>
        /// <returns>System.Byte[].</returns>
        private static byte[] GetHeaderBytes(string fileName, string mimeType, string field, string boundary)
        {
            var header = "\r\n--" + boundary +
                         $"\r\nContent-Disposition: form-data; name=\"{field}\"; filename=\"{fileName}\"\r\nContent-Type: {mimeType}\r\n\r\n";

            var headerBytes = header.ToAsciiBytes();
            return headerBytes;
        }

        /// <summary>
        /// Uploads the file.
        /// </summary>
        /// <param name="webRequest">The web request.</param>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="method">The method.</param>
        /// <param name="field">The field.</param>
        public static void UploadFile(this WebRequest webRequest, Stream fileStream, string fileName, string mimeType,
            string accept = null, Action<HttpWebRequest> requestFilter = null, string method = "POST", string field = "file")
        {
            var httpReq = (HttpWebRequest)webRequest;
            httpReq.Method = method;

            if (accept != null)
                httpReq.Accept = accept;

            requestFilter?.Invoke(httpReq);

            var boundary = Guid.NewGuid().ToString("N");

            httpReq.ContentType = "multipart/form-data; boundary=\"" + boundary + "\"";

            var boundaryBytes = ("\r\n--" + boundary + "--\r\n").ToAsciiBytes();

            var headerBytes = GetHeaderBytes(fileName, mimeType, field, boundary);

            var contentLength = fileStream.Length + headerBytes.Length + boundaryBytes.Length;
            PclExport.Instance.InitHttpWebRequest(httpReq,
                contentLength: contentLength, allowAutoRedirect: false, keepAlive: false);

            if (ResultsFilter != null)
            {
                ResultsFilter.UploadStream(httpReq, fileStream, fileName);
                return;
            }

            using var outputStream = PclExport.Instance.GetRequestStream(httpReq);
            outputStream.Write(headerBytes, 0, headerBytes.Length);
            fileStream.CopyTo(outputStream, 4096);
            outputStream.Write(boundaryBytes, 0, boundaryBytes.Length);
            PclExport.Instance.CloseStream(outputStream);
        }

        /// <summary>
        /// Upload file as an asynchronous operation.
        /// </summary>
        /// <param name="webRequest">The web request.</param>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="method">The method.</param>
        /// <param name="field">The field.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public static async Task UploadFileAsync(this WebRequest webRequest, Stream fileStream, string fileName, string mimeType,
            string accept = null, Action<HttpWebRequest> requestFilter = null, string method = "POST", string field = "file",
            CancellationToken token = default)
        {
            var httpReq = (HttpWebRequest)webRequest;
            httpReq.Method = method;

            if (accept != null)
                httpReq.Accept = accept;

            requestFilter?.Invoke(httpReq);

            var boundary = Guid.NewGuid().ToString("N");

            httpReq.ContentType = "multipart/form-data; boundary=\"" + boundary + "\"";

            var boundaryBytes = ("\r\n--" + boundary + "--\r\n").ToAsciiBytes();

            var headerBytes = GetHeaderBytes(fileName, mimeType, field, boundary);

            var contentLength = fileStream.Length + headerBytes.Length + boundaryBytes.Length;
            PclExport.Instance.InitHttpWebRequest(httpReq,
                contentLength: contentLength, allowAutoRedirect: false, keepAlive: false);

            if (ResultsFilter != null)
            {
                ResultsFilter.UploadStream(httpReq, fileStream, fileName);
                return;
            }

            using var outputStream = PclExport.Instance.GetRequestStream(httpReq);
            await outputStream.WriteAsync(headerBytes, 0, headerBytes.Length, token).ConfigAwait();
            await fileStream.CopyToAsync(outputStream, 4096, token).ConfigAwait();
            await outputStream.WriteAsync(boundaryBytes, 0, boundaryBytes.Length, token).ConfigAwait();
            PclExport.Instance.CloseStream(outputStream);
        }

        /// <summary>
        /// Uploads the file.
        /// </summary>
        /// <param name="webRequest">The web request.</param>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <exception cref="System.ArgumentNullException">fileName</exception>
        /// <exception cref="System.ArgumentException">Mime-type not found for file: " + fileName</exception>
        public static void UploadFile(this WebRequest webRequest, Stream fileStream, string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));
            var mimeType = MimeTypes.GetMimeType(fileName);
            if (mimeType == null)
                throw new ArgumentException("Mime-type not found for file: " + fileName);

            UploadFile(webRequest, fileStream, fileName, mimeType);
        }

        /// <summary>
        /// Upload file as an asynchronous operation.
        /// </summary>
        /// <param name="webRequest">The web request.</param>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">fileName</exception>
        /// <exception cref="System.ArgumentException">Mime-type not found for file: " + fileName</exception>
        public static async Task UploadFileAsync(this WebRequest webRequest, Stream fileStream, string fileName, CancellationToken token = default)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));
            var mimeType = MimeTypes.GetMimeType(fileName);
            if (mimeType == null)
                throw new ArgumentException("Mime-type not found for file: " + fileName);

            await UploadFileAsync(webRequest, fileStream, fileName, mimeType, token: token).ConfigAwait();
        }

        /// <summary>
        /// Posts the XML to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="data">The data.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PostXmlToUrl(this string url, object data,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "POST", requestBody: data.ToXml(), contentType: MimeTypes.Xml, accept: MimeTypes.Xml,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Posts the CSV to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="data">The data.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PostCsvToUrl(this string url, object data,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "POST", requestBody: data.ToCsv(), contentType: MimeTypes.Csv, accept: MimeTypes.Csv,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Puts the XML to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="data">The data.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PutXmlToUrl(this string url, object data,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "PUT", requestBody: data.ToXml(), contentType: MimeTypes.Xml, accept: MimeTypes.Xml,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Puts the CSV to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="data">The data.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PutCsvToUrl(this string url, object data,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "PUT", requestBody: data.ToCsv(), contentType: MimeTypes.Csv, accept: MimeTypes.Csv,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Posts the file to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="uploadFileInfo">The upload file information.</param>
        /// <param name="uploadFileMimeType">Type of the upload file MIME.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <returns>WebResponse.</returns>
        public static WebResponse PostFileToUrl(this string url,
            FileInfo uploadFileInfo, string uploadFileMimeType,
            string accept = null,
            Action<HttpWebRequest> requestFilter = null)
        {
            var webReq = (HttpWebRequest)WebRequest.Create(url);
            using (var fileStream = uploadFileInfo.OpenRead())
            {
                var fileName = uploadFileInfo.Name;

                webReq.UploadFile(fileStream, fileName, uploadFileMimeType, accept: accept, requestFilter: requestFilter, method: "POST");
            }

            if (ResultsFilter != null)
                return null;

            return webReq.GetResponse();
        }

        /// <summary>
        /// Post file to URL as an asynchronous operation.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="uploadFileInfo">The upload file information.</param>
        /// <param name="uploadFileMimeType">Type of the upload file MIME.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task&lt;WebResponse&gt; representing the asynchronous operation.</returns>
        public static async Task<WebResponse> PostFileToUrlAsync(this string url,
            FileInfo uploadFileInfo, string uploadFileMimeType,
            string accept = null,
            Action<HttpWebRequest> requestFilter = null, CancellationToken token = default)
        {
            var webReq = (HttpWebRequest)WebRequest.Create(url);
            using (var fileStream = uploadFileInfo.OpenRead())
            {
                var fileName = uploadFileInfo.Name;

                await webReq.UploadFileAsync(fileStream, fileName, uploadFileMimeType, accept: accept, requestFilter: requestFilter, method: "POST", token: token).ConfigAwait();
            }

            if (ResultsFilter != null)
                return null;

            return await webReq.GetResponseAsync().ConfigAwait();
        }

        /// <summary>
        /// Puts the file to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="uploadFileInfo">The upload file information.</param>
        /// <param name="uploadFileMimeType">Type of the upload file MIME.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <returns>WebResponse.</returns>
        public static WebResponse PutFileToUrl(this string url,
            FileInfo uploadFileInfo, string uploadFileMimeType,
            string accept = null,
            Action<HttpWebRequest> requestFilter = null)
        {
            var webReq = (HttpWebRequest)WebRequest.Create(url);
            using (var fileStream = uploadFileInfo.OpenRead())
            {
                var fileName = uploadFileInfo.Name;

                webReq.UploadFile(fileStream, fileName, uploadFileMimeType, accept: accept, requestFilter: requestFilter, method: "PUT");
            }

            if (ResultsFilter != null)
                return null;

            return webReq.GetResponse();
        }

        /// <summary>
        /// Put file to URL as an asynchronous operation.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="uploadFileInfo">The upload file information.</param>
        /// <param name="uploadFileMimeType">Type of the upload file MIME.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task&lt;WebResponse&gt; representing the asynchronous operation.</returns>
        public static async Task<WebResponse> PutFileToUrlAsync(this string url,
            FileInfo uploadFileInfo, string uploadFileMimeType,
            string accept = null,
            Action<HttpWebRequest> requestFilter = null, CancellationToken token = default)
        {
            var webReq = (HttpWebRequest)WebRequest.Create(url);
            using (var fileStream = uploadFileInfo.OpenRead())
            {
                var fileName = uploadFileInfo.Name;

                await webReq.UploadFileAsync(fileStream, fileName, uploadFileMimeType, accept: accept, requestFilter: requestFilter, method: "PUT", token: token).ConfigAwait();
            }

            if (ResultsFilter != null)
                return null;

            return await webReq.GetResponseAsync().ConfigAwait();
        }

        /// <summary>
        /// Uploads the file.
        /// </summary>
        /// <param name="webRequest">The web request.</param>
        /// <param name="uploadFileInfo">The upload file information.</param>
        /// <param name="uploadFileMimeType">Type of the upload file MIME.</param>
        /// <returns>WebResponse.</returns>
        public static WebResponse UploadFile(this WebRequest webRequest,
            FileInfo uploadFileInfo, string uploadFileMimeType)
        {
            using (var fileStream = uploadFileInfo.OpenRead())
            {
                var fileName = uploadFileInfo.Name;

                webRequest.UploadFile(fileStream, fileName, uploadFileMimeType);
            }

            if (ResultsFilter != null)
                return null;

            return webRequest.GetResponse();
        }

        /// <summary>
        /// Upload file as an asynchronous operation.
        /// </summary>
        /// <param name="webRequest">The web request.</param>
        /// <param name="uploadFileInfo">The upload file information.</param>
        /// <param name="uploadFileMimeType">Type of the upload file MIME.</param>
        /// <returns>A Task&lt;WebResponse&gt; representing the asynchronous operation.</returns>
        public static async Task<WebResponse> UploadFileAsync(this WebRequest webRequest,
            FileInfo uploadFileInfo, string uploadFileMimeType)
        {
            using (var fileStream = uploadFileInfo.OpenRead())
            {
                var fileName = uploadFileInfo.Name;

                await webRequest.UploadFileAsync(fileStream, fileName, uploadFileMimeType).ConfigAwait();
            }

            if (ResultsFilter != null)
                return null;

            return await webRequest.GetResponseAsync().ConfigAwait();
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
    /// Interface IHasStatusDescription
    /// </summary>
    public interface IHasStatusDescription
    {
        /// <summary>
        /// Gets the status description.
        /// </summary>
        /// <value>The status description.</value>
        string StatusDescription { get; }
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
    /// Implements the <see cref="ServiceStack.IHttpResultsFilter" />
    /// </summary>
    /// <seealso cref="ServiceStack.IHttpResultsFilter" />
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
}

namespace ServiceStack
{
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

            if (secondaryType.StartsWith("pkc")
                || secondaryType.StartsWith("x-pkc")
                || secondaryType.StartsWith("font")
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

    /// <summary>
    /// Class CompressionTypes.
    /// </summary>
    public static class CompressionTypes
    {
        /// <summary>
        /// All compression types
        /// </summary>
        public static readonly string[] AllCompressionTypes = new[] { Deflate, GZip };

        /// <summary>
        /// The default
        /// </summary>
        public const string Default = Deflate;
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
        /// <returns><c>true</c> if the specified compression type is valid; otherwise, <c>false</c>.</returns>
        public static bool IsValid(string compressionType)
        {
            return compressionType == Deflate || compressionType == GZip;
        }

        /// <summary>
        /// Asserts the is valid.
        /// </summary>
        /// <param name="compressionType">Type of the compression.</param>
        /// <exception cref="System.NotSupportedException"></exception>
        public static void AssertIsValid(string compressionType)
        {
            if (!IsValid(compressionType))
            {
                throw new NotSupportedException(compressionType
                    + " is not a supported compression type. Valid types: gzip, deflate.");
            }
        }

        /// <summary>
        /// Gets the extension.
        /// </summary>
        /// <param name="compressionType">Type of the compression.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.NotSupportedException">Unknown compressionType: " + compressionType</exception>
        public static string GetExtension(string compressionType)
        {
            switch (compressionType)
            {
                case Deflate:
                case GZip:
                    return "." + compressionType;
                default:
                    throw new NotSupportedException(
                        "Unknown compressionType: " + compressionType);
            }
        }
    }

    /// <summary>
    /// Class HttpStatus.
    /// </summary>
    public static class HttpStatus
    {
        /// <summary>
        /// Gets the status description.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <returns>System.String.</returns>
        public static string GetStatusDescription(int statusCode)
        {
            if (statusCode >= 100 && statusCode < 600)
            {
                int i = statusCode / 100;
                int j = statusCode % 100;

                if (j < Descriptions[i].Length)
                    return Descriptions[i][j];
            }

            return string.Empty;
        }

        /// <summary>
        /// The descriptions
        /// </summary>
        private static readonly string[][] Descriptions = new string[][]
        {
            null,
            new[]
            { 
                /* 100 */ "Continue",
                /* 101 */ "Switching Protocols",
                /* 102 */ "Processing"
            },
            new[]
            { 
                /* 200 */ "OK",
                /* 201 */ "Created",
                /* 202 */ "Accepted",
                /* 203 */ "Non-Authoritative Information",
                /* 204 */ "No Content",
                /* 205 */ "Reset Content",
                /* 206 */ "Partial Content",
                /* 207 */ "Multi-Status"
            },
            new[]
            { 
                /* 300 */ "Multiple Choices",
                /* 301 */ "Moved Permanently",
                /* 302 */ "Found",
                /* 303 */ "See Other",
                /* 304 */ "Not Modified",
                /* 305 */ "Use Proxy",
                /* 306 */ string.Empty,
                /* 307 */ "Temporary Redirect"
            },
            new[]
            { 
                /* 400 */ "Bad Request",
                /* 401 */ "Unauthorized",
                /* 402 */ "Payment Required",
                /* 403 */ "Forbidden",
                /* 404 */ "Not Found",
                /* 405 */ "Method Not Allowed",
                /* 406 */ "Not Acceptable",
                /* 407 */ "Proxy Authentication Required",
                /* 408 */ "Request Timeout",
                /* 409 */ "Conflict",
                /* 410 */ "Gone",
                /* 411 */ "Length Required",
                /* 412 */ "Precondition Failed",
                /* 413 */ "Request Entity Too Large",
                /* 414 */ "Request-Uri Too Long",
                /* 415 */ "Unsupported Media Type",
                /* 416 */ "Requested Range Not Satisfiable",
                /* 417 */ "Expectation Failed",
                /* 418 */ string.Empty,
                /* 419 */ string.Empty,
                /* 420 */ string.Empty,
                /* 421 */ string.Empty,
                /* 422 */ "Unprocessable Entity",
                /* 423 */ "Locked",
                /* 424 */ "Failed Dependency"
            },
            new[]
            { 
                /* 500 */ "Internal Server Error",
                /* 501 */ "Not Implemented",
                /* 502 */ "Bad Gateway",
                /* 503 */ "Service Unavailable",
                /* 504 */ "Gateway Timeout",
                /* 505 */ "Http Version Not Supported",
                /* 506 */ string.Empty,
                /* 507 */ "Insufficient Storage"
            }
        };
    }
}
