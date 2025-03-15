// ***********************************************************************
// <copyright file="PostgreSqlDateOnlyConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#if NET9_0_OR_GREATER
namespace ServiceStack.OrmLite.PostgreSQL.Converters
{
    using System;

    /// <summary>
    /// Class PostgreSqlDateOnlyConverter.
    /// Implements the <see cref="ServiceStack.OrmLite.PostgreSQL.Converters.PostgreSqlDateTimeConverter" />
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.PostgreSQL.Converters.PostgreSqlDateTimeConverter" />
    public class PostgreSqlDateOnlyConverter : PostgreSqlDateTimeConverter
    {
        /// <summary>
        /// Quoted Value in SQL Statement
        /// </summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public override string ToQuotedString(Type fieldType, object value)
        {
            var dateOnly = (DateOnly)value;
            return this.DateTimeFmt(dateOnly.ToDateTime(default, DateTimeKind.Utc), "yyyy-MM-dd HH:mm:ss.fff");
        }

        /// <summary>
        /// Parameterized value in parameterized queries
        /// </summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.Object.</returns>
        public override object ToDbValue(Type fieldType, object value)
        {
            var dateOnly = (DateOnly)value;
            var dateTime = dateOnly.ToDateTime(default, DateTimeKind.Utc);
            if (dateTime.Kind != DateTimeKind.Utc)
            {
                dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            }

            return dateTime;
        }

        /// <summary>
        /// Froms the database value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Object.</returns>
        public override object FromDbValue(object value)
        {
            var dateTime = (DateTime)base.FromDbValue(value);
            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                dateTime = dateTime.ToLocalTime();
            }

            var dateOnly = DateOnly.FromDateTime(dateTime);
            return dateOnly;
        }
    }
}

#endif