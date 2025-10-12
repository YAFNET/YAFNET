// ***********************************************************************
// <copyright file="IOrmLiteConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

using System;
using System.Data;

using ServiceStack.Logging;

/// <summary>
/// Interface IOrmLiteConverter
/// </summary>
public interface IOrmLiteConverter
{
    /// <summary>
    /// Gets or sets the dialect provider.
    /// </summary>
    /// <value>The dialect provider.</value>
    IOrmLiteDialectProvider DialectProvider { get; set; }

    /// <summary>
    /// Gets the type of the database.
    /// </summary>
    /// <value>The type of the database.</value>
    DbType DbType { get; }

    /// <summary>
    /// Gets the column definition.
    /// </summary>
    /// <value>The column definition.</value>
    string ColumnDefinition { get; }

    /// <summary>
    /// Converts to quotedstring.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    string ToQuotedString(Type fieldType, object value);

    /// <summary>
    /// Initializes the database parameter.
    /// </summary>
    /// <param name="p">The p.</param>
    /// <param name="fieldType">Type of the field.</param>
    void InitDbParam(IDbDataParameter p, Type fieldType);

    /// <summary>
    /// Converts to dbvalue.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    object ToDbValue(Type fieldType, object value);

    /// <summary>
    /// Froms the database value.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    object FromDbValue(Type fieldType, object value);

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="columnIndex">Index of the column.</param>
    /// <param name="values">The values.</param>
    /// <returns>System.Object.</returns>
    object GetValue(IDataReader reader, int columnIndex, object[] values);
}

/// <summary>
/// Interface IHasColumnDefinitionLength
/// </summary>
public interface IHasColumnDefinitionLength
{
    /// <summary>
    /// Gets the column definition.
    /// </summary>
    /// <param name="length">The length.</param>
    /// <returns>System.String.</returns>
    string GetColumnDefinition(int? length);
}

/// <summary>
/// Interface IHasColumnDefinitionPrecision
/// </summary>
public interface IHasColumnDefinitionPrecision
{
    /// <summary>
    /// Gets the column definition.
    /// </summary>
    /// <param name="precision">The precision.</param>
    /// <param name="scale">The scale.</param>
    /// <returns>System.String.</returns>
    string GetColumnDefinition(int? precision, int? scale);
}

/// <summary>
/// Class OrmLiteConverter.
/// Implements the <see cref="ServiceStack.OrmLite.IOrmLiteConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.IOrmLiteConverter" />
public abstract class OrmLiteConverter : IOrmLiteConverter
{
    /// <summary>
    /// The log
    /// </summary>
    public static ILog Log => OrmLiteLog.Log;

    /// <summary>
    /// RDBMS Dialect this Converter is for. Injected at registration.
    /// </summary>
    /// <value>The dialect provider.</value>
    public IOrmLiteDialectProvider DialectProvider { get; set; }

    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public abstract string ColumnDefinition { get; }

    /// <summary>
    /// Used in DB Params. Defaults to DbType.String
    /// </summary>
    /// <value>The type of the database.</value>
    public virtual DbType DbType => DbType.String;

    /// <summary>
    /// Quoted Value in SQL Statement
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public virtual string ToQuotedString(Type fieldType, object value)
    {
        return this.DialectProvider.GetQuotedValue(value.ToString());
    }

    /// <summary>
    /// Customize how DB Param is initialized. Useful for supporting RDBMS-specific Types.
    /// </summary>
    /// <param name="p">The p.</param>
    /// <param name="fieldType">Type of the field.</param>
    public virtual void InitDbParam(IDbDataParameter p, Type fieldType)
    {
        p.DbType = this.DbType;
    }

    /// <summary>
    /// Parameterized value in parameterized queries
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public virtual object ToDbValue(Type fieldType, object value)
    {
        return value;
    }

    /// <summary>
    /// Value from DB to Populate on POCO Data Model with
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public virtual object FromDbValue(Type fieldType, object value)
    {
        return value;
    }

    /// <summary>
    /// Retrieve Value from ADO.NET IDataReader. Defaults to reader.GetValue()
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="columnIndex">Index of the column.</param>
    /// <param name="values">The values.</param>
    /// <returns>System.Object.</returns>
    public virtual object GetValue(IDataReader reader, int columnIndex, object[] values)
    {
        var value = values != null
                        ? values[columnIndex]
                        : reader.GetValue(columnIndex);

        return value == DBNull.Value ? null : value;
    }
}

/// <summary>
/// For Types that are natively supported by RDBMS's and shouldn't be quoted
/// </summary>
public abstract class NativeValueOrmLiteConverter : OrmLiteConverter
{
    /// <summary>
    /// Quoted Value in SQL Statement
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public override string ToQuotedString(Type fieldType, object value)
    {
        return value.ToString();
    }
}

/// <summary>
/// Class OrmLiteConverterExtensions.
/// </summary>
public static class OrmLiteConverterExtensions
{
    /// <summary>
    /// Converts the number.
    /// </summary>
    /// <param name="converter">The converter.</param>
    /// <param name="toIntegerType">Type of to integer.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public static object ConvertNumber(this IOrmLiteConverter converter, Type toIntegerType, object value)
    {
        return converter.DialectProvider.ConvertNumber(toIntegerType, value);
    }

    /// <summary>
    /// Converts the number.
    /// </summary>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <param name="toIntegerType">Type of to integer.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public static object ConvertNumber(this IOrmLiteDialectProvider dialectProvider, Type toIntegerType, object value)
    {
        if (value == null)
        {
            return null;
        }

        if (value.GetType() == toIntegerType)
        {
            return value;
        }

        var typeCode = toIntegerType.GetUnderlyingTypeCode();

        switch (typeCode)
        {
            case TypeCode.Byte:
                return Convert.ToByte(value);
            case TypeCode.SByte:
                return Convert.ToSByte(value);
            case TypeCode.Int16:
                return Convert.ToInt16(value);
            case TypeCode.UInt16:
                return Convert.ToUInt16(value);
            case TypeCode.Int32:
                return Convert.ToInt32(value);
            case TypeCode.UInt32:
                return Convert.ToUInt32(value);
            case TypeCode.Int64:
                return Convert.ToInt64(value);
            case TypeCode.UInt64:
                if (value is byte[] byteValue)
                {
                    return OrmLiteUtils.ConvertToULong(byteValue);
                }

                return Convert.ToUInt64(value);
            case TypeCode.Single:
                return Convert.ToSingle(value);
            case TypeCode.Double:
                return Convert.ToDouble(value);
            case TypeCode.Decimal:
                return Convert.ToDecimal(value);
        }

        var convertedValue = dialectProvider.StringSerializer.DeserializeFromString(value.ToString(), toIntegerType);
        return convertedValue;
    }
}