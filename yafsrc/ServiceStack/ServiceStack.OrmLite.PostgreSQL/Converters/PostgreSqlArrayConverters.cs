// ***********************************************************************
// <copyright file="PostgreSqlArrayConverters.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite.PostgreSQL.Converters;

using System;
using System.Data;

using ServiceStack.OrmLite.Converters;

/// <summary>
/// Class PostrgreSqlByteArrayConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.ByteArrayConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.ByteArrayConverter" />
public class PostrgreSqlByteArrayConverter : ByteArrayConverter
{
    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "BYTEA";

    /// <summary>
    /// Quoted Value in SQL Statement
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public override string ToQuotedString(Type fieldType, object value)
    {
        return "E'" + this.ToBinary(value) + "'";
    }
}

/// <summary>
/// Class PostgreSqlStringArrayConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.ReferenceTypeConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.ReferenceTypeConverter" />
public class PostgreSqlStringArrayConverter : ReferenceTypeConverter
{
    /// <summary>
    /// Gets the column definition.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "text[]";

    /// <summary>
    /// Used in DB Params. Defaults to DbType.String
    /// </summary>
    /// <value>The type of the database.</value>
    public override DbType DbType => DbType.Object;

    /// <summary>
    /// Gets the column definition.
    /// </summary>
    /// <param name="stringLength">Length of the string.</param>
    /// <returns>System.String.</returns>
    public override string GetColumnDefinition(int? stringLength)
    {
        return stringLength != null
                   ? base.GetColumnDefinition(stringLength)
                   : this.ColumnDefinition;
    }

    /// <summary>
    /// Quoted Value in SQL Statement
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public override string ToQuotedString(Type fieldType, object value)
    {
        var stringArray = (string[])value;
        return this.ToArray(stringArray);
    }

    /// <summary>
    /// Parameterized value in parameterized queries
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object ToDbValue(Type fieldType, object value)
    {
        return fieldType == typeof(string[])
                   ? value
                   : this.DialectProvider.StringSerializer.SerializeToString(value);
    }

    /// <summary>
    /// Froms the database value.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object FromDbValue(Type fieldType, object value)
    {
        return value is string strVal
                   ? strVal.FromJson<string[]>()
                   : value;
    }
}

/// <summary>
/// Class PostgreSqlArrayConverterBase.
/// Implements the <see cref="ServiceStack.OrmLite.NativeValueOrmLiteConverter" />
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="ServiceStack.OrmLite.NativeValueOrmLiteConverter" />
public abstract class PostgreSqlArrayConverterBase<T> : NativeValueOrmLiteConverter
{
    /// <summary>
    /// Used in DB Params. Defaults to DbType.String
    /// </summary>
    /// <value>The type of the database.</value>
    public override DbType DbType => DbType.Object;

    /// <summary>
    /// Quoted Value in SQL Statement
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public override string ToQuotedString(Type fieldType, object value)
    {
        var integerArray = (T[])value;
        return this.ToArray(integerArray);
    }
}

/// <summary>
/// Class PostgreSqlShortArrayConverter.
/// </summary>
/// <seealso cref="short" />
public class PostgreSqlShortArrayConverter : PostgreSqlArrayConverterBase<short>
{
    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "short[]";
}

/// <summary>
/// Class PostgreSqlIntArrayConverter.
/// Implements the <see cref="int" />
/// </summary>
/// <seealso cref="int" />
public class PostgreSqlIntArrayConverter : PostgreSqlArrayConverterBase<int>
{
    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "integer[]";
}

/// <summary>
/// Class PostgreSqlLongArrayConverter.
/// Implements the <see cref="long" />
/// </summary>
/// <seealso cref="long" />
public class PostgreSqlLongArrayConverter : PostgreSqlArrayConverterBase<long>
{
    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "bigint[]";
}

/// <summary>
/// Class PostgreSqlFloatArrayConverter.
/// Implements the <see cref="float" />
/// </summary>
/// <seealso cref="float" />
public class PostgreSqlFloatArrayConverter : PostgreSqlArrayConverterBase<float>
{
    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "real[]";
}

/// <summary>
/// Class PostgreSqlDoubleArrayConverter.
/// Implements the <see cref="double" />
/// </summary>
/// <seealso cref="double" />
public class PostgreSqlDoubleArrayConverter : PostgreSqlArrayConverterBase<double>
{
    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "double precision[]";
}

/// <summary>
/// Class PostgreSqlDecimalArrayConverter.
/// Implements the <see cref="decimal" />
/// </summary>
/// <seealso cref="decimal" />
public class PostgreSqlDecimalArrayConverter : PostgreSqlArrayConverterBase<decimal>
{
    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "numeric[]";
}

/// <summary>
/// Class PostgreSqlDateTimeTimeStampArrayConverter.
/// Implements the <see cref="DateTime" />
/// </summary>
/// <seealso cref="DateTime" />
public class PostgreSqlDateTimeTimeStampArrayConverter : PostgreSqlArrayConverterBase<DateTime>
{
    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "timestamp[]";
}

/// <summary>
/// Class PostgreSqlDateTimeOffsetTimeStampTzArrayConverter.
/// Implements the <see cref="DateTimeOffset" />
/// </summary>
/// <seealso cref="DateTimeOffset" />
public class PostgreSqlDateTimeOffsetTimeStampTzArrayConverter : PostgreSqlArrayConverterBase<DateTimeOffset>
{
    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "timestamp with time zone[]";
}

/// <summary>
/// Class PostgreSqlConverterExtensions.
/// </summary>
public static class PostgreSqlConverterExtensions
{
    /// <param name="converter">The converter.</param>
    extension(IOrmLiteConverter converter)
    {
        /// <summary>
        /// based on Npgsql2's source: Npgsql2\src\NpgsqlTypes\NpgsqlTypeConverters.cs
        /// </summary>
        /// <param name="NativeData">The native data.</param>
        /// <returns>System.String.</returns>
        public string ToBinary(object NativeData)
        {
            var byteArray = (byte[])NativeData;
            var res = StringBuilderCache.Allocate();
            foreach (var b in byteArray)
            {
                if (b is >= 0x20 and < 0x7F && b != 0x27 && b != 0x5C)
                {
                    res.Append((char)b);
                }
                else
                {
                    res.Append("\\\\")
                        .Append((char)('0' + (7 & (b >> 6))))
                        .Append((char)('0' + (7 & (b >> 3))))
                        .Append((char)('0' + (7 & b)));
                }
            }

            return StringBuilderCache.ReturnAndFree(res);
        }

        /// <summary>
        /// Converts to array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns>System.String.</returns>
        public string ToArray<T>(T[] source)
        {
            var values = StringBuilderCache.Allocate();
            foreach (var value in source)
            {
                if (values.Length > 0)
                {
                    values.Append(',');
                }

                values.Append(converter.DialectProvider.GetQuotedValue(value, typeof(T)));
            }

            return "{" + StringBuilderCache.ReturnAndFree(values) + "}";
        }
    }
}