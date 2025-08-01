// ***********************************************************************
// <copyright file="LoadList.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite.Support;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Class LoadList.
/// </summary>
/// <typeparam name="Into">The type of the into.</typeparam>
/// <typeparam name="From">The type of from.</typeparam>
internal abstract class LoadList<Into, From>
{
    /// <summary>
    /// The database command
    /// </summary>
    protected IDbCommand dbCmd;

    /// <summary>
    /// The q
    /// </summary>
    protected SqlExpression<From> q;

    /// <summary>
    /// The dialect provider
    /// </summary>
    protected IOrmLiteDialectProvider dialectProvider;

    /// <summary>
    /// The parent results
    /// </summary>
    protected List<Into> parentResults;

    /// <summary>
    /// The model definition
    /// </summary>
    protected ModelDefinition modelDef;

    /// <summary>
    /// The field defs
    /// </summary>
    protected FieldDefinition[] fieldDefs;

    /// <summary>
    /// The sub SQL
    /// </summary>
    protected string subSql;

    /// <summary>
    /// Gets the field defs.
    /// </summary>
    /// <value>The field defs.</value>
    public FieldDefinition[] FieldDefs => this.fieldDefs;

    /// <summary>
    /// Gets the parent results.
    /// </summary>
    /// <value>The parent results.</value>
    public List<Into> ParentResults => this.parentResults;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoadList{Into, From}" /> class.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="q">The q.</param>
    protected LoadList(IDbCommand dbCmd, SqlExpression<From> q)
    {
        this.dialectProvider = dbCmd.GetDialectProvider();

        q ??= this.dialectProvider.SqlExpression<From>();

        this.dbCmd = dbCmd;
        this.q = q;

        //Use .Clone() to prevent SqlExpressionSelectFilter from adding params to original query
        var parentQ = q.Clone();
        var sql = parentQ.SelectInto<Into>(QueryType.Select);
        this.parentResults = dbCmd.ExprConvertToList<Into>(sql, parentQ.Params, onlyFields: q.OnlyFields);

        this.modelDef = ModelDefinition<Into>.Definition;
        this.fieldDefs = this.modelDef.ReferenceFieldDefinitionsArray;

        var subQ = q.Clone();
        var subQSql = this.dialectProvider.GetLoadChildrenSubSelect(subQ);
        this.subSql = this.dialectProvider.MergeParamsIntoSql(subQSql, subQ.Params);
    }

    /// <summary>
    /// Gets the reference list SQL.
    /// </summary>
    /// <param name="refModelDef">The reference model definition.</param>
    /// <param name="refField">The reference field.</param>
    /// <returns>System.String.</returns>
    protected string GetRefListSql(ModelDefinition refModelDef, FieldDefinition refField)
    {
        var sqlRef = $"SELECT {this.dialectProvider.GetColumnNames(refModelDef)} " +
                     $"FROM {this.dialectProvider.GetQuotedTableName(refModelDef)} " +
                     $"WHERE {this.dialectProvider.GetQuotedColumnName(refField)} " +
                     $"IN ({this.subSql})";

        if (OrmLiteConfig.LoadReferenceSelectFilter != null)
        {
            sqlRef = OrmLiteConfig.LoadReferenceSelectFilter(refModelDef.ModelType, sqlRef);
        }

        return sqlRef;
    }

    /// <summary>
    /// Sets the list child results.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="refType">Type of the reference.</param>
    /// <param name="childResults">The child results.</param>
    /// <param name="refField">The reference field.</param>
    protected void SetListChildResults(FieldDefinition fieldDef, Type refType, IList childResults, FieldDefinition refField)
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

