// ***********************************************************************
// <copyright file="DeserializeType.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ServiceStack.OrmLite.Base.Text.Common;

/// <summary>
/// Class DeserializeType.
/// </summary>
/// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
public static class DeserializeType<TSerializer>
    where TSerializer : ITypeSerializer
{
    /// <summary>
    /// The serializer
    /// </summary>
    private readonly static ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

    /// <summary>
    /// Gets the parse method.
    /// </summary>
    /// <param name="typeConfig">The type configuration.</param>
    /// <returns>ParseStringDelegate.</returns>
    static internal ParseStringDelegate GetParseMethod(TypeConfig typeConfig)
    {
        return v => GetParseStringSpanMethod(typeConfig)(v.AsSpan());
    }

    /// <summary>
    /// Gets the parse string span method.
    /// </summary>
    /// <param name="typeConfig">The type configuration.</param>
    /// <returns>ParseStringSpanDelegate.</returns>
    static internal ParseStringSpanDelegate GetParseStringSpanMethod(TypeConfig typeConfig)
    {
        var type = typeConfig.Type;

        if (!type.IsStandardClass())
        {
            return null;
        }

        var accessors = DeserializeTypeRef.GetTypeAccessors(typeConfig, Serializer);

        var ctorFn = JsConfig.ModelFactory(type);
        if (accessors == null)
        {
            return value => ctorFn();
        }

        if (typeof(TSerializer) == typeof(Json.JsonTypeSerializer))
        {
            return new StringToTypeContext(typeConfig, ctorFn, accessors).DeserializeJson;
        }

        return new StringToTypeContext(typeConfig, ctorFn, accessors).DeserializeJsv;
    }

    /// <summary>
    /// Struct StringToTypeContext
    /// </summary>
    readonly internal struct StringToTypeContext
    {
        /// <summary>
        /// The type configuration
        /// </summary>
        private readonly TypeConfig typeConfig;
        /// <summary>
        /// The ctor function
        /// </summary>
        private readonly EmptyCtorDelegate ctorFn;
        /// <summary>
        /// The accessors
        /// </summary>
        private readonly KeyValuePair<string, TypeAccessor>[] accessors;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringToTypeContext" /> struct.
        /// </summary>
        /// <param name="typeConfig">The type configuration.</param>
        /// <param name="ctorFn">The ctor function.</param>
        /// <param name="accessors">The accessors.</param>
        public StringToTypeContext(TypeConfig typeConfig, EmptyCtorDelegate ctorFn, KeyValuePair<string, TypeAccessor>[] accessors)
        {
            this.typeConfig = typeConfig;
            this.ctorFn = ctorFn;
            this.accessors = accessors;
        }

        /// <summary>
        /// Deserializes the json.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Object.</returns>
        internal object DeserializeJson(ReadOnlySpan<char> value)
        {
            return DeserializeTypeRefJson.StringToType(value, this.typeConfig, this.ctorFn, this.accessors);
        }

        /// <summary>
        /// Deserializes the JSV.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Object.</returns>
        internal object DeserializeJsv(ReadOnlySpan<char> value)
        {
            return DeserializeTypeRefJsv.StringToType(value, this.typeConfig, this.ctorFn, this.accessors);
        }
    }

    /// <summary>
    /// Objects the type of the string to.
    /// </summary>
    /// <param name="strType">Type of the string.</param>
    /// <returns>System.Object.</returns>
    public static object ObjectStringToType(ReadOnlySpan<char> strType)
    {
        var type = ExtractType(strType);
        if (type != null)
        {
            var parseFn = Serializer.GetParseStringSpanFn(type);
            var propertyValue = parseFn(strType);
            return propertyValue;
        }

        var config = JsConfig.GetConfig();

        if (config.ConvertObjectTypesIntoStringDictionary && !strType.IsNullOrEmpty())
        {
            var firstChar = strType[0];
            var endChar = strType[^1];
            switch (firstChar)
            {
                case JsWriter.MapStartChar when endChar == JsWriter.MapEndChar:
                    {
                        var dynamicMatch = DeserializeDictionary<TSerializer>.ParseDictionary<string, object>(strType, null, v => Serializer.UnescapeString(v).ToString(), v => Serializer.UnescapeString(v).ToString());
                        if (dynamicMatch is {Count: > 0})
                        {
                            return dynamicMatch;
                        }

                        break;
                    }
                case JsWriter.ListStartChar when endChar == JsWriter.ListEndChar:
                    return DeserializeList<List<object>, TSerializer>.ParseStringSpan(strType);
            }
        }

        var primitiveType = config.TryToParsePrimitiveTypeValues ? ParsePrimitive(strType) : null;
        if (primitiveType != null)
        {
            return primitiveType;
        }

        if (Serializer.ObjectDeserializer != null && typeof(TSerializer) == typeof(Json.JsonTypeSerializer))
        {
            return !strType.IsNullOrEmpty()
                ? Serializer.ObjectDeserializer(strType)
                : strType.Value();
        }

        return Serializer.UnescapeString(strType).Value();
    }

    /// <summary>
    /// Extracts the type.
    /// </summary>
    /// <param name="strType">Type of the string.</param>
    /// <returns>Type.</returns>
    public static Type ExtractType(string strType)
    {
        return ExtractType(strType.AsSpan());
    }

    //TODO: optimize ExtractType
    /// <summary>
    /// Extracts the type.
    /// </summary>
    /// <param name="strType">Type of the string.</param>
    /// <returns>Type.</returns>
    public static Type ExtractType(ReadOnlySpan<char> strType)
    {
        if (strType.IsEmpty || strType.Length <= 1)
        {
            return null;
        }

        var hasWhitespace = Json.JsonUtils.WhiteSpaceChars.Contains(strType[1]);
        if (hasWhitespace)
        {
            var pos = strType.IndexOf('"');
            if (pos >= 0)
            {
                strType = ("{" + strType.Substring(pos, strType.Length - pos)).AsSpan();
            }
        }

        var typeAttrInObject = Serializer.TypeAttrInObject;
        if (strType.Length > typeAttrInObject.Length
            && strType.Slice(0, typeAttrInObject.Length).EqualsOrdinal(typeAttrInObject))
        {
            var propIndex = typeAttrInObject.Length;
            var typeName = Serializer.UnescapeSafeString(Serializer.EatValue(strType, ref propIndex)).ToString();

            var type = JsConfig.TypeFinder(typeName);

            JsWriter.AssertAllowedRuntimeType(type);

            if (type == null)
            {
                Tracer.Instance.WriteWarning("Could not find type: " + typeName);
                return null;
            }

            return ReflectionOptimizer.Instance.UseType(type);
        }
        return null;
    }

    /// <summary>
    /// Parses the type of the abstract.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public static object ParseAbstractType<T>(ReadOnlySpan<char> value)
    {
        if (typeof(T).IsAbstract)
        {
            if (value.IsNullOrEmpty())
            {
                return null;
            }

            var concreteType = ExtractType(value);
            if (concreteType != null)
            {
                var fn = Serializer.GetParseStringSpanFn(concreteType);
                if (fn == ParseAbstractType<T>)
                {
                    return null;
                }

                var ret = fn(value);
                return ret;
            }
            Tracer.Instance.WriteWarning(
                "Could not deserialize Abstract Type with unknown concrete type: " + typeof(T).FullName);
        }
        return null;
    }

    /// <summary>
    /// Parses the quoted primitive.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public static object ParseQuotedPrimitive(string value)
    {
        var config = JsConfig.GetConfig();
        var fn = config.ParsePrimitiveFn;
        var result = fn?.Invoke(value);
        if (result != null)
        {
            return result;
        }

        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        if (Guid.TryParse(value, out var guidValue))
        {
            return guidValue;
        }

        if (value.StartsWith(DateTimeSerializer.EscapedWcfJsonPrefix, StringComparison.Ordinal) || value.StartsWith(DateTimeSerializer.WcfJsonPrefix, StringComparison.Ordinal))
        {
            return DateTimeSerializer.ParseWcfJsonDate(value);
        }

        if (JsConfig.DateHandler == DateHandler.ISO8601)
        {
            // check that we have UTC ISO8601 date:
            // YYYY-MM-DDThh:mm:ssZ
            // YYYY-MM-DDThh:mm:ss+02:00
            // YYYY-MM-DDThh:mm:ss-02:00
            if (value.Length > 14 && value[10] == 'T' &&
                (value.EndsWithInvariant("Z")
                 || value[^6] == '+'
                 || value[^6] == '-'))
            {
                return DateTimeSerializer.ParseShortestXsdDateTime(value);
            }
        }

        if (config.DateHandler == DateHandler.RFC1123)
        {
            // check that we have RFC1123 date:
            // ddd, dd MMM yyyy HH:mm:ss GMT
            if (value.Length == 29 && value.EndsWithInvariant("GMT"))
            {
                return DateTimeSerializer.ParseRFC1123DateTime(value);
            }
        }

        return Serializer.UnescapeString(value);
    }

    /// <summary>
    /// Parses the primitive.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public static object ParsePrimitive(string value)
    {
        return ParsePrimitive(value.AsSpan());
    }

    /// <summary>
    /// Parses the primitive.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public static object ParsePrimitive(ReadOnlySpan<char> value)
    {
        var fn = JsConfig.ParsePrimitiveFn;
        var result = fn?.Invoke(value.ToString());
        if (result != null)
        {
            return result;
        }

        if (value.IsNullOrEmpty())
        {
            return null;
        }

        return value.TryParseBoolean(out var boolValue) ? boolValue : value.ParseNumber();
    }

    /// <summary>
    /// Parses the primitive.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="firstChar">The first character.</param>
    /// <returns>System.Object.</returns>
    static internal object ParsePrimitive(string value, char firstChar)
    {
        if (typeof(TSerializer) == typeof(Json.JsonTypeSerializer))
        {
            return firstChar == JsWriter.QuoteChar
                       ? ParseQuotedPrimitive(value)
                       : ParsePrimitive(value);
        }
        return ParsePrimitive(value) ?? ParseQuotedPrimitive(value);
    }
}

