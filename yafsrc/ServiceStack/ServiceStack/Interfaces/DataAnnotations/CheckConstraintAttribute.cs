// ***********************************************************************
// <copyright file="CheckConstraintAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.DataAnnotations
{
    /// <summary>
    /// Class CheckConstraintAttribute.
    /// Implements the <see cref="ServiceStack.AttributeBase" />
    /// </summary>
    /// <seealso cref="ServiceStack.AttributeBase" />
    [AttributeUsage(AttributeTargets.Property)]
    public class CheckConstraintAttribute : AttributeBase
    {
        /// <summary>
        /// Gets the constraint.
        /// </summary>
        /// <value>The constraint.</value>
        public string Constraint { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckConstraintAttribute"/> class.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        public CheckConstraintAttribute(string constraint)
        {
            this.Constraint = constraint;
        }
    }
}