        var untypedApi = this.dbCmd.CreateTypedApi(refType);
        foreach (var result in this.parentResults)
        {
            var pkValue = this.modelDef.PrimaryKey.GetValue(result);
            if (map.TryGetValue(pkValue, out refValues))
            {
                var castResults = untypedApi.Cast(refValues);
                fieldDef.SetValue(result, castResults);
            }
        }
    }

    /// <summary>
    /// Gets the reference self SQL.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="refSelf">The reference self.</param>
    /// <param name="refModelDef">The reference model definition.</param>
    /// <returns>System.String.</returns>
    protected string GetRefSelfSql(ModelDefinition modelDef, FieldDefinition refSelf, ModelDefinition refModelDef, FieldDefinition refId)
    {
        return this.dialectProvider.GetRefSelfSql(this.q.Clone(), modelDef, refSelf, refModelDef, refId);
    }

    /// <summary>
    /// Gets the reference field SQL.
    /// </summary>
    /// <param name="refModelDef">The reference model definition.</param>
    /// <param name="refField">The reference field.</param>
    /// <returns>System.String.</returns>
    protected string GetRefFieldSql(ModelDefinition refModelDef, FieldDefinition refField)
    {
        return this.dialectProvider.GetRefFieldSql(this.subSql, refModelDef, refField);
    }

    /// <summary>
    /// Gets the field reference SQL.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="fieldRef">The field reference.</param>
    /// <returns>System.String.</returns>
    protected string GetFieldReferenceSql(FieldDefinition fieldDef, FieldReference fieldRef)
    {
        return this.dialectProvider.GetFieldReferenceSql(this.subSql, fieldDef, fieldRef);
    }

    /// <summary>
    /// Creates the reference map.
    /// </summary>
    /// <returns>Dictionary&lt;System.Object, System.Object&gt;.</returns>
    protected Dictionary<object, object> CreateRefMap()
    {
        return OrmLiteConfig.IsCaseInsensitive
                   ? new Dictionary<object, object>(CaseInsensitiveObjectComparer.Instance)
                   : [];
    }

    /// <summary>
    /// Class CaseInsensitiveObjectComparer.
    /// </summary>
    public class CaseInsensitiveObjectComparer : IEqualityComparer<object>
    {
        /// <summary>
        /// The instance
        /// </summary>
        public static CaseInsensitiveObjectComparer Instance = new();

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type T to compare.</param>
        /// <param name="y">The second object of type T to compare.</param>
        /// <returns><see langword="true" /> if the specified objects are equal; otherwise, <see langword="false" />.</returns>
        public new bool Equals(object x, object y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x is string xStr && y is string yStr
                       ? xStr.Equals(yStr, StringComparison.OrdinalIgnoreCase)
                       : x.Equals(y);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public int GetHashCode(object obj)
        {
            var str = obj as string;
            return str?.ToUpper().GetHashCode() ?? obj.GetHashCode();
        }
    }

    /// <summary>
    /// Sets the reference self child results.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="refModelDef">The reference model definition.</param>
    /// <param name="refSelf">The reference self.</param>
    /// <param name="childResults">The child results.</param>
    protected void SetRefSelfChildResults(FieldDefinition fieldDef, ModelDefinition refModelDef, FieldDefinition refSelf, IList childResults, FieldDefinition refId)
    {
        var map = this.CreateRefMap();

        foreach (var result in childResults)
        {
            var pkValue = refId.GetValue(result);
            map[pkValue] = result;
        }

        foreach (var result in this.parentResults)
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
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="refField">The reference field.</param>
    /// <param name="childResults">The child results.</param>
    protected void SetRefFieldChildResults(FieldDefinition fieldDef, FieldDefinition refField, IList childResults)
    {
        var map = this.CreateRefMap();

        foreach (var result in childResults)
        {
            var refValue = refField.GetValue(result);
            map[refValue] = result;
        }

        foreach (var result in this.parentResults)
        {
            var pkValue = this.modelDef.PrimaryKey.GetValue(result);
            if (map.TryGetValue(pkValue, out var childResult))
            {
                fieldDef.SetValue(result, childResult);
            }
        }
    }

    /// <summary>
    /// Sets the field reference child results.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="fieldRef">The field reference.</param>
    /// <param name="childResults">The child results.</param>
    protected void SetFieldReferenceChildResults(FieldDefinition fieldDef, FieldReference fieldRef, IList childResults)
    {
        var map = this.CreateRefMap();

        var refField = fieldRef.RefModelDef.PrimaryKey;
        foreach (var result in childResults)
        {
            var refValue = refField.GetValue(result);
            var refFieldValue = fieldRef.RefFieldDef.GetValue(result);
            map[refValue] = refFieldValue;
        }

        foreach (var result in this.parentResults)
        {
            var fkValue = fieldRef.RefIdFieldDef.GetValue(result);
            if (map.TryGetValue(fkValue, out var childResult))
            {
                fieldDef.SetValue(result, childResult);
            }
        }
    }
}

/// <summary>
/// Class LoadListSync.
/// Implements the <see cref="ServiceStack.OrmLite.Support.LoadList{Into, From}" />
/// </summary>
/// <typeparam name="Into">The type of the into.</typeparam>
/// <typeparam name="From">The type of from.</typeparam>
/// <seealso cref="ServiceStack.OrmLite.Support.LoadList{Into, From}" />
internal class LoadListSync<Into, From> : LoadList<Into, From>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoadListSync{Into, From}" /> class.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="q">The q.</param>
    public LoadListSync(IDbCommand dbCmd, SqlExpression<From> q) : base(dbCmd, q) { }

    /// <summary>
    /// Sets the reference field list.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="refType">Type of the reference.</param>
    public void SetRefFieldList(FieldDefinition fieldDef, Type refType)
    {
        var refModelDef = refType.GetModelDefinition();
        var refField = this.modelDef.GetRefFieldDef(refModelDef, refType);

        var sqlRef = this.GetRefListSql(refModelDef, refField);

        var childResults = this.dbCmd.ConvertToList(refType, sqlRef);

        this.SetListChildResults(fieldDef, refType, childResults, refField);
    }

    /// <summary>
    /// Sets the reference field.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="refType">Type of the reference.</param>
    public void SetRefField(FieldDefinition fieldDef, Type refType)
    {
        var refModelDef = refType.GetModelDefinition();

        var refSelf = this.modelDef.GetSelfRefFieldDefIfExists(refModelDef, fieldDef);
        var refField = refSelf == null
                           ? this.modelDef.GetRefFieldDef(refModelDef, refType)
                           : this.modelDef.GetRefFieldDefIfExists(refModelDef);

        if (refSelf != null)
        {
            var refId = fieldDef.ReferenceRefId != null
                ? refModelDef.GetFieldDefinition(fieldDef.ReferenceRefId)
                  ?? throw new NotSupportedException($"{fieldDef.ReferenceRefId} is not a property of {refModelDef.Name}")
                : refModelDef.PrimaryKey;
            var sqlRef = this.GetRefSelfSql(this.modelDef, refSelf, refModelDef, refId);
            var childResults = this.dbCmd.ConvertToList(refType, sqlRef);
            this.SetRefSelfChildResults(fieldDef, refModelDef, refSelf, childResults, refId);
        }
        else if (refField != null)
        {
            var sqlRef = this.GetRefFieldSql(refModelDef, refField);
            var childResults = this.dbCmd.ConvertToList(refType, sqlRef);
            this.SetRefFieldChildResults(fieldDef, refField, childResults);
        }
    }

    /// <summary>
    /// Sets the field reference.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="fieldRef">The field reference.</param>
    public void SetFieldReference(FieldDefinition fieldDef, FieldReference fieldRef)
    {
        var sqlRef = this.GetFieldReferenceSql(fieldDef, fieldRef);
        var childResults = this.dbCmd.ConvertToList(fieldRef.RefModel, sqlRef);
        this.SetFieldReferenceChildResults(fieldDef, fieldRef, childResults);
    }
}

