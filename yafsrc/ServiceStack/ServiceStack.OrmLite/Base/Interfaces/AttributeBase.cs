// ***********************************************************************
// <copyright file="AttributeBase.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#nullable enable
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
    /// The type identifier
    /// </summary>
    readonly protected Guid typeId = Guid.NewGuid(); //Hack required to give Attributes unique identity
}