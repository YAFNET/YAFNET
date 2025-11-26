// ***********************************************************************
// <copyright file="SqlExpression.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using ServiceStack.OrmLite.Base.Common;
using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Class SqlExpression.
/// Implements the <see cref="ServiceStack.OrmLite.ISqlExpression" />
/// Implements the <see cref="ServiceStack.OrmLite.IHasUntypedSqlExpression" />
/// Implements the <see cref="ServiceStack.OrmLite.IHasDialectProvider" />
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="ServiceStack.OrmLite.ISqlExpression" />
/// <seealso cref="ServiceStack.OrmLite.IHasUntypedSqlExpression" />
/// <seealso cref="ServiceStack.OrmLite.IHasDialectProvider" />
public abstract partial class SqlExpression<T> : IHasUntypedSqlExpression, IHasDialectProvider
{
    /// <summary>
    /// The true literal
    /// </summary>
    public const string TrueLiteral = "(1=1)";
    /// <summary>
    /// The false literal
    /// </summary>
    public const string FalseLiteral = "(1=0)";

    /// <summary>
    /// The underlying expression
    /// </summary>
    private Expression<Func<T, bool>> underlyingExpression;
    /// <summary>
    /// The order by properties
    /// </summary>
    private List<string> orderByProperties = [];
    /// <summary>
    /// The select expression
    /// </summary>
    private string selectExpression = string.Empty;
    /// <summary>
    /// From expression
    /// </summary>
    private string fromExpression;

    /// <summary>
    /// The order by
    /// </summary>
    private string orderBy = string.Empty;
    /// <summary>
    /// Gets or sets the only fields.
    /// </summary>
    /// <value>The only fields.</value>
    public HashSet<string> OnlyFields { get; protected set; }

    /// <summary>
    /// Gets or sets the update fields.
    /// </summary>
    /// <value>The update fields.</value>
    public List<string> UpdateFields { get; set; }
    /// <summary>
    /// Gets or sets the insert fields.
    /// </summary>
    /// <value>The insert fields.</value>
    public List<string> InsertFields { get; set; }

    /// <summary>
    /// The model definition
    /// </summary>
    protected ModelDefinition modelDef;
    /// <summary>
    /// Gets or sets the table alias.
    /// </summary>
    /// <value>The table alias.</value>
    public string TableAlias { get; set; }
    /// <summary>
    /// Gets or sets the dialect provider.
    /// </summary>
    /// <value>The dialect provider.</value>
    public IOrmLiteDialectProvider DialectProvider { get; set; }
    /// <summary>
    /// Gets or sets the parameters.
    /// </summary>
    /// <value>The parameters.</value>
    public List<IDbDataParameter> Params { get; set; }
    /// <summary>
    /// Gets or sets the SQL filter.
    /// </summary>
    /// <value>The SQL filter.</value>
    public Func<string, string> SqlFilter { get; set; }
    /// <summary>
    /// Gets or sets the select filter.
    /// </summary>
    /// <value>The select filter.</value>
    public static Action<SqlExpression<T>> SelectFilter { get; set; }
    /// <summary>
    /// Gets or sets the rows.
    /// </summary>
    /// <value>The rows.</value>
    public int? Rows { get; set; }
    /// <summary>
    /// Gets or sets the offset.
    /// </summary>
    /// <value>The offset.</value>
    public int? Offset { get; set; }
    /// <summary>
    /// Gets or sets the name of the prefix field with table.
    /// </summary>
    /// <value>The name of the prefix field with table.</value>
    public bool PrefixFieldWithTableName { get; set; }
    /// <summary>
    /// Gets or sets the use select properties as aliases.
    /// </summary>
    /// <value>The use select properties as aliases.</value>
    public bool UseSelectPropertiesAsAliases { get; set; }

    /// <summary>
    /// Gets or sets the use join type as aliases.
    /// </summary>
    /// <value>The use join type as aliases.</value>
    public bool UseJoinTypeAsAliases { get; set; }

    /// <summary>
    /// Gets or sets the where statement without where string.
    /// </summary>
    /// <value>The where statement without where string.</value>
    public bool WhereStatementWithoutWhereString { get; set; }

    /// <summary>
    /// Gets the tags.
    /// </summary>
    /// <value>The tags.</value>
    public ISet<string> Tags { get; } = new HashSet<string>();
    /// <summary>
    /// Gets or sets the allow escape wildcards.
    /// </summary>
    /// <value>The allow escape wildcards.</value>
    public bool AllowEscapeWildcards { get; set; } = true;

    /// <summary>
    /// Gets or sets the custom select.
    /// </summary>
    /// <value>The custom select.</value>
    protected bool CustomSelect { get; set; }
    /// <summary>
    /// The use field name
    /// </summary>
    protected bool useFieldName;
    /// <summary>
    /// The select distinct
    /// </summary>
    protected bool selectDistinct;
    /// <summary>
    /// The skip parameterization for this expression
    /// </summary>
    protected bool skipParameterizationForThisExpression;
    /// <summary>
    /// The has ensure conditions
    /// </summary>
    private bool hasEnsureConditions;
    /// <summary>
    /// The in SQL method call
    /// </summary>
    private bool inSqlMethodCall;

    /// <summary>
    /// Gets the sep.
    /// </summary>
    /// <value>The sep.</value>
    protected string Sep { get; private set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlExpression{T}" /> class.
    /// </summary>
    /// <param name="dialectProvider">The dialect provider.</param>
    protected SqlExpression(IOrmLiteDialectProvider dialectProvider)
    {
        this.UpdateFields = [];
        this.InsertFields = [];

        this.modelDef = typeof(T).GetModelDefinition();
        this.PrefixFieldWithTableName = OrmLiteConfig.IncludeTablePrefixes;
        this.WhereStatementWithoutWhereString = false;

        this.DialectProvider = dialectProvider;
        this.Params = [];
        this.tableDefs.Add(this.modelDef);

        var initFilter = OrmLiteConfig.SqlExpressionInitFilter;

        initFilter?.Invoke(this.GetUntyped());
    }

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> Clone()
    {
        return this.CopyTo(this.DialectProvider.SqlExpression<T>());
    }

    /// <summary>
    /// Adds the tag.
    /// </summary>
    /// <param name="tag">The tag.</param>
    public virtual void AddTag(string tag)
    {
        //Debug.Assert(!string.IsNullOrEmpty(tag));
        this.Tags.Add(tag);
    }

    /// <summary>
    /// Copies to.
    /// </summary>
    /// <param name="to">To.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    protected virtual SqlExpression<T> CopyTo(SqlExpression<T> to)
    {
        to.modelDef = this.modelDef;
        to.tableDefs = this.tableDefs;

        to.UpdateFields = this.UpdateFields;
        to.InsertFields = this.InsertFields;

        to.selectExpression = this.selectExpression;
        to.OnlyFields = this.OnlyFields != null ? new HashSet<string>(this.OnlyFields, StringComparer.OrdinalIgnoreCase) : null;

        to.TableAlias = this.TableAlias;
        to.fromExpression = this.fromExpression;
        to.WhereExpression = this.WhereExpression;
        to.GroupByExpression = this.GroupByExpression;
        to.HavingExpression = this.HavingExpression;
        to.orderBy = this.orderBy;
        to.orderByProperties = this.orderByProperties;

        to.Offset = this.Offset;
        to.Rows = this.Rows;

        to.CustomSelect = this.CustomSelect;
        to.PrefixFieldWithTableName = this.PrefixFieldWithTableName;
        to.useFieldName = this.useFieldName;
        to.selectDistinct = this.selectDistinct;
        to.WhereStatementWithoutWhereString = this.WhereStatementWithoutWhereString;
        to.skipParameterizationForThisExpression = this.skipParameterizationForThisExpression;
        to.UseSelectPropertiesAsAliases = this.UseSelectPropertiesAsAliases;
        to.hasEnsureConditions = this.hasEnsureConditions;

        to.Params = [..this.Params];

        to.underlyingExpression = this.underlyingExpression;
        to.SqlFilter = this.SqlFilter;

        return to;
    }

    /// <summary>
    /// Generate a unique SHA1 hash of expression with param values for caching
    /// </summary>
    /// <param name="includeParams">The include parameters.</param>
    /// <returns>string.</returns>
    public string ComputeHash(bool includeParams = true)
    {
        var uniqueExpr = this.Dump(includeParams);

        // fastest up to 500 chars https://wintermute79.wordpress.com/2014/10/10/c-sha-1-benchmark/
        var hash = SHA1.HashData(Encoding.ASCII.GetBytes(uniqueExpr));
        //using var sha1 = TextConfig.CreateSha();
        //var hash = sha1.ComputeHash(Encoding.ASCII.GetBytes(uniqueExpr));
        var hexFormat = hash.ToHex();

        return hexFormat;
    }

    /// <summary>
    /// Dump internal state of this SqlExpression into a string
    /// </summary>
    /// <param name="includeParams">The include parameters.</param>
    /// <returns>string.</returns>
    public string Dump(bool includeParams)
    {
        var sb = StringBuilderCache.Allocate();

        sb.Append('<').Append(this.ModelDef.Name);
        foreach (var tableDef in this.tableDefs)
        {
            sb.Append(',').Append(tableDef);
        }

        sb.Append('>').AppendLine();

        if (!this.UpdateFields.IsEmpty())
        {
            sb.AppendLine(this.UpdateFields.Join(","));
        }

        if (!this.InsertFields.IsEmpty())
        {
            sb.AppendLine(this.InsertFields.Join(","));
        }

        if (!string.IsNullOrEmpty(this.selectExpression))
        {
            sb.AppendLine(this.selectExpression);
        }

        if (!this.OnlyFields.IsEmpty())
        {
            sb.AppendLine(this.OnlyFields.Join(","));
        }

        if (!string.IsNullOrEmpty(this.TableAlias))
        {
            sb.AppendLine(this.TableAlias);
        }

        if (!string.IsNullOrEmpty(this.fromExpression))
        {
            sb.AppendLine(this.fromExpression);
        }

        if (!string.IsNullOrEmpty(this.WhereExpression))
        {
            sb.AppendLine(this.WhereExpression);
        }

        if (!string.IsNullOrEmpty(this.GroupByExpression))
        {
            sb.AppendLine(this.GroupByExpression);
        }

        if (!string.IsNullOrEmpty(this.HavingExpression))
        {
            sb.AppendLine(this.HavingExpression);
        }

        if (!string.IsNullOrEmpty(this.orderBy))
        {
            sb.AppendLine(this.orderBy);
        }

        if (!this.orderByProperties.IsEmpty())
        {
            sb.AppendLine(this.orderByProperties.Join(","));
        }

        if (this.Offset != null || this.Rows != null)
        {
            sb.Append(this.Offset ?? 0).Append(',').Append(this.Rows ?? 0).AppendLine();
        }

        sb.Append("FLAGS:");
        sb.Append(this.CustomSelect ? "1" : "0");
        sb.Append(this.PrefixFieldWithTableName ? "1" : "0");
        sb.Append(this.useFieldName ? "1" : "0");
        sb.Append(this.selectDistinct ? "1" : "0");
        sb.Append(this.WhereStatementWithoutWhereString ? "1" : "0");
        sb.Append(this.skipParameterizationForThisExpression ? "1" : "0");
        sb.Append(this.UseSelectPropertiesAsAliases ? "1" : "0");
        sb.Append(this.hasEnsureConditions ? "1" : "0");
        sb.AppendLine();

        if (includeParams)
        {
            sb.Append("PARAMS:").Append(this.Params.Count).AppendLine();
            if (this.Params.Count > 0)
            {
                foreach (var p in this.Params)
                {
                    sb.Append(p.ParameterName).Append('=');
                    sb.AppendLine(p.Value.ConvertTo<string>());
                }
            }
        }

        var uniqueExpr = StringBuilderCache.ReturnAndFree(sb);
        return uniqueExpr;
    }

    /// <summary>
    /// Clear select expression. All properties will be selected.
    /// </summary>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Select()
    {
        return this.Select(string.Empty);
    }

    /// <summary>
    /// Selects if distinct.
    /// </summary>
    /// <param name="selectExpression">The select expression.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    internal SqlExpression<T> SelectIfDistinct(string selectExpression)
    {
        return this.selectDistinct ? this.SelectDistinct(selectExpression) : this.Select(selectExpression);
    }

    /// <summary>
    /// set the specified selectExpression.
    /// </summary>
    /// <param name="selectExpression">raw Select expression: "SomeField1, SomeField2 from SomeTable"</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Select(string selectExpression)
    {
        selectExpression?.SqlVerifyFragment();

        return this.UnsafeSelect(selectExpression);
    }

    /// <summary>
    /// set the specified DISTINCT selectExpression.
    /// </summary>
    /// <param name="selectExpression">raw Select expression: "SomeField1, SomeField2 from SomeTable"</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> SelectDistinct(string selectExpression)
    {
        selectExpression?.SqlVerifyFragment();

        return this.UnsafeSelect(selectExpression, true);
    }

    /// <summary>
    /// Unsafes the select.
    /// </summary>
    /// <param name="rawSelect">The raw select.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> UnsafeSelect(string rawSelect)
    {
        return this.UnsafeSelect(rawSelect, false);
    }

    /// <summary>
    /// Unsafes the select.
    /// </summary>
    /// <param name="rawSelect">The raw select.</param>
    /// <param name="distinct">The distinct.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> UnsafeSelect(string rawSelect, bool distinct)
    {
        if (string.IsNullOrEmpty(rawSelect))
        {
            this.BuildSelectExpression(string.Empty, distinct);
        }
        else
        {
            this.selectExpression = "SELECT " + (distinct ? "DISTINCT " : string.Empty) + rawSelect;
            this.CustomSelect = true;
            this.OnlyFields = null;
        }

        return this;
    }

    /// <summary>
    /// Set the specified selectExpression using matching fields.
    /// </summary>
    /// <param name="fields">Matching Fields: "SomeField1, SomeField2"</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Select(string[] fields)
    {
        return this.Select(fields, false);
    }

    /// <summary>
    /// Set the specified DISTINCT selectExpression using matching fields.
    /// </summary>
    /// <param name="fields">Matching Fields: "SomeField1, SomeField2"</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> SelectDistinct(string[] fields)
    {
        return this.Select(fields, true);
    }

    /// <summary>
    /// Selects the specified fields.
    /// </summary>
    /// <param name="fields">The fields.</param>
    /// <param name="distinct">The distinct.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    internal virtual SqlExpression<T> Select(string[] fields, bool distinct)
    {
        if (fields == null || fields.Length == 0)
        {
            return this.Select(string.Empty);
        }

        this.useFieldName = true;

        var allTableDefs = this.GetAllTables();

        var fieldsList = new List<string>();
        var sb = StringBuilderCache.Allocate();
        foreach (var field in fields)
        {
            if (string.IsNullOrEmpty(field))
            {
                continue;
            }

            if (field.EndsWith(".*"))
            {
                var tableName = field[..^2];
                var tableDef = allTableDefs.FirstOrDefault(x => string.Equals(x.Name, tableName, StringComparison.OrdinalIgnoreCase));
                if (tableDef != null)
                {
                    foreach (var fieldDef in tableDef.FieldDefinitionsArray)
                    {
                        var qualifiedField = this.GetQuotedColumnName(tableDef, fieldDef.Name);
                        if (fieldDef.CustomSelect != null)
                        {
                            qualifiedField += " AS " + fieldDef.Name;
                        }

                        if (sb.Length > 0)
                        {
                            sb.Append(", ");
                        }

                        sb.Append(qualifiedField);
                        fieldsList.Add(fieldDef.Name);
                    }
                }
            }
            else
            {
                fieldsList.Add(field); // Could be non-matching referenced property

                var match = this.FirstMatchingField(field);
                if (match == null)
                {
                    continue;
                }

                var fieldDef = match.Item2;
                var qualifiedName = this.GetQuotedColumnName(match.Item1, fieldDef.Name);
                if (fieldDef.CustomSelect != null)
                {
                    qualifiedName += " AS " + fieldDef.Name;
                }

                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }

                sb.Append(qualifiedName);
            }
        }

        this.UnsafeSelect(StringBuilderCache.ReturnAndFree(sb), distinct);
        this.OnlyFields = new HashSet<string>(fieldsList, StringComparer.OrdinalIgnoreCase);

        return this;
    }

    /// <summary>
    /// Internals the select.
    /// </summary>
    /// <param name="fields">The fields.</param>
    /// <param name="distinct">The distinct.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    private SqlExpression<T> InternalSelect(Expression fields, bool distinct = false)
    {
        this.Reset(this.Sep = string.Empty);

        this.CustomSelect = true;
        var selectSql = this.Visit(fields);
        if (!IsSqlClass(selectSql))
        {
            selectSql = this.ConvertToParam(selectSql);
        }

        this.BuildSelectExpression(selectSql.ToString(), distinct);
        return this;
    }

    /// <summary>
    /// Fields to be selected.
    /// </summary>
    /// <param name="fields">x=&gt; x.SomeProperty1 or x=&gt; new{ x.SomeProperty1, x.SomeProperty2}</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Select(Expression<Func<T, object>> fields)
    {
        return this.InternalSelect(fields);
    }

    /// <summary>
    /// Selects the specified fields.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Select<Table1>(Expression<Func<Table1, object>> fields)
    {
        return this.InternalSelect(fields);
    }

    /// <summary>
    /// Selects the specified fields.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Select<Table1, Table2>(Expression<Func<Table1, Table2, object>> fields)
    {
        return this.InternalSelect(fields);
    }

    /// <summary>
    /// Selects the specified fields.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Select<Table1, Table2, Table3>(Expression<Func<Table1, Table2, Table3, object>> fields)
    {
        return this.InternalSelect(fields);
    }

    /// <summary>
    /// Selects the specified fields.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <typeparam name="Table4">The type of the table4.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Select<Table1, Table2, Table3, Table4>(Expression<Func<Table1, Table2, Table3, Table4, object>> fields)
    {
        return this.InternalSelect(fields);
    }

    /// <summary>
    /// Selects the specified fields.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <typeparam name="Table4">The type of the table4.</typeparam>
    /// <typeparam name="Table5">The type of the table5.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Select<Table1, Table2, Table3, Table4, Table5>(Expression<Func<Table1, Table2, Table3, Table4, Table5, object>> fields)
    {
        return this.InternalSelect(fields);
    }

    /// <summary>
    /// Selects the specified fields.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <typeparam name="Table4">The type of the table4.</typeparam>
    /// <typeparam name="Table5">The type of the table5.</typeparam>
    /// <typeparam name="Table6">The type of the table6.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Select<Table1, Table2, Table3, Table4, Table5, Table6>(Expression<Func<Table1, Table2, Table3, Table4, Table5, Table6, object>> fields)
    {
        return this.InternalSelect(fields);
    }

    /// <summary>
    /// Selects the specified fields.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <typeparam name="Table4">The type of the table4.</typeparam>
    /// <typeparam name="Table5">The type of the table5.</typeparam>
    /// <typeparam name="Table6">The type of the table6.</typeparam>
    /// <typeparam name="Table7">The type of the table7.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Select<Table1, Table2, Table3, Table4, Table5, Table6, Table7>(Expression<Func<Table1, Table2, Table3, Table4, Table5, Table6, Table7, object>> fields)
    {
        return this.InternalSelect(fields);
    }

    /// <summary>
    /// Selects the specified fields.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <typeparam name="Table4">The type of the table4.</typeparam>
    /// <typeparam name="Table5">The type of the table5.</typeparam>
    /// <typeparam name="Table6">The type of the table6.</typeparam>
    /// <typeparam name="Table7">The type of the table7.</typeparam>
    /// <typeparam name="Table8">The type of the table8.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Select<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8>(Expression<Func<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, object>> fields)
    {
        return this.InternalSelect(fields);
    }

