// ***********************************************************************
// <copyright file="IUntypedSqlExpression.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.OrmLite;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

/// <summary>
/// Interface IHasUntypedSqlExpression
/// </summary>
public interface IHasUntypedSqlExpression
{
    /// <summary>
    /// Gets the untyped.
    /// </summary>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression GetUntyped();
}

/// <summary>
/// Interface IUntypedSqlExpression
/// Implements the <see cref="ServiceStack.OrmLite.ISqlExpression" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.ISqlExpression" />
public interface IUntypedSqlExpression : ISqlExpression
{
    /// <summary>
    /// Gets or sets the table alias.
    /// </summary>
    /// <value>The table alias.</value>
    string TableAlias { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [prefix field with table name].
    /// </summary>
    /// <value><c>true</c> if [prefix field with table name]; otherwise, <c>false</c>.</value>
    bool PrefixFieldWithTableName { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [where statement without where string].
    /// </summary>
    /// <value><c>true</c> if [where statement without where string]; otherwise, <c>false</c>.</value>
    bool WhereStatementWithoutWhereString { get; set; }
    /// <summary>
    /// Gets or sets the dialect provider.
    /// </summary>
    /// <value>The dialect provider.</value>
    IOrmLiteDialectProvider DialectProvider { get; set; }
    /// <summary>
    /// Gets or sets the select expression.
    /// </summary>
    /// <value>The select expression.</value>
    string SelectExpression { get; set; }
    /// <summary>
    /// Gets or sets from expression.
    /// </summary>
    /// <value>From expression.</value>
    string FromExpression { get; set; }
    /// <summary>
    /// Gets the body expression.
    /// </summary>
    /// <value>The body expression.</value>
    string BodyExpression { get; }
    /// <summary>
    /// Gets or sets the where expression.
    /// </summary>
    /// <value>The where expression.</value>
    string WhereExpression { get; set; }
    /// <summary>
    /// Gets or sets the group by expression.
    /// </summary>
    /// <value>The group by expression.</value>
    string GroupByExpression { get; set; }
    /// <summary>
    /// Gets or sets the having expression.
    /// </summary>
    /// <value>The having expression.</value>
    string HavingExpression { get; set; }
    /// <summary>
    /// Gets or sets the order by expression.
    /// </summary>
    /// <value>The order by expression.</value>
    string OrderByExpression { get; set; }
    /// <summary>
    /// Gets or sets the rows.
    /// </summary>
    /// <value>The rows.</value>
    int? Rows { get; set; }
    /// <summary>
    /// Gets or sets the offset.
    /// </summary>
    /// <value>The offset.</value>
    int? Offset { get; set; }
    /// <summary>
    /// Gets or sets the update fields.
    /// </summary>
    /// <value>The update fields.</value>
    List<string> UpdateFields { get; set; }
    /// <summary>
    /// Gets or sets the insert fields.
    /// </summary>
    /// <value>The insert fields.</value>
    List<string> InsertFields { get; set; }
    /// <summary>
    /// Gets the model definition.
    /// </summary>
    /// <value>The model definition.</value>
    ModelDefinition ModelDef { get; }
    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression Clone();

    /// <summary>
    /// Selects this instance.
    /// </summary>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression Select();
    /// <summary>
    /// Selects the specified select expression.
    /// </summary>
    /// <param name="selectExpression">The select expression.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression Select(string selectExpression);
    /// <summary>
    /// Unsafes the select.
    /// </summary>
    /// <param name="rawSelect">The raw select.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression UnsafeSelect(string rawSelect);

    /// <summary>
    /// Selects the specified fields.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression Select<Table1, Table2>(Expression<Func<Table1, Table2, object>> fields);
    /// <summary>
    /// Selects the specified fields.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression Select<Table1, Table2, Table3>(Expression<Func<Table1, Table2, Table3, object>> fields);
    /// <summary>
    /// Selects the distinct.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression SelectDistinct<Table1, Table2>(Expression<Func<Table1, Table2, object>> fields);
    /// <summary>
    /// Selects the distinct.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression SelectDistinct<Table1, Table2, Table3>(Expression<Func<Table1, Table2, Table3, object>> fields);
    /// <summary>
    /// Selects the distinct.
    /// </summary>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression SelectDistinct();
    /// <summary>
    /// Froms the specified tables.
    /// </summary>
    /// <param name="tables">The tables.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression From(string tables);
    /// <summary>
    /// Unsafes from.
    /// </summary>
    /// <param name="rawFrom">The raw from.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression UnsafeFrom(string rawFrom);
    /// <summary>
    /// Wheres this instance.
    /// </summary>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression Where();
    /// <summary>
    /// Unsafes the where.
    /// </summary>
    /// <param name="rawSql">The raw SQL.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression UnsafeWhere(string rawSql, params object[] filterParams);
    /// <summary>
    /// Ensures the specified SQL filter.
    /// </summary>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression Ensure(string sqlFilter, params object[] filterParams);
    /// <summary>
    /// Wheres the specified SQL filter.
    /// </summary>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression Where(string sqlFilter, params object[] filterParams);
    /// <summary>
    /// Unsafes the and.
    /// </summary>
    /// <param name="rawSql">The raw SQL.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression UnsafeAnd(string rawSql, params object[] filterParams);
    /// <summary>
    /// Ands the specified SQL filter.
    /// </summary>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression And(string sqlFilter, params object[] filterParams);
    /// <summary>
    /// Unsafes the or.
    /// </summary>
    /// <param name="rawSql">The raw SQL.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression UnsafeOr(string rawSql, params object[] filterParams);
    /// <summary>
    /// Ors the specified SQL filter.
    /// </summary>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression Or(string sqlFilter, params object[] filterParams);
    /// <summary>
    /// Adds the condition.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression AddCondition(string condition, string sqlFilter, params object[] filterParams);
    /// <summary>
    /// Groups the by.
    /// </summary>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression GroupBy();
    /// <summary>
    /// Groups the by.
    /// </summary>
    /// <param name="groupBy">The group by.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression GroupBy(string groupBy);
    /// <summary>
    /// Havings this instance.
    /// </summary>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression Having();
    /// <summary>
    /// Havings the specified SQL filter.
    /// </summary>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression Having(string sqlFilter, params object[] filterParams);
    /// <summary>
    /// Orders the by.
    /// </summary>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression OrderBy();
    /// <summary>
    /// Orders the by.
    /// </summary>
    /// <param name="orderBy">The order by.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression OrderBy(string orderBy);
    /// <summary>
    /// Gets the model definition.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>ModelDefinition.</returns>
    ModelDefinition GetModelDefinition(FieldDefinition fieldDef);
    /// <summary>
    /// Orders the by fields.
    /// </summary>
    /// <param name="fields">The fields.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression OrderByFields(params FieldDefinition[] fields);
    /// <summary>
    /// Orders the by fields descending.
    /// </summary>
    /// <param name="fields">The fields.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression OrderByFieldsDescending(params FieldDefinition[] fields);
    /// <summary>
    /// Orders the by fields.
    /// </summary>
    /// <param name="fieldNames">The field names.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression OrderByFields(params string[] fieldNames);
    /// <summary>
    /// Orders the by fields descending.
    /// </summary>
    /// <param name="fieldNames">The field names.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression OrderByFieldsDescending(params string[] fieldNames);
    /// <summary>
    /// Orders the by.
    /// </summary>
    /// <typeparam name="Table">The type of the table.</typeparam>
    /// <param name="keySelector">The key selector.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression OrderBy<Table>(Expression<Func<Table, object>> keySelector);
    /// <summary>
    /// Thens the by.
    /// </summary>
    /// <param name="orderBy">The order by.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression ThenBy(string orderBy);
    /// <summary>
    /// Thens the by.
    /// </summary>
    /// <typeparam name="Table">The type of the table.</typeparam>
    /// <param name="keySelector">The key selector.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression ThenBy<Table>(Expression<Func<Table, object>> keySelector);
    /// <summary>
    /// Orders the by descending.
    /// </summary>
    /// <typeparam name="Table">The type of the table.</typeparam>
    /// <param name="keySelector">The key selector.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression OrderByDescending<Table>(Expression<Func<Table, object>> keySelector);
    /// <summary>
    /// Orders the by descending.
    /// </summary>
    /// <param name="orderBy">The order by.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression OrderByDescending(string orderBy);
    /// <summary>
    /// Thens the by descending.
    /// </summary>
    /// <param name="orderBy">The order by.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression ThenByDescending(string orderBy);
    /// <summary>
    /// Thens the by descending.
    /// </summary>
    /// <typeparam name="Table">The type of the table.</typeparam>
    /// <param name="keySelector">The key selector.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression ThenByDescending<Table>(Expression<Func<Table, object>> keySelector);

    /// <summary>
    /// Skips the specified skip.
    /// </summary>
    /// <param name="skip">The skip.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression Skip(int? skip = null);
    /// <summary>
    /// Takes the specified take.
    /// </summary>
    /// <param name="take">The take.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression Take(int? take = null);
    /// <summary>
    /// Limits the specified skip.
    /// </summary>
    /// <param name="skip">The skip.</param>
    /// <param name="rows">The rows.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression Limit(int skip, int rows);
    /// <summary>
    /// Limits the specified skip.
    /// </summary>
    /// <param name="skip">The skip.</param>
    /// <param name="rows">The rows.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression Limit(int? skip, int? rows);
    /// <summary>
    /// Limits the specified rows.
    /// </summary>
    /// <param name="rows">The rows.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression Limit(int rows);
    /// <summary>
    /// Limits this instance.
    /// </summary>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression Limit();
    /// <summary>
    /// Clears the limits.
    /// </summary>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression ClearLimits();
    /// <summary>
    /// Updates the specified update fields.
    /// </summary>
    /// <param name="updateFields">The update fields.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression Update(List<string> updateFields);
    /// <summary>
    /// Updates this instance.
    /// </summary>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression Update();
    /// <summary>
    /// Inserts the specified insert fields.
    /// </summary>
    /// <param name="insertFields">The insert fields.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression Insert(List<string> insertFields);
    /// <summary>
    /// Inserts this instance.
    /// </summary>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression Insert();

    /// <summary>
    /// Creates the parameter.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <param name="direction">The direction.</param>
    /// <param name="dbType">Type of the database.</param>
    /// <returns>IDbDataParameter.</returns>
    IDbDataParameter CreateParam(string name, object value = null, ParameterDirection direction = ParameterDirection.Input, DbType? dbType = null);
    /// <summary>
    /// Joins the specified join expr.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression Join<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr = null);
    /// <summary>
    /// Joins the specified source type.
    /// </summary>
    /// <param name="sourceType">Type of the source.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression Join(Type sourceType, Type targetType, Expression joinExpr = null);
    /// <summary>
    /// Lefts the join.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression LeftJoin<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr = null);
    /// <summary>
    /// Lefts the join.
    /// </summary>
    /// <param name="sourceType">Type of the source.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression LeftJoin(Type sourceType, Type targetType, Expression joinExpr = null);
    /// <summary>
    /// Rights the join.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression RightJoin<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr = null);
    /// <summary>
    /// Fulls the join.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression FullJoin<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr = null);
    /// <summary>
    /// Crosses the join.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression CrossJoin<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr = null);
    /// <summary>
    /// Customs the join.
    /// </summary>
    /// <param name="joinString">The join string.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression CustomJoin(string joinString);
    /// <summary>
    /// Ensures the specified predicate.
    /// </summary>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression Ensure<Target>(Expression<Func<Target, bool>> predicate);
    /// <summary>
    /// Ensures the specified predicate.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression Ensure<Source, Target>(Expression<Func<Source, Target, bool>> predicate);
    /// <summary>
    /// Wheres the specified predicate.
    /// </summary>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression Where<Target>(Expression<Func<Target, bool>> predicate);
    /// <summary>
    /// Wheres the specified predicate.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression Where<Source, Target>(Expression<Func<Source, Target, bool>> predicate);
    /// <summary>
    /// Ands the specified predicate.
    /// </summary>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression And<Target>(Expression<Func<Target, bool>> predicate);
    /// <summary>
    /// Ands the specified predicate.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression And<Source, Target>(Expression<Func<Source, Target, bool>> predicate);
    /// <summary>
    /// Ors the specified predicate.
    /// </summary>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression Or<Target>(Expression<Func<Target, bool>> predicate);
    /// <summary>
    /// Ors the specified predicate.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    IUntypedSqlExpression Or<Source, Target>(Expression<Func<Source, Target, bool>> predicate);

    /// <summary>
    /// SQLs the table.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    string SqlTable(ModelDefinition modelDef);
    /// <summary>
    /// SQLs the column.
    /// </summary>
    /// <param name="columnName">Name of the column.</param>
    /// <returns>System.String.</returns>
    string SqlColumn(string columnName);
    /// <summary>
    /// Converts to deleterowstatement.
    /// </summary>
    /// <returns>System.String.</returns>
    string ToDeleteRowStatement();
    /// <summary>
    /// Converts to countstatement.
    /// </summary>
    /// <returns>System.String.</returns>
    string ToCountStatement();
    /// <summary>
    /// Gets all fields.
    /// </summary>
    /// <returns>IList&lt;System.String&gt;.</returns>
    IList<string> GetAllFields();
    /// <summary>
    /// Firsts the matching field.
    /// </summary>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>Tuple&lt;ModelDefinition, FieldDefinition&gt;.</returns>
    Tuple<ModelDefinition, FieldDefinition> FirstMatchingField(string fieldName);
}

/// <summary>
/// Class UntypedSqlExpressionProxy.
/// Implements the <see cref="ServiceStack.OrmLite.IUntypedSqlExpression" />
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="ServiceStack.OrmLite.IUntypedSqlExpression" />
public class UntypedSqlExpressionProxy<T> : IUntypedSqlExpression
{
    /// <summary>
    /// The q
    /// </summary>
    private SqlExpression<T> q;

    /// <summary>
    /// Initializes a new instance of the <see cref="UntypedSqlExpressionProxy{T}" /> class.
    /// </summary>
    /// <param name="q">The q.</param>
    public UntypedSqlExpressionProxy(SqlExpression<T> q)
    {
        this.q = q;
    }

    /// <summary>
    /// Gets or sets the table alias.
    /// </summary>
    /// <value>The table alias.</value>
    public string TableAlias
    {
        get => q.TableAlias;
        set => q.TableAlias = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether [prefix field with table name].
    /// </summary>
    /// <value><c>true</c> if [prefix field with table name]; otherwise, <c>false</c>.</value>
    public bool PrefixFieldWithTableName
    {
        get => q.PrefixFieldWithTableName;
        set => q.PrefixFieldWithTableName = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether [where statement without where string].
    /// </summary>
    /// <value><c>true</c> if [where statement without where string]; otherwise, <c>false</c>.</value>
    public bool WhereStatementWithoutWhereString
    {
        get => q.WhereStatementWithoutWhereString;
        set => q.WhereStatementWithoutWhereString = value;
    }

    /// <summary>
    /// Gets or sets the dialect provider.
    /// </summary>
    /// <value>The dialect provider.</value>
    public IOrmLiteDialectProvider DialectProvider
    {
        get => q.DialectProvider;
        set => q.DialectProvider = value;
    }

    /// <summary>
    /// Gets or sets the parameters.
    /// </summary>
    /// <value>The parameters.</value>
    public List<IDbDataParameter> Params
    {
        get => q.Params;
        set => q.Params = value;
    }

    /// <summary>
    /// Gets or sets the select expression.
    /// </summary>
    /// <value>The select expression.</value>
    public string SelectExpression
    {
        get => q.SelectExpression;
        set => q.SelectExpression = value;
    }

    /// <summary>
    /// Gets or sets from expression.
    /// </summary>
    /// <value>From expression.</value>
    public string FromExpression
    {
        get => q.FromExpression;
        set => q.FromExpression = value;
    }

    /// <summary>
    /// Gets the body expression.
    /// </summary>
    /// <value>The body expression.</value>
    public string BodyExpression => q.BodyExpression;

    /// <summary>
    /// Gets or sets the where expression.
    /// </summary>
    /// <value>The where expression.</value>
    public string WhereExpression
    {
        get => q.WhereExpression;
        set => q.WhereExpression = value;
    }

    /// <summary>
    /// Gets or sets the group by expression.
    /// </summary>
    /// <value>The group by expression.</value>
    public string GroupByExpression
    {
        get => q.GroupByExpression;
        set => q.GroupByExpression = value;
    }

    /// <summary>
    /// Gets or sets the having expression.
    /// </summary>
    /// <value>The having expression.</value>
    public string HavingExpression
    {
        get => q.HavingExpression;
        set => q.HavingExpression = value;
    }

    /// <summary>
    /// Gets or sets the order by expression.
    /// </summary>
    /// <value>The order by expression.</value>
    public string OrderByExpression
    {
        get => q.OrderByExpression;
        set => q.OrderByExpression = value;
    }

    /// <summary>
    /// Gets or sets the rows.
    /// </summary>
    /// <value>The rows.</value>
    public int? Rows
    {
        get => q.Rows;
        set => q.Rows = value;
    }

    /// <summary>
    /// Gets or sets the offset.
    /// </summary>
    /// <value>The offset.</value>
    public int? Offset
    {
        get => q.Offset;
        set => q.Offset = value;
    }

    /// <summary>
    /// Gets or sets the update fields.
    /// </summary>
    /// <value>The update fields.</value>
    public List<string> UpdateFields
    {
        get => q.UpdateFields;
        set => q.UpdateFields = value;
    }

    /// <summary>
    /// Gets or sets the insert fields.
    /// </summary>
    /// <value>The insert fields.</value>
    public List<string> InsertFields
    {
        get => q.InsertFields;
        set => q.InsertFields = value;
    }

    /// <summary>
    /// Gets the model definition.
    /// </summary>
    /// <value>The model definition.</value>
    public ModelDefinition ModelDef => q.ModelDef;


    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression Clone()
    {
        q.Clone();
        return this;
    }

    /// <summary>
    /// Selects this instance.
    /// </summary>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression Select()
    {
        q.Select();
        return this;
    }

    /// <summary>
    /// Selects the specified select expression.
    /// </summary>
    /// <param name="selectExpression">The select expression.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression Select(string selectExpression)
    {
        q.Select(selectExpression);
        return this;
    }

    /// <summary>
    /// Unsafes the select.
    /// </summary>
    /// <param name="rawSelect">The raw select.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression UnsafeSelect(string rawSelect)
    {
        q.UnsafeSelect(rawSelect);
        return this;
    }

    /// <summary>
    /// Selects the specified fields.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression Select<Table1, Table2>(Expression<Func<Table1, Table2, object>> fields)
    {
        q.Select(fields);
        return this;
    }

    /// <summary>
    /// Selects the specified fields.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression Select<Table1, Table2, Table3>(Expression<Func<Table1, Table2, Table3, object>> fields)
    {
        q.Select(fields);
        return this;
    }

    /// <summary>
    /// Selects the distinct.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression SelectDistinct<Table1, Table2>(Expression<Func<Table1, Table2, object>> fields)
    {
        q.SelectDistinct(fields);
        return this;
    }

    /// <summary>
    /// Selects the distinct.
    /// </summary>
    /// <typeparam name="Table1">The type of the table1.</typeparam>
    /// <typeparam name="Table2">The type of the table2.</typeparam>
    /// <typeparam name="Table3">The type of the table3.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression SelectDistinct<Table1, Table2, Table3>(Expression<Func<Table1, Table2, Table3, object>> fields)
    {
        q.SelectDistinct(fields);
        return this;
    }

    /// <summary>
    /// Selects the distinct.
    /// </summary>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression SelectDistinct()
    {
        q.SelectDistinct();
        return this;
    }

    /// <summary>
    /// Froms the specified tables.
    /// </summary>
    /// <param name="tables">The tables.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression From(string tables)
    {
        q.From(tables);
        return this;
    }

    /// <summary>
    /// Unsafes from.
    /// </summary>
    /// <param name="rawFrom">The raw from.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression UnsafeFrom(string rawFrom)
    {
        q.UnsafeFrom(rawFrom);
        return this;
    }

    /// <summary>
    /// Wheres this instance.
    /// </summary>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression Where()
    {
        q.Where();
        return this;
    }

    /// <summary>
    /// Unsafes the where.
    /// </summary>
    /// <param name="rawSql">The raw SQL.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression UnsafeWhere(string rawSql, params object[] filterParams)
    {
        q.UnsafeWhere(rawSql, filterParams);
        return this;
    }

    /// <summary>
    /// Ensures the specified SQL filter.
    /// </summary>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression Ensure(string sqlFilter, params object[] filterParams)
    {
        q.Ensure(sqlFilter, filterParams);
        return this;
    }

    /// <summary>
    /// Wheres the specified SQL filter.
    /// </summary>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression Where(string sqlFilter, params object[] filterParams)
    {
        q.Where(sqlFilter, filterParams);
        return this;
    }

    /// <summary>
    /// Unsafes the and.
    /// </summary>
    /// <param name="rawSql">The raw SQL.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression UnsafeAnd(string rawSql, params object[] filterParams)
    {
        q.UnsafeAnd(rawSql, filterParams);
        return this;
    }

    /// <summary>
    /// Ands the specified SQL filter.
    /// </summary>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression And(string sqlFilter, params object[] filterParams)
    {
        q.And(sqlFilter, filterParams);
        return this;
    }

    /// <summary>
    /// Unsafes the or.
    /// </summary>
    /// <param name="rawSql">The raw SQL.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression UnsafeOr(string rawSql, params object[] filterParams)
    {
        q.UnsafeOr(rawSql, filterParams);
        return this;
    }

    /// <summary>
    /// Ors the specified SQL filter.
    /// </summary>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression Or(string sqlFilter, params object[] filterParams)
    {
        q.Or(sqlFilter, filterParams);
        return this;
    }

    /// <summary>
    /// Adds the condition.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression AddCondition(string condition, string sqlFilter, params object[] filterParams)
    {
        q.AddCondition(condition, sqlFilter, filterParams);
        return this;
    }

    /// <summary>
    /// Groups the by.
    /// </summary>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression GroupBy()
    {
        q.GroupBy();
        return this;
    }

    /// <summary>
    /// Groups the by.
    /// </summary>
    /// <param name="groupBy">The group by.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression GroupBy(string groupBy)
    {
        q.GroupBy(groupBy);
        return this;
    }

    /// <summary>
    /// Havings this instance.
    /// </summary>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression Having()
    {
        q.Having();
        return this;
    }

    /// <summary>
    /// Havings the specified SQL filter.
    /// </summary>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression Having(string sqlFilter, params object[] filterParams)
    {
        q.Having(sqlFilter, filterParams);
        return this;
    }

    /// <summary>
    /// Orders the by.
    /// </summary>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression OrderBy()
    {
        q.OrderBy();
        return this;
    }

    /// <summary>
    /// Orders the by.
    /// </summary>
    /// <param name="orderBy">The order by.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression OrderBy(string orderBy)
    {
        q.OrderBy(orderBy);
        return this;
    }

    /// <summary>
    /// Gets the model definition.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>ModelDefinition.</returns>
    public ModelDefinition GetModelDefinition(FieldDefinition fieldDef)
    {
        return q.GetModelDefinition(fieldDef);
    }

    /// <summary>
    /// Orders the by fields.
    /// </summary>
    /// <param name="fields">The fields.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression OrderByFields(params FieldDefinition[] fields)
    {
        q.OrderByFields(fields);
        return this;
    }

    /// <summary>
    /// Orders the by fields descending.
    /// </summary>
    /// <param name="fields">The fields.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression OrderByFieldsDescending(params FieldDefinition[] fields)
    {
        q.OrderByFieldsDescending(fields);
        return this;
    }

    /// <summary>
    /// Orders the by fields.
    /// </summary>
    /// <param name="fieldNames">The field names.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression OrderByFields(params string[] fieldNames)
    {
        q.OrderByFields(fieldNames);
        return this;
    }

    /// <summary>
    /// Orders the by fields descending.
    /// </summary>
    /// <param name="fieldNames">The field names.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression OrderByFieldsDescending(params string[] fieldNames)
    {
        q.OrderByFieldsDescending(fieldNames);
        return this;
    }

    /// <summary>
    /// Orders the by.
    /// </summary>
    /// <typeparam name="Table">The type of the table.</typeparam>
    /// <param name="keySelector">The key selector.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression OrderBy<Table>(Expression<Func<Table, object>> keySelector)
    {
        q.OrderBy(keySelector);
        return this;
    }

    /// <summary>
    /// Thens the by.
    /// </summary>
    /// <param name="orderBy">The order by.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression ThenBy(string orderBy)
    {
        q.ThenBy(orderBy);
        return this;
    }

    /// <summary>
    /// Thens the by.
    /// </summary>
    /// <typeparam name="Table">The type of the table.</typeparam>
    /// <param name="keySelector">The key selector.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression ThenBy<Table>(Expression<Func<Table, object>> keySelector)
    {
        q.ThenBy(keySelector);
        return this;
    }

    /// <summary>
    /// Orders the by descending.
    /// </summary>
    /// <typeparam name="Table">The type of the table.</typeparam>
    /// <param name="keySelector">The key selector.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression OrderByDescending<Table>(Expression<Func<Table, object>> keySelector)
    {
        q.OrderByDescending(keySelector);
        return this;
    }

    /// <summary>
    /// Orders the by descending.
    /// </summary>
    /// <param name="orderBy">The order by.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression OrderByDescending(string orderBy)
    {
        q.OrderByDescending(orderBy);
        return this;
    }

    /// <summary>
    /// Thens the by descending.
    /// </summary>
    /// <param name="orderBy">The order by.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression ThenByDescending(string orderBy)
    {
        q.ThenByDescending(orderBy);
        return this;
    }

    /// <summary>
    /// Thens the by descending.
    /// </summary>
    /// <typeparam name="Table">The type of the table.</typeparam>
    /// <param name="keySelector">The key selector.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression ThenByDescending<Table>(Expression<Func<Table, object>> keySelector)
    {
        q.ThenByDescending(keySelector);
        return this;
    }

    /// <summary>
    /// Skips the specified skip.
    /// </summary>
    /// <param name="skip">The skip.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression Skip(int? skip = null)
    {
        q.Skip(skip);
        return this;
    }

    /// <summary>
    /// Takes the specified take.
    /// </summary>
    /// <param name="take">The take.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression Take(int? take = null)
    {
        q.Take(take);
        return this;
    }

    /// <summary>
    /// Limits the specified skip.
    /// </summary>
    /// <param name="skip">The skip.</param>
    /// <param name="rows">The rows.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression Limit(int skip, int rows)
    {
        q.Limit(skip, rows);
        return this;
    }

    /// <summary>
    /// Limits the specified skip.
    /// </summary>
    /// <param name="skip">The skip.</param>
    /// <param name="rows">The rows.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression Limit(int? skip, int? rows)
    {
        q.Limit(skip, rows);
        return this;
    }

    /// <summary>
    /// Limits the specified rows.
    /// </summary>
    /// <param name="rows">The rows.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression Limit(int rows)
    {
        q.Limit(rows);
        return this;
    }

    /// <summary>
    /// Limits this instance.
    /// </summary>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression Limit()
    {
        q.Limit();
        return this;
    }

    /// <summary>
    /// Clears the limits.
    /// </summary>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression ClearLimits()
    {
        q.ClearLimits();
        return this;
    }

    /// <summary>
    /// Updates the specified update fields.
    /// </summary>
    /// <param name="updateFields">The update fields.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression Update(List<string> updateFields)
    {
        q.Update(updateFields);
        return this;
    }

    /// <summary>
    /// Updates this instance.
    /// </summary>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression Update()
    {
        q.Update();
        return this;
    }

    /// <summary>
    /// Inserts the specified insert fields.
    /// </summary>
    /// <param name="insertFields">The insert fields.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression Insert(List<string> insertFields)
    {
        q.Insert(insertFields);
        return this;
    }

    /// <summary>
    /// Inserts this instance.
    /// </summary>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression Insert()
    {
        q.Insert();
        return this;
    }

    /// <summary>
    /// Creates the parameter.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <param name="direction">The direction.</param>
    /// <param name="dbType">Type of the database.</param>
    /// <returns>IDbDataParameter.</returns>
    public IDbDataParameter CreateParam(string name, object value = null, ParameterDirection direction = ParameterDirection.Input, DbType? dbType = null)
    {
        return q.CreateParam(name, value, direction, dbType);
    }

    /// <summary>
    /// Joins the specified join expr.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression Join<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr = null)
    {
        q.Join(joinExpr);
        return this;
    }

    /// <summary>
    /// Joins the specified source type.
    /// </summary>
    /// <param name="sourceType">Type of the source.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression Join(Type sourceType, Type targetType, Expression joinExpr = null)
    {
        q.Join(sourceType, targetType, joinExpr);
        return this;
    }

    /// <summary>
    /// Lefts the join.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression LeftJoin<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr = null)
    {
        q.LeftJoin(joinExpr);
        return this;
    }

    /// <summary>
    /// Lefts the join.
    /// </summary>
    /// <param name="sourceType">Type of the source.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression LeftJoin(Type sourceType, Type targetType, Expression joinExpr = null)
    {
        q.LeftJoin(sourceType, targetType, joinExpr);
        return this;
    }

    /// <summary>
    /// Rights the join.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression RightJoin<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr = null)
    {
        q.RightJoin(joinExpr);
        return this;
    }

    /// <summary>
    /// Fulls the join.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression FullJoin<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr = null)
    {
        q.FullJoin(joinExpr);
        return this;
    }

    /// <summary>
    /// Crosses the join.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="joinExpr">The join expr.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression CrossJoin<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr = null)
    {
        q.CrossJoin(joinExpr);
        return this;
    }

    /// <summary>
    /// Customs the join.
    /// </summary>
    /// <param name="joinString">The join string.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression CustomJoin(string joinString)
    {
        q.CustomJoin(joinString);
        return this;
    }

    /// <summary>
    /// Wheres the specified predicate.
    /// </summary>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression Where<Target>(Expression<Func<Target, bool>> predicate)
    {
        q.Where(predicate);
        return this;
    }

    /// <summary>
    /// Ensures the specified predicate.
    /// </summary>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression Ensure<Target>(Expression<Func<Target, bool>> predicate)
    {
        q.Ensure(predicate);
        return this;
    }

    /// <summary>
    /// Wheres the specified predicate.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression Where<Source, Target>(Expression<Func<Source, Target, bool>> predicate)
    {
        q.Where(predicate);
        return this;
    }

    /// <summary>
    /// Ensures the specified predicate.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression Ensure<Source, Target>(Expression<Func<Source, Target, bool>> predicate)
    {
        q.Ensure(predicate);
        return this;
    }

    /// <summary>
    /// Ands the specified predicate.
    /// </summary>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression And<Target>(Expression<Func<Target, bool>> predicate)
    {
        q.And(predicate);
        return this;
    }

    /// <summary>
    /// Ands the specified predicate.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression And<Source, Target>(Expression<Func<Source, Target, bool>> predicate)
    {
        q.And(predicate);
        return this;
    }

    /// <summary>
    /// Ors the specified predicate.
    /// </summary>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression Or<Target>(Expression<Func<Target, bool>> predicate)
    {
        q.Or(predicate);
        return this;
    }

    /// <summary>
    /// Ors the specified predicate.
    /// </summary>
    /// <typeparam name="Source">The type of the source.</typeparam>
    /// <typeparam name="Target">The type of the target.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public IUntypedSqlExpression Or<Source, Target>(Expression<Func<Source, Target, bool>> predicate)
    {
        q.Or(predicate);
        return this;
    }

    /// <summary>
    /// SQLs the table.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    public string SqlTable(ModelDefinition modelDef)
    {
        return q.SqlTable(modelDef);
    }

    /// <summary>
    /// SQLs the column.
    /// </summary>
    /// <param name="columnName">Name of the column.</param>
    /// <returns>System.String.</returns>
    public string SqlColumn(string columnName)
    {
        return q.SqlColumn(columnName);
    }

    /// <summary>
    /// Converts to deleterowstatement.
    /// </summary>
    /// <returns>System.String.</returns>
    public string ToDeleteRowStatement()
    {
        return q.ToDeleteRowStatement();
    }

    /// <summary>
    /// Converts to selectstatement.
    /// </summary>
    /// <returns>System.String.</returns>
    public string ToSelectStatement() => ToSelectStatement(QueryType.Select);
    /// <summary>
    /// Converts to selectstatement.
    /// </summary>
    /// <param name="forType">For type.</param>
    /// <returns>System.String.</returns>
    public string ToSelectStatement(QueryType forType)
    {
        return q.ToSelectStatement(forType);
    }

    /// <summary>
    /// Converts to countstatement.
    /// </summary>
    /// <returns>System.String.</returns>
    public string ToCountStatement()
    {
        return q.ToCountStatement();
    }

    /// <summary>
    /// Gets all fields.
    /// </summary>
    /// <returns>IList&lt;System.String&gt;.</returns>
    public IList<string> GetAllFields()
    {
        return q.GetAllFields();
    }

    /// <summary>
    /// Firsts the matching field.
    /// </summary>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>Tuple&lt;ModelDefinition, FieldDefinition&gt;.</returns>
    public Tuple<ModelDefinition, FieldDefinition> FirstMatchingField(string fieldName)
    {
        return q.FirstMatchingField(fieldName);
    }

    /// <summary>
    /// Selects the into.
    /// </summary>
    /// <typeparam name="TModel">The type of the t model.</typeparam>
    /// <returns>System.String.</returns>
    public string SelectInto<TModel>() => q.SelectInto<TModel>();
    /// <summary>
    /// Selects the into.
    /// </summary>
    /// <typeparam name="TModel">The type of the t model.</typeparam>
    /// <param name="queryType">Type of the query.</param>
    /// <returns>System.String.</returns>
    public string SelectInto<TModel>(QueryType queryType) => q.SelectInto<TModel>(queryType);
}

/// <summary>
/// Class SqlExpressionExtensions.
/// </summary>
public static class SqlExpressionExtensions
{
    /// <summary>
    /// Gets the untyped SQL expression.
    /// </summary>
    /// <param name="sqlExpression">The SQL expression.</param>
    /// <returns>IUntypedSqlExpression.</returns>
    public static IUntypedSqlExpression GetUntypedSqlExpression(this ISqlExpression sqlExpression)
    {
        var hasUntyped = sqlExpression as IHasUntypedSqlExpression;
        return hasUntyped?.GetUntyped();
    }

    /// <summary>
    /// Converts to dialectprovider.
    /// </summary>
    /// <param name="sqlExpression">The SQL expression.</param>
    /// <returns>IOrmLiteDialectProvider.</returns>
    public static IOrmLiteDialectProvider ToDialectProvider(this ISqlExpression sqlExpression) =>
        (sqlExpression as IHasDialectProvider)?.DialectProvider ?? OrmLiteConfig.DialectProvider;

    /// <summary>
    /// Tables the specified SQL expression.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sqlExpression">The SQL expression.</param>
    /// <returns>System.String.</returns>
    public static string Table<T>(this ISqlExpression sqlExpression) => sqlExpression.ToDialectProvider().GetQuotedTableName(typeof(T).GetModelDefinition());

    /// <summary>
    /// Tables the specified dialect.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dialect">The dialect.</param>
    /// <returns>System.String.</returns>
    public static string Table<T>(this IOrmLiteDialectProvider dialect) => dialect.GetQuotedTableName(typeof(T).GetModelDefinition());

    /// <summary>
    /// Columns the specified property expression.
    /// </summary>
    /// <typeparam name="Table">The type of the table.</typeparam>
    /// <param name="sqlExpression">The SQL expression.</param>
    /// <param name="propertyExpression">The property expression.</param>
    /// <param name="prefixTable">if set to <c>true</c> [prefix table].</param>
    /// <returns>System.String.</returns>
    public static string Column<Table>(this ISqlExpression sqlExpression, Expression<Func<Table, object>> propertyExpression, bool prefixTable = false) =>
        sqlExpression.ToDialectProvider().Column(propertyExpression, prefixTable);

    /// <summary>
    /// Columns the type of the database.
    /// </summary>
    /// <typeparam name="Table">The type of the table.</typeparam>
    /// <param name="dialect">The dialect.</param>
    /// <param name="propertyExpression">The property expression.</param>
    /// <param name="prefixTable">if set to <c>true</c> [prefix table].</param>
    /// <returns>Type.</returns>
    /// <exception cref="System.ArgumentException">Expected Lambda MemberExpression but received: " + propertyExpression.Name</exception>
    public static Type ColumnDbType<Table>(this IOrmLiteDialectProvider dialect, Expression<Func<Table, object>> propertyExpression, bool prefixTable = false)
    {
        string propertyName = null;
        Expression expr = propertyExpression;

        if (expr is LambdaExpression lambda)
            expr = lambda.Body;

        if (expr.NodeType == ExpressionType.Convert && expr is UnaryExpression unary)
            expr = unary.Operand;

        if (expr is MemberExpression member)
            propertyName = member.Member.Name;

        if (propertyName == null)
            propertyName = expr.ToPropertyInfo()?.Name;

        if (propertyName != null)
            return dialect.ColumnDbType<Table>(propertyName, prefixTable);

        throw new ArgumentException("Expected Lambda MemberExpression but received: " + propertyExpression.Name);
    }

    /// <summary>
    /// Columns the specified property expression.
    /// </summary>
    /// <typeparam name="Table">The type of the table.</typeparam>
    /// <param name="dialect">The dialect.</param>
    /// <param name="propertyExpression">The property expression.</param>
    /// <param name="prefixTable">if set to <c>true</c> [prefix table].</param>
    /// <returns>System.String.</returns>
    /// <exception cref="System.ArgumentException">Expected Lambda MemberExpression but received: " + propertyExpression.Name</exception>
    public static string Column<Table>(this IOrmLiteDialectProvider dialect, Expression<Func<Table, object>> propertyExpression, bool prefixTable = false)
    {
        string propertyName = null;
        Expression expr = propertyExpression;

        if (expr is LambdaExpression lambda)
            expr = lambda.Body;

        if (expr.NodeType == ExpressionType.Convert && expr is UnaryExpression unary)
            expr = unary.Operand;

        if (expr is MemberExpression member)
            propertyName = member.Member.Name;

        if (propertyName == null)
            propertyName = expr.ToPropertyInfo()?.Name;

        if (propertyName != null)
            return dialect.Column<Table>(propertyName, prefixTable);

        throw new ArgumentException("Expected Lambda MemberExpression but received: " + propertyExpression.Name);
    }

    /// <summary>
    /// Columns the specified property name.
    /// </summary>
    /// <typeparam name="Table">The type of the table.</typeparam>
    /// <param name="sqlExpression">The SQL expression.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="prefixTable">if set to <c>true</c> [prefix table].</param>
    /// <returns>System.String.</returns>
    public static string Column<Table>(this ISqlExpression sqlExpression, string propertyName, bool prefixTable = false) =>
        sqlExpression.ToDialectProvider().Column<Table>(propertyName, prefixTable);

    /// <summary>
    /// Columns the type of the database.
    /// </summary>
    /// <typeparam name="Table">The type of the table.</typeparam>
    /// <param name="dialect">The dialect.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="prefixTable">if set to <c>true</c> [prefix table].</param>
    /// <returns>Type.</returns>
    public static Type ColumnDbType<Table>(this IOrmLiteDialectProvider dialect, string propertyName, bool prefixTable = false)
    {
        var tableDef = typeof(Table).GetModelDefinition();

        var fieldDef = tableDef.FieldDefinitions.FirstOrDefault(x => x.Name == propertyName);

        return fieldDef.ColumnType;
    }

    /// <summary>
    /// Columns the specified property name.
    /// </summary>
    /// <typeparam name="Table">The type of the table.</typeparam>
    /// <param name="dialect">The dialect.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="prefixTable">if set to <c>true</c> [prefix table].</param>
    /// <returns>System.String.</returns>
    public static string Column<Table>(this IOrmLiteDialectProvider dialect, string propertyName, bool prefixTable = false)
    {
        var tableDef = typeof(Table).GetModelDefinition();

        var fieldDef = tableDef.FieldDefinitions.FirstOrDefault(x => x.Name == propertyName);
        var fieldName = fieldDef != null
                            ? fieldDef.FieldName
                            : propertyName;

        return prefixTable
                   ? dialect.GetQuotedColumnName(tableDef, fieldName)
                   : dialect.GetQuotedColumnName(fieldName);
    }
}