// ***********************************************************************
// <copyright file="TypeSerializer.Generic.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using ServiceStack.OrmLite.Base.Text.Jsv;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Class TypeSerializer.
/// Implements the <see cref="ITypeSerializer{T}" />
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="ITypeSerializer{T}" />
public class TypeSerializer<T> : ITypeSerializer<T>
{
    /// <summary>
    /// Parses the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>T.</returns>
    public T DeserializeFromString(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return default;
        }

        return (T)JsvReader<T>.Parse(value);
    }

    /// <summary>
    /// Serializes to string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public string SerializeToString(T value)
    {
        if (value == null)
        {
            return null;
        }

        if (typeof(T) == typeof(string))
        {
            return value as string;
        }

        var writer = StringWriterThreadStatic.Allocate();
        JsvWriter<T>.WriteObject(writer, value);
        return StringWriterThreadStatic.ReturnAndFree(writer);
    }
}