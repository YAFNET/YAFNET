// ***********************************************************************
// <copyright file="IdAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.DataAnnotations;

/// <summary>
/// Class IdAttribute.
/// Implements the <see cref="ServiceStack.AttributeBase" />
/// </summary>
/// <seealso cref="ServiceStack.AttributeBase" />
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method)]
public class IdAttribute : AttributeBase
{
    /// <summary>
    /// Gets the identifier.
    /// </summary>
    /// <value>The identifier.</value>
    public int Id { get; }
    /// <summary>
    /// Initializes a new instance of the <see cref="IdAttribute"/> class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    public IdAttribute(int id)
    {
        Id = id;
    }
}