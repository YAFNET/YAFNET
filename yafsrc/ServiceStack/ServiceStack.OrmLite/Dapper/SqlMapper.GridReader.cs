// ***********************************************************************
// <copyright file="SqlMapper.GridReader.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Globalization;
namespace ServiceStack.OrmLite.Dapper;

/// <summary>
/// Class SqlMapper.
/// </summary>
public static partial class SqlMapper
{
    /// <summary>
    /// The grid reader provides interfaces for reading multiple result sets from a Dapper query
    /// </summary>
    public partial class GridReader : IDisposable
    {
        /// <summary>
        /// The reader
        /// </summary>
        private IDataReader reader;
        /// <summary>
        /// The identity
        /// </summary>
        private readonly Identity identity;
        /// <summary>
        /// The add to cache
        /// </summary>
        private readonly bool addToCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="GridReader" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="identity">The identity.</param>
        /// <param name="callbacks">The callbacks.</param>
        /// <param name="addToCache">if set to <c>true</c> [add to cache].</param>
        internal GridReader(IDbCommand command, IDataReader reader, Identity identity, IParameterCallbacks callbacks, bool addToCache)
        {
            Command = command;
            this.reader = reader;
            this.identity = identity;
            this.callbacks = callbacks;
            this.addToCache = addToCache;
        }

        /// <summary>
        /// Read the next grid of results, returned as a dynamic object.
        /// </summary>
        /// <param name="buffered">Whether the results should be buffered in memory.</param>
        /// <returns>IEnumerable&lt;dynamic&gt;.</returns>
        /// <remarks>Note: each row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public IEnumerable<dynamic> Read(bool buffered = true) => ReadImpl<dynamic>(typeof(DapperRow), buffered);

        /// <summary>
        /// Read an individual row of the next grid of results, returned as a dynamic object.
        /// </summary>
        /// <returns>dynamic.</returns>
        /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public dynamic ReadFirst() => ReadRow<dynamic>(typeof(DapperRow), Row.First);

        /// <summary>
        /// Read an individual row of the next grid of results, returned as a dynamic object.
        /// </summary>
        /// <returns>dynamic.</returns>
        /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public dynamic ReadFirstOrDefault() => ReadRow<dynamic>(typeof(DapperRow), Row.FirstOrDefault);

        /// <summary>
        /// Read an individual row of the next grid of results, returned as a dynamic object.
        /// </summary>
        /// <returns>dynamic.</returns>
        /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public dynamic ReadSingle() => ReadRow<dynamic>(typeof(DapperRow), Row.Single);

        /// <summary>
        /// Read an individual row of the next grid of results, returned as a dynamic object.
        /// </summary>
        /// <returns>dynamic.</returns>
        /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public dynamic ReadSingleOrDefault() => ReadRow<dynamic>(typeof(DapperRow), Row.SingleOrDefault);

        /// <summary>
        /// Read the next grid of results.
        /// </summary>
        /// <typeparam name="T">The type to read.</typeparam>
        /// <param name="buffered">Whether the results should be buffered in memory.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public IEnumerable<T> Read<T>(bool buffered = true) => ReadImpl<T>(typeof(T), buffered);

        /// <summary>
        /// Read an individual row of the next grid of results.
        /// </summary>
        /// <typeparam name="T">The type to read.</typeparam>
        /// <returns>T.</returns>
        public T ReadFirst<T>() => ReadRow<T>(typeof(T), Row.First);

        /// <summary>
        /// Read an individual row of the next grid of results.
        /// </summary>
        /// <typeparam name="T">The type to read.</typeparam>
        /// <returns>T.</returns>
        public T ReadFirstOrDefault<T>() => ReadRow<T>(typeof(T), Row.FirstOrDefault);

        /// <summary>
        /// Read an individual row of the next grid of results.
        /// </summary>
        /// <typeparam name="T">The type to read.</typeparam>
        /// <returns>T.</returns>
        public T ReadSingle<T>() => ReadRow<T>(typeof(T), Row.Single);

        /// <summary>
        /// Read an individual row of the next grid of results.
        /// </summary>
        /// <typeparam name="T">The type to read.</typeparam>
        /// <returns>T.</returns>
        public T ReadSingleOrDefault<T>() => ReadRow<T>(typeof(T), Row.SingleOrDefault);

