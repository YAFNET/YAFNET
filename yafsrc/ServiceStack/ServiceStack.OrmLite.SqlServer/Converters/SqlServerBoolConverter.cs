// ***********************************************************************
// <copyright file="SqlServerBoolConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.SqlServer.Converters;

using System;
using System.Data;

using ServiceStack.OrmLite.Converters;

/// <summary>
/// Class SqlServerBoolConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.BoolConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.BoolConverter" />
public class SqlServerBoolConverter : BoolConverter
{
    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "BIT";

    /// <summary>
    /// Used in DB Params. Defaults to DbType.String
    /// </summary>
    /// <value>The type of the database.</value>
    public override DbType DbType => DbType.Boolean;

    /// <summary>
    /// Parameterized value in parameterized queries
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object ToDbValue(Type fieldType, object value)
    {
        if (value is bool)
        {
            return value;
        }

        return 0 != (long)this.ConvertNumber(typeof(long), value);
    }

    /// <summary>
    /// Quoted Value in SQL Statement
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public override string ToQuotedString(Type fieldType, object value)
    {
        var boolValue = (bool)value;
        return base.DialectProvider.GetQuotedValue(boolValue ? 1 : 0, typeof(int));
    }

    /// <summary>
    /// Froms the database value.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object FromDbValue(Type fieldType, object value)
    {
        if (value is bool)
            return value;

        return 0 != (long)this.ConvertNumber(typeof(long), value);
    }
}