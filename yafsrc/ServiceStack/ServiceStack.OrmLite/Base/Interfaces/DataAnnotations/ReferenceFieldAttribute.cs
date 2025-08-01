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
    /// The Foreign Key Table name
    /// </summary>
    public Type Model { get; set; }

    /// <summary>
    /// The Field name on current Model to use for the Foreign Key Table Lookup.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Specify Field to reference (if different from property name)
    /// </summary>
    public string Field { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceFieldAttribute"/> class.
    /// </summary>
    public ReferenceFieldAttribute() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceFieldAttribute"/> class.
    /// </summary>
    /// <param name="model">The Foreign Key Table name</param>
    /// <param name="id">The Field name on current Model to use for the Foreign Key Table Lookup.</param>
    public ReferenceFieldAttribute(Type model, string id)
    {
        this.Model = model;
        this.Id = id;
    }
}