// ***********************************************************************
// <copyright file="DateTimeConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
//

using System;
using System.Globalization;

#if NET9_0_OR_GREATER
namespace ServiceStack.OrmLite.Converters;

/// <summary>
/// Class TimeOnlyConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.TimeSpanAsIntConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.TimeSpanAsIntConverter" />
public class TimeOnlyConverter : TimeSpanAsIntConverter
{
    /// <summary>
    /// Quoted Value in SQL Statement
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public override string ToQuotedString(Type fieldType, object value)
    {
        return ((TimeOnly)value).Ticks.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Parameterized value in parameterized queries
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object ToDbValue(Type fieldType, object value)
    {
        var timespan = (TimeOnly)value;
        return timespan.Ticks;
    }

    /// <summary>
    /// From the database value.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object FromDbValue(Type fieldType, object value)
    {
        var ticks = (long)this.ConvertNumber(typeof(long), value);
        var timeSpan = TimeSpan.FromTicks(ticks);
        return TimeOnly.FromTimeSpan(timeSpan);
    }
}

#endif
