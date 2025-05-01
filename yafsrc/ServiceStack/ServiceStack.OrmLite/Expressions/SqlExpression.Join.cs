// ***********************************************************************
// <copyright file="SqlExpression.Join.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

/// <summary>
/// Delegate JoinFormatDelegate
/// </summary>
/// <param name="dialect">The dialect.</param>
/// <param name="tableDef">The table definition.</param>
/// <param name="joinExpr">The join expr.</param>
/// <returns>System.String.</returns>
public delegate string JoinFormatDelegate(IOrmLiteDialectProvider dialect, ModelDefinition tableDef, string joinExpr);

/// <summary>
/// Class TableOptions.
/// </summary>
public class TableOptions
{
    /// <summary>
    /// Gets or sets the expression.
    /// </summary>
    /// <value>The expression.</value>
    public string Expression { get; set; }

    /// <summary>
    /// Gets or sets the alias.
    /// </summary>
    /// <value>The alias.</value>
    public string Alias { get; set; }

    /// <summary>
    /// The join format
    /// </summary>
    internal JoinFormatDelegate JoinFormat;

    /// <summary>
    /// The model definition
    /// </summary>
    internal ModelDefinition ModelDef;

    /// <summary>
    /// The parameter name
    /// </summary>
    internal string ParamName;
}

/// <summary>
/// Class SqlExpression.
/// Implements the <see cref="ServiceStack.OrmLite.ISqlExpression" />
/// Implements the <see cref="ServiceStack.OrmLite.IHasUntypedSqlExpression" />
/// Implements the <see cref="ServiceStack.OrmLite.IHasDialectProvider" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.ISqlExpression" />
/// <seealso cref="ServiceStack.OrmLite.IHasUntypedSqlExpression" />
/// <seealso cref="ServiceStack.OrmLite.IHasDialectProvider" />
public abstract partial class SqlExpression<T> : ISqlExpression
{
    /// <summary>
    /// The table defs
    /// </summary>
    protected List<ModelDefinition> tableDefs = [];

    /// <summary>
    /// Gets all tables.
    /// </summary>
    /// <returns>List&lt;ModelDefinition&gt;.</returns>
    public List<ModelDefinition> GetAllTables()
    {
        var allTableDefs = new List<ModelDefinition> { this.modelDef };
        allTableDefs.AddRange(this.tableDefs);
        return allTableDefs;
    }

    /// <summary>
    /// Adds the reference table if not exists.
    /// </summary>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> AddReferenceTableIfNotExists<Target>()
    {
        var tableDef = typeof(Target).GetModelDefinition();
        if (!this.tableDefs.Contains(tableDef))
        {
            this.tableDefs.Add(tableDef);
        }

        return this;
    }

    /// <summary>
    /// Customs the join.
    /// </summary>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinString">The join string.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> CustomJoin<Target>(string joinString)
    {
        this.AddReferenceTableIfNotExists<Target>();
        return this.CustomJoin(joinString);
    }

    /// <summary>
    /// Determines whether [is joined table] [the specified type].
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns><c>true</c> if [is joined table] [the specified type]; otherwise, <c>false</c>.</returns>
    public bool IsJoinedTable(Type type)
    {
        return this.tableDefs.FirstOrDefault(x => x.ModelType == type) != null;
    }

    /// <summary>
    /// Joins the specified join expr.
    /// </summary>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> Join<Target>(Expression<Func<T, Target, bool>> joinExpr = null)
    {
        return this.InternalJoin("INNER JOIN", joinExpr);
    }

    /// <summary>
    /// Joins the specified join expr.
    /// </summary>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <param name="options">The options.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    /// <exception cref="System.ArgumentNullException">options</exception>
    /// <exception cref="System.ArgumentException">Can't set both Join Expression and TableOptions Expression</exception>
    public SqlExpression<T> Join<Target>(Expression<Func<T, Target, bool>> joinExpr, TableOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (options.Expression != null)
        {
            throw new ArgumentException("Can't set both Join Expression and TableOptions Expression");
        }

        return this.InternalJoin("INNER JOIN", joinExpr, options);
    }

    /// <summary>
    /// Joins the specified join expr.
    /// </summary>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <param name="joinFormat">The join format.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    /// <exception cref="System.ArgumentNullException">joinFormat</exception>
    public SqlExpression<T> Join<Target>(Expression<Func<T, Target, bool>> joinExpr, JoinFormatDelegate joinFormat)
    {
        return this.InternalJoin("INNER JOIN", joinExpr,
            joinFormat ?? throw new ArgumentNullException(nameof(joinFormat)));
    }

    /// <summary>
    /// Joins the specified join expr.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> Join<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr = null)
    {
        return this.InternalJoin("INNER JOIN", joinExpr);
    }

    /// <summary>
    /// Joins the specified join expr.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <param name="joinFormat">The join format.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> Join<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr, JoinFormatDelegate joinFormat)
    {
        return this.InternalJoin("INNER JOIN", joinExpr, joinFormat);
    }

    /// <summary>
    /// Joins the specified join expr.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <param name="options">The options.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> Join<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr, TableOptions options)
    {
        return this.InternalJoin("INNER JOIN", joinExpr, options);
    }

