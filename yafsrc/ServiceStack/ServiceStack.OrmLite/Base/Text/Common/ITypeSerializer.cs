// ***********************************************************************
// <copyright file="ITypeSerializer.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.IO;

using ServiceStack.OrmLite.Base.Text.Json;

namespace ServiceStack.OrmLite.Base.Text.Common;

/// <summary>
/// Delegate ObjectDeserializerDelegate
/// </summary>
/// <param name="value">The value.</param>
/// <returns>System.Object.</returns>
public delegate object ObjectDeserializerDelegate(ReadOnlySpan<char> value);

/// <summary>Interface ITypeSerializer</summary>
public interface ITypeSerializer
{
    /// <summary>
    /// Gets or sets the object deserializer.
    /// </summary>
    /// <value>The object deserializer.</value>
    ObjectDeserializerDelegate ObjectDeserializer { get; set; }

    /// <summary>
    /// Gets a value indicating whether [include null values].
    /// </summary>
    /// <value><c>true</c> if [include null values]; otherwise, <c>false</c>.</value>
    bool IncludeNullValues { get; }

    /// <summary>
    /// Gets a value indicating whether [include null values in dictionaries].
    /// </summary>
    /// <value><c>true</c> if [include null values in dictionaries]; otherwise, <c>false</c>.</value>
    bool IncludeNullValuesInDictionaries { get; }

    /// <summary>
    /// Gets the type attribute in object.
    /// </summary>
    /// <value>The type attribute in object.</value>
    string TypeAttrInObject { get; }

    /// <summary>
    /// Gets the write function.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>WriteObjectDelegate.</returns>
    WriteObjectDelegate GetWriteFn<T>();

    /// <summary>
    /// Gets the write function.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>WriteObjectDelegate.</returns>
    WriteObjectDelegate GetWriteFn(Type type);

    /// <summary>
    /// Gets the type information.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>TypeInfo.</returns>
    TypeInfo GetTypeInfo(Type type);

    /// <summary>
    /// Writes the raw string.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    void WriteRawString(TextWriter writer, string value);

    /// <summary>
    /// Writes the name of the property.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    void WritePropertyName(TextWriter writer, string value);

    /// <summary>
    /// Writes the built in.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    void WriteBuiltIn(TextWriter writer, object value);

    /// <summary>
    /// Writes the object string.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    void WriteObjectString(TextWriter writer, object value);

    /// <summary>
    /// Writes the exception.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    void WriteException(TextWriter writer, object value);

    /// <summary>
    /// Writes the string.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    void WriteString(TextWriter writer, string value);

    /// <summary>
    /// Writes the formattable object string.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    void WriteFormattableObjectString(TextWriter writer, object value);

    /// <summary>
    /// Writes the date time.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oDateTime">The o date time.</param>
    void WriteDateTime(TextWriter writer, object oDateTime);

    /// <summary>
    /// Writes the nullable date time.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="dateTime">The date time.</param>
    void WriteNullableDateTime(TextWriter writer, object dateTime);

    /// <summary>
    /// Writes the date time offset.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oDateTimeOffset">The o date time offset.</param>
    void WriteDateTimeOffset(TextWriter writer, object oDateTimeOffset);

    /// <summary>
    /// Writes the nullable date time offset.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="dateTimeOffset">The date time offset.</param>
    void WriteNullableDateTimeOffset(TextWriter writer, object dateTimeOffset);

    /// <summary>
    /// Writes the time span.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="timeSpan">The date time offset.</param>
    void WriteTimeSpan(TextWriter writer, object timeSpan);

    /// <summary>
    /// Writes the nullable time span.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="timeSpan">The date time offset.</param>
    void WriteNullableTimeSpan(TextWriter writer, object timeSpan);

    /// <summary>
    /// Writes the unique identifier.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oValue">The o value.</param>
    void WriteGuid(TextWriter writer, object oValue);

    /// <summary>
    /// Writes the nullable unique identifier.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oValue">The o value.</param>
    void WriteNullableGuid(TextWriter writer, object oValue);

    /// <summary>
    /// Writes the bytes.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oByteValue">The o byte value.</param>
    void WriteBytes(TextWriter writer, object oByteValue);

    /// <summary>
    /// Writes the character.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="charValue">The character value.</param>
    void WriteChar(TextWriter writer, object charValue);

    /// <summary>
    /// Writes the byte.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="byteValue">The byte value.</param>
    void WriteByte(TextWriter writer, object byteValue);

