// ***********************************************************************
// <copyright file="PostgreSqlBoolConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.PostgreSQL.Converters;

using ServiceStack.OrmLite.Converters;

/// <summary>
/// Class PostgreSqlBoolConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.BoolConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.BoolConverter" />
public class PostgreSqlBoolConverter : BoolConverter
{
    /// <summary>
    /// Gets the column definition.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "BOOLEAN";
}