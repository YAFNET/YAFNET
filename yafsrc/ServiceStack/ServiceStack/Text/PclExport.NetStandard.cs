// ***********************************************************************
// <copyright file="PclExport.NetStandard.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ServiceStack.Text;
using ServiceStack.Text.Common;
using ServiceStack.Text.Json;

using System.Globalization;
using System.Reflection;
using System.Net;
using System.Collections.Specialized;

namespace ServiceStack;

/// <summary>
/// Class NetStandardPclExport.
/// Implements the <see cref="PclExport" />
/// </summary>
/// <seealso cref="PclExport" />
public class NetStandardPclExport : PclExport
{
    /// <summary>
    /// The provider
    /// </summary>
    public static NetStandardPclExport Provider = new();

    /// <summary>
    /// All date time formats
    /// </summary>
    static string[] allDateTimeFormats = new string[]
                                             {
                                                 "yyyy-MM-ddTHH:mm:ss.FFFFFFFzzzzzz",
                                                 "yyyy-MM-ddTHH:mm:ss.FFFFFFF",
                                                 "yyyy-MM-ddTHH:mm:ss.FFFFFFFZ",
                                                 "HH:mm:ss.FFFFFFF",
                                                 "HH:mm:ss.FFFFFFFZ",
                                                 "HH:mm:ss.FFFFFFFzzzzzz",
                                                 "yyyy-MM-dd",
                                                 "yyyy-MM-ddZ",
                                                 "yyyy-MM-ddzzzzzz",
                                                 "yyyy-MM",
                                                 "yyyy-MMZ",
                                                 "yyyy-MMzzzzzz",
                                                 "yyyy",
                                                 "yyyyZ",
                                                 "yyyyzzzzzz",
                                                 "--MM-dd",
                                                 "--MM-ddZ",
                                                 "--MM-ddzzzzzz",
                                                 "---dd",
                                                 "---ddZ",
                                                 "---ddzzzzzz",
                                                 "--MM--",
                                                 "--MM--Z",
                                                 "--MM--zzzzzz",
                                             };

    /// <summary>
    /// Initializes a new instance of the <see cref="NetStandardPclExport"/> class.
    /// </summary>
    public NetStandardPclExport()
    {
        this.PlatformName = Platforms.NetStandard;
        this.DirSep = Path.DirectorySeparatorChar;
    }

    /// <summary>
    /// Reads all text.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns>System.String.</returns>
    public override string ReadAllText(string filePath)
    {
        using (var reader = File.OpenText(filePath))
        {
            return reader.ReadToEnd();
        }
    }

    /// <summary>
    /// Files the exists.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public override bool FileExists(string filePath)
    {
        return File.Exists(filePath);
    }

    /// <summary>
    /// Directories the exists.
    /// </summary>
    /// <param name="dirPath">The dir path.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public override bool DirectoryExists(string dirPath)
    {
        return Directory.Exists(dirPath);
    }

    /// <summary>
    /// Creates the directory.
    /// </summary>
    /// <param name="dirPath">The dir path.</param>
    public override void CreateDirectory(string dirPath)
    {
        Directory.CreateDirectory(dirPath);
    }

    /// <summary>
    /// Gets the file names.
    /// </summary>
    /// <param name="dirPath">The dir path.</param>
    /// <param name="searchPattern">The search pattern.</param>
    /// <returns>System.String[].</returns>
    public override string[] GetFileNames(string dirPath, string searchPattern = null)
    {
        if (!Directory.Exists(dirPath))
            return TypeConstants.EmptyStringArray;

        return searchPattern != null
                   ? Directory.GetFiles(dirPath, searchPattern)
                   : Directory.GetFiles(dirPath);
    }

    /// <summary>
    /// Gets the directory names.
    /// </summary>
    /// <param name="dirPath">The dir path.</param>
    /// <param name="searchPattern">The search pattern.</param>
    /// <returns>System.String[].</returns>
    public override string[] GetDirectoryNames(string dirPath, string searchPattern = null)
    {
        if (!Directory.Exists(dirPath))
            return TypeConstants.EmptyStringArray;

        return searchPattern != null
                   ? Directory.GetDirectories(dirPath, searchPattern)
                   : Directory.GetDirectories(dirPath);
    }

