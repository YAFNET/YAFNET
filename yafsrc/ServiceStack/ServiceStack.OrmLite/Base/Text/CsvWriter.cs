// ***********************************************************************
// <copyright file="CsvWriter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

using ServiceStack.OrmLite.Base.Common;
using ServiceStack.OrmLite.Base.Text.Common;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Class CsvDictionaryWriter.
/// </summary>
internal class CsvDictionaryWriter
{
    /// <summary>
    /// Writes the row.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="row">The row.</param>
    public static void WriteRow(TextWriter writer, IEnumerable<string> row)
    {
        if (writer == null)
        {
            return; //AOT
        }

        var ranOnce = false;
        var itemSep = CsvConfig.ItemSeperatorString;
        foreach (var field in row)
        {
            if (ranOnce)
            {
                writer.Write(itemSep);
            }
            else
            {
                ranOnce = true;
            }

            writer.Write(field.ToCsvField());
        }

        writer.Write(CsvConfig.RowSeparatorString);
    }

    /// <summary>
    /// Writes the object row.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="row">The row.</param>
    public static void WriteObjectRow(TextWriter writer, IEnumerable<object> row)
    {
        if (writer == null)
        {
            return; //AOT
        }

        var ranOnce = false;
        var itemSep = CsvConfig.ItemSeperatorString;
        foreach (var field in row)
        {
            if (ranOnce)
            {
                writer.Write(itemSep);
            }
            else
            {
                ranOnce = true;
            }

            writer.Write(field.ToCsvField());
        }

        writer.Write(CsvConfig.RowSeparatorString);
    }

    /// <summary>
    /// Writes the specified writer.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="records">The records.</param>
    public static void Write(TextWriter writer, IEnumerable<KeyValuePair<string, object>> records)
    {
        if (records == null)
        {
            return; //AOT
        }

        var requireHeaders = !CsvConfig<IEnumerable<KeyValuePair<string, object>>>.OmitHeaders;
        if (requireHeaders)
        {
            var keys = records.Select(x => x.Key);
            WriteRow(writer, keys);
        }

        var values = records.Select(x => x.Value);
        WriteObjectRow(writer, values);
    }

    /// <summary>
    /// Writes the specified writer.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="records">The records.</param>
    public static void Write(TextWriter writer, IEnumerable<KeyValuePair<string, string>> records)
    {
        if (records == null)
        {
            return; //AOT
        }

        var requireHeaders = !CsvConfig<IEnumerable<KeyValuePair<string, string>>>.OmitHeaders;
        if (requireHeaders)
        {
            var keys = records.Select(x => x.Key);
            WriteRow(writer, keys);
        }

        var values = records.Select(x => x.Value);
        WriteObjectRow(writer, values);
    }

    /// <summary>
    /// Writes the specified writer.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="records">The records.</param>
    public static void Write(TextWriter writer, IEnumerable<IDictionary<string, object>> records)
    {
        if (records == null)
        {
            return; //AOT
        }

        var requireHeaders = !CsvConfig<Dictionary<string, object>>.OmitHeaders;
        foreach (var record in records)
        {
            if (requireHeaders)
            {
                if (record != null)
                {
                    WriteRow(writer, record.Keys);
                }

                requireHeaders = false;
            }
            if (record != null)
            {
                WriteObjectRow(writer, record.Values);
            }
        }
    }

    /// <summary>
    /// Writes the specified writer.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="records">The records.</param>
    public static void Write(TextWriter writer, IEnumerable<IDictionary<string, string>> records)
    {
        if (records == null)
        {
            return; //AOT
        }

        var allKeys = new HashSet<string>();
        var cachedRecords = new List<IDictionary<string, string>>();

        foreach (var record in records)
        {
            foreach (var key in record.Keys)
            {
                allKeys.Add(key);
            }
            cachedRecords.Add(record);
        }

        var headers = allKeys.OrderBy(key => key).ToList();
        if (!CsvConfig<Dictionary<string, string>>.OmitHeaders)
        {
            WriteRow(writer, headers);
        }
        foreach (var fullRecord in cachedRecords.Select(cachedRecord => headers.ConvertAll(header =>
                     cachedRecord.TryGetValue(header, out var value) ? value : null)))
        {
            WriteRow(writer, fullRecord);
        }
    }
}

/// <summary>
/// Class CsvWriter.
/// </summary>
public static class CsvWriter
{
    /// <summary>
    /// Determines whether [has any escape chars] [the specified value].
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if [has any escape chars] [the specified value]; otherwise, <c>false</c>.</returns>
    public static bool HasAnyEscapeChars(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        if (CsvConfig.EscapeStrings.Exists(value.Contains))
        {
            return true;
        }

        return value[0] is JsWriter.ListStartChar or JsWriter.MapStartChar;
    }

