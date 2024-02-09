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
using System.Threading.Tasks;

namespace ServiceStack.OrmLite.Base.Text;

public static partial class HttpUtils
{
    public static string UserAgent = "ServiceStack.Text/net472";

    public static Encoding UseEncoding { get; set; } = new UTF8Encoding(false);


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

    public static bool IsInternalServerError(this Exception ex)
    {
        return GetStatus(ex) == HttpStatusCode.InternalServerError;
    }

    public static HttpStatusCode? GetStatus(this Exception ex)
    {
        if (ex is WebException webEx)
            return GetStatus(webEx);

        if (ex is IHasStatusCode hasStatus)
            return (HttpStatusCode) hasStatus.StatusCode;

        return null;
    }

    public static HttpStatusCode? GetStatus(this WebException webEx)
    {
        var httpRes = webEx?.Response as HttpWebResponse;
        return httpRes?.StatusCode;
    }

    public static bool HasStatus(this Exception ex, HttpStatusCode statusCode)
    {
        return GetStatus(ex) == statusCode;
    }

    public static string ReadToEnd(this WebResponse webRes)
    {
        using var stream = webRes.GetResponseStream();
        return stream.ReadToEnd(UseEncoding);
    }

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
public interface IHasStatusCode
{
    int StatusCode { get; }
}