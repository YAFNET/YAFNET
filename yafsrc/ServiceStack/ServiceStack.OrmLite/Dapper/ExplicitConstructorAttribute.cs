// ***********************************************************************
// <copyright file="ExplicitConstructorAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.OrmLite.Dapper;

/// <summary>
/// Tell Dapper to use an explicit constructor, passing nulls or 0s for all parameters
/// </summary>
[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
public sealed class ExplicitConstructorAttribute : Attribute
{
}