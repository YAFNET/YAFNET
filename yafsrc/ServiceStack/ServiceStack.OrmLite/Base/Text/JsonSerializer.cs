// ***********************************************************************
// <copyright file="JsonSerializer.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.IO;
using System.Net;

using ServiceStack.OrmLite.Base.Text.Common;
using ServiceStack.OrmLite.Base.Text.Json;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Creates an instance of a Type from a string value
/// </summary>
public static class JsonSerializer
{
    /// <summary>
    /// Initializes static members of the <see cref="JsonSerializer" /> class.
    /// </summary>
    static JsonSerializer()
    {
        JsConfig.InitStatics();
    }

    /// <summary>
    /// Gets or sets the on serialize.
    /// </summary>
    /// <value>The on serialize.</value>
    public static Action<object> OnSerialize { get; set; }

    /// <summary>
    /// Deserializes from string.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <returns>T.</returns>
    public static T DeserializeFromString<T>(string value)
    {
        return JsonReader<T>.Parse(value) is T obj ? obj : default;
    }

    /// <summary>
    /// Deserializes from string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="type">The type.</param>
    /// <returns>System.Object.</returns>
    public static object DeserializeFromString(string value, Type type)
    {
        return string.IsNullOrEmpty(value)
                   ? null
                   : JsonReader.GetParseFn(type)(value);
    }

    /// <summary>
    /// Serializes to string.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public static string SerializeToString<T>(T value)
    {
        if (value == null || value is Delegate)
        {
            return null;
        }

        if (typeof(T) == typeof(object))
        {
            return SerializeToString(value, value.GetType());
        }
        if (typeof(T).IsAbstract || typeof(T).IsInterface)
        {
            var prevState = JsState.IsWritingDynamic;
            JsState.IsWritingDynamic = true;
            var result = SerializeToString(value, value.GetType());
            JsState.IsWritingDynamic = prevState;
            return result;
        }

        var writer = StringWriterThreadStatic.Allocate();
        if (typeof(T) == typeof(string))
        {
            JsonUtils.WriteString(writer, value as string);
        }
        else
        {
            WriteObjectToWriter(value, JsonWriter<T>.GetRootObjectWriteFn(value), writer);
        }
        return StringWriterThreadStatic.ReturnAndFree(writer);
    }

    /// <summary>
    /// Serializes to string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="type">The type.</param>
    /// <returns>System.String.</returns>
    public static string SerializeToString(object value, Type type)
    {
        if (value == null)
        {
            return null;
        }

        var writer = StringWriterThreadStatic.Allocate();
        if (type == typeof(string))
        {
            JsonUtils.WriteString(writer, value as string);
        }
        else
        {
            OnSerialize?.Invoke(value);
            WriteObjectToWriter(value, JsonWriter.GetWriteFn(type), writer);
        }
        return StringWriterThreadStatic.ReturnAndFree(writer);
    }

    /// <summary>
    /// Writes the object to writer.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="serializeFn">The serialize function.</param>
    /// <param name="writer">The writer.</param>
    private static void WriteObjectToWriter(object value, WriteObjectDelegate serializeFn, TextWriter writer)
    {
        if (!JsConfig.Indent)
        {
            serializeFn(writer, value);
        }
        else
        {
            var sb = StringBuilderCache.Allocate();
            using var captureJson = new StringWriter(sb);
            serializeFn(captureJson, value);
            captureJson.Flush();
            var json = StringBuilderCache.ReturnAndFree(sb);
            var indentJson = json.IndentJson();
            writer.Write(indentJson);
        }
    }
}

/// <summary>
/// Class JsonStringSerializer.
/// Implements the <see cref="IStringSerializer" />
/// </summary>
/// <seealso cref="IStringSerializer" />
public class JsonStringSerializer : IStringSerializer
{
    /// <summary>
    /// Deserializes from string.
    /// </summary>
    /// <typeparam name="To">The type of to.</typeparam>
    /// <param name="serializedText">The serialized text.</param>
    /// <returns>To.</returns>
    public To DeserializeFromString<To>(string serializedText)
    {
        return JsonSerializer.DeserializeFromString<To>(serializedText);
    }

    /// <summary>
    /// Deserializes from string.
    /// </summary>
    /// <param name="serializedText">The serialized text.</param>
    /// <param name="type">The type.</param>
    /// <returns>System.Object.</returns>
    public object DeserializeFromString(string serializedText, Type type)
    {
        return JsonSerializer.DeserializeFromString(serializedText, type);
    }

    /// <summary>
    /// Serializes to string.
    /// </summary>
    /// <typeparam name="TFrom">The type of the t from.</typeparam>
    /// <param name="from">From.</param>
    /// <returns>System.String.</returns>
    public string SerializeToString<TFrom>(TFrom from)
    {
        return JsonSerializer.SerializeToString(from);
    }
}