// ***********************************************************************
// <copyright file="SqlServerByteArrayConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using ServiceStack.OrmLite.Converters;

namespace ServiceStack.OrmLite.SqlServer.Converters;

/// <summary>
/// Class SqlServerByteArrayConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.ByteArrayConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.ByteArrayConverter" />
public class SqlServerByteArrayConverter : ByteArrayConverter
{
    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "VARBINARY(MAX)";

    /// <summary>
    /// Converts to quotedstring.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public override string ToQuotedString(Type fieldType, object value)
    {
        return "0x" + BitConverter.ToString((byte[])value).Replace("-", "");
    }
}