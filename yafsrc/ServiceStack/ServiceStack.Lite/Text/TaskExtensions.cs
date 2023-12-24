// ***********************************************************************
// <copyright file="TaskExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Text;

using System;

/// <summary>
/// Class TaskExtensions.
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// Unwraps if single exception.
    /// </summary>
    /// <param name="ex">The ex.</param>
    /// <returns>Exception.</returns>
    public static Exception UnwrapIfSingleException(this Exception ex)
    {
        if (ex is not AggregateException aex)
            return ex;

        if (aex.InnerExceptions is { Count: 1 })
            return aex.InnerExceptions[0].UnwrapIfSingleException();

        return aex;
    }
}