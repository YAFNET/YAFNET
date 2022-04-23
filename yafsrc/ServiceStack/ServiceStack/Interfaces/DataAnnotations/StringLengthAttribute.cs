// ***********************************************************************
// <copyright file="StringLengthAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************


namespace ServiceStack.DataAnnotations;

/// <summary>
/// Class StringLengthAttribute.
/// Implements the <see cref="ServiceStack.AttributeBase" />
/// </summary>
/// <seealso cref="ServiceStack.AttributeBase" />
public class StringLengthAttribute : AttributeBase
{
    /// <summary>
    /// The maximum text
    /// </summary>
    public const int MaxText = int.MaxValue;
    /// <summary>
    /// Gets or sets the minimum length.
    /// </summary>
    /// <value>The minimum length.</value>
    public int MinimumLength { get; set; }
    /// <summary>
    /// Gets or sets the maximum length.
    /// </summary>
    /// <value>The maximum length.</value>
    public int MaximumLength { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringLengthAttribute"/> class.
    /// </summary>
    /// <param name="maximumLength">The maximum length.</param>
    public StringLengthAttribute(int maximumLength)
    {
        MaximumLength = maximumLength;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringLengthAttribute"/> class.
    /// </summary>
    /// <param name="minimumLength">The minimum length.</param>
    /// <param name="maximumLength">The maximum length.</param>
    public StringLengthAttribute(int minimumLength, int maximumLength)
    {
        MinimumLength = minimumLength;
        MaximumLength = maximumLength;
    }
}