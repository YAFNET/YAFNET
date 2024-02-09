// ***********************************************************************
// <copyright file="RowVersionAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.DataAnnotations;

/// <summary>
/// Used to indicate that property is a row version incremented automatically by the database
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class RowVersionAttribute : AttributeBase
{
}