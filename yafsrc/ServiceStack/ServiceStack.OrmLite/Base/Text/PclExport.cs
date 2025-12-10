// ***********************************************************************
// <copyright file="PclExport.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using ServiceStack.OrmLite.Base.Text.Common;

namespace ServiceStack.OrmLite.Base.Text;

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
        public const string NetStandard = "NETStd";

        /// <summary>
        /// The net core
        /// </summary>
        public const string Net10 = "NET10";
    }

    /// <summary>
    /// The instance
    /// </summary>
    public static PclExport Instance { get; private set; }
        = new NetPclExport();

    /// <summary>
    /// Gets the reflection.
    /// </summary>
    /// <value>The reflection.</value>
    public static ReflectionOptimizer Reflection => ReflectionOptimizer.Instance;

    /// <summary>
    /// Initializes static members of the <see cref="PclExport" /> class.
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
        {
            return false;
        }

        var mi = type.GetMethod("Configure");
        if (mi != null)
        {
            _ = mi.Invoke(null, []);
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
    public Task EmptyTask { get; set; }

    /// <summary>
    /// The dir sep
    /// </summary>
    public char DirSep { get; set; } = '\\';

    /// <summary>
    /// The alt dir sep
    /// </summary>
    public char AltDirSep { get; } = '/';

    /// <summary>
    /// The dir seps
    /// </summary>
    public readonly static char[] DirSeps = ['\\', '/'];

    /// <summary>
    /// The platform name
    /// </summary>
    public string PlatformName { get; set; } = "Unknown";

    /// <summary>
    /// The regex options
    /// </summary>
    public RegexOptions RegexOptions { get; } = RegexOptions.None;

    /// <summary>
    /// The invariant comparison
    /// </summary>
    public StringComparison InvariantComparison { get; } = StringComparison.Ordinal;

    /// <summary>
    /// The invariant comparison ignore case
    /// </summary>
    public StringComparison InvariantComparisonIgnoreCase { get; } = StringComparison.OrdinalIgnoreCase;

    /// <summary>
    /// The invariant comparer ignore case
    /// </summary>
    public StringComparer InvariantComparerIgnoreCase { get; } = StringComparer.OrdinalIgnoreCase;

    // HACK: The only way to detect anonymous types right now.

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
    /// Creates the directory.
    /// </summary>
    /// <param name="dirPath">The dir path.</param>
    public virtual void CreateDirectory(string dirPath)
    {
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
    /// Gets all assemblies.
    /// </summary>
    /// <returns>Assembly[].</returns>
    public virtual Assembly[] GetAllAssemblies()
    {
        return [];
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
        return
            Array.Find(type.GetInterfaces(), t => t.IsGenericType
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
    /// Gets the dictionary parse string span method.
    /// </summary>
    /// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
    /// <param name="type">The type.</param>
    /// <returns>ParseStringSpanDelegate.</returns>
    public virtual ParseStringSpanDelegate GetDictionaryParseStringSpanMethod<TSerializer>(Type type)
        where TSerializer : ITypeSerializer
    {
        return null;
    }

    /// <summary>
    /// Gets the specialized collection parse string span method.
    /// </summary>
    /// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
    /// <param name="type">The type.</param>
    /// <returns>ParseStringSpanDelegate.</returns>
    public virtual ParseStringSpanDelegate GetSpecializedCollectionParseStringSpanMethod<TSerializer>(Type type)
        where TSerializer : ITypeSerializer
    {
        return null;
    }

    /// <summary>
    /// Gets the js reader parse string span method.
    /// </summary>
    /// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
    /// <param name="type">The type.</param>
    /// <returns>ParseStringSpanDelegate.</returns>
    public virtual ParseStringSpanDelegate GetJsReaderParseStringSpanMethod<TSerializer>(Type type)
        where TSerializer : ITypeSerializer
    {
        return null;
    }


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
    public virtual DataContractAttribute GetWeakDataContract(Type type)
    {
        return null;
    }

    /// <summary>
    /// Gets the weak data member.
    /// </summary>
    /// <param name="pi">The pi.</param>
    /// <returns>DataMemberAttribute.</returns>
    public virtual DataMemberAttribute GetWeakDataMember(PropertyInfo pi)
    {
        return null;
    }

    /// <summary>
    /// Gets the weak data member.
    /// </summary>
    /// <param name="pi">The pi.</param>
    /// <returns>DataMemberAttribute.</returns>
    public virtual DataMemberAttribute GetWeakDataMember(FieldInfo pi)
    {
        return null;
    }

    /// <summary>
    /// Registers for aot.
    /// </summary>
    public virtual void RegisterForAot() { }
}