// ***********************************************************************
// <copyright file="DateTimeOffsetConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Data;
using System.Globalization;

namespace ServiceStack.OrmLite.Converters;

/// <summary>
/// Class DateTimeOffsetConverter.
/// Implements the <see cref="ServiceStack.OrmLite.OrmLiteConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.OrmLiteConverter" />
public class DateTimeOffsetConverter : OrmLiteConverter
{
    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "DATETIMEOFFSET";
    /// <summary>
    /// Used in DB Params. Defaults to DbType.String
    /// </summary>
    /// <value>The type of the database.</value>
    public override DbType DbType => DbType.DateTimeOffset;

    //From OrmLiteDialectProviderBase:
    /// <summary>
    /// Froms the database value.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object FromDbValue(Type fieldType, object value)
    {
        if (value is string strValue)
        {
            var moment = DateTimeOffset.Parse(strValue, null, DateTimeStyles.RoundtripKind);
            return moment;
        }
        if (value.GetType() == fieldType)
        {
            return value;
        }
        if (value is DateTime)
        {
            return new DateTimeOffset((DateTime)value);
        }
        var convertedValue = DialectProvider.StringSerializer.DeserializeFromString(value.ToString(), fieldType);
        return convertedValue;
    }
}