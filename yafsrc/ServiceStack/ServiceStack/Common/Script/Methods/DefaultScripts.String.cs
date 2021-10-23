// ***********************************************************************
// <copyright file="DefaultScripts.String.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ServiceStack.Script;
using ServiceStack.Text;

namespace ServiceStack.Script
{
    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Class DefaultScripts.
    /// Implements the <see cref="ServiceStack.Script.ScriptMethods" />
    /// Implements the <see cref="ServiceStack.Script.IConfigureScriptContext" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptMethods" />
    /// <seealso cref="ServiceStack.Script.IConfigureScriptContext" />
    public partial class DefaultScripts
    {
        /// <summary>
        /// Raws the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>IRawString.</returns>
        public IRawString raw(object value)
        {
            if (value == null || Equals(value, JsNull.Value))
                return ScriptConstants.EmptyRawString;
            if (value is string s)
                return s == string.Empty ? ScriptConstants.EmptyRawString : s.ToRawString();
            if (value is IRawString r)
                return r;
            if (value is bool b)
                return b ? ScriptConstants.TrueRawString : ScriptConstants.FalseRawString;

            var rawStr = value.ToString().ToRawString();
            return rawStr;
        }

        /// <summary>
        /// Applications the setting.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public string appSetting(string name) => Context.AppSettings.GetString(name);

        /// <summary>
        /// Indents this instance.
        /// </summary>
        /// <returns>System.String.</returns>
        public string indent() => Context.Args[ScriptConstants.DefaultIndent] as string;
        /// <summary>
        /// Indentses the specified count.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns>System.String.</returns>
        public string indents(int count) => repeat(Context.Args[ScriptConstants.DefaultIndent] as string, count);
        /// <summary>
        /// Spaces this instance.
        /// </summary>
        /// <returns>System.String.</returns>
        public string space() => " ";
        /// <summary>
        /// Spaceses the specified count.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns>System.String.</returns>
        public string spaces(int count) => padLeft("", count, ' ');
        /// <summary>
        /// News the line.
        /// </summary>
        /// <returns>System.String.</returns>
        public string newLine() => Context.Args[ScriptConstants.DefaultNewLine] as string;
        /// <summary>
        /// News the lines.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns>System.String.</returns>
        public string newLines(int count) => repeat(newLine(), count);
        /// <summary>
        /// News the line.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>System.String.</returns>
        public string newLine(string target) => target + newLine();

        /// <summary>
        /// Currencies the specified decimal value.
        /// </summary>
        /// <param name="decimalValue">The decimal value.</param>
        /// <returns>System.String.</returns>
        public string currency(decimal decimalValue) => currency(decimalValue, null); //required to support 1/2 vars
        /// <summary>
        /// Currencies the specified decimal value.
        /// </summary>
        /// <param name="decimalValue">The decimal value.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>System.String.</returns>
        public string currency(decimal decimalValue, string culture)
        {
            var cultureInfo = culture != null
                ? new CultureInfo(culture)
                : (CultureInfo)Context.Args[ScriptConstants.DefaultCulture];

            var fmt = string.Format(cultureInfo, "{0:C}", decimalValue);
            return fmt;
        }

        /// <summary>
        /// Formats the raw.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="fmt">The FMT.</param>
        /// <returns>IRawString.</returns>
        public IRawString formatRaw(object obj, string fmt) => raw(string.Format(fmt.Replace("{{", "{").Replace("}}", "}"), obj));

        /// <summary>
        /// Formats the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="format">The format.</param>
        /// <returns>System.String.</returns>
        public string format(object obj, string format) => obj is IFormattable formattable
            ? formattable.ToString(format, null)
            : string.Format(format, obj);

