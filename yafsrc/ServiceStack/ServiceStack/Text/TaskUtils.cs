// ***********************************************************************
// <copyright file="TaskUtils.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Text;

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Class TaskUtils.
/// </summary>
public static class TaskUtils
{
    /// <summary>
    /// Ins the task.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result">The result.</param>
    /// <returns>Task&lt;T&gt;.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<T> InTask<T>(this T result) => Task.FromResult(result);

    /// <summary>
    /// Ins the task.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ex">The ex.</param>
    /// <returns>Task&lt;T&gt;.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<T> InTask<T>(this Exception ex)
    {
        var taskSource = new TaskCompletionSource<T>();
        taskSource.TrySetException(ex);
        return taskSource.Task;
    }

    /// <summary>
    /// Thens the specified function.
    /// </summary>
    /// <typeparam name="From">The type of from.</typeparam>
    /// <typeparam name="To">The type of to.</typeparam>
    /// <param name="task">The task.</param>
    /// <param name="fn">The function.</param>
    /// <returns>Task&lt;To&gt;.</returns>
    public static Task<To> Then<From, To>(this Task<From> task, Func<From, To> fn)
    {
        var tcs = new TaskCompletionSource<To>();
        task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                    tcs.TrySetException(t.Exception.InnerExceptions);
                else if (t.IsCanceled)
                    tcs.TrySetCanceled();
                else
                    tcs.TrySetResult(fn(t.Result));
            }, TaskContinuationOptions.ExecuteSynchronously);

        return tcs.Task;
    }

    /// <summary>
    /// Sleeps the specified time ms.
    /// </summary>
    /// <param name="timeMs">The time ms.</param>
    public static void Sleep(int timeMs)
    {
        Thread.Sleep(timeMs);
    }
}