    /// <summary>
    /// Writes the s byte.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="sbyteValue">The sbyte value.</param>
    void WriteSByte(TextWriter writer, object sbyteValue);

    /// <summary>
    /// Writes the int16.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="intValue">The int value.</param>
    void WriteInt16(TextWriter writer, object intValue);

    /// <summary>
    /// Writes the u int16.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="intValue">The int value.</param>
    void WriteUInt16(TextWriter writer, object intValue);

    /// <summary>
    /// Writes the int32.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="intValue">The int value.</param>
    void WriteInt32(TextWriter writer, object intValue);

    /// <summary>
    /// Writes the u int32.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="uintValue">The uint value.</param>
    void WriteUInt32(TextWriter writer, object uintValue);

    /// <summary>
    /// Writes the int64.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="longValue">The long value.</param>
    void WriteInt64(TextWriter writer, object longValue);

    /// <summary>
    /// Writes the u int64.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="ulongValue">The ulong value.</param>
    void WriteUInt64(TextWriter writer, object ulongValue);

    /// <summary>
    /// Writes the bool.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="boolValue">The bool value.</param>
    void WriteBool(TextWriter writer, object boolValue);

    /// <summary>
    /// Writes the float.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="floatValue">The float value.</param>
    void WriteFloat(TextWriter writer, object floatValue);

    /// <summary>
    /// Writes the double.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="doubleValue">The double value.</param>
    void WriteDouble(TextWriter writer, object doubleValue);

    /// <summary>
    /// Writes the decimal.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="decimalValue">The decimal value.</param>
    void WriteDecimal(TextWriter writer, object decimalValue);

    /// <summary>
    /// Writes the enum.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="enumValue">The enum value.</param>
    void WriteEnum(TextWriter writer, object enumValue);

    /// <summary>
    /// Write Date Only
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oDateOnly">The date value.</param>
    void WriteDateOnly(TextWriter writer, object oDateOnly);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oDateOnly">The date value.</param>
    void WriteNullableDateOnly(TextWriter writer, object oDateOnly);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oTimeOnly">The time value.</param>
    void WriteTimeOnly(TextWriter writer, object oTimeOnly);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oTimeOnly">The time value.</param>
    void WriteNullableTimeOnly(TextWriter writer, object oTimeOnly);

    /// <summary>
    /// Gets the parse string span function.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>ParseStringSpanDelegate.</returns>
    ParseStringSpanDelegate GetParseStringSpanFn(Type type);

    /// <summary>
    /// Parses the raw string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    string ParseRawString(string value);

    /// <summary>
    /// Parses the string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    string ParseString(string value);

    /// <summary>
    /// Parses the string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    string ParseString(ReadOnlySpan<char> value);

    /// <summary>
    /// Unescapes the string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    string UnescapeString(string value);

    /// <summary>
    /// Unescapes the string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    ReadOnlySpan<char> UnescapeString(ReadOnlySpan<char> value);

    /// <summary>
    /// Unescapes the string as object.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    object UnescapeStringAsObject(ReadOnlySpan<char> value);

    /// <summary>
    /// Unescapes the safe string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    ReadOnlySpan<char> UnescapeSafeString(ReadOnlySpan<char> value);

    /// <summary>
    /// Eats the type value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="i">The i.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    ReadOnlySpan<char> EatTypeValue(ReadOnlySpan<char> value, ref int i);

    /// <summary>
    /// Eats the map start character.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="i">The i.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    bool EatMapStartChar(ReadOnlySpan<char> value, ref int i);

    /// <summary>
    /// Eats the map key.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="i">The i.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    ReadOnlySpan<char> EatMapKey(ReadOnlySpan<char> value, ref int i);

    /// <summary>
    /// Eats the map key seperator.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="i">The i.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    bool EatMapKeySeperator(ReadOnlySpan<char> value, ref int i);

    /// <summary>
    /// Eats the whitespace.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="i">The i.</param>
    void EatWhitespace(ReadOnlySpan<char> value, ref int i);

    /// <summary>
    /// Eats the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="i">The i.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    ReadOnlySpan<char> EatValue(ReadOnlySpan<char> value, ref int i);

    /// <summary>
    /// Eats the item seperator or map end character.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="i">The i.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    bool EatItemSeperatorOrMapEndChar(ReadOnlySpan<char> value, ref int i);
}