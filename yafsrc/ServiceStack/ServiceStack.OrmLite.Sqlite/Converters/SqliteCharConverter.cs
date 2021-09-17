// ***********************************************************************
// <copyright file="SqliteCharConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.Sqlite.Converters
{
    using System;

    /// <summary>
    /// Class SqliteCharConverter.
    /// Implements the <see cref="ServiceStack.OrmLite.OrmLiteConverter" />
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.OrmLiteConverter" />
    public class SqliteCharConverter : OrmLiteConverter
    {
        /// <summary>
        /// SQL Column Definition used in CREATE Table.
        /// </summary>
        /// <value>The column definition.</value>
        public override string ColumnDefinition => "CHAR(1)";

        /// <summary>
        /// Parameterized value in parameterized queries
        /// </summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.Object.</returns>
        public override object ToDbValue(Type fieldType, object value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Froms the database value.
        /// </summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.Object.</returns>
        public override object FromDbValue(Type fieldType, object value)
        {
            return ((string)value)[0];
        }

    }
}