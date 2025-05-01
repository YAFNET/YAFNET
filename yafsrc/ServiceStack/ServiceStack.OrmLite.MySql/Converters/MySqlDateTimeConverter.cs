// ***********************************************************************
// <copyright file="MySqlDateTimeConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.MySql.Converters;

using global::MySql.Data.Types;

/// <summary>
/// Class MySqlDateTimeConverter.
/// Implements the <see cref="ServiceStack.OrmLite.MySql.Converters.MySqlDateTimeConverterBase" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.MySql.Converters.MySqlDateTimeConverterBase" />
public class MySqlDateTimeConverter : MySqlDateTimeConverterBase
{
    /// <summary>
    /// Froms the database value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object FromDbValue(object value)
    {
        // TODO throws error if connection string option not set - https://stackoverflow.com/questions/5754822/unable-to-convert-mysql-date-time-value-to-system-datetime
        if (value is MySqlDateTime time)
        {
            return time.GetDateTime();
        }

        return base.FromDbValue(value);
    }
}

/// <summary>
/// Class MySql55DateTimeConverter.
/// Implements the <see cref="ServiceStack.OrmLite.MySql.Converters.MySqlDateTimeConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.MySql.Converters.MySqlDateTimeConverter" />
public class MySql55DateTimeConverter : MySqlDateTimeConverter
{
    /// <summary>
    /// CURRENT_TIMESTAMP as a default for DATETIME type is only available in 10.x. If you're using 5.5, it should a TIMESTAMP column
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "TIMESTAMP";
}