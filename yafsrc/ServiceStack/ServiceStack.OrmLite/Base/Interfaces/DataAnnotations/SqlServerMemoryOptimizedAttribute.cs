// ***********************************************************************
// <copyright file="SqlServerMemoryOptimizedAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.DataAnnotations;

// https://msdn.microsoft.com/en-us/library/dn553122.aspx
/// <summary>
/// Class SqlServerMemoryOptimizedAttribute.
/// Implements the <see cref="ServiceStack.AttributeBase" />
/// </summary>
/// <seealso cref="ServiceStack.AttributeBase" />
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class SqlServerMemoryOptimizedAttribute : AttributeBase
{
    /// <summary>
    /// Gets or sets the durability.
    /// </summary>
    /// <value>The durability.</value>
    public SqlServerDurability? Durability { get; set; }
}

/// <summary>
/// Enum SqlServerDurability
/// </summary>
public enum SqlServerDurability
{
    /// <summary>
    /// The schema only
    /// </summary>
    SchemaOnly, // (non-durable table) recreated upon server restart, data is lost, no transaction logging and checkpoints
    /// <summary>
    /// The schema and data
    /// </summary>
    SchemaAndData  // (durable table) data persists upon server restart
}