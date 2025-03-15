// ***********************************************************************
// <copyright file="MySqlExpression.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.MySql;

using System;
using System.Linq.Expressions;

/// <summary>
/// Class MySqlExpression.
/// Implements the <see cref="ServiceStack.OrmLite.SqlExpression{T}" />
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="ServiceStack.OrmLite.SqlExpression{T}" />
public class MySqlExpression<T> : SqlExpression<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlExpression{T}" /> class.
    /// </summary>
    /// <param name="dialectProvider">The dialect provider.</param>
    public MySqlExpression(IOrmLiteDialectProvider dialectProvider)
        : base(dialectProvider) { }

    /// <summary>
    /// Converts to cast.
    /// </summary>
    /// <param name="quotedColName">Name of the quoted col.</param>
    /// <returns>string.</returns>
    override protected string ToCast(string quotedColName)
    {
        return $"cast({quotedColName} as char(1000))";
    }

    /// <summary>
    /// Converts to deleterowstatement.
    /// </summary>
    /// <returns>string.</returns>
    public override string ToDeleteRowStatement()
    {
        return base.tableDefs.Count > 1
                   ? $"DELETE {this.DialectProvider.GetQuotedTableName(this.modelDef)} {this.FromExpression} {this.WhereExpression}"
                   : base.ToDeleteRowStatement();
    }

    /// <summary>
    /// Visits the column access method.
    /// </summary>
    /// <param name="m">The m.</param>
    /// <returns>object.</returns>
    override protected object VisitColumnAccessMethod(MethodCallExpression m)
    {
        if (m.Method.Name != nameof(ToString) || m.Object?.Type != typeof(DateTime))
        {
            return base.VisitColumnAccessMethod(m);
        }

        var args = this.VisitExpressionList(m.Arguments);
        var quotedColName = this.Visit(m.Object);

        if (!IsSqlClass(quotedColName))
        {
            quotedColName = this.ConvertToParam(quotedColName);
        }

        var arg = args.Count > 0 ? args[0] : null;
        var statement = arg == null ? this.ToCast(quotedColName.ToString()) : $"DATE_FORMAT({quotedColName},'{arg}')";
        return new PartialSqlString(statement);
    }

    /// <summary>
    /// Creates the in sub query SQL.
    /// </summary>
    /// <param name="quotedColName">Name of the quoted col.</param>
    /// <param name="subSelect">The sub select.</param>
    /// <returns>System.String.</returns>
    override protected string CreateInSubQuerySql(object quotedColName, string subSelect)
    {
        return $"{quotedColName} IN (SELECT * FROM ({subSelect})  SubQuery)";
    }
}