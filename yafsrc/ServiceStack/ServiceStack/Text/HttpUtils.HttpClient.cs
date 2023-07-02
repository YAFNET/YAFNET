// ***********************************************************************
// <copyright file="HttpUtils.HttpClient.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using ServiceStack.Text;

namespace ServiceStack;

/// <summary>
/// Class HttpUtils.
/// </summary>
public static partial class HttpUtils
{
    /// <summary>
    /// Class HttpClientFactory.
    /// </summary>
    private class HttpClientFactory
    {
        /// <summary>
        /// The lazy handler
        /// </summary>
        private readonly Lazy<HttpMessageHandler> lazyHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientFactory"/> class.
        /// </summary>
        /// <param name="handler">The handler.</param>
        internal HttpClientFactory(Func<HttpClientHandler> handler) =>
            lazyHandler = new Lazy<HttpMessageHandler>(
                () => handler(),
                LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Creates the client.
        /// </summary>
        /// <returns>System.Net.Http.HttpClient.</returns>
        public HttpClient CreateClient() => new(lazyHandler.Value, disposeHandler: false);
    }

    // Ok to use HttpClientHandler which now uses SocketsHttpHandler
    // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Net.Http/src/System/Net/Http/HttpClientHandler.cs#L16
    /// <summary>
    /// Gets or sets the HTTP client handler factory.
    /// </summary>
    /// <value>The HTTP client handler factory.</value>
    public static Func<HttpClientHandler> HttpClientHandlerFactory { get; set; } = () =>
        new()
            {
                UseDefaultCredentials = true,
                AutomaticDecompression = DecompressionMethods.Brotli | DecompressionMethods.Deflate
                                                                     | DecompressionMethods.GZip,
            };

    // This was the least desirable end to this sadness https://github.com/dotnet/aspnetcore/issues/28385
    // Requires <PackageReference Include="Microsoft.Extensions.Http" Version="6.0.0" /> 
    // public static IHttpClientFactory ClientFactory { get; set; } = new ServiceCollection()
    //     .AddHttpClient()
    //     .Configure<HttpClientFactoryOptions>(options => 
    //         options.HttpMessageHandlerBuilderActions.Add(builder => builder.PrimaryHandler = HandlerFactory))
    //     .BuildServiceProvider().GetRequiredService<IHttpClientFactory>();

    // Escape & BYO IHttpClientFactory
    /// <summary>
    /// The client factory
    /// </summary>
    private static HttpClientFactory? clientFactory;

    /// <summary>
    /// Gets or sets the create client.
    /// </summary>
    /// <value>The create client.</value>
    public static Func<HttpClient> CreateClient { get; set; } = () =>
        {
            try
            {
                clientFactory ??= new(HttpClientHandlerFactory);
                return clientFactory.CreateClient();
            }
            catch (Exception ex)
            {
                Tracer.Instance.WriteError(ex);
                return new HttpClient();
            }
        };

    /// <summary>
    /// Creates this instance.
    /// </summary>
    /// <returns>System.Net.Http.HttpClient.</returns>
    public static HttpClient Create() => CreateClient();

    /// <summary>
    /// Gets the json from URL.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="requestFilter">The request filter.</param>
    /// <param name="responseFilter">The response filter.</param>
    /// <returns>string.</returns>
    public static string GetJsonFromUrl(
        this string url,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return url.GetStringFromUrl(accept: MimeTypes.Json, requestFilter, responseFilter);
    }

    /// <summary>
    /// Gets the json from URL asynchronous.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="requestFilter">The request filter.</param>
    /// <param name="responseFilter">The response filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>System.Threading.Tasks.Task&lt;string&gt;.</returns>
    public static Task<string> GetJsonFromUrlAsync(
        this string url,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return url.GetStringFromUrlAsync(accept: MimeTypes.Json, requestFilter, responseFilter, token: token);
    }

    /// <summary>
    /// Gets the XML from URL.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="requestFilter">The request filter.</param>
    /// <param name="responseFilter">The response filter.</param>
    /// <returns>string.</returns>
    public static string GetXmlFromUrl(
        this string url,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return url.GetStringFromUrl(accept: MimeTypes.Xml, requestFilter, responseFilter);
    }

    /// <summary>
    /// Gets the XML from URL asynchronous.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="requestFilter">The request filter.</param>
    /// <param name="responseFilter">The response filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>System.Threading.Tasks.Task&lt;string&gt;.</returns>
    public static Task<string> GetXmlFromUrlAsync(
        this string url,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return url.GetStringFromUrlAsync(accept: MimeTypes.Xml, requestFilter, responseFilter, token: token);
    }

    /// <summary>
    /// Gets the CSV from URL.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="requestFilter">The request filter.</param>
    /// <param name="responseFilter">The response filter.</param>
    /// <returns>string.</returns>
    public static string GetCsvFromUrl(
        this string url,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return url.GetStringFromUrl(accept: MimeTypes.Csv, requestFilter, responseFilter);
    }

    /// <summary>
    /// Gets the CSV from URL asynchronous.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="requestFilter">The request filter.</param>
    /// <param name="responseFilter">The response filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>System.Threading.Tasks.Task&lt;string&gt;.</returns>
    public static Task<string> GetCsvFromUrlAsync(
        this string url,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return url.GetStringFromUrlAsync(accept: MimeTypes.Csv, requestFilter, responseFilter, token: token);
    }

    /// <summary>
    /// Gets the string from URL.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="accept">The accept.</param>
    /// <param name="requestFilter">The request filter.</param>
    /// <param name="responseFilter">The response filter.</param>
    /// <returns>string.</returns>
    public static string GetStringFromUrl(
        this string url,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return SendStringToUrl(
            url,
            method: HttpMethods.Get,
            accept: accept,
            requestFilter: requestFilter,
            responseFilter: responseFilter);
    }

