// ***********************************************************************
// <copyright file="JsonSerializer.Generic.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using ServiceStack.OrmLite.Base.Text.Common;
using ServiceStack.OrmLite.Base.Text.Json;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Class JsonSerializer.
/// Implements the <see cref="ITypeSerializer{T}" />
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="ITypeSerializer{T}" />
public class JsonSerializer<T> : ITypeSerializer<T>
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

        return (T)JsonReader<T>.Parse(value);
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

        if (typeof(T) == typeof(object) || typeof(T).IsAbstract || typeof(T).IsInterface)
        {
            var prevState = JsState.IsWritingDynamic;
            if (typeof(T).IsAbstract || typeof(T).IsInterface)
            {
                JsState.IsWritingDynamic = true;
            }

            var result = JsonSerializer.SerializeToString(value, value.GetType());
            if (typeof(T).IsAbstract || typeof(T).IsInterface)
            {
                JsState.IsWritingDynamic = prevState;
            }

            return result;
        }

        var writer = StringWriterThreadStatic.Allocate();
        JsonWriter<T>.WriteObject(writer, value);
        return StringWriterThreadStatic.ReturnAndFree(writer);
    }
}