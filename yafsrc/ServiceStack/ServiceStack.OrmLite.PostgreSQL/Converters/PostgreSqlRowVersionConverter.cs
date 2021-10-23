// ***********************************************************************
// <copyright file="PostgreSqlRowVersionConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.PostgreSQL.Converters
{
    using System;

    using ServiceStack.OrmLite.Converters;

    /// <summary>
    /// Class PostgreSqlRowVersionConverter.
    /// Implements the <see cref="ServiceStack.OrmLite.Converters.RowVersionConverter" />
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.Converters.RowVersionConverter" />
    public class PostgreSqlRowVersionConverter : RowVersionConverter
    {
        /// <summary>
        /// Parameterized value in parameterized queries
        /// </summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.Object.</returns>
        public override object ToDbValue(Type fieldType, object value)
        {
            var ret = base.ToDbValue(fieldType, value);
            if (ret is ulong u)
                return (long) u;
            return ret;
        }

        /// <summary>
        /// Froms the database value.
        /// </summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.Object.</returns>
        public override object FromDbValue(Type fieldType, object value)
        {
            var ret = base.FromDbValue(fieldType, value);
            return ret;
        }
    }
}