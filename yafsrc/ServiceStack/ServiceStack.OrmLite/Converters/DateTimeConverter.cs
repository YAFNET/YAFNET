// ***********************************************************************
// <copyright file="DateTimeConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Data;
using System.Globalization;

using ServiceStack.Text.Common;

namespace ServiceStack.OrmLite.Converters;

/// <summary>
/// Class DateTimeConverter.
/// Implements the <see cref="ServiceStack.OrmLite.OrmLiteConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.OrmLiteConverter" />
public class DateTimeConverter : OrmLiteConverter
{
    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "DATETIME";

    /// <summary>
    /// Used in DB Params. Defaults to DbType.String
    /// </summary>
    /// <value>The type of the database.</value>
    public override DbType DbType => DbType.DateTime;

    /// <summary>
    /// Gets or sets the date style.
    /// </summary>
    /// <value>The date style.</value>
    public DateTimeKind DateStyle { get; set; }

    /// <summary>
    /// Quoted Value in SQL Statement
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public override string ToQuotedString(Type fieldType, object value)
    {
        var dateTime = (DateTime)value;
        return DateTimeFmt(dateTime, "yyyy-MM-dd HH:mm:ss.fff");
    }

    /// <summary>
    /// Dates the time FMT.
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <param name="dateTimeFormat">The date time format.</param>
    /// <returns>System.String.</returns>
    public virtual string DateTimeFmt(DateTime dateTime, string dateTimeFormat)
    {
        if (DateStyle == DateTimeKind.Utc && dateTime.Kind == DateTimeKind.Local)
            dateTime = dateTime.ToUniversalTime();

        if (DateStyle == DateTimeKind.Local && dateTime.Kind != DateTimeKind.Local)
            dateTime = dateTime.ToLocalTime();

        return DialectProvider.GetQuotedValue(dateTime.ToString(dateTimeFormat, CultureInfo.InvariantCulture), typeof(string));
    }

    /// <summary>
    /// Parameterized value in parameterized queries
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object ToDbValue(Type fieldType, object value)
    {
        if (value == null)
        {
            return null;
        }

        var dateTime = (DateTime)value;
        if (DateStyle == DateTimeKind.Utc && dateTime.Kind == DateTimeKind.Local)
        {
            dateTime = dateTime.ToUniversalTime();
        }
        else if (DateStyle == DateTimeKind.Local && dateTime.Kind != DateTimeKind.Local)
        {
            dateTime = dateTime.Kind == DateTimeKind.Utc
                           ? dateTime.ToLocalTime()
                           : DateTime.SpecifyKind(dateTime, DateTimeKind.Utc).ToLocalTime();
        }

        return dateTime;
    }

    /// <summary>
    /// Value from DB to Populate on POCO Data Model with
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object FromDbValue(Type fieldType, object value)
    {
        if (value is string strValue)
        {
            value = DateTimeSerializer.ParseShortestXsdDateTime(strValue);
        }

        return FromDbValue(value);
    }

    /// <summary>
    /// Froms the database value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public virtual object FromDbValue(object value)
    {
        var dateTime = (DateTime)value;
        if (DateStyle == DateTimeKind.Utc)
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

        if (DateStyle == DateTimeKind.Local && dateTime.Kind != DateTimeKind.Local)
        {
            dateTime = dateTime.Kind == DateTimeKind.Utc
                           ? dateTime.ToLocalTime()
                           : DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
        }

        return dateTime;
    }
}