    /// <summary>
    /// Maps the absolute path.
    /// </summary>
    /// <param name="relativePath">The relative path.</param>
    /// <param name="appendPartialPathModifier">The append partial path modifier.</param>
    /// <returns>System.String.</returns>
    public override string MapAbsolutePath(string relativePath, string appendPartialPathModifier)
    {
        if (relativePath.StartsWith("~"))
        {
            var assemblyDirectoryPath = AppContext.BaseDirectory;

            // Escape the assembly bin directory to the hostname directory
            var hostDirectoryPath = appendPartialPathModifier != null
                                        ? assemblyDirectoryPath + appendPartialPathModifier
                                        : assemblyDirectoryPath;

            return Path.GetFullPath(relativePath.Replace("~", hostDirectoryPath));
        }
        return relativePath;
    }

    /// <summary>
    /// Configures this instance.
    /// </summary>
    /// <returns>PclExport.</returns>
    public static PclExport Configure()
    {
        Configure(Provider);
        return Provider;
    }

    /// <summary>
    /// Gets the environment variable.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public override string GetEnvironmentVariable(string name) => Environment.GetEnvironmentVariable(name);

    /// <summary>
    /// Writes the line.
    /// </summary>
    /// <param name="line">The line.</param>
    public override void WriteLine(string line) => Console.WriteLine(line);

    /// <summary>
    /// Writes the line.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="args">The arguments.</param>
    public override void WriteLine(string format, params object[] args) => Console.WriteLine(format, args);

    /// <summary>
    /// Adds the compression.
    /// </summary>
    /// <param name="webReq">The web req.</param>
    public override void AddCompression(WebRequest webReq)
    {
        try
        {
            var httpReq = (HttpWebRequest)webReq;
            httpReq.Headers[HttpRequestHeader.AcceptEncoding] = "gzip,deflate";
            httpReq.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        }
        catch (Exception ex)
        {
            Tracer.Instance.WriteError(ex);
        }
    }

    /// <summary>
    /// Adds the header.
    /// </summary>
    /// <param name="webReq">The web req.</param>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    public override void AddHeader(WebRequest webReq, string name, string value)
    {
        webReq.Headers[name] = value;
    }

    /// <summary>
    /// Gets all assemblies.
    /// </summary>
    /// <returns>Assembly[].</returns>
    public override Assembly[] GetAllAssemblies()
    {
        return AppDomain.CurrentDomain.GetAssemblies();
    }

    /// <summary>
    /// Gets the assembly code base.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <returns>System.String.</returns>
    public override string GetAssemblyCodeBase(Assembly assembly)
    {
        var dll = typeof(PclExport).Assembly;
        var pi = dll.GetType().GetProperty("CodeBase");
        var codeBase = pi?.GetProperty(dll).ToString();
        return codeBase;
    }

    /// <summary>
    /// Gets the assembly path.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>System.String.</returns>
    public override string GetAssemblyPath(Type source)
    {
        var codeBase = GetAssemblyCodeBase(source.GetTypeInfo().Assembly);
        if (codeBase == null)
            return null;

        var assemblyUri = new Uri(codeBase);
        return assemblyUri.LocalPath;
    }

    /// <summary>
    /// Gets the ASCII string.
    /// </summary>
    /// <param name="bytes">The bytes.</param>
    /// <param name="index">The index.</param>
    /// <param name="count">The count.</param>
    /// <returns>System.String.</returns>
    public override string GetAsciiString(byte[] bytes, int index, int count)
    {
        return System.Text.Encoding.ASCII.GetString(bytes, index, count);
    }

    /// <summary>
    /// Gets the ASCII bytes.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <returns>System.Byte[].</returns>
    public override byte[] GetAsciiBytes(string str)
    {
        return System.Text.Encoding.ASCII.GetBytes(str);
    }

