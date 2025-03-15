// ***********************************************************************
// <copyright file="DateTimeConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;

#if NET9_0_OR_GREATER
namespace ServiceStack.OrmLite.Converters;

/// <summary>
/// Class DateOnlyConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.DateTimeConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.DateTimeConverter" />
public class DateOnlyConverter : DateTimeConverter
{
    /// <summary>
    /// Quoted Value in SQL Statement
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public override string ToQuotedString(Type fieldType, object value)
    {
        var dateOnly = (DateOnly)value;
        return this.DateTimeFmt(dateOnly.ToDateTime(default, DateTimeKind.Local), "yyyy-MM-dd HH:mm:ss.fff");
    }

    /// <summary>
    /// Parameterized value in parameterized queries
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object ToDbValue(Type fieldType, object value)
    {
        var dateOnly = (DateOnly)value;
        return base.ToDbValue(typeof(DateTime), dateOnly.ToDateTime(default, DateTimeKind.Local));
    }

    /// <summary>
    /// From the database value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object FromDbValue(object value)
    {
        var dateTime = (DateTime)base.FromDbValue(value);
        if (dateTime.Kind != DateTimeKind.Local)
        {
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
        }

        var dateOnly = DateOnly.FromDateTime(dateTime);
        return dateOnly;
    }
}

#endif
