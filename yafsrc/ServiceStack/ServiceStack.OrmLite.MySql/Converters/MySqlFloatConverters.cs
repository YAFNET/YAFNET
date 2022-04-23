// ***********************************************************************
// <copyright file="MySqlFloatConverters.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using ServiceStack.OrmLite.Converters;

namespace ServiceStack.OrmLite.MySql.Converters;

/// <summary>
/// Class MySqlDecimalConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.DecimalConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.DecimalConverter" />
public class MySqlDecimalConverter : DecimalConverter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlDecimalConverter"/> class.
    /// </summary>
    public MySqlDecimalConverter() : base(38,6) { }
}