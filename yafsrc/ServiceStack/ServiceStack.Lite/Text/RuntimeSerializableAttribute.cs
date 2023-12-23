// ***********************************************************************
// <copyright file="RuntimeSerializableAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.Text;

/// <summary>
/// Allow Type to be deserialized into late-bound object Types using __type info
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class RuntimeSerializableAttribute : Attribute { }