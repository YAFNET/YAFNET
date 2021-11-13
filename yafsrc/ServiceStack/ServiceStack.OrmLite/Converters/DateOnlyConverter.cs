// ***********************************************************************
// <copyright file="DateTimeConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;

#if NET6_0
namespace ServiceStack.OrmLite.Converters;

public class DateOnlyConverter : DateTimeConverter
{
    public override string ToQuotedString(Type fieldType, object value)
    {
        var dateOnly = (DateOnly)value;
        return DateTimeFmt(dateOnly.ToDateTime(default, DateTimeKind.Local), "yyyy-MM-dd HH:mm:ss.fff");
    }

    public override object ToDbValue(Type fieldType, object value)
    {
        var dateOnly = (DateOnly)value;
        return base.ToDbValue(typeof(DateTime), dateOnly.ToDateTime(default, DateTimeKind.Local));
    }

    public override object FromDbValue(object value)
    {
        var dateTime = (DateTime)base.FromDbValue(value);
        if (dateTime.Kind != DateTimeKind.Local)
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
        var dateOnly = DateOnly.FromDateTime(dateTime);
        return dateOnly;
    }
}

#endif