        /// <summary>
        /// FMTs the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg">The argument.</param>
        /// <returns>System.String.</returns>
        public string fmt(string format, object arg)
        {
            if (arg is object[] args)
                return string.Format(format, args);

            if (arg is List<object> argsList)
                return string.Format(format, argsList.ToArray());

            return string.Format(format, arg);
        }
        /// <summary>
        /// FMTs the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        /// <returns>System.String.</returns>
        public string fmt(string format, object arg0, object arg1) => string.Format(format, arg0, arg1);
        /// <summary>
        /// FMTs the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        /// <returns>System.String.</returns>
        public string fmt(string format, object arg0, object arg1, object arg2) => string.Format(format, arg0, arg1, arg2);

        /// <summary>
        /// Appends the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="suffix">The suffix.</param>
        /// <returns>System.String.</returns>
        public string append(string target, string suffix) => target + suffix;
        /// <summary>
        /// Appends the line.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>System.String.</returns>
        public string appendLine(string target) => target + newLine();

        /// <summary>
        /// Appends the FMT.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="format">The format.</param>
        /// <param name="arg">The argument.</param>
        /// <returns>System.String.</returns>
        public string appendFmt(string target, string format, object arg) => target + fmt(format, arg);
        /// <summary>
        /// Appends the FMT.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        /// <returns>System.String.</returns>
        public string appendFmt(string target, string format, object arg0, object arg1) => target + fmt(format, arg0, arg1);
        /// <summary>
        /// Appends the FMT.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        /// <returns>System.String.</returns>
        public string appendFmt(string target, string format, object arg0, object arg1, object arg2) => target + fmt(format, arg0, arg1, arg2);

        /// <summary>
        /// Dates the format.
        /// </summary>
        /// <param name="dateValue">The date value.</param>
        /// <returns>System.String.</returns>
        public string dateFormat(DateTime dateValue) => dateValue.ToString((string)Context.Args[ScriptConstants.DefaultDateFormat]);
        /// <summary>
        /// Dates the format.
        /// </summary>
        /// <param name="dateValue">The date value.</param>
        /// <param name="format">The format.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.ArgumentNullException">format</exception>
        public string dateFormat(DateTime dateValue, string format) => dateValue.ToString(format ?? throw new ArgumentNullException(nameof(format)));
        /// <summary>
        /// Dates the time format.
        /// </summary>
        /// <param name="dateValue">The date value.</param>
        /// <returns>System.String.</returns>
        public string dateTimeFormat(DateTime dateValue) => dateValue.ToString((string)Context.Args[ScriptConstants.DefaultDateTimeFormat]);
        /// <summary>
        /// Times the format.
        /// </summary>
        /// <param name="timeValue">The time value.</param>
        /// <returns>System.String.</returns>
        public string timeFormat(TimeSpan timeValue) => timeValue.ToString((string)Context.Args[ScriptConstants.DefaultTimeFormat]);
        /// <summary>
        /// Times the format.
        /// </summary>
        /// <param name="timeValue">The time value.</param>
        /// <param name="format">The format.</param>
        /// <returns>System.String.</returns>
        public string timeFormat(TimeSpan timeValue, string format) => timeValue.ToString(format);

        /// <summary>
        /// Splits the case.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        public string splitCase(string text) => text.SplitCamelCase().Replace('_', ' ').Replace("  ", " ");
        /// <summary>
        /// Humanizes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        public string humanize(string text) => splitCase(text).ToTitleCase();
        /// <summary>
        /// Titles the case.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        public string titleCase(string text) => text.ToTitleCase();
        /// <summary>
        /// Pascals the case.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        public string pascalCase(string text) => text.ToPascalCase();
        /// <summary>
        /// Camels the case.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        public string camelCase(string text) => text.ToCamelCase();
        /// <summary>
        /// Snakes the case.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        public string snakeCase(string text) => text.ToLowercaseUnderscore();
        /// <summary>
        /// Kebabs the case.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        public string kebabCase(string text) => text.ToLowercaseUnderscore().Replace("_", "-");

