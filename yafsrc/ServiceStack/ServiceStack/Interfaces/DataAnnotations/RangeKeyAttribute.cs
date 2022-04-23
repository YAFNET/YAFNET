// ***********************************************************************
// <copyright file="RangeKeyAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.DataAnnotations;

/// <summary>
/// Range Key Attribute used to specify which property is the RangeKey, e.g. in DynamoDb.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class RangeKeyAttribute : AttributeBase
{
}