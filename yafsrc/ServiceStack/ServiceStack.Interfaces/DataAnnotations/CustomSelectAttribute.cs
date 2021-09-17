// ***********************************************************************
// <copyright file="CustomSelectAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.DataAnnotations
{
    /// <summary>
    /// Class CustomSelectAttribute.
    /// Implements the <see cref="ServiceStack.AttributeBase" />
    /// </summary>
    /// <seealso cref="ServiceStack.AttributeBase" />
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomSelectAttribute : AttributeBase
    {
        /// <summary>
        /// Gets or sets the SQL.
        /// </summary>
        /// <value>The SQL.</value>
        public string Sql { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomSelectAttribute"/> class.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        public CustomSelectAttribute(string sql) => Sql = sql;
    }

    /// <summary>
    /// Class CustomInsertAttribute.
    /// Implements the <see cref="ServiceStack.AttributeBase" />
    /// </summary>
    /// <seealso cref="ServiceStack.AttributeBase" />
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomInsertAttribute : AttributeBase
    {
        /// <summary>
        /// Gets or sets the SQL.
        /// </summary>
        /// <value>The SQL.</value>
        public string Sql { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomInsertAttribute"/> class.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        public CustomInsertAttribute(string sql) => Sql = sql;
    }

    /// <summary>
    /// Class CustomUpdateAttribute.
    /// Implements the <see cref="ServiceStack.AttributeBase" />
    /// </summary>
    /// <seealso cref="ServiceStack.AttributeBase" />
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomUpdateAttribute : AttributeBase
    {
        /// <summary>
        /// Gets or sets the SQL.
        /// </summary>
        /// <value>The SQL.</value>
        public string Sql { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUpdateAttribute"/> class.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        public CustomUpdateAttribute(string sql) => Sql = sql;
    }
}