// ***********************************************************************
// <copyright file="SqlServerFloatConverters.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.SqlServer.Converters;

using ServiceStack.OrmLite.Converters;

/// <summary>
/// Class SqlServerFloatConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.FloatConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.FloatConverter" />
public class SqlServerFloatConverter : FloatConverter
{
    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "FLOAT";
}

/// <summary>
/// Class SqlServerDoubleConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.DoubleConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.DoubleConverter" />
public class SqlServerDoubleConverter : DoubleConverter
{
    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "FLOAT";
}

/// <summary>
/// Class SqlServerDecimalConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.DecimalConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.DecimalConverter" />
public class SqlServerDecimalConverter : DecimalConverter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerDecimalConverter"/> class.
    /// </summary>
    public SqlServerDecimalConverter() : base(38, 6) { }
}