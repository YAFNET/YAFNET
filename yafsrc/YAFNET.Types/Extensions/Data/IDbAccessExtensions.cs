/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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
    /// <summary>
    /// Creates the connection.
    /// </summary>
    /// <param name="dbAccess">The DB access.</param>
    /// <returns>
    /// The <see cref="DbConnection" /> .
    /// </returns>
    public static DbConnection CreateConnection(this IDbAccess dbAccess)
    {
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
    public static DbConnection CreateConnectionOpen(this IDbAccess dbAccess)
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
    /// <param name="dbAccess">
    /// The Database access.
    /// </param>
    /// <returns>
    /// The <see cref="DbConnection"/> .
    /// </returns>
    public async static Task<IDbConnection> CreateConnectionOpenAsync(this IDbAccess dbAccess)
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
    /// <param name="dbAccess">The database access.</param>
    /// <returns>IDbConnectionFactory.</returns>
    public static IDbConnectionFactory ResolveDbFactory(this IDbAccess dbAccess)
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
    /// <param name="dbAccess">
    /// The Database access.
    /// </param>
    /// <param name="update">
    /// The update.
    /// </param>
    /// <returns>
    /// The <see cref="int"/>.
    /// </returns>
    public static int Update<T>(this IDbAccess dbAccess, T update)
        where T : IEntity
    {
        return dbAccess.Execute(db => db.Connection.Update(update));
    }

    /// <summary>
    /// Update/Insert entity.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbAccess">The database access.</param>
    /// <param name="entity">The entity.</param>
    /// <param name="where">The where.</param>
    public static void Upsert<T>(this IDbAccess dbAccess, T entity,
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
    /// <param name="dbAccess">
    /// The Database access.
    /// </param>
    /// <param name="update">
    /// The update.
    /// </param>
    /// <param name="token">
    /// The cancellation token
    /// </param>
    /// <returns>
    /// The <see cref="int"/>.
    /// </returns>
    public static Task<int> UpdateAsync<T>(this IDbAccess dbAccess, T update, CancellationToken token = default)
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
    /// <param name="dbAccess">The database access.</param>
    /// <param name="updateFields">The update fields.</param>
    /// <param name="where">The where.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <returns></returns>
    public static int UpdateAdd<T>(
        this IDbAccess dbAccess,
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
    /// <returns></returns>
    public static int UpdateOnly<T>(
        this IDbAccess dbAccess,
        Expression<Func<T>> updateFields,
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
    /// <param name="dbAccess">The database access.</param>
    /// <param name="updateFields">The update fields.</param>
    /// <param name="where">The where.</param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static Task<int> UpdateOnlyAsync<T>(
        this IDbAccess dbAccess,
        Expression<Func<T>> updateFields,
        Expression<Func<T, bool>> where = null,
        CancellationToken token = default)
        where T : class, IEntity, new()
    {
        return dbAccess.ExecuteAsync(
            db => db.UpdateOnlyFieldsAsync(
                updateFields,
                where, //OrmLiteConfig.DialectProvider.SqlExpression<T>().Where(where),
                token: token));
    }

    /// <summary>
    /// Checks whether a Table Exists. E.g:
    /// <para>db.TableExists&lt;Person&gt;()</para>
    /// </summary>
    public static bool TableExists<T>(this IDbAccess dbAccess)
        where T : class, IEntity, new()
    {
        return dbAccess.Execute(
            db => db.Connection.TableExists<T>());
    }

    /// <summary>
    /// Returns true if the Query returns any records that match the supplied SqlExpression, E.g:
    /// <para>db.Exists(db.From&lt;Person&gt;().Where(x =&gt; x.Age &lt; 50))</para>
    /// </summary>
    public static bool Exists<T>(this IDbAccess dbAccess, Expression<Func<T, bool>> where = null)
        where T : class, IEntity, new()
    {
        return dbAccess.Execute(
            db => db.Connection.Exists(OrmLiteConfig.DialectProvider.SqlExpression<T>().Where(where)));
    }

    /// <summary>
    /// Returns true if the Query returns any records that match the supplied SqlExpression, E.g:
    /// <para>db.Exists(db.From&lt;Person&gt;().Where(x =&gt; x.Age &lt; 50))</para>
    /// </summary>
    public static Task<bool> ExistsAsync<T>(this IDbAccess dbAccess, Expression<Func<T, bool>> where = null)
        where T : class, IEntity, new()
    {
        return dbAccess.ExecuteAsync(
            db => db.ExistsAsync(OrmLiteConfig.DialectProvider.SqlExpression<T>().Where(where)));
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
    public static string GetDatabaseFragmentationInfo(this IDbAccess dbAccess)
    {
        var result = new StringBuilder();

        try
        {
            var infos = dbAccess.Execute(
                db => db.Connection.SqlList<dynamic>(
                    OrmLiteConfig.DialectProvider.DatabaseFragmentationInfo(db.Connection.Database)));

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
    /// <param name="dbAccess">The database access.</param>
    /// <returns>Returns the size of the database</returns>
    public static int GetDatabaseSize(this IDbAccess dbAccess)
    {
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
    public static string GetSQLVersion(this IDbAccess dbAccess)
    {
        var version = dbAccess.Execute(
            db => db.Connection.Scalar<string>(OrmLiteConfig.DialectProvider.SQLVersion()));

        var serverName = OrmLiteConfig.DialectProvider.SQLServerName();

        return version.StartsWith(serverName) ? version : $"{serverName} {version}";
    }

    /// <summary>
    /// The shrink database.
    /// </summary>
    /// <param name="dbAccess">The database access.</param>
    public static string ShrinkDatabase(this IDbAccess dbAccess)
    {
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
    public static string ReIndexDatabase(this IDbAccess dbAccess, string objectQualifier)
    {
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
    public static string ChangeRecoveryMode(this IDbAccess dbAccess, string recoveryMode)
    {
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
    public static string RunSql(
        this IDbAccess dbAccess,
        string sql,
        int timeOut)
    {
        return dbAccess.Execute(
            db =>
                {
                    using var cmd = db.Connection.CreateCommand();

                    // added so command won't timeout anymore...
                    cmd.CommandTimeout = timeOut;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;

                    return db.GetDialectProvider().InnerRunSqlExecuteReader(cmd);
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
        this IDbAccess dbAccess,
        string script,
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

                        // added so command won't timeout anymore...
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