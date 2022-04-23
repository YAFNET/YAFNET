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
public class SqlServerTableHint
{
    /// <summary>
    /// The read uncommitted
    /// </summary>
    public static JoinFormatDelegate ReadUncommitted = (dialect, tableDef, expr) => $"{dialect.GetQuotedTableName(tableDef)} WITH (READUNCOMMITTED) {expr}";
    /// <summary>
    /// The read committed
    /// </summary>
    public static JoinFormatDelegate ReadCommitted = (dialect, tableDef, expr) => $"{dialect.GetQuotedTableName(tableDef)} WITH (READCOMMITTED) {expr}";
    /// <summary>
    /// The read past
    /// </summary>
    public static JoinFormatDelegate ReadPast = (dialect, tableDef, expr) => $"{dialect.GetQuotedTableName(tableDef)} WITH (READPAST) {expr}";
    /// <summary>
    /// The serializable
    /// </summary>
    public static JoinFormatDelegate Serializable = (dialect, tableDef, expr) => $"{dialect.GetQuotedTableName(tableDef)} WITH (SERIALIZABLE) {expr}";
    /// <summary>
    /// The repeatable read
    /// </summary>
    public static JoinFormatDelegate RepeatableRead = (dialect, tableDef, expr) => $"{dialect.GetQuotedTableName(tableDef)} WITH (REPEATABLEREAD) {expr}";
    /// <summary>
    /// The no lock
    /// </summary>
    public static JoinFormatDelegate NoLock = (dialect, tableDef, expr) => $"{dialect.GetQuotedTableName(tableDef)} WITH (NOLOCK) {expr}";
}