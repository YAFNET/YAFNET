// ***********************************************************************
// <copyright file="CompositeKeyAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace ServiceStack.DataAnnotations;

/// <summary>
/// Class CompositeKeyAttribute.
/// Implements the <see cref="ServiceStack.AttributeBase" />
/// </summary>
/// <seealso cref="ServiceStack.AttributeBase" />
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public class CompositeKeyAttribute : AttributeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeKeyAttribute" /> class.
    /// </summary>
    public CompositeKeyAttribute()
    {
        this.FieldNames = new List<string>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeKeyAttribute" /> class.
    /// </summary>
    /// <param name="fieldNames">The field names.</param>
    public CompositeKeyAttribute(params string[] fieldNames)
    {
        this.FieldNames = new List<string>(fieldNames);
    }

    /// <summary>
    /// Gets or sets the field names.
    /// </summary>
    /// <value>The field names.</value>
    public List<string> FieldNames { get; set; }
}