    /// <summary>
    /// Gets the string from URL asynchronous.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="accept">The accept.</param>
    /// <param name="requestFilter">The request filter.</param>
    /// <param name="responseFilter">The response filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>System.Threading.Tasks.Task&lt;string&gt;.</returns>
    public static Task<string> GetStringFromUrlAsync(
        this string url,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return SendStringToUrlAsync(
            url,
            method: HttpMethods.Get,
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
    /// <returns>string.</returns>
    public static string PostStringToUrl(
        this string url,
        string? requestBody = null,
        string? contentType = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return SendStringToUrl(
            url,
            method: HttpMethods.Post,
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
    /// <returns>System.Threading.Tasks.Task&lt;string&gt;.</returns>
    public static Task<string> PostStringToUrlAsync(
        this string url,
        string? requestBody = null,
        string? contentType = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return SendStringToUrlAsync(
            url,
            method: HttpMethods.Post,
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
    /// <returns>string.</returns>
    public static string PostToUrl(
        this string url,
        string? formData = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return SendStringToUrl(
            url,
            method: HttpMethods.Post,
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
    /// <returns>System.Threading.Tasks.Task&lt;string&gt;.</returns>
    public static Task<string> PostToUrlAsync(
        this string url,
        string? formData = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return SendStringToUrlAsync(
            url,
            method: HttpMethods.Post,
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
    /// <returns>string.</returns>
    public static string PostToUrl(
        this string url,
        object? formData = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        string? postFormData = formData != null ? QueryStringSerializer.SerializeToString(formData) : null;

        return SendStringToUrl(
            url,
            method: HttpMethods.Post,
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
    /// <returns>System.Threading.Tasks.Task&lt;string&gt;.</returns>
    public static Task<string> PostToUrlAsync(
        this string url,
        object? formData = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        string? postFormData = formData != null ? QueryStringSerializer.SerializeToString(formData) : null;

        return SendStringToUrlAsync(
            url,
            method: HttpMethods.Post,
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
    /// <returns>string.</returns>
    public static string PostJsonToUrl(
        this string url,
        string json,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return SendStringToUrl(
            url,
            method: HttpMethods.Post,
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
    /// <returns>System.Threading.Tasks.Task&lt;string&gt;.</returns>
    public static Task<string> PostJsonToUrlAsync(
        this string url,
        string json,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return SendStringToUrlAsync(
            url,
            method: HttpMethods.Post,
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
    /// <returns>string.</returns>
    public static string PostJsonToUrl(
        this string url,
        object data,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return SendStringToUrl(
            url,
            method: HttpMethods.Post,
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
    /// <returns>System.Threading.Tasks.Task&lt;string&gt;.</returns>
    public static Task<string> PostJsonToUrlAsync(
        this string url,
        object data,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return SendStringToUrlAsync(
            url,
            method: HttpMethods.Post,
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
    /// <returns>string.</returns>
    public static string PostXmlToUrl(
        this string url,
        string xml,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return SendStringToUrl(
            url,
            method: HttpMethods.Post,
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
    /// <returns>System.Threading.Tasks.Task&lt;string&gt;.</returns>
    public static Task<string> PostXmlToUrlAsync(
        this string url,
        string xml,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return SendStringToUrlAsync(
            url,
            method: HttpMethods.Post,
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
    /// <returns>string.</returns>
    public static string PostCsvToUrl(
        this string url,
        string csv,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return SendStringToUrl(
            url,
            method: HttpMethods.Post,
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
    /// <returns>System.Threading.Tasks.Task&lt;string&gt;.</returns>
    public static Task<string> PostCsvToUrlAsync(
        this string url,
        string csv,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return SendStringToUrlAsync(
            url,
            method: HttpMethods.Post,
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
    /// <returns>string.</returns>
    public static string PutStringToUrl(
        this string url,
        string? requestBody = null,
        string? contentType = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return SendStringToUrl(
            url,
            method: HttpMethods.Put,
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
    /// <returns>System.Threading.Tasks.Task&lt;string&gt;.</returns>
    public static Task<string> PutStringToUrlAsync(
        this string url,
        string? requestBody = null,
        string? contentType = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return SendStringToUrlAsync(
            url,
            method: HttpMethods.Put,
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
    /// <returns>string.</returns>
    public static string PutToUrl(
        this string url,
        string? formData = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return SendStringToUrl(
            url,
            method: HttpMethods.Put,
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
    /// <returns>System.Threading.Tasks.Task&lt;string&gt;.</returns>
    public static Task<string> PutToUrlAsync(
        this string url,
        string? formData = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return SendStringToUrlAsync(
            url,
            method: HttpMethods.Put,
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
    /// <returns>string.</returns>
    public static string PutToUrl(
        this string url,
        object? formData = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        string? postFormData = formData != null ? QueryStringSerializer.SerializeToString(formData) : null;

        return SendStringToUrl(
            url,
            method: HttpMethods.Put,
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
    /// <returns>System.Threading.Tasks.Task&lt;string&gt;.</returns>
    public static Task<string> PutToUrlAsync(
        this string url,
        object? formData = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        string? postFormData = formData != null ? QueryStringSerializer.SerializeToString(formData) : null;

        return SendStringToUrlAsync(
            url,
            method: HttpMethods.Put,
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
    /// <returns>string.</returns>
    public static string PutJsonToUrl(
        this string url,
        string json,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return SendStringToUrl(
            url,
            method: HttpMethods.Put,
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
    /// <returns>System.Threading.Tasks.Task&lt;string&gt;.</returns>
    public static Task<string> PutJsonToUrlAsync(
        this string url,
        string json,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return SendStringToUrlAsync(
            url,
            method: HttpMethods.Put,
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
    /// <returns>string.</returns>
    public static string PutJsonToUrl(
        this string url,
        object data,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return SendStringToUrl(
            url,
            method: HttpMethods.Put,
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
    /// <returns>System.Threading.Tasks.Task&lt;string&gt;.</returns>
    public static Task<string> PutJsonToUrlAsync(
        this string url,
        object data,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return SendStringToUrlAsync(
            url,
            method: HttpMethods.Put,
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
    /// <returns>string.</returns>
    public static string PutXmlToUrl(
        this string url,
        string xml,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return SendStringToUrl(
            url,
            method: HttpMethods.Put,
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
    /// <returns>System.Threading.Tasks.Task&lt;string&gt;.</returns>
    public static Task<string> PutXmlToUrlAsync(
        this string url,
        string xml,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return SendStringToUrlAsync(
            url,
            method: HttpMethods.Put,
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
    /// <returns>string.</returns>
    public static string PutCsvToUrl(
        this string url,
        string csv,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return SendStringToUrl(
            url,
            method: HttpMethods.Put,
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
    /// <returns>System.Threading.Tasks.Task&lt;string&gt;.</returns>
    public static Task<string> PutCsvToUrlAsync(
        this string url,
        string csv,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return SendStringToUrlAsync(
            url,
            method: HttpMethods.Put,
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
    /// <returns>string.</returns>
    public static string PatchStringToUrl(
        this string url,
        string? requestBody = null,
        string? contentType = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return SendStringToUrl(
            url,
            method: HttpMethods.Patch,
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
    /// <returns>System.Threading.Tasks.Task&lt;string&gt;.</returns>
    public static Task<string> PatchStringToUrlAsync(
        this string url,
        string? requestBody = null,
        string? contentType = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return SendStringToUrlAsync(
            url,
            method: HttpMethods.Patch,
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
    /// <returns>string.</returns>
    public static string PatchToUrl(
        this string url,
        string? formData = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return SendStringToUrl(
            url,
            method: HttpMethods.Patch,
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
    /// <returns>System.Threading.Tasks.Task&lt;string&gt;.</returns>
    public static Task<string> PatchToUrlAsync(
        this string url,
        string? formData = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return SendStringToUrlAsync(
            url,
            method: HttpMethods.Patch,
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
    /// <returns>string.</returns>
    public static string PatchToUrl(
        this string url,
        object? formData = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        string? postFormData = formData != null ? QueryStringSerializer.SerializeToString(formData) : null;

        return SendStringToUrl(
            url,
            method: HttpMethods.Patch,
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
    /// <returns>System.Threading.Tasks.Task&lt;string&gt;.</returns>
    public static Task<string> PatchToUrlAsync(
        this string url,
        object? formData = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        string? postFormData = formData != null ? QueryStringSerializer.SerializeToString(formData) : null;

        return SendStringToUrlAsync(
            url,
            method: HttpMethods.Patch,
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
    /// <returns>string.</returns>
    public static string PatchJsonToUrl(
        this string url,
        string json,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return SendStringToUrl(
            url,
            method: HttpMethods.Patch,
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
    /// <returns>System.Threading.Tasks.Task&lt;string&gt;.</returns>
    public static Task<string> PatchJsonToUrlAsync(
        this string url,
        string json,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return SendStringToUrlAsync(
            url,
            method: HttpMethods.Patch,
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
    /// <returns>string.</returns>
    public static string PatchJsonToUrl(
        this string url,
        object data,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return SendStringToUrl(
            url,
            method: HttpMethods.Patch,
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
    /// <returns>System.Threading.Tasks.Task&lt;string&gt;.</returns>
    public static Task<string> PatchJsonToUrlAsync(
        this string url,
        object data,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return SendStringToUrlAsync(
            url,
            method: HttpMethods.Patch,
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
    /// <returns>string.</returns>
    public static string DeleteFromUrl(
        this string url,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return SendStringToUrl(
            url,
            method: HttpMethods.Delete,
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
    /// <returns>System.Threading.Tasks.Task&lt;string&gt;.</returns>
    public static Task<string> DeleteFromUrlAsync(
        this string url,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return SendStringToUrlAsync(
            url,
            method: HttpMethods.Delete,
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
    /// <returns>string.</returns>
    public static string OptionsFromUrl(
        this string url,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return SendStringToUrl(
            url,
            method: HttpMethods.Options,
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
    /// <returns>System.Threading.Tasks.Task&lt;string&gt;.</returns>
    public static Task<string> OptionsFromUrlAsync(
        this string url,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return SendStringToUrlAsync(
            url,
            method: HttpMethods.Options,
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
    /// <returns>string.</returns>
    public static string HeadFromUrl(
        this string url,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return SendStringToUrl(
            url,
            method: HttpMethods.Head,
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
    /// <returns>System.Threading.Tasks.Task&lt;string&gt;.</returns>
    public static Task<string> HeadFromUrlAsync(
        this string url,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return SendStringToUrlAsync(
            url,
            method: HttpMethods.Head,
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
    /// <returns>string.</returns>
    public static string SendStringToUrl(
        this string url,
        string method = HttpMethods.Post,
        string? requestBody = null,
        string? contentType = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return Create().SendStringToUrl(
            url,
            method: method,
            requestBody: requestBody,
            contentType: contentType,
            accept: accept,
            requestFilter: requestFilter,
            responseFilter: responseFilter);
    }

    /// <summary>
    /// Sends the string to URL.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="url">The URL.</param>
    /// <param name="method">The method.</param>
    /// <param name="requestBody">The request body.</param>
    /// <param name="contentType">Type of the content.</param>
    /// <param name="accept">The accept.</param>
    /// <param name="requestFilter">The request filter.</param>
    /// <param name="responseFilter">The response filter.</param>
    /// <returns>string.</returns>
    public static string SendStringToUrl(
        this HttpClient client,
        string url,
        string method = HttpMethods.Post,
        string? requestBody = null,
        string? contentType = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        var httpReq = new HttpRequestMessage(new HttpMethod(method), url);
        httpReq.Headers.Add(HttpHeaders.Accept, accept);

        if (requestBody != null)
        {
            httpReq.Content = new StringContent(requestBody, UseEncoding);
            if (contentType != null)
                httpReq.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
        }

        requestFilter?.Invoke(httpReq);

        var httpRes = client.Send(httpReq);
        responseFilter?.Invoke(httpRes);
        httpRes.EnsureSuccessStatusCode();
        return httpRes.Content.ReadAsStream().ReadToEnd(UseEncoding);
    }

    /// <summary>
    /// Sends the string to URL asynchronous.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="method">The method.</param>
    /// <param name="requestBody">The request body.</param>
    /// <param name="contentType">Type of the content.</param>
    /// <param name="accept">The accept.</param>
    /// <param name="requestFilter">The request filter.</param>
    /// <param name="responseFilter">The response filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>System.Threading.Tasks.Task&lt;string&gt;.</returns>
    public static Task<string> SendStringToUrlAsync(
        this string url,
        string method = HttpMethods.Post,
        string? requestBody = null,
        string? contentType = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return Create().SendStringToUrlAsync(
            url,
            method: method,
            requestBody: requestBody,
            contentType: contentType,
            accept: accept,
            requestFilter: requestFilter,
            responseFilter: responseFilter,
            token);
    }

    /// <summary>
    /// Send string to URL as an asynchronous operation.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="url">The URL.</param>
    /// <param name="method">The method.</param>
    /// <param name="requestBody">The request body.</param>
    /// <param name="contentType">Type of the content.</param>
    /// <param name="accept">The accept.</param>
    /// <param name="requestFilter">The request filter.</param>
    /// <param name="responseFilter">The response filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;string&gt; representing the asynchronous operation.</returns>
    public static async Task<string> SendStringToUrlAsync(
        this HttpClient client,
        string url,
        string method = HttpMethods.Post,
        string? requestBody = null,
        string? contentType = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        var httpReq = new HttpRequestMessage(new HttpMethod(method), url);
        httpReq.Headers.Add(HttpHeaders.Accept, accept);

        if (requestBody != null)
        {
            httpReq.Content = new StringContent(requestBody, UseEncoding);
            if (contentType != null)
                httpReq.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
        }

        requestFilter?.Invoke(httpReq);

        var httpRes = await client.SendAsync(httpReq, token).ConfigAwait();
        responseFilter?.Invoke(httpRes);
        httpRes.EnsureSuccessStatusCode();
        return await httpRes.Content.ReadAsStringAsync(token).ConfigAwait();
    }

    /// <summary>
    /// Gets the bytes from URL.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="accept">The accept.</param>
    /// <param name="requestFilter">The request filter.</param>
    /// <param name="responseFilter">The response filter.</param>
    /// <returns>byte[].</returns>
    public static byte[] GetBytesFromUrl(
        this string url,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return url.SendBytesToUrl(
            method: HttpMethods.Get,
            accept: accept,
            requestFilter: requestFilter,
            responseFilter: responseFilter);
    }

    /// <summary>
    /// Gets the bytes from URL asynchronous.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="accept">The accept.</param>
    /// <param name="requestFilter">The request filter.</param>
    /// <param name="responseFilter">The response filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>System.Threading.Tasks.Task&lt;byte[]&gt;.</returns>
    public static Task<byte[]> GetBytesFromUrlAsync(
        this string url,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return url.SendBytesToUrlAsync(
            method: HttpMethods.Get,
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
    /// <returns>byte[].</returns>
    public static byte[] PostBytesToUrl(
        this string url,
        byte[]? requestBody = null,
        string? contentType = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return SendBytesToUrl(
            url,
            method: HttpMethods.Post,
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
    /// <returns>System.Threading.Tasks.Task&lt;byte[]&gt;.</returns>
    public static Task<byte[]> PostBytesToUrlAsync(
        this string url,
        byte[]? requestBody = null,
        string? contentType = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return SendBytesToUrlAsync(
            url,
            method: HttpMethods.Post,
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
    /// <returns>byte[].</returns>
    public static byte[] PutBytesToUrl(
        this string url,
        byte[]? requestBody = null,
        string? contentType = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return SendBytesToUrl(
            url,
            method: HttpMethods.Put,
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
    /// <returns>System.Threading.Tasks.Task&lt;byte[]&gt;.</returns>
    public static Task<byte[]> PutBytesToUrlAsync(
        this string url,
        byte[]? requestBody = null,
        string? contentType = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return SendBytesToUrlAsync(
            url,
            method: HttpMethods.Put,
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
    /// <returns>byte[].</returns>
    public static byte[] SendBytesToUrl(
        this string url,
        string method = HttpMethods.Post,
        byte[]? requestBody = null,
        string? contentType = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return Create().SendBytesToUrl(
            url,
            method: method,
            requestBody: requestBody,
            contentType: contentType,
            accept: accept,
            requestFilter: requestFilter,
            responseFilter: responseFilter);
    }

    /// <summary>
    /// Sends the bytes to URL.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="url">The URL.</param>
    /// <param name="method">The method.</param>
    /// <param name="requestBody">The request body.</param>
    /// <param name="contentType">Type of the content.</param>
    /// <param name="accept">The accept.</param>
    /// <param name="requestFilter">The request filter.</param>
    /// <param name="responseFilter">The response filter.</param>
    /// <returns>byte[].</returns>
    public static byte[] SendBytesToUrl(
        this HttpClient client,
        string url,
        string method = HttpMethods.Post,
        byte[]? requestBody = null,
        string? contentType = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        var httpReq = new HttpRequestMessage(new HttpMethod(method), url);
        httpReq.Headers.Add(HttpHeaders.Accept, accept);

        if (requestBody != null)
        {
            httpReq.Content = new ReadOnlyMemoryContent(requestBody);
            if (contentType != null)
                httpReq.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
        }

        requestFilter?.Invoke(httpReq);

        var httpRes = client.Send(httpReq);
        responseFilter?.Invoke(httpRes);
        httpRes.EnsureSuccessStatusCode();
        return httpRes.Content.ReadAsStream().ReadFully();
    }

    /// <summary>
    /// Sends the bytes to URL asynchronous.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="method">The method.</param>
    /// <param name="requestBody">The request body.</param>
    /// <param name="contentType">Type of the content.</param>
    /// <param name="accept">The accept.</param>
    /// <param name="requestFilter">The request filter.</param>
    /// <param name="responseFilter">The response filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>System.Threading.Tasks.Task&lt;byte[]&gt;.</returns>
    public static Task<byte[]> SendBytesToUrlAsync(
        this string url,
        string method = HttpMethods.Post,
        byte[]? requestBody = null,
        string? contentType = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return Create().SendBytesToUrlAsync(
            url,
            method,
            requestBody,
            contentType,
            accept,
            requestFilter,
            responseFilter,
            token);
    }

    /// <summary>
    /// Send bytes to URL as an asynchronous operation.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="url">The URL.</param>
    /// <param name="method">The method.</param>
    /// <param name="requestBody">The request body.</param>
    /// <param name="contentType">Type of the content.</param>
    /// <param name="accept">The accept.</param>
    /// <param name="requestFilter">The request filter.</param>
    /// <param name="responseFilter">The response filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;byte[]&gt; representing the asynchronous operation.</returns>
    public static async Task<byte[]> SendBytesToUrlAsync(
        this HttpClient client,
        string url,
        string method = HttpMethods.Post,
        byte[]? requestBody = null,
        string? contentType = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        var httpReq = new HttpRequestMessage(new HttpMethod(method), url);
        httpReq.Headers.Add(HttpHeaders.Accept, accept);

        if (requestBody != null)
        {
            httpReq.Content = new ReadOnlyMemoryContent(requestBody);
            if (contentType != null)
                httpReq.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
        }

        requestFilter?.Invoke(httpReq);

        var httpRes = await client.SendAsync(httpReq, token).ConfigAwait();
        responseFilter?.Invoke(httpRes);
        httpRes.EnsureSuccessStatusCode();
        return await (await httpRes.Content.ReadAsStreamAsync(token).ConfigAwait()).ReadFullyAsync(token)
                   .ConfigAwait();
    }

    /// <summary>
    /// Gets the stream from URL.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="accept">The accept.</param>
    /// <param name="requestFilter">The request filter.</param>
    /// <param name="responseFilter">The response filter.</param>
    /// <returns>System.IO.Stream.</returns>
    public static Stream GetStreamFromUrl(
        this string url,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return url.SendStreamToUrl(
            method: HttpMethods.Get,
            accept: accept,
            requestFilter: requestFilter,
            responseFilter: responseFilter);
    }

    /// <summary>
    /// Gets the stream from URL asynchronous.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="accept">The accept.</param>
    /// <param name="requestFilter">The request filter.</param>
    /// <param name="responseFilter">The response filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>System.Threading.Tasks.Task&lt;System.IO.Stream&gt;.</returns>
    public static Task<Stream> GetStreamFromUrlAsync(
        this string url,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return url.SendStreamToUrlAsync(
            method: HttpMethods.Get,
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
    /// <returns>System.IO.Stream.</returns>
    public static Stream PostStreamToUrl(
        this string url,
        Stream? requestBody = null,
        string? contentType = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return SendStreamToUrl(
            url,
            method: HttpMethods.Post,
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
    /// <returns>System.Threading.Tasks.Task&lt;System.IO.Stream&gt;.</returns>
    public static Task<Stream> PostStreamToUrlAsync(
        this string url,
        Stream? requestBody = null,
        string? contentType = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return SendStreamToUrlAsync(
            url,
            method: HttpMethods.Post,
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
    /// <returns>System.IO.Stream.</returns>
    public static Stream PutStreamToUrl(
        this string url,
        Stream? requestBody = null,
        string? contentType = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return SendStreamToUrl(
            url,
            method: HttpMethods.Put,
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
    /// <returns>System.Threading.Tasks.Task&lt;System.IO.Stream&gt;.</returns>
    public static Task<Stream> PutStreamToUrlAsync(
        this string url,
        Stream? requestBody = null,
        string? contentType = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return SendStreamToUrlAsync(
            url,
            method: HttpMethods.Put,
            contentType: contentType,
            requestBody: requestBody,
            accept: accept,
            requestFilter: requestFilter,
            responseFilter: responseFilter,
            token: token);
    }

    /// <summary>
    /// Sends the stream to URL.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="method">The method.</param>
    /// <param name="requestBody">The request body.</param>
    /// <param name="contentType">Type of the content.</param>
    /// <param name="accept">The accept.</param>
    /// <param name="requestFilter">The request filter.</param>
    /// <param name="responseFilter">The response filter.</param>
    /// <returns>System.IO.Stream.</returns>
    public static Stream SendStreamToUrl(
        this string url,
        string method = HttpMethods.Post,
        Stream? requestBody = null,
        string? contentType = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return Create().SendStreamToUrl(
            url,
            method: method,
            requestBody: requestBody,
            contentType: contentType,
            accept: accept,
            requestFilter: requestFilter,
            responseFilter: responseFilter);
    }

    /// <summary>
    /// Sends the stream to URL.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="url">The URL.</param>
    /// <param name="method">The method.</param>
    /// <param name="requestBody">The request body.</param>
    /// <param name="contentType">Type of the content.</param>
    /// <param name="accept">The accept.</param>
    /// <param name="requestFilter">The request filter.</param>
    /// <param name="responseFilter">The response filter.</param>
    /// <returns>System.IO.Stream.</returns>
    public static Stream SendStreamToUrl(
        this HttpClient client,
        string url,
        string method = HttpMethods.Post,
        Stream? requestBody = null,
        string? contentType = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        var httpReq = new HttpRequestMessage(new HttpMethod(method), url);
        httpReq.Headers.Add(HttpHeaders.Accept, accept);

        if (requestBody != null)
        {
            httpReq.Content = new StreamContent(requestBody);
            if (contentType != null)
                httpReq.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
        }

        requestFilter?.Invoke(httpReq);

        var httpRes = client.Send(httpReq);
        responseFilter?.Invoke(httpRes);
        httpRes.EnsureSuccessStatusCode();
        return httpRes.Content.ReadAsStream();
    }

    /// <summary>
    /// Sends the stream to URL asynchronous.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="method">The method.</param>
    /// <param name="requestBody">The request body.</param>
    /// <param name="contentType">Type of the content.</param>
    /// <param name="accept">The accept.</param>
    /// <param name="requestFilter">The request filter.</param>
    /// <param name="responseFilter">The response filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>System.Threading.Tasks.Task&lt;System.IO.Stream&gt;.</returns>
    public static Task<Stream> SendStreamToUrlAsync(
        this string url,
        string method = HttpMethods.Post,
        Stream? requestBody = null,
        string? contentType = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return Create().SendStreamToUrlAsync(
            url,
            method: method,
            requestBody: requestBody,
            contentType: contentType,
            accept: accept,
            requestFilter: requestFilter,
            responseFilter: responseFilter,
            token);
    }

    /// <summary>
    /// Send stream to URL as an asynchronous operation.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="url">The URL.</param>
    /// <param name="method">The method.</param>
    /// <param name="requestBody">The request body.</param>
    /// <param name="contentType">Type of the content.</param>
    /// <param name="accept">The accept.</param>
    /// <param name="requestFilter">The request filter.</param>
    /// <param name="responseFilter">The response filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.IO.Stream&gt; representing the asynchronous operation.</returns>
    public static async Task<Stream> SendStreamToUrlAsync(
        this HttpClient client,
        string url,
        string method = HttpMethods.Post,
        Stream? requestBody = null,
        string? contentType = null,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        var httpReq = new HttpRequestMessage(new HttpMethod(method), url);
        httpReq.Headers.Add(HttpHeaders.Accept, accept);

        if (requestBody != null)
        {
            httpReq.Content = new StreamContent(requestBody);
            if (contentType != null)
                httpReq.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
        }

        requestFilter?.Invoke(httpReq);

        var httpRes = await client.SendAsync(httpReq, token).ConfigAwait();
        responseFilter?.Invoke(httpRes);
        httpRes.EnsureSuccessStatusCode();
        return await httpRes.Content.ReadAsStreamAsync(token).ConfigAwait();
    }

    /// <summary>
    /// Gets the response status.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <returns>System.Net.HttpStatusCode?.</returns>
    public static HttpStatusCode? GetResponseStatus(this string url)
    {
        try
        {
            var client = Create();
            var httpReq = new HttpRequestMessage(new HttpMethod(HttpMethods.Get), url);
            httpReq.Headers.Add(HttpHeaders.Accept, "*/*");
            var httpRes = client.Send(httpReq);
            return httpRes.StatusCode;
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
    /// <returns>System.Net.Http.HttpResponseMessage?.</returns>
    public static HttpResponseMessage? GetErrorResponse(this string url)
    {
        var client = Create();
        var httpReq = new HttpRequestMessage(new HttpMethod(HttpMethods.Get), url);
        httpReq.Headers.Add(HttpHeaders.Accept, "*/*");
        var httpRes = client.Send(httpReq);
        return httpRes.IsSuccessStatusCode ? null : httpRes;
    }

    /// <summary>
    /// Get error response as an asynchronous operation.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.Net.Http.HttpResponseMessage?&gt; representing the asynchronous operation.</returns>
    public static async Task<HttpResponseMessage?> GetErrorResponseAsync(
        this string url,
        CancellationToken token = default)
    {
        var client = Create();
        var httpReq = new HttpRequestMessage(new HttpMethod(HttpMethods.Get), url);
        httpReq.Headers.Add(HttpHeaders.Accept, "*/*");
        var httpRes = await client.SendAsync(httpReq, token).ConfigAwait();
        return httpRes.IsSuccessStatusCode ? null : httpRes;
    }

    /// <summary>
    /// Reads to end.
    /// </summary>
    /// <param name="webRes">The web resource.</param>
    /// <returns>string.</returns>
    public static string ReadToEnd(this HttpResponseMessage webRes)
    {
        using var stream = webRes.Content.ReadAsStream();
        return stream.ReadToEnd(UseEncoding);
    }

    /// <summary>
    /// Reads to end asynchronous.
    /// </summary>
    /// <param name="webRes">The web resource.</param>
    /// <returns>System.Threading.Tasks.Task&lt;string&gt;.</returns>
    public static Task<string> ReadToEndAsync(this HttpResponseMessage webRes)
    {
        using var stream = webRes.Content.ReadAsStream();
        return stream.ReadToEndAsync(UseEncoding);
    }

    /// <summary>
    /// Reads the lines.
    /// </summary>
    /// <param name="webRes">The web resource.</param>
    /// <returns>System.Collections.Generic.IEnumerable&lt;string&gt;.</returns>
    public static IEnumerable<string> ReadLines(this HttpResponseMessage webRes)
    {
        using var stream = webRes.Content.ReadAsStream();
        using var reader = new StreamReader(stream, UseEncoding, true, 1024, leaveOpen: true);
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            yield return line;
        }
    }

    /// <summary>
    /// Uploads the file.
    /// </summary>
    /// <param name="httpReq">The HTTP req.</param>
    /// <param name="fileStream">The file stream.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="mimeType">Type of the MIME.</param>
    /// <param name="accept">The accept.</param>
    /// <param name="method">The method.</param>
    /// <param name="field">The field.</param>
    /// <param name="requestFilter">The request filter.</param>
    /// <param name="responseFilter">The response filter.</param>
    /// <returns>System.Net.Http.HttpResponseMessage.</returns>
    public static HttpResponseMessage UploadFile(
        this HttpRequestMessage httpReq,
        Stream fileStream,
        string fileName,
        string mimeType,
        string accept = "*/*",
        string method = HttpMethods.Post,
        string field = "file",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return Create().UploadFile(
            httpReq,
            fileStream,
            fileName,
            mimeType,
            accept,
            method,
            field,
            requestFilter,
            responseFilter);
    }


    /// <summary>
    /// Uploads the file.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="httpReq">The HTTP req.</param>
    /// <param name="fileStream">The file stream.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="mimeType">Type of the MIME.</param>
    /// <param name="accept">The accept.</param>
    /// <param name="method">The method.</param>
    /// <param name="field">The field.</param>
    /// <param name="requestFilter">The request filter.</param>
    /// <param name="responseFilter">The response filter.</param>
    /// <returns>System.Net.Http.HttpResponseMessage.</returns>
    /// <exception cref="ArgumentException">nameof(httpReq.RequestUri)</exception>
    public static HttpResponseMessage UploadFile(
        this HttpClient client,
        HttpRequestMessage httpReq,
        Stream fileStream,
        string fileName,
        string? mimeType = null,
        string accept = "*/*",
        string method = HttpMethods.Post,
        string field = "file",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        if (httpReq.RequestUri == null)
            throw new ArgumentException(nameof(httpReq.RequestUri));

        httpReq.Method = new HttpMethod(method);
        httpReq.Headers.Add(HttpHeaders.Accept, accept);
        requestFilter?.Invoke(httpReq);

        using var content = new MultipartFormDataContent();
        var fileBytes = fileStream.ReadFully();
        using var fileContent = new ByteArrayContent(fileBytes, 0, fileBytes.Length);
        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                                                     {
                                                         Name = "file", FileName = fileName
                                                     };
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(mimeType ?? MimeTypes.GetMimeType(fileName));
        content.Add(fileContent, "file", fileName);

        var httpRes = client.Send(httpReq);
        responseFilter?.Invoke(httpRes);
        httpRes.EnsureSuccessStatusCode();
        return httpRes;
    }

    /// <summary>
    /// Uploads the file asynchronous.
    /// </summary>
    /// <param name="httpReq">The HTTP req.</param>
    /// <param name="fileStream">The file stream.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="mimeType">Type of the MIME.</param>
    /// <param name="accept">The accept.</param>
    /// <param name="method">The method.</param>
    /// <param name="field">The field.</param>
    /// <param name="requestFilter">The request filter.</param>
    /// <param name="responseFilter">The response filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>System.Threading.Tasks.Task&lt;System.Net.Http.HttpResponseMessage&gt;.</returns>
    public static Task<HttpResponseMessage> UploadFileAsync(
        this HttpRequestMessage httpReq,
        Stream fileStream,
        string fileName,
        string? mimeType = null,
        string accept = "*/*",
        string method = HttpMethods.Post,
        string field = "file",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        return Create().UploadFileAsync(
            httpReq,
            fileStream,
            fileName,
            mimeType,
            accept,
            method,
            field,
            requestFilter,
            responseFilter,
            token);
    }

    /// <summary>
    /// Upload file as an asynchronous operation.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="httpReq">The HTTP req.</param>
    /// <param name="fileStream">The file stream.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="mimeType">Type of the MIME.</param>
    /// <param name="accept">The accept.</param>
    /// <param name="method">The method.</param>
    /// <param name="field">The field.</param>
    /// <param name="requestFilter">The request filter.</param>
    /// <param name="responseFilter">The response filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.Net.Http.HttpResponseMessage&gt; representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">nameof(httpReq.RequestUri)</exception>
    public static async Task<HttpResponseMessage> UploadFileAsync(
        this HttpClient client,
        HttpRequestMessage httpReq,
        Stream fileStream,
        string fileName,
        string? mimeType = null,
        string accept = "*/*",
        string method = HttpMethods.Post,
        string field = "file",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        if (httpReq.RequestUri == null)
            throw new ArgumentException(nameof(httpReq.RequestUri));

        httpReq.Method = new HttpMethod(method);
        httpReq.Headers.Add(HttpHeaders.Accept, accept);
        requestFilter?.Invoke(httpReq);

        using var content = new MultipartFormDataContent();
        var fileBytes = await fileStream.ReadFullyAsync(token).ConfigAwait();
        using var fileContent = new ByteArrayContent(fileBytes, 0, fileBytes.Length);
        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                                                     {
                                                         Name = "file", FileName = fileName
                                                     };
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(mimeType ?? MimeTypes.GetMimeType(fileName));
        content.Add(fileContent, "file", fileName);

        var httpRes = await client.SendAsync(httpReq, token).ConfigAwait();
        responseFilter?.Invoke(httpRes);
        httpRes.EnsureSuccessStatusCode();
        return httpRes;
    }

    /// <summary>
    /// Uploads the file.
    /// </summary>
    /// <param name="httpReq">The HTTP req.</param>
    /// <param name="fileStream">The file stream.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <exception cref="ArgumentNullException">nameof(fileName)</exception>
    /// <exception cref="ArgumentException">Mime-type not found for file: " + fileName</exception>
    public static void UploadFile(this HttpRequestMessage httpReq, Stream fileStream, string fileName)
    {
        if (fileName == null)
            throw new ArgumentNullException(nameof(fileName));
        var mimeType = MimeTypes.GetMimeType(fileName);
        if (mimeType == null)
            throw new ArgumentException("Mime-type not found for file: " + fileName);

        UploadFile(httpReq, fileStream, fileName, mimeType);
    }

    /// <summary>
    /// Upload file as an asynchronous operation.
    /// </summary>
    /// <param name="webRequest">The web request.</param>
    /// <param name="fileStream">The file stream.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.Threading.Tasks.Task&gt; representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">nameof(fileName)</exception>
    /// <exception cref="ArgumentException">Mime-type not found for file: " + fileName</exception>
    public static async Task UploadFileAsync(
        this HttpRequestMessage webRequest,
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
    /// <returns>string.</returns>
    public static string PostXmlToUrl(
        this string url,
        object data,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return SendStringToUrl(
            url,
            method: HttpMethods.Post,
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
    /// <returns>string.</returns>
    public static string PostCsvToUrl(
        this string url,
        object data,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return SendStringToUrl(
            url,
            method: HttpMethods.Post,
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
    /// <returns>string.</returns>
    public static string PutXmlToUrl(
        this string url,
        object data,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return SendStringToUrl(
            url,
            method: HttpMethods.Put,
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
    /// <returns>string.</returns>
    public static string PutCsvToUrl(
        this string url,
        object data,
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        return SendStringToUrl(
            url,
            method: HttpMethods.Put,
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
    /// <param name="responseFilter">The response filter.</param>
    /// <returns>System.Net.Http.HttpResponseMessage.</returns>
    public static HttpResponseMessage PostFileToUrl(
        this string url,
        FileInfo uploadFileInfo,
        string uploadFileMimeType,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        var webReq = new HttpRequestMessage(HttpMethod.Post, url);
        using var fileStream = uploadFileInfo.OpenRead();
        var fileName = uploadFileInfo.Name;

        return webReq.UploadFile(
            fileStream,
            fileName,
            uploadFileMimeType,
            accept: accept,
            method: HttpMethods.Post,
            requestFilter: requestFilter,
            responseFilter: responseFilter);
    }

    /// <summary>
    /// Post file to URL as an asynchronous operation.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="uploadFileInfo">The upload file information.</param>
    /// <param name="uploadFileMimeType">Type of the upload file MIME.</param>
    /// <param name="accept">The accept.</param>
    /// <param name="requestFilter">The request filter.</param>
    /// <param name="responseFilter">The response filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.Net.Http.HttpResponseMessage&gt; representing the asynchronous operation.</returns>
    public static async Task<HttpResponseMessage> PostFileToUrlAsync(
        this string url,
        FileInfo uploadFileInfo,
        string uploadFileMimeType,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        var webReq = new HttpRequestMessage(HttpMethod.Post, url);
        await using var fileStream = uploadFileInfo.OpenRead();
        var fileName = uploadFileInfo.Name;

        return await webReq.UploadFileAsync(
                   fileStream,
                   fileName,
                   uploadFileMimeType,
                   accept: accept,
                   method: HttpMethods.Post,
                   requestFilter: requestFilter,
                   responseFilter: responseFilter,
                   token: token).ConfigAwait();
    }

    /// <summary>
    /// Puts the file to URL.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="uploadFileInfo">The upload file information.</param>
    /// <param name="uploadFileMimeType">Type of the upload file MIME.</param>
    /// <param name="accept">The accept.</param>
    /// <param name="requestFilter">The request filter.</param>
    /// <param name="responseFilter">The response filter.</param>
    /// <returns>System.Net.Http.HttpResponseMessage.</returns>
    public static HttpResponseMessage PutFileToUrl(
        this string url,
        FileInfo uploadFileInfo,
        string uploadFileMimeType,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null)
    {
        var webReq = new HttpRequestMessage(HttpMethod.Put, url);
        using var fileStream = uploadFileInfo.OpenRead();
        var fileName = uploadFileInfo.Name;

        return webReq.UploadFile(
            fileStream,
            fileName,
            uploadFileMimeType,
            accept: accept,
            method: HttpMethods.Post,
            requestFilter: requestFilter,
            responseFilter: responseFilter);
    }

    /// <summary>
    /// Put file to URL as an asynchronous operation.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="uploadFileInfo">The upload file information.</param>
    /// <param name="uploadFileMimeType">Type of the upload file MIME.</param>
    /// <param name="accept">The accept.</param>
    /// <param name="requestFilter">The request filter.</param>
    /// <param name="responseFilter">The response filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.Net.Http.HttpResponseMessage&gt; representing the asynchronous operation.</returns>
    public static async Task<HttpResponseMessage> PutFileToUrlAsync(
        this string url,
        FileInfo uploadFileInfo,
        string uploadFileMimeType,
        string accept = "*/*",
        Action<HttpRequestMessage>? requestFilter = null,
        Action<HttpResponseMessage>? responseFilter = null,
        CancellationToken token = default)
    {
        var webReq = new HttpRequestMessage(HttpMethod.Put, url);
        await using var fileStream = uploadFileInfo.OpenRead();
        var fileName = uploadFileInfo.Name;

        return await webReq.UploadFileAsync(
                   fileStream,
                   fileName,
                   uploadFileMimeType,
                   accept: accept,
                   method: HttpMethods.Post,
                   requestFilter: requestFilter,
                   responseFilter: responseFilter,
                   token: token).ConfigAwait();
    }

    /// <summary>
    /// Adds the header.
    /// </summary>
    /// <param name="res">The resource.</param>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    public static void AddHeader(this HttpRequestMessage res, string name, string value) =>
        res.WithHeader(name, value);

    /// <summary>
    /// Returns first Request Header in HttpRequestMessage Headers and Content.Headers
    /// </summary>
    /// <param name="req">The req.</param>
    /// <param name="name">The name.</param>
    /// <returns>string?.</returns>
    public static string? GetHeader(this HttpRequestMessage req, string name)
    {
        if (RequestHeadersResolver.TryGetValue(name, out var fn))
            return fn(req);

        return req.Headers.TryGetValues(name, out var headers)
                   ? headers.FirstOrDefault()
                   :
                   req.Content?.Headers.TryGetValues(name, out var contentHeaders) == true
                       ?
                       contentHeaders!.FirstOrDefault()
                       : null;
    }

    /// <summary>
    /// Gets or sets the request headers resolver.
    /// </summary>
    /// <value>The request headers resolver.</value>
    public static Dictionary<string, Func<HttpRequestMessage, string?>> RequestHeadersResolver { get; set; } =
        new(StringComparer.OrdinalIgnoreCase) { };

    /// <summary>
    /// Gets or sets the response headers resolver.
    /// </summary>
    /// <value>The response headers resolver.</value>
    public static Dictionary<string, Func<HttpResponseMessage, string?>> ResponseHeadersResolver { get; set; } =
        new(StringComparer.OrdinalIgnoreCase) { };

    /// <summary>
    /// Returns first Response Header in HttpResponseMessage Headers and Content.Headers
    /// </summary>
    /// <param name="res">The resource.</param>
    /// <param name="name">The name.</param>
    /// <returns>string?.</returns>
    public static string? GetHeader(this HttpResponseMessage res, string name)
    {
        if (ResponseHeadersResolver.TryGetValue(name, out var fn))
            return fn(res);

        return res.Headers.TryGetValues(name, out var headers)
                   ? headers.FirstOrDefault()
                   :
                   res.Content?.Headers.TryGetValues(name, out var contentHeaders) == true
                       ?
                       contentHeaders!.FirstOrDefault()
                       : null;
    }

    /// <summary>
    /// Withes the header.
    /// </summary>
    /// <param name="httpReq">The HTTP req.</param>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Net.Http.HttpRequestMessage.</returns>
    /// <exception cref="NotSupportedException">Can't set ContentType before Content is populated</exception>
    public static HttpRequestMessage WithHeader(this HttpRequestMessage httpReq, string name, string value)
    {
        if (name.Equals(HttpHeaders.Authorization, StringComparison.OrdinalIgnoreCase))
        {
            httpReq.Headers.Authorization =
                new AuthenticationHeaderValue(value.LeftPart(' '), value.RightPart(' '));
        }
        else if (name.Equals(HttpHeaders.ContentType, StringComparison.OrdinalIgnoreCase))
        {
            if (httpReq.Content == null)
                throw new NotSupportedException("Can't set ContentType before Content is populated");
            httpReq.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(value);
        }
        else if (name.Equals(HttpHeaders.Referer, StringComparison.OrdinalIgnoreCase))
        {
            httpReq.Headers.Referrer = new Uri(value);
        }
        else if (name.Equals(HttpHeaders.UserAgent, StringComparison.OrdinalIgnoreCase))
        {
            httpReq.Headers.UserAgent.ParseAdd(value);
        }
        else
        {
            httpReq.Headers.Add(name, value);
        }

        return httpReq;
    }

    /// <summary>
    /// Populate HttpRequestMessage with a simpler, untyped API
    /// Syntax compatible with HttpWebRequest
    /// </summary>
    /// <param name="httpReq">The HTTP req.</param>
    /// <param name="configure">The configure.</param>
    /// <returns>System.Net.Http.HttpRequestMessage.</returns>
    /// <exception cref="NotSupportedException">Can't set ContentType before Content is populated</exception>
    public static HttpRequestMessage With(this HttpRequestMessage httpReq, Action<HttpRequestConfig> configure)
    {
        var config = new HttpRequestConfig();
        configure(config);

        var headers = config.Headers;

        if (config.Accept != null)
        {
            httpReq.Headers.Accept.Clear(); //override or consistent behavior
            httpReq.Headers.Accept.Add(new(config.Accept));
        }

        if (config.UserAgent != null)
            headers.Add(new(HttpHeaders.UserAgent, config.UserAgent));
        if (config.ContentType != null)
        {
            if (httpReq.Content == null)
                throw new NotSupportedException("Can't set ContentType before Content is populated");
            httpReq.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(config.ContentType);
        }

        if (config.Referer != null)
            httpReq.Headers.Referrer = new Uri(config.Referer);
        if (config.Authorization != null)
            httpReq.Headers.Authorization = new AuthenticationHeaderValue(
                config.Authorization.Name,
                config.Authorization.Value);
        if (config.Range != null)
            httpReq.Headers.Range = new RangeHeaderValue(config.Range.From, config.Range.To);
        if (config.Expect != null)
            httpReq.Headers.Expect.Add(new(config.Expect));

        if (config.TransferEncodingChunked != null)
            httpReq.Headers.TransferEncodingChunked = config.TransferEncodingChunked.Value;
        else if (config.TransferEncoding?.Length > 0)
        {
            foreach (var enc in config.TransferEncoding)
            {
                httpReq.Headers.TransferEncoding.Add(new(enc));
            }
        }

        foreach (var entry in headers)
        {
            httpReq.WithHeader(entry.Name, entry.Value);
        }

        return httpReq;
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
        List<HttpRequestConfig.NameValue>? headers = null)
    {
        var client = Create();
        var httpReq = new HttpRequestMessage(HttpMethod.Get, downloadUrl).With(
            c =>
                {
                    c.Accept = "*/*";
                    if (headers != null) c.Headers = headers;
                });

        var httpRes = client.Send(httpReq);
        httpRes.EnsureSuccessStatusCode();

        using var fs = new FileStream(fileName, FileMode.CreateNew);
        var bytes = httpRes.Content.ReadAsStream().ReadFully();
        fs.Write(bytes);
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
    /// <returns>bool.</returns>
    public static bool MatchesContentType(this HttpResponseMessage res, string matchesContentType) =>
        MimeTypes.MatchesContentType(res.GetHeader(HttpHeaders.ContentType), matchesContentType);

    /// <summary>
    /// Gets the length of the content.
    /// </summary>
    /// <param name="res">The resource.</param>
    /// <returns>long?.</returns>
    public static long? GetContentLength(this HttpResponseMessage res) => res.Content.Headers.ContentLength;
}