// ***********************************************************************
// <copyright file="IPropertyInvoker.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************/

using System;
using System.Reflection;

namespace ServiceStack.OrmLite
{
    /// <summary>
    /// Interface IPropertyInvoker
    /// </summary>
    public interface IPropertyInvoker
    {
        /// <summary>
        /// Gets or sets the convert value function.
        /// </summary>
        /// <value>The convert value function.</value>
        Func<object, Type, object> ConvertValueFn { get; set; }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="onInstance">The on instance.</param>
        /// <param name="withValue">The with value.</param>
        void SetPropertyValue(PropertyInfo propertyInfo, Type fieldType, object onInstance, object withValue);

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="fromInstance">From instance.</param>
        /// <returns>System.Object.</returns>
        object GetPropertyValue(PropertyInfo propertyInfo, object fromInstance);
    }
}