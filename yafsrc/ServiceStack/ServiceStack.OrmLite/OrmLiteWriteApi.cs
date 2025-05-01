// ***********************************************************************
// <copyright file="OrmLiteWriteApi.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using ServiceStack.Data;
using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

/// <summary>
/// Class OrmLiteWriteApi.
/// </summary>
public static class OrmLiteWriteApi
{
    /// <summary>
    /// Get the last SQL statement that was executed.
    /// </summary>
    /// <param name="dbConn">The database connection.</param>
    /// <returns>System.String.</returns>
    public static string GetLastSql(this IDbConnection dbConn)
    {
        return dbConn is OrmLiteConnection ormLiteConn ? ormLiteConn.LastCommandText : null;
    }

    /// <summary>
    /// Get the last SQL statement that was executed (include parameters).
    /// </summary>
    public static string GetMergedParamsLastSql(this IDbConnection dbConn)
    {
        if (dbConn is OrmLiteConnection ormLiteConn)
        {
            var dbCmd = ormLiteConn.LastCommand;
            var commandText = dbCmd.CommandText;
            var dialectProvider = ormLiteConn.GetDialectProvider();
            foreach (IDataParameter parameter in dbCmd.Parameters)
            {
                var type = GetTypeFromDbType(parameter.DbType);
                commandText = commandText.Replace(parameter.ParameterName, dialectProvider.GetQuotedValue(parameter.Value, type));
            }

            return commandText;
        }
        else
        {
            return null;
        }

        static Type GetTypeFromDbType(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.Binary:
                    return typeof(byte[]);
                case DbType.Byte:
                    return typeof(byte);
                case DbType.Boolean:
                    return typeof(bool);
                case DbType.Currency:
                    return typeof(decimal);
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                    return typeof(DateTime);
                case DbType.Decimal:
                    return typeof(decimal);
                case DbType.Double:
                    return typeof(double);
                case DbType.Guid:
                    return typeof(Guid);
                case DbType.Int16:
                    return typeof(short);
                case DbType.Int32:
                    return typeof(int);
                case DbType.Int64:
                    return typeof(long);
                case DbType.SByte:
                    return typeof(sbyte);
                case DbType.Single:
                    return typeof(float);
                case DbType.String:
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.StringFixedLength:
                case DbType.Xml:
                    return typeof(string);
                case DbType.Time:
                    return typeof(TimeSpan);
                case DbType.UInt16:
                    return typeof(ushort);
                case DbType.UInt32:
                    return typeof(uint);
                case DbType.UInt64:
                    return typeof(ulong);
                case DbType.VarNumeric:
                    return typeof(decimal);
                case DbType.Object:
                default:
                    return typeof(object);
            }
        }
    }

    /// <summary>
    /// Gets the last SQL and parameters.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>System.String.</returns>
    public static string GetLastSqlAndParams(this IDbCommand dbCmd)
    {
        var sb = StringBuilderCache.Allocate();
        sb.AppendLine(dbCmd.CommandText)
            .AppendLine("PARAMS: ");

        foreach (IDataParameter parameter in dbCmd.Parameters)
        {
            sb.Append(parameter.ParameterName).Append(": ")
                .Append(parameter.Value.ToJsv())
                .Append(" : ").AppendLine((parameter.Value ?? DBNull.Value).GetType().Name);
        }
        sb.AppendLine();

        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// Execute any arbitrary raw SQL.
    /// </summary>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="sql">The SQL.</param>
    /// <returns>number of rows affected</returns>
    public static int ExecuteSql(this IDbConnection dbConn, string sql)
    {
        return dbConn.Exec(dbCmd => dbCmd.ExecuteSql(sql));
    }

    /// <summary>
    /// Execute any arbitrary raw SQL with db params.
    /// </summary>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="dbParams">The database parameters.</param>
    /// <returns>number of rows affected</returns>
    public static int ExecuteSql(this IDbConnection dbConn, string sql, object dbParams)
    {
        return dbConn.Exec(dbCmd => dbCmd.ExecuteSql(sql, dbParams));
    }

    /// <summary>
    /// Execute any arbitrary raw SQL with db params.
    /// </summary>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="dbParams">The database parameters.</param>
    /// <returns>number of rows affected</returns>
    public static int ExecuteSql(this IDbConnection dbConn, string sql, Dictionary<string, object> dbParams)
    {
        return dbConn.Exec(dbCmd => dbCmd.ExecuteSql(sql, dbParams));
    }

    /// <summary>
    /// Insert 1 POCO, use selectIdentity to retrieve the last insert AutoIncrement id (if any). E.g:
    /// <para>var id = db.Insert(new Person { Id = 1, FirstName = "Jimi }, selectIdentity:true)</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="obj">The object.</param>
    /// <param name="selectIdentity">if set to <c>true</c> [select identity].</param>
    /// <param name="enableIdentityInsert">Enable Identity Insert</param>
    /// <returns>System.Int64.</returns>
    public static long Insert<T>(this IDbConnection dbConn, T obj, bool selectIdentity = false, bool enableIdentityInsert = false)
    {
        return dbConn.Exec(dbCmd => dbCmd.Insert(obj, commandFilter: null, selectIdentity: selectIdentity, enableIdentityInsert));
    }

    /// <summary>
    /// Insert 1 POCO and modify populated IDbCommand with a commandFilter. E.g:
    /// <para>var id = db.Insert(new Person { Id = 1, FirstName = "Jimi }, dbCmd =&gt; applyFilter(dbCmd))</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="obj">The object.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <param name="selectIdentity">if set to <c>true</c> [select identity].</param>
    /// <returns>System.Int64.</returns>
    public static long Insert<T>(this IDbConnection dbConn, T obj, Action<IDbCommand> commandFilter, bool selectIdentity = false)
    {
        return dbConn.Exec(dbCmd => dbCmd.Insert(obj, commandFilter: commandFilter, selectIdentity: selectIdentity));
    }

    /// <summary>
    /// Insert 1 POCO, use selectIdentity to retrieve the last insert AutoIncrement id (if any). E.g:
    /// <para>var id = db.Insert(new Dictionary&lt;string,object&gt; { ["Id"] = 1, ["FirstName"] = "Jimi }, selectIdentity:true)</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="obj">The object.</param>
    /// <param name="selectIdentity">if set to <c>true</c> [select identity].</param>
    /// <returns>System.Int64.</returns>
    public static long Insert<T>(this IDbConnection dbConn, Dictionary<string, object> obj, bool selectIdentity = false)
    {
        return dbConn.Exec(dbCmd => dbCmd.Insert<T>(obj, commandFilter: null, selectIdentity: selectIdentity));
    }

    /// <summary>
    /// Insert 1 POCO, use selectIdentity to retrieve the last insert AutoIncrement id (if any). E.g:
    /// <para>var id = db.Insert(new Dictionary&lt;string,object&gt; { ["Id"] = 1, ["FirstName"] = "Jimi }, dbCmd =&gt; applyFilter(dbCmd))</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <param name="obj">The object.</param>
    /// <param name="selectIdentity">if set to <c>true</c> [select identity].</param>
    /// <returns>System.Int64.</returns>
    public static long Insert<T>(
        this IDbConnection dbConn,
        Action<IDbCommand> commandFilter,
        Dictionary<string, object> obj,
        bool selectIdentity = false)
    {
        return dbConn.Exec(dbCmd => dbCmd.Insert<T>(obj, commandFilter: commandFilter, selectIdentity: selectIdentity));
    }

    /// <summary>
    /// Insert 1 or more POCOs in a transaction using Table default values when defined. E.g:
    /// <para>db.InsertUsingDefaults(new Person { FirstName = "Tupac", LastName = "Shakur" },</para><para>                       new Person { FirstName = "Biggie", LastName = "Smalls" })</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="objs">The objs.</param>
    public static void InsertUsingDefaults<T>(this IDbConnection dbConn, params T[] objs)
    {
        dbConn.Exec(dbCmd => dbCmd.InsertUsingDefaults(objs));
    }

    /// <summary>
    /// Insert results from SELECT SqlExpression, use selectIdentity to retrieve the last insert AutoIncrement id (if any). E.g:
    /// <para>db.InsertIntoSelect&lt;Contact&gt;(db.From&lt;Person&gt;().Select(x =&gt; new { x.Id, Surname == x.LastName }))</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="query">The query.</param>
    /// <returns>System.Int64.</returns>
    public static long InsertIntoSelect<T>(this IDbConnection dbConn, ISqlExpression query)
    {
        return dbConn.Exec(dbCmd => dbCmd.InsertIntoSelect<T>(query, commandFilter: null));
    }

    /// <summary>
    /// Insert results from SELECT SqlExpression, use selectIdentity to retrieve the last insert AutoIncrement id (if any). E.g:
    /// <para>db.InsertIntoSelect&lt;Contact&gt;(db.From&lt;Person&gt;().Select(x =&gt; new { x.Id, Surname == x.LastName }))</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="query">The query.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <returns>System.Int64.</returns>
    public static long InsertIntoSelect<T>(this IDbConnection dbConn, ISqlExpression query, Action<IDbCommand> commandFilter)
    {
        return dbConn.Exec(dbCmd => dbCmd.InsertIntoSelect<T>(query, commandFilter: commandFilter));
    }

    /// <summary>
    /// Insert a collection of POCOs in a transaction. E.g:
    /// <para>db.InsertAll(new[] { new Person { Id = 9, FirstName = "Biggie", LastName = "Smalls", Age = 24 } })</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="objs">The objs.</param>
    public static void InsertAll<T>(this IDbConnection dbConn, IEnumerable<T> objs)
    {
        dbConn.Exec(dbCmd => dbCmd.InsertAll(objs, commandFilter: null));
    }

    /// <summary>
    /// Insert a collection of POCOs in a transaction and modify populated IDbCommand with a commandFilter. E.g:
    /// <para>db.InsertAll(new[] { new Person { Id = 9, FirstName = "Biggie", LastName = "Smalls", Age = 24 } },</para><para>             dbCmd =&gt; applyFilter(dbCmd))</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="objs">The objs.</param>
    /// <param name="commandFilter">The command filter.</param>
    public static void InsertAll<T>(this IDbConnection dbConn, IEnumerable<T> objs, Action<IDbCommand> commandFilter)
    {
        dbConn.Exec(dbCmd => dbCmd.InsertAll(objs, commandFilter: commandFilter));
    }

    /// <summary>
    /// Insert 1 or more POCOs in a transaction. E.g:
    /// <para>db.Insert(new Person { Id = 1, FirstName = "Tupac", LastName = "Shakur", Age = 25 },</para><para>          new Person { Id = 2, FirstName = "Biggie", LastName = "Smalls", Age = 24 })</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="objs">The objs.</param>
    public static void Insert<T>(this IDbConnection dbConn, params T[] objs)
    {
        dbConn.Exec(dbCmd => dbCmd.Insert(commandFilter: null, objs: objs));
    }

    /// <summary>
    /// Insert 1 or more POCOs in a transaction and modify populated IDbCommand with a commandFilter. E.g:
    /// <para>db.Insert(dbCmd =&gt; applyFilter(dbCmd),</para><para>          new Person { Id = 1, FirstName = "Tupac", LastName = "Shakur", Age = 25 },</para><para>          new Person { Id = 2, FirstName = "Biggie", LastName = "Smalls", Age = 24 })</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <param name="objs">The objs.</param>
    public static void Insert<T>(this IDbConnection dbConn, Action<IDbCommand> commandFilter, params T[] objs)
    {
        dbConn.Exec(dbCmd => dbCmd.Insert(commandFilter: commandFilter, objs: objs));
    }

    /// <summary>
    /// Uses the most optimal approach to bulk insert multiple rows for each RDBMS provider
    /// </summary>
    public static void BulkInsert<T>(this IDbConnection dbConn, IEnumerable<T> objs, BulkInsertConfig config = null)
    {
        dbConn.Dialect().BulkInsert(dbConn, objs, config);
    }

    /// <summary>
    /// Updates 1 POCO. All fields are updated except for the PrimaryKey which is used as the identity selector. E.g:
    /// <para>db.Update(new Person { Id = 1, FirstName = "Jimi", LastName = "Hendrix", Age = 27 })</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="obj">The object.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <returns>System.Int32.</returns>
    public static int Update<T>(this IDbConnection dbConn, T obj, Action<IDbCommand> commandFilter = null)
    {
        return dbConn.Exec(dbCmd => dbCmd.Update(obj, commandFilter));
    }

    /// <summary>
    /// Updates 1 POCO. All fields are updated except for the PrimaryKey which is used as the identity selector. E.g:
    /// <para>db.Update(new Dictionary&lt;string,object&gt; { ["Id"] = 1, ["FirstName"] = "Jimi" })</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="obj">The object.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <returns>System.Int32.</returns>
    public static int Update<T>(this IDbConnection dbConn, Dictionary<string, object> obj, Action<IDbCommand> commandFilter = null)
    {
        return dbConn.Exec(dbCmd => dbCmd.Update<T>(obj, commandFilter));
    }

    /// <summary>
    /// Updates 1 or more POCOs in a transaction. E.g:
    /// <para>db.Update(new Person { Id = 1, FirstName = "Tupac", LastName = "Shakur", Age = 25 },</para><para>new Person { Id = 2, FirstName = "Biggie", LastName = "Smalls", Age = 24 })</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="objs">The objs.</param>
    /// <returns>System.Int32.</returns>
    public static int Update<T>(this IDbConnection dbConn, params T[] objs)
    {
        return dbConn.Exec(dbCmd => dbCmd.Update(objs, commandFilter: null));
    }
    /// <summary>
    /// Updates the specified command filter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <param name="objs">The objs.</param>
    /// <returns>System.Int32.</returns>
    public static int Update<T>(this IDbConnection dbConn, Action<IDbCommand> commandFilter, params T[] objs)
    {
        return dbConn.Exec(dbCmd => dbCmd.Update(objs, commandFilter));
    }

    /// <summary>
    /// Updates 1 or more POCOs in a transaction. E.g:
    /// <para>db.UpdateAll(new[] { new Person { Id = 1, FirstName = "Jimi", LastName = "Hendrix", Age = 27 } })</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="objs">The objs.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <returns>System.Int32.</returns>
    public static int UpdateAll<T>(this IDbConnection dbConn, IEnumerable<T> objs, Action<IDbCommand> commandFilter = null)
    {
        return dbConn.Exec(dbCmd => dbCmd.UpdateAll(objs, commandFilter));
    }

    /// <summary>
    /// Delete rows using an anonymous type filter. E.g:
    /// <para>db.Delete&lt;Person&gt;(new { FirstName = "Jimi", Age = 27 })</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="anonFilter">The anon filter.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <returns>number of rows deleted</returns>
    public static int Delete<T>(this IDbConnection dbConn, object anonFilter, Action<IDbCommand> commandFilter = null)
    {
        return dbConn.Exec(dbCmd => dbCmd.Delete<T>(anonFilter, commandFilter));
    }

    /// <summary>
    /// Delete rows using an Object Dictionary filters. E.g:
    /// <para>db.Delete&lt;Person&gt;(new Dictionary&lt;string,object&gt; { ["FirstName"] = "Jimi", ["Age"] = 27 })</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="filters">The filters.</param>
    /// <returns>number of rows deleted</returns>
    public static int Delete<T>(this IDbConnection dbConn, Dictionary<string, object> filters)
    {
        return dbConn.Exec(dbCmd => dbCmd.Delete<T>(filters));
    }

    /// <summary>
    /// Delete 1 row using all fields in the filter. E.g:
    /// <para>db.Delete(new Person { Id = 1, FirstName = "Jimi", LastName = "Hendrix", Age = 27 })</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="allFieldsFilter">All fields filter.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <returns>number of rows deleted</returns>
    public static int Delete<T>(this IDbConnection dbConn, T allFieldsFilter, Action<IDbCommand> commandFilter = null)
    {
        return dbConn.Exec(dbCmd => dbCmd.Delete(allFieldsFilter, commandFilter));
    }

    /// <summary>
    /// Delete 1 or more rows in a transaction using all fields in the filter. E.g:
    /// <para>db.Delete(new Person { Id = 1, FirstName = "Jimi", LastName = "Hendrix", Age = 27 })</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="allFieldsFilters">All fields filters.</param>
    /// <returns>System.Int32.</returns>
    public static int Delete<T>(this IDbConnection dbConn, params T[] allFieldsFilters)
    {
        return dbConn.Exec(dbCmd => dbCmd.Delete(allFieldsFilters));
    }

    /// <summary>
    /// Delete 1 or more rows using only field with non-default values in the filter. E.g:
    /// <para>db.DeleteNonDefaults(new Person { FirstName = "Jimi", Age = 27 })</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="nonDefaultsFilter">The non defaults filter.</param>
    /// <returns>number of rows deleted</returns>
    public static int DeleteNonDefaults<T>(this IDbConnection dbConn, T nonDefaultsFilter)
    {
        return dbConn.Exec(dbCmd => dbCmd.DeleteNonDefaults(nonDefaultsFilter));
    }

    /// <summary>
    /// Delete 1 or more rows in a transaction using only field with non-default values in the filter. E.g:
    /// <para>db.DeleteNonDefaults(new Person { FirstName = "Jimi", Age = 27 },
    /// new Person { FirstName = "Janis", Age = 27 })</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="nonDefaultsFilters">The non defaults filters.</param>
    /// <returns>number of rows deleted</returns>
    public static int DeleteNonDefaults<T>(this IDbConnection dbConn, params T[] nonDefaultsFilters)
    {
        return dbConn.Exec(dbCmd => dbCmd.DeleteNonDefaults(nonDefaultsFilters));
    }

    /// <summary>
    /// Delete 1 row by the PrimaryKey. E.g:
    /// <para>db.DeleteById&lt;Person&gt;(1)</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <returns>number of rows deleted</returns>
    public static int DeleteById<T>(this IDbConnection dbConn, object id, Action<IDbCommand> commandFilter = null)
    {
        return dbConn.Exec(dbCmd => dbCmd.DeleteById<T>(id, commandFilter));
    }

    /// <summary>
    /// Delete 1 row by the PrimaryKey where the rowVersion matches the optimistic concurrency field.
    /// Will throw <exception cref="OptimisticConcurrencyException">RowModifiedException</exception> if the
    /// row does not exist or has a different row version.
    /// E.g: <para>db.DeleteById&lt;Person&gt;(1)</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="rowVersion">The row version.</param>
    /// <param name="commandFilter">The command filter.</param>
    public static void DeleteById<T>(this IDbConnection dbConn, object id, ulong rowVersion, Action<IDbCommand> commandFilter = null)
    {
        dbConn.Exec(dbCmd => dbCmd.DeleteById<T>(id, rowVersion, commandFilter));
    }

    /// <summary>
    /// Delete all rows identified by the PrimaryKeys. E.g:
    /// <para>db.DeleteById&lt;Person&gt;(new[] { 1, 2, 3 })</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="idValues">The identifier values.</param>
    /// <returns>number of rows deleted</returns>
    public static int DeleteByIds<T>(this IDbConnection dbConn, IEnumerable idValues)
    {
        return dbConn.Exec(dbCmd => dbCmd.DeleteByIds<T>(idValues));
    }

    /// <summary>
    /// Delete all rows in the generic table type. E.g:
    /// <para>db.DeleteAll&lt;Person&gt;()</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <returns>number of rows deleted</returns>
    public static int DeleteAll<T>(this IDbConnection dbConn)
    {
        return dbConn.Exec(dbCmd => dbCmd.DeleteAll<T>());
    }

    /// <summary>
    /// Delete all rows provided. E.g:
    /// <para>db.DeleteAll&lt;Person&gt;(people)</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="rows">The rows.</param>
    /// <returns>number of rows deleted</returns>
    public static int DeleteAll<T>(this IDbConnection dbConn, IEnumerable<T> rows)
    {
        return dbConn.Exec(dbCmd => dbCmd.DeleteAll(rows));
    }

    /// <summary>
    /// Delete all rows in the runtime table type. E.g:
    /// <para>db.DeleteAll(typeof(Person))</para>
    /// </summary>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="tableType">Type of the table.</param>
    /// <returns>number of rows deleted</returns>
    public static int DeleteAll(this IDbConnection dbConn, Type tableType)
    {
        return dbConn.Exec(dbCmd => dbCmd.DeleteAll(tableType));
    }

    /// <summary>
    /// Delete rows using a SqlFormat filter. E.g:
    /// <para>db.Delete&lt;Person&gt;("Age &gt; @age", new { age = 42 })</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="anonType">Type of the anon.</param>
    /// <returns>number of rows deleted</returns>
    public static int Delete<T>(this IDbConnection dbConn, string sqlFilter, object anonType)
    {
        return dbConn.Exec(dbCmd => dbCmd.Delete<T>(sqlFilter, anonType));
    }

    /// <summary>
    /// Delete rows using a SqlFormat filter. E.g:
    /// <para>db.Delete(typeof(Person), "Age &gt; @age", new { age = 42 })</para>
    /// </summary>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="tableType">Type of the table.</param>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="anonType">Type of the anon.</param>
    /// <returns>number of rows deleted</returns>
    public static int Delete(this IDbConnection dbConn, Type tableType, string sqlFilter, object anonType)
    {
        return dbConn.Exec(dbCmd => dbCmd.Delete(tableType, sqlFilter, anonType));
    }

    /// <summary>
    /// Insert a new row or update existing row. Returns true if a new row was inserted.
    /// Optional references param decides whether to save all related references as well. E.g:
    /// <para>db.Save(customer, references:true)</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="obj">The object.</param>
    /// <param name="references">if set to <c>true</c> [references].</param>
    /// <returns>true if a row was inserted; false if it was updated</returns>
    public static bool Save<T>(this IDbConnection dbConn, T obj, bool references = false)
    {
        if (!references)
        {
            return dbConn.Exec(dbCmd => dbCmd.Save(obj));
        }

        var trans = dbConn.OpenTransactionIfNotExists();
        return dbConn.Exec(dbCmd =>
            {
                using (trans)
                {
                    var ret = dbCmd.Save(obj);
                    dbCmd.SaveAllReferences(obj);
                    trans?.Commit();
                    return ret;
                }
            });
    }

    /// <summary>
    /// Insert new rows or update existing rows. Return number of rows added E.g:
    /// <para>db.Save(new Person { Id = 10, FirstName = "Amy", LastName = "Winehouse", Age = 27 })</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="objs">The objs.</param>
    /// <returns>number of rows added</returns>
    public static int Save<T>(this IDbConnection dbConn, params T[] objs)
    {
        return dbConn.Exec(dbCmd => dbCmd.Save(objs));
    }

    /// <summary>
    /// Insert new rows or update existing rows. Return number of rows added E.g:
    /// <para>db.SaveAll(new [] { new Person { Id = 10, FirstName = "Amy", LastName = "Winehouse", Age = 27 } })</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="objs">The objs.</param>
    /// <returns>number of rows added</returns>
    public static int SaveAll<T>(this IDbConnection dbConn, IEnumerable<T> objs)
    {
        return dbConn.Exec(dbCmd => dbCmd.SaveAll(objs));
    }

    /// <summary>
    /// Populates all related references on the instance with its primary key and saves them. Uses '(T)Id' naming convention. E.g:
    /// <para>db.SaveAllReferences(customer)</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="instance">The instance.</param>
    public static void SaveAllReferences<T>(this IDbConnection dbConn, T instance)
    {
        dbConn.Exec(dbCmd => dbCmd.SaveAllReferences(instance));
    }

    /// <summary>
    /// Populates the related references with the instance primary key and saves them. Uses '(T)Id' naming convention. E.g:
    /// <para>db.SaveReference(customer, customer.Orders)</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TRef">The type of the t reference.</typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="instance">The instance.</param>
    /// <param name="refs">The refs.</param>
    public static void SaveReferences<T, TRef>(this IDbConnection dbConn, T instance, params TRef[] refs)
    {
        dbConn.Exec(dbCmd => dbCmd.SaveReferences(instance, refs));
    }

    /// <summary>
    /// Populates the related references with the instance primary key and saves them. Uses '(T)Id' naming convention. E.g:
    /// <para>db.SaveReference(customer, customer.Orders)</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TRef">The type of the t reference.</typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="instance">The instance.</param>
    /// <param name="refs">The refs.</param>
    public static void SaveReferences<T, TRef>(this IDbConnection dbConn, T instance, List<TRef> refs)
    {
        dbConn.Exec(dbCmd => dbCmd.SaveReferences(instance, refs.ToArray()));
    }

    /// <summary>
    /// Populates the related references with the instance primary key and saves them. Uses '(T)Id' naming convention. E.g:
    /// <para>db.SaveReferences(customer, customer.Orders)</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TRef">The type of the t reference.</typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="instance">The instance.</param>
    /// <param name="refs">The refs.</param>
    public static void SaveReferences<T, TRef>(this IDbConnection dbConn, T instance, IEnumerable<TRef> refs)
    {
        dbConn.Exec(dbCmd => dbCmd.SaveReferences(instance, refs.ToArray()));
    }

    /// <summary>
    /// Gets the row version.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="id">The identifier.</param>
    /// <returns>System.Object.</returns>
    public static object GetRowVersion<T>(this IDbConnection dbConn, object id)
    {
        return dbConn.Exec(dbCmd => dbCmd.GetRowVersion(typeof(T).GetModelDefinition(), id));
    }

    /// <summary>
    /// Gets the row version.
    /// </summary>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="modelType">Type of the model.</param>
    /// <param name="id">The identifier.</param>
    /// <returns>System.Object.</returns>
    public static object GetRowVersion(this IDbConnection dbConn, Type modelType, object id)
    {
        return dbConn.Exec(dbCmd => dbCmd.GetRowVersion(modelType.GetModelDefinition(), id));
    }

    // Procedures
    /// <summary>
    /// Executes the procedure.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="obj">The object.</param>
    public static void ExecuteProcedure<T>(this IDbConnection dbConn, T obj)
    {
        dbConn.Exec(dbCmd => dbCmd.ExecuteProcedure(obj));
    }

    /// <summary>
    /// Generates inline UPDATE SQL Statement
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="item">The item.</param>
    /// <param name="updateFields">The update fields.</param>
    /// <returns>System.String.</returns>
    public static string ToUpdateStatement<T>(this IDbConnection dbConn, T item, ICollection<string> updateFields = null)
    {
        return dbConn.Exec(dbCmd => dbCmd.GetDialectProvider().ToUpdateStatement(dbCmd, item, updateFields));
    }

    /// <summary>
    /// Generates inline INSERT SQL Statement
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="item">The item.</param>
    /// <param name="insertFields">The insert fields.</param>
    /// <returns>System.String.</returns>
    public static string ToInsertStatement<T>(this IDbConnection dbConn, T item, ICollection<string> insertFields = null)
    {
        return dbConn.Exec(dbCmd => dbCmd.GetDialectProvider().ToInsertStatement(dbCmd, item, insertFields));
    }
}