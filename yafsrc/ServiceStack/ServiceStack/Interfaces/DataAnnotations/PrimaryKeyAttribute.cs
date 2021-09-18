// ***********************************************************************
// <copyright file="PrimaryKeyAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.DataAnnotations
{
    /// <summary>
    /// Primary key attribute.
    /// use to indicate that property is part of the pk
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : AttributeBase
    {
    }
}