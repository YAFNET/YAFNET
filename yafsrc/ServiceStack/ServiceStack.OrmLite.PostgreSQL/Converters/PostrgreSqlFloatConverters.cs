// ***********************************************************************
// <copyright file="PostrgreSqlFloatConverters.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.PostgreSQL.Converters;

using System;
using System.Data;

using ServiceStack.OrmLite.Converters;

/// <summary>
/// Class PostrgreSqlFloatConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.FloatConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.FloatConverter" />
public class PostrgreSqlFloatConverter : FloatConverter
{
    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "DOUBLE PRECISION";
}

/// <summary>
/// Class PostrgreSqlDoubleConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.DoubleConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.DoubleConverter" />
public class PostrgreSqlDoubleConverter : DoubleConverter
{
    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "DOUBLE PRECISION";
}

/// <summary>
/// Class PostrgreSqlDecimalConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.DecimalConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.DecimalConverter" />
public class PostrgreSqlDecimalConverter : DecimalConverter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PostrgreSqlDecimalConverter" /> class.
    /// </summary>
    public PostrgreSqlDecimalConverter()
        : base(38, 6) {}

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
        catch (OverflowException)
        {
            return decimal.MaxValue;
        }
    }
}