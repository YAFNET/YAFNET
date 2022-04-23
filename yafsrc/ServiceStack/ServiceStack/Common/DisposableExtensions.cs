// ***********************************************************************
// <copyright file="DisposableExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack;

using System;
using System.Collections.Generic;

using ServiceStack.Logging;

/// <summary>
/// Class DisposableExtensions.
/// </summary>
public static class DisposableExtensions
{
    /// <summary>
    /// Disposes the specified log.
    /// </summary>
    /// <param name="resources">The resources.</param>
    /// <param name="log">The log.</param>
    public static void Dispose(this IEnumerable<IDisposable> resources, ILog log)
    {
        foreach (var disposable in resources)
        {
            if (disposable == null)
                continue;

            try
            {
                disposable.Dispose();
            }
            catch (Exception ex)
            {
                log?.Error($"Error disposing of '{disposable.GetType().FullName}'", ex);
            }
        }
    }

    /// <summary>
    /// Disposes the specified resources.
    /// </summary>
    /// <param name="resources">The resources.</param>
    public static void Dispose(this IEnumerable<IDisposable> resources)
    {
        Dispose(resources, null);
    }

    /// <summary>
    /// Disposes the specified disposables.
    /// </summary>
    /// <param name="disposables">The disposables.</param>
    public static void Dispose(params IDisposable[] disposables)
    {
        Dispose(disposables, null);
    }

    /// <summary>
    /// Runs the specified run action then dispose.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="disposable">The disposable.</param>
    /// <param name="runActionThenDispose">The run action then dispose.</param>
    public static void Run<T>(this T disposable, Action<T> runActionThenDispose)
        where T : IDisposable
    {
        using (disposable)
        {
            runActionThenDispose(disposable);
        }
    }
}