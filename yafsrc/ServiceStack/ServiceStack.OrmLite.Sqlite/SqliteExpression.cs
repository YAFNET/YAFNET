// ***********************************************************************
// <copyright file="SqliteExpression.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.OrmLite.Sqlite;

using System.Linq.Expressions;

/// <summary>
/// Class SqliteExpression.
/// Implements the <see cref="ServiceStack.OrmLite.SqlExpression{T}" />
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="ServiceStack.OrmLite.SqlExpression{T}" />
public class SqliteExpression<T> : SqlExpression<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqliteExpression{T}" /> class.
    /// </summary>
    /// <param name="dialectProvider">The dialect provider.</param>
    public SqliteExpression(IOrmLiteDialectProvider dialectProvider)
        : base(dialectProvider) {}

    /// <summary>
    /// Visits the column access method.
    /// </summary>
    /// <param name="m">The m.</param>
    /// <returns>object.</returns>
    protected override object VisitColumnAccessMethod(MethodCallExpression m)
    {
        var args = this.VisitExpressionList(m.Arguments);
        var quotedColName = Visit(m.Object);
        string statement;

        switch (m.Method.Name)
        {
            case "Substring":
                var startIndex = int.Parse(args[0].ToString()) + 1;
                if (args.Count == 2)
                {
                    var length = int.Parse(args[1].ToString());
                    statement = $"substr({quotedColName}, {startIndex}, {length})";
                }
                else
                    statement = $"substr({quotedColName}, {startIndex})";
                break;
            default:
                return base.VisitColumnAccessMethod(m);
        }
        return new PartialSqlString(statement);
    }

    /// <summary>
    /// Visits the SQL method call.
    /// </summary>
    /// <param name="m">The m.</param>
    /// <returns>object.</returns>
    protected override object VisitSqlMethodCall(MethodCallExpression m)
    {
        var args = this.VisitInSqlExpressionList(m.Arguments);
        object quotedColName = args[0];
        args.RemoveAt(0);

        string statement;

        switch (m.Method.Name)
        {
            case "In":
                statement = ConvertInExpressionToSql(m, quotedColName);
                break;
            case "Desc":
                statement = $"{quotedColName} DESC";
                break;
            case "As":
                statement = $"{quotedColName} AS {base.DialectProvider.GetQuotedColumnName(RemoveQuoteFromAlias(args[0].ToString()))}";
                break;
            case "Sum":
            case "Count":
            case "Min":
            case "Max":
            case "Avg":
                statement = $"{m.Method.Name}({quotedColName}{(args.Count == 1 ? $",{args[0]}" : string.Empty)})";
                break;
            case "CountDistinct":
                statement = $"COUNT(DISTINCT {quotedColName})";
                break;
            default:
                return base.VisitSqlMethodCall(m);
        }

        return new PartialSqlString(statement);
    }

    /// <summary>
    /// Converts to lengthpartialstring.
    /// </summary>
    /// <param name="arg">The argument.</param>
    /// <returns>PartialSqlString.</returns>
    protected override PartialSqlString ToLengthPartialString(object arg)
    {
        return new PartialSqlString($"LENGTH({arg})");
    }
}