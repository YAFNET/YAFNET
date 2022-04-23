// ***********************************************************************
// <copyright file="DecimalLengthAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.DataAnnotations;

/// <summary>
/// Decimal length attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class DecimalLengthAttribute : AttributeBase
{
    /// <summary>
    /// Gets or sets the precision.
    /// </summary>
    /// <value>The precision.</value>
    public int Precision { get; set; }
    /// <summary>
    /// Gets or sets the scale.
    /// </summary>
    /// <value>The scale.</value>
    public int Scale { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DecimalLengthAttribute"/> class.
    /// </summary>
    /// <param name="precision">The precision.</param>
    /// <param name="scale">The scale.</param>
    public DecimalLengthAttribute(int precision, int scale)
    {
        Precision = precision;
        Scale = scale;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DecimalLengthAttribute"/> class.
    /// </summary>
    /// <param name="precision">The precision.</param>
    public DecimalLengthAttribute(int precision)
        : this(precision, 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DecimalLengthAttribute"/> class.
    /// </summary>
    public DecimalLengthAttribute()
        : this(18, 0)
    {
    }

}