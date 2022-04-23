// ***********************************************************************
// <copyright file="IntegerConverters.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Data;

namespace ServiceStack.OrmLite.Converters;

/// <summary>
/// Class IntegerConverter.
/// Implements the <see cref="ServiceStack.OrmLite.NativeValueOrmLiteConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.NativeValueOrmLiteConverter" />
public abstract class IntegerConverter : NativeValueOrmLiteConverter
{
    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "INTEGER";
    /// <summary>
    /// Used in DB Params. Defaults to DbType.String
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
        return this.ConvertNumber(fieldType, value);
    }

    /// <summary>
    /// Value from DB to Populate on POCO Data Model with
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object FromDbValue(Type fieldType, object value)
    {
        return this.ConvertNumber(fieldType, value);
    }
}

/// <summary>
/// Class ByteConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.IntegerConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.IntegerConverter" />
public class ByteConverter : IntegerConverter
{
    /// <summary>
    /// Gets the type of the database.
    /// </summary>
    /// <value>The type of the database.</value>
    public override DbType DbType => DbType.Byte;
}

/// <summary>
/// Class SByteConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.IntegerConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.IntegerConverter" />
public class SByteConverter : IntegerConverter
{
    /// <summary>
    /// Gets the type of the database.
    /// </summary>
    /// <value>The type of the database.</value>
    public override DbType DbType => DbType.SByte;
}

/// <summary>
/// Class Int16Converter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.IntegerConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.IntegerConverter" />
public class Int16Converter : IntegerConverter
{
    /// <summary>
    /// Gets the type of the database.
    /// </summary>
    /// <value>The type of the database.</value>
    public override DbType DbType => DbType.Int16;
}

/// <summary>
/// Class UInt16Converter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.IntegerConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.IntegerConverter" />
public class UInt16Converter : IntegerConverter
{
    /// <summary>
    /// Gets the type of the database.
    /// </summary>
    /// <value>The type of the database.</value>
    public override DbType DbType => DbType.UInt16;
}

/// <summary>
/// Class Int32Converter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.IntegerConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.IntegerConverter" />
public class Int32Converter : IntegerConverter { }

/// <summary>
/// Class UInt32Converter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.IntegerConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.IntegerConverter" />
public class UInt32Converter : IntegerConverter
{
    /// <summary>
    /// Gets the type of the database.
    /// </summary>
    /// <value>The type of the database.</value>
    public override DbType DbType => DbType.UInt32;
}

/// <summary>
/// Class Int64Converter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.IntegerConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.IntegerConverter" />
public class Int64Converter : IntegerConverter
{
    /// <summary>
    /// Gets the type of the database.
    /// </summary>
    /// <value>The type of the database.</value>
    public override DbType DbType => DbType.Int64;
    /// <summary>
    /// Gets the column definition.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "BIGINT";
}

/// <summary>
/// Class UInt64Converter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.IntegerConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.IntegerConverter" />
public class UInt64Converter : IntegerConverter
{
    /// <summary>
    /// Gets the type of the database.
    /// </summary>
    /// <value>The type of the database.</value>
    public override DbType DbType => DbType.UInt64;
    /// <summary>
    /// Gets the column definition.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "BIGINT";
}