// ***********************************************************************
// <copyright file="SchemaAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.DataAnnotations
{
    /// <summary>
    /// Used to annotate an Entity with its DB schema
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SchemaAttribute : AttributeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public SchemaAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
    }
}