    /// <summary>
    /// Selects the specified fields.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <typeparam name="Table4">The type of the table4.</typeparam>
    /// <typeparam name="Table5">The type of the table5.</typeparam>
    /// <typeparam name="Table6">The type of the table6.</typeparam>
    /// <typeparam name="Table7">The type of the table7.</typeparam>
    /// <typeparam name="Table8">The type of the table8.</typeparam>
    /// <typeparam name="Table9">The type of the table9.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Select<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9>(Expression<Func<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9, object>> fields)
    {
        return this.InternalSelect(fields);
    }

    /// <summary>
    /// Selects the specified fields.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <typeparam name="Table4">The type of the table4.</typeparam>
    /// <typeparam name="Table5">The type of the table5.</typeparam>
    /// <typeparam name="Table6">The type of the table6.</typeparam>
    /// <typeparam name="Table7">The type of the table7.</typeparam>
    /// <typeparam name="Table8">The type of the table8.</typeparam>
    /// <typeparam name="Table9">The type of the table9.</typeparam>
    /// <typeparam name="Table10">The type of the table10.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Select<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9, Table10>(Expression<Func<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9, Table10, object>> fields)
    {
        return this.InternalSelect(fields);
    }

    /// <summary>
    /// Selects the specified fields.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <typeparam name="Table4">The type of the table4.</typeparam>
    /// <typeparam name="Table5">The type of the table5.</typeparam>
    /// <typeparam name="Table6">The type of the table6.</typeparam>
    /// <typeparam name="Table7">The type of the table7.</typeparam>
    /// <typeparam name="Table8">The type of the table8.</typeparam>
    /// <typeparam name="Table9">The type of the table9.</typeparam>
    /// <typeparam name="Table10">The type of the table10.</typeparam>
    /// <typeparam name="Table11">The type of the table11.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Select<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9, Table10, Table11>(Expression<Func<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9, Table10, Table11, object>> fields)
    {
        return this.InternalSelect(fields);
    }

    /// <summary>
    /// Selects the specified fields.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <typeparam name="Table4">The type of the table4.</typeparam>
    /// <typeparam name="Table5">The type of the table5.</typeparam>
    /// <typeparam name="Table6">The type of the table6.</typeparam>
    /// <typeparam name="Table7">The type of the table7.</typeparam>
    /// <typeparam name="Table8">The type of the table8.</typeparam>
    /// <typeparam name="Table9">The type of the table9.</typeparam>
    /// <typeparam name="Table10">The type of the table10.</typeparam>
    /// <typeparam name="Table11">The type of the table11.</typeparam>
    /// <typeparam name="Table12">The type of the table12.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Select<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9, Table10, Table11, Table12>(Expression<Func<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9, Table10, Table11, Table12, object>> fields)
    {
        return this.InternalSelect(fields);
    }

    /// <summary>
    /// Selects the distinct.
    /// </summary>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> SelectDistinct(Expression<Func<T, object>> fields)
    {
        return this.InternalSelect(fields, true);
    }

    /// <summary>
    /// Selects the distinct.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> SelectDistinct<Table1>(Expression<Func<Table1, object>> fields)
    {
        return this.InternalSelect(fields, true);
    }

    /// <summary>
    /// Selects the distinct.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> SelectDistinct<Table1, Table2>(Expression<Func<Table1, Table2, object>> fields)
    {
        return this.InternalSelect(fields, true);
    }

    /// <summary>
    /// Selects the distinct.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> SelectDistinct<Table1, Table2, Table3>(Expression<Func<Table1, Table2, Table3, object>> fields)
    {
        return this.InternalSelect(fields, true);
    }

    /// <summary>
    /// Selects the distinct.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <typeparam name="Table4">The type of the table4.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> SelectDistinct<Table1, Table2, Table3, Table4>(Expression<Func<Table1, Table2, Table3, Table4, object>> fields)
    {
        return this.InternalSelect(fields, true);
    }

    /// <summary>
    /// Selects the distinct.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <typeparam name="Table4">The type of the table4.</typeparam>
    /// <typeparam name="Table5">The type of the table5.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> SelectDistinct<Table1, Table2, Table3, Table4, Table5>(Expression<Func<Table1, Table2, Table3, Table4, Table5, object>> fields)
    {
        return this.InternalSelect(fields, true);
    }

    /// <summary>
    /// Selects the distinct.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <typeparam name="Table4">The type of the table4.</typeparam>
    /// <typeparam name="Table5">The type of the table5.</typeparam>
    /// <typeparam name="Table6">The type of the table6.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> SelectDistinct<Table1, Table2, Table3, Table4, Table5, Table6>(Expression<Func<Table1, Table2, Table3, Table4, Table5, Table6, object>> fields)
    {
        return this.InternalSelect(fields, true);
    }

    /// <summary>
    /// Selects the distinct.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <typeparam name="Table4">The type of the table4.</typeparam>
    /// <typeparam name="Table5">The type of the table5.</typeparam>
    /// <typeparam name="Table6">The type of the table6.</typeparam>
    /// <typeparam name="Table7">The type of the table7.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> SelectDistinct<Table1, Table2, Table3, Table4, Table5, Table6, Table7>(Expression<Func<Table1, Table2, Table3, Table4, Table5, Table6, Table7, object>> fields)
    {
        return this.InternalSelect(fields, true);
    }

    /// <summary>
    /// Selects the distinct.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <typeparam name="Table4">The type of the table4.</typeparam>
    /// <typeparam name="Table5">The type of the table5.</typeparam>
    /// <typeparam name="Table6">The type of the table6.</typeparam>
    /// <typeparam name="Table7">The type of the table7.</typeparam>
    /// <typeparam name="Table8">The type of the table8.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> SelectDistinct<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8>(Expression<Func<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, object>> fields)
    {
        return this.InternalSelect(fields, true);
    }

    /// <summary>
    /// Selects the distinct.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <typeparam name="Table4">The type of the table4.</typeparam>
    /// <typeparam name="Table5">The type of the table5.</typeparam>
    /// <typeparam name="Table6">The type of the table6.</typeparam>
    /// <typeparam name="Table7">The type of the table7.</typeparam>
    /// <typeparam name="Table8">The type of the table8.</typeparam>
    /// <typeparam name="Table9">The type of the table9.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> SelectDistinct<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9>(Expression<Func<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9, object>> fields)
    {
        return this.InternalSelect(fields, true);
    }

    /// <summary>
    /// Selects the distinct.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <typeparam name="Table4">The type of the table4.</typeparam>
    /// <typeparam name="Table5">The type of the table5.</typeparam>
    /// <typeparam name="Table6">The type of the table6.</typeparam>
    /// <typeparam name="Table7">The type of the table7.</typeparam>
    /// <typeparam name="Table8">The type of the table8.</typeparam>
    /// <typeparam name="Table9">The type of the table9.</typeparam>
    /// <typeparam name="Table10">The type of the table10.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> SelectDistinct<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9, Table10>(Expression<Func<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9, Table10, object>> fields)
    {
        return this.InternalSelect(fields, true);
    }

    /// <summary>
    /// Selects the distinct.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <typeparam name="Table4">The type of the table4.</typeparam>
    /// <typeparam name="Table5">The type of the table5.</typeparam>
    /// <typeparam name="Table6">The type of the table6.</typeparam>
    /// <typeparam name="Table7">The type of the table7.</typeparam>
    /// <typeparam name="Table8">The type of the table8.</typeparam>
    /// <typeparam name="Table9">The type of the table9.</typeparam>
    /// <typeparam name="Table10">The type of the table10.</typeparam>
    /// <typeparam name="Table11">The type of the table11.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> SelectDistinct<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9, Table10, Table11>(Expression<Func<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9, Table10, Table11, object>> fields)
    {
        return this.InternalSelect(fields, true);
    }

    /// <summary>
    /// Selects the distinct.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <typeparam name="Table4">The type of the table4.</typeparam>
    /// <typeparam name="Table5">The type of the table5.</typeparam>
    /// <typeparam name="Table6">The type of the table6.</typeparam>
    /// <typeparam name="Table7">The type of the table7.</typeparam>
    /// <typeparam name="Table8">The type of the table8.</typeparam>
    /// <typeparam name="Table9">The type of the table9.</typeparam>
    /// <typeparam name="Table10">The type of the table10.</typeparam>
    /// <typeparam name="Table11">The type of the table11.</typeparam>
    /// <typeparam name="Table12">The type of the table12.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> SelectDistinct<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9, Table10, Table11, Table12>(Expression<Func<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9, Table10, Table11, Table12, object>> fields)
    {
        return this.InternalSelect(fields, true);
    }

    /// <summary>
    /// Selects the distinct.
    /// </summary>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> SelectDistinct()
    {
        this.selectDistinct = true;
        return this;
    }

    /// <summary>
    /// Froms the specified tables.
    /// </summary>
    /// <param name="tables">The tables.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> From(string tables)
    {
        tables?.SqlVerifyFragment();

        return this.UnsafeFrom(tables);
    }

    /// <summary>
    /// Includes the table prefix.
    /// </summary>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> IncludeTablePrefix()
    {
        this.PrefixFieldWithTableName = true;
        return this;
    }

    /// <summary>
    /// Sets the table alias.
    /// </summary>
    /// <param name="tableAlias">The table alias.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> SetTableAlias(string tableAlias)
    {
        this.PrefixFieldWithTableName = tableAlias != null;
        this.TableAlias = tableAlias;
        return this;
    }

    /// <summary>
    /// Unsafes from.
    /// </summary>
    /// <param name="rawFrom">The raw from.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> UnsafeFrom(string rawFrom)
    {
        if (string.IsNullOrEmpty(rawFrom))
        {
            this.FromExpression = null;
        }
        else
        {
            var singleTable = rawFrom.ToLower().IndexOfAny("join", ",") == -1;
            this.FromExpression = singleTable
                                      ? " \nFROM " + this.DialectProvider.QuoteTable(rawFrom)
                                      : " \nFROM " + rawFrom;
        }

        return this;
    }

    /// <summary>
    /// Wheres this instance.
    /// </summary>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Where()
    {
        this.underlyingExpression = null; // Where() clears the expression

        this.WhereExpression = null;
        return this;
    }

    /// <summary>
    /// Formats the filter.
    /// </summary>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>string.</returns>
    private string FormatFilter(string sqlFilter, params object[] filterParams)
    {
        if (string.IsNullOrEmpty(sqlFilter))
        {
            return null;
        }

        for (var i = 0; i < filterParams.Length; i++)
        {
            var pLiteral = "{" + i + "}";
            var filterParam = filterParams[i];

            if (filterParam is SqlInValues sqlParams)
            {
                if (sqlParams.Count > 0)
                {
                    var sqlIn = this.CreateInParamSql(sqlParams.GetValues());
                    sqlFilter = sqlFilter.Replace(pLiteral, sqlIn);
                }
                else
                {
                    sqlFilter = sqlFilter.Replace(pLiteral, SqlInValues.EmptyIn);
                }
            }
            else
            {
                var p = this.AddParam(filterParam);
                sqlFilter = sqlFilter.Replace(pLiteral, p.ParameterName);
            }
        }

        return sqlFilter;
    }

    /// <summary>
    /// Creates the in parameter SQL.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <returns>string.</returns>
    private string CreateInParamSql(IEnumerable values)
    {
        var sbParams = StringBuilderCache.Allocate();
        foreach (var item in values)
        {
            var p = this.AddParam(item);

            if (sbParams.Length > 0)
            {
                sbParams.Append(',');
            }

            sbParams.Append(p.ParameterName);
        }

        var sqlIn = StringBuilderCache.ReturnAndFree(sbParams);
        return sqlIn;
    }

    /// <summary>
    /// Unsafes the where.
    /// </summary>
    /// <param name="rawSql">The raw SQL.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> UnsafeWhere(string rawSql, params object[] filterParams)
    {
        return this.AppendToWhere("AND", this.FormatFilter(rawSql, filterParams));
    }

    /// <summary>
    /// Wheres the specified SQL filter.
    /// </summary>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Where(string sqlFilter, params object[] filterParams)
    {
        return this.AppendToWhere("AND", this.FormatFilter(sqlFilter.SqlVerifyFragment(), filterParams));
    }

    /// <summary>
    /// Unsafes the and.
    /// </summary>
    /// <param name="rawSql">The raw SQL.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> UnsafeAnd(string rawSql, params object[] filterParams)
    {
        return this.AppendToWhere("AND", this.FormatFilter(rawSql, filterParams));
    }

    /// <summary>
    /// Ands the specified SQL filter.
    /// </summary>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> And(string sqlFilter, params object[] filterParams)
    {
        return this.AppendToWhere("AND", this.FormatFilter(sqlFilter.SqlVerifyFragment(), filterParams));
    }

    /// <summary>
    /// Unsafes the or.
    /// </summary>
    /// <param name="rawSql">The raw SQL.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> UnsafeOr(string rawSql, params object[] filterParams)
    {
        return this.AppendToWhere("OR", this.FormatFilter(rawSql, filterParams));
    }

    /// <summary>
    /// Ors the specified SQL filter.
    /// </summary>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Or(string sqlFilter, params object[] filterParams)
    {
        return this.AppendToWhere("OR", this.FormatFilter(sqlFilter.SqlVerifyFragment(), filterParams));
    }

    /// <summary>
    /// Adds the condition.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> AddCondition(string condition, string sqlFilter, params object[] filterParams)
    {
        return this.AppendToWhere(condition, this.FormatFilter(sqlFilter.SqlVerifyFragment(), filterParams));
    }

    /// <summary>
    /// Wheres the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Where(Expression<Func<T, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Wheres the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Where(Expression<Func<T, bool>> predicate, params object[] filterParams)
    {
        return this.AppendToWhere("AND", predicate, filterParams);
    }

    /// <summary>
    /// Ands the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> And(Expression<Func<T, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Ands the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> And(Expression<Func<T, bool>> predicate, params object[] filterParams)
    {
        return this.AppendToWhere("AND", predicate, filterParams);
    }

    /// <summary>
    /// Ors the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Or(Expression<Func<T, bool>> predicate)
    {
        return this.AppendToWhere("OR", predicate);
    }

    /// <summary>
    /// Ors the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Or(Expression<Func<T, bool>> predicate, params object[] filterParams)
    {
        return this.AppendToWhere("OR", predicate, filterParams);
    }

    /// <summary>
    /// Wheres the exists.
    /// </summary>
    /// <param name="subSelect">The sub select.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> WhereExists(ISqlExpression subSelect)
    {
        var sql = subSelect.ToSelectStatement(QueryType.Select);
        var mergedSql = this.DialectProvider.MergeParamsIntoSql(sql, subSelect.Params);
        return this.AppendToWhere("AND", this.FormatFilter($"EXISTS ({mergedSql})"));
    }

    /// <summary>
    /// Wheres the not exists.
    /// </summary>
    /// <param name="subSelect">The sub select.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> WhereNotExists(ISqlExpression subSelect)
    {
        var sql = subSelect.ToSelectStatement(QueryType.Select);
        var mergedSql = this.DialectProvider.MergeParamsIntoSql(sql, subSelect.Params);
        return this.AppendToWhere("AND", this.FormatFilter($"NOT EXISTS ({mergedSql})"));
    }

    /// <summary>
    /// The original lambda
    /// </summary>
    private LambdaExpression originalLambda;

    /// <summary>
    /// Resets the specified sep.
    /// </summary>
    /// <param name="sep">The sep.</param>
    /// <param name="useFieldName">Name of the use field.</param>
    private void Reset(string sep = " ", bool useFieldName = true)
    {
        this.Sep = sep;
        this.useFieldName = useFieldName;
        this.originalLambda = null;
    }

    /// <summary>
    /// Appends to where.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    protected SqlExpression<T> AppendToWhere(string condition, Expression predicate, object[] filterParams)
    {
        if (predicate == null)
        {
            return this;
        }

        this.Reset();

        var newExpr = WhereExpressionToString(this.Visit(predicate));
        var formatExpr = this.FormatFilter(newExpr, filterParams);
        return this.AppendToWhere(condition, formatExpr);
    }

    /// <summary>
    /// Appends to where.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    protected SqlExpression<T> AppendToWhere(string condition, Expression predicate)
    {
        if (predicate == null)
        {
            return this;
        }

        this.Reset();

        var newExpr = WhereExpressionToString(this.Visit(predicate));
        return this.AppendToWhere(condition, newExpr);
    }

    /// <summary>
    /// Wheres the expression to string.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns>string.</returns>
    private static string WhereExpressionToString(object expression)
    {
        if (expression is bool b)
        {
            return b ? TrueLiteral : FalseLiteral;
        }

        return expression.ToString();
    }

    /// <summary>
    /// Appends to where.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <param name="sqlExpression">The SQL expression.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    /// <exception cref="NotSupportedException">Invalid whereExpression Expression with Ensure Conditions</exception>
    protected SqlExpression<T> AppendToWhere(string condition, string sqlExpression)
    {
        var addExpression = string.IsNullOrEmpty(this.WhereExpression)
                                ? (this.WhereStatementWithoutWhereString ? string.Empty : "WHERE ") + sqlExpression
                                : " " + condition + " " + sqlExpression;

        if (!this.hasEnsureConditions)
        {
            this.WhereExpression += addExpression;
        }
        else
        {
            if (this.WhereExpression![^1] != ')')
            {
                throw new NotSupportedException("Invalid whereExpression Expression with Ensure Conditions");
            }

            // insert before normal WHERE parens: {EnsureConditions} AND (1+1)
            if (this.WhereExpression.EndsWith(TrueLiteral, StringComparison.Ordinal))
            {
                // insert before ^1+1)
                this.WhereExpression = string.Concat(
                    this.WhereExpression.AsSpan(0, this.WhereExpression.Length - (TrueLiteral.Length - 1))
                    , sqlExpression, ")");
            }
            else
            {
                // insert before ^)
                this.WhereExpression = string.Concat(this.WhereExpression.AsSpan(0, this.WhereExpression.Length - 1)
                    , addExpression, ")");
            }
        }

        return this;
    }

    /// <summary>
    /// Ensures the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Ensure(Expression<Func<T, bool>> predicate)
    {
        return this.AppendToEnsure(predicate);
    }

    /// <summary>
    /// Ensures the specified predicate.
    /// </summary>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Ensure<Target>(Expression<Func<Target, bool>> predicate)
    {
        return this.AppendToEnsure(predicate);
    }

    /// <summary>
    /// Ensures the specified predicate.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Ensure<Source, Target>(Expression<Func<Source, Target, bool>> predicate)
    {
        return this.AppendToEnsure(predicate);
    }

    /// <summary>
    /// Ensures the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Ensure<T1, T2, T3>(Expression<Func<T1, T2, T3, bool>> predicate)
    {
        return this.AppendToEnsure(predicate);
    }

    /// <summary>
    /// Ensures the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Ensure<T1, T2, T3, T4>(Expression<Func<T1, T2, T3, T4, bool>> predicate)
    {
        return this.AppendToEnsure(predicate);
    }

    /// <summary>
    /// Ensures the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Ensure<T1, T2, T3, T4, T5>(Expression<Func<T1, T2, T3, T4, T5, bool>> predicate)
    {
        return this.AppendToEnsure(predicate);
    }

    /// <summary>
    /// Appends to ensure.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    protected SqlExpression<T> AppendToEnsure(Expression predicate)
    {
        if (predicate == null)
        {
            return this;
        }

        this.Reset();

        var newExpr = WhereExpressionToString(this.Visit(predicate));
        return this.Ensure(newExpr);
    }

    /// <summary>
    /// Add a WHERE Condition to always be applied, irrespective of other WHERE conditions
    /// </summary>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    /// <exception cref="NotSupportedException">Invalid whereExpression Expression with Ensure Conditions</exception>
    public SqlExpression<T> Ensure(string sqlFilter, params object[] filterParams)
    {
        var condition = this.FormatFilter(sqlFilter, filterParams);
        if (string.IsNullOrEmpty(this.WhereExpression))
        {
            this.WhereExpression = "WHERE " + condition
                                            + " AND " + TrueLiteral; // allow subsequent WHERE conditions to be inserted before parens
        }
        else
        {
            if (!this.hasEnsureConditions)
            {
                var existingExpr = this.WhereExpression.StartsWith("WHERE ", StringComparison.OrdinalIgnoreCase)
                                       ? this.WhereExpression.Substring("WHERE ".Length)
                                       : this.WhereExpression;

                this.WhereExpression = "WHERE " + condition + " AND (" + existingExpr + ")";
            }
            else
            {
                if (!this.WhereExpression.StartsWith("WHERE ", StringComparison.OrdinalIgnoreCase))
                {
                    throw new NotSupportedException("Invalid whereExpression Expression with Ensure Conditions");
                }

                this.WhereExpression = $"WHERE {condition} AND {this.WhereExpression["WHERE ".Length..]}";
            }
        }

        this.hasEnsureConditions = true;
        return this;
    }

