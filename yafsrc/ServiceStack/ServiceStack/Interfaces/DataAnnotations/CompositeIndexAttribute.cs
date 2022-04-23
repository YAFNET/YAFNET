// ***********************************************************************
// <copyright file="CompositeIndexAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace ServiceStack.DataAnnotations;

/// <summary>
/// Class CompositeIndexAttribute.
/// Implements the <see cref="ServiceStack.AttributeBase" />
/// </summary>
/// <seealso cref="ServiceStack.AttributeBase" />
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public class CompositeIndexAttribute : AttributeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeIndexAttribute"/> class.
    /// </summary>
    public CompositeIndexAttribute()
    {
        this.FieldNames = new List<string>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeIndexAttribute"/> class.
    /// </summary>
    /// <param name="fieldNames">The field names.</param>
    public CompositeIndexAttribute(params string[] fieldNames)
    {
        this.FieldNames = new List<string>(fieldNames);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeIndexAttribute"/> class.
    /// </summary>
    /// <param name="unique">if set to <c>true</c> [unique].</param>
    /// <param name="fieldNames">The field names.</param>
    public CompositeIndexAttribute(bool unique, params string[] fieldNames)
    {
        this.Unique = unique;
        this.FieldNames = new List<string>(fieldNames);
    }

    /// <summary>
    /// Gets or sets the field names.
    /// </summary>
    /// <value>The field names.</value>
    public List<string> FieldNames { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="CompositeIndexAttribute"/> is unique.
    /// </summary>
    /// <value><c>true</c> if unique; otherwise, <c>false</c>.</value>
    public bool Unique { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }
}