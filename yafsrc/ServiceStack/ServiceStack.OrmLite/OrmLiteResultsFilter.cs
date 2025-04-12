// ***********************************************************************
// <copyright file="OrmLiteResultsFilter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************


using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using ServiceStack.OrmLite.Base.Common;
using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

/// <summary>
/// Interface IOrmLiteResultsFilter
/// </summary>
public interface IOrmLiteResultsFilter
{
    /// <summary>
    /// Gets the last insert identifier.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>System.Int64.</returns>
    long GetLastInsertId(IDbCommand dbCmd);

    /// <summary>
    /// Gets the list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>List&lt;T&gt;.</returns>
    List<T> GetList<T>(IDbCommand dbCmd);

    /// <summary>
    /// Gets the reference list.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="refType">Type of the reference.</param>
    /// <returns>IList.</returns>
    IList GetRefList(IDbCommand dbCmd, Type refType);

    /// <summary>
    /// Gets the single.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>T.</returns>
    T GetSingle<T>(IDbCommand dbCmd);

    /// <summary>
    /// Gets the reference single.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="refType">Type of the reference.</param>
    /// <returns>System.Object.</returns>
    object GetRefSingle(IDbCommand dbCmd, Type refType);

    /// <summary>
    /// Gets the scalar.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>T.</returns>
    T GetScalar<T>(IDbCommand dbCmd);

    /// <summary>
    /// Gets the scalar.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>System.Object.</returns>
    object GetScalar(IDbCommand dbCmd);

    /// <summary>
    /// Gets the long scalar.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>System.Int64.</returns>
    long GetLongScalar(IDbCommand dbCmd);

    /// <summary>
    /// Gets the column.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>List&lt;T&gt;.</returns>
    List<T> GetColumn<T>(IDbCommand dbCmd);

    /// <summary>
    /// Gets the column distinct.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>HashSet&lt;T&gt;.</returns>
    HashSet<T> GetColumnDistinct<T>(IDbCommand dbCmd);

    /// <summary>
    /// Gets the dictionary.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>Dictionary&lt;K, V&gt;.</returns>
    Dictionary<K, V> GetDictionary<K, V>(IDbCommand dbCmd);

    /// <summary>
    /// Gets the key value pairs.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>List&lt;KeyValuePair&lt;K, V&gt;&gt;.</returns>
    List<KeyValuePair<K, V>> GetKeyValuePairs<K, V>(IDbCommand dbCmd);

    /// <summary>
    /// Gets the lookup.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>Dictionary&lt;K, List&lt;V&gt;&gt;.</returns>
    Dictionary<K, List<V>> GetLookup<K, V>(IDbCommand dbCmd);

    /// <summary>
    /// Executes the SQL.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>System.Int32.</returns>
    int ExecuteSql(IDbCommand dbCmd);
}

/// <summary>
/// Class OrmLiteResultsFilter.
/// Implements the <see cref="ServiceStack.OrmLite.IOrmLiteResultsFilter" />
/// Implements the <see cref="System.IDisposable" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.IOrmLiteResultsFilter" />
/// <seealso cref="System.IDisposable" />
public class OrmLiteResultsFilter : IOrmLiteResultsFilter, IDisposable
{
    /// <summary>
    /// Gets or sets the results.
    /// </summary>
    /// <value>The results.</value>
    public IEnumerable Results { get; set; }
    /// <summary>
    /// Gets or sets the reference results.
    /// </summary>
    /// <value>The reference results.</value>
    public IEnumerable RefResults { get; set; }
    /// <summary>
    /// Gets or sets the column results.
    /// </summary>
    /// <value>The column results.</value>
    public IEnumerable ColumnResults { get; set; }
    /// <summary>
    /// Gets or sets the column distinct results.
    /// </summary>
    /// <value>The column distinct results.</value>
    public IEnumerable ColumnDistinctResults { get; set; }
    /// <summary>
    /// Gets or sets the dictionary results.
    /// </summary>
    /// <value>The dictionary results.</value>
    public IDictionary DictionaryResults { get; set; }
    /// <summary>
    /// Gets or sets the lookup results.
    /// </summary>
    /// <value>The lookup results.</value>
    public IDictionary LookupResults { get; set; }
    /// <summary>
    /// Gets or sets the single result.
    /// </summary>
    /// <value>The single result.</value>
    public object SingleResult { get; set; }
    /// <summary>
    /// Gets or sets the reference single result.
    /// </summary>
    /// <value>The reference single result.</value>
    public object RefSingleResult { get; set; }
    /// <summary>
    /// Gets or sets the scalar result.
    /// </summary>
    /// <value>The scalar result.</value>
    public object ScalarResult { get; set; }
    /// <summary>
    /// Gets or sets the long scalar result.
    /// </summary>
    /// <value>The long scalar result.</value>
    public long LongScalarResult { get; set; }
    /// <summary>
    /// Gets or sets the last insert identifier.
    /// </summary>
    /// <value>The last insert identifier.</value>
    public long LastInsertId { get; set; }
    /// <summary>
    /// Gets or sets the execute SQL result.
    /// </summary>
    /// <value>The execute SQL result.</value>
    public int ExecuteSqlResult { get; set; }

