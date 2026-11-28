// ***********************************************************************
// <copyright file="SqliteGuidConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.Sqlite.Converters;

using System;
using System.Data;

using ServiceStack.OrmLite.Converters;


/// <summary>
/// Class SqliteGuidConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.GuidConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.GuidConverter" />
public class SqliteGuidConverter : GuidConverter
{
    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "CHAR(36)";

    /// <summary>
    /// Quoted Value in SQL Statement
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public override string ToQuotedString(Type fieldType, object value)
    {
        var guid = (Guid)value;
        var bytes = guid.ToByteArray();
        var fmt = "x'" + BitConverter.ToString(bytes).Replace("-", "") + "'";
        return fmt;
    }

    /// <summary>
    /// Retrieve Value from ADO.NET IDataReader. Defaults to reader.GetValue()
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="columnIndex">Index of the column.</param>
    /// <param name="values">The values.</param>
    /// <returns>System.Object.</returns>
    public override object GetValue(IDataReader reader, int columnIndex, object[] values)
    {
        if (values != null)
        {
            if (values[columnIndex] == DBNull.Value)
            {
                return null;
            }
        }
        else
        {
            if (reader.IsDBNull(columnIndex))
            {
                return null;
            }
        }

        return reader.GetGuid(columnIndex);
    }
}

/// <summary>
/// The Guid converter for Microsoft.Data.Sqlite, which binds a Guid as text. The Guid is written as a 16 byte blob,
/// as System.Data.SQLite does, so a database works with both providers (and a Guid parameter matches the stored value).
/// Microsoft.Data.Sqlite's GetGuid() reads both, blobs and text.
/// </summary>
/// <seealso cref="SqliteGuidConverter" />
public class SqliteDataGuidConverter : SqliteGuidConverter
{
    /// <summary>
    /// The value of the parameter, the bytes of the Guid.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object ToDbValue(Type fieldType, object value)
    {
        return value is Guid guid ? guid.ToByteArray() : value;
    }
}
