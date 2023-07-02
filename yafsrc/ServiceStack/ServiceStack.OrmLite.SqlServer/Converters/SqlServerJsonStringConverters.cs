// ***********************************************************************
// <copyright file="SqlServerJsonStringConverters.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.SqlServer.Converters;

using System;

using ServiceStack.Text;

/// <summary>
/// Class SqlServerJsonStringConverter.
/// Implements the <see cref="ServiceStack.OrmLite.SqlServer.Converters.SqlServerStringConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.SqlServer.Converters.SqlServerStringConverter" />
public class SqlServerJsonStringConverter : SqlServerStringConverter
{
    /// <summary>
    /// json string to object
    /// Value from DB to Populate on POCO Data Model with
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object FromDbValue(Type fieldType, object value)
    {
        if (value is string raw && fieldType.HasAttributeCached<SqlJsonAttribute>())
        {
            return JsonSerializer.DeserializeFromString(raw, fieldType);
        }

        return base.FromDbValue(fieldType, value);
    }

    /// <summary>
    /// object to json string
    /// Parameterized value in parameterized queries
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object ToDbValue(Type fieldType, object value)
    {
        return value.GetType().HasAttributeCached<SqlJsonAttribute>()
                   ? JsonSerializer.SerializeToString(value, value.GetType())
                   : base.ToDbValue(fieldType, value);
    }
}

/// <summary>
/// Class SqlJsonAttribute.
/// Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Class)]
public class SqlJsonAttribute : Attribute
{
}