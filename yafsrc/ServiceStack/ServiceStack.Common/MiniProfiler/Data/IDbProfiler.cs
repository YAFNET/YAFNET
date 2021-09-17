// ***********************************************************************
// <copyright file="IDbProfiler.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Data.Common;

namespace ServiceStack.MiniProfiler.Data
{
    /// <summary>
    /// A callback for ProfiledDbConnection and family
    /// </summary>
    public interface IDbProfiler
    {
        /// <summary>
        /// Called when a command starts executing
        /// </summary>
        /// <param name="profiledDbCommand">The profiled database command.</param>
        /// <param name="executeType">Type of the execute.</param>
        void ExecuteStart(DbCommand profiledDbCommand, ExecuteType executeType);

        /// <summary>
        /// Called when a reader finishes executing
        /// </summary>
        /// <param name="profiledDbCommand">The profiled database command.</param>
        /// <param name="executeType">Type of the execute.</param>
        /// <param name="reader">The reader.</param>
        void ExecuteFinish(DbCommand profiledDbCommand, ExecuteType executeType, DbDataReader reader);

        /// <summary>
        /// Called when a reader is done iterating through the data
        /// </summary>
        /// <param name="reader">The reader.</param>
        void ReaderFinish(DbDataReader reader);

        /// <summary>
        /// Called when an error happens during execution of a command
        /// </summary>
        /// <param name="profiledDbCommand">The profiled database command.</param>
        /// <param name="executeType">Type of the execute.</param>
        /// <param name="exception">The exception.</param>
        void OnError(DbCommand profiledDbCommand, ExecuteType executeType, Exception exception);

        /// <summary>
        /// True if the profiler instance is active
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        bool IsActive { get; }
    }
}
