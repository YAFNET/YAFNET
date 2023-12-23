// ***********************************************************************
// <copyright file="SqlServerCollateAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.DataAnnotations;

// https://msdn.microsoft.com/en-us/library/ms184391.aspx
/// <summary>
/// Class SqlServerCollateAttribute.
/// Implements the <see cref="ServiceStack.AttributeBase" />
/// </summary>
/// <seealso cref="ServiceStack.AttributeBase" />
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class SqlServerCollateAttribute : AttributeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerCollateAttribute" /> class.
    /// </summary>
    /// <param name="collation">The collation.</param>
    public SqlServerCollateAttribute(string collation) { Collation = collation; }

    /// <summary>
    /// Gets or sets the collation.
    /// </summary>
    /// <value>The collation.</value>
    public string Collation { get; set; }
}