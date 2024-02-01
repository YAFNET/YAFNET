// ***********************************************************************
// <copyright file="TextConfig.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Security.Cryptography;
using System.Text.Json;

using ServiceStack.Text.SystemJson;

namespace ServiceStack.Text;

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
    public static Config SystemJsonTextConfig { get; set; } = new()
    {
        TextCase = TextCase.CamelCase,
        SystemJsonCompatible = true
    };

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static JsonSerializerOptions DefaultSystemJsonOptions()
    {
        return new JsonSerializerOptions {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = {
                new JsonEnumMemberStringEnumConverter(),
                new XsdTimeSpanJsonConverter(),
                new XsdTimeOnlyJsonConverter(),
            },
            TypeInfoResolver = DataContractResolver.Instance,
        };
    }

    public static JsonSerializerOptions SystemJsonOptions { get; set; } = DefaultSystemJsonOptions();

    public static JsonSerializerOptions CustomSystemJsonOptions(
        JsonSerializerOptions systemJsonOptions, JsConfigScope jsScope)
    {
        var to = new JsonSerializerOptions(systemJsonOptions)
        {
            PropertyNamingPolicy = jsScope.TextCase switch
            {
                TextCase.CamelCase => JsonNamingPolicy.CamelCase,
                TextCase.SnakeCase => JsonNamingPolicy.SnakeCaseLower,
                _ => null
            }
        };

        if (jsScope.ExcludeDefaultValues)
        {
            to.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;
        }

        if (jsScope.IncludeNullValues)
        {
            to.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never;
        }

        if (jsScope.Indent)
        {
            to.WriteIndented = true;
        }

        return to;
    }
#endif
}
