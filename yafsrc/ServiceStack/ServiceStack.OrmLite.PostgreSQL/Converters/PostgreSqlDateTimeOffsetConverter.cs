// ***********************************************************************
// <copyright file="PostgreSqlDateTimeOffsetConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.PostgreSQL.Converters;

using System;
using System.Globalization;

using ServiceStack.OrmLite.Converters;

/// <summary>
/// Class PostgreSqlDateTimeOffsetConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.DateTimeOffsetConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.DateTimeOffsetConverter" />
public class PostgreSqlDateTimeOffsetConverter : DateTimeOffsetConverter
{
    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "timestamp with time zone";

    /// <summary>
    /// Quoted Value in SQL Statement
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public override string ToQuotedString(Type fieldType, object value)
    {
        var dateValue = (DateTimeOffset) value;
        const string iso8601Format = "yyyy-MM-dd HH:mm:ss.fff zzz";
        return base.DialectProvider.GetQuotedValue(dateValue.ToString(iso8601Format, CultureInfo.InvariantCulture), typeof(string));
    }

    /// <summary>
    /// Converts to dbvalue.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object ToDbValue(Type fieldType, object value)
    {
        return value;
    }
}