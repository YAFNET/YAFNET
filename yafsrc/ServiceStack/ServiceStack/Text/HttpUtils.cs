// ***********************************************************************
// <copyright file="HttpUtils.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
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
    /// The user agent
    /// </summary>
    public static string UserAgent = "ServiceStack.Text" +
#if NET7_0_OR_GREATER
        "/net6"
#elif NETSTANDARD2_0
        "/std2.0"
#elif NETFX
                                     "/net472"
#else
        "/unknown"
#endif
        ;

    /// <summary>
    /// Gets or sets the use encoding.
    /// </summary>
    /// <value>The use encoding.</value>
    public static Encoding UseEncoding { get; set; } = new UTF8Encoding(false);

    /// <summary>
    /// Adds the query parameters.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>System.String.</returns>
    public static string AddQueryParams(this string url, Dictionary<string, string> args)
    {
        var sb = StringBuilderCache.Allocate()
            .Append(url);

        var i = 0;

        foreach (var entry in args)
        {
            if (entry.Value == null)
                continue;

            sb.Append(i++ == 0 && url.IndexOf('?') == -1 ? '?' : '&');
            sb.Append($"{entry.Key.UrlEncode()}={entry.Value.UrlEncode()}");
        }

        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// Adds the query parameters.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="queryParams">The query parameters.</param>
    /// <returns>System.String.</returns>
    public static string AddQueryParams(this string url, NameValueCollection queryParams)
    {
        var sb = StringBuilderCache.Allocate()
            .Append(url);

        foreach (string key in queryParams.AllKeys)
        {
            var values = queryParams.GetValues(key);
            if (values == null)
                continue;

            var i = 0;
            foreach (var value in values)
            {
                sb.Append(i++ == 0 && url.IndexOf('?') == -1 ? '?' : '&');
                sb.Append($"{key.UrlEncode()}={value.UrlEncode()}");
            }
        }

        return StringBuilderCache.ReturnAndFree(sb);
    }

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
        url ??= "";

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
        url ??= "";

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

                var newUrl = url.Substring(0, existingKeyPos + key.Length + 1) + "=" + val.UrlEncode()
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
        url ??= "";

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
        url ??= "";

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

                var newUrl = url.Substring(0, existingKeyPos + key.Length + 1) + "=" + val.UrlEncode()
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
    /// Gets the request stream asynchronous.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>Task&lt;Stream&gt;.</returns>
    public static Task<Stream> GetRequestStreamAsync(this WebRequest request)
    {
        return GetRequestStreamAsync((HttpWebRequest) request);
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
            request.BeginGetRequestStream(
                iar =>
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
                    },
                null);
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
    public static Task<TBase> ConvertTo<TDerived, TBase>(this Task<TDerived> task)
        where TDerived : TBase
    {
        var tcs = new TaskCompletionSource<TBase>();
        task.ContinueWith(t => tcs.SetResult(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion);
        task.ContinueWith(
            t => tcs.SetException(t.Exception.InnerExceptions),
            TaskContinuationOptions.OnlyOnFaulted);
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
        return GetResponseAsync((HttpWebRequest) request).ConvertTo<HttpWebResponse, WebResponse>();
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
            request.BeginGetResponse(
                iar =>
                    {
                        try
                        {
                            var response = (HttpWebResponse) request.EndGetResponse(iar);
                            tcs.SetResult(response);
                        }
                        catch (Exception exc)
                        {
                            tcs.SetException(exc);
                        }
                    },
                null);
        }
        catch (Exception exc)
        {
            tcs.SetException(exc);
        }

        return tcs.Task;
    }

    /// <summary>
    /// Determines whether the specified ex is any300.
    /// </summary>
    /// <param name="ex">The ex.</param>
    /// <returns><c>true</c> if the specified ex is any300; otherwise, <c>false</c>.</returns>
    public static bool IsAny300(this Exception ex)
    {
        var status = ex.GetStatus();
        return status is >= HttpStatusCode.MultipleChoices and < HttpStatusCode.BadRequest;
    }

    /// <summary>
    /// Determines whether the specified ex is any400.
    /// </summary>
    /// <param name="ex">The ex.</param>
    /// <returns><c>true</c> if the specified ex is any400; otherwise, <c>false</c>.</returns>
    public static bool IsAny400(this Exception ex)
    {
        var status = ex.GetStatus();
        return status is >= HttpStatusCode.BadRequest and < HttpStatusCode.InternalServerError;
    }

    /// <summary>
    /// Determines whether the specified ex is any500.
    /// </summary>
    /// <param name="ex">The ex.</param>
    /// <returns><c>true</c> if the specified ex is any500; otherwise, <c>false</c>.</returns>
    public static bool IsAny500(this Exception ex)
    {
        var status = ex.GetStatus();
        return status >= HttpStatusCode.InternalServerError && (int) status < 600;
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
    /// Gets the status.
    /// </summary>
    /// <param name="ex">The ex.</param>
    /// <returns>System.Nullable&lt;HttpStatusCode&gt;.</returns>
    public static HttpStatusCode? GetStatus(this Exception ex)
    {
#if NET7_0_OR_GREATER
        if (ex is System.Net.Http.HttpRequestException httpEx)
            return GetStatus(httpEx);
#endif

        if (ex is WebException webEx)
            return GetStatus(webEx);

        if (ex is IHasStatusCode hasStatus)
            return (HttpStatusCode) hasStatus.StatusCode;

        return null;
    }

#if NET7_0_OR_GREATER
    public static HttpStatusCode? GetStatus(this System.Net.Http.HttpRequestException ex) => ex.StatusCode;
#endif

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
        if (ex is not WebException webEx || webEx.Response == null
                                         || webEx.Status != WebExceptionStatus.ProtocolError)
            return null;

        var errorResponse = (HttpWebResponse) webEx.Response;
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
        if (ex is not WebException webEx || webEx.Response == null
                                         || webEx.Status != WebExceptionStatus.ProtocolError)
            return null;

        var errorResponse = (HttpWebResponse) webEx.Response;
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
        while (reader.ReadLine() is { } line)
        {
            yield return line;
        }
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