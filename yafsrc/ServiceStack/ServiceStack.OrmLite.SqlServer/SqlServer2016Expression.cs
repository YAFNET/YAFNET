// ***********************************************************************
// <copyright file="SqlServer2016Expression.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.OrmLite.SqlServer;

using ServiceStack.Text;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

/// <summary>
/// Class SqlServer2016Expression.
/// Implements the <see cref="ServiceStack.OrmLite.SqlServer.SqlServerExpression{T}" />
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="ServiceStack.OrmLite.SqlServer.SqlServerExpression{T}" />
public class SqlServer2016Expression<T> : SqlServerExpression<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServer2016Expression{T}"/> class.
    /// </summary>
    /// <param name="dialectProvider">The dialect provider.</param>
    public SqlServer2016Expression(IOrmLiteDialectProvider dialectProvider)
        : base(dialectProvider) { }

    /// <summary>
    /// Visits the SQL method call.
    /// </summary>
    /// <param name="m">The m.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="System.NotSupportedException"></exception>
    protected override object VisitSqlMethodCall(MethodCallExpression m)
    {
        List<object> args = this.VisitInSqlExpressionList(m.Arguments);
        object quotedColName = args[0];
        args.RemoveAt(0);

        string statement;

        switch (m.Method.Name)
        {
            case nameof(Sql.In):
                statement = this.ConvertInExpressionToSql(m, quotedColName);
                break;
            case nameof(Sql.Desc):
                statement = $"{quotedColName} DESC";
                break;
            case nameof(Sql.As):
                statement = $"{quotedColName} AS {this.DialectProvider.GetQuotedColumnName(this.RemoveQuoteFromAlias(args[0].ToString()))}";
                break;
            case nameof(Sql.Cast):
                statement = this.DialectProvider.SqlCast(quotedColName, args[0].ToString());
                break;
            case nameof(Sql.Sum):
            case nameof(Sql.Count):
            case nameof(Sql.Min):
            case nameof(Sql.Max):
            case nameof(Sql.Avg):
                statement = $"{m.Method.Name}({quotedColName}{(args.Count == 1 ? $",{args[0]}" : string.Empty)})";
                break;
            case nameof(Sql.CountDistinct):
                statement = $"COUNT(DISTINCT {quotedColName})";
                break;
            case nameof(Sql.AllFields):
                var argDef = m.Arguments[0].Type.GetModelMetadata();
                statement = this.DialectProvider.GetQuotedTableName(argDef) + ".*";
                break;
            case nameof(Sql.JoinAlias):
            case nameof(Sql.TableAlias):
                statement = args[0] + "." + quotedColName.ToString().LastRightPart('.');
                break;
            case nameof(Sql.Custom):
                statement = quotedColName.ToString();
                break;
            case nameof(Sql.IsJson):
                statement = $"ISJSON({quotedColName})";
                break;
            case nameof(Sql.JsonValue):
                statement = $"JSON_VALUE({quotedColName}, '{args[0]}')";
                break;
            case nameof(Sql.JsonQuery):
                statement = $"JSON_QUERY({quotedColName}";
                if (this.DialectProvider is SqlServer2017OrmLiteDialectProvider && args.Count > 0)
                {
                    statement += $", '{args[0]}'";
                }

                statement += ")";
                break;
            default:
                throw new NotSupportedException();
        }

        return new PartialSqlString(statement);
    }
}