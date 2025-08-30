// ***********************************************************************
// <copyright file="ForeignKeyConstraint.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

using System;

/// <summary>
/// Class ForeignKeyConstraint.
/// </summary>
public class ForeignKeyConstraint
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForeignKeyConstraint" /> class.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="onDelete">The on delete.</param>
    /// <param name="onUpdate">The on update.</param>
    /// <param name="foreignKeyName">Name of the foreign key.</param>
    public ForeignKeyConstraint(
        Type type,
        string onDelete = null,
        string onUpdate = null,
        string foreignKeyName = null)
    {
        this.ReferenceType = type;
        this.OnDelete = onDelete;
        this.OnUpdate = onUpdate;
        this.ForeignKeyName = foreignKeyName;
    }

    /// <summary>
    /// Gets the type of the reference.
    /// </summary>
    /// <value>The type of the reference.</value>
    public Type ReferenceType { get; private set; }

    /// <summary>
    /// Gets the on delete.
    /// </summary>
    /// <value>The on delete.</value>
    public string OnDelete { get; private set; }

    /// <summary>
    /// Gets the on update.
    /// </summary>
    /// <value>The on update.</value>
    public string OnUpdate { get; private set; }

    /// <summary>
    /// Gets the name of the foreign key.
    /// </summary>
    /// <value>The name of the foreign key.</value>
    public string ForeignKeyName { get; }

    /// <summary>
    /// Gets the name of the foreign key.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="refModelDef">The reference model definition.</param>
    /// <param name="namingStrategy">The naming strategy.</param>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>System.String.</returns>
    public string GetForeignKeyName(
        ModelDefinition modelDef,
        ModelDefinition refModelDef,
        INamingStrategy namingStrategy,
        FieldDefinition fieldDef)
    {
        if (this.ForeignKeyName.IsNullOrEmpty())
        {
            var modelName = modelDef.IsInSchema
                                ? $"{modelDef.Schema}_{namingStrategy.GetTableName(modelDef)}"
                                : namingStrategy.GetTableName(modelDef);

            var refModelName = refModelDef.IsInSchema
                                   ? $"{refModelDef.Schema}_{namingStrategy.GetTableName(refModelDef)}"
                                   : namingStrategy.GetTableName(refModelDef);

            var fkName = $"FK_{modelName}_{refModelName}_{fieldDef.FieldName}";
            return namingStrategy.ApplyNameRestrictions(fkName);
        }

        return this.ForeignKeyName;
    }
}