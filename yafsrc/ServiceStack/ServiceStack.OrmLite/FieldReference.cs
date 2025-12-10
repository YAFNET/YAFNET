// ***********************************************************************
// <copyright file="FieldReference.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite;

using System;

/// <summary>
/// Class FieldReference.
/// </summary>
public class FieldReference
{
    /// <summary>
    /// Gets the field definition.
    /// </summary>
    /// <value>The field definition.</value>
    public FieldDefinition FieldDef { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FieldReference"/> class.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    public FieldReference(FieldDefinition fieldDef)
    {
        this.FieldDef = fieldDef;
    }

    /// <summary>
    /// Foreign Key Table name
    /// </summary>
    /// <value>The reference model.</value>
    public Type RefModel { get; set; }

    /// <summary>
    /// Gets the reference model definition.
    /// </summary>
    /// <value>The reference model definition.</value>
    public ModelDefinition RefModelDef => field ??= this.RefModel.GetModelDefinition();

    /// <summary>
    /// The Field name on current Model to use for the Foreign Key Table Lookup
    /// </summary>
    /// <value>The reference identifier.</value>
    public string RefId { get; set; }

    /// <summary>
    /// Gets the reference identifier field definition.
    /// </summary>
    /// <value>The reference identifier field definition.</value>
    /// <exception cref="System.ArgumentException">Could not find '{this.RefId}' in '{this.RefModel.Name}'</exception>
    public FieldDefinition RefIdFieldDef =>
        field ??= this.FieldDef.ModelDef.GetFieldDefinition(this.RefId)
                  ?? throw new ArgumentException($"Could not find '{this.RefId}' in '{this.RefModel.Name}'");

    /// <summary>
    /// Specify Field to reference (if different from property name)
    /// </summary>
    /// <value>The reference field.</value>
    public string RefField { get; set; }

    /// <summary>
    /// Gets the reference field definition.
    /// </summary>
    /// <value>The reference field definition.</value>
    /// <exception cref="System.ArgumentException">Could not find '{this.RefField}' in '{this.RefModelDef.Name}'</exception>
    public FieldDefinition RefFieldDef =>
        field ??= this.RefModelDef.GetFieldDefinition(this.RefField)
                  ?? throw new ArgumentException($"Could not find '{this.RefField}' in '{this.RefModelDef.Name}'");
}