    /// <summary>
    /// Joins the specified source type.
    /// </summary>
    /// <param name="sourceType">Type of the source.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> Join(Type sourceType, Type targetType, Expression joinExpr = null)
    {
        return this.InternalJoin("INNER JOIN", joinExpr, sourceType.GetModelDefinition(), targetType.GetModelDefinition());
    }

    /// <summary>
    /// Joins the specified source type.
    /// </summary>
    /// <param name="sourceType">Type of the source.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="joinExpr">The join expr.</param>
    /// <param name="options">The options.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> Join(Type sourceType, Type targetType, Expression joinExpr, TableOptions options)
    {
        return this.InternalJoin("INNER JOIN", joinExpr, sourceType.GetModelDefinition(), targetType.GetModelDefinition(),
            options);
    }

    /// <summary>
    /// Lefts the join.
    /// </summary>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> LeftJoin<Target>(Expression<Func<T, Target, bool>> joinExpr = null)
    {
        return this.InternalJoin("LEFT JOIN", joinExpr);
    }

    /// <summary>
    /// Lefts the join.
    /// </summary>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <param name="joinFormat">The join format.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    /// <exception cref="System.ArgumentNullException">joinFormat</exception>
    public SqlExpression<T> LeftJoin<Target>(Expression<Func<T, Target, bool>> joinExpr, JoinFormatDelegate joinFormat)
    {
        return this.InternalJoin("LEFT JOIN", joinExpr,
            joinFormat ?? throw new ArgumentNullException(nameof(joinFormat)));
    }

    /// <summary>
    /// Lefts the join.
    /// </summary>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <param name="options">The options.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    /// <exception cref="System.ArgumentNullException">options</exception>
    public SqlExpression<T> LeftJoin<Target>(Expression<Func<T, Target, bool>> joinExpr, TableOptions options)
    {
        return this.InternalJoin("LEFT JOIN", joinExpr, options ?? throw new ArgumentNullException(nameof(options)));
    }

    /// <summary>
    /// Lefts the join.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> LeftJoin<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr = null)
    {
        return this.InternalJoin("LEFT JOIN", joinExpr);
    }

    /// <summary>
    /// Lefts the join.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <param name="joinFormat">The join format.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> LeftJoin<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr, JoinFormatDelegate joinFormat)
    {
        return this.InternalJoin("LEFT JOIN", joinExpr, joinFormat);
    }

    /// <summary>
    /// Lefts the join.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <param name="options">The options.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> LeftJoin<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr, TableOptions options)
    {
        return this.InternalJoin("LEFT JOIN", joinExpr, options);
    }

    /// <summary>
    /// Lefts the join.
    /// </summary>
    /// <param name="sourceType">Type of the source.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> LeftJoin(Type sourceType, Type targetType, Expression joinExpr = null)
    {
        return this.InternalJoin("LEFT JOIN", joinExpr, sourceType.GetModelDefinition(), targetType.GetModelDefinition());
    }

    /// <summary>
    /// Lefts the join.
    /// </summary>
    /// <param name="sourceType">Type of the source.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="joinExpr">The join expr.</param>
    /// <param name="options">The options.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> LeftJoin(Type sourceType, Type targetType, Expression joinExpr, TableOptions options)
    {
        return this.InternalJoin("LEFT JOIN", joinExpr, sourceType.GetModelDefinition(), targetType.GetModelDefinition(),
            options);
    }

    /// <summary>
    /// Rights the join.
    /// </summary>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> RightJoin<Target>(Expression<Func<T, Target, bool>> joinExpr = null)
    {
        return this.InternalJoin("RIGHT JOIN", joinExpr);
    }

    /// <summary>
    /// Rights the join.
    /// </summary>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <param name="joinFormat">The join format.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    /// <exception cref="System.ArgumentNullException">joinFormat</exception>
    public SqlExpression<T> RightJoin<Target>(Expression<Func<T, Target, bool>> joinExpr, JoinFormatDelegate joinFormat)
    {
        return this.InternalJoin("RIGHT JOIN", joinExpr,
            joinFormat ?? throw new ArgumentNullException(nameof(joinFormat)));
    }

    /// <summary>
    /// Rights the join.
    /// </summary>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <param name="options">The options.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    /// <exception cref="System.ArgumentNullException">options</exception>
    public SqlExpression<T> RightJoin<Target>(Expression<Func<T, Target, bool>> joinExpr, TableOptions options)
    {
        return this.InternalJoin("RIGHT JOIN", joinExpr, options ?? throw new ArgumentNullException(nameof(options)));
    }

    /// <summary>
    /// Rights the join.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> RightJoin<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr = null)
    {
        return this.InternalJoin("RIGHT JOIN", joinExpr);
    }

    /// <summary>
    /// Rights the join.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <param name="joinFormat">The join format.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> RightJoin<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr, JoinFormatDelegate joinFormat)
    {
        return this.InternalJoin("RIGHT JOIN", joinExpr, joinFormat);
    }

    /// <summary>
    /// Rights the join.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <param name="options">The options.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> RightJoin<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr, TableOptions options)
    {
        return this.InternalJoin("RIGHT JOIN", joinExpr, options);
    }

