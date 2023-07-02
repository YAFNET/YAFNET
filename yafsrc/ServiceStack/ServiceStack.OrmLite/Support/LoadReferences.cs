// ***********************************************************************
// <copyright file="LoadReferences.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.Support;

using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

using ServiceStack.Text;

/// <summary>
/// Class LoadReferences.
/// </summary>
/// <typeparam name="T"></typeparam>
internal abstract class LoadReferences<T>
{
    /// <summary>
    /// The database command
    /// </summary>
    protected IDbCommand dbCmd;

    /// <summary>
    /// The instance
    /// </summary>
    protected T instance;

    /// <summary>
    /// The model definition
    /// </summary>
    protected ModelDefinition modelDef;

    /// <summary>
    /// The field defs
    /// </summary>
    protected FieldDefinition[] fieldDefs;

    /// <summary>
    /// The pk value
    /// </summary>
    protected object pkValue;

    /// <summary>
    /// The dialect provider
    /// </summary>
    protected IOrmLiteDialectProvider dialectProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoadReferences{T}" /> class.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="instance">The instance.</param>
    protected LoadReferences(IDbCommand dbCmd, T instance)
    {
        this.dbCmd = dbCmd;
        this.instance = instance;

        modelDef = ModelDefinition<T>.Definition;
        fieldDefs = modelDef.ReferenceFieldDefinitionsArray;
        pkValue = modelDef.PrimaryKey.GetValue(instance);
        dialectProvider = dbCmd.GetDialectProvider();
    }

    /// <summary>
    /// Gets the field defs.
    /// </summary>
    /// <value>The field defs.</value>
    public FieldDefinition[] FieldDefs => fieldDefs;

    /// <summary>
    /// Gets the reference list SQL.
    /// </summary>
    /// <param name="refType">Type of the reference.</param>
    /// <returns>System.String.</returns>
    protected string GetRefListSql(Type refType)
    {
        var refModelDef = refType.GetModelDefinition();

        var refField = modelDef.GetRefFieldDef(refModelDef, refType);

        var sqlFilter = dialectProvider.GetQuotedColumnName(refField.FieldName) + "={0}";
        var sql = dialectProvider.ToSelectStatement(refType, sqlFilter, pkValue);

        if (OrmLiteConfig.LoadReferenceSelectFilter != null)
            sql = OrmLiteConfig.LoadReferenceSelectFilter(refType, sql);

        return sql;
    }

    /// <summary>
    /// Gets the reference field SQL.
    /// </summary>
    /// <param name="refType">Type of the reference.</param>
    /// <param name="refField">The reference field.</param>
    /// <returns>System.String.</returns>
    protected string GetRefFieldSql(Type refType, FieldDefinition refField)
    {
        var sqlFilter = dialectProvider.GetQuotedColumnName(refField.FieldName) + "={0}";
        var sql = dialectProvider.ToSelectStatement(refType, sqlFilter, pkValue);

        if (OrmLiteConfig.LoadReferenceSelectFilter != null)
            sql = OrmLiteConfig.LoadReferenceSelectFilter(refType, sql);

        return sql;
    }

    /// <summary>
    /// Gets the reference self SQL.
    /// </summary>
    /// <param name="refType">Type of the reference.</param>
    /// <param name="refSelf">The reference self.</param>
    /// <param name="refModelDef">The reference model definition.</param>
    /// <returns>System.String.</returns>
    protected string GetRefSelfSql(Type refType, FieldDefinition refSelf, ModelDefinition refModelDef)
    {
        //Load Self Table.RefTableId PK
        var refPkValue = refSelf.GetValue(instance);
        if (refPkValue == null)
            return null;

        var sqlFilter = dialectProvider.GetQuotedColumnName(refModelDef.PrimaryKey.FieldName) + "={0}";
        var sql = dialectProvider.ToSelectStatement(refType, sqlFilter, refPkValue);

        if (OrmLiteConfig.LoadReferenceSelectFilter != null)
            sql = OrmLiteConfig.LoadReferenceSelectFilter(refType, sql);

        return sql;
    }
    /// <summary>
    /// Gets the field reference SQL.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="fieldRef">The field reference.</param>
    /// <returns>System.String.</returns>
    protected string GetFieldReferenceSql(FieldDefinition fieldDef, FieldReference fieldRef)
    {
        var refPkValue = fieldRef.RefIdFieldDef.GetValue(instance);
        if (refPkValue == null)
            return null;

        var refModelDef = fieldRef.RefModelDef;

        var pk = dialectProvider.GetQuotedColumnName(refModelDef.PrimaryKey);
        var sqlRef = dialectProvider.ToSelectStatement(fieldRef.RefModel,
            $"SELECT {pk}, {dialectProvider.GetQuotedColumnName(fieldRef.RefFieldDef)} " +
            $"FROM {dialectProvider.GetQuotedTableName(refModelDef)} " +
            $"WHERE {pk}" + "={0}", refPkValue);

        if (OrmLiteConfig.LoadReferenceSelectFilter != null)
            sqlRef = OrmLiteConfig.LoadReferenceSelectFilter(fieldRef.RefModel, sqlRef);

        return sqlRef;
    }
}



