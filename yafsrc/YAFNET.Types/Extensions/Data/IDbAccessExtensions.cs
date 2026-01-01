/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

using ServiceStack.Data;

namespace YAF.Types.Extensions.Data;

using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using YAF.Types.Interfaces.Data;

/// <summary>
///     The DBAccess extensions.
/// </summary>
public static class IDbAccessExtensions
{
    /// <param name="dbAccess">The DB access.</param>
    extension(IDbAccess dbAccess)
    {
        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <returns>
        /// The <see cref="DbConnection" /> .
        /// </returns>
        public DbConnection CreateConnection()
        {
            var connection = dbAccess.DbProviderFactory.CreateConnection();
            connection.ConnectionString = dbAccess.Information.ConnectionString();

            return connection;
        }

        /// <summary>
        /// Get an open DB connection.
        /// </summary>
        /// <returns>
        /// The <see cref="DbConnection"/> .
        /// </returns>
        public DbConnection CreateConnectionOpen()
        {
            var connection = dbAccess.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                // open it up...
                connection.Open();
            }

            return connection;
        }

        /// <summary>
        /// Get an open DB connection.
        /// </summary>
        /// <returns>
        /// The <see cref="DbConnection"/> .
        /// </returns>
        public async Task<IDbConnection> CreateConnectionOpenAsync()
        {
            var factory = new OrmLiteConnectionFactory(
                dbAccess.Information.ConnectionString(),
                OrmLiteConfig.DialectProvider);

            var connection = await factory.OpenDbConnectionAsync();

            return connection;
        }

        /// <summary>
        /// Resolves the database factory.
        /// </summary>
        /// <returns>IDbConnectionFactory.</returns>
        public IDbConnectionFactory ResolveDbFactory()
        {
            return  new OrmLiteConnectionFactory(
                dbAccess.Information.ConnectionString(),
                OrmLiteConfig.DialectProvider);
        }

        /// <summary>
        /// Runs the update command.
        /// </summary>
        /// <typeparam name="T">
        /// The type Parameter
        /// </typeparam>
        /// <param name="update">
        /// The update.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Update<T>(T update)
            where T : IEntity
        {
            return dbAccess.Execute(db => db.Connection.Update(update));
        }

        /// <summary>
        /// Update/Insert entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="where">The where.</param>
        public void Upsert<T>(T entity,
            Expression<Func<T, bool>> where)
            where T : IEntity
        {
            dbAccess.Execute(db => db.Connection.Upsert(entity, where));
        }