/// <summary>
/// Class TypeAccessorUtils.
/// </summary>
static internal class TypeAccessorUtils
{
    /// <summary>
    /// Gets the specified property name.
    /// </summary>
    /// <param name="accessors">The accessors.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="lenient">if set to <c>true</c> [lenient].</param>
    /// <returns>TypeAccessor.</returns>
    static internal TypeAccessor Get(this KeyValuePair<string, TypeAccessor>[] accessors, ReadOnlySpan<char> propertyName, bool lenient)
    {
        var testValue = FindPropertyAccessor(accessors, propertyName);
        if (testValue != null)
        {
            return testValue;
        }

        if (lenient)
        {
            return FindPropertyAccessor(accessors,
                propertyName.ToString().Replace("-", string.Empty).Replace("_", string.Empty).AsSpan());
        }

        return null;
    }

    /// <summary>
    /// Finds the property accessor.
    /// </summary>
    /// <param name="accessors">The accessors.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>TypeAccessor.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //Binary Search
    private static TypeAccessor FindPropertyAccessor(IReadOnlyList<KeyValuePair<string, TypeAccessor>> accessors, ReadOnlySpan<char> propertyName)
    {
        var lo = 0;
        var hi = accessors.Count - 1;
        var mid = (lo + hi + 1) / 2;

        while (lo <= hi)
        {
            var test = accessors[mid];
            var cmp = propertyName.CompareTo(test.Key.AsSpan(), StringComparison.OrdinalIgnoreCase);
            switch (cmp)
            {
                case 0:
                    return test.Value;
                case < 0:
                    hi = mid - 1;
                    break;
                default:
                    lo = mid + 1;
                    break;
            }

            mid = (lo + hi + 1) / 2;
        }
        return null;
    }
}

