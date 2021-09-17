// ***********************************************************************
// <copyright file="TypeSerializer.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Text.Common;
using ServiceStack.Text.Jsv;
using ServiceStack.Text.Pools;

namespace ServiceStack.Text
{
    /// <summary>
    /// Creates an instance of a Type from a string value
    /// </summary>
    public static class TypeSerializer
    {
        /// <summary>
        /// Initializes static members of the <see cref="TypeSerializer"/> class.
        /// </summary>
        static TypeSerializer()
        {
            JsConfig.InitStatics();
        }

        /// <summary>
        /// Gets or sets the ut f8 encoding.
        /// </summary>
        /// <value>The ut f8 encoding.</value>
        [Obsolete("Use JsConfig.UTF8Encoding")]
        public static UTF8Encoding UTF8Encoding
        {
            get => JsConfig.UTF8Encoding;
            set => JsConfig.UTF8Encoding = value;
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
            if (string.IsNullOrEmpty(value)) return default(T);
            return (T)JsvReader<T>.Parse(value);
        }

        /// <summary>
        /// Deserializes from span.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        public static T DeserializeFromSpan<T>(ReadOnlySpan<char> value)
        {
            if (value.IsEmpty) return default(T);
            return (T)JsvReader<T>.Parse(value);
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
            if (value == null) return null;
            if (value is string str)
                return str.EncodeJsv();

            OnSerialize?.Invoke(value);
            var writer = StringWriterThreadStatic.Allocate();
            JsvWriter.GetWriteFn(type)(writer, value);
            return StringWriterThreadStatic.ReturnAndFree(writer);
        }

        /// <summary>
        /// Serializes to writer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        public static void SerializeToWriter<T>(T value, TextWriter writer)
        {
            if (value == null) return;
            if (value is string str)
            {
                writer.Write(str.EncodeJsv());
            }
            else if (typeof(T) == typeof(object))
            {
                SerializeToWriter(value, value.GetType(), writer);
            }
            else if (typeof(T).IsAbstract || typeof(T).IsInterface)
            {
                JsState.IsWritingDynamic = false;
                SerializeToWriter(value, value.GetType(), writer);
                JsState.IsWritingDynamic = true;
            }
            else
            {
                JsvWriter<T>.WriteRootObject(writer, value);
            }
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
            if (value is string str)
            {
                writer.Write(str.EncodeJsv());
                return;
            }

            OnSerialize?.Invoke(value);
            JsvWriter.GetWriteFn(type)(writer, value);
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
                JsState.IsWritingDynamic = false;
                SerializeToStream(value, value.GetType(), stream);
                JsState.IsWritingDynamic = true;
            }
            else
            {
                var writer = new StreamWriter(stream, JsConfig.UTF8Encoding);
                JsvWriter<T>.WriteRootObject(writer, value);
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
            var writer = new StreamWriter(stream, JsConfig.UTF8Encoding);
            JsvWriter.GetWriteFn(type)(writer, value);
            writer.Flush();
        }

