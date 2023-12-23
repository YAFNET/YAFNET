// ***********************************************************************
// <copyright file="IStringSerializer.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.Text;

/// <summary>
/// Interface IStringSerializer
/// </summary>
public interface IStringSerializer
{
    /// <summary>
    /// Deserializes from string.
    /// </summary>
    /// <typeparam name="To">The type of to.</typeparam>
    /// <param name="serializedText">The serialized text.</param>
    /// <returns>To.</returns>
    To DeserializeFromString<To>(string serializedText);
    /// <summary>
    /// Deserializes from string.
    /// </summary>
    /// <param name="serializedText">The serialized text.</param>
    /// <param name="type">The type.</param>
    /// <returns>System.Object.</returns>
    object DeserializeFromString(string serializedText, Type type);
    /// <summary>
    /// Serializes to string.
    /// </summary>
    /// <typeparam name="TFrom">The type of the t from.</typeparam>
    /// <param name="from">From.</param>
    /// <returns>System.String.</returns>
    string SerializeToString<TFrom>(TFrom from);
}