    /// <summary>
    /// Writes the item seperator if ran once.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="ranOnce">if set to <c>true</c> [ran once].</param>
    static internal void WriteItemSeperatorIfRanOnce(TextWriter writer, ref bool ranOnce)
    {
        if (ranOnce)
        {
            writer.Write(CsvConfig.ItemSeperatorString);
        }
        else
        {
            ranOnce = true;
        }
    }
}

/// <summary>
/// Class CsvWriter.
/// </summary>
/// <typeparam name="T"></typeparam>
public class CsvWriter<T>
{
    /// <summary>
    /// The delimiter character
    /// </summary>
    public const char DelimiterChar = ',';

    /// <summary>
    /// Gets or sets the headers.
    /// </summary>
    /// <value>The headers.</value>
    public static List<string> Headers { get; set; }

    /// <summary>
    /// The property getters
    /// </summary>
    static internal List<GetMemberDelegate<T>> PropertyGetters;
    /// <summary>
    /// The property infos
    /// </summary>
    static internal List<PropertyInfo> PropertyInfos;

    /// <summary>
    /// The optimized writer
    /// </summary>
    private readonly static WriteObjectDelegate OptimizedWriter;

    /// <summary>
    /// Initializes static members of the <see cref="CsvWriter{T}" /> class.
    /// </summary>
    static CsvWriter()
    {
        if (typeof(T) == typeof(string))
        {
            OptimizedWriter = (w, o) => WriteRow(w, (IEnumerable<string>)o);
            return;
        }

        Reset();
    }

    /// <summary>
    /// Resets this instance.
    /// </summary>
    static internal void Reset()
    {
        Headers = [];

        PropertyGetters = [];
        PropertyInfos = [];
        foreach (var propertyInfo in TypeConfig<T>.Properties)
        {
            if (!propertyInfo.CanRead || propertyInfo.GetGetMethod(true) == null)
            {
                continue;
            }

            if (!TypeSerializer.CanCreateFromString(propertyInfo.PropertyType))
            {
                continue;
            }

            PropertyGetters.Add(propertyInfo.CreateGetter<T>());
            PropertyInfos.Add(propertyInfo);

            var propertyName = propertyInfo.Name;
            var dcsDataMemberName = propertyInfo.GetDataMemberName();
            if (dcsDataMemberName != null)
            {
                propertyName = dcsDataMemberName;
            }

            Headers.Add(propertyName);
        }
    }

    /// <summary>
    /// Gets the single row.
    /// </summary>
    /// <param name="records">The records.</param>
    /// <param name="recordType">Type of the record.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    private static List<string> GetSingleRow(IEnumerable<T> records, Type recordType)
    {
        return records.Select(value => recordType == typeof(string) ? value as string : TypeSerializer.SerializeToString(value)).ToList();
    }

    /// <summary>
    /// Gets the rows.
    /// </summary>
    /// <param name="records">The records.</param>
    /// <returns>List&lt;List&lt;System.String&gt;&gt;.</returns>
    public static List<List<string>> GetRows(IEnumerable<T> records)
    {
        var rows = new List<List<string>>();

        if (records == null)
        {
            return rows;
        }

        if (typeof(T).IsValueType || typeof(T) == typeof(string))
        {
            rows.Add(GetSingleRow(records, typeof(T)));
            return rows;
        }

        foreach (var record in records)
        {
            var row = new List<string>();
            foreach (var propertyGetter in PropertyGetters)
            {
                var value = propertyGetter(record) ?? "";

                var valueStr = value as string;
                var strValue = valueStr ?? TypeSerializer.SerializeToString(value);

                row.Add(strValue);
            }
            rows.Add(row);
        }

        return rows;
    }

    /// <summary>
    /// Writes the object.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="records">The records.</param>
    public static void WriteObject(TextWriter writer, object records)
    {
        if (writer == null)
        {
            return; //AOT
        }

        Write(writer, (IEnumerable<T>)records);
    }

    /// <summary>
    /// Writes the object row.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="record">The record.</param>
    public static void WriteObjectRow(TextWriter writer, object record)
    {
        if (writer == null)
        {
            return; //AOT
        }

        WriteRow(writer, (T)record);
    }

