// ***********************************************************************
// <copyright file="ByteArrayConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System.Data;

namespace ServiceStack.OrmLite.Converters;

/// <summary>
/// Class ByteArrayConverter.
/// Implements the <see cref="ServiceStack.OrmLite.OrmLiteConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.OrmLiteConverter" />
public class ByteArrayConverter : OrmLiteConverter
{
    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "BLOB";
    /// <summary>
    /// Gets the type of the database.
    /// </summary>
    /// <value>The type of the database.</value>
    public override DbType DbType => DbType.Binary;
}