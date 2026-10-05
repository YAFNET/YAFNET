// ***********************************************************************
// <copyright file="SqlExpression.SystemTime.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;

namespace ServiceStack.OrmLite;

public abstract partial class SqlExpression<T>
{
    // FOR SYSTEM_TIME ... of the query's table, which reads previous versions of its rows
    private string systemTimeClause;

    /// <summary>
    /// Whether this query reads previous versions of a system-versioned table, e.g. with AsOf()
    /// </summary>
    public bool HasSystemTime => this.systemTimeClause != null;

    /// <summary>
    /// Query a system-versioned table as it was at a time, e.g:
    /// <para>db.From&lt;Product&gt;().AsOf(new DateTime(2026, 1, 1))</para>
    /// The time is in UTC for SQL Server, and the time zone of the connection for MariaDB.
    /// </summary>
    public virtual SqlExpression<T> AsOf(DateTime time) =>
        this.ForSystemTime("AS OF " + this.AddSystemTimeParam(time));

    /// <summary>
    /// Query every version of the rows of a system-versioned table that were current at any time between two
    /// times, e.g. to see the changes that were made to a row:
    /// <para>db.From&lt;Product&gt;().VersionsBetween(from, to).Where(x => x.Id == 1)</para>
    /// </summary>
    public virtual SqlExpression<T> VersionsBetween(DateTime from, DateTime to) =>
        this.ForSystemTime("BETWEEN " + this.AddSystemTimeParam(from) + " AND " + this.AddSystemTimeParam(to));

    /// <summary>
    /// Query every version of the rows of a system-versioned table, current and previous
    /// </summary>
    public virtual SqlExpression<T> AllVersions() => this.ForSystemTime("ALL");

    private string AddSystemTimeParam(DateTime time) => this.ConvertToParam(this.DialectProvider.ToSystemTime(time));

    private SqlExpression<T> ForSystemTime(string condition)
    {
        if (this.systemTimeClause != null)
        {
            throw new InvalidOperationException("This query already reads the versions of its table as of a time");
        }

        var clause = this.DialectProvider.ToSystemTimeClause(condition);

        // Follows the query's table, before its alias and any joins
        if (!string.IsNullOrEmpty(this.fromExpression))
        {
            var fromTable = " \nFROM " + this.DialectProvider.GetQuotedTableName(this.modelDef);
            if (!this.fromExpression.StartsWith(fromTable, StringComparison.Ordinal))
            {
                throw new NotSupportedException("The versions of a table can't be queried with a custom FROM expression");
            }

            this.fromExpression = fromTable + " " + clause + this.fromExpression.Substring(fromTable.Length);
        }

        this.systemTimeClause = clause;
        return this;
    }
}
