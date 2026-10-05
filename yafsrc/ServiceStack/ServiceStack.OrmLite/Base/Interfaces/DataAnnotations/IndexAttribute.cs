// ***********************************************************************
// <copyright file="IndexAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.DataAnnotations;

/// <summary>
/// Class IndexAttribute.
/// Implements the <see cref="ServiceStack.AttributeBase" />
/// </summary>
/// <seealso cref="ServiceStack.AttributeBase" />
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Struct)]
public class IndexAttribute : AttributeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IndexAttribute"/> class.
    /// </summary>
    public IndexAttribute() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexAttribute"/> class.
    /// </summary>
    /// <param name="unique">if set to <c>true</c> [unique].</param>
    public IndexAttribute(bool unique)
    {
        Unique = unique;
    }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="IndexAttribute"/> is unique.
    /// </summary>
    /// <value><c>true</c> if unique; otherwise, <c>false</c>.</value>
    public bool Unique { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="IndexAttribute"/> is clustered.
    /// </summary>
    /// <value><c>true</c> if clustered; otherwise, <c>false</c>.</value>
    public bool Clustered { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [non clustered].
    /// </summary>
    /// <value><c>true</c> if [non clustered]; otherwise, <c>false</c>.</value>
    public bool NonClustered { get; set; }

    /// <summary>
    /// Only index the rows matching this SQL condition (a filtered or partial index), e.g:
    /// <para>[Index(Where = "{{DeletedDate}} IS NULL")]</para>
    /// {Property} names are replaced with their quoted column name.
    /// </summary>
    /// <value>The where condition.</value>
    public string Where { get; set; }

    /// <summary>
    /// Other properties whose columns are kept in the index (a covering index), so queries that only use them
    /// and the indexed column are answered from the index
    /// </summary>
    /// <value>The included properties.</value>
    public string[] Include { get; set; }
}
