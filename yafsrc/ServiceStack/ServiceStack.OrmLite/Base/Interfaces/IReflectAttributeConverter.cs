// ***********************************************************************
// <copyright file="IReflectAttributeConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#nullable enable
namespace ServiceStack;

using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// Interface IReflectAttributeConverter
/// </summary>
public interface IReflectAttributeConverter;

/// <summary>
/// Class ReflectAttribute.
/// </summary>
public class ReflectAttribute
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the constructor arguments.
    /// </summary>
    /// <value>The constructor arguments.</value>
    public List<KeyValuePair<PropertyInfo, object>>? ConstructorArgs { get; set; }

    /// <summary>
    /// Gets or sets the property arguments.
    /// </summary>
    /// <value>The property arguments.</value>
    public List<KeyValuePair<PropertyInfo, object>>? PropertyArgs { get; set; }
}