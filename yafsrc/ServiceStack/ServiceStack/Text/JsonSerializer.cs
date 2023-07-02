// ***********************************************************************
// <copyright file="JsonSerializer.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.IO;
using System.Net;

using ServiceStack.Text.Common;
using ServiceStack.Text.Json;

namespace ServiceStack.Text;

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
    /// The buffer size
    /// </summary>
    public static int BufferSize = 1024;

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
    /// Deserializes from span.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <returns>T.</returns>
    public static T DeserializeFromSpan<T>(ReadOnlySpan<char> value)
    {
        return JsonReader<T>.Parse(value) is T obj ? obj : default;
    }

    /// <summary>
    /// Deserializes from reader.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader">The reader.</param>
    /// <returns>T.</returns>
    public static T DeserializeFromReader<T>(TextReader reader)
    {
        return DeserializeFromString<T>(reader.ReadToEnd());
    }

    /// <summary>
    /// Deserializes from span.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public static object DeserializeFromSpan(Type type, ReadOnlySpan<char> value)
    {
        return value.IsEmpty
                   ? null
                   : JsonReader.GetParseSpanFn(type)(value);
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
    /// Deserializes from reader.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="type">The type.</param>
    /// <returns>System.Object.</returns>
    public static object DeserializeFromReader(TextReader reader, Type type)
    {
        return DeserializeFromString(reader.ReadToEnd(), type);
    }

    /// <summary>
    /// Serializes to string.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public static string SerializeToString<T>(T value)
    {
        if (value == null || value is Delegate) return null;
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
        if (value == null) return null;

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
    /// Serializes to writer.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="type">The type.</param>
    /// <param name="writer">The writer.</param>
    public static void SerializeToWriter(object value, Type type, TextWriter writer)
    {
        if (value == null) return;
        if (type == typeof(string))
        {
            JsonUtils.WriteString(writer, value as string);
            return;
        }

        OnSerialize?.Invoke(value);
        WriteObjectToWriter(value, JsonWriter.GetWriteFn(type), writer);
    }

    /// <summary>
    /// Serializes to stream.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <param name="stream">The stream.</param>
    public static void SerializeToStream<T>(T value, Stream stream)
    {
        if (value == null) return;
        if (typeof(T) == typeof(object))
        {
            SerializeToStream(value, value.GetType(), stream);
        }
        else if (typeof(T).IsAbstract || typeof(T).IsInterface)
        {
            var prevState = JsState.IsWritingDynamic;
            JsState.IsWritingDynamic = false;
            SerializeToStream(value, value.GetType(), stream);
            JsState.IsWritingDynamic = prevState;
        }
        else
        {
            var writer = new StreamWriter(stream, JsConfig.UTF8Encoding, BufferSize, true);
            WriteObjectToWriter(value, JsonWriter<T>.GetRootObjectWriteFn(value), writer);
            writer.Flush();
        }
    }

    /// <summary>
    /// Serializes to stream.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="type">The type.</param>
    /// <param name="stream">The stream.</param>
    public static void SerializeToStream(object value, Type type, Stream stream)
    {
        OnSerialize?.Invoke(value);
        var writer = new StreamWriter(stream, JsConfig.UTF8Encoding, BufferSize, true);
        WriteObjectToWriter(value, JsonWriter.GetWriteFn(type), writer);
        writer.Flush();
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

    /// <summary>
    /// Deserializes from stream.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream">The stream.</param>
    /// <returns>T.</returns>
    public static T DeserializeFromStream<T>(Stream stream)
    {
        return (T)MemoryProvider.Instance.Deserialize(stream, typeof(T), DeserializeFromSpan);
    }

    /// <summary>
    /// Deserializes from stream.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="stream">The stream.</param>
    /// <returns>System.Object.</returns>
    public static object DeserializeFromStream(Type type, Stream stream)
    {
        return MemoryProvider.Instance.Deserialize(stream, type, DeserializeFromSpan);
    }

    /// <summary>
    /// Deserializes the response.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="webRequest">The web request.</param>
    /// <returns>T.</returns>
    public static T DeserializeResponse<T>(WebRequest webRequest)
    {
        using var webRes = PclExport.Instance.GetResponse(webRequest);
        using var stream = webRes.GetResponseStream();
        return DeserializeFromStream<T>(stream);
    }

    /// <summary>
    /// Deserializes the response.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type">The type.</param>
    /// <param name="webRequest">The web request.</param>
    /// <returns>System.Object.</returns>
    public static object DeserializeResponse<T>(Type type, WebRequest webRequest)
    {
        using var webRes = PclExport.Instance.GetResponse(webRequest);
        using var stream = webRes.GetResponseStream();
        return DeserializeFromStream(type, stream);
    }

    /// <summary>
    /// Deserializes the request.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="webRequest">The web request.</param>
    /// <returns>T.</returns>
    public static T DeserializeRequest<T>(WebRequest webRequest)
    {
        using var webRes = PclExport.Instance.GetResponse(webRequest);
        return DeserializeResponse<T>(webRes);
    }

    /// <summary>
    /// Deserializes the request.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="webRequest">The web request.</param>
    /// <returns>System.Object.</returns>
    public static object DeserializeRequest(Type type, WebRequest webRequest)
    {
        using var webRes = PclExport.Instance.GetResponse(webRequest);
        return DeserializeResponse(type, webRes);
    }

    /// <summary>
    /// Deserializes the response.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="webResponse">The web response.</param>
    /// <returns>T.</returns>
    public static T DeserializeResponse<T>(WebResponse webResponse)
    {
        using var stream = webResponse.GetResponseStream();
        return DeserializeFromStream<T>(stream);
    }

    /// <summary>
    /// Deserializes the response.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="webResponse">The web response.</param>
    /// <returns>System.Object.</returns>
    public static object DeserializeResponse(Type type, WebResponse webResponse)
    {
        using var stream = webResponse.GetResponseStream();
        return DeserializeFromStream(type, stream);
    }
}

/// <summary>
/// Class JsonStringSerializer.
/// Implements the <see cref="ServiceStack.Text.IStringSerializer" />
/// </summary>
/// <seealso cref="ServiceStack.Text.IStringSerializer" />
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