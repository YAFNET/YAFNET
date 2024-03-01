// ***********************************************************************
// <copyright file="XsdTimeOnlyJsonConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ServiceStack.OrmLite.Base.Text.SystemJson;

/// <summary>
/// Class XsdTimeOnlyJsonConverter.
/// </summary>
public class XsdTimeOnlyJsonConverter : JsonConverter<TimeOnly>
{
    /// <summary>
    /// Reads the specified reader.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">The options.</param>
    /// <returns>System.TimeOnly.</returns>
    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var formatted = reader.GetString()!;
        return TimeOnly.FromTimeSpan(Text.Support.TimeSpanConverter.FromXsdDuration(formatted));
    }

    /// <summary>
    /// Writes the specified writer.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    /// <param name="options">The options.</param>
    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
    {
        var formatted = Text.Support.TimeSpanConverter.ToXsdDuration(value.ToTimeSpan());
        writer.WriteStringValue(formatted);
    }
}