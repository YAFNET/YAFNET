/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2015 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Types.Interfaces.Data
{
    using System;
    using System.Data;
    using System.Data.Common;

    using ServiceStack.OrmLite;

    /// <summary>
    ///     The DbAccess extensions.
    /// </summary>
    public static class IDbAccessExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The begin transaction.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        /// <param name="isolationLevel">
        /// The isolation level.
        /// </param>
        /// <returns>
        /// The <see cref="IDbTransaction"/> .
        /// </returns>
        public static IDbTransaction BeginTransaction([NotNull] this IDbAccess dbAccess, IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
        {
            CodeContracts.VerifyNotNull(dbAccess, "dbAccess");

            return dbAccess.CreateConnectionOpen().BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// The create connection.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        /// <returns>
        /// The <see cref="DbConnection"/> .
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
        /// Get an open db connection.
        /// </summary>
        /// <param name="dbAccess">
        /// The db Access.
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
        /// The execute non query.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        /// <param name="cmd">
        /// The cmd.
        /// </param>
        /// <param name="dbTransaction">
        /// The db Transaction.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> .
        /// </returns>
        public static int ExecuteNonQuery(
            [NotNull] this IDbAccess dbAccess, [NotNull] IDbCommand cmd, [CanBeNull] IDbTransaction dbTransaction = null)
        {
            CodeContracts.VerifyNotNull(dbAccess, "dbAccess");
            CodeContracts.VerifyNotNull(cmd, "cmd");

            return dbAccess.Execute((c) => c.ExecuteNonQuery(), cmd, dbTransaction);
        }

        /// <summary>
        /// Executes a non query in a transaction
        /// </summary>
        /// <param name="dbAccess"></param>
        /// <param name="cmd"></param>
        /// <param name="useTransaction"></param>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(
            [NotNull] this IDbAccess dbAccess, [NotNull] IDbCommand cmd, bool useTransaction, IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
        {
            CodeContracts.VerifyNotNull(dbAccess, "dbAccess");
            CodeContracts.VerifyNotNull(cmd, "cmd");

            if (!useTransaction) return dbAccess.ExecuteNonQuery(cmd);

            using (var dbTransaction = dbAccess.BeginTransaction(isolationLevel))
            {
                var result = dbAccess.Execute((c) => c.ExecuteNonQuery(), cmd, dbTransaction);
                dbTransaction.Commit();

                return result;
            }
        }

        /// <summary>
        /// The execute scalar.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        /// <param name="cmd">
        /// The cmd.
        /// </param>
        /// <param name="dbTransaction">
        /// The db Transaction.
        /// </param>
        /// <returns>
        /// The execute scalar.
        /// </returns>
        public static object ExecuteScalar(
            [NotNull] this IDbAccess dbAccess, [NotNull] IDbCommand cmd, [CanBeNull] IDbTransaction dbTransaction = null)
        {
            CodeContracts.VerifyNotNull(dbAccess, "dbAccess");
            CodeContracts.VerifyNotNull(cmd, "cmd");

            return dbAccess.Execute((c) => c.ExecuteScalar(), cmd, dbTransaction);
        }

        /// <summary>
        /// The get data.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        /// <param name="cmd">
        /// The cmd.
        /// </param>
        /// <param name="dbTransaction">
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/> .
        /// </returns>
        public static DataTable GetData(
            [NotNull] this IDbAccess dbAccess, [NotNull] IDbCommand cmd, [CanBeNull] IDbTransaction dbTransaction = null)
        {
            CodeContracts.VerifyNotNull(dbAccess, "dbAccess");
            CodeContracts.VerifyNotNull(cmd, "cmd");

            return dbAccess.GetDataset(cmd, dbTransaction).Tables[0];
        }

        /// <summary>
        /// The get dataset.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        /// <param name="cmd">
        /// The cmd.
        /// </param>
        /// <param name="dbTransaction">
        /// </param>
        /// <returns>
        /// The <see cref="DataSet"/> .
        /// </returns>
        public static DataSet GetDataset([NotNull] this IDbAccess dbAccess, [NotNull] IDbCommand cmd, [CanBeNull] IDbTransaction dbTransaction = null)
        {
            CodeContracts.VerifyNotNull(dbAccess, "dbAccess");
            CodeContracts.VerifyNotNull(cmd, "cmd");

            return dbAccess.Execute(
                (c) =>
                    {
                        var ds = new DataSet();

                        IDbDataAdapter dataAdapter = dbAccess.DbProviderFactory.CreateDataAdapter();

                        if (dataAdapter != null)
                        {
                            dataAdapter.SelectCommand = cmd;
                            dataAdapter.Fill(ds);
                        }

                        return ds;
                    }, 
                cmd, 
                dbTransaction);
        }

        /// <summary>
        /// The get reader.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        /// <param name="cmd">
        /// The cmd.
        /// </param>
        /// <param name="dbTransaction">
        /// The db transaction.
        /// </param>
        /// <returns>
        /// The <see cref="IDataReader"/> .
        /// </returns>
        public static IDataReader GetReader([NotNull] this IDbAccess dbAccess, [NotNull] IDbCommand cmd, [NotNull] IDbTransaction dbTransaction)
        {
            CodeContracts.VerifyNotNull(dbAccess, "dbAccess");
            CodeContracts.VerifyNotNull(cmd, "cmd");
            CodeContracts.VerifyNotNull(dbTransaction, "dbTransaction");

            return dbAccess.Execute((c) => c.ExecuteReader(), cmd, dbTransaction);
        }

        /// <summary>
        /// The get table.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetTableName<T>(this IDbAccess dbAccess)
        {
            return OrmLiteConfig.DialectProvider.GetQuotedTableName(ModelDefinition<T>.Definition);
        }

        /// <summary>
        /// Insert the entity using the model provided.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        /// <param name="insert">
        /// The insert.
        /// </param>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int Insert<T>([NotNull] this IDbAccess dbAccess, [NotNull] T insert, [CanBeNull] IDbTransaction transaction = null)
            where T : IEntity
        {
            CodeContracts.VerifyNotNull(dbAccess, "dbAccess");

            if (transaction != null && transaction.Connection != null)
            {
                using (var command = OrmLiteConfig.DialectProvider.CreateParameterizedInsertStatement(insert, transaction.Connection))
                {
                    command.Populate(transaction);
                    dbAccess.ExecuteNonQuery(command, transaction);

                    return (int)OrmLiteConfig.DialectProvider.GetLastInsertId(command);
                }
            }

            // no transaction
            using (var connection = dbAccess.CreateConnectionOpen())
            {
                using (var command = OrmLiteConfig.DialectProvider.CreateParameterizedInsertStatement(insert, connection))
                {
                    command.Connection = connection;
                    dbAccess.ExecuteNonQuery(command, transaction);

                    return (int)OrmLiteConfig.DialectProvider.GetLastInsertId(command);
                }
            }
        }

        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        /// <param name="runFunc">
        /// The run func.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/> .
        /// </returns>
        public static T Run<T>([NotNull] this IDbAccess dbAccess, Func<IDbConnection, T> runFunc)
        {
            CodeContracts.VerifyNotNull(dbAccess, "dbAccess");
            CodeContracts.VerifyNotNull(runFunc, "runFunc");

            using (var connection = dbAccess.CreateConnectionOpen())
            {
                return runFunc(connection);
            }
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        /// <param name="update">
        /// The update.
        /// </param>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int Update<T>([NotNull] this IDbAccess dbAccess, [NotNull] T update, [CanBeNull] IDbTransaction transaction = null)
            where T : IEntity
        {
            CodeContracts.VerifyNotNull(dbAccess, "dbAccess");

            using (var connection = dbAccess.CreateConnection())
            {
                using (var command = OrmLiteConfig.DialectProvider.CreateParameterizedUpdateStatement(update, connection))
                {
                    return dbAccess.ExecuteNonQuery(command, transaction);
                }
            }
        }

        #endregion
    }
}