    /// <summary>
    /// Ins the same assembly.
    /// </summary>
    /// <param name="t1">The t1.</param>
    /// <param name="t2">The t2.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public override bool InSameAssembly(Type t1, Type t2)
    {
        return t1.Assembly == t2.Assembly;
    }

    /// <summary>
    /// Gets the type of the generic collection.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>Type.</returns>
    public override Type GetGenericCollectionType(Type type)
    {
        return type.GetTypeInfo().ImplementedInterfaces.FirstOrDefault(t =>
            t.IsGenericType
            && t.GetGenericTypeDefinition() == typeof(ICollection<>));
    }

    /// <summary>
    /// Parses the XSD date time as UTC.
    /// </summary>
    /// <param name="dateTimeStr">The date time string.</param>
    /// <returns>DateTime.</returns>
    public override DateTime ParseXsdDateTimeAsUtc(string dateTimeStr)
    {
        return DateTime.ParseExact(dateTimeStr, allDateTimeFormats, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowLeadingWhite|DateTimeStyles.AllowTrailingWhite|DateTimeStyles.AdjustToUniversal)
            .Prepare(parsedAsUtc: true);
    }

    //public override DateTime ToStableUniversalTime(DateTime dateTime)
    //{
    //    // .Net 2.0 - 3.5 has an issue with DateTime.ToUniversalTime, but works ok with TimeZoneInfo.ConvertTimeToUtc.
    //    // .Net 4.0+ does this under the hood anyway.
    //    return TimeZoneInfo.ConvertTimeToUtc(dateTime);
    //}

    /// <summary>
    /// Gets the specialized collection parse method.
    /// </summary>
    /// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
    /// <param name="type">The type.</param>
    /// <returns>ParseStringDelegate.</returns>
    public override ParseStringDelegate GetSpecializedCollectionParseMethod<TSerializer>(Type type)
    {
        if (type == typeof(StringCollection))
        {
            return v => ParseStringCollection<TSerializer>(v.AsSpan());
        }
        return null;
    }

    /// <summary>
    /// Gets the specialized collection parse string span method.
    /// </summary>
    /// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
    /// <param name="type">The type.</param>
    /// <returns>ParseStringSpanDelegate.</returns>
    public override ParseStringSpanDelegate GetSpecializedCollectionParseStringSpanMethod<TSerializer>(Type type)
    {
        if (type == typeof(StringCollection))
        {
            return ParseStringCollection<TSerializer>;
        }
        return null;
    }

    /// <summary>
    /// Parses the string collection.
    /// </summary>
    /// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>StringCollection.</returns>
    private static StringCollection ParseStringCollection<TSerializer>(ReadOnlySpan<char> value) where TSerializer : ITypeSerializer
    {
        if ((value = DeserializeListWithElements<TSerializer>.StripList(value)).IsNullOrEmpty()) 
            return value.IsEmpty ? null : new StringCollection();

        var result = new StringCollection();

        if (value.Length > 0)
        {
            foreach (var item in DeserializeListWithElements<TSerializer>.ParseStringList(value))
            {
                result.Add(item);
            }
        }

        return result;
    }
    /// <summary>
    /// Sets the user agent.
    /// </summary>
    /// <param name="httpReq">The HTTP req.</param>
    /// <param name="value">The value.</param>
    public override void SetUserAgent(HttpWebRequest httpReq, string value)
    {
        try
        {
            httpReq.UserAgent = value;
        }
        catch (Exception e) // API may have been removed by Xamarin's Linker
        {
            Tracer.Instance.WriteError(e);
        }
    }

    /// <summary>
    /// Sets the length of the content.
    /// </summary>
    /// <param name="httpReq">The HTTP req.</param>
    /// <param name="value">The value.</param>
    public override void SetContentLength(HttpWebRequest httpReq, long value)
    {
        try
        {
            httpReq.ContentLength = value;
        }
        catch (Exception e) // API may have been removed by Xamarin's Linker
        {
            Tracer.Instance.WriteError(e);
        }
    }

