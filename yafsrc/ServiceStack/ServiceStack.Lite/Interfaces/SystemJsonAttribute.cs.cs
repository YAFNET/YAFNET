// ***********************************************************************
// <copyright file="SystemJsonAttribute.cs.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#if NET8_0_OR_GREATER

using System;

namespace ServiceStack;

[Flags]
public enum UseSystemJson
{
    Never = 0,
    Request = 1 << 0,
    Response = 1 << 1,
    Always = Request | Response,
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class SystemJsonAttribute(UseSystemJson use) : AttributeBase
{
    public UseSystemJson Use { get; set; } = use;
}

#endif