        /// <summary>
        /// Texts the style.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="headerStyle">The header style.</param>
        /// <returns>System.String.</returns>
        public string textStyle(string text, string headerStyle)
        {
            if (text == null) return null;
            switch (headerStyle)
            {
                case nameof(splitCase):
                    return splitCase(text);
                case nameof(humanize):
                    return humanize(text);
                case nameof(titleCase):
                    return titleCase(text);
                case nameof(pascalCase):
                    return pascalCase(text);
                case nameof(camelCase):
                    return camelCase(text);
                case nameof(snakeCase):
                    return snakeCase(text);
                case nameof(kebabCase):
                    return kebabCase(text);
            }
            return text;
        }

        /// <summary>
        /// Lowers the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        public string lower(string text) => text?.ToLower();
        /// <summary>
        /// Uppers the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        public string upper(string text) => text?.ToUpper();

        /// <summary>
        /// Substrings the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns>System.String.</returns>
        public string substring(string text, int startIndex) => text.SafeSubstring(startIndex);
        /// <summary>
        /// Substrings the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The length.</param>
        /// <returns>System.String.</returns>
        public string substring(string text, int startIndex, int length) => text.SafeSubstring(startIndex, length);

        /// <summary>
        /// Substrings the with elipsis.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="length">The length.</param>
        /// <returns>System.String.</returns>
        [Obsolete("typo")] public string substringWithElipsis(string text, int length) => text.SubstringWithEllipsis(0, length);
        /// <summary>
        /// Substrings the with elipsis.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The length.</param>
        /// <returns>System.String.</returns>
        [Obsolete("typo")] public string substringWithElipsis(string text, int startIndex, int length) => text.SubstringWithEllipsis(startIndex, length);

        /// <summary>
        /// Substrings the with ellipsis.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="length">The length.</param>
        /// <returns>System.String.</returns>
        public string substringWithEllipsis(string text, int length) => text.SubstringWithEllipsis(0, length);
        /// <summary>
        /// Substrings the with ellipsis.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The length.</param>
        /// <returns>System.String.</returns>
        public string substringWithEllipsis(string text, int startIndex, int length) => text.SubstringWithEllipsis(startIndex, length);

        /// <summary>
        /// Lefts the part.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="needle">The needle.</param>
        /// <returns>System.String.</returns>
        public string leftPart(string text, string needle) => text.LeftPart(needle);
        /// <summary>
        /// Rights the part.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="needle">The needle.</param>
        /// <returns>System.String.</returns>
        public string rightPart(string text, string needle) => text.RightPart(needle);
        /// <summary>
        /// Lasts the left part.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="needle">The needle.</param>
        /// <returns>System.String.</returns>
        public string lastLeftPart(string text, string needle) => text.LastLeftPart(needle);
        /// <summary>
        /// Lasts the right part.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="needle">The needle.</param>
        /// <returns>System.String.</returns>
        public string lastRightPart(string text, string needle) => text.LastRightPart(needle);


        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="other">The other.</param>
        /// <returns>System.Int32.</returns>
        public int compareTo(string text, string other) => string.Compare(text, other, (StringComparison)Context.Args[ScriptConstants.DefaultStringComparison]);

        /// <summary>
        /// Startses the with.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="needle">The needle.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool startsWith(string text, string needle) => text?.StartsWith(needle) == true;
        /// <summary>
        /// Endses the with.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="needle">The needle.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool endsWith(string text, string needle) => text?.EndsWith(needle) == true;

        /// <summary>
        /// Replaces the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>System.String.</returns>
        public string replace(string text, string oldValue, string newValue) => text.Replace(oldValue, newValue);

        /// <summary>
        /// Trims the start.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        public string trimStart(string text) => text?.TrimStart();
        /// <summary>
        /// Trims the start.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="c">The c.</param>
        /// <returns>System.String.</returns>
        public string trimStart(string text, char c) => text?.TrimStart(c);
        /// <summary>
        /// Trims the end.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        public string trimEnd(string text) => text?.TrimEnd();
        /// <summary>
        /// Trims the end.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="c">The c.</param>
        /// <returns>System.String.</returns>
        public string trimEnd(string text, char c) => text?.TrimEnd(c);
        /// <summary>
        /// Trims the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        public string trim(string text) => text?.Trim();
        /// <summary>
        /// Trims the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="c">The c.</param>
        /// <returns>System.String.</returns>
        public string trim(string text, char c) => text?.Trim(c);