    /// <summary>
    /// Gets or sets the execute SQL function.
    /// </summary>
    /// <value>The execute SQL function.</value>
    public Func<IDbCommand, int> ExecuteSqlFn { get; set; }
    /// <summary>
    /// Gets or sets the results function.
    /// </summary>
    /// <value>The results function.</value>
    public Func<IDbCommand, Type, IEnumerable> ResultsFn { get; set; }
    /// <summary>
    /// Gets or sets the reference results function.
    /// </summary>
    /// <value>The reference results function.</value>
    public Func<IDbCommand, Type, IEnumerable> RefResultsFn { get; set; }
    /// <summary>
    /// Gets or sets the column results function.
    /// </summary>
    /// <value>The column results function.</value>
    public Func<IDbCommand, Type, IEnumerable> ColumnResultsFn { get; set; }
    /// <summary>
    /// Gets or sets the column distinct results function.
    /// </summary>
    /// <value>The column distinct results function.</value>
    public Func<IDbCommand, Type, IEnumerable> ColumnDistinctResultsFn { get; set; }
    /// <summary>
    /// Gets or sets the dictionary results function.
    /// </summary>
    /// <value>The dictionary results function.</value>
    public Func<IDbCommand, Type, Type, IDictionary> DictionaryResultsFn { get; set; }
    /// <summary>
    /// Gets or sets the lookup results function.
    /// </summary>
    /// <value>The lookup results function.</value>
    public Func<IDbCommand, Type, Type, IDictionary> LookupResultsFn { get; set; }
    /// <summary>
    /// Gets or sets the single result function.
    /// </summary>
    /// <value>The single result function.</value>
    public Func<IDbCommand, Type, object> SingleResultFn { get; set; }
    /// <summary>
    /// Gets or sets the reference single result function.
    /// </summary>
    /// <value>The reference single result function.</value>
    public Func<IDbCommand, Type, object> RefSingleResultFn { get; set; }
    /// <summary>
    /// Gets or sets the scalar result function.
    /// </summary>
    /// <value>The scalar result function.</value>
    public Func<IDbCommand, Type, object> ScalarResultFn { get; set; }
    /// <summary>
    /// Gets or sets the long scalar result function.
    /// </summary>
    /// <value>The long scalar result function.</value>
    public Func<IDbCommand, long> LongScalarResultFn { get; set; }
    /// <summary>
    /// Gets or sets the last insert identifier function.
    /// </summary>
    /// <value>The last insert identifier function.</value>
    public Func<IDbCommand, long> LastInsertIdFn { get; set; }

