// ***********************************************************************
// <copyright file="JsWriter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using ServiceStack.Text.Json;
using ServiceStack.Text.Jsv;

namespace ServiceStack.Text.Common;

using System.Linq;

/// <summary>
/// Class JsWriter.
/// </summary>
public static class JsWriter
{
    /// <summary>
    /// The type attribute
    /// </summary>
    public const string TypeAttr = "__type";

    /// <summary>
    /// The map start character
    /// </summary>
    public const char MapStartChar = '{';
    /// <summary>
    /// The map key seperator
    /// </summary>
    public const char MapKeySeperator = ':';
    /// <summary>
    /// The item seperator
    /// </summary>
    public const char ItemSeperator = ',';
    /// <summary>
    /// The map end character
    /// </summary>
    public const char MapEndChar = '}';
    /// <summary>
    /// The map null value
    /// </summary>
    public const string MapNullValue = "\"\"";
    /// <summary>
    /// The empty map
    /// </summary>
    public const string EmptyMap = "{}";

    /// <summary>
    /// The list start character
    /// </summary>
    public const char ListStartChar = '[';
    /// <summary>
    /// The list end character
    /// </summary>
    public const char ListEndChar = ']';
    /// <summary>
    /// The return character
    /// </summary>
    public const char ReturnChar = '\r';
    /// <summary>
    /// The line feed character
    /// </summary>
    public const char LineFeedChar = '\n';

    /// <summary>
    /// The quote character
    /// </summary>
    public const char QuoteChar = '"';
    /// <summary>
    /// The quote string
    /// </summary>
    public const string QuoteString = "\"";
    /// <summary>
    /// The escaped quote string
    /// </summary>
    public const string EscapedQuoteString = "\\\"";
    /// <summary>
    /// The item seperator string
    /// </summary>
    public const string ItemSeperatorString = ",";
    /// <summary>
    /// The map key seperator string
    /// </summary>
    public const string MapKeySeperatorString = ":";

    /// <summary>
    /// The CSV chars
    /// </summary>
    public static readonly char[] CsvChars = new[] { ItemSeperator, QuoteChar };
    /// <summary>
    /// The escape chars
    /// </summary>
    public static readonly char[] EscapeChars = new[] { QuoteChar, MapKeySeperator, ItemSeperator, MapStartChar, MapEndChar, ListStartChar, ListEndChar, ReturnChar, LineFeedChar };

    /// <summary>
    /// The length from largest character
    /// </summary>
    private const int LengthFromLargestChar = '}' + 1;
    /// <summary>
    /// The escape character flags
    /// </summary>
    private static readonly bool[] EscapeCharFlags = new bool[LengthFromLargestChar];

    /// <summary>
    /// Initializes static members of the <see cref="JsWriter" /> class.
    /// </summary>
    static JsWriter()
    {
        foreach (var escapeChar in EscapeChars)
        {
            EscapeCharFlags[escapeChar] = true;
        }
        var loadConfig = JsConfig.TextCase; //force load
    }

