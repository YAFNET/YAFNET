// ***********************************************************************
// <copyright file="CompositePrimaryKeyAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.DataAnnotations
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Class CompositePrimaryKeyAttribute.
    /// Implements the <see cref="ServiceStack.AttributeBase" />
    /// </summary>
    /// <seealso cref="ServiceStack.AttributeBase" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class CompositePrimaryKeyAttribute : AttributeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompositePrimaryKeyAttribute"/> class.
        /// </summary>
        /// <param name="fieldNames">The field names.</param>
        public CompositePrimaryKeyAttribute(params string[] fieldNames)
        {
            this.FieldNames = new List<string>(fieldNames);
        }

        /// <summary>
        /// Gets or sets the field names.
        /// </summary>
        /// <value>The field names.</value>
        public List<string> FieldNames { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
    }
}