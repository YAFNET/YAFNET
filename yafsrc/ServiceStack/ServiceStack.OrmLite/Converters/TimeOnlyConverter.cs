// ***********************************************************************
// <copyright file="DateTimeConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
//

using System;
using System.Globalization;

#if NET7_0_OR_GREATER
namespace ServiceStack.OrmLite.Converters;

public class TimeOnlyConverter : TimeSpanAsIntConverter
{
    public override string ToQuotedString(Type fieldType, object value)
    {
        return ((TimeOnly)value).Ticks.ToString(CultureInfo.InvariantCulture);
    }

    public override object ToDbValue(Type fieldType, object value)
    {
        var timespan = (TimeOnly)value;
        return timespan.Ticks;
    }

    public override object FromDbValue(Type fieldType, object value)
    {
        var ticks = (long)this.ConvertNumber(typeof(long), value);
        var timeSpan = TimeSpan.FromTicks(ticks);
        return TimeOnly.FromTimeSpan(timeSpan);
    }
}

#endif
