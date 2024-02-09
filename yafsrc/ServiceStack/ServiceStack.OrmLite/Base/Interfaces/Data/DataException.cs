// ***********************************************************************
// <copyright file="DataException.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Data;

using System;

/// <summary>
/// Class DataException.
/// Implements the <see cref="System.Exception" />
/// </summary>
/// <seealso cref="System.Exception" />
public class DataException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataException"/> class.
    /// </summary>
    public DataException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DataException(string message) : base(message) { }
}