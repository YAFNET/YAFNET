// ***********************************************************************
// <copyright file="FeatureSupport.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Data;

namespace ServiceStack.OrmLite.Dapper;

/// <summary>
/// Handles variances in features per DBMS
/// </summary>
internal class FeatureSupport
{
    /// <summary>
    /// The default
    /// </summary>
    private static readonly FeatureSupport
        Default = new(false),

        Postgres = new(true);

    /// <summary>
    /// Gets the feature set based on the passed connection
    /// </summary>
    /// <param name="connection">The connection to get supported features for.</param>
    /// <returns>FeatureSupport.</returns>
    public static FeatureSupport Get(IDbConnection connection)
    {
        string name = connection?.GetType().Name;
        if (string.Equals(name, "npgsqlconnection", StringComparison.OrdinalIgnoreCase)) return Postgres;
        return Default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureSupport"/> class.
    /// </summary>
    /// <param name="arrays">if set to <c>true</c> [arrays].</param>
    private FeatureSupport(bool arrays)
    {
        Arrays = arrays;
    }

    /// <summary>
    /// True if the db supports array columns e.g. Postgresql
    /// </summary>
    /// <value><c>true</c> if arrays; otherwise, <c>false</c>.</value>
    public bool Arrays { get; }
}