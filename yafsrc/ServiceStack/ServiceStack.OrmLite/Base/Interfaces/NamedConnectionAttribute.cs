// ***********************************************************************
// <copyright file="NamedConnectionAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#nullable enable
using System;

namespace ServiceStack;

/// <summary>
/// Class NamedConnectionAttribute.
/// Implements the <see cref="ServiceStack.AttributeBase" />
/// </summary>
/// <seealso cref="ServiceStack.AttributeBase" />
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class NamedConnectionAttribute : AttributeBase
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NamedConnectionAttribute"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    public NamedConnectionAttribute(string name)
    {
        this.Name = name;
    }
}