        /// <summary>
        /// Pads the left.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="totalWidth">The total width.</param>
        /// <returns>System.String.</returns>
        public string padLeft(string text, int totalWidth) => text?.PadLeft(AssertWithinMaxQuota(totalWidth));
        /// <summary>
        /// Pads the left.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="totalWidth">The total width.</param>
        /// <param name="padChar">The pad character.</param>
        /// <returns>System.String.</returns>
        public string padLeft(string text, int totalWidth, char padChar) => text?.PadLeft(AssertWithinMaxQuota(totalWidth), padChar);
        /// <summary>
        /// Pads the right.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="totalWidth">The total width.</param>
        /// <returns>System.String.</returns>
        public string padRight(string text, int totalWidth) => text?.PadRight(AssertWithinMaxQuota(totalWidth));
        /// <summary>
        /// Pads the right.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="totalWidth">The total width.</param>
        /// <param name="padChar">The pad character.</param>
        /// <returns>System.String.</returns>
        public string padRight(string text, int totalWidth, char padChar) => text?.PadRight(AssertWithinMaxQuota(totalWidth), padChar);

        /// <summary>
        /// Splits the on first.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="needle">The needle.</param>
        /// <returns>System.String[].</returns>
        public string[] splitOnFirst(string text, string needle) => text.SplitOnFirst(needle);
        /// <summary>
        /// Splits the on last.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="needle">The needle.</param>
        /// <returns>System.String[].</returns>
        public string[] splitOnLast(string text, string needle) => text.SplitOnLast(needle);
        /// <summary>
        /// Splits the specified string list.
        /// </summary>
        /// <param name="stringList">The string list.</param>
        /// <returns>System.String[].</returns>
        public string[] split(string stringList) => stringList.Split(',');
        /// <summary>
        /// Splits the specified string list.
        /// </summary>
        /// <param name="stringList">The string list.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns>System.String[].</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public string[] split(string stringList, object delimiter)
        {
            if (delimiter is IEnumerable<object> objDelims)
                delimiter = objDelims.Select(x => x.ToString());

            if (delimiter is char c)
                return stringList.Split(c);
            if (delimiter is string s)
                return s.Length == 1
                    ? stringList.Split(s[0])
                    : stringList.Split(new[] { s }, StringSplitOptions.RemoveEmptyEntries);
            if (delimiter is IEnumerable<string> strDelims)
                return strDelims.All(x => x.Length == 1)
                    ? stringList.Split(strDelims.Select(x => x[0]).ToArray(), StringSplitOptions.RemoveEmptyEntries)
                    : stringList.Split(strDelims.ToArray(), StringSplitOptions.RemoveEmptyEntries);

            throw new NotSupportedException($"{delimiter} is not a valid delimiter");
        }

        /// <summary>
        /// Globs the specified strings.
        /// </summary>
        /// <param name="strings">The strings.</param>
        /// <param name="pattern">The pattern.</param>
        /// <returns>System.String[].</returns>
        public string[] glob(IEnumerable<string> strings, string pattern)
        {
            var list = strings?.ToList() ?? new List<string>();
            return list.Where(x => x.Glob(pattern)).ToArray();
        }

        /// <summary>
        /// Globlns the specified strings.
        /// </summary>
        /// <param name="strings">The strings.</param>
        /// <param name="pattern">The pattern.</param>
        /// <returns>System.String.</returns>
        public string globln(IEnumerable<string> strings, string pattern) => joinln(glob(strings, pattern));

