// ***********************************************************************
// <copyright file="ITypeSerializer.Generic.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Interface ITypeSerializer
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ITypeSerializer<T>
{
    /// <summary>
    /// Parses the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>T.</returns>
    T DeserializeFromString(string value);

    /// <summary>
    /// Serializes to string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    string SerializeToString(T value);
}