// ***********************************************************************
// <copyright file="SqlServerDateTimeConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using ServiceStack.OrmLite.Converters;
using System.Globalization;

namespace ServiceStack.OrmLite.SqlServer.Converters;

/// <summary>
/// Class SqlServerDateTimeConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.DateTimeConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.DateTimeConverter" />
public class SqlServerDateTimeConverter : DateTimeConverter
{
    /// <summary>
    /// The date time format
    /// </summary>
    const string DateTimeFormat = "yyyyMMdd HH:mm:ss.fff";

    /// <summary>
    /// Quoted Value in SQL Statement
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