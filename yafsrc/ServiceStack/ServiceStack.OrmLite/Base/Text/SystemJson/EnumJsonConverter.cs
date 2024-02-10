// ***********************************************************************
// <copyright file="EnumJsonConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#if NET6_0_OR_GREATER
#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ServiceStack.OrmLite.Base.Text.SystemJson;

public class JsonEnumMemberStringEnumConverter(JsonNamingPolicy? namingPolicy = null, bool allowIntegerValues = true)
    : JsonConverterFactory
{
    public JsonEnumMemberStringEnumConverter() : this(null, true) { }

    private readonly JsonStringEnumConverter stringConverter = new(namingPolicy, allowIntegerValues);

    public override bool CanConvert(Type typeToConvert) => this.stringConverter.CanConvert(typeToConvert)
        && typeToConvert.GetCustomAttribute<FlagsAttribute>() is null;

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var query = from field in typeToConvert.GetFields(BindingFlags.Public | BindingFlags.Static)
                    let attr = field.GetCustomAttribute<EnumMemberAttribute>()
                    where attr is { Value: not null }
                    select (field.Name, attr.Value);
        var dictionary = query.ToDictionary(p => p.Item1, p => p.Item2);
        if (dictionary.Count > 0)
            return new JsonStringEnumConverter(
                new DictionaryLookupNamingPolicy(dictionary, namingPolicy), allowIntegerValues)
                .CreateConverter(typeToConvert, options);
        
        return this.stringConverter.CreateConverter(typeToConvert, options);
    }
}

public class JsonNamingPolicyDecorator(JsonNamingPolicy? underlyingNamingPolicy) : JsonNamingPolicy
{
    public override string ConvertName (string name) => underlyingNamingPolicy?.ConvertName(name) ?? name;
}

internal class DictionaryLookupNamingPolicy(Dictionary<string, string> dictionary, JsonNamingPolicy? underlyingNamingPolicy)
    : JsonNamingPolicyDecorator(underlyingNamingPolicy)
{
    readonly Dictionary<string, string> dictionary = dictionary ?? throw new ArgumentNullException();

    public override string ConvertName (string name) => 
        this.dictionary.TryGetValue(name, out var value) ? value : base.ConvertName(name);
}

#endif
