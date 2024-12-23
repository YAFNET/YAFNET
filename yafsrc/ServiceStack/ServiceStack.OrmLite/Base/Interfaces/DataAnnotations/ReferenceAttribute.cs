// ***********************************************************************
// <copyright file="ReferenceAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#nullable enable

using System;

namespace ServiceStack.DataAnnotations;

/// <summary>
/// Class ReferenceAttribute.
/// Implements the <see cref="ServiceStack.AttributeBase" />
/// </summary>
/// <seealso cref="ServiceStack.AttributeBase" />
[AttributeUsage(AttributeTargets.Property)]
public class ReferenceAttribute : AttributeBase
{
    /// <summary>
    /// Gets or sets the self identifier.
    /// </summary>
    /// <value>The self identifier.</value>
    public string? SelfId { get; set; }

    /// <summary>
    /// Gets or sets the reference identifier.
    /// </summary>
    /// <value>The reference identifier.</value>
    public string? RefId { get; set; }
}