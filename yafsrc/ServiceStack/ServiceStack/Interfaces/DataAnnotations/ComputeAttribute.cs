// ***********************************************************************
// <copyright file="ComputeAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.DataAnnotations
{
    /// <summary>
    /// Compute attribute.
    /// Use to indicate that a property is a Calculated Field.
    /// Use [Persisted] attribute to persist column
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ComputeAttribute : AttributeBase
    {
        /// <summary>
        /// Gets or sets the expression.
        /// </summary>
        /// <value>The expression.</value>
        public string Expression { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComputeAttribute"/> class.
        /// </summary>
        public ComputeAttribute() : this(string.Empty) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComputeAttribute"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public ComputeAttribute(string expression)
        {
            Expression = expression;
        }
    }

    /// <summary>
    /// Class ComputedAttribute.
    /// Implements the <see cref="ServiceStack.AttributeBase" />
    /// </summary>
    /// <seealso cref="ServiceStack.AttributeBase" />
    [AttributeUsage(AttributeTargets.Property)]
    public class ComputedAttribute : AttributeBase { }

    /// <summary>
    /// Whether to persist field
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PersistedAttribute : AttributeBase { }
}

