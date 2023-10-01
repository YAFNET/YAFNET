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
    /// <summary>
    /// Initializes a new instance of the <see cref="InfoException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public InfoException(string message) : base(message) { }

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
    public override string ToString() => Message;
}