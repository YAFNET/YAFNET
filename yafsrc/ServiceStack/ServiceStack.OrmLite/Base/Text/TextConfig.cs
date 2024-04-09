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

    /// <summary>
    /// Gets the system json option filters.
    /// </summary>
    /// <value>The system json option filters.</value>
    public static List<Action<JsonSerializerOptions>> SystemJsonOptionFilters { get; } = [
        DefaultConfigureSystemJsonOptions
    ];

    /// <summary>
    /// Defaults the configure system json options.
    /// </summary>
    /// <param name="options">The options.</param>
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

    /// <summary>
    /// Applies the system json options.
    /// </summary>
    /// <param name="options">The options.</param>
    public static void ApplySystemJsonOptions(JsonSerializerOptions options)
    {
        foreach (var configure in SystemJsonOptionFilters)
        {
            configure(options);
        }
    }

    /// <summary>
    /// Creates the system json options.
    /// </summary>
    /// <returns>System.Text.Json.JsonSerializerOptions.</returns>
    public static JsonSerializerOptions CreateSystemJsonOptions()
    {
        var to = new JsonSerializerOptions();
        ApplySystemJsonOptions(to);
        return to;
    }

    /// <summary>
    /// Gets or sets the system json options.
    /// </summary>
    /// <value>The system json options.</value>
    public static JsonSerializerOptions SystemJsonOptions { get; set; } = CreateSystemJsonOptions();
}