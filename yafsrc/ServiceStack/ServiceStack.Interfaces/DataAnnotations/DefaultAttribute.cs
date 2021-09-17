// ***********************************************************************
// <copyright file="DefaultAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.DataAnnotations
{
    /// <summary>
    /// Class DefaultAttribute.
    /// Implements the <see cref="ServiceStack.AttributeBase" />
    /// </summary>
    /// <seealso cref="ServiceStack.AttributeBase" />
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DefaultAttribute : AttributeBase
    {
        /// <summary>
        /// Gets or sets the int value.
        /// </summary>
        /// <value>The int value.</value>
        public int IntValue { get; set; }
        /// <summary>
        /// Gets or sets the double value.
        /// </summary>
        /// <value>The double value.</value>
        public double DoubleValue { get; set; }

        /// <summary>
        /// Gets or sets the default type.
        /// </summary>
        /// <value>The default type.</value>
        public Type DefaultType { get; set; }
        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        /// <value>The default value.</value>
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [on update].
        /// </summary>
        /// <value><c>true</c> if [on update]; otherwise, <c>false</c>.</value>
        public bool OnUpdate { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAttribute"/> class.
        /// </summary>
        /// <param name="intValue">The int value.</param>
        public DefaultAttribute(int intValue)
        {
            this.IntValue = intValue;
            this.DefaultType = typeof(int);
            this.DefaultValue = this.IntValue.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAttribute"/> class.
        /// </summary>
        /// <param name="doubleValue">The double value.</param>
        public DefaultAttribute(double doubleValue)
        {
            this.DoubleValue = doubleValue;
            this.DefaultType = typeof(double);
            this.DefaultValue = doubleValue.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAttribute"/> class.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        public DefaultAttribute(string defaultValue)
        {
            this.DefaultType = typeof(string);
            this.DefaultValue = defaultValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAttribute"/> class.
        /// </summary>
        /// <param name="defaultType">The default type.</param>
        /// <param name="defaultValue">The default value.</param>
        public DefaultAttribute(Type defaultType, string defaultValue)
        {
            this.DefaultValue = defaultValue;
            this.DefaultType = defaultType;
        }
    }
}