    /// <summary>
    /// Gets or sets the SQL filter.
    /// </summary>
    /// <value>The SQL filter.</value>
    public Action<string> SqlFilter { get; set; }
    /// <summary>
    /// Gets or sets the SQL command filter.
    /// </summary>
    /// <value>The SQL command filter.</value>
    public Action<IDbCommand> SqlCommandFilter { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [print SQL].
    /// </summary>
    /// <value><c>true</c> if [print SQL]; otherwise, <c>false</c>.</value>
    public bool PrintSql { get; set; }

    /// <summary>
    /// The previous filter
    /// </summary>
    private readonly IOrmLiteResultsFilter previousFilter;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrmLiteResultsFilter" /> class.
    /// </summary>
    /// <param name="results">The results.</param>
    public OrmLiteResultsFilter(IEnumerable results = null)
    {
        this.Results = results ?? new object[] { };

        this.previousFilter = OrmLiteConfig.ResultsFilter;
        OrmLiteConfig.ResultsFilter = this;
    }

    /// <summary>
    /// Filters the specified database command.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    private void Filter(IDbCommand dbCmd)
    {
        this.SqlFilter?.Invoke(dbCmd.CommandText);

        this.SqlCommandFilter?.Invoke(dbCmd);

        if (this.PrintSql)
        {
            Console.WriteLine(dbCmd.CommandText);
        }
    }

    /// <summary>
    /// Gets the results.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>IEnumerable.</returns>
    private IEnumerable GetResults<T>(IDbCommand dbCmd)
    {
        return this.ResultsFn != null ? this.ResultsFn(dbCmd, typeof(T)) : this.Results;
    }

    /// <summary>
    /// Gets the reference results.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="refType">Type of the reference.</param>
    /// <returns>IEnumerable.</returns>
    private IEnumerable GetRefResults(IDbCommand dbCmd, Type refType)
    {
        return this.RefResultsFn != null ? this.RefResultsFn(dbCmd, refType) : this.RefResults;
    }

    /// <summary>
    /// Gets the column results.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>IEnumerable.</returns>
    private IEnumerable GetColumnResults<T>(IDbCommand dbCmd)
    {
        return this.ColumnResultsFn != null ? this.ColumnResultsFn(dbCmd, typeof(T)) : this.ColumnResults;
    }

    /// <summary>
    /// Gets the column distinct results.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>IEnumerable.</returns>
    private IEnumerable GetColumnDistinctResults<T>(IDbCommand dbCmd)
    {
        return this.ColumnDistinctResultsFn != null ? this.ColumnDistinctResultsFn(dbCmd, typeof(T)) : this.ColumnDistinctResults;
    }

    /// <summary>
    /// Gets the dictionary results.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>IDictionary.</returns>
    private IDictionary GetDictionaryResults<K, V>(IDbCommand dbCmd)
    {
        return this.DictionaryResultsFn != null ? this.DictionaryResultsFn(dbCmd, typeof(K), typeof(V)) : this.DictionaryResults;
    }

    /// <summary>
    /// Gets the lookup results.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>IDictionary.</returns>
    private IDictionary GetLookupResults<K, V>(IDbCommand dbCmd)
    {
        return this.LookupResultsFn != null ? this.LookupResultsFn(dbCmd, typeof(K), typeof(V)) : this.LookupResults;
    }

    /// <summary>
    /// Gets the single result.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>System.Object.</returns>
    private object GetSingleResult<T>(IDbCommand dbCmd)
    {
        return this.SingleResultFn != null ? this.SingleResultFn(dbCmd, typeof(T)) : this.SingleResult;
    }

    /// <summary>
    /// Gets the reference single result.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="refType">Type of the reference.</param>
    /// <returns>System.Object.</returns>
    private object GetRefSingleResult(IDbCommand dbCmd, Type refType)
    {
        return this.RefSingleResultFn != null ? this.RefSingleResultFn(dbCmd, refType) : this.RefSingleResult;
    }

    /// <summary>
    /// Gets the scalar result.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>System.Object.</returns>
    private object GetScalarResult<T>(IDbCommand dbCmd)
    {
        return this.ScalarResultFn != null ? this.ScalarResultFn(dbCmd, typeof(T)) : this.ScalarResult;
    }

    /// <summary>
    /// Gets the long scalar result.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>System.Int64.</returns>
    private long GetLongScalarResult(IDbCommand dbCmd)
    {
        return this.LongScalarResultFn?.Invoke(dbCmd) ?? this.LongScalarResult;
    }

    /// <summary>
    /// Gets the last insert identifier.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>System.Int64.</returns>
    public long GetLastInsertId(IDbCommand dbCmd)
    {
        return this.LastInsertIdFn?.Invoke(dbCmd) ?? this.LastInsertId;
    }

    /// <summary>
    /// Gets the list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>List&lt;T&gt;.</returns>
    public List<T> GetList<T>(IDbCommand dbCmd)
    {
        this.Filter(dbCmd);
        return [.. (from object result in this.GetResults<T>(dbCmd) select (T)result)];
    }

    /// <summary>
    /// Gets the reference list.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="refType">Type of the reference.</param>
    /// <returns>IList.</returns>
    public IList GetRefList(IDbCommand dbCmd, Type refType)
    {
        this.Filter(dbCmd);
        var list = (IList)typeof(List<>).GetCachedGenericType(refType).CreateInstance();
        foreach (var result in this.GetRefResults(dbCmd, refType).Safe())
        {
            list.Add(result);
        }
        return list;
    }

    /// <summary>
    /// Gets the single.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>T.</returns>
    public T GetSingle<T>(IDbCommand dbCmd)
    {
        this.Filter(dbCmd);
        if (this.SingleResult != null || this.SingleResultFn != null)
        {
            return (T)this.GetSingleResult<T>(dbCmd);
        }

        foreach (var result in this.GetResults<T>(dbCmd))
        {
            return (T)result;
        }
        return default(T);
    }

    /// <summary>
    /// Gets the reference single.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="refType">Type of the reference.</param>
    /// <returns>System.Object.</returns>
    public object GetRefSingle(IDbCommand dbCmd, Type refType)
    {
        this.Filter(dbCmd);
        if (this.RefSingleResult != null || this.RefSingleResultFn != null)
        {
            return this.GetRefSingleResult(dbCmd, refType);
        }

        foreach (var result in this.GetRefResults(dbCmd, refType).Safe())
        {
            return result;
        }
        return null;
    }

    /// <summary>
    /// Gets the scalar.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>T.</returns>
    public T GetScalar<T>(IDbCommand dbCmd)
    {
        this.Filter(dbCmd);
        return this.ConvertTo<T>(this.GetScalarResult<T>(dbCmd));
    }

    /// <summary>
    /// Gets the long scalar.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>System.Int64.</returns>
    public long GetLongScalar(IDbCommand dbCmd)
    {
        this.Filter(dbCmd);
        return this.GetLongScalarResult(dbCmd);
    }

    /// <summary>
    /// Converts to.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <returns>T.</returns>
    private T ConvertTo<T>(object value)
    {
        if (value == null)
        {
            return default(T);
        }

        if (value is T)
        {
            return (T)value;
        }

        var typeCode = typeof(T).GetUnderlyingTypeCode();
        var strValue = value.ToString();
        switch (typeCode)
        {
            case TypeCode.Boolean:
                return (T)(object)Convert.ToBoolean(strValue);
            case TypeCode.Byte:
                return (T)(object)Convert.ToByte(strValue);
            case TypeCode.Int16:
                return (T)(object)Convert.ToInt16(strValue);
            case TypeCode.Int32:
                return (T)(object)Convert.ToInt32(strValue);
            case TypeCode.Int64:
                return (T)(object)Convert.ToInt64(strValue);
            case TypeCode.Single:
                return (T)(object)Convert.ToSingle(strValue);
            case TypeCode.Double:
                return (T)(object)Convert.ToDouble(strValue);
            case TypeCode.Decimal:
                return (T)(object)Convert.ToDecimal(strValue);
        }

        return (T)value;
    }

    /// <summary>
    /// Gets the scalar.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>System.Object.</returns>
    public object GetScalar(IDbCommand dbCmd)
    {
        this.Filter(dbCmd);
        return this.GetScalarResult<object>(dbCmd) ?? this.GetResults<object>(dbCmd).Cast<object>().FirstOrDefault();
    }

    /// <summary>
    /// Gets the column.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>List&lt;T&gt;.</returns>
    public List<T> GetColumn<T>(IDbCommand dbCmd)
    {
        this.Filter(dbCmd);
        return [.. (from object result in this.GetColumnResults<T>(dbCmd).Safe() select (T)result)];
    }

    /// <summary>
    /// Gets the column distinct.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>HashSet&lt;T&gt;.</returns>
    public HashSet<T> GetColumnDistinct<T>(IDbCommand dbCmd)
    {
        this.Filter(dbCmd);
        var results = this.GetColumnDistinctResults<T>(dbCmd) ?? this.GetColumnResults<T>(dbCmd);
        return (from object result in results select (T)result).ToSet();
    }

    /// <summary>
    /// Gets the dictionary.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>Dictionary&lt;K, V&gt;.</returns>
    public Dictionary<K, V> GetDictionary<K, V>(IDbCommand dbCmd)
    {
        this.Filter(dbCmd);
        var to = new Dictionary<K, V>();
        var map = this.GetDictionaryResults<K, V>(dbCmd);
        if (map == null)
        {
            return to;
        }

        foreach (DictionaryEntry entry in map)
        {
            to.Add((K)entry.Key, (V)entry.Value);
        }

        return to;
    }

    /// <summary>
    /// Gets the key value pairs.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>List&lt;KeyValuePair&lt;K, V&gt;&gt;.</returns>
    public List<KeyValuePair<K, V>> GetKeyValuePairs<K, V>(IDbCommand dbCmd)
    {
        return [.. this.GetDictionary<K, V>(dbCmd)];
    }

    /// <summary>
    /// Gets the lookup.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>Dictionary&lt;K, List&lt;V&gt;&gt;.</returns>
    public Dictionary<K, List<V>> GetLookup<K, V>(IDbCommand dbCmd)
    {
        this.Filter(dbCmd);
        var to = new Dictionary<K, List<V>>();
        var map = this.GetLookupResults<K, V>(dbCmd);
        if (map == null)
        {
            return to;
        }

        foreach (DictionaryEntry entry in map)
        {
            var key = (K)entry.Key;

            if (!to.TryGetValue(key, out var list))
            {
                to[key] = list = [];
            }

            list.AddRange(from object item in (IEnumerable)entry.Value select (V)item);
        }

        return to;
    }

    /// <summary>
    /// Executes the SQL.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>System.Int32.</returns>
    public int ExecuteSql(IDbCommand dbCmd)
    {
        this.Filter(dbCmd);
        return this.ExecuteSqlFn?.Invoke(dbCmd)
               ?? this.ExecuteSqlResult;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        OrmLiteConfig.ResultsFilter = this.previousFilter;
    }
}

/// <summary>
/// Class CaptureSqlFilter.
/// Implements the <see cref="ServiceStack.OrmLite.OrmLiteResultsFilter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.OrmLiteResultsFilter" />
public class CaptureSqlFilter : OrmLiteResultsFilter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CaptureSqlFilter" /> class.
    /// </summary>
    public CaptureSqlFilter()
    {
        this.SqlCommandFilter = this.CaptureSqlCommand;
        this.SqlCommandHistory = [];
    }