        /// <summary>
        /// Read the next grid of results.
        /// </summary>
        /// <param name="type">The type to read.</param>
        /// <param name="buffered">Whether to buffer the results.</param>
        /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">type</exception>
        public IEnumerable<object> Read(Type type, bool buffered = true)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return ReadImpl<object>(type, buffered);
        }

        /// <summary>
        /// Read an individual row of the next grid of results.
        /// </summary>
        /// <param name="type">The type to read.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.ArgumentNullException">type</exception>
        public object ReadFirst(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return ReadRow<object>(type, Row.First);
        }

        /// <summary>
        /// Read an individual row of the next grid of results.
        /// </summary>
        /// <param name="type">The type to read.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.ArgumentNullException">type</exception>
        public object ReadFirstOrDefault(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return ReadRow<object>(type, Row.FirstOrDefault);
        }

        /// <summary>
        /// Read an individual row of the next grid of results.
        /// </summary>
        /// <param name="type">The type to read.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.ArgumentNullException">type</exception>
        public object ReadSingle(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return ReadRow<object>(type, Row.Single);
        }

        /// <summary>
        /// Read an individual row of the next grid of results.
        /// </summary>
        /// <param name="type">The type to read.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.ArgumentNullException">type</exception>
        public object ReadSingleOrDefault(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return ReadRow<object>(type, Row.SingleOrDefault);
        }

        /// <summary>
        /// Reads the implementation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The type.</param>
        /// <param name="buffered">if set to <c>true</c> [buffered].</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        /// <exception cref="System.ObjectDisposedException">The reader has been disposed; this can happen after all data has been consumed</exception>
        /// <exception cref="System.InvalidOperationException">Query results must be consumed in the correct order, and each result can only be consumed once</exception>
        private IEnumerable<T> ReadImpl<T>(Type type, bool buffered)
        {
            if (reader == null) throw new ObjectDisposedException(GetType().FullName, "The reader has been disposed; this can happen after all data has been consumed");
            if (IsConsumed) throw new InvalidOperationException("Query results must be consumed in the correct order, and each result can only be consumed once");
            var typedIdentity = identity.ForGrid(type, gridIndex);
            CacheInfo cache = GetCacheInfo(typedIdentity, null, addToCache);
            var deserializer = cache.Deserializer;

            int hash = GetColumnHash(reader);
            if (deserializer.Func == null || deserializer.Hash != hash)
            {
                deserializer = new DeserializerState(hash, GetDeserializer(type, reader, 0, -1, false));
                cache.Deserializer = deserializer;
            }
            IsConsumed = true;
            var result = ReadDeferred<T>(gridIndex, deserializer.Func, type);
            return buffered ? result.ToList() : result;
        }

        /// <summary>
        /// Reads the row.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The type.</param>
        /// <param name="row">The row.</param>
        /// <returns>T.</returns>
        /// <exception cref="System.ObjectDisposedException">The reader has been disposed; this can happen after all data has been consumed</exception>
        /// <exception cref="System.InvalidOperationException">Query results must be consumed in the correct order, and each result can only be consumed once</exception>
        private T ReadRow<T>(Type type, Row row)
        {
            if (reader == null) throw new ObjectDisposedException(GetType().FullName, "The reader has been disposed; this can happen after all data has been consumed");
            if (IsConsumed) throw new InvalidOperationException("Query results must be consumed in the correct order, and each result can only be consumed once");
            IsConsumed = true;

            T result = default(T);
            if (reader.Read() && reader.FieldCount != 0)
            {
                var typedIdentity = identity.ForGrid(type, gridIndex);
                CacheInfo cache = GetCacheInfo(typedIdentity, null, addToCache);
                var deserializer = cache.Deserializer;

                int hash = GetColumnHash(reader);
                if (deserializer.Func == null || deserializer.Hash != hash)
                {
                    deserializer = new DeserializerState(hash, GetDeserializer(type, reader, 0, -1, false));
                    cache.Deserializer = deserializer;
                }
                object val = deserializer.Func(reader);
                if (val == null || val is T)
                {
                    result = (T)val;
                }
                else
                {
                    var convertToType = Nullable.GetUnderlyingType(type) ?? type;
                    result = (T)Convert.ChangeType(val, convertToType, CultureInfo.InvariantCulture);
                }
                if ((row & Row.Single) != 0 && reader.Read()) ThrowMultipleRows(row);
                while (reader.Read()) { /* ignore subsequent rows */ }
            }
            else if ((row & Row.FirstOrDefault) == 0) // demanding a row, and don't have one
            {
                ThrowZeroRows(row);
            }
            NextResult();
            return result;
        }

