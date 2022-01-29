// ***********************************************************************
// <copyright file="PclExport.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Text
{
    using ServiceStack.Text.Common;

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>
    /// Class PclExport.
    /// </summary>
    public abstract class PclExport
    {
        /// <summary>
        /// Class Platforms.
        /// </summary>
        public static class Platforms
        {
            /// <summary>
            /// The net standard
            /// </summary>
            public const string NetStandard = "NETStandard";
            /// <summary>
            /// The net core
            /// </summary>
            public const string NetCore = "NetCore";
            /// <summary>
            /// The net48
            /// </summary>
            public const string Net48 = "Net48";
        }

        /// <summary>
        /// The instance
        /// </summary>
        public static PclExport Instance
#if NETFX
          = new Net48PclExport()
#elif NETSTANDARD2_0
          = new NetStandardPclExport()
#elif NETCORE || NET6_0_OR_GREATER
          = new NetCorePclExport()
#endif
        ;

        /// <summary>
        /// Gets the reflection.
        /// </summary>
        /// <value>The reflection.</value>
        public static ReflectionOptimizer Reflection => ReflectionOptimizer.Instance;

        /// <summary>
        /// Initializes static members of the <see cref="PclExport"/> class.
        /// </summary>
        static PclExport() { }

        /// <summary>
        /// Configures the provider.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool ConfigureProvider(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type == null)
                return false;

            var mi = type.GetMethod("Configure");
            if (mi != null)
            {
                mi.Invoke(null, Array.Empty<object>());
            }

            return true;
        }

        /// <summary>
        /// Configures the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public static void Configure(PclExport instance)
        {
            Instance = instance ?? Instance;

            if (Instance is { EmptyTask: null })
            {
                var tcs = new TaskCompletionSource<object>();
                tcs.SetResult(null);
                Instance.EmptyTask = tcs.Task;
            }
        }

        /// <summary>
        /// The empty task
        /// </summary>
        public Task EmptyTask;

        /// <summary>
        /// The dir sep
        /// </summary>
        public char DirSep = '\\';

        /// <summary>
        /// The alt dir sep
        /// </summary>
        public char AltDirSep = '/';

        /// <summary>
        /// The dir seps
        /// </summary>
        public static readonly char[] DirSeps = { '\\', '/' };

        /// <summary>
        /// The platform name
        /// </summary>
        public string PlatformName = "Unknown";

        /// <summary>
        /// The regex options
        /// </summary>
        public RegexOptions RegexOptions = RegexOptions.None;

        /// <summary>
        /// The invariant comparison
        /// </summary>
        public StringComparison InvariantComparison = StringComparison.Ordinal;

        /// <summary>
        /// The invariant comparison ignore case
        /// </summary>
        public StringComparison InvariantComparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;

        /// <summary>
        /// The invariant comparer
        /// </summary>
        public StringComparer InvariantComparer = StringComparer.Ordinal;

        /// <summary>
        /// The invariant comparer ignore case
        /// </summary>
        public StringComparer InvariantComparerIgnoreCase = StringComparer.OrdinalIgnoreCase;

        /// <summary>
        /// Reads all text.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>System.String.</returns>
        public abstract string ReadAllText(string filePath);

        // HACK: The only way to detect anonymous types right now.
        /// <summary>
        /// Determines whether [is anonymous type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is anonymous type] [the specified type]; otherwise, <c>false</c>.</returns>
        public virtual bool IsAnonymousType(Type type)
        {
            return type.IsGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>", StringComparison.Ordinal) || type.Name.StartsWith("VB$", StringComparison.Ordinal));
        }

        /// <summary>
        /// Converts to invariantupper.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public virtual string ToInvariantUpper(char value)
        {
            return value.ToString().ToUpperInvariant();
        }

        /// <summary>
        /// Files the exists.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool FileExists(string filePath)
        {
            return false;
        }

        /// <summary>
        /// Directories the exists.
        /// </summary>
        /// <param name="dirPath">The dir path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool DirectoryExists(string dirPath)
        {
            return false;
        }

        /// <summary>
        /// Creates the directory.
        /// </summary>
        /// <param name="dirPath">The dir path.</param>
        public virtual void CreateDirectory(string dirPath)
        {
        }

        /// <summary>
        /// Gets the environment variable.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public virtual string GetEnvironmentVariable(string name)
        {
            return null;
        }

        /// <summary>
        /// Gets the file names.
        /// </summary>
        /// <param name="dirPath">The dir path.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <returns>System.String[].</returns>
        public virtual string[] GetFileNames(string dirPath, string searchPattern = null)
        {
            return TypeConstants.EmptyStringArray;
        }

        /// <summary>
        /// Gets the directory names.
        /// </summary>
        /// <param name="dirPath">The dir path.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <returns>System.String[].</returns>
        public virtual string[] GetDirectoryNames(string dirPath, string searchPattern = null)
        {
            return TypeConstants.EmptyStringArray;
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="line">The line.</param>
        public virtual void WriteLine(string line)
        {
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="args">The arguments.</param>
        public virtual void WriteLine(string line, params object[] args)
        {
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
        public virtual void Config(HttpWebRequest req,
            bool? allowAutoRedirect = null,
            TimeSpan? timeout = null,
            TimeSpan? readWriteTimeout = null,
            string userAgent = null,
            bool? preAuthenticate = null)
        {
        }

        /// <summary>
        /// Adds the compression.
        /// </summary>
        /// <param name="webRequest">The web request.</param>
        public virtual void AddCompression(WebRequest webRequest)
        {
        }

        /// <summary>
        /// Gets the request stream.
        /// </summary>
        /// <param name="webRequest">The web request.</param>
        /// <returns>Stream.</returns>
        public virtual Stream GetRequestStream(WebRequest webRequest)
        {
            var async = webRequest.GetRequestStreamAsync();
            async.Wait();
            return async.Result;
        }

        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <param name="webRequest">The web request.</param>
        /// <returns>WebResponse.</returns>
        public virtual WebResponse GetResponse(WebRequest webRequest)
        {
            try
            {
                var async = webRequest.GetResponseAsync();
                async.Wait();
                return async.Result;
            }
            catch (Exception ex)
            {
                throw ex.UnwrapIfSingleException();
            }
        }

        /// <summary>
        /// Gets the response asynchronous.
        /// </summary>
        /// <param name="webRequest">The web request.</param>
        /// <returns>Task&lt;WebResponse&gt;.</returns>
        public virtual Task<WebResponse> GetResponseAsync(WebRequest webRequest)
        {
            return webRequest.GetResponseAsync();
        }

        /// <summary>
        /// Determines whether [is debug build] [the specified assembly].
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns><c>true</c> if [is debug build] [the specified assembly]; otherwise, <c>false</c>.</returns>
        public virtual bool IsDebugBuild(Assembly assembly)
        {
            return assembly.AllAttributes()
                .Any(x => x.GetType().Name == "DebuggableAttribute");
        }

        /// <summary>
        /// Maps the absolute path.
        /// </summary>
        /// <param name="relativePath">The relative path.</param>
        /// <param name="appendPartialPathModifier">The append partial path modifier.</param>
        /// <returns>System.String.</returns>
        public virtual string MapAbsolutePath(string relativePath, string appendPartialPathModifier)
        {
            return relativePath;
        }

        /// <summary>
        /// Loads the assembly.
        /// </summary>
        /// <param name="assemblyPath">The assembly path.</param>
        /// <returns>Assembly.</returns>
        public virtual Assembly LoadAssembly(string assemblyPath)
        {
            return null;
        }

        /// <summary>
        /// Adds the header.
        /// </summary>
        /// <param name="webReq">The web req.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public virtual void AddHeader(WebRequest webReq, string name, string value)
        {
            webReq.Headers[name] = value;
        }

        /// <summary>
        /// Sets the user agent.
        /// </summary>
        /// <param name="httpReq">The HTTP req.</param>
        /// <param name="value">The value.</param>
        public virtual void SetUserAgent(HttpWebRequest httpReq, string value)
        {
            httpReq.Headers[HttpRequestHeader.UserAgent] = value;
        }

        /// <summary>
        /// Sets the length of the content.
        /// </summary>
        /// <param name="httpReq">The HTTP req.</param>
        /// <param name="value">The value.</param>
        public virtual void SetContentLength(HttpWebRequest httpReq, long value)
        {
            httpReq.Headers[HttpRequestHeader.ContentLength] = value.ToString();
        }

        /// <summary>
        /// Sets the allow automatic redirect.
        /// </summary>
        /// <param name="httpReq">The HTTP req.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public virtual void SetAllowAutoRedirect(HttpWebRequest httpReq, bool value)
        {
        }

        /// <summary>
        /// Gets all assemblies.
        /// </summary>
        /// <returns>Assembly[].</returns>
        public virtual Assembly[] GetAllAssemblies()
        {
            return Array.Empty<Assembly>();
        }

        /// <summary>
        /// Finds the type.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>Type.</returns>
        public virtual Type FindType(string typeName, string assemblyName)
        {
            return null;
        }

        /// <summary>
        /// Gets the assembly code base.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>System.String.</returns>
        public virtual string GetAssemblyCodeBase(Assembly assembly)
        {
            return assembly.FullName;
        }

        /// <summary>
        /// Gets the assembly path.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>System.String.</returns>
        public virtual string GetAssemblyPath(Type source)
        {
            return null;
        }

        /// <summary>
        /// Gets the ASCII string.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>System.String.</returns>
        public virtual string GetAsciiString(byte[] bytes)
        {
            return GetAsciiString(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Gets the ASCII string.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <returns>System.String.</returns>
        public virtual string GetAsciiString(byte[] bytes, int index, int count)
        {
            return Encoding.UTF8.GetString(bytes, index, count);
        }

        /// <summary>
        /// Gets the ASCII bytes.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>System.Byte[].</returns>
        public virtual byte[] GetAsciiBytes(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        /// <summary>
        /// Gets the ut f8 encoding.
        /// </summary>
        /// <param name="emitBom">if set to <c>true</c> [emit bom].</param>
        /// <returns>Encoding.</returns>
        public virtual Encoding GetUTF8Encoding(bool emitBom = false)
        {
            return new UTF8Encoding(emitBom);
        }
        
        /// <summary>
        /// Ins the same assembly.
        /// </summary>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool InSameAssembly(Type t1, Type t2)
        {
            return t1.AssemblyQualifiedName != null && t1.AssemblyQualifiedName.Equals(t2.AssemblyQualifiedName);
        }

        /// <summary>
        /// Gets the type of the generic collection.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Type.</returns>
        public virtual Type GetGenericCollectionType(Type type)
        {
            return type.GetInterfaces()
                .FirstOrDefault(t => t.IsGenericType
                && t.GetGenericTypeDefinition() == typeof(ICollection<>));
        }

        /// <summary>
        /// Converts to xsddatetimestring.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>System.String.</returns>
        public virtual string ToXsdDateTimeString(DateTime dateTime)
        {
            return System.Xml.XmlConvert.ToString(dateTime.ToStableUniversalTime(), DateTimeSerializer.XsdDateTimeFormat);
        }

        /// <summary>
        /// Converts to localxsddatetimestring.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>System.String.</returns>
        public virtual string ToLocalXsdDateTimeString(DateTime dateTime)
        {
            return System.Xml.XmlConvert.ToString(dateTime, DateTimeSerializer.XsdDateTimeFormat);
        }

        /// <summary>
        /// Parses the XSD date time.
        /// </summary>
        /// <param name="dateTimeStr">The date time string.</param>
        /// <returns>DateTime.</returns>
        public virtual DateTime ParseXsdDateTime(string dateTimeStr)
        {
            return System.Xml.XmlConvert.ToDateTimeOffset(dateTimeStr).DateTime;
        }

        /// <summary>
        /// Parses the XSD date time as UTC.
        /// </summary>
        /// <param name="dateTimeStr">The date time string.</param>
        /// <returns>DateTime.</returns>
        public virtual DateTime ParseXsdDateTimeAsUtc(string dateTimeStr)
        {
            return DateTimeSerializer.ParseManual(dateTimeStr, DateTimeKind.Utc)
                ?? DateTime.ParseExact(dateTimeStr, DateTimeSerializer.XsdDateTimeFormat, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts to stableuniversaltime.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>DateTime.</returns>
        public virtual DateTime ToStableUniversalTime(DateTime dateTime)
        {
            // Silverlight 3, 4 and 5 all work ok with DateTime.ToUniversalTime, but have no TimeZoneInfo.ConverTimeToUtc implementation.
            return dateTime.ToUniversalTime();
        }

        /// <summary>
        /// Gets the dictionary parse method.
        /// </summary>
        /// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns>ParseStringDelegate.</returns>
        public virtual ParseStringDelegate GetDictionaryParseMethod<TSerializer>(Type type)
            where TSerializer : ITypeSerializer => null;

        /// <summary>
        /// Gets the dictionary parse string span method.
        /// </summary>
        /// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns>ParseStringSpanDelegate.</returns>
        public virtual ParseStringSpanDelegate GetDictionaryParseStringSpanMethod<TSerializer>(Type type)
            where TSerializer : ITypeSerializer => null;

        /// <summary>
        /// Gets the specialized collection parse method.
        /// </summary>
        /// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns>ParseStringDelegate.</returns>
        public virtual ParseStringDelegate GetSpecializedCollectionParseMethod<TSerializer>(Type type)
            where TSerializer : ITypeSerializer => null;

        /// <summary>
        /// Gets the specialized collection parse string span method.
        /// </summary>
        /// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns>ParseStringSpanDelegate.</returns>
        public virtual ParseStringSpanDelegate GetSpecializedCollectionParseStringSpanMethod<TSerializer>(Type type)
            where TSerializer : ITypeSerializer => null;

        /// <summary>
        /// Gets the js reader parse string span method.
        /// </summary>
        /// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns>ParseStringSpanDelegate.</returns>
        public virtual ParseStringSpanDelegate GetJsReaderParseStringSpanMethod<TSerializer>(Type type)
            where TSerializer : ITypeSerializer => null;


        /// <summary>
        /// Initializes the HTTP web request.
        /// </summary>
        /// <param name="httpReq">The HTTP req.</param>
        /// <param name="contentLength">Length of the content.</param>
        /// <param name="allowAutoRedirect">if set to <c>true</c> [allow automatic redirect].</param>
        /// <param name="keepAlive">if set to <c>true</c> [keep alive].</param>
        public virtual void InitHttpWebRequest(HttpWebRequest httpReq,
            long? contentLength = null, bool allowAutoRedirect = true, bool keepAlive = true)
        { }

        /// <summary>
        /// Closes the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public virtual void CloseStream(Stream stream)
        {
            stream.Flush();
        }

        /// <summary>
        /// Resets the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public virtual void ResetStream(Stream stream)
        {
            stream.Position = 0;
        }

        /// <summary>
        /// Verifies the license key text.
        /// </summary>
        /// <param name="licenseKeyText">The license key text.</param>
        /// <returns>LicenseKey.</returns>
        public virtual LicenseKey VerifyLicenseKeyText(string licenseKeyText)
        {
            return licenseKeyText.ToLicenseKey();
        }

        /// <summary>
        /// Verifies the license key text fallback.
        /// </summary>
        /// <param name="licenseKeyText">The license key text.</param>
        /// <returns>LicenseKey.</returns>
        public virtual LicenseKey VerifyLicenseKeyTextFallback(string licenseKeyText)
        {
            return licenseKeyText.ToLicenseKeyFallback();
        }

        /// <summary>
        /// Begins the thread affinity.
        /// </summary>
        public virtual void BeginThreadAffinity() { }
        /// <summary>
        /// Ends the thread affinity.
        /// </summary>
        public virtual void EndThreadAffinity() { }

        /// <summary>
        /// Gets the weak data contract.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>DataContractAttribute.</returns>
        public virtual DataContractAttribute GetWeakDataContract(Type type) => null;
        /// <summary>
        /// Gets the weak data member.
        /// </summary>
        /// <param name="pi">The pi.</param>
        /// <returns>DataMemberAttribute.</returns>
        public virtual DataMemberAttribute GetWeakDataMember(PropertyInfo pi) => null;
        /// <summary>
        /// Gets the weak data member.
        /// </summary>
        /// <param name="pi">The pi.</param>
        /// <returns>DataMemberAttribute.</returns>
        public virtual DataMemberAttribute GetWeakDataMember(FieldInfo pi) => null;

        /// <summary>
        /// Registers for aot.
        /// </summary>
        public virtual void RegisterForAot() { }
        /// <summary>
        /// Gets the stack trace.
        /// </summary>
        /// <returns>System.String.</returns>
        public virtual string GetStackTrace() => null;
    }

}