        /// <summary>
        /// Runs the update command.
        /// </summary>
        /// <typeparam name="T">
        /// The type Parameter
        /// </typeparam>
        /// <param name="update">
        /// The update.
        /// </param>
        /// <param name="token">
        /// The cancellation token
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public Task<int> UpdateAsync<T>(T update, CancellationToken token = default)
            where T : IEntity
        {
            return dbAccess.ExecuteAsync(db => db.UpdateAsync(update, token: token));
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
        /// <param name="updateFields">The update fields.</param>
        /// <param name="where">The where.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <returns></returns>
        public int UpdateAdd<T>(Expression<Func<T>> updateFields,
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
        /// <param name="updateFields">The update fields.</param>
        /// <param name="where">The where.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <returns></returns>
        public Task<int> UpdateAddAsync<T>(Expression<Func<T>> updateFields,
            Expression<Func<T, bool>> where = null,
            Action<IDbCommand> commandFilter = null)
            where T : class, IEntity, IHaveID, new()
        {
            return dbAccess.ExecuteAsync(
                db => db.UpdateAddAsync(
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
        /// <param name="updateFields">The update fields.</param>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        public int UpdateOnly<T>(Expression<Func<T>> updateFields,
            Expression<Func<T, bool>> where = null)
            where T : class, IEntity, new()
        {
            return dbAccess.Execute(
                db => db.Connection.UpdateOnlyFields(
                    updateFields,
                    where));
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
        /// <param name="updateFields">The update fields.</param>
        /// <param name="where">The where.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<int> UpdateOnlyAsync<T>(Expression<Func<T>> updateFields,
            Expression<Func<T, bool>> where = null,
            CancellationToken token = default)
            where T : class, IEntity, new()
        {
            return dbAccess.ExecuteAsync(
                db => db.UpdateOnlyFieldsAsync(
                    updateFields,
                    where,
                    token: token));
        }

        /// <summary>
        /// Checks whether a Table Exists. E.g:
        /// <para>db.TableExists&lt;Person&gt;()</para>
        /// </summary>
        public bool TableExists<T>()
            where T : class, IEntity, new()
        {
            return dbAccess.Execute(
                db => db.Connection.TableExists<T>());
        }

        /// <summary>
        /// Checks whether a Table Exists. E.g:
        /// <para>db.TableExists&lt;Person&gt;()</para>
        /// </summary>
        public Task<bool> TableExistsAsync<T>()
            where T : class, IEntity, new()
        {
            return dbAccess.ExecuteAsync(
                db => db.TableExistsAsync<T>());
        }

        /// <summary>
        /// Returns true if the Query returns any records that match the supplied SqlExpression, E.g:
        /// <para>db.Exists(db.From&lt;Person&gt;().Where(x =&gt; x.Age &lt; 50))</para>
        /// </summary>
        public bool Exists<T>(Expression<Func<T, bool>> where = null)
            where T : class, IEntity, new()
        {
            return dbAccess.Execute(
                db => db.Connection.Exists(OrmLiteConfig.DialectProvider.SqlExpression<T>().Where(where)));
        }

        /// <summary>
        /// Returns true if the Query returns any records that match the supplied SqlExpression, E.g:
        /// <para>db.Exists(db.From&lt;Person&gt;().Where(x =&gt; x.Age &lt; 50))</para>
        /// </summary>
        public Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> where = null)
            where T : class, IEntity, new()
        {
            return dbAccess.ExecuteAsync(
                db => db.ExistsAsync(OrmLiteConfig.DialectProvider.SqlExpression<T>().Where(where)));
        }

        /// <summary>
        /// The get database fragmentation info.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public async Task<string> GetDatabaseFragmentationInfoAsync()
        {
            var result = new StringBuilder();

            try
            {
                var infos = await dbAccess.ExecuteAsync(
                    db => db.SqlListAsync<dynamic>(
                        OrmLiteConfig.DialectProvider.DatabaseFragmentationInfo(db.Database)));

                if (infos is null)
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
        /// <returns>Returns the size of the database</returns>
        public Task<int> GetDatabaseSizeAsync()
        {
            return dbAccess.ExecuteAsync(
                db => db.ScalarAsync<int>(OrmLiteConfig.DialectProvider.DatabaseSize(db.Database)));
        }

        /// <summary>
        /// Gets the current SQL Engine Edition.
        /// </summary>
        /// <returns>
        /// Returns the current SQL Engine Edition.
        /// </returns>
        public async Task<string> GetSqlVersionAsync()
        {
            var version = await dbAccess.ExecuteAsync(
                db => db.ScalarAsync<string>(OrmLiteConfig.DialectProvider.SQLVersion()));

            var serverName = OrmLiteConfig.DialectProvider.SQLServerName();

            return version.StartsWith(serverName) ? version : $"{serverName} {version}";
        }

        /// <summary>
        /// The shrink database.
        /// </summary>
        public Task<string> ShrinkDatabaseAsync()
        {
            return dbAccess.ExecuteAsync(
                db => db.ScalarAsync<string>(
                    OrmLiteConfig.DialectProvider.ShrinkDatabase(db.Database)));
        }

        /// <summary>
        /// Re-Index the Database
        /// </summary>
        /// <param name="objectQualifier">
        /// The object Qualifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public Task<string> ReIndexDatabaseAsync(string objectQualifier)
        {
            return dbAccess.ExecuteAsync(
                db => db.ScalarAsync<string>(
                    OrmLiteConfig.DialectProvider.ReIndexDatabase(db.Database, objectQualifier)));
        }

        /// <summary>
        /// Change Database Recovery Mode
        /// </summary>
        /// <param name="recoveryMode">
        /// The recovery mode.
        /// </param>
        public async Task<string> ChangeRecoveryModeAsync(string recoveryMode)
        {
            try
            {
                return await dbAccess.ExecuteAsync(
                    db => db.ScalarAsync<string>(
                        OrmLiteConfig.DialectProvider.ReIndexDatabase(db.Database, recoveryMode)));
            }
            catch (Exception)
            {
                return $"Not Supported by {OrmLiteConfig.DialectProvider.SQLServerName()} Server";
            }
        }

        /// <summary>
        /// Run the SQL Statement.
        /// </summary>
        /// <param name="sql">
        /// The SQL Statement.
        /// </param>
        /// <param name="timeOut">
        /// The time-Out.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string RunSql(string sql,
            int timeOut)
        {
            return dbAccess.Execute(
                db =>
                {
                    using var cmd = db.Connection.CreateCommand();

                    // added so command won't time out anymore...
                    cmd.CommandTimeout = timeOut;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;

                    return db.GetDialectProvider().InnerRunSqlExecuteReader(cmd);
                });
        }

        /// <summary>
        /// The system initialize execute scripts.
        /// </summary>
        /// <param name="script">
        /// The script.
        /// </param>
        /// <param name="scriptFile">
        /// The script file.
        /// </param>
        /// <param name="timeOut">
        /// The time-Out.
        /// </param>
        public void SystemInitializeExecuteScripts(string script,
            string scriptFile,
            int timeOut)
        {
            var statements = Regex.Split(script, @"\sGO\s", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100))
                .ToList();

            using var trans = dbAccess.CreateConnectionOpen().BeginTransaction();

            statements.Select(sql0 => sql0.Trim()).Where(sql => sql.Length > 0).ForEach(
                sql =>
                {
                    try
                    {
                        using var cmd = trans.Connection.CreateCommand();

                        // added so command won't time out anymore...
                        cmd.CommandTimeout = timeOut;
                        cmd.Transaction = trans;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = sql.Trim();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception x)
                    {
                        trans.Rollback();
                        throw new FormatException($"FILE:\n{scriptFile}\n\nERROR:\n{x.Message}\n\nSTATEMENT:\n{sql}");
                    }
                });

            trans.Commit();
        }
    }
}