        /// <summary>
        /// Multis the read internal.
        /// </summary>
        /// <typeparam name="TFirst">The type of the t first.</typeparam>
        /// <typeparam name="TSecond">The type of the t second.</typeparam>
        /// <typeparam name="TThird">The type of the t third.</typeparam>
        /// <typeparam name="TFourth">The type of the t fourth.</typeparam>
        /// <typeparam name="TFifth">The type of the t fifth.</typeparam>
        /// <typeparam name="TSixth">The type of the t sixth.</typeparam>
        /// <typeparam name="TSeventh">The type of the t seventh.</typeparam>
        /// <typeparam name="TReturn">The type of the t return.</typeparam>
        /// <param name="func">The function.</param>
        /// <param name="splitOn">The split on.</param>
        /// <returns>IEnumerable&lt;TReturn&gt;.</returns>
        private IEnumerable<TReturn> MultiReadInternal<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(Delegate func, string splitOn)
        {
            var identity = this.identity.ForGrid<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(typeof(TReturn), gridIndex);

            IsConsumed = true;

            try
            {
                foreach (var r in MultiMapImpl<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(null, default(CommandDefinition), func, splitOn, reader, identity, false))
                {
                    yield return r;
                }
            }
            finally
            {
                NextResult();
            }
        }

        /// <summary>
        /// Multis the read internal.
        /// </summary>
        /// <typeparam name="TReturn">The type of the t return.</typeparam>
        /// <param name="types">The types.</param>
        /// <param name="map">The map.</param>
        /// <param name="splitOn">The split on.</param>
        /// <returns>IEnumerable&lt;TReturn&gt;.</returns>
        private IEnumerable<TReturn> MultiReadInternal<TReturn>(Type[] types, Func<object[], TReturn> map, string splitOn)
        {
            var identity = this.identity.ForGrid(typeof(TReturn), types, gridIndex);
            try
            {
                foreach (var r in MultiMapImpl<TReturn>(null, default(CommandDefinition), types, map, splitOn, reader, identity, false))
                {
                    yield return r;
                }
            }
            finally
            {
                NextResult();
            }
        }

        /// <summary>
        /// Read multiple objects from a single record set on the grid.
        /// </summary>
        /// <typeparam name="TFirst">The first type in the record set.</typeparam>
        /// <typeparam name="TSecond">The second type in the record set.</typeparam>
        /// <typeparam name="TReturn">The type to return from the record set.</typeparam>
        /// <param name="func">The mapping function from the read types to the return type.</param>
        /// <param name="splitOn">The field(s) we should split and read the second object from (defaults to "id")</param>
        /// <param name="buffered">Whether to buffer results in memory.</param>
        /// <returns>IEnumerable&lt;TReturn&gt;.</returns>
        public IEnumerable<TReturn> Read<TFirst, TSecond, TReturn>(Func<TFirst, TSecond, TReturn> func, string splitOn = "id", bool buffered = true)
        {
            var result = MultiReadInternal<TFirst, TSecond, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(func, splitOn);
            return buffered ? result.ToList() : result;
        }

