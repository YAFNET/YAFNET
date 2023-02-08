// ***********************************************************************
// <copyright file="JsConfig.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using ServiceStack.Text.Common;
using ServiceStack.Text.Json;
using ServiceStack.Text.Jsv;

namespace ServiceStack.Text;

/// <summary>
/// 
/// </summary>
public static class JsConfig
{
    static JsConfig()
    {
        Reset();
    }

    /// <summary>
    /// force deterministic initialization of static constructor
    /// </summary>
    public static void InitStatics() { }

    /// <summary>
    /// Mark JsConfig global config as initialized and assert it's no longer mutated
    /// </summary>
    public static void Init() => Config.Init();

    /// <summary>
    /// Initialize global config and assert that it's no longer mutated
    /// </summary>
    /// <param name="config"></param>
    public static void Init(Config config) => Config.Init(config);

    /// <summary>
    /// 
    /// </summary>
    public static bool HasInit => Config.HasInit;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static JsConfigScope BeginScope()
    {
        return new JsConfigScope(); // Populated with Config.Instance
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="config"></param>
    /// <param name="scope"></param>
    /// <returns></returns>
    public static JsConfigScope CreateScope(string config, JsConfigScope scope = null)
    {
        if (string.IsNullOrEmpty(config))
            return scope;

        scope ??= BeginScope();

        var items = config.Split(',');
        foreach (var item in items)
        {
            var parts = item.SplitOnFirst(':');
            var key = parts[0].ToLower();
            var value = parts.Length == 2 ? parts[1].ToLower() : null;
            var boolValue = parts.Length == 1 || value != "false" && value != "0";

            switch (key)
            {
                case "cotisd":
                case "convertobjecttypesintostringdictionary":
                    scope.ConvertObjectTypesIntoStringDictionary = boolValue;
                    break;
                case "ttpptv":
                case "trytoparseprimitivetypevalues":
                    scope.TryToParsePrimitiveTypeValues = boolValue;
                    break;
                case "ttpnt":
                case "trytoparsenumerictype":
                    scope.TryToParseNumericType = boolValue;
                    break;
                case "edv":
                case "excludedefaultvalues":
                    scope.ExcludeDefaultValues = boolValue;
                    break;
                case "inv":
                case "includenullvalues":
                    scope.IncludeNullValues = boolValue;
                    break;
                case "invid":
                case "includenullvaluesindictionaries":
                    scope.IncludeNullValuesInDictionaries = boolValue;
                    break;
                case "ide":
                case "includedefaultenums":
                    scope.IncludeDefaultEnums = boolValue;
                    break;
                case "eti":
                case "excludetypeinfo":
                    scope.ExcludeTypeInfo = boolValue;
                    break;
                case "iti":
                case "includetypeinfo":
                    scope.IncludeTypeInfo = boolValue;
                    break;
                case "i":
                case "pp": //pretty-print
                case "indent":
                    scope.Indent = boolValue;
                    break;
                case "eccn":
                case "emitcamelcasenames":
                    scope.TextCase = boolValue ? TextCase.CamelCase : scope.TextCase;
                    break;
                case "elun":
                case "emitlowercaseunderscorenames":
                    scope.TextCase = boolValue ? TextCase.SnakeCase : scope.TextCase;
                    break;
                case "pi":
                case "preferinterfaces":
                    scope.PreferInterfaces = boolValue;
                    break;
                case "tode":
                case "throwondeserializationerror":
                case "toe":
                case "throwonerror":
                    scope.ThrowOnError = boolValue;
                    break;
                case "teai":
                case "treatenumasinteger":
                    scope.TreatEnumAsInteger = boolValue;
                    break;
                case "sdtc":
                case "skipdatetimeconversion":
                    scope.SkipDateTimeConversion = boolValue;
                    break;
                case "auu":
                case "alwaysuseutc":
                    scope.AlwaysUseUtc = boolValue;
                    break;
                case "au":
                case "assumeutc":
                    scope.AssumeUtc = boolValue;
                    break;
                case "auo":
                case "appendutcoffset":
                    scope.AppendUtcOffset = boolValue;
                    break;
                case "ipf":
                case "includepublicfields":
                    scope.IncludePublicFields = boolValue;
                    break;
                case "dh":
                case "datehandler":
                    switch (value)
                    {
                        case "timestampoffset":
                        case "to":
                            scope.DateHandler = DateHandler.TimestampOffset;
                            break;
                        case "dcjsc":
                        case "dcjscompatible":
                            scope.DateHandler = DateHandler.DCJSCompatible;
                            break;
                        case "iso8601":
                            scope.DateHandler = DateHandler.ISO8601;
                            break;
                        case "iso8601do":
                        case "iso8601dateonly":
                            scope.DateHandler = DateHandler.ISO8601DateOnly;
                            break;
                        case "iso8601dt":
                        case "iso8601datetime":
                            scope.DateHandler = DateHandler.ISO8601DateTime;
                            break;
                        case "rfc1123":
                            scope.DateHandler = DateHandler.RFC1123;
                            break;
                        case "ut":
                        case "unixtime":
                            scope.DateHandler = DateHandler.UnixTime;
                            break;
                        case "utm":
                        case "unixtimems":
                            scope.DateHandler = DateHandler.UnixTimeMs;
                            break;
                    }
                    break;
                case "tsh":
                case "timespanhandler":
                    switch (value)
                    {
                        case "df":
                        case "durationformat":
                            scope.TimeSpanHandler = TimeSpanHandler.DurationFormat;
                            break;
                        case "sf":
                        case "standardformat":
                            scope.TimeSpanHandler = TimeSpanHandler.StandardFormat;
                            break;
                    }
                    break;
                case "pc":
                case "propertyconvention":
                    switch (value)
                    {
                        case "l":
                        case "lenient":
                            scope.PropertyConvention = PropertyConvention.Lenient;
                            break;
                        case "s":
                        case "strict":
                            scope.PropertyConvention = PropertyConvention.Strict;
                            break;
                    }
                    break;
                case "tc":
                case "textcase":
                    switch (value)
                    {
                        case "d":
                        case "default":
                            scope.TextCase = TextCase.Default;
                            break;
                        case "pc":
                        case "pascalcase":
                            scope.TextCase = TextCase.PascalCase;
                            break;
                        case "cc":
                        case "camelcase":
                            scope.TextCase = TextCase.CamelCase;
                            break;
                        case "sc":
                        case "snakecase":
                            scope.TextCase = TextCase.SnakeCase;
                            break;
                    }
                    break;
            }
        }

        return scope;
    }

    /// <summary>
    /// 
    /// </summary>
    public static UTF8Encoding UTF8Encoding { get; set; } = new(false);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public static JsConfigScope With(Config config) => (JsConfigScope)new JsConfigScope().Populate(config);

    /// <summary>
    /// 
    /// </summary>
    public static bool TryToParsePrimitiveTypeValues
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.TryToParsePrimitiveTypeValues : Config.Instance.TryToParsePrimitiveTypeValues;
        set => Config.AssertNotInit().TryToParsePrimitiveTypeValues = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public static bool TryParseIntoBestFit
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.TryParseIntoBestFit : Config.Instance.TryParseIntoBestFit;
        set => Config.AssertNotInit().TryParseIntoBestFit = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public static bool ExcludeDefaultValues
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.ExcludeDefaultValues : Config.Instance.ExcludeDefaultValues;
        set => Config.AssertNotInit().ExcludeDefaultValues = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public static bool IncludeNullValues
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.IncludeNullValues : Config.Instance.IncludeNullValues;
        set => Config.AssertNotInit().IncludeNullValues = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public static bool IncludeNullValuesInDictionaries
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.IncludeNullValuesInDictionaries : Config.Instance.IncludeNullValuesInDictionaries;
        set => Config.AssertNotInit().IncludeNullValuesInDictionaries = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public static bool TreatEnumAsInteger
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.TreatEnumAsInteger : Config.Instance.TreatEnumAsInteger;
        set => Config.AssertNotInit().TreatEnumAsInteger = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public static bool ExcludeTypeInfo
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.ExcludeTypeInfo : Config.Instance.ExcludeTypeInfo;
        set => Config.AssertNotInit().ExcludeTypeInfo = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public static bool IncludeTypeInfo
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.IncludeTypeInfo : Config.Instance.IncludeTypeInfo;
        set => Config.AssertNotInit().IncludeTypeInfo = value;
    }

