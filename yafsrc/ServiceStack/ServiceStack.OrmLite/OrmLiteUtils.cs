// ***********************************************************************
// <copyright file="OrmLiteUtils.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using ServiceStack.Logging;
using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

/// <summary>
/// Class EOT.
/// </summary>
internal class EOT { }

/// <summary>
/// Class OrmLiteUtils.
/// </summary>
public static class OrmLiteUtils
{
    /// <summary>
    /// The maximum cached index fields
    /// </summary>
    private const int maxCachedIndexFields = 10000;

    /// <summary>
    /// The index fields cache
    /// </summary>
    private readonly static Dictionary<IndexFieldsCacheKey, Tuple<FieldDefinition, int, IOrmLiteConverter>[]> indexFieldsCache
        = new(maxCachedIndexFields);

    /// <summary>
    /// The log
    /// </summary>
    static internal ILog Log = LogManager.GetLogger(typeof(OrmLiteUtils));

    /// <summary>
    /// Handles the exception.
    /// </summary>
    /// <param name="ex">The ex.</param>
    /// <param name="message">The message.</param>
    /// <param name="args">The arguments.</param>
    public static void HandleException(Exception ex, string message = null, params object[] args)
    {
        if (OrmLiteConfig.ThrowOnError)
        {
            throw ex;
        }

        Log.Error(ex, message ?? ex.Message, args);
    }

    /// <summary>
    /// Debugs the command.
    /// </summary>
    /// <param name="log">The log.</param>
    /// <param name="cmd">The command.</param>
    public static void DebugCommand(this ILog log, IDbCommand cmd)
    {
        log.Debug(GetDebugString(cmd));
    }