/// <summary>
/// Class TypeAccessor.
/// </summary>
internal class TypeAccessor
{
    /// <summary>
    /// The get property
    /// </summary>
    internal ParseStringSpanDelegate GetProperty;
    /// <summary>
    /// The set property
    /// </summary>
    internal SetMemberDelegate SetProperty;
    /// <summary>
    /// The property type
    /// </summary>
    internal Type PropertyType;

    /// <summary>
    /// Extracts the type.
    /// </summary>
    /// <param name="Serializer">The serializer.</param>
    /// <param name="strType">Type of the string.</param>
    /// <returns>Type.</returns>
    public static Type ExtractType(ITypeSerializer Serializer, string strType)
    {
        return ExtractType(Serializer, strType.AsSpan());
    }

    /// <summary>
    /// Extracts the type.
    /// </summary>
    /// <param name="Serializer">The serializer.</param>
    /// <param name="strType">Type of the string.</param>
    /// <returns>Type.</returns>
    public static Type ExtractType(ITypeSerializer Serializer, ReadOnlySpan<char> strType)
    {
        if (strType.IsEmpty || strType.Length <= 1)
        {
            return null;
        }

        var hasWhitespace = Json.JsonUtils.WhiteSpaceChars.Contains(strType[1]);
        if (hasWhitespace)
        {
            var pos = strType.IndexOf('"');
            if (pos >= 0)
            {
                strType = ("{" + strType.Substring(pos)).AsSpan();
            }
        }

        var typeAttrInObject = Serializer.TypeAttrInObject;
        if (strType.Length > typeAttrInObject.Length
            && strType.Slice(0, typeAttrInObject.Length).EqualsOrdinal(typeAttrInObject))
        {
            var propIndex = typeAttrInObject.Length;
            var typeName = Serializer.EatValue(strType, ref propIndex).ToString();
            var type = JsConfig.TypeFinder(typeName);

            if (type == null)
            {
                Tracer.Instance.WriteWarning("Could not find type: " + typeName);
            }

            return type;
        }
        return null;
    }

