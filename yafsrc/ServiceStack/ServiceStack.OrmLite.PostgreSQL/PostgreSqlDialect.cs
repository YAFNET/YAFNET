// ***********************************************************************
// <copyright file="PostgreSqlDialect.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite
{
    using ServiceStack.OrmLite.PostgreSQL;

    /// <summary>
    /// Class PostgreSqlDialect.
    /// </summary>
    public static class PostgreSqlDialect
    {
        /// <summary>
        /// Gets the provider.
        /// </summary>
        /// <value>The provider.</value>
        public static IOrmLiteDialectProvider Provider => PostgreSqlDialectProvider.Instance;
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static PostgreSqlDialectProvider Instance => PostgreSqlDialectProvider.Instance;
    }
}