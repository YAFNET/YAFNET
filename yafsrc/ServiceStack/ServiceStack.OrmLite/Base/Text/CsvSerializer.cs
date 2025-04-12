// ***********************************************************************
// <copyright file="CsvSerializer.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

using ServiceStack.OrmLite.Base.Text.Common;
using ServiceStack.OrmLite.Base.Text.Jsv;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Class CsvSerializer.
/// </summary>
public class CsvSerializer
{
    //Don't emit UTF8 BOM by default
    /// <summary>
    /// Gets or sets the use encoding.
    /// </summary>
    /// <value>The use encoding.</value>
    public static Encoding UseEncoding { get; set; } = PclExport.Instance.GetUTF8Encoding();

    /// <summary>
    /// Gets or sets the on serialize.
    /// </summary>
    /// <value>The on serialize.</value>
    public static Action<object> OnSerialize { get; set; }

    /// <summary>
    /// The write function cache
    /// </summary>
    private static Dictionary<Type, WriteObjectDelegate> WriteFnCache = [];

    /// <summary>
    /// Gets the write function.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>WriteObjectDelegate.</returns>
    static internal WriteObjectDelegate GetWriteFn(Type type)
    {
        try
        {
            if (WriteFnCache.TryGetValue(type, out var writeFn))
            {
                return writeFn;
            }

            var genericType = typeof(CsvSerializer<>).MakeGenericType(type);
            var mi = genericType.GetStaticMethod("WriteFn");
            var writeFactoryFn = (Func<WriteObjectDelegate>)mi.MakeDelegate(
                typeof(Func<WriteObjectDelegate>));

            writeFn = writeFactoryFn();

            Dictionary<Type, WriteObjectDelegate> snapshot, newCache;
            do
            {
                snapshot = WriteFnCache;
                newCache = new Dictionary<Type, WriteObjectDelegate>(WriteFnCache) { [type] = writeFn };
            } while (!ReferenceEquals(
                         Interlocked.CompareExchange(ref WriteFnCache, newCache, snapshot), snapshot));

            return writeFn;
        }
        catch (Exception ex)
        {
            Tracer.Instance.WriteError(ex);
            throw;
        }
    }

    /// <summary>
    /// The read function cache
    /// </summary>
    private static Dictionary<Type, ParseStringDelegate> ReadFnCache = [];

    /// <summary>
    /// Gets the read function.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>ParseStringDelegate.</returns>
    static internal ParseStringDelegate GetReadFn(Type type)
    {
        try
        {
            if (ReadFnCache.TryGetValue(type, out var writeFn))
            {
                return writeFn;
            }

            var genericType = typeof(CsvSerializer<>).MakeGenericType(type);
            var mi = genericType.GetStaticMethod("ReadFn");
            var writeFactoryFn = (Func<ParseStringDelegate>)mi.MakeDelegate(
                typeof(Func<ParseStringDelegate>));

            writeFn = writeFactoryFn();

            Dictionary<Type, ParseStringDelegate> snapshot, newCache;
            do
            {
                snapshot = ReadFnCache;
                newCache = new Dictionary<Type, ParseStringDelegate>(ReadFnCache) { [type] = writeFn };
            } while (!ReferenceEquals(
                         Interlocked.CompareExchange(ref ReadFnCache, newCache, snapshot), snapshot));

            return writeFn;
        }
        catch (Exception ex)
        {
            Tracer.Instance.WriteError(ex);
            throw;
        }
    }

    /// <summary>
    /// Serializes to CSV.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="records">The records.</param>
    /// <returns>System.String.</returns>
    public static string SerializeToCsv<T>(IEnumerable<T> records)
    {
        var writer = StringWriterThreadStatic.Allocate();
        writer.WriteCsv(records);
        return StringWriterThreadStatic.ReturnAndFree(writer);
    }

    /// <summary>
    /// Serializes to string.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public static string SerializeToString<T>(T value)
    {
        if (value == null)
        {
            return null;
        }

        if (typeof(T) == typeof(string))
        {
            return value as string;
        }

        var writer = StringWriterThreadStatic.Allocate();
        CsvSerializer<T>.WriteObject(writer, value);
        return StringWriterThreadStatic.ReturnAndFree(writer);
    }

