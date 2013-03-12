/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Types.Interfaces.Data
{
    using System;
    using System.Data;
    using System.Data.Common;

    using ServiceStack.OrmLite;

    /// <summary>
    ///     The i db access v 2 extensions.
    /// </summary>
    public static class IDbAccessV2Extensions
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
        public static IDbTransaction BeginTransaction([NotNull] this IDbAccessV2 dbAccess, IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
        {
            CodeContracts.ArgumentNotNull(dbAccess, "dbAccess");

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
        public static DbConnection CreateConnection([NotNull] this IDbAccessV2 dbAccess)
        {
            CodeContracts.ArgumentNotNull(dbAccess, "dbAccess");

            var connection = dbAccess.DbProviderFactory.CreateConnection();
            connection.ConnectionString = dbAccess.ConnectionString;

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
        public static DbConnection CreateConnectionOpen([NotNull] this IDbAccessV2 dbAccess)
        {
            CodeContracts.ArgumentNotNull(dbAccess, "dbAccess");

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
            [NotNull] this IDbAccessV2 dbAccess, [NotNull] IDbCommand cmd, [CanBeNull] IDbTransaction dbTransaction = null)
        {
            CodeContracts.ArgumentNotNull(dbAccess, "dbAccess");
            CodeContracts.ArgumentNotNull(cmd, "cmd");

            return dbAccess.Execute((c) => c.ExecuteNonQuery(), cmd, dbTransaction);
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
            [NotNull] this IDbAccessV2 dbAccess, [NotNull] IDbCommand cmd, [CanBeNull] IDbTransaction dbTransaction = null)
        {
            CodeContracts.ArgumentNotNull(dbAccess, "dbAccess");
            CodeContracts.ArgumentNotNull(cmd, "cmd");

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
            [NotNull] this IDbAccessV2 dbAccess, [NotNull] IDbCommand cmd, [CanBeNull] IDbTransaction dbTransaction = null)
        {
            CodeContracts.ArgumentNotNull(dbAccess, "dbAccess");
            CodeContracts.ArgumentNotNull(cmd, "cmd");

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
        public static DataSet GetDataset([NotNull] this IDbAccessV2 dbAccess, [NotNull] IDbCommand cmd, [CanBeNull] IDbTransaction dbTransaction = null)
        {
            CodeContracts.ArgumentNotNull(dbAccess, "dbAccess");
            CodeContracts.ArgumentNotNull(cmd, "cmd");

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
        public static IDataReader GetReader([NotNull] this IDbAccessV2 dbAccess, [NotNull] IDbCommand cmd, [NotNull] IDbTransaction dbTransaction)
        {
            CodeContracts.ArgumentNotNull(dbAccess, "dbAccess");
            CodeContracts.ArgumentNotNull(cmd, "cmd");
            CodeContracts.ArgumentNotNull(dbTransaction, "dbTransaction");

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
        public static string GetTableName<T>(this IDbAccessV2 dbAccess)
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
        public static int Insert<T>([NotNull] this IDbAccessV2 dbAccess, [NotNull] T insert, [CanBeNull] IDbTransaction transaction = null)
            where T : IEntity
        {
            CodeContracts.ArgumentNotNull(dbAccess, "dbAccess");

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
        public static T Run<T>([NotNull] this IDbAccessV2 dbAccess, Func<IDbConnection, T> runFunc)
        {
            CodeContracts.ArgumentNotNull(dbAccess, "dbAccess");
            CodeContracts.ArgumentNotNull(runFunc, "runFunc");

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
        public static int Update<T>([NotNull] this IDbAccessV2 dbAccess, [NotNull] T update, [CanBeNull] IDbTransaction transaction = null)
            where T : IEntity
        {
            CodeContracts.ArgumentNotNull(dbAccess, "dbAccess");

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