        /// <summary>
        /// Parses the key value text.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        public Dictionary<string, string> parseKeyValueText(string target) => target?.ParseKeyValueText();
        /// <summary>
        /// Parses the key value text.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        public Dictionary<string, string> parseKeyValueText(string target, string delimiter) => target?.ParseKeyValueText(delimiter);

        /// <summary>
        /// Parses as key values.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>IEnumerable&lt;KeyValuePair&lt;System.String, System.String&gt;&gt;.</returns>
        public IEnumerable<KeyValuePair<string, string>> parseAsKeyValues(string target) => target?.ParseAsKeyValues();
        /// <summary>
        /// Parses as key values.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns>IEnumerable&lt;KeyValuePair&lt;System.String, System.String&gt;&gt;.</returns>
        public IEnumerable<KeyValuePair<string, string>> parseAsKeyValues(string target, string delimiter) => target?.ParseAsKeyValues(delimiter);

        /// <summary>
        /// Keyses the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>ICollection.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public ICollection keys(object target) =>
            target is IDictionary d
                ? d.Keys
                : target is IList l
                    ? times(l.Count)
                    : throw new NotSupportedException($"{target.GetType().Name} is not supported");
        /// <summary>
        /// Valueses the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>ICollection.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public ICollection values(object target) =>
            target is IDictionary d
                ? d.Values
                : target is IList l
                    ? l
                    : throw new NotSupportedException($"{target.GetType().Name} is not supported");

        /// <summary>
        /// Adds the path.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="pathToAppend">The path to append.</param>
        /// <returns>System.String.</returns>
        public string addPath(string target, string pathToAppend) => target.AppendPath(pathToAppend);
        /// <summary>
        /// Adds the paths.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="pathsToAppend">The paths to append.</param>
        /// <returns>System.String.</returns>
        public string addPaths(string target, IEnumerable pathsToAppend) =>
            target.AppendPath(pathsToAppend.Map(x => x.ToString()).ToArray());

        /// <summary>
        /// Adds the query string.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="urlParams">The URL parameters.</param>
        /// <returns>System.String.</returns>
        public string addQueryString(string url, object urlParams) =>
            urlParams.AssertOptions(nameof(addQueryString)).Aggregate(url, (current, entry) => current.AddQueryParam(entry.Key, entry.Value));

        /// <summary>
        /// Adds the hash parameters.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="urlParams">The URL parameters.</param>
        /// <returns>System.String.</returns>
        public string addHashParams(string url, object urlParams) =>
            urlParams.AssertOptions(nameof(addHashParams)).Aggregate(url, (current, entry) => current.AddHashParam(entry.Key, entry.Value));

        /// <summary>
        /// Sets the query string.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="urlParams">The URL parameters.</param>
        /// <returns>System.String.</returns>
        public string setQueryString(string url, object urlParams) =>
            urlParams.AssertOptions(nameof(setQueryString)).Aggregate(url, (current, entry) => current.SetQueryParam(entry.Key, entry.Value?.ToString()));

        /// <summary>
        /// Sets the hash parameters.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="urlParams">The URL parameters.</param>
        /// <returns>System.String.</returns>
        public string setHashParams(string url, object urlParams) =>
            urlParams.AssertOptions(nameof(setHashParams)).Aggregate(url, (current, entry) => current.SetHashParam(entry.Key, entry.Value?.ToString()));

        /// <summary>
        /// Repeatings the specified times.
        /// </summary>
        /// <param name="times">The times.</param>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        public string repeating(int times, string text) => repeat(text, AssertWithinMaxQuota(times));
        /// <summary>
        /// Repeats the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="times">The times.</param>
        /// <returns>System.String.</returns>
        public string repeat(string text, int times)
        {
            AssertWithinMaxQuota(times);
            var sb = StringBuilderCache.Allocate();
            for (var i = 0; i < times; i++)
            {
                sb.Append(text);
            }
            return StringBuilderCache.ReturnAndFree(sb);
        }

