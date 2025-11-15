// ***********************************************************************
// <copyright file="EnumJsonConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#if NET10_0_OR_GREATER
#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ServiceStack.OrmLite.Base.Text.SystemJson;

/// <summary>
/// Class JsonEnumMemberStringEnumConverter.
/// Implements the <see cref="System.Text.Json.Serialization.JsonConverterFactory" />
/// </summary>
/// <seealso cref="System.Text.Json.Serialization.JsonConverterFactory" />
public class JsonEnumMemberStringEnumConverter(JsonNamingPolicy? namingPolicy = null, bool allowIntegerValues = true)
    : JsonConverterFactory
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonEnumMemberStringEnumConverter"/> class.
    /// </summary>
    public JsonEnumMemberStringEnumConverter() : this(null) { }

    /// <summary>
    /// The string converter
    /// </summary>
    private readonly JsonStringEnumConverter stringConverter = new(namingPolicy, allowIntegerValues);

    /// <summary>
    /// Determines whether this instance can convert the specified type to convert.
    /// </summary>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <returns>bool.</returns>
    public override bool CanConvert(Type typeToConvert)
    {
        return this.stringConverter.CanConvert(typeToConvert)
               && typeToConvert.GetCustomAttribute<FlagsAttribute>() is null;
    }

    /// <summary>
    /// Creates the converter.
    /// </summary>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">The options.</param>
    /// <returns>System.Text.Json.Serialization.JsonConverter.</returns>
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var query = from field in typeToConvert.GetFields(BindingFlags.Public | BindingFlags.Static)
                    let attr = field.GetCustomAttribute<EnumMemberAttribute>()
                    where attr is { Value: not null }
                    select (field.Name, attr.Value);
        var dictionary = query.ToDictionary(p => p.Name, p => p.Value);
        if (dictionary.Count > 0)
        {
            return new JsonStringEnumConverter(
                    new DictionaryLookupNamingPolicy(dictionary, namingPolicy), allowIntegerValues)
                .CreateConverter(typeToConvert, options);
        }

        return this.stringConverter.CreateConverter(typeToConvert, options);
    }
}

/// <summary>
/// Class JsonNamingPolicyDecorator.
/// Implements the <see cref="System.Text.Json.JsonNamingPolicy" />
/// </summary>
/// <seealso cref="System.Text.Json.JsonNamingPolicy" />
public class JsonNamingPolicyDecorator(JsonNamingPolicy? underlyingNamingPolicy) : JsonNamingPolicy
{
    /// <summary>
    /// Converts the name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>string.</returns>
    public override string ConvertName (string name)
    {
        return underlyingNamingPolicy?.ConvertName(name) ?? name;
    }
}

/// <summary>
/// Class DictionaryLookupNamingPolicy.
/// Implements the <see cref="ServiceStack.OrmLite.Base.Text.SystemJson.JsonNamingPolicyDecorator" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Base.Text.SystemJson.JsonNamingPolicyDecorator" />
internal class DictionaryLookupNamingPolicy(Dictionary<string, string> dictionary, JsonNamingPolicy? underlyingNamingPolicy)
    : JsonNamingPolicyDecorator(underlyingNamingPolicy)
{
    /// <summary>
    /// The dictionary
    /// </summary>
    private readonly Dictionary<string, string> dictionary = dictionary ?? throw new ArgumentNullException();

    /// <summary>
    /// Converts the name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>string.</returns>
    public override string ConvertName (string name)
    {
        return this.dictionary.TryGetValue(name, out var value) ? value : base.ConvertName(name);
    }
}

#endif
