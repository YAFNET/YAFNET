// ***********************************************************************
// <copyright file="StringExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using ServiceStack.OrmLite.Base.Common;
using ServiceStack.OrmLite.Base.Text.Common;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Class StringExtensions.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Encodes the JSV.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>string.</returns>
    public static string EncodeJsv(this string value)
    {
        if (JsState.QueryStringMode)
        {
            return UrlEncode(value);
        }
        return string.IsNullOrEmpty(value) || !JsWriter.HasAnyEscapeChars(value)
                   ? value
                   : string.Concat
                   (
                       JsWriter.QuoteString,
                       value.Replace(JsWriter.QuoteString, TypeSerializer.DoubleQuoteString),
                       JsWriter.QuoteString
                   );
    }

    /// <summary>
    /// URLs the encode.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="upperCase">The upper case.</param>
    /// <returns>string.</returns>
    public static string UrlEncode(this string text, bool upperCase = false)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        var sb = StringBuilderThreadStatic.Allocate();
        var fmt = upperCase ? "X2" : "x2";

        foreach (var charCode in Encoding.UTF8.GetBytes(text))
        {
            switch (charCode)
            {
                case >= 65 and <= 90:
                case >= 97 and <= 122:
                case >= 48 and <= 57:
                // A-Z
                // a-z
                // 0-9
                // ,-.
                case >= 44 and <= 46:
                    sb.Append((char)charCode);
                    break;
                case 32:
                    sb.Append('+');
                    break;
                default:
                    sb.Append('%' + charCode.ToString(fmt));
                    break;
            }
        }

        return StringBuilderThreadStatic.ReturnAndFree(sb);
    }


    /// <summary>
    /// Froms the UTF8 bytes.
    /// </summary>
    /// <param name="bytes">The bytes.</param>
    /// <returns>string.</returns>
    public static string FromUtf8Bytes(this byte[] bytes)
    {
        return bytes == null ? null
                   : bytes.Length > 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF
                       ? Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3)
                       : Encoding.UTF8.GetString(bytes, 0, bytes.Length);
    }

    /// <summary>
    /// Converts to utf8bytes.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>byte[].</returns>
    public static byte[] ToUtf8Bytes(this string value)
    {
        return Encoding.UTF8.GetBytes(value);
    }

    /// <summary>
    /// Lefts the part.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <returns>string.</returns>
    public static string LeftPart(this string strVal, char needle)
    {
        if (strVal == null)
        {
            return null;
        }

        var pos = strVal.IndexOf(needle);
        return pos == -1
                   ? strVal
                   : strVal[..pos];
    }

    /// <summary>
    /// Lefts the part.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <returns>string.</returns>
    public static string LeftPart(this string strVal, string needle)
    {
        if (strVal == null)
        {
            return null;
        }

        var pos = strVal.IndexOf(needle, StringComparison.OrdinalIgnoreCase);
        return pos == -1
                   ? strVal
                   : strVal[..pos];
    }

    /// <summary>
    /// Rights the part.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <returns>string.</returns>
    public static string RightPart(this string strVal, char needle)
    {
        if (strVal == null)
        {
            return null;
        }

        var pos = strVal.IndexOf(needle);
        return pos == -1
                   ? strVal
                   : strVal[(pos + 1)..];
    }

    /// <summary>
    /// Rights the part.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <returns>string.</returns>
    public static string RightPart(this string strVal, string needle)
    {
        if (strVal == null)
        {
            return null;
        }

        var pos = strVal.IndexOf(needle, StringComparison.OrdinalIgnoreCase);
        return pos == -1
                   ? strVal
                   : strVal[(pos + needle.Length)..];
    }

    /// <summary>
    /// Lasts the left part.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <returns>string.</returns>
    public static string LastLeftPart(this string strVal, char needle)
    {
        if (strVal == null)
        {
            return null;
        }

        var pos = strVal.LastIndexOf(needle);
        return pos == -1
                   ? strVal
                   : strVal[..pos];
    }

    /// <summary>
    /// Lasts the right part.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <returns>string.</returns>
    public static string LastRightPart(this string strVal, char needle)
    {
        if (strVal == null)
        {
            return null;
        }

        var pos = strVal.LastIndexOf(needle);
        return pos == -1
                   ? strVal
                   : strVal[(pos + 1)..];
    }

    /// <summary>
    /// Lasts the right part.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <returns>string.</returns>
    public static string LastRightPart(this string strVal, string needle)
    {
        if (strVal == null)
        {
            return null;
        }

        var pos = strVal.LastIndexOf(needle, StringComparison.OrdinalIgnoreCase);
        return pos == -1
                   ? strVal
                   : strVal[(pos + needle.Length)..];
    }

    /// <summary>
    /// Splits the on first.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <returns>string[].</returns>
    public static string[] SplitOnFirst(this string strVal, char needle)
    {
        if (strVal == null)
        {
            return TypeConstants.EmptyStringArray;
        }

        var pos = strVal.IndexOf(needle);
        return pos == -1
                   ? [strVal]
                   : [strVal[..pos], strVal[(pos + 1)..]];
    }

    /// <summary>
    /// Splits the on first.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <returns>string[].</returns>
    public static string[] SplitOnFirst(this string strVal, string needle)
    {
        if (strVal == null)
        {
            return TypeConstants.EmptyStringArray;
        }

        var pos = strVal.IndexOf(needle, StringComparison.OrdinalIgnoreCase);
        return pos == -1
                   ? [strVal]
                   : [strVal[..pos], strVal[(pos + needle.Length)..]];
    }

    /// <summary>
    /// Splits the on last.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <returns>string[].</returns>
    public static string[] SplitOnLast(this string strVal, char needle)
    {
        if (strVal == null)
        {
            return TypeConstants.EmptyStringArray;
        }

        var pos = strVal.LastIndexOf(needle);
        return pos == -1
                   ? [strVal]
                   : [strVal[..pos], strVal[(pos + 1)..]];
    }

    /// <summary>
    /// Splits the on last.
    /// </summary>
    /// <param name="strVal">The string value.</param>
    /// <param name="needle">The needle.</param>
    /// <returns>string[].</returns>
    public static string[] SplitOnLast(this string strVal, string needle)
    {
        if (strVal == null)
        {
            return TypeConstants.EmptyStringArray;
        }

        var pos = strVal.LastIndexOf(needle, StringComparison.OrdinalIgnoreCase);
        return pos == -1
                   ? [strVal]
                   : [strVal[..pos], strVal[(pos + needle.Length)..]];
    }

    /// <summary>
    /// Withouts the extension.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns>string.</returns>
    public static string WithoutExtension(this string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return null;
        }

        var extPos = filePath.LastIndexOf('.');
        if (extPos == -1)
        {
            return filePath;
        }

        var dirPos = filePath.LastIndexOfAny(PclExport.DirSeps);
        return extPos > dirPos ? filePath[..extPos] : filePath;
    }

    /// <summary>
    /// Converts to jsv.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">The object.</param>
    /// <returns>string.</returns>
    public static string ToJsv<T>(this T obj)
    {
        return TypeSerializer.SerializeToString(obj);
    }

    /// <summary>
    /// Froms the JSV.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="jsv">The JSV.</param>
    /// <returns>T.</returns>
    public static T FromJsv<T>(this string jsv)
    {
        return TypeSerializer.DeserializeFromString<T>(jsv);
    }

    /// <summary>
    /// Froms the json.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json">The json.</param>
    /// <returns>T.</returns>
    public static T FromJson<T>(this string json)
    {
        return JsonSerializer.DeserializeFromString<T>(json);
    }

    /// <summary>
    /// Converts to csv.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">The object.</param>
    /// <returns>string.</returns>
    public static string ToCsv<T>(this T obj)
    {
        return CsvSerializer.SerializeToString(obj);
    }

    /// <summary>
    /// Converts to csv.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">The object.</param>
    /// <param name="configure">The configure.</param>
    /// <returns>System.String.</returns>
    public static string ToCsv<T>(this T obj, Action<Config> configure)
    {
        var config = new Config();
        configure(config);
        using (JsConfig.With(config))
        {
            return ToCsv(obj);
        }
    }

    /// <summary>
    /// Froms the CSV.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="csv">The CSV.</param>
    /// <returns>T.</returns>
    public static T FromCsv<T>(this string csv)
    {
        return CsvSerializer.DeserializeFromString<T>(csv);
    }

    /// <summary>
    /// Formats the with.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>string.</returns>
    public static string FormatWith(this string text, params object[] args)
    {
        return string.Format(text, args);
    }

    /// <summary>
    /// FMTs the specified text.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="arg1">The arg1.</param>
    /// <returns>string.</returns>
    public static string Fmt(this string text, object arg1)
    {
        return string.Format(text, arg1);
    }

    /// <summary>
    /// FMTs the specified text.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="arg1">The arg1.</param>
    /// <param name="arg2">The arg2.</param>
    /// <returns>string.</returns>
    public static string Fmt(this string text, object arg1, object arg2)
    {
        return string.Format(text, arg1, arg2);
    }

    /// <summary>
    /// FMTs the specified text.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="arg1">The arg1.</param>
    /// <param name="arg2">The arg2.</param>
    /// <param name="arg3">The arg3.</param>
    /// <returns>string.</returns>
    public static string Fmt(this string text, object arg1, object arg2, object arg3)
    {
        return string.Format(text, arg1, arg2, arg3);
    }

    /// <summary>
    /// Starts the with ignore case.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="startsWith">The starts with.</param>
    /// <returns>bool.</returns>
    public static bool StartsWithIgnoreCase(this string text, string startsWith)
    {
        return text != null
               && text.StartsWith(startsWith, PclExport.Instance.InvariantComparisonIgnoreCase);
    }

    /// <summary>
    /// Ends the with ignore case.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="endsWith">The ends with.</param>
    /// <returns>bool.</returns>
    public static bool EndsWithIgnoreCase(this string text, string endsWith)
    {
        return text != null
               && text.EndsWith(endsWith, PclExport.Instance.InvariantComparisonIgnoreCase);
    }

    /// <summary>
    /// Files the exists.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns>bool.</returns>
    public static bool FileExists(this string filePath)
    {
        return PclExport.Instance.FileExists(filePath);
    }

    /// <summary>
    /// Creates the directory.
    /// </summary>
    /// <param name="dirPath">The dir path.</param>
    public static void CreateDirectory(this string dirPath)
    {
        PclExport.Instance.CreateDirectory(dirPath);
    }

    /// <summary>
    /// Indexes the of any.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="needles">The needles.</param>
    /// <returns>int.</returns>
    public static int IndexOfAny(this string text, params string[] needles)
    {
        return IndexOfAny(text, 0, needles);
    }

    /// <summary>
    /// Indexes the of any.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="startIndex">The start index.</param>
    /// <param name="needles">The needles.</param>
    /// <returns>int.</returns>
    public static int IndexOfAny(this string text, int startIndex, params string[] needles)
    {
        var firstPos = -1;
        if (text != null)
        {
            foreach (var needle in needles)
            {
                var pos = text.IndexOf(needle, startIndex, StringComparison.Ordinal);
                if (pos >= 0 && (firstPos == -1 || pos < firstPos))
                {
                    firstPos = pos;
                }
            }
        }

        return firstPos;
    }

    /// <summary>
    /// Strips the quotes.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>string.</returns>
    public static string StripQuotes(this string text)
    {
        return string.IsNullOrEmpty(text) || text.Length < 2
                   ? text
                   : text[0] == '"' && text[^1] == '"' ||
                     text[0] == '\'' && text[^1] == '\'' ||
                     text[0] == '`' && text[^1] == '`'
                       ? text[1..^1]
                       : text;
    }

    /// <summary>
    /// The lower case offset
    /// </summary>
    private const int LowerCaseOffset = 'a' - 'A';
    /// <summary>
    /// Converts to camelcase.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>string.</returns>
    public static string ToCamelCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        var len = value.Length;
        var newValue = new char[len];
        var firstPart = true;

        for (var i = 0; i < len; ++i)
        {
            var c0 = value[i];
            var c1 = i < len - 1 ? value[i + 1] : 'A';
            var c0isUpper = c0 is >= 'A' and <= 'Z';
            var c1isUpper = c1 is >= 'A' and <= 'Z';

            if (firstPart && c0isUpper && (c1isUpper || i == 0))
            {
                c0 = (char)(c0 + LowerCaseOffset);
            }
            else
            {
                firstPart = false;
            }

            newValue[i] = c0;
        }

        return new string(newValue);
    }

    /// <summary>
    /// Converts to pascalcase.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>string.</returns>
    public static string ToPascalCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        if (value.Contains('_'))
        {
            var parts = value.Split('_');
            var sb = StringBuilderThreadStatic.Allocate();
            foreach (var part in parts)
            {
                if (string.IsNullOrEmpty(part))
                {
                    continue;
                }

                var str = part.ToCamelCase();
                sb.Append(char.ToUpper(str[0]) + str.SafeSubstring(1, str.Length));
            }
            return StringBuilderThreadStatic.ReturnAndFree(sb);
        }

        var camelCase = value.ToCamelCase();
        return char.ToUpper(camelCase[0]) + camelCase.SafeSubstring(1, camelCase.Length);
    }

    /// <summary>
    /// Converts to titlecase.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>string.</returns>
    public static string ToTitleCase(this string value)
    {
        return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value).Replace("_", string.Empty);
    }

    /// <summary>
    /// Converts to uppercaseunderscore.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>string.</returns>
    public static string ToUppercaseUnderscore(this string value) => value.ToLowercaseUnderscore().ToUpper();

    /// <summary>
    /// Converts to kebabcase.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>string.</returns>
    public static string ToKebabCase(this string value) => value.ToLowercaseUnderscore().Replace("_", "-");

    /// <summary>
    /// Converts to lowercaseunderscore.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>string.</returns>
    public static string ToLowercaseUnderscore(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        value = value.ToCamelCase();

        var sb = StringBuilderThreadStatic.Allocate();
        foreach (var t in value)
        {
            if (char.IsDigit(t) || char.IsLetter(t) && char.IsLower(t) || t == '_')
            {
                sb.Append(t);
            }
            else
            {
                sb.Append('_');
                sb.Append(char.ToLower(t));
            }
        }
        return StringBuilderThreadStatic.ReturnAndFree(sb);
    }

    /// <summary>
    /// Safes the substring.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="startIndex">The start index.</param>
    /// <param name="length">The length.</param>
    /// <returns>string.</returns>
    public static string SafeSubstring(this string value, int startIndex, int length)
    {
        if (string.IsNullOrEmpty(value) || length <= 0)
        {
            return string.Empty;
        }

        if (startIndex < 0)
        {
            startIndex = 0;
        }

        if (value.Length >= startIndex + length)
        {
            return value.Substring(startIndex, length);
        }

        return value.Length > startIndex ? value[startIndex..] : string.Empty;
    }

    /// <summary>
    /// Substrings the with ellipsis.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="startIndex">The start index.</param>
    /// <param name="length">The length.</param>
    /// <returns>string.</returns>
    public static string SubstringWithEllipsis(this string value, int startIndex, int length)
    {
        var str = value.SafeSubstring(startIndex, length);
        return str.Length == length
                   ? str + "..."
                   : str;
    }

    /// <summary>
    /// Compares the ignore case.
    /// </summary>
    /// <param name="strA">The string a.</param>
    /// <param name="strB">The string b.</param>
    /// <returns>int.</returns>
    public static int CompareIgnoreCase(this string strA, string strB)
    {
        return string.Compare(strA, strB, PclExport.Instance.InvariantComparisonIgnoreCase);
    }

    /// <summary>
    /// Endses the with invariant.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="endsWith">The ends with.</param>
    /// <returns>bool.</returns>
    public static bool EndsWithInvariant(this string str, string endsWith)
    {
        return str.EndsWith(endsWith, PclExport.Instance.InvariantComparison);
    }

    /// <summary>
    /// The invalid variable chars regex
    /// </summary>
    private readonly static Regex InvalidVarCharsRegex = new(@"[^A-Za-z0-9_]", RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(100));

    /// <summary>
    /// Determines whether the specified value is empty.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>bool.</returns>
    public static bool IsEmpty(this string value)
    {
        return string.IsNullOrEmpty(value);
    }

    /// <summary>
    /// Determines whether [is null or empty] [the specified value].
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>bool.</returns>
    public static bool IsNullOrEmpty(this string value)
    {
        return string.IsNullOrEmpty(value);
    }

    /// <summary>
    /// Equalses the ignore case.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="other">The other.</param>
    /// <returns>bool.</returns>
    public static bool EqualsIgnoreCase(this string value, string other)
    {
        return string.Equals(value, other, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Replaces the first.
    /// </summary>
    /// <param name="haystack">The haystack.</param>
    /// <param name="needle">The needle.</param>
    /// <param name="replacement">The replacement.</param>
    /// <returns>string.</returns>
    public static string ReplaceFirst(this string haystack, string needle, string replacement)
    {
        var pos = haystack.IndexOf(needle, StringComparison.Ordinal);
        if (pos < 0)
        {
            return haystack;
        }

        return haystack[..pos] + replacement + haystack[(pos + needle.Length)..];
    }

    /// <summary>
    /// Determines whether the specified text contains any.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="testMatches">The test matches.</param>
    /// <returns>bool.</returns>
    public static bool ContainsAny(this string text, params string[] testMatches)
    {
        return testMatches.Exists(text.Contains);
    }

    /// <summary>
    /// Determines whether the specified text contains any.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="testMatches">The test matches.</param>
    /// <param name="comparisonType">Type of the comparison.</param>
    /// <returns>bool.</returns>
    public static bool ContainsAny(this string text, string[] testMatches, StringComparison comparisonType)
    {
        return testMatches.Exists(testMatch => text.Contains(testMatch, comparisonType));
    }

    /// <summary>
    /// Safes the name of the variable.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>string.</returns>
    public static string SafeVarName(this string text)
    {
        return !string.IsNullOrEmpty(text)
            ? InvalidVarCharsRegex.Replace(text, "_")
            : null;
    }

    /// <summary>
    /// Joins the specified items.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <returns>string.</returns>
    public static string Join(this List<string> items)
    {
        return string.Join(JsWriter.ItemSeperatorString, [.. items]);
    }

    /// <summary>
    /// Joins the specified items.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="delimeter">The delimeter.</param>
    /// <returns>string.</returns>
    public static string Join(this List<string> items, string delimeter)
    {
        return string.Join(delimeter, [.. items]);
    }

    /// <summary>
    /// The system type chars
    /// </summary>
    private readonly static char[] SystemTypeChars = ['<', '>', '+'];

    /// <summary>
    /// Determines whether [is user type] [the specified type].
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>bool.</returns>
    public static bool IsUserType(this Type type)
    {
        return type.IsClass
               && !type.IsSystemType();
    }

    /// <summary>
    /// Determines whether [is system type] [the specified type].
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>bool.</returns>
    public static bool IsSystemType(this Type type)
    {
        return type.Namespace == null
               || type.Namespace.StartsWith("System")
               || type.Name.IndexOfAny(SystemTypeChars) >= 0;
    }

    /// <summary>
    /// Determines whether the specified type is tuple.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>bool.</returns>
    public static bool IsTuple(this Type type)
    {
        return type.Name.StartsWith("Tuple`");
    }

    /// <summary>
    /// Determines whether the specified text is int.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>bool.</returns>
    public static bool IsInt(this string text)
    {
        return !string.IsNullOrEmpty(text) && int.TryParse(text, out _);
    }

    /// <summary>
    /// Converts the string representation of a number to an integer.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>int.</returns>
    public static int ToInt(this string text)
    {
        return text == null ? 0 : int.Parse(text);
    }

    /// <summary>
    /// Converts the string representation of a number to an integer.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>int.</returns>
    public static int ToInt(this string text, int defaultValue)
    {
        return int.TryParse(text, out var ret) ? ret : defaultValue;
    }

    /// <summary>
    /// Converts to long.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>long.</returns>
    public static long ToLong(this string text)
    {
        return long.Parse(text);
    }

    /// <summary>
    /// Converts to long.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>long.</returns>
    public static long ToLong(this string text, long defaultValue)
    {
        return long.TryParse(text, out var ret) ? ret : defaultValue;
    }

    /// <summary>
    /// Converts to double.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>double.</returns>
    public static double ToDouble(this string text)
    {
        return text == null ? 0 : double.Parse(text);
    }

    /// <summary>
    /// Converts to double.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>double.</returns>
    public static double ToDouble(this string text, double defaultValue)
    {
        return double.TryParse(text, out var ret) ? ret : defaultValue;
    }

    /// <summary>
    /// Converts to decimal.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>decimal.</returns>
    public static decimal ToDecimal(this string text)
    {
        return text == null ? 0 : decimal.Parse(text);
    }

    /// <summary>
    /// Converts to decimal.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>decimal.</returns>
    public static decimal ToDecimal(this string text, decimal defaultValue)
    {
        return decimal.TryParse(text, out var ret) ? ret : defaultValue;
    }

    /// <summary>
    /// Matcheses the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="pattern">The pattern.</param>
    /// <returns>bool.</returns>
    public static bool Matches(this string value, string pattern)
    {
        return value.Glob(pattern);
    }

    /// <summary>
    /// Globs the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="pattern">The pattern.</param>
    /// <returns>bool.</returns>
    public static bool Glob(this string value, string pattern)
    {
        int pos;
        for (pos = 0; pattern.Length != pos; pos++)
        {
            switch (pattern[pos])
            {
                case '?':
                    break;

                case '*':
                    for (var i = value.Length; i >= pos; i--)
                    {
                        if (Glob(value[i..], pattern[(pos + 1)..]))
                        {
                            return true;
                        }
                    }
                    return false;

                default:
                    if (value.Length == pos || char.ToUpper(pattern[pos]) != char.ToUpper(value[pos]))
                    {
                        return false;
                    }
                    break;
            }
        }

        return value.Length == pos;
    }

    /// <summary>
    /// Trims the prefixes.
    /// </summary>
    /// <param name="fromString">From string.</param>
    /// <param name="prefixes">The prefixes.</param>
    /// <returns>string.</returns>
    public static string TrimPrefixes(this string fromString, params string[] prefixes)
    {
        if (string.IsNullOrEmpty(fromString))
        {
            return fromString;
        }

        foreach (var prefix in prefixes)
        {
            if (fromString.StartsWith(prefix))
            {
                return fromString[prefix.Length..];
            }
        }

        return fromString;
    }

    /// <summary>
    /// Reads the lines.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>System.Collections.Generic.IEnumerable&lt;string&gt;.</returns>
    public static IEnumerable<string> ReadLines(this string text)
    {
        var reader = new StringReader(text ?? "");
        while (reader.ReadLine() is { } line)
        {
            yield return line;
        }
    }

    /// <summary>
    /// Converts to hex.
    /// </summary>
    /// <param name="hashBytes">The hash bytes.</param>
    /// <param name="upper">The upper.</param>
    /// <returns>string.</returns>
    public static string ToHex(this byte[] hashBytes, bool upper = false)
    {
        var len = hashBytes.Length * 2;
        var chars = new char[len];
        var index = 0;
        for (var i = 0; i < len; i += 2)
        {
            var b = hashBytes[index++];
            chars[i] = GetHexValue(b / 16, upper);
            chars[i + 1] = GetHexValue(b % 16, upper);
        }
        return new string(chars);
    }

    /// <summary>
    /// Gets the hexadecimal value.
    /// </summary>
    /// <param name="i">The i.</param>
    /// <param name="upper">The upper.</param>
    /// <returns>char.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">i - must be between 0 and 15</exception>
    private static char GetHexValue(int i, bool upper)
    {
        if (i < 0 || i > 15)
        {
            throw new ArgumentOutOfRangeException(nameof(i), "must be between 0 and 15");
        }

        return i < 10
                   ? (char)(i + '0')
                   : (char)(i - 10 + (upper ? 'A' : 'a'));
    }

}