    /// <summary>
    /// Creates the specified serializer.
    /// </summary>
    /// <param name="serializer">The serializer.</param>
    /// <param name="typeConfig">The type configuration.</param>
    /// <param name="propertyInfo">The property information.</param>
    /// <returns>TypeAccessor.</returns>
    public static TypeAccessor Create(ITypeSerializer serializer, TypeConfig typeConfig, PropertyInfo propertyInfo)
    {
        return new TypeAccessor
                   {
                       PropertyType = propertyInfo.PropertyType,
                       GetProperty = GetPropertyMethod(serializer, propertyInfo),
                       SetProperty = GetSetPropertyMethod(typeConfig, propertyInfo)
                   };
    }

    /// <summary>
    /// Gets the property method.
    /// </summary>
    /// <param name="serializer">The serializer.</param>
    /// <param name="propertyInfo">The property information.</param>
    /// <returns>ParseStringSpanDelegate.</returns>
    static internal ParseStringSpanDelegate GetPropertyMethod(ITypeSerializer serializer, PropertyInfo propertyInfo)
    {
        var getPropertyFn = serializer.GetParseStringSpanFn(propertyInfo.PropertyType);

        if (propertyInfo.DeclaringType != null && JsConfig.AllowRuntimeTypeInTypes?.Count > 0 &&
            (propertyInfo.PropertyType == typeof(object) ||
             propertyInfo.PropertyType.HasInterface(typeof(IEnumerable<object>))))
        {
            return value =>
            {
                var hold = JsState.DeclaringType;
                try
                {
                    JsState.DeclaringType = propertyInfo.DeclaringType;
                    return getPropertyFn(value);
                }
                finally
                {
                    JsState.DeclaringType = hold;
                }
            };

        }
        return getPropertyFn;
    }

