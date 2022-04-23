// ***********************************************************************
// <copyright file="ReferencesAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.DataAnnotations;

using System;

/// <summary>
/// Class ReferencesAttribute.
/// Implements the <see cref="ServiceStack.AttributeBase" />
/// </summary>
/// <seealso cref="ServiceStack.AttributeBase" />
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public class ReferencesAttribute : AttributeBase
{
    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>The type.</value>
    public Type Type { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferencesAttribute"/> class.
    /// </summary>
    /// <param name="type">The type.</param>
    public ReferencesAttribute(Type type)
    {
        this.Type = type;
    }
}