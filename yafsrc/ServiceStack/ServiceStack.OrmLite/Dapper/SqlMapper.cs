// ***********************************************************************
// <copyright file="SqlMapper.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
/*
 License: http://www.apache.org/licenses/LICENSE-2.0
 Home page: https://github.com/StackExchange/dapper-dot-net
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

namespace ServiceStack.OrmLite.Dapper
{
    /// <summary>
    /// Dapper, a light weight object mapper for ADO.NET
    /// </summary>
    public static partial class SqlMapper
    {
        /// <summary>
        /// Class PropertyInfoByNameComparer.
        /// </summary>
        private class PropertyInfoByNameComparer : IComparer<PropertyInfo>
        {
            /// <summary>
            /// Compares the specified x.
            /// </summary>
            /// <param name="x">The x.</param>
            /// <param name="y">The y.</param>
            /// <returns>int.</returns>
            public int Compare(PropertyInfo x, PropertyInfo y) => string.CompareOrdinal(x.Name, y.Name);
        }

        /// <summary>
        /// Gets the column hash.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="startBound">The start bound.</param>
        /// <param name="length">The length.</param>
        /// <returns>int.</returns>
        private static int GetColumnHash(IDataReader reader, int startBound = 0, int length = -1)
        {
            unchecked
            {
                int max = length < 0 ? reader.FieldCount : startBound + length;
                int hash = -37 * startBound + max;
                for (int i = startBound; i < max; i++)
                {
                    object tmp = reader.GetName(i);
                    hash = -79 * (hash * 31 + (tmp?.GetHashCode() ?? 0)) + (reader.GetFieldType(i)?.GetHashCode() ?? 0);
                }
                return hash;
            }
        }

        /// <summary>
        /// Called if the query cache is purged via PurgeQueryCache
        /// </summary>
        public static event EventHandler QueryCachePurged;
        /// <summary>
        /// Called when [query cache purged].
        /// </summary>
        private static void OnQueryCachePurged()
        {
            var handler = QueryCachePurged;
            handler?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// The query cache
        /// </summary>
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<Identity, CacheInfo> _queryCache = new();
        /// <summary>
        /// Sets the query cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        private static void SetQueryCache(Identity key, CacheInfo value)
        {
            if (Interlocked.Increment(ref collect) == COLLECT_PER_ITEMS)
            {
                CollectCacheGarbage();
            }
            _queryCache[key] = value;
        }

        /// <summary>
        /// Collects the cache garbage.
        /// </summary>
        private static void CollectCacheGarbage()
        {
            try
            {
                foreach (var pair in _queryCache)
                {
                    if (pair.Value.GetHitCount() <= COLLECT_HIT_COUNT_MIN)
                    {
                        _queryCache.TryRemove(pair.Key, out CacheInfo cache);
                    }
                }
            }

            finally
            {
                Interlocked.Exchange(ref collect, 0);
            }
        }

        /// <summary>
        /// The collect per items
        /// </summary>
        private const int COLLECT_PER_ITEMS = 1000, COLLECT_HIT_COUNT_MIN = 0;
        /// <summary>
        /// The collect
        /// </summary>
        private static int collect;
        /// <summary>
        /// Tries the get query cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>bool.</returns>
        private static bool TryGetQueryCache(Identity key, out CacheInfo value)
        {
            if (_queryCache.TryGetValue(key, out value))
            {
                value.RecordHit();
                return true;
            }
            value = null;
            return false;
        }

        /// <summary>
        /// Purge the query cache
        /// </summary>
        public static void PurgeQueryCache()
        {
            _queryCache.Clear();
            TypeDeserializerCache.Purge();
            OnQueryCachePurged();
        }

        /// <summary>
        /// Purges the type of the query cache by.
        /// </summary>
        /// <param name="type">The type.</param>
        private static void PurgeQueryCacheByType(Type type)
        {
            foreach (var entry in _queryCache)
            {
                if (entry.Key.type == type)
                    _queryCache.TryRemove(entry.Key, out CacheInfo cache);
            }
            TypeDeserializerCache.Purge(type);
        }

        /// <summary>
        /// Return a count of all the cached queries by Dapper
        /// </summary>
        /// <returns>int.</returns>
        public static int GetCachedSQLCount()
        {
            return _queryCache.Count;
        }

        /// <summary>
        /// Return a list of all the queries cached by Dapper
        /// </summary>
        /// <param name="ignoreHitCountAbove">The ignore hit count above.</param>
        /// <returns>System.Collections.Generic.IEnumerable&lt;System.Tuple&lt;string, string, int&gt;&gt;.</returns>
        public static IEnumerable<Tuple<string, string, int>> GetCachedSQL(int ignoreHitCountAbove = int.MaxValue)
        {
            var data = _queryCache.Select(pair => Tuple.Create(pair.Key.connectionString, pair.Key.sql, pair.Value.GetHitCount()));
            return ignoreHitCountAbove < int.MaxValue
                    ? data.Where(tuple => tuple.Item3 <= ignoreHitCountAbove)
                    : data;
        }

        /// <summary>
        /// Deep diagnostics only: find any hash collisions in the cache
        /// </summary>
        /// <returns>System.Collections.Generic.IEnumerable&lt;System.Tuple&lt;int, int&gt;&gt;.</returns>
        public static IEnumerable<Tuple<int, int>> GetHashCollissions()
        {
            var counts = new Dictionary<int, int>();
            foreach (var key in _queryCache.Keys)
            {
                if (!counts.TryGetValue(key.hashCode, out int count))
                {
                    counts.Add(key.hashCode, 1);
                }
                else
                {
                    counts[key.hashCode] = count + 1;
                }
            }
            return from pair in counts
                   where pair.Value > 1
                   select Tuple.Create(pair.Key, pair.Value);
        }

        /// <summary>
        /// The type map
        /// </summary>
        private static Dictionary<Type, DbType> typeMap;

        /// <summary>
        /// Cctors this instance.
        /// </summary>
        static SqlMapper()
        {
            typeMap = new Dictionary<Type, DbType>
            {
                [typeof(byte)] = DbType.Byte,
                [typeof(sbyte)] = DbType.SByte,
                [typeof(short)] = DbType.Int16,
                [typeof(ushort)] = DbType.UInt16,
                [typeof(int)] = DbType.Int32,
                [typeof(uint)] = DbType.UInt32,
                [typeof(long)] = DbType.Int64,
                [typeof(ulong)] = DbType.UInt64,
                [typeof(float)] = DbType.Single,
                [typeof(double)] = DbType.Double,
                [typeof(decimal)] = DbType.Decimal,
                [typeof(bool)] = DbType.Boolean,
                [typeof(string)] = DbType.String,
                [typeof(char)] = DbType.StringFixedLength,
                [typeof(Guid)] = DbType.Guid,
                [typeof(DateTime)] = DbType.DateTime,
                [typeof(DateTimeOffset)] = DbType.DateTimeOffset,
                [typeof(TimeSpan)] = DbType.Time,
                [typeof(byte[])] = DbType.Binary,
                [typeof(byte?)] = DbType.Byte,
                [typeof(sbyte?)] = DbType.SByte,
                [typeof(short?)] = DbType.Int16,
                [typeof(ushort?)] = DbType.UInt16,
                [typeof(int?)] = DbType.Int32,
                [typeof(uint?)] = DbType.UInt32,
                [typeof(long?)] = DbType.Int64,
                [typeof(ulong?)] = DbType.UInt64,
                [typeof(float?)] = DbType.Single,
                [typeof(double?)] = DbType.Double,
                [typeof(decimal?)] = DbType.Decimal,
                [typeof(bool?)] = DbType.Boolean,
                [typeof(char?)] = DbType.StringFixedLength,
                [typeof(Guid?)] = DbType.Guid,
                [typeof(DateTime?)] = DbType.DateTime,
                [typeof(DateTimeOffset?)] = DbType.DateTimeOffset,
                [typeof(TimeSpan?)] = DbType.Time,
                [typeof(object)] = DbType.Object
            };
            ResetTypeHandlers(false);
        }

        /// <summary>
        /// Clear the registered type handlers.
        /// </summary>
        public static void ResetTypeHandlers() => ResetTypeHandlers(true);

        /// <summary>
        /// Resets the type handlers.
        /// </summary>
        /// <param name="clone">The clone.</param>
        private static void ResetTypeHandlers(bool clone)
        {
            typeHandlers = new Dictionary<Type, ITypeHandler>();
            AddTypeHandlerImpl(typeof(DataTable), new DataTableHandler(), clone);
            AddTypeHandlerImpl(typeof(XmlDocument), new XmlDocumentHandler(), clone);
            AddTypeHandlerImpl(typeof(XDocument), new XDocumentHandler(), clone);
            AddTypeHandlerImpl(typeof(XElement), new XElementHandler(), clone);
        }

        /// <summary>
        /// Configure the specified type to be mapped to a given db-type.
        /// </summary>
        /// <param name="type">The type to map from.</param>
        /// <param name="dbType">The database type to map to.</param>
        public static void AddTypeMap(Type type, DbType dbType)
        {
            // use clone, mutate, replace to avoid threading issues
            var snapshot = typeMap;

            if (snapshot.TryGetValue(type, out DbType oldValue) && oldValue == dbType) return; // nothing to do

            typeMap = new Dictionary<Type, DbType>(snapshot) { [type] = dbType };
        }

        /// <summary>
        /// Removes the specified type from the Type/DbType mapping table.
        /// </summary>
        /// <param name="type">The type to remove from the current map.</param>
        public static void RemoveTypeMap(Type type)
        {
            // use clone, mutate, replace to avoid threading issues
            var snapshot = typeMap;

            if (!snapshot.ContainsKey(type)) return; // nothing to do

            var newCopy = new Dictionary<Type, DbType>(snapshot);
            newCopy.Remove(type);

            typeMap = newCopy;
        }

        /// <summary>
        /// Configure the specified type to be processed by a custom handler.
        /// </summary>
        /// <param name="type">The type to handle.</param>
        /// <param name="handler">The handler to process the <paramref name="type" />.</param>
        public static void AddTypeHandler(Type type, ITypeHandler handler) => AddTypeHandlerImpl(type, handler, true);

        /// <summary>
        /// Determines whether [has type handler] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>bool.</returns>
        internal static bool HasTypeHandler(Type type) => typeHandlers.ContainsKey(type);

        /// <summary>
        /// Configure the specified type to be processed by a custom handler.
        /// </summary>
        /// <param name="type">The type to handle.</param>
        /// <param name="handler">The handler to process the <paramref name="type" />.</param>
        /// <param name="clone">Whether to clone the current type handler map.</param>
        /// <exception cref="ArgumentNullException">nameof(type)</exception>
        public static void AddTypeHandlerImpl(Type type, ITypeHandler handler, bool clone)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            Type secondary = null;
            if (type.IsValueType)
            {
                var underlying = Nullable.GetUnderlyingType(type);
                if (underlying == null)
                {
                    secondary = typeof(Nullable<>).MakeGenericType(type); // the Nullable<T>
                    // type is already the T
                }
                else
                {
                    secondary = type; // the Nullable<T>
                    type = underlying; // the T
                }
            }

            var snapshot = typeHandlers;
            if (snapshot.TryGetValue(type, out ITypeHandler oldValue) && handler == oldValue) return; // nothing to do

            var newCopy = clone ? new Dictionary<Type, ITypeHandler>(snapshot) : snapshot;

#pragma warning disable 618
            typeof(TypeHandlerCache<>).MakeGenericType(type).GetMethod(nameof(TypeHandlerCache<int>.SetHandler), BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { handler });
            if (secondary != null)
            {
                typeof(TypeHandlerCache<>).MakeGenericType(secondary).GetMethod(nameof(TypeHandlerCache<int>.SetHandler), BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { handler });
            }
#pragma warning restore 618
            if (handler == null)
            {
                newCopy.Remove(type);
                if (secondary != null) newCopy.Remove(secondary);
            }
            else
            {
                newCopy[type] = handler;
                if (secondary != null) newCopy[secondary] = handler;
            }
            typeHandlers = newCopy;
        }

        /// <summary>
        /// Configure the specified type to be processed by a custom handler.
        /// </summary>
        /// <typeparam name="T">The type to handle.</typeparam>
        /// <param name="handler">The handler for the type <typeparamref name="T" />.</param>
        public static void AddTypeHandler<T>(TypeHandler<T> handler) => AddTypeHandlerImpl(typeof(T), handler, true);

        /// <summary>
        /// The type handlers
        /// </summary>
        private static Dictionary<Type, ITypeHandler> typeHandlers;

        /// <summary>
        /// The linq binary
        /// </summary>
        internal const string LinqBinary = "System.Data.Linq.Binary";

        /// <summary>
        /// The obsolete internal usage only
        /// </summary>
        private const string ObsoleteInternalUsageOnly = "This method is for internal use only";

        /// <summary>
        /// Get the DbType that maps to a given value.
        /// </summary>
        /// <param name="value">The object to get a corresponding database type for.</param>
        /// <returns>System.Data.DbType.</returns>
        [Obsolete(ObsoleteInternalUsageOnly, false)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static DbType GetDbType(object value)
        {
            if (value == null || value is DBNull) return DbType.Object;

            return LookupDbType(value.GetType(), "n/a", false, out ITypeHandler handler);
        }

        /// <summary>
        /// OBSOLETE: For internal usage only. Lookup the DbType and handler for a given Type and member
        /// </summary>
        /// <param name="type">The type to lookup.</param>
        /// <param name="name">The name (for error messages).</param>
        /// <param name="demand">Whether to demand a value (throw if missing).</param>
        /// <param name="handler">The handler for <paramref name="type" />.</param>
        /// <returns>System.Data.DbType.</returns>
        /// <exception cref="UdtTypeHandler">geography</exception>
        [Obsolete(ObsoleteInternalUsageOnly, false)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static DbType LookupDbType(Type type, string name, bool demand, out ITypeHandler handler)
        {
            handler = null;
            var nullUnderlyingType = Nullable.GetUnderlyingType(type);
            if (nullUnderlyingType != null) type = nullUnderlyingType;
            if (type.IsEnum && !typeMap.ContainsKey(type))
            {
                type = Enum.GetUnderlyingType(type);
            }
            if (typeMap.TryGetValue(type, out DbType dbType))
            {
                return dbType;
            }
            if (type.FullName == LinqBinary)
            {
                return DbType.Binary;
            }
            if (typeHandlers.TryGetValue(type, out handler))
            {
                return DbType.Object;
            }
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                // auto-detect things like IEnumerable<SqlDataRecord> as a family
                if (type.IsInterface && type.IsGenericType
                    && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                    && typeof(IEnumerable<IDataRecord>).IsAssignableFrom(type))
                {
                    var argTypes = type.GetGenericArguments();
                    if (typeof(IDataRecord).IsAssignableFrom(argTypes[0]))
                    {
                        try
                        {
                            handler = (ITypeHandler)Activator.CreateInstance(
                                typeof(SqlDataRecordHandler<>).MakeGenericType(argTypes));
                            AddTypeHandlerImpl(type, handler, true);
                            return DbType.Object;
                        }
                        catch
                        {
                            handler = null;
                        }
                    }
                }
                return DynamicParameters.EnumerableMultiParameter;
            }

            switch (type.FullName)
            {
                case "Microsoft.SqlServer.Types.SqlGeography":
                    AddTypeHandler(type, handler = new UdtTypeHandler("geography"));
                    return DbType.Object;
                case "Microsoft.SqlServer.Types.SqlGeometry":
                    AddTypeHandler(type, handler = new UdtTypeHandler("geometry"));
                    return DbType.Object;
                case "Microsoft.SqlServer.Types.SqlHierarchyId":
                    AddTypeHandler(type, handler = new UdtTypeHandler("hierarchyid"));
                    return DbType.Object;
            }

            if (demand)
                throw new NotSupportedException($"The member {name} of type {type.FullName} cannot be used as a parameter value");
            return DbType.Object;
        }

        /// <summary>
        /// Obtains the data as a list; if it is *already* a list, the original object is returned without
        /// any duplication; otherwise, ToList() is invoked.
        /// </summary>
        /// <typeparam name="T">The type of element in the list.</typeparam>
        /// <param name="source">The enumerable to return as a list.</param>
        /// <returns>System.Collections.Generic.List&lt;T&gt;.</returns>
        public static List<T> AsList<T>(this IEnumerable<T> source) =>
            source == null || source is List<T> ? (List<T>)source : source.ToList();

        /// <summary>
        /// Execute parameterized SQL.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>The number of rows affected.</returns>
        public static int Execute(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.Buffered);
            return ExecuteImpl(cnn, ref command);
        }

        /// <summary>
        /// Execute parameterized SQL.
        /// </summary>
        /// <param name="cnn">The connection to execute on.</param>
        /// <param name="command">The command to execute on this connection.</param>
        /// <returns>The number of rows affected.</returns>
        public static int Execute(this IDbConnection cnn, CommandDefinition command) => ExecuteImpl(cnn, ref command);

        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <param name="cnn">The connection to execute on.</param>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="param">The parameters to use for this command.</param>
        /// <param name="transaction">The transaction to use for this command.</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>The first cell selected as <see cref="object" />.</returns>
        public static object ExecuteScalar(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.Buffered);
            return ExecuteScalarImpl<object>(cnn, ref command);
        }

        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="cnn">The connection to execute on.</param>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="param">The parameters to use for this command.</param>
        /// <param name="transaction">The transaction to use for this command.</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>The first cell returned, as <typeparamref name="T" />.</returns>
        public static T ExecuteScalar<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.Buffered);
            return ExecuteScalarImpl<T>(cnn, ref command);
        }

        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <param name="cnn">The connection to execute on.</param>
        /// <param name="command">The command to execute.</param>
        /// <returns>The first cell selected as <see cref="object" />.</returns>
        public static object ExecuteScalar(this IDbConnection cnn, CommandDefinition command) =>
            ExecuteScalarImpl<object>(cnn, ref command);

        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="cnn">The connection to execute on.</param>
        /// <param name="command">The command to execute.</param>
        /// <returns>The first cell selected as <typeparamref name="T" />.</returns>
        public static T ExecuteScalar<T>(this IDbConnection cnn, CommandDefinition command) =>
            ExecuteScalarImpl<T>(cnn, ref command);

        /// <summary>
        /// Gets the multi execute.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns>System.Collections.IEnumerable.</returns>
        private static IEnumerable GetMultiExec(object param)
        {
            return param is IEnumerable
                   && !(param is string
                        || param is IEnumerable<KeyValuePair<string, object>>
                        || param is IDynamicParameters) ? (IEnumerable)param : null;
        }

        /// <summary>
        /// Executes the implementation.
        /// </summary>
        /// <param name="cnn">The CNN.</param>
        /// <param name="command">The command.</param>
        /// <returns>int.</returns>
        private static int ExecuteImpl(this IDbConnection cnn, ref CommandDefinition command)
        {
            object param = command.Parameters;
            IEnumerable multiExec = GetMultiExec(param);
            Identity identity;
            CacheInfo info = null;
            if (multiExec != null)
            {
                if ((command.Flags & CommandFlags.Pipelined) != 0)
                {
                    // this includes all the code for concurrent/overlapped query
                    return ExecuteMultiImplAsync(cnn, command, multiExec).Result;
                }
                bool isFirst = true;
                int total = 0;
                bool wasClosed = cnn.State == ConnectionState.Closed;
                try
                {
                    if (wasClosed) cnn.Open();
                    using (var cmd = command.SetupCommand(cnn, null))
                    {
                        string masterSql = null;
                        foreach (var obj in multiExec)
                        {
                            if (isFirst)
                            {
                                masterSql = cmd.CommandText;
                                isFirst = false;
                                identity = new Identity(command.CommandText, cmd.CommandType, cnn, null, obj.GetType());
                                info = GetCacheInfo(identity, obj, command.AddToCache);
                            }
                            else
                            {
                                cmd.CommandText = masterSql; // because we do magic replaces on "in" etc
                                cmd.Parameters.Clear(); // current code is Add-tastic
                            }
                            info.ParamReader(cmd, obj);
                            total += cmd.ExecuteNonQuery();
                        }
                    }
                    command.OnCompleted();
                }
                finally
                {
                    if (wasClosed) cnn.Close();
                }
                return total;
            }

            // nice and simple
            if (param != null)
            {
                identity = new Identity(command.CommandText, command.CommandType, cnn, null, param.GetType());
                info = GetCacheInfo(identity, param, command.AddToCache);
            }
            return ExecuteCommand(cnn, ref command, param == null ? null : info.ParamReader);
        }

        /// <summary>
        /// Execute parameterized SQL and return an <see cref="IDataReader" />.
        /// </summary>
        /// <param name="cnn">The connection to execute on.</param>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="param">The parameters to use for this command.</param>
        /// <param name="transaction">The transaction to use for this command.</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>An <see cref="IDataReader" /> that can be used to iterate over the results of the SQL query.</returns>
        /// <example>
        ///   <code><![CDATA[
        /// DataTable table = new DataTable("MyTable");
        /// using (var reader = ExecuteReader(cnn, sql, param))
        /// {
        /// table.Load(reader);
        /// }
        /// ]]></code>
        /// </example>
        /// <remarks>This is typically used when the results of a query are not processed by Dapper, for example, used to fill a <see cref="DataTable" />
        /// or <see cref="T:DataSet" />.</remarks>
        public static IDataReader ExecuteReader(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.Buffered);
            var reader = ExecuteReaderImpl(cnn, ref command, CommandBehavior.Default, out IDbCommand dbcmd);
            return WrappedReader.Create(dbcmd, reader);
        }

        /// <summary>
        /// Execute parameterized SQL and return an <see cref="IDataReader" />.
        /// </summary>
        /// <param name="cnn">The connection to execute on.</param>
        /// <param name="command">The command to execute.</param>
        /// <returns>An <see cref="IDataReader" /> that can be used to iterate over the results of the SQL query.</returns>
        /// <remarks>This is typically used when the results of a query are not processed by Dapper, for example, used to fill a <see cref="DataTable" />
        /// or <see cref="T:DataSet" />.</remarks>
        public static IDataReader ExecuteReader(this IDbConnection cnn, CommandDefinition command)
        {
            var reader = ExecuteReaderImpl(cnn, ref command, CommandBehavior.Default, out IDbCommand dbcmd);
            return WrappedReader.Create(dbcmd, reader);
        }

        /// <summary>
        /// Execute parameterized SQL and return an <see cref="IDataReader" />.
        /// </summary>
        /// <param name="cnn">The connection to execute on.</param>
        /// <param name="command">The command to execute.</param>
        /// <param name="commandBehavior">The <see cref="CommandBehavior" /> flags for this reader.</param>
        /// <returns>An <see cref="IDataReader" /> that can be used to iterate over the results of the SQL query.</returns>
        /// <remarks>This is typically used when the results of a query are not processed by Dapper, for example, used to fill a <see cref="DataTable" />
        /// or <see cref="T:DataSet" />.</remarks>
        public static IDataReader ExecuteReader(this IDbConnection cnn, CommandDefinition command, CommandBehavior commandBehavior)
        {
            var reader = ExecuteReaderImpl(cnn, ref command, commandBehavior, out IDbCommand dbcmd);
            return WrappedReader.Create(dbcmd, reader);
        }

        /// <summary>
        /// Return a sequence of dynamic objects with properties matching the columns.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="buffered">Whether to buffer the results in memory.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>System.Collections.Generic.IEnumerable&lt;dynamic&gt;.</returns>
        /// <remarks>Note: each row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public static IEnumerable<dynamic> Query(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null) =>
            Query<DapperRow>(cnn, sql, param as object, transaction, buffered, commandTimeout, commandType);

        /// <summary>
        /// Return a dynamic object with properties matching the columns.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>dynamic.</returns>
        /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public static dynamic QueryFirst(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            QueryFirst<DapperRow>(cnn, sql, param as object, transaction, commandTimeout, commandType);

        /// <summary>
        /// Return a dynamic object with properties matching the columns.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>dynamic.</returns>
        /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public static dynamic QueryFirstOrDefault(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            QueryFirstOrDefault<DapperRow>(cnn, sql, param as object, transaction, commandTimeout, commandType);

        /// <summary>
        /// Return a dynamic object with properties matching the columns.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>dynamic.</returns>
        /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public static dynamic QuerySingle(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            QuerySingle<DapperRow>(cnn, sql, param as object, transaction, commandTimeout, commandType);

        /// <summary>
        /// Return a dynamic object with properties matching the columns.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>dynamic.</returns>
        /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public static dynamic QuerySingleOrDefault(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            QuerySingleOrDefault<DapperRow>(cnn, sql, param as object, transaction, commandTimeout, commandType);

        /// <summary>
        /// Executes a query, returning the data typed as <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type of results to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="buffered">Whether to buffer results in memory.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).</returns>
        public static IEnumerable<T> Query<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None);
            var data = QueryImpl<T>(cnn, command, typeof(T));
            return command.Buffered ? data.ToList() : data;
        }

        /// <summary>
        /// Executes a single-row query, returning the data typed as <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type of result to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).</returns>
        public static T QueryFirst<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None);
            return QueryRowImpl<T>(cnn, Row.First, ref command, typeof(T));
        }

        /// <summary>
        /// Executes a single-row query, returning the data typed as <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type of result to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).</returns>
        public static T QueryFirstOrDefault<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None);
            return QueryRowImpl<T>(cnn, Row.FirstOrDefault, ref command, typeof(T));
        }

        /// <summary>
        /// Executes a single-row query, returning the data typed as <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type of result to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).</returns>
        public static T QuerySingle<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None);
            return QueryRowImpl<T>(cnn, Row.Single, ref command, typeof(T));
        }

        /// <summary>
        /// Executes a single-row query, returning the data typed as <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type of result to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).</returns>
        public static T QuerySingleOrDefault<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None);
            return QueryRowImpl<T>(cnn, Row.SingleOrDefault, ref command, typeof(T));
        }

        /// <summary>
        /// Executes a single-row query, returning the data typed as <paramref name="type" />.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="type">The type to return.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="buffered">Whether to buffer results in memory.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).</returns>
        /// <exception cref="ArgumentNullException">nameof(type)</exception>
        public static IEnumerable<object> Query(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None);
            var data = QueryImpl<object>(cnn, command, type);
            return command.Buffered ? data.ToList() : data;
        }

        /// <summary>
        /// Executes a single-row query, returning the data typed as <paramref name="type" />.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="type">The type to return.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).</returns>
        /// <exception cref="ArgumentNullException">nameof(type)</exception>
        public static object QueryFirst(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None);
            return QueryRowImpl<object>(cnn, Row.First, ref command, type);
        }

        /// <summary>
        /// Executes a single-row query, returning the data typed as <paramref name="type" />.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="type">The type to return.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).</returns>
        /// <exception cref="ArgumentNullException">nameof(type)</exception>
        public static object QueryFirstOrDefault(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None);
            return QueryRowImpl<object>(cnn, Row.FirstOrDefault, ref command, type);
        }

        /// <summary>
        /// Executes a single-row query, returning the data typed as <paramref name="type" />.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="type">The type to return.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).</returns>
        /// <exception cref="ArgumentNullException">nameof(type)</exception>
        public static object QuerySingle(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None);
            return QueryRowImpl<object>(cnn, Row.Single, ref command, type);
        }

        /// <summary>
        /// Executes a single-row query, returning the data typed as <paramref name="type" />.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="type">The type to return.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).</returns>
        /// <exception cref="ArgumentNullException">nameof(type)</exception>
        public static object QuerySingleOrDefault(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None);
            return QueryRowImpl<object>(cnn, Row.SingleOrDefault, ref command, type);
        }

        /// <summary>
        /// Executes a query, returning the data typed as <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type of results to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="command">The command used to query on this connection.</param>
        /// <returns>A sequence of data of <typeparamref name="T" />; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).</returns>
        public static IEnumerable<T> Query<T>(this IDbConnection cnn, CommandDefinition command)
        {
            var data = QueryImpl<T>(cnn, command, typeof(T));
            return command.Buffered ? data.ToList() : data;
        }

        /// <summary>
        /// Executes a query, returning the data typed as <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type of results to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="command">The command used to query on this connection.</param>
        /// <returns>A single instance or null of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).</returns>
        public static T QueryFirst<T>(this IDbConnection cnn, CommandDefinition command) =>
            QueryRowImpl<T>(cnn, Row.First, ref command, typeof(T));

        /// <summary>
        /// Executes a query, returning the data typed as <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type of results to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="command">The command used to query on this connection.</param>
        /// <returns>A single or null instance of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).</returns>
        public static T QueryFirstOrDefault<T>(this IDbConnection cnn, CommandDefinition command) =>
            QueryRowImpl<T>(cnn, Row.FirstOrDefault, ref command, typeof(T));

        /// <summary>
        /// Executes a query, returning the data typed as <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type of results to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="command">The command used to query on this connection.</param>
        /// <returns>A single instance of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).</returns>
        public static T QuerySingle<T>(this IDbConnection cnn, CommandDefinition command) =>
            QueryRowImpl<T>(cnn, Row.Single, ref command, typeof(T));

        /// <summary>
        /// Executes a query, returning the data typed as <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type of results to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="command">The command used to query on this connection.</param>
        /// <returns>A single instance of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).</returns>
        public static T QuerySingleOrDefault<T>(this IDbConnection cnn, CommandDefinition command) =>
            QueryRowImpl<T>(cnn, Row.SingleOrDefault, ref command, typeof(T));

        /// <summary>
        /// Execute a command that returns multiple result sets, and access each in turn.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>ServiceStack.OrmLite.Dapper.SqlMapper.GridReader.</returns>
        public static GridReader QueryMultiple(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.Buffered);
            return QueryMultipleImpl(cnn, ref command);
        }

        /// <summary>
        /// Execute a command that returns multiple result sets, and access each in turn.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="command">The command to execute for this query.</param>
        /// <returns>ServiceStack.OrmLite.Dapper.SqlMapper.GridReader.</returns>
        public static GridReader QueryMultiple(this IDbConnection cnn, CommandDefinition command) =>
            QueryMultipleImpl(cnn, ref command);

        /// <summary>
        /// Queries the multiple implementation.
        /// </summary>
        /// <param name="cnn">The CNN.</param>
        /// <param name="command">The command.</param>
        /// <returns>ServiceStack.OrmLite.Dapper.SqlMapper.GridReader.</returns>
        /// <exception cref="Identity">command.CommandText, command.CommandType, cnn, typeof(GridReader), param?.GetType()</exception>
        private static GridReader QueryMultipleImpl(this IDbConnection cnn, ref CommandDefinition command)
        {
            object param = command.Parameters;
            var identity = new Identity(command.CommandText, command.CommandType, cnn, typeof(GridReader), param?.GetType());
            CacheInfo info = GetCacheInfo(identity, param, command.AddToCache);

            IDbCommand cmd = null;
            IDataReader reader = null;
            bool wasClosed = cnn.State == ConnectionState.Closed;
            try
            {
                if (wasClosed) cnn.Open();
                cmd = command.SetupCommand(cnn, info.ParamReader);
                reader = ExecuteReaderWithFlagsFallback(cmd, wasClosed, CommandBehavior.SequentialAccess);

                var result = new GridReader(cmd, reader, identity, command.Parameters as DynamicParameters, command.AddToCache);
                cmd = null; // now owned by result
                wasClosed = false; // *if* the connection was closed and we got this far, then we now have a reader
                // with the CloseConnection flag, so the reader will deal with the connection; we
                // still need something in the "finally" to ensure that broken SQL still results
                // in the connection closing itself
                return result;
            }
            catch
            {
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        try { cmd?.Cancel(); }
                        catch { /* don't spoil the existing exception */ }
                    }
                    reader.Dispose();
                }
                cmd?.Dispose();
                if (wasClosed) cnn.Close();
                throw;
            }
        }

        /// <summary>
        /// Executes the reader with flags fallback.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <param name="wasClosed">The was closed.</param>
        /// <param name="behavior">The behavior.</param>
        /// <returns>System.Data.IDataReader.</returns>
        private static IDataReader ExecuteReaderWithFlagsFallback(IDbCommand cmd, bool wasClosed, CommandBehavior behavior)
        {
            try
            {
                return cmd.ExecuteReader(GetBehavior(wasClosed, behavior));
            }
            catch (ArgumentException ex)
            { // thanks, Sqlite!
                if (Settings.DisableCommandBehaviorOptimizations(behavior, ex))
                {
                    // we can retry; this time it will have different flags
                    return cmd.ExecuteReader(GetBehavior(wasClosed, behavior));
                }
                throw;
            }
        }

        /// <summary>
        /// Queries the implementation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cnn">The CNN.</param>
        /// <param name="command">The command.</param>
        /// <param name="effectiveType">Type of the effective.</param>
        /// <returns>System.Collections.Generic.IEnumerable&lt;T&gt;.</returns>
        private static IEnumerable<T> QueryImpl<T>(this IDbConnection cnn, CommandDefinition command, Type effectiveType)
        {
            object param = command.Parameters;
            var identity = new Identity(command.CommandText, command.CommandType, cnn, effectiveType, param?.GetType());
            var info = GetCacheInfo(identity, param, command.AddToCache);

            IDbCommand cmd = null;
            IDataReader reader = null;

            bool wasClosed = cnn.State == ConnectionState.Closed;
            try
            {
                cmd = command.SetupCommand(cnn, info.ParamReader);

                if (wasClosed) cnn.Open();
                reader = ExecuteReaderWithFlagsFallback(cmd, wasClosed, CommandBehavior.SequentialAccess | CommandBehavior.SingleResult);
                wasClosed = false; // *if* the connection was closed and we got this far, then we now have a reader
                // with the CloseConnection flag, so the reader will deal with the connection; we
                // still need something in the "finally" to ensure that broken SQL still results
                // in the connection closing itself
                var tuple = info.Deserializer;
                int hash = GetColumnHash(reader);
                if (tuple.Func == null || tuple.Hash != hash)
                {
                    if (reader.FieldCount == 0) //https://code.google.com/p/dapper-dot-net/issues/detail?id=57
                        yield break;
                    tuple = info.Deserializer = new DeserializerState(hash, GetDeserializer(effectiveType, reader, 0, -1, false));
                    if (command.AddToCache) SetQueryCache(identity, info);
                }

                var func = tuple.Func;
                var convertToType = Nullable.GetUnderlyingType(effectiveType) ?? effectiveType;
                while (reader.Read())
                {
                    object val = func(reader);
                    if (val == null || val is T)
                    {
                        yield return (T)val;
                    }
                    else
                    {
                        yield return (T)Convert.ChangeType(val, convertToType, CultureInfo.InvariantCulture);
                    }
                }
                while (reader.NextResult()) { /* ignore subsequent result sets */ }
                // happy path; close the reader cleanly - no
                // need for "Cancel" etc
                reader.Dispose();
                reader = null;

                command.OnCompleted();
            }
            finally
            {
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        try { cmd.Cancel(); }
                        catch { /* don't spoil the existing exception */ }
                    }
                    reader.Dispose();
                }
                if (wasClosed) cnn.Close();
                cmd?.Dispose();
            }
        }

        /// <summary>
        /// Enum Row
        /// </summary>
        [Flags]
        internal enum Row
        {
            /// <summary>
            /// The first
            /// </summary>
            First = 0,
            /// <summary>
            /// The first or default
            /// </summary>
            FirstOrDefault = 1, //  & FirstOrDefault != 0: allow zero rows
            /// <summary>
            /// The single
            /// </summary>
            Single = 2, // & Single != 0: demand at least one row
            /// <summary>
            /// The single or default
            /// </summary>
            SingleOrDefault = 3
        }

        /// <summary>
        /// The error two rows
        /// </summary>
        private static readonly int[] ErrTwoRows = new int[2], ErrZeroRows = Array.Empty<int>();
        /// <summary>
        /// Throws the multiple rows.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <exception cref="InvalidOperationException"></exception>
        private static void ThrowMultipleRows(Row row)
        {
            switch (row)
            {  // get the standard exception from the runtime
                case Row.Single: ErrTwoRows.Single();
                    break;
                case Row.SingleOrDefault: ErrTwoRows.SingleOrDefault();
                    break;
                default: throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Throws the zero rows.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <exception cref="InvalidOperationException"></exception>
        private static void ThrowZeroRows(Row row)
        {
            switch (row)
            { // get the standard exception from the runtime
                case Row.First: ErrZeroRows.First(); break;
                case Row.Single: ErrZeroRows.Single(); break;
                default: throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Queries the row implementation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cnn">The CNN.</param>
        /// <param name="row">The row.</param>
        /// <param name="command">The command.</param>
        /// <param name="effectiveType">Type of the effective.</param>
        /// <returns>T.</returns>
        private static T QueryRowImpl<T>(IDbConnection cnn, Row row, ref CommandDefinition command, Type effectiveType)
        {
            object param = command.Parameters;
            var identity = new Identity(command.CommandText, command.CommandType, cnn, effectiveType, param?.GetType());
            var info = GetCacheInfo(identity, param, command.AddToCache);

            IDbCommand cmd = null;
            IDataReader reader = null;

            bool wasClosed = cnn.State == ConnectionState.Closed;
            try
            {
                cmd = command.SetupCommand(cnn, info.ParamReader);

                if (wasClosed) cnn.Open();
                reader = ExecuteReaderWithFlagsFallback(cmd, wasClosed, (row & Row.Single) != 0
                    ? CommandBehavior.SequentialAccess | CommandBehavior.SingleResult // need to allow multiple rows, to check fail condition
                    : CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.SingleRow);
                wasClosed = false; // *if* the connection was closed and we got this far, then we now have a reader

                T result = default;
                if (reader.Read() && reader.FieldCount != 0)
                {
                    // with the CloseConnection flag, so the reader will deal with the connection; we
                    // still need something in the "finally" to ensure that broken SQL still results
                    // in the connection closing itself
                    var tuple = info.Deserializer;
                    int hash = GetColumnHash(reader);
                    if (tuple.Func == null || tuple.Hash != hash)
                    {
                        tuple = info.Deserializer = new DeserializerState(hash, GetDeserializer(effectiveType, reader, 0, -1, false));
                        if (command.AddToCache) SetQueryCache(identity, info);
                    }

                    var func = tuple.Func;
                    object val = func(reader);
                    if (val == null || val is T)
                    {
                        result = (T)val;
                    }
                    else
                    {
                        var convertToType = Nullable.GetUnderlyingType(effectiveType) ?? effectiveType;
                        result = (T)Convert.ChangeType(val, convertToType, CultureInfo.InvariantCulture);
                    }
                    if ((row & Row.Single) != 0 && reader.Read()) ThrowMultipleRows(row);
                    while (reader.Read()) { /* ignore subsequent rows */ }
                }
                else if ((row & Row.FirstOrDefault) == 0) // demanding a row, and don't have one
                {
                    ThrowZeroRows(row);
                }
                while (reader.NextResult()) { /* ignore subsequent result sets */ }
                // happy path; close the reader cleanly - no
                // need for "Cancel" etc
                reader.Dispose();
                reader = null;

                command.OnCompleted();
                return result;
            }
            finally
            {
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        try { cmd.Cancel(); }
                        catch { /* don't spoil the existing exception */ }
                    }
                    reader.Dispose();
                }
                if (wasClosed) cnn.Close();
                cmd?.Dispose();
            }
        }

        /// <summary>
        /// Perform a multi-mapping query with 2 input types.
        /// This returns a single type, combined from the raw types via <paramref name="map" />.
        /// </summary>
        /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
        /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
        /// <typeparam name="TReturn">The combined type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="map">The function to map row types to the return type.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="buffered">Whether to buffer the results in memory.</param>
        /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>An enumerable of <typeparamref name="TReturn" />.</returns>
        public static IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
            MultiMap<TFirst, TSecond, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(cnn, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);

        /// <summary>
        /// Perform a multi-mapping query with 3 input types.
        /// This returns a single type, combined from the raw types via <paramref name="map" />.
        /// </summary>
        /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
        /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
        /// <typeparam name="TThird">The third type in the recordset.</typeparam>
        /// <typeparam name="TReturn">The combined type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="map">The function to map row types to the return type.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="buffered">Whether to buffer the results in memory.</param>
        /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>An enumerable of <typeparamref name="TReturn" />.</returns>
        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
            MultiMap<TFirst, TSecond, TThird, DontMap, DontMap, DontMap, DontMap, TReturn>(cnn, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);

        /// <summary>
        /// Perform a multi-mapping query with 4 input types.
        /// This returns a single type, combined from the raw types via <paramref name="map" />.
        /// </summary>
        /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
        /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
        /// <typeparam name="TThird">The third type in the recordset.</typeparam>
        /// <typeparam name="TFourth">The fourth type in the recordset.</typeparam>
        /// <typeparam name="TReturn">The combined type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="map">The function to map row types to the return type.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="buffered">Whether to buffer the results in memory.</param>
        /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>An enumerable of <typeparamref name="TReturn" />.</returns>
        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
            MultiMap<TFirst, TSecond, TThird, TFourth, DontMap, DontMap, DontMap, TReturn>(cnn, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);

        /// <summary>
        /// Perform a multi-mapping query with 5 input types.
        /// This returns a single type, combined from the raw types via <paramref name="map" />.
        /// </summary>
        /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
        /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
        /// <typeparam name="TThird">The third type in the recordset.</typeparam>
        /// <typeparam name="TFourth">The fourth type in the recordset.</typeparam>
        /// <typeparam name="TFifth">The fifth type in the recordset.</typeparam>
        /// <typeparam name="TReturn">The combined type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="map">The function to map row types to the return type.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="buffered">Whether to buffer the results in memory.</param>
        /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>An enumerable of <typeparamref name="TReturn" />.</returns>
        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
            MultiMap<TFirst, TSecond, TThird, TFourth, TFifth, DontMap, DontMap, TReturn>(cnn, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);

        /// <summary>
        /// Perform a multi-mapping query with 6 input types.
        /// This returns a single type, combined from the raw types via <paramref name="map" />.
        /// </summary>
        /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
        /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
        /// <typeparam name="TThird">The third type in the recordset.</typeparam>
        /// <typeparam name="TFourth">The fourth type in the recordset.</typeparam>
        /// <typeparam name="TFifth">The fifth type in the recordset.</typeparam>
        /// <typeparam name="TSixth">The sixth type in the recordset.</typeparam>
        /// <typeparam name="TReturn">The combined type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="map">The function to map row types to the return type.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="buffered">Whether to buffer the results in memory.</param>
        /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>An enumerable of <typeparamref name="TReturn" />.</returns>
        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
            MultiMap<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, DontMap, TReturn>(cnn, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);

        /// <summary>
        /// Perform a multi-mapping query with 7 input types. If you need more types -&gt; use Query with Type[] parameter.
        /// This returns a single type, combined from the raw types via <paramref name="map" />.
        /// </summary>
        /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
        /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
        /// <typeparam name="TThird">The third type in the recordset.</typeparam>
        /// <typeparam name="TFourth">The fourth type in the recordset.</typeparam>
        /// <typeparam name="TFifth">The fifth type in the recordset.</typeparam>
        /// <typeparam name="TSixth">The sixth type in the recordset.</typeparam>
        /// <typeparam name="TSeventh">The seventh type in the recordset.</typeparam>
        /// <typeparam name="TReturn">The combined type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="map">The function to map row types to the return type.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="buffered">Whether to buffer the results in memory.</param>
        /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>An enumerable of <typeparamref name="TReturn" />.</returns>
        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
            MultiMap<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(cnn, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);

        /// <summary>
        /// Perform a multi-mapping query with an arbitrary number of input types.
        /// This returns a single type, combined from the raw types via <paramref name="map" />.
        /// </summary>
        /// <typeparam name="TReturn">The combined type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="types">Array of types in the recordset.</param>
        /// <param name="map">The function to map row types to the return type.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="buffered">Whether to buffer the results in memory.</param>
        /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>An enumerable of <typeparamref name="TReturn" />.</returns>
        public static IEnumerable<TReturn> Query<TReturn>(this IDbConnection cnn, string sql, Type[] types, Func<object[], TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        {
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None);
            var results = MultiMapImpl(cnn, command, types, map, splitOn, null, null, true);
            return buffered ? results.ToList() : results;
        }

        /// <summary>
        /// Multis the map.
        /// </summary>
        /// <typeparam name="TFirst">The type of the t first.</typeparam>
        /// <typeparam name="TSecond">The type of the t second.</typeparam>
        /// <typeparam name="TThird">The type of the t third.</typeparam>
        /// <typeparam name="TFourth">The type of the t fourth.</typeparam>
        /// <typeparam name="TFifth">The type of the t fifth.</typeparam>
        /// <typeparam name="TSixth">The type of the t sixth.</typeparam>
        /// <typeparam name="TSeventh">The type of the t seventh.</typeparam>
        /// <typeparam name="TReturn">The type of the t return.</typeparam>
        /// <param name="cnn">The CNN.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="map">The map.</param>
        /// <param name="param">The parameter.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="buffered">The buffered.</param>
        /// <param name="splitOn">The split on.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>System.Collections.Generic.IEnumerable&lt;TReturn&gt;.</returns>
        private static IEnumerable<TReturn> MultiMap<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
            this IDbConnection cnn, string sql, Delegate map, object param, IDbTransaction transaction, bool buffered, string splitOn, int? commandTimeout, CommandType? commandType)
        {
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None);
            var results = MultiMapImpl<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(cnn, command, map, splitOn, null, null, true);
            return buffered ? results.ToList() : results;
        }

        /// <summary>
        /// Multis the map implementation.
        /// </summary>
        /// <typeparam name="TFirst">The type of the t first.</typeparam>
        /// <typeparam name="TSecond">The type of the t second.</typeparam>
        /// <typeparam name="TThird">The type of the t third.</typeparam>
        /// <typeparam name="TFourth">The type of the t fourth.</typeparam>
        /// <typeparam name="TFifth">The type of the t fifth.</typeparam>
        /// <typeparam name="TSixth">The type of the t sixth.</typeparam>
        /// <typeparam name="TSeventh">The type of the t seventh.</typeparam>
        /// <typeparam name="TReturn">The type of the t return.</typeparam>
        /// <param name="cnn">The CNN.</param>
        /// <param name="command">The command.</param>
        /// <param name="map">The map.</param>
        /// <param name="splitOn">The split on.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="identity">The identity.</param>
        /// <param name="finalize">The finalize.</param>
        /// <returns>System.Collections.Generic.IEnumerable&lt;TReturn&gt;.</returns>
        private static IEnumerable<TReturn> MultiMapImpl<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(this IDbConnection cnn, CommandDefinition command, Delegate map, string splitOn, IDataReader reader, Identity identity, bool finalize)
        {
            object param = command.Parameters;
            identity ??= new Identity<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(command.CommandText, command.CommandType, cnn, typeof(TFirst), param?.GetType());
            CacheInfo cinfo = GetCacheInfo(identity, param, command.AddToCache);

            IDbCommand ownedCommand = null;
            IDataReader ownedReader = null;

            bool wasClosed = cnn?.State == ConnectionState.Closed;
            try
            {
                if (reader == null)
                {
                    ownedCommand = command.SetupCommand(cnn, cinfo.ParamReader);
                    if (wasClosed) cnn.Open();
                    ownedReader = ExecuteReaderWithFlagsFallback(ownedCommand, wasClosed, CommandBehavior.SequentialAccess | CommandBehavior.SingleResult);
                    reader = ownedReader;
                }
                var deserializer = default(DeserializerState);
                Func<IDataReader, object>[] otherDeserializers;

                int hash = GetColumnHash(reader);
                if ((deserializer = cinfo.Deserializer).Func == null || (otherDeserializers = cinfo.OtherDeserializers) == null || hash != deserializer.Hash)
                {
                    var deserializers = GenerateDeserializers(identity, splitOn, reader);
                    deserializer = cinfo.Deserializer = new DeserializerState(hash, deserializers[0]);
                    otherDeserializers = cinfo.OtherDeserializers = deserializers.Skip(1).ToArray();
                    if (command.AddToCache) SetQueryCache(identity, cinfo);
                }

                Func<IDataReader, TReturn> mapIt = GenerateMapper<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(deserializer.Func, otherDeserializers, map);

                if (mapIt != null)
                {
                    while (reader.Read())
                    {
                        yield return mapIt(reader);
                    }
                    if (finalize)
                    {
                        while (reader.NextResult()) { /* ignore remaining result sets */ }
                        command.OnCompleted();
                    }
                }
            }
            finally
            {
                try
                {
                    ownedReader?.Dispose();
                }
                finally
                {
                    ownedCommand?.Dispose();
                    if (wasClosed) cnn.Close();
                }
            }
        }

        /// <summary>
        /// Gets the behavior.
        /// </summary>
        /// <param name="close">The close.</param>
        /// <param name="default">The default.</param>
        /// <returns>System.Data.CommandBehavior.</returns>
        private static CommandBehavior GetBehavior(bool close, CommandBehavior @default)
        {
            return (close ? @default | CommandBehavior.CloseConnection : @default) & Settings.AllowedCommandBehaviors;
        }

        /// <summary>
        /// Multis the map implementation.
        /// </summary>
        /// <typeparam name="TReturn">The type of the t return.</typeparam>
        /// <param name="cnn">The CNN.</param>
        /// <param name="command">The command.</param>
        /// <param name="types">The types.</param>
        /// <param name="map">The map.</param>
        /// <param name="splitOn">The split on.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="identity">The identity.</param>
        /// <param name="finalize">The finalize.</param>
        /// <returns>System.Collections.Generic.IEnumerable&lt;TReturn&gt;.</returns>
        /// <exception cref="ArgumentException">you must provide at least one type to deserialize</exception>
        private static IEnumerable<TReturn> MultiMapImpl<TReturn>(this IDbConnection cnn, CommandDefinition command, Type[] types, Func<object[], TReturn> map, string splitOn, IDataReader reader, Identity identity, bool finalize)
        {
            if (types.Length < 1)
            {
                throw new ArgumentException("you must provide at least one type to deserialize");
            }

            object param = command.Parameters;
            identity ??= new IdentityWithTypes(command.CommandText, command.CommandType, cnn, types[0], param?.GetType(), types);
            CacheInfo cinfo = GetCacheInfo(identity, param, command.AddToCache);

            IDbCommand ownedCommand = null;
            IDataReader ownedReader = null;

            bool wasClosed = cnn?.State == ConnectionState.Closed;
            try
            {
                if (reader == null)
                {
                    ownedCommand = command.SetupCommand(cnn, cinfo.ParamReader);
                    if (wasClosed) cnn.Open();
                    ownedReader = ExecuteReaderWithFlagsFallback(ownedCommand, wasClosed, CommandBehavior.SequentialAccess | CommandBehavior.SingleResult);
                    reader = ownedReader;
                }
                DeserializerState deserializer;
                Func<IDataReader, object>[] otherDeserializers;

                int hash = GetColumnHash(reader);
                if ((deserializer = cinfo.Deserializer).Func == null || (otherDeserializers = cinfo.OtherDeserializers) == null || hash != deserializer.Hash)
                {
                    var deserializers = GenerateDeserializers(identity, splitOn, reader);
                    deserializer = cinfo.Deserializer = new DeserializerState(hash, deserializers[0]);
                    otherDeserializers = cinfo.OtherDeserializers = deserializers.Skip(1).ToArray();
                    SetQueryCache(identity, cinfo);
                }

                Func<IDataReader, TReturn> mapIt = GenerateMapper(types.Length, deserializer.Func, otherDeserializers, map);

                if (mapIt != null)
                {
                    while (reader.Read())
                    {
                        yield return mapIt(reader);
                    }
                    if (finalize)
                    {
                        while (reader.NextResult()) { /* ignore subsequent result sets */ }
                        command.OnCompleted();
                    }
                }
            }
            finally
            {
                try
                {
                    ownedReader?.Dispose();
                }
                finally
                {
                    ownedCommand?.Dispose();
                    if (wasClosed) cnn.Close();
                }
            }
        }

        /// <summary>
        /// Generates the mapper.
        /// </summary>
        /// <typeparam name="TFirst">The type of the t first.</typeparam>
        /// <typeparam name="TSecond">The type of the t second.</typeparam>
        /// <typeparam name="TThird">The type of the t third.</typeparam>
        /// <typeparam name="TFourth">The type of the t fourth.</typeparam>
        /// <typeparam name="TFifth">The type of the t fifth.</typeparam>
        /// <typeparam name="TSixth">The type of the t sixth.</typeparam>
        /// <typeparam name="TSeventh">The type of the t seventh.</typeparam>
        /// <typeparam name="TReturn">The type of the t return.</typeparam>
        /// <param name="deserializer">The deserializer.</param>
        /// <param name="otherDeserializers">The other deserializers.</param>
        /// <param name="map">The map.</param>
        /// <returns>System.Func&lt;System.Data.IDataReader, TReturn&gt;.</returns>
        /// <exception cref="NotSupportedException"></exception>
        private static Func<IDataReader, TReturn> GenerateMapper<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(Func<IDataReader, object> deserializer, Func<IDataReader, object>[] otherDeserializers, object map)
        {
            switch (otherDeserializers.Length)
            {
                case 1:
                    return r => ((Func<TFirst, TSecond, TReturn>)map)((TFirst)deserializer(r), (TSecond)otherDeserializers[0](r));
                case 2:
                    return r => ((Func<TFirst, TSecond, TThird, TReturn>)map)((TFirst)deserializer(r), (TSecond)otherDeserializers[0](r), (TThird)otherDeserializers[1](r));
                case 3:
                    return r => ((Func<TFirst, TSecond, TThird, TFourth, TReturn>)map)((TFirst)deserializer(r), (TSecond)otherDeserializers[0](r), (TThird)otherDeserializers[1](r), (TFourth)otherDeserializers[2](r));
                case 4:
                    return r => ((Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>)map)((TFirst)deserializer(r), (TSecond)otherDeserializers[0](r), (TThird)otherDeserializers[1](r), (TFourth)otherDeserializers[2](r), (TFifth)otherDeserializers[3](r));
                case 5:
                    return r => ((Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>)map)((TFirst)deserializer(r), (TSecond)otherDeserializers[0](r), (TThird)otherDeserializers[1](r), (TFourth)otherDeserializers[2](r), (TFifth)otherDeserializers[3](r), (TSixth)otherDeserializers[4](r));
                case 6:
                    return r => ((Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>)map)((TFirst)deserializer(r), (TSecond)otherDeserializers[0](r), (TThird)otherDeserializers[1](r), (TFourth)otherDeserializers[2](r), (TFifth)otherDeserializers[3](r), (TSixth)otherDeserializers[4](r), (TSeventh)otherDeserializers[5](r));
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Generates the mapper.
        /// </summary>
        /// <typeparam name="TReturn">The type of the t return.</typeparam>
        /// <param name="length">The length.</param>
        /// <param name="deserializer">The deserializer.</param>
        /// <param name="otherDeserializers">The other deserializers.</param>
        /// <param name="map">The map.</param>
        /// <returns>System.Func&lt;System.Data.IDataReader, TReturn&gt;.</returns>
        private static Func<IDataReader, TReturn> GenerateMapper<TReturn>(int length, Func<IDataReader, object> deserializer, Func<IDataReader, object>[] otherDeserializers, Func<object[], TReturn> map)
        {
            return r =>
            {
                var objects = new object[length];
                objects[0] = deserializer(r);

                for (var i = 1; i < length; ++i)
                {
                    objects[i] = otherDeserializers[i - 1](r);
                }

                return map(objects);
            };
        }

        /// <summary>
        /// Generates the deserializers.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <param name="splitOn">The split on.</param>
        /// <param name="reader">The reader.</param>
        /// <returns>System.Func&lt;System.Data.IDataReader, object&gt;[].</returns>
        private static Func<IDataReader, object>[] GenerateDeserializers(Identity identity, string splitOn, IDataReader reader)
        {
            var deserializers = new List<Func<IDataReader, object>>();
            var splits = splitOn.Split(',').Select(s => s.Trim()).ToArray();
            bool isMultiSplit = splits.Length > 1;

            int typeCount = identity.TypeCount;
            if (identity.GetType(0) == typeof(object))
            {
                // we go left to right for dynamic multi-mapping so that the madness of TestMultiMappingVariations
                // is supported
                bool first = true;
                int currentPos = 0;
                int splitIdx = 0;
                string currentSplit = splits[splitIdx];

                for (int i = 0; i < typeCount; i++)
                {
                    Type type = identity.GetType(i);
                    if (type == typeof(DontMap))
                    {
                        break;
                    }

                    int splitPoint = GetNextSplitDynamic(currentPos, currentSplit, reader);
                    if (isMultiSplit && splitIdx < splits.Length - 1)
                    {
                        currentSplit = splits[++splitIdx];
                    }
                    deserializers.Add(GetDeserializer(type, reader, currentPos, splitPoint - currentPos, !first));
                    currentPos = splitPoint;
                    first = false;
                }
            }
            else
            {
                // in this we go right to left through the data reader in order to cope with properties that are
                // named the same as a subsequent primary key that we split on
                int currentPos = reader.FieldCount;
                int splitIdx = splits.Length - 1;
                var currentSplit = splits[splitIdx];
                for (var typeIdx = typeCount - 1; typeIdx >= 0; --typeIdx)
                {
                    var type = identity.GetType(typeIdx);
                    if (type == typeof(DontMap))
                    {
                        continue;
                    }

                    int splitPoint = 0;
                    if (typeIdx > 0)
                    {
                        splitPoint = GetNextSplit(currentPos, currentSplit, reader);
                        if (isMultiSplit && splitIdx > 0)
                        {
                            currentSplit = splits[--splitIdx];
                        }
                    }

                    deserializers.Add(GetDeserializer(type, reader, splitPoint, currentPos - splitPoint, typeIdx > 0));
                    currentPos = splitPoint;
                }

                deserializers.Reverse();
            }

            return deserializers.ToArray();
        }

        /// <summary>
        /// Gets the next split dynamic.
        /// </summary>
        /// <param name="startIdx">The start index.</param>
        /// <param name="splitOn">The split on.</param>
        /// <param name="reader">The reader.</param>
        /// <returns>int.</returns>
        private static int GetNextSplitDynamic(int startIdx, string splitOn, IDataReader reader)
        {
            if (startIdx == reader.FieldCount)
            {
                throw MultiMapException(reader);
            }

            if (splitOn == "*")
            {
                return ++startIdx;
            }

            for (var i = startIdx + 1; i < reader.FieldCount; ++i)
            {
                if (string.Equals(splitOn, reader.GetName(i), StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return reader.FieldCount;
        }

        /// <summary>
        /// Gets the next split.
        /// </summary>
        /// <param name="startIdx">The start index.</param>
        /// <param name="splitOn">The split on.</param>
        /// <param name="reader">The reader.</param>
        /// <returns>int.</returns>
        private static int GetNextSplit(int startIdx, string splitOn, IDataReader reader)
        {
            if (splitOn == "*")
            {
                return --startIdx;
            }

            for (var i = startIdx - 1; i > 0; --i)
            {
                if (string.Equals(splitOn, reader.GetName(i), StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            throw MultiMapException(reader);
        }

        /// <summary>
        /// Gets the cache information.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <param name="exampleParameters">The example parameters.</param>
        /// <param name="addToCache">The add to cache.</param>
        /// <returns>ServiceStack.OrmLite.Dapper.SqlMapper.CacheInfo.</returns>
        /// <exception cref="InvalidOperationException">An enumerable sequence of parameters (arrays, lists, etc) is not allowed in this context</exception>
        private static CacheInfo GetCacheInfo(Identity identity, object exampleParameters, bool addToCache)
        {
            if (!TryGetQueryCache(identity, out CacheInfo info))
            {
                if (GetMultiExec(exampleParameters) != null)
                {
                    throw new InvalidOperationException("An enumerable sequence of parameters (arrays, lists, etc) is not allowed in this context");
                }
                info = new CacheInfo();
                if (identity.parametersType != null)
                {
                    Action<IDbCommand, object> reader;
                    if (exampleParameters is IDynamicParameters)
                    {
                        reader = (cmd, obj) => ((IDynamicParameters)obj).AddParameters(cmd, identity);
                    }
                    else if (exampleParameters is IEnumerable<KeyValuePair<string, object>>)
                    {
                        reader = (cmd, obj) =>
                        {
                            IDynamicParameters mapped = new DynamicParameters(obj);
                            mapped.AddParameters(cmd, identity);
                        };
                    }
                    else
                    {
                        var literals = GetLiteralTokens(identity.sql);
                        reader = CreateParamInfoGenerator(identity, false, true, literals);
                    }
                    if ((identity.commandType == null || identity.commandType == CommandType.Text) && ShouldPassByPosition(identity.sql))
                    {
                        var tail = reader;
                        reader = (cmd, obj) =>
                        {
                            tail(cmd, obj);
                            PassByPosition(cmd);
                        };
                    }
                    info.ParamReader = reader;
                }
                if (addToCache) SetQueryCache(identity, info);
            }
            return info;
        }

        /// <summary>
        /// Shoulds the pass by position.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>bool.</returns>
        private static bool ShouldPassByPosition(string sql)
        {
            return sql?.IndexOf('?') >= 0 && pseudoPositional.IsMatch(sql);
        }

        /// <summary>
        /// Passes the by position.
        /// </summary>
        /// <param name="cmd">The command.</param>
        private static void PassByPosition(IDbCommand cmd)
        {
            if (cmd.Parameters.Count == 0) return;

            Dictionary<string, IDbDataParameter> parameters = new(StringComparer.Ordinal);

            foreach (IDbDataParameter param in cmd.Parameters)
            {
                if (!string.IsNullOrEmpty(param.ParameterName)) parameters[param.ParameterName] = param;
            }
            HashSet<string> consumed = new(StringComparer.Ordinal);
            bool firstMatch = true;
            cmd.CommandText = pseudoPositional.Replace(cmd.CommandText, match =>
            {
                string key = match.Groups[1].Value;
                if (!consumed.Add(key))
                {
                    throw new InvalidOperationException("When passing parameters by position, each parameter can only be referenced once");
                }
                else if (parameters.TryGetValue(key, out IDbDataParameter param))
                {
                    if (firstMatch)
                    {
                        firstMatch = false;
                        cmd.Parameters.Clear(); // only clear if we are pretty positive that we've found this pattern successfully
                    }
                    // if found, return the anonymous token "?"
                    cmd.Parameters.Add(param);
                    parameters.Remove(key);
                    consumed.Add(key);
                    return "?";
                }
                else
                {
                    // otherwise, leave alone for simple debugging
                    return match.Value;
                }
            });
        }

        /// <summary>
        /// Gets the deserializer.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="startBound">The start bound.</param>
        /// <param name="length">The length.</param>
        /// <param name="returnNullIfFirstMissing">The return null if first missing.</param>
        /// <returns>System.Func&lt;System.Data.IDataReader, object&gt;.</returns>
        private static Func<IDataReader, object> GetDeserializer(Type type, IDataReader reader, int startBound, int length, bool returnNullIfFirstMissing)
        {
            // dynamic is passed in as Object ... by c# design
            if (type == typeof(object) || type == typeof(DapperRow))
            {
                return GetDapperRowDeserializer(reader, startBound, length, returnNullIfFirstMissing);
            }
            Type underlyingType = null;
            if (!(typeMap.ContainsKey(type) || type.IsEnum || type.FullName == LinqBinary
                || type.IsValueType && (underlyingType = Nullable.GetUnderlyingType(type)) != null && underlyingType.IsEnum))
            {
                if (typeHandlers.TryGetValue(type, out ITypeHandler handler))
                {
                    return GetHandlerDeserializer(handler, type, startBound);
                }
                return GetTypeDeserializer(type, reader, startBound, length, returnNullIfFirstMissing);
            }
            return GetStructDeserializer(type, underlyingType ?? type, startBound);
        }

        /// <summary>
        /// Gets the handler deserializer.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="type">The type.</param>
        /// <param name="startBound">The start bound.</param>
        /// <returns>System.Func&lt;System.Data.IDataReader, object&gt;.</returns>
        private static Func<IDataReader, object> GetHandlerDeserializer(ITypeHandler handler, Type type, int startBound)
        {
            return reader => handler.Parse(type, reader.GetValue(startBound));
        }

        /// <summary>
        /// Multis the map exception.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>System.Exception.</returns>
        private static Exception MultiMapException(IDataRecord reader)
        {
            bool hasFields = false;
            try { hasFields = reader != null && reader.FieldCount != 0; }
            catch { /* don't throw when trying to throw */ }
            if (hasFields)
                return new ArgumentException("When using the multi-mapping APIs ensure you set the splitOn param if you have keys other than Id", "splitOn");
            else
                return new InvalidOperationException("No columns were selected");
        }

        /// <summary>
        /// Gets the dapper row deserializer.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="startBound">The start bound.</param>
        /// <param name="length">The length.</param>
        /// <param name="returnNullIfFirstMissing">The return null if first missing.</param>
        /// <returns>System.Func&lt;System.Data.IDataReader, object&gt;.</returns>
        /// <exception cref="DapperTable">names</exception>
        internal static Func<IDataReader, object> GetDapperRowDeserializer(IDataRecord reader, int startBound, int length, bool returnNullIfFirstMissing)
        {
            var fieldCount = reader.FieldCount;
            if (length == -1)
            {
                length = fieldCount - startBound;
            }

            if (fieldCount <= startBound)
            {
                throw MultiMapException(reader);
            }

            var effectiveFieldCount = Math.Min(fieldCount - startBound, length);

            DapperTable table = null;

            return
                r =>
                {
                    if (table == null)
                    {
                        string[] names = new string[effectiveFieldCount];
                        for (int i = 0; i < effectiveFieldCount; i++)
                        {
                            names[i] = r.GetName(i + startBound);
                        }
                        table = new DapperTable(names);
                    }

                    var values = new object[effectiveFieldCount];

                    if (returnNullIfFirstMissing)
                    {
                        values[0] = r.GetValue(startBound);
                        if (values[0] is DBNull)
                        {
                            return null;
                        }
                    }

                    if (startBound == 0)
                    {
                        for (int i = 0; i < values.Length; i++)
                        {
                            object val = r.GetValue(i);
                            values[i] = val is DBNull ? null : val;
                        }
                    }
                    else
                    {
                        var begin = returnNullIfFirstMissing ? 1 : 0;
                        for (var iter = begin; iter < effectiveFieldCount; ++iter)
                        {
                            object obj = r.GetValue(iter + startBound);
                            values[iter] = obj is DBNull ? null : obj;
                        }
                    }
                    return new DapperRow(table, values);
                };
        }
        /// <summary>
        /// Internal use only.
        /// </summary>
        /// <param name="value">The object to convert to a character.</param>
        /// <returns>char.</returns>
        /// <exception cref="ArgumentNullException">nameof(value)</exception>
        /// <exception cref="ArgumentNullException">nameof(value)</exception>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(ObsoleteInternalUsageOnly, false)]
        public static char ReadChar(object value)
        {
            return value switch {
                null or DBNull => throw new ArgumentNullException(nameof(value)),
                string {Length: 1} s => s[0],
                char c => c,
                _ => throw new ArgumentException("A single-character was expected", nameof(value))
            };
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        /// <param name="value">The object to convert to a character.</param>
        /// <returns>char?.</returns>
        /// <exception cref="ArgumentException">A single-character was expected, nameof(value)</exception>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(ObsoleteInternalUsageOnly, false)]
        public static char? ReadNullableChar(object value)
        {
            switch (value)
            {
                case null:
                case DBNull:
                    return null;
                case string {Length: 1} s:
                    return s[0];
                case char c:
                    return c;
                default:
                    throw new ArgumentException("A single-character was expected", nameof(value));
            }
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        /// <param name="parameters">The parameter collection to search in.</param>
        /// <param name="command">The command for this fetch.</param>
        /// <param name="name">The name of the parameter to get.</param>
        /// <returns>System.Data.IDbDataParameter.</returns>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(ObsoleteInternalUsageOnly, true)]
        public static IDbDataParameter FindOrAddParameter(IDataParameterCollection parameters, IDbCommand command, string name)
        {
            IDbDataParameter result;
            if (parameters.Contains(name))
            {
                result = (IDbDataParameter)parameters[name];
            }
            else
            {
                result = command.CreateParameter();
                result.ParameterName = name;
                parameters.Add(result);
            }
            return result;
        }

        /// <summary>
        /// Gets the list padding extra count.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns>int.</returns>
        internal static int GetListPaddingExtraCount(int count)
        {
            switch (count)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    return 0; // no padding
            }
            if (count < 0) return 0;

            int padFactor;
            if (count <= 150) padFactor = 10;
            else if (count <= 750) padFactor = 50;
            else if (count <= 2000) padFactor = 100; // note: max param count for SQL Server
            else if (count <= 2070) padFactor = 10; // try not to over-pad as we approach that limit
            else if (count <= 2100) return 0; // just don't pad between 2070 and 2100, to minimize the crazy
            else padFactor = 200; // above that, all bets are off!

            // if we have 17, factor = 10; 17 % 10 = 7, we need 3 more
            int intoBlock = count % padFactor;
            return intoBlock == 0 ? 0 : padFactor - intoBlock;
        }

        /// <summary>
        /// Gets the in list regex.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="byPosition">The by position.</param>
        /// <returns>string.</returns>
        private static string GetInListRegex(string name, bool byPosition) => byPosition
            ? @"(\?)" + Regex.Escape(name) + @"\?(?!\w)(\s+(?i)unknown(?-i))?"
            : "([?@:]" + Regex.Escape(name) + @")(?!\w)(\s+(?i)unknown(?-i))?";

        /// <summary>
        /// Internal use only.
        /// </summary>
        /// <param name="command">The command to pack parameters for.</param>
        /// <param name="namePrefix">The name prefix for these parameters.</param>
        /// <param name="value">The parameter value can be an <see cref="IEnumerable{T}" /></param>
        /// <exception cref="NotSupportedException">The first item in a list-expansion cannot be null</exception>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(ObsoleteInternalUsageOnly, false)]
        public static void PackListParameters(IDbCommand command, string namePrefix, object value)
        {
            // initially we tried TVP, however it performs quite poorly.
            // keep in mind SQL support up to 2000 params easily in sp_executesql, needing more is rare

            if (FeatureSupport.Get(command.Connection).Arrays)
            {
                var arrayParm = command.CreateParameter();
                arrayParm.Value = SanitizeParameterValue(value);
                arrayParm.ParameterName = namePrefix;
                command.Parameters.Add(arrayParm);
            }
            else
            {
                bool byPosition = ShouldPassByPosition(command.CommandText);
                var list = value as IEnumerable;
                var count = 0;
                bool isString = value is IEnumerable<string>;
                bool isDbString = value is IEnumerable<DbString>;
                DbType dbType = 0;

                int splitAt = SqlMapper.Settings.InListStringSplitCount;
                bool viaSplit = splitAt >= 0
                    && TryStringSplit(ref list, splitAt, namePrefix, command, byPosition);

                if (list != null && !viaSplit)
                {
                    object lastValue = null;
                    foreach (var item in list)
                    {
                        if (++count == 1) // first item: fetch some type info
                        {
                            if (item == null)
                            {
                                throw new NotSupportedException("The first item in a list-expansion cannot be null");
                            }
                            if (!isDbString)
                            {
                                dbType = LookupDbType(item.GetType(), "", true, out ITypeHandler handler);
                            }
                        }
                        var nextName = namePrefix + count.ToString();
                        if (isDbString && item is DbString)
                        {
                            var str = item as DbString;
                            str.AddParameter(command, nextName);
                        }
                        else
                        {
                            var listParam = command.CreateParameter();
                            listParam.ParameterName = nextName;
                            if (isString)
                            {
                                listParam.Size = DbString.DefaultLength;
                                if (item != null && ((string)item).Length > DbString.DefaultLength)
                                {
                                    listParam.Size = -1;
                                }
                            }

                            var tmp = listParam.Value = SanitizeParameterValue(item);
                            if (tmp != null && !(tmp is DBNull))
                                lastValue = tmp; // only interested in non-trivial values for padding

                            if (listParam.DbType != dbType)
                            {
                                listParam.DbType = dbType;
                            }
                            command.Parameters.Add(listParam);
                        }
                    }
                    if (Settings.PadListExpansions && !isDbString && lastValue != null)
                    {
                        int padCount = GetListPaddingExtraCount(count);
                        for (int i = 0; i < padCount; i++)
                        {
                            count++;
                            var padParam = command.CreateParameter();
                            padParam.ParameterName = namePrefix + count.ToString();
                            if (isString) padParam.Size = DbString.DefaultLength;
                            padParam.DbType = dbType;
                            padParam.Value = lastValue;
                            command.Parameters.Add(padParam);
                        }
                    }
                }

                if (viaSplit)
                {
                    // already done
                }
                else
                {
                    var regexIncludingUnknown = GetInListRegex(namePrefix, byPosition);
                    if (count == 0)
                    {
                        command.CommandText = Regex.Replace(command.CommandText, regexIncludingUnknown, match =>
                        {
                            var variableName = match.Groups[1].Value;
                            if (match.Groups[2].Success)
                            {
                                // looks like an optimize hint; leave it alone!
                                return match.Value;
                            }
                            else
                            {
                                return "(SELECT " + variableName + " WHERE 1 = 0)";
                            }
                        }, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant);
                        var dummyParam = command.CreateParameter();
                        dummyParam.ParameterName = namePrefix;
                        dummyParam.Value = DBNull.Value;
                        command.Parameters.Add(dummyParam);
                    }
                    else
                    {
                        command.CommandText = Regex.Replace(command.CommandText, regexIncludingUnknown, match =>
                        {
                            var variableName = match.Groups[1].Value;
                            if (match.Groups[2].Success)
                            {
                                // looks like an optimize hint; expand it
                                var suffix = match.Groups[2].Value;

                                var sb = GetStringBuilder().Append(variableName).Append(1).Append(suffix);
                                for (int i = 2; i <= count; i++)
                                {
                                    sb.Append(',').Append(variableName).Append(i).Append(suffix);
                                }
                                return sb.__ToStringRecycle();
                            }
                            else
                            {
                                var sb = GetStringBuilder().Append('(').Append(variableName);
                                if (!byPosition) sb.Append(1); else sb.Append(namePrefix).Append(1).Append(variableName);
                                for (int i = 2; i <= count; i++)
                                {
                                    sb.Append(',').Append(variableName);
                                    if (!byPosition) sb.Append(i); else sb.Append(namePrefix).Append(i).Append(variableName);
                                }
                                return sb.Append(')').__ToStringRecycle();
                            }
                        }, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant);
                    }
                }
            }
        }

        /// <summary>
        /// Tries the string split.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="splitAt">The split at.</param>
        /// <param name="namePrefix">The name prefix.</param>
        /// <param name="command">The command.</param>
        /// <param name="byPosition">The by position.</param>
        /// <returns>bool.</returns>
        private static bool TryStringSplit(ref IEnumerable list, int splitAt, string namePrefix, IDbCommand command, bool byPosition)
        {
            if (list == null || splitAt < 0) return false;
            switch (list)
            {
                case IEnumerable<int> l:
                    return TryStringSplit(ref l, splitAt, namePrefix, command, "int", byPosition, (sb, i) => sb.Append(i.ToString(CultureInfo.InvariantCulture)));
                case IEnumerable<long> l:
                    return TryStringSplit(ref l, splitAt, namePrefix, command, "bigint", byPosition, (sb, i) => sb.Append(i.ToString(CultureInfo.InvariantCulture)));
                case IEnumerable<short> l:
                    return TryStringSplit(ref l, splitAt, namePrefix, command, "smallint", byPosition, (sb, i) => sb.Append(i.ToString(CultureInfo.InvariantCulture)));
                case IEnumerable<byte> l:
                    return TryStringSplit(ref l, splitAt, namePrefix, command, "tinyint", byPosition, (sb, i) => sb.Append(i.ToString(CultureInfo.InvariantCulture)));
            }
            return false;
        }

        /// <summary>
        /// Tries the string split.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="splitAt">The split at.</param>
        /// <param name="namePrefix">The name prefix.</param>
        /// <param name="command">The command.</param>
        /// <param name="colType">Type of the col.</param>
        /// <param name="byPosition">The by position.</param>
        /// <param name="append">The append.</param>
        /// <returns>bool.</returns>
        private static bool TryStringSplit<T>(ref IEnumerable<T> list, int splitAt, string namePrefix, IDbCommand command, string colType, bool byPosition,
            Action<StringBuilder, T> append)
        {
            var typed = list as ICollection<T>;
            if (typed == null)
            {
                typed = list.ToList();
                list = typed; // because we still need to be able to iterate it, even if we fail here
            }
            if (typed.Count < splitAt) return false;

            string varName = null;
            var regexIncludingUnknown = GetInListRegex(namePrefix, byPosition);
            var sql = Regex.Replace(command.CommandText, regexIncludingUnknown, match =>
            {
                var variableName = match.Groups[1].Value;
                if (match.Groups[2].Success)
                {
                    // looks like an optimize hint; leave it alone!
                    return match.Value;
                }
                else
                {
                    varName = variableName;
                    return "(select cast([value] as " + colType + ") from string_split(" + variableName + ",','))";
                }
            }, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant);
            if (varName == null) return false; // couldn't resolve the var!

            command.CommandText = sql;
            var concatenatedParam = command.CreateParameter();
            concatenatedParam.ParameterName = namePrefix;
            concatenatedParam.DbType = DbType.AnsiString;
            concatenatedParam.Size = -1;
            string val;
            using (var iter = typed.GetEnumerator())
            {
                if (iter.MoveNext())
                {
                    var sb = GetStringBuilder();
                    append(sb, iter.Current);
                    while (iter.MoveNext())
                    {
                        append(sb.Append(','), iter.Current);
                    }
                    val = sb.ToString();
                }
                else
                {
                    val = "";
                }
            }
            concatenatedParam.Value = val;
            command.Parameters.Add(concatenatedParam);
            return true;
        }

        /// <summary>
        /// OBSOLETE: For internal usage only. Sanitizes the paramter value with proper type casting.
        /// </summary>
        /// <param name="value">The value to sanitize.</param>
        /// <returns>object.</returns>
        [Obsolete(ObsoleteInternalUsageOnly, false)]
        public static object SanitizeParameterValue(object value)
        {
            switch (value)
            {
                case null:
                    return DBNull.Value;
                case Enum:
                {
                    TypeCode typeCode = value is IConvertible ? ((IConvertible)value).GetTypeCode() : Type.GetTypeCode(Enum.GetUnderlyingType(value.GetType()));
                    switch (typeCode)
                    {
                        case TypeCode.Byte: return (byte)value;
                        case TypeCode.SByte: return (sbyte)value;
                        case TypeCode.Int16: return (short)value;
                        case TypeCode.Int32: return (int)value;
                        case TypeCode.Int64: return (long)value;
                        case TypeCode.UInt16: return (ushort)value;
                        case TypeCode.UInt32: return (uint)value;
                        case TypeCode.UInt64: return (ulong)value;
                    }

                    break;
                }
            }

            return value;
        }

        /// <summary>
        /// Filters the parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="sql">The SQL.</param>
        /// <returns>System.Collections.Generic.IEnumerable&lt;System.Reflection.PropertyInfo&gt;.</returns>
        private static IEnumerable<PropertyInfo> FilterParameters(IEnumerable<PropertyInfo> parameters, string sql)
        {
            var list = new List<PropertyInfo>(16);
            foreach (var p in parameters)
            {
                if (Regex.IsMatch(sql, @"[?@:]" + p.Name + @"([^\p{L}\p{N}_]+|$)", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant))
                    list.Add(p);
            }
            return list;
        }

        // look for ? / @ / : *by itself*
        /// <summary>
        /// The smells like OLE database
        /// </summary>
        private static readonly Regex smellsLikeOleDb = new(@"(?<![\p{L}\p{N}@_])[?@:](?![\p{L}\p{N}@_])", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled),
            literalTokens = new(@"(?<![\p{L}\p{N}_])\{=([\p{L}\p{N}_]+)\}", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled),
            pseudoPositional = new(@"\?([\p{L}_][\p{L}\p{N}_]*)\?", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        /// <summary>
        /// Replace all literal tokens with their text form.
        /// </summary>
        /// <param name="parameters">The parameter lookup to do replacements with.</param>
        /// <param name="command">The command to repalce parameters in.</param>
        public static void ReplaceLiterals(this IParameterLookup parameters, IDbCommand command)
        {
            var tokens = GetLiteralTokens(command.CommandText);
            if (tokens.Count != 0) ReplaceLiterals(parameters, command, tokens);
        }

        /// <summary>
        /// The format
        /// </summary>
        internal static readonly MethodInfo format = typeof(SqlMapper).GetMethod("Format", BindingFlags.Public | BindingFlags.Static);

        /// <summary>
        /// Convert numeric values to their string form for SQL literal purposes.
        /// </summary>
        /// <param name="value">The value to get a string for.</param>
        /// <returns>string.</returns>
        /// <exception cref="NotSupportedException">$"The type '{value.GetType().Name}' is not supported for SQL literals.</exception>
        [Obsolete(ObsoleteInternalUsageOnly)]
        public static string Format(object value)
        {
            if (value == null)
            {
                return "null";
            }
            else
            {
                switch (Type.GetTypeCode(value.GetType()))
                {
                    case TypeCode.DBNull:
                        return "null";
                    case TypeCode.Boolean:
                        return (bool)value ? "1" : "0";
                    case TypeCode.Byte:
                        return ((byte)value).ToString(CultureInfo.InvariantCulture);
                    case TypeCode.SByte:
                        return ((sbyte)value).ToString(CultureInfo.InvariantCulture);
                    case TypeCode.UInt16:
                        return ((ushort)value).ToString(CultureInfo.InvariantCulture);
                    case TypeCode.Int16:
                        return ((short)value).ToString(CultureInfo.InvariantCulture);
                    case TypeCode.UInt32:
                        return ((uint)value).ToString(CultureInfo.InvariantCulture);
                    case TypeCode.Int32:
                        return ((int)value).ToString(CultureInfo.InvariantCulture);
                    case TypeCode.UInt64:
                        return ((ulong)value).ToString(CultureInfo.InvariantCulture);
                    case TypeCode.Int64:
                        return ((long)value).ToString(CultureInfo.InvariantCulture);
                    case TypeCode.Single:
                        return ((float)value).ToString(CultureInfo.InvariantCulture);
                    case TypeCode.Double:
                        return ((double)value).ToString(CultureInfo.InvariantCulture);
                    case TypeCode.Decimal:
                        return ((decimal)value).ToString(CultureInfo.InvariantCulture);
                    default:
                        var multiExec = GetMultiExec(value);
                        if (multiExec != null)
                        {
                            StringBuilder sb = null;
                            bool first = true;
                            foreach (object subval in multiExec)
                            {
                                if (first)
                                {
                                    sb = GetStringBuilder().Append('(');
                                    first = false;
                                }
                                else
                                {
                                    sb.Append(',');
                                }
                                sb.Append(Format(subval));
                            }
                            if (first)
                            {
                                return "(select null where 1=0)";
                            }
                            else
                            {
                                return sb.Append(')').__ToStringRecycle();
                            }
                        }
                        throw new NotSupportedException($"The type '{value.GetType().Name}' is not supported for SQL literals.");
                }
            }
        }

        /// <summary>
        /// Replaces the literals.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="command">The command.</param>
        /// <param name="tokens">The tokens.</param>
        internal static void ReplaceLiterals(IParameterLookup parameters, IDbCommand command, IList<LiteralToken> tokens)
        {
            var sql = command.CommandText;
            foreach (var token in tokens)
            {
                object value = parameters[token.Member];
#pragma warning disable 0618
                string text = Format(value);
#pragma warning restore 0618
                sql = sql.Replace(token.Token, text);
            }
            command.CommandText = sql;
        }

        /// <summary>
        /// Gets the literal tokens.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>System.Collections.Generic.IList&lt;ServiceStack.OrmLite.Dapper.SqlMapper.LiteralToken&gt;.</returns>
        internal static IList<LiteralToken> GetLiteralTokens(string sql)
        {
            if (string.IsNullOrEmpty(sql)) return LiteralToken.None;
            if (!literalTokens.IsMatch(sql)) return LiteralToken.None;

            var matches = literalTokens.Matches(sql);
            var found = new HashSet<string>(StringComparer.Ordinal);
            List<LiteralToken> list = new(matches.Count);
            foreach (Match match in matches)
            {
                string token = match.Value;
                if (found.Add(match.Value))
                {
                    list.Add(new LiteralToken(token, match.Groups[1].Value));
                }
            }
            return list.Count == 0 ? LiteralToken.None : list;
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        /// <param name="identity">The identity of the generator.</param>
        /// <param name="checkForDuplicates">Whether to check for duplicates.</param>
        /// <param name="removeUnused">Whether to remove unused parameters.</param>
        /// <returns>System.Action&lt;System.Data.IDbCommand, object&gt;.</returns>
        public static Action<IDbCommand, object> CreateParamInfoGenerator(Identity identity, bool checkForDuplicates, bool removeUnused) =>
            CreateParamInfoGenerator(identity, checkForDuplicates, removeUnused, GetLiteralTokens(identity.sql));

        /// <summary>
        /// Determines whether [is value tuple] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>bool.</returns>
        private static bool IsValueTuple(Type type) => type?.IsValueType == true && type.FullName.StartsWith("System.ValueTuple`", StringComparison.Ordinal);

        /// <summary>
        /// Creates the parameter information generator.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <param name="checkForDuplicates">The check for duplicates.</param>
        /// <param name="removeUnused">The remove unused.</param>
        /// <param name="literals">The literals.</param>
        /// <returns>System.Action&lt;System.Data.IDbCommand, object&gt;.</returns>
        /// <exception cref="NotSupportedException">ValueTuple should not be used for parameters - the language-level names are not available to use as parameter names, and it adds unnecessary boxing</exception>
        internal static Action<IDbCommand, object> CreateParamInfoGenerator(Identity identity, bool checkForDuplicates, bool removeUnused, IList<LiteralToken> literals)
        {
            Type type = identity.parametersType;

            if (IsValueTuple(type))
            {
                throw new NotSupportedException("ValueTuple should not be used for parameters - the language-level names are not available to use as parameter names, and it adds unnecessary boxing");
            }

            bool filterParams = false;
            if (removeUnused && identity.commandType.GetValueOrDefault(CommandType.Text) == CommandType.Text)
            {
                filterParams = !smellsLikeOleDb.IsMatch(identity.sql);
            }
            var dm = new DynamicMethod("ParamInfo" + Guid.NewGuid().ToString(), null, new[] { typeof(IDbCommand), typeof(object) }, type, true);

            var il = dm.GetILGenerator();

            bool isStruct = type.IsValueType;
            var _sizeLocal = (LocalBuilder)null;
            LocalBuilder GetSizeLocal() => _sizeLocal ??= il.DeclareLocal(typeof(int));
            il.Emit(OpCodes.Ldarg_1); // stack is now [untyped-param]

            LocalBuilder typedParameterLocal;
            if (isStruct)
            {
                typedParameterLocal = il.DeclareLocal(type.MakeByRefType()); // note: ref-local
                il.Emit(OpCodes.Unbox, type); // stack is now [typed-param]
            }
            else
            {
                typedParameterLocal = il.DeclareLocal(type);
                il.Emit(OpCodes.Castclass, type); // stack is now [typed-param]
            }
            il.Emit(OpCodes.Stloc, typedParameterLocal); // stack is now empty

            il.Emit(OpCodes.Ldarg_0); // stack is now [command]
            il.EmitCall(OpCodes.Callvirt, typeof(IDbCommand).GetProperty(nameof(IDbCommand.Parameters)).GetGetMethod(), null); // stack is now [parameters]

            var allTypeProps = type.GetProperties();
            var propsList = new List<PropertyInfo>(allTypeProps.Length);
            propsList.AddRange(allTypeProps.Where(p => p.GetIndexParameters().Length == 0));

            var ctors = type.GetConstructors();
            ParameterInfo[] ctorParams;
            IEnumerable<PropertyInfo> props = null;
            // try to detect tuple patterns, e.g. anon-types, and use that to choose the order
            // otherwise: alphabetical
            if (ctors.Length == 1 && propsList.Count == (ctorParams = ctors[0].GetParameters()).Length)
            {
                // check if reflection was kind enough to put everything in the right order for us
                bool ok = true;
                for (int i = 0; i < propsList.Count; i++)
                {
                    if (!string.Equals(propsList[i].Name, ctorParams[i].Name, StringComparison.OrdinalIgnoreCase))
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok)
                {
                    // pre-sorted; the reflection gods have smiled upon us
                    props = propsList;
                }
                else
                { // might still all be accounted for; check the hard way
                    var positionByName = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                    foreach (var param in ctorParams)
                    {
                        positionByName[param.Name] = param.Position;
                    }
                    if (positionByName.Count == propsList.Count)
                    {
                        int[] positions = new int[propsList.Count];
                        ok = true;
                        for (int i = 0; i < propsList.Count; i++)
                        {
                            if (!positionByName.TryGetValue(propsList[i].Name, out int pos))
                            {
                                ok = false;
                                break;
                            }
                            positions[i] = pos;
                        }
                        if (ok)
                        {
                            props = propsList.ToArray();
                            Array.Sort(positions, (PropertyInfo[])props);
                        }
                    }
                }
            }
            if (props == null)
            {
                propsList.Sort(new PropertyInfoByNameComparer());
                props = propsList;
            }
            if (filterParams)
            {
                props = FilterParameters(props, identity.sql);
            }

            var callOpCode = isStruct ? OpCodes.Call : OpCodes.Callvirt;
            foreach (var prop in props)
            {
                if (typeof(ICustomQueryParameter).IsAssignableFrom(prop.PropertyType))
                {
                    il.Emit(OpCodes.Ldloc, typedParameterLocal); // stack is now [parameters] [typed-param]
                    il.Emit(callOpCode, prop.GetGetMethod()); // stack is [parameters] [custom]
                    il.Emit(OpCodes.Ldarg_0); // stack is now [parameters] [custom] [command]
                    il.Emit(OpCodes.Ldstr, prop.Name); // stack is now [parameters] [custom] [command] [name]
                    il.EmitCall(OpCodes.Callvirt, prop.PropertyType.GetMethod(nameof(ICustomQueryParameter.AddParameter)), null); // stack is now [parameters]
                    continue;
                }
#pragma warning disable 618
                DbType dbType = LookupDbType(prop.PropertyType, prop.Name, true, out ITypeHandler handler);
#pragma warning restore 618
                if (dbType == DynamicParameters.EnumerableMultiParameter)
                {
                    // this actually represents special handling for list types;
                    il.Emit(OpCodes.Ldarg_0); // stack is now [parameters] [command]
                    il.Emit(OpCodes.Ldstr, prop.Name); // stack is now [parameters] [command] [name]
                    il.Emit(OpCodes.Ldloc, typedParameterLocal); // stack is now [parameters] [command] [name] [typed-param]
                    il.Emit(callOpCode, prop.GetGetMethod()); // stack is [parameters] [command] [name] [typed-value]
                    if (prop.PropertyType.IsValueType)
                    {
                        il.Emit(OpCodes.Box, prop.PropertyType); // stack is [parameters] [command] [name] [boxed-value]
                    }
                    il.EmitCall(OpCodes.Call, typeof(SqlMapper).GetMethod(nameof(SqlMapper.PackListParameters)), null); // stack is [parameters]
                    continue;
                }
                il.Emit(OpCodes.Dup); // stack is now [parameters] [parameters]

                il.Emit(OpCodes.Ldarg_0); // stack is now [parameters] [parameters] [command]

                if (checkForDuplicates)
                {
                    // need to be a little careful about adding; use a utility method
                    il.Emit(OpCodes.Ldstr, prop.Name); // stack is now [parameters] [parameters] [command] [name]
                    il.EmitCall(OpCodes.Call, typeof(SqlMapper).GetMethod(nameof(SqlMapper.FindOrAddParameter)), null); // stack is [parameters] [parameter]
                }
                else
                {
                    // no risk of duplicates; just blindly add
                    il.EmitCall(OpCodes.Callvirt, typeof(IDbCommand).GetMethod(nameof(IDbCommand.CreateParameter)), null);// stack is now [parameters] [parameters] [parameter]

                    il.Emit(OpCodes.Dup);// stack is now [parameters] [parameters] [parameter] [parameter]
                    il.Emit(OpCodes.Ldstr, prop.Name); // stack is now [parameters] [parameters] [parameter] [parameter] [name]
                    il.EmitCall(OpCodes.Callvirt, typeof(IDataParameter).GetProperty(nameof(IDataParameter.ParameterName)).GetSetMethod(), null);// stack is now [parameters] [parameters] [parameter]
                }
                if (dbType != DbType.Time && handler == null) // https://connect.microsoft.com/VisualStudio/feedback/details/381934/sqlparameter-dbtype-dbtype-time-sets-the-parameter-to-sqldbtype-datetime-instead-of-sqldbtype-time
                {
                    il.Emit(OpCodes.Dup);// stack is now [parameters] [[parameters]] [parameter] [parameter]
                    if (dbType == DbType.Object && prop.PropertyType == typeof(object)) // includes dynamic
                    {
                        // look it up from the param value
                        il.Emit(OpCodes.Ldloc, typedParameterLocal); // stack is now [parameters] [[parameters]] [parameter] [parameter] [typed-param]
                        il.Emit(callOpCode, prop.GetGetMethod()); // stack is [parameters] [[parameters]] [parameter] [parameter] [object-value]
                        il.Emit(OpCodes.Call, typeof(SqlMapper).GetMethod(nameof(SqlMapper.GetDbType), BindingFlags.Static | BindingFlags.Public)); // stack is now [parameters] [[parameters]] [parameter] [parameter] [db-type]
                    }
                    else
                    {
                        // constant value; nice and simple
                        EmitInt32(il, (int)dbType);// stack is now [parameters] [[parameters]] [parameter] [parameter] [db-type]
                    }
                    il.EmitCall(OpCodes.Callvirt, typeof(IDataParameter).GetProperty(nameof(IDataParameter.DbType)).GetSetMethod(), null);// stack is now [parameters] [[parameters]] [parameter]
                }

                il.Emit(OpCodes.Dup);// stack is now [parameters] [[parameters]] [parameter] [parameter]
                EmitInt32(il, (int)ParameterDirection.Input);// stack is now [parameters] [[parameters]] [parameter] [parameter] [dir]
                il.EmitCall(OpCodes.Callvirt, typeof(IDataParameter).GetProperty(nameof(IDataParameter.Direction)).GetSetMethod(), null);// stack is now [parameters] [[parameters]] [parameter]

                il.Emit(OpCodes.Dup);// stack is now [parameters] [[parameters]] [parameter] [parameter]
                il.Emit(OpCodes.Ldloc, typedParameterLocal); // stack is now [parameters] [[parameters]] [parameter] [parameter] [typed-param]
                il.Emit(callOpCode, prop.GetGetMethod()); // stack is [parameters] [[parameters]] [parameter] [parameter] [typed-value]
                bool checkForNull;
                if (prop.PropertyType.IsValueType)
                {
                    var propType = prop.PropertyType;
                    var nullType = Nullable.GetUnderlyingType(propType);
                    bool callSanitize = false;

                    if ((nullType ?? propType).IsEnum)
                    {
                        if (nullType != null)
                        {
                            // Nullable<SomeEnum>; we want to box as the underlying type; that's just *hard*; for
                            // simplicity, box as Nullable<SomeEnum> and call SanitizeParameterValue
                            callSanitize = checkForNull = true;
                        }
                        else
                        {
                            checkForNull = false;
                            // non-nullable enum; we can do that! just box to the wrong type! (no, really)
                            switch (Type.GetTypeCode(Enum.GetUnderlyingType(propType)))
                            {
                                case TypeCode.Byte: propType = typeof(byte); break;
                                case TypeCode.SByte: propType = typeof(sbyte); break;
                                case TypeCode.Int16: propType = typeof(short); break;
                                case TypeCode.Int32: propType = typeof(int); break;
                                case TypeCode.Int64: propType = typeof(long); break;
                                case TypeCode.UInt16: propType = typeof(ushort); break;
                                case TypeCode.UInt32: propType = typeof(uint); break;
                                case TypeCode.UInt64: propType = typeof(ulong); break;
                            }
                        }
                    }
                    else
                    {
                        checkForNull = nullType != null;
                    }
                    il.Emit(OpCodes.Box, propType); // stack is [parameters] [[parameters]] [parameter] [parameter] [boxed-value]
                    if (callSanitize)
                    {
                        checkForNull = false; // handled by sanitize
                        il.EmitCall(OpCodes.Call, typeof(SqlMapper).GetMethod(nameof(SanitizeParameterValue)), null);
                        // stack is [parameters] [[parameters]] [parameter] [parameter] [boxed-value]
                    }
                }
                else
                {
                    checkForNull = true; // if not a value-type, need to check
                }
                if (checkForNull)
                {
                    // relative stack: [boxed value]
                    il.Emit(OpCodes.Dup);// relative stack: [boxed value] [boxed value]
                    Label notNull = il.DefineLabel();
                    Label? allDone = dbType == DbType.String || dbType == DbType.AnsiString ? il.DefineLabel() : (Label?)null;
                    il.Emit(OpCodes.Brtrue_S, notNull);
                    // relative stack [boxed value = null]
                    il.Emit(OpCodes.Pop); // relative stack empty
                    il.Emit(OpCodes.Ldsfld, typeof(DBNull).GetField(nameof(DBNull.Value))); // relative stack [DBNull]
                    if (dbType == DbType.String || dbType == DbType.AnsiString)
                    {
                        EmitInt32(il, 0);
                        il.Emit(OpCodes.Stloc, GetSizeLocal());
                    }
                    if (allDone != null) il.Emit(OpCodes.Br_S, allDone.Value);
                    il.MarkLabel(notNull);
                    if (prop.PropertyType == typeof(string))
                    {
                        il.Emit(OpCodes.Dup); // [string] [string]
                        il.EmitCall(OpCodes.Callvirt, typeof(string).GetProperty(nameof(string.Length)).GetGetMethod(), null); // [string] [length]
                        EmitInt32(il, DbString.DefaultLength); // [string] [length] [4000]
                        il.Emit(OpCodes.Cgt); // [string] [0 or 1]
                        Label isLong = il.DefineLabel(), lenDone = il.DefineLabel();
                        il.Emit(OpCodes.Brtrue_S, isLong);
                        EmitInt32(il, DbString.DefaultLength); // [string] [4000]
                        il.Emit(OpCodes.Br_S, lenDone);
                        il.MarkLabel(isLong);
                        EmitInt32(il, -1); // [string] [-1]
                        il.MarkLabel(lenDone);
                        il.Emit(OpCodes.Stloc, GetSizeLocal()); // [string]
                    }
                    if (prop.PropertyType.FullName == LinqBinary)
                    {
                        il.EmitCall(OpCodes.Callvirt, prop.PropertyType.GetMethod("ToArray", BindingFlags.Public | BindingFlags.Instance), null);
                    }
                    if (allDone != null) il.MarkLabel(allDone.Value);
                    // relative stack [boxed value or DBNull]
                }

                if (handler != null)
                {
#pragma warning disable 618
                    il.Emit(OpCodes.Call, typeof(TypeHandlerCache<>).MakeGenericType(prop.PropertyType).GetMethod(nameof(TypeHandlerCache<int>.SetValue))); // stack is now [parameters] [[parameters]] [parameter]
#pragma warning restore 618
                }
                else
                {
                    il.EmitCall(OpCodes.Callvirt, typeof(IDataParameter).GetProperty(nameof(IDataParameter.Value)).GetSetMethod(), null);// stack is now [parameters] [[parameters]] [parameter]
                }

                if (prop.PropertyType == typeof(string))
                {
                    var endOfSize = il.DefineLabel();
                    var sizeLocal = GetSizeLocal();
                    // don't set if 0
                    il.Emit(OpCodes.Ldloc, sizeLocal); // [parameters] [[parameters]] [parameter] [size]
                    il.Emit(OpCodes.Brfalse_S, endOfSize); // [parameters] [[parameters]] [parameter]

                    il.Emit(OpCodes.Dup);// stack is now [parameters] [[parameters]] [parameter] [parameter]
                    il.Emit(OpCodes.Ldloc, sizeLocal); // stack is now [parameters] [[parameters]] [parameter] [parameter] [size]
                    il.EmitCall(OpCodes.Callvirt, typeof(IDbDataParameter).GetProperty(nameof(IDbDataParameter.Size)).GetSetMethod(), null); // stack is now [parameters] [[parameters]] [parameter]

                    il.MarkLabel(endOfSize);
                }
                if (checkForDuplicates)
                {
                    // stack is now [parameters] [parameter]
                    il.Emit(OpCodes.Pop); // don't need parameter any more
                }
                else
                {
                    // stack is now [parameters] [parameters] [parameter]
                    // blindly add
                    il.EmitCall(OpCodes.Callvirt, typeof(IList).GetMethod(nameof(IList.Add)), null); // stack is now [parameters]
                    il.Emit(OpCodes.Pop); // IList.Add returns the new index (int); we don't care
                }
            }

            // stack is currently [parameters]
            il.Emit(OpCodes.Pop); // stack is now empty

            if (literals.Count != 0 && propsList != null)
            {
                il.Emit(OpCodes.Ldarg_0); // command
                il.Emit(OpCodes.Ldarg_0); // command, command
                var cmdText = typeof(IDbCommand).GetProperty(nameof(IDbCommand.CommandText));
                il.EmitCall(OpCodes.Callvirt, cmdText.GetGetMethod(), null); // command, sql
                Dictionary<Type, LocalBuilder> locals = null;
                LocalBuilder local = null;
                foreach (var literal in literals)
                {
                    // find the best member, preferring case-sensitive
                    PropertyInfo exact = null, fallback = null;
                    string huntName = literal.Member;
                    foreach (var t in propsList)
                    {
                        string thisName = t.Name;
                        if (string.Equals(thisName, huntName, StringComparison.OrdinalIgnoreCase))
                        {
                            fallback = t;
                            if (string.Equals(thisName, huntName, StringComparison.Ordinal))
                            {
                                exact = fallback;
                                break;
                            }
                        }
                    }
                    var prop = exact ?? fallback;

                    if (prop != null)
                    {
                        il.Emit(OpCodes.Ldstr, literal.Token);
                        il.Emit(OpCodes.Ldloc, typedParameterLocal); // command, sql, typed parameter
                        il.EmitCall(callOpCode, prop.GetGetMethod(), null); // command, sql, typed value
                        Type propType = prop.PropertyType;
                        var typeCode = Type.GetTypeCode(propType);
                        switch (typeCode)
                        {
                            case TypeCode.Boolean:
                                Label ifTrue = il.DefineLabel(), allDone = il.DefineLabel();
                                il.Emit(OpCodes.Brtrue_S, ifTrue);
                                il.Emit(OpCodes.Ldstr, "0");
                                il.Emit(OpCodes.Br_S, allDone);
                                il.MarkLabel(ifTrue);
                                il.Emit(OpCodes.Ldstr, "1");
                                il.MarkLabel(allDone);
                                break;
                            case TypeCode.Byte:
                            case TypeCode.SByte:
                            case TypeCode.UInt16:
                            case TypeCode.Int16:
                            case TypeCode.UInt32:
                            case TypeCode.Int32:
                            case TypeCode.UInt64:
                            case TypeCode.Int64:
                            case TypeCode.Single:
                            case TypeCode.Double:
                            case TypeCode.Decimal:
                                // need to stloc, ldloca, call
                                // re-use existing locals (both the last known, and via a dictionary)
                                var convert = GetToString(typeCode);
                                if (local == null || local.LocalType != propType)
                                {
                                    if (locals == null)
                                    {
                                        locals = new Dictionary<Type, LocalBuilder>();
                                        local = null;
                                    }
                                    else
                                    {
                                        if (!locals.TryGetValue(propType, out local)) local = null;
                                    }
                                    if (local == null)
                                    {
                                        local = il.DeclareLocal(propType);
                                        locals.Add(propType, local);
                                    }
                                }
                                il.Emit(OpCodes.Stloc, local); // command, sql
                                il.Emit(OpCodes.Ldloca, local); // command, sql, ref-to-value
                                il.EmitCall(OpCodes.Call, InvariantCulture, null); // command, sql, ref-to-value, culture
                                il.EmitCall(OpCodes.Call, convert, null); // command, sql, string value
                                break;
                            default:
                                if (propType.IsValueType) il.Emit(OpCodes.Box, propType); // command, sql, object value
                                il.EmitCall(OpCodes.Call, format, null); // command, sql, string value
                                break;
                        }
                        il.EmitCall(OpCodes.Callvirt, StringReplace, null);
                    }
                }
                il.EmitCall(OpCodes.Callvirt, cmdText.GetSetMethod(), null); // empty
            }

            il.Emit(OpCodes.Ret);
            return (Action<IDbCommand, object>)dm.CreateDelegate(typeof(Action<IDbCommand, object>));
        }

        /// <summary>
        /// To strings
        /// </summary>
        private static readonly Dictionary<TypeCode, MethodInfo> toStrings = new[]
        {
            typeof(bool), typeof(sbyte), typeof(byte), typeof(ushort), typeof(short),
            typeof(uint), typeof(int), typeof(ulong), typeof(long), typeof(float), typeof(double), typeof(decimal)
        }.ToDictionary(Type.GetTypeCode, x => x.GetPublicInstanceMethod(nameof(object.ToString), new[] { typeof(IFormatProvider) }));

        /// <summary>
        /// Gets to string.
        /// </summary>
        /// <param name="typeCode">The type code.</param>
        /// <returns>System.Reflection.MethodInfo.</returns>
        private static MethodInfo GetToString(TypeCode typeCode)
        {
            return toStrings.TryGetValue(typeCode, out MethodInfo method) ? method : null;
        }

        /// <summary>
        /// The string replace
        /// </summary>
        private static readonly MethodInfo StringReplace = typeof(string).GetPublicInstanceMethod(nameof(string.Replace), new Type[] { typeof(string), typeof(string) }),
            InvariantCulture = typeof(CultureInfo).GetProperty(nameof(CultureInfo.InvariantCulture), BindingFlags.Public | BindingFlags.Static).GetGetMethod();

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="cnn">The CNN.</param>
        /// <param name="command">The command.</param>
        /// <param name="paramReader">The parameter reader.</param>
        /// <returns>int.</returns>
        private static int ExecuteCommand(IDbConnection cnn, ref CommandDefinition command, Action<IDbCommand, object> paramReader)
        {
            IDbCommand cmd = null;
            bool wasClosed = cnn.State == ConnectionState.Closed;
            try
            {
                cmd = command.SetupCommand(cnn, paramReader);
                if (wasClosed) cnn.Open();
                int result = cmd.ExecuteNonQuery();
                command.OnCompleted();
                return result;
            }
            finally
            {
                if (wasClosed) cnn.Close();
                cmd?.Dispose();
            }
        }

        /// <summary>
        /// Executes the scalar implementation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cnn">The CNN.</param>
        /// <param name="command">The command.</param>
        /// <returns>T.</returns>
        private static T ExecuteScalarImpl<T>(IDbConnection cnn, ref CommandDefinition command)
        {
            Action<IDbCommand, object> paramReader = null;
            object param = command.Parameters;
            if (param != null)
            {
                var identity = new Identity(command.CommandText, command.CommandType, cnn, null, param.GetType());
                paramReader = GetCacheInfo(identity, command.Parameters, command.AddToCache).ParamReader;
            }

            IDbCommand cmd = null;
            bool wasClosed = cnn.State == ConnectionState.Closed;
            object result;
            try
            {
                cmd = command.SetupCommand(cnn, paramReader);
                if (wasClosed) cnn.Open();
                result = cmd.ExecuteScalar();
                command.OnCompleted();
            }
            finally
            {
                if (wasClosed) cnn.Close();
                cmd?.Dispose();
            }
            return Parse<T>(result);
        }

        /// <summary>
        /// Executes the reader implementation.
        /// </summary>
        /// <param name="cnn">The CNN.</param>
        /// <param name="command">The command.</param>
        /// <param name="commandBehavior">The command behavior.</param>
        /// <param name="cmd">The command.</param>
        /// <returns>System.Data.IDataReader.</returns>
        private static IDataReader ExecuteReaderImpl(IDbConnection cnn, ref CommandDefinition command, CommandBehavior commandBehavior, out IDbCommand cmd)
        {
            Action<IDbCommand, object> paramReader = GetParameterReader(cnn, ref command);
            cmd = null;
            bool wasClosed = cnn.State == ConnectionState.Closed, disposeCommand = true;
            try
            {
                cmd = command.SetupCommand(cnn, paramReader);
                if (wasClosed) cnn.Open();
                var reader = ExecuteReaderWithFlagsFallback(cmd, wasClosed, commandBehavior);
                wasClosed = false; // don't dispose before giving it to them!
                disposeCommand = false;
                // note: command.FireOutputCallbacks(); would be useless here; parameters come at the **end** of the TDS stream
                return reader;
            }
            finally
            {
                if (wasClosed) cnn.Close();
                if (cmd != null && disposeCommand) cmd.Dispose();
            }
        }

        /// <summary>
        /// Gets the parameter reader.
        /// </summary>
        /// <param name="cnn">The CNN.</param>
        /// <param name="command">The command.</param>
        /// <returns>System.Action&lt;System.Data.IDbCommand, object&gt;.</returns>
        /// <exception cref="NotSupportedException">MultiExec is not supported by ExecuteReader</exception>
        private static Action<IDbCommand, object> GetParameterReader(IDbConnection cnn, ref CommandDefinition command)
        {
            object param = command.Parameters;
            IEnumerable multiExec = GetMultiExec(param);
            CacheInfo info = null;
            if (multiExec != null)
            {
                throw new NotSupportedException("MultiExec is not supported by ExecuteReader");
            }

            // nice and simple
            if (param != null)
            {
                var identity = new Identity(command.CommandText, command.CommandType, cnn, null, param.GetType());
                info = GetCacheInfo(identity, param, command.AddToCache);
            }
            var paramReader = info?.ParamReader;
            return paramReader;
        }

        /// <summary>
        /// Gets the structure deserializer.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="effectiveType">Type of the effective.</param>
        /// <param name="index">The index.</param>
        /// <returns>System.Func&lt;System.Data.IDataReader, object&gt;.</returns>
        private static Func<IDataReader, object> GetStructDeserializer(Type type, Type effectiveType, int index)
        {
            // no point using special per-type handling here; it boils down to the same, plus not all are supported anyway (see: SqlDataReader.GetChar - not supported!)
#pragma warning disable 618
            if (type == typeof(char))
            { // this *does* need special handling, though
                return r => ReadChar(r.GetValue(index));
            }
            if (type == typeof(char?))
            {
                return r => ReadNullableChar(r.GetValue(index));
            }
            if (type.FullName == LinqBinary)
            {
                return r => Activator.CreateInstance(type, r.GetValue(index));
            }
#pragma warning restore 618

            if (effectiveType.IsEnum)
            {   // assume the value is returned as the correct type (int/byte/etc), but box back to the typed enum
                return r =>
                {
                    var val = r.GetValue(index);
                    if (val is float || val is double || val is decimal)
                    {
                        val = Convert.ChangeType(val, Enum.GetUnderlyingType(effectiveType), CultureInfo.InvariantCulture);
                    }
                    return val is DBNull ? null : Enum.ToObject(effectiveType, val);
                };
            }
            if (typeHandlers.TryGetValue(type, out ITypeHandler handler))
            {
                return r =>
                {
                    var val = r.GetValue(index);
                    return val is DBNull ? null : handler.Parse(type, val);
                };
            }
            return r =>
            {
                var val = r.GetValue(index);
                return val is DBNull ? null : val;
            };
        }

        /// <summary>
        /// Parses the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        private static T Parse<T>(object value)
        {
            if (value == null || value is DBNull) return default;
            if (value is T) return (T)value;
            var type = typeof(T);
            type = Nullable.GetUnderlyingType(type) ?? type;
            if (type.IsEnum)
            {
                if (value is float || value is double || value is decimal)
                {
                    value = Convert.ChangeType(value, Enum.GetUnderlyingType(type), CultureInfo.InvariantCulture);
                }
                return (T)Enum.ToObject(type, value);
            }
            if (typeHandlers.TryGetValue(type, out ITypeHandler handler))
            {
                return (T)handler.Parse(type, value);
            }
            return (T)Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// The enum parse
        /// </summary>
        private static readonly MethodInfo
                    enumParse = typeof(Enum).GetMethod(nameof(Enum.Parse), new Type[] { typeof(Type), typeof(string), typeof(bool) }),
                    getItem = typeof(IDataRecord).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Where(p => p.GetIndexParameters().Length > 0 && p.GetIndexParameters()[0].ParameterType == typeof(int))
                        .Select(p => p.GetGetMethod()).First();

        /// <summary>
        /// Gets type-map for the given type
        /// </summary>
        /// <returns>Type map instance, default is to create new instance of DefaultTypeMap</returns>
        public static Func<Type, ITypeMap> TypeMapProvider = (Type type) => new DefaultTypeMap(type);

        /// <summary>
        /// Gets type-map for the given <see cref="Type" />.
        /// </summary>
        /// <param name="type">The type to get a map for.</param>
        /// <returns>Type map implementation, DefaultTypeMap instance if no override present</returns>
        /// <exception cref="ArgumentNullException">nameof(type)</exception>
        public static ITypeMap GetTypeMap(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            var map = (ITypeMap)_typeMaps[type];
            if (map == null)
            {
                lock (_typeMaps)
                {   // double-checked; store this to avoid reflection next time we see this type
                    // since multiple queries commonly use the same domain-entity/DTO/view-model type
                    map = (ITypeMap)_typeMaps[type];

                    if (map == null)
                    {
                        map = TypeMapProvider(type);
                        _typeMaps[type] = map;
                    }
                }
            }
            return map;
        }

        // use Hashtable to get free lockless reading
        /// <summary>
        /// The type maps
        /// </summary>
        private static readonly Hashtable _typeMaps = new();

        /// <summary>
        /// Set custom mapping for type deserializers
        /// </summary>
        /// <param name="type">Entity type to override</param>
        /// <param name="map">Mapping rules impementation, null to remove custom map</param>
        /// <exception cref="ArgumentNullException">nameof(type)</exception>
        public static void SetTypeMap(Type type, ITypeMap map)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (map == null || map is DefaultTypeMap)
            {
                lock (_typeMaps)
                {
                    _typeMaps.Remove(type);
                }
            }
            else
            {
                lock (_typeMaps)
                {
                    _typeMaps[type] = map;
                }
            }

            PurgeQueryCacheByType(type);
        }

        /// <summary>
        /// Internal use only
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="startBound">The start bound.</param>
        /// <param name="length">The length.</param>
        /// <param name="returnNullIfFirstMissing">The return null if first missing.</param>
        /// <returns>System.Func&lt;System.Data.IDataReader, object&gt;.</returns>
        public static Func<IDataReader, object> GetTypeDeserializer(
            Type type, IDataReader reader, int startBound = 0, int length = -1, bool returnNullIfFirstMissing = false
        )
        {
            return TypeDeserializerCache.GetReader(type, reader, startBound, length, returnNullIfFirstMissing);
        }

        /// <summary>
        /// Gets the temporary local.
        /// </summary>
        /// <param name="il">The il.</param>
        /// <param name="locals">The locals.</param>
        /// <param name="type">The type.</param>
        /// <param name="initAndLoad">The initialize and load.</param>
        /// <returns>System.Reflection.Emit.LocalBuilder.</returns>
        /// <exception cref="ArgumentNullException">nameof(type)</exception>
        private static LocalBuilder GetTempLocal(ILGenerator il, ref Dictionary<Type, LocalBuilder> locals, Type type, bool initAndLoad)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            locals ??= new Dictionary<Type, LocalBuilder>();
            if (!locals.TryGetValue(type, out LocalBuilder found))
            {
                found = il.DeclareLocal(type);
                locals.Add(type, found);
            }
            if (initAndLoad)
            {
                il.Emit(OpCodes.Ldloca, found);
                il.Emit(OpCodes.Initobj, type);
                il.Emit(OpCodes.Ldloca, found);
                il.Emit(OpCodes.Ldobj, type);
            }
            return found;
        }

        /// <summary>
        /// Gets the type deserializer implementation.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="startBound">The start bound.</param>
        /// <param name="length">The length.</param>
        /// <param name="returnNullIfFirstMissing">The return null if first missing.</param>
        /// <returns>System.Func&lt;System.Data.IDataReader, object&gt;.</returns>
        /// <exception cref="DynamicMethod">Deserialize" + Guid.NewGuid().ToString(), returnType, new[] { typeof(IDataReader) }, type, true</exception>
        private static Func<IDataReader, object> GetTypeDeserializerImpl(
            Type type, IDataReader reader, int startBound = 0, int length = -1, bool returnNullIfFirstMissing = false
        )
        {
            if (length == -1)
            {
                length = reader.FieldCount - startBound;
            }

            if (reader.FieldCount <= startBound)
            {
                throw MultiMapException(reader);
            }

            var returnType = type.IsValueType ? typeof(object) : type;
            var dm = new DynamicMethod("Deserialize" + Guid.NewGuid().ToString(), returnType, new[] { typeof(IDataReader) }, type, true);
            var il = dm.GetILGenerator();

            if (IsValueTuple(type))
            {
                GenerateValueTupleDeserializer(type, reader, startBound, length, il);
            }
            else
            {
                GenerateDeserializerFromMap(type, reader, startBound, length, returnNullIfFirstMissing, il);
            }

            var funcType = System.Linq.Expressions.Expression.GetFuncType(typeof(IDataReader), returnType);
            return (Func<IDataReader, object>)dm.CreateDelegate(funcType);
        }

        /// <summary>
        /// Generates the value tuple deserializer.
        /// </summary>
        /// <param name="valueTupleType">Type of the value tuple.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="startBound">The start bound.</param>
        /// <param name="length">The length.</param>
        /// <param name="il">The il.</param>
        private static void GenerateValueTupleDeserializer(Type valueTupleType, IDataReader reader, int startBound, int length, ILGenerator il)
        {
            var currentValueTupleType = valueTupleType;

            var constructors = new List<ConstructorInfo>();
            var languageTupleElementTypes = new List<Type>();

            while (true)
            {
                var arity = int.Parse(currentValueTupleType.Name.Substring("ValueTuple`".Length), CultureInfo.InvariantCulture);
                var constructorParameterTypes = new Type[arity];
                var restField = (FieldInfo)null;

                foreach (var field in currentValueTupleType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    if (field.Name == "Rest")
                    {
                        restField = field;
                    }
                    else if (field.Name.StartsWith("Item", StringComparison.Ordinal))
                    {
                        var elementNumber = int.Parse(field.Name.Substring("Item".Length), CultureInfo.InvariantCulture);
                        constructorParameterTypes[elementNumber - 1] = field.FieldType;
                    }
                }

                var itemFieldCount = constructorParameterTypes.Length;
                if (restField != null) itemFieldCount--;

                for (var i = 0; i < itemFieldCount; i++)
                {
                    languageTupleElementTypes.Add(constructorParameterTypes[i]);
                }

                if (restField != null)
                {
                    constructorParameterTypes[constructorParameterTypes.Length - 1] = restField.FieldType;
                }

                constructors.Add(currentValueTupleType.GetConstructor(constructorParameterTypes));

                if (restField is null) break;

                currentValueTupleType = restField.FieldType;
                if (!IsValueTuple(currentValueTupleType))
                {
                    throw new InvalidOperationException("The Rest field of a ValueTuple must contain a nested ValueTuple of arity 1 or greater.");
                }
            }

            var stringEnumLocal = (LocalBuilder)null;

            for (var i = 0; i < languageTupleElementTypes.Count; i++)
            {
                var targetType = languageTupleElementTypes[i];

                if (i < length)
                {
                    LoadReaderValueOrBranchToDBNullLabel(
                        il,
                        startBound + i,
                        ref stringEnumLocal,
                        valueCopyLocal: null,
                        reader.GetFieldType(startBound + i),
                        targetType,
                        out var isDbNullLabel);

                    var finishLabel = il.DefineLabel();
                    il.Emit(OpCodes.Br_S, finishLabel);
                    il.MarkLabel(isDbNullLabel);
                    il.Emit(OpCodes.Pop);

                    LoadDefaultValue(il, targetType);

                    il.MarkLabel(finishLabel);
                }
                else
                {
                    LoadDefaultValue(il, targetType);
                }
            }

            for (var i = constructors.Count - 1; i >= 0; i--)
            {
                il.Emit(OpCodes.Newobj, constructors[i]);
            }

            il.Emit(OpCodes.Box, valueTupleType);
            il.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Generates the deserializer from map.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="startBound">The start bound.</param>
        /// <param name="length">The length.</param>
        /// <param name="returnNullIfFirstMissing">The return null if first missing.</param>
        /// <param name="il">The il.</param>
        /// <exception cref="InvalidOperationException">$"A parameterless default constructor or one matching signature {proposedTypes} is required for {type.FullName} materialization</exception>
        private static void GenerateDeserializerFromMap(Type type, IDataReader reader, int startBound, int length, bool returnNullIfFirstMissing, ILGenerator il)
        {
            var currentIndexDiagnosticLocal = il.DeclareLocal(typeof(int));
            var returnValueLocal = il.DeclareLocal(type);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc, currentIndexDiagnosticLocal);

            var names = Enumerable.Range(startBound, length).Select(reader.GetName).ToArray();

            ITypeMap typeMap = GetTypeMap(type);

            int index = startBound;
            ConstructorInfo specializedConstructor = null;

            bool supportInitialize = false;
            Dictionary<Type, LocalBuilder> structLocals = null;
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Ldloca, returnValueLocal);
                il.Emit(OpCodes.Initobj, type);
            }
            else
            {
                var types = new Type[length];
                for (int i = startBound; i < startBound + length; i++)
                {
                    types[i - startBound] = reader.GetFieldType(i);
                }

                var explicitConstr = typeMap.FindExplicitConstructor();
                if (explicitConstr != null)
                {
                    var consPs = explicitConstr.GetParameters();
                    foreach (var p in consPs)
                    {
                        if (!p.ParameterType.IsValueType)
                        {
                            il.Emit(OpCodes.Ldnull);
                        }
                        else
                        {
                            GetTempLocal(il, ref structLocals, p.ParameterType, true);
                        }
                    }

                    il.Emit(OpCodes.Newobj, explicitConstr);
                    il.Emit(OpCodes.Stloc, returnValueLocal);
                    supportInitialize = typeof(ISupportInitialize).IsAssignableFrom(type);
                    if (supportInitialize)
                    {
                        il.Emit(OpCodes.Ldloc, returnValueLocal);
                        il.EmitCall(OpCodes.Callvirt, typeof(ISupportInitialize).GetMethod(nameof(ISupportInitialize.BeginInit)), null);
                    }
                }
                else
                {
                    var ctor = typeMap.FindConstructor(names, types);
                    if (ctor == null)
                    {
                        string proposedTypes = "(" + string.Join(", ", types.Select((t, i) => t.FullName + " " + names[i]).ToArray()) + ")";
                        throw new InvalidOperationException($"A parameterless default constructor or one matching signature {proposedTypes} is required for {type.FullName} materialization");
                    }

                    if (ctor.GetParameters().Length == 0)
                    {
                        il.Emit(OpCodes.Newobj, ctor);
                        il.Emit(OpCodes.Stloc, returnValueLocal);
                        supportInitialize = typeof(ISupportInitialize).IsAssignableFrom(type);
                        if (supportInitialize)
                        {
                            il.Emit(OpCodes.Ldloc, returnValueLocal);
                            il.EmitCall(OpCodes.Callvirt, typeof(ISupportInitialize).GetMethod(nameof(ISupportInitialize.BeginInit)), null);
                        }
                    }
                    else
                    {
                        specializedConstructor = ctor;
                    }
                }
            }

            il.BeginExceptionBlock();
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Ldloca, returnValueLocal); // [target]
            }
            else if (specializedConstructor == null)
            {
                il.Emit(OpCodes.Ldloc, returnValueLocal); // [target]
            }

            var members = (specializedConstructor != null
                ? names.Select(n => typeMap.GetConstructorParameter(specializedConstructor, n))
                : names.Select(n => typeMap.GetMember(n))).ToList();

            // stack is now [target]
            bool first = true;
            var allDone = il.DefineLabel();
            var stringEnumLocal = (LocalBuilder)null;
            var valueCopyDiagnosticLocal = il.DeclareLocal(typeof(object));
            bool applyNullSetting = Settings.ApplyNullValues;
            foreach (var item in members)
            {
                if (item != null)
                {
                    if (specializedConstructor == null)
                        il.Emit(OpCodes.Dup); // stack is now [target][target]
                    Label finishLabel = il.DefineLabel();
                    Type memberType = item.MemberType;

                    // Save off the current index for access if an exception is thrown
                    EmitInt32(il, index);
                    il.Emit(OpCodes.Stloc, currentIndexDiagnosticLocal);

                    LoadReaderValueOrBranchToDBNullLabel(il, index, ref stringEnumLocal, valueCopyDiagnosticLocal, reader.GetFieldType(index), memberType, out var isDbNullLabel);

                    if (specializedConstructor == null)
                    {
                        // Store the value in the property/field
                        if (item.Property != null)
                        {
                            il.Emit(type.IsValueType ? OpCodes.Call : OpCodes.Callvirt, DefaultTypeMap.GetPropertySetter(item.Property, type));
                        }
                        else
                        {
                            il.Emit(OpCodes.Stfld, item.Field); // stack is now [target]
                        }
                    }

                    il.Emit(OpCodes.Br_S, finishLabel); // stack is now [target]

                    il.MarkLabel(isDbNullLabel); // incoming stack: [target][target][value]
                    if (specializedConstructor != null)
                    {
                        il.Emit(OpCodes.Pop);
                        LoadDefaultValue(il, item.MemberType);
                    }
                    else if (applyNullSetting && (!memberType.IsValueType || Nullable.GetUnderlyingType(memberType) != null))
                    {
                        il.Emit(OpCodes.Pop); // stack is now [target][target]
                        // can load a null with this value
                        if (memberType.IsValueType)
                        { // must be Nullable<T> for some T
                            GetTempLocal(il, ref structLocals, memberType, true); // stack is now [target][target][null]
                        }
                        else
                        { // regular reference-type
                            il.Emit(OpCodes.Ldnull); // stack is now [target][target][null]
                        }

                        // Store the value in the property/field
                        if (item.Property != null)
                        {
                            il.Emit(type.IsValueType ? OpCodes.Call : OpCodes.Callvirt, DefaultTypeMap.GetPropertySetter(item.Property, type));
                            // stack is now [target]
                        }
                        else
                        {
                            il.Emit(OpCodes.Stfld, item.Field); // stack is now [target]
                        }
                    }
                    else
                    {
                        il.Emit(OpCodes.Pop); // stack is now [target][target]
                        il.Emit(OpCodes.Pop); // stack is now [target]
                    }

                    if (first && returnNullIfFirstMissing)
                    {
                        il.Emit(OpCodes.Pop);
                        il.Emit(OpCodes.Ldnull); // stack is now [null]
                        il.Emit(OpCodes.Stloc, returnValueLocal);
                        il.Emit(OpCodes.Br, allDone);
                    }

                    il.MarkLabel(finishLabel);
                }
                first = false;
                index++;
            }
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Pop);
            }
            else
            {
                if (specializedConstructor != null)
                {
                    il.Emit(OpCodes.Newobj, specializedConstructor);
                }
                il.Emit(OpCodes.Stloc, returnValueLocal); // stack is empty
                if (supportInitialize)
                {
                    il.Emit(OpCodes.Ldloc, returnValueLocal);
                    il.EmitCall(OpCodes.Callvirt, typeof(ISupportInitialize).GetMethod(nameof(ISupportInitialize.EndInit)), null);
                }
            }
            il.MarkLabel(allDone);
            il.BeginCatchBlock(typeof(Exception)); // stack is Exception
            il.Emit(OpCodes.Ldloc, currentIndexDiagnosticLocal); // stack is Exception, index
            il.Emit(OpCodes.Ldarg_0); // stack is Exception, index, reader
            il.Emit(OpCodes.Ldloc, valueCopyDiagnosticLocal); // stack is Exception, index, reader, value
            il.EmitCall(OpCodes.Call, typeof(SqlMapper).GetMethod(nameof(SqlMapper.ThrowDataException)), null);
            il.EndExceptionBlock();

            il.Emit(OpCodes.Ldloc, returnValueLocal); // stack is [rval]
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Box, type);
            }
            il.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Loads the default value.
        /// </summary>
        /// <param name="il">The il.</param>
        /// <param name="type">The type.</param>
        private static void LoadDefaultValue(ILGenerator il, Type type)
        {
            if (type.IsValueType)
            {
                var local = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldloca, local);
                il.Emit(OpCodes.Initobj, type);
                il.Emit(OpCodes.Ldloc, local);
            }
            else
            {
                il.Emit(OpCodes.Ldnull);
            }
        }

        /// <summary>
        /// Loads the reader value or branch to database null label.
        /// </summary>
        /// <param name="il">The il.</param>
        /// <param name="index">The index.</param>
        /// <param name="stringEnumLocal">The string enum local.</param>
        /// <param name="valueCopyLocal">The value copy local.</param>
        /// <param name="colType">Type of the col.</param>
        /// <param name="memberType">Type of the member.</param>
        /// <param name="isDbNullLabel">The is database null label.</param>
        private static void LoadReaderValueOrBranchToDBNullLabel(ILGenerator il, int index, ref LocalBuilder stringEnumLocal, LocalBuilder valueCopyLocal, Type colType, Type memberType, out Label isDbNullLabel)
        {
            isDbNullLabel = il.DefineLabel();
            il.Emit(OpCodes.Ldarg_0); // stack is now [...][reader]
            EmitInt32(il, index); // stack is now [...][reader][index]
            il.Emit(OpCodes.Callvirt, getItem); // stack is now [...][value-as-object]

            if (valueCopyLocal != null)
            {
                il.Emit(OpCodes.Dup); // stack is now [...][value-as-object][value-as-object]
                il.Emit(OpCodes.Stloc, valueCopyLocal); // stack is now [...][value-as-object]
            }

            if (memberType == typeof(char) || memberType == typeof(char?))
            {
                il.EmitCall(OpCodes.Call, typeof(SqlMapper).GetMethod(
                    memberType == typeof(char) ? nameof(SqlMapper.ReadChar) : nameof(SqlMapper.ReadNullableChar), BindingFlags.Static | BindingFlags.Public), null); // stack is now [...][typed-value]
            }
            else
            {
                il.Emit(OpCodes.Dup); // stack is now [...][value-as-object][value-as-object]
                il.Emit(OpCodes.Isinst, typeof(DBNull)); // stack is now [...][value-as-object][DBNull or null]
                il.Emit(OpCodes.Brtrue_S, isDbNullLabel); // stack is now [...][value-as-object]

                // unbox nullable enums as the primitive, i.e. byte etc

                var nullUnderlyingType = Nullable.GetUnderlyingType(memberType);
                var unboxType = nullUnderlyingType?.IsEnum == true ? nullUnderlyingType : memberType;

                if (unboxType.IsEnum)
                {
                    Type numericType = Enum.GetUnderlyingType(unboxType);
                    if (colType == typeof(string))
                    {
                        if (stringEnumLocal == null)
                        {
                            stringEnumLocal = il.DeclareLocal(typeof(string));
                        }
                        il.Emit(OpCodes.Castclass, typeof(string)); // stack is now [...][string]
                        il.Emit(OpCodes.Stloc, stringEnumLocal); // stack is now [...]
                        il.Emit(OpCodes.Ldtoken, unboxType); // stack is now [...][enum-type-token]
                        il.EmitCall(OpCodes.Call, typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle)), null);// stack is now [...][enum-type]
                        il.Emit(OpCodes.Ldloc, stringEnumLocal); // stack is now [...][enum-type][string]
                        il.Emit(OpCodes.Ldc_I4_1); // stack is now [...][enum-type][string][true]
                        il.EmitCall(OpCodes.Call, enumParse, null); // stack is now [...][enum-as-object]
                        il.Emit(OpCodes.Unbox_Any, unboxType); // stack is now [...][typed-value]
                    }
                    else
                    {
                        FlexibleConvertBoxedFromHeadOfStack(il, colType, unboxType, numericType);
                    }

                    if (nullUnderlyingType != null)
                    {
                        il.Emit(OpCodes.Newobj, memberType.GetConstructor(new[] { nullUnderlyingType })); // stack is now [...][typed-value]
                    }
                }
                else if (memberType.FullName == LinqBinary)
                {
                    il.Emit(OpCodes.Unbox_Any, typeof(byte[])); // stack is now [...][byte-array]
                    il.Emit(OpCodes.Newobj, memberType.GetConstructor(new Type[] { typeof(byte[]) }));// stack is now [...][binary]
                }
                else
                {
                    TypeCode dataTypeCode = Type.GetTypeCode(colType), unboxTypeCode = Type.GetTypeCode(unboxType);
                    bool hasTypeHandler;
                    if ((hasTypeHandler = typeHandlers.ContainsKey(unboxType)) || colType == unboxType || dataTypeCode == unboxTypeCode || dataTypeCode == Type.GetTypeCode(nullUnderlyingType))
                    {
                        if (hasTypeHandler)
                        {
#pragma warning disable 618
                            il.EmitCall(OpCodes.Call, typeof(TypeHandlerCache<>).MakeGenericType(unboxType).GetMethod(nameof(TypeHandlerCache<int>.Parse)), null); // stack is now [...][typed-value]
#pragma warning restore 618
                        }
                        else
                        {
                            il.Emit(OpCodes.Unbox_Any, unboxType); // stack is now [...][typed-value]
                        }
                    }
                    else
                    {
                        // not a direct match; need to tweak the unbox
                        FlexibleConvertBoxedFromHeadOfStack(il, colType, nullUnderlyingType ?? unboxType, null);
                        if (nullUnderlyingType != null)
                        {
                            il.Emit(OpCodes.Newobj, unboxType.GetConstructor(new[] { nullUnderlyingType })); // stack is now [...][typed-value]
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Flexibles the convert boxed from head of stack.
        /// </summary>
        /// <param name="il">The il.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="via">The via.</param>
        private static void FlexibleConvertBoxedFromHeadOfStack(ILGenerator il, Type from, Type to, Type via)
        {
            MethodInfo op;
            if (from == (via ?? to))
            {
                il.Emit(OpCodes.Unbox_Any, to); // stack is now [target][target][typed-value]
            }
            else if ((op = GetOperator(from, to)) != null)
            {
                // this is handy for things like decimal <===> double
                il.Emit(OpCodes.Unbox_Any, from); // stack is now [target][target][data-typed-value]
                il.Emit(OpCodes.Call, op); // stack is now [target][target][typed-value]
            }
            else
            {
                bool handled = false;
                OpCode opCode = default;
                switch (Type.GetTypeCode(from))
                {
                    case TypeCode.Boolean:
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                        handled = true;
                        switch (Type.GetTypeCode(via ?? to))
                        {
                            case TypeCode.Byte:
                                opCode = OpCodes.Conv_Ovf_I1_Un; break;
                            case TypeCode.SByte:
                                opCode = OpCodes.Conv_Ovf_I1; break;
                            case TypeCode.UInt16:
                                opCode = OpCodes.Conv_Ovf_I2_Un; break;
                            case TypeCode.Int16:
                                opCode = OpCodes.Conv_Ovf_I2; break;
                            case TypeCode.UInt32:
                                opCode = OpCodes.Conv_Ovf_I4_Un; break;
                            case TypeCode.Boolean: // boolean is basically an int, at least at this level
                            case TypeCode.Int32:
                                opCode = OpCodes.Conv_Ovf_I4; break;
                            case TypeCode.UInt64:
                                opCode = OpCodes.Conv_Ovf_I8_Un; break;
                            case TypeCode.Int64:
                                opCode = OpCodes.Conv_Ovf_I8; break;
                            case TypeCode.Single:
                                opCode = OpCodes.Conv_R4; break;
                            case TypeCode.Double:
                                opCode = OpCodes.Conv_R8; break;
                            default:
                                handled = false;
                                break;
                        }
                        break;
                }
                if (handled)
                {
                    il.Emit(OpCodes.Unbox_Any, from); // stack is now [target][target][col-typed-value]
                    il.Emit(opCode); // stack is now [target][target][typed-value]
                    if (to == typeof(bool))
                    { // compare to zero; I checked "csc" - this is the trick it uses; nice
                        il.Emit(OpCodes.Ldc_I4_0);
                        il.Emit(OpCodes.Ceq);
                        il.Emit(OpCodes.Ldc_I4_0);
                        il.Emit(OpCodes.Ceq);
                    }
                }
                else
                {
                    il.Emit(OpCodes.Ldtoken, via ?? to); // stack is now [target][target][value][member-type-token]
                    il.EmitCall(OpCodes.Call, typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle)), null); // stack is now [target][target][value][member-type]
                    il.EmitCall(OpCodes.Call, InvariantCulture, null); // stack is now [target][target][value][member-type][culture]
                    il.EmitCall(OpCodes.Call, typeof(Convert).GetMethod(nameof(Convert.ChangeType), new Type[] { typeof(object), typeof(Type), typeof(IFormatProvider) }), null); // stack is now [target][target][boxed-member-type-value]
                    il.Emit(OpCodes.Unbox_Any, to); // stack is now [target][target][typed-value]
                }
            }
        }

        /// <summary>
        /// Gets the operator.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns>System.Reflection.MethodInfo.</returns>
        private static MethodInfo GetOperator(Type from, Type to)
        {
            if (to == null) return null;
            MethodInfo[] fromMethods, toMethods;
            return ResolveOperator(fromMethods = from.GetMethods(BindingFlags.Static | BindingFlags.Public), from, to, "op_Implicit")
                ?? ResolveOperator(toMethods = to.GetMethods(BindingFlags.Static | BindingFlags.Public), from, to, "op_Implicit")
                ?? ResolveOperator(fromMethods, from, to, "op_Explicit")
                ?? ResolveOperator(toMethods, from, to, "op_Explicit");
        }

        /// <summary>
        /// Resolves the operator.
        /// </summary>
        /// <param name="methods">The methods.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.Reflection.MethodInfo.</returns>
        private static MethodInfo ResolveOperator(MethodInfo[] methods, Type from, Type to, string name)
        {
            for (int i = 0; i < methods.Length; i++)
            {
                if (methods[i].Name != name || methods[i].ReturnType != to) continue;
                var args = methods[i].GetParameters();
                if (args.Length != 1 || args[0].ParameterType != from) continue;
                return methods[i];
            }
            return null;
        }

        /// <summary>
        /// Throws a data exception, only used internally
        /// </summary>
        /// <param name="ex">The exception to throw.</param>
        /// <param name="index">The index the exception occured at.</param>
        /// <param name="reader">The reader the exception occured in.</param>
        /// <param name="value">The value that caused the exception.</param>
        /// <exception cref="DataException">$"Error parsing column {index} ({name}={formattedValue}), ex</exception>
        [Obsolete(ObsoleteInternalUsageOnly, false)]
        public static void ThrowDataException(Exception ex, int index, IDataReader reader, object value)
        {
            Exception toThrow;
            try
            {
                string name = "(n/a)", formattedValue = "(n/a)";
                if (reader != null && index >= 0 && index < reader.FieldCount)
                {
                    name = reader.GetName(index);
                    try
                    {
                        if (value == null || value is DBNull)
                        {
                            formattedValue = "<null>";
                        }
                        else
                        {
                            formattedValue = Convert.ToString(value) + " - " + Type.GetTypeCode(value.GetType());
                        }
                    }
                    catch (Exception valEx)
                    {
                        formattedValue = valEx.Message;
                    }
                }
                toThrow = new DataException($"Error parsing column {index} ({name}={formattedValue})", ex);
            }
            catch
            { // throw the **original** exception, wrapped as DataException
                toThrow = new DataException(ex.Message, ex);
            }
            throw toThrow;
        }

        /// <summary>
        /// Emits the int32.
        /// </summary>
        /// <param name="il">The il.</param>
        /// <param name="value">The value.</param>
        private static void EmitInt32(ILGenerator il, int value)
        {
            switch (value)
            {
                case -1: il.Emit(OpCodes.Ldc_I4_M1); break;
                case 0: il.Emit(OpCodes.Ldc_I4_0); break;
                case 1: il.Emit(OpCodes.Ldc_I4_1); break;
                case 2: il.Emit(OpCodes.Ldc_I4_2); break;
                case 3: il.Emit(OpCodes.Ldc_I4_3); break;
                case 4: il.Emit(OpCodes.Ldc_I4_4); break;
                case 5: il.Emit(OpCodes.Ldc_I4_5); break;
                case 6: il.Emit(OpCodes.Ldc_I4_6); break;
                case 7: il.Emit(OpCodes.Ldc_I4_7); break;
                case 8: il.Emit(OpCodes.Ldc_I4_8); break;
                default:
                    if (value >= -128 && value <= 127)
                    {
                        il.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldc_I4, value);
                    }
                    break;
            }
        }

        /// <summary>
        /// How should connection strings be compared for equivalence? Defaults to StringComparer.Ordinal.
        /// Providing a custom implementation can be useful for allowing multi-tenancy databases with identical
        /// schema to share strategies. Note that usual equivalence rules apply: any equivalent connection strings
        /// <b>MUST</b> yield the same hash-code.
        /// </summary>
        /// <value>The connection string comparer.</value>
        public static IEqualityComparer<string> ConnectionStringComparer
        {
            get => connectionStringComparer;
            set => connectionStringComparer = value ?? StringComparer.Ordinal;
        }

        /// <summary>
        /// The connection string comparer
        /// </summary>
        private static IEqualityComparer<string> connectionStringComparer = StringComparer.Ordinal;

        /// <summary>
        /// Key used to indicate the type name associated with a DataTable.
        /// </summary>
        private const string DataTableTypeNameKey = "dapper:TypeName";

        /// <summary>
        /// Used to pass a DataTable as a <see cref="TableValuedParameter" />.
        /// </summary>
        /// <param name="table">The <see cref="DataTable" /> to create this parameter for.</param>
        /// <param name="typeName">The name of the type this parameter is for.</param>
        /// <returns>ServiceStack.OrmLite.Dapper.SqlMapper.ICustomQueryParameter.</returns>
        public static ICustomQueryParameter AsTableValuedParameter(this DataTable table, string typeName = null) =>
            new TableValuedParameter(table, typeName);

        /// <summary>
        /// Associate a DataTable with a type name.
        /// </summary>
        /// <param name="table">The <see cref="DataTable" /> that does with the <paramref name="typeName" />.</param>
        /// <param name="typeName">The name of the type this table is for.</param>
        public static void SetTypeName(this DataTable table, string typeName)
        {
            if (table != null)
            {
                if (string.IsNullOrEmpty(typeName))
                    table.ExtendedProperties.Remove(DataTableTypeNameKey);
                else
                    table.ExtendedProperties[DataTableTypeNameKey] = typeName;
            }
        }

        /// <summary>
        /// Fetch the type name associated with a <see cref="DataTable" />.
        /// </summary>
        /// <param name="table">The <see cref="DataTable" /> that has a type name associated with it.</param>
        /// <returns>string.</returns>
        public static string GetTypeName(this DataTable table) =>
            table?.ExtendedProperties[DataTableTypeNameKey] as string;

        /// <summary>
        /// Used to pass a IEnumerable&lt;SqlDataRecord&gt; as a TableValuedParameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list of records to convert to TVPs.</param>
        /// <param name="typeName">The sql parameter type name.</param>
        /// <returns>ServiceStack.OrmLite.Dapper.SqlMapper.ICustomQueryParameter.</returns>
        public static ICustomQueryParameter AsTableValuedParameter<T>(this IEnumerable<T> list, string typeName = null) where T : IDataRecord =>
            new SqlDataRecordListTVPParameter<T>(list, typeName);

        // one per thread
        /// <summary>
        /// The per thread string builder cache
        /// </summary>
        [ThreadStatic]
        private static StringBuilder perThreadStringBuilderCache;
        /// <summary>
        /// Gets the string builder.
        /// </summary>
        /// <returns>System.Text.StringBuilder.</returns>
        private static StringBuilder GetStringBuilder()
        {
            var tmp = perThreadStringBuilderCache;
            if (tmp != null)
            {
                perThreadStringBuilderCache = null;
                tmp.Length = 0;
                return tmp;
            }
            return new StringBuilder();
        }

        /// <summary>
        /// To the string recycle.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>string.</returns>
        private static string __ToStringRecycle(this StringBuilder obj)
        {
            if (obj == null) return "";
            var s = obj.ToString();
            perThreadStringBuilderCache = perThreadStringBuilderCache ?? obj;
            return s;
        }
    }
}
