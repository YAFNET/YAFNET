// ***********************************************************************
// <copyright file="ApiAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#nullable enable
using System;

namespace ServiceStack;

/// <summary>
/// Class GenerateBodyParameter.
/// </summary>
public static class GenerateBodyParameter
{
    /// <summary>
    /// Generates body DTO parameter only if `DisableAutoDtoInBodyParam = false`
    /// </summary>
    public const int IfNotDisabled = 0;
    /// <summary>
    /// Always generate body DTO for request
    /// </summary>
    public const int Always = 1;
    /// <summary>
    /// Never generate body DTO for request
    /// </summary>
    public const int Never = 2;
}

/// <summary>
/// Class ApiAttribute.
/// Implements the <see cref="ServiceStack.AttributeBase" />
/// </summary>
/// <seealso cref="ServiceStack.AttributeBase" />
[AttributeUsage(AttributeTargets.Class)]
public class ApiAttribute : AttributeBase
{
    /// <summary>
    /// The overall description of an API. Used by Swagger.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }

    /// <summary>
    /// Create or not body param for request type when verb is POST or PUT.
    /// Value can be one of the constants of `GenerateBodyParam` class:
    /// `GenerateBodyParam.IfNotDisabled` (default value), `GenerateBodyParam.Always`, `GenerateBodyParam.Never`
    /// </summary>
    /// <value>The body parameter.</value>
    public int BodyParameter { get; set; }

    /// <summary>
    /// Tells if body param is required
    /// </summary>
    /// <value><c>true</c> if this instance is required; otherwise, <c>false</c>.</value>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiAttribute" /> class.
    /// </summary>
    public ApiAttribute() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiAttribute" /> class.
    /// </summary>
    /// <param name="description">The description.</param>
    public ApiAttribute(string description) : this(description, GenerateBodyParameter.IfNotDisabled) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiAttribute" /> class.
    /// </summary>
    /// <param name="description">The description.</param>
    /// <param name="generateBodyParameter">The generate body parameter.</param>
    public ApiAttribute(string description, int generateBodyParameter) : this(description, generateBodyParameter, false) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiAttribute" /> class.
    /// </summary>
    /// <param name="description">The description.</param>
    /// <param name="generateBodyParameter">The generate body parameter.</param>
    /// <param name="isRequired">if set to <c>true</c> [is required].</param>
    public ApiAttribute(string description, int generateBodyParameter, bool isRequired)
    {
        this.Description = description;
        this.BodyParameter = generateBodyParameter;
        this.IsRequired = isRequired;
    }
}