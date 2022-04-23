// ***********************************************************************
// <copyright file="Extensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Threading.Tasks;

namespace ServiceStack.OrmLite.Dapper;

/// <summary>
/// Class Extensions.
/// </summary>
internal static class Extensions
{
    /// <summary>
    /// Creates a <see cref="Task{TResult}" /> with a less specific generic parameter that perfectly mirrors the
    /// state of the specified <paramref name="task" />.
    /// </summary>
    /// <typeparam name="TFrom">The type of the t from.</typeparam>
    /// <typeparam name="TTo">The type of the t to.</typeparam>
    /// <param name="task">The task.</param>
    /// <returns>Task&lt;TTo&gt;.</returns>
    /// <exception cref="System.ArgumentNullException">task</exception>
    internal static Task<TTo> CastResult<TFrom, TTo>(this Task<TFrom> task)
        where TFrom : TTo
    {
        if (task is null) throw new ArgumentNullException(nameof(task));

        if (task.Status == TaskStatus.RanToCompletion)
            return Task.FromResult((TTo)task.Result);

        var source = new TaskCompletionSource<TTo>();
        task.ContinueWith(OnTaskCompleted<TFrom, TTo>, state: source, TaskContinuationOptions.ExecuteSynchronously);
        return source.Task;
    }

    /// <summary>
    /// Called when [task completed].
    /// </summary>
    /// <typeparam name="TFrom">The type of the t from.</typeparam>
    /// <typeparam name="TTo">The type of the t to.</typeparam>
    /// <param name="completedTask">The completed task.</param>
    /// <param name="state">The state.</param>
    private static void OnTaskCompleted<TFrom, TTo>(Task<TFrom> completedTask, object state)
        where TFrom : TTo
    {
        var source = (TaskCompletionSource<TTo>)state;

        switch (completedTask.Status)
        {
            case TaskStatus.RanToCompletion:
                source.SetResult(completedTask.Result);
                break;
            case TaskStatus.Canceled:
                source.SetCanceled();
                break;
            case TaskStatus.Faulted:
                source.SetException(completedTask.Exception.InnerExceptions);
                break;
        }
    }
}