// ***********************************************************************
// <copyright file="SpecialConverters.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Runtime.Serialization;

using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite.Converters;

/// <summary>
/// Enum EnumKind
/// </summary>
public enum EnumKind
{
    /// <summary>
    /// The string
    /// </summary>
    String,

    /// <summary>
    /// The int
    /// </summary>
    Int,

    /// <summary>
    /// The character
    /// </summary>
    Char,

    /// <summary>
    /// The enum member
    /// </summary>
    EnumMember
}

/// <summary>
/// Class EnumConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.StringConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.StringConverter" />
public class EnumConverter : StringConverter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnumConverter" /> class.
    /// </summary>
    public EnumConverter() : base(255) { }

    /// <summary>
    /// The enum type cache
    /// </summary>
    static Dictionary<Type, EnumKind> enumTypeCache = [];

    /// <summary>
    /// Gets the kind of the enum.
    /// </summary>
    /// <param name="enumType">Type of the enum.</param>
    /// <returns>EnumKind.</returns>
    public static EnumKind GetEnumKind(Type enumType)
    {
        if (enumTypeCache.TryGetValue(enumType, out var enumKind))
        {
            return enumKind;
        }

        enumKind = IsIntEnum(enumType)
            ? EnumKind.Int
            : enumType.HasAttributeCached<EnumAsCharAttribute>()
                ? EnumKind.Char
                : HasEnumMembers(enumType)
                    ? EnumKind.EnumMember
                    : EnumKind.String;

        Dictionary<Type, EnumKind> snapshot, newCache;
        do
        {
            snapshot = enumTypeCache;
            newCache = new Dictionary<Type, EnumKind>(enumTypeCache)
            {
                [enumType] = enumKind
            };
        } while (!ReferenceEquals(
                     System.Threading.Interlocked.CompareExchange(ref enumTypeCache, newCache, snapshot), snapshot));

        return enumKind;
    }

    /// <summary>
    /// Initializes the database parameter.
    /// </summary>
    /// <param name="p">The p.</param>
    /// <param name="fieldType">Type of the field.</param>
    public override void InitDbParam(IDbDataParameter p, Type fieldType)
    {
        var enumKind = GetEnumKind(fieldType);

        p.DbType = enumKind == EnumKind.Int
            ? Enum.GetUnderlyingType(fieldType) == typeof(long)
                ? DbType.Int64
                : DbType.Int32
            : this.DbType;
    }

    /// <summary>
    /// Quoted Value in SQL Statement
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public override string ToQuotedString(Type fieldType, object value)
    {
        var enumKind = GetEnumKind(fieldType);
        if (enumKind == EnumKind.Int)
        {
            return this.ConvertNumber(Enum.GetUnderlyingType(fieldType), value).ToString();
        }

        if (enumKind == EnumKind.Char)
        {
            return this.DialectProvider.GetQuotedValue(ToCharValue(value).ToString());
        }

        var isEnumFlags = fieldType.IsEnumFlags() ||
                          !fieldType.IsEnum && fieldType.IsNumericType(); //i.e. is real int && not Enum

        if (!isEnumFlags && long.TryParse(value.ToString(), out var enumValue))
        {
            value = Enum.ToObject(fieldType, enumValue);
        }

        var enumString = enumKind == EnumKind.EnumMember
            ? value.ToString()
            : this.DialectProvider.StringSerializer.SerializeToString(value);
        if (enumString is null or "null")
        {
            enumString = value.ToString();
        }

        return !isEnumFlags
            ? this.DialectProvider.GetQuotedValue(enumString.Trim('"'))
            : enumString;
    }

    /// <summary>
    /// Parameterized value in parameterized queries
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object ToDbValue(Type fieldType, object value)
    {
        var enumKind = GetEnumKind(fieldType);

        if (value.GetType().IsEnum)
        {
            if (enumKind == EnumKind.Int)
            {
                return Convert.ChangeType(value, Enum.GetUnderlyingType(fieldType));
            }

            if (enumKind == EnumKind.Char)
            {
                return Convert.ChangeType(value, typeof(char));
            }
        }

        if (enumKind == EnumKind.Char)
        {
            var charValue = ToCharValue(value);
            return charValue;
        }

        if (long.TryParse(value.ToString(), out var enumValue))
        {
            if (enumKind == EnumKind.Int)
            {
                return enumValue;
            }

            value = Enum.ToObject(fieldType, enumValue);
        }

        if (enumKind == EnumKind.EnumMember) // Don't use serialized Enum Value
        {
            return value.ToString();
        }

        var enumString = this.DialectProvider.StringSerializer.SerializeToString(value);
        return enumString != null && enumString != "null"
            ? enumString.Trim('"')
            : value.ToString();
    }

    /// <summary>
    /// Converts to charvalue.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Char.</returns>
    public static char ToCharValue(object value)
    {
        var charValue = value switch
        {
            char c => c,
            string {Length: 1} s => s[0],
            int i => (char)i,
            _ => (char)Convert.ChangeType(value, typeof(char))
        };
        return charValue;
    }

    //cache expensive to calculate operation
    /// <summary>
    /// The int enums
    /// </summary>
    readonly static ConcurrentDictionary<Type, bool> intEnums = new();

    /// <summary>
    /// Determines whether [is int enum] [the specified field type].
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <returns><c>true</c> if [is int enum] [the specified field type]; otherwise, <c>false</c>.</returns>
    public static bool IsIntEnum(Type fieldType)
    {
        var isIntEnum = intEnums.GetOrAdd(fieldType, type =>
            type.IsEnumFlags() ||
            type.HasAttributeCached<EnumAsIntAttribute>() ||
            !type.IsEnum &&
            type.IsNumericType()); //i.e. is real int && not Enum)

        return isIntEnum;
    }

    /// <summary>
    /// Determines whether [has enum members] [the specified enum type].
    /// </summary>
    /// <param name="enumType">Type of the enum.</param>
    /// <returns><c>true</c> if [has enum members] [the specified enum type]; otherwise, <c>false</c>.</returns>
    public static bool HasEnumMembers(Type enumType)
    {
        var enumMembers = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (var fi in enumMembers)
        {
            var enumMemberAttr = fi.FirstAttribute<EnumMemberAttribute>();
            if (enumMemberAttr?.Value != null)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Froms the database value.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object FromDbValue(Type fieldType, object value)
    {
        var enumKind = GetEnumKind(fieldType);

        if (enumKind == EnumKind.Char)
        {
            return Enum.ToObject(fieldType, (int)ToCharValue(value));
        }

        if (value is string strVal)
        {
            if (string.IsNullOrEmpty(strVal))
            {
                return Enum.ToObject(fieldType, 0);
            }

            return Enum.TryParse(fieldType, strVal, ignoreCase: true, out var ret)
                ? ret
                : TypeSerializer.DeserializeFromString(strVal, fieldType);
        }

        return Enum.ToObject(fieldType, value);
    }
}

/// <summary>
/// Class RowVersionConverter.
/// Implements the <see cref="ServiceStack.OrmLite.OrmLiteConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.OrmLiteConverter" />
public class RowVersionConverter : OrmLiteConverter
{
    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "BIGINT";

    /// <summary>
    /// Used in DB Params. Defaults to DbType.String
    /// </summary>
    /// <value>The type of the database.</value>
    public override DbType DbType => DbType.Int64;

    /// <summary>
    /// Value from DB to Populate on POCO Data Model with
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="DiagnosticEvent.Exception">Rowversion property must be declared as either byte[] or ulong</exception>
    /// <exception cref="System.Exception">Rowversion property must be declared as either byte[] or ulong</exception>
    public override object FromDbValue(Type fieldType, object value)
    {
        if (value is byte[] bytes)
        {
            if (fieldType == typeof(byte[]))
            {
                return bytes;
            }

            if (fieldType == typeof(ulong))
            {
                return OrmLiteUtils.ConvertToULong(bytes);
            }

            // an SQL row version has to be declared as either byte[] OR ulong...
            throw new Exception("Rowversion property must be declared as either byte[] or ulong");
        }

        return value != null
            ? this.ConvertNumber(typeof(ulong), value)
            : null;
    }
}

/// <summary>
/// Class ReferenceTypeConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.StringConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.StringConverter" />
public class ReferenceTypeConverter : StringConverter
{
    /// <summary>
    /// Gets the column definition.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => this.DialectProvider.GetStringConverter().MaxColumnDefinition;

    /// <summary>
    /// Gets the maximum column definition.
    /// </summary>
    /// <value>The maximum column definition.</value>
    public override string MaxColumnDefinition => this.DialectProvider.GetStringConverter().MaxColumnDefinition;

    /// <summary>
    /// Gets the column definition.
    /// </summary>
    /// <param name="stringLength">Length of the string.</param>
    /// <returns>System.String.</returns>
    public override string GetColumnDefinition(int? stringLength)
    {
        return stringLength != null
            ? base.GetColumnDefinition(stringLength)
            : this.MaxColumnDefinition;
    }

    /// <summary>
    /// Quoted Value in SQL Statement
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public override string ToQuotedString(Type fieldType, object value)
    {
        return this.DialectProvider.GetQuotedValue(this.DialectProvider.StringSerializer.SerializeToString(value));
    }

    /// <summary>
    /// Parameterized value in parameterized queries
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object ToDbValue(Type fieldType, object value)
    {
        //Let ADO.NET providers handle byte[]
        return fieldType == typeof(byte[])
            ? value
            : this.DialectProvider.StringSerializer.SerializeToString(value);
    }

    /// <summary>
    /// Froms the database value.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object FromDbValue(Type fieldType, object value)
    {
        if (value is string str)
        {
            return this.DialectProvider.StringSerializer.DeserializeFromString(str, fieldType);
        }

        var convertedValue = value.ConvertTo(fieldType);
        return convertedValue;
    }
}

/// <summary>
/// Class ValueTypeConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.StringConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.StringConverter" />
public class ValueTypeConverter : StringConverter
{
    /// <summary>
    /// Gets the column definition.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => this.DialectProvider.GetStringConverter().MaxColumnDefinition;
    /// <summary>
    /// Gets the maximum column definition.
    /// </summary>
    /// <value>The maximum column definition.</value>
    public override string MaxColumnDefinition => this.DialectProvider.GetStringConverter().MaxColumnDefinition;

    /// <summary>
    /// Gets the column definition.
    /// </summary>
    /// <param name="stringLength">Length of the string.</param>
    /// <returns>System.String.</returns>
    public override string GetColumnDefinition(int? stringLength)
    {
        return stringLength != null
            ? base.GetColumnDefinition(stringLength)
            : this.MaxColumnDefinition;
    }

    /// <summary>
    /// Quoted Value in SQL Statement
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public override string ToQuotedString(Type fieldType, object value)
    {
        return this.DialectProvider.GetQuotedValue(this.DialectProvider.StringSerializer.SerializeToString(value));
    }

    /// <summary>
    /// Parameterized value in parameterized queries
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object ToDbValue(Type fieldType, object value)
    {
        var convertedValue = this.DialectProvider.StringSerializer.DeserializeFromString(value.ToString(), fieldType);
        return convertedValue;
    }

    /// <summary>
    /// Froms the database value.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object FromDbValue(Type fieldType, object value)
    {
        if (fieldType.IsInstanceOfType(value))
        {
            return value;
        }

        var convertedValue = this.DialectProvider.StringSerializer.DeserializeFromString(value.ToString(), fieldType);
        return convertedValue;
    }
}