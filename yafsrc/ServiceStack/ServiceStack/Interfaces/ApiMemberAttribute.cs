// ***********************************************************************
// <copyright file="ApiMemberAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;

namespace ServiceStack
{
    /// <summary>
    /// Class ApiMemberAttribute.
    /// Implements the <see cref="ServiceStack.AttributeBase" />
    /// </summary>
    /// <seealso cref="ServiceStack.AttributeBase" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ApiMemberAttribute : AttributeBase
    {
        /// <summary>
        /// Gets or sets verb to which applies attribute. By default applies to all verbs.
        /// </summary>
        /// <value>The verb.</value>
        public string Verb { get; set; }

        /// <summary>
        /// Gets or sets parameter type: It can be only one of the following: path, query, body, form, or header.
        /// </summary>
        /// <value>The type of the parameter.</value>
        public string ParameterType { get; set; }

        /// <summary>
        /// Gets or sets unique name for the parameter. Each name must be unique, even if they are associated with different paramType values.
        /// </summary>
        /// <value>The name.</value>
        /// <remarks>Other notes on the name field:
        /// If paramType is body, the name is used only for UI and code generation.
        /// If paramType is path, the name field must correspond to the associated path segment from the path field in the api object.
        /// If paramType is query, the name field corresponds to the query param name.</remarks>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the human-readable description for the parameter.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// For path, query, and header paramTypes, this field must be a primitive. For body, this can be a complex or container datatype.
        /// </summary>
        /// <value>The type of the data.</value>
        public string DataType { get; set; }

        /// <summary>
        /// Fine-tuned primitive type definition.
        /// </summary>
        /// <value>The format.</value>
        public string Format { get; set; }

        /// <summary>
        /// For path, this is always true. Otherwise, this field tells the client whether or not the field must be supplied.
        /// </summary>
        /// <value><c>true</c> if this instance is required; otherwise, <c>false</c>.</value>
        public bool IsRequired { get; set; }

        /// <summary>
        /// For query params, this specifies that a comma-separated list of values can be passed to the API. For path and body types, this field cannot be true.
        /// </summary>
        /// <value><c>true</c> if [allow multiple]; otherwise, <c>false</c>.</value>
        public bool AllowMultiple { get; set; }

        /// <summary>
        /// Gets or sets route to which applies attribute, matches using StartsWith. By default applies to all routes.
        /// </summary>
        /// <value>The route.</value>
        public string Route { get; set; }

        /// <summary>
        /// Whether to exclude this property from being included in the ModelSchema
        /// </summary>
        /// <value><c>true</c> if [exclude in schema]; otherwise, <c>false</c>.</value>
        public bool ExcludeInSchema { get; set; }
    }
}