/// <summary>
/// Class LoadListAsync.
/// Implements the <see cref="ServiceStack.OrmLite.Support.LoadList{Into, From}" />
/// </summary>
/// <typeparam name="Into">The type of the into.</typeparam>
/// <typeparam name="From">The type of from.</typeparam>
/// <seealso cref="ServiceStack.OrmLite.Support.LoadList{Into, From}" />
internal class LoadListAsync<Into, From> : LoadList<Into, From>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoadListAsync{Into, From}" /> class.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="expr">The expr.</param>
    public LoadListAsync(IDbCommand dbCmd, SqlExpression<From> expr) : base(dbCmd, expr) { }

    /// <summary>
    /// Set reference field list as an asynchronous operation.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="refType">Type of the reference.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task SetRefFieldListAsync(FieldDefinition fieldDef, Type refType, CancellationToken token)
    {
        var refModelDef = refType.GetModelDefinition();
        var refField = this.modelDef.GetRefFieldDef(refModelDef, refType);

        var sqlRef = this.GetRefListSql(refModelDef, refField);

        var childResults = await this.dbCmd.ConvertToListAsync(refType, sqlRef, token).ConfigAwait();

        this.SetListChildResults(fieldDef, refType, childResults, refField);
    }

    /// <summary>
    /// Set reference field as an asynchronous operation.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="refType">Type of the reference.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task SetRefFieldAsync(FieldDefinition fieldDef, Type refType, CancellationToken token)
    {
        var refModelDef = refType.GetModelDefinition();

        var refSelf = this.modelDef.GetSelfRefFieldDefIfExists(refModelDef, fieldDef);
        var refField = refSelf == null
                           ? this.modelDef.GetRefFieldDef(refModelDef, refType)
                           : this.modelDef.GetRefFieldDefIfExists(refModelDef);

        if (refSelf != null)
        {
            var refId = fieldDef.ReferenceRefId != null
                ? refModelDef.GetFieldDefinition(fieldDef.ReferenceRefId)
                  ?? throw new NotSupportedException($"{fieldDef.ReferenceRefId} is not a property of {refModelDef.Name}")
                : refModelDef.PrimaryKey;
            var sqlRef = this.GetRefSelfSql(this.modelDef, refSelf, refModelDef, refId);
            var childResults = await this.dbCmd.ConvertToListAsync(refType, sqlRef, token).ConfigAwait();
            this.SetRefSelfChildResults(fieldDef, refModelDef, refSelf, childResults, refId);
        }
        else if (refField != null)
        {
            var sqlRef = this.GetRefFieldSql(refModelDef, refField);
            var childResults = await this.dbCmd.ConvertToListAsync(refType, sqlRef, token).ConfigAwait();
            this.SetRefFieldChildResults(fieldDef, refField, childResults);
        }
    }

    /// <summary>
    /// Set field reference as an asynchronous operation.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="fieldRef">The field reference.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task SetFieldReferenceAsync(FieldDefinition fieldDef, FieldReference fieldRef, CancellationToken token)
    {
        var sqlRef = this.GetFieldReferenceSql(fieldDef, fieldRef);
        var childResults = await this.dbCmd.ConvertToListAsync(fieldRef.RefModel, sqlRef, token).ConfigAwait();
        this.SetFieldReferenceChildResults(fieldDef, fieldRef, childResults);
    }
}