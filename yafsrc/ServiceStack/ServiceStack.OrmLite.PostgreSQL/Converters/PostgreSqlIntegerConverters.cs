// ***********************************************************************
// <copyright file="PostgreSqlIntegerConverters.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.PostgreSQL.Converters;

using System;
using System.Data;

using ServiceStack.OrmLite.Converters;

//throws unknown type exceptions in parameterized queries, e.g: p.DbType = DbType.SByte
/// <summary>
/// Class PostrgreSqlSByteConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.SByteConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.SByteConverter" />
public class PostrgreSqlSByteConverter : SByteConverter
{
    /// <summary>
    /// Gets the type of the database.
    /// </summary>
    /// <value>The type of the database.</value>
    public override DbType DbType => DbType.Byte;
}

/// <summary>
/// Class PostrgreSqlUInt16Converter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.UInt16Converter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.UInt16Converter" />
public class PostrgreSqlUInt16Converter : UInt16Converter
{
    /// <summary>
    /// Gets the type of the database.
    /// </summary>
    /// <value>The type of the database.</value>
    public override DbType DbType => DbType.Int16;

    /// <summary>
    /// Parameterized value in parameterized queries
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object ToDbValue(Type fieldType, object value)
    {
        return this.ConvertNumber(typeof(short), value);
    }
}

/// <summary>
/// Class PostrgreSqlUInt32Converter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.UInt32Converter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.UInt32Converter" />
public class PostrgreSqlUInt32Converter : UInt32Converter
{
    /// <summary>
    /// Gets the type of the database.
    /// </summary>
    /// <value>The type of the database.</value>
    public override DbType DbType => DbType.Int32;

    /// <summary>
    /// Parameterized value in parameterized queries
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object ToDbValue(Type fieldType, object value)
    {
        return this.ConvertNumber(typeof(int), value);
    }
}

/// <summary>
/// Class PostrgreSqlUInt64Converter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.UInt64Converter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.UInt64Converter" />
public class PostrgreSqlUInt64Converter : UInt64Converter
{
    /// <summary>
    /// Gets the type of the database.
    /// </summary>
    /// <value>The type of the database.</value>
    public override DbType DbType => DbType.Int64;

    /// <summary>
    /// Converts to dbvalue.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object ToDbValue(Type fieldType, object value)
    {
        return this.ConvertNumber(typeof(long), value);
    }
}