/// <summary>
/// Class LoadReferencesSync.
/// Implements the <see cref="ServiceStack.OrmLite.Support.LoadReferences{T}" />
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="ServiceStack.OrmLite.Support.LoadReferences{T}" />
internal class LoadReferencesSync<T> : LoadReferences<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoadReferencesSync{T}" /> class.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="instance">The instance.</param>
    public LoadReferencesSync(IDbCommand dbCmd, T instance)
        : base(dbCmd, instance) { }

    /// <summary>
    /// Sets the reference field list.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="refType">Type of the reference.</param>
    public void SetRefFieldList(FieldDefinition fieldDef, Type refType)
    {
        var sql = GetRefListSql(refType);

        var results = dbCmd.ConvertToList(refType, sql);
        fieldDef.SetValue(instance, results);
    }

    /// <summary>
    /// Sets the reference field.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="refType">Type of the reference.</param>
    public void SetRefField(FieldDefinition fieldDef, Type refType)
    {
        var refModelDef = refType.GetModelDefinition();

        var refSelf = modelDef.GetSelfRefFieldDefIfExists(refModelDef, fieldDef);
        var refField = refSelf == null
                           ? modelDef.GetRefFieldDef(refModelDef, refType)
                           : modelDef.GetRefFieldDefIfExists(refModelDef);

        if (refSelf != null)
        {
            var sql = GetRefSelfSql(refType, refSelf, refModelDef);
            if (sql == null)
                return;

            var result = dbCmd.ConvertTo(refType, sql);
            fieldDef.SetValue(instance, result);
        }
        else if (refField != null)
        {
            var sql = GetRefFieldSql(refType, refField);
            var result = dbCmd.ConvertTo(refType, sql);
            fieldDef.SetValue(instance, result);
        }
    }

    /// <summary>
    /// Sets the field reference.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="fieldRef">The field reference.</param>
    public void SetFieldReference(FieldDefinition fieldDef, FieldReference fieldRef)
    {
        var sqlRef = GetFieldReferenceSql(fieldDef, fieldRef);
        var result = dbCmd.ConvertTo(fieldRef.RefModel, sqlRef);
        var refFieldValue = fieldRef.RefFieldDef.GetValue(result);
        fieldDef.SetValue(instance, refFieldValue);
    }
}

#if ASYNC
/// <summary>
/// Class LoadReferencesAsync.
/// Implements the <see cref="ServiceStack.OrmLite.Support.LoadReferences{T}" />
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="ServiceStack.OrmLite.Support.LoadReferences{T}" />
internal class LoadReferencesAsync<T> : LoadReferences<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoadReferencesAsync{T}" /> class.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="instance">The instance.</param>
    public LoadReferencesAsync(IDbCommand dbCmd, T instance)
        : base(dbCmd, instance) { }

    /// <summary>
    /// Sets the reference field list.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="refType">Type of the reference.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    public async Task SetRefFieldList(FieldDefinition fieldDef, Type refType, CancellationToken token)
    {
        var sql = GetRefListSql(refType);

        var results = await dbCmd.ConvertToListAsync(refType, sql, token).ConfigAwait();
        fieldDef.SetValue(instance, results);
    }

    /// <summary>
    /// Sets the reference field.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="refType">Type of the reference.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    public async Task SetRefField(FieldDefinition fieldDef, Type refType, CancellationToken token)
    {
        var refModelDef = refType.GetModelDefinition();

        var refSelf = modelDef.GetSelfRefFieldDefIfExists(refModelDef, fieldDef);
        var refField = refSelf == null
                           ? modelDef.GetRefFieldDef(refModelDef, refType)
                           : modelDef.GetRefFieldDefIfExists(refModelDef);

        if (refField != null)
        {
            var sql = GetRefFieldSql(refType, refField);
            var result = await dbCmd.ConvertToAsync(refType, sql, token).ConfigAwait();
            fieldDef.SetValue(instance, result);
        }
        else if (refSelf != null)
        {
            var sql = GetRefSelfSql(refType, refSelf, refModelDef);
            if (sql == null)
                return;

            var result = await dbCmd.ConvertToAsync(refType, sql, token).ConfigAwait();
            fieldDef.SetValue(instance, result);
        }
    }

    /// <summary>
    /// Sets the field reference.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="fieldRef">The field reference.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    public async Task SetFieldReference(FieldDefinition fieldDef, FieldReference fieldRef, CancellationToken token)
    {
        var sqlRef = GetFieldReferenceSql(fieldDef, fieldRef);
        var result = await dbCmd.ConvertToAsync(fieldRef.RefModel, sqlRef, token).ConfigAwait();
        var refFieldValue = fieldRef.RefFieldDef.GetValue(result);
        fieldDef.SetValue(instance, refFieldValue);
    }
}
#endif