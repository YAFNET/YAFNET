// ***********************************************************************
// <copyright file="TagAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#nullable enable
using System;

namespace ServiceStack;

/// <summary>
/// Class TagAttribute.
/// Implements the <see cref="ServiceStack.AttributeBase" />
/// </summary>
/// <seealso cref="ServiceStack.AttributeBase" />
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class TagAttribute : AttributeBase
{
    /// <summary>
    /// Get or sets tag name
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }

    /// <summary>
    /// Get or sets operation verbs for which the attribute be applied
    /// </summary>
    /// <value>The apply to.</value>
    public ApplyTo ApplyTo { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TagAttribute" /> class.
    /// </summary>
    public TagAttribute() : this(null) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="TagAttribute" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    public TagAttribute(string name) : this(name, ApplyTo.All) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="TagAttribute" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="applyTo">The apply to.</param>
    public TagAttribute(string name, ApplyTo applyTo)
    {
        this.Name = name;
        this.ApplyTo = applyTo;
    }
}