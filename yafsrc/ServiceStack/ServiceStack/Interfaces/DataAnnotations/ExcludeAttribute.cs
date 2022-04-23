// ***********************************************************************
// <copyright file="ExcludeAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.DataAnnotations;

/// <summary>
/// Mark types that are to be excluded from specified features
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ExcludeAttribute : AttributeBase
{
    /// <summary>
    /// Gets or sets the feature.
    /// </summary>
    /// <value>The feature.</value>
    public Feature Feature { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExcludeAttribute"/> class.
    /// </summary>
    /// <param name="feature">The feature.</param>
    public ExcludeAttribute(Feature feature)
    {
        Feature = feature;
    }
}

/// <summary>
/// Class ExcludeMetadataAttribute.
/// Implements the <see cref="ServiceStack.DataAnnotations.ExcludeAttribute" />
/// </summary>
/// <seealso cref="ServiceStack.DataAnnotations.ExcludeAttribute" />
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class ExcludeMetadataAttribute : ExcludeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExcludeMetadataAttribute"/> class.
    /// </summary>
    public ExcludeMetadataAttribute() : base(Feature.Metadata | Feature.Soap) { }
}