// ***********************************************************************
// <copyright file="SqlServerSpecialConverters.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using ServiceStack.OrmLite.Converters;

namespace ServiceStack.OrmLite.SqlServer.Converters;

/// <summary>
/// Class SqlServerRowVersionConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.RowVersionConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.RowVersionConverter" />
public class SqlServerRowVersionConverter : RowVersionConverter
{
    /// <summary>
    /// Gets the column definition.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "rowversion";
}