        /// <summary>
        /// Escapes the single quotes.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        public string escapeSingleQuotes(string text) => text?.Replace("'", "\\'");
        /// <summary>
        /// Escapes the double quotes.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        public string escapeDoubleQuotes(string text) => text?.Replace("\"", "\\\"");
        /// <summary>
        /// Escapes the backticks.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        public string escapeBackticks(string text) => text?.Replace("`", "\\`");
        /// <summary>
        /// Escapes the prime quotes.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        public string escapePrimeQuotes(string text) => text?.Replace("′", "\\′");
        /// <summary>
        /// Escapes the new lines.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        public string escapeNewLines(string text) => text?.Replace("\r", "\\r").Replace("\n", "\\n");

        /// <summary>
        /// Jses the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>IRawString.</returns>
        public IRawString jsString(string text) => string.IsNullOrEmpty(text)
            ? RawString.Empty
            : escapeNewLines(escapeSingleQuotes(text)).ToRawString();
        /// <summary>
        /// Jses the quoted string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>IRawString.</returns>
        public IRawString jsQuotedString(string text) =>
            ("'" + escapeNewLines(escapeSingleQuotes(text ?? "")) + "'").ToRawString();

        /// <summary>
        /// Serializes the specified scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="items">The items.</param>
        /// <param name="jsconfig">The jsconfig.</param>
        /// <param name="fn">The function.</param>
        private async Task serialize(ScriptScopeContext scope, object items, string jsconfig, Func<object, string> fn)
        {
            var defaultJsConfig = Context.Args[ScriptConstants.DefaultJsConfig] as string;
            jsconfig = jsconfig != null && !string.IsNullOrEmpty(defaultJsConfig)
                ? defaultJsConfig + "," + jsconfig
                : defaultJsConfig;

            if (jsconfig != null)
            {
                using (JsConfig.CreateScope(jsconfig))
                {
                    await scope.OutputStream.WriteAsync(fn(items)).ConfigAwait();
                    return;
                }
            }
            await scope.OutputStream.WriteAsync(items.ToJson()).ConfigAwait();
        }

        /// <summary>
        /// Serializes the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="jsconfig">The jsconfig.</param>
        /// <param name="fn">The function.</param>
        /// <returns>IRawString.</returns>
        private IRawString serialize(object target, string jsconfig, Func<object, string> fn)
        {
            var defaultJsConfig = Context.Args[ScriptConstants.DefaultJsConfig] as string;
            jsconfig = jsconfig != null && !string.IsNullOrEmpty(defaultJsConfig)
                ? defaultJsConfig + "," + jsconfig
                : defaultJsConfig;

            if (jsconfig == null)
                return fn(target.AssertNoCircularDeps()).ToRawString();

            using (JsConfig.CreateScope(jsconfig))
            {
                return fn(target).ToRawString();
            }
        }

        /// <summary>
        /// Jsons the or null.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns>System.String.</returns>
        static string jsonOrNull(object x)
        {
            if (x != null)
            {
                var json = x.ToJson();
                if (!string.IsNullOrEmpty(json))
                    return json;
            }
            return "null";
        }

        //Filters
        /// <summary>
        /// Jsons the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>IRawString.</returns>
        public IRawString json(object value) => serialize(value, null, jsonOrNull);
        /// <summary>
        /// Jsons the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="jsconfig">The jsconfig.</param>
        /// <returns>IRawString.</returns>
        public IRawString json(object value, string jsconfig) => serialize(value, jsconfig, jsonOrNull);
        /// <summary>
        /// JSVs the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>IRawString.</returns>
        public IRawString jsv(object value) => serialize(value, null, x => x.ToJsv() ?? "");
        /// <summary>
        /// JSVs the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="jsconfig">The jsconfig.</param>
        /// <returns>IRawString.</returns>
        public IRawString jsv(object value, string jsconfig) => serialize(value, jsconfig, x => x.ToJsv() ?? "");
        /// <summary>
        /// CSVs the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>IRawString.</returns>
        public IRawString csv(object value) => (value.AssertNoCircularDeps().ToCsv() ?? "").ToRawString();
        /// <summary>
        /// Dumps the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>IRawString.</returns>
        public IRawString dump(object value) => serialize(value, null, x => x.Dump() ?? "");
        /// <summary>
        /// Indents the json.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>IRawString.</returns>
        public IRawString indentJson(object value) => indentJson(value, null);
        /// <summary>
        /// Indents the json.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="jsconfig">The jsconfig.</param>
        /// <returns>IRawString.</returns>
        public IRawString indentJson(object value, string jsconfig) =>
            (value is string js ? js : json(value).ToRawString()).IndentJson().ToRawString();

