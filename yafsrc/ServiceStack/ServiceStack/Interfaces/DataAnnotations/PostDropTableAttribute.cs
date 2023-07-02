// ***********************************************************************
// <copyright file="PostDropTableAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.DataAnnotations;

/// <summary>
/// Class PostDropTableAttribute.
/// Implements the <see cref="ServiceStack.AttributeBase" />
/// </summary>
/// <seealso cref="ServiceStack.AttributeBase" />
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class PostDropTableAttribute : AttributeBase
{
    /// <summary>
    /// Gets or sets the SQL.
    /// </summary>
    /// <value>The SQL.</value>
    public string Sql { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PostDropTableAttribute" /> class.
    /// </summary>
    /// <param name="sql">The SQL.</param>
    public PostDropTableAttribute(string sql)
    {
        Sql = sql;
    }
}