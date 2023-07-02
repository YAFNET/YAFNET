// ***********************************************************************
// <copyright file="SqlInValues.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite;

using System.Collections;

/// <summary>
/// Class SqlInValues.
/// </summary>
public class SqlInValues
{
    /// <summary>
    /// The empty in
    /// </summary>
    public const string EmptyIn = "NULL";

    /// <summary>
    /// The values
    /// </summary>
    private readonly IEnumerable values;
    /// <summary>
    /// The dialect provider
    /// </summary>
    private readonly IOrmLiteDialectProvider dialectProvider;

    /// <summary>
    /// Gets the count.
    /// </summary>
    /// <value>The count.</value>
    public int Count { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlInValues" /> class.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <param name="dialectProvider">The dialect provider.</param>
    public SqlInValues(IEnumerable values, IOrmLiteDialectProvider dialectProvider = null)
    {
        this.values = values;
        this.dialectProvider = dialectProvider ?? OrmLiteConfig.DialectProvider;

        if (values == null) return;
        foreach (var value in values)
        {
            ++Count;
        }
    }

    /// <summary>
    /// Converts to sqlinstring.
    /// </summary>
    /// <returns>System.String.</returns>
    public string ToSqlInString()
    {
        return Count == 0
                   ? EmptyIn
                   : OrmLiteUtils.SqlJoin(values, dialectProvider);
    }

    /// <summary>
    /// Gets the values.
    /// </summary>
    /// <returns>IEnumerable.</returns>
    public IEnumerable GetValues()
    {
        return values;
    }
}