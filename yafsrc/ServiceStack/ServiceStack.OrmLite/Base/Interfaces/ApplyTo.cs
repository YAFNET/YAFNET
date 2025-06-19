// ***********************************************************************
// <copyright file="ApplyTo.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#nullable enable
using System;

namespace ServiceStack;

/// <summary>
/// Enum ApplyTo
/// </summary>
[Flags]
public enum ApplyTo
{
    /// <summary>
    /// The none
    /// </summary>
    None = 0,
    /// <summary>
    /// All
    /// </summary>
    All = int.MaxValue,
    /// <summary>
    /// The get
    /// </summary>
    Get = 1 << 0,
    /// <summary>
    /// The post
    /// </summary>
    Post = 1 << 1,
    /// <summary>
    /// The put
    /// </summary>
    Put = 1 << 2,
    /// <summary>
    /// The delete
    /// </summary>
    Delete = 1 << 3,
    /// <summary>
    /// The patch
    /// </summary>
    Patch = 1 << 4,
    /// <summary>
    /// The options
    /// </summary>
    Options = 1 << 5,
    /// <summary>
    /// The head
    /// </summary>
    Head = 1 << 6,
    /// <summary>
    /// The connect
    /// </summary>
    Connect = 1 << 7,
    /// <summary>
    /// The trace
    /// </summary>
    Trace = 1 << 8,
    /// <summary>
    /// The property patch
    /// </summary>
    PropPatch = 1 << 10,
    /// <summary>
    /// The mk col
    /// </summary>
    MkCol = 1 << 11,
    /// <summary>
    /// The copy
    /// </summary>
    Copy = 1 << 12,
    /// <summary>
    /// The move
    /// </summary>
    Move = 1 << 13,
    /// <summary>
    /// The lock
    /// </summary>
    Lock = 1 << 14,
    /// <summary>
    /// The un lock
    /// </summary>
    UnLock = 1 << 15,
    /// <summary>
    /// The report
    /// </summary>
    Report = 1 << 16,
    /// <summary>
    /// The check in
    /// </summary>
    CheckIn = 1 << 18,
    /// <summary>
    /// The update
    /// </summary>
    Update = 1 << 21,
    /// <summary>
    /// The label
    /// </summary>
    Label = 1 << 22,
    /// <summary>
    /// The merge
    /// </summary>
    Merge = 1 << 23,
    /// <summary>
    /// The mk activity
    /// </summary>
    MkActivity = 1 << 24,
    /// <summary>
    /// The order patch
    /// </summary>
    OrderPatch = 1 << 25,
    /// <summary>
    /// The acl
    /// </summary>
    Acl = 1 << 26,
    /// <summary>
    /// The search
    /// </summary>
    Search = 1 << 27,
    /// <summary>
    /// The version control
    /// </summary>
    VersionControl = 1 << 28,
    /// <summary>
    /// The base line control
    /// </summary>
    BaseLineControl = 1 << 29
}