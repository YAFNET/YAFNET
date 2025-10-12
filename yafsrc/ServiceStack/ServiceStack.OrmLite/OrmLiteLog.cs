// ***********************************************************************
// <copyright file="OrmLiteLog.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Data;

using ServiceStack.Logging;

namespace ServiceStack.OrmLite;

/// <summary>
/// Adding logging in own class to delay premature static log configuration
/// </summary>
public static class OrmLiteLog
{
    public static ILog Log = LogManager.GetLogger(typeof(OrmLiteLog));

    static internal T WithLog<T>(this IDbCommand cmd, T result, ILog log = null)
    {
        log ??= Log;

        if (!log.IsDebugEnabled || cmd is not OrmLiteCommand ormCmd)
        {
            return result;
        }

        var elapsed = ormCmd.GetElapsedTime();
        if (elapsed == TimeSpan.Zero)
        {
            elapsed = System.Diagnostics.Stopwatch.GetElapsedTime(ormCmd.StartTimestamp, System.Diagnostics.Stopwatch.GetTimestamp());
        }

        if (elapsed != TimeSpan.Zero)
        {
            log.DebugFormat("TIME: {0:N3}ms", elapsed.TotalMilliseconds);
        }
        return result;
    }

}