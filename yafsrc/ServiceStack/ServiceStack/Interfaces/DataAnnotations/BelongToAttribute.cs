// ***********************************************************************
// <copyright file="BelongToAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;

namespace ServiceStack.DataAnnotations;

/// <summary>
/// BelongToAttribute
/// Use to indicate that a join column belongs to another table.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class BelongToAttribute : AttributeBase
{
    /// <summary>
    /// Gets or sets the type of the belong to table.
    /// </summary>
    /// <value>The type of the belong to table.</value>
    public Type BelongToTableType { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BelongToAttribute"/> class.
    /// </summary>
    /// <param name="belongToTableType">Type of the belong to table.</param>
    public BelongToAttribute(Type belongToTableType)
    {
        BelongToTableType = belongToTableType;
    }
}