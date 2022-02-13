// ***********************************************************************
// <copyright file="PclExport.Net48.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
#if NETFX
namespace ServiceStack.Text
{
    using ServiceStack.Text.Common;
    using ServiceStack.Text.Json;

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;

    /// <summary>
    /// Class NetFxPclExport.
    /// Implements the <see cref="PclExport" />
    /// </summary>
    /// <seealso cref="PclExport" />
    public class NetFxPclExport : PclExport
    {
        /// <summary>
        /// The provider
        /// </summary>
        public static NetFxPclExport Provider = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="NetFxPclExport"/> class.
        /// </summary>
        public NetFxPclExport()
        {
            this.DirSep = Path.DirectorySeparatorChar;
            this.AltDirSep = Path.DirectorySeparatorChar == '/' ? '\\' : '/';
            this.RegexOptions = RegexOptions.Compiled;
            this.InvariantComparison = StringComparison.InvariantCulture;
            this.InvariantComparisonIgnoreCase = StringComparison.InvariantCultureIgnoreCase;
            this.InvariantComparer = StringComparer.InvariantCulture;
            this.InvariantComparerIgnoreCase = StringComparer.InvariantCultureIgnoreCase;

            this.PlatformName = Platforms.NetFx;
            ReflectionOptimizer.Instance = EmitReflectionOptimizer.Provider;
        }

        /// <summary>
        /// Configures this instance.
        /// </summary>
        /// <returns>ServiceStack.PclExport.</returns>
        public static PclExport Configure()
        {
            Configure(Provider);
            return Provider;
        }

        /// <summary>
        /// Reads all text.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>System.String.</returns>
        public override string ReadAllText(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        /// <summary>
        /// Converts to invariantupper.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public override string ToInvariantUpper(char value)
        {
            return value.ToString(CultureInfo.InvariantCulture).ToUpper();
        }

        /// <summary>
        /// Determines whether [is anonymous type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is anonymous type] [the specified type]; otherwise, <c>false</c>.</returns>
        public override bool IsAnonymousType(Type type)
        {
            return type.HasAttribute<CompilerGeneratedAttribute>()
                   && type.IsGenericType && type.Name.Contains("AnonymousType")
                   && (type.Name.StartsWith("<>", StringComparison.Ordinal) || type.Name.StartsWith("VB$", StringComparison.Ordinal))
                   && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
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
        /// Gets the environment variable.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public override string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="line">The line.</param>
        public override void WriteLine(string line)
        {
            Console.WriteLine(line);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public override void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }

        /// <summary>
        /// Adds the compression.
        /// </summary>
        /// <param name="webReq">The web req.</param>
        public override void AddCompression(WebRequest webReq)
        {
            var httpReq = (HttpWebRequest)webReq;
            httpReq.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            httpReq.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        }

        /// <summary>
        /// Gets the request stream.
        /// </summary>
        /// <param name="webRequest">The web request.</param>
        /// <returns>Stream.</returns>
        public override Stream GetRequestStream(WebRequest webRequest)
        {
            return webRequest.GetRequestStream();
        }

        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <param name="webRequest">The web request.</param>
        /// <returns>WebResponse.</returns>
        public override WebResponse GetResponse(WebRequest webRequest)
        {
            return webRequest.GetResponse();
        }

#if !LITE
        /// <summary>
        /// Determines whether [is debug build] [the specified assembly].
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns><c>true</c> if [is debug build] [the specified assembly]; otherwise, <c>false</c>.</returns>
        public override bool IsDebugBuild(Assembly assembly)
        {
            return assembly.AllAttributes()
                .OfType<System.Diagnostics.DebuggableAttribute>()
                .Select(attr => attr.IsJITTrackingEnabled)
                .FirstOrDefault();
        }
#endif

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
                var assemblyDirectoryPath = Path.GetDirectoryName(new Uri(typeof(PathUtils).Assembly.EscapedCodeBase).LocalPath);

                // Escape the assembly bin directory to the hostname directory
                var hostDirectoryPath = appendPartialPathModifier != null
                    ? assemblyDirectoryPath + appendPartialPathModifier
                    : assemblyDirectoryPath;

                return Path.GetFullPath(relativePath.Replace("~", hostDirectoryPath));
            }
            return relativePath;
        }

        /// <summary>
        /// Loads the assembly.
        /// </summary>
        /// <param name="assemblyPath">The assembly path.</param>
        /// <returns>Assembly.</returns>
        public override Assembly LoadAssembly(string assemblyPath)
        {
            return Assembly.LoadFrom(assemblyPath);
        }

