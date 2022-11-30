// ***********************************************************************
// <copyright file="JsonTypeSerializer.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;

using ServiceStack.Text.Common;

namespace ServiceStack.Text.Json;

public ref struct SpanIndex
{
    public ReadOnlySpan<char> Span { get; }

    public int Index { get; }

    public SpanIndex(ReadOnlySpan<char> value, int index)
    {
        Span = value;
        Index = index;
    }
}

/// <summary>
/// Struct JsonTypeSerializer
/// Implements the <see cref="ServiceStack.Text.Common.ITypeSerializer" />
/// </summary>
/// <seealso cref="ServiceStack.Text.Common.ITypeSerializer" />
public struct JsonTypeSerializer
    : ITypeSerializer
{
    /// <summary>
    /// The instance
    /// </summary>
    public static ITypeSerializer Instance = new JsonTypeSerializer();

    /// <summary>
    /// Gets or sets the object deserializer.
    /// </summary>
    /// <value>The object deserializer.</value>
    public ObjectDeserializerDelegate ObjectDeserializer { get; set; }

    /// <summary>
    /// Gets a value indicating whether [include null values].
    /// </summary>
    /// <value><c>true</c> if [include null values]; otherwise, <c>false</c>.</value>
    public bool IncludeNullValues => JsConfig.IncludeNullValues;

    /// <summary>
    /// Gets a value indicating whether [include null values in dictionaries].
    /// </summary>
    /// <value><c>true</c> if [include null values in dictionaries]; otherwise, <c>false</c>.</value>
    public bool IncludeNullValuesInDictionaries => JsConfig.IncludeNullValuesInDictionaries;

    /// <summary>
    /// Gets the type attribute in object.
    /// </summary>
    /// <value>The type attribute in object.</value>
    public string TypeAttrInObject => JsConfig.JsonTypeAttrInObject;

    /// <summary>
    /// Gets the type attribute in object.
    /// </summary>
    /// <param name="typeAttr">The type attribute.</param>
    /// <returns>System.String.</returns>
    internal static string GetTypeAttrInObject(string typeAttr) => $"{{\"{typeAttr}\":";

    /// <summary>
    /// Gets the write function.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>WriteObjectDelegate.</returns>
    public WriteObjectDelegate GetWriteFn<T>() => JsonWriter<T>.WriteFn();

    /// <summary>
    /// Gets the write function.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>WriteObjectDelegate.</returns>
    public WriteObjectDelegate GetWriteFn(Type type) => JsonWriter.GetWriteFn(type);

    /// <summary>
    /// Gets the type information.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>TypeInfo.</returns>
    public TypeInfo GetTypeInfo(Type type) => JsonWriter.GetTypeInfo(type);

    /// <summary>
    /// Shortcut escape when we're sure value doesn't contain any escaped chars
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    public void WriteRawString(TextWriter writer, string value)
    {
        writer.Write(JsWriter.QuoteChar);
        writer.Write(value);
        writer.Write(JsWriter.QuoteChar);
    }

    /// <summary>
    /// Writes the name of the property.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    public void WritePropertyName(TextWriter writer, string value)
    {
        if (JsState.WritingKeyCount > 0)
        {
            writer.Write(JsWriter.EscapedQuoteString);
            writer.Write(value);
            writer.Write(JsWriter.EscapedQuoteString);
        }
        else
        {
            WriteRawString(writer, value);
        }
    }

    /// <summary>
    /// Writes the string.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    public void WriteString(TextWriter writer, string value)
    {
        JsonUtils.WriteString(writer, value);
    }

    /// <summary>
    /// Writes the built in.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    public void WriteBuiltIn(TextWriter writer, object value)
    {
        if (JsState.WritingKeyCount > 0 && !JsState.IsWritingValue) writer.Write(JsonUtils.QuoteChar);

        WriteRawString(writer, value.ToString());

        if (JsState.WritingKeyCount > 0 && !JsState.IsWritingValue) writer.Write(JsonUtils.QuoteChar);
    }

    /// <summary>
    /// Writes the object string.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    public void WriteObjectString(TextWriter writer, object value)
    {
        JsonUtils.WriteString(writer, value?.ToString());
    }

    /// <summary>
    /// Writes the formattable object string.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    public void WriteFormattableObjectString(TextWriter writer, object value)
    {
        var formattable = value as IFormattable;
        JsonUtils.WriteString(writer, formattable?.ToString(null, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Writes the exception.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    public void WriteException(TextWriter writer, object value)
    {
        WriteString(writer, ((Exception)value).Message);
    }

    /// <summary>
    /// Writes the date time.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oDateTime">The o date time.</param>
    public void WriteDateTime(TextWriter writer, object oDateTime)
    {
        var dateTime = (DateTime)oDateTime;
        switch (JsConfig.DateHandler)
        {
            case DateHandler.UnixTime:
                writer.Write(dateTime.ToUnixTime());
                return;
            case DateHandler.UnixTimeMs:
                writer.Write(dateTime.ToUnixTimeMs());
                return;
        }

        writer.Write(JsWriter.QuoteString);
        DateTimeSerializer.WriteWcfJsonDate(writer, dateTime);
        writer.Write(JsWriter.QuoteString);
    }

    /// <summary>
    /// Writes the nullable date time.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="dateTime">The date time.</param>
    public void WriteNullableDateTime(TextWriter writer, object dateTime)
    {
        if (dateTime == null)
            writer.Write(JsonUtils.Null);
        else
            WriteDateTime(writer, dateTime);
    }

    /// <summary>
    /// Writes the date time offset.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oDateTimeOffset">The o date time offset.</param>
    public void WriteDateTimeOffset(TextWriter writer, object oDateTimeOffset)
    {
        writer.Write(JsWriter.QuoteString);
        DateTimeSerializer.WriteWcfJsonDateTimeOffset(writer, (DateTimeOffset)oDateTimeOffset);
        writer.Write(JsWriter.QuoteString);
    }

    /// <summary>
    /// Writes the nullable date time offset.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="dateTimeOffset">The date time offset.</param>
    public void WriteNullableDateTimeOffset(TextWriter writer, object dateTimeOffset)
    {
        if (dateTimeOffset == null)
            writer.Write(JsonUtils.Null);
        else
            WriteDateTimeOffset(writer, dateTimeOffset);
    }

    /// <summary>
    /// Writes the time span.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oTimeSpan">The o time span.</param>
    public void WriteTimeSpan(TextWriter writer, object oTimeSpan)
    {
        var stringValue = JsConfig.TimeSpanHandler == TimeSpanHandler.StandardFormat
                              ? oTimeSpan.ToString()
                              : DateTimeSerializer.ToXsdTimeSpanString((TimeSpan)oTimeSpan);
        WriteRawString(writer, stringValue);
    }

    /// <summary>
    /// Writes the nullable time span.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oTimeSpan">The o time span.</param>
    public void WriteNullableTimeSpan(TextWriter writer, object oTimeSpan)
    {

        if (oTimeSpan == null) return;
        WriteTimeSpan(writer, ((TimeSpan?)oTimeSpan).Value);
    }

#if NET6_0
        public void WriteDateOnly(TextWriter writer, object oDateOnly)
        {
            var dateOnly = (DateOnly)oDateOnly;
            switch (JsConfig.DateHandler)
            {
                case DateHandler.UnixTime:
                    writer.Write(dateOnly.ToUnixTime());
                    break;
                case DateHandler.UnixTimeMs:
                    writer.Write(dateOnly.ToUnixTimeMs());
                    break;
                default:
                    writer.Write(JsWriter.QuoteString);
                    writer.Write(dateOnly.ToString("O"));
                    writer.Write(JsWriter.QuoteString);
                    break;
            }
        }

        public void WriteNullableDateOnly(TextWriter writer, object oDateOnly)
        {
            if (oDateOnly == null)
                writer.Write(JsonUtils.Null);
            else
                WriteDateOnly(writer, oDateOnly);
        }

        public void WriteTimeOnly(TextWriter writer, object oTimeOnly)
        {
            var stringValue = JsConfig.TimeSpanHandler == TimeSpanHandler.StandardFormat
                ? oTimeOnly.ToString()
                : DateTimeSerializer.ToXsdTimeSpanString(((TimeOnly)oTimeOnly).ToTimeSpan());
            WriteRawString(writer, stringValue);
        }

        public void WriteNullableTimeOnly(TextWriter writer, object oTimeOnly)
        {
            if (oTimeOnly == null) return;
            WriteTimeSpan(writer, ((TimeOnly?)oTimeOnly).Value.ToTimeSpan());
        }
#endif

    /// <summary>
    /// Writes the unique identifier.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oValue">The o value.</param>
    public void WriteGuid(TextWriter writer, object oValue)
    {
        WriteRawString(writer, ((Guid)oValue).ToString("N"));
    }

    /// <summary>
    /// Writes the nullable unique identifier.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oValue">The o value.</param>
    public void WriteNullableGuid(TextWriter writer, object oValue)
    {
        if (oValue == null) return;
        WriteRawString(writer, ((Guid)oValue).ToString("N"));
    }

    /// <summary>
    /// Writes the bytes.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oByteValue">The o byte value.</param>
    public void WriteBytes(TextWriter writer, object oByteValue)
    {
        if (oByteValue == null) return;
        WriteRawString(writer, Convert.ToBase64String((byte[])oByteValue));
    }

    /// <summary>
    /// Writes the character.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="charValue">The character value.</param>
    public void WriteChar(TextWriter writer, object charValue)
    {
        if (charValue == null)
            writer.Write(JsonUtils.Null);
        else
            WriteString(writer, ((char)charValue).ToString());
    }

    /// <summary>
    /// Writes the byte.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="byteValue">The byte value.</param>
    public void WriteByte(TextWriter writer, object byteValue)
    {
        if (byteValue == null)
            writer.Write(JsonUtils.Null);
        else
            writer.Write((byte)byteValue);
    }

    /// <summary>
    /// Writes the s byte.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="sbyteValue">The sbyte value.</param>
    public void WriteSByte(TextWriter writer, object sbyteValue)
    {
        if (sbyteValue == null)
            writer.Write(JsonUtils.Null);
        else
            writer.Write((sbyte)sbyteValue);
    }

    /// <summary>
    /// Writes the int16.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="intValue">The int value.</param>
    public void WriteInt16(TextWriter writer, object intValue)
    {
        if (intValue == null)
            writer.Write(JsonUtils.Null);
        else
            writer.Write((short)intValue);
    }

    /// <summary>
    /// Writes the u int16.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="intValue">The int value.</param>
    public void WriteUInt16(TextWriter writer, object intValue)
    {
        if (intValue == null)
            writer.Write(JsonUtils.Null);
        else
            writer.Write((ushort)intValue);
    }

    /// <summary>
    /// Writes the int32.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="intValue">The int value.</param>
    public void WriteInt32(TextWriter writer, object intValue)
    {
        if (intValue == null)
            writer.Write(JsonUtils.Null);
        else
            writer.Write((int)intValue);
    }

    /// <summary>
    /// Writes the u int32.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="uintValue">The uint value.</param>
    public void WriteUInt32(TextWriter writer, object uintValue)
    {
        if (uintValue == null)
            writer.Write(JsonUtils.Null);
        else
            writer.Write((uint)uintValue);
    }

    /// <summary>
    /// Writes the int64.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="integerValue">The integer value.</param>
    public void WriteInt64(TextWriter writer, object integerValue)
    {
        if (integerValue == null)
            writer.Write(JsonUtils.Null);
        else
            writer.Write((long)integerValue);
    }

    /// <summary>
    /// Writes the u int64.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="ulongValue">The ulong value.</param>
    public void WriteUInt64(TextWriter writer, object ulongValue)
    {
        if (ulongValue == null)
        {
            writer.Write(JsonUtils.Null);
        }
        else
            writer.Write((ulong)ulongValue);
    }

    /// <summary>
    /// Writes the bool.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="boolValue">The bool value.</param>
    public void WriteBool(TextWriter writer, object boolValue)
    {
        if (boolValue == null)
            writer.Write(JsonUtils.Null);
        else
            writer.Write((bool)boolValue ? JsonUtils.True : JsonUtils.False);
    }

    /// <summary>
    /// Writes the float.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="floatValue">The float value.</param>
    public void WriteFloat(TextWriter writer, object floatValue)
    {
        if (floatValue == null)
        {
            writer.Write(JsonUtils.Null);
        }
        else
        {
            var floatVal = (float)floatValue;
            if (Equals(floatVal, float.MaxValue) || Equals(floatVal, float.MinValue))
                writer.Write(floatVal.ToString("r", CultureInfo.InvariantCulture));
            else
                writer.Write(floatVal.ToString("r", CultureInfo.InvariantCulture));
        }
    }

    /// <summary>
    /// Writes the double.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="doubleValue">The double value.</param>
    public void WriteDouble(TextWriter writer, object doubleValue)
    {
        if (doubleValue == null)
        {
            writer.Write(JsonUtils.Null);
        }
        else
        {
            var doubleVal = (double)doubleValue;
            if (Equals(doubleVal, double.MaxValue) || Equals(doubleVal, double.MinValue))
                writer.Write(doubleVal.ToString("r", CultureInfo.InvariantCulture));
            else
                writer.Write(doubleVal.ToString(CultureInfo.InvariantCulture));
        }
    }

    /// <summary>
    /// Writes the decimal.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="decimalValue">The decimal value.</param>
    public void WriteDecimal(TextWriter writer, object decimalValue)
    {
        writer.Write(
            decimalValue == null ? JsonUtils.Null : ((decimal)decimalValue).ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Writes the enum.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="enumValue">The enum value.</param>
    public void WriteEnum(TextWriter writer, object enumValue)
    {
        if (enumValue == null)
            return;
        var serializedValue = CachedTypeInfo.Get(enumValue.GetType()).EnumInfo.GetSerializedValue(enumValue);
        if (serializedValue is string strEnum)
            WriteRawString(writer, strEnum);
        else
            JsWriter.WriteEnumFlags(writer, enumValue);
    }

    /// <summary>
    /// Gets the parse function.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>ParseStringDelegate.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ParseStringDelegate GetParseFn<T>()
    {
        return JsonReader.Instance.GetParseFn<T>();
    }

    /// <summary>
    /// Gets the parse string span function.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>ParseStringSpanDelegate.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ParseStringSpanDelegate GetParseStringSpanFn<T>()
    {
        return JsonReader.Instance.GetParseStringSpanFn<T>();
    }

    /// <summary>
    /// Gets the parse function.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>ParseStringDelegate.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ParseStringDelegate GetParseFn(Type type)
    {
        return JsonReader.GetParseFn(type);
    }

    /// <summary>
    /// Gets the parse string span function.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>ParseStringSpanDelegate.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ParseStringSpanDelegate GetParseStringSpanFn(Type type)
    {
        return JsonReader.GetParseStringSpanFn(type);
    }

    /// <summary>
    /// Parses the raw string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ParseRawString(string value)
    {
        return value;
    }

    /// <summary>
    /// Parses the string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ParseString(ReadOnlySpan<char> value)
    {
        return value.IsNullOrEmpty() ? null : ParseRawString(value.ToString());
    }

    /// <summary>
    /// Parses the string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ParseString(string value)
    {
        return string.IsNullOrEmpty(value) ? value : ParseRawString(value);
    }

    /// <summary>
    /// Determines whether [is empty map] [the specified value].
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="i">The i.</param>
    /// <returns><c>true</c> if [is empty map] [the specified value]; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmptyMap(ReadOnlySpan<char> value, int i = 1)
    {
        for (; i < value.Length; i++) { var c = value[i]; if (!JsonUtils.IsWhiteSpace(c)) break; } //Whitespace inline
        if (value.Length == i) return true;
        return value[i++] == JsWriter.MapEndChar;
    }

    /// <summary>
    /// Parses the string.
    /// </summary>
    /// <param name="json">The json.</param>
    /// <param name="index">The index.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    /// <exception cref="System.Exception">Invalid unquoted string starting with: " + json.SafeSubstring(50).ToString()</exception>
    /// <exception cref="System.Exception">Invalid unquoted string ending with: " + json.SafeSubstring(json.Length - 50, 50).ToString()</exception>
    internal static ReadOnlySpan<char> ParseString(ReadOnlySpan<char> json, ref int index)
    {
        var jsonLength = json.Length;

        if (json[index] != JsonUtils.QuoteChar)
            throw new Exception("Invalid unquoted string starting with: " + json.SafeSubstring(50).ToString());

        var startIndex = ++index;
        do
        {
            var c = json[index];

            if (c == JsonUtils.QuoteChar)
                break;

            if (c == JsonUtils.EscapeChar)
            {
                index++;
                if (json[index] == 'u')
                    index += 4;
            }

        } while (index++ < jsonLength);

        if (index == jsonLength)
            throw new Exception("Invalid unquoted string ending with: " + json.SafeSubstring(json.Length - 50, 50).ToString());

        index++;
        var str = json.Slice(startIndex, Math.Min(index, jsonLength) - startIndex - 1);
        return str.Length == 0 ? TypeConstants.EmptyStringSpan : str;
    }

    /// <summary>
    /// Unescapes the string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string UnescapeString(string value)
    {
        return UnescapeJsonString(value, 0);
    }

    /// <summary>
    /// Unescapes the string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<char> UnescapeString(ReadOnlySpan<char> value)
    {
        return UnescapeJsonString(value, 0);
    }

    /// <summary>
    /// Unescapes the string as object.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object UnescapeStringAsObject(ReadOnlySpan<char> value)
    {
        return UnescapeJsString(value, JsonUtils.QuoteChar, removeQuotes: true, 0).Span.Value();
    }

    /// <summary>
    /// Unescapes the safe string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string UnescapeSafeString(string value) => UnescapeSafeString(value.AsSpan()).ToString();

    /// <summary>
    /// Unescapes the safe string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<char> UnescapeSafeString(ReadOnlySpan<char> value)
    {
        if (value.IsEmpty)
            return value;

        if (value[0] == JsonUtils.QuoteChar && value[value.Length - 1] == JsonUtils.QuoteChar)
            return value.Slice(1, value.Length - 2);

        return value;
    }

    /// <summary>
    /// The is safe json chars
    /// </summary>
    static readonly char[] IsSafeJsonChars = { JsonUtils.QuoteChar, JsonUtils.EscapeChar };

    /// <summary>
    /// Unescapes the json string.
    /// </summary>
    /// <param name="json">The json.</param>
    /// <param name="index">The index.</param>
    /// <returns>System.String.</returns>
    private static string UnescapeJsonString(string json, int index)
    {
        return json != null
                   ? UnescapeJsonString(json.AsSpan(), index).ToString()
                   : null;
    }

    /// <summary>
    /// Unescapes the json string.
    /// </summary>
    /// <param name="json">The json.</param>
    /// <param name="index">The index.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    private static ReadOnlySpan<char> UnescapeJsonString(ReadOnlySpan<char> json, int index) =>
        UnescapeJsString(json, JsonUtils.QuoteChar, true, index).Span;

    /// <summary>
    /// Unescapes the js string.
    /// </summary>
    /// <param name="json">The json.</param>
    /// <param name="quoteChar">The quote character.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public static ReadOnlySpan<char> UnescapeJsString(ReadOnlySpan<char> json, char quoteChar)
    {
        var ignore = 0;
        return UnescapeJsString(json, quoteChar, false, ignore).Span;
    }

    /// <summary>
    /// Unescapes the js string.
    /// </summary>
    /// <param name="json">The json.</param>
    /// <param name="quoteChar">The quote character.</param>
    /// <param name="removeQuotes">if set to <c>true</c> [remove quotes].</param>
    /// <param name="index">The index.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public static SpanIndex UnescapeJsString(ReadOnlySpan<char> json, char quoteChar, bool removeQuotes, int index)
    {
        if (json.IsNullOrEmpty()) return new(json, index);
        var jsonLength = json.Length;
        var buffer = json;

        var firstChar = buffer[index];
        if (firstChar == quoteChar)
        {
            index++;

            //MicroOp: See if we can short-circuit evaluation (to avoid StringBuilder)
            var jsonAtIndex = json.Slice(index);
            var strEndPos = jsonAtIndex.IndexOfAny(IsSafeJsonChars);
            if (strEndPos == -1)
                return new(jsonAtIndex.Slice(0, jsonLength), index);

            if (jsonAtIndex[strEndPos] == quoteChar)
            {
                var potentialValue = jsonAtIndex.Slice(0, strEndPos);
                index += strEndPos + 1;
                return new(potentialValue.Length > 0
                              ? potentialValue
                              : TypeConstants.EmptyStringSpan, index);
            }
        }
        else
        {
            var i = index;
            var end = jsonLength;

            while (i < end)
            {
                var c = buffer[i];
                if (c == quoteChar || c == JsonUtils.EscapeChar)
                    break;
                i++;
            }
            if (i == end)
                return new(buffer.Slice(index, jsonLength - index), index);
        }

        return new(Unescape(json, removeQuotes: removeQuotes, quoteChar: quoteChar), index);
    }

    /// <summary>
    /// Unescapes the specified input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>System.String.</returns>
    public static string Unescape(string input) => Unescape(input, true);
    /// <summary>
    /// Unescapes the specified input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="removeQuotes">if set to <c>true</c> [remove quotes].</param>
    /// <returns>System.String.</returns>
    public static string Unescape(string input, bool removeQuotes) => Unescape(input.AsSpan(), removeQuotes).ToString();

    /// <summary>
    /// Unescapes the specified input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public static ReadOnlySpan<char> Unescape(ReadOnlySpan<char> input) => Unescape(input, true);

    /// <summary>
    /// Unescapes the specified input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="removeQuotes">if set to <c>true</c> [remove quotes].</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public static ReadOnlySpan<char> Unescape(ReadOnlySpan<char> input, bool removeQuotes) =>
        Unescape(input, removeQuotes, JsonUtils.QuoteChar);

    /// <summary>
    /// Unescapes the specified input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="removeQuotes">if set to <c>true</c> [remove quotes].</param>
    /// <param name="quoteChar">The quote character.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public static ReadOnlySpan<char> Unescape(ReadOnlySpan<char> input, bool removeQuotes, char quoteChar)
    {
        var length = input.Length;
        int start = 0;
        int count = 0;
        var output = StringBuilderThreadStatic.Allocate();
        for (; count < length;)
        {
            var c = input[count];
            if (removeQuotes)
            {
                if (c == quoteChar)
                {
                    if (start != count)
                    {
                        output.Append(input.Slice(start, count - start));
                    }
                    count++;
                    start = count;
                    continue;
                }
            }

            if (c == JsonUtils.EscapeChar)
            {
                if (start != count)
                {
                    output.Append(input.Slice(start, count - start));
                }
                start = count;
                count++;
                if (count >= length) continue;

                //we will always be parsing an escaped char here
                c = input[count];

                switch (c)
                {
                    case 'a':
                        output.Append('\a');
                        count++;
                        break;
                    case 'b':
                        output.Append('\b');
                        count++;
                        break;
                    case 'f':
                        output.Append('\f');
                        count++;
                        break;
                    case 'n':
                        output.Append('\n');
                        count++;
                        break;
                    case 'r':
                        output.Append('\r');
                        count++;
                        break;
                    case 'v':
                        output.Append('\v');
                        count++;
                        break;
                    case 't':
                        output.Append('\t');
                        count++;
                        break;
                    case 'u':
                        if (count + 4 < length)
                        {
                            var unicodeString = input.Slice(count + 1, 4);
                            var unicodeIntVal = MemoryProvider.Instance.ParseUInt32(unicodeString, NumberStyles.HexNumber);
                            output.Append(ConvertFromUtf32((int)unicodeIntVal));
                            count += 5;
                        }
                        else
                        {
                            output.Append(c);
                        }
                        break;
                    case 'x':
                        if (count + 4 < length)
                        {
                            var unicodeString = input.Slice(count + 1, 4);
                            var unicodeIntVal = MemoryProvider.Instance.ParseUInt32(unicodeString, NumberStyles.HexNumber);
                            output.Append(ConvertFromUtf32((int)unicodeIntVal));
                            count += 5;
                        }
                        else
                        if (count + 2 < length)
                        {
                            var unicodeString = input.Slice(count + 1, 2);
                            var unicodeIntVal = MemoryProvider.Instance.ParseUInt32(unicodeString, NumberStyles.HexNumber);
                            output.Append(ConvertFromUtf32((int)unicodeIntVal));
                            count += 3;
                        }
                        else
                        {
                            output.Append(input.Slice(start, count - start));
                        }
                        break;
                    default:
                        output.Append(c);
                        count++;
                        break;
                }
                start = count;
            }
            else
            {
                count++;
            }
        }
        output.Append(input.Slice(start, length - start));
        return StringBuilderThreadStatic.ReturnAndFree(output).AsSpan();
    }

    /// <summary>
    /// Given a character as utf32, returns the equivalent string provided that the character
    /// is legal json.
    /// </summary>
    /// <param name="utf32">The utf32.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">utf32 - The argument must be from 0 to 0x10FFFF.</exception>
    public static string ConvertFromUtf32(int utf32)
    {
        switch (utf32)
        {
            case < 0:
            case > 0x10FFFF:
                throw new ArgumentOutOfRangeException(nameof(utf32), "The argument must be from 0 to 0x10FFFF.");
            case < 0x10000:
                return new string((char)utf32, 1);
            default:
                utf32 -= 0x10000;
                return new string(new[] { (char)((utf32 >> 10) + 0xD800), (char)(utf32 % 0x0400 + 0xDC00) });
        }
    }

    /// <summary>
    /// Eats the type value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="i">The i.</param>
    /// <returns>System.String.</returns>
    public string EatTypeValue(string value, ref int i)
    {
        return EatValue(value, ref i);
    }

    /// <summary>
    /// Eats the type value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="i">The i.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public ReadOnlySpan<char> EatTypeValue(ReadOnlySpan<char> value, ref int i)
    {
        return EatValue(value, ref i);
    }

    /// <summary>
    /// Eats the map start character.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="i">The i.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool EatMapStartChar(string value, ref int i) => EatMapStartChar(value.AsSpan(), ref i);

    /// <summary>
    /// Eats the map start character.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="i">The i.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool EatMapStartChar(ReadOnlySpan<char> value, ref int i)
    {
        for (; i < value.Length; i++) { var c = value[i]; if (!JsonUtils.IsWhiteSpace(c)) break; } //Whitespace inline
        return value[i++] == JsWriter.MapStartChar;
    }

    /// <summary>
    /// Eats the map key.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="i">The i.</param>
    /// <returns>System.String.</returns>
    public string EatMapKey(string value, ref int i) => EatMapKey(value.AsSpan(), ref i).ToString();

    /// <summary>
    /// Eats the map key.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="i">The i.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public ReadOnlySpan<char> EatMapKey(ReadOnlySpan<char> value, ref int i)
    {
        var valueLength = value.Length;
        for (; i < value.Length; i++) { var c = value[i]; if (!JsonUtils.IsWhiteSpace(c)) break; } //Whitespace inline

        var tokenStartPos = i;
        var valueChar = value[i];

        switch (valueChar)
        {
            //If we are at the end, return.
            case JsWriter.ItemSeperator:
            case JsWriter.MapEndChar:
                return default;

            //Is Within Quotes, i.e. "..."
            case JsWriter.QuoteChar:
                return ParseString(value, ref i);
        }

        //Is Value
        while (++i < valueLength)
        {
            valueChar = value[i];

            if (valueChar == JsWriter.ItemSeperator
                //If it doesn't have quotes it's either a keyword or number so also has a ws boundary
                || JsonUtils.IsWhiteSpace(valueChar)
               )
            {
                break;
            }
        }

        return value.Slice(tokenStartPos, i - tokenStartPos);
    }

    /// <summary>
    /// Eats the map key seperator.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="i">The i.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool EatMapKeySeperator(string value, ref int i) => EatMapKeySeperator(value.AsSpan(), ref i);


    /// <summary>
    /// Eats the map key seperator.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="i">The i.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool EatMapKeySeperator(ReadOnlySpan<char> value, ref int i)
    {
        for (; i < value.Length; i++) { var c = value[i]; if (!JsonUtils.IsWhiteSpace(c)) break; } //Whitespace inline
        if (value.Length == i) return false;
        return value[i++] == JsWriter.MapKeySeperator;
    }

    /// <summary>
    /// Eats the item seperator or map end character.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="i">The i.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool EatItemSeperatorOrMapEndChar(string value, ref int i)
    {
        return EatItemSeperatorOrMapEndChar(value.AsSpan(), ref i);
    }

    /// <summary>
    /// Eats the item seperator or map end character.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="i">The i.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="System.Exception">Expected '{JsWriter.ItemSeperator}' or '{JsWriter.MapEndChar}'</exception>
    public bool EatItemSeperatorOrMapEndChar(ReadOnlySpan<char> value, ref int i)
    {
        for (; i < value.Length; i++) { var c = value[i]; if (!JsonUtils.IsWhiteSpace(c)) break; } //Whitespace inline

        if (i == value.Length) return false;

        var success = value[i] == JsWriter.ItemSeperator || value[i] == JsWriter.MapEndChar;

        if (success)
        {
            i++;

            for (; i < value.Length; i++) { var c = value[i]; if (!JsonUtils.IsWhiteSpace(c)) break; } //Whitespace inline
        }
        else if (Env.StrictMode)
        {
            throw new Exception(
                $"Expected '{JsWriter.ItemSeperator}' or '{JsWriter.MapEndChar}'");
        }

        return success;
    }

    /// <summary>
    /// Eats the whitespace.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="i">The i.</param>
    public void EatWhitespace(ReadOnlySpan<char> value, ref int i)
    {
        for (; i < value.Length; i++) { var c = value[i]; if (!JsonUtils.IsWhiteSpace(c)) break; } //Whitespace inline
    }

    /// <summary>
    /// Eats the whitespace.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="i">The i.</param>
    public void EatWhitespace(string value, ref int i)
    {
        for (; i < value.Length; i++) { var c = value[i]; if (!JsonUtils.IsWhiteSpace(c)) break; } //Whitespace inline
    }

    /// <summary>
    /// Eats the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="i">The i.</param>
    /// <returns>System.String.</returns>
    public string EatValue(string value, ref int i)
    {
        return EatValue(value.AsSpan(), ref i).ToString();
    }

    /// <summary>
    /// Eats the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="i">The i.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public ReadOnlySpan<char> EatValue(ReadOnlySpan<char> value, ref int i)
    {
        var buf = value;
        var valueLength = value.Length;
        if (i == valueLength) return default;

        while (i < valueLength && JsonUtils.IsWhiteSpace(buf[i])) i++; //Whitespace inline
        if (i == valueLength) return default;

        var tokenStartPos = i;
        var valueChar = buf[i];
        var withinQuotes = false;
        var endsToEat = 1;

        switch (valueChar)
        {
            //If we are at the end, return.
            case JsWriter.ItemSeperator:
            case JsWriter.MapEndChar:
                return default;

            //Is Within Quotes, i.e. "..."
            case JsWriter.QuoteChar:
                return ParseString(value, ref i);

            //Is Type/Map, i.e. {...}
            case JsWriter.MapStartChar:
                while (++i < valueLength)
                {
                    valueChar = buf[i];

                    switch (valueChar)
                    {
                        case JsonUtils.EscapeChar:
                            i++;
                            continue;
                        case JsWriter.QuoteChar:
                            withinQuotes = !withinQuotes;
                            break;
                    }

                    if (withinQuotes)
                        continue;

                    if (valueChar == JsWriter.MapStartChar)
                        endsToEat++;

                    if (valueChar == JsWriter.MapEndChar && --endsToEat == 0)
                    {
                        i++;
                        break;
                    }
                }
                return value.Slice(tokenStartPos, i - tokenStartPos);

            //Is List, i.e. [...]
            case JsWriter.ListStartChar:
                while (++i < valueLength)
                {
                    valueChar = buf[i];

                    switch (valueChar)
                    {
                        case JsonUtils.EscapeChar:
                            i++;
                            continue;
                        case JsWriter.QuoteChar:
                            withinQuotes = !withinQuotes;
                            break;
                    }

                    if (withinQuotes)
                        continue;

                    if (valueChar == JsWriter.ListStartChar)
                        endsToEat++;

                    if (valueChar == JsWriter.ListEndChar && --endsToEat == 0)
                    {
                        i++;
                        break;
                    }
                }
                return value.Slice(tokenStartPos, i - tokenStartPos);
        }

        //Is Value
        while (++i < valueLength)
        {
            valueChar = buf[i];

            if (valueChar == JsWriter.ItemSeperator
                || valueChar == JsWriter.MapEndChar
                //If it doesn't have quotes it's either a keyword or number so also has a ws boundary
                || JsonUtils.IsWhiteSpace(valueChar)
               )
            {
                break;
            }
        }

        var strValue = value.Slice(tokenStartPos, i - tokenStartPos);

        return strValue.Equals(JsonUtils.Null.AsSpan(), StringComparison.Ordinal)
                   ? default
                   : strValue;
    }
}