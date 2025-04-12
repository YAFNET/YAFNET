// ***********************************************************************
// <copyright file="CsvReader.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

using ServiceStack.OrmLite.Base.Text.Common;
using ServiceStack.OrmLite.Base.Text.Jsv;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Class CsvReader.
/// </summary>
public static class CsvReader
{
    /// <summary>
    /// Parses the lines.
    /// </summary>
    /// <param name="csv">The CSV.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    public static List<string> ParseLines(string csv)
    {
        var rows = new List<string>();
        if (string.IsNullOrEmpty(csv))
        {
            return rows;
        }

        var withinQuotes = false;
        var lastPos = 0;

        var i = -1;
        var len = csv.Length;
        while (++i < len)
        {
            var c = csv[i];
            if (c == JsWriter.QuoteChar)
            {
                var isLiteralQuote = i + 1 < len && csv[i + 1] == JsWriter.QuoteChar;
                if (isLiteralQuote)
                {
                    i++;
                    continue;
                }

                withinQuotes = !withinQuotes;
            }

            if (withinQuotes)
            {
                continue;
            }

            if (c == JsWriter.LineFeedChar)
            {
                var str = i > 0 && csv[i - 1] == JsWriter.ReturnChar
                              ? csv.Substring(lastPos, i - lastPos - 1)
                              : csv.Substring(lastPos, i - lastPos);

                if (str.Length > 0)
                {
                    rows.Add(str);
                }

                lastPos = i + 1;
            }
        }

        if (i > lastPos)
        {
            var str = csv.Substring(lastPos, i - lastPos);
            if (str.Length > 0)
            {
                rows.Add(str);
            }
        }

        return rows;
    }

    /// <summary>
    /// Parses the fields.
    /// </summary>
    /// <param name="line">The line.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    public static List<string> ParseFields(string line)
    {
        return ParseFields(line, null);
    }

    /// <summary>
    /// Parses the fields.
    /// </summary>
    /// <param name="line">The line.</param>
    /// <param name="parseFn">The parse function.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    public static List<string> ParseFields(string line, Func<string, string> parseFn)
    {
        var to = new List<string>();
        if (string.IsNullOrEmpty(line))
        {
            return to;
        }

        var i = -1;
        var len = line.Length;
        while (++i <= len)
        {
            var value = EatValue(line, ref i);
            to.Add(parseFn != null ? parseFn(value.FromCsvField()) : value.FromCsvField());
        }

        return to;
    }

    //Originally from JsvTypeSerializer.EatValue()
    /// <summary>
    /// Eats the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="i">The i.</param>
    /// <returns>System.String.</returns>
    public static string EatValue(string value, ref int i)
    {
        var tokenStartPos = i;
        var valueLength = value.Length;
        if (i == valueLength)
        {
            return null;
        }

        var valueChar = value[i];
        var withinQuotes = false;
        var endsToEat = 1;
        var itemSeperator = CsvConfig.ItemSeperatorString.Length == 1
                                ? CsvConfig.ItemSeperatorString[0]
                                : JsWriter.ItemSeperator;

        if (valueChar == itemSeperator || valueChar == JsWriter.MapEndChar)
        {
            return null;
        }

        switch (valueChar)
        {
            //Is Within Quotes, i.e. "..."
            case JsWriter.QuoteChar:
                {
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
                    return value.Substring(tokenStartPos, i - tokenStartPos);
                }
            //Is Type/Map, i.e. {...}
            case JsWriter.MapStartChar:
                {
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

                        if (valueChar == JsWriter.MapStartChar)
                        {
                            endsToEat++;
                        }

                        if (valueChar == JsWriter.MapEndChar)
                        {
                            endsToEat--;
                        }
                    }
                    if (endsToEat > 0)
                    {
                        //Unmatched start and end char, give up
                        i = tokenStartPos;
                        valueChar = value[i];
                    }
                    else
                    {
                        return value.Substring(tokenStartPos, i - tokenStartPos);
                    }

                    break;
                }
        }