    /// <summary>
    /// Rights the join.
    /// </summary>
    /// <param name="sourceType">Type of the source.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> RightJoin(Type sourceType, Type targetType, Expression joinExpr = null)
    {
        return this.InternalJoin("RIGHT JOIN", joinExpr, sourceType.GetModelDefinition(), targetType.GetModelDefinition());
    }

    /// <summary>
    /// Rights the join.
    /// </summary>
    /// <param name="sourceType">Type of the source.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="joinExpr">The join expr.</param>
    /// <param name="options">The options.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> RightJoin(Type sourceType, Type targetType, Expression joinExpr, TableOptions options)
    {
        return this.InternalJoin("RIGHT JOIN", joinExpr, sourceType.GetModelDefinition(), targetType.GetModelDefinition(),
            options);
    }

    /// <summary>
    /// Fulls the join.
    /// </summary>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> FullJoin<Target>(Expression<Func<T, Target, bool>> joinExpr = null)
    {
        return this.InternalJoin("FULL JOIN", joinExpr);
    }

    /// <summary>
    /// Fulls the join.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> FullJoin<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr = null)
    {
        return this.InternalJoin("FULL JOIN", joinExpr);
    }

    /// <summary>
    /// Crosses the join.
    /// </summary>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> CrossJoin<Target>(Expression<Func<T, Target, bool>> joinExpr = null)
    {
        return this.InternalJoin("CROSS JOIN", joinExpr);
    }

    /// <summary>
    /// Crosses the join.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> CrossJoin<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr = null)
    {
        return this.InternalJoin("CROSS JOIN", joinExpr);
    }

    /// <summary>
    /// Internals the join.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinType">Type of the join.</param>
    /// <param name="joinExpr">The join expr.</param>
    /// <param name="joinFormat">The join format.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    protected SqlExpression<T> InternalJoin<Source, Target>(string joinType, Expression<Func<Source, Target, bool>> joinExpr, JoinFormatDelegate joinFormat)
    {
        return this.InternalJoin(joinType, joinExpr,
            joinFormat != null ? new TableOptions { JoinFormat = joinFormat } : null);
    }

    /// <summary>
    /// Internals the join.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinType">Type of the join.</param>
    /// <param name="joinExpr">The join expr.</param>
    /// <param name="options">The options.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    protected SqlExpression<T> InternalJoin<Source, Target>(string joinType, Expression<Func<Source, Target, bool>> joinExpr, TableOptions options = null)
    {
        var sourceDef = typeof(Source).GetModelDefinition();
        var targetDef = typeof(Target).GetModelDefinition();

        return this.InternalJoin(joinType, joinExpr, sourceDef, targetDef, options);
    }

    /// <summary>
    /// Internals the join.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinType">Type of the join.</param>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    protected SqlExpression<T> InternalJoin<Source, Target>(string joinType, Expression joinExpr)
    {
        var sourceDef = typeof(Source).GetModelDefinition();
        var targetDef = typeof(Target).GetModelDefinition();

        return this.InternalJoin(joinType, joinExpr, sourceDef, targetDef);
    }

    /// <summary>
    /// Joins the specified join expr.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> Join<Source, Target, T3>(Expression<Func<Source, Target, T3, bool>> joinExpr)
    {
        return this.InternalJoin<Source, Target>("INNER JOIN", joinExpr);
    }

    /// <summary>
    /// Lefts the join.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> LeftJoin<Source, Target, T3>(Expression<Func<Source, Target, T3, bool>> joinExpr)
    {
        return this.InternalJoin<Source, Target>("LEFT JOIN", joinExpr);
    }

    /// <summary>
    /// Rights the join.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> RightJoin<Source, Target, T3>(Expression<Func<Source, Target, T3, bool>> joinExpr)
    {
        return this.InternalJoin<Source, Target>("RIGHT JOIN", joinExpr);
    }

    /// <summary>
    /// Fulls the join.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> FullJoin<Source, Target, T3>(Expression<Func<Source, Target, T3, bool>> joinExpr)
    {
        return this.InternalJoin<Source, Target>("FULL JOIN", joinExpr);
    }

    /// <summary>
    /// Joins the specified join expr.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> Join<Source, Target, T3, T4>(Expression<Func<Source, Target, T3, T4, bool>> joinExpr)
    {
        return this.InternalJoin<Source, Target>("INNER JOIN", joinExpr);
    }

    /// <summary>
    /// Lefts the join.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> LeftJoin<Source, Target, T3, T4>(Expression<Func<Source, Target, T3, T4, bool>> joinExpr)
    {
        return this.InternalJoin<Source, Target>("LEFT JOIN", joinExpr);
    }

    /// <summary>
    /// Rights the join.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> RightJoin<Source, Target, T3, T4>(Expression<Func<Source, Target, T3, T4, bool>> joinExpr)
    {
        return this.InternalJoin<Source, Target>("RIGHT JOIN", joinExpr);
    }

    /// <summary>
    /// Fulls the join.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> FullJoin<Source, Target, T3, T4>(Expression<Func<Source, Target, T3, T4, bool>> joinExpr)
    {
        return this.InternalJoin<Source, Target>("FULL JOIN", joinExpr);
    }