    /// <summary>
    /// micro optimizations: using flags instead of value.IndexOfAny(EscapeChars)
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if [has any escape chars] [the specified value]; otherwise, <c>false</c>.</returns>
    public static bool HasAnyEscapeChars(string value)
    {
        var len = value.Length;
        for (var i = 0; i < len; i++)
        {
            var c = value[i];
            if (c >= LengthFromLargestChar || !EscapeCharFlags[c]) continue;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Writes the item seperator if ran once.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="ranOnce">if set to <c>true</c> [ran once].</param>
    internal static void WriteItemSeperatorIfRanOnce(TextWriter writer, ref bool ranOnce)
    {
        if (ranOnce)
            writer.Write(ItemSeperator);
        else
            ranOnce = true;
    }

    /// <summary>
    /// Shoulds the use default to string method.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    internal static bool ShouldUseDefaultToStringMethod(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
        switch (underlyingType.GetTypeCode())
        {
            case TypeCode.SByte:
            case TypeCode.Byte:
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.Int32:
            case TypeCode.UInt32:
            case TypeCode.Int64:
            case TypeCode.UInt64:
            case TypeCode.Single:
            case TypeCode.Double:
            case TypeCode.Decimal:
            case TypeCode.DateTime:
                return true;
        }

        return underlyingType == typeof(Guid);
    }

    /// <summary>
    /// Gets the type serializer.
    /// </summary>
    /// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
    /// <returns>ITypeSerializer.</returns>
    /// <exception cref="System.NotSupportedException"></exception>
    public static ITypeSerializer GetTypeSerializer<TSerializer>()
    {
        if (typeof(TSerializer) == typeof(JsvTypeSerializer))
            return JsvTypeSerializer.Instance;

        if (typeof(TSerializer) == typeof(JsonTypeSerializer))
            return JsonTypeSerializer.Instance;

        throw new NotSupportedException(typeof(TSerializer).Name);
    }

    /// <summary>
    /// Writes the enum flags.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="enumFlagValue">The enum flag value.</param>
    public static void WriteEnumFlags(TextWriter writer, object enumFlagValue)
    {
        if (enumFlagValue == null) return;

        var typeCode = Enum.GetUnderlyingType(enumFlagValue.GetType()).GetTypeCode();
        switch (typeCode)
        {
            case TypeCode.SByte:
                writer.Write((sbyte)enumFlagValue);
                break;
            case TypeCode.Byte:
                writer.Write((byte)enumFlagValue);
                break;
            case TypeCode.Int16:
                writer.Write((short)enumFlagValue);
                break;
            case TypeCode.UInt16:
                writer.Write((ushort)enumFlagValue);
                break;
            case TypeCode.Int32:
                writer.Write((int)enumFlagValue);
                break;
            case TypeCode.UInt32:
                writer.Write((uint)enumFlagValue);
                break;
            case TypeCode.Int64:
                writer.Write((long)enumFlagValue);
                break;
            case TypeCode.UInt64:
                writer.Write((ulong)enumFlagValue);
                break;
            default:
                writer.Write((int)enumFlagValue);
                break;
        }
    }

    /// <summary>
    /// Shoulds the type of the allow runtime.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool ShouldAllowRuntimeType(Type type)
    {
        if (!JsState.IsRuntimeType)
            return true;

        if (JsConfig.AllowRuntimeType?.Invoke(type) == true)
            return true;

        var allowAttributesNamed = JsConfig.AllowRuntimeTypeWithAttributesNamed;
        if (allowAttributesNamed?.Count > 0)
        {
            var oAttrs = type.AllAttributes();
            foreach (var oAttr in oAttrs)
            {
                if (!(oAttr is Attribute attr)) continue;
                if (allowAttributesNamed.Contains(attr.GetType().Name))
                    return true;
            }
        }

        var allowInterfacesNamed = JsConfig.AllowRuntimeTypeWithInterfacesNamed;
        if (allowInterfacesNamed?.Count > 0)
        {
            var interfaces = type.GetInterfaces();
            return interfaces.Any(interfaceType => allowInterfacesNamed.Contains(interfaceType.Name));
        }

        return false;
    }

    /// <summary>
    /// Asserts the type of the allowed runtime.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <exception cref="System.NotSupportedException"></exception>
    public static void AssertAllowedRuntimeType(Type type)
    {
        if (!ShouldAllowRuntimeType(type))
            throw new NotSupportedException($"{type.Name} is not an allowed Runtime Type. Whitelist Type with [RuntimeSerializable] or IRuntimeSerializable.");
    }
}

/// <summary>
/// Class JsWriter.
/// </summary>
/// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
public class JsWriter<TSerializer>
    where TSerializer : ITypeSerializer
{
    /// <summary>
    /// The serializer
    /// </summary>
    private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

    /// <summary>
    /// Initializes a new instance of the <see cref="JsWriter{TSerializer}" /> class.
    /// </summary>
    public JsWriter()
    {
        this.SpecialTypes = new Dictionary<Type, WriteObjectDelegate>
                                {
                                    { typeof(Uri), Serializer.WriteObjectString },
                                    { typeof(Type), WriteType },
                                    { typeof(Exception), Serializer.WriteException },
                                };
    }

    /// <summary>
    /// Gets the value type to string method.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>WriteObjectDelegate.</returns>
    public WriteObjectDelegate GetValueTypeToStringMethod(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type);
        var isNullable = underlyingType != null;
        underlyingType ??= type;

        switch (underlyingType.IsEnum)
        {
            case false:
                {
                    var typeCode = underlyingType.GetTypeCode();

                    if (typeCode == TypeCode.Char)
                        return Serializer.WriteChar;
                    if (typeCode == TypeCode.Int32)
                        return Serializer.WriteInt32;
                    if (typeCode == TypeCode.Int64)
                        return Serializer.WriteInt64;
                    if (typeCode == TypeCode.UInt64)
                        return Serializer.WriteUInt64;
                    if (typeCode == TypeCode.UInt32)
                        return Serializer.WriteUInt32;

                    if (typeCode == TypeCode.Byte)
                        return Serializer.WriteByte;
                    if (typeCode == TypeCode.SByte)
                        return Serializer.WriteSByte;

                    if (typeCode == TypeCode.Int16)
                        return Serializer.WriteInt16;
                    if (typeCode == TypeCode.UInt16)
                        return Serializer.WriteUInt16;

                    if (typeCode == TypeCode.Boolean)
                        return Serializer.WriteBool;

                    if (typeCode == TypeCode.Single)
                        return Serializer.WriteFloat;

                    if (typeCode == TypeCode.Double)
                        return Serializer.WriteDouble;

                    if (typeCode == TypeCode.Decimal)
                        return Serializer.WriteDecimal;

                    if (typeCode == TypeCode.DateTime)
                        if (isNullable)
                            return Serializer.WriteNullableDateTime;
                        else
                            return Serializer.WriteDateTime;

                    if (type == typeof(DateTimeOffset))
                        return Serializer.WriteDateTimeOffset;

                    if (type == typeof(DateTimeOffset?))
                        return Serializer.WriteNullableDateTimeOffset;

                    if (type == typeof(TimeSpan))
                        return Serializer.WriteTimeSpan;

                    if (type == typeof(TimeSpan?))
                        return Serializer.WriteNullableTimeSpan;

                    if (type == typeof(Guid))
                        return Serializer.WriteGuid;

                    if (type == typeof(Guid?))
                        return Serializer.WriteNullableGuid;
                    break;
#if NET6_0
                if (type == typeof(DateOnly))
                    if (isNullable)
                        return Serializer.WriteNullableDateOnly;
                    else
                        return Serializer.WriteDateOnly;
                if (type == typeof(DateOnly?))
                    return Serializer.WriteDateOnly;

                if (type == typeof(TimeOnly))
                    if (isNullable)
                        return Serializer.WriteNullableTimeOnly;
                    else
                        return Serializer.WriteTimeOnly;
                if (type == typeof(TimeOnly?))
                    return Serializer.WriteTimeOnly;
#endif
                }
            case true:
                return Serializer.WriteEnum;
        }

        if (type.HasInterface(typeof(IFormattable)))
            return Serializer.WriteFormattableObjectString;

        if (type.HasInterface(typeof(IValueWriter)))
            return WriteValue;

        return Serializer.WriteObjectString;
    }