    /// <summary>
    /// Sets the allow automatic redirect.
    /// </summary>
    /// <param name="httpReq">The HTTP req.</param>
    /// <param name="value">if set to <c>true</c> [value].</param>
    public override void SetAllowAutoRedirect(HttpWebRequest httpReq, bool value)
    {
        try
        {
            httpReq.AllowAutoRedirect = value;
        }
        catch (Exception e) // API may have been removed by Xamarin's Linker
        {
            Tracer.Instance.WriteError(e);
        }
    }

    /// <summary>
    /// Initializes the HTTP web request.
    /// </summary>
    /// <param name="httpReq">The HTTP req.</param>
    /// <param name="contentLength">Length of the content.</param>
    /// <param name="allowAutoRedirect">if set to <c>true</c> [allow automatic redirect].</param>
    /// <param name="keepAlive">if set to <c>true</c> [keep alive].</param>
    public override void InitHttpWebRequest(HttpWebRequest httpReq,
                                            long? contentLength = null, bool allowAutoRedirect = true, bool keepAlive = true)
    {
        httpReq.UserAgent = Env.ServerUserAgent;
        httpReq.AllowAutoRedirect = allowAutoRedirect;
        httpReq.KeepAlive = keepAlive;

        if (contentLength != null)
        {
            SetContentLength(httpReq, contentLength.Value);
        }
    }

    /// <summary>
    /// Configurations the specified req.
    /// </summary>
    /// <param name="req">The req.</param>
    /// <param name="allowAutoRedirect">if set to <c>true</c> [allow automatic redirect].</param>
    /// <param name="timeout">The timeout.</param>
    /// <param name="readWriteTimeout">The read write timeout.</param>
    /// <param name="userAgent">The user agent.</param>
    /// <param name="preAuthenticate">if set to <c>true</c> [pre authenticate].</param>
    public override void Config(HttpWebRequest req,
                                bool? allowAutoRedirect = null,
                                TimeSpan? timeout = null,
                                TimeSpan? readWriteTimeout = null,
                                string userAgent = null,
                                bool? preAuthenticate = null)
    {
        try
        {
            //req.MaximumResponseHeadersLength = int.MaxValue; //throws "The message length limit was exceeded" exception
            if (allowAutoRedirect.HasValue) 
                req.AllowAutoRedirect = allowAutoRedirect.Value;

            if (userAgent != null)
                req.UserAgent = userAgent;

            if (readWriteTimeout.HasValue) req.ReadWriteTimeout = (int) readWriteTimeout.Value.TotalMilliseconds;
            if (timeout.HasValue) req.Timeout = (int) timeout.Value.TotalMilliseconds;

            if (preAuthenticate.HasValue)
                req.PreAuthenticate = preAuthenticate.Value;
        }
        catch (Exception ex)
        {
            Tracer.Instance.WriteError(ex);
        }
    }

    /// <summary>
    /// Gets the stack trace.
    /// </summary>
    /// <returns>System.String.</returns>
    public override string GetStackTrace() => Environment.StackTrace;

    /// <summary>
    /// Initializes for aot.
    /// </summary>
    public static void InitForAot()
    {
    }

    /// <summary>
    /// Class Poco.
    /// </summary>
    internal class Poco
    {
        /// <summary>
        /// Gets or sets the dummy.
        /// </summary>
        /// <value>The dummy.</value>
        public string Dummy { get; set; }
    }