    /// <summary>
    /// Writes the specified writer.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="records">The records.</param>
    public static void Write(TextWriter writer, IEnumerable<T> records)
    {
        if (writer == null)
        {
            return; //AOT
        }

        if (records == null)
        {
            return;
        }

        if (typeof(T) == typeof(Dictionary<string, string>) || typeof(T) == typeof(IDictionary<string, string>))
        {
            CsvDictionaryWriter.Write(writer, (IEnumerable<IDictionary<string, string>>)records);
            return;
        }

        if (typeof(T).IsAssignableFrom(typeof(Dictionary<string, object>))) //also does `object`
        {
            var dynamicList = records.Select(x => x.ToObjectDictionary()).ToList();
            CsvDictionaryWriter.Write(writer, dynamicList);
            return;
        }

        if (OptimizedWriter != null)
        {
            OptimizedWriter(writer, records);
            return;
        }

        var recordsList = records.ToList();

        var headers = Headers;
        var propGetters = PropertyGetters;
        var treatAsSingleRow = typeof(T).IsValueType || typeof(T) == typeof(string);

        if (!treatAsSingleRow && JsConfig.ExcludeDefaultValues)
        {
            var hasValues = new bool[headers.Count];
            var defaultValues = new object[headers.Count];
            for (var i = 0; i < PropertyInfos.Count; i++)
            {
                defaultValues[i] = PropertyInfos[i].PropertyType.GetDefaultValue();
            }

            foreach (var record in recordsList)
            {
                for (var i = 0; i < propGetters.Count; i++)
                {
                    var propGetter = propGetters[i];
                    var value = propGetter(record);

                    if (value != null && !value.Equals(defaultValues[i]))
                    {
                        hasValues[i] = true;
                    }
                }
            }

            if (hasValues.Exists(x => x == false))
            {
                var newHeaders = new List<string>();
                var newGetters = new List<GetMemberDelegate<T>>();

                for (var i = 0; i < hasValues.Length; i++)
                {
                    if (hasValues[i])
                    {
                        newHeaders.Add(headers[i]);
                        newGetters.Add(propGetters[i]);
                    }
                }

                headers = newHeaders;
                propGetters = newGetters;
            }
        }

        var itemSep = CsvConfig.ItemSeperatorString;
        var rowSep = CsvConfig.RowSeparatorString;
        var safeCulture = CultureInfo.InvariantCulture.Equals(CultureInfo.CurrentCulture);

        if(!CsvConfig<T>.OmitHeaders && headers.Count > 0)
        {
            var ranOnce = false;
            foreach (var header in headers)
            {
                if (ranOnce)
                {
                    writer.Write(itemSep);
                }
                else
                {
                    ranOnce = true;
                }

                writer.Write(header);
            }
            writer.Write(CsvConfig.RowSeparatorString);
        }

        if (treatAsSingleRow)
        {
            var singleRow = GetSingleRow(recordsList, typeof(T));
            WriteRow(writer, singleRow);
            return;
        }

        foreach (var record in recordsList)
        {
            var ranOnce = false;
            foreach (var propGetter in propGetters)
            {
                if (ranOnce)
                {
                    writer.Write(itemSep);
                }
                else
                {
                    ranOnce = true;
                }

                var value = propGetter(record);
                if (value != null)
                {
                    var valueType = value.GetType();
                    switch (valueType.GetTypeCode())
                    {
                        case TypeCode.Boolean:
                        case TypeCode.Byte:
                        case TypeCode.SByte:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.UInt16:
                        case TypeCode.UInt32:
                        case TypeCode.UInt64:
                            writer.Write(valueType.IsEnum
                                             ? TypeSerializer.SerializeToString(value)
                                             : value.ToString());
                            break;
                        default:
                            if (safeCulture && valueType.GetTypeCode() is TypeCode.Single or TypeCode.Double or TypeCode.Decimal)
                            {
                                writer.Write(value.ToString());
                            }
                            else
                            {
                                var strValue = value as string
                                               ?? TypeSerializer.SerializeToString(value).StripQuotes();
                                writer.Write(strValue.ToCsvField());
                            }
                            break;
                    }
                }
            }
            writer.Write(rowSep);
        }
    }

    /// <summary>
    /// Writes the row.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="row">The row.</param>
    public static void WriteRow(TextWriter writer, T row)
    {
        if (writer == null)
        {
            return; //AOT
        }

        switch (row)
        {
            case IEnumerable<KeyValuePair<string, object>> kvps:
                CsvDictionaryWriter.Write(writer, kvps);
                break;
            case IEnumerable<KeyValuePair<string, string>> kvpStrings:
                CsvDictionaryWriter.Write(writer, kvpStrings);
                break;
            default:
                Write(writer, [row]);
                break;
        }
    }

    /// <summary>
    /// Writes the row.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="row">The row.</param>
    public static void WriteRow(TextWriter writer, IEnumerable<string> row)
    {
        if (writer == null)
        {
            return; //AOT
        }

        var ranOnce = false;
        foreach (var field in row)
        {
            CsvWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);

            writer.Write(field.ToCsvField());
        }
        writer.Write(CsvConfig.RowSeparatorString);
    }

    /// <summary>
    /// Writes the specified writer.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="rows">The rows.</param>
    public static void Write(TextWriter writer, IEnumerable<List<string>> rows)
    {
        if (writer == null)
        {
            return; //AOT
        }

        if (Headers.Count > 0)
        {
            var ranOnce = false;
            foreach (var header in Headers)
            {
                CsvWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);

                writer.Write(header);
            }
            writer.Write(CsvConfig.RowSeparatorString);
        }

        foreach (var row in rows)
        {
            WriteRow(writer, row);
        }
    }
}