    /// <summary>
    /// Gets the write function.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>WriteObjectDelegate.</returns>
    public WriteObjectDelegate GetWriteFn<T>()
    {
        if (typeof(T) == typeof(string))
        {
            return Serializer.WriteObjectString;
        }

        WriteObjectDelegate ret = null;

        var onSerializingFn = JsConfig<T>.OnSerializingFn;
        if (onSerializingFn != null)
        {
            var writeFn = GetCoreWriteFn<T>();
            ret = (w, x) => writeFn(w, onSerializingFn((T)x));
        }

        if (JsConfig<T>.HasSerializeFn)
        {
            ret = JsConfig<T>.WriteFn<TSerializer>;
        }

        ret ??= GetCoreWriteFn<T>();

        var onSerializedFn = JsConfig<T>.OnSerializedFn;
        if (onSerializedFn == null) return ret;
        {
            var writerFunc = ret;
            ret = (w, x) =>
            {
                writerFunc(w, x);
                onSerializedFn((T)x);
            };
        }

        return ret;
    }

    /// <summary>
    /// Writes the value.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    public void WriteValue(TextWriter writer, object value)
    {
        var valueWriter = (IValueWriter)value;
        valueWriter.WriteTo(Serializer, writer);
    }

    /// <summary>
    /// Throws the task not supported.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    /// <exception cref="System.NotSupportedException">Serializing Task's is not supported. Did you forget to await it?</exception>
    private static void ThrowTaskNotSupported(TextWriter writer, object value) =>
        throw new NotSupportedException("Serializing Task's is not supported. Did you forget to await it?");

