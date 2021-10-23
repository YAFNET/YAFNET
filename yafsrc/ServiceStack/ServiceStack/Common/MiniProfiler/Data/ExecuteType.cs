// ***********************************************************************
// <copyright file="ExecuteType.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.MiniProfiler.Data
{
    /// <summary>
    /// Categories of sql statements.
    /// </summary>
    public enum ExecuteType : byte
    {
        /// <summary>
        /// Unknown
        /// </summary>
        None = 0,

        /// <summary>
        /// DML statements that alter database state, e.g. INSERT, UPDATE
        /// </summary>
        NonQuery = 1,

        /// <summary>
        /// Statements that return a single record
        /// </summary>
        Scalar = 2,

        /// <summary>
        /// Statements that iterate over a result set
        /// </summary>
        Reader = 3
    }
}