    public static bool Indent
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.Indent : Config.Instance.Indent;
        set => Config.AssertNotInit().Indent = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public static string TypeAttr
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.TypeAttr : Config.Instance.TypeAttr;
        set
        {
            var config = Config.AssertNotInit();
            config.TypeAttr = value;
        }
    }

    internal static string JsonTypeAttrInObject => JsConfigScope.Current != null ? JsConfigScope.Current.JsonTypeAttrInObject : Config.Instance.JsonTypeAttrInObject;

    internal static string JsvTypeAttrInObject => JsConfigScope.Current != null ? JsConfigScope.Current.JsvTypeAttrInObject : Config.Instance.JsvTypeAttrInObject;

    /// <summary>
    /// 
    /// </summary>
    public static Func<Type, string> TypeWriter
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.TypeWriter : Config.Instance.TypeWriter;
        set => Config.AssertNotInit().TypeWriter = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public static Func<string, Type> TypeFinder
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.TypeFinder : Config.Instance.TypeFinder;
        set => Config.AssertNotInit().TypeFinder = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public static Func<string, object> ParsePrimitiveFn
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.ParsePrimitiveFn : Config.Instance.ParsePrimitiveFn;
        set => Config.AssertNotInit().ParsePrimitiveFn = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public static DateHandler DateHandler
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.DateHandler : Config.Instance.DateHandler;
        set => Config.AssertNotInit().DateHandler = value;
    }

    /// <summary>
    /// Sets which format to use when serializing TimeSpans
    /// </summary>
    public static TimeSpanHandler TimeSpanHandler
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.TimeSpanHandler : Config.Instance.TimeSpanHandler;
        set => Config.AssertNotInit().TimeSpanHandler = value;
    }

    /// <summary>
    /// Text case to use for property names (Default = PascalCase)
    /// </summary>
    public static TextCase TextCase
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.TextCase : Config.Instance.TextCase;
        set => Config.AssertNotInit().TextCase = value;
    }

    /// <summary>
    /// Avoid multiple static property checks by getting snapshot of active config
    /// </summary>
    /// <returns></returns>
    public static Config GetConfig() => JsConfigScope.Current ?? Config.Instance;

    /// <summary>
    /// Gets or sets a value indicating if the framework should throw serialization exceptions
    /// or continue regardless of serialization errors. If <see langword="true"/>  the framework
    /// will throw; otherwise, it will parse as many fields as possible. The default is <see langword="false"/>.
    /// </summary>
    public static bool ThrowOnError
    {
        // obeying the use of ThreadStatic, but allowing for setting JsConfig once as is the normal case
        get => JsConfigScope.Current != null ? JsConfigScope.Current.ThrowOnError : Config.Instance.ThrowOnError;
        set => Config.AssertNotInit().ThrowOnError = value;
    }

    /// <summary>
    /// Gets or sets a value indicating if the framework should skip automatic <see cref="DateTime"/> conversions.
    /// Dates will be handled literally, any included timezone encoding will be lost and the date will be treaded as DateTimeKind.Local
    /// Utc formatted input will result in DateTimeKind.Utc output. Any input without TZ data will be set DateTimeKind.Unspecified
    /// This will take precedence over other flags like AlwaysUseUtc
    /// JsConfig.DateHandler = DateHandler.ISO8601 should be used when set true for consistent de/serialization.
    /// </summary>
    public static bool SkipDateTimeConversion
    {
        // obeying the use of ThreadStatic, but allowing for setting JsConfig once as is the normal case
        get => JsConfigScope.Current != null ? JsConfigScope.Current.SkipDateTimeConversion : Config.Instance.SkipDateTimeConversion;
        set => Config.AssertNotInit().SkipDateTimeConversion = value;
    }

    /// <summary>
    /// Gets or sets a value indicating if the framework should always assume <see cref="DateTime"/> is in UTC format if Kind is Unspecified.
    /// </summary>
    public static bool AssumeUtc
    {
        // obeying the use of ThreadStatic, but allowing for setting JsConfig once as is the normal case
        get => JsConfigScope.Current != null ? JsConfigScope.Current.AssumeUtc : Config.Instance.AssumeUtc;
        set => Config.AssertNotInit().AssumeUtc = value;
    }

    internal static HashSet<Type> HasSerializeFn = new();

    internal static HashSet<Type> HasIncludeDefaultValue = new();

    /// <summary>
    /// 
    /// </summary>
    public static HashSet<Type> TreatValueAsRefTypes = new();

    /// <summary>
    /// If set to true, Interface types will be preferred over concrete types when serializing.
    /// </summary>
    public static bool PreferInterfaces
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.PreferInterfaces : Config.Instance.PreferInterfaces;
        set => Config.AssertNotInit().PreferInterfaces = value;
    }

    internal static bool TreatAsRefType(Type valueType)
    {
        return TreatValueAsRefTypes.Contains(valueType.IsGenericType ? valueType.GetGenericTypeDefinition() : valueType);
    }

    /// <summary>
    /// If set to true, Interface types will be preferred over concrete types when serializing.
    /// </summary>
    public static bool IncludePublicFields
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.IncludePublicFields : Config.Instance.IncludePublicFields;
        set => Config.AssertNotInit().IncludePublicFields = value;
    }

    /// <summary>
    /// Sets the maximum depth to avoid circular dependencies
    /// </summary>
    public static int MaxDepth
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.MaxDepth : Config.Instance.MaxDepth;
        set => Config.AssertNotInit().MaxDepth = value;
    }

    /// <summary>
    /// Set this to enable your own type construction provider.
    /// This is helpful for integration with IoC containers where you need to call the container constructor.
    /// Return null if you don't know how to construct the type and the parameterless constructor will be used.
    /// </summary>
    public static EmptyCtorFactoryDelegate ModelFactory
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.ModelFactory : Config.Instance.ModelFactory;
        set => Config.AssertNotInit().ModelFactory = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public static HashSet<Type> ExcludeTypes
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.ExcludeTypes : Config.Instance.ExcludeTypes;
        set => Config.AssertNotInit().ExcludeTypes = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public static HashSet<string> ExcludeTypeNames
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.ExcludeTypeNames : Config.Instance.ExcludeTypeNames;
        set => Config.AssertNotInit().ExcludeTypeNames = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public static HashSet<string> AllowRuntimeTypeWithAttributesNamed { get; set; } = new();

    /// <summary>
    /// 
    /// </summary>
    public static HashSet<string> AllowRuntimeTypeWithInterfacesNamed { get; set; } = new();

    /// <summary>
    /// 
    /// </summary>
    public static HashSet<string> AllowRuntimeTypeInTypes { get; set; } = new();

    /// <summary>
    /// 
    /// </summary>
    public static HashSet<string> AllowRuntimeTypeInTypesWithNamespaces { get; set; } = new();

    /// <summary>
    /// 
    /// </summary>
    public static Func<Type, bool> AllowRuntimeType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [allow runtime interfaces].
    /// </summary>
    /// <value><c>true</c> if [allow runtime interfaces]; otherwise, <c>false</c>.</value>
    public static bool AllowRuntimeInterfaces { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public static void Reset()
    {
        foreach (var rawSerializeType in HasSerializeFn.ToArray())
        {
            Reset(rawSerializeType);
        }

        foreach (var rawSerializeType in HasIncludeDefaultValue.ToArray())
        {
            Reset(rawSerializeType);
        }

        foreach (var uniqueType in __uniqueTypes.ToArray())
        {
            Reset(uniqueType);
        }

        Env.StrictMode = false;
        Config.Reset();
        AutoMappingUtils.Reset();
        HasSerializeFn = new HashSet<Type>();
        HasIncludeDefaultValue = new HashSet<Type>();
        TreatValueAsRefTypes = new HashSet<Type> { typeof(KeyValuePair<,>) };
        __uniqueTypes = new HashSet<Type>();

        // Called when writing each string, too expensive to maintain as scoped config
        AllowRuntimeInterfaces = true;
        AllowRuntimeType = null;
        AllowRuntimeTypeWithAttributesNamed = new HashSet<string>
                                                  {
                                                      nameof(SerializableAttribute),
                                                      nameof(DataContractAttribute),
                                                      nameof(RuntimeSerializableAttribute),
                                                  };
        AllowRuntimeTypeWithInterfacesNamed = new HashSet<string>
                                                  {
                                                      "IConvertible",
                                                      "ISerializable",
                                                      "IRuntimeSerializable",
                                                      "IMeta",
                                                      "IReturn`1",
                                                      "IReturnVoid",
                                                      "IVerb",
                                                      "ICrud",
                                                      "IMeta",
                                                      "IAuthTokens",
                                                      "IHasResponseStatus",
                                                      "IHasId`1"
                                                  };
        AllowRuntimeTypeInTypesWithNamespaces = new HashSet<string>
                                                    {
                                                        "ServiceStack.Auth",
                                                        "ServiceStack.Messaging"
                                                    };
        AllowRuntimeTypeInTypes = new();
        PlatformExtensions.ClearRuntimeAttributes();
        ReflectionExtensions.Reset();
        JsState.Reset();
    }

    private static void Reset(Type cachesForType)
    {
        typeof(JsConfig<>).MakeGenericType(cachesForType).InvokeReset();
        typeof(TypeConfig<>).MakeGenericType(cachesForType).InvokeReset();
    }

    internal static void InvokeReset(this Type genericType)
    {
        var methodInfo = genericType.GetStaticMethod("Reset");
        methodInfo.Invoke(null, null);
    }

    internal static HashSet<Type> __uniqueTypes = new();
    internal static int __uniqueTypesCount;

    internal static void AddUniqueType(Type type)
    {
        if (__uniqueTypes.Contains(type))
            return;

        HashSet<Type> newTypes, snapshot;
        do
        {
            snapshot = __uniqueTypes;
            newTypes = new HashSet<Type>(__uniqueTypes) { type };
            __uniqueTypesCount = newTypes.Count;

        }

        while (!ReferenceEquals(
                   Interlocked.CompareExchange(ref __uniqueTypes, newTypes, snapshot), snapshot));
    }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class JsConfig<T>
{
    static JsConfig()
    {
        // Run the type's static constructor (which may set OnDeserialized, etc.) before we cache any information about it
        RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
    }

    internal static Config GetConfig()
    {
        var config = new Config().Populate(JsConfig.GetConfig());
        if (TextCase != TextCase.Default)
            config.TextCase = TextCase;
        return config;
    }

    /// <summary>
    /// Always emit type info for this type.  Takes precedence over ExcludeTypeInfo
    /// </summary>
    public static bool? IncludeTypeInfo;

    /// <summary>
    /// Never emit type info for this type
    /// </summary>
    public static bool? ExcludeTypeInfo;

    /// <summary>
    /// Text case to use for property names (Default = PascalCase)
    /// </summary>
    public static TextCase TextCase { get; set; }

    /// <summary>
    /// Define custom serialization fn for BCL Structs
    /// </summary>
    private static Func<T, string> serializeFn;

    /// <summary>
    /// 
    /// </summary>
    public static Func<T, string> SerializeFn
    {
        get => serializeFn;
        set
        {
            serializeFn = value;
            if (value != null)
                JsConfig.HasSerializeFn.Add(typeof(T));
            else
                JsConfig.HasSerializeFn.Remove(typeof(T));

            ClearFnCaches();
        }
    }

    /// <summary>
    /// Whether there is a fn (raw or otherwise)
    /// </summary>
    public static bool HasSerializeFn => !JsState.InSerializer<T>() && (serializeFn != null || rawSerializeFn != null);

    /// <summary>
    /// Define custom raw serialization fn
    /// </summary>
    private static Func<T, string> rawSerializeFn;

    /// <summary>
    /// 
    /// </summary>
    public static Func<T, string> RawSerializeFn
    {
        get => rawSerializeFn;
        set
        {
            rawSerializeFn = value;
            if (value != null)
                JsConfig.HasSerializeFn.Add(typeof(T));
            else
                JsConfig.HasSerializeFn.Remove(typeof(T));

            ClearFnCaches();
        }
    }

    /// <summary>
    /// Define custom serialization hook
    /// </summary>
    private static Func<T, T> onSerializingFn;
    /// <summary>
    /// 
    /// </summary>
    public static Func<T, T> OnSerializingFn
    {
        get => onSerializingFn;
        set { onSerializingFn = value; RefreshWrite(); }
    }

    /// <summary>
    /// Define custom after serialization hook
    /// </summary>
    private static Action<T> onSerializedFn;

    /// <summary>
    /// 
    /// </summary>
    public static Action<T> OnSerializedFn
    {
        get => onSerializedFn;
        set { onSerializedFn = value; RefreshWrite(); }
    }

    /// <summary>
    /// Define custom deserialization fn for BCL Structs
    /// </summary>
    private static Func<string, T> deSerializeFn;

    /// <summary>
    /// 
    /// </summary>
    public static Func<string, T> DeSerializeFn
    {
        get => deSerializeFn;
        set { deSerializeFn = value; RefreshRead(); }
    }

    /// <summary>
    /// Define custom raw deserialization fn for objects
    /// </summary>
    private static Func<string, T> rawDeserializeFn;

    /// <summary>
    /// 
    /// </summary>
    public static Func<string, T> RawDeserializeFn
    {
        get => rawDeserializeFn;
        set { rawDeserializeFn = value; RefreshRead(); }
    }

    /// <summary>
    /// 
    /// </summary>
    public static bool HasDeserializeFn => !JsState.InDeserializer<T>() && (DeSerializeFn != null || RawDeserializeFn != null);

    private static Func<T, T> onDeserializedFn;

    /// <summary>
    /// 
    /// </summary>
    public static Func<T, T> OnDeserializedFn
    {
        get => onDeserializedFn;
        set { onDeserializedFn = value; RefreshRead(); }
    }

    /// <summary>
    /// 
    /// </summary>
    public static bool HasDeserializingFn => OnDeserializingFn != null;

    private static Func<T, string, object, object> onDeserializingFn;

    /// <summary>
    /// 
    /// </summary>
    public static Func<T, string, object, object> OnDeserializingFn
    {
        get => onDeserializingFn;
        set { onDeserializingFn = value; RefreshRead(); }
    }

    /// <summary>
    /// Exclude specific properties of this type from being serialized
    /// </summary>
    public static string[] ExcludePropertyNames;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="obj"></param>
    /// <typeparam name="TSerializer"></typeparam>
    public static void WriteFn<TSerializer>(TextWriter writer, object obj)
    {
        if (RawSerializeFn != null && !JsState.InSerializer<T>())
        {
            JsState.RegisterSerializer<T>();
            try
            {
                writer.Write(RawSerializeFn((T)obj));
            }
            finally
            {
                JsState.UnRegisterSerializer<T>();
            }
        }
        else if (SerializeFn != null && !JsState.InSerializer<T>())
        {
            JsState.RegisterSerializer<T>();
            try
            {
                var serializer = JsWriter.GetTypeSerializer<TSerializer>();
                serializer.WriteString(writer, SerializeFn((T)obj));
            }
            finally
            {
                JsState.UnRegisterSerializer<T>();
            }
        }
        else
        {
            var writerFn = JsonWriter.Instance.GetWriteFn<T>();
            writerFn(writer, obj);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static object ParseFn(string str)
    {
        return DeSerializeFn(str);
    }

    internal static object ParseFn(ITypeSerializer serializer, string str)
    {
        if (RawDeserializeFn != null && !JsState.InDeserializer<T>())
        {
            JsState.RegisterDeserializer<T>();
            try
            {
                return RawDeserializeFn(str);
            }
            finally
            {
                JsState.UnRegisterDeserializer<T>();
            }
        }

        if (DeSerializeFn != null && !JsState.InDeserializer<T>())
        {
            JsState.RegisterDeserializer<T>();
            try
            {
                return DeSerializeFn(serializer.UnescapeString(str));
            }
            finally
            {
                JsState.UnRegisterDeserializer<T>();
            }
        }

        var parseFn = JsonReader.Instance.GetParseFn<T>();
        return parseFn(str);
    }

    internal static void ClearFnCaches()
    {
        JsonWriter<T>.Reset();
        JsvWriter<T>.Reset();
    }

    /// <summary>
    /// 
    /// </summary>
    public static void Reset()
    {
        RawSerializeFn = null;
        DeSerializeFn = null;
        ExcludePropertyNames = null;
        TextCase = TextCase.Default;
        IncludeTypeInfo = ExcludeTypeInfo = null;
    }

    /// <summary>
    /// 
    /// </summary>
    public static void RefreshRead()
    {
        JsonReader<T>.Refresh();
        JsvReader<T>.Refresh();
    }

    /// <summary>
    /// 
    /// </summary>
    public static void RefreshWrite()
    {
        JsonWriter<T>.Refresh();
        JsvWriter<T>.Refresh();
    }
}

/// <summary>
/// 
/// </summary>
public enum PropertyConvention
{
    /// <summary>
    /// The property names on target types must match property names in the JSON source
    /// </summary>
    Strict,

    /// <summary>
    /// The property names on target types may not match the property names in the JSON source
    /// </summary>
    Lenient
}

/// <summary>
/// 
/// </summary>
public enum DateHandler
{
    /// <summary>
    /// 
    /// </summary>
    TimestampOffset,
    /// <summary>
    /// 
    /// </summary>
    DCJSCompatible,
    /// <summary>
    /// 
    /// </summary>
    ISO8601,
    /// <summary>
    /// 
    /// </summary>
    ISO8601DateOnly,
    /// <summary>
    /// 
    /// </summary>
    ISO8601DateTime,
    /// <summary>
    /// 
    /// </summary>
    RFC1123,
    /// <summary>
    /// 
    /// </summary>
    UnixTime,
    /// <summary>
    /// 
    /// </summary>
    UnixTimeMs,
}

/// <summary>
/// 
/// </summary>
public enum TimeSpanHandler
{
    /// <summary>
    /// Uses the xsd format like PT15H10M20S
    /// </summary>
    DurationFormat,

    /// <summary>
    /// Uses the standard .net ToString method of the TimeSpan class
    /// </summary>
    StandardFormat
}

/// <summary>
/// 
/// </summary>
public enum TextCase
{
    /// <summary>
    /// If unspecified uses PascalCase
    /// </summary>
    Default,

    /// <summary>
    /// PascalCase
    /// </summary>
    PascalCase,

    /// <summary>
    /// camelCase
    /// </summary>
    CamelCase,

    /// <summary>
    /// snake_case
    /// </summary>
    SnakeCase,
}