    /// <summary>
    /// Serializes to stream.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <param name="stream">The stream.</param>
    public static void SerializeToStream<T>(T value, Stream stream)
    {
        if (value == null)
        {
            return;
        }

        var writer = new StreamWriter(stream, UseEncoding);
        CsvSerializer<T>.WriteObject(writer, value);
        writer.Flush();
    }

    /// <summary>
    /// Serializes to stream.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="stream">The stream.</param>
    public static void SerializeToStream(object obj, Stream stream)
    {
        if (obj == null)
        {
            return;
        }

        var writer = new StreamWriter(stream, UseEncoding);
        var writeFn = GetWriteFn(obj.GetType());
        writeFn(writer, obj);
        writer.Flush();
    }

    /// <summary>
    /// Deserializes from string.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="text">The text.</param>
    /// <returns>T.</returns>
    public static T DeserializeFromString<T>(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return default;
        }

        var results = CsvSerializer<T>.ReadObject(text);
        return ConvertFrom<T>(results);
    }

    /// <summary>
    /// Deserializes from string.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="text">The text.</param>
    /// <returns>System.Object.</returns>
    public static object DeserializeFromString(Type type, string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        var hold = JsState.IsCsv;
        JsState.IsCsv = true;
        try
        {
            var fn = GetReadFn(type);
            var result = fn(text);
            var converted = ConvertFrom(type, result);
            return converted;
        }
        finally
        {
            JsState.IsCsv = hold;
        }
    }

    /// <summary>
    /// Writes the late bound object.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    public static void WriteLateBoundObject(TextWriter writer, object value)
    {
        if (value == null)
        {
            return;
        }

        var writeFn = GetWriteFn(value.GetType());
        writeFn(writer, value);
    }

    /// <summary>
    /// Reads the late bound object.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public static object ReadLateBoundObject(Type type, string value)
    {
        if (value == null)
        {
            return null;
        }

        var readFn = GetReadFn(type);
        return readFn(value);
    }

    /// <summary>
    /// Converts from.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="results">The results.</param>
    /// <returns>T.</returns>
    static internal T ConvertFrom<T>(object results)
    {
        if (results is T variable)
        {
            return variable;
        }

        foreach (var ci in typeof(T).GetConstructors())
        {
            var ciParams = ci.GetParameters();
            if (ciParams.Length == 1)
            {
                var pi = ciParams.First();
                if (pi.ParameterType.IsAssignableFrom(typeof(T)))
                {
                    var to = ci.Invoke([results]);
                    return (T)to;
                }
            }
        }

        return results.ConvertTo<T>();
    }

    /// <summary>
    /// Converts from.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="results">The results.</param>
    /// <returns>System.Object.</returns>
    static internal object ConvertFrom(Type type, object results)
    {
        if (type.IsInstanceOfType(results))
        {
            return results;
        }

        foreach (var ci in type.GetConstructors())
        {
            var ciParams = ci.GetParameters();
            if (ciParams.Length == 1)
            {
                var pi = ciParams.First();
                if (pi.ParameterType.IsAssignableFrom(type))
                {
                    var to = ci.Invoke([results]);
                    return to;
                }
            }
        }

        return results.ConvertTo(type);
    }

    /// <summary>
    /// Initializes the aot.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void InitAot<T>()
    {
        CsvSerializer<T>.WriteFn();
        CsvSerializer<T>.WriteObject(null, null);
        CsvWriter<T>.Write(null, default(IEnumerable<T>));
        CsvWriter<T>.WriteRow(null, default(T));
        CsvWriter<T>.WriteObject(null, null);
        CsvWriter<T>.WriteObjectRow(null, null);

        CsvReader<T>.ReadRow(null);
        CsvReader<T>.ReadObject(null);
        CsvReader<T>.ReadObjectRow(null);
        CsvReader<T>.ReadStringDictionary(null);
    }

    /// <summary>
    /// Properties For Type
    /// </summary>
    /// <typeparam name="T">the type</typeparam>
    /// <returns>System.ValueTuple&lt;System.String, Type&gt;[].</returns>
    public static (string PropertyName, Type PropertyType)[] PropertiesFor<T>()
    {
        return CsvSerializer<T>.Properties;
    }
}

