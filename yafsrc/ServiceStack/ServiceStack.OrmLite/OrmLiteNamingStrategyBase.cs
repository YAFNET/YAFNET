// ***********************************************************************
// <copyright file="OrmLiteNamingStrategyBase.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System.Collections.Generic;

namespace ServiceStack.OrmLite;

/// <summary>
/// Class OrmLiteNamingStrategyBase.
/// Implements the <see cref="ServiceStack.OrmLite.INamingStrategy" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.INamingStrategy" />
public class OrmLiteNamingStrategyBase : INamingStrategy
{
    public Dictionary<string, string> SchemaAliases { get; } = new();
    public Dictionary<string, string> TableAliases { get; } = new();
    public Dictionary<string, string> ColumnAliases { get; } = new();

    public virtual string GetTableAlias(string name) => name;
    public virtual string GetColumnAlias(string name) => name;
    public virtual string GetSchemaName(string name) => name == null ? null
        : SchemaAliases.TryGetValue(name, out var alias)
            ? alias
            : name;

    /// <summary>
    /// Gets the name of the schema.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    public virtual string GetSchemaName(ModelDefinition modelDef) => GetSchemaName(modelDef.Schema);

    /// <summary>
    /// Gets the name of the table.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public virtual string GetTableName(string name) => TableAliases.TryGetValue(name, out var alias)
        ? alias
        : name;

    /// <summary>
    /// Gets the name of the table.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    public virtual string GetTableName(ModelDefinition modelDef) => modelDef.Alias != null
        ? GetTableAlias(modelDef.Alias)
        : GetTableName(modelDef.Name);

    /// <summary>
    /// Gets the name of the column.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public virtual string GetColumnName(string name) => ColumnAliases.TryGetValue(name, out var alias)
        ? alias
        : name;

    /// <summary>
    /// Gets the name of the sequence.
    /// </summary>
    /// <param name="modelName">Name of the model.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>System.String.</returns>
    public virtual string GetSequenceName(string modelName, string fieldName) => "SEQ_" + modelName + "_" + fieldName;

    /// <summary>
    /// Applies the name restrictions.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public virtual string ApplyNameRestrictions(string name) => name;
}