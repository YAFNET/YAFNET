// ***********************************************************************
// <copyright file="ValidateAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;

namespace ServiceStack;

/// <summary>
/// Worker Thread names
/// </summary>
public static class Workers
{
    /// <summary>
    /// Worker name for OrmLite default DB Connection
    /// </summary>
    public const string AppDb = "app.db";
    public const string JobsDb = "jobs.db";
}

/// <summary>
/// Maintain Locks 
/// </summary>
public static class Locks
{
    public readonly static Lock AppDb = new();
    public readonly static Lock JobsDb = new();

    public static Dictionary<string, Lock> Workers { get; } = new()
    {
        [ServiceStack.Workers.AppDb] = AppDb,
        [ServiceStack.Workers.JobsDb] = JobsDb
    };

    public static Dictionary<string, Lock> NamedConnections { get; } = new();

    public static void AddLock(string name)
    {
        NamedConnections[name] = name switch
        {
            ServiceStack.Workers.AppDb => AppDb,
            ServiceStack.Workers.JobsDb => JobsDb,
            _ => new Lock()
        };
    }

    public static Lock? TryGetLock(string worker)
    {
        return Workers.GetValueOrDefault(worker);

    }

    public static Lock GetDbLock(string? namedConnection = null)
    {
        return namedConnection != null
            ? NamedConnections.TryGetValue(namedConnection, out var oLock)
                ? oLock
                : throw new ArgumentException("Named Connection does not exist", nameof(namedConnection))
            : AppDb;
    }
}

/// <summary>
/// Execute AutoQuery Create/Update/Delete Request DTO in a background thread
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class WorkerAttribute(string name) : AttributeBase
{
    public string Name { get; set; } = name;
}