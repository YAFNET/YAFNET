﻿// ***********************************************************************
// <copyright file="DeserializeBuiltin.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;

using ServiceStack.OrmLite.Base.Text.Json;

namespace ServiceStack.OrmLite.Base.Text.Common;

/// <summary>
/// Class DeserializeBuiltin.
/// </summary>
/// <typeparam name="T"></typeparam>
public static class DeserializeBuiltin<T>
{
    /// <summary>
    /// The cached parse function
    /// </summary>
    private readonly static ParseStringSpanDelegate CachedParseFn;

    /// <summary>
    /// Initializes static members of the <see cref="DeserializeBuiltin{T}" /> class.
    /// </summary>
    static DeserializeBuiltin()
    {
        CachedParseFn = GetParseStringSpanFn();
    }

    /// <summary>
    /// Gets the parse.
    /// </summary>
    /// <value>The parse.</value>
    public static ParseStringDelegate Parse => v => CachedParseFn(v.AsSpan());

    /// <summary>
    /// Gets the parse string span.
    /// </summary>
    /// <value>The parse string span.</value>
    public static ParseStringSpanDelegate ParseStringSpan => CachedParseFn;

    /// <summary>
    /// Gets the parse string span function.
    /// </summary>
    /// <returns>ParseStringSpanDelegate.</returns>
    private static ParseStringSpanDelegate GetParseStringSpanFn()
    {
        var nullableType = Nullable.GetUnderlyingType(typeof(T));
        if (nullableType == null)
        {
            var typeCode = typeof(T).GetTypeCode();
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return value => value.ParseBoolean();
                case TypeCode.SByte:
                    return SignedInteger<sbyte>.ParseObject;
                case TypeCode.Byte:
                    return UnsignedInteger<byte>.ParseObject;
                case TypeCode.Int16:
                    return SignedInteger<short>.ParseObject;
                case TypeCode.UInt16:
                    return UnsignedInteger<ushort>.ParseObject;
                case TypeCode.Int32:
                    return SignedInteger<int>.ParseObject;
                case TypeCode.UInt32:
                    return UnsignedInteger<uint>.ParseObject;
                case TypeCode.Int64:
                    return SignedInteger<long>.ParseObject;
                case TypeCode.UInt64:
                    return UnsignedInteger<ulong>.ParseObject;

                case TypeCode.Single:
                    return value => MemoryProvider.Instance.ParseFloat(value);
                case TypeCode.Double:
                    return value => MemoryProvider.Instance.ParseDouble(value);
                case TypeCode.Decimal:
                    return value => MemoryProvider.Instance.ParseDecimal(value);
                case TypeCode.DateTime:
                    return value => DateTimeSerializer.ParseShortestXsdDateTime(value.ToString());
                case TypeCode.Char:
                    return value => value.Length == 0 ? (char)0 : value.Length == 1 ? value[0] : JsonTypeSerializer.Unescape(value)[0];
            }

            if (typeof(T) == typeof(Guid))
            {
                return value => value.ParseGuid();
            }

            if (typeof(T) == typeof(DateTimeOffset))
            {
                return value => DateTimeSerializer.ParseDateTimeOffset(value.ToString());
            }

            if (typeof(T) == typeof(TimeSpan))
            {
                return value => DateTimeSerializer.ParseTimeSpan(value.ToString());
            }
#if NET9_0_OR_GREATER
                if (typeof(T) == typeof(DateOnly))
                {
                    return value => DateOnly.FromDateTime(DateTimeSerializer.ParseShortestXsdDateTime(value.ToString()));
                }

                if (typeof(T) == typeof(TimeOnly))
                {
                    return value => TimeOnly.FromTimeSpan(DateTimeSerializer.ParseTimeSpan(value.ToString()));
                }
#endif
        }
        else
        {
            var typeCode = nullableType.GetTypeCode();
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return value => value.IsNullOrEmpty()
                                        ? null
                                        : value.ParseBoolean();
                case TypeCode.SByte:
                    return SignedInteger<sbyte>.ParseNullableObject;
                case TypeCode.Byte:
                    return UnsignedInteger<byte>.ParseNullableObject;
                case TypeCode.Int16:
                    return SignedInteger<short>.ParseNullableObject;
                case TypeCode.UInt16:
                    return UnsignedInteger<ushort>.ParseNullableObject;
                case TypeCode.Int32:
                    return SignedInteger<int>.ParseNullableObject;
                case TypeCode.UInt32:
                    return UnsignedInteger<uint>.ParseNullableObject;
                case TypeCode.Int64:
                    return SignedInteger<long>.ParseNullableObject;
                case TypeCode.UInt64:
                    return UnsignedInteger<ulong>.ParseNullableObject;

                case TypeCode.Single:
                    return value => value.IsNullOrEmpty() ? null : value.ParseFloat();
                case TypeCode.Double:
                    return value => value.IsNullOrEmpty() ? null : value.ParseDouble();
                case TypeCode.Decimal:
                    return value => value.IsNullOrEmpty() ? null : value.ParseDecimal();
                case TypeCode.DateTime:
                    return value => DateTimeSerializer.ParseShortestNullableXsdDateTime(value.ToString());
                case TypeCode.Char:
                    return value => value.IsEmpty ? null : value.Length == 1 ? value[0] : JsonTypeSerializer.Unescape(value)[0];
            }

            if (typeof(T) == typeof(TimeSpan?))
            {
                return value => DateTimeSerializer.ParseNullableTimeSpan(value.ToString());
            }

            if (typeof(T) == typeof(Guid?))
            {
                return value => value.IsNullOrEmpty() ? null : value.ParseGuid();
            }

            if (typeof(T) == typeof(DateTimeOffset?))
            {
                return value => DateTimeSerializer.ParseNullableDateTimeOffset(value.ToString());
            }

            if (typeof(T) == typeof(DateOnly?))
            {
                return value => value.IsNullOrEmpty() ? default : DateOnly.FromDateTime(DateTimeSerializer.ParseShortestXsdDateTime(value.ToString()));
            }

            if (typeof(T) == typeof(TimeOnly?))
            {
                return value => value.IsNullOrEmpty() ? default : TimeOnly.FromTimeSpan(DateTimeSerializer.ParseTimeSpan(value.ToString()));
            }
        }

        return null;
    }
}