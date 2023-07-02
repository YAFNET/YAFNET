// ***********************************************************************
// <copyright file="AttributeBase.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************


using System;

namespace ServiceStack;

/// <summary>
/// Class AttributeBase.
/// Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
public class AttributeBase : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeBase" /> class.
    /// </summary>
    public AttributeBase()
    {
        this.typeId = Guid.NewGuid();
    }

    /// <summary>
    /// The type identifier
    /// </summary>
    protected readonly Guid typeId; //Hack required to give Attributes unique identity
}