// ***********************************************************************
// <copyright file="InfoException.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using JetBrains.Annotations;
using System;

namespace ServiceStack;

/// <summary>
/// When only Exception message is important and StackTrace is irrelevant
/// </summary>
public class InfoException : Exception
{
    public InfoException([CanBeNull] string message) : base(message) { }

    public override string ToString() => Message;
}