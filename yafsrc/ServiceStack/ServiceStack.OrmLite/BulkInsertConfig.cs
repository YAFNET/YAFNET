// ***********************************************************************
// <copyright file="BulkInsertConfig.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#nullable enable

using System.Collections.Generic;

namespace ServiceStack.OrmLite;

/// <summary>
/// Enum BulkInsertMode
/// </summary>
public enum BulkInsertMode
{
    /// <summary>
    /// The binary
    /// </summary>
    Binary,

    /// <summary>
    /// The CSV
    /// </summary>
    Csv,

    /// <summary>
    /// The SQL
    /// </summary>
    Sql,
}

/// <summary>
/// Class BulkInsertConfig.
/// </summary>
public class BulkInsertConfig
{
    /// <summary>
    /// Gets or sets the size of the batch.
    /// </summary>
    /// <value>The size of the batch.</value>
    public int BatchSize { get; set; } = 1000;

    /// <summary>
    /// Gets or sets the mode.
    /// </summary>
    /// <value>The mode.</value>
    public BulkInsertMode Mode { get; set; } = BulkInsertMode.Csv;

    /// <summary>
    /// Gets or sets the insert fields.
    /// </summary>
    /// <value>The insert fields.</value>
    public ICollection<string>? InsertFields { get; set; } = null;
}