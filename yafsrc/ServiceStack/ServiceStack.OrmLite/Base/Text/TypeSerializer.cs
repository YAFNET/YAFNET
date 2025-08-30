// ***********************************************************************
// <copyright file="TypeSerializer.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

using ServiceStack.OrmLite.Base.Text.Common;
using ServiceStack.OrmLite.Base.Text.Jsv;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Creates an instance of a Type from a string value
/// </summary>
public static class TypeSerializer
{
    /// <summary>
    /// Initializes static members of the <see cref="TypeSerializer" /> class.
    /// </summary>
    static TypeSerializer()
    {
        JsConfig.InitStatics();
    }

    /// <summary>
    /// Gets or sets the on serialize.
    /// </summary>
    /// <value>The on serialize.</value>
    public static Action<object> OnSerialize { get; set; }

    /// <summary>
    /// The double quote string
    /// </summary>
    public const string DoubleQuoteString = "\"\"";

    /// <summary>
    /// Determines whether the specified type is convertible from string.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns><c>true</c> if the specified type is convertible from string; otherwise, <c>false</c>.</returns>
    public static bool CanCreateFromString(Type type)
    {
        return JsvReader.GetParseFn(type) != null;
    }

    /// <summary>
    /// Parses the specified value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <returns>T.</returns>
    public static T DeserializeFromString<T>(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return default;
        }

