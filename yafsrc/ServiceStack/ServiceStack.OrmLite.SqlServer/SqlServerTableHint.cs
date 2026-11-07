// ***********************************************************************
// <copyright file="SqlServerTableHint.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.OrmLite.SqlServer;

/// <summary>
/// Class SqlServerTableHint.
/// </summary>
public static class SqlServerTableHint
{
    /// <summary>
    /// The read uncommitted
    /// </summary>
    public static readonly JoinFormatDelegate ReadUncommitted = (dialect, tableDef, expr) => $"{dialect.GetQuotedTableName(tableDef)} WITH (READUNCOMMITTED) {expr}";
    /// <summary>
    /// The read committed
    /// </summary>
    public static readonly JoinFormatDelegate ReadCommitted = (dialect, tableDef, expr) => $"{dialect.GetQuotedTableName(tableDef)} WITH (READCOMMITTED) {expr}";
    /// <summary>
    /// The read past
    /// </summary>
    public static readonly JoinFormatDelegate ReadPast = (dialect, tableDef, expr) => $"{dialect.GetQuotedTableName(tableDef)} WITH (READPAST) {expr}";
    /// <summary>
    /// The serializable
    /// </summary>
    public static readonly JoinFormatDelegate Serializable = (dialect, tableDef, expr) => $"{dialect.GetQuotedTableName(tableDef)} WITH (SERIALIZABLE) {expr}";
    /// <summary>
    /// The repeatable read
    /// </summary>
    public static readonly JoinFormatDelegate RepeatableRead = (dialect, tableDef, expr) => $"{dialect.GetQuotedTableName(tableDef)} WITH (REPEATABLEREAD) {expr}";
    /// <summary>
    /// The no lock
    /// </summary>
    public static readonly JoinFormatDelegate NoLock = (dialect, tableDef, expr) => $"{dialect.GetQuotedTableName(tableDef)} WITH (NOLOCK) {expr}";
}