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
using System.Threading.Tasks;

using ServiceStack.Text;

namespace ServiceStack.Script;

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
    /// News the line.
    /// </summary>
    /// <returns>System.String.</returns>
    public string newLine() => Context.Args[ScriptConstants.DefaultNewLine] as string;

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
    /// Dates the format.
    /// </summary>
    /// <param name="dateValue">The date value.</param>
    /// <returns>System.String.</returns>
    public string dateFormat(DateTime dateValue) => dateValue.ToString((string)Context.Args[ScriptConstants.DefaultDateFormat]);
  
    /// <summary>
    /// Times the format.
    /// </summary>
    /// <param name="timeValue">The time value.</param>
    /// <returns>System.String.</returns>
    public string timeFormat(TimeSpan timeValue) => timeValue.ToString((string)Context.Args[ScriptConstants.DefaultTimeFormat]);

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
                       : stringList.Split([s], StringSplitOptions.RemoveEmptyEntries);
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
        var list = strings?.ToList() ?? [];
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
    /// Valueses the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>ICollection.</returns>
    /// <exception cref="System.NotSupportedException"></exception>
    public ICollection values(object target) =>
        target is IDictionary d
            ? d.Values
            : target as IList ?? throw new NotSupportedException($"{target.GetType().Name} is not supported");

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
    /// Evals the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="js">The js.</param>
    /// <returns>System.Object.</returns>
    public object eval(ScriptScopeContext scope, string js) => JS.eval(js, scope);

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