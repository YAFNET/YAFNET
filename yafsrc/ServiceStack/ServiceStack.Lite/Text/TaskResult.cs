// ***********************************************************************
// <copyright file="TaskResult.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Text;

using System.Threading.Tasks;

/// <summary>
/// Class TaskResult.
/// </summary>
public static class TaskResult
{
    /// <summary>
    /// The zero
    /// </summary>
    public static Task<int> Zero { get; }
    /// <summary>
    /// The one
    /// </summary>
    public static Task<int> One { get; }
    /// <summary>
    /// The true
    /// </summary>
    public readonly static Task<bool> True;
    /// <summary>
    /// The false
    /// </summary>
    public readonly static Task<bool> False;
    /// <summary>
    /// The finished
    /// </summary>
    public readonly static Task Finished;
    /// <summary>
    /// The canceled
    /// </summary>
    public readonly static Task Canceled;

    /// <summary>
    /// Initializes static members of the <see cref="TaskResult" /> class.
    /// </summary>
    static TaskResult()
    {
        Finished = ((object)null).InTask();
        True = true.InTask();
        False = false.InTask();
        Zero = 0.InTask();
        One = 1.InTask();

        var tcs = new TaskCompletionSource<object>();
        tcs.SetCanceled();
        Canceled = tcs.Task;
    }
}

/// <summary>
/// Class TaskResult.
/// </summary>
/// <typeparam name="T"></typeparam>
internal class TaskResult<T>
{
    /// <summary>
    /// The canceled
    /// </summary>
    public readonly static Task<T> Canceled;
    /// <summary>
    /// The default
    /// </summary>
    public readonly static Task<T> Default;

    /// <summary>
    /// Initializes static members of the <see cref="TaskResult{T}" /> class.
    /// </summary>
    static TaskResult()
    {
        Default = ((T)typeof(T).GetDefaultValue()).InTask();

        var tcs = new TaskCompletionSource<T>();
        tcs.SetCanceled();
        Canceled = tcs.Task;
    }
}