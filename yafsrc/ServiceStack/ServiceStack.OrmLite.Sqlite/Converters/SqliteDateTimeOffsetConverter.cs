// ***********************************************************************
// <copyright file="SqliteDateTimeOffsetConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.Sqlite.Converters;

using System;

using ServiceStack.OrmLite.Converters;

/// <summary>
/// Class SqliteDateTimeOffsetConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.DateTimeOffsetConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.DateTimeOffsetConverter" />
public class SqliteDateTimeOffsetConverter : DateTimeOffsetConverter
{
    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "VARCHAR(8000)";

    /// <summary>
    /// Quoted Value in SQL Statement
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public override string ToQuotedString(Type fieldType, object value)
    {
        var dateTimeOffsetValue = (DateTimeOffset)value;
        return base.DialectProvider.GetQuotedValue(dateTimeOffsetValue.ToString("o"), typeof(string));
    }

    /// <summary>
    /// Converts to dbvalue.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object ToDbValue(Type fieldType, object value)
    {
        var dateTimeOffsetValue = (DateTimeOffset)value;
        return dateTimeOffsetValue.ToString("o");
    }
}