    /// <summary>
    /// Lists the expression.
    /// </summary>
    /// <param name="expr">The expr.</param>
    /// <param name="strExpr">The string expr.</param>
    /// <returns>string.</returns>
    private string ListExpression(Expression expr, string strExpr)
    {
        if (expr is LambdaExpression lambdaExpr)
        {
            if (lambdaExpr.Parameters.Count == 1 && lambdaExpr.Body is MemberExpression me)
            {
                var tableDef = lambdaExpr.Parameters[0].Type.GetModelMetadata();
                var fieldDef = tableDef?.GetFieldDefinition(me.Member.Name);
                if (fieldDef != null)
                {
                    return this.DialectProvider.GetQuotedColumnName(tableDef, me.Member.Name);
                }
            }
        }

        return strExpr;
    }

    /// <summary>
    /// Groups the by.
    /// </summary>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> GroupBy()
    {
        return this.GroupBy(string.Empty);
    }

    /// <summary>
    /// Groups the by.
    /// </summary>
    /// <param name="groupBy">The group by.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> GroupBy(string groupBy)
    {
        return this.UnsafeGroupBy(groupBy.SqlVerifyFragment());
    }

    /// <summary>
    /// Unsafes the group by.
    /// </summary>
    /// <param name="groupBy">The group by.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> UnsafeGroupBy(string groupBy)
    {
        if (!string.IsNullOrEmpty(groupBy))
        {
            this.GroupByExpression = "GROUP BY " + groupBy;
        }

        return this;
    }

    /// <summary>
    /// Internals the group by.
    /// </summary>
    /// <param name="expr">The expr.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    private SqlExpression<T> InternalGroupBy(Expression expr)
    {
        this.Reset(this.Sep = string.Empty);

        var groupByExpr = this.Visit(expr);
        if (IsSqlClass(groupByExpr))
        {
            StripAliases(groupByExpr as SelectList); // No "AS ColumnAlias" in GROUP BY, just the column names/expressions

            return this.GroupBy(groupByExpr.ToString());
        }

        if (groupByExpr is string strExpr)
        {
            return this.GroupBy(this.ListExpression(expr, strExpr));
        }

        return this;
    }

    /// <summary>
    /// Groups the by.
    /// </summary>
    /// <typeparam name="Table">The type of the table.</typeparam>
    /// <param name="keySelector">The key selector.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> GroupBy<Table>(Expression<Func<Table, object>> keySelector)
    {
        return this.InternalGroupBy(keySelector);
    }

    /// <summary>
    /// Groups the by.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <param name="keySelector">The key selector.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> GroupBy<Table1, Table2>(Expression<Func<Table1, Table2, object>> keySelector)
    {
        return this.InternalGroupBy(keySelector);
    }

    /// <summary>
    /// Groups the by.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <param name="keySelector">The key selector.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> GroupBy<Table1, Table2, Table3>(Expression<Func<Table1, Table2, Table3, object>> keySelector)
    {
        return this.InternalGroupBy(keySelector);
    }

    /// <summary>
    /// Groups the by.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <typeparam name="Table4">The type of the table4.</typeparam>
    /// <param name="keySelector">The key selector.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> GroupBy<Table1, Table2, Table3, Table4>(Expression<Func<Table1, Table2, Table3, Table4, object>> keySelector)
    {
        return this.InternalGroupBy(keySelector);
    }

    /// <summary>
    /// Groups the by.
    /// </summary>
    /// <param name="keySelector">The key selector.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> GroupBy(Expression<Func<T, object>> keySelector)
    {
        return this.InternalGroupBy(keySelector);
    }

    /// <summary>
    /// Havings this instance.
    /// </summary>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Having()
    {
        return this.Having(string.Empty);
    }

    /// <summary>
    /// Havings the specified SQL filter.
    /// </summary>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Having(string sqlFilter, params object[] filterParams)
    {
        this.HavingExpression = this.FormatFilter(sqlFilter.SqlVerifyFragment(), filterParams);

        if (this.HavingExpression != null)
        {
            this.HavingExpression = "HAVING " + this.HavingExpression;
        }

        return this;
    }

    /// <summary>
    /// Unsafes the having.
    /// </summary>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> UnsafeHaving(string sqlFilter, params object[] filterParams)
    {
        this.HavingExpression = this.FormatFilter(sqlFilter, filterParams);

        if (this.HavingExpression != null)
        {
            this.HavingExpression = "HAVING " + this.HavingExpression;
        }

        return this;
    }

    /// <summary>
    /// Appends the having.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    protected SqlExpression<T> AppendHaving(Expression predicate)
    {
        if (predicate != null)
        {
            this.Reset();

            this.HavingExpression = WhereExpressionToString(this.Visit(predicate));
            if (!string.IsNullOrEmpty(this.HavingExpression))
            {
                this.HavingExpression = "HAVING " + this.HavingExpression;
            }
        }
        else
        {
            this.HavingExpression = string.Empty;
        }

        return this;
    }

    /// <summary>
    /// Havings the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Having(Expression<Func<T, bool>> predicate)
    {
        return this.AppendHaving(predicate);
    }

    /// <summary>
    /// Havings the specified predicate.
    /// </summary>
    /// <typeparam name="Table">The type of the table.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Having<Table>(Expression<Func<Table, bool>> predicate)
    {
        return this.AppendHaving(predicate);
    }

    /// <summary>
    /// Havings the specified predicate.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Having<Table1, Table2>(Expression<Func<Table1, Table2, bool>> predicate)
    {
        return this.AppendHaving(predicate);
    }

    /// <summary>
    /// Havings the specified predicate.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Having<Table1, Table2, Table3>(Expression<Func<Table1, Table2, Table3, bool>> predicate)
    {
        return this.AppendHaving(predicate);
    }

    /// <summary>
    /// Orders the by.
    /// </summary>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> OrderBy()
    {
        return this.OrderBy(string.Empty);
    }

    /// <summary>
    /// Orders the by.
    /// </summary>
    /// <param name="orderBy">The order by.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> OrderBy(string orderBy)
    {
        return this.UnsafeOrderBy(orderBy.SqlVerifyFragment());
    }

    /// <summary>
    /// Orders the by.
    /// </summary>
    /// <param name="columnIndex">Index of the column.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> OrderBy(long columnIndex)
    {
        return this.UnsafeOrderBy(columnIndex.ToString());
    }

    /// <summary>
    /// Unsafes the order by.
    /// </summary>
    /// <param name="orderBy">The order by.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> UnsafeOrderBy(string orderBy)
    {
        this.orderByProperties.Clear();
        if (!string.IsNullOrEmpty(orderBy))
        {
            this.orderByProperties.Add(orderBy);
        }

        this.BuildOrderByClauseInternal();
        return this;
    }

    /// <summary>
    /// Orders the by random.
    /// </summary>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> OrderByRandom()
    {
        return this.OrderBy(this.DialectProvider.SqlRandom);
    }

    /// <summary>
    /// Gets the model definition.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>ServiceStack.OrmLite.ModelDefinition.</returns>
    public ModelDefinition GetModelDefinition(FieldDefinition fieldDef)
    {
        if (this.modelDef.FieldDefinitions.Any(x => x == fieldDef))
        {
            return this.modelDef;
        }

        return this.tableDefs
            .FirstOrDefault(tableDef => tableDef.FieldDefinitions.Any(x => x == fieldDef));
    }

    /// <summary>
    /// Orders the by fields.
    /// </summary>
    /// <param name="orderBySuffix">The order by suffix.</param>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    private SqlExpression<T> OrderByFields(string orderBySuffix, FieldDefinition[] fields)
    {
        this.orderByProperties.Clear();

        if (fields.Length == 0)
        {
            this.orderBy = null;
            return this;
        }

        this.useFieldName = true;

        var sbOrderBy = StringBuilderCache.Allocate();
        foreach (var field in fields)
        {
            var tableDef = this.GetModelDefinition(field);
            var qualifiedName = tableDef != null
                                    ? this.GetQuotedColumnName(tableDef, field.Name)
                                    : this.DialectProvider.GetQuotedColumnName(field);

            if (sbOrderBy.Length > 0)
            {
                sbOrderBy.Append(", ");
            }

            sbOrderBy.Append(qualifiedName + orderBySuffix);
        }

        this.orderBy = "ORDER BY " + StringBuilderCache.ReturnAndFree(sbOrderBy);
        return this;
    }

    /// <summary>
    /// Class OrderBySuffix.
    /// </summary>
    private static class OrderBySuffix
    {
        /// <summary>
        /// The asc
        /// </summary>
        public const string Asc = "";
        /// <summary>
        /// The desc
        /// </summary>
        public const string Desc = " DESC";
    }

    /// <summary>
    /// Orders the by fields.
    /// </summary>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> OrderByFields(params FieldDefinition[] fields)
    {
        return this.OrderByFields(OrderBySuffix.Asc, fields);
    }

    /// <summary>
    /// Orders the by fields descending.
    /// </summary>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> OrderByFieldsDescending(params FieldDefinition[] fields)
    {
        return this.OrderByFields(OrderBySuffix.Desc, fields);
    }

    /// <summary>
    /// Orders the by fields.
    /// </summary>
    /// <param name="orderBySuffix">The order by suffix.</param>
    /// <param name="fieldNames">The field names.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    private SqlExpression<T> OrderByFields(string orderBySuffix, string[] fieldNames)
    {
        this.orderByProperties.Clear();

        if (fieldNames.Length == 0)
        {
            this.orderBy = null;
            return this;
        }

        this.useFieldName = true;

        var sbOrderBy = StringBuilderCache.Allocate();
        foreach (var fieldName in fieldNames)
        {
            var reverse = fieldName.StartsWith('-');
            var useSuffix = reverse
                                ? orderBySuffix == OrderBySuffix.Asc ? OrderBySuffix.Desc : OrderBySuffix.Asc
                                : orderBySuffix;
            var useName = reverse ? fieldName[1..] : fieldName;

            var field = this.FirstMatchingField(useName);
            var qualifiedName = field != null
                                    ? this.GetQuotedColumnName(field.Item1, field.Item2.Name)
                                    : string.Equals("Random", useName, StringComparison.OrdinalIgnoreCase)
                                        ? this.DialectProvider.SqlRandom
                                        : throw new ArgumentException("Could not find field " + useName);

            if (sbOrderBy.Length > 0)
            {
                sbOrderBy.Append(", ");
            }

            sbOrderBy.Append(qualifiedName + useSuffix);
        }

        this.orderBy = "ORDER BY " + StringBuilderCache.ReturnAndFree(sbOrderBy);
        return this;
    }

    /// <summary>
    /// Orders the by fields.
    /// </summary>
    /// <param name="fieldNames">The field names.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> OrderByFields(params string[] fieldNames)
    {
        return this.OrderByFields(string.Empty, fieldNames);
    }

    /// <summary>
    /// Orders the by fields descending.
    /// </summary>
    /// <param name="fieldNames">The field names.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> OrderByFieldsDescending(params string[] fieldNames)
    {
        return this.OrderByFields(" DESC", fieldNames);
    }

    /// <summary>
    /// Orders the by.
    /// </summary>
    /// <param name="keySelector">The key selector.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> OrderBy(Expression<Func<T, object>> keySelector)
    {
        return this.OrderByInternal(keySelector);
    }

    /// <summary>
    /// Orders the by.
    /// </summary>
    /// <typeparam name="Table">The type of the table.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> OrderBy<Table>(Expression<Func<Table, object>> fields)
    {
        return this.OrderByInternal(fields);
    }

    /// <summary>
    /// Orders the by.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> OrderBy<Table1, Table2>(Expression<Func<Table1, Table2, object>> fields)
    {
        return this.OrderByInternal(fields);
    }

    /// <summary>
    /// Orders the by.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> OrderBy<Table1, Table2, Table3>(Expression<Func<Table1, Table2, Table3, object>> fields)
    {
        return this.OrderByInternal(fields);
    }

    /// <summary>
    /// Orders the by.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <typeparam name="Table4">The type of the table4.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> OrderBy<Table1, Table2, Table3, Table4>(Expression<Func<Table1, Table2, Table3, Table4, object>> fields)
    {
        return this.OrderByInternal(fields);
    }

    /// <summary>
    /// Orders the by.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <typeparam name="Table4">The type of the table4.</typeparam>
    /// <typeparam name="Table5">The type of the table5.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> OrderBy<Table1, Table2, Table3, Table4, Table5>(Expression<Func<Table1, Table2, Table3, Table4, Table5, object>> fields)
    {
        return this.OrderByInternal(fields);
    }

    /// <summary>
    /// Orders the by internal.
    /// </summary>
    /// <param name="expr">The expr.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    private SqlExpression<T> OrderByInternal(Expression expr)
    {
        this.Reset(this.Sep = string.Empty);

        this.orderByProperties.Clear();
        var orderBySql = this.Visit(expr);
        if (IsSqlClass(orderBySql))
        {
            var fields = orderBySql.ToString();
            this.orderByProperties.Add(fields);
            this.BuildOrderByClauseInternal();
        }
        else if (orderBySql is string strExpr)
        {
            return this.GroupBy(this.ListExpression(expr, strExpr));
        }

        return this;
    }

    /// <summary>
    /// Determines whether [is SQL class] [the specified object].
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>bool.</returns>
    public static bool IsSqlClass(object obj)
    {
        return obj != null &&
               (obj is PartialSqlString ||
                obj is SelectList);
    }

    /// <summary>
    /// Thens the by.
    /// </summary>
    /// <param name="orderBy">The order by.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> ThenBy(string orderBy)
    {
        orderBy.SqlVerifyFragment();
        this.orderByProperties.Add(orderBy);
        this.BuildOrderByClauseInternal();
        return this;
    }

    /// <summary>
    /// Thens the by.
    /// </summary>
    /// <param name="keySelector">The key selector.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> ThenBy(Expression<Func<T, object>> keySelector)
    {
        return this.ThenByInternal(keySelector);
    }

    /// <summary>
    /// Thens the by.
    /// </summary>
    /// <typeparam name="Table">The type of the table.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> ThenBy<Table>(Expression<Func<Table, object>> fields)
    {
        return this.ThenByInternal(fields);
    }

    /// <summary>
    /// Thens the by.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> ThenBy<Table1, Table2>(Expression<Func<Table1, Table2, object>> fields)
    {
        return this.ThenByInternal(fields);
    }

    /// <summary>
    /// Thens the by.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> ThenBy<Table1, Table2, Table3>(Expression<Func<Table1, Table2, Table3, object>> fields)
    {
        return this.ThenByInternal(fields);
    }

    /// <summary>
    /// Thens the by.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <typeparam name="Table4">The type of the table4.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> ThenBy<Table1, Table2, Table3, Table4>(Expression<Func<Table1, Table2, Table3, Table4, object>> fields)
    {
        return this.ThenByInternal(fields);
    }

    /// <summary>
    /// Thens the by.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <typeparam name="Table4">The type of the table4.</typeparam>
    /// <typeparam name="Table5">The type of the table5.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> ThenBy<Table1, Table2, Table3, Table4, Table5>(Expression<Func<Table1, Table2, Table3, Table4, Table5, object>> fields)
    {
        return this.ThenByInternal(fields);
    }

    /// <summary>
    /// Thens the by internal.
    /// </summary>
    /// <param name="keySelector">The key selector.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    private SqlExpression<T> ThenByInternal(Expression keySelector)
    {
        this.Reset(this.Sep = string.Empty);

        var orderBySql = this.Visit(keySelector);
        if (IsSqlClass(orderBySql))
        {
            var fields = orderBySql.ToString();
            this.orderByProperties.Add(fields);
            this.BuildOrderByClauseInternal();
        }

        return this;
    }

    /// <summary>
    /// Orders the by descending.
    /// </summary>
    /// <param name="keySelector">The key selector.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> OrderByDescending(Expression<Func<T, object>> keySelector)
    {
        return this.OrderByDescendingInternal(keySelector);
    }

    /// <summary>
    /// Orders the by descending.
    /// </summary>
    /// <typeparam name="Table">The type of the table.</typeparam>
    /// <param name="keySelector">The key selector.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> OrderByDescending<Table>(Expression<Func<Table, object>> keySelector)
    {
        return this.OrderByDescendingInternal(keySelector);
    }

    /// <summary>
    /// Orders the by descending.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> OrderByDescending<Table1, Table2>(Expression<Func<Table1, Table2, object>> fields)
    {
        return this.OrderByDescendingInternal(fields);
    }

    /// <summary>
    /// Orders the by descending.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> OrderByDescending<Table1, Table2, Table3>(Expression<Func<Table1, Table2, Table3, object>> fields)
    {
        return this.OrderByDescendingInternal(fields);
    }

    /// <summary>
    /// Orders the by descending.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <typeparam name="Table4">The type of the table4.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> OrderByDescending<Table1, Table2, Table3, Table4>(Expression<Func<Table1, Table2, Table3, Table4, object>> fields)
    {
        return this.OrderByDescendingInternal(fields);
    }

    /// <summary>
    /// Orders the by descending.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <typeparam name="Table4">The type of the table4.</typeparam>
    /// <typeparam name="Table5">The type of the table5.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> OrderByDescending<Table1, Table2, Table3, Table4, Table5>(Expression<Func<Table1, Table2, Table3, Table4, Table5, object>> fields)
    {
        return this.OrderByDescendingInternal(fields);
    }

    /// <summary>
    /// Orders the by descending internal.
    /// </summary>
    /// <param name="keySelector">The key selector.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    private SqlExpression<T> OrderByDescendingInternal(Expression keySelector)
    {
        this.Reset(this.Sep = string.Empty);

        this.orderByProperties.Clear();
        var orderBySql = this.Visit(keySelector);
        if (IsSqlClass(orderBySql))
        {
            var fields = orderBySql.ToString();
            fields.ParseTokens()
                .Each(x => this.orderByProperties.Add(x + " DESC"));
            this.BuildOrderByClauseInternal();
        }

        return this;
    }

    /// <summary>
    /// Orders the by descending.
    /// </summary>
    /// <param name="orderBy">The order by.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> OrderByDescending(string orderBy)
    {
        return this.UnsafeOrderByDescending(orderBy.SqlVerifyFragment());
    }

    /// <summary>
    /// Orders the by descending.
    /// </summary>
    /// <param name="columnIndex">Index of the column.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> OrderByDescending(long columnIndex)
    {
        return this.UnsafeOrderByDescending(columnIndex.ToString());
    }

    /// <summary>
    /// Unsafes the order by descending.
    /// </summary>
    /// <param name="orderBy">The order by.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    private SqlExpression<T> UnsafeOrderByDescending(string orderBy)
    {
        this.orderByProperties.Clear();
        this.orderByProperties.Add(orderBy + " DESC");
        this.BuildOrderByClauseInternal();
        return this;
    }

    /// <summary>
    /// Thens the by descending.
    /// </summary>
    /// <param name="orderBy">The order by.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> ThenByDescending(string orderBy)
    {
        orderBy.SqlVerifyFragment();
        this.orderByProperties.Add(orderBy + " DESC");
        this.BuildOrderByClauseInternal();
        return this;
    }

    /// <summary>
    /// Thens the by descending.
    /// </summary>
    /// <param name="keySelector">The key selector.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> ThenByDescending(Expression<Func<T, object>> keySelector)
    {
        return this.ThenByDescendingInternal(keySelector);
    }

