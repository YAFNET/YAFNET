// ***********************************************************************
// <copyright file="PostgreSqlExpression.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.PostgreSQL
{
    using System.Linq;

    /// <summary>
    /// Class PostgreSqlExpression.
    /// Implements the <see cref="ServiceStack.OrmLite.SqlExpression{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="ServiceStack.OrmLite.SqlExpression{T}" />
    public class PostgreSqlExpression<T> : SqlExpression<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlExpression{T}" /> class.
        /// </summary>
        /// <param name="dialectProvider">The dialect provider.</param>
        public PostgreSqlExpression(IOrmLiteDialectProvider dialectProvider)
            : base(dialectProvider) {}

        /// <summary>
        /// Gets the name of the quoted column.
        /// </summary>
        /// <param name="tableDef">The table definition.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>string.</returns>
        protected override string GetQuotedColumnName(ModelDefinition tableDef, string memberName)
        {
            if (!useFieldName)
            {
                return memberName;
            }

            var fieldDef = tableDef.FieldDefinitions.FirstOrDefault(x => x.Name == memberName);
            if (fieldDef is { IsRowVersion: true } && !PrefixFieldWithTableName)
                return PostgreSqlDialectProvider.RowVersionFieldComparer;

            return base.GetQuotedColumnName(tableDef, memberName);
        }
    }

}