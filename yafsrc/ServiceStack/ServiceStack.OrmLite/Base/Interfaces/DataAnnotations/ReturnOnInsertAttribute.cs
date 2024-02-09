// ***********************************************************************
// <copyright file="ReturnOnInsertAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;

namespace ServiceStack.DataAnnotations;

/// <summary>
/// ReturnAttribute
/// Use to indicate that a property should be included in the
/// returning/output clause of INSERT sql sentences
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ReturnOnInsertAttribute : AttributeBase { }