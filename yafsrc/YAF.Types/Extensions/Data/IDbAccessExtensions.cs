/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Types.Extensions.Data
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Linq.Expressions;

    using ServiceStack.OrmLite;

    using YAF.Types.Interfaces.Data;

    /// <summary>
    ///     The DBAccess extensions.
    /// </summary>
    public static class IDbAccessExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The begin transaction.
        /// </summary>
        /// <param name="dbAccess">
        /// The DB access.
        /// </param>
        /// <param name="isolationLevel">
        /// The isolation level.
        /// </param>
        /// <returns>
        /// The <see cref="IDbTransaction"/> .
        /// </returns>
        public static IDbTransaction BeginTransaction(
            [NotNull] this IDbAccess dbAccess,
            IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
        {
            CodeContracts.VerifyNotNull(dbAccess, "dbAccess");

            return dbAccess.CreateConnectionOpen().BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <param name="dbAccess">The DB access.</param>
        /// <returns>
        /// The <see cref="DbConnection" /> .
        /// </returns>
        [NotNull]
        public static DbConnection CreateConnection([NotNull] this IDbAccess dbAccess)
        {
            CodeContracts.VerifyNotNull(dbAccess, "dbAccess");

            var connection = dbAccess.DbProviderFactory.CreateConnection();
            connection.ConnectionString = dbAccess.Information.ConnectionString();

            return connection;
        }

        /// <summary>
        /// Get an open DB connection.
        /// </summary>
        /// <param name="dbAccess">
        /// The DB Access.
        /// </param>
        /// <returns>
        /// The <see cref="DbConnection"/> .
        /// </returns>
        [NotNull]
        public static DbConnection CreateConnectionOpen([NotNull] this IDbAccess dbAccess)
        {
            CodeContracts.VerifyNotNull(dbAccess, "dbAccess");

            var connection = dbAccess.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                // open it up...
                connection.Open();
            }

            return connection;
        }

        /// <summary>
        /// Executes a non query in a transaction
        /// </summary>
        /// <param name="dbAccess">
        /// The DB access.
        /// </param>
        /// <param name="cmd">
        /// The command.
        /// </param>
        /// <param name="dbTransaction">
        /// The DB Transaction.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> .
        /// </returns>
        public static int ExecuteNonQuery(
            [NotNull] this IDbAccess dbAccess,
            [NotNull] IDbCommand cmd,
            [CanBeNull] IDbTransaction dbTransaction = null)
        {
            CodeContracts.VerifyNotNull(dbAccess, "dbAccess");
            CodeContracts.VerifyNotNull(cmd, "cmd");

            return dbAccess.Execute(c => c.ExecuteNonQuery(), cmd, dbTransaction);
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="dbAccess">The DB access.</param>
        /// <param name="cmd">The command.</param>
        /// <param name="dbTransaction">The DB Transaction.</param>
        /// <returns>
        /// Returns the Data
        /// </returns>
        public static object ExecuteScalar(
            [NotNull] this IDbAccess dbAccess,
            [NotNull] IDbCommand cmd,
            [CanBeNull] IDbTransaction dbTransaction = null)
        {
            CodeContracts.VerifyNotNull(dbAccess, "dbAccess");
            CodeContracts.VerifyNotNull(cmd, "cmd");

            return dbAccess.Execute(c => c.ExecuteScalar(), cmd, dbTransaction);
        }

        /// <summary>
        /// Runs the update command.
        /// </summary>
        /// <typeparam name="T">
        /// The type Parameter
        /// </typeparam>
        /// <param name="dbAccess">
        /// The DB access.
        /// </param>
        /// <param name="update">
        /// The update.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int Update<T>(
            [NotNull] this IDbAccess dbAccess,
            [NotNull] T update)
            where T : IEntity
        {
            CodeContracts.VerifyNotNull(dbAccess, "dbAccess");

            return dbAccess.Execute(
                db => db.Connection.Update(
                    update));
        }

        /// <summary>
        /// Update record, updating only fields specified in updateOnly that matches the where condition (if any), E.g:
        /// Numeric fields generates an increment sql which is useful to increment counters, etc...
        /// avoiding concurrency conflicts
        /// 
        ///   db.UpdateAdd(() => new Person { Age = 5 }, where: p => p.LastName == "Hendrix");
        ///   UPDATE "Person" SET "Age" = "Age" + 5 WHERE ("LastName" = 'Hendrix')
        /// 
        ///   db.UpdateAdd(() => new Person { Age = 5 });
        ///   UPDATE "Person" SET "Age" = "Age" + 5
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbAccess">The database access.</param>
        /// <param name="updateFields">The update fields.</param>
        /// <param name="where">The where.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <returns></returns>
        public static int UpdateAdd<T>(
            [NotNull] this IDbAccess dbAccess,
            Expression<Func<T>> updateFields,
            Expression<Func<T, bool>> where = null,
            Action<IDbCommand> commandFilter = null)
            where T : class, IEntity, IHaveID, new()
        {
            return dbAccess.Execute(
                db => db.Connection.UpdateAdd(
                    updateFields,
                    OrmLiteConfig.DialectProvider.SqlExpression<T>().Where(where),
                    commandFilter));
        }

        /// <summary>
        ///  Update only fields in the specified expression that matches the where condition (if any), E.g:
        ///   
        ///   db.UpdateOnly(() => new Person { FirstName = "JJ" }, where: p => p.LastName == "Hendrix");
        ///   UPDATE "Person" SET "FirstName" = 'JJ' WHERE ("LastName" = 'Hendrix')
        ///
        ///   db.UpdateOnly(() => new Person { FirstName = "JJ" });
        ///   UPDATE "Person" SET "FirstName" = 'JJ'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbAccess">The database access.</param>
        /// <param name="updateFields">The update fields.</param>
        /// <param name="where">The where.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <returns></returns>
        public static int UpdateOnly<T>(
            [NotNull] this IDbAccess dbAccess,
            Expression<Func<T>> updateFields,
            Expression<Func<T, bool>> where = null,
            Action<IDbCommand> commandFilter = null)
            where T : class, IEntity, new()
        {
            return dbAccess.Execute(
                db => db.Connection.UpdateOnly(
                    updateFields,
                    OrmLiteConfig.DialectProvider.SqlExpression<T>().Where(where),
                    commandFilter));
        }

        /// <summary>
        /// Returns true if the Query returns any records that match the supplied SqlExpression, E.g:
        /// <para>db.Exists(db.From&lt;Person&gt;().Where(x =&gt; x.Age &lt; 50))</para>
        /// </summary>
        public static bool Exists<T>([NotNull] this IDbAccess dbAccess, Expression<Func<T, bool>> where = null)
            where T : class, IEntity, new()
        {
            return dbAccess.Execute(
                db => db.Connection.Exists(OrmLiteConfig.DialectProvider.SqlExpression<T>().Where(where)));
        }

        #endregion
    }
}