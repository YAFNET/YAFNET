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
            {
                httpReq.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
            }
        }

        requestFilter?.Invoke(httpReq);

        var httpRes = client.Send(httpReq);
        responseFilter?.Invoke(httpRes);
        httpRes.EnsureSuccessStatusCode();
        return httpRes.Content.ReadAsStream().ReadToEnd(UseEncoding);
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
    /// Reads the lines.
    /// </summary>
    /// <param name="webRes">The web resource.</param>
    /// <returns>System.Collections.Generic.IEnumerable&lt;string&gt;.</returns>
    public static IEnumerable<string> ReadLines(this HttpResponseMessage webRes)
    {
        using var stream = webRes.Content.ReadAsStream();
        using var reader = new StreamReader(stream, UseEncoding, true, 1024, leaveOpen: true);
        while (reader.ReadLine() is { } line)
        {
            yield return line;
        }
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
            {
                throw new NotSupportedException("Can't set ContentType before Content is populated");
            }

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
        {
            headers.Add(new(HttpHeaders.UserAgent, config.UserAgent));
        }

        if (config.ContentType != null)
        {
            if (httpReq.Content == null)
            {
                throw new NotSupportedException("Can't set ContentType before Content is populated");
            }

            httpReq.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(config.ContentType);
        }

        if (config.Referer != null)
        {
            httpReq.Headers.Referrer = new Uri(config.Referer);
        }

        if (config.Authorization != null)
        {
            httpReq.Headers.Authorization = new AuthenticationHeaderValue(
                config.Authorization.Name,
                config.Authorization.Value);
        }

        if (config.Range != null)
        {
            httpReq.Headers.Range = new RangeHeaderValue(config.Range.From, config.Range.To);
        }

        if (config.Expect != null)
        {
            httpReq.Headers.Expect.Add(new(config.Expect));
        }

        if (config.TransferEncodingChunked != null)
        {
            httpReq.Headers.TransferEncodingChunked = config.TransferEncodingChunked.Value;
        }
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
}