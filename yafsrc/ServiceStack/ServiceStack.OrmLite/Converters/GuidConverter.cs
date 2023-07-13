// ***********************************************************************
// <copyright file="GuidConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Data;

namespace ServiceStack.OrmLite.Converters;

/// <summary>
/// Class GuidConverter.
/// Implements the <see cref="ServiceStack.OrmLite.OrmLiteConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.OrmLiteConverter" />
public class GuidConverter : OrmLiteConverter
{
    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "GUID";

    /// <summary>
    /// Gets the type of the database.
    /// </summary>
    /// <value>The type of the database.</value>
    public override DbType DbType => DbType.Guid;

    /// <summary>
    /// Value from DB to Populate on POCO Data Model with
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object FromDbValue(Type fieldType, object value)
    {
        if (value is string s)
            return Guid.Parse(s);

        return base.FromDbValue(fieldType, value);
    }
}