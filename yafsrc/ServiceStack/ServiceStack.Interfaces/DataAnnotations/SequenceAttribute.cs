// ***********************************************************************
// <copyright file="SequenceAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.DataAnnotations
{
    /// <summary>
    /// Class SequenceAttribute.
    /// Implements the <see cref="ServiceStack.AttributeBase" />
    /// </summary>
    /// <seealso cref="ServiceStack.AttributeBase" />
    // </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SequenceAttribute : AttributeBase
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public SequenceAttribute(string name)
        {
            this.Name = name;
        }
    }
}