        if (valueChar == JsWriter.ListStartChar) //Is List, i.e. [...]
        {
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
            if (endsToEat > 0)
            {
                //Unmatched start and end char, give up
                i = tokenStartPos;
                valueChar = value[i];
            }
            else
            {
                return value.Substring(tokenStartPos, i - tokenStartPos);
            }
        }

        //if value starts with MapStartChar, check MapEndChar to terminate
        var specEndChar = itemSeperator;
        if (value[tokenStartPos] == JsWriter.MapStartChar)
        {
            specEndChar = JsWriter.MapEndChar;
        }

        while (++i < valueLength) //Is Value
        {
            valueChar = value[i];

            if (valueChar == itemSeperator || valueChar == specEndChar)
            {
                break;
            }
        }

        return value.Substring(tokenStartPos, i - tokenStartPos);
    }
}

/// <summary>
/// Class CsvReader.
/// </summary>
/// <typeparam name="T"></typeparam>
public static class CsvReader<T>
{
    /// <summary>
    /// Gets or sets the headers.
    /// </summary>
    /// <value>The headers.</value>
    public static List<string> Headers { get; set; }

    /// <summary>
    /// The property setters
    /// </summary>
    static internal List<SetMemberDelegate<T>> PropertySetters;
    /// <summary>
    /// The property setters map
    /// </summary>
    static internal Dictionary<string, SetMemberDelegate<T>> PropertySettersMap;

    /// <summary>
    /// The property converters
    /// </summary>
    static internal List<ParseStringDelegate> PropertyConverters;
    /// <summary>
    /// The property converters map
    /// </summary>
    static internal Dictionary<string, ParseStringDelegate> PropertyConvertersMap;

    /// <summary>
    /// Initializes static members of the <see cref="CsvReader{T}" /> class.
    /// </summary>
    static CsvReader()
    {
        Reset();
    }

    /// <summary>
    /// Resets this instance.
    /// </summary>
    static internal void Reset()
    {
        Headers = [];

        PropertySetters = [];
        PropertySettersMap = new Dictionary<string, SetMemberDelegate<T>>(PclExport.Instance.InvariantComparerIgnoreCase);

        PropertyConverters = [];
        PropertyConvertersMap = new Dictionary<string, ParseStringDelegate>(PclExport.Instance.InvariantComparerIgnoreCase);

        foreach (var propertyInfo in TypeConfig<T>.Properties)
        {
            if (!propertyInfo.CanWrite || propertyInfo.GetSetMethod(true) == null)
            {
                continue;
            }

            if (!TypeSerializer.CanCreateFromString(propertyInfo.PropertyType))
            {
                continue;
            }

            var propertyName = propertyInfo.Name;
            var setter = propertyInfo.CreateSetter<T>();
            PropertySetters.Add(setter);

            var converter = JsvReader.GetParseFn(propertyInfo.PropertyType);
            PropertyConverters.Add(converter);

            var dcsDataMemberName = propertyInfo.GetDataMemberName();
            if (dcsDataMemberName != null)
            {
                propertyName = dcsDataMemberName;
            }

            Headers.Add(propertyName);
            PropertySettersMap[propertyName] = setter;
            PropertyConvertersMap[propertyName] = converter;
        }
    }

    /// <summary>
    /// Gets the single row.
    /// </summary>
    /// <param name="rows">The rows.</param>
    /// <param name="recordType">Type of the record.</param>
    /// <returns>List&lt;T&gt;.</returns>
    private static List<T> GetSingleRow(IEnumerable<string> rows, Type recordType)
    {
        return [.. rows.Select(value => recordType == typeof(string) ? (T)(object)value : TypeSerializer.DeserializeFromString<T>(value))];
    }

    /// <summary>
    /// Gets the rows.
    /// </summary>
    /// <param name="records">The records.</param>
    /// <returns>List&lt;T&gt;.</returns>
    public static List<T> GetRows(IEnumerable<string> records)
    {
        var rows = new List<T>();

        if (records == null)
        {
            return rows;
        }

        if (typeof(T).IsValueType || typeof(T) == typeof(string))
        {
            return GetSingleRow(records, typeof(T));
        }

        foreach (var record in records)
        {
            var to = typeof(T).CreateInstance<T>();
            foreach (var propertySetter in PropertySetters)
            {
                propertySetter(to, record);
            }
            rows.Add(to);
        }

        return rows;
    }

