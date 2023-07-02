// ***********************************************************************
// <copyright file="PostgreSqlDecimalConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.PostgreSQL.Converters;

using System;
using System.Data;

using ServiceStack.OrmLite.Converters;

/// <summary>
/// Class PostgreSqlDecimalConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.DecimalConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.DecimalConverter" />
public class PostgreSqlDecimalConverter : DecimalConverter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PostgreSqlDecimalConverter" /> class.
    /// </summary>
    public PostgreSqlDecimalConverter() : base(38, 6) { }

    /// <summary>
    /// Gets the column definition.
    /// </summary>
    /// <param name="precision">The precision.</param>
    /// <param name="scale">The scale.</param>
    /// <returns>System.String.</returns>
    public override string GetColumnDefinition(int? precision, int? scale)
    {
        return $"NUMERIC({precision.GetValueOrDefault(Precision)},{scale.GetValueOrDefault(Scale)})";
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="columnIndex">Index of the column.</param>
    /// <param name="values">The values.</param>
    /// <returns>System.Object.</returns>
    public override object GetValue(IDataReader reader, int columnIndex, object[] values)
    {
        try
        {
            return base.GetValue(reader, columnIndex, values);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}