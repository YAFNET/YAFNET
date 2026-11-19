// ***********************************************************************
// <copyright file="DataContractResolver.cs" company="ServiceStack, Inc.">
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
using System.Text.Json.Serialization.Metadata;

namespace ServiceStack.OrmLite.Base.Text.SystemJson;

/// <summary>
/// Class DataContractResolver.
/// https://github.com/dotnet/runtime/blob/v8.0.1/src/libraries/System.Text.Json/tests/System.Text.Json.Tests/Serialization/TypeInfoResolverFunctionalTests.cs#L671
/// Implements the <see cref="System.Text.Json.Serialization.Metadata.DefaultJsonTypeInfoResolver" />
/// </summary>
/// <seealso cref="System.Text.Json.Serialization.Metadata.DefaultJsonTypeInfoResolver" />
public class DataContractResolver : DefaultJsonTypeInfoResolver
{
    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static DataContractResolver Instance { get; } = new();

    /// <summary>
    /// Gets the naming policy converters.
    /// </summary>
    /// <value>The naming policy converters.</value>
    public Dictionary<JsonNamingPolicy, Func<string, string>> NamingPolicyConverters { get; } = new()
    {
        [JsonNamingPolicy.CamelCase] = s => s.ToCamelCase(),
        [JsonNamingPolicy.SnakeCaseLower] = s => s.ToLowercaseUnderscore(),
        [JsonNamingPolicy.SnakeCaseUpper] = s => s.ToUppercaseUnderscore(),
        [JsonNamingPolicy.KebabCaseLower] = s => s.ToKebabCase(),
        [JsonNamingPolicy.KebabCaseUpper] = s => s.ToKebabCase().ToUpper()
    };

    /// <summary>
    /// Gets the type information.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="options">The options.</param>
    /// <returns>System.Text.Json.Serialization.Metadata.JsonTypeInfo.</returns>
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var jsonTypeInfo = base.GetTypeInfo(type, options);

        if (jsonTypeInfo.Kind == JsonTypeInfoKind.Object)
        {
            var propInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var hasDataContract = type.GetCustomAttribute<DataContractAttribute>() is not null;
            var propInfosToIgnore = propInfos.Where(x => x.GetCustomAttribute<IgnoreDataMemberAttribute>() is not null)
                .Select(x => x.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (hasDataContract)
            {
                jsonTypeInfo.Properties.Clear();
                foreach (var propInfo in propInfos)
                {
                    if (propInfo.GetCustomAttribute<IgnoreDataMemberAttribute>() is not null)
                    {
                        continue;
                    }

                    var attr = propInfo.GetCustomAttribute<DataMemberAttribute>();
                    var name = options.PropertyNamingPolicy == JsonNamingPolicy.CamelCase
                        ? propInfo.Name.ToCamelCase()
                        : (options.PropertyNamingPolicy == JsonNamingPolicy.SnakeCaseLower ||
                           options.PropertyNamingPolicy == JsonNamingPolicy.SnakeCaseUpper)
                            ? propInfo.Name.ToLowercaseUnderscore()
                            : propInfo.Name;

                    if (attr?.Name != null)
                    {
                        name = attr.Name;
                    }

                    var jsonPropertyInfo = jsonTypeInfo.CreateJsonPropertyInfo(propInfo.PropertyType, name);
                    jsonPropertyInfo.Order = attr?.Order ?? 0;
                    jsonPropertyInfo.Get =
                        propInfo.CanRead
                            ? propInfo.GetValue
                            : null;

                    jsonPropertyInfo.Set = propInfo.CanWrite
                        ? propInfo.SetValue
                        : null;

                    jsonTypeInfo.Properties.Add(jsonPropertyInfo);
                }
            }
            else if (propInfosToIgnore.Count > 0)
            {
                var propsToRemove = jsonTypeInfo.Properties
                    .Where(jsonPropertyInfo => propInfosToIgnore.Contains(jsonPropertyInfo.Name)).ToList();
                foreach (var jsonPropertyInfo in propsToRemove)
                {
                    jsonTypeInfo.Properties.Remove(jsonPropertyInfo);
                }
            }
        }

        return jsonTypeInfo;
    }
}

#endif