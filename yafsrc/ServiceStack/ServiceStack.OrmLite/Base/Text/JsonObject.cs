// ***********************************************************************
// <copyright file="JsonObject.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using ServiceStack.OrmLite.Base.Text.Common;
using ServiceStack.OrmLite.Base.Text.Json;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Class JsonExtensions.
/// </summary>
public static class JsonExtensions
{
    /// <param name="map">The map.</param>
    extension(Dictionary<string, string> map)
    {
        /// <summary>
        /// JSON to Type
        /// </summary>
        /// <typeparam name="T">the type.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>the type</returns>
        public T JsonTo<T>(string key)
        {
            return Get<T>(map, key);
        }

        /// <summary>
        /// Get JSON string value converted to T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>T.</returns>
        public T Get<T>(string key, T defaultValue = default)
        {
            if (map == null)
            {
                return default;
            }

            return map.TryGetValue(key, out var strVal) ? JsonSerializer.DeserializeFromString<T>(strVal) : defaultValue;
        }

        /// <summary>
        /// Gets the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns>T[].</returns>
        public T[] GetArray<T>(string key)
        {
            if (map == null)
            {
                return TypeConstants<T>.EmptyArray;
            }

            return map.TryGetValue(key, out var value)
                ? map is JsonObject ? value.FromJson<T[]>() : value.FromJsv<T[]>()
                : TypeConstants<T>.EmptyArray;
        }

        /// <summary>
        /// Get JSON string value
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public string Get(string key)
        {
            if (map == null)
            {
                return null;
            }

            return map.TryGetValue(key, out var strVal)
                ? JsonTypeSerializer.Instance.UnescapeString(strVal)
                : null;
        }
    }

    /// <summary>
    /// Converts all.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="jsonArrayObjects">The json array objects.</param>
    /// <param name="converter">The converter.</param>
    /// <returns>List&lt;T&gt;.</returns>
    public static List<T> ConvertAll<T>(this JsonArrayObjects jsonArrayObjects, Func<JsonObject, T> converter)
    {
        var results = new List<T>();

        foreach (var jsonObject in jsonArrayObjects)
        {
            results.Add(converter(jsonObject));
        }

        return results;
    }

    /// <param name="jsonObject">The json object.</param>
    extension(JsonObject jsonObject)
    {
        /// <summary>
        /// Converts to.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="convertFn">The convert function.</param>
        /// <returns>T.</returns>
        public T ConvertTo<T>(Func<JsonObject, T> convertFn)
        {
            return jsonObject == null
                ? default
                : convertFn(jsonObject);
        }

        /// <summary>
        /// Converts to dictionary.
        /// </summary>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        public Dictionary<string, string> ToDictionary()
        {
            return jsonObject == null
                ? []
                : new Dictionary<string, string>(jsonObject);
        }
    }
}

