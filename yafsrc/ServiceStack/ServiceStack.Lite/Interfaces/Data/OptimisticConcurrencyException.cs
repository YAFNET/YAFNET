// ***********************************************************************
// <copyright file="OptimisticConcurrencyException.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Data;

/// <summary>
/// Class OptimisticConcurrencyException.
/// Implements the <see cref="ServiceStack.Data.DataException" />
/// </summary>
/// <seealso cref="ServiceStack.Data.DataException" />
public class OptimisticConcurrencyException : DataException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OptimisticConcurrencyException" /> class.
    /// </summary>
    public OptimisticConcurrencyException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="OptimisticConcurrencyException" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public OptimisticConcurrencyException(string message) : base(message) { }
}