        //Blocks
        /// <summary>
        /// Jsons the specified scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="items">The items.</param>
        /// <returns>Task.</returns>
        public Task json(ScriptScopeContext scope, object items) => json(scope, items, null);
        /// <summary>
        /// Jsons the specified scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="items">The items.</param>
        /// <param name="jsConfig">The js configuration.</param>
        /// <returns>Task.</returns>
        public Task json(ScriptScopeContext scope, object items, string jsConfig) => serialize(scope, items, jsConfig, x => x.ToJson());

        /// <summary>
        /// JSVs the specified scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="items">The items.</param>
        /// <returns>Task.</returns>
        public Task jsv(ScriptScopeContext scope, object items) => jsv(scope, items, null);
        /// <summary>
        /// JSVs the specified scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="items">The items.</param>
        /// <param name="jsConfig">The js configuration.</param>
        /// <returns>Task.</returns>
        public Task jsv(ScriptScopeContext scope, object items, string jsConfig) => serialize(scope, items, jsConfig, x => x.ToJsv());

        /// <summary>
        /// Dumps the specified scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="items">The items.</param>
        /// <returns>Task.</returns>
        public Task dump(ScriptScopeContext scope, object items) => jsv(scope, items, null);
        /// <summary>
        /// Dumps the specified scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="items">The items.</param>
        /// <param name="jsConfig">The js configuration.</param>
        /// <returns>Task.</returns>
        public Task dump(ScriptScopeContext scope, object items, string jsConfig) => serialize(scope, items, jsConfig, x => x.Dump());

        /// <summary>
        /// CSVs the specified scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="items">The items.</param>
        /// <returns>Task.</returns>
        public Task csv(ScriptScopeContext scope, object items) => scope.OutputStream.WriteAsync(items.ToCsv());
        /// <summary>
        /// XMLs the specified scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="items">The items.</param>
        /// <returns>Task.</returns>
        public Task xml(ScriptScopeContext scope, object items) => scope.OutputStream.WriteAsync(items.ToXml());

        /// <summary>
        /// Jsons to object.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>JsonObject.</returns>
        public JsonObject jsonToObject(string json) => JsonObject.Parse(json);
        /// <summary>
        /// Jsons to array objects.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>JsonArrayObjects.</returns>
        public JsonArrayObjects jsonToArrayObjects(string json) => JsonArrayObjects.Parse(json);
        /// <summary>
        /// Jsons to object dictionary.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
        public Dictionary<string, object> jsonToObjectDictionary(string json) => json.FromJson<Dictionary<string, object>>();
        /// <summary>
        /// Jsons to string dictionary.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        public Dictionary<string, string> jsonToStringDictionary(string json) => json.FromJson<Dictionary<string, string>>();

        /// <summary>
        /// JSVs to object dictionary.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
        public Dictionary<string, object> jsvToObjectDictionary(string json) => json.FromJsv<Dictionary<string, object>>();
        /// <summary>
        /// JSVs to string dictionary.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        public Dictionary<string, string> jsvToStringDictionary(string json) => json.FromJsv<Dictionary<string, string>>();

        /// <summary>
        /// Evals the specified scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="js">The js.</param>
        /// <returns>System.Object.</returns>
        public object eval(ScriptScopeContext scope, string js) => JS.eval(js, scope);
        /// <summary>
        /// Parses the json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>System.Object.</returns>
        public object parseJson(string json) => JSON.parse(json);

