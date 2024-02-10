// ***********************************************************************
// <copyright file="TextConfig.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

using ServiceStack.OrmLite.Base.Text.SystemJson;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Class TextConfig.
/// </summary>
public class TextConfig
{
    /// <summary>
    /// Gets or sets the create sha.
    /// </summary>
    /// <value>The create sha.</value>
    public static Func<SHA1> CreateSha { get; set; } = SHA1.Create;

#if NET8_0_OR_GREATER
    /// <summary>
    /// Config scope of ServiceStack.Text when System.Text.Json is enabled
    /// </summary>
    public static Config SystemJsonTextConfig { get; set; } = new() {
        TextCase = TextCase.CamelCase,
        SystemJsonCompatible = true
    };

    public static void ConfigureJsonOptions(Action<JsonSerializerOptions> configure)
    {
        SystemJsonOptionFilters.Add(configure);
        SystemJsonOptions = CreateSystemJsonOptions();
    }

    public static List<Action<JsonSerializerOptions>> SystemJsonOptionFilters { get; } = [
        DefaultConfigureSystemJsonOptions,
    ];

    public static void DefaultConfigureSystemJsonOptions(JsonSerializerOptions options)
    {
        options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        options.PropertyNameCaseInsensitive = true;
        options.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.Converters.Add(new JsonEnumMemberStringEnumConverter());
        options.Converters.Add(new XsdTimeSpanJsonConverter());
        options.Converters.Add(new XsdTimeOnlyJsonConverter());
        options.TypeInfoResolver = SystemJson.DataContractResolver.Instance;
    }

    public static void ApplySystemJsonOptions(JsonSerializerOptions options)
    {
        foreach (var configure in SystemJsonOptionFilters)
        {
            configure(options);
        }
    }

    public static JsonSerializerOptions CreateSystemJsonOptions()
    {
        var to = new JsonSerializerOptions();
        ApplySystemJsonOptions(to);
        return to;
    }

    public static JsonSerializerOptions SystemJsonOptions { get; set; } = CreateSystemJsonOptions();

    public static JsonSerializerOptions CustomSystemJsonOptions(JsonSerializerOptions systemJsonOptions,
        JsConfigScope jsScope)
    {
        var to = new JsonSerializerOptions(systemJsonOptions) {
            PropertyNamingPolicy = jsScope.TextCase switch {
                TextCase.CamelCase => JsonNamingPolicy.CamelCase,
                TextCase.SnakeCase => JsonNamingPolicy.SnakeCaseLower,
                _ => null
            }
        };
        if (jsScope.ExcludeDefaultValues)
        {
            to.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        }

        if (jsScope.IncludeNullValues)
        {
            to.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
        }

        if (jsScope.Indent)
        {
            to.WriteIndented = true;
        }

        return to;
    }
#endif
}