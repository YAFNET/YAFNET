// ***********************************************************************
// <copyright file="OrmLiteLog.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

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

        return result;
    }
}