    /// <summary>
    /// Captures the SQL command.
    /// </summary>
    /// <param name="command">The command.</param>
    private void CaptureSqlCommand(IDbCommand command)
    {
        this.SqlCommandHistory.Add(new SqlCommandDetails(command));
    }

    /// <summary>
    /// Gets or sets the SQL command history.
    /// </summary>
    /// <value>The SQL command history.</value>
    public List<SqlCommandDetails> SqlCommandHistory { get; set; }

    /// <summary>
    /// Gets the SQL statements.
    /// </summary>
    /// <value>The SQL statements.</value>
    public List<string> SqlStatements => this.SqlCommandHistory.Map(x => x.Sql);
}

/// <summary>
/// Class SqlCommandDetails.
/// </summary>
public class SqlCommandDetails
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlCommandDetails" /> class.
    /// </summary>
    /// <param name="command">The command.</param>
    public SqlCommandDetails(IDbCommand command)
    {
        if (command == null)
        {
            return;
        }

        this.Sql = command.CommandText;
        if (command.Parameters.Count <= 0)
        {
            return;
        }

        this.Parameters = [];

        foreach (IDataParameter parameter in command.Parameters)
        {
            if (!this.Parameters.ContainsKey(parameter.ParameterName))
            {
                this.Parameters.Add(parameter.ParameterName, parameter.Value);
            }
        }
    }

    /// <summary>
    /// Gets or sets the SQL.
    /// </summary>
    /// <value>The SQL.</value>
    public string Sql { get; set; }
    /// <summary>
    /// Gets or sets the parameters.
    /// </summary>
    /// <value>The parameters.</value>
    public Dictionary<string, object> Parameters { get; set; }
}