        /// <summary>
        /// Read multiple objects from a single record set on the grid.
        /// </summary>
        /// <typeparam name="TFirst">The first type in the record set.</typeparam>
        /// <typeparam name="TSecond">The second type in the record set.</typeparam>
        /// <typeparam name="TThird">The third type in the record set.</typeparam>
        /// <typeparam name="TReturn">The type to return from the record set.</typeparam>
        /// <param name="func">The mapping function from the read types to the return type.</param>
        /// <param name="splitOn">The field(s) we should split and read the second object from (defaults to "id")</param>
        /// <param name="buffered">Whether to buffer results in memory.</param>
        /// <returns>IEnumerable&lt;TReturn&gt;.</returns>
        public IEnumerable<TReturn> Read<TFirst, TSecond, TThird, TReturn>(Func<TFirst, TSecond, TThird, TReturn> func, string splitOn = "id", bool buffered = true)
        {
            var result = MultiReadInternal<TFirst, TSecond, TThird, DontMap, DontMap, DontMap, DontMap, TReturn>(func, splitOn);
            return buffered ? result.ToList() : result;
        }

        /// <summary>
        /// Read multiple objects from a single record set on the grid
        /// </summary>
        /// <typeparam name="TFirst">The first type in the record set.</typeparam>
        /// <typeparam name="TSecond">The second type in the record set.</typeparam>
        /// <typeparam name="TThird">The third type in the record set.</typeparam>
        /// <typeparam name="TFourth">The fourth type in the record set.</typeparam>
        /// <typeparam name="TReturn">The type to return from the record set.</typeparam>
        /// <param name="func">The mapping function from the read types to the return type.</param>
        /// <param name="splitOn">The field(s) we should split and read the second object from (defaults to "id")</param>
        /// <param name="buffered">Whether to buffer results in memory.</param>
        /// <returns>IEnumerable&lt;TReturn&gt;.</returns>
        public IEnumerable<TReturn> Read<TFirst, TSecond, TThird, TFourth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TReturn> func, string splitOn = "id", bool buffered = true)
        {
            var result = MultiReadInternal<TFirst, TSecond, TThird, TFourth, DontMap, DontMap, DontMap, TReturn>(func, splitOn);
            return buffered ? result.ToList() : result;
        }

        /// <summary>
        /// Read multiple objects from a single record set on the grid
        /// </summary>
        /// <typeparam name="TFirst">The first type in the record set.</typeparam>
        /// <typeparam name="TSecond">The second type in the record set.</typeparam>
        /// <typeparam name="TThird">The third type in the record set.</typeparam>
        /// <typeparam name="TFourth">The fourth type in the record set.</typeparam>
        /// <typeparam name="TFifth">The fifth type in the record set.</typeparam>
        /// <typeparam name="TReturn">The type to return from the record set.</typeparam>
        /// <param name="func">The mapping function from the read types to the return type.</param>
        /// <param name="splitOn">The field(s) we should split and read the second object from (defaults to "id")</param>
        /// <param name="buffered">Whether to buffer results in memory.</param>
        /// <returns>IEnumerable&lt;TReturn&gt;.</returns>
        public IEnumerable<TReturn> Read<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> func, string splitOn = "id", bool buffered = true)
        {
            var result = MultiReadInternal<TFirst, TSecond, TThird, TFourth, TFifth, DontMap, DontMap, TReturn>(func, splitOn);
            return buffered ? result.ToList() : result;
        }

        /// <summary>
        /// Read multiple objects from a single record set on the grid
        /// </summary>
        /// <typeparam name="TFirst">The first type in the record set.</typeparam>
        /// <typeparam name="TSecond">The second type in the record set.</typeparam>
        /// <typeparam name="TThird">The third type in the record set.</typeparam>
        /// <typeparam name="TFourth">The fourth type in the record set.</typeparam>
        /// <typeparam name="TFifth">The fifth type in the record set.</typeparam>
        /// <typeparam name="TSixth">The sixth type in the record set.</typeparam>
        /// <typeparam name="TReturn">The type to return from the record set.</typeparam>
        /// <param name="func">The mapping function from the read types to the return type.</param>
        /// <param name="splitOn">The field(s) we should split and read the second object from (defaults to "id")</param>
        /// <param name="buffered">Whether to buffer results in memory.</param>
        /// <returns>IEnumerable&lt;TReturn&gt;.</returns>
        public IEnumerable<TReturn> Read<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> func, string splitOn = "id", bool buffered = true)
        {
            var result = MultiReadInternal<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, DontMap, TReturn>(func, splitOn);
            return buffered ? result.ToList() : result;
        }

        /// <summary>
        /// Read multiple objects from a single record set on the grid
        /// </summary>
        /// <typeparam name="TFirst">The first type in the record set.</typeparam>
        /// <typeparam name="TSecond">The second type in the record set.</typeparam>
        /// <typeparam name="TThird">The third type in the record set.</typeparam>
        /// <typeparam name="TFourth">The fourth type in the record set.</typeparam>
        /// <typeparam name="TFifth">The fifth type in the record set.</typeparam>
        /// <typeparam name="TSixth">The sixth type in the record set.</typeparam>
        /// <typeparam name="TSeventh">The seventh type in the record set.</typeparam>
        /// <typeparam name="TReturn">The type to return from the record set.</typeparam>
        /// <param name="func">The mapping function from the read types to the return type.</param>
        /// <param name="splitOn">The field(s) we should split and read the second object from (defaults to "id")</param>
        /// <param name="buffered">Whether to buffer results in memory.</param>
        /// <returns>IEnumerable&lt;TReturn&gt;.</returns>
        public IEnumerable<TReturn> Read<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> func, string splitOn = "id", bool buffered = true)
        {
            var result = MultiReadInternal<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(func, splitOn);
            return buffered ? result.ToList() : result;
        }

        /// <summary>
        /// Read multiple objects from a single record set on the grid
        /// </summary>
        /// <typeparam name="TReturn">The type to return from the record set.</typeparam>
        /// <param name="types">The types to read from the result set.</param>
        /// <param name="map">The mapping function from the read types to the return type.</param>
        /// <param name="splitOn">The field(s) we should split and read the second object from (defaults to "id")</param>
        /// <param name="buffered">Whether to buffer results in memory.</param>
        /// <returns>IEnumerable&lt;TReturn&gt;.</returns>
        public IEnumerable<TReturn> Read<TReturn>(Type[] types, Func<object[], TReturn> map, string splitOn = "id", bool buffered = true)
        {
            var result = MultiReadInternal(types, map, splitOn);
            return buffered ? result.ToList() : result;
        }

        /// <summary>
        /// Reads the deferred.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index">The index.</param>
        /// <param name="deserializer">The deserializer.</param>
        /// <param name="effectiveType">Type of the effective.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        private IEnumerable<T> ReadDeferred<T>(int index, Func<IDataReader, object> deserializer, Type effectiveType)
        {
            try
            {
                var convertToType = Nullable.GetUnderlyingType(effectiveType) ?? effectiveType;
                while (index == gridIndex && reader.Read())
                {
                    object val = deserializer(reader);
                    if (val == null || val is T)
                    {
                        yield return (T)val;
                    }
                    else
                    {
                        yield return (T)Convert.ChangeType(val, convertToType, CultureInfo.InvariantCulture);
                    }
                }
            }
            finally // finally so that First etc progresses things even when multiple rows
            {
                if (index == gridIndex)
                {
                    NextResult();
                }
            }
        }

        /// <summary>
        /// The grid index
        /// </summary>
        private int gridIndex, readCount;
        /// <summary>
        /// The callbacks
        /// </summary>
        private readonly IParameterCallbacks callbacks;

        /// <summary>
        /// Has the underlying reader been consumed?
        /// </summary>
        /// <value><c>true</c> if this instance is consumed; otherwise, <c>false</c>.</value>
        public bool IsConsumed { get; private set; }

        /// <summary>
        /// The command associated with the reader
        /// </summary>
        /// <value>The command.</value>
        public IDbCommand Command { get; set; }

        /// <summary>
        /// Nexts the result.
        /// </summary>
        private void NextResult()
        {
            if (reader.NextResult())
            {
                readCount++;
                gridIndex++;
                IsConsumed = false;
            }
            else
            {
                // happy path; close the reader cleanly - no
                // need for "Cancel" etc
                reader.Dispose();
                reader = null;
                callbacks?.OnCompleted();
                Dispose();
            }
        }

        /// <summary>
        /// Dispose the grid, closing and disposing both the underlying reader and command.
        /// </summary>
        public void Dispose()
        {
            if (reader != null)
            {
                if (!reader.IsClosed) Command?.Cancel();
                reader.Dispose();
                reader = null;
            }
            if (Command != null)
            {
                Command.Dispose();
                Command = null;
            }
        }
    }
}