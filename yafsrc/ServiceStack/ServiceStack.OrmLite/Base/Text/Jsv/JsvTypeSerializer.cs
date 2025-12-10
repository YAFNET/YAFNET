// ***********************************************************************
// <copyright file="JsvTypeSerializer.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;

using ServiceStack.OrmLite.Base.Text.Common;
using ServiceStack.OrmLite.Base.Text.Json;

namespace ServiceStack.OrmLite.Base.Text.Jsv;

/// <summary>
/// Struct JsvTypeSerializer
/// Implements the <see cref="ITypeSerializer" />
/// </summary>
/// <seealso cref="ITypeSerializer" />
public struct JsvTypeSerializer
    : ITypeSerializer
{
    /// <summary>
    /// The instance
    /// </summary>
    public static ITypeSerializer Instance { get; } = new JsvTypeSerializer();

    /// <summary>
    /// Gets or sets the object deserializer.
    /// </summary>
    /// <value>The object deserializer.</value>
    public ObjectDeserializerDelegate ObjectDeserializer { get; set; }

    /// <summary>
    /// Gets a value indicating whether [include null values].
    /// </summary>
    /// <value><c>true</c> if [include null values]; otherwise, <c>false</c>.</value>
    public readonly bool IncludeNullValues => false;

    /// <summary>
    /// Gets a value indicating whether [include null values in dictionaries].
    /// </summary>
    /// <value><c>true</c> if [include null values in dictionaries]; otherwise, <c>false</c>.</value>
    public readonly bool IncludeNullValuesInDictionaries => false;

    /// <summary>
    /// Gets the type attribute in object.
    /// </summary>
    /// <value>The type attribute in object.</value>
    public readonly string TypeAttrInObject => JsConfig.JsvTypeAttrInObject;

    /// <summary>
    /// Gets the type attribute in object.
    /// </summary>
    /// <param name="typeAttr">The type attribute.</param>
    /// <returns>System.String.</returns>
    static internal string GetTypeAttrInObject(string typeAttr)
    {
        return $"{{{typeAttr}:";
    }

    /// <summary>
    /// Gets the write function.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>WriteObjectDelegate.</returns>
    public readonly WriteObjectDelegate GetWriteFn<T>()
    {
        return JsvWriter<T>.WriteFn();
    }

    /// <summary>
    /// Gets the write function.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>WriteObjectDelegate.</returns>
    public readonly WriteObjectDelegate GetWriteFn(Type type)
    {
        return JsvWriter.GetWriteFn(type);
    }

    /// <summary>
    /// The default type information
    /// </summary>
    readonly static TypeInfo DefaultTypeInfo = new() { EncodeMapKey = false };

    /// <summary>
    /// Gets the type information.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>TypeInfo.</returns>
    public readonly TypeInfo GetTypeInfo(Type type)
    {
        return DefaultTypeInfo;
    }

    /// <summary>
    /// Writes the raw string.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    public readonly void WriteRawString(TextWriter writer, string value)
    {
        writer.Write(value.EncodeJsv());
    }

    /// <summary>
    /// Writes the name of the property.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    public readonly void WritePropertyName(TextWriter writer, string value)
    {
        writer.Write(value);
    }

    /// <summary>
    /// Writes the built in.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    public readonly void WriteBuiltIn(TextWriter writer, object value)
    {
        writer.Write(value);
    }

    /// <summary>
    /// Writes the object string.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    public void WriteObjectString(TextWriter writer, object value)
    {
        if (value != null)
        {
            if (value is string strValue)
            {
                this.WriteString(writer, strValue);
            }
            else
            {
                writer.Write(value.ToString().EncodeJsv());
            }
        }
    }

    /// <summary>
    /// Writes the exception.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    public readonly void WriteException(TextWriter writer, object value)
    {
        writer.Write(((Exception)value).Message.EncodeJsv());
    }

    /// <summary>
    /// Writes the string.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    public readonly void WriteString(TextWriter writer, string value)
    {
        switch (JsState.QueryStringMode)
        {
            case true when !string.IsNullOrEmpty(value) && value.StartsWith(JsWriter.QuoteString) && value.EndsWith(JsWriter.QuoteString):
            case true when !string.IsNullOrEmpty(value) && value.Contains(JsWriter.ItemSeperatorString):
                value = string.Concat(JsWriter.QuoteChar, value, JsWriter.QuoteChar);
                break;
        }

        writer.Write(value == "" ? "\"\"" : value.EncodeJsv());
    }

    /// <summary>
    /// Writes the formattable object string.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    public readonly void WriteFormattableObjectString(TextWriter writer, object value)
    {
        var f = (IFormattable)value;
        writer.Write(f.ToString(null, CultureInfo.InvariantCulture).EncodeJsv());
    }

    /// <summary>
    /// Writes the date time.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oDateTime">The o date time.</param>
    public readonly void WriteDateTime(TextWriter writer, object oDateTime)
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

        writer.Write(DateTimeSerializer.ToShortestXsdDateTimeString((DateTime)oDateTime));
    }

    /// <summary>
    /// Writes the nullable date time.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="dateTime">The date time.</param>
    public void WriteNullableDateTime(TextWriter writer, object dateTime)
    {
        if (dateTime == null)
        {
            return;
        }

        this.WriteDateTime(writer, dateTime);
    }

    /// <summary>
    /// Writes the date time offset.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oDateTimeOffset">The o date time offset.</param>
    public readonly void WriteDateTimeOffset(TextWriter writer, object oDateTimeOffset)
    {
        writer.Write(((DateTimeOffset)oDateTimeOffset).ToString("o"));
    }

    /// <summary>
    /// Writes the nullable date time offset.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="dateTimeOffset">The date time offset.</param>
    public void WriteNullableDateTimeOffset(TextWriter writer, object dateTimeOffset)
    {
        if (dateTimeOffset == null)
        {
            return;
        }

        this.WriteDateTimeOffset(writer, dateTimeOffset);
    }

    /// <summary>
    /// Writes the time span.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oTimeSpan">The o time span.</param>
    public readonly void WriteTimeSpan(TextWriter writer, object oTimeSpan)
    {
        writer.Write(DateTimeSerializer.ToXsdTimeSpanString((TimeSpan)oTimeSpan));
    }

    /// <summary>
    /// Writes the nullable time span.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oTimeSpan">The o time span.</param>
    public readonly void WriteNullableTimeSpan(TextWriter writer, object oTimeSpan)
    {
        if (oTimeSpan == null)
        {
            return;
        }

        writer.Write(DateTimeSerializer.ToXsdTimeSpanString((TimeSpan?)oTimeSpan));
    }

    /// <summary>
    /// Writes the unique identifier.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oValue">The o value.</param>
    public readonly void WriteGuid(TextWriter writer, object oValue)
    {
        writer.Write(((Guid)oValue).ToString("N"));
    }

    /// <summary>
    /// Writes the nullable unique identifier.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oValue">The o value.</param>
    public readonly void WriteNullableGuid(TextWriter writer, object oValue)
    {
        if (oValue == null)
        {
            return;
        }

        writer.Write(((Guid)oValue).ToString("N"));
    }

    /// <summary>
    /// Writes the bytes.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oByteValue">The o byte value.</param>
    public readonly void WriteBytes(TextWriter writer, object oByteValue)
    {
        if (oByteValue == null)
        {
            return;
        }

        writer.Write(Convert.ToBase64String((byte[])oByteValue));
    }

    /// <summary>
    /// Writes the character.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="charValue">The character value.</param>
    public readonly void WriteChar(TextWriter writer, object charValue)
    {
        if (charValue == null)
        {
            return;
        }

        writer.Write((char)charValue);
    }

    /// <summary>
    /// Writes the byte.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="byteValue">The byte value.</param>
    public readonly void WriteByte(TextWriter writer, object byteValue)
    {
        if (byteValue == null)
        {
            return;
        }

        writer.Write((byte)byteValue);
    }

    /// <summary>
    /// Writes the s byte.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="sbyteValue">The sbyte value.</param>
    public readonly void WriteSByte(TextWriter writer, object sbyteValue)
    {
        if (sbyteValue == null)
        {
            return;
        }

        writer.Write((sbyte)sbyteValue);
    }

    /// <summary>
    /// Writes the int16.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="intValue">The int value.</param>
    public readonly void WriteInt16(TextWriter writer, object intValue)
    {
        if (intValue == null)
        {
            return;
        }

        writer.Write((short)intValue);
    }

    /// <summary>
    /// Writes the u int16.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="intValue">The int value.</param>
    public readonly void WriteUInt16(TextWriter writer, object intValue)
    {
        if (intValue == null)
        {
            return;
        }

        writer.Write((ushort)intValue);
    }

    /// <summary>
    /// Writes the int32.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="intValue">The int value.</param>
    public readonly void WriteInt32(TextWriter writer, object intValue)
    {
        if (intValue == null)
        {
            return;
        }

        writer.Write((int)intValue);
    }

    /// <summary>
    /// Writes the u int32.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="uintValue">The uint value.</param>
    public readonly void WriteUInt32(TextWriter writer, object uintValue)
    {
        if (uintValue == null)
        {
            return;
        }

        writer.Write((uint)uintValue);
    }

    /// <summary>
    /// Writes the u int64.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="ulongValue">The ulong value.</param>
    public readonly void WriteUInt64(TextWriter writer, object ulongValue)
    {
        if (ulongValue == null)
        {
            return;
        }

        writer.Write((ulong)ulongValue);
    }

    /// <summary>
    /// Writes the int64.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="longValue">The long value.</param>
    public readonly void WriteInt64(TextWriter writer, object longValue)
    {
        if (longValue == null)
        {
            return;
        }

        writer.Write((long)longValue);
    }

    /// <summary>
    /// Writes the bool.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="boolValue">The bool value.</param>
    public readonly void WriteBool(TextWriter writer, object boolValue)
    {
        if (boolValue == null)
        {
            return;
        }

        writer.Write((bool)boolValue);
    }

    /// <summary>
    /// Writes the float.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="floatValue">The float value.</param>
    public readonly void WriteFloat(TextWriter writer, object floatValue)
    {
        if (floatValue == null)
        {
            return;
        }

        var floatVal = (float)floatValue;
        var cultureInfo = JsState.IsCsv ? CsvConfig.RealNumberCultureInfo : null;

        switch (floatVal)
        {
            case float.MaxValue:
            case float.MinValue:
                writer.Write(floatVal.ToString("r", cultureInfo ?? CultureInfo.InvariantCulture));
                break;
            default:
                writer.Write(floatVal.ToString(cultureInfo ?? CultureInfo.InvariantCulture));
                break;
        }
    }

    /// <summary>
    /// Writes the double.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="doubleValue">The double value.</param>
    public readonly void WriteDouble(TextWriter writer, object doubleValue)
    {
        if (doubleValue == null)
        {
            return;
        }

        var doubleVal = (double)doubleValue;
        var cultureInfo = JsState.IsCsv ? CsvConfig.RealNumberCultureInfo : null;

        if (Equals(doubleVal, double.MaxValue) || Equals(doubleVal, double.MinValue))
        {
            writer.Write(doubleVal.ToString("r", cultureInfo ?? CultureInfo.InvariantCulture));
        }
        else
        {
            writer.Write(doubleVal.ToString(cultureInfo ?? CultureInfo.InvariantCulture));
        }
    }

    /// <summary>
    /// Writes the decimal.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="decimalValue">The decimal value.</param>
    public readonly void WriteDecimal(TextWriter writer, object decimalValue)
    {
        if (decimalValue == null)
        {
            return;
        }

        var cultureInfo = JsState.IsCsv ? CsvConfig.RealNumberCultureInfo : null;

        writer.Write(((decimal)decimalValue).ToString(cultureInfo ?? CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Writes the enum.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="enumValue">The enum value.</param>
    public readonly void WriteEnum(TextWriter writer, object enumValue)
    {
        if (enumValue == null)
        {
            return;
        }

        var serializedValue = CachedTypeInfo.Get(enumValue.GetType()).EnumInfo.GetSerializedValue(enumValue);
        if (serializedValue is string strEnum)
        {
            writer.Write(strEnum);
        }
        else
        {
            JsWriter.WriteEnumFlags(writer, enumValue);
        }
    }

    /// <summary>
    /// Writes the date only.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oDateOnly">The o date only.</param>
    public readonly void WriteDateOnly(TextWriter writer, object oDateOnly)
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
                    writer.Write(dateOnly.ToString("O"));
                    break;
            }
        }

    /// <summary>
    /// Writes the nullable date only.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oDateOnly">The o date only.</param>
    public void WriteNullableDateOnly(TextWriter writer, object oDateOnly)
        {
            if (oDateOnly == null)
            {
                return;
            }

            this.WriteDateOnly(writer, oDateOnly);
        }

    /// <summary>
    /// Writes the time only.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oTimeOnly">The o time only.</param>
    public void WriteTimeOnly(TextWriter writer, object oTimeOnly)
        {
            var stringValue = JsConfig.TimeSpanHandler == TimeSpanHandler.StandardFormat
                ? oTimeOnly.ToString()
                : DateTimeSerializer.ToXsdTimeSpanString(((TimeOnly)oTimeOnly).ToTimeSpan());
        this.WriteRawString(writer, stringValue);
        }

    /// <summary>
    /// Writes the nullable time only.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oTimeOnly">The o time only.</param>
    public void WriteNullableTimeOnly(TextWriter writer, object oTimeOnly)
    {
        if (oTimeOnly == null)
        {
            return;
        }

        this.WriteTimeSpan(writer, ((TimeOnly?)oTimeOnly).Value.ToTimeSpan());
    }

    /// <summary>
    /// Gets the parse function.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>ParseStringDelegate.</returns>
    public readonly ParseStringDelegate GetParseFn<T>()
    {
        return JsvReader.Instance.GetParseFn<T>();
    }

    /// <summary>
    /// Gets the parse string span function.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>ParseStringSpanDelegate.</returns>
    public readonly ParseStringSpanDelegate GetParseStringSpanFn(Type type)
    {
        return JsvReader.GetParseStringSpanFn(type);
    }

    /// <summary>
    /// Unescapes the string as object.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object UnescapeStringAsObject(ReadOnlySpan<char> value)
    {
        return this.UnescapeSafeString(value).Value();
    }

    /// <summary>
    /// Unescapes the safe string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public readonly ReadOnlySpan<char> UnescapeSafeString(ReadOnlySpan<char> value)
    {
        return JsState.IsCsv
            ? value // already unescaped in CsvReader.ParseFields()
            : value.FromCsvField();
    }

    /// <summary>
    /// Parses the raw string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public readonly string ParseRawString(string value)
    {
        return value;
    }

    /// <summary>
    /// Parses the string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public readonly string ParseString(string value)
    {
        return value.FromCsvField();
    }

    /// <summary>
    /// Parses the string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public readonly string ParseString(ReadOnlySpan<char> value)
    {
        return value.ToString().FromCsvField();
    }

    /// <summary>
    /// Unescapes the string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public readonly string UnescapeString(string value)
    {
        return value.FromCsvField();
    }

    /// <summary>
    /// Unescapes the string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public readonly ReadOnlySpan<char> UnescapeString(ReadOnlySpan<char> value)
    {
        return value.FromCsvField();
    }

    /// <summary>
    /// Eats the type value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="i">The i.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public ReadOnlySpan<char> EatTypeValue(ReadOnlySpan<char> value, ref int i)
    {
        return this.EatValue(value, ref i);
    }

    /// <summary>
    /// Eats the map start character.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="i">The i.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public readonly bool EatMapStartChar(ReadOnlySpan<char> value, ref int i)
    {
        var success = value[i] == JsWriter.MapStartChar;
        if (success)
        {
            i++;
        }

        return success;
    }

    /// <summary>
    /// Eats the map key.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="i">The i.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public readonly ReadOnlySpan<char> EatMapKey(ReadOnlySpan<char> value, ref int i)
    {
        var tokenStartPos = i;

        var valueLength = value.Length;

        var valueChar = value[tokenStartPos];

        switch (valueChar)
        {
            case JsWriter.QuoteChar:
                while (++i < valueLength)
                {
                    valueChar = value[i];

                    if (valueChar != JsWriter.QuoteChar)
                    {
                        continue;
                    }

                    var isLiteralQuote = i + 1 < valueLength && value[i + 1] == JsWriter.QuoteChar;

                    i++; //skip quote
                    if (!isLiteralQuote)
                    {
                        break;
                    }
                }
                return value.Slice(tokenStartPos, i - tokenStartPos);

            //Is Type/Map, i.e. {...}
            case JsWriter.MapStartChar:
                var endsToEat = 1;
                var withinQuotes = false;
                while (++i < valueLength && endsToEat > 0)
                {
                    valueChar = value[i];

                    if (valueChar == JsWriter.QuoteChar)
                    {
                        withinQuotes = !withinQuotes;
                    }

                    if (withinQuotes)
                    {
                        continue;
                    }

                    switch (valueChar)
                    {
                        case JsWriter.MapStartChar:
                            endsToEat++;
                            break;
                        case JsWriter.MapEndChar:
                            endsToEat--;
                            break;
                    }
                }
                return value.Slice(tokenStartPos, i - tokenStartPos);
        }

        while (value[++i] != JsWriter.MapKeySeperator) { }
        return value.Slice(tokenStartPos, i - tokenStartPos);
    }

    /// <summary>
    /// Eats the map key seperator.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="i">The i.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public readonly bool EatMapKeySeperator(ReadOnlySpan<char> value, ref int i)
    {
        return value[i++] == JsWriter.MapKeySeperator;
    }

    /// <summary>
    /// Eats the item seperator or map end character.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="i">The i.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="DiagnosticEvent.Exception">Expected '{JsWriter.ItemSeperator}' or '{JsWriter.MapEndChar}'</exception>
    /// <exception cref="System.Exception">Expected '{JsWriter.ItemSeperator}' or '{JsWriter.MapEndChar}'</exception>
    public readonly bool EatItemSeperatorOrMapEndChar(ReadOnlySpan<char> value, ref int i)
    {
        if (i == value.Length)
        {
            return false;
        }

        var success = value[i] == JsWriter.ItemSeperator
                      || value[i] == JsWriter.MapEndChar;

        if (success)
        {
            i++;
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
    public readonly void EatWhitespace(ReadOnlySpan<char> value, ref int i) { }

    /// <summary>
    /// Eats the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="i">The i.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public readonly ReadOnlySpan<char> EatValue(ReadOnlySpan<char> value, ref int i)
    {
        var tokenStartPos = i;
        var valueLength = value.Length;
        if (i == valueLength)
        {
            return default;
        }

        var valueChar = value[i];
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
                while (++i < valueLength)
                {
                    valueChar = value[i];

                    if (valueChar != JsWriter.QuoteChar)
                    {
                        continue;
                    }

                    var isLiteralQuote = i + 1 < valueLength && value[i + 1] == JsWriter.QuoteChar;

                    i++; //skip quote
                    if (!isLiteralQuote)
                    {
                        break;
                    }
                }
                return value.Slice(tokenStartPos, i - tokenStartPos);

            //Is Type/Map, i.e. {...}
            case JsWriter.MapStartChar:
                while (++i < valueLength && endsToEat > 0)
                {
                    valueChar = value[i];

                    if (valueChar == JsWriter.QuoteChar)
                    {
                        withinQuotes = !withinQuotes;
                    }

                    if (withinQuotes)
                    {
                        continue;
                    }

                    switch (valueChar)
                    {
                        case JsWriter.MapStartChar:
                            endsToEat++;
                            break;
                        case JsWriter.MapEndChar:
                            endsToEat--;
                            break;
                    }
                }
                return value.Slice(tokenStartPos, i - tokenStartPos);

            //Is List, i.e. [...]
            case JsWriter.ListStartChar:
                while (++i < valueLength && endsToEat > 0)
                {
                    valueChar = value[i];

                    if (valueChar == JsWriter.QuoteChar)
                    {
                        withinQuotes = !withinQuotes;
                    }

                    if (withinQuotes)
                    {
                        continue;
                    }

                    switch (valueChar)
                    {
                        case JsWriter.ListStartChar:
                            endsToEat++;
                            break;
                        case JsWriter.ListEndChar:
                            endsToEat--;
                            break;
                    }
                }
                return value.Slice(tokenStartPos, i - tokenStartPos);
        }

        //Is Value
        while (++i < valueLength)
        {
            valueChar = value[i];

            if (valueChar == JsWriter.ItemSeperator
                || valueChar == JsWriter.MapEndChar)
            {
                break;
            }
        }

        return value.Slice(tokenStartPos, i - tokenStartPos);
    }
}