/// <summary>
/// Class CsvSerializer.
/// </summary>
/// <typeparam name="T"></typeparam>
public static class CsvSerializer<T>
{
    /// <summary>
    /// The write cache function
    /// </summary>
    private readonly static WriteObjectDelegate WriteCacheFn;

    /// <summary>
    /// The read cache function
    /// </summary>
    private readonly static ParseStringDelegate ReadCacheFn;

    /// <summary>
    /// Gets the properties.
    /// </summary>
    /// <value>The properties.</value>
    public static (string PropertyName, Type PropertyType)[] Properties { get; } = [];

    /// <summary>
    /// Writes the function.
    /// </summary>
    /// <returns>WriteObjectDelegate.</returns>
    public static WriteObjectDelegate WriteFn()
    {
        return WriteCacheFn;
    }

    /// <summary>
    /// The ignore response status
    /// </summary>
    private const string IgnoreResponseStatus = "ResponseStatus";

    /// <summary>
    /// The value getter
    /// </summary>
    private static GetMemberDelegate valueGetter;

    /// <summary>
    /// The write element function
    /// </summary>
    private static WriteObjectDelegate writeElementFn;

    /// <summary>
    /// Gets the write function.
    /// </summary>
    /// <returns>WriteObjectDelegate.</returns>
    private static WriteObjectDelegate GetWriteFn()
    {
        PropertyInfo firstCandidate = null;
        PropertyInfo bestCandidate = null;

        if (typeof(T).IsValueType)
        {
            return JsvWriter<T>.WriteObject;
        }

        //If type is an enumerable property itself write that
        var bestCandidateEnumerableType = typeof(T).GetTypeWithGenericTypeDefinitionOf(typeof(IEnumerable<>));
        if (bestCandidateEnumerableType != null)
        {
            var dictionaryOrKvps = typeof(T).HasInterface(typeof(IEnumerable<KeyValuePair<string, object>>))
                                   || typeof(T).HasInterface(typeof(IEnumerable<KeyValuePair<string, string>>));
            if (dictionaryOrKvps)
            {
                return WriteSelf;
            }

            var elementType = bestCandidateEnumerableType.GetGenericArguments()[0];
            writeElementFn = CreateWriteFn(elementType);

            return WriteEnumerableType;
        }

        //Look for best candidate property if DTO
        if (typeof(T).IsDto() || typeof(T).HasAttribute<CsvAttribute>())
        {
            var properties = TypeConfig<T>.Properties;
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.Name == IgnoreResponseStatus)
                {
                    continue;
                }

                if (propertyInfo.PropertyType == typeof(string)
                    || propertyInfo.PropertyType.IsValueType
                    || propertyInfo.PropertyType == typeof(byte[]))
                {
                    continue;
                }

                if (firstCandidate == null)
                {
                    firstCandidate = propertyInfo;
                }

                var enumProperty = propertyInfo.PropertyType
                    .GetTypeWithGenericTypeDefinitionOf(typeof(IEnumerable<>));

                if (enumProperty != null)
                {
                    bestCandidateEnumerableType = enumProperty;
                    bestCandidate = propertyInfo;
                    break;
                }
            }
        }

        //If is not DTO or no candidates exist, write self
        var noCandidatesExist = bestCandidate == null && firstCandidate == null;
        if (noCandidatesExist)
        {
            return WriteSelf;
        }

        //If is DTO and has an enumerable property serialize that
        if (bestCandidateEnumerableType != null)
        {
            valueGetter = bestCandidate.CreateGetter();
            var elementType = bestCandidateEnumerableType.GetGenericArguments()[0];
            writeElementFn = CreateWriteFn(elementType);

            return WriteEnumerableProperty;
        }

        //If is DTO and has non-enumerable, reference type property serialize that
        valueGetter = firstCandidate.CreateGetter();
        writeElementFn = CreateWriteRowFn(firstCandidate.PropertyType);

        return WriteNonEnumerableType;
    }

    /// <summary>
    /// Creates the write function.
    /// </summary>
    /// <param name="elementType">Type of the element.</param>
    /// <returns>WriteObjectDelegate.</returns>
    private static WriteObjectDelegate CreateWriteFn(Type elementType)
    {
        return CreateCsvWriterFn(elementType, "WriteObject");
    }

    /// <summary>
    /// Creates the write row function.
    /// </summary>
    /// <param name="elementType">Type of the element.</param>
    /// <returns>WriteObjectDelegate.</returns>
    private static WriteObjectDelegate CreateWriteRowFn(Type elementType)
    {
        return CreateCsvWriterFn(elementType, "WriteObjectRow");
    }

    /// <summary>
    /// Creates the CSV writer function.
    /// </summary>
    /// <param name="elementType">Type of the element.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <returns>WriteObjectDelegate.</returns>
    private static WriteObjectDelegate CreateCsvWriterFn(Type elementType, string methodName)
    {
        var genericType = typeof(CsvWriter<>).MakeGenericType(elementType);
        var mi = genericType.GetStaticMethod(methodName);
        var writeFn = (WriteObjectDelegate)mi.MakeDelegate(typeof(WriteObjectDelegate));
        return writeFn;
    }

    /// <summary>
    /// Writes the type of the enumerable.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="obj">The object.</param>
    public static void WriteEnumerableType(TextWriter writer, object obj)
    {
        writeElementFn(writer, obj);
    }

    /// <summary>
    /// Writes the self.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="obj">The object.</param>
    public static void WriteSelf(TextWriter writer, object obj)
    {
        CsvWriter<T>.WriteRow(writer, (T)obj);
    }

    /// <summary>
    /// Writes the enumerable property.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="obj">The object.</param>
    public static void WriteEnumerableProperty(TextWriter writer, object obj)
    {
        if (obj == null)
        {
            return; //AOT
        }

        var enumerableProperty = valueGetter(obj);
        writeElementFn(writer, enumerableProperty);
    }

    /// <summary>
    /// Writes the type of the non enumerable.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="obj">The object.</param>
    public static void WriteNonEnumerableType(TextWriter writer, object obj)
    {
        var nonEnumerableType = valueGetter(obj);
        writeElementFn(writer, nonEnumerableType);
    }

    /// <summary>
    /// Writes the object.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    public static void WriteObject(TextWriter writer, object value)
    {
        var hold = JsState.IsCsv;
        JsState.IsCsv = true;
        try
        {
            CsvSerializer.OnSerialize?.Invoke(value);
            WriteCacheFn(writer, value);
        }
        finally
        {
            JsState.IsCsv = hold;
        }
    }

    /// <summary>
    /// Initializes static members of the <see cref="CsvSerializer{T}" /> class.
    /// </summary>
    static CsvSerializer()
    {
        if (typeof(T) == typeof(object))
        {
            WriteCacheFn = CsvSerializer.WriteLateBoundObject;
            ReadCacheFn = str => CsvSerializer.ReadLateBoundObject(typeof(T), str);
        }
        else
        {
            WriteCacheFn = GetWriteFn();
            ReadCacheFn = GetReadFn();
            var writers = WriteType<T, JsvTypeSerializer>.PropertyWriters;
            if (writers != null)
            {
                Properties = [.. writers.Select(x => (x.propertyName, x.PropertyType))];
            }
        }
    }

    /// <summary>
    /// Reads the function.
    /// </summary>
    /// <returns>ParseStringDelegate.</returns>
    public static ParseStringDelegate ReadFn()
    {
        return ReadCacheFn;
    }

    /// <summary>
    /// The value setter
    /// </summary>
    private static SetMemberDelegate valueSetter;

    /// <summary>
    /// The read element function
    /// </summary>
    private static ParseStringDelegate readElementFn;

    /// <summary>
    /// Gets the read function.
    /// </summary>
    /// <returns>ParseStringDelegate.</returns>
    private static ParseStringDelegate GetReadFn()
    {
        PropertyInfo firstCandidate = null;
        Type bestCandidateEnumerableType = null;
        PropertyInfo bestCandidate = null;

        if (typeof(T).IsValueType)
        {
            return JsvReader<T>.Parse;
        }

        //If type is an enumerable property itself write that
        bestCandidateEnumerableType = typeof(T).GetTypeWithGenericTypeDefinitionOf(typeof(IEnumerable<>));
        if (bestCandidateEnumerableType != null)
        {
            var elementType = bestCandidateEnumerableType.GetGenericArguments()[0];
            readElementFn = CreateReadFn(elementType);

            return ReadEnumerableType;
        }

        //Look for best candidate property if DTO
        if (typeof(T).IsDto() || typeof(T).HasAttribute<CsvAttribute>())
        {
            var properties = TypeConfig<T>.Properties;
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.Name == IgnoreResponseStatus)
                {
                    continue;
                }

                if (propertyInfo.PropertyType == typeof(string)
                    || propertyInfo.PropertyType.IsValueType
                    || propertyInfo.PropertyType == typeof(byte[]))
                {
                    continue;
                }

                if (firstCandidate == null)
                {
                    firstCandidate = propertyInfo;
                }

                var enumProperty = propertyInfo.PropertyType
                    .GetTypeWithGenericTypeDefinitionOf(typeof(IEnumerable<>));

                if (enumProperty != null)
                {
                    bestCandidateEnumerableType = enumProperty;
                    bestCandidate = propertyInfo;
                    break;
                }
            }
        }

        //If is not DTO or no candidates exist, write self
        var noCandidatesExist = bestCandidate == null && firstCandidate == null;
        if (noCandidatesExist)
        {
            return ReadSelf;
        }

        //If is DTO and has an enumerable property serialize that
        if (bestCandidateEnumerableType != null)
        {
            valueSetter = bestCandidate.CreateSetter();
            var elementType = bestCandidateEnumerableType.GetGenericArguments()[0];
            readElementFn = CreateReadFn(elementType);

            return ReadEnumerableProperty;
        }

        //If is DTO and has non-enumerable, reference type property serialize that
        valueSetter = firstCandidate.CreateSetter();
        readElementFn = CreateReadRowFn(firstCandidate.PropertyType);

        return ReadNonEnumerableType;
    }

    /// <summary>
    /// Creates the read function.
    /// </summary>
    /// <param name="elementType">Type of the element.</param>
    /// <returns>ParseStringDelegate.</returns>
    private static ParseStringDelegate CreateReadFn(Type elementType)
    {
        return CreateCsvReadFn(elementType, "ReadObject");
    }

    /// <summary>
    /// Creates the read row function.
    /// </summary>
    /// <param name="elementType">Type of the element.</param>
    /// <returns>ParseStringDelegate.</returns>
    private static ParseStringDelegate CreateReadRowFn(Type elementType)
    {
        return CreateCsvReadFn(elementType, "ReadObjectRow");
    }

    /// <summary>
    /// Creates the CSV read function.
    /// </summary>
    /// <param name="elementType">Type of the element.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <returns>ParseStringDelegate.</returns>
    private static ParseStringDelegate CreateCsvReadFn(Type elementType, string methodName)
    {
        var genericType = typeof(CsvReader<>).MakeGenericType(elementType);
        var mi = genericType.GetStaticMethod(methodName);
        var readFn = (ParseStringDelegate)mi.MakeDelegate(typeof(ParseStringDelegate));
        return readFn;
    }

    /// <summary>
    /// Reads the type of the enumerable.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public static object ReadEnumerableType(string value)
    {
        return readElementFn(value);
    }

    /// <summary>
    /// Reads the self.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public static object ReadSelf(string value)
    {
        return CsvReader<T>.ReadRow(value);
    }

    /// <summary>
    /// Reads the enumerable property.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <returns>System.Object.</returns>
    public static object ReadEnumerableProperty(string row)
    {
        if (row == null)
        {
            return null; //AOT
        }

        var value = readElementFn(row);
        var to = typeof(T).CreateInstance();
        valueSetter(to, value);
        return to;
    }

    /// <summary>
    /// Reads the type of the non enumerable.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <returns>System.Object.</returns>
    public static object ReadNonEnumerableType(string row)
    {
        if (row == null)
        {
            return null; //AOT
        }

        var value = readElementFn(row);
        var to = typeof(T).CreateInstance();
        valueSetter(to, value);
        return to;
    }

    /// <summary>
    /// Reads the object.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public static object ReadObject(string value)
    {
        if (value == null)
        {
            return null; //AOT
        }

        var hold = JsState.IsCsv;
        JsState.IsCsv = true;
        try
        {
            return ReadCacheFn(value);
        }
        finally
        {
            JsState.IsCsv = hold;
        }
    }
}