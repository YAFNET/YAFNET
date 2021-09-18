// ***********************************************************************
// <copyright file="PageAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack
{
    /// <summary>
    /// Specify a VirtualPath or Layout for a Code Page
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class PageAttribute : AttributeBase
    {
        /// <summary>
        /// Gets or sets the virtual path.
        /// </summary>
        /// <value>The virtual path.</value>
        public string VirtualPath { get; set; }
        /// <summary>
        /// Gets or sets the layout.
        /// </summary>
        /// <value>The layout.</value>
        public string Layout { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageAttribute"/> class.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <param name="layout">The layout.</param>
        public PageAttribute(string virtualPath, string layout = null)
        {
            VirtualPath = virtualPath;
            Layout = layout;
        }
    }

    /// <summary>
    /// Specify static page arguments
    /// </summary>
    public class PageArgAttribute : AttributeBase
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageArgAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public PageArgAttribute(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
