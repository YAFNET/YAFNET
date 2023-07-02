// ***********************************************************************
// <copyright file="HttpUtils.WebRequest.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#if !NET7_0_OR_GREATER

namespace ServiceStack
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using ServiceStack.Text;

/// <summary>
/// Class HttpUtils.
/// </summary>
    public static partial class HttpUtils
    {
        /// <summary>
        /// The results filter
        /// </summary>
        [ThreadStatic]
        public static IHttpResultsFilter ResultsFilter;

        /// <summary>
        /// Gets the json from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string GetJsonFromUrl(
            this string url,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
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
        public static Task<string> GetJsonFromUrlAsync(
            this string url,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
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
        public static string GetXmlFromUrl(
            this string url,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
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
        public static Task<string> GetXmlFromUrlAsync(
            this string url,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
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
        public static string GetCsvFromUrl(
            this string url,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
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
        public static Task<string> GetCsvFromUrlAsync(
            this string url,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
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
        public static string GetStringFromUrl(
            this string url,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
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
        public static Task<string> GetStringFromUrlAsync(
            this string url,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
        {
            return SendStringToUrlAsync(
                url,
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter,
                token: token);
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
        public static string PostStringToUrl(
            this string url,
            string requestBody = null,
            string contentType = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(
                url,
                method: "POST",
                requestBody: requestBody,
                contentType: contentType,
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
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
        public static Task<string> PostStringToUrlAsync(
            this string url,
            string requestBody = null,
            string contentType = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
        {
            return SendStringToUrlAsync(
                url,
                method: "POST",
                requestBody: requestBody,
                contentType: contentType,
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter,
                token: token);
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
        public static string PostToUrl(
            this string url,
            string formData = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(
                url,
                method: "POST",
                contentType: MimeTypes.FormUrlEncoded,
                requestBody: formData,
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
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
        public static Task<string> PostToUrlAsync(
            this string url,
            string formData = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
        {
            return SendStringToUrlAsync(
                url,
                method: "POST",
                contentType: MimeTypes.FormUrlEncoded,
                requestBody: formData,
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter,
                token: token);
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
        public static string PostToUrl(
            this string url,
            object formData = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            string postFormData = formData != null ? QueryStringSerializer.SerializeToString(formData) : null;

            return SendStringToUrl(
                url,
                method: "POST",
                contentType: MimeTypes.FormUrlEncoded,
                requestBody: postFormData,
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
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
        public static Task<string> PostToUrlAsync(
            this string url,
            object formData = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
        {
            string postFormData = formData != null ? QueryStringSerializer.SerializeToString(formData) : null;

            return SendStringToUrlAsync(
                url,
                method: "POST",
                contentType: MimeTypes.FormUrlEncoded,
                requestBody: postFormData,
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter,
                token: token);
        }

        /// <summary>
        /// Posts the json to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="json">The json.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PostJsonToUrl(
            this string url,
            string json,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(
                url,
                method: "POST",
                requestBody: json,
                contentType: MimeTypes.Json,
                accept: MimeTypes.Json,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
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
        public static Task<string> PostJsonToUrlAsync(
            this string url,
            string json,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
        {
            return SendStringToUrlAsync(
                url,
                method: "POST",
                requestBody: json,
                contentType: MimeTypes.Json,
                accept: MimeTypes.Json,
                requestFilter: requestFilter,
                responseFilter: responseFilter,
                token: token);
        }

        /// <summary>
        /// Posts the json to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="data">The data.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PostJsonToUrl(
            this string url,
            object data,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(
                url,
                method: "POST",
                requestBody: data.ToJson(),
                contentType: MimeTypes.Json,
                accept: MimeTypes.Json,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
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
        public static Task<string> PostJsonToUrlAsync(
            this string url,
            object data,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
        {
            return SendStringToUrlAsync(
                url,
                method: "POST",
                requestBody: data.ToJson(),
                contentType: MimeTypes.Json,
                accept: MimeTypes.Json,
                requestFilter: requestFilter,
                responseFilter: responseFilter,
                token: token);
        }

        /// <summary>
        /// Posts the XML to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="xml">The XML.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PostXmlToUrl(
            this string url,
            string xml,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(
                url,
                method: "POST",
                requestBody: xml,
                contentType: MimeTypes.Xml,
                accept: MimeTypes.Xml,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
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
        public static Task<string> PostXmlToUrlAsync(
            this string url,
            string xml,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
        {
            return SendStringToUrlAsync(
                url,
                method: "POST",
                requestBody: xml,
                contentType: MimeTypes.Xml,
                accept: MimeTypes.Xml,
                requestFilter: requestFilter,
                responseFilter: responseFilter,
                token: token);
        }

        /// <summary>
        /// Posts the CSV to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="csv">The CSV.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PostCsvToUrl(
            this string url,
            string csv,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(
                url,
                method: "POST",
                requestBody: csv,
                contentType: MimeTypes.Csv,
                accept: MimeTypes.Csv,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
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
        public static Task<string> PostCsvToUrlAsync(
            this string url,
            string csv,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
        {
            return SendStringToUrlAsync(
                url,
                method: "POST",
                requestBody: csv,
                contentType: MimeTypes.Csv,
                accept: MimeTypes.Csv,
                requestFilter: requestFilter,
                responseFilter: responseFilter,
                token: token);
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
        public static string PutStringToUrl(
            this string url,
            string requestBody = null,
            string contentType = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(
                url,
                method: "PUT",
                requestBody: requestBody,
                contentType: contentType,
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
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
        public static Task<string> PutStringToUrlAsync(
            this string url,
            string requestBody = null,
            string contentType = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
        {
            return SendStringToUrlAsync(
                url,
                method: "PUT",
                requestBody: requestBody,
                contentType: contentType,
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter,
                token: token);
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
        public static string PutToUrl(
            this string url,
            string formData = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(
                url,
                method: "PUT",
                contentType: MimeTypes.FormUrlEncoded,
                requestBody: formData,
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
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
        public static Task<string> PutToUrlAsync(
            this string url,
            string formData = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
        {
            return SendStringToUrlAsync(
                url,
                method: "PUT",
                contentType: MimeTypes.FormUrlEncoded,
                requestBody: formData,
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter,
                token: token);
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
        public static string PutToUrl(
            this string url,
            object formData = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            string postFormData = formData != null ? QueryStringSerializer.SerializeToString(formData) : null;

            return SendStringToUrl(
                url,
                method: "PUT",
                contentType: MimeTypes.FormUrlEncoded,
                requestBody: postFormData,
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
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
        public static Task<string> PutToUrlAsync(
            this string url,
            object formData = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
        {
            string postFormData = formData != null ? QueryStringSerializer.SerializeToString(formData) : null;

            return SendStringToUrlAsync(
                url,
                method: "PUT",
                contentType: MimeTypes.FormUrlEncoded,
                requestBody: postFormData,
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter,
                token: token);
        }

        /// <summary>
        /// Puts the json to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="json">The json.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PutJsonToUrl(
            this string url,
            string json,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(
                url,
                method: "PUT",
                requestBody: json,
                contentType: MimeTypes.Json,
                accept: MimeTypes.Json,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
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
        public static Task<string> PutJsonToUrlAsync(
            this string url,
            string json,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
        {
            return SendStringToUrlAsync(
                url,
                method: "PUT",
                requestBody: json,
                contentType: MimeTypes.Json,
                accept: MimeTypes.Json,
                requestFilter: requestFilter,
                responseFilter: responseFilter,
                token: token);
        }

        /// <summary>
        /// Puts the json to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="data">The data.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PutJsonToUrl(
            this string url,
            object data,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(
                url,
                method: "PUT",
                requestBody: data.ToJson(),
                contentType: MimeTypes.Json,
                accept: MimeTypes.Json,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
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
        public static Task<string> PutJsonToUrlAsync(
            this string url,
            object data,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
        {
            return SendStringToUrlAsync(
                url,
                method: "PUT",
                requestBody: data.ToJson(),
                contentType: MimeTypes.Json,
                accept: MimeTypes.Json,
                requestFilter: requestFilter,
                responseFilter: responseFilter,
                token: token);
        }

        /// <summary>
        /// Puts the XML to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="xml">The XML.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PutXmlToUrl(
            this string url,
            string xml,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(
                url,
                method: "PUT",
                requestBody: xml,
                contentType: MimeTypes.Xml,
                accept: MimeTypes.Xml,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
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
        public static Task<string> PutXmlToUrlAsync(
            this string url,
            string xml,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
        {
            return SendStringToUrlAsync(
                url,
                method: "PUT",
                requestBody: xml,
                contentType: MimeTypes.Xml,
                accept: MimeTypes.Xml,
                requestFilter: requestFilter,
                responseFilter: responseFilter,
                token: token);
        }

        /// <summary>
        /// Puts the CSV to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="csv">The CSV.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PutCsvToUrl(
            this string url,
            string csv,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(
                url,
                method: "PUT",
                requestBody: csv,
                contentType: MimeTypes.Csv,
                accept: MimeTypes.Csv,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
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
        public static Task<string> PutCsvToUrlAsync(
            this string url,
            string csv,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
        {
            return SendStringToUrlAsync(
                url,
                method: "PUT",
                requestBody: csv,
                contentType: MimeTypes.Csv,
                accept: MimeTypes.Csv,
                requestFilter: requestFilter,
                responseFilter: responseFilter,
                token: token);
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
        public static string PatchStringToUrl(
            this string url,
            string requestBody = null,
            string contentType = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(
                url,
                method: "PATCH",
                requestBody: requestBody,
                contentType: contentType,
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
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
        public static Task<string> PatchStringToUrlAsync(
            this string url,
            string requestBody = null,
            string contentType = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
        {
            return SendStringToUrlAsync(
                url,
                method: "PATCH",
                requestBody: requestBody,
                contentType: contentType,
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter,
                token: token);
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
        public static string PatchToUrl(
            this string url,
            string formData = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(
                url,
                method: "PATCH",
                contentType: MimeTypes.FormUrlEncoded,
                requestBody: formData,
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
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
        public static Task<string> PatchToUrlAsync(
            this string url,
            string formData = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
        {
            return SendStringToUrlAsync(
                url,
                method: "PATCH",
                contentType: MimeTypes.FormUrlEncoded,
                requestBody: formData,
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter,
                token: token);
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
        public static string PatchToUrl(
            this string url,
            object formData = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            string postFormData = formData != null ? QueryStringSerializer.SerializeToString(formData) : null;

            return SendStringToUrl(
                url,
                method: "PATCH",
                contentType: MimeTypes.FormUrlEncoded,
                requestBody: postFormData,
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
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
        public static Task<string> PatchToUrlAsync(
            this string url,
            object formData = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
        {
            string postFormData = formData != null ? QueryStringSerializer.SerializeToString(formData) : null;

            return SendStringToUrlAsync(
                url,
                method: "PATCH",
                contentType: MimeTypes.FormUrlEncoded,
                requestBody: postFormData,
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter,
                token: token);
        }

        /// <summary>
        /// Patches the json to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="json">The json.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PatchJsonToUrl(
            this string url,
            string json,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(
                url,
                method: "PATCH",
                requestBody: json,
                contentType: MimeTypes.Json,
                accept: MimeTypes.Json,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
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
        public static Task<string> PatchJsonToUrlAsync(
            this string url,
            string json,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
        {
            return SendStringToUrlAsync(
                url,
                method: "PATCH",
                requestBody: json,
                contentType: MimeTypes.Json,
                accept: MimeTypes.Json,
                requestFilter: requestFilter,
                responseFilter: responseFilter,
                token: token);
        }

        /// <summary>
        /// Patches the json to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="data">The data.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PatchJsonToUrl(
            this string url,
            object data,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(
                url,
                method: "PATCH",
                requestBody: data.ToJson(),
                contentType: MimeTypes.Json,
                accept: MimeTypes.Json,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
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
        public static Task<string> PatchJsonToUrlAsync(
            this string url,
            object data,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
        {
            return SendStringToUrlAsync(
                url,
                method: "PATCH",
                requestBody: data.ToJson(),
                contentType: MimeTypes.Json,
                accept: MimeTypes.Json,
                requestFilter: requestFilter,
                responseFilter: responseFilter,
                token: token);
        }

        /// <summary>
        /// Deletes from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string DeleteFromUrl(
            this string url,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(
                url,
                method: "DELETE",
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
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
        public static Task<string> DeleteFromUrlAsync(
            this string url,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
        {
            return SendStringToUrlAsync(
                url,
                method: "DELETE",
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter,
                token: token);
        }

        /// <summary>
        /// Optionses from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string OptionsFromUrl(
            this string url,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(
                url,
                method: "OPTIONS",
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
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
        public static Task<string> OptionsFromUrlAsync(
            this string url,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
        {
            return SendStringToUrlAsync(
                url,
                method: "OPTIONS",
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter,
                token: token);
        }

        /// <summary>
        /// Heads from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="accept">The accept.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string HeadFromUrl(
            this string url,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(
                url,
                method: "HEAD",
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
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
        public static Task<string> HeadFromUrlAsync(
            this string url,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
        {
            return SendStringToUrlAsync(
                url,
                method: "HEAD",
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter,
                token: token);
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
        public static async Task<string> SendStringToUrlAsync(
            this string url,
            string method = null,
            string requestBody = null,
            string contentType = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
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
                var result = ResultsFilter.GetString(webReq, requestBody);
                return result;
            }

            if (requestBody != null)
            {
                using var reqStream = PclExport.Instance.GetRequestStream(webReq);
                using var writer = new StreamWriter(reqStream, UseEncoding);
                await writer.WriteAsync(requestBody).ConfigAwait();
            }
            else if (method != null && HasRequestBody(method))
            {
                webReq.ContentLength = 0;
            }

            using var webRes = await webReq.GetResponseAsync().ConfigAwait();
            responseFilter?.Invoke((HttpWebResponse) webRes);
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
        public static byte[] GetBytesFromUrl(
            this string url,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
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
        public static Task<byte[]> GetBytesFromUrlAsync(
            this string url,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
        {
            return url.SendBytesToUrlAsync(
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter,
                token: token);
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
        public static byte[] PostBytesToUrl(
            this string url,
            byte[] requestBody = null,
            string contentType = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            return SendBytesToUrl(
                url,
                method: "POST",
                contentType: contentType,
                requestBody: requestBody,
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
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
        public static Task<byte[]> PostBytesToUrlAsync(
            this string url,
            byte[] requestBody = null,
            string contentType = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
        {
            return SendBytesToUrlAsync(
                url,
                method: "POST",
                contentType: contentType,
                requestBody: requestBody,
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter,
                token: token);
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
        public static byte[] PutBytesToUrl(
            this string url,
            byte[] requestBody = null,
            string contentType = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            return SendBytesToUrl(
                url,
                method: "PUT",
                contentType: contentType,
                requestBody: requestBody,
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
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
        public static Task<byte[]> PutBytesToUrlAsync(
            this string url,
            byte[] requestBody = null,
            string contentType = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
        {
            return SendBytesToUrlAsync(
                url,
                method: "PUT",
                contentType: contentType,
                requestBody: requestBody,
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter,
                token: token);
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
        public static byte[] SendBytesToUrl(
            this string url,
            string method = null,
            byte[] requestBody = null,
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
                return ResultsFilter.GetBytes(webReq, requestBody);
            }

            if (requestBody != null)
            {
                using var req = PclExport.Instance.GetRequestStream(webReq);
                req.Write(requestBody, 0, requestBody.Length);
            }

            using var webRes = PclExport.Instance.GetResponse(webReq);
            responseFilter?.Invoke((HttpWebResponse) webRes);

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
        public static async Task<byte[]> SendBytesToUrlAsync(
            this string url,
            string method = null,
            byte[] requestBody = null,
            string contentType = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
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
                var result = ResultsFilter.GetBytes(webReq, requestBody);
                return result;
            }

            if (requestBody != null)
            {
                using var req = PclExport.Instance.GetRequestStream(webReq);
                await req.WriteAsync(requestBody, 0, requestBody.Length, token).ConfigAwait();
            }

            var webRes = await webReq.GetResponseAsync().ConfigAwait();
            responseFilter?.Invoke((HttpWebResponse) webRes);

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
        public static Stream GetStreamFromUrl(
            this string url,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
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
        public static Task<Stream> GetStreamFromUrlAsync(
            this string url,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
        {
            return url.SendStreamToUrlAsync(
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter,
                token: token);
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
        public static Stream PostStreamToUrl(
            this string url,
            Stream requestBody = null,
            string contentType = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            return SendStreamToUrl(
                url,
                method: "POST",
                contentType: contentType,
                requestBody: requestBody,
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
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
        public static Task<Stream> PostStreamToUrlAsync(
            this string url,
            Stream requestBody = null,
            string contentType = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
        {
            return SendStreamToUrlAsync(
                url,
                method: "POST",
                contentType: contentType,
                requestBody: requestBody,
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter,
                token: token);
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
        public static Stream PutStreamToUrl(
            this string url,
            Stream requestBody = null,
            string contentType = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            return SendStreamToUrl(
                url,
                method: "PUT",
                contentType: contentType,
                requestBody: requestBody,
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
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
        public static Task<Stream> PutStreamToUrlAsync(
            this string url,
            Stream requestBody = null,
            string contentType = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
        {
            return SendStreamToUrlAsync(
                url,
                method: "PUT",
                contentType: contentType,
                requestBody: requestBody,
                accept: accept,
                requestFilter: requestFilter,
                responseFilter: responseFilter,
                token: token);
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
        public static Stream SendStreamToUrl(
            this string url,
            string method = null,
            Stream requestBody = null,
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
                return new MemoryStream(ResultsFilter.GetBytes(webReq, requestBody.ReadFully()));
            }

            if (requestBody != null)
            {
                using var req = PclExport.Instance.GetRequestStream(webReq);
                requestBody.CopyTo(req);
            }

            var webRes = PclExport.Instance.GetResponse(webReq);
            responseFilter?.Invoke((HttpWebResponse) webRes);

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
        public static async Task<Stream> SendStreamToUrlAsync(
            this string url,
            string method = null,
            Stream requestBody = null,
            string contentType = null,
            string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null,
            CancellationToken token = default)
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
                return new MemoryStream(
                    ResultsFilter.GetBytes(webReq, await requestBody.ReadFullyAsync(token).ConfigAwait()));
            }

            if (requestBody != null)
            {
                using var req = PclExport.Instance.GetRequestStream(webReq);
                await requestBody.CopyToAsync(req, token).ConfigAwait();
            }

            var webRes = await webReq.GetResponseAsync().ConfigAwait();
            responseFilter?.Invoke((HttpWebResponse) webRes);

            var stream = webRes.GetResponseStream();
            return stream;
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
                var webReq = (HttpWebRequest) WebRequest.Create(url);
                using var webRes = PclExport.Instance.GetResponse(webReq);
                var httpRes = webRes as HttpWebResponse;
                return httpRes?.StatusCode;
            }
            catch (Exception ex)
            {
                return ex.GetStatus();
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
                return (HttpWebResponse) webEx.Response;
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
                return (HttpWebResponse) webEx.Response;
            }
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
        public static void UploadFile(
            this WebRequest webRequest,
            Stream fileStream,
            string fileName,
            string mimeType,
            string accept = null,
            Action<HttpWebRequest> requestFilter = null,
            string method = "POST",
            string field = "file")
        {
            var httpReq = (HttpWebRequest) webRequest;
            httpReq.Method = method;

            if (accept != null)
                httpReq.Accept = accept;

            requestFilter?.Invoke(httpReq);

            var boundary = Guid.NewGuid().ToString("N");

            httpReq.ContentType = "multipart/form-data; boundary=\"" + boundary + "\"";

            var boundaryBytes = ("\r\n--" + boundary + "--\r\n").ToAsciiBytes();

            var headerBytes = GetHeaderBytes(fileName, mimeType, field, boundary);

            var contentLength = fileStream.Length + headerBytes.Length + boundaryBytes.Length;
            PclExport.Instance.InitHttpWebRequest(
                httpReq,
                contentLength: contentLength,
                allowAutoRedirect: false,
                keepAlive: false);

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
        public static async Task UploadFileAsync(
            this WebRequest webRequest,
            Stream fileStream,
            string fileName,
            string mimeType,
            string accept = null,
            Action<HttpWebRequest> requestFilter = null,
            string method = "POST",
            string field = "file",
            CancellationToken token = default)
        {
            var httpReq = (HttpWebRequest) webRequest;
            httpReq.Method = method;

            if (accept != null)
                httpReq.Accept = accept;

            requestFilter?.Invoke(httpReq);

            var boundary = Guid.NewGuid().ToString("N");

            httpReq.ContentType = "multipart/form-data; boundary=\"" + boundary + "\"";

            var boundaryBytes = ("\r\n--" + boundary + "--\r\n").ToAsciiBytes();

            var headerBytes = GetHeaderBytes(fileName, mimeType, field, boundary);

            var contentLength = fileStream.Length + headerBytes.Length + boundaryBytes.Length;
            PclExport.Instance.InitHttpWebRequest(
                httpReq,
                contentLength: contentLength,
                allowAutoRedirect: false,
                keepAlive: false);

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
        public static async Task UploadFileAsync(
            this WebRequest webRequest,
            Stream fileStream,
            string fileName,
            CancellationToken token = default)
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
        public static string PostXmlToUrl(
            this string url,
            object data,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(
                url,
                method: "POST",
                requestBody: data.ToXml(),
                contentType: MimeTypes.Xml,
                accept: MimeTypes.Xml,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
        }

        /// <summary>
        /// Posts the CSV to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="data">The data.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PostCsvToUrl(
            this string url,
            object data,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(
                url,
                method: "POST",
                requestBody: data.ToCsv(),
                contentType: MimeTypes.Csv,
                accept: MimeTypes.Csv,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
        }

        /// <summary>
        /// Puts the XML to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="data">The data.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PutXmlToUrl(
            this string url,
            object data,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(
                url,
                method: "PUT",
                requestBody: data.ToXml(),
                contentType: MimeTypes.Xml,
                accept: MimeTypes.Xml,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
        }

        /// <summary>
        /// Puts the CSV to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="data">The data.</param>
        /// <param name="requestFilter">The request filter.</param>
        /// <param name="responseFilter">The response filter.</param>
        /// <returns>System.String.</returns>
        public static string PutCsvToUrl(
            this string url,
            object data,
            Action<HttpWebRequest> requestFilter = null,
            Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(
                url,
                method: "PUT",
                requestBody: data.ToCsv(),
                contentType: MimeTypes.Csv,
                accept: MimeTypes.Csv,
                requestFilter: requestFilter,
                responseFilter: responseFilter);
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
        public static WebResponse PostFileToUrl(
            this string url,
            FileInfo uploadFileInfo,
            string uploadFileMimeType,
            string accept = null,
            Action<HttpWebRequest> requestFilter = null)
        {
            var webReq = (HttpWebRequest) WebRequest.Create(url);
            using (var fileStream = uploadFileInfo.OpenRead())
            {
                var fileName = uploadFileInfo.Name;

                webReq.UploadFile(
                    fileStream,
                    fileName,
                    uploadFileMimeType,
                    accept: accept,
                    requestFilter: requestFilter,
                    method: "POST");
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
        public static async Task<WebResponse> PostFileToUrlAsync(
            this string url,
            FileInfo uploadFileInfo,
            string uploadFileMimeType,
            string accept = null,
            Action<HttpWebRequest> requestFilter = null,
            CancellationToken token = default)
        {
            var webReq = (HttpWebRequest) WebRequest.Create(url);
            using (var fileStream = uploadFileInfo.OpenRead())
            {
                var fileName = uploadFileInfo.Name;

                await webReq.UploadFileAsync(
                    fileStream,
                    fileName,
                    uploadFileMimeType,
                    accept: accept,
                    requestFilter: requestFilter,
                    method: "POST",
                    token: token).ConfigAwait();
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
        public static WebResponse PutFileToUrl(
            this string url,
            FileInfo uploadFileInfo,
            string uploadFileMimeType,
            string accept = null,
            Action<HttpWebRequest> requestFilter = null)
        {
            var webReq = (HttpWebRequest) WebRequest.Create(url);
            using (var fileStream = uploadFileInfo.OpenRead())
            {
                var fileName = uploadFileInfo.Name;

                webReq.UploadFile(
                    fileStream,
                    fileName,
                    uploadFileMimeType,
                    accept: accept,
                    requestFilter: requestFilter,
                    method: "PUT");
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
        public static async Task<WebResponse> PutFileToUrlAsync(
            this string url,
            FileInfo uploadFileInfo,
            string uploadFileMimeType,
            string accept = null,
            Action<HttpWebRequest> requestFilter = null,
            CancellationToken token = default)
        {
            var webReq = (HttpWebRequest) WebRequest.Create(url);
            using (var fileStream = uploadFileInfo.OpenRead())
            {
                var fileName = uploadFileInfo.Name;

                await webReq.UploadFileAsync(
                    fileStream,
                    fileName,
                    uploadFileMimeType,
                    accept: accept,
                    requestFilter: requestFilter,
                    method: "PUT",
                    token: token).ConfigAwait();
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
        public static WebResponse UploadFile(
            this WebRequest webRequest,
            FileInfo uploadFileInfo,
            string uploadFileMimeType)
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
        public static async Task<WebResponse> UploadFileAsync(
            this WebRequest webRequest,
            FileInfo uploadFileInfo,
            string uploadFileMimeType)
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
            var header = "\r\n--" + boundary
                                  + $"\r\nContent-Disposition: form-data; name=\"{field}\"; filename=\"{fileName}\"\r\nContent-Type: {mimeType}\r\n\r\n";

            var headerBytes = header.ToAsciiBytes();
            return headerBytes;
        }

        /// <summary>
        /// Downloads the file to.
        /// </summary>
        /// <param name="downloadUrl">The download URL.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="headers">The headers.</param>
        public static void DownloadFileTo(
            this string downloadUrl,
            string fileName,
            List<HttpRequestConfig.NameValue> headers = null)
        {
            var webClient = new WebClient();
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    webClient.Headers[header.Name] = header.Value;
                }
            }

            webClient.DownloadFile(downloadUrl, fileName);
        }

        /// <summary>
        /// Sets the range.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        public static void SetRange(this HttpWebRequest request, long from, long? to)
        {
            if (to != null)
                request.AddRange(from, to.Value);
            else
                request.AddRange(from);
        }

        /// <summary>
        /// Adds the header.
        /// </summary>
        /// <param name="res">The resource.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public static void AddHeader(this HttpWebRequest res, string name, string value) => res.Headers[name] = value;

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <param name="res">The resource.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public static string GetHeader(this HttpWebRequest res, string name) => res.Headers.Get(name);

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <param name="res">The resource.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public static string GetHeader(this HttpWebResponse res, string name) => res.Headers.Get(name);

        /// <summary>
        /// Withes the header.
        /// </summary>
        /// <param name="httpReq">The HTTP req.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>HttpWebRequest.</returns>
        public static HttpWebRequest WithHeader(this HttpWebRequest httpReq, string name, string value)
        {
            httpReq.Headers[name] = value;
            return httpReq;
        }

        /// <summary>
        /// Populate HttpRequestMessage with a simpler, untyped API
        /// Syntax compatible with HttpClient's HttpRequestMessage
        /// </summary>
        /// <param name="httpReq">The HTTP req.</param>
        /// <param name="configure">The configure.</param>
        /// <returns>HttpWebRequest.</returns>
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

/// <summary>
/// Interface IHttpResultsFilter
/// </summary>
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
/// </summary>
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
        /// Disposes this instance.
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
            return StringResultFn != null ? StringResultFn(webReq, reqBody) : StringResult;
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <param name="webReq">The web req.</param>
        /// <param name="reqBody">The req body.</param>
        /// <returns>System.Byte[].</returns>
        public byte[] GetBytes(HttpWebRequest webReq, byte[] reqBody)
        {
            return BytesResultFn != null ? BytesResultFn(webReq, reqBody) : BytesResult;
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
/// Class HttpClientExt.
/// </summary>
    public static class HttpClientExt
    {
        /// <summary>
        /// Case-insensitive, trimmed compare of two content types from start to ';', i.e. without charset suffix
        /// </summary>
        /// <param name="res">The resource.</param>
        /// <param name="matchesContentType">Type of the matches content.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool MatchesContentType(this HttpWebResponse res, string matchesContentType) =>
            MimeTypes.MatchesContentType(res.Headers[HttpHeaders.ContentType], matchesContentType);

        /// <summary>
        /// Returns null for unknown Content-length
        /// Syntax + Behavior compatible with HttpClient HttpResponseMessage
        /// </summary>
        /// <param name="res">The resource.</param>
        /// <returns>System.Nullable&lt;System.Int64&gt;.</returns>
        public static long? GetContentLength(this HttpWebResponse res) =>
            res.ContentLength == -1 ? null : res.ContentLength;
    }
}

#endif
