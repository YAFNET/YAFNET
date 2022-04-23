// ***********************************************************************
// <copyright file="CsvAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.Text;

/// <summary>
/// Enum CsvBehavior
/// </summary>
public enum CsvBehavior
{
    /// <summary>
    /// The first enumerable
    /// </summary>
    FirstEnumerable
}

/// <summary>
/// Class CsvAttribute.
/// Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
public class CsvAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the CSV behavior.
    /// </summary>
    /// <value>The CSV behavior.</value>
    public CsvBehavior CsvBehavior { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="CsvAttribute"/> class.
    /// </summary>
    /// <param name="csvBehavior">The CSV behavior.</param>
    public CsvAttribute(CsvBehavior csvBehavior)
    {
        this.CsvBehavior = csvBehavior;
    }
}