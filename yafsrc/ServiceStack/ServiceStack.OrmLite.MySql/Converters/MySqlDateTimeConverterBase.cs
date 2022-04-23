// ***********************************************************************
// <copyright file="MySqlDateTimeConverterBase.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using ServiceStack.OrmLite.Converters;

namespace ServiceStack.OrmLite.MySql.Converters;

/// <summary>
/// Class MySqlDateTimeConverterBase.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.DateTimeConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.DateTimeConverter" />
public abstract class MySqlDateTimeConverterBase : DateTimeConverter
{
    /// <summary>
    /// Gets or sets the precision.
    /// </summary>
    /// <value>The precision.</value>
    public int Precision { get; set; } = 0;

    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => Precision == 0
                                                   ? "DATETIME"
                                                   : $"DATETIME({Precision})";

    /// <summary>
    /// Converts to quotedstring.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public override string ToQuotedString(Type fieldType, object value)
    {
        /*
         * ms not contained in format. MySql ignores ms part anyway
         * for more details see: http://dev.mysql.com/doc/refman/5.1/en/datetime.html
         */
        var dateTime = (DateTime)value;
        var suffix = Precision > 0
                         ? "." + new string('f', Precision)
                         : "";
        return DateTimeFmt(dateTime, "yyyy-MM-dd HH:mm:ss" + suffix);
    }

}