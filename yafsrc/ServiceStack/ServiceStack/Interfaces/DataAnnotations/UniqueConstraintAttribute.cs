// ***********************************************************************
// <copyright file="UniqueConstraintAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace ServiceStack.DataAnnotations;

/// <summary>
/// Class UniqueConstraintAttribute.
/// Implements the <see cref="ServiceStack.AttributeBase" />
/// </summary>
/// <seealso cref="ServiceStack.AttributeBase" />
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public class UniqueConstraintAttribute : AttributeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UniqueConstraintAttribute" /> class.
    /// </summary>
    public UniqueConstraintAttribute()
    {
        this.FieldNames = new List<string>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UniqueConstraintAttribute" /> class.
    /// </summary>
    /// <param name="fieldNames">The field names.</param>
    public UniqueConstraintAttribute(params string[] fieldNames)
    {
        this.FieldNames = new List<string>(fieldNames);
    }

    /// <summary>
    /// Gets or sets the field names.
    /// </summary>
    /// <value>The field names.</value>
    public List<string> FieldNames { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }
}