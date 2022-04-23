// ***********************************************************************
// <copyright file="SqlServerDateTime2Converter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Data;
using System.Globalization;

namespace ServiceStack.OrmLite.SqlServer.Converters;

/// <summary>
/// Class SqlServerDateTime2Converter.
/// Implements the <see cref="ServiceStack.OrmLite.SqlServer.Converters.SqlServerDateTimeConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.SqlServer.Converters.SqlServerDateTimeConverter" />
public class SqlServerDateTime2Converter : SqlServerDateTimeConverter
{
    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "DATETIME2";

    /// <summary>
    /// The date time format
    /// </summary>
    const string DateTimeFormat = "yyyyMMdd HH:mm:ss.fffffff";

    /// <summary>
    /// Used in DB Params. Defaults to DbType.String
    /// </summary>
    /// <value>The type of the database.</value>
    public override DbType DbType => DbType.DateTime2;

    /// <summary>
    /// Converts to quotedstring.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public override string ToQuotedString(Type fieldType, object value)
    {
        return DateTimeFmt((DateTime)value, DateTimeFormat);
    }

    /// <summary>
    /// Froms the database value.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object FromDbValue(Type fieldType, object value)
    {
        if (value is string str && DateTime.TryParseExact(str, DateTimeFormat, null, DateTimeStyles.None, out var date))
            return date;

        return base.FromDbValue(fieldType, value);
    }
}