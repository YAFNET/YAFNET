// ***********************************************************************
// <copyright file="ReferenceAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.DataAnnotations;

using System;

/// <summary>
/// Populate with a field from a foreign table in Load* APIs
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ReferenceFieldAttribute : AttributeBase
{
    /// <summary>
    /// Foreign Key Table name
    /// </summary>
    /// <value>The model.</value>
    public Type Model { get; set; }

    /// <summary>
    /// The Field name on current Model to use for the Foreign Key Table Lookup
    /// </summary>
    /// <value>The identifier.</value>
    public string Id { get; set; }

    /// <summary>
    /// Specify Field to reference (if different from property name)
    /// </summary>
    /// <value>The field.</value>
    public string Field { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceFieldAttribute"/> class.
    /// </summary>
    public ReferenceFieldAttribute() { }
    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceFieldAttribute"/> class.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <param name="id">The identifier.</param>
    public ReferenceFieldAttribute(Type model, string id)
    {
        Model = model;
        Id = id;
    }
}