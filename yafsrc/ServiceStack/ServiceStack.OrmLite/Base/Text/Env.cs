// ***********************************************************************
// <copyright file="Env.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Class Env.
/// </summary>
public static class Env
{
    /// <summary>
    /// Initializes static members of the <see cref="Env" /> class.
    /// </summary>
    /// <exception cref="System.ArgumentException">PclExport.Instance needs to be initialized</exception>
    static Env()
    {
        if (PclExport.Instance == null)
        {
            throw new ArgumentException("PclExport.Instance needs to be initialized");
        }

        try
        {
            var fxDesc = RuntimeInformation.FrameworkDescription;
            IsMono = fxDesc.Contains("Mono");
        }
        catch (Exception)
        {
        } //throws PlatformNotSupportedException in AWS lambda
    }


    /// <summary>
    /// Gets or sets a value indicating whether this instance is mono.
    /// </summary>
    /// <value><c>true</c> if this instance is mono; otherwise, <c>false</c>.</value>
    public static bool IsMono { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [strict mode].
    /// </summary>
    /// <value><c>true</c> if [strict mode]; otherwise, <c>false</c>.</value>
    public static bool StrictMode {
        get;
        set => Config.Instance.ThrowOnError = field = value;
    }

    /// <summary>
    /// The get release date.
    /// </summary>
    /// <returns>The <see cref="DateTime" />.</returns>
    public static DateTime GetReleaseDate()
    {
        return new DateTime(2001, 01, 01);
    }

    /// <summary>
    /// The continue on captured context
    /// </summary>
    public const bool ContinueOnCapturedContext = false;

    /// <summary>
    /// Configurations the await.
    /// </summary>
    /// <param name="task">The task.</param>
    /// <returns>ConfiguredTaskAwaitable.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConfiguredTaskAwaitable ConfigAwait(this Task task)
    {
        return task.ConfigureAwait(ContinueOnCapturedContext);
    }

    /// <summary>
    /// Configurations the await.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="task">The task.</param>
    /// <returns>ConfiguredTaskAwaitable&lt;T&gt;.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConfiguredTaskAwaitable<T> ConfigAwait<T>(this Task<T> task)
    {
        return task.ConfigureAwait(ContinueOnCapturedContext);
    }

    /// <summary>
    /// Configurations the await.
    /// </summary>
    /// <param name="task">The task.</param>
    /// <returns>ConfiguredValueTaskAwaitable.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConfiguredValueTaskAwaitable ConfigAwait(this ValueTask task)
    {
        return task.ConfigureAwait(ContinueOnCapturedContext);
    }
}