        /// <summary>
        /// Parses the CSV.
        /// </summary>
        /// <param name="csv">The CSV.</param>
        /// <returns>List&lt;List&lt;System.String&gt;&gt;.</returns>
        public List<List<string>> parseCsv(string csv)
        {
            var trimmedBody = StringBuilderCache.Allocate();
            foreach (var line in csv.ReadLines())
            {
                trimmedBody.AppendLine(line.Trim());
            }
            var strList = trimmedBody.ToString().FromCsv<List<List<string>>>();
            return strList;
        }

        /// <summary>
        /// Parses the key values.
        /// </summary>
        /// <param name="keyValuesText">The key values text.</param>
        /// <returns>List&lt;KeyValuePair&lt;System.String, System.String&gt;&gt;.</returns>
        public List<KeyValuePair<string, string>> parseKeyValues(string keyValuesText) =>
            parseKeyValues(keyValuesText, " ");
        /// <summary>
        /// Parses the key values.
        /// </summary>
        /// <param name="keyValuesText">The key values text.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns>List&lt;KeyValuePair&lt;System.String, System.String&gt;&gt;.</returns>
        public List<KeyValuePair<string, string>> parseKeyValues(string keyValuesText, string delimiter) =>
            keyValuesText.Trim().ParseAsKeyValues(delimiter);

        /// <summary>
        /// The invalid chars regex
        /// </summary>
        private static readonly Regex InvalidCharsRegex = new Regex(@"[^a-z0-9\s-]", RegexOptions.Compiled);
        /// <summary>
        /// The spaces regex
        /// </summary>
        private static readonly Regex SpacesRegex = new Regex(@"\s", RegexOptions.Compiled);
        /// <summary>
        /// The collapse hyphens regex
        /// </summary>
        private static readonly Regex CollapseHyphensRegex = new Regex("-+", RegexOptions.Compiled);

        /// <summary>
        /// Generates the slug.
        /// </summary>
        /// <param name="phrase">The phrase.</param>
        /// <returns>System.String.</returns>
        public string generateSlug(string phrase)
        {
            var str = phrase.ToLower()
                .Replace("#", "sharp")  // c#, f# => csharp, fsharp
                .Replace("++", "pp");   // c++ => cpp

            str = InvalidCharsRegex.Replace(str, "-");
            //// convert multiple spaces into one space   
            //str = CollapseSpacesRegex.Replace(str, " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= 100 ? str.Length : 100).Trim();
            str = SpacesRegex.Replace(str, "-");
            str = CollapseHyphensRegex.Replace(str, "-");

            if (string.IsNullOrEmpty(str))
                return null;

            if (str[0] == '-')
                str = str.Substring(1);
            if (str[str.Length - 1] == '-')
                str = str.Substring(0, str.Length - 1);

            return str;
        }

        /// <summary>
        /// Determines whether the specified file or ext is binary.
        /// </summary>
        /// <param name="fileOrExt">The file or ext.</param>
        /// <returns><c>true</c> if the specified file or ext is binary; otherwise, <c>false</c>.</returns>
        public bool isBinary(string fileOrExt) => MimeTypes.IsBinary(MimeTypes.GetMimeType(fileOrExt));
        /// <summary>
        /// Contents the type.
        /// </summary>
        /// <param name="fileOrExt">The file or ext.</param>
        /// <returns>System.String.</returns>
        public string contentType(string fileOrExt) => MimeTypes.GetMimeType(fileOrExt);

        /// <summary>
        /// Splits the lines.
        /// </summary>
        /// <param name="contents">The contents.</param>
        /// <returns>System.String[].</returns>
        public static string[] splitLines(string contents) => contents.Replace("\r", "").Split('\n');
        /// <summary>
        /// Reads the lines.
        /// </summary>
        /// <param name="contents">The contents.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public IEnumerable<string> readLines(string contents) => contents.ReadLines();
    }
}