// ***********************************************************************
// <copyright file="Property.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack;

using System.Collections.Generic;
using System.Runtime.Serialization;

/// <summary>
/// Class Property.
/// </summary>
[DataContract]
public class Property
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    [DataMember(Order = 1)]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>The value.</value>
    [DataMember(Order = 2)]
    public string Value { get; set; }
}

/// <summary>
/// Class Properties.
/// Implements the <see cref="Property" />
/// </summary>
/// <seealso cref="Property" />
[CollectionDataContract(ItemName = "Property")]
public class Properties : List<Property>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Properties" /> class.
    /// </summary>
    public Properties() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Properties" /> class.
    /// </summary>
    /// <param name="collection">The collection.</param>
    public Properties(IEnumerable<Property> collection)
        : base(collection)
    {
    }
}