    /// <summary>
    /// Internals the create SQL from expression.
    /// </summary>
    /// <param name="joinExpr">The join expr.</param>
    /// <param name="isCrossJoin">if set to <c>true</c> [is cross join].</param>
    /// <returns>System.String.</returns>
    private string InternalCreateSqlFromExpression(Expression joinExpr, bool isCrossJoin)
    {
        return $"{(isCrossJoin ? "WHERE" : "ON")} {this.VisitJoin(joinExpr)}";
    }

    /// <summary>
    /// Internals the create SQL from definitions.
    /// </summary>
    /// <param name="sourceDef">The source definition.</param>
    /// <param name="targetDef">The target definition.</param>
    /// <param name="isCrossJoin">if set to <c>true</c> [is cross join].</param>
    /// <returns>System.String.</returns>
    /// <exception cref="System.ArgumentException">Could not infer relationship between {sourceDef.ModelName} and {targetDef.ModelName}</exception>
    private string InternalCreateSqlFromDefinitions(ModelDefinition sourceDef, ModelDefinition targetDef, bool isCrossJoin)
    {
        var parentDef = sourceDef;
        var childDef = targetDef;

        var refField = parentDef.GetExplicitRefFieldDefIfExists(childDef);
        if (refField == null && childDef.GetExplicitRefFieldDefIfExists(parentDef) != null)
        {
            parentDef = targetDef;
            childDef = sourceDef;
            refField = parentDef.GetExplicitRefFieldDefIfExists(childDef);
        }
        else
        {
            refField = parentDef.GetRefFieldDefIfExists(childDef); if (refField == null)
            {
                parentDef = targetDef;
                childDef = sourceDef;
                refField = parentDef.GetRefFieldDefIfExists(childDef);
            }
        }

        if (refField == null)
        {
            if (!isCrossJoin)
            {
                throw new ArgumentException($"Could not infer relationship between {sourceDef.ModelName} and {targetDef.ModelName}");
            }

            return string.Empty;
        }

        return string.Format("{0}\n({1}.{2} = {3}.{4})",
            isCrossJoin ? "WHERE" : "ON",
            this.DialectProvider.GetQuotedTableName(parentDef),
            this.SqlColumn(parentDef.PrimaryKey.FieldName),
            this.DialectProvider.GetQuotedTableName(childDef),
            this.SqlColumn(refField.FieldName));
    }

    /// <summary>
    /// Customs the join.
    /// </summary>
    /// <param name="joinString">The join string.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public SqlExpression<T> CustomJoin(string joinString)
    {
        this.PrefixFieldWithTableName = true;
        this.FromExpression += " " + joinString;
        return this;
    }

    /// <summary>
    /// Hold the <see cref="TableOptions" /> for each Join and clear them at the end of the Join
    /// </summary>
    private TableOptions joinAlias;

    /// <summary>
    /// If <see cref="UseJoinTypeAsAliases" /> is enabled, record the <see cref="TableOptions" /> set for different types each time Join
    /// </summary>
    private Dictionary<ModelDefinition, TableOptions> joinAliases;

    /// <summary>
    /// Internals the join.
    /// </summary>
    /// <param name="joinType">Type of the join.</param>
    /// <param name="joinExpr">The join expr.</param>
    /// <param name="sourceDef">The source definition.</param>
    /// <param name="targetDef">The target definition.</param>
    /// <param name="options">The options.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    protected virtual SqlExpression<T> InternalJoin(string joinType, Expression joinExpr, ModelDefinition sourceDef, ModelDefinition targetDef, TableOptions options = null)
    {
        this.PrefixFieldWithTableName = true;

        this.Reset();

        var joinFormat = options?.JoinFormat;
        if (options?.Alias != null)
        {
            // Set joinAlias
            options.ParamName = joinExpr is LambdaExpression l && l.Parameters.Count == 2
                                    ? l.Parameters[1].Name
                                    : null;
            if (options.ParamName != null)
            {
                joinFormat = null;
                options.ModelDef = targetDef;
                this.joinAlias = options;

                if (this.UseJoinTypeAsAliases)
                {
                    this.joinAliases ??= [];
                    //If join multiple times and set different TableOptions, only the last setting will be used
                    this.joinAliases[targetDef] = options;
                }
            }
        }

        if (!this.tableDefs.Contains(sourceDef))
        {
            this.tableDefs.Add(sourceDef);
        }

        if (!this.tableDefs.Contains(targetDef))
        {
            this.tableDefs.Add(targetDef);
        }

        var isCrossJoin = "CROSS JOIN" == joinType;

        var sqlExpr = joinExpr != null
                          ? this.InternalCreateSqlFromExpression(joinExpr, isCrossJoin)
                          : this.InternalCreateSqlFromDefinitions(sourceDef, targetDef, isCrossJoin);

        var joinDef = this.tableDefs.Contains(targetDef) && !this.tableDefs.Contains(sourceDef)
                          ? sourceDef
                          : targetDef;

        this.FromExpression += joinFormat != null
                                   ? $" {joinType} {joinFormat(this.DialectProvider, joinDef, sqlExpr)}"
                                   : this.joinAlias != null
                                       ? $" {joinType} {this.SqlTable(joinDef)} {this.DialectProvider.GetQuotedName(this.joinAlias.Alias)} {sqlExpr}"
                                       : $" {joinType} {this.SqlTable(joinDef)} {sqlExpr}";


        if (this.joinAlias != null)
        {
            // Unset joinAlias
            this.joinAlias = null;
            if (options != null)
            {
                options.ParamName = null;
                options.ModelDef = null;
            }
        }

        return this;
    }