        return (T)JsvReader<T>.Parse(value);
    }

    /// <summary>
    /// Parses the specified type.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="type">The type.</param>
    /// <returns>System.Object.</returns>
    public static object DeserializeFromString(string value, Type type)
    {
        return value == null
                   ? null
                   : JsvReader.GetParseFn(type)(value);
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
                   : JsvReader.GetParseSpanFn(type)(value);
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
            JsState.IsWritingDynamic = true;
            var result = SerializeToString(value, value.GetType());
            JsState.IsWritingDynamic = false;
            return result;
        }

        var writer = StringWriterThreadStatic.Allocate();
        JsvWriter<T>.WriteRootObject(writer, value);
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
        switch (value)
        {
            case null:
                return null;
            case string str:
                return str.EncodeJsv();
        }

        OnSerialize?.Invoke(value);
        var writer = StringWriterThreadStatic.Allocate();
        JsvWriter.GetWriteFn(type)(writer, value);
        return StringWriterThreadStatic.ReturnAndFree(writer);
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
        var writer = new StreamWriter(stream, JsConfig.UTF8Encoding);
        JsvWriter.GetWriteFn(type)(writer, value);
        writer.Flush();
    }

    /// <summary>
    /// Print string.Format to Console.WriteLine
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="args">The arguments.</param>
    public static void Print(this string text, params object[] args)
    {
        if (args.Length > 0)
        {
            PclExport.Instance.WriteLine(text, args);
        }
        else
        {
            PclExport.Instance.WriteLine(text);
        }
    }

    /// <summary>
    /// Prints the specified int value.
    /// </summary>
    /// <param name="intValue">The int value.</param>
    public static void Print(this int intValue)
    {
        PclExport.Instance.WriteLine(intValue.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Prints the specified long value.
    /// </summary>
    /// <param name="longValue">The long value.</param>
    public static void Print(this long longValue)
    {
        PclExport.Instance.WriteLine(longValue.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Determines whether [has circular references] [the specified value].
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if [has circular references] [the specified value]; otherwise, <c>false</c>.</returns>
    public static bool HasCircularReferences(object value)
    {
        return HasCircularReferences(value, null);
    }

    /// <summary>
    /// Determines whether [has circular references] [the specified value].
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="parentValues">The parent values.</param>
    /// <returns><c>true</c> if [has circular references] [the specified value]; otherwise, <c>false</c>.</returns>
    private static bool HasCircularReferences(object value, Stack<object> parentValues)
    {
        var type = value?.GetType();

        if (type is not { IsClass: true } || value is string or Type)
        {
            return false;
        }

        if (parentValues == null)
        {
            parentValues = new Stack<object>();
            parentValues.Push(value);
        }

        bool CheckValue(object key)
        {
            if (parentValues.Contains(key))
            {
                return true;
            }

            parentValues.Push(key);

            if (HasCircularReferences(key, parentValues))
            {
                return true;
            }

            parentValues.Pop();
            return false;
        }

        if (value is IEnumerable valueEnumerable)
        {
            foreach (var item in valueEnumerable)
            {
                if (item == null)
                {
                    continue;
                }

                var itemType = item.GetType();
                if (itemType.IsGenericType && itemType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                {
                    var props = TypeProperties.Get(itemType);
                    var key = props.GetPublicGetter("Key")(item);

                    if (CheckValue(key))
                    {
                        return true;
                    }

                    var val = props.GetPublicGetter("Value")(item);

                    if (CheckValue(val))
                    {
                        return true;
                    }
                }

                if (CheckValue(item))
                {
                    return true;
                }
            }
        }
        else
        {
            var props = type.GetSerializableProperties();

            foreach (var pi in props)
            {
                if (pi.GetIndexParameters().Length > 0)
                {
                    continue;
                }

                try
                {
                    var mi = pi.GetGetMethod(false);
                    var pValue = mi != null ? mi.Invoke(value, null) : null;
                    if (pValue == null)
                    {
                        continue;
                    }

                    if (CheckValue(pValue))
                    {
                        return true;
                    }
                }
                catch (TargetInvocationException e)
                {
                    Tracer.Instance.WriteError($"Failed to access property {type.Name}.{pi.Name}: {e.InnerException?.Message}", e);
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Timeses the specified count.
    /// </summary>
    /// <param name="count">The count.</param>
    /// <param name="fn">The function.</param>
    private static void times(int count, Action fn)
    {
        for (var i = 0; i < count; i++)
        {
            fn();
        }
    }

    /// <summary>
    /// The indent
    /// </summary>
    private const string Indent = "    ";
    /// <summary>
    /// Indents the json.
    /// </summary>
    /// <param name="json">The json.</param>
    /// <returns>System.String.</returns>
    public static string IndentJson(this string json)
    {
        var indent = 0;
        var quoted = false;
        var sb = StringBuilderThreadStatic.Allocate();

        for (var i = 0; i < json.Length; i++)
        {
            var ch = json[i];
            switch (ch)
            {
                case '{':
                case '[':
                    sb.Append(ch);
                    if (!quoted)
                    {
                        sb.AppendLine();
                        times(++indent, () => sb.Append(Indent));
                    }
                    break;
                case '}':
                case ']':
                    if (!quoted)
                    {
                        sb.AppendLine();
                        times(--indent, () => sb.Append(Indent));
                    }
                    sb.Append(ch);
                    break;
                case '"':
                    sb.Append(ch);
                    var escaped = false;
                    var index = i;
                    while (index > 0 && json[--index] == '\\')
                    {
                        escaped = !escaped;
                    }

                    if (!escaped)
                    {
                        quoted = !quoted;
                    }

                    break;
                case ',':
                    sb.Append(ch);
                    if (!quoted)
                    {
                        sb.AppendLine();
                        times(indent, () => sb.Append(Indent));
                    }
                    break;
                case ':':
                    sb.Append(ch);
                    if (!quoted)
                    {
                        sb.Append(' ');
                    }

                    break;
                default:
                    sb.Append(ch);
                    break;
            }
        }
        return StringBuilderThreadStatic.ReturnAndFree(sb);
    }
}

/// <summary>
/// Class JsvStringSerializer.
/// Implements the <see cref="IStringSerializer" />
/// </summary>
/// <seealso cref="IStringSerializer" />
public class JsvStringSerializer : IStringSerializer
{
    /// <summary>
    /// Deserializes from string.
    /// </summary>
    /// <typeparam name="To">The type of to.</typeparam>
    /// <param name="serializedText">The serialized text.</param>
    /// <returns>To.</returns>
    public To DeserializeFromString<To>(string serializedText)
    {
        return TypeSerializer.DeserializeFromString<To>(serializedText);
    }

    /// <summary>
    /// Deserializes from string.
    /// </summary>
    /// <param name="serializedText">The serialized text.</param>
    /// <param name="type">The type.</param>
    /// <returns>System.Object.</returns>
    public object DeserializeFromString(string serializedText, Type type)
    {
        return TypeSerializer.DeserializeFromString(serializedText, type);
    }

    /// <summary>
    /// Serializes to string.
    /// </summary>
    /// <typeparam name="TFrom">The type of the t from.</typeparam>
    /// <param name="from">From.</param>
    /// <returns>System.String.</returns>
    public string SerializeToString<TFrom>(TFrom from)
    {
        return TypeSerializer.SerializeToString(from);
    }
}