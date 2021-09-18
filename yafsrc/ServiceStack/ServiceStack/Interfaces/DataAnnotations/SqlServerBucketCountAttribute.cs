// ***********************************************************************
// <copyright file="SqlServerBucketCountAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.DataAnnotations
{
    // https://msdn.microsoft.com/en-us/library/dn494956.aspx
    /// <summary>
    /// Class SqlServerBucketCountAttribute.
    /// Implements the <see cref="ServiceStack.AttributeBase" />
    /// </summary>
    /// <seealso cref="ServiceStack.AttributeBase" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SqlServerBucketCountAttribute : AttributeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerBucketCountAttribute"/> class.
        /// </summary>
        /// <param name="count">The count.</param>
        public SqlServerBucketCountAttribute(int count) { Count = count; }

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count { get; set; }
    }
}