    /// <summary>
    /// Selects the into.
    /// </summary>
    /// <typeparam name="TModel">The type of the t model.</typeparam>
    /// <returns>string.</returns>
    public string SelectInto<TModel>()
    {
        return this.SelectInto<TModel>(QueryType.Select);
    }

    /// <summary>
    /// Selects the into.
    /// </summary>
    /// <typeparam name="TModel">The type of the t model.</typeparam>
    /// <param name="queryType">Type of the query.</param>
    /// <returns>System.String.</returns>
    public string SelectInto<TModel>(QueryType queryType)
    {
        if (this.CustomSelect && this.OnlyFields == null || typeof(TModel) == typeof(T) && !this.PrefixFieldWithTableName)
        {
            return this.ToSelectStatement(queryType);
        }

        this.useFieldName = true;

        var sbSelect = StringBuilderCache.Allocate();
        var selectDef = this.modelDef;
        var orderedDefs = this.tableDefs;

        if (typeof(TModel) != typeof(List<object>) &&
            typeof(TModel) != typeof(Dictionary<string, object>) &&
            typeof(TModel) != typeof(object) && // dynamic
            !typeof(TModel).IsValueTuple())
        {
            selectDef = typeof(TModel).GetModelDefinition();
            if (selectDef != this.modelDef && this.tableDefs.Contains(selectDef))
            {
                orderedDefs = [.. this.tableDefs]; // clone
                orderedDefs.Remove(selectDef);
                orderedDefs.Insert(0, selectDef);
            }
        }

        foreach (var fieldDef in selectDef.FieldDefinitions)
        {
            var found = false;

            if (fieldDef.BelongToModelName != null)
            {
                var tableDef = orderedDefs.FirstOrDefault(x => x.Name == fieldDef.BelongToModelName);
                if (tableDef != null)
                {
                    var matchingField = FindWeakMatch(tableDef, fieldDef);
                    if (matchingField != null)
                    {
                        if (this.OnlyFields == null || this.OnlyFields.Contains(fieldDef.Name))
                        {
                            if (sbSelect.Length > 0)
                            {
                                sbSelect.Append(", ");
                            }

                            if (fieldDef.CustomSelect == null)
                            {
                                if (!fieldDef.IsRowVersion)
                                {
                                    sbSelect.Append($"{this.GetQuotedColumnName(tableDef, matchingField.Name)} AS {this.SqlColumn(fieldDef.Name)}");
                                }
                                else
                                {
                                    sbSelect.Append(this.DialectProvider.GetRowVersionSelectColumn(fieldDef, this.DialectProvider.GetTableName(tableDef.ModelName)));
                                }
                            }
                            else
                            {
                                sbSelect.Append(fieldDef.CustomSelect + " AS " + fieldDef.FieldName);
                            }

                            continue;
                        }
                    }
                }
            }

            foreach (var tableDef in orderedDefs)
            {
                foreach (var tableFieldDef in tableDef.FieldDefinitions)
                {
                    if (tableFieldDef.Name == fieldDef.Name)
                    {
                        if (this.OnlyFields != null && !this.OnlyFields.Contains(fieldDef.Name))
                        {
                            continue;
                        }

                        if (sbSelect.Length > 0)
                        {
                            sbSelect.Append(", ");
                        }

                        var tableAlias = tableDef == this.modelDef // Use TableAlias if source modelDef
                                             ? this.TableAlias
                                             : null;

                        if (fieldDef.CustomSelect == null)
                        {
                            if (!fieldDef.IsRowVersion)
                            {
                                sbSelect.Append(tableAlias == null
                                                    ? this.GetQuotedColumnName(tableDef, tableFieldDef.Name)
                                                    : this.GetQuotedColumnName(tableDef, tableAlias, tableFieldDef.Name));

                                if (tableFieldDef.RequiresAlias)
                                {
                                    sbSelect.Append(" AS ").Append(this.SqlColumn(fieldDef.Name));
                                }
                            }
                            else
                            {
                                sbSelect.Append(this.DialectProvider.GetRowVersionSelectColumn(fieldDef, this.DialectProvider.GetTableName(tableAlias ?? tableDef.ModelName, tableDef.Schema)));
                            }
                        }
                        else
                        {
                            sbSelect.Append(tableFieldDef.CustomSelect).Append(" AS ").Append(tableFieldDef.FieldName);
                        }

                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    break;
                }
            }

            if (!found)
            {
                // Add support for auto mapping `{Table}{Field}` convention
                foreach (var tableDef in orderedDefs)
                {
                    var matchingField = FindWeakMatch(tableDef, fieldDef);
                    if (matchingField != null)
                    {
                        if (this.OnlyFields != null && !this.OnlyFields.Contains(fieldDef.Name))
                        {
                            continue;
                        }

                        if (sbSelect.Length > 0)
                        {
                            sbSelect.Append(", ");
                        }

                        var tableAlias = tableDef == this.modelDef // Use TableAlias if source modelDef
                                             ? this.TableAlias
                                             : null;

                        sbSelect.Append($"{this.DialectProvider.GetQuotedColumnName(tableDef, tableAlias, matchingField)} as {this.SqlColumn(fieldDef.Name)}");

                        break;
                    }
                }
            }
        }

        var select = StringBuilderCache.ReturnAndFree(sbSelect);

        var columns = select.Length > 0 ? select : "*";
        this.SelectExpression = "SELECT " + (this.selectDistinct ? "DISTINCT " : string.Empty) + columns;

        return this.ToSelectStatement(queryType);
    }

    /// <summary>
    /// Finds the weak match.
    /// </summary>
    /// <param name="tableDef">The table definition.</param>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>FieldDefinition.</returns>
    private static FieldDefinition FindWeakMatch(ModelDefinition tableDef, FieldDefinition fieldDef)
    {
        return tableDef.FieldDefinitions
            .FirstOrDefault(x =>
                string.Compare(tableDef.Name + x.Name, fieldDef.Name, StringComparison.OrdinalIgnoreCase) == 0
                || string.Compare(tableDef.ModelName + x.FieldName, fieldDef.Name, StringComparison.OrdinalIgnoreCase) == 0);
    }

    /// <summary>
    /// Wheres the specified predicate.
    /// </summary>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Where<Target>(Expression<Func<Target, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Wheres the specified predicate.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Where<Source, Target>(Expression<Func<Source, Target, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Wheres the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Where<T1, T2, T3>(Expression<Func<T1, T2, T3, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Wheres the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Where<T1, T2, T3, T4>(Expression<Func<T1, T2, T3, T4, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Wheres the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Where<T1, T2, T3, T4, T5>(Expression<Func<T1, T2, T3, T4, T5, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Wheres the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Where<T1, T2, T3, T4, T5, T6>(Expression<Func<T1, T2, T3, T4, T5, T6, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Wheres the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <typeparam name="T7">The type of the t7.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Where<T1, T2, T3, T4, T5, T6, T7>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Wheres the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <typeparam name="T7">The type of the t7.</typeparam>
    /// <typeparam name="T8">The type of the t8.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Where<T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Wheres the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <typeparam name="T7">The type of the t7.</typeparam>
    /// <typeparam name="T8">The type of the t8.</typeparam>
    /// <typeparam name="T9">The type of the t9.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Where<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Wheres the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <typeparam name="T7">The type of the t7.</typeparam>
    /// <typeparam name="T8">The type of the t8.</typeparam>
    /// <typeparam name="T9">The type of the t9.</typeparam>
    /// <typeparam name="T10">The type of the T10.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Where<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Wheres the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <typeparam name="T7">The type of the t7.</typeparam>
    /// <typeparam name="T8">The type of the t8.</typeparam>
    /// <typeparam name="T9">The type of the t9.</typeparam>
    /// <typeparam name="T10">The type of the T10.</typeparam>
    /// <typeparam name="T11">The type of the T11.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Where<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Wheres the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <typeparam name="T7">The type of the t7.</typeparam>
    /// <typeparam name="T8">The type of the t8.</typeparam>
    /// <typeparam name="T9">The type of the t9.</typeparam>
    /// <typeparam name="T10">The type of the T10.</typeparam>
    /// <typeparam name="T11">The type of the T11.</typeparam>
    /// <typeparam name="T12">The type of the T12.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Where<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Wheres the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <typeparam name="T7">The type of the t7.</typeparam>
    /// <typeparam name="T8">The type of the t8.</typeparam>
    /// <typeparam name="T9">The type of the t9.</typeparam>
    /// <typeparam name="T10">The type of the T10.</typeparam>
    /// <typeparam name="T11">The type of the T11.</typeparam>
    /// <typeparam name="T12">The type of the T12.</typeparam>
    /// <typeparam name="T13">The type of the T13.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Where<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Wheres the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <typeparam name="T7">The type of the t7.</typeparam>
    /// <typeparam name="T8">The type of the t8.</typeparam>
    /// <typeparam name="T9">The type of the t9.</typeparam>
    /// <typeparam name="T10">The type of the T10.</typeparam>
    /// <typeparam name="T11">The type of the T11.</typeparam>
    /// <typeparam name="T12">The type of the T12.</typeparam>
    /// <typeparam name="T13">The type of the T13.</typeparam>
    /// <typeparam name="T14">The type of the T14.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Where<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Wheres the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <typeparam name="T7">The type of the t7.</typeparam>
    /// <typeparam name="T8">The type of the t8.</typeparam>
    /// <typeparam name="T9">The type of the t9.</typeparam>
    /// <typeparam name="T10">The type of the T10.</typeparam>
    /// <typeparam name="T11">The type of the T11.</typeparam>
    /// <typeparam name="T12">The type of the T12.</typeparam>
    /// <typeparam name="T13">The type of the T13.</typeparam>
    /// <typeparam name="T14">The type of the T14.</typeparam>
    /// <typeparam name="T15">The type of the T15.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Where<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Ands the specified predicate.
    /// </summary>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> And<Target>(Expression<Func<Target, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Ands the specified predicate.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> And<Source, Target>(Expression<Func<Source, Target, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Ands the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> And<T1, T2, T3>(Expression<Func<T1, T2, T3, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Ands the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> And<T1, T2, T3, T4>(Expression<Func<T1, T2, T3, T4, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Ands the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> And<T1, T2, T3, T4, T5>(Expression<Func<T1, T2, T3, T4, T5, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Ands the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> And<T1, T2, T3, T4, T5, T6>(Expression<Func<T1, T2, T3, T4, T5, T6, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Ands the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <typeparam name="T7">The type of the t7.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> And<T1, T2, T3, T4, T5, T6, T7>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Ands the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <typeparam name="T7">The type of the t7.</typeparam>
    /// <typeparam name="T8">The type of the t8.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> And<T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Ands the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <typeparam name="T7">The type of the t7.</typeparam>
    /// <typeparam name="T8">The type of the t8.</typeparam>
    /// <typeparam name="T9">The type of the t9.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> And<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Ands the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <typeparam name="T7">The type of the t7.</typeparam>
    /// <typeparam name="T8">The type of the t8.</typeparam>
    /// <typeparam name="T9">The type of the t9.</typeparam>
    /// <typeparam name="T10">The type of the T10.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> And<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Ands the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <typeparam name="T7">The type of the t7.</typeparam>
    /// <typeparam name="T8">The type of the t8.</typeparam>
    /// <typeparam name="T9">The type of the t9.</typeparam>
    /// <typeparam name="T10">The type of the T10.</typeparam>
    /// <typeparam name="T11">The type of the T11.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> And<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Ands the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <typeparam name="T7">The type of the t7.</typeparam>
    /// <typeparam name="T8">The type of the t8.</typeparam>
    /// <typeparam name="T9">The type of the t9.</typeparam>
    /// <typeparam name="T10">The type of the T10.</typeparam>
    /// <typeparam name="T11">The type of the T11.</typeparam>
    /// <typeparam name="T12">The type of the T12.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> And<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Ands the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <typeparam name="T7">The type of the t7.</typeparam>
    /// <typeparam name="T8">The type of the t8.</typeparam>
    /// <typeparam name="T9">The type of the t9.</typeparam>
    /// <typeparam name="T10">The type of the T10.</typeparam>
    /// <typeparam name="T11">The type of the T11.</typeparam>
    /// <typeparam name="T12">The type of the T12.</typeparam>
    /// <typeparam name="T13">The type of the T13.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> And<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Ands the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <typeparam name="T7">The type of the t7.</typeparam>
    /// <typeparam name="T8">The type of the t8.</typeparam>
    /// <typeparam name="T9">The type of the t9.</typeparam>
    /// <typeparam name="T10">The type of the T10.</typeparam>
    /// <typeparam name="T11">The type of the T11.</typeparam>
    /// <typeparam name="T12">The type of the T12.</typeparam>
    /// <typeparam name="T13">The type of the T13.</typeparam>
    /// <typeparam name="T14">The type of the T14.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> And<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Ands the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <typeparam name="T7">The type of the t7.</typeparam>
    /// <typeparam name="T8">The type of the t8.</typeparam>
    /// <typeparam name="T9">The type of the t9.</typeparam>
    /// <typeparam name="T10">The type of the T10.</typeparam>
    /// <typeparam name="T11">The type of the T11.</typeparam>
    /// <typeparam name="T12">The type of the T12.</typeparam>
    /// <typeparam name="T13">The type of the T13.</typeparam>
    /// <typeparam name="T14">The type of the T14.</typeparam>
    /// <typeparam name="T15">The type of the T15.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> And<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, bool>> predicate)
    {
        return this.AppendToWhere("AND", predicate);
    }

    /// <summary>
    /// Ors the specified predicate.
    /// </summary>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Or<Target>(Expression<Func<Target, bool>> predicate)
    {
        return this.AppendToWhere("OR", predicate);
    }

    /// <summary>
    /// Ors the specified predicate.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Or<Source, Target>(Expression<Func<Source, Target, bool>> predicate)
    {
        return this.AppendToWhere("OR", predicate);
    }

    /// <summary>
    /// Ors the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Or<T1, T2, T3>(Expression<Func<T1, T2, T3, bool>> predicate)
    {
        return this.AppendToWhere("OR", predicate);
    }

    /// <summary>
    /// Ors the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Or<T1, T2, T3, T4>(Expression<Func<T1, T2, T3, T4, bool>> predicate)
    {
        return this.AppendToWhere("OR", predicate);
    }

    /// <summary>
    /// Ors the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Or<T1, T2, T3, T4, T5>(Expression<Func<T1, T2, T3, T4, T5, bool>> predicate)
    {
        return this.AppendToWhere("OR", predicate);
    }

    /// <summary>
    /// Ors the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Or<T1, T2, T3, T4, T5, T6>(Expression<Func<T1, T2, T3, T4, T5, T6, bool>> predicate)
    {
        return this.AppendToWhere("OR", predicate);
    }

    /// <summary>
    /// Ors the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <typeparam name="T7">The type of the t7.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Or<T1, T2, T3, T4, T5, T6, T7>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, bool>> predicate)
    {
        return this.AppendToWhere("OR", predicate);
    }

    /// <summary>
    /// Ors the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <typeparam name="T7">The type of the t7.</typeparam>
    /// <typeparam name="T8">The type of the t8.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Or<T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, bool>> predicate)
    {
        return this.AppendToWhere("OR", predicate);
    }

    /// <summary>
    /// Ors the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <typeparam name="T7">The type of the t7.</typeparam>
    /// <typeparam name="T8">The type of the t8.</typeparam>
    /// <typeparam name="T9">The type of the t9.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Or<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, bool>> predicate)
    {
        return this.AppendToWhere("OR", predicate);
    }

    /// <summary>
    /// Ors the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <typeparam name="T7">The type of the t7.</typeparam>
    /// <typeparam name="T8">The type of the t8.</typeparam>
    /// <typeparam name="T9">The type of the t9.</typeparam>
    /// <typeparam name="T10">The type of the T10.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Or<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> predicate)
    {
        return this.AppendToWhere("OR", predicate);
    }

    /// <summary>
    /// Ors the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <typeparam name="T7">The type of the t7.</typeparam>
    /// <typeparam name="T8">The type of the t8.</typeparam>
    /// <typeparam name="T9">The type of the t9.</typeparam>
    /// <typeparam name="T10">The type of the T10.</typeparam>
    /// <typeparam name="T11">The type of the T11.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Or<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> predicate)
    {
        return this.AppendToWhere("OR", predicate);
    }

    /// <summary>
    /// Ors the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <typeparam name="T7">The type of the t7.</typeparam>
    /// <typeparam name="T8">The type of the t8.</typeparam>
    /// <typeparam name="T9">The type of the t9.</typeparam>
    /// <typeparam name="T10">The type of the T10.</typeparam>
    /// <typeparam name="T11">The type of the T11.</typeparam>
    /// <typeparam name="T12">The type of the T12.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Or<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> predicate)
    {
        return this.AppendToWhere("OR", predicate);
    }

    /// <summary>
    /// Ors the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <typeparam name="T7">The type of the t7.</typeparam>
    /// <typeparam name="T8">The type of the t8.</typeparam>
    /// <typeparam name="T9">The type of the t9.</typeparam>
    /// <typeparam name="T10">The type of the T10.</typeparam>
    /// <typeparam name="T11">The type of the T11.</typeparam>
    /// <typeparam name="T12">The type of the T12.</typeparam>
    /// <typeparam name="T13">The type of the T13.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Or<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, bool>> predicate)
    {
        return this.AppendToWhere("OR", predicate);
    }

    /// <summary>
    /// Ors the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <typeparam name="T7">The type of the t7.</typeparam>
    /// <typeparam name="T8">The type of the t8.</typeparam>
    /// <typeparam name="T9">The type of the t9.</typeparam>
    /// <typeparam name="T10">The type of the T10.</typeparam>
    /// <typeparam name="T11">The type of the T11.</typeparam>
    /// <typeparam name="T12">The type of the T12.</typeparam>
    /// <typeparam name="T13">The type of the T13.</typeparam>
    /// <typeparam name="T14">The type of the T14.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Or<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, bool>> predicate)
    {
        return this.AppendToWhere("OR", predicate);
    }

    /// <summary>
    /// Ors the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <typeparam name="T7">The type of the t7.</typeparam>
    /// <typeparam name="T8">The type of the t8.</typeparam>
    /// <typeparam name="T9">The type of the t9.</typeparam>
    /// <typeparam name="T10">The type of the T10.</typeparam>
    /// <typeparam name="T11">The type of the T11.</typeparam>
    /// <typeparam name="T12">The type of the T12.</typeparam>
    /// <typeparam name="T13">The type of the T13.</typeparam>
    /// <typeparam name="T14">The type of the T14.</typeparam>
    /// <typeparam name="T15">The type of the T15.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> Or<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, bool>> predicate)
    {
        return this.AppendToWhere("OR", predicate);
    }

    /// <summary>
    /// Firsts the matching field.
    /// </summary>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>Tuple&lt;ModelDefinition, FieldDefinition&gt;.</returns>
    public Tuple<ModelDefinition, FieldDefinition> FirstMatchingField(string fieldName)
    {
        foreach (var tableDef in this.tableDefs)
        {
            var firstField = tableDef.FieldDefinitions.FirstOrDefault(
                x => string.Compare(x.Name, fieldName, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(
                         x.FieldName,
                         fieldName,
                         StringComparison.OrdinalIgnoreCase) == 0);

            if (firstField != null)
            {
                return Tuple.Create(tableDef, firstField);
            }
        }

        // Fallback to fully qualified '{Table}{Field}' property convention
        foreach (var tableDef in this.tableDefs)
        {
            var firstField = tableDef.FieldDefinitions.FirstOrDefault(
                x => string.Compare(tableDef.Name + x.Name, fieldName, StringComparison.OrdinalIgnoreCase) == 0 ||
                     string.Compare(
                         tableDef.ModelName + x.FieldName,
                         fieldName,
                         StringComparison.OrdinalIgnoreCase) == 0);

            if (firstField != null)
            {
                return Tuple.Create(tableDef, firstField);
            }
        }

        return null;
    }
}