    /// <summary>
    /// Gets the debug string.
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <returns>string.</returns>
    public static string GetDebugString(this IDbCommand cmd)
    {
        var sb = StringBuilderCache.Allocate();

        sb.Append("SQL: ").Append(cmd.CommandText);

        if (cmd.Parameters.Count <= 0)
        {
            return StringBuilderCache.ReturnAndFree(sb);
        }

        sb.AppendLine()
            .Append("PARAMS: ");

        for (var i = 0; i < cmd.Parameters.Count; i++)
        {
            var p = (IDataParameter)cmd.Parameters[i];
            if (i > 0)
            {
                sb.Append(", ");
            }

            sb.Append($"{p.ParameterName}={p.Value}");
        }

        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// Creates the instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>T.</returns>
    public static T CreateInstance<T>()
    {
        return (T)ReflectionExtensions.CreateInstance<T>();
    }

    /// <summary>
    /// Determines whether the specified type is tuple.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>bool.</returns>
    static internal bool IsTuple(this Type type)
    {
        return type.Name.StartsWith("Tuple`", StringComparison.Ordinal);
    }

    /// <summary>
    /// Determines whether [is value tuple] [the specified type].
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>bool.</returns>
    static internal bool IsValueTuple(this Type type)
    {
        return type.Name.StartsWith("ValueTuple`", StringComparison.Ordinal);
    }

    /// <summary>
    /// Determines whether this instance is scalar.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>bool.</returns>
    public static bool IsScalar<T>()
    {
        var isScalar = typeof(T).IsValueType && !typeof(T).IsValueTuple() || typeof(T) == typeof(string);
        return isScalar;
    }

    /// <summary>
    /// Converts to.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader">The reader.</param>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <param name="onlyFields">The only fields.</param>
    /// <returns>T.</returns>
    public static T ConvertTo<T>(this IDataReader reader, IOrmLiteDialectProvider dialectProvider, HashSet<string> onlyFields = null)
    {
        using (reader)
        {
            if (reader.Read())
            {
                if (typeof(T) == typeof(List<object>))
                {
                    return (T)(object)reader.ConvertToListObjects();
                }

                if (typeof(T) == typeof(Dictionary<string, object>))
                {
                    return (T)(object)reader.ConvertToDictionaryObjects();
                }

                var values = new object[reader.FieldCount];

                if (typeof(T).IsValueTuple())
                {
                    return reader.ConvertToValueTuple<T>(values, dialectProvider);
                }

                var row = CreateInstance<T>();
                var indexCache = reader.GetIndexFieldsCache(ModelDefinition<T>.Definition, dialectProvider, onlyFields: onlyFields);
                row.PopulateWithSqlReader(dialectProvider, reader, indexCache, values);
                return row;
            }
            return default;
        }
    }

    /// <summary>
    /// Converts to list objects.
    /// </summary>
    /// <param name="dataReader">The data reader.</param>
    /// <returns>System.Collections.Generic.List&lt;object&gt;.</returns>
    public static List<object> ConvertToListObjects(this IDataReader dataReader)
    {
        var row = new List<object>();
        for (var i = 0; i < dataReader.FieldCount; i++)
        {
            var dbValue = dataReader.GetValue(i);
            row.Add(dbValue is DBNull ? null : dbValue);
        }
        return row;
    }

    /// <summary>
    /// Converts to dictionary objects.
    /// </summary>
    /// <param name="dataReader">The data reader.</param>
    /// <returns>System.Collections.Generic.Dictionary&lt;string, object&gt;.</returns>
    public static Dictionary<string, object> ConvertToDictionaryObjects(this IDataReader dataReader)
    {
        var row = new Dictionary<string, object>();
        for (var i = 0; i < dataReader.FieldCount; i++)
        {
            var dbValue = dataReader.GetValue(i);
            row[dataReader.GetName(i).Trim()] = dbValue is DBNull ? null : dbValue;
        }
        return row;
    }

    /// <summary>
    /// Converts to expando object.
    /// </summary>
    /// <param name="dataReader">The data reader.</param>
    /// <returns>System.Collections.Generic.IDictionary&lt;string, object&gt;.</returns>
    public static IDictionary<string, object> ConvertToExpandoObject(this IDataReader dataReader)
    {
        var row = (IDictionary<string, object>)new ExpandoObject();
        for (var i = 0; i < dataReader.FieldCount; i++)
        {
            var dbValue = dataReader.GetValue(i);
            row[dataReader.GetName(i).Trim()] = dbValue is DBNull ? null : dbValue;
        }
        return row;
    }

    /// <summary>
    /// Converts to value tuple.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader">The reader.</param>
    /// <param name="values">The values.</param>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <returns>T.</returns>
    public static T ConvertToValueTuple<T>(this IDataReader reader, object[] values, IOrmLiteDialectProvider dialectProvider)
    {
        var row = typeof(T).CreateInstance();
        var typeFields = TypeFields.Get(typeof(T));

        values = reader.PopulateValues(values, dialectProvider);

        for (var i = 0; i < reader.FieldCount; i++)
        {
            var itemName = "Item" + (i + 1);
            var field = typeFields.GetAccessor(itemName);
            if (field == null)
            {
                break;
            }

            var fieldType = field.FieldInfo.FieldType;
            var converter = dialectProvider.GetConverterBestMatch(fieldType);

            var dbValue = converter.GetValue(reader, i, values);
            if (dbValue == null)
            {
                continue;
            }

            if (dbValue.GetType() == fieldType)
            {
                field.PublicSetterRef(ref row, dbValue);
            }
            else
            {
                var fieldValue = converter.FromDbValue(fieldType, dbValue);
                field.PublicSetterRef(ref row, fieldValue);
            }
        }
        return (T)row;
    }

    /// <summary>
    /// Converts to list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader">The reader.</param>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <param name="onlyFields">The only fields.</param>
    /// <returns>System.Collections.Generic.List&lt;T&gt;.</returns>
    public static List<T> ConvertToList<T>(this IDataReader reader, IOrmLiteDialectProvider dialectProvider, HashSet<string> onlyFields = null)
    {
        if (typeof(T) == typeof(List<object>))
        {
            var to = new List<List<object>>();
            using (reader)
            {
                while (reader.Read())
                {
                    var row = reader.ConvertToListObjects();
                    to.Add(row);
                }
            }
            return (List<T>)(object)to;
        }
        if (typeof(T) == typeof(Dictionary<string, object>))
        {
            var to = new List<Dictionary<string, object>>();
            using (reader)
            {
                while (reader.Read())
                {
                    var row = reader.ConvertToDictionaryObjects();
                    to.Add(row);
                }
            }
            return (List<T>)(object)to;
        }
        if (typeof(T) == typeof(object))
        {
            var to = new List<object>();
            using (reader)
            {
                while (reader.Read())
                {
                    var row = reader.ConvertToExpandoObject();
                    to.Add(row);
                }
            }
            return (List<T>)(object)to.ToList();
        }
        if (typeof(T).IsValueTuple())
        {
            var to = new List<T>();
            var values = new object[reader.FieldCount];
            using (reader)
            {
                while (reader.Read())
                {
                    var row = reader.ConvertToValueTuple<T>(values, dialectProvider);
                    to.Add(row);
                }
            }
            return to;
        }
        if (typeof(T).IsTuple())
        {
            var to = new List<T>();
            using (reader)
            {
                var genericArgs = typeof(T).GetGenericArguments();
                var modelIndexCaches = reader.GetMultiIndexCaches(dialectProvider, onlyFields, genericArgs);

                var values = new object[reader.FieldCount];
                var genericTupleMi = typeof(T).GetGenericTypeDefinition().GetCachedGenericType(genericArgs);
                var activator = genericTupleMi.GetConstructor(genericArgs).GetActivator();

                while (reader.Read())
                {
                    var tupleArgs = reader.ToMultiTuple(dialectProvider, modelIndexCaches, genericArgs, values);
                    var tuple = activator([.. tupleArgs]);
                    to.Add((T)tuple);
                }
            }
            return to;
        }
        else
        {
            var to = new List<T>();
            using (reader)
            {
                var indexCache = reader.GetIndexFieldsCache(ModelDefinition<T>.Definition, dialectProvider, onlyFields: onlyFields);
                var values = new object[reader.FieldCount];
                while (reader.Read())
                {
                    var row = CreateInstance<T>();
                    row.PopulateWithSqlReader(dialectProvider, reader, indexCache, values);
                    to.Add(row);
                }
            }
            return to;
        }
    }

    /// <summary>
    /// Converts to multituple.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <param name="modelIndexCaches">The model index caches.</param>
    /// <param name="genericArgs">The generic arguments.</param>
    /// <param name="values">The values.</param>
    /// <returns>System.Collections.Generic.List&lt;object&gt;.</returns>
    static internal List<object> ToMultiTuple(this IDataReader reader,
                                              IOrmLiteDialectProvider dialectProvider,
                                              List<Tuple<FieldDefinition, int, IOrmLiteConverter>[]> modelIndexCaches,
                                              Type[] genericArgs,
                                              object[] values)
    {
        var tupleArgs = new List<object>();
        for (var i = 0; i < modelIndexCaches.Count; i++)
        {
            var indexCache = modelIndexCaches[i];
            var partialRow = genericArgs[i].CreateInstance();
            partialRow.PopulateWithSqlReader(dialectProvider, reader, indexCache, values);
            tupleArgs.Add(partialRow);
        }
        return tupleArgs;
    }

    /// <summary>
    /// Gets the multi index caches.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <param name="onlyFields">The only fields.</param>
    /// <param name="genericArgs">The generic arguments.</param>
    /// <returns>System.Collections.Generic.List&lt;System.Tuple&lt;ServiceStack.OrmLite.FieldDefinition, int, ServiceStack.OrmLite.IOrmLiteConverter&gt;[]&gt;.</returns>
    /// <exception cref="DiagnosticEvent.Exception">'{modelType.Name}' is not a table type</exception>
    static internal List<Tuple<FieldDefinition, int, IOrmLiteConverter>[]> GetMultiIndexCaches(
        this IDataReader reader,
        IOrmLiteDialectProvider dialectProvider,
        HashSet<string> onlyFields,
        Type[] genericArgs)
    {
        var modelIndexCaches = new List<Tuple<FieldDefinition, int, IOrmLiteConverter>[]>();
        var startPos = 0;

        foreach (var modelType in genericArgs)
        {
            var modelDef = modelType.GetModelDefinition();

            if (modelDef == null)
            {
                throw new Exception($"'{modelType.Name}' is not a table type");
            }

            var endPos = startPos;
            for (; endPos < reader.FieldCount; endPos++)
            {
                if (string.Equals("EOT", reader.GetName(endPos), StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }

            var noEOT = endPos == reader.FieldCount; // If no explicit EOT delimiter, split by field count
            if (genericArgs.Length > 0 && noEOT)
            {
                endPos = startPos + modelDef.FieldDefinitionsArray.Length;
            }

            var indexCache = reader.GetIndexFieldsCache(modelDef, dialectProvider, onlyFields,
                startPos: startPos, endPos: endPos);

            modelIndexCaches.Add(indexCache);

            startPos = noEOT ? endPos : endPos + 1;
        }
        return modelIndexCaches;
    }

    /// <summary>
    /// Converts to.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <param name="type">The type.</param>
    /// <returns>object.</returns>
    public static object ConvertTo(this IDataReader reader, IOrmLiteDialectProvider dialectProvider, Type type)
    {
        var modelDef = type.GetModelDefinition();
        using (reader)
        {
            if (reader.Read())
            {
                var row = type.CreateInstance();
                var indexCache = reader.GetIndexFieldsCache(modelDef, dialectProvider);
                var values = new object[reader.FieldCount];
                row.PopulateWithSqlReader(dialectProvider, reader, indexCache, values);
                return row;
            }
            return type.GetDefaultValue();
        }
    }

    /// <summary>
    /// Converts to list.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <param name="type">The type.</param>
    /// <returns>System.Collections.IList.</returns>
    public static IList ConvertToList(this IDataReader reader, IOrmLiteDialectProvider dialectProvider, Type type)
    {
        var modelDef = type.GetModelDefinition();
        var listInstance = typeof(List<>).GetCachedGenericType(type).CreateInstance();
        var to = (IList)listInstance;
        using (reader)
        {
            var indexCache = reader.GetIndexFieldsCache(modelDef, dialectProvider);
            var values = new object[reader.FieldCount];
            while (reader.Read())
            {
                var row = type.CreateInstance();
                row.PopulateWithSqlReader(dialectProvider, reader, indexCache, values);
                to.Add(row);
            }
        }
        return to;
    }

    /// <summary>
    /// Gets the column names.
    /// </summary>
    /// <param name="tableType">Type of the table.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>string.</returns>
    static internal string GetColumnNames(this Type tableType, IOrmLiteDialectProvider dialect)
    {
        return GetColumnNames(tableType.GetModelDefinition(), dialect);
    }

    /// <summary>
    /// Gets the column names.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>string.</returns>
    public static string GetColumnNames(this ModelDefinition modelDef, IOrmLiteDialectProvider dialect)
    {
        return dialect.GetColumnNames(modelDef);
    }

    /// <summary>
    /// Converts to selectstring.
    /// </summary>
    /// <typeparam name="TItem">The type of the t item.</typeparam>
    /// <param name="items">The items.</param>
    /// <returns>string.</returns>
    public static string ToSelectString<TItem>(this IEnumerable<TItem> items)
    {
        var sb = StringBuilderCache.Allocate();

        foreach (var item in items)
        {
            if (sb.Length > 0)
            {
                sb.Append(", ");
            }

            sb.Append(item);
        }

        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// Sets the ids in SQL parameters.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="idValues">The identifier values.</param>
    /// <returns>string.</returns>
    static internal string SetIdsInSqlParams(this IDbCommand dbCmd, IEnumerable idValues)
    {
        var inArgs = Sql.Flatten(idValues);
        var sbParams = StringBuilderCache.Allocate();
        foreach (var item in inArgs)
        {
            if (sbParams.Length > 0)
            {
                sbParams.Append(",");
            }

            sbParams.Append(dbCmd.AddParam(dbCmd.Parameters.Count.ToString(), item).ParameterName);
        }
        var sqlIn = StringBuilderCache.ReturnAndFree(sbParams);
        return sqlIn;
    }

    /// <summary>
    /// SQLs the FMT.
    /// </summary>
    /// <param name="sqlText">The SQL text.</param>
    /// <param name="sqlParams">The SQL parameters.</param>
    /// <returns>string.</returns>
    public static string SqlFmt(this string sqlText, params object[] sqlParams)
    {
        return SqlFmt(sqlText, OrmLiteConfig.DialectProvider, sqlParams);
    }

    /// <summary>
    /// SQLs the FMT.
    /// </summary>
    /// <param name="sqlText">The SQL text.</param>
    /// <param name="dialect">The dialect.</param>
    /// <param name="sqlParams">The SQL parameters.</param>
    /// <returns>string.</returns>
    public static string SqlFmt(this string sqlText, IOrmLiteDialectProvider dialect, params object[] sqlParams)
    {
        if (sqlParams.Length == 0)
        {
            return sqlText;
        }

        var escapedParams = new List<string>();
        foreach (var sqlParam in sqlParams)
        {
            if (sqlParam == null)
            {
                escapedParams.Add("NULL");
            }
            else
            {
                if (sqlParam is SqlInValues sqlInValues)
                {
                    escapedParams.Add(sqlInValues.ToSqlInString());
                }
                else
                {
                    escapedParams.Add(dialect.GetQuotedValue(sqlParam, sqlParam.GetType()));
                }
            }
        }
        return string.Format(sqlText, escapedParams.ToArray());
    }

    /// <summary>
    /// SQLs the column.
    /// </summary>
    /// <param name="columnName">Name of the column.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>string.</returns>
    public static string SqlColumn(this string columnName, IOrmLiteDialectProvider dialect = null)
    {
        return (dialect ?? OrmLiteConfig.DialectProvider).GetQuotedColumnName(columnName);
    }

    /// <summary>
    /// SQLs the column raw.
    /// </summary>
    /// <param name="columnName">Name of the column.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>string.</returns>
    public static string SqlColumnRaw(this string columnName, IOrmLiteDialectProvider dialect = null)
    {
        return (dialect ?? OrmLiteConfig.DialectProvider).NamingStrategy.GetColumnName(columnName);
    }

    /// <summary>
    /// SQLs the table.
    /// </summary>
    /// <param name="tableName">Name of the table.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>string.</returns>
    public static string SqlTable(this string tableName, IOrmLiteDialectProvider dialect = null)
    {
        return (dialect ?? OrmLiteConfig.DialectProvider).GetQuotedTableName(tableName);
    }

    /// <summary>
    /// SQLs the table raw.
    /// </summary>
    /// <param name="tableName">Name of the table.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>string.</returns>
    public static string SqlTableRaw(this string tableName, IOrmLiteDialectProvider dialect = null)
    {
        return (dialect ?? OrmLiteConfig.DialectProvider).NamingStrategy.GetTableName(tableName);
    }

    /// <summary>
    /// SQLs the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>string.</returns>
    public static string SqlValue(this object value)
    {
        return "{0}".SqlFmt(value);
    }

    /// <summary>
    /// The verify fragment reg ex
    /// </summary>
    public static Regex VerifyFragmentRegEx = new("([^\\w]|^)+(--|;--|;|%|/\\*|\\*/|@@|@|char|nchar|varchar|nvarchar|alter|begin|cast|create|cursor|declare|delete|drop|end|exec|execute|fetch|insert|kill|open|select|sys|sysobjects|syscolumns|table|update)([^\\w]|$)+",
        RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase,
        TimeSpan.FromMilliseconds(100));

    /// <summary>
    /// The verify SQL reg ex
    /// </summary>
    public static Regex VerifySqlRegEx = new("([^\\w]|^)+(--|;--|;|%|/\\*|\\*/|@@|@|char|nchar|varchar|nvarchar|alter|begin|cast|create|cursor|declare|delete|drop|end|exec|execute|fetch|insert|kill|open|table|update)([^\\w]|$)+",
        RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase,
        TimeSpan.FromMilliseconds(100));

    /// <summary>
    /// Gets or sets the SQL verify fragment function.
    /// </summary>
    /// <value>The SQL verify fragment function.</value>
    public static Func<string, string> SqlVerifyFragmentFn { get; set; }

    /// <summary>
    /// Determines whether [is unsafe SQL] [the specified SQL].
    /// </summary>
    /// <param name="sql">The SQL.</param>
    /// <param name="verifySql">The verify SQL.</param>
    /// <returns>bool.</returns>
    public static bool isUnsafeSql(string sql, Regex verifySql)
    {
        if (sql == null)
        {
            return false;
        }

        if (SqlVerifyFragmentFn != null)
        {
            SqlVerifyFragmentFn(sql);
            return false;
        }

        var fragmentToVerify = sql
            .StripQuotedStrings('\'')
            .StripQuotedStrings('"')
            .StripQuotedStrings('`')
            .ToLower();

        var match = verifySql.Match(fragmentToVerify);
        return match.Success;
    }

    /// <summary>
    /// SQLs the verify fragment.
    /// </summary>
    /// <param name="sqlFragment">The SQL fragment.</param>
    /// <returns>string.</returns>
    /// <exception cref="System.ArgumentException">Potential illegal fragment detected: " + sqlFragment</exception>
    public static string SqlVerifyFragment(this string sqlFragment)
    {
        if (isUnsafeSql(sqlFragment, VerifyFragmentRegEx))
        {
            throw new ArgumentException("Potential illegal fragment detected: " + sqlFragment);
        }

        return sqlFragment;
    }

    /// <summary>
    /// The illegal SQL fragment tokens
    /// </summary>
    public static string[] IllegalSqlFragmentTokens = [
        "--", ";--", ";", "%", "/*", "*/", "@@", "@",
                                                          "char", "nchar", "varchar", "nvarchar",
                                                          "alter", "begin", "cast", "create", "cursor", "declare", "delete",
                                                          "drop", "end", "exec", "execute", "fetch", "insert", "kill",
                                                          "open", "select", "sys", "sysobjects", "syscolumns", "table", "update"
    ];

    /// <summary>
    /// SQLs the verify fragment.
    /// </summary>
    /// <param name="sqlFragment">The SQL fragment.</param>
    /// <param name="illegalFragments">The illegal fragments.</param>
    /// <returns>string.</returns>
    /// <exception cref="System.ArgumentException">Potential illegal fragment detected: " + sqlFragment</exception>
    public static string SqlVerifyFragment(this string sqlFragment, IEnumerable<string> illegalFragments)
    {
        if (sqlFragment == null)
        {
            return null;
        }

        var fragmentToVerify = sqlFragment
            .StripQuotedStrings('\'')
            .StripQuotedStrings('"')
            .StripQuotedStrings('`')
            .ToLower();

        foreach (var illegalFragment in illegalFragments)
        {
            if (fragmentToVerify.IndexOf(illegalFragment, StringComparison.Ordinal) >= 0)
            {
                throw new ArgumentException("Potential illegal fragment detected: " + sqlFragment);
            }
        }

        return sqlFragment;
    }

    /// <summary>
    /// SQLs the parameter.
    /// </summary>
    /// <param name="paramValue">The parameter value.</param>
    /// <returns>string.</returns>
    public static string SqlParam(this string paramValue)
    {
        return paramValue.Replace("'", "''");
    }

    /// <summary>
    /// Strips the quoted strings.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="quote">The quote.</param>
    /// <returns>string.</returns>
    public static string StripQuotedStrings(this string text, char quote = '\'')
    {
        var sb = StringBuilderCache.Allocate();
        var inQuotes = false;
        foreach (var c in text)
        {
            if (c == quote)
            {
                inQuotes = !inQuotes;
                continue;
            }

            if (!inQuotes)
            {
                sb.Append(c);
            }
        }

        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// SQLs the join.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values">The values.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>string.</returns>
    public static string SqlJoin<T>(this List<T> values, IOrmLiteDialectProvider dialect = null)
    {
        if (values == null)
        {
            return string.Empty;
        }

        dialect ??= OrmLiteConfig.DialectProvider;

        var sb = StringBuilderCache.Allocate();
        foreach (var value in values)
        {
            if (sb.Length > 0)
            {
                sb.Append(",");
            }

            sb.Append(dialect.GetQuotedValue(value, value.GetType()));
        }

        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// SQLs the join.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>string.</returns>
    public static string SqlJoin(IEnumerable values, IOrmLiteDialectProvider dialect = null)
    {
        if (values == null)
        {
            return string.Empty;
        }

        dialect ??= OrmLiteConfig.DialectProvider;

        var sb = StringBuilderCache.Allocate();
        foreach (var value in values)
        {
            if (sb.Length > 0)
            {
                sb.Append(",");
            }

            sb.Append(value is null
                          ? "NULL"
                          : dialect.GetQuotedValue(value, value.GetType()));
        }

        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// SQLs the in values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values">The values.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>ServiceStack.OrmLite.SqlInValues.</returns>
    public static SqlInValues SqlInValues<T>(this T[] values, IOrmLiteDialectProvider dialect = null)
    {
        return new(values, dialect);
    }

    /// <summary>
    /// SQLs the in parameters.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values">The values.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>string.</returns>
    public static string SqlInParams<T>(this T[] values, IOrmLiteDialectProvider dialect = null)
    {
        var sb = StringBuilderCache.Allocate();
        if (dialect == null)
        {
            dialect = OrmLiteConfig.DialectProvider;
        }

        for (var i = 0; i < values.Length; i++)
        {
            if (sb.Length > 0)
            {
                sb.Append(',');
            }

            var paramName = dialect.ParamString + "v" + i;
            sb.Append(paramName);
        }

        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// Gets the field names.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns>System.String[].</returns>
    public static string[] GetFieldNames(this IDataReader reader)
    {
        var count = reader.FieldCount;
        var fields = new string[count];
        for (var i = 0; i < count; i++)
        {
            fields[i] = reader.GetName(i);
        }
        return fields;
    }

    /// <summary>
    /// Gets the index fields cache.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="modelDefinition">The model definition.</param>
    /// <param name="dialect">The dialect.</param>
    /// <param name="onlyFields">The only fields.</param>
    /// <param name="startPos">The start position.</param>
    /// <param name="endPos">The end position.</param>
    /// <returns>System.Tuple&lt;ServiceStack.OrmLite.FieldDefinition, int, ServiceStack.OrmLite.IOrmLiteConverter&gt;[].</returns>
    public static Tuple<FieldDefinition, int, IOrmLiteConverter>[] GetIndexFieldsCache(this IDataReader reader,
        ModelDefinition modelDefinition,
        IOrmLiteDialectProvider dialect,
        HashSet<string> onlyFields = null,
        int startPos = 0,
        int? endPos = null)
    {
        var fieldCount = reader.FieldCount;
        var sb = StringBuilderCache.Allocate();
        for (int i = 0; i < fieldCount; i++)
        {
            if (sb.Length > 0)
                sb.Append(", ");
            sb.Append(reader.GetName(i));
        }
        var fieldNames = StringBuilderCache.ReturnAndFree(sb);

        var end = endPos.GetValueOrDefault(fieldCount);
        var cacheKey = (startPos == 0 && end == fieldCount && onlyFields == null)
            ? new IndexFieldsCacheKey(fieldNames, modelDefinition, dialect)
                           : null;

        Tuple<FieldDefinition, int, IOrmLiteConverter>[] value;
        if (cacheKey != null)
        {
            lock (indexFieldsCache)
            {
                if (indexFieldsCache.TryGetValue(cacheKey, out value))
                {
                    return value;
                }
            }
        }

        var cache = new List<Tuple<FieldDefinition, int, IOrmLiteConverter>>();
        var ignoredFields = modelDefinition.IgnoredFieldDefinitions;
        var remainingFieldDefs = modelDefinition.FieldDefinitionsArray
            .Where(x => !ignoredFields.Contains(x) && x.SetValueFn != null).ToList();

        var mappedReaderColumns = new bool[end];

        for (var i = startPos; i < end; i++)
        {
            var columnName = reader.GetName(i);
            var fieldDef = modelDefinition.GetFieldDefinition(columnName);
            if (fieldDef == null)
            {
                foreach (var def in modelDefinition.FieldDefinitionsArray)
                {
                    if (string.Equals(dialect.NamingStrategy.GetColumnName(def.FieldName), columnName,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        fieldDef = def;
                        break;
                    }
                }
            }

            if (fieldDef != null && !ignoredFields.Contains(fieldDef) && fieldDef.SetValueFn != null)
            {
                remainingFieldDefs.Remove(fieldDef);
                mappedReaderColumns[i] = true;
                cache.Add(Tuple.Create(fieldDef, i, dialect.GetConverterBestMatch(fieldDef)));
            }
        }

        if (remainingFieldDefs.Count > 0 && onlyFields == null)
        {
            var dbFieldMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (var i = startPos; i < end; i++)
            {
                if (!mappedReaderColumns[i])
                {
                    var fieldName = reader.GetName(i);
                    dbFieldMap[fieldName] = i;
                }
            }

            if (dbFieldMap.Count > 0)
            {
                foreach (var fieldDef in remainingFieldDefs)
                {
                    var index = FindColumnIndex(dialect, fieldDef, dbFieldMap);
                    if (index != NotFound)
                    {
                        cache.Add(Tuple.Create(fieldDef, index, dialect.GetConverterBestMatch(fieldDef)));
                    }
                }
            }
        }

        var result = cache.ToArray();

        if (cacheKey != null)
        {
            lock (indexFieldsCache)
            {
                if (indexFieldsCache.TryGetValue(cacheKey, out value))
                {
                    return value;
                }

                if (indexFieldsCache.Count < maxCachedIndexFields)
                {
                    indexFieldsCache.Add(cacheKey, result);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// The not found
    /// </summary>
    private const int NotFound = -1;
    /// <summary>
    /// Finds the index of the column.
    /// </summary>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="dbFieldMap">The database field map.</param>
    /// <returns>int.</returns>
    static internal int FindColumnIndex(IOrmLiteDialectProvider dialectProvider, FieldDefinition fieldDef, Dictionary<string, int> dbFieldMap)
    {
        var fieldName = dialectProvider.NamingStrategy.GetColumnName(fieldDef.FieldName);
        if (dbFieldMap.TryGetValue(fieldName, out var index))
        {
            return index;
        }

        index = TryGuessColumnIndex(fieldName, dbFieldMap);
        if (index != NotFound)
        {
            return index;
        }

        // Try fallback to original field name when overriden by alias
        if (fieldDef.Alias != null && !OrmLiteConfig.DisableColumnGuessFallback)
        {
            var alias = dialectProvider.NamingStrategy.GetColumnName(fieldDef.Name);
            if (dbFieldMap.TryGetValue(alias, out index))
            {
                return index;
            }

            index = TryGuessColumnIndex(alias, dbFieldMap);
            if (index != NotFound)
            {
                return index;
            }
        }

        return NotFound;
    }

    /// <summary>
    /// The allowed property chars regex
    /// </summary>
    private readonly static Regex AllowedPropertyCharsRegex = new(@"[^0-9a-zA-Z_]",
        RegexOptions.Compiled | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(100));

    /// <summary>
    /// Tries the index of the guess column.
    /// </summary>
    /// <param name="fieldName">Name of the field.</param>
    /// <param name="dbFieldMap">The database field map.</param>
    /// <returns>int.</returns>
    private static int TryGuessColumnIndex(string fieldName, Dictionary<string, int> dbFieldMap)
    {
        if (OrmLiteConfig.DisableColumnGuessFallback)
        {
            return NotFound;
        }

        foreach (var entry in dbFieldMap)
        {
            var dbFieldName = entry.Key;
            var i = entry.Value;

            // First guess: Maybe the DB field has underscores? (most common)
            // e.g. CustomerId (C#) vs customer_id (DB)
            var dbFieldNameWithNoUnderscores = dbFieldName.Replace("_", "");
            if (string.Compare(fieldName, dbFieldNameWithNoUnderscores, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return i;
            }

            // Next guess: Maybe the DB field has special characters?
            // e.g. Quantity (C#) vs quantity% (DB)
            var dbFieldNameSanitized = AllowedPropertyCharsRegex.Replace(dbFieldName, string.Empty);
            if (string.Compare(fieldName, dbFieldNameSanitized, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return i;
            }

            // Next guess: Maybe the DB field has special characters *and* has underscores?
            // e.g. Quantity (C#) vs quantity_% (DB)
            if (string.Compare(fieldName, dbFieldNameSanitized.Replace("_", string.Empty), StringComparison.OrdinalIgnoreCase) == 0)
            {
                return i;
            }

            // Next guess: Maybe the DB field has some prefix that we don't have in our C# field?
            // e.g. CustomerId (C#) vs t130CustomerId (DB)
            if (dbFieldName.EndsWith(fieldName, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }

            // Next guess: Maybe the DB field has some prefix that we don't have in our C# field *and* has underscores?
            // e.g. CustomerId (C#) vs t130_CustomerId (DB)
            if (dbFieldNameWithNoUnderscores.EndsWith(fieldName, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }

            // Next guess: Maybe the DB field has some prefix that we don't have in our C# field *and* has special characters?
            // e.g. CustomerId (C#) vs t130#CustomerId (DB)
            if (dbFieldNameSanitized.EndsWith(fieldName, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }

            // Next guess: Maybe the DB field has some prefix that we don't have in our C# field *and* has underscores *and* has special characters?
            // e.g. CustomerId (C#) vs t130#Customer_I#d (DB)
            if (dbFieldNameSanitized.Replace("_", "").EndsWith(fieldName, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }

            // Cater for Naming Strategies like PostgreSQL that has lower_underscore names
            if (dbFieldNameSanitized.Replace("_", "").EndsWith(fieldName.Replace("_", ""), StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        return NotFound;
    }

    /// <summary>
    /// Determines whether [is reference type] [the specified field type].
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <returns>bool.</returns>
    public static bool IsRefType(this Type fieldType)
    {
        return (!fieldType.UnderlyingSystemType.IsValueType
                || JsConfig.TreatValueAsRefTypes.Contains(fieldType.IsGenericType
                                                              ? fieldType.GetGenericTypeDefinition()
                                                              : fieldType))
               && fieldType != typeof(string);
    }

    /// <summary>
    /// Strips the table prefixes.
    /// </summary>
    /// <param name="selectExpression">The select expression.</param>
    /// <returns>string.</returns>
    public static string StripTablePrefixes(this string selectExpression)
    {
        if (selectExpression.IndexOf('.') < 0)
        {
            return selectExpression;
        }

        var sb = StringBuilderCache.Allocate();
        var tokens = selectExpression.Split(' ');
        foreach (var token in tokens)
        {
            var parts = token.SplitOnLast('.');
            if (parts.Length > 1)
            {
                sb.Append(" " + parts[parts.Length - 1]);
            }
            else
            {
                sb.Append(" " + token);
            }
        }

        return StringBuilderCache.ReturnAndFree(sb).Trim();
    }

    /// <summary>
    /// The quoted chars
    /// </summary>
    private readonly static char[] QuotedChars = ['"', '`', '[', ']'];

    /// <summary>
    /// Aliases the or column.
    /// </summary>
    /// <param name="quotedExpr">The quoted expr.</param>
    /// <returns>string.</returns>
    public static string AliasOrColumn(this string quotedExpr)
    {
        var ret = quotedExpr.LastRightPart(" AS ").Trim();
        return ret;
    }

    /// <summary>
    /// Strips the database quotes.
    /// </summary>
    /// <param name="quotedExpr">The quoted expr.</param>
    /// <returns>string.</returns>
    public static string StripDbQuotes(this string quotedExpr)
    {
        return quotedExpr.Trim(QuotedChars);
    }

    /// <summary>
    /// Prints the SQL.
    /// </summary>
    public static void PrintSql()
    {
        OrmLiteConfig.BeforeExecFilter = cmd => Console.WriteLine(cmd.GetDebugString());
    }

    /// <summary>
    /// Uns the print SQL.
    /// </summary>
    public static void UnPrintSql()
    {
        OrmLiteConfig.BeforeExecFilter = null;
    }

    /// <summary>
    /// Captures the SQL.
    /// </summary>
    /// <returns>System.Text.StringBuilder.</returns>
    public static StringBuilder CaptureSql()
    {
        var sb = StringBuilderCache.Allocate();
        CaptureSql(sb);
        return sb;
    }

    /// <summary>
    /// Captures the SQL.
    /// </summary>
    /// <param name="sb">The sb.</param>
    public static void CaptureSql(StringBuilder sb)
    {
        OrmLiteConfig.BeforeExecFilter = cmd => sb.AppendLine(cmd.GetDebugString());
    }

    /// <summary>
    /// Uns the capture SQL.
    /// </summary>
    public static void UnCaptureSql()
    {
        OrmLiteConfig.BeforeExecFilter = null;
    }

    /// <summary>
    /// Uns the capture SQL and free.
    /// </summary>
    /// <param name="sb">The sb.</param>
    /// <returns>string.</returns>
    public static string UnCaptureSqlAndFree(StringBuilder sb)
    {
        OrmLiteConfig.BeforeExecFilter = null;
        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// Gets the model definition.
    /// </summary>
    /// <param name="modelType">Type of the model.</param>
    /// <returns>ServiceStack.OrmLite.ModelDefinition.</returns>
    public static ModelDefinition GetModelDefinition(Type modelType)
    {
        return modelType.GetModelDefinition();
    }

    /// <summary>
    /// Converts to u long.
    /// </summary>
    /// <param name="bytes">The bytes.</param>
    /// <returns>ulong.</returns>
    public static ulong ConvertToULong(byte[] bytes)
    {
        Array.Reverse(bytes); //Correct Endianness
        var ulongValue = BitConverter.ToUInt64(bytes, 0);
        return ulongValue;
    }

    /// <summary>
    /// Merges the specified parent.
    /// </summary>
    /// <typeparam name="Parent">The type of the parent.</typeparam>
    /// <typeparam name="Child">The type of the child.</typeparam>
    /// <param name="parent">The parent.</param>
    /// <param name="children">The children.</param>
    /// <returns>System.Collections.Generic.List&lt;Parent&gt;.</returns>
    public static List<Parent> Merge<Parent, Child>(this Parent parent, List<Child> children)
    {
        return new List<Parent> { parent }.Merge(children);
    }

    /// <summary>
    /// Merges the specified parents.
    /// </summary>
    /// <typeparam name="Parent">The type of the parent.</typeparam>
    /// <typeparam name="Child">The type of the child.</typeparam>
    /// <param name="parents">The parents.</param>
    /// <param name="children">The children.</param>
    /// <returns>System.Collections.Generic.List&lt;Parent&gt;.</returns>
    /// <exception cref="DiagnosticEvent.Exception">Could not find Child Reference for '{typeof(Child).Name}' on Parent '{typeof(Parent).Name}'</exception>
    public static List<Parent> Merge<Parent, Child>(this List<Parent> parents, List<Child> children)
    {
        var modelDef = ModelDefinition<Parent>.Definition;

        var hasChildRef = false;

        foreach (var fieldDef in modelDef.ReferenceFieldDefinitionsArray)
        {
            if (fieldDef.FieldType != typeof(Child) && fieldDef.FieldType != typeof(List<Child>))
            {
                continue;
            }

            hasChildRef = true;

            var listInterface = fieldDef.FieldType.GetTypeWithGenericInterfaceOf(typeof(IList<>));
            if (listInterface != null)
            {
                var refType = listInterface.GetGenericArguments()[0];
                var refModelDef = refType.GetModelDefinition();
                var refField = modelDef.GetRefFieldDef(refModelDef, refType);

                SetListChildResults(parents, modelDef, fieldDef, refType, children, refField);
            }
            else
            {
                var refType = fieldDef.FieldType;

                var refModelDef = refType.GetModelDefinition();

                var refSelf = modelDef.GetSelfRefFieldDefIfExists(refModelDef, fieldDef);
                var refField = refSelf == null
                                   ? modelDef.GetRefFieldDef(refModelDef, refType)
                                   : modelDef.GetRefFieldDefIfExists(refModelDef);

                if (refSelf != null)
                {
                    SetRefSelfChildResults(parents, fieldDef, refModelDef, refSelf, children);
                }
                else if (refField != null)
                {
                    SetRefFieldChildResults(parents, modelDef, fieldDef, refField, children);
                }
            }
        }

        if (!hasChildRef)
        {
            throw new Exception($"Could not find Child Reference for '{typeof(Child).Name}' on Parent '{typeof(Parent).Name}'");
        }

        return parents;
    }

    /// <summary>
    /// Sets the list child results.
    /// </summary>
    /// <typeparam name="Parent">The type of the parent.</typeparam>
    /// <param name="parents">The parents.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="refType">Type of the reference.</param>
    /// <param name="childResults">The child results.</param>
    /// <param name="refField">The reference field.</param>
    static internal void SetListChildResults<Parent>(List<Parent> parents, ModelDefinition modelDef,
                                                     FieldDefinition fieldDef, Type refType, IList childResults, FieldDefinition refField)
    {
        var map = new Dictionary<object, List<object>>();
        List<object> refValues;

        foreach (var result in childResults)
        {
            var refValue = refField.GetValue(result);
            if (!map.TryGetValue(refValue, out refValues))
            {
                map[refValue] = refValues = [];
            }
            refValues.Add(result);
        }

        var untypedApi = refType.CreateTypedApi();
        foreach (var result in parents)
        {
            var pkValue = modelDef.PrimaryKey.GetValue(result);
            if (map.TryGetValue(pkValue, out refValues))
            {
                var castResults = untypedApi.Cast(refValues);
                fieldDef.SetValue(result, castResults);
            }
        }
    }

    /// <summary>
    /// Sets the reference self child results.
    /// </summary>
    /// <typeparam name="Parent">The type of the parent.</typeparam>
    /// <param name="parents">The parents.</param>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="refModelDef">The reference model definition.</param>
    /// <param name="refSelf">The reference self.</param>
    /// <param name="childResults">The child results.</param>
    static internal void SetRefSelfChildResults<Parent>(List<Parent> parents, FieldDefinition fieldDef, ModelDefinition refModelDef, FieldDefinition refSelf, IList childResults)
    {
        var map = new Dictionary<object, object>();
        foreach (var result in childResults)
        {
            var pkValue = refModelDef.PrimaryKey.GetValue(result);
            map[pkValue] = result;
        }

        foreach (var result in parents)
        {
            var fkValue = refSelf.GetValue(result);
            if (fkValue != null && map.TryGetValue(fkValue, out var childResult))
            {
                fieldDef.SetValue(result, childResult);
            }
        }
    }

    /// <summary>
    /// Sets the reference field child results.
    /// </summary>
    /// <typeparam name="Parent">The type of the parent.</typeparam>
    /// <param name="parents">The parents.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="refField">The reference field.</param>
    /// <param name="childResults">The child results.</param>
    static internal void SetRefFieldChildResults<Parent>(List<Parent> parents, ModelDefinition modelDef,
                                                         FieldDefinition fieldDef, FieldDefinition refField, IList childResults)
    {
        var map = new Dictionary<object, object>();

        foreach (var result in childResults)
        {
            var refValue = refField.GetValue(result);
            map[refValue] = result;
        }

        foreach (var result in parents)
        {
            var pkValue = modelDef.PrimaryKey.GetValue(result);
            if (map.TryGetValue(pkValue, out var childResult))
            {
                fieldDef.SetValue(result, childResult);
            }
        }
    }

    /// <summary>
    /// Asserts the type of the not anon.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void AssertNotAnonType<T>()
    {
        if (typeof(T) == typeof(object))
        {
            throw new ArgumentException("T generic argument should be a Table but was typeof(object)");
        }

        if (typeof(T) == typeof(Dictionary<string, object>))
        {
            throw new ArgumentException("T generic argument should be a Table but was typeof(Dictionary<string,object>)");
        }

        if (typeof(ISqlExpression).IsAssignableFrom(typeof(T)))
        {
            throw new ArgumentException("T generic argument should be a Table but was an ISqlExpression");
        }
    }

    /// <summary>
    /// Gets the non default value insert fields.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <param name="obj">The object.</param>
    /// <returns>System.Collections.Generic.List&lt;string&gt;.</returns>
    public static List<string> GetNonDefaultValueInsertFields<T>(this IOrmLiteDialectProvider dialectProvider, object obj)
    {
        AssertNotAnonType<T>();

        var insertFields = new List<string>();
        var modelDef = typeof(T).GetModelDefinition();
        foreach (var fieldDef in modelDef.FieldDefinitionsArray)
        {
            if (!string.IsNullOrEmpty(dialectProvider.GetDefaultValue(fieldDef)))
            {
                var value = fieldDef.GetValue(obj);
                if (value == null/* || value.Equals(fieldDef.FieldTypeDefaultValue)*/)
                {
                    continue;
                }
            }
            insertFields.Add(fieldDef.Name);
        }

        return insertFields.Count == modelDef.FieldDefinitionsArray.Length
                   ? null
                   : insertFields;
    }

    /// <summary>
    /// Parses the tokens.
    /// </summary>
    /// <param name="expr">The expr.</param>
    /// <returns>System.Collections.Generic.List&lt;string&gt;.</returns>
    public static List<string> ParseTokens(this string expr)
    {
        var to = new List<string>();

        if (string.IsNullOrEmpty(expr))
        {
            return to;
        }

        var inDoubleQuotes = false;
        var inSingleQuotes = false;
        var inBracesCount = 0;

        var pos = 0;

        for (var i = 0; i < expr.Length; i++)
        {
            var c = expr[i];
            if (inDoubleQuotes)
            {
                if (c == '"')
                {
                    inDoubleQuotes = false;
                }

                continue;
            }
            if (inSingleQuotes)
            {
                if (c == '\'')
                {
                    inSingleQuotes = false;
                }

                continue;
            }
            if (c == '"')
            {
                inDoubleQuotes = true;
                continue;
            }
            if (c == '\'')
            {
                inSingleQuotes = true;
                continue;
            }
            if (c == '(')
            {
                inBracesCount++;
                continue;
            }
            if (c == ')')
            {
                inBracesCount--;
                if (inBracesCount > 0)
                {
                    continue;
                }

                var endPos = expr.IndexOf(',', i);
                if (endPos == -1)
                {
                    endPos = expr.Length;
                }

                var arg = expr.Substring(pos, endPos - pos).Trim();
                if (!string.IsNullOrEmpty(arg))
                {
                    to.Add(arg);
                }

                pos = endPos;
                continue;
            }

            if (c == ',')
            {
                if (inBracesCount == 0)
                {
                    var arg = expr.Substring(pos, i - pos).Trim();
                    if (!string.IsNullOrEmpty(arg))
                    {
                        to.Add(arg);
                    }

                    pos = i + 1;
                }
            }
        }

        var remaining = expr.Substring(pos, expr.Length - pos);
        if (!string.IsNullOrEmpty(remaining))
        {
            remaining = remaining.Trim();
        }

        if (!string.IsNullOrEmpty(remaining))
        {
            to.Add(remaining);
        }

        return to;
    }

    /// <summary>
    /// Alls the anon fields.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>string[].</returns>
    public static string[] AllAnonFields(this Type type)
    {
        return [.. type.GetPublicProperties().Select(x => x.Name)];
    }

    /// <summary>
    /// Evals the factory function.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="expr">The expr.</param>
    /// <returns>T.</returns>
    public static T EvalFactoryFn<T>(this Expression<Func<T>> expr)
    {
        var factoryFn = (Func<T>)CachedExpressionCompiler.Evaluate(expr);
        var model = factoryFn();
        return model;
    }

    /// <summary>
    /// Joins the alias.
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <returns>ServiceStack.OrmLite.JoinFormatDelegate.</returns>
    public static JoinFormatDelegate JoinAlias(string alias)
    {
        return (dialect, tableDef, expr) =>
            $"{dialect.GetQuotedTableName(tableDef)} {alias} {expr.Replace(dialect.GetQuotedTableName(tableDef), dialect.GetQuotedTableName(alias))}";
    }

    /// <summary>
    /// RDBMS Quoted string 'literal'
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>string.</returns>
    public static string QuotedLiteral(string text)
    {
        return text == null || text.IndexOf('\'') >= 0
            ? text
            : "'" + text + "'";
    }

    /// <summary>
    /// Unquoteds the name of the column.
    /// </summary>
    /// <param name="columnExpr">The column expr.</param>
    /// <returns>string.</returns>
    public static string UnquotedColumnName(string columnExpr)
    {
        return columnExpr.LastRightPart('.').StripDbQuotes();
    }

    /// <summary>
    /// Orders the by fields.
    /// </summary>
    /// <param name="dialect">The dialect.</param>
    /// <param name="orderBy">The order by.</param>
    /// <returns>string.</returns>
    public static string OrderByFields(IOrmLiteDialectProvider dialect, string orderBy)
    {
        if (string.IsNullOrEmpty(orderBy))
        {
            return string.Empty;
        }

        var sb = StringBuilderCache.Allocate();

        var fields = orderBy.Split(',');
        const string Asc = "";
        const string Desc = " DESC";

        var orderBySuffix = Asc;
        foreach (var fieldName in fields)
        {
            if (sb.Length > 0)
            {
                sb.Append(", ");
            }

            var reverse = fieldName.StartsWith("-");
            var useSuffix = reverse
                                ? orderBySuffix == Asc ? Desc : Asc
                                : orderBySuffix;
            var useName = reverse ? fieldName.Substring(1) : fieldName;
            var quotedName = dialect.GetQuotedColumnName(useName);

            sb.Append(quotedName + useSuffix);
        }
        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// The regex password
    /// </summary>
    public static Regex RegexPassword = new(
        "(Password|Pwd)=([^;,]+(,\\d+)?)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase,
        TimeSpan.FromMilliseconds(100));

    /// <summary>
    /// Masks the password.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <returns>System.String.</returns>
    public static string MaskPassword(string connectionString)
    {
        return connectionString != null
            ? RegexPassword.Replace(connectionString, "$1=***")
            : null;
    }
}