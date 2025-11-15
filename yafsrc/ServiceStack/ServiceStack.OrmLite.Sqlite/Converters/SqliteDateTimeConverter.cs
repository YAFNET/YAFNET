// ***********************************************************************
// <copyright file="SqliteDateTimeConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using ServiceStack.OrmLite.Base.Text;
using ServiceStack.OrmLite.Base.Text.Common;

namespace ServiceStack.OrmLite.Sqlite.Converters;

using System;
using System.Data;
using System.Globalization;

using ServiceStack.OrmLite.Converters;

/// <summary>
/// Class SqliteNativeDateTimeConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.DateTimeConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.DateTimeConverter" />
public class SqliteNativeDateTimeConverter : DateTimeConverter
{
    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "VARCHAR(8000)";

    /// <summary>
    /// Used in DB Params. Defaults to DbType.String
    /// </summary>
    /// <value>The type of the database.</value>
    public override DbType DbType => DbType.DateTime;

    /// <summary>
    /// Quoted Value in SQL Statement
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public override string ToQuotedString(Type fieldType, object value)
    {
        var dateTime = (DateTime)value;

        switch (this.DateStyle)
        {
            case DateTimeKind.Unspecified:
                dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
                break;
            case DateTimeKind.Local when dateTime.Kind == DateTimeKind.Unspecified:
                dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc).ToLocalTime();
                break;
            case DateTimeKind.Utc:
                dateTime = dateTime.Kind == DateTimeKind.Local
                               ? DateTime.SpecifyKind(dateTime, DateTimeKind.Local).ToUniversalTime()
                               : DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

                return this.DialectProvider.GetQuotedValue(dateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture), typeof(string));
        }

        var dateStr = DateTimeSerializer.ToLocalXsdDateTimeString(dateTime);
        dateStr = dateStr.Replace("T", " ");
        const int tzPos = 6; //"-00:00".Length;
        var timeZoneMod = dateStr.Substring(dateStr.Length - tzPos, 1);
        if (timeZoneMod is "+" or "-")
        {
            dateStr = dateStr[..^tzPos];
        }

        return this.DialectProvider.GetQuotedValue(dateStr, typeof(string));
    }

    /// <summary>
    /// Value from DB to Populate on POCO Data Model with
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object FromDbValue(Type fieldType, object value)
    {
        var dateTime = (DateTime)value;

        if (this.DateStyle == DateTimeKind.Unspecified)
        {
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
        }

        return base.FromDbValue(dateTime);
    }

    /// <summary>
    /// Retrieve Value from ADO.NET IDataReader. Defaults to reader.GetValue()
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="columnIndex">Index of the column.</param>
    /// <param name="values">The values.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="DiagnosticEvent.Exception">Converting from {value.GetType().Name} to DateTime is not supported</exception>
    /// <exception cref="System.Exception">Converting from {value.GetType().Name} to DateTime is not supported</exception>
    public override object GetValue(IDataReader reader, int columnIndex, object[] values)
    {
        try
        {
            if (values != null && values[columnIndex] == DBNull.Value)
            {
                return null;
            }

            if (reader.IsDBNull(columnIndex))
            {
                return null;
            }

            return reader.GetDateTime(columnIndex);
        }
        catch (Exception ex)
        {
            var value = base.GetValue(reader, columnIndex, values);
            if (value == null)
            {
                return null;
            }

            if (value is not string dateStr)
            {
                throw new Exception($"Converting from {value.GetType().Name} to DateTime is not supported");
            }

            Log.Warn("Error reading string as DateTime in Sqlite: " + dateStr, ex);
            return DateTime.Parse(dateStr);
        }
    }
}

/// <summary>
/// New behavior from using System.Data.SQLite.Core
/// </summary>
public class SqliteCoreDateTimeConverter : SqliteNativeDateTimeConverter
{
    /// <summary>
    /// Parameterized value in parameterized queries
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object ToDbValue(Type fieldType, object value)
    {
        var dateTime = (DateTime)value;
        switch (this.DateStyle)
        {
            case DateTimeKind.Utc when dateTime.Kind == DateTimeKind.Local:
                dateTime = dateTime.ToUniversalTime();
                break;
            case DateTimeKind.Utc:
                {
                    if (dateTime.Kind == DateTimeKind.Unspecified)
                    {
                        dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                    }

                    break;
                }
            case DateTimeKind.Local when dateTime.Kind != DateTimeKind.Local:
                dateTime = dateTime.Kind == DateTimeKind.Utc
                               ? dateTime.ToLocalTime()
                               : DateTime.SpecifyKind(dateTime, DateTimeKind.Utc).ToLocalTime();
                break;
            case DateTimeKind.Unspecified when dateTime.Kind == DateTimeKind.Utc:
                dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
                break;
        }

        return dateTime;
    }

    /// <summary>
    /// Froms the database value.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object FromDbValue(Type fieldType, object value)
    {
        var dateTime = (DateTime)value;

        switch (this.DateStyle)
        {
            case DateTimeKind.Utc:
#if NET10_0_OR_GREATER
                //.NET Core returns correct Local time but as Unspecified so change to Local and Convert to UTC
                dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local).ToUniversalTime();
#else
                dateTime = dateTime.ToUniversalTime();
#endif
                break;
            case DateTimeKind.Local when dateTime.Kind != DateTimeKind.Local:
                dateTime = dateTime.Kind == DateTimeKind.Utc
                               ? dateTime.ToLocalTime()
                               : DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
                break;
        }

        return dateTime;
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
        var ret = base.GetValue(reader, columnIndex, values);
        return ret;
    }
}

/// <summary>
/// Class SqliteDataDateTimeConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Sqlite.Converters.SqliteCoreDateTimeConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Sqlite.Converters.SqliteCoreDateTimeConverter" />
public class SqliteDataDateTimeConverter : SqliteCoreDateTimeConverter
{
    /// <summary>
    /// Froms the database value.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object FromDbValue(Type fieldType, object value)
    {
        var dateTime = (DateTime)value;

        if (this.DateStyle == DateTimeKind.Utc)
        {
            //.NET Core returns correct Local time but as Unspecified so change to Local and Convert to UTC
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc); // don't convert
        }

        if (this.DateStyle == DateTimeKind.Local && dateTime.Kind != DateTimeKind.Local)
        {
            dateTime = dateTime.Kind == DateTimeKind.Utc
                           ? dateTime.ToLocalTime()
                           : DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
        }

        return dateTime;
    }
}

/// <summary>
/// Class SqliteWindowsDateTimeConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Sqlite.Converters.SqliteNativeDateTimeConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Sqlite.Converters.SqliteNativeDateTimeConverter" />
public class SqliteWindowsDateTimeConverter : SqliteNativeDateTimeConverter
{
    /// <summary>
    /// Froms the database value.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object FromDbValue(Type fieldType, object value)
    {
        var dateTime = (DateTime)value;

        if (this.DateStyle == DateTimeKind.Utc)
        {
            dateTime = dateTime.ToUniversalTime();
        }

        if (this.DateStyle == DateTimeKind.Local && dateTime.Kind != DateTimeKind.Local)
        {
            dateTime = dateTime.Kind == DateTimeKind.Utc
                           ? dateTime.ToLocalTime()
                           : DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
        }

        return dateTime;
    }
}