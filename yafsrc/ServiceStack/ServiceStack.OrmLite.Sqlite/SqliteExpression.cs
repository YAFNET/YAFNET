// ***********************************************************************
// <copyright file="SqliteExpression.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.Sqlite;

using System;
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
        : base(dialectProvider)
    {
    }

    /// <summary>
    /// Visits the column access method.
    /// </summary>
    /// <param name="m">The m.</param>
    /// <returns>object.</returns>
    override protected object VisitColumnAccessMethod(MethodCallExpression m)
    {
        var args = this.VisitExpressionList(m.Arguments);
        var quotedColName = this.Visit(m.Object);

        if (!IsSqlClass(quotedColName))
        {
            quotedColName = this.ConvertToParam(quotedColName);
        }

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
                {
                    statement = $"substr({quotedColName}, {startIndex})";
                }

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
    override protected object VisitSqlMethodCall(MethodCallExpression m)
    {
        var args = this.VisitInSqlExpressionList(m.Arguments);
        object quotedColName = args[0];
        args.RemoveAt(0);

        string statement;

        switch (m.Method.Name)
        {
            case nameof(string.ToString) when m.Object?.Type == typeof(DateTime):
            {
                var arg = args.Count > 0 ? args[0] : null;
                statement = arg == null ? this.ToCast(quotedColName.ToString()) : $"strftime('{arg}',{quotedColName})";

                return new PartialSqlString(statement);
            }
            case nameof(string.Substring):
            {
                var startIndex = int.Parse(args[0].ToString()) + 1;
                if (args.Count == 2)
                {
                    var length = int.Parse(args[1].ToString());
                    statement = $"substr({quotedColName}, {startIndex}, {length})";
                }
                else
                {
                    statement = $"substr({quotedColName}, {startIndex})";
                }

                return new PartialSqlString(statement);
            }
            default:
                return base.VisitSqlMethodCall(m);
        }
    }

    /// <summary>
    /// Converts to lengthpartialstring.
    /// </summary>
    /// <param name="arg">The argument.</param>
    /// <returns>PartialSqlString.</returns>
    override protected PartialSqlString ToLengthPartialString(object arg)
    {
        return new PartialSqlString($"LENGTH({arg})");
    }

    protected override object VisitIsJsonMethod(object json) =>
        new PartialSqlString($"json_valid({json})");

    protected override object VisitJsonValueMethod(object json, JsonPathExpression path, Type returnType)
    {
        var extract = $"json_extract({json}, {path})";
        var jsonType = $"json_type({json}, {path})";
        var type = Nullable.GetUnderlyingType(returnType) ?? returnType;

        if (type == typeof(string) || type.IsEnum)
            return JsonScalar(
                $"CASE WHEN {jsonType} NOT IN ('object','array','null') THEN {extract} END", returnType);

        if (type == typeof(bool))
            return JsonScalar(
                $"CASE WHEN {jsonType} IN ('true','false') THEN {extract} END", returnType);

        if (type == typeof(char) || type == typeof(Guid) || type == typeof(DateTime)
            || type == typeof(DateTimeOffset) || type == typeof(TimeSpan))
            return JsonScalar(
                $"CASE WHEN {jsonType} = 'text' THEN {extract} END", returnType);

        return JsonScalar(
            $"CAST(CASE WHEN {jsonType} IN ('integer','real') THEN {extract} END AS {GetJsonDbType(type)})", returnType);
    }

    protected override object VisitJsonQueryMethod(object json, JsonPathExpression path, Type returnType)
    {
        var extract = $"json_extract({json}, {path})";
        return new PartialSqlString(
            $"CASE WHEN json_type({json}, {path}) IN ('object','array') THEN {extract} END");
    }

    protected override object VisitJsonExistsMethod(object json, JsonPathExpression path) =>
        new PartialSqlString($"(json_type({json}, {path}) IS NOT NULL)");

    protected override object VisitJsonTypeMethod(object json, JsonPathExpression path) => JsonValueType(
        $"CASE json_type({json}, {path}) " +
        "WHEN 'null' THEN 'Null' WHEN 'text' THEN 'String' " +
        "WHEN 'integer' THEN 'Number' WHEN 'real' THEN 'Number' " +
        "WHEN 'true' THEN 'Boolean' WHEN 'false' THEN 'Boolean' " +
        "WHEN 'array' THEN 'Array' WHEN 'object' THEN 'Object' END");

    protected override object VisitJsonArrayLengthMethod(object json, JsonPathExpression path) =>
        new PartialSqlString(
            $"CASE WHEN json_type({json}, {path}) = 'array' THEN json_array_length({json}, {path}) END");

    protected override object VisitJsonArrayContainsMethod(object json, JsonPathExpression path, object value, Type valueType)
    {
        valueType = Nullable.GetUnderlyingType(valueType) ?? valueType;
        string typePredicate;
        if (value.ToString() == "null")
            return new PartialSqlString(
                $"EXISTS (SELECT 1 FROM json_each({json}, {path}) j WHERE j.type = 'null')");
        if (valueType == typeof(bool))
            typePredicate = "j.type IN ('true','false')";
        else if (valueType == typeof(string) || valueType == typeof(char) || valueType == typeof(Guid)
                 || valueType == typeof(DateTime) || valueType == typeof(DateTimeOffset) || valueType.IsEnum)
            typePredicate = "j.type = 'text'";
        else
            typePredicate = "j.type IN ('integer','real')";

        return new PartialSqlString(
            $"EXISTS (SELECT 1 FROM json_each({json}, {path}) j WHERE {typePredicate} AND j.atom = {value})");
    }

    protected override PartialSqlString ToLengthPartialString(object arg)
    {
        return new PartialSqlString($"LENGTH({arg})");
    }
}