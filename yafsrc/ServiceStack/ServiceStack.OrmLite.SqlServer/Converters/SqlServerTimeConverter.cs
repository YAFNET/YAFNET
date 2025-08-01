// ***********************************************************************
// <copyright file="SqlServerTimeConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Data;

namespace ServiceStack.OrmLite.SqlServer.Converters;

/// <summary>
/// Class SqlServerTimeConverter.
/// Implements the <see cref="ServiceStack.OrmLite.OrmLiteConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.OrmLiteConverter" />
public class SqlServerTimeConverter : OrmLiteConverter
{
    /// <summary>
    /// The time span offset
    /// </summary>
    private readonly static DateTime timeSpanOffset = new(1900, 01, 01);

    /// <summary>
    /// Gets or sets the precision.
    /// </summary>
    /// <value>The precision.</value>
    public int? Precision { get; set; }

    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => this.Precision != null
                                                   ? $"TIME({this.Precision.Value})"
                                                   : "TIME";

    /// <summary>
    /// Used in DB Params. Defaults to DbType.String
    /// </summary>
    /// <value>The type of the database.</value>
    public override DbType DbType => DbType.DateTime;

    /// <summary>
    /// Converts to dbvalue.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object ToDbValue(Type fieldType, object value)
    {
        var timeSpan = (TimeSpan)value;
        return timeSpanOffset + timeSpan;
    }
}