        /// <summary>
        /// Adds the header.
        /// </summary>
        /// <param name="webReq">The web req.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public override void AddHeader(WebRequest webReq, string name, string value)
        {
            webReq.Headers.Add(name, value);
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
        /// Finds the type.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>Type.</returns>
        public override Type FindType(string typeName, string assemblyName)
        {
            var binPath = AssemblyUtils.GetAssemblyBinPath(Assembly.GetExecutingAssembly());
            Assembly assembly = null;
            var assemblyDllPath = binPath + $"{assemblyName}.dll";
            if (File.Exists(assemblyDllPath))
            {
                assembly = AssemblyUtils.LoadAssembly(assemblyDllPath);
            }
            var assemblyExePath = binPath + $"{assemblyName}.exe";
            if (File.Exists(assemblyExePath))
            {
                assembly = AssemblyUtils.LoadAssembly(assemblyExePath);
            }
            return assembly?.GetType(typeName);
        }

        /// <summary>
        /// Gets the assembly code base.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>System.String.</returns>
        public override string GetAssemblyCodeBase(Assembly assembly)
        {
            return assembly.CodeBase;
        }

        /// <summary>
        /// Gets the assembly path.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>System.String.</returns>
        public override string GetAssemblyPath(Type source)
        {
            var assemblyUri = new Uri(source.Assembly.EscapedCodeBase);
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
            return Encoding.ASCII.GetString(bytes, index, count);
        }

        /// <summary>
        /// Gets the ASCII bytes.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>System.Byte[].</returns>
        public override byte[] GetAsciiBytes(string str)
        {
            return Encoding.ASCII.GetBytes(str);
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
            return type.FindInterfaces((t, critera) =>
                t.IsGenericType
                && t.GetGenericTypeDefinition() == typeof(ICollection<>), null).FirstOrDefault();
        }

        /// <summary>
        /// Converts to xsddatetimestring.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>System.String.</returns>
        public override string ToXsdDateTimeString(DateTime dateTime)
        {
#if !LITE
            return System.Xml.XmlConvert.ToString(dateTime.ToStableUniversalTime(), System.Xml.XmlDateTimeSerializationMode.Utc);
#else
            return dateTime.ToStableUniversalTime().ToString(DateTimeSerializer.XsdDateTimeFormat);
#endif
        }

        /// <summary>
        /// Converts to localxsddatetimestring.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>System.String.</returns>
        public override string ToLocalXsdDateTimeString(DateTime dateTime)
        {
#if !LITE
            return System.Xml.XmlConvert.ToString(dateTime, System.Xml.XmlDateTimeSerializationMode.Local);
#else
            return dateTime.ToString(DateTimeSerializer.XsdDateTimeFormat);
#endif
        }

        /// <summary>
        /// Parses the XSD date time.
        /// </summary>
        /// <param name="dateTimeStr">The date time string.</param>
        /// <returns>DateTime.</returns>
        public override DateTime ParseXsdDateTime(string dateTimeStr)
        {
#if !LITE
            return System.Xml.XmlConvert.ToDateTime(dateTimeStr, System.Xml.XmlDateTimeSerializationMode.Utc);
#else
            return DateTime.ParseExact(dateTimeStr, DateTimeSerializer.XsdDateTimeFormat, CultureInfo.InvariantCulture);
#endif
        }

#if !LITE
        /// <summary>
        /// Parses the XSD date time as UTC.
        /// </summary>
        /// <param name="dateTimeStr">The date time string.</param>
        /// <returns>DateTime.</returns>
        public override DateTime ParseXsdDateTimeAsUtc(string dateTimeStr)
        {
            return System.Xml.XmlConvert.ToDateTime(dateTimeStr, System.Xml.XmlDateTimeSerializationMode.Utc).Prepare(true);
        }
#endif

        /// <summary>
        /// Converts to stableuniversaltime.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>DateTime.</returns>
        public override DateTime ToStableUniversalTime(DateTime dateTime)
        {
            // .Net 2.0 - 3.5 has an issue with DateTime.ToUniversalTime, but works ok with TimeZoneInfo.ConvertTimeToUtc.
            // .Net 4.0+ does this under the hood anyway.
            return TimeZoneInfo.ConvertTimeToUtc(dateTime);
        }

        /// <summary>
        /// Gets the dictionary parse method.
        /// </summary>
        /// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns>ParseStringDelegate.</returns>
        public override ParseStringDelegate GetDictionaryParseMethod<TSerializer>(Type type)
        {
            if (type == typeof(Hashtable))
            {
                return SerializerUtils<TSerializer>.ParseHashtable;
            }
            return null;
        }

        /// <summary>
        /// Gets the dictionary parse string span method.
        /// </summary>
        /// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns>ParseStringSpanDelegate.</returns>
        public override ParseStringSpanDelegate GetDictionaryParseStringSpanMethod<TSerializer>(Type type)
        {
            if (type == typeof(Hashtable))
            {
                return SerializerUtils<TSerializer>.ParseHashtable;
            }
            return null;
        }

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
                return SerializerUtils<TSerializer>.ParseStringCollection<TSerializer>;
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
                return SerializerUtils<TSerializer>.ParseStringCollection<TSerializer>;
            }
            return null;
        }

