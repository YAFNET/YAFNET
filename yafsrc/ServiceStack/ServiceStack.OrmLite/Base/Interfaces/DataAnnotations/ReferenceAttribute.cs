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
/// Implements the <see cref="AttributeBase" />
/// </summary>
/// <seealso cref="AttributeBase" />
[AttributeUsage(AttributeTargets.Property)]
public class ReferenceAttribute : AttributeBase
{
    /// <summary>
    /// Use the specified Field on this POCO as the FK field for the referenced POCO Complex Type.
    /// </summary>
    public string? SelfId { get; set; }

    /// <summary>
    /// Specify the FK field to match of the referenced POCO Complex Type (default Primary Key).
    /// </summary>
    public string? RefId { get; set; }

    /// <summary>
    /// If configured will display the Reference Field instead of the default rendered complex type.
    /// </summary>
    public string? RefLabel { get; set; }
}