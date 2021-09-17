// ***********************************************************************
// <copyright file="PostgreSqlHstoreConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.PostgreSQL.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using Npgsql;

    using NpgsqlTypes;

    using ServiceStack.OrmLite.Converters;

    /// <summary>
    /// Class PostgreSqlHstoreConverter.
    /// Implements the <see cref="ServiceStack.OrmLite.Converters.ReferenceTypeConverter" />
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.Converters.ReferenceTypeConverter" />
    public class PostgreSqlHstoreConverter : ReferenceTypeConverter
    {
        /// <summary>
        /// Gets the column definition.
        /// </summary>
        /// <value>The column definition.</value>
        public override string ColumnDefinition => "hstore";

        /// <summary>
        /// Used in DB Params. Defaults to DbType.String
        /// </summary>
        /// <value>The type of the database.</value>
        public override DbType DbType => DbType.Object;

        /// <summary>
        /// Froms the database value.
        /// </summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.Object.</returns>
        public override object FromDbValue(Type fieldType, object value)
        {
            return (IDictionary<string, string>)value;
        }

        /// <summary>
        /// Parameterized value in parameterized queries
        /// </summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.Object.</returns>
        public override object ToDbValue(Type fieldType, object value)
        {
            return (IDictionary<string, string>)value;
        }

        /// <summary>
        /// Customize how DB Param is initialized. Useful for supporting RDBMS-specific Types.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="fieldType">Type of the field.</param>
        public override void InitDbParam(IDbDataParameter p, Type fieldType)
        {
            var sqlParam = (NpgsqlParameter)p;
            sqlParam.NpgsqlDbType = NpgsqlDbType.Hstore;
            base.InitDbParam(p, fieldType);
        }

        /// <summary>
        /// Gets the column definition.
        /// </summary>
        /// <param name="stringLength">Length of the string.</param>
        /// <returns>System.String.</returns>
        public override string GetColumnDefinition(int? stringLength) => ColumnDefinition;
    }
}