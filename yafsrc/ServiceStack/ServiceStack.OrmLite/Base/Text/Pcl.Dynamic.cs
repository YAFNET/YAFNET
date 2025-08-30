// ***********************************************************************
// <copyright file="Pcl.Dynamic.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

using ServiceStack.OrmLite.Base.Text.Common;
using ServiceStack.OrmLite.Base.Text.Json;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Class DeserializeDynamic.
/// </summary>
/// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
public static class DeserializeDynamic<TSerializer>
    where TSerializer : ITypeSerializer
{
    /// <summary>
    /// The serializer
    /// </summary>
    private readonly static ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

    /// <summary>
    /// The cached parse function
    /// </summary>
    private readonly static ParseStringSpanDelegate CachedParseFn;
    /// <summary>
    /// Initializes static members of the <see cref="DeserializeDynamic{TSerializer}" /> class.
    /// </summary>
    static DeserializeDynamic()
    {
        CachedParseFn = ParseDynamic;
    }

    /// <summary>
    /// Gets the parse.
    /// </summary>
    /// <value>The parse.</value>
    public static ParseStringDelegate Parse => v => CachedParseFn(v.AsSpan());

    /// <summary>
    /// Gets the parse string span.
    /// </summary>
    /// <value>The parse string span.</value>
    public static ParseStringSpanDelegate ParseStringSpan => CachedParseFn;

    /// <summary>
    /// Parses the dynamic.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>IDynamicMetaObjectProvider.</returns>
    public static IDynamicMetaObjectProvider ParseDynamic(ReadOnlySpan<char> value)
    {
        var index = VerifyAndGetStartIndex(value, typeof(ExpandoObject));

        var result = new ExpandoObject();

        if (JsonTypeSerializer.IsEmptyMap(value))
        {
            return result;
        }

        var container = (IDictionary<string, object>)result;

        var tryToParsePrimitiveTypes = JsConfig.TryToParsePrimitiveTypeValues;

        var valueLength = value.Length;
        while (index < valueLength)
        {
            var keyValue = Serializer.EatMapKey(value, ref index);
            Serializer.EatMapKeySeperator(value, ref index);
            var elementValue = Serializer.EatValue(value, ref index);

            var mapKey = Serializer.UnescapeString(keyValue).ToString();

            if (JsonUtils.IsJsObject(elementValue))
            {
                container[mapKey] = ParseDynamic(elementValue);
            }
            else if (JsonUtils.IsJsArray(elementValue))
            {
                container[mapKey] = DeserializeList<List<object>, TSerializer>.ParseStringSpan(elementValue);
            }
            else if (tryToParsePrimitiveTypes)
            {
                container[mapKey] = DeserializeType<TSerializer>.ParsePrimitive(elementValue) ?? Serializer.UnescapeString(elementValue).Value();
            }
            else
            {
                container[mapKey] = Serializer.UnescapeString(elementValue).Value();
            }

            Serializer.EatItemSeperatorOrMapEndChar(value, ref index);
        }

        return result;
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
        if (!Serializer.EatMapStartChar(value, ref index))
        {
            //Don't throw ex because some KeyValueDataContractDeserializer don't have '{}'
            Tracer.Instance.WriteDebug("WARN: Map definitions should start with a '{0}', expecting serialized type '{1}', got string starting with: {2}",
                JsWriter.MapStartChar, createMapType != null ? createMapType.Name : "Dictionary<,>", value.Substring(0, value.Length < 50 ? value.Length : 50));
        }
        return index;
    }
}

/// <summary>
/// Class DynamicJson.
/// Implements the <see cref="System.Dynamic.DynamicObject" />
/// </summary>
/// <seealso cref="System.Dynamic.DynamicObject" />
public class DynamicJson : DynamicObject
{
    /// <summary>
    /// The hash
    /// </summary>
    private readonly IDictionary<string, object> _hash = new Dictionary<string, object>();

    /// <summary>
    /// Serializes the specified instance.
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <returns>System.String.</returns>
    public static string Serialize(dynamic instance)
    {
        var json = JsonSerializer.SerializeToString(instance);
        return json;
    }

    /// <summary>
    /// Deserializes the specified json.
    /// </summary>
    /// <param name="json">The json.</param>
    /// <returns>dynamic.</returns>
    public static dynamic Deserialize(string json)
    {
        // Support arbitrary nesting by using JsonObject
        var deserialized = JsonSerializer.DeserializeFromString<JsonObject>(json);
        var hash = deserialized.ToUnescapedDictionary().ToObjectDictionary();
        return new DynamicJson(hash);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicJson" /> class.
    /// </summary>
    /// <param name="hash">The hash.</param>
    public DynamicJson(IEnumerable<KeyValuePair<string, object>> hash)
    {
        this._hash.Clear();
        foreach (var entry in hash)
        {
            this._hash.Add(Underscored(entry.Key), entry.Value);
        }
    }

    /// <summary>
    /// Provides the implementation for operations that set member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as setting a value for a property.
    /// </summary>
    /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member to which the value is being assigned. For example, for the statement sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
    /// <param name="value">The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, the <paramref name="value" /> is "Test".</param>
    /// <returns><see langword="true" /> if the operation is successful; otherwise, <see langword="false" />. If this method returns <see langword="false" />, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)</returns>
    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
        var name = Underscored(binder.Name);
        this._hash[name] = value;
        return this._hash[name] == value;
    }

    /// <summary>
    /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
    /// </summary>
    /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
    /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result" />.</param>
    /// <returns><see langword="true" /> if the operation is successful; otherwise, <see langword="false" />. If this method returns <see langword="false" />, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)</returns>
    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        var name = Underscored(binder.Name);
        return this.YieldMember(name, out result);
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public override string ToString()
    {
        return JsonSerializer.SerializeToString(this._hash);
    }

    /// <summary>
    /// Yields the member.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    private bool YieldMember(string name, out object result)
    {
        if (this._hash.ContainsKey(name))
        {
            var json = this._hash[name].ToString();
            if (json.TrimStart(' ').StartsWith('{'))
            {
                result = Deserialize(json);
                return true;
            }
            else if (json.TrimStart(' ').StartsWith('['))
            {
                result = JsonArrayObjects.Parse(json).Select(a =>
                    {
                        var hash = a.ToDictionary<KeyValuePair<string, string>, string, object>(entry => entry.Key, entry => entry.Value);
                        return new DynamicJson(hash);
                    }).ToArray();
                return true;
            }
            result = json;
            return this._hash[name] == result;
        }
        result = null;
        return false;
    }

    /// <summary>
    /// Underscoreds the specified pascal case.
    /// </summary>
    /// <param name="pascalCase">The pascal case.</param>
    /// <returns>System.String.</returns>
    static internal string Underscored(string pascalCase)
    {
        return Underscored(pascalCase.ToCharArray());
    }

    /// <summary>
    /// Underscoreds the specified pascal case.
    /// </summary>
    /// <param name="pascalCase">The pascal case.</param>
    /// <returns>System.String.</returns>
    static internal string Underscored(IEnumerable<char> pascalCase)
    {
        var sb = StringBuilderCache.Allocate();
        var i = 0;
        foreach (var c in pascalCase)
        {
            if (char.IsUpper(c) && i > 0)
            {
                sb.Append('_');
            }
            sb.Append(c);
            i++;
        }
        return StringBuilderCache.ReturnAndFree(sb).ToLowerInvariant();
    }
}