    /// <summary>
    /// Registers for aot.
    /// </summary>
    public override void RegisterForAot()
    {
        RegisterTypeForAot<Poco>();

        RegisterElement<Poco, string>();

        RegisterElement<Poco, bool>();
        RegisterElement<Poco, char>();
        RegisterElement<Poco, byte>();
        RegisterElement<Poco, sbyte>();
        RegisterElement<Poco, short>();
        RegisterElement<Poco, ushort>();
        RegisterElement<Poco, int>();
        RegisterElement<Poco, uint>();

        RegisterElement<Poco, long>();
        RegisterElement<Poco, ulong>();
        RegisterElement<Poco, float>();
        RegisterElement<Poco, double>();
        RegisterElement<Poco, decimal>();

        RegisterElement<Poco, bool?>();
        RegisterElement<Poco, char?>();
        RegisterElement<Poco, byte?>();
        RegisterElement<Poco, sbyte?>();
        RegisterElement<Poco, short?>();
        RegisterElement<Poco, ushort?>();
        RegisterElement<Poco, int?>();
        RegisterElement<Poco, uint?>();
        RegisterElement<Poco, long?>();
        RegisterElement<Poco, ulong?>();
        RegisterElement<Poco, float?>();
        RegisterElement<Poco, double?>();
        RegisterElement<Poco, decimal?>();

        //RegisterElement<Poco, JsonValue>();

        RegisterTypeForAot<DayOfWeek>(); // used by DateTime

        // register built in structs
        RegisterTypeForAot<Guid>();
        RegisterTypeForAot<TimeSpan>();
        RegisterTypeForAot<DateTime>();
        RegisterTypeForAot<DateTimeOffset>();

        RegisterTypeForAot<Guid?>();
        RegisterTypeForAot<TimeSpan?>();
        RegisterTypeForAot<DateTime?>();
        RegisterTypeForAot<DateTimeOffset?>();
    }

    /// <summary>
    /// Registers the type for aot.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void RegisterTypeForAot<T>()
    {
        AotConfig.RegisterSerializers<T>();
    }

    /// <summary>
    /// Registers the query string writer.
    /// </summary>
    public static void RegisterQueryStringWriter()
    {
        var i = 0;
        if (QueryStringWriter<Poco>.WriteFn() != null) i++;
    }

    /// <summary>
    /// Registers the element.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TElement">The type of the t element.</typeparam>
    /// <returns>System.Int32.</returns>
    public static int RegisterElement<T, TElement>()
    {
        var i = 0;
        i += AotConfig.RegisterSerializers<TElement>();
        AotConfig.RegisterElement<T, TElement, JsonTypeSerializer>();
        AotConfig.RegisterElement<T, TElement, Text.Jsv.JsvTypeSerializer>();
        return i;
    }

    /// <summary>
    /// Class AotConfig.
    /// </summary>
    internal class AotConfig
    {
        /// <summary>
        /// The json reader
        /// </summary>
        internal static JsReader<JsonTypeSerializer> jsonReader;
        /// <summary>
        /// The json writer
        /// </summary>
        internal static JsWriter<JsonTypeSerializer> jsonWriter;
        /// <summary>
        /// The JSV reader
        /// </summary>
        internal static JsReader<Text.Jsv.JsvTypeSerializer> jsvReader;
        /// <summary>
        /// The JSV writer
        /// </summary>
        internal static JsWriter<Text.Jsv.JsvTypeSerializer> jsvWriter;
        /// <summary>
        /// The json serializer
        /// </summary>
        internal static JsonTypeSerializer jsonSerializer;
        /// <summary>
        /// The JSV serializer
        /// </summary>
        internal static Text.Jsv.JsvTypeSerializer jsvSerializer;

        /// <summary>
        /// Initializes static members of the <see cref="AotConfig"/> class.
        /// </summary>
        static AotConfig()
        {
            jsonSerializer = new JsonTypeSerializer();
            jsvSerializer = new Text.Jsv.JsvTypeSerializer();
            jsonReader = new JsReader<JsonTypeSerializer>();
            jsonWriter = new JsWriter<JsonTypeSerializer>();
            jsvReader = new JsReader<Text.Jsv.JsvTypeSerializer>();
            jsvWriter = new JsWriter<Text.Jsv.JsvTypeSerializer>();
        }

        /// <summary>
        /// Registers the serializers.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>System.Int32.</returns>
        internal static int RegisterSerializers<T>()
        {
            var i = 0;
            i += Register<T, JsonTypeSerializer>();
            if (jsonSerializer.GetParseFn<T>() != null) i++;
            if (jsonSerializer.GetWriteFn<T>() != null) i++;
            if (jsonReader.GetParseFn<T>() != null) i++;
            if (jsonWriter.GetWriteFn<T>() != null) i++;

            i += Register<T, Text.Jsv.JsvTypeSerializer>();
            if (jsvSerializer.GetParseFn<T>() != null) i++;
            if (jsvSerializer.GetWriteFn<T>() != null) i++;
            if (jsvReader.GetParseFn<T>() != null) i++;
            if (jsvWriter.GetWriteFn<T>() != null) i++;

            //RegisterCsvSerializer<T>();
            RegisterQueryStringWriter();
            return i;
        }