    /// <summary>
    /// Thens the by descending.
    /// </summary>
    /// <typeparam name="Table">The type of the table.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> ThenByDescending<Table>(Expression<Func<Table, object>> fields)
    {
        return this.ThenByDescendingInternal(fields);
    }

    /// <summary>
    /// Thens the by descending.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> ThenByDescending<Table1, Table2>(Expression<Func<Table1, Table2, object>> fields)
    {
        return this.ThenByDescendingInternal(fields);
    }

    /// <summary>
    /// Thens the by descending.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> ThenByDescending<Table1, Table2, Table3>(Expression<Func<Table1, Table2, Table3, object>> fields)
    {
        return this.ThenByDescendingInternal(fields);
    }

    /// <summary>
    /// Thens the by descending.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <typeparam name="Table4">The type of the table4.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> ThenByDescending<Table1, Table2, Table3, Table4>(Expression<Func<Table1, Table2, Table3, Table4, object>> fields)
    {
        return this.ThenByDescendingInternal(fields);
    }

    /// <summary>
    /// Thens the by descending.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <typeparam name="Table4">The type of the table4.</typeparam>
    /// <typeparam name="Table5">The type of the table5.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> ThenByDescending<Table1, Table2, Table3, Table4, Table5>(Expression<Func<Table1, Table2, Table3, Table4, Table5, object>> fields)
    {
        return this.ThenByDescendingInternal(fields);
    }

    /// <summary>
    /// Thens the by descending internal.
    /// </summary>
    /// <param name="keySelector">The key selector.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    private SqlExpression<T> ThenByDescendingInternal(Expression keySelector)
    {
        this.Reset(this.Sep = string.Empty);

        var orderBySql = this.Visit(keySelector);
        if (IsSqlClass(orderBySql))
        {
            var fields = orderBySql.ToString();
            fields.ParseTokens()
                .Each(x => this.orderByProperties.Add(x + " DESC"));
            this.BuildOrderByClauseInternal();
        }

        return this;
    }

    /// <summary>
    /// Builds the order by clause internal.
    /// </summary>
    private void BuildOrderByClauseInternal()
    {
        if (this.orderByProperties.Count > 0)
        {
            var sb = StringBuilderCache.Allocate();
            foreach (var prop in this.orderByProperties)
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }

                sb.Append(prop);
            }