/// <summary>
/// Class JsonObject.
/// Implements the <see cref="string" />
/// </summary>
/// <seealso cref="string" />
public class JsonObject : Dictionary<string, string>, IEnumerable<KeyValuePair<string, string>>
{
    /// <summary>
    /// Get JSON string value
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>System.String.</returns>
    public new string this[string key]
    {
        get => this.Get(key);
        set => base[key] = value;
    }

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns>A <see cref="T:System.Collections.Generic.Dictionary`2.Enumerator" /> structure for the <see cref="T:System.Collections.Generic.Dictionary`2" />.</returns>
    public new Enumerator GetEnumerator()
    {
        var to = new Dictionary<string, string>();
        foreach (var key in this.Keys)
        {
            to[key] = this[key];
        }

        return to.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    /// <summary>
    /// Converts to an unescaped dictionary.
    /// </summary>
    /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
    public Dictionary<string, string> ToUnescapedDictionary()
    {
        var to = new Dictionary<string, string>();
        var enumerateAsConcreteDict = (Dictionary<string, string>)this;
        foreach (var entry in enumerateAsConcreteDict)
        {
            to[entry.Key] = entry.Value;
        }

        return to;
    }

    /// <summary>
    /// Parses the specified json.
    /// </summary>
    /// <param name="json">The json.</param>
    /// <returns>JsonObject.</returns>
    public static JsonObject Parse(string json)
    {
        return JsonSerializer.DeserializeFromString<JsonObject>(json);
    }

    /// <summary>
    /// Parses the array.
    /// </summary>
    /// <param name="json">The json.</param>
    /// <returns>JsonArrayObjects.</returns>
    public static JsonArrayObjects ParseArray(string json)
    {
        return JsonArrayObjects.Parse(json);
    }

    /// <summary>
    /// Objects the specified property name.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>JsonObject.</returns>
    public JsonObject Object(string propertyName)
    {
        return this.TryGetValue(propertyName, out var strValue)
                   ? Parse(strValue)
                   : null;
    }

    /// <summary>
    /// Get unescaped string value
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>System.String.</returns>
    public string Child(string key)
    {
        return this.GetValueOrDefault(key);
    }

    /// <summary>
    /// Write JSON Array, Object, bool or number values as raw string
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    public static void WriteValue(TextWriter writer, object value)
    {
        var strValue = value as string;
        if (!string.IsNullOrEmpty(strValue))
        {
            var firstChar = strValue[0];
            var lastChar = strValue[^1];
            if (firstChar == JsWriter.MapStartChar && lastChar == JsWriter.MapEndChar
                || firstChar == JsWriter.ListStartChar && lastChar == JsWriter.ListEndChar
                || JsonUtils.True == strValue
                || JsonUtils.False == strValue
                || IsJavaScriptNumber(strValue))
            {
                writer.Write(strValue);
                return;
            }
        }

        JsonUtils.WriteString(writer, strValue);
    }

    /// <summary>
    /// Determines whether [is java script number] [the specified string value].
    /// </summary>
    /// <param name="strValue">The string value.</param>
    /// <returns><c>true</c> if [is java script number] [the specified string value]; otherwise, <c>false</c>.</returns>
    private static bool IsJavaScriptNumber(string strValue)
    {
        var firstChar = strValue[0];
        if (firstChar == '0')
        {
            if (strValue.Length == 1)
            {
                return true;
            }

            if (!strValue.Contains("."))
            {
                return false;
            }
        }

        if (!strValue.Contains("."))
        {
            if (long.TryParse(strValue, out var longValue))
            {
                return longValue is < JsonUtils.MaxInteger and > JsonUtils.MinInteger;
            }

            return false;
        }

        if (double.TryParse(strValue, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var doubleValue))
        {
            return doubleValue is < JsonUtils.MaxInteger and > JsonUtils.MinInteger;
        }

        return false;
    }

    /// <summary>
    /// Converts to.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>T.</returns>
    public T ConvertTo<T>()
    {
        return (T)this.ConvertTo(typeof(T));
    }

    /// <summary>
    /// Converts to.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>System.Object.</returns>
    public object ConvertTo(Type type)
    {
        var map = new Dictionary<string, object>();

        foreach (var entry in this)
        {
            map[entry.Key] = entry.Value;
        }

        return map.FromObjectDictionary(type);
    }
}

/// <summary>
/// Class JsonArrayObjects.
/// Implements the <see cref="JsonObject" />
/// </summary>
/// <seealso cref="JsonObject" />
public class JsonArrayObjects : List<JsonObject>
{
    /// <summary>
    /// Parses the specified json.
    /// </summary>
    /// <param name="json">The json.</param>
    /// <returns>JsonArrayObjects.</returns>
    public static JsonArrayObjects Parse(string json)
    {
        return JsonSerializer.DeserializeFromString<JsonArrayObjects>(json);
    }
}

/// <summary>
/// Interface IValueWriter
/// </summary>
public interface IValueWriter
{
    /// <summary>
    /// Writes to.
    /// </summary>
    /// <param name="serializer">The serializer.</param>
    /// <param name="writer">The writer.</param>
    void WriteTo(ITypeSerializer serializer, TextWriter writer);
}

/// <summary>
/// Struct JsonValue
/// Implements the <see cref="IValueWriter" />
/// </summary>
/// <seealso cref="IValueWriter" />
public readonly struct JsonValue : IValueWriter
{
    /// <summary>
    /// The json
    /// </summary>
    private readonly string json;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonValue" /> struct.
    /// </summary>
    /// <param name="json">The json.</param>
    public JsonValue(string json)
    {
        this.json = json;
    }

    /// <summary>
    /// As type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>T.</returns>
    public T As<T>()
    {
        return JsonSerializer.DeserializeFromString<T>(this.json);
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public override string ToString()
    {
        return this.json;
    }

    /// <summary>
    /// Writes to.
    /// </summary>
    /// <param name="serializer">The serializer.</param>
    /// <param name="writer">The writer.</param>
    public void WriteTo(ITypeSerializer serializer, TextWriter writer)
    {
        writer.Write(this.json ?? JsonUtils.Null);
    }
}