// ***********************************************************************
// <copyright file="ITypeSerializer.Generic.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.IO;

namespace ServiceStack.Text;

/// <summary>
/// Interface ITypeSerializer
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ITypeSerializer<T>
{
    /// <summary>
    /// Determines whether this serializer can create the specified type from a string.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns><c>true</c> if this instance [can create from string] the specified type; otherwise, <c>false</c>.</returns>
    bool CanCreateFromString(Type type);

    /// <summary>
    /// Parses the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>T.</returns>
    T DeserializeFromString(string value);

    /// <summary>
    /// Deserializes from reader.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns>T.</returns>
    T DeserializeFromReader(TextReader reader);

    /// <summary>
    /// Serializes to string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    string SerializeToString(T value);

    /// <summary>
    /// Serializes to writer.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="writer">The writer.</param>
    void SerializeToWriter(T value, TextWriter writer);
}