        /// <summary>
        /// Gets the js reader parse string span method.
        /// </summary>
        /// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns>ParseStringSpanDelegate.</returns>
        public override ParseStringSpanDelegate GetJsReaderParseStringSpanMethod<TSerializer>(Type type)
        {
#if !LITE
            if (type.IsAssignableFrom(typeof(System.Dynamic.IDynamicMetaObjectProvider)) ||
                type.HasInterface(typeof(System.Dynamic.IDynamicMetaObjectProvider)))
            {
                return DeserializeDynamic<TSerializer>.ParseStringSpan;
            }
#endif
            return null;
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
                httpReq.ContentLength = contentLength.Value;
            }
        }

        /// <summary>
        /// Closes the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public override void CloseStream(Stream stream)
        {
            stream.Close();
        }

        /// <summary>
        /// Begins the thread affinity.
        /// </summary>
        public override void BeginThreadAffinity()
        {
            Thread.BeginThreadAffinity();
        }

        /// <summary>
        /// Ends the thread affinity.
        /// </summary>
        public override void EndThreadAffinity()
        {
            Thread.EndThreadAffinity();
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
            req.MaximumResponseHeadersLength = int.MaxValue; //throws "The message length limit was exceeded" exception
            if (allowAutoRedirect.HasValue) req.AllowAutoRedirect = allowAutoRedirect.Value;
            if (readWriteTimeout.HasValue) req.ReadWriteTimeout = (int)readWriteTimeout.Value.TotalMilliseconds;
            if (timeout.HasValue) req.Timeout = (int)timeout.Value.TotalMilliseconds;
            if (userAgent != null) req.UserAgent = userAgent;
            if (preAuthenticate.HasValue) req.PreAuthenticate = preAuthenticate.Value;
        }

        /// <summary>
        /// Sets the user agent.
        /// </summary>
        /// <param name="httpReq">The HTTP req.</param>
        /// <param name="value">The value.</param>
        public override void SetUserAgent(HttpWebRequest httpReq, string value)
        {
            httpReq.UserAgent = value;
        }

        /// <summary>
        /// Sets the length of the content.
        /// </summary>
        /// <param name="httpReq">The HTTP req.</param>
        /// <param name="value">The value.</param>
        public override void SetContentLength(HttpWebRequest httpReq, long value)
        {
            httpReq.ContentLength = value;
        }

        /// <summary>
        /// Sets the allow automatic redirect.
        /// </summary>
        /// <param name="httpReq">The HTTP req.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public override void SetAllowAutoRedirect(HttpWebRequest httpReq, bool value)
        {
            httpReq.AllowAutoRedirect = value;
        }

        /// <summary>
        /// Gets the stack trace.
        /// </summary>
        /// <returns>System.String.</returns>
        public override string GetStackTrace()
        {
            return Environment.StackTrace;
        }

        /// <summary>
        /// Gets the weak data contract.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>DataContractAttribute.</returns>
        public override DataContractAttribute GetWeakDataContract(Type type)
        {
            return type.GetWeakDataContract();
        }

        /// <summary>
        /// Gets the weak data member.
        /// </summary>
        /// <param name="pi">The pi.</param>
        /// <returns>DataMemberAttribute.</returns>
        public override DataMemberAttribute GetWeakDataMember(PropertyInfo pi)
        {
            return pi.GetWeakDataMember();
        }

        /// <summary>
        /// Gets the weak data member.
        /// </summary>
        /// <param name="pi">The pi.</param>
        /// <returns>DataMemberAttribute.</returns>
        public override DataMemberAttribute GetWeakDataMember(FieldInfo pi)
        {
            return pi.GetWeakDataMember();
        }
    }

    /// <summary>
    /// Class SerializerUtils.
    /// </summary>
    /// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
    internal class SerializerUtils<TSerializer>
        where TSerializer : ITypeSerializer
    {
        /// <summary>
        /// The serializer
        /// </summary>
        private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

        /// <summary>
        /// Verifies the start index of the and get.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="createMapType">Type of the create map.</param>
        /// <returns>int.</returns>
        private static int VerifyAndGetStartIndex(ReadOnlySpan<char> value, Type createMapType)
        {
            var index = 0;
            if (!Serializer.EatMapStartChar(value, ref index))
            {
                //Don't throw ex because some KeyValueDataContractDeserializer don't have '{}'
                Tracer.Instance.WriteDebug("WARN: Map definitions should start with a '{0}', expecting serialized type '{1}', got string starting with: {2}",
                    JsWriter.MapStartChar, createMapType != null ? createMapType.Name : "Dictionary<,>", value.Substring(0, value.Length < 50 ? value.Length : 50));
            }
            return index;
        }

        /// <summary>
        /// Parses the hashtable.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Collections.Hashtable.</returns>
        public static Hashtable ParseHashtable(string value) => ParseHashtable(value.AsSpan());

        /// <summary>
        /// Parses the hashtable.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Collections.Hashtable.</returns>
        public static Hashtable ParseHashtable(ReadOnlySpan<char> value)
        {
            if (value.IsEmpty)
                return null;

            var index = VerifyAndGetStartIndex(value, typeof(Hashtable));

            var result = new Hashtable();

            if (JsonTypeSerializer.IsEmptyMap(value, index)) return result;

            var valueLength = value.Length;
            while (index < valueLength)
            {
                var keyValue = Serializer.EatMapKey(value, ref index);
                Serializer.EatMapKeySeperator(value, ref index);
                var elementValue = Serializer.EatValue(value, ref index);
                if (keyValue.IsEmpty) continue;

                var mapKey = keyValue.ToString();
                var mapValue = elementValue.Value();

                result[mapKey] = mapValue;

                Serializer.EatItemSeperatorOrMapEndChar(value, ref index);
            }

            return result;
        }

        /// <summary>
        /// Parses the string collection.
        /// </summary>
        /// <typeparam name="TS">The type of the ts.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>System.Collections.Specialized.StringCollection.</returns>
        public static StringCollection ParseStringCollection<TS>(string value) where TS : ITypeSerializer => ParseStringCollection<TS>(value.AsSpan());


        /// <summary>
        /// Parses the string collection.
        /// </summary>
        /// <typeparam name="TS">The type of the ts.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>System.Collections.Specialized.StringCollection.</returns>
        public static StringCollection ParseStringCollection<TS>(ReadOnlySpan<char> value) where TS : ITypeSerializer
        {
            if ((value = DeserializeListWithElements<TS>.StripList(value)).IsNullOrEmpty())
                return value.IsEmpty ? null : new StringCollection();

            return ToStringCollection(DeserializeListWithElements<TSerializer>.ParseStringList(value));
        }

        /// <summary>
        /// Converts to stringcollection.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>System.Collections.Specialized.StringCollection.</returns>
        public static StringCollection ToStringCollection(List<string> items)
        {
            var to = new StringCollection();
            foreach (var item in items)
            {
                to.Add(item);
            }
            return to;
        }
    }

    /// <summary>
    /// Class PclExportExt.
    /// </summary>
    public static class PclExportExt
    {
        //XmlSerializer
        /// <summary>
        /// Compresses to stream.
        /// </summary>
        /// <typeparam name="TXmlDto">The type of the t XML dto.</typeparam>
        /// <param name="from">From.</param>
        /// <param name="stream">The stream.</param>
        public static void CompressToStream<TXmlDto>(TXmlDto from, Stream stream)
        {
            using var deflateStream = new System.IO.Compression.DeflateStream(stream, System.IO.Compression.CompressionMode.Compress);
            using var xw = new System.Xml.XmlTextWriter(deflateStream, Encoding.UTF8);
            var serializer = new DataContractSerializer(@from.GetType());
            serializer.WriteObject(xw, @from);
            xw.Flush();
        }

        /// <summary>
        /// Compresses the specified from.
        /// </summary>
        /// <typeparam name="TXmlDto">The type of the t XML dto.</typeparam>
        /// <param name="from">From.</param>
        /// <returns>byte[].</returns>
        public static byte[] Compress<TXmlDto>(TXmlDto from)
        {
            using var ms = MemoryStreamFactory.GetStream();
            CompressToStream(from, ms);

            return ms.ToArray();
        }

        //ReflectionExtensions
        /// <summary>
        /// The data contract
        /// </summary>
        private const string DataContract = "DataContractAttribute";

        /// <summary>
        /// Gets the weak data contract.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.Runtime.Serialization.DataContractAttribute.</returns>
        public static DataContractAttribute GetWeakDataContract(this Type type)
        {
            var attr = type.AllAttributes().FirstOrDefault(x => x.GetType().Name == DataContract);
            if (attr != null)
            {
                var attrType = attr.GetType();

                var accessor = TypeProperties.Get(attr.GetType());

                return new DataContractAttribute
                {
                    Name = (string)accessor.GetPublicGetter("Name")(attr),
                    Namespace = (string)accessor.GetPublicGetter("Namespace")(attr),
                };
            }
            return null;
        }

        /// <summary>
        /// Gets the weak data member.
        /// </summary>
        /// <param name="pi">The pi.</param>
        /// <returns>System.Runtime.Serialization.DataMemberAttribute.</returns>
        public static DataMemberAttribute GetWeakDataMember(this PropertyInfo pi)
        {
            var attr = pi.AllAttributes().FirstOrDefault(x => x.GetType().Name == ReflectionExtensions.DataMember);
            if (attr != null)
            {
                var attrType = attr.GetType();

                var accessor = TypeProperties.Get(attr.GetType());

                var newAttr = new DataMemberAttribute
                {
                    Name = (string)accessor.GetPublicGetter("Name")(attr),
                    EmitDefaultValue = (bool)accessor.GetPublicGetter("EmitDefaultValue")(attr),
                    IsRequired = (bool)accessor.GetPublicGetter("IsRequired")(attr),
                };

                var order = (int)accessor.GetPublicGetter("Order")(attr);
                if (order >= 0)
                    newAttr.Order = order; //Throws Exception if set to -1

                return newAttr;
            }
            return null;
        }

        /// <summary>
        /// Gets the weak data member.
        /// </summary>
        /// <param name="pi">The pi.</param>
        /// <returns>System.Runtime.Serialization.DataMemberAttribute.</returns>
        public static DataMemberAttribute GetWeakDataMember(this FieldInfo pi)
        {
            var attr = pi.AllAttributes().FirstOrDefault(x => x.GetType().Name == ReflectionExtensions.DataMember);
            if (attr != null)
            {
                var attrType = attr.GetType();

                var accessor = TypeProperties.Get(attr.GetType());

                var newAttr = new DataMemberAttribute
                {
                    Name = (string)accessor.GetPublicGetter("Name")(attr),
                    EmitDefaultValue = (bool)accessor.GetPublicGetter("EmitDefaultValue")(attr),
                    IsRequired = (bool)accessor.GetPublicGetter("IsRequired")(attr),
                };

                var order = (int)accessor.GetPublicGetter("Order")(attr);
                if (order >= 0)
                    newAttr.Order = order; //Throws Exception if set to -1

                return newAttr;
            }
            return null;
        }
    }

    //Not using it here, but @marcgravell's stuff is too good not to include
    // http://code.google.com/p/fast-member/ Apache License 2.0
    /// <summary>
    /// Represents an individual object, allowing access to members by-name
    /// </summary>
    public abstract class ObjectAccessor
    {
        /// <summary>
        /// Get or Set the value of a named member for the underlying object
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>object.</returns>
        public abstract object this[string name] { get; set; }
        /// <summary>
        /// The object represented by this instance
        /// </summary>
        /// <value>The target.</value>
        public abstract object Target { get; }
        /// <summary>
        /// Use the target types definition of equality
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>bool.</returns>
        public override bool Equals(object obj)
        {
            return Target.Equals(obj);
        }
        /// <summary>
        /// Obtain the hash of the target object
        /// </summary>
        /// <returns>int.</returns>
        public override int GetHashCode()
        {
            return Target.GetHashCode();
        }
        /// <summary>
        /// Use the target's definition of a string representation
        /// </summary>
        /// <returns>string.</returns>
        public override string ToString()
        {
            return Target.ToString();
        }

        /// <summary>
        /// Wraps an individual object, allowing by-name access to that instance
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>ServiceStack.Text.FastMember.ObjectAccessor.</returns>
        /// <exception cref="ArgumentNullException">target</exception>
        public static ObjectAccessor Create(object target)
        {
            if (target == null) throw new ArgumentNullException("target");
            //IDynamicMetaObjectProvider dlr = target as IDynamicMetaObjectProvider;
            //if (dlr != null) return new DynamicWrapper(dlr); // use the DLR
            return new TypeAccessorWrapper(target, TypeAccessor.Create(target.GetType()));
        }

        /// <summary>
        /// Class TypeAccessorWrapper. This class cannot be inherited.
        /// Implements the <see cref="ObjectAccessor" />
        /// </summary>
        /// <seealso cref="ObjectAccessor" />
        private sealed class TypeAccessorWrapper : ObjectAccessor
        {
            /// <summary>
            /// The accessor
            /// </summary>
            private readonly TypeAccessor accessor;
            /// <summary>
            /// Initializes a new instance of the <see cref="TypeAccessorWrapper"/> class.
            /// </summary>
            /// <param name="target">The target.</param>
            /// <param name="accessor">The accessor.</param>
            public TypeAccessorWrapper(object target, TypeAccessor accessor)
            {
                this.Target = target;
                this.accessor = accessor;
            }
            /// <summary>
            /// Get or Set the value of a named member for the underlying object
            /// </summary>
            /// <param name="name">The name.</param>
            /// <returns>object.</returns>
            public override object this[string name]
            {
                get => accessor[Target, name.ToUpperInvariant()];
                set => accessor[Target, name.ToUpperInvariant()] = value;
            }
            /// <summary>
            /// The object represented by this instance
            /// </summary>
            /// <value>The target.</value>
            public override object Target { get; }
        }

        //sealed class DynamicWrapper : ObjectAccessor
        //{
        //    private readonly IDynamicMetaObjectProvider target;
        //    public override object Target
        //    {
        //        get { return target; }
        //    }
        //    public DynamicWrapper(IDynamicMetaObjectProvider target)
        //    {
        //        this.target = target;
        //    }
        //    public override object this[string name]
        //    {
        //        get { return CallSiteCache.GetValue(name, target); }
        //        set { CallSiteCache.SetValue(name, target, value); }
        //    }
        //}
    }

    /// <summary>
    /// Provides by-name member-access to objects of a given type
    /// </summary>
    public abstract class TypeAccessor
    {
        // hash-table has better read-without-locking semantics than dictionary
        /// <summary>
        /// The type lookyp
        /// </summary>
        private static readonly Hashtable typeLookyp = new();

        /// <summary>
        /// Create a new instance of this type
        /// </summary>
        /// <returns>object.</returns>
        /// <exception cref="NotSupportedException"></exception>
        public virtual object CreateNew() { throw new NotSupportedException(); }

        /// <summary>
        /// Provides a type-specific accessor, allowing by-name access for all objects of that type
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>ServiceStack.Text.FastMember.TypeAccessor.</returns>
        /// <exception cref="ArgumentNullException">nameof(type)</exception>
        /// <remarks>The accessor is cached internally; a pre-existing accessor may be returned</remarks>
        public static TypeAccessor Create(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            var obj = (TypeAccessor)typeLookyp[type];
            if (obj != null) return obj;

            lock (typeLookyp)
            {
                // double-check
                obj = (TypeAccessor)typeLookyp[type];
                if (obj != null) return obj;

                obj = CreateNew(type);

                typeLookyp[type] = obj;
                return obj;
            }
        }

        //sealed class DynamicAccessor : TypeAccessor
        //{
        //    public static readonly DynamicAccessor Singleton = new DynamicAccessor();
        //    private DynamicAccessor(){}
        //    public override object this[object target, string name]
        //    {
        //        get { return CallSiteCache.GetValue(name, target); }
        //        set { CallSiteCache.SetValue(name, target, value); }
        //    }
        //}

        /// <summary>
        /// The assembly
        /// </summary>
        private static AssemblyBuilder assembly;
        /// <summary>
        /// The module
        /// </summary>
        private static ModuleBuilder module;
        /// <summary>
        /// The counter
        /// </summary>
        private static int counter;

        /// <summary>
        /// Writes the getter.
        /// </summary>
        /// <param name="il">The il.</param>
        /// <param name="type">The type.</param>
        /// <param name="props">The props.</param>
        /// <param name="fields">The fields.</param>
        /// <param name="isStatic">The is static.</param>
        private static void WriteGetter(ILGenerator il, Type type, IEnumerable<PropertyInfo> props, IEnumerable<FieldInfo> fields, bool isStatic)
        {
            LocalBuilder loc = type.IsValueType ? il.DeclareLocal(type) : null;
            OpCode propName = isStatic ? OpCodes.Ldarg_1 : OpCodes.Ldarg_2, target = isStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1;
            foreach (PropertyInfo prop in props)
            {
                if (prop.GetIndexParameters().Length != 0 || !prop.CanRead) continue;
                var getFn = prop.GetGetMethod();
                if (getFn == null) continue; //Mono

                Label next = il.DefineLabel();
                il.Emit(propName);
                il.Emit(OpCodes.Ldstr, prop.Name);
                il.EmitCall(OpCodes.Call, strinqEquals, null);
                il.Emit(OpCodes.Brfalse_S, next);
                // match:
                il.Emit(target);
                Cast(il, type, loc);
                il.EmitCall(type.IsValueType ? OpCodes.Call : OpCodes.Callvirt, getFn, null);
                if (prop.PropertyType.IsValueType)
                {
                    il.Emit(OpCodes.Box, prop.PropertyType);
                }
                il.Emit(OpCodes.Ret);
                // not match:
                il.MarkLabel(next);
            }
            foreach (FieldInfo field in fields)
            {
                Label next = il.DefineLabel();
                il.Emit(propName);
                il.Emit(OpCodes.Ldstr, field.Name);
                il.EmitCall(OpCodes.Call, strinqEquals, null);
                il.Emit(OpCodes.Brfalse_S, next);
                // match:
                il.Emit(target);
                Cast(il, type, loc);
                il.Emit(OpCodes.Ldfld, field);
                if (field.FieldType.IsValueType)
                {
                    il.Emit(OpCodes.Box, field.FieldType);
                }
                il.Emit(OpCodes.Ret);
                // not match:
                il.MarkLabel(next);
            }
            il.Emit(OpCodes.Ldstr, "name");
            il.Emit(OpCodes.Newobj, typeof(ArgumentOutOfRangeException).GetConstructor(new Type[] { typeof(string) }));
            il.Emit(OpCodes.Throw);
        }
        /// <summary>
        /// Writes the setter.
        /// </summary>
        /// <param name="il">The il.</param>
        /// <param name="type">The type.</param>
        /// <param name="props">The props.</param>
        /// <param name="fields">The fields.</param>
        /// <param name="isStatic">The is static.</param>
        private static void WriteSetter(ILGenerator il, Type type, IEnumerable<PropertyInfo> props, IEnumerable<FieldInfo> fields, bool isStatic)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Ldstr, "Write is not supported for structs");
                il.Emit(OpCodes.Newobj, typeof(NotSupportedException).GetConstructor(new Type[] { typeof(string) }));
                il.Emit(OpCodes.Throw);
            }
            else
            {
                OpCode propName = isStatic ? OpCodes.Ldarg_1 : OpCodes.Ldarg_2,
                    target = isStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1,
                    value = isStatic ? OpCodes.Ldarg_2 : OpCodes.Ldarg_3;
                LocalBuilder loc = type.IsValueType ? il.DeclareLocal(type) : null;
                foreach (PropertyInfo prop in props)
                {
                    if (prop.GetIndexParameters().Length != 0 || !prop.CanWrite) continue;
                    var setFn = prop.GetSetMethod();
                    if (setFn == null) continue; //Mono

                    Label next = il.DefineLabel();
                    il.Emit(propName);
                    il.Emit(OpCodes.Ldstr, prop.Name);
                    il.EmitCall(OpCodes.Call, strinqEquals, null);
                    il.Emit(OpCodes.Brfalse_S, next);
                    // match:
                    il.Emit(target);
                    Cast(il, type, loc);
                    il.Emit(value);
                    Cast(il, prop.PropertyType, null);
                    il.EmitCall(type.IsValueType ? OpCodes.Call : OpCodes.Callvirt, setFn, null);
                    il.Emit(OpCodes.Ret);
                    // not match:
                    il.MarkLabel(next);
                }
                foreach (FieldInfo field in fields)
                {
                    Label next = il.DefineLabel();
                    il.Emit(propName);
                    il.Emit(OpCodes.Ldstr, field.Name);
                    il.EmitCall(OpCodes.Call, strinqEquals, null);
                    il.Emit(OpCodes.Brfalse_S, next);
                    // match:
                    il.Emit(target);
                    Cast(il, type, loc);
                    il.Emit(value);
                    Cast(il, field.FieldType, null);
                    il.Emit(OpCodes.Stfld, field);
                    il.Emit(OpCodes.Ret);
                    // not match:
                    il.MarkLabel(next);
                }
                il.Emit(OpCodes.Ldstr, "name");
                il.Emit(OpCodes.Newobj, typeof(ArgumentOutOfRangeException).GetConstructor(new Type[] { typeof(string) }));
                il.Emit(OpCodes.Throw);
            }
        }
        /// <summary>
        /// The strinq equals
        /// </summary>
        private static readonly MethodInfo strinqEquals = typeof(string).GetMethod("op_Equality", new Type[] { typeof(string), typeof(string) });

        /// <summary>
        /// Class DelegateAccessor. This class cannot be inherited.
        /// Implements the <see cref="TypeAccessor" />
        /// </summary>
        /// <seealso cref="TypeAccessor" />
        private sealed class DelegateAccessor : TypeAccessor
        {
            /// <summary>
            /// The getter
            /// </summary>
            private readonly Func<object, string, object> getter;
            /// <summary>
            /// The setter
            /// </summary>
            private readonly Action<object, string, object> setter;
            /// <summary>
            /// The ctor
            /// </summary>
            private readonly Func<object> ctor;
            /// <summary>
            /// Initializes a new instance of the <see cref="DelegateAccessor"/> class.
            /// </summary>
            /// <param name="getter">The getter.</param>
            /// <param name="setter">The setter.</param>
            /// <param name="ctor">The ctor.</param>
            public DelegateAccessor(Func<object, string, object> getter, Action<object, string, object> setter, Func<object> ctor)
            {
                this.getter = getter;
                this.setter = setter;
                this.ctor = ctor;
            }
            /// <summary>
            /// Does this type support new instances via a parameterless constructor?
            /// </summary>
            /// <value>The create new supported.</value>
            public bool CreateNewSupported => ctor != null;

            /// <summary>
            /// Create a new instance of this type
            /// </summary>
            /// <returns>object.</returns>
            public override object CreateNew()
            {
                return ctor != null ? ctor() : base.CreateNew();
            }
            /// <summary>
            /// Get or set the value of a named member on the target instance
            /// </summary>
            /// <param name="target">The target.</param>
            /// <param name="name">The name.</param>
            /// <returns>object.</returns>
            public override object this[object target, string name]
            {
                get => getter(target, name);
                set => setter(target, name, value);
            }
        }

        /// <summary>
        /// Determines whether [is fully public] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>bool.</returns>
        private static bool IsFullyPublic(Type type)
        {
            while (type.IsNestedPublic) type = type.DeclaringType;
            return type.IsPublic;
        }

        /// <summary>
        /// Creates the new.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>ServiceStack.Text.FastMember.TypeAccessor.</returns>
        private static TypeAccessor CreateNew(Type type)
        {
            //if (typeof(IDynamicMetaObjectProvider).IsAssignableFrom(type))
            //{
            //    return DynamicAccessor.Singleton;
            //}

            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            ConstructorInfo ctor = null;
            if (type.IsClass && !type.IsAbstract)
            {
                ctor = type.GetConstructor(Type.EmptyTypes);
            }
            ILGenerator il;
            if (!IsFullyPublic(type))
            {
                DynamicMethod dynGetter = new(type.FullName + "_get", typeof(object), new Type[] { typeof(object), typeof(string) }, type, true),
                    dynSetter = new(type.FullName + "_set", null, new Type[] { typeof(object), typeof(string), typeof(object) }, type, true);
                WriteGetter(dynGetter.GetILGenerator(), type, props, fields, true);
                WriteSetter(dynSetter.GetILGenerator(), type, props, fields, true);
                DynamicMethod dynCtor = null;
                if (ctor != null)
                {
                    dynCtor = new DynamicMethod(type.FullName + "_ctor", typeof(object), Type.EmptyTypes, type, true);
                    il = dynCtor.GetILGenerator();
                    il.Emit(OpCodes.Newobj, ctor);
                    il.Emit(OpCodes.Ret);
                }
                return new DelegateAccessor(
                    (Func<object, string, object>)dynGetter.CreateDelegate(typeof(Func<object, string, object>)),
                    (Action<object, string, object>)dynSetter.CreateDelegate(typeof(Action<object, string, object>)),
                    dynCtor == null ? null : (Func<object>)dynCtor.CreateDelegate(typeof(Func<object>)));
            }

            // note this region is synchronized; only one is being created at a time so we don't need to stress about the builders
            if (assembly == null)
            {
                AssemblyName name = new("FastMember_dynamic");
                assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
                module = assembly.DefineDynamicModule(name.Name);
            }
            TypeBuilder tb = module.DefineType("FastMember_dynamic." + type.Name + "_" + Interlocked.Increment(ref counter),
                (typeof(TypeAccessor).Attributes | TypeAttributes.Sealed) & ~TypeAttributes.Abstract, typeof(TypeAccessor));

            tb.DefineDefaultConstructor(MethodAttributes.Public);
            PropertyInfo indexer = typeof(TypeAccessor).GetProperty("Item");
            MethodInfo baseGetter = indexer.GetGetMethod(), baseSetter = indexer.GetSetMethod();
            MethodBuilder body = tb.DefineMethod(baseGetter.Name, baseGetter.Attributes & ~MethodAttributes.Abstract, typeof(object), new Type[] { typeof(object), typeof(string) });
            il = body.GetILGenerator();
            WriteGetter(il, type, props, fields, false);
            tb.DefineMethodOverride(body, baseGetter);

            body = tb.DefineMethod(baseSetter.Name, baseSetter.Attributes & ~MethodAttributes.Abstract, null, new Type[] { typeof(object), typeof(string), typeof(object) });
            il = body.GetILGenerator();
            WriteSetter(il, type, props, fields, false);
            tb.DefineMethodOverride(body, baseSetter);

            if (ctor != null)
            {
                MethodInfo baseMethod = typeof(TypeAccessor).GetProperty("CreateNewSupported").GetGetMethod();
                body = tb.DefineMethod(baseMethod.Name, baseMethod.Attributes, typeof(bool), Type.EmptyTypes);
                il = body.GetILGenerator();
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Ret);
                tb.DefineMethodOverride(body, baseMethod);

                baseMethod = typeof(TypeAccessor).GetMethod("CreateNew");
                body = tb.DefineMethod(baseMethod.Name, baseMethod.Attributes, typeof(object), Type.EmptyTypes);
                il = body.GetILGenerator();
                il.Emit(OpCodes.Newobj, ctor);
                il.Emit(OpCodes.Ret);
                tb.DefineMethodOverride(body, baseMethod);
            }

            return (TypeAccessor)Activator.CreateInstance(tb.CreateType());
        }

        /// <summary>
        /// Casts the specified il.
        /// </summary>
        /// <param name="il">The il.</param>
        /// <param name="type">The type.</param>
        /// <param name="addr">The addr.</param>
        private static void Cast(ILGenerator il, Type type, LocalBuilder addr)
        {
            if (type == typeof(object)) { }
            else if (type.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, type);
                if (addr != null)
                {
                    il.Emit(OpCodes.Stloc, addr);
                    il.Emit(OpCodes.Ldloca_S, addr);
                }
            }
            else
            {
                il.Emit(OpCodes.Castclass, type);
            }
        }

        /// <summary>
        /// Get or set the value of a named member on the target instance
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="name">The name.</param>
        /// <returns>object.</returns>
        public abstract object this[object target, string name]
        {
            get;
            set;
        }
    }

}
#endif
