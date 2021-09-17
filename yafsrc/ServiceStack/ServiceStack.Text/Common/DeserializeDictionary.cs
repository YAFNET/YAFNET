// ***********************************************************************
// <copyright file="DeserializeDictionary.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace ServiceStack.Text.Common
{
    /// <summary>
    /// Class DeserializeDictionary.
    /// </summary>
    /// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
    public static class DeserializeDictionary<TSerializer>
        where TSerializer : ITypeSerializer
    {
        /// <summary>
        /// The serializer
        /// </summary>
        private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

        /// <summary>
        /// The key index
        /// </summary>
        private const int KeyIndex = 0;

        /// <summary>
        /// The value index
        /// </summary>
        private const int ValueIndex = 1;

        /// <summary>
        /// Gets the parse method.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>ParseStringDelegate.</returns>
        public static ParseStringDelegate GetParseMethod(Type type) => v => GetParseStringSpanMethod(type)(v.AsSpan());

        /// <summary>
        /// Gets the parse string span method.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>ParseStringSpanDelegate.</returns>
        public static ParseStringSpanDelegate GetParseStringSpanMethod(Type type)
        {
            var mapInterface = type.GetTypeWithGenericInterfaceOf(typeof(IDictionary<,>));
            if (mapInterface == null)
            {
                var fn = PclExport.Instance.GetDictionaryParseStringSpanMethod<TSerializer>(type);
                if (fn != null)
                    return fn;

                if (type == typeof(IDictionary))
                {
                    return GetParseStringSpanMethod(typeof(Dictionary<object, object>));
                }
                if (typeof(IDictionary).IsAssignableFrom(type))
                {
                    return s => ParseIDictionary(s, type);
                }

                throw new ArgumentException($"Type {type.FullName} is not of type IDictionary<,>");
            }

            //optimized access for regularly used types
            if (type == typeof(Dictionary<string, string>))
                return ParseStringDictionary;
            if (type == typeof(JsonObject))
                return ParseJsonObject;
            if (typeof(JsonObject).IsAssignableFrom(type))
            {
                var method = typeof(DeserializeDictionary<TSerializer>).GetMethod("ParseInheritedJsonObject");
                method = method.MakeGenericMethod(type);
                return Delegate.CreateDelegate(typeof(ParseStringSpanDelegate), method) as ParseStringSpanDelegate;
            }

            var dictionaryArgs = mapInterface.GetGenericArguments();
            var keyTypeParseMethod = Serializer.GetParseStringSpanFn(dictionaryArgs[KeyIndex]);
            if (keyTypeParseMethod == null) return null;

            var valueTypeParseMethod = Serializer.GetParseStringSpanFn(dictionaryArgs[ValueIndex]);
            if (valueTypeParseMethod == null) return null;

            var createMapType = type.HasAnyTypeDefinitionsOf(typeof(Dictionary<,>), typeof(IDictionary<,>))
                ? null : type;

            return value => ParseDictionaryType(value, createMapType, dictionaryArgs, keyTypeParseMethod, valueTypeParseMethod);
        }

        /// <summary>
        /// Parses the json object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>JsonObject.</returns>
        public static JsonObject ParseJsonObject(string value) => ParseJsonObject(value.AsSpan());

        /// <summary>
        /// Parses the inherited json object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        public static T ParseInheritedJsonObject<T>(ReadOnlySpan<char> value) where T : JsonObject, new()
        {
            if (value.Length == 0)
                return null;

            var index = VerifyAndGetStartIndex(value, typeof(T));

            var result = new T();

            if (Json.JsonTypeSerializer.IsEmptyMap(value, index)) return result;

            var valueLength = value.Length;
            while (index < valueLength)
            {
                var keyValue = Serializer.EatMapKey(value, ref index);
                Serializer.EatMapKeySeperator(value, ref index);
                var elementValue = Serializer.EatValue(value, ref index);
                if (keyValue.IsEmpty) continue;

                var mapKey = keyValue.ToString();
                var mapValue = elementValue.Value();

                result[mapKey] = mapValue;

                Serializer.EatItemSeperatorOrMapEndChar(value, ref index);
            }

            return result;
        }

        /// <summary>
        /// Parses the json object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>JsonObject.</returns>
        public static JsonObject ParseJsonObject(ReadOnlySpan<char> value)
        {
            if (value.Length == 0)
                return null;

            var index = VerifyAndGetStartIndex(value, typeof(JsonObject));

            var result = new JsonObject();

            if (Json.JsonTypeSerializer.IsEmptyMap(value, index)) return result;

            var valueLength = value.Length;
            while (index < valueLength)
            {
                var keyValue = Serializer.EatMapKey(value, ref index);
                Serializer.EatMapKeySeperator(value, ref index);
                var elementValue = Serializer.EatValue(value, ref index);
                if (keyValue.IsEmpty) continue;

                var mapKey = keyValue.ToString();
                var mapValue = elementValue.Value();

                result[mapKey] = mapValue;

                Serializer.EatItemSeperatorOrMapEndChar(value, ref index);
            }

            return result;
        }

        /// <summary>
        /// Parses the string dictionary.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        public static Dictionary<string, string> ParseStringDictionary(string value) => ParseStringDictionary(value.AsSpan());

        /// <summary>
        /// Parses the string dictionary.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        public static Dictionary<string, string> ParseStringDictionary(ReadOnlySpan<char> value)
        {
            if (value.IsEmpty)
                return null;

            var index = VerifyAndGetStartIndex(value, typeof(Dictionary<string, string>));

            var result = new Dictionary<string, string>();

            if (Json.JsonTypeSerializer.IsEmptyMap(value, index)) return result;

            var valueLength = value.Length;
            while (index < valueLength)
            {
                var keyValue = Serializer.EatMapKey(value, ref index);
                Serializer.EatMapKeySeperator(value, ref index);
                var elementValue = Serializer.EatValue(value, ref index);
                if (keyValue.IsEmpty) continue;

                var mapKey = Serializer.UnescapeString(keyValue);
                var mapValue = Serializer.UnescapeString(elementValue);

                result[mapKey.ToString()] = mapValue.Value();

                Serializer.EatItemSeperatorOrMapEndChar(value, ref index);
            }

            return result;
        }

        /// <summary>
        /// Parses the i dictionary.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="dictType">Type of the dictionary.</param>
        /// <returns>IDictionary.</returns>
        public static IDictionary ParseIDictionary(string value, Type dictType) => ParseIDictionary(value.AsSpan(), dictType);

        /// <summary>
        /// Parses the i dictionary.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="dictType">Type of the dictionary.</param>
        /// <returns>IDictionary.</returns>
        public static IDictionary ParseIDictionary(ReadOnlySpan<char> value, Type dictType)
        {
            if (value.IsEmpty) return null;

            var index = VerifyAndGetStartIndex(value, dictType);

            var valueParseMethod = Serializer.GetParseStringSpanFn(typeof(object));
            if (valueParseMethod == null) return null;

            var to = (IDictionary)dictType.CreateInstance();

            if (Json.JsonTypeSerializer.IsEmptyMap(value, index)) return to;

            var valueLength = value.Length;
            while (index < valueLength)
            {
                var keyValue = Serializer.EatMapKey(value, ref index);
                Serializer.EatMapKeySeperator(value, ref index);
                var elementStartIndex = index;
                var elementValue = Serializer.EatTypeValue(value, ref index);
                if (keyValue.IsEmpty) continue;

                var mapKey = valueParseMethod(keyValue);

                if (elementStartIndex < valueLength)
                {
                    Serializer.EatWhitespace(value, ref elementStartIndex);
                    to[mapKey] = DeserializeType<TSerializer>.ParsePrimitive(elementValue.Value(), value[elementStartIndex]);
                }
                else
                {
                    to[mapKey] = valueParseMethod(elementValue);
                }

                Serializer.EatItemSeperatorOrMapEndChar(value, ref index);
            }

            return to;
        }

        /// <summary>
        /// Parses the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="createMapType">Type of the create map.</param>
        /// <param name="parseKeyFn">The parse key function.</param>
        /// <param name="parseValueFn">The parse value function.</param>
        /// <returns>IDictionary&lt;TKey, TValue&gt;.</returns>
        public static IDictionary<TKey, TValue> ParseDictionary<TKey, TValue>(
            string value, Type createMapType,
            ParseStringDelegate parseKeyFn, ParseStringDelegate parseValueFn)
        {
            return ParseDictionary<TKey, TValue>(value.AsSpan(),
                createMapType,
                v => parseKeyFn(v.ToString()),
                v => parseValueFn(v.ToString())
                );
        }


        /// <summary>
        /// Parses the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="createMapType">Type of the create map.</param>
        /// <param name="parseKeyFn">The parse key function.</param>
        /// <param name="parseValueFn">The parse value function.</param>
        /// <returns>IDictionary&lt;TKey, TValue&gt;.</returns>
        public static IDictionary<TKey, TValue> ParseDictionary<TKey, TValue>(
            ReadOnlySpan<char> value, Type createMapType,
            ParseStringSpanDelegate parseKeyFn, ParseStringSpanDelegate parseValueFn)
        {
            if (value.IsEmpty) return null;

            var to = createMapType == null
                ? new Dictionary<TKey, TValue>()
                : (IDictionary<TKey, TValue>)createMapType.CreateInstance();

            var objDeserializer = Json.JsonTypeSerializer.Instance.ObjectDeserializer;
            if (to is Dictionary<string, object> && objDeserializer != null && typeof(TSerializer) == typeof(Json.JsonTypeSerializer))
                return (IDictionary<TKey, TValue>)objDeserializer(value);

            var config = JsConfig.GetConfig();

            var tryToParseItemsAsDictionaries =
                config.ConvertObjectTypesIntoStringDictionary && typeof(TValue) == typeof(object);
            var tryToParseItemsAsPrimitiveTypes =
                config.TryToParsePrimitiveTypeValues && typeof(TValue) == typeof(object);

            var index = VerifyAndGetStartIndex(value, createMapType);

            if (Json.JsonTypeSerializer.IsEmptyMap(value, index)) return to;

            var valueLength = value.Length;
            while (index < valueLength)
            {
                var keyValue = Serializer.EatMapKey(value, ref index);
                Serializer.EatMapKeySeperator(value, ref index);
                var elementStartIndex = index;
                var elementValue = Serializer.EatTypeValue(value, ref index);
                if (keyValue.IsNullOrEmpty()) continue;

                TKey mapKey = (TKey)parseKeyFn(keyValue);

                if (tryToParseItemsAsDictionaries)
                {
                    Serializer.EatWhitespace(value, ref elementStartIndex);
                    if (elementStartIndex < valueLength && value[elementStartIndex] == JsWriter.MapStartChar)
                    {
                        var tmpMap = ParseDictionary<TKey, TValue>(elementValue, createMapType, parseKeyFn, parseValueFn);
                        if (tmpMap != null && tmpMap.Count > 0)
                        {
                            to[mapKey] = (TValue)tmpMap;
                        }
                    }
                    else if (elementStartIndex < valueLength && value[elementStartIndex] == JsWriter.ListStartChar)
                    {
                        to[mapKey] = (TValue)DeserializeList<List<object>, TSerializer>.ParseStringSpan(elementValue);
                    }
                    else
                    {
                        to[mapKey] = (TValue)(tryToParseItemsAsPrimitiveTypes && elementStartIndex < valueLength
                            ? DeserializeType<TSerializer>.ParsePrimitive(elementValue.Value(), value[elementStartIndex])
                            : parseValueFn(elementValue).Value());
                    }
                }
                else
                {
                    if (tryToParseItemsAsPrimitiveTypes && elementStartIndex < valueLength)
                    {
                        Serializer.EatWhitespace(value, ref elementStartIndex);
                        to[mapKey] = (TValue)DeserializeType<TSerializer>.ParsePrimitive(elementValue.Value(), value[elementStartIndex]);
                    }
                    else
                    {
                        to[mapKey] = (TValue)parseValueFn(elementValue).Value();
                    }
                }

                Serializer.EatItemSeperatorOrMapEndChar(value, ref index);
            }

            return to;
        }

        /// <summary>
        /// Verifies the start index of the and get.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="createMapType">Type of the create map.</param>
        /// <returns>System.Int32.</returns>
        private static int VerifyAndGetStartIndex(ReadOnlySpan<char> value, Type createMapType)
        {
            var index = 0;
            if (value.Length > 0 && !Serializer.EatMapStartChar(value, ref index))
            {
                //Don't throw ex because some KeyValueDataContractDeserializer don't have '{}'
                Tracer.Instance.WriteDebug("WARN: Map definitions should start with a '{0}', expecting serialized type '{1}', got string starting with: {2}",
                    JsWriter.MapStartChar, createMapType != null ? createMapType.Name : "Dictionary<,>", value.Substring(0, value.Length < 50 ? value.Length : 50));
            }
            return index;
        }

        /// <summary>
        /// The parse delegate cache
        /// </summary>
        private static Dictionary<TypesKey, ParseDictionaryDelegate> ParseDelegateCache
            = new();

        /// <summary>
        /// Delegate ParseDictionaryDelegate
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="createMapType">Type of the create map.</param>
        /// <param name="keyParseFn">The key parse function.</param>
        /// <param name="valueParseFn">The value parse function.</param>
        /// <returns>System.Object.</returns>
        private delegate object ParseDictionaryDelegate(ReadOnlySpan<char> value, Type createMapType,
            ParseStringSpanDelegate keyParseFn, ParseStringSpanDelegate valueParseFn);

        /// <summary>
        /// Parses the type of the dictionary.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="createMapType">Type of the create map.</param>
        /// <param name="argTypes">The argument types.</param>
        /// <param name="keyParseFn">The key parse function.</param>
        /// <param name="valueParseFn">The value parse function.</param>
        /// <returns>System.Object.</returns>
        public static object ParseDictionaryType(string value, Type createMapType, Type[] argTypes,
            ParseStringDelegate keyParseFn, ParseStringDelegate valueParseFn) =>
            ParseDictionaryType(value.AsSpan(), createMapType, argTypes,
                v => keyParseFn(v.ToString()), v => valueParseFn(v.ToString()));

        /// <summary>
        /// The signature
        /// </summary>
        private static readonly Type[] signature = { typeof(ReadOnlySpan<char>), typeof(Type), typeof(ParseStringSpanDelegate), typeof(ParseStringSpanDelegate) };

        /// <summary>
        /// Parses the type of the dictionary.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="createMapType">Type of the create map.</param>
        /// <param name="argTypes">The argument types.</param>
        /// <param name="keyParseFn">The key parse function.</param>
        /// <param name="valueParseFn">The value parse function.</param>
        /// <returns>System.Object.</returns>
        public static object ParseDictionaryType(ReadOnlySpan<char> value, Type createMapType, Type[] argTypes,
            ParseStringSpanDelegate keyParseFn, ParseStringSpanDelegate valueParseFn)
        {
            var key = new TypesKey(argTypes[0], argTypes[1]);
            if (ParseDelegateCache.TryGetValue(key, out var parseDelegate))
                return parseDelegate(value, createMapType, keyParseFn, valueParseFn);

            var mi = typeof(DeserializeDictionary<TSerializer>).GetStaticMethod("ParseDictionary", signature);
            var genericMi = mi.MakeGenericMethod(argTypes);
            parseDelegate = (ParseDictionaryDelegate)genericMi.MakeDelegate(typeof(ParseDictionaryDelegate));

            Dictionary<TypesKey, ParseDictionaryDelegate> snapshot, newCache;
            do
            {
                snapshot = ParseDelegateCache;
                newCache = new Dictionary<TypesKey, ParseDictionaryDelegate>(ParseDelegateCache)
                {
                    [key] = parseDelegate
                };

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref ParseDelegateCache, newCache, snapshot), snapshot));

            return parseDelegate(value, createMapType, keyParseFn, valueParseFn);
        }

        /// <summary>
        /// Struct TypesKey
        /// </summary>
        private struct TypesKey
        {
            /// <summary>
            /// Gets the type1.
            /// </summary>
            /// <value>The type1.</value>
            private Type Type1 { get; }
            /// <summary>
            /// Gets the type2.
            /// </summary>
            /// <value>The type2.</value>
            private Type Type2 { get; }

            /// <summary>
            /// The hashcode
            /// </summary>
            private readonly int hashcode;

            /// <summary>
            /// Initializes a new instance of the <see cref="TypesKey" /> struct.
            /// </summary>
            /// <param name="type1">The type1.</param>
            /// <param name="type2">The type2.</param>
            public TypesKey(Type type1, Type type2)
            {
                Type1 = type1;
                Type2 = type2;
                unchecked
                {
                    hashcode = Type1.GetHashCode() ^ (37 * Type2.GetHashCode());
                }
            }

            /// <summary>
            /// Determines whether the specified <see cref="object" /> is equal to this instance.
            /// </summary>
            /// <param name="obj">The object to compare with the current instance.</param>
            /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
            public override bool Equals(object obj)
            {
                var types = (TypesKey)obj;

                return Type1 == types.Type1 && Type2 == types.Type2;
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
            public override int GetHashCode() => hashcode;
        }
    }
}