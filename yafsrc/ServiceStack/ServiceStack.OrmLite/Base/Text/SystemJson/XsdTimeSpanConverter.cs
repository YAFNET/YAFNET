// ***********************************************************************
// <copyright file="XsdTimeSpanConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ServiceStack.OrmLite.Base.Text.SystemJson;

/// <summary>
/// Class XsdTimeSpanJsonConverter.
/// Implements the <see cref="TimeSpan" />
/// </summary>
/// <seealso cref="TimeSpan" />
public class XsdTimeSpanJsonConverter : JsonConverter<TimeSpan>
{
    /// <summary>
    /// Reads and converts the JSON to type T
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    /// <returns>The converted value.</returns>
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var formatted = reader.GetString()!;
        return Text.Support.TimeSpanConverter.FromXsdDuration(formatted);
    }

    /// <summary>
    /// Writes a specified value as JSON.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The value to convert to JSON.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        var formatted = Text.Support.TimeSpanConverter.ToXsdDuration(value);
        writer.WriteStringValue(formatted);
    }
}