    /// <summary>
    /// Reads the object.
    /// </summary>
    /// <param name="csv">The CSV.</param>
    /// <returns>System.Object.</returns>
    public static object ReadObject(string csv)
    {
        return csv == null ? null : Read(CsvReader.ParseLines(csv));
    }

    /// <summary>
    /// Reads the object row.
    /// </summary>
    /// <param name="csv">The CSV.</param>
    /// <returns>System.Object.</returns>
    public static object ReadObjectRow(string csv)
    {
        if (csv == null)
        {
            return null; //AOT
        }

        return ReadRow(csv);
    }

    /// <summary>
    /// Reads the string dictionary.
    /// </summary>
    /// <param name="rows">The rows.</param>
    /// <returns>List&lt;Dictionary&lt;System.String, System.String&gt;&gt;.</returns>
    public static List<Dictionary<string, string>> ReadStringDictionary(IEnumerable<string> rows)
    {
        if (rows == null)
        {
            return [];
        }

        var to = new List<Dictionary<string, string>>();

        List<string> headers = null;
        foreach (var row in rows)
        {
            if (headers == null)
            {
                headers = CsvReader.ParseFields(row);
                continue;
            }

            var values = CsvReader.ParseFields(row);
            var map = new Dictionary<string, string>();
            for (var i = 0; i < headers.Count; i++)
            {
                var header = headers[i];
                map[header] = values[i];
            }

            to.Add(map);
        }

        return to;
    }

    /// <summary>
    /// Reads the specified rows.
    /// </summary>
    /// <param name="rows">The rows.</param>
    /// <returns>List&lt;T&gt;.</returns>
    public static List<T> Read(List<string> rows)
    {
        var to = new List<T>();
        if (rows == null || rows.Count == 0)
        {
            return to; //AOT
        }

        if (typeof(T).IsAssignableFrom(typeof(Dictionary<string, string>)))
        {
            return ReadStringDictionary(rows).ConvertAll(x => (T)(object)x);
        }

        if (typeof(T).IsAssignableFrom(typeof(List<string>)))
        {
            return [..rows.Select(x => (T)(object)CsvReader.ParseFields(x))];
        }

        List<string> headers = null;
        if (!CsvConfig<T>.OmitHeaders || Headers.Count == 0)
        {
            headers = CsvReader.ParseFields(rows[0], s => s.Trim());
        }

        if (typeof(T).IsValueType || typeof(T) == typeof(string))
        {
            return rows.Count == 1
                       ? GetSingleRow(CsvReader.ParseFields(rows[0]), typeof(T))
                       : GetSingleRow(rows, typeof(T));
        }

        for (var rowIndex = headers == null ? 0 : 1; rowIndex < rows.Count; rowIndex++)
        {
            var row = rows[rowIndex];
            var o = typeof(T).CreateInstance<T>();

            var fields = CsvReader.ParseFields(row);
            for (var i = 0; i < fields.Count; i++)
            {
                var setter = i < PropertySetters.Count ? PropertySetters[i] : null;
                if (headers != null)
                {
                    PropertySettersMap.TryGetValue(headers[i], out setter);
                }

                if (setter == null)
                {
                    continue;
                }

                var converter = i < PropertyConverters.Count ? PropertyConverters[i] : null;
                if (headers != null)
                {
                    PropertyConvertersMap.TryGetValue(headers[i], out converter);
                }

                if (converter == null)
                {
                    continue;
                }

                var field = fields[i];
                var convertedValue = converter(field);
                setter(o, convertedValue);
            }

            to.Add(o);
        }

        return to;
    }

    /// <summary>
    /// Reads the row.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>T.</returns>
    public static T ReadRow(string value)
    {
        return value == null ? default : Read(CsvReader.ParseLines(value)).FirstOrDefault();
    }

}