// ***********************************************************************
// <copyright file="INamingStrategy.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System.Collections.Generic;

namespace ServiceStack.OrmLite;

/// <summary>
/// Interface INamingStrategy
/// </summary>
public interface INamingStrategy
{
    Dictionary<string, string> SchemaAliases { get; }
    Dictionary<string, string> TableAliases { get; }
    Dictionary<string, string> ColumnAliases { get; }

    /// <summary>
    /// Gets the table alias.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    string GetTableAlias(string name);

    /// <summary>
    /// Gets the column alias.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    string GetColumnAlias(string name);

    /// <summary>
    /// Gets the name of the schema.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    string GetSchemaName(string name);

    /// <summary>
    /// Gets the name of the schema.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    string GetSchemaName(ModelDefinition modelDef);

    /// <summary>
    /// Gets the name of the table.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    string GetTableName(string name);

    /// <summary>
    /// Gets the name of the table.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    string GetTableName(ModelDefinition modelDef);

    /// <summary>
    /// Gets the name of the column.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    string GetColumnName(string name);

    /// <summary>
    /// Gets the name of the sequence.
    /// </summary>
    /// <param name="modelName">Name of the model.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>System.String.</returns>
    string GetSequenceName(string modelName, string fieldName);

    /// <summary>
    /// Applies the name restrictions.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    string ApplyNameRestrictions(string name);
}