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
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Text.RegularExpressions;

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
        /// The Database access.
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
            CodeContracts.VerifyNotNull(dbAccess);

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
            CodeContracts.VerifyNotNull(dbAccess);

            var connection = dbAccess.DbProviderFactory.CreateConnection();
            connection.ConnectionString = dbAccess.Information.ConnectionString();

            return connection;
        }

        /// <summary>
        /// Get an open DB connection.
        /// </summary>
        /// <param name="dbAccess">
        /// The Database access.
        /// </param>
        /// <returns>
        /// The <see cref="DbConnection"/> .
        /// </returns>
        [NotNull]
        public static DbConnection CreateConnectionOpen([NotNull] this IDbAccess dbAccess)
        {
            CodeContracts.VerifyNotNull(dbAccess);

            var connection = dbAccess.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                // open it up...
                connection.Open();
            }

            return connection;
        }

        /// <summary>
        /// Runs the update command.
        /// </summary>
        /// <typeparam name="T">
        /// The type Parameter
        /// </typeparam>
        /// <param name="dbAccess">
        /// The Database access.
        /// </param>
        /// <param name="update">
        /// The update.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int Update<T>([NotNull] this IDbAccess dbAccess, [NotNull] T update)
            where T : IEntity
        {
            CodeContracts.VerifyNotNull(dbAccess);

            return dbAccess.Execute(db => db.Connection.Update(update));
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
                db => db.Connection.UpdateOnlyFields(
                    updateFields,
                    where, //OrmLiteConfig.DialectProvider.SqlExpression<T>().Where(where),
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

        /// <summary>
        /// The get database fragmentation info.
        /// </summary>
        /// <param name="dbAccess">
        /// The Database access.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetDatabaseFragmentationInfo([NotNull] this IDbAccess dbAccess)
        {
            CodeContracts.VerifyNotNull(dbAccess);

            var result = new StringBuilder();

            try
            {
                var infos = dbAccess.Execute(
                    db => db.Connection.SqlList<dynamic>(
                        OrmLiteConfig.DialectProvider.DatabaseFragmentationInfo(db.Connection.Database)));

                if (infos == null)
                {
                    return null;
                }

                infos.ForEach(
                    info =>
                    {
                        IDictionary<string, object> propertyValues = info;

                        propertyValues.Keys.ForEach(
                            property => result.AppendFormat("{0} : {1} ", property, propertyValues[property]));

                        result.Append("\r\n");
                    });
            }
            catch (Exception)
            {
                return $"Not Supported by {OrmLiteConfig.DialectProvider.SQLServerName()} Server";
            }

            return result.ToString();
        }

        /// <summary>
        /// Gets the size of the database.
        /// </summary>
        /// <param name="dbAccess">The database access.</param>
        /// <returns>Returns the size of the database</returns>
        public static int GetDatabaseSize([NotNull] this IDbAccess dbAccess)
        {
            CodeContracts.VerifyNotNull(dbAccess);

            return dbAccess.Execute(
                db => db.Connection.Scalar<int>(OrmLiteConfig.DialectProvider.DatabaseSize(db.Connection.Database)));
        }

        /// <summary>
        /// Gets the current SQL Engine Edition.
        /// </summary>
        /// <param name="dbAccess">The database access.</param>
        /// <returns>
        /// Returns the current SQL Engine Edition.
        /// </returns>
        public static string GetSQLVersion([NotNull] this IDbAccess dbAccess)
        {
            CodeContracts.VerifyNotNull(dbAccess);

            var version = dbAccess.Execute(
                db => db.Connection.Scalar<string>(OrmLiteConfig.DialectProvider.SQLVersion()));

            var serverName = OrmLiteConfig.DialectProvider.SQLServerName();

            return version.StartsWith(serverName) ? version : $"{serverName} {version}";
        }

        /// <summary>
        /// The shrink database.
        /// </summary>
        /// <param name="dbAccess">The database access.</param>
        public static string ShrinkDatabase([NotNull] this IDbAccess dbAccess)
        {
            CodeContracts.VerifyNotNull(dbAccess);

            return dbAccess.Execute(
                db => db.Connection.Scalar<string>(
                    OrmLiteConfig.DialectProvider.ShrinkDatabase(db.Connection.Database)));
        }

        /// <summary>
        /// Re-Index the Database
        /// </summary>
        /// <param name="dbAccess">
        /// The database access.
        /// </param>
        /// <param name="objectQualifier">
        /// The object Qualifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ReIndexDatabase([NotNull] this IDbAccess dbAccess, string objectQualifier)
        {
            CodeContracts.VerifyNotNull(dbAccess);

            return dbAccess.Execute(
                db => db.Connection.Scalar<string>(
                    OrmLiteConfig.DialectProvider.ReIndexDatabase(db.Connection.Database, objectQualifier)));
        }

        /// <summary>
        /// Change Database Recovery Mode
        /// </summary>
        /// <param name="dbAccess">
        /// The Database access.
        /// </param>
        /// <param name="recoveryMode">
        /// The recovery mode.
        /// </param>
        public static string ChangeRecoveryMode(this IDbAccess dbAccess, [NotNull] string recoveryMode)
        {
            CodeContracts.VerifyNotNull(dbAccess);

            try
            {
                return dbAccess.Execute(
                    db => db.Connection.Scalar<string>(
                        OrmLiteConfig.DialectProvider.ReIndexDatabase(db.Connection.Database, recoveryMode)));
            }
            catch (Exception)
            {
                return $"Not Supported by {OrmLiteConfig.DialectProvider.SQLServerName()} Server";
            }
        }

        /// <summary>
        /// Run the SQL Statement.
        /// </summary>
        /// <param name="dbAccess">
        /// The Database access.
        /// </param>
        /// <param name="sql">
        /// The SQL Statement.
        /// </param>
        /// <param name="timeOut">
        /// The time Out.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string RunSQL(
            [NotNull] this IDbAccess dbAccess,
            [NotNull] string sql,
            [NotNull] int timeOut)
        {
            CodeContracts.VerifyNotNull(dbAccess);

            return dbAccess.Execute(
                    db =>
                    {
                        using (var cmd = db.Connection.CreateCommand())
                        {
                            // added so command won't timeout anymore...
                            cmd.CommandTimeout = timeOut;
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = sql;

                            return db.GetDialectProvider().InnerRunSqlExecuteReader(cmd);
                        }
                    });
        }

        /// <summary>
        /// The system initialize execute scripts.
        /// </summary>
        /// <param name="dbAccess">
        /// The Database access.
        /// </param>
        /// <param name="script">
        /// The script.
        /// </param>
        /// <param name="scriptFile">
        /// The script file.
        /// </param>
        /// <param name="timeOut">
        /// The time Out.
        /// </param>
        public static void SystemInitializeExecuteScripts(
            [NotNull] this IDbAccess dbAccess,
            [NotNull] string script,
            [NotNull] string scriptFile,
            [NotNull] int timeOut)
        {
            CodeContracts.VerifyNotNull(dbAccess);

            var statements = Regex.Split(script, "\\sGO\\s", RegexOptions.IgnoreCase).ToList();

            using (var trans = dbAccess.CreateConnectionOpen().BeginTransaction())
            {
                foreach (var sql in statements.Select(sql0 => sql0.Trim()))
                {
                    try
                    {
                        if (sql.Length <= 0)
                        {
                            continue;
                        }

                        using (var cmd = trans.Connection.CreateCommand())
                        {
                            // added so command won't timeout anymore...
                            cmd.CommandTimeout = timeOut;
                            cmd.Transaction = trans;
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = sql.Trim();
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception x)
                    {
                        trans.Rollback();
                        throw new Exception($"FILE:\n{scriptFile}\n\nERROR:\n{x.Message}\n\nSTATEMENT:\n{sql}");
                    }
                }

                trans.Commit();
            }
        }

        #endregion
    }
}