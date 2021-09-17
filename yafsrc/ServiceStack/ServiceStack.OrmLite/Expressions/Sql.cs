// ***********************************************************************
// <copyright file="Sql.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Class SQL.
    /// </summary>
    public static partial class Sql
    {
        /// <summary>
        /// The varchar
        /// </summary>
        public static string VARCHAR = nameof(VARCHAR);

        /// <summary>
        /// Flattens the specified list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns>List&lt;System.Object&gt;.</returns>
        public static List<object> Flatten(IEnumerable list)
        {
            var ret = new List<object>();
            if (list == null) return ret;

            foreach (var item in list)
            {
                if (item == null) continue;

                if (item is IEnumerable arr && item is not string)
                {
                    ret.AddRange(arr.Cast<object>());
                }
                else
                {
                    ret.Add(item);
                }
            }
            return ret;
        }

        /// <summary>
        /// Ins the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TItem">The type of the t item.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="list">The list.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool In<T, TItem>(T value, params TItem[] list) => value != null && Flatten(list).Any(obj => obj.ToString() == value.ToString());

        /// <summary>
        /// Ins the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TItem">The type of the t item.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="query">The query.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool In<T, TItem>(T value, SqlExpression<TItem> query) => value != null && query != null;

        /// <summary>
        /// Ascs the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string Asc<T>(T value) => value == null ? "" : value + " ASC";

        /// <summary>
        /// Descs the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string Desc<T>(T value) => value == null ? "" : value + " DESC";

        /// <summary>
        /// Ases the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="asValue">As value.</param>
        /// <returns>System.String.</returns>
        public static string As<T>(T value, object asValue) => value == null ? "" : $"{value} AS {asValue}";

        /// <summary>
        /// Sums the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        public static T Sum<T>(T value) => value;

        /// <summary>
        /// Sums the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string Sum(string value) => $"SUM({value})";

        /// <summary>
        /// Counts the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        public static T Count<T>(T value) => value;

        /// <summary>
        /// Counts the distinct.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        public static T CountDistinct<T>(T value) => value;

        /// <summary>
        /// Counts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string Count(string value) => $"COUNT({value})";

        /// <summary>
        /// Determines the minimum of the parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        public static T Min<T>(T value) => value;

        /// <summary>
        /// Determines the minimum of the parameters.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string Min(string value) => $"MIN({value})";

        /// <summary>
        /// Determines the maximum of the parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        public static T Max<T>(T value) => value;

        /// <summary>
        /// Determines the maximum of the parameters.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string Max(string value) => $"MAX({value})";

        /// <summary>
        /// Averages the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        public static T Avg<T>(T value) => value;

        /// <summary>
        /// Averages the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string Avg(string value) => $"AVG({value})";

        /// <summary>
        /// Alls the fields.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <returns>T.</returns>
        public static T AllFields<T>(T item) => item;

        /// <summary>
        /// Joins the alias.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="tableAlias">The table alias.</param>
        /// <returns>System.String.</returns>
        [Obsolete("Use TableAlias")]
        public static string JoinAlias(string property, string tableAlias) => tableAlias;

        /// <summary>
        /// Tables the alias.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="tableAlias">The table alias.</param>
        /// <returns>System.String.</returns>
        public static string TableAlias(string property, string tableAlias) => tableAlias;

        /// <summary>
        /// Joins the alias.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">The property.</param>
        /// <param name="tableAlias">The table alias.</param>
        /// <returns>T.</returns>
        [Obsolete("Use TableAlias")]
        public static T JoinAlias<T>(T property, string tableAlias) => default;

        /// <summary>
        /// Tables the alias.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">The property.</param>
        /// <param name="tableAlias">The table alias.</param>
        /// <returns>T.</returns>
        public static T TableAlias<T>(T property, string tableAlias) => default;

        /// <summary>
        /// Customs the specified custom SQL.
        /// </summary>
        /// <param name="customSql">The custom SQL.</param>
        /// <returns>System.String.</returns>
        public static string Custom(string customSql) => customSql;

        /// <summary>
        /// Customs the specified custom SQL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="customSql">The custom SQL.</param>
        /// <returns>T.</returns>
        public static T Custom<T>(string customSql) => default;

        /// <summary>
        /// Casts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="castAs">The cast as.</param>
        /// <returns>System.String.</returns>
        public static string Cast(object value, string castAs) => $"CAST({value} AS {castAs})";

        /// <summary>
        /// The eot
        /// </summary>
        public const string EOT = "0 EOT";
    }

    /// <summary>
    /// SQL Server 2016 specific features
    /// </summary>
    public static partial class Sql
    {
        /// <summary>Tests whether a string contains valid JSON.</summary>
        /// <param name="expression">The string to test.</param>
        /// <returns>Returns True if the string contains valid JSON; otherwise, returns False. Returns null if expression is null.</returns>
        /// <remarks>ISJSON does not check the uniqueness of keys at the same level.</remarks>
        /// <see href="https://docs.microsoft.com/en-us/sql/t-sql/functions/isjson-transact-sql"/>
        public static bool? IsJson(string expression) => null;

        /// <summary>Extracts a scalar value from a JSON string.</summary>
        /// <param name="expression">
        /// An expression. Typically the name of a variable or a column that contains JSON text.<br/><br/>
        /// If <b>JSON_VALUE</b> finds JSON that is not valid in expression before it finds the value identified by <i>path</i>, the function returns an error. If <b>JSON_VALUE</b> doesn't find the value identified by <i>path</i>, it scans the entire text and returns an error if it finds JSON that is not valid anywhere in <i>expression</i>.
        /// </param>
        /// <param name="path">
        /// A JSON path that specifies the property to extract. For more info, see <see href="https://docs.microsoft.com/en-us/sql/relational-databases/json/json-path-expressions-sql-server">JSON Path Expressions (SQL Server)</see>.<br/><br/>
        /// In SQL Server 2017 and in Azure SQL Database, you can provide a variable as the value of <i>path</i>.<br/><br/>
        /// If the format of path isn't valid, <b>JSON_VALUE</b> returns an error.<br/><br/>
        /// </param>
        /// <returns>
        /// Returns a single text value of type nvarchar(4000). The collation of the returned value is the same as the collation of the input expression.
        /// If the value is greater than 4000 characters: <br/><br/>
        /// <ul>
        /// <li>In lax mode, <b>JSON_VALUE</b> returns null.</li>
        /// <li>In strict mode, <b>JSON_VALUE</b> returns an error.</li>
        /// </ul>
        /// <br/>
        /// If you have to return scalar values greater than 4000 characters, use <b>OPENJSON</b> instead of <b>JSON_VALUE</b>. For more info, see <see href="https://docs.microsoft.com/en-us/sql/t-sql/functions/openjson-transact-sql">OPENJSON (Transact-SQL)</see>.
        /// </returns>
        /// <see href="https://docs.microsoft.com/en-us/sql/t-sql/functions/json-value-transact-sql"/>
        public static T JsonValue<T>(string expression, string path) => default;

        /// <summary>Extracts a scalar value from a JSON string.</summary>
        /// <param name="expression">
        /// An expression. Typically the name of a variable or a column that contains JSON text.<br/><br/>
        /// If <b>JSON_VALUE</b> finds JSON that is not valid in expression before it finds the value identified by <i>path</i>, the function returns an error. If <b>JSON_VALUE</b> doesn't find the value identified by <i>path</i>, it scans the entire text and returns an error if it finds JSON that is not valid anywhere in <i>expression</i>.
        /// </param>
        /// <param name="path">
        /// A JSON path that specifies the property to extract. For more info, see <see href="https://docs.microsoft.com/en-us/sql/relational-databases/json/json-path-expressions-sql-server">JSON Path Expressions (SQL Server)</see>.<br/><br/>
        /// In SQL Server 2017 and in Azure SQL Database, you can provide a variable as the value of <i>path</i>.<br/><br/>
        /// If the format of path isn't valid, <b>JSON_VALUE</b> returns an error.<br/><br/>
        /// </param>
        /// <returns>
        /// Returns a single text value of type nvarchar(4000). The collation of the returned value is the same as the collation of the input expression.
        /// If the value is greater than 4000 characters: <br/><br/>
        /// <ul>
        /// <li>In lax mode, <b>JSON_VALUE</b> returns null.</li>
        /// <li>In strict mode, <b>JSON_VALUE</b> returns an error.</li>
        /// </ul>
        /// <br/>
        /// If you have to return scalar values greater than 4000 characters, use <b>OPENJSON</b> instead of <b>JSON_VALUE</b>. For more info, see <see href="https://docs.microsoft.com/en-us/sql/t-sql/functions/openjson-transact-sql">OPENJSON (Transact-SQL)</see>.
        /// </returns>
        /// <see href="https://docs.microsoft.com/en-us/sql/t-sql/functions/json-value-transact-sql"/>
        public static string JsonValue(string expression, string path) => string.Empty;


        /// <summary>
        /// Extracts an object or an array from a JSON string.<br/><br/>
        /// To extract a scalar value from a JSON string instead of an object or an array, see <see href="https://docs.microsoft.com/en-us/sql/t-sql/functions/json-value-transact-sql">JSON_VALUE(Transact-SQL)</see>.
        /// For info about the differences between <b>JSON_VALUE</b> and <b>JSON_QUERY</b>, see <see href="https://docs.microsoft.com/en-us/sql/relational-databases/json/validate-query-and-change-json-data-with-built-in-functions-sql-server#JSONCompare">Compare JSON_VALUE and JSON_QUERY</see>.
        /// </summary>
        /// <param name="expression">
        /// An expression. Typically the name of a variable or a column that contains JSON text.<br/><br/>
        /// If <b>JSON_QUERY</b> finds JSON that is not valid in <i>expression</i> before it finds the value identified by <i>path</i>, the function returns an error. If <b>JSON_QUERY</b> doesn't find the value identified by <i>path</i>, it scans the entire text and returns an error if it finds JSON that is not valid anywhere in <i>expression</i>.
        /// </param>
        /// A JSON path that specifies the object or the array to extract.<br/><br/>
        /// In SQL Server 2017 and in Azure SQL Database, you can provide a variable as the value of <i>path</i>.<br/><br/>
        /// The JSON path can specify lax or strict mode for parsing.If you don't specify the parsing mode, lax mode is the default. For more info, see <see href="https://docs.microsoft.com/en-us/sql/relational-databases/json/json-path-expressions-sql-server">JSON Path Expressions (SQL Server)</see>.<br/><br/>
        /// The default value for path is '$'. As a result, if you don't provide a value for path, <b>JSON_QUERY</b> returns the input <i>expression</i>.<br/><br/>
        /// If the format of <i>path</i> isn't valid, <b>JSON_QUERY</b> returns an error.
        /// <returns>
        /// Returns a JSON fragment of type T. The collation of the returned value is the same as the collation of the input expression.<br/><br/>
        /// If the value is not an object or an array:
        /// <ul>
        /// <li>In lax mode, <b>JSON_QUERY</b> returns null.</li>
        /// <li>In strict mode, <b>JSON_QUERY</b> returns an error.</li>
        /// </ul>
        /// </returns>
        public static string JsonQuery(string expression) => string.Empty;

        /// <summary>
        /// Jsons the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>T.</returns>
        public static T JsonQuery<T>(string expression) => default;

        /// <summary>
        /// SQL Server 2017+
        /// Jsons the query.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="path">The path.</param>
        /// <returns>System.String.</returns>
        public static string JsonQuery(string expression, string path) => string.Empty;

        /// <summary>
        ///  SQL Server 2017+
        /// Jsons the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="path">The path.</param>
        /// <returns>T.</returns>
        public static T JsonQuery<T>(string expression, string path) => default;
    }
}