    /// <summary>
    /// Gets the core write function.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>WriteObjectDelegate.</returns>
    private WriteObjectDelegate GetCoreWriteFn<T>()
    {
        if (typeof(T).IsInstanceOf(typeof(System.Threading.Tasks.Task)))
            return ThrowTaskNotSupported;

        if (typeof(T).IsValueType && !JsConfig.TreatAsRefType(typeof(T)) || JsConfig<T>.HasSerializeFn)
        {
            return JsConfig<T>.HasSerializeFn
                       ? JsConfig<T>.WriteFn<TSerializer>
                       : GetValueTypeToStringMethod(typeof(T));
        }

        var specialWriteFn = GetSpecialWriteFn(typeof(T));
        if (specialWriteFn != null)
        {
            return specialWriteFn;
        }

        if (typeof(T).IsArray)
        {
            if (typeof(T) == typeof(byte[]))
                return (w, x) => WriteLists.WriteBytes(Serializer, w, x);

            if (typeof(T) == typeof(string[]))
                return (w, x) => WriteLists.WriteStringArray(Serializer, w, x);

            if (typeof(T) == typeof(int[]))
                return WriteListsOfElements<int, TSerializer>.WriteGenericArrayValueType;
            if (typeof(T) == typeof(long[]))
                return WriteListsOfElements<long, TSerializer>.WriteGenericArrayValueType;

            var elementType = typeof(T).GetElementType();
            var writeFn = WriteListsOfElements<TSerializer>.GetGenericWriteArray(elementType);
            return writeFn;
        }

        if (typeof(T).HasGenericType() ||
            typeof(T).HasInterface(typeof(IDictionary<string, object>))) // is ExpandoObject?
        {
            if (typeof(T).IsOrHasGenericInterfaceTypeOf(typeof(IList<>)))
                return WriteLists<T, TSerializer>.Write;

            var mapInterface = typeof(T).GetTypeWithGenericTypeDefinitionOf(typeof(IDictionary<,>));
            if (mapInterface != null)
            {
                var mapTypeArgs = mapInterface.GetGenericArguments();
                var writeFn = WriteDictionary<TSerializer>.GetWriteGenericDictionary(
                    mapTypeArgs[0], mapTypeArgs[1]);

                var keyWriteFn = Serializer.GetWriteFn(mapTypeArgs[0]);
                var valueWriteFn = typeof(JsonObject).IsAssignableFrom(typeof(T))
                                       ? JsonObject.WriteValue
                                       : Serializer.GetWriteFn(mapTypeArgs[1]);

                return (w, x) => writeFn(w, x, keyWriteFn, valueWriteFn);
            }
        }

        var enumerableInterface = typeof(T).GetTypeWithGenericTypeDefinitionOf(typeof(IEnumerable<>));
        if (enumerableInterface != null)
        {
            var elementType = enumerableInterface.GetGenericArguments()[0];
            var writeFn = WriteListsOfElements<TSerializer>.GetGenericWriteEnumerable(elementType);
            return writeFn;
        }

        var isDictionary = typeof(T) != typeof(IEnumerable) && typeof(T) != typeof(ICollection)
                                                            && (typeof(T).IsAssignableFrom(typeof(IDictionary)) || typeof(T).HasInterface(typeof(IDictionary)));
        if (isDictionary)
        {
            return WriteDictionary<TSerializer>.WriteIDictionary;
        }

        var isEnumerable = typeof(T).IsAssignableFrom(typeof(IEnumerable))
                           || typeof(T).HasInterface(typeof(IEnumerable));
        if (isEnumerable)
        {
            return WriteListsOfElements<TSerializer>.WriteIEnumerable;
        }

        if (typeof(T).HasInterface(typeof(IValueWriter)))
            return WriteValue;

        if (typeof(T).IsClass || typeof(T).IsInterface || JsConfig.TreatAsRefType(typeof(T)))
        {
            var typeToStringMethod = WriteType<T, TSerializer>.Write;
            if (typeToStringMethod != null)
            {
                return typeToStringMethod;
            }
        }

        return Serializer.WriteBuiltIn;
    }

    /// <summary>
    /// The special types
    /// </summary>
    public readonly Dictionary<Type, WriteObjectDelegate> SpecialTypes;

    /// <summary>
    /// Gets the special write function.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>WriteObjectDelegate.</returns>
    public WriteObjectDelegate GetSpecialWriteFn(Type type)
    {
        if (SpecialTypes.TryGetValue(type, out var writeFn))
            return writeFn;

        if (type.IsInstanceOfType(typeof(Type)))
            return WriteType;

        if (type.IsInstanceOf(typeof(Exception)))
            return Serializer.WriteException;

        return null;
    }

    /// <summary>
    /// Writes the type.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    public void WriteType(TextWriter writer, object value)
    {
        Serializer.WriteRawString(writer, JsConfig.TypeWriter((Type)value));
    }

    /// <summary>
    /// Initializes the aot.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void InitAot<T>()
    {
        WriteListsOfElements<T, TSerializer>.WriteList(null, null);
        WriteListsOfElements<T, TSerializer>.WriteIList(null, null);
        WriteListsOfElements<T, TSerializer>.WriteEnumerable(null, null);
        WriteListsOfElements<T, TSerializer>.WriteListValueType(null, null);
        WriteListsOfElements<T, TSerializer>.WriteIListValueType(null, null);
        WriteListsOfElements<T, TSerializer>.WriteGenericArrayValueType(null, null);
        WriteListsOfElements<T, TSerializer>.WriteArray(null, null);

        TranslateListWithElements<T>.LateBoundTranslateToGenericICollection(null, null);
        TranslateListWithConvertibleElements<T, T>.LateBoundTranslateToGenericICollection(null, null);

        QueryStringWriter<T>.WriteObject(null, null);
    }
}