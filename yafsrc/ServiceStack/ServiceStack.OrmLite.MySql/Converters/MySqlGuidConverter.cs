// ***********************************************************************
// <copyright file="MySqlGuidConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using ServiceStack.OrmLite.Converters;

namespace ServiceStack.OrmLite.MySql.Converters;

/// <summary>
/// Class MySqlGuidConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.GuidConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.GuidConverter" />
public class MySqlGuidConverter : GuidConverter
{
    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "CHAR(36)";

    /// <summary>
    /// Converts to quotedstring.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public override string ToQuotedString(Type fieldType, object value)
    {
        var guid = (Guid)value;
        return DialectProvider.GetQuotedValue(guid.ToString("d"), typeof(string));
    }
}