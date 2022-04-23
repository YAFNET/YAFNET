// ***********************************************************************
// <copyright file="MySqlDateTimeOffsetConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using ServiceStack.OrmLite.Converters;

namespace ServiceStack.OrmLite.MySql.Converters;

/// <summary>
/// Class MySqlDateTimeOffsetConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.DateTimeOffsetConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.DateTimeOffsetConverter" />
public class MySqlDateTimeOffsetConverter : DateTimeOffsetConverter
{
    /// <summary>
    /// Gets the column definition.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "VARCHAR(255)";
}