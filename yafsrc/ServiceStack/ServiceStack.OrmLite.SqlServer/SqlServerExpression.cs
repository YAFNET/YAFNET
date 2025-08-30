// ***********************************************************************
// <copyright file="SqlServerExpression.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Data;
using System.Linq.Expressions;

using ServiceStack.OrmLite.Base.Text;
using ServiceStack.OrmLite.SqlServer.Converters;

namespace ServiceStack.OrmLite.SqlServer;

/// <summary>
/// Class SqlServerExpression.
/// Implements the <see cref="ServiceStack.OrmLite.SqlExpression{T}" />
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="ServiceStack.OrmLite.SqlExpression{T}" />
public class SqlServerExpression<T> : SqlExpression<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerExpression{T}" /> class.
    /// </summary>
    /// <param name="dialectProvider">The dialect provider.</param>
    public SqlServerExpression(IOrmLiteDialectProvider dialectProvider)
        : base(dialectProvider) { }

    /// <summary>
    /// Prepares the update statement.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="item">The item.</param>
    /// <param name="excludeDefaults">The exclude defaults.</param>
    public override void PrepareUpdateStatement(IDbCommand dbCmd, T item, bool excludeDefaults = false)
    {
        SqlServerExpressionUtils.PrepareSqlServerUpdateStatement(dbCmd, this, item, excludeDefaults);
    }

    /// <summary>
    /// Gets the coalesce expression.
    /// </summary>
    /// <param name="b">The b.</param>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>System.String.</returns>
    override protected string GetCoalesceExpression(BinaryExpression b, object left, object right)
    {
        return b.Type == typeof(bool) ? $"COALESCE({left},{right}) = 1" : $"COALESCE({left},{right})";
    }

    /// <summary>
    /// Gets the substring SQL.
    /// </summary>
    /// <param name="quotedColumn">The quoted column.</param>
    /// <param name="startIndex">The start index.</param>
    /// <param name="length">The length.</param>
    /// <returns>string.</returns>
    public override string GetSubstringSql(object quotedColumn, int startIndex, int? length = null)
    {
        return length != null
                   ? $"substring({quotedColumn}, {startIndex}, {length.Value})"
                   : $"substring({quotedColumn}, {startIndex}, LEN({quotedColumn}) - {startIndex} + 1)";
    }

    /// <summary>
    /// Converts to lengthpartialstring.
    /// </summary>
    /// <param name="arg">The argument.</param>
    /// <returns>ServiceStack.OrmLite.PartialSqlString.</returns>
    override protected PartialSqlString ToLengthPartialString(object arg)
    {
        return new PartialSqlString($"LEN({arg})");
    }

    /// <summary>
    /// Converts to placeholder and parameter.
    /// </summary>
    /// <param name="right">The right.</param>
    override protected void ConvertToPlaceholderAndParameter(ref object right)
    {
        var paramName = this.Params.Count.ToString();
        var paramValue = right;
        var parameter = this.CreateParam(paramName, paramValue);

        // Prevents a new plan cache for each different string length. Every string is parameterized as NVARCHAR(max)
        if (parameter.DbType == DbType.String)
        {
            parameter.Size = -1;
        }

        this.Params.Add(parameter);

        right = parameter.ParameterName;
    }

    /// <summary>
    /// Visits the filter.
    /// </summary>
    /// <param name="operand">The operand.</param>
    /// <param name="originalLeft">The original left.</param>
    /// <param name="originalRight">The original right.</param>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    override protected void VisitFilter(string operand, object originalLeft, object originalRight, ref object left, ref object right)
    {
        base.VisitFilter(operand, originalLeft, originalRight, ref left, ref right);

        if (originalRight is TimeSpan && this.DialectProvider.GetConverter<TimeSpan>() is SqlServerTimeConverter)
        {
            right = $"CAST({right} AS TIME)";
        }
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
}

/// <summary>
/// Class SqlServerExpressionUtils.
/// </summary>
internal class SqlServerExpressionUtils
{
    /// <summary>
    /// Prepares the SQL server update statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="q">The q.</param>
    /// <param name="item">The item.</param>
    /// <param name="excludeDefaults">if set to <c>true</c> [exclude defaults].</param>
    /// <exception cref="System.ArgumentException">No non-null or non-default values were provided for type: {typeof(T).Name}</exception>
    static internal void PrepareSqlServerUpdateStatement<T>(IDbCommand dbCmd, SqlExpression<T> q, T item, bool excludeDefaults = false)
    {
        q.CopyParamsTo(dbCmd);

        var modelDef = q.ModelDef;
        var dialectProvider = q.DialectProvider;

        var setFields = StringBuilderCache.Allocate();

        foreach (var fieldDef in modelDef.FieldDefinitions)
        {
            if (fieldDef.ShouldSkipUpdate())
            {
                continue;
            }

            if (fieldDef.IsRowVersion)
            {
                continue;
            }

            if (q.UpdateFields.Count > 0
                && !q.UpdateFields.Contains(fieldDef.Name)
                || fieldDef.AutoIncrement)
            {
                continue; // added
            }

            var value = fieldDef.GetValue(item);
            if (excludeDefaults
                && (value == null || !fieldDef.IsNullable && value.Equals(value.GetType().GetDefaultValue())))
            {
                continue;
            }

            if (setFields.Length > 0)
            {
                setFields.Append(", ");
            }

            setFields
                .Append(dialectProvider.GetQuotedColumnName(fieldDef))
                .Append('=')
                .Append(dialectProvider.GetUpdateParam(dbCmd, value, fieldDef));
        }

        var strFields = StringBuilderCache.ReturnAndFree(setFields);
        if (strFields.Length == 0)
        {
            throw new ArgumentException($"No non-null or non-default values were provided for type: {typeof(T).Name}");
        }

        dbCmd.CommandText = $"UPDATE {dialectProvider.GetQuotedTableName(modelDef)} SET {strFields} {q.WhereExpression}";
    }
}