            this.orderBy = "ORDER BY " + StringBuilderCache.ReturnAndFree(sb);
        }
        else
        {
            this.orderBy = null;
        }
    }

    /// <summary>
    /// Offset of the first row to return. The offset of the initial row is 0
    /// </summary>
    /// <param name="skip">The skip.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Skip(int? skip = null)
    {
        this.Offset = skip;
        return this;
    }

    /// <summary>
    /// Number of rows returned by a SELECT statement
    /// </summary>
    /// <param name="take">The take.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Take(int? take = null)
    {
        this.Rows = take;
        return this;
    }

    /// <summary>
    /// Set the specified offset and rows for SQL Limit clause.
    /// </summary>
    /// <param name="skip">Offset of the first row to return. The offset of the initial row is 0</param>
    /// <param name="rows">Number of rows returned by a SELECT statement</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Limit(int skip, int rows)
    {
        this.Offset = skip;
        this.Rows = rows;
        return this;
    }

    /// <summary>
    /// Set the specified offset and rows for SQL Limit clause where they exist.
    /// </summary>
    /// <param name="skip">Offset of the first row to return. The offset of the initial row is 0</param>
    /// <param name="rows">Number of rows returned by a SELECT statement</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Limit(int? skip, int? rows)
    {
        this.Offset = skip;
        this.Rows = rows;
        return this;
    }

    /// <summary>
    /// Set the specified rows for Sql Limit clause.
    /// </summary>
    /// <param name="rows">Number of rows returned by a SELECT statement</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Limit(int rows)
    {
        this.Offset = null;
        this.Rows = rows;
        return this;
    }

    /// <summary>
    /// Clear Sql Limit clause
    /// </summary>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Limit()
    {
        this.Offset = null;
        this.Rows = null;
        return this;
    }

    /// <summary>
    /// Clear Offset and Limit clauses. Alias for Limit()
    /// </summary>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> ClearLimits()
    {
        return this.Limit();
    }

    /// <summary>
    /// Fields to be updated.
    /// </summary>
    /// <param name="updateFields">The update fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Update(List<string> updateFields)
    {
        this.UpdateFields = updateFields;
        return this;
    }

    /// <summary>
    /// Fields to be updated.
    /// </summary>
    /// <param name="updateFields">The update fields.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Update(IEnumerable<string> updateFields)
    {
        this.UpdateFields = [..updateFields];
        return this;
    }

    /// <summary>
    /// Fields to be updated.
    /// </summary>
    /// <param name="fields">x=&gt; x.SomeProperty1 or x=&gt; new { x.SomeProperty1, x.SomeProperty2 }</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Update(Expression<Func<T, object>> fields)
    {
        this.Reset(this.Sep = string.Empty, this.useFieldName = false);
        this.UpdateFields = [.. fields.GetFieldNames()];
        return this;
    }

    /// <summary>
    /// Clear UpdateFields list ( all fields will be updated)
    /// </summary>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Update()
    {
        this.UpdateFields = [];
        return this;
    }

    /// <summary>
    /// Fields to be inserted.
    /// </summary>
    /// <typeparam name="TKey">objectWithProperties</typeparam>
    /// <param name="fields">x=&gt; x.SomeProperty1 or x=&gt; new{ x.SomeProperty1, x.SomeProperty2}</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Insert<TKey>(Expression<Func<T, TKey>> fields)
    {
        this.Reset(this.Sep = string.Empty, this.useFieldName = false);
        var fieldList = this.Visit(fields);
        this.InsertFields = [.. fieldList.ToString().Split(',').Select(f => f.Trim())];
        return this;
    }

    /// <summary>
    /// fields to be inserted.
    /// </summary>
    /// <param name="insertFields">IList&lt;string&gt; containing Names of properties to be inserted</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Insert(List<string> insertFields)
    {
        this.InsertFields = insertFields;
        return this;
    }

    /// <summary>
    /// Clear InsertFields list ( all fields will be inserted)
    /// </summary>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Insert()
    {
        this.InsertFields = [];
        return this;
    }

    /// <summary>
    /// Withes the SQL filter.
    /// </summary>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <returns>ServiceStack.OrmLite.SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> WithSqlFilter(Func<string, string> sqlFilter)
    {
        this.SqlFilter = sqlFilter;
        return this;
    }

    /// <summary>
    /// SQLs the table.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>string.</returns>
    public string SqlTable(ModelDefinition modelDef)
    {
        return this.DialectProvider.GetQuotedTableName(modelDef);
    }

    public string SqlColumn(FieldDefinition fieldDef)
    {
        return this.DialectProvider.GetQuotedColumnName(fieldDef);
    }

    /// <summary>
    /// SQLs the column.
    /// </summary>
    /// <param name="columnName">Name of the column.</param>
    /// <returns>string.</returns>
    public string SqlColumn(string columnName)
    {
        return this.DialectProvider.GetQuotedColumnName(columnName);
    }

    /// <summary>
    /// Adds the parameter.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Data.IDbDataParameter.</returns>
    public virtual IDbDataParameter AddParam(object value)
    {
        var paramName = this.Params.Count.ToString();
        var paramValue = value;

        var parameter = this.CreateParam(paramName, paramValue);
        this.DialectProvider.InitQueryParam(parameter);
        this.Params.Add(parameter);
        return parameter;
    }

    /// <summary>
    /// Converts to parameter.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>string.</returns>
    public string ConvertToParam(object value)
    {
        var p = this.AddParam(value);
        return p.ParameterName;
    }

    /// <summary>
    /// Copies the parameters to.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    public virtual void CopyParamsTo(IDbCommand dbCmd)
    {
        try
        {
            foreach (var sqlParam in this.Params)
            {
                dbCmd.Parameters.Add(sqlParam);
            }
        }
        catch (Exception)
        {
            // SQL Server + PostgreSql doesn't allow re-using db params in multiple queries
            foreach (var sqlParam in this.Params)
            {
                var p = dbCmd.CreateParameter();
                p.PopulateWith(sqlParam);
                dbCmd.Parameters.Add(p);
            }
        }
    }

    /// <summary>
    /// Converts to deleterowstatement.
    /// </summary>
    /// <returns>string.</returns>
    public virtual string ToDeleteRowStatement()
    {
        string sql;
        var hasTableJoin = this.tableDefs.Count > 1;
        if (hasTableJoin)
        {
            var clone = this.Clone();
            var pk = this.DialectProvider.GetQuotedColumnName(this.modelDef, this.modelDef.PrimaryKey);
            clone.Select(pk);
            var subSql = clone.ToSelectStatement(QueryType.Select);
            sql = $"DELETE FROM {this.DialectProvider.GetQuotedTableName(this.modelDef)} WHERE {pk} IN ({subSql})";
        }
        else
        {
            sql = $"DELETE FROM {this.DialectProvider.GetQuotedTableName(this.modelDef)} {this.WhereExpression}";
        }

        return this.SqlFilter != null
                   ? this.SqlFilter(sql)
                   : sql;
    }

    /// <summary>
    /// Prepares the update statement.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="item">The item.</param>
    /// <param name="excludeDefaults">The exclude defaults.</param>
    /// <exception cref="ArgumentException">$"No non-null or non-default values were provided for type: {typeof(T).Name}</exception>
    public virtual void PrepareUpdateStatement(IDbCommand dbCmd, T item, bool excludeDefaults = false)
    {
        this.CopyParamsTo(dbCmd);

        var setFields = StringBuilderCache.Allocate();

        foreach (var fieldDef in this.modelDef.FieldDefinitions)
        {
            if (fieldDef.ShouldSkipUpdate())
            {
                continue;
            }

            if (fieldDef.IsRowVersion)
            {
                continue;
            }

            if (this.UpdateFields.Count > 0
                && !this.UpdateFields.Contains(fieldDef.Name))
            {
                continue; // added
            }

            var value = fieldDef.GetValue(item);
            if (excludeDefaults
                && (value == null || !fieldDef.IsNullable && value.Equals(value.GetType().GetDefaultValue())))
            {
                continue;
            }

            if (setFields.Length > 0)
            {
                setFields.Append(", ");
            }

            setFields
                .Append(this.DialectProvider.GetQuotedColumnName(fieldDef))
                .Append('=')
                .Append(this.DialectProvider.GetUpdateParam(dbCmd, value, fieldDef));
        }

        if (setFields.Length == 0)
        {
            throw new ArgumentException($"No non-null or non-default values were provided for type: {typeof(T).Name}");
        }

        var sql = $"UPDATE {this.DialectProvider.GetQuotedTableName(this.modelDef)} " +
                  $"SET {StringBuilderCache.ReturnAndFree(setFields)} {this.WhereExpression}";

        dbCmd.CommandText = this.SqlFilter != null
                                ? this.SqlFilter(sql)
                                : sql;
    }

    /// <summary>
    /// Prepares the update statement.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="updateFields">The update fields.</param>
    /// <exception cref="ArgumentException">$"No non-null or non-default values were provided for type: {typeof(T).Name}</exception>
    public virtual void PrepareUpdateStatement(IDbCommand dbCmd, Dictionary<string, object> updateFields)
    {
        this.CopyParamsTo(dbCmd);

        var setFields = StringBuilderCache.Allocate();

        foreach (var entry in updateFields)
        {
            var fieldDef = this.ModelDef.AssertFieldDefinition(entry.Key);
            if (fieldDef.ShouldSkipUpdate())
            {
                continue;
            }

            if (fieldDef.IsRowVersion)
            {
                continue;
            }

            if (this.UpdateFields.Count > 0
                && !this.UpdateFields.Contains(fieldDef.Name)) // added
            {
                continue;
            }

            var value = entry.Value;
            if (value == null && !fieldDef.IsNullable)
            {
                continue;
            }

            if (setFields.Length > 0)
            {
                setFields.Append(", ");
            }

            setFields
                .Append(this.DialectProvider.GetQuotedColumnName(fieldDef))
                .Append('=')
                .Append(this.DialectProvider.GetUpdateParam(dbCmd, value, fieldDef));
        }

        if (setFields.Length == 0)
        {
            throw new ArgumentException($"No non-null or non-default values were provided for type: {typeof(T).Name}");
        }

        var sql = $"UPDATE {this.DialectProvider.GetQuotedTableName(this.modelDef)} " +
                  $"SET {StringBuilderCache.ReturnAndFree(setFields)} {this.WhereExpression}";

        dbCmd.CommandText = this.SqlFilter != null
                                ? this.SqlFilter(sql)
                                : sql;
    }

    /// <summary>
    /// Converts to selectstatement.
    /// </summary>
    /// <returns>string.</returns>
    public virtual string ToSelectStatement()
    {
        return this.ToSelectStatement(QueryType.Select);
    }

    /// <summary>
    /// Converts to selectstatement.
    /// </summary>
    /// <param name="forType">For type.</param>
    /// <returns>string.</returns>
    public virtual string ToSelectStatement(QueryType forType)
    {
        SelectFilter?.Invoke(this);
        OrmLiteConfig.SqlExpressionSelectFilter?.Invoke(this.GetUntyped());

        var sql = this.DialectProvider
            .ToSelectStatement(forType, this.modelDef, this.SelectExpression, this.BodyExpression, this.OrderByExpression, offset: this.Offset, rows: this.Rows, this.Tags);

        return this.SqlFilter != null
                   ? this.SqlFilter(sql)
                   : sql;
    }

    /// <summary>
    /// Merge params into an encapsulated SQL Statement with embedded param values
    /// </summary>
    /// <returns>string.</returns>
    public virtual string ToMergedParamsSelectStatement()
    {
        var sql = this.ToSelectStatement(QueryType.Select);
        var mergedSql = this.DialectProvider.MergeParamsIntoSql(sql, this.Params);
        return mergedSql;
    }

    /// <summary>
    /// Converts to countstatement.
    /// </summary>
    /// <returns>string.</returns>
    public virtual string ToCountStatement()
    {
        SelectFilter?.Invoke(this);
        OrmLiteConfig.SqlExpressionSelectFilter?.Invoke(this.GetUntyped());

        var sql = "SELECT COUNT(*)" + this.BodyExpression;

        return this.SqlFilter != null
                   ? this.SqlFilter(sql)
                   : sql;
    }

    /// <summary>
    /// Gets or sets the select expression.
    /// </summary>
    /// <value>The select expression.</value>
    public string SelectExpression
    {
        get
        {
            if (string.IsNullOrEmpty(this.selectExpression))
            {
                this.BuildSelectExpression(string.Empty, false);
            }

            return this.selectExpression;
        }

        set => this.selectExpression = value;
    }

    /// <summary>
    /// Gets or sets from expression.
    /// </summary>
    /// <value>From expression.</value>
    public string FromExpression
    {
        get => string.IsNullOrEmpty(this.fromExpression)
                   ? " \nFROM " + this.DialectProvider.GetQuotedTableName(this.modelDef) + (this.TableAlias != null ? " " + this.DialectProvider.GetQuotedName(this.TableAlias) : string.Empty)
                   : this.fromExpression;
        set => this.fromExpression = value;
    }

    /// <summary>
    /// Gets the body expression.
    /// </summary>
    /// <value>The body expression.</value>
    public string BodyExpression =>
        this.FromExpression
        + (string.IsNullOrEmpty(this.WhereExpression) ? string.Empty : "\n" + this.WhereExpression)
        + (string.IsNullOrEmpty(this.GroupByExpression) ? string.Empty : "\n" + this.GroupByExpression)
        + (string.IsNullOrEmpty(this.HavingExpression) ? string.Empty : "\n" + this.HavingExpression);

    /// <summary>
    /// Gets or sets the where expression.
    /// </summary>
    /// <value>The where expression.</value>
    public string WhereExpression { get; set; }

    /// <summary>
    /// Gets or sets the group by expression.
    /// </summary>
    /// <value>The group by expression.</value>
    public string GroupByExpression { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the having expression.
    /// </summary>
    /// <value>The having expression.</value>
    public string HavingExpression { get; set; }

    /// <summary>
    /// Gets or sets the order by expression.
    /// </summary>
    /// <value>The order by expression.</value>
    public string OrderByExpression
    {
        get => string.IsNullOrEmpty(this.orderBy) ? string.Empty : "\n" + this.orderBy;
        set => this.orderBy = value;
    }

    /// <summary>
    /// Gets or sets the model definition.
    /// </summary>
    /// <value>The model definition.</value>
    public ModelDefinition ModelDef
    {
        get => this.modelDef;
        protected set => this.modelDef = value;
    }

    /// <summary>
    /// Gets or sets the name of the use field.
    /// </summary>
    /// <value>The name of the use field.</value>
    protected internal bool UseFieldName
    {
        get => this.useFieldName;
        set => this.useFieldName = value;
    }

    /// <summary>
    /// Visits the specified exp.
    /// </summary>
    /// <param name="exp">The exp.</param>
    /// <returns>object.</returns>
    public virtual object Visit(Expression exp)
    {
        if (exp == null)
        {
            return string.Empty;
        }

        switch (exp.NodeType)
        {
            case ExpressionType.Lambda:
                return this.VisitLambda(exp as LambdaExpression);
            case ExpressionType.MemberAccess:
                return this.VisitMemberAccess(exp as MemberExpression);
            case ExpressionType.Constant:
                return this.VisitConstant(exp as ConstantExpression);
            case ExpressionType.Add:
            case ExpressionType.AddChecked:
            case ExpressionType.Subtract:
            case ExpressionType.SubtractChecked:
            case ExpressionType.Multiply:
            case ExpressionType.MultiplyChecked:
            case ExpressionType.Divide:
            case ExpressionType.Modulo:
            case ExpressionType.AndAlso:
            case ExpressionType.OrElse:
            case ExpressionType.LessThan:
            case ExpressionType.LessThanOrEqual:
            case ExpressionType.GreaterThan:
            case ExpressionType.GreaterThanOrEqual:
            case ExpressionType.Equal:
            case ExpressionType.NotEqual:
            case ExpressionType.Coalesce:
            case ExpressionType.ArrayIndex:
            case ExpressionType.And:
            case ExpressionType.Or:
            case ExpressionType.ExclusiveOr:
            case ExpressionType.RightShift:
            case ExpressionType.LeftShift:
                // return "(" + VisitBinary(exp as BinaryExpression) + ")";
                return this.VisitBinary(exp as BinaryExpression);
            case ExpressionType.Negate:
            case ExpressionType.NegateChecked:
            case ExpressionType.Not:
            case ExpressionType.Convert:
            case ExpressionType.ConvertChecked:
            case ExpressionType.ArrayLength:
            case ExpressionType.Quote:
            case ExpressionType.TypeAs:
                return this.VisitUnary(exp as UnaryExpression);
            case ExpressionType.Parameter:
                return this.VisitParameter(exp as ParameterExpression);
            case ExpressionType.Call:
                return this.VisitMethodCall(exp as MethodCallExpression);
            case ExpressionType.Invoke:
                return this.VisitInvocation(exp as InvocationExpression);
            case ExpressionType.New:
                return this.VisitNew(exp as NewExpression);
            case ExpressionType.NewArrayInit:
            case ExpressionType.NewArrayBounds:
                return this.VisitNewArray(exp as NewArrayExpression);
            case ExpressionType.MemberInit:
                return this.VisitMemberInit(exp as MemberInitExpression);
            case ExpressionType.Index:
                return this.VisitIndexExpression(exp as IndexExpression);
            case ExpressionType.Conditional:
                return this.VisitConditional(exp as ConditionalExpression);
            default:
                return exp.ToString();
        }
    }

    /// <summary>
    /// Visits the join.
    /// </summary>
    /// <param name="exp">The exp.</param>
    /// <returns>object.</returns>
    protected virtual object VisitJoin(Expression exp)
    {
        this.skipParameterizationForThisExpression = true;
        var visitedExpression = this.Visit(exp);
        this.skipParameterizationForThisExpression = false;
        return visitedExpression;
    }

    /// <summary>
    /// Visits the lambda.
    /// </summary>
    /// <param name="lambda">The lambda.</param>
    /// <returns>object.</returns>
    protected virtual object VisitLambda(LambdaExpression lambda)
    {
        this.originalLambda ??= lambda;

        if (lambda.Body.NodeType == ExpressionType.MemberAccess && this.Sep == " ")
        {
            var m = lambda.Body as MemberExpression;

            if (m.Expression != null)
            {
                var r = this.VisitMemberAccess(m);
                if (r is not PartialSqlString)
                {
                    return r;
                }

                if (m.Expression.Type.IsNullableType())
                {
                    return r.ToString();
                }

                return $"{r}={this.GetQuotedTrueValue()}";
            }
        }
        else if (lambda.Body.NodeType == ExpressionType.Conditional && this.Sep == " ")
        {
            var c = lambda.Body as ConditionalExpression;

            var r = this.VisitConditional(c);
            if (r is not PartialSqlString)
            {
                return r;
            }

            return $"{r}={this.GetQuotedTrueValue()}";
        }

        return this.Visit(lambda.Body);
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="type">The type.</param>
    /// <returns>object.</returns>
    public virtual object GetValue(object value, Type type)
    {
        if (this.skipParameterizationForThisExpression)
        {
            return this.DialectProvider.GetQuotedValue(value, type);
        }

        var paramValue = this.DialectProvider.GetParamValue(value, type);
        return paramValue ?? PartialSqlString.Null;
    }

    protected bool IsNull(object expr)
    {
        return expr == null || PartialSqlString.Null.Equals(expr);
    }

    /// <summary>
    /// Visits the binary.
    /// </summary>
    /// <param name="b">The b.</param>
    /// <returns>object.</returns>
    protected virtual object VisitBinary(BinaryExpression b)
    {
        object originalLeft = null, originalRight = null, left, right;
        var operand = this.BindOperant(b.NodeType);   // sep= " " ??
        if (operand is "AND" or "OR")
        {
            if (this.IsBooleanComparison(b.Left))
            {
                left = this.VisitMemberAccess((MemberExpression)b.Left);
                if (left is PartialSqlString)
                {
                    left = new PartialSqlString($"{left}={this.GetQuotedTrueValue()}");
                }
            }
            else if (b.Left is ConditionalExpression expression)
            {
                left = this.VisitConditional(expression);
                if (left is PartialSqlString)
                {
                    left = new PartialSqlString($"{left}={this.GetQuotedTrueValue()}");
                }
            }
            else
            {
                left = this.Visit(b.Left);
            }

            if (this.IsBooleanComparison(b.Right))
            {
                right = this.VisitMemberAccess((MemberExpression)b.Right);
                if (right is PartialSqlString)
                {
                    right = new PartialSqlString($"{right}={this.GetQuotedTrueValue()}");
                }
            }
            else if (b.Right is ConditionalExpression expression)
            {
                right = this.VisitConditional(expression);
                if (right is PartialSqlString)
                {
                    right = new PartialSqlString($"{right}={this.GetQuotedTrueValue()}");
                }
            }
            else
            {
                right = this.Visit(b.Right);
            }

            if (left is not PartialSqlString && right is not PartialSqlString)
            {
                var result = CachedExpressionCompiler.Evaluate(this.PreEvaluateBinary(b, left, right));
                return result;
            }

            if (left is not PartialSqlString)
            {
                left = (bool)left ? this.GetTrueExpression() : this.GetFalseExpression();
            }

            if (right is not PartialSqlString)
            {
                right = (bool)right ? this.GetTrueExpression() : this.GetFalseExpression();
            }
        }
        else if (operand is "=" or "<>" && b.Left is MethodCallExpression methodExpr && methodExpr.Method.Name == "CompareString")
        {
            // Handle VB.NET converting (x => x.Name == "Foo") into (x => CompareString(x.Name, "Foo", False)
            var args = this.VisitExpressionList(methodExpr.Arguments);
            right = this.GetValue(args[1], typeof(string));
            this.ConvertToPlaceholderAndParameter(ref right);
            return new PartialSqlString($"({args[0]} {operand} {right})");
        }
        else
        {
            originalLeft = left = this.Visit(b.Left);
            originalRight = right = this.Visit(b.Right);

            // Handle "expr = true/false", including with the constant on the left
            if (operand is "=" or "<>")
            {
                if (left is bool)
                {
                    Swap(ref left, ref right); // Should be safe to swap for equality/inequality checks
                }

                if (right is bool && this.IsNull(left))
                {
                    if (operand == "=")
                    {
                        return false; // "null == true/false" becomes "false"
                    }

                    if (operand == "<>")
                    {
                        return true; // "null != true/false" becomes "true"
                    }
                }

                if (right is bool rightBool && !this.IsFieldName(left) && b.Left is not ConditionalExpression)
                {
                    // Don't change anything when "expr" is a column name or ConditionalExpression - then we really want "ColName = 1" or (Case When 1=0 Then 1 Else 0 End = 1)
                    if (operand == "=")
                    {
                        return rightBool ? left : this.GetNotValue(left); // "expr == true" becomes "expr", "expr == false" becomes "not (expr)"
                    }

                    if (operand == "<>")
                    {
                        return rightBool ? this.GetNotValue(left) : left; // "expr != true" becomes "not (expr)", "expr != false" becomes "expr"
                    }
                }
            }

            var leftEnum = left as EnumMemberAccess;
            //The real type should be read when a non-direct member is accessed. For example Sql.TableAlias(x.State, "p"),alias conversion should be performed when "x.State" is an enum
            if (leftEnum == null && left is PartialSqlString pss && pss.EnumMember != null)
            {
                leftEnum = pss.EnumMember;
            }

            var rightEnum = right as EnumMemberAccess;

            var rightNeedsCoercing = leftEnum != null && rightEnum == null;
            var leftNeedsCoercing = rightEnum != null && leftEnum == null;

            if (rightNeedsCoercing)
            {
                if (right is not PartialSqlString)
                {
                    right = this.GetValue(right, leftEnum.EnumType);
                }
            }
            else if (leftNeedsCoercing)
            {
                if (left is not PartialSqlString)
                {
                    left = this.DialectProvider.GetQuotedValue(left, rightEnum.EnumType);
                }
            }
            else if (left is not PartialSqlString && right is not PartialSqlString)
            {
                var evaluatedValue = CachedExpressionCompiler.Evaluate(this.PreEvaluateBinary(b, left, right));
                var result = this.VisitConstant(Expression.Constant(evaluatedValue));
                return result;
            }
            else if (left is not PartialSqlString)
            {
                //left = DialectProvider.GetQuotedValue(left, left?.GetType());
                left = this.GetValue(left, left?.GetType());
            }
            else if (right is not PartialSqlString)
            {
                right = this.GetValue(right, right?.GetType());
            }
        }

        if (this.IsNull(left))
        {
            Swap(ref left, ref right); // "null is x" will not work, so swap the operands
        }

        var separator = this.Sep;
        if (this.IsNull(right))
        {
            if (operand == "=")
            {
                operand = "is";
            }
            else if (operand == "<>")
            {
                operand = "is not";
            }

            separator = " ";
        }

        if (operand == "+" && b.Left.Type == typeof(string) && b.Right.Type == typeof(string))
        {
            return this.BuildConcatExpression([left, right]);
        }

        this.VisitFilter(operand, originalLeft, originalRight, ref left, ref right);

        return operand switch
        {
            "MOD" => new PartialSqlString(this.GetModExpression(b, left.ToString(), right.ToString())),
            "COALESCE" => new PartialSqlString(this.GetCoalesceExpression(b, left.ToString(), right.ToString())),
            _ => new PartialSqlString("(" + left + separator + operand + separator + right + ")")
        };
    }

    protected virtual string GetModExpression(BinaryExpression b, object left, object right)
    {
        return $"MOD({left},{right})";
    }

    /// <summary>
    /// Gets the coalesce expression.
    /// </summary>
    /// <param name="b">The b.</param>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>System.String.</returns>
    protected virtual string GetCoalesceExpression(BinaryExpression b, object left, object right)
    {
        return $"COALESCE({left},{right})";
    }

    /// <summary>
    /// Pres the evaluate binary.
    /// </summary>
    /// <param name="b">The b.</param>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>System.Linq.Expressions.BinaryExpression.</returns>
    private BinaryExpression PreEvaluateBinary(BinaryExpression b, object left, object right)
    {
        var visitedBinaryExp = b;

        if (this.IsParameterAccess(b.Left) || this.IsParameterAccess(b.Right))
        {
            var eLeft = !this.IsParameterAccess(b.Left) ? b.Left : Expression.Constant(left, b.Left.Type);
            var eRight = !this.IsParameterAccess(b.Right) ? b.Right : Expression.Constant(right, b.Right.Type);
            if (b.NodeType == ExpressionType.Coalesce)
            {
                visitedBinaryExp = Expression.Coalesce(eLeft, eRight, b.Conversion);
            }
            else
            {
                visitedBinaryExp = Expression.MakeBinary(b.NodeType, eLeft, eRight, b.IsLiftedToNull, b.Method);
            }
        }

        return visitedBinaryExp;
    }

    /// <summary>
    /// Determines whether the expression is the parameter inside MemberExpression which should be compared with TrueExpression.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <returns>Returns true if the specified expression is the parameter inside MemberExpression which should be compared with TrueExpression;
    /// otherwise, false.</returns>
    protected virtual bool IsBooleanComparison(Expression e)
    {
        if (e is not MemberExpression m)
        {
            return false;
        }

        if (m.Member.DeclaringType.IsNullableType() &&
            m.Member.Name == "HasValue") // nameof(Nullable<bool>.HasValue)
        {
            return false;
        }

        return this.IsParameterAccess(m);
    }

    /// <summary>
    /// Determines whether the expression is the parameter.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <returns>Returns true if the specified expression is parameter;
    /// otherwise, false.</returns>
    protected virtual bool IsParameterAccess(Expression e)
    {
        return this.CheckExpressionForTypes(e, [ExpressionType.Parameter]);
    }

    /// <summary>
    /// Determines whether the expression is a Parameter or Convert Expression.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <returns>Returns true if the specified expression is parameter or convert;
    /// otherwise, false.</returns>
    protected virtual bool IsParameterOrConvertAccess(Expression e)
    {
        return this.CheckExpressionForTypes(e, [ExpressionType.Parameter, ExpressionType.Convert]);
    }

    /// <summary>
    /// Check whether the expression is a constant expression to determine
    /// whether we should use the expression value instead of Column Name
    /// </summary>
    /// <param name="e">The e.</param>
    /// <returns>bool.</returns>
    protected virtual bool IsConstantExpression(Expression e)
    {
        return this.CheckExpressionForTypes(e, [ExpressionType.Constant]);
    }

    /// <summary>
    /// Checks the expression for types.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <param name="types">The types.</param>
    /// <returns>bool.</returns>
    protected bool CheckExpressionForTypes(Expression e, ExpressionType[] types)
    {
        while (e != null)
        {
            if (types.Contains(e.NodeType))
            {
                var subUnaryExpr = e as UnaryExpression;
                var isSubExprAccess = subUnaryExpr?.Operand is IndexExpression;
                if (!isSubExprAccess)
                {
                    return true;
                }
            }

            if (e is BinaryExpression binaryExpr)
            {
                if (this.CheckExpressionForTypes(binaryExpr.Left, types))
                {
                    return true;
                }

                if (this.CheckExpressionForTypes(binaryExpr.Right, types))
                {
                    return true;
                }
            }

            if (e is MethodCallExpression methodCallExpr)
            {
                for (var i = 0; i < methodCallExpr.Arguments.Count; i++)
                {
                    if (this.CheckExpressionForTypes(methodCallExpr.Arguments[i], types))
                    {
                        return true;
                    }
                }

                if (this.CheckExpressionForTypes(methodCallExpr.Object, types))
                {
                    return true;
                }
            }

            if (e is UnaryExpression unaryExpr)
            {
                if (this.CheckExpressionForTypes(unaryExpr.Operand, types))
                {
                    return true;
                }
            }

            if (e is ConditionalExpression condExpr)
            {
                if (this.CheckExpressionForTypes(condExpr.Test, types))
                {
                    return true;
                }

                if (this.CheckExpressionForTypes(condExpr.IfTrue, types))
                {
                    return true;
                }

                if (this.CheckExpressionForTypes(condExpr.IfFalse, types))
                {
                    return true;
                }
            }

            var memberExpr = e as MemberExpression;
            e = memberExpr?.Expression;
        }

        return false;
    }

    /// <summary>
    /// Swaps the specified left.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    private static void Swap(ref object left, ref object right)
    {
        (right, left) = (left, right);
    }

    /// <summary>
    /// Visits the filter.
    /// </summary>
    /// <param name="operand">The operand.</param>
    /// <param name="originalLeft">The original left.</param>
    /// <param name="originalRight">The original right.</param>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    protected virtual void VisitFilter(string operand, object originalLeft, object originalRight, ref object left,
        ref object right)
    {
        if (this.skipParameterizationForThisExpression)
        {
            return;
        }

        if (originalLeft is EnumMemberAccess && originalRight is EnumMemberAccess)
        {
            return;
        }

        if (operand is "AND" or "OR")
        {
            return;
        }

        if (left is not PartialSqlString && left?.ToString() != "null")
        {
            if (!(originalLeft is MemberExpression leftMe && this.IsTableColumn(leftMe)))
            {
                this.ConvertToPlaceholderAndParameter(ref left);
            }
            else
            {
                left = this.DialectProvider.GetQuotedValue(left, left?.GetType());
            }
        }

        if (right is not PartialSqlString && right?.ToString() != "null")
        {
            if (!(originalRight is MemberExpression rightMe && this.IsTableColumn(rightMe)))
            {
                this.ConvertToPlaceholderAndParameter(ref right);
            }
            else
            {
                right = this.DialectProvider.GetQuotedValue(right, right?.GetType());
            }
        }
    }

    /// <summary>
    /// Converts to placeholder and parameter.
    /// </summary>
    /// <param name="right">The right.</param>
    protected virtual void ConvertToPlaceholderAndParameter(ref object right)
    {
        var parameter = this.AddParam(right);

        right = parameter.ParameterName;
    }

    /// <summary>
    /// Visits the member access.
    /// </summary>
    /// <param name="m">The m.</param>
    /// <returns>object.</returns>
    protected virtual object VisitMemberAccess(MemberExpression m)
    {
        if (m.Expression != null)
        {
            if (m.Member.DeclaringType.IsNullableType())
            {
                if (m.Member.Name == nameof(Nullable<bool>.Value))
                {
                    return this.Visit(m.Expression);
                }

                if (m.Member.Name == nameof(Nullable<bool>.HasValue))
                {
                    var doesNotEqualNull = Expression.MakeBinary(ExpressionType.NotEqual, m.Expression, Expression.Constant(null));
                    return this.Visit(doesNotEqualNull); // Nullable<T>.HasValue is equivalent to "!= null"
                }

                throw new ArgumentException($"Expression '{m}' accesses unsupported property '{m.Member}' of Nullable<T>");
            }

            if (m.Member.DeclaringType == typeof(string) &&
                m.Member.Name == nameof(string.Length))
            {
                return this.VisitLengthStringProperty(m);
            }

            if (this.IsParameterOrConvertAccess(m))
            {
                // We don't want to use the Column Name for constant expressions unless we're in a Sql. method call
                if (this.inSqlMethodCall || !this.IsConstantExpression(m))
                {
                    return this.GetMemberExpression(m);
                }
            }
        }

        return CachedExpressionCompiler.Evaluate(m);
    }

    /// <summary>
    /// Determines whether [is table column] [the specified m].
    /// </summary>
    /// <param name="m">The m.</param>
    /// <returns>bool.</returns>
    protected bool IsTableColumn(MemberExpression m)
    {
        var modelType = m.Expression?.Type;
        if (m.Expression?.NodeType == ExpressionType.Convert)
        {
            if (m.Expression is UnaryExpression unaryExpr)
            {
                modelType = unaryExpr.Operand.Type;
            }
        }
        return modelType?.GetModelDefinition() != null;
    }

    /// <summary>
    /// Gets the member expression.
    /// </summary>
    /// <param name="m">The m.</param>
    /// <returns>object.</returns>
    protected virtual object GetMemberExpression(MemberExpression m)
    {
        var propertyInfo = m.Member as PropertyInfo;

        var modelType = m.Expression.Type;
        if (m.Expression.NodeType == ExpressionType.Convert)
        {
            if (m.Expression is UnaryExpression unaryExpr)
            {
                modelType = unaryExpr.Operand.Type;
            }
        }

        var tableDef = modelType.GetModelDefinition();

        var tableAlias = this.GetTableAlias(m, tableDef);
        var columnName = tableAlias == null
                             ? this.GetQuotedColumnName(tableDef, m.Member.Name)
                             : this.GetQuotedColumnName(tableDef, tableAlias, m.Member.Name);

        if (propertyInfo != null && propertyInfo.PropertyType.IsEnum)
        {
            return new EnumMemberAccess(columnName, propertyInfo.PropertyType);
        }

        return new PartialSqlString(columnName);
    }

    /// <summary>
    /// Gets the table alias.
    /// </summary>
    /// <param name="m">The m.</param>
    /// <param name="tableDef">the table definition.</param>
    /// <returns>string.</returns>
    protected virtual string GetTableAlias(MemberExpression m, ModelDefinition tableDef)
    {
        if (this.originalLambda == null)
        {
            return null;
        }

        if (m.Expression is ParameterExpression pe)
        {
            var tableParamName = this.originalLambda.Parameters.Count > 0 && this.originalLambda.Parameters[0].Type == this.ModelDef.ModelType
                                     ? this.originalLambda.Parameters[0].Name
                                     : null;

            if (pe.Type == this.ModelDef.ModelType && pe.Name == tableParamName)
            {
                return this.TableAlias;
            }

            var joinType = this.joinAlias?.ModelDef?.ModelType;
            var joinParamName = this.originalLambda.Parameters.Count > 1 && this.originalLambda.Parameters[1].Type == joinType
                                    ? this.originalLambda.Parameters[1].Name
                                    : null;

            if (pe.Type == joinType && pe.Name == joinParamName)
            {
                return this.joinAlias.Alias;
            }
        }

        if (this.UseJoinTypeAsAliases && this.joinAliases != null && this.joinAliases.TryGetValue(tableDef, out var tableOptions))
        {
            return tableOptions.Alias;
        }

        return null;
    }

    /// <summary>
    /// Visits the member initialize.
    /// </summary>
    /// <param name="exp">The exp.</param>
    /// <returns>object.</returns>
    protected virtual object VisitMemberInit(MemberInitExpression exp)
    {
        return CachedExpressionCompiler.Evaluate(exp);
    }

    /// <summary>
    /// Visits the new.
    /// </summary>
    /// <param name="nex">The nex.</param>
    /// <returns>object.</returns>
    protected virtual object VisitNew(NewExpression nex)
    {
        var isAnonType = nex.Type.Name.StartsWith("<>");
        if (isAnonType)
        {
            var exprs = this.VisitExpressionList(nex.Arguments);

            for (var i = 0; i < exprs.Count; ++i)
            {
                exprs[i] = this.SetAnonTypePropertyNamesForSelectExpression(exprs[i], nex.Arguments[i], nex.Members[i]);
            }

            return new SelectList(exprs);
        }

        return CachedExpressionCompiler.Evaluate(nex);
    }

    /// <summary>
    /// Determines whether [is lambda argument] [the specified expr].
    /// </summary>
    /// <param name="expr">The expr.</param>
    /// <returns>bool.</returns>
    private bool IsLambdaArg(Expression expr)
    {
        return expr switch {
                ParameterExpression pe => this.IsLambdaArg(pe.Name),
                UnaryExpression ue when ue.Operand is ParameterExpression uepe => this.IsLambdaArg(uepe.Name),
                _ => false
            };
    }

    /// <summary>
    /// Determines whether [is lambda argument] [the specified name].
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>bool.</returns>
    private bool IsLambdaArg(string name)
    {
        var args = this.originalLambda?.Parameters;
        if (args != null)
        {
            foreach (var x in args)
            {
                if (x.Name == name)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Sets the anon type property names for select expression.
    /// </summary>
    /// <param name="expr">The expr.</param>
    /// <param name="arg">The argument.</param>
    /// <param name="member">The member.</param>
    /// <returns>object.</returns>
    private object SetAnonTypePropertyNamesForSelectExpression(object expr, Expression arg, MemberInfo member)
    {
        // When selecting a column use the anon type property name, rather than the table property name, as the returned column name
        if (arg is MemberExpression propExpr && this.IsLambdaArg(propExpr.Expression))
        {
            if (this.UseSelectPropertiesAsAliases || // Use anon property alias when explicitly requested
                propExpr.Member.Name != member.Name || // or when names don't match
                propExpr.Expression.Type != this.ModelDef.ModelType)
            {
                return new SelectItemExpression(this.DialectProvider, expr.ToString(), member.Name);
            }
            /*|| // or when selecting a field from a different table
                                member.Name != ModelDef.FieldDefinitions.First(x => x.Name == member.Name).FieldName)*/  // or when name and alias don't match

            return expr;
        }

        // When selecting an entire table use the anon type property name as a prefix for the returned column name
        // to allow the caller to distinguish properties with the same names from different tables
        var selectList = arg is ParameterExpression paramExpr && paramExpr.Name != member.Name
                             ? expr as SelectList
                             : null;
        if (selectList != null)
        {
            foreach (var item in selectList.Items)
            {
                if (item is SelectItem selectItem)
                {
                    if (!string.IsNullOrEmpty(selectItem.Alias))
                    {
                        selectItem.Alias = member.Name + selectItem.Alias;
                    }
                    else
                    {
                        if (item is SelectItemColumn columnItem)
                        {
                            columnItem.Alias = member.Name + columnItem.GetColumnName();
                        }
                    }
                }
            }
        }

        var methodCallExpr = arg as MethodCallExpression;
        var mi = methodCallExpr?.Method;
        var declareType = mi?.DeclaringType;

        if (declareType != null && declareType.Name == nameof(Sql))
        {
            if (mi.Name is nameof(Sql.TableAlias) or nameof(Sql.JoinAlias))
            {
                if (expr is PartialSqlString ps && ps.Text.Contains(','))
                {
                    return ps;                                               // new { buyer = Sql.TableAlias(b, "buyer")
                }

                return new PartialSqlString(expr + " AS " + member.Name);    // new { BuyerName = Sql.TableAlias(b.Name, "buyer") }
            }

            if (mi.Name != nameof(Sql.Desc) && mi.Name != nameof(Sql.Asc) && mi.Name != nameof(Sql.As) && mi.Name != nameof(Sql.AllFields))
            {
                return new PartialSqlString(expr + " AS " + member.Name);    // new { Alias = Sql.Count("*") }
            }
        }

        if (expr is string s && s == Sql.EOT) // new { t1 = Sql.EOT, t2 = "0 EOT" }
        {
            return new PartialSqlString(s);
        }

        if (arg is ConditionalExpression ce ||                           // new { Alias = x.Value > 1 ? 1 : x.Value }
            arg is BinaryExpression be ||                           // new { Alias = x.First + " " + x.Last }
            arg is MemberExpression me ||                           // new { Alias = DateTime.UtcNow }
            arg is ConstantExpression ct)
        {
            // new { Alias = 1 }
            IOrmLiteConverter converter;
            var strExpr = expr is not PartialSqlString && (converter = this.DialectProvider.GetConverterBestMatch(expr.GetType())) != null
                              ? converter.ToQuotedString(expr.GetType(), expr)
                              : expr.ToString();

            return new PartialSqlString(OrmLiteUtils.UnquotedColumnName(strExpr) != member.Name
                                            ? strExpr + " AS " + member.Name
                                            : strExpr);
        }

        var usePropertyAlias = this.UseSelectPropertiesAsAliases ||
                               expr is PartialSqlString p && Equals(p, PartialSqlString.Null); // new { Alias = (DateTime?)null }
        return usePropertyAlias
                   ? new SelectItemExpression(this.DialectProvider, expr.ToString(), member.Name)
                   : expr;
    }

    /// <summary>
    /// Strips the aliases.
    /// </summary>
    /// <param name="selectList">The select list.</param>
    private static void StripAliases(SelectList selectList)
    {
        if (selectList == null)
        {
            return;
        }

        foreach (var item in selectList.Items)
        {
            if (item is SelectItem selectItem)
            {
                selectItem.Alias = null;
            }
            else if (item is PartialSqlString p)
            {
                if (p.Text.Contains(' '))
                {
                    var right = p.Text.RightPart(' ');
                    if (right.StartsWithIgnoreCase("AS "))
                    {
                        p.Text = p.Text.LeftPart(' ');
                    }
                }
            }
        }
    }

    /// <summary>
    /// Class SelectList.
    /// </summary>
    private class SelectList
    {
        /// <summary>
        /// The items
        /// </summary>
        public readonly IEnumerable<object> Items;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlExpression{T}.SelectList" /> class.
        /// </summary>
        /// <param name="items">The items.</param>
        public SelectList(IEnumerable<object> items)
        {
            this.Items = items;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>string.</returns>
        public override string ToString()
        {
            return this.Items.ToSelectString();
        }
    }

    /// <summary>
    /// Visits the parameter.
    /// </summary>
    /// <param name="p">The p.</param>
    /// <returns>object.</returns>
    protected virtual object VisitParameter(ParameterExpression p)
    {
        var paramModelDef = p.Type.GetModelDefinition();
        if (paramModelDef != null)
        {
            var tablePrefix = paramModelDef == this.ModelDef && this.TableAlias != null
                                  ? this.TableAlias
                                  : paramModelDef.ModelName;
            return new SelectList(this.DialectProvider.GetColumnNames(paramModelDef, tablePrefix));
        }

        return p.Name;
    }

    /// <summary>
    /// Visits the constant.
    /// </summary>
    /// <param name="c">The c.</param>
    /// <returns>object.</returns>
    protected virtual object VisitConstant(ConstantExpression c)
    {
        if (c.Value == null)
        {
            return PartialSqlString.Null;
        }

        return c.Value;
    }

    /// <summary>
    /// Visits the unary.
    /// </summary>
    /// <param name="u">The u.</param>
    /// <returns>object.</returns>
    protected virtual object VisitUnary(UnaryExpression u)
    {
        switch (u.NodeType)
        {
            case ExpressionType.Not:
                var o = this.Visit(u.Operand);
                return this.GetNotValue(o);
            case ExpressionType.Convert:
                if (u.Method != null)
                {
                    var e = u.Operand;
                    if (this.IsParameterAccess(e))
                    {
                        return this.Visit(e);
                    }

                    return CachedExpressionCompiler.Evaluate(u);
                }

                break;
        }
        return this.Visit(u.Operand);
    }

    /// <summary>
    /// Visits the index expression.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <returns>object.</returns>
    /// <exception cref="NotImplementedException">Unknown Expression: " + e</exception>
    protected virtual object VisitIndexExpression(IndexExpression e)
    {
        var arg = e.Arguments[0];
        var oIndex = arg is ConstantExpression constant
                         ? constant.Value
                         : CachedExpressionCompiler.Evaluate(arg);

        var index = (int)Convert.ChangeType(oIndex, typeof(int));
        var oCollection = CachedExpressionCompiler.Evaluate(e.Object);

        if (oCollection is List<object> list)
        {
            return list[index];
        }

        throw new NotImplementedException("Unknown Expression: " + e);
    }

    /// <summary>
    /// Visits the conditional.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <returns>object.</returns>
    protected virtual object VisitConditional(ConditionalExpression e)
    {
        var test = this.IsBooleanComparison(e.Test)
                       ? new PartialSqlString($"{this.VisitMemberAccess((MemberExpression)e.Test)}={this.GetQuotedTrueValue()}")
                       : this.Visit(e.Test);

        if (test is bool b)
        {
            if (b)
            {
                var ifTrue = this.Visit(e.IfTrue);
                if (!IsSqlClass(ifTrue))
                {
                    if (this.Sep == " ")
                    {
                        ifTrue = new PartialSqlString(this.ConvertToParam(ifTrue));
                    }
                }
                else if (e.IfTrue.Type == typeof(bool))
                {
                    var isBooleanComparison = this.IsBooleanComparison(e.IfTrue);
                    if (!isBooleanComparison)
                    {
                        if (this.Sep == " ")
                        {
                            ifTrue = ifTrue.ToString();
                        }
                        else
                        {
                            ifTrue = new PartialSqlString($"(CASE WHEN {ifTrue} THEN {1} ELSE {0} END)");
                        }
                    }
                }

                return ifTrue;
            }

            var ifFalse = this.Visit(e.IfFalse);
            if (!IsSqlClass(ifFalse))
            {
                if (this.Sep == " ")
                {
                    ifFalse = new PartialSqlString(this.ConvertToParam(ifFalse));
                }
            }
            else if (e.IfFalse.Type == typeof(bool))
            {
                var isBooleanComparison = this.IsBooleanComparison(e.IfFalse);
                if (!isBooleanComparison)
                {
                    if (this.Sep == " ")
                    {
                        ifFalse = ifFalse.ToString();
                    }
                    else
                    {
                        ifFalse = new PartialSqlString($"(CASE WHEN {ifFalse} THEN {1} ELSE {0} END)");
                    }
                }
            }

            return ifFalse;
        }
        else
        {
            var ifTrue = this.Visit(e.IfTrue);
            if (!IsSqlClass(ifTrue))
            {
                ifTrue = this.ConvertToParam(ifTrue);
            }
            else if (e.IfTrue.Type == typeof(bool))
            {
                var isBooleanComparison = this.IsBooleanComparison(e.IfTrue);
                if (!isBooleanComparison)
                {
                    ifTrue = $"(CASE WHEN {ifTrue} THEN {this.GetQuotedTrueValue()} ELSE {this.GetQuotedFalseValue()} END)";
                }
            }

            var ifFalse = this.Visit(e.IfFalse);
            if (!IsSqlClass(ifFalse))
            {
                ifFalse = this.ConvertToParam(ifFalse);
            }
            else if (e.IfFalse.Type == typeof(bool))
            {
                var isBooleanComparison = this.IsBooleanComparison(e.IfFalse);
                if (!isBooleanComparison)
                {
                    ifFalse = $"(CASE WHEN {ifFalse} THEN {this.GetQuotedTrueValue()} ELSE {this.GetQuotedFalseValue()} END)";
                }
            }

            return new PartialSqlString($"(CASE WHEN {test} THEN {ifTrue} ELSE {ifFalse} END)");
        }
    }

    /// <summary>
    /// Gets the not value.
    /// </summary>
    /// <param name="o">The o.</param>
    /// <returns>object.</returns>
    private object GetNotValue(object o)
    {
        if (o is not PartialSqlString)
        {
            return !(bool)o;
        }

        if (this.IsFieldName(o))
        {
            return new PartialSqlString(o + "=" + this.GetQuotedFalseValue());
        }

        return new PartialSqlString("NOT (" + o + ")");
    }

    /// <summary>
    /// Determines whether [is column access] [the specified m].
    /// </summary>
    /// <param name="m">The m.</param>
    /// <returns>bool.</returns>
    protected virtual bool IsColumnAccess(MethodCallExpression m)
    {
        if (m.Object == null)
        {
            foreach (var arg in m.Arguments)
            {
                if (arg is not LambdaExpression && this.IsParameterAccess(arg))
                {
                    return true;
                }
            }

            return false;
        }

        if (m.Object is MethodCallExpression methCallExp)
        {
            return this.IsColumnAccess(methCallExp);
        }

        if (m.Object is ConditionalExpression condExp)
        {
            return this.IsParameterAccess(condExp);
        }

        if (m.Object is UnaryExpression unaryExp)
        {
            return this.IsParameterAccess(unaryExp);
        }

        var exp = m.Object as MemberExpression;
        return this.IsParameterAccess(exp)
               && this.IsJoinedTable(exp.Expression.Type);
    }

    /// <summary>
    /// Visits the invocation.
    /// </summary>
    /// <param name="m">The m.</param>
    /// <returns>System.Object.</returns>
    protected virtual object VisitInvocation(InvocationExpression m)
    {
        return this.EvaluateExpression(m);
    }

    /// <summary>
    /// Visits the method call.
    /// </summary>
    /// <param name="m">The m.</param>
    /// <returns>object.</returns>
    protected virtual object VisitMethodCall(MethodCallExpression m)
    {
        if (m.Method.DeclaringType == typeof(Sql))
        {
            var hold = this.inSqlMethodCall;
            this.inSqlMethodCall = true;
            var ret = this.VisitSqlMethodCall(m);
            this.inSqlMethodCall = hold;
            return ret;
        }

        // In C# 14, array.Contains() can resolve to MemoryExtensions.Contains (extension method)
        // which should be treated as a collection Contains (SQL IN) not string Contains (SQL LIKE)
        // Check this FIRST before other Contains checks
        if (IsSpanContainsOnArray(m))
        {
            return VisitSpanContainsMethodCall(m);
        }

        if (this.IsStaticArrayMethod(m))
        {
            return this.VisitStaticArrayMethodCall(m);
        }

        if (IsEnumerableMethod(m))
        {
            return this.VisitEnumerableMethodCall(m);
        }

        if (this.IsStaticStringMethod(m))
        {
            return this.VisitStaticStringMethodCall(m);
        }

        if (this.IsColumnAccess(m))
        {
            return this.VisitColumnAccessMethod(m);
        }

        return this.EvaluateExpression(m);
    }

    /// <summary>
    /// Evaluates the expression.
    /// </summary>
    /// <param name="m">The m.</param>
    /// <returns>object.</returns>
    private object EvaluateExpression(Expression m)
    {
        try
        {
            return CachedExpressionCompiler.Evaluate(m);
        }
        catch (InvalidOperationException)
        {
            if (this.originalLambda == null)
            {
                throw;
            }

            // Can't use expression.Compile() if lambda expression contains captured parameters.
            // Fallback invokes expression with default parameters from original lambda expression
            var lambda = Expression.Lambda(m, this.originalLambda.Parameters).Compile();

            var exprParams = new object[this.originalLambda.Parameters.Count];
            for (var i = 0; i < this.originalLambda.Parameters.Count; i++)
            {
                var p = this.originalLambda.Parameters[i];
                exprParams[i] = p.Type.CreateInstance();
            }

            var ret = lambda.DynamicInvoke(exprParams);
            return ret;
        }
    }

    /// <summary>
    /// Visits the expression list.
    /// </summary>
    /// <param name="original">The original.</param>
    /// <returns>System.Collections.Generic.List&lt;object&gt;.</returns>
    protected virtual List<object> VisitExpressionList(ReadOnlyCollection<Expression> original)
    {
        var list = new List<object>();
        for (int i = 0, n = original.Count; i < n; i++)
        {
            var e = original[i];
            if (e.NodeType == ExpressionType.NewArrayInit ||
                e.NodeType == ExpressionType.NewArrayBounds)
            {
                list.AddRange(this.VisitNewArrayFromExpressionList(e as NewArrayExpression));
            }
            else
            {
                list.Add(this.Visit(e));
            }
        }

        return list;
    }

    /// <summary>
    /// Visits the in SQL expression list.
    /// </summary>
    /// <param name="original">The original.</param>
    /// <returns>System.Collections.Generic.List&lt;object&gt;.</returns>
    protected virtual List<object> VisitInSqlExpressionList(ReadOnlyCollection<Expression> original)
    {
        var list = new List<object>();
        for (int i = 0, n = original.Count; i < n; i++)
        {
            var e = original[i];
            if (e.NodeType == ExpressionType.NewArrayInit ||
                e.NodeType == ExpressionType.NewArrayBounds)
            {
                list.AddRange(this.VisitNewArrayFromExpressionList(e as NewArrayExpression));
            }
            else if (e.NodeType == ExpressionType.MemberAccess)
            {
                list.Add(this.VisitMemberAccess(e as MemberExpression));
            }
            else
            {
                list.Add(this.Visit(e));
            }
        }

        return list;
    }

    /// <summary>
    /// Visits the new array.
    /// </summary>
    /// <param name="na">The na.</param>
    /// <returns>object.</returns>
    protected virtual object VisitNewArray(NewArrayExpression na)
    {
        var exprs = this.VisitExpressionList(na.Expressions);
        var sb = StringBuilderCache.Allocate();
        foreach (var e in exprs)
        {
            sb.Append(sb.Length > 0 ? "," + e : e);
        }

        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// Visits the new array from expression list.
    /// </summary>
    /// <param name="na">The na.</param>
    /// <returns>System.Collections.Generic.List&lt;object&gt;.</returns>
    protected virtual List<object> VisitNewArrayFromExpressionList(NewArrayExpression na)
    {
        var exprs = this.VisitExpressionList(na.Expressions);
        return exprs;
    }

    /// <summary>
    /// Binds the operant.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <returns>string.</returns>
    protected virtual string BindOperant(ExpressionType e)
    {
        switch (e)
        {
            case ExpressionType.Equal:
                return "=";
            case ExpressionType.NotEqual:
                return "<>";
            case ExpressionType.GreaterThan:
                return ">";
            case ExpressionType.GreaterThanOrEqual:
                return ">=";
            case ExpressionType.LessThan:
                return "<";
            case ExpressionType.LessThanOrEqual:
                return "<=";
            case ExpressionType.AndAlso:
                return "AND";
            case ExpressionType.OrElse:
                return "OR";
            case ExpressionType.Add:
                return "+";
            case ExpressionType.Subtract:
                return "-";
            case ExpressionType.Multiply:
                return "*";
            case ExpressionType.Divide:
                return "/";
            case ExpressionType.Modulo:
                return "MOD";
            case ExpressionType.Coalesce:
                return "COALESCE";
            case ExpressionType.And:
                return "&";
            case ExpressionType.Or:
                return "|";
            case ExpressionType.ExclusiveOr:
                return "^";
            case ExpressionType.LeftShift:
                return "<<";
            case ExpressionType.RightShift:
                return ">>";
            default:
                return e.ToString();
        }
    }

    /// <summary>
    /// Gets the name of the quoted column.
    /// </summary>
    /// <param name="tableDef">The table definition.</param>
    /// <param name="memberName">Name of the member.</param>
    /// <returns>string.</returns>
    protected virtual string GetQuotedColumnName(ModelDefinition tableDef, string memberName)
    {
        // Always call if no tableAlias to exec overrides
        return this.GetQuotedColumnName(tableDef, tableDef == this.modelDef ? this.TableAlias : null, memberName);
    }

    /// <summary>
    /// Gets the name of the quoted column.
    /// </summary>
    /// <param name="tableDef">The table definition.</param>
    /// <param name="tableAlias">The table alias.</param>
    /// <param name="memberName">Name of the member.</param>
    /// <returns>string.</returns>
    protected virtual string GetQuotedColumnName(ModelDefinition tableDef, string tableAlias, string memberName)
    {
        if (this.useFieldName)
        {
            var fd = tableDef.FieldDefinitions.FirstOrDefault(x => x.Name == memberName);
            var fieldName = fd != null
                                ? fd.FieldName
                                : memberName;

            if (tableDef.ModelType.IsInterface && this.ModelDef.ModelType.HasInterface(tableDef.ModelType))
            {
                tableDef = this.ModelDef;
            }

            if (fd?.CustomSelect != null)
            {
                return fd.CustomSelect;
            }

            var includePrefix = this.PrefixFieldWithTableName && !tableDef.ModelType.IsInterface;
            return includePrefix
                       ? (tableAlias == null
                           ? fd != null
                               ? this.DialectProvider.GetQuotedColumnName(tableDef, fd)
                               : this.DialectProvider.GetQuotedColumnName(tableDef, fieldName)
                           : fd != null
                               ? this.DialectProvider.GetQuotedColumnName(tableDef, tableAlias, fd)
                               : this.DialectProvider.GetQuotedColumnName(tableDef, tableAlias, fieldName))
                : fd != null
                ? this.DialectProvider.GetQuotedColumnName(fd)
                : this.DialectProvider.GetQuotedColumnName(fieldName);
        }

        return memberName;
    }

    /// <summary>
    /// Removes the quote from alias.
    /// </summary>
    /// <param name="exp">The exp.</param>
    /// <returns>string.</returns>
    protected string RemoveQuoteFromAlias(string exp)
    {
        if ((exp.StartsWith('"') || exp.StartsWith('`') || exp.StartsWith('\''))
            &&
            (exp.EndsWith('"') || exp.EndsWith('`') || exp.EndsWith('\'')))
        {
            exp = exp[1..];
            exp = exp[..^1];
        }

        return exp;
    }

    /// <summary>
    /// Determines whether [is field name] [the specified quoted exp].
    /// </summary>
    /// <param name="quotedExp">The quoted exp.</param>
    /// <returns>bool.</returns>
    protected virtual bool IsFieldName(object quotedExp)
    {
        var fieldExpr = quotedExp.ToString().StripTablePrefixes();
        var unquotedExpr = fieldExpr.StripDbQuotes();

        var isTableField = this.modelDef.FieldDefinitionsArray
            .Any(x => this.GetColumnName(x.FieldName) == unquotedExpr);
        if (isTableField)
        {
            return true;
        }

        var isJoinedField = this.tableDefs.Any(t => t.FieldDefinitionsArray
            .Any(x => this.GetColumnName(x.FieldName) == unquotedExpr));

        return isJoinedField;
    }

    /// <summary>
    /// Gets the name of the column.
    /// </summary>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>string.</returns>
    protected string GetColumnName(string fieldName)
    {
        return this.DialectProvider.NamingStrategy.GetColumnName(fieldName);
    }

    /// <summary>
    /// Gets the true expression.
    /// </summary>
    /// <returns>object.</returns>
    protected object GetTrueExpression()
    {
        return new PartialSqlString($"({this.GetQuotedTrueValue()}={this.GetQuotedTrueValue()})");
    }

    /// <summary>
    /// Gets the false expression.
    /// </summary>
    /// <returns>object.</returns>
    protected object GetFalseExpression()
    {
        return new PartialSqlString($"({this.GetQuotedTrueValue()}={this.GetQuotedFalseValue()})");
    }

    /// <summary>
    /// The quoted true
    /// </summary>
    private string quotedTrue;
    /// <summary>
    /// Gets the quoted true value.
    /// </summary>
    /// <returns>object.</returns>
    protected object GetQuotedTrueValue()
    {
        return new PartialSqlString(this.quotedTrue ??= this.DialectProvider.GetQuotedValue(true, typeof(bool)));
    }

    /// <summary>
    /// The quoted false
    /// </summary>
    private string quotedFalse;
    /// <summary>
    /// Gets the quoted false value.
    /// </summary>
    /// <returns>object.</returns>
    protected object GetQuotedFalseValue()
    {
        return new PartialSqlString(this.quotedFalse ??= this.DialectProvider.GetQuotedValue(false, typeof(bool)));
    }

    /// <summary>
    /// Builds the select expression.
    /// </summary>
    /// <param name="fields">The fields.</param>
    /// <param name="distinct">The distinct.</param>
    private void BuildSelectExpression(string fields, bool distinct)
    {
        this.OnlyFields = null;
        this.selectDistinct = distinct;

        this.selectExpression = $"SELECT {(this.selectDistinct ? "DISTINCT " : string.Empty)}" +
                                (string.IsNullOrEmpty(fields)
                                     ? this.DialectProvider.GetColumnNames(this.modelDef, this.PrefixFieldWithTableName ? this.TableAlias ?? this.ModelDef.ModelName : null).ToSelectString()
                                     : fields);
    }

    /// <summary>
    /// Gets all fields.
    /// </summary>
    /// <returns>System.Collections.Generic.IList&lt;string&gt;.</returns>
    public IList<string> GetAllFields()
    {
        return this.modelDef.FieldDefinitions.ConvertAll(r => r.Name);
    }

    /// <summary>
    /// Determines whether [is static array method] [the specified m].
    /// </summary>
    /// <param name="m">The m.</param>
    /// <returns>bool.</returns>
    protected virtual bool IsStaticArrayMethod(MethodCallExpression m)
    {
        if (m.Method.Name != "Contains" || m.Arguments.Count != 2)
        {
            return false;
        }

        // In C# 14, array.Contains() can resolve to different methods due to span conversions
        // We need to check if this is a collection Contains (for SQL IN) vs string Contains (for SQL LIKE)
        // Static array methods have m.Object == null
        if (m.Object != null)
        {
            return false;
        }

        // Check if the declaring type is a collection-related type (not string-related)
        var declaringType = m.Method.DeclaringType;
        if (declaringType == null)
        {
            return false;
        }

        // Exclude string/span Contains methods that should use LIKE
        if (declaringType == typeof(string) ||
            declaringType.Name == "MemoryExtensions" ||
            declaringType.FullName?.StartsWith("System.MemoryExtensions") == true)
        {
            return false;
        }

        // Check if first argument is a collection type (not string)
        if (m.Arguments.Count > 0)
        {
            var firstArgType = m.Arguments[0].Type;
            if (firstArgType == typeof(string))
            {
                return false;
            }

            // Accept if it's an array or implements IEnumerable<>
            if (firstArgType.IsArray ||
                firstArgType.IsOrHasGenericInterfaceTypeOf(typeof(IEnumerable<>)))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Visits the static array method call.
    /// </summary>
    /// <param name="m">The m.</param>
    /// <returns>object.</returns>
    /// <exception cref="NotSupportedException"></exception>
    protected virtual object VisitStaticArrayMethodCall(MethodCallExpression m)
    {
        switch (m.Method.Name)
        {
            case "Contains":
                var args = this.VisitExpressionList(m.Arguments);
                var quotedColName = args.Last();

                var memberExpr = m.Arguments[0];
                if (memberExpr.NodeType == ExpressionType.MemberAccess)
                {
                    memberExpr = m.Arguments[0] as MemberExpression;
                }

                return this.ToInPartialString(memberExpr, quotedColName);

            default:
                throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Determines whether [is enumerable method] [the specified m].
    /// </summary>
    /// <param name="m">The m.</param>
    /// <returns>bool.</returns>
    private static bool IsEnumerableMethod(MethodCallExpression m)
    {
        if (m.Method.Name != "Contains" || m.Arguments.Count != 1 || m.Object == null)
        {
            return false;
        }

        var objectType = m.Object.Type;

        // Exclude string Contains (should use LIKE)
        if (objectType == typeof(string))
        {
            return false;
        }

        // In C# 14, need to exclude span-based Contains methods
        var declaringType = m.Method.DeclaringType;
        if (declaringType != null &&
            (declaringType.Name == "MemoryExtensions" ||
             declaringType.FullName?.StartsWith("System.MemoryExtensions") == true))
        {
            return false;
        }

        // Accept collection Contains (should use IN)
        return objectType.IsOrHasGenericInterfaceTypeOf(typeof(IEnumerable<>));
    }

    private static bool IsSpanContainsOnArray(MethodCallExpression m)
    {
        // In C# 14, array.Contains() can resolve to MemoryExtensions.Contains<T>(ReadOnlySpan<T>, T)
        // This is a static extension method where:
        // - m.Object is null (static method)
        // - m.Arguments[0] is the collection/array (converted to span)
        // - m.Arguments[1] is the value to search for

        if (m.Method.Name != "Contains")
        {
            return false;
        }

        // Must be a static method (extension method)
        if (m.Object != null)
        {
            return false;
        }

        // Must have 2 or 3 arguments (2 for basic Contains, 3 for Contains with comparer)
        if (m.Arguments.Count != 2 && m.Arguments.Count != 3)
        {
            return false;
        }

        var declaringType = m.Method.DeclaringType;
        if (declaringType == null)
        {
            return false;
        }

        // Check if this is MemoryExtensions.Contains
        var isMemoryExtensions = declaringType.Name == "MemoryExtensions" ||
                                 declaringType.FullName?.StartsWith("System.MemoryExtensions") == true;

        if (!isMemoryExtensions)
        {
            return false;
        }

        // Check the second argument type to determine if this is string/char Contains or collection Contains
        var secondArgType = m.Arguments[1].Type;

        // If the second argument is a string or ReadOnlySpan<char>, this is string Contains (should use LIKE)
        if (secondArgType == typeof(string) || secondArgType == typeof(char))
        {
            return false;
        }

        if (secondArgType.Name == "ReadOnlySpan`1" && secondArgType.GenericTypeArguments.Length > 0 &&
            secondArgType.GenericTypeArguments[0] == typeof(char))
        {
            return false;
        }

        // Otherwise, this is a collection Contains (should use IN)
        // The second argument is the value being searched for (e.g., x.Id)
        return true;
    }

    /// <summary>
    /// Visits the enumerable method call.
    /// </summary>
    /// <param name="m">The m.</param>
    /// <returns>object.</returns>
    /// <exception cref="NotSupportedException"></exception>
    protected virtual object VisitEnumerableMethodCall(MethodCallExpression m)
    {
        switch (m.Method.Name)
        {
            case "Contains":
                var args = this.VisitExpressionList(m.Arguments);
                var quotedColName = args[0];
                return this.ToInPartialString(m.Object, quotedColName);

            default:
                throw new NotSupportedException();
        }
    }

    protected virtual object VisitSpanContainsMethodCall(MethodCallExpression m)
    {
        // Handle MemoryExtensions.Contains<T>(ReadOnlySpan<T>, T) which is used in C# 14
        // for array.Contains() calls. This is a static extension method where:
        // - m.Arguments[0] is the collection/array (converted to span)
        // - m.Arguments[1] is the value to search for
        switch (m.Method.Name)
        {
            case "Contains":
                List<object> args = this.VisitExpressionList(m.Arguments);
                // args[0] is the collection, args[1] is the column/value to check
                object quotedColName = args[1];
                Expression collectionExpr = m.Arguments[0];
                return ToInPartialString(collectionExpr, quotedColName);

            default:
                throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Converts to inpartialstring.
    /// </summary>
    /// <param name="memberExpr">The member expr.</param>
    /// <param name="quotedColName">Name of the quoted col.</param>
    /// <returns>object.</returns>
    private object ToInPartialString(Expression memberExpr, object quotedColName)
    {
        var result = this.EvaluateExpression(memberExpr);

        var inArgs = Sql.Flatten(result as IEnumerable);

        var sqlIn = inArgs.Count > 0
                        ? this.CreateInParamSql(inArgs)
                        : "NULL";

        var statement = $"{quotedColName} IN ({sqlIn})";
        return new PartialSqlString(statement);
    }

    /// <summary>
    /// Determines whether [is static string method] [the specified m].
    /// </summary>
    /// <param name="m">The m.</param>
    /// <returns>bool.</returns>
    protected virtual bool IsStaticStringMethod(MethodCallExpression m)
    {
        return m.Object == null
               && (m.Method.Name == nameof(string.Concat) || m.Method.Name == nameof(string.Compare));
    }

    /// <summary>
    /// Visits the static string method call.
    /// </summary>
    /// <param name="m">The m.</param>
    /// <returns>object.</returns>
    /// <exception cref="NotSupportedException"></exception>
    protected virtual object VisitStaticStringMethodCall(MethodCallExpression m)
    {
        return m.Method.Name switch {
            nameof(string.Concat) => this.BuildConcatExpression(this.VisitExpressionList(m.Arguments)),
            nameof(string.Compare) => this.BuildCompareExpression(this.VisitExpressionList(m.Arguments)),
            _ => throw new NotSupportedException()
        };
    }

    /// <summary>
    /// Visits the length string property.
    /// </summary>
    /// <param name="m">The m.</param>
    /// <returns>object.</returns>
    private object VisitLengthStringProperty(MemberExpression m)
    {
        var sql = this.Visit(m.Expression);
        if (!IsSqlClass(sql))
        {
            if (sql == null)
            {
                return 0;
            }

            sql = ((string)sql).Length;
            return sql;
        }

        return this.ToLengthPartialString(sql);
    }

    /// <summary>
    /// Converts to lengthpartialstring.
    /// </summary>
    /// <param name="arg">The argument.</param>
    /// <returns>ServiceStack.OrmLite.PartialSqlString.</returns>
    protected virtual PartialSqlString ToLengthPartialString(object arg)
    {
        return new PartialSqlString($"CHAR_LENGTH({arg})");
    }

    /// <summary>
    /// Builds the concat expression.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>ServiceStack.OrmLite.PartialSqlString.</returns>
    private PartialSqlString BuildConcatExpression(List<object> args)
    {
        for (var i = 0; i < args.Count; i++)
        {
            if (args[i] is not PartialSqlString)
            {
                args[i] = this.ConvertToParam(args[i]);
            }
        }

        return this.ToConcatPartialString(args);
    }

    /// <summary>
    /// Builds the compare expression.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>ServiceStack.OrmLite.PartialSqlString.</returns>
    private PartialSqlString BuildCompareExpression(List<object> args)
    {
        for (var i = 0; i < args.Count; i++)
        {
            if (args[i] is not PartialSqlString)
            {
                args[i] = this.ConvertToParam(args[i]);
            }
        }

        return this.ToComparePartialString(args);
    }

    /// <summary>
    /// Converts to concatpartialstring.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>ServiceStack.OrmLite.PartialSqlString.</returns>
    protected PartialSqlString ToConcatPartialString(List<object> args)
    {
        return new PartialSqlString(this.DialectProvider.SqlConcat(args));
    }

    /// <summary>
    /// Converts to comparepartialstring.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>ServiceStack.OrmLite.PartialSqlString.</returns>
    protected virtual PartialSqlString ToComparePartialString(List<object> args)
    {
        return new PartialSqlString($"(CASE WHEN {args[0]} = {args[1]} THEN 0 WHEN {args[0]} > {args[1]} THEN 1 ELSE -1 END)");
    }

    /// <summary>
    /// Visits the SQL method call.
    /// </summary>
    /// <param name="m">The m.</param>
    /// <returns>object.</returns>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="SelectList">this.DialectProvider.GetColumnNames(paramModelDef, alias)</exception>
    protected virtual object VisitSqlMethodCall(MethodCallExpression m)
    {
        var args = this.VisitInSqlExpressionList(m.Arguments);
        var quotedColName = args[0];
        var columnEnumMemberAccess = args[0] as EnumMemberAccess;
        args.RemoveAt(0);

        string statement;

        switch (m.Method.Name)
        {
            case nameof(Sql.In):
                statement = this.ConvertInExpressionToSql(m, quotedColName);
                break;
            case nameof(Sql.Asc):
                statement = $"{quotedColName} ASC";
                break;
            case nameof(Sql.Desc):
                statement = $"{quotedColName} DESC";
                break;
            case nameof(Sql.As):
                statement = $"{quotedColName} AS {this.DialectProvider.GetQuotedColumnName(this.RemoveQuoteFromAlias(args[0].ToString()))}";
                break;
            case nameof(Sql.Cast):
                statement = this.DialectProvider.SqlCast(quotedColName, args[0].ToString());
                break;
            case nameof(Sql.Sum):
            case nameof(Sql.Count):
            case nameof(Sql.Min):
            case nameof(Sql.Max):
            case nameof(Sql.Avg):
                statement = $"{m.Method.Name}({quotedColName}{(args.Count == 1 ? $",{args[0]}" : string.Empty)})";
                break;
            case nameof(Sql.CountDistinct):
                statement = $"COUNT(DISTINCT {quotedColName})";
                break;
            case nameof(Sql.AllFields):
                var argDef = m.Arguments[0].Type.GetModelMetadata();
                statement = this.DialectProvider.GetQuotedTableName(argDef) + ".*";
                break;
            case nameof(Sql.JoinAlias):
            case nameof(Sql.TableAlias):
                if (quotedColName is SelectList && m.Arguments.Count == 2 && m.Arguments[0] is ParameterExpression p)
                {
                    var paramModelDef = p.Type.GetModelDefinition();
                    var alias = this.Visit(m.Arguments[1]).ToString();
                    statement = new SelectList(this.DialectProvider.GetColumnNames(paramModelDef, alias)).ToString();
                }
                else
                {
                    //statement = args[0] + "." + quotedColName.ToString().LastRightPart('.');
                    statement = this.DialectProvider.GetQuotedName(args[0].ToString()) + "." + quotedColName.ToString().LastRightPart('.');
                }
                break;
            case nameof(Sql.Custom):
                statement = quotedColName.ToString();
                break;
            default:
                throw new NotSupportedException();
        }

        return new PartialSqlString(statement, columnEnumMemberAccess);
    }

    /// <summary>
    /// Converts the in expression to SQL.
    /// </summary>
    /// <param name="m">The m.</param>
    /// <param name="quotedColName">Name of the quoted col.</param>
    /// <returns>string.</returns>
    /// <exception cref="NotSupportedException">$"In({argValue.GetType()})</exception>
    protected string ConvertInExpressionToSql(MethodCallExpression m, object quotedColName)
    {
        var argValue = this.EvaluateExpression(m.Arguments[1]);

        if (argValue == null)
        {
            return FalseLiteral; // "column IN (NULL)" is always false
        }

        if (quotedColName is not PartialSqlString)
        {
            quotedColName = this.ConvertToParam(quotedColName);
        }

        if (argValue is IEnumerable enumerableArg)
        {
            var inArgs = Sql.Flatten(enumerableArg);
            if (inArgs.Count == 0)
            {
                return FalseLiteral; // "column IN ([])" is always false
            }

            var sqlIn = this.CreateInParamSql(inArgs);
            return $"{quotedColName} IN ({sqlIn})";
        }

        if (argValue is ISqlExpression exprArg)
        {
            var subSelect = exprArg.ToSelectStatement(QueryType.Select);
            var renameParams = new List<Tuple<string, string>>();
            foreach (var p in exprArg.Params)
            {
                var oldName = p.ParameterName;
                var newName = this.DialectProvider.GetParam(this.Params.Count.ToString());
                if (oldName != newName)
                {
                    var pClone = this.DialectProvider.CreateParam().PopulateWith(p);
                    renameParams.Add(Tuple.Create(oldName, newName));
                    pClone.ParameterName = newName;
                    this.Params.Add(pClone);
                }
                else
                {
                    this.Params.Add(p);
                }
            }

            // regex replace doesn't work when param is at end of string "AND a = :0"
            var lastChar = subSelect[^1];
            if (!(char.IsWhiteSpace(lastChar) || lastChar == ')'))
            {
                subSelect += " ";
            }

            for (var i = renameParams.Count - 1; i >= 0; i--)
            {
                // Replace complete db params [@1] and not partial tokens [@1]0
                var paramsRegex = new Regex(
                    renameParams[i].Item1 + "([^\\d])",
                    RegexOptions.None,
                    TimeSpan.FromMilliseconds(100));
                subSelect = paramsRegex.Replace(subSelect, renameParams[i].Item2 + "$1");
            }

            return this.CreateInSubQuerySql(quotedColName, subSelect);
        }

        throw new NotSupportedException($"In({argValue.GetType()})");
    }

    /// <summary>
    /// Creates the in sub query SQL.
    /// </summary>
    /// <param name="quotedColName">Name of the quoted col.</param>
    /// <param name="subSelect">The sub select.</param>
    /// <returns>string.</returns>
    protected virtual string CreateInSubQuerySql(object quotedColName, string subSelect)
    {
        return $"{quotedColName} IN ({subSelect})";
    }

    /// <summary>
    /// Visits the column access method.
    /// </summary>
    /// <param name="m">The m.</param>
    /// <returns>object.</returns>
    /// <exception cref="NotSupportedException"></exception>
    protected virtual object VisitColumnAccessMethod(MethodCallExpression m)
    {
        var args = this.VisitExpressionList(m.Arguments);
        var quotedColName = this.Visit(m.Object);
        if (!IsSqlClass(quotedColName))
        {
            quotedColName = this.ConvertToParam(quotedColName);
        }

        string statement;

        var arg = args.Count > 0 ? args[0] : null;

        string wildcardArg, escapeSuffix = string.Empty;

        if (this.AllowEscapeWildcards)
        {
            wildcardArg = arg != null ? this.DialectProvider.EscapeWildcards(arg.ToString()) : string.Empty;
            escapeSuffix = wildcardArg.Contains('^') ? " escape '^'" : string.Empty;
        }
        else
        {
            wildcardArg = arg != null ? arg.ToString() : string.Empty;
        }

        switch (m.Method.Name)
        {
            case "Trim":
                statement = $"ltrim(rtrim({quotedColName}))";
                break;
            case "LTrim":
                statement = $"ltrim({quotedColName})";
                break;
            case "RTrim":
                statement = $"rtrim({quotedColName})";
                break;
            case "ToUpper":
                statement = $"upper({quotedColName})";
                break;
            case "ToLower":
                statement = $"lower({quotedColName})";
                break;
            case "Equals":
                var argType = arg?.GetType();
                var converter = argType != null && argType != typeof(string)
                                    ? this.DialectProvider.GetConverterBestMatch(argType)
                                    : null;
                statement = converter != null
                                ? $"{quotedColName}={this.ConvertToParam(converter.ToDbValue(argType, arg))}"
                                : $"{quotedColName}={this.ConvertToParam(arg)}";
                break;
            case "StartsWith":
                statement = !OrmLiteConfig.StripUpperInLike
                                ? $"upper({quotedColName}) like {this.ConvertToParam(wildcardArg.ToUpper() + "%")}{escapeSuffix}"
                                : $"{quotedColName} like {this.ConvertToParam(wildcardArg + "%")}{escapeSuffix}";
                break;
            case "EndsWith":
                statement = !OrmLiteConfig.StripUpperInLike
                                ? $"upper({quotedColName}) like {this.ConvertToParam("%" + wildcardArg.ToUpper())}{escapeSuffix}"
                                : $"{quotedColName} like {this.ConvertToParam("%" + wildcardArg)}{escapeSuffix}";
                break;
            case "Contains":
                statement = !OrmLiteConfig.StripUpperInLike
                                ? $"upper({quotedColName}) like {this.ConvertToParam("%" + wildcardArg.ToUpper() + "%")}{escapeSuffix}"
                                : $"{quotedColName} like {this.ConvertToParam("%" + wildcardArg + "%")}{escapeSuffix}";
                break;
            case "Substring":
                var startIndex = int.Parse(args[0].ToString()) + 1;
                statement = args.Count == 2
                                ? this.GetSubstringSql(quotedColName, startIndex, int.Parse(args[1].ToString()))
                                : this.GetSubstringSql(quotedColName, startIndex);
                break;
            case "ToString":
                statement = m.Object?.Type == typeof(string)
                                ? $"({quotedColName})"
                                : this.ToCast(quotedColName.ToString());
                break;
            default:
                throw new NotSupportedException();
        }
        return new PartialSqlString(statement);
    }

    /// <summary>
    /// Converts to cast.
    /// </summary>
    /// <param name="quotedColName">Name of the quoted col.</param>
    /// <returns>string.</returns>
    protected virtual string ToCast(string quotedColName)
    {
        return $"cast({quotedColName} as varchar(1000))";
    }

    /// <summary>
    /// Gets the substring SQL.
    /// </summary>
    /// <param name="quotedColumn">The quoted column.</param>
    /// <param name="startIndex">The start index.</param>
    /// <param name="length">The length.</param>
    /// <returns>string.</returns>
    public virtual string GetSubstringSql(object quotedColumn, int startIndex, int? length = null)
    {
        return length != null
                   ? $"substring({quotedColumn} from {startIndex} for {length.Value})"
                   : $"substring({quotedColumn} from {startIndex})";
    }

    /// <summary>
    /// Creates the parameter.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <param name="direction">The direction.</param>
    /// <param name="dbType">Type of the database.</param>
    /// <param name="sourceVersion">The source version.</param>
    /// <returns>System.Data.IDbDataParameter.</returns>
    public IDbDataParameter CreateParam(string name,
                                        object value = null,
                                        ParameterDirection direction = ParameterDirection.Input,
                                        DbType? dbType = null,
                                        DataRowVersion sourceVersion = DataRowVersion.Default)
    {
        var p = this.DialectProvider.CreateParam();
        p.ParameterName = this.DialectProvider.GetParam(name);
        p.Direction = direction;

        if (!this.DialectProvider.IsMySqlConnector())
        {
            // throws NotSupportedException
            p.SourceVersion = sourceVersion;
        }

        this.DialectProvider.ConfigureParam(p, value, dbType);

        return p;
    }

    /// <summary>
    /// Gets the untyped.
    /// </summary>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression GetUntyped()
    {
        return new UntypedSqlExpressionProxy<T>(this);
    }
}

/// <summary>
/// Interface ISqlExpression
/// </summary>
public interface ISqlExpression
{
    /// <summary>
    /// Gets the parameters.
    /// </summary>
    /// <value>The parameters.</value>
    List<IDbDataParameter> Params { get; }

    /// <summary>
    /// Converts to selectstatement.
    /// </summary>
    /// <returns>string.</returns>
    string ToSelectStatement();
    /// <summary>
    /// Converts to selectstatement.
    /// </summary>
    /// <param name="forType">For type.</param>
    /// <returns>string.</returns>
    string ToSelectStatement(QueryType forType);
    /// <summary>
    /// Selects the into.
    /// </summary>
    /// <typeparam name="TModel">The type of the t model.</typeparam>
    /// <returns>string.</returns>
    string SelectInto<TModel>();
    /// <summary>
    /// Selects the into.
    /// </summary>
    /// <typeparam name="TModel">The type of the t model.</typeparam>
    /// <param name="forType">For type.</param>
    /// <returns>string.</returns>
    string SelectInto<TModel>(QueryType forType);
}

/// <summary>
/// Enum QueryType
/// </summary>
public enum QueryType
{
    /// <summary>
    /// The select
    /// </summary>
    Select,
    /// <summary>
    /// The single
    /// </summary>
    Single,
    /// <summary>
    /// The scalar
    /// </summary>
    Scalar
}

/// <summary>
/// Interface IHasDialectProvider
/// </summary>
public interface IHasDialectProvider
{
    /// <summary>
    /// Gets the dialect provider.
    /// </summary>
    /// <value>The dialect provider.</value>
    IOrmLiteDialectProvider DialectProvider { get; }
}

/// <summary>
/// Class PartialSqlString.
/// </summary>
public class PartialSqlString
{
    /// <summary>
    /// The null
    /// </summary>
    public static PartialSqlString Null = new("null");

    /// <summary>
    /// Initializes a new instance of the <see cref="PartialSqlString" /> class.
    /// </summary>
    /// <param name="text">The text.</param>
    public PartialSqlString(string text) : this(text, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PartialSqlString" /> class.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="enumMember">The enum member.</param>
    public PartialSqlString(string text, EnumMemberAccess enumMember)
    {
        this.Text = text;
        this.EnumMember = enumMember;
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <value>The text.</value>
    public string Text { get; internal set; }

    /// <summary>
    /// The enum member
    /// </summary>
    public readonly EnumMemberAccess EnumMember;

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>string.</returns>
    public override string ToString()
    {
        return this.Text;
    }

    /// <summary>
    /// Equalses the specified other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns>bool.</returns>
    protected bool Equals(PartialSqlString other)
    {
        return this.Text == other.Text;
    }

    /// <summary>
    /// Equalses the specified object.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>bool.</returns>
    public override bool Equals(object obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return this.Equals((PartialSqlString)obj);
    }

    /// <summary>
    /// Gets the hash code.
    /// </summary>
    /// <returns>int.</returns>
    public override int GetHashCode()
    {
        return this.Text != null ? this.Text.GetHashCode() : 0;
    }
}

/// <summary>
/// Class EnumMemberAccess.
/// Implements the <see cref="ServiceStack.OrmLite.PartialSqlString" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.PartialSqlString" />
public class EnumMemberAccess : PartialSqlString
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnumMemberAccess" /> class.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="enumType">Type of the enum.</param>
    /// <exception cref="ArgumentException">Type not valid, nameof(enumType)</exception>
    public EnumMemberAccess(string text, Type enumType)
        : base(text)
    {
        if (!enumType.IsEnum)
        {
            throw new ArgumentException("Type not valid", nameof(enumType));
        }

        this.EnumType = enumType;
    }

    /// <summary>
    /// Gets the type of the enum.
    /// </summary>
    /// <value>The type of the enum.</value>
    public Type EnumType { get; }
}

/// <summary>
/// Class SelectItem.
/// </summary>
public abstract class SelectItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectItem" /> class.
    /// </summary>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <param name="alias">The alias.</param>
    protected SelectItem(IOrmLiteDialectProvider dialectProvider, string alias)
    {
        this.DialectProvider = dialectProvider ?? throw new ArgumentNullException(nameof(dialectProvider));

        this.Alias = alias;
    }

    /// <summary>
    /// Unquoted alias for the column or expression being selected.
    /// </summary>
    /// <value>The alias.</value>
    public string Alias { get; set; }

    /// <summary>
    /// Gets or sets the dialect provider.
    /// </summary>
    /// <value>The dialect provider.</value>
    protected IOrmLiteDialectProvider DialectProvider { get; set; }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>string.</returns>
    public override abstract string ToString();
}

/// <summary>
/// Class SelectItemExpression.
/// Implements the <see cref="ServiceStack.OrmLite.SelectItem" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.SelectItem" />
public class SelectItemExpression : SelectItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectItemExpression" /> class.
    /// </summary>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <param name="selectExpression">The select expression.</param>
    /// <param name="alias">The alias.</param>
    /// <exception cref="ArgumentNullException">nameof(selectExpression)</exception>
    /// <exception cref="ArgumentNullException">nameof(alias)</exception>
    public SelectItemExpression(IOrmLiteDialectProvider dialectProvider, string selectExpression, string alias)
        : base(dialectProvider, alias)
    {
        if (string.IsNullOrEmpty(selectExpression))
        {
            throw new ArgumentNullException(nameof(selectExpression));
        }

        if (string.IsNullOrEmpty(alias))
        {
            throw new ArgumentNullException(nameof(alias));
        }

        this.SelectExpression = selectExpression;
        this.Alias = alias;
    }

    /// <summary>
    /// The SQL expression being selected, including any necessary quoting.
    /// </summary>
    /// <value>The select expression.</value>
    public string SelectExpression { get; set; }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>string.</returns>
    public override string ToString()
    {
        var text = this.SelectExpression;
        if (!string.IsNullOrEmpty(this.Alias)) // Note that even though Alias must be non-empty in the constructor it may be set to null/empty later
        {
            return text + " AS " + this.DialectProvider.GetQuotedName(this.Alias);
        }

        return text;
    }
}

/// <summary>
/// Class SelectItemColumn.
/// Implements the <see cref="ServiceStack.OrmLite.SelectItem" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.SelectItem" />
public class SelectItemColumn : SelectItem
{
    private FieldDefinition fieldDef;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectItemColumn" /> class.
    /// </summary>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="quotedTableAlias">The quoted table alias.</param>
    public SelectItemColumn(IOrmLiteDialectProvider dialectProvider, FieldDefinition fieldDef, string quotedTableAlias = null)
            : base(dialectProvider, null)
    {
        this.fieldDef = fieldDef;
        this.QuotedTableAlias = quotedTableAlias;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectItemColumn" /> class.
    /// </summary>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <param name="columnAlias">The column alias.</param>
    /// <param name="quotedTableAlias">The quoted table alias.</param>
    /// <exception cref="ArgumentNullException">nameof(columnName)</exception>
    public SelectItemColumn(IOrmLiteDialectProvider dialectProvider, string columnName, string columnAlias = null, string quotedTableAlias = null)
        : base(dialectProvider, columnAlias)
    {
        if (string.IsNullOrEmpty(columnName))
        {
            throw new ArgumentNullException(nameof(columnName));
        }

        this.ColumnName = columnName;
        this.QuotedTableAlias = quotedTableAlias;
    }

    /// <summary>
    /// Unquoted column name being selected.
    /// </summary>
    /// <value>The name of the column.</value>
    public string ColumnName { get; set; }

    /// <summary>
    /// Table name or alias used to prefix the column name, if any. Already quoted.
    /// </summary>
    /// <value>The quoted table alias.</value>
    public string QuotedTableAlias { get; set; }

    /// <summary>
    /// Gets the name of the column.
    /// </summary>
    /// <returns>System.String.</returns>
    public string GetColumnName()
    {
        return this.fieldDef != null ? this.fieldDef.Name : this.ColumnName;
    }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>string.</returns>
    public override string ToString()
    {
        var text = this.fieldDef != null
            ? this.DialectProvider.GetQuotedColumnName(this.fieldDef)
            : this.DialectProvider.GetQuotedColumnName(this.ColumnName);

        if (!string.IsNullOrEmpty(this.QuotedTableAlias))
        {
            text = this.QuotedTableAlias + "." + text;
        }

        if (!string.IsNullOrEmpty(this.Alias))
        {
            text += " AS " + this.DialectProvider.GetQuotedName(this.Alias);
        }

        return text;
    }
}

/// <summary>
/// Class OrmLiteDataParameter.
/// Implements the <see cref="System.Data.IDbDataParameter" />
/// </summary>
/// <seealso cref="System.Data.IDbDataParameter" />
public class OrmLiteDataParameter : IDbDataParameter
{
    /// <summary>
    /// Gets or sets the type of the database.
    /// </summary>
    /// <value>The type of the database.</value>
    public DbType DbType { get; set; }
    /// <summary>
    /// Gets or sets the direction.
    /// </summary>
    /// <value>The direction.</value>
    public ParameterDirection Direction { get; set; }
    /// <summary>
    /// Gets or sets the is nullable.
    /// </summary>
    /// <value>The is nullable.</value>
    public bool IsNullable { get; set; }
    /// <summary>
    /// Gets or sets the name of the parameter.
    /// </summary>
    /// <value>The name of the parameter.</value>
    public string ParameterName { get; set; }
    /// <summary>
    /// Gets or sets the source column.
    /// </summary>
    /// <value>The source column.</value>
    public string SourceColumn { get; set; }
    /// <summary>
    /// Gets or sets the source version.
    /// </summary>
    /// <value>The source version.</value>
    public DataRowVersion SourceVersion { get; set; }
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>The value.</value>
    public object Value { get; set; }
    /// <summary>
    /// Gets or sets the precision.
    /// </summary>
    /// <value>The precision.</value>
    public byte Precision { get; set; }
    /// <summary>
    /// Gets or sets the scale.
    /// </summary>
    /// <value>The scale.</value>
    public byte Scale { get; set; }
    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    /// <value>The size.</value>
    public int Size { get; set; }
}

/// <summary>
/// Class DbDataParameterExtensions.
/// </summary>
public static class DbDataParameterExtensions
{
    /// <summary>
    /// Creates the parameter.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="dbType">Type of the database.</param>
    /// <param name="precision">The precision.</param>
    /// <param name="scale">The scale.</param>
    /// <param name="size">The size.</param>
    /// <returns>System.Data.IDbDataParameter.</returns>
    public static IDbDataParameter CreateParam(this IDbConnection db,
                                               string name,
                                               object value = null,
                                               Type fieldType = null,
                                               DbType? dbType = null,
                                               byte? precision = null,
                                               byte? scale = null,
                                               int? size = null)
    {
        return db.GetDialectProvider().CreateParam(name, value, fieldType, dbType, precision, scale, size);
    }

    /// <param name="dialectProvider">The dialect provider.</param>
    extension(IOrmLiteDialectProvider dialectProvider)
    {
        /// <summary>
        /// Creates the parameter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="dbType">Type of the database.</param>
        /// <param name="precision">The precision.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="size">The size.</param>
        /// <returns>System.Data.IDbDataParameter.</returns>
        public IDbDataParameter CreateParam(string name,
            object value = null,
            Type fieldType = null,
            DbType? dbType = null,
            byte? precision = null,
            byte? scale = null,
            int? size = null)
        {
            var p = dialectProvider.CreateParam();

            p.ParameterName = dialectProvider.GetParam(name);

            dialectProvider.ConfigureParam(p, value, dbType);

            if (precision != null)
            {
                p.Precision = precision.Value;
            }

            if (scale != null)
            {
                p.Scale = scale.Value;
            }

            if (size != null)
            {
                p.Size = size.Value;
            }

            return p;
        }

        /// <summary>
        /// Configures the parameter.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="value">The value.</param>
        /// <param name="dbType">Type of the database.</param>
        internal void ConfigureParam(IDbDataParameter p, object value, DbType? dbType)
        {
            if (value != null)
            {
                dialectProvider.InitDbParam(p, value.GetType());
                p.Value = dialectProvider.GetParamValue(value, value.GetType());
            }
            else
            {
                p.Value = DBNull.Value;
            }

            // Can't check DbType in PostgreSQL before p.Value is assigned
            if (p.Value is string strValue && strValue.Length > p.Size)
            {
                var stringConverter = dialectProvider.GetStringConverter();
                p.Size = strValue.Length > stringConverter.StringLength
                    ? strValue.Length
                    : stringConverter.StringLength;
            }

            if (dbType != null)
            {
                p.DbType = dbType.Value;
            }
        }

        /// <summary>
        /// Adds the query parameter.
        /// </summary>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="value">The value.</param>
        /// <param name="fieldDef">The field definition.</param>
        /// <returns>System.Data.IDbDataParameter.</returns>
        public IDbDataParameter AddQueryParam(IDbCommand dbCmd,
            object value,
            FieldDefinition fieldDef)
        {
            return dialectProvider.AddParam(dbCmd, value, fieldDef, paramFilter: dialectProvider.InitQueryParam);
        }

        /// <summary>
        /// Adds the update parameter.
        /// </summary>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="value">The value.</param>
        /// <param name="fieldDef">The field definition.</param>
        /// <returns>System.Data.IDbDataParameter.</returns>
        public IDbDataParameter AddUpdateParam(IDbCommand dbCmd,
            object value,
            FieldDefinition fieldDef)
        {
            return dialectProvider.AddParam(dbCmd, value, fieldDef, paramFilter: dialectProvider.InitUpdateParam);
        }

        /// <summary>
        /// Adds the parameter.
        /// </summary>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="value">The value.</param>
        /// <param name="fieldDef">The field definition.</param>
        /// <param name="paramFilter">The parameter filter.</param>
        /// <returns>System.Data.IDbDataParameter.</returns>
        public IDbDataParameter AddParam(IDbCommand dbCmd,
            object value,
            FieldDefinition fieldDef, Action<IDbDataParameter> paramFilter)
        {
            var paramName = dbCmd.Parameters.Count.ToString();
            var parameter = dialectProvider.CreateParam(paramName, value, fieldDef?.ColumnType);

            paramFilter?.Invoke(parameter);

            if (fieldDef != null)
            {
                dialectProvider.SetParameter(fieldDef, parameter);
            }

            dbCmd.Parameters.Add(parameter);

            return parameter;
        }

        /// <summary>
        /// Gets the insert parameter.
        /// </summary>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="value">The value.</param>
        /// <param name="fieldDef">The field definition.</param>
        /// <returns>string.</returns>
        public string GetInsertParam(IDbCommand dbCmd,
            object value,
            FieldDefinition fieldDef)
        {
            var p = dialectProvider.AddUpdateParam(dbCmd, value, fieldDef);
            return fieldDef.CustomInsert != null
                ? string.Format(fieldDef.CustomInsert, p.ParameterName)
                : p.ParameterName;
        }

        /// <summary>
        /// Gets the update parameter.
        /// </summary>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="value">The value.</param>
        /// <param name="fieldDef">The field definition.</param>
        /// <returns>string.</returns>
        public string GetUpdateParam(IDbCommand dbCmd,
            object value,
            FieldDefinition fieldDef)
        {
            var p = dialectProvider.AddUpdateParam(dbCmd, value, fieldDef);

            return fieldDef.CustomUpdate != null
                ? string.Format(fieldDef.CustomUpdate, p.ParameterName)
                : p.ParameterName;
        }
    }
}