// ***********************************************************************
// <copyright file="DbConnectionFactory.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
#if !SL5
using System;
using System.Data;

namespace ServiceStack.Data
{
    /// <summary>
    /// Class DbConnectionFactory.
    /// Implements the <see cref="ServiceStack.Data.IDbConnectionFactory" />
    /// </summary>
    /// <seealso cref="ServiceStack.Data.IDbConnectionFactory" />
    public class DbConnectionFactory : IDbConnectionFactory
    {
        /// <summary>
        /// The connection factory function
        /// </summary>
        private readonly Func<IDbConnection> connectionFactoryFn;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbConnectionFactory"/> class.
        /// </summary>
        /// <param name="connectionFactoryFn">The connection factory function.</param>
        public DbConnectionFactory(Func<IDbConnection> connectionFactoryFn)
        {
            this.connectionFactoryFn = connectionFactoryFn;
        }

        /// <summary>
        /// Opens the database connection.
        /// </summary>
        /// <returns>IDbConnection.</returns>
        public IDbConnection OpenDbConnection()
        {
            var dbConn = CreateDbConnection();
            dbConn.Open();
            return dbConn;
        }

        /// <summary>
        /// Creates the database connection.
        /// </summary>
        /// <returns>IDbConnection.</returns>
        public IDbConnection CreateDbConnection()
        {
            return connectionFactoryFn();
        }
    }
}
#endif