    /// <summary>
    /// Gets the set property method.
    /// </summary>
    /// <param name="typeConfig">The type configuration.</param>
    /// <param name="propertyInfo">The property information.</param>
    /// <returns>SetMemberDelegate.</returns>
    private static SetMemberDelegate GetSetPropertyMethod(TypeConfig typeConfig, PropertyInfo propertyInfo)
    {
        if (typeConfig.Type != propertyInfo.DeclaringType)
        {
            propertyInfo = propertyInfo.DeclaringType.GetProperty(propertyInfo.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        if (!propertyInfo.CanWrite && !typeConfig.EnableAnonymousFieldSetters)
        {
            return null;
        }

        FieldInfo fieldInfo = null;
        if (!propertyInfo.CanWrite)
        {
            var fieldNameFormat = Env.IsMono ? "<{0}>" : "<{0}>i__Field";
            var fieldName = string.Format(fieldNameFormat, propertyInfo.Name);

            var fieldInfos = typeConfig.Type.GetWritableFields();
            foreach (var f in fieldInfos)
            {
                if (f.IsInitOnly && f.FieldType == propertyInfo.PropertyType && f.Name.EqualsIgnoreCase(fieldName))
                {
                    fieldInfo = f;
                    break;
                }
            }

            if (fieldInfo == null)
            {
                return null;
            }
        }

        return propertyInfo.CanWrite
                   ? ReflectionOptimizer.Instance.CreateSetter(propertyInfo)
                   : ReflectionOptimizer.Instance.CreateSetter(fieldInfo);
    }

    /// <summary>
    /// Creates the specified serializer.
    /// </summary>
    /// <param name="serializer">The serializer.</param>
    /// <param name="typeConfig">The type configuration.</param>
    /// <param name="fieldInfo">The field information.</param>
    /// <returns>TypeAccessor.</returns>
    public static TypeAccessor Create(ITypeSerializer serializer, TypeConfig typeConfig, FieldInfo fieldInfo)
    {
        return new TypeAccessor
                   {
                       PropertyType = fieldInfo.FieldType,
                       GetProperty = serializer.GetParseStringSpanFn(fieldInfo.FieldType),
                       SetProperty = GetSetFieldMethod(typeConfig, fieldInfo)
                   };
    }

    /// <summary>
    /// Gets the set field method.
    /// </summary>
    /// <param name="typeConfig">The type configuration.</param>
    /// <param name="fieldInfo">The field information.</param>
    /// <returns>SetMemberDelegate.</returns>
    private static SetMemberDelegate GetSetFieldMethod(TypeConfig typeConfig, FieldInfo fieldInfo)
    {
        if (typeConfig.Type != fieldInfo.DeclaringType)
        {
            fieldInfo = fieldInfo.DeclaringType.GetField(fieldInfo.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        return ReflectionOptimizer.Instance.CreateSetter(fieldInfo);
    }
}

/// <summary>
/// Class DeserializeTypeExensions.
/// </summary>
public static class DeserializeTypeExensions
{
    /// <summary>
    /// Determines whether [has] [the specified flag].
    /// </summary>
    /// <param name="flags">The flags.</param>
    /// <param name="flag">The flag.</param>
    /// <returns><c>true</c> if [has] [the specified flag]; otherwise, <c>false</c>.</returns>
    public static bool Has(this ParseAsType flags, ParseAsType flag)
    {
        return (flag & flags) != 0;
    }

    /// <summary>
    /// Parses the number.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public static object ParseNumber(this ReadOnlySpan<char> value)
    {
        return ParseNumber(value, JsConfig.TryParseIntoBestFit);
    }

    /// <summary>
    /// Parses the number.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="bestFit">if set to <c>true</c> [best fit].</param>
    /// <returns>System.Object.</returns>
    public static object ParseNumber(this ReadOnlySpan<char> value, bool bestFit)
    {
        if (value.Length == 1)
        {
            int singleDigit = value[0];
            if (singleDigit >= 48 || singleDigit <= 57) // 0 - 9
            {
                var result = singleDigit - 48;
                if (bestFit)
                {
                    return (byte)result;
                }

                return result;
            }
        }

        var config = JsConfig.GetConfig();

        // Parse as decimal
        var acceptDecimal = config.ParsePrimitiveFloatingPointTypes.Has(ParseAsType.Decimal);
        var isDecimal = value.TryParseDecimal(out var decimalValue);

        switch (isDecimal)
        {
            // Check if the number is an Primitive Integer type given that we have a decimal
            case true when decimalValue == decimal.Truncate(decimalValue):
                {
                    // Value is a whole number
                    var parseAs = config.ParsePrimitiveIntegerTypes;
                    if (parseAs.Has(ParseAsType.Byte) && decimalValue is <= byte.MaxValue and >= byte.MinValue)
                    {
                        return (byte)decimalValue;
                    }

                    if (parseAs.Has(ParseAsType.SByte) && decimalValue is <= sbyte.MaxValue and >= sbyte.MinValue)
                    {
                        return (sbyte)decimalValue;
                    }

                    if (parseAs.Has(ParseAsType.Int16) && decimalValue is <= short.MaxValue and >= short.MinValue)
                    {
                        return (short)decimalValue;
                    }

                    if (parseAs.Has(ParseAsType.UInt16) && decimalValue is <= ushort.MaxValue and >= ushort.MinValue)
                    {
                        return (ushort)decimalValue;
                    }

                    if (parseAs.Has(ParseAsType.Int32) && decimalValue is <= int.MaxValue and >= int.MinValue)
                    {
                        return (int)decimalValue;
                    }

                    if (parseAs.Has(ParseAsType.UInt32) && decimalValue is <= uint.MaxValue and >= uint.MinValue)
                    {
                        return (uint)decimalValue;
                    }

                    if (parseAs.Has(ParseAsType.Int64) && decimalValue is <= long.MaxValue and >= long.MinValue)
                    {
                        return (long)decimalValue;
                    }

                    if (parseAs.Has(ParseAsType.UInt64) && decimalValue is <= ulong.MaxValue and >= ulong.MinValue)
                    {
                        return (ulong)decimalValue;
                    }

                    return decimalValue;
                }
            // Value is a floating point number
            // Return a decimal if the user accepts a decimal
            case true when acceptDecimal:
                return decimalValue;
        }

        var acceptFloat = config.ParsePrimitiveFloatingPointTypes.HasFlag(ParseAsType.Single);
        var isFloat = value.TryParseFloat(out var floatValue);
        if (acceptFloat && isFloat)
        {
            return floatValue;
        }

        var acceptDouble = config.ParsePrimitiveFloatingPointTypes.HasFlag(ParseAsType.Double);
        var isDouble = value.TryParseDouble(out var doubleValue);
        if (acceptDouble && isDouble)
        {
            return doubleValue;
        }

        if (isDecimal)
        {
            return decimalValue;
        }

        if (isFloat)
        {
            return floatValue;
        }

        if (isDouble)
        {
            return doubleValue;
        }

        return null;
    }
}