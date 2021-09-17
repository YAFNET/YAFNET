// ***********************************************************************
// <copyright file="PostgreSqlXmlConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.PostgreSQL.Converters
{
    using System;
    using System.Data;

    /// <summary>
    /// Class PostgreSqlXmlConverter.
    /// Implements the <see cref="ServiceStack.OrmLite.PostgreSQL.Converters.PostgreSqlStringConverter" />
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.PostgreSQL.Converters.PostgreSqlStringConverter" />
    public class PostgreSqlXmlConverter : PostgreSqlStringConverter
    {
        /// <summary>
        /// SQL Column Definition used in CREATE Table.
        /// </summary>
        /// <value>The column definition.</value>
        public override string ColumnDefinition => "XML";
        /// <summary>
        /// Customize how DB Param is initialized. Useful for supporting RDBMS-specific Types.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="fieldType">Type of the field.</param>
        public override void InitDbParam(IDbDataParameter p, Type fieldType) => p.DbType = DbType.Xml;
        /// <summary>
        /// Parameterized value in parameterized queries
        /// </summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.Object.</returns>
        public override object ToDbValue(Type fieldType, object value) => value?.ToString();
        /// <summary>
        /// Converts to quotedstring.
        /// </summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public override string ToQuotedString(Type fieldType, object value) => 
            base.ToQuotedString(fieldType, value.ToString());
    }
}