        /// <summary>
        /// Clones the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        public static T Clone<T>(T value)
        {
            var serializedValue = SerializeToString(value);
            var cloneObj = DeserializeFromString<T>(serializedValue);
            return cloneObj;
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
        /// Deserializes from stream asynchronous.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public static Task<object> DeserializeFromStreamAsync(Type type, Stream stream)
        {
            return MemoryProvider.Instance.DeserializeAsync(stream, type, DeserializeFromSpan);
        }

        /// <summary>
        /// Deserialize from stream as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream">The stream.</param>
        /// <returns>A Task&lt;T&gt; representing the asynchronous operation.</returns>
        public static async Task<T> DeserializeFromStreamAsync<T>(Stream stream)
        {
            var obj = await MemoryProvider.Instance.DeserializeAsync(stream, typeof(T), DeserializeFromSpan).ConfigAwait();
            return (T)obj;
        }

        /// <summary>
        /// Useful extension method to get the Dictionary[string,string] representation of any POCO type.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        public static Dictionary<string, string> ToStringDictionary(this object obj)
        {
            if (obj == null)
                return new Dictionary<string, string>();

            if (obj is Dictionary<string, string> strDictionary)
                return strDictionary;

            if (obj is IEnumerable<KeyValuePair<string, string>> kvpStrings)
            {
                var to = new Dictionary<string, string>();
                foreach (var kvp in kvpStrings)
                {
                    to[kvp.Key] = kvp.Value;
                }
                return to;
            }

            if (obj is IEnumerable<KeyValuePair<string, object>> kvps)
                return PlatformExtensions.ToStringDictionary(kvps);

            if (obj is NameValueCollection nvc)
            {
                var to = new Dictionary<string, string>();
                for (var i = 0; i < nvc.Count; i++)
                {
                    to[nvc.GetKey(i)] = nvc.Get(i);
                }
                return to;
            }

            var jsv = SerializeToString(obj);
            var map = DeserializeFromString<Dictionary<string, string>>(jsv);
            return map;
        }

        /// <summary>
        /// Recursively prints the contents of any POCO object in a human-friendly, readable format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns>System.String.</returns>
        public static string Dump<T>(this T instance)
        {
            return SerializeAndFormat(instance);
        }

        /// <summary>
        /// Print Dump to Console.WriteLine
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        public static void PrintDump<T>(this T instance)
        {
            PclExport.Instance.WriteLine(SerializeAndFormat(instance));
        }

        /// <summary>
        /// Print string.Format to Console.WriteLine
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="args">The arguments.</param>
        public static void Print(this string text, params object[] args)
        {
            if (args.Length > 0)
                PclExport.Instance.WriteLine(text, args);
            else
                PclExport.Instance.WriteLine(text);
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
        /// Serializes the and format.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns>System.String.</returns>
        public static string SerializeAndFormat<T>(this T instance)
        {
            if (instance is Delegate fn)
                return Dump(fn);

            var dtoStr = !HasCircularReferences(instance)
                ? SerializeToString(instance)
                : SerializeToString(instance.ToSafePartialObjectDictionary());
            var formatStr = JsvFormatter.Format(dtoStr);
            return formatStr;
        }

        /// <summary>
        /// Dumps the specified function.
        /// </summary>
        /// <param name="fn">The function.</param>
        /// <returns>System.String.</returns>
        public static string Dump(this Delegate fn)
        {
            var method = fn.GetType().GetMethod("Invoke");
            var sb = StringBuilderThreadStatic.Allocate();
            foreach (var param in method.GetParameters())
            {
                if (sb.Length > 0)
                    sb.Append(", ");

                sb.AppendFormat("{0} {1}", param.ParameterType.Name, param.Name);
            }

            var methodName = fn.Method.Name;
            var info = $"{method.ReturnType.Name} {methodName}({StringBuilderThreadStatic.ReturnAndFree(sb)})";
            return info;
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

            if (type == null || !type.IsClass || value is string || value is Type)
                return false;

            if (parentValues == null)
            {
                parentValues = new Stack<object>();
                parentValues.Push(value);
            }

            bool CheckValue(object key)
            {
                if (parentValues.Contains(key))
                    return true;

                parentValues.Push(key);

                if (HasCircularReferences(key, parentValues))
                    return true;

                parentValues.Pop();
                return false;
            }

            if (value is IEnumerable valueEnumerable)
            {
                foreach (var item in valueEnumerable)
                {
                    if (item == null)
                        continue;

                    var itemType = item.GetType();
                    if (itemType.IsGenericType && itemType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                    {
                        var props = TypeProperties.Get(itemType);
                        var key = props.GetPublicGetter("Key")(item);

                        if (CheckValue(key))
                            return true;

                        var val = props.GetPublicGetter("Value")(item);

                        if (CheckValue(val))
                            return true;
                    }

                    if (CheckValue(item))
                        return true;
                }
            }
            else
            {
                var props = type.GetSerializableProperties();

                foreach (var pi in props)
                {
                    if (pi.GetIndexParameters().Length > 0)
                        continue;

                    var mi = pi.GetGetMethod(nonPublic: false);
                    var pValue = mi != null ? mi.Invoke(value, null) : null;
                    if (pValue == null)
                        continue;

                    if (CheckValue(pValue))
                        return true;
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
            for (var i = 0; i < count; i++) fn();
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
                            escaped = !escaped;
                        if (!escaped)
                            quoted = !quoted;
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
                            sb.Append(" ");
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
    /// Implements the <see cref="ServiceStack.Text.IStringSerializer" />
    /// </summary>
    /// <seealso cref="ServiceStack.Text.IStringSerializer" />
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
        public string SerializeToString<TFrom>(TFrom @from)
        {
            return TypeSerializer.SerializeToString(@from);
        }
    }
}