        /// <summary>
        /// Registers the CSV serializer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        internal static void RegisterCsvSerializer<T>()
        {
            CsvSerializer<T>.WriteFn();
            CsvSerializer<T>.WriteObject(null, null);
            CsvWriter<T>.Write(null, default(IEnumerable<T>));
            CsvWriter<T>.WriteRow(null, default(T));
        }

        /// <summary>
        /// Gets the parse function.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>ParseStringDelegate.</returns>
        public static ParseStringDelegate GetParseFn(Type type)
        {
            var parseFn = JsonTypeSerializer.Instance.GetParseFn(type);
            return parseFn;
        }

        /// <summary>
        /// Registers this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
        /// <returns>System.Int32.</returns>
        internal static int Register<T, TSerializer>() where TSerializer : ITypeSerializer
        {
            var i = 0;

            if (JsonWriter<T>.WriteFn() != null) i++;
            if (JsonWriter.Instance.GetWriteFn<T>() != null) i++;
            if (JsonReader.Instance.GetParseFn<T>() != null) i++;
            if (JsonReader<T>.Parse(default(ReadOnlySpan<char>)) != null) i++;
            if (JsonReader<T>.GetParseFn() != null) i++;
            //if (JsWriter.GetTypeSerializer<JsonTypeSerializer>().GetWriteFn<T>() != null) i++;
            if (new List<T>() != null) i++;
            if (new T[0] != null) i++;

            JsConfig<T>.ExcludeTypeInfo = false;

            if (JsConfig<T>.OnDeserializedFn != null) i++;
            if (JsConfig<T>.HasDeserializeFn) i++;
            if (JsConfig<T>.SerializeFn != null) i++;
            if (JsConfig<T>.DeSerializeFn != null) i++;
            //JsConfig<T>.SerializeFn = arg => "";
            //JsConfig<T>.DeSerializeFn = arg => default(T);
            if (TypeConfig<T>.Properties != null) i++;

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
            return i;
        }

        /// <summary>
        /// Registers the element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TElement">The type of the t element.</typeparam>
        /// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
        internal static void RegisterElement<T, TElement, TSerializer>() where TSerializer : ITypeSerializer
        {
            DeserializeDictionary<TSerializer>.ParseDictionary<T, TElement>(default(ReadOnlySpan<char>), null, null, null);
            DeserializeDictionary<TSerializer>.ParseDictionary<TElement, T>(default(ReadOnlySpan<char>), null, null, null);

            ToStringDictionaryMethods<T, TElement, TSerializer>.WriteIDictionary(null, null, null, null);
            ToStringDictionaryMethods<TElement, T, TSerializer>.WriteIDictionary(null, null, null, null);

            // Include List deserialisations from the Register<> method above.  This solves issue where List<Guid> properties on responses deserialise to null.
            // No idea why this is happening because there is no visible exception raised.  Suspect IOS is swallowing an AOT exception somewhere.
            DeserializeArrayWithElements<TElement, TSerializer>.ParseGenericArray(default(ReadOnlySpan<char>), null);
            DeserializeListWithElements<TElement, TSerializer>.ParseGenericList(default(ReadOnlySpan<char>), null, null);

            // Cannot use the line below for some unknown reason - when trying to compile to run on device, mtouch bombs during native code compile.
            // Something about this line or its inner workings is offensive to mtouch. Luckily this was not needed for my List<Guide> issue.
            // DeserializeCollection<JsonTypeSerializer>.ParseCollection<TElement>(null, null, null);

            TranslateListWithElements<TElement>.LateBoundTranslateToGenericICollection(null, typeof(List<TElement>));
            TranslateListWithConvertibleElements<TElement, TElement>.LateBoundTranslateToGenericICollection(null, typeof(List<TElement>));
        }
    }

}