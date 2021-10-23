// ***********************************************************************
// <copyright file="SqlServer2019OrmLiteDialectProvider.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.OrmLite.SqlServer
{
    /// <summary>
    /// Class SqlServer2019OrmLiteDialectProvider.
    /// Implements the <see cref="SqlServer2017OrmLiteDialectProvider" />
    /// </summary>
    /// <seealso cref="SqlServer2017OrmLiteDialectProvider" />
    public class SqlServer2019OrmLiteDialectProvider : SqlServer2017OrmLiteDialectProvider
    {
        /// <summary>
        /// The instance
        /// </summary>
        public static new SqlServer2019OrmLiteDialectProvider Instance = new();
    }
}