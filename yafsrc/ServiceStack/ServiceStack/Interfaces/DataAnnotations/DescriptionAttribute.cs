// ***********************************************************************
// <copyright file="DescriptionAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************


namespace ServiceStack.DataAnnotations;

/// <summary>
/// Class DescriptionAttribute.
/// Implements the <see cref="ServiceStack.AttributeBase" />
/// </summary>
/// <seealso cref="ServiceStack.AttributeBase" />
public class DescriptionAttribute : AttributeBase
{
    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DescriptionAttribute" /> class.
    /// </summary>
    /// <param name="description">The description.</param>
    public DescriptionAttribute(string description)
    {
        Description = description;
    }
}