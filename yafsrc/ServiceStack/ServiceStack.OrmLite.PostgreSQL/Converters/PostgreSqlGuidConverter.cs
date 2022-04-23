// ***********************************************************************
// <copyright file="PostgreSqlGuidConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.PostgreSQL.Converters;

using System;

using ServiceStack.OrmLite.Converters;

/// <summary>
/// Class PostgreSqlGuidConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.GuidConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.GuidConverter" />
public class PostgreSqlGuidConverter : GuidConverter
{
    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "UUID";

    /// <summary>
    /// Converts to quotedstring.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public override string ToQuotedString(Type fieldType, object value)
    {
        var guidValue = (Guid)value;
        return base.DialectProvider.GetQuotedValue(guidValue.ToString("N"), typeof(string));
    }
}