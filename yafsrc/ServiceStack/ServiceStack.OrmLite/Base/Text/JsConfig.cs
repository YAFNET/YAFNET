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

using ServiceStack.OrmLite.Base.Text.Common;
using ServiceStack.OrmLite.Base.Text.Json;
using ServiceStack.OrmLite.Base.Text.Jsv;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Class JsConfig.
/// </summary>
public static class JsConfig
{
    /// <summary>
    /// Initializes static members of the <see cref="JsConfig"/> class.
    /// </summary>
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
    public static void Init()
    {
        Config.Init();
    }

    /// <summary>
    /// Initialize global config and assert that it's no longer mutated
    /// </summary>
    /// <param name="config">The configuration.</param>
    public static void Init(Config config)
    {
        Config.Init(config);
    }

    /// <summary>
    /// Gets a value indicating whether this instance has initialize.
    /// </summary>
    /// <value><c>true</c> if this instance has initialize; otherwise, <c>false</c>.</value>
    public static bool HasInit => Config.HasInit;

    /// <summary>
    /// Begins the scope.
    /// </summary>
    /// <returns>JsConfigScope.</returns>
    public static JsConfigScope BeginScope()
    {
        return new JsConfigScope(); // Populated with Config.Instance
    }

    /// <summary>
    /// Creates the scope.
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <param name="scope">The scope.</param>
    /// <returns>JsConfigScope.</returns>
    public static JsConfigScope CreateScope(string config, JsConfigScope scope = null)
    {
        if (string.IsNullOrEmpty(config))
        {
            return scope;
        }

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
                case "epcn":
                case "emitpascalcasenames":
                    scope.TextCase = boolValue ? TextCase.PascalCase : scope.TextCase;
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
    /// Gets or sets the ut f8 encoding.
    /// </summary>
    /// <value>The ut f8 encoding.</value>
    public static UTF8Encoding UTF8Encoding { get; set; } = new(false);

    /// <summary>
    /// Withes the specified configuration.
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <returns>JsConfigScope.</returns>
    public static JsConfigScope With(Config config)
    {
        return (JsConfigScope)new JsConfigScope().Populate(config);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [try to parse primitive type values].
    /// </summary>
    /// <value><c>true</c> if [try to parse primitive type values]; otherwise, <c>false</c>.</value>
    public static bool TryToParsePrimitiveTypeValues
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.TryToParsePrimitiveTypeValues : Config.Instance.TryToParsePrimitiveTypeValues;
        set => Config.AssertNotInit().TryToParsePrimitiveTypeValues = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether [try parse into best fit].
    /// </summary>
    /// <value><c>true</c> if [try parse into best fit]; otherwise, <c>false</c>.</value>
    public static bool TryParseIntoBestFit
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.TryParseIntoBestFit : Config.Instance.TryParseIntoBestFit;
        set => Config.AssertNotInit().TryParseIntoBestFit = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether [exclude default values].
    /// </summary>
    /// <value><c>true</c> if [exclude default values]; otherwise, <c>false</c>.</value>
    public static bool ExcludeDefaultValues
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.ExcludeDefaultValues : Config.Instance.ExcludeDefaultValues;
        set => Config.AssertNotInit().ExcludeDefaultValues = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether [include null values].
    /// </summary>
    /// <value><c>true</c> if [include null values]; otherwise, <c>false</c>.</value>
    public static bool IncludeNullValues
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.IncludeNullValues : Config.Instance.IncludeNullValues;
        set => Config.AssertNotInit().IncludeNullValues = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether [include null values in dictionaries].
    /// </summary>
    /// <value><c>true</c> if [include null values in dictionaries]; otherwise, <c>false</c>.</value>
    public static bool IncludeNullValuesInDictionaries
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.IncludeNullValuesInDictionaries : Config.Instance.IncludeNullValuesInDictionaries;
        set => Config.AssertNotInit().IncludeNullValuesInDictionaries = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether [treat enum as integer].
    /// </summary>
    /// <value><c>true</c> if [treat enum as integer]; otherwise, <c>false</c>.</value>
    public static bool TreatEnumAsInteger
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.TreatEnumAsInteger : Config.Instance.TreatEnumAsInteger;
        set => Config.AssertNotInit().TreatEnumAsInteger = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether [exclude type information].
    /// </summary>
    /// <value><c>true</c> if [exclude type information]; otherwise, <c>false</c>.</value>
    public static bool ExcludeTypeInfo
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.ExcludeTypeInfo : Config.Instance.ExcludeTypeInfo;
        set => Config.AssertNotInit().ExcludeTypeInfo = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether [include type information].
    /// </summary>
    /// <value><c>true</c> if [include type information]; otherwise, <c>false</c>.</value>
    public static bool IncludeTypeInfo
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.IncludeTypeInfo : Config.Instance.IncludeTypeInfo;
        set => Config.AssertNotInit().IncludeTypeInfo = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="JsConfig"/> is indent.
    /// </summary>
    /// <value><c>true</c> if indent; otherwise, <c>false</c>.</value>
    public static bool Indent
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.Indent : Config.Instance.Indent;
        set => Config.AssertNotInit().Indent = value;
    }

    /// <summary>
    /// Gets or sets the type attribute.
    /// </summary>
    /// <value>The type attribute.</value>
    public static string TypeAttr
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.TypeAttr : Config.Instance.TypeAttr;
        set
        {
            var config = Config.AssertNotInit();
            config.TypeAttr = value;
        }
    }

    /// <summary>
    /// Gets the json type attribute in object.
    /// </summary>
    /// <value>The json type attribute in object.</value>
    static internal string JsonTypeAttrInObject => JsConfigScope.Current != null ? JsConfigScope.Current.JsonTypeAttrInObject : Config.Instance.JsonTypeAttrInObject;

    /// <summary>
    /// Gets the JSV type attribute in object.
    /// </summary>
    /// <value>The JSV type attribute in object.</value>
    static internal string JsvTypeAttrInObject => JsConfigScope.Current != null ? JsConfigScope.Current.JsvTypeAttrInObject : Config.Instance.JsvTypeAttrInObject;

    /// <summary>
    /// Gets or sets the type writer.
    /// </summary>
    /// <value>The type writer.</value>
    public static Func<Type, string> TypeWriter
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.TypeWriter : Config.Instance.TypeWriter;
        set => Config.AssertNotInit().TypeWriter = value;
    }

    /// <summary>
    /// Gets or sets the type finder.
    /// </summary>
    /// <value>The type finder.</value>
    public static Func<string, Type> TypeFinder
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.TypeFinder : Config.Instance.TypeFinder;
        set => Config.AssertNotInit().TypeFinder = value;
    }

    /// <summary>
    /// Gets or sets the parse primitive function.
    /// </summary>
    /// <value>The parse primitive function.</value>
    public static Func<string, object> ParsePrimitiveFn
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.ParsePrimitiveFn : Config.Instance.ParsePrimitiveFn;
        set => Config.AssertNotInit().ParsePrimitiveFn = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether [system json compatible].
    /// </summary>
    /// <value><c>true</c> if [system json compatible]; otherwise, <c>false</c>.</value>
    public static bool SystemJsonCompatible {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.SystemJsonCompatible : Config.Instance.SystemJsonCompatible;
        set => Config.AssertNotInit().SystemJsonCompatible = value;
    }

    /// <summary>
    /// Gets or sets the date handler.
    /// </summary>
    /// <value>The date handler.</value>
    public static DateHandler DateHandler
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.DateHandler : Config.Instance.DateHandler;
        set => Config.AssertNotInit().DateHandler = value;
    }

    /// <summary>
    /// Sets which format to use when serializing TimeSpans
    /// </summary>
    /// <value>The time span handler.</value>
    public static TimeSpanHandler TimeSpanHandler
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.TimeSpanHandler : Config.Instance.TimeSpanHandler;
        set => Config.AssertNotInit().TimeSpanHandler = value;
    }

    /// <summary>
    /// Text case to use for property names (Default = PascalCase)
    /// </summary>
    /// <value>The text case.</value>
    public static TextCase TextCase
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.TextCase : Config.Instance.TextCase;
        set => Config.AssertNotInit().TextCase = value;
    }

    /// <summary>
    /// Avoid multiple static property checks by getting snapshot of active config
    /// </summary>
    /// <returns>Config.</returns>
    public static Config GetConfig()
    {
        return JsConfigScope.Current ?? Config.Instance;
    }

    /// <summary>
    /// Gets or sets a value indicating if the framework should throw serialization exceptions
    /// or continue regardless of serialization errors. If <see langword="true" />  the framework
    /// will throw; otherwise, it will parse as many fields as possible. The default is <see langword="false" />.
    /// </summary>
    /// <value><c>true</c> if [throw on error]; otherwise, <c>false</c>.</value>
    public static bool ThrowOnError
    {
        // obeying the use of ThreadStatic, but allowing for setting JsConfig once as is the normal case
        get => JsConfigScope.Current != null ? JsConfigScope.Current.ThrowOnError : Config.Instance.ThrowOnError;
        set => Config.AssertNotInit().ThrowOnError = value;
    }

    /// <summary>
    /// Gets or sets a value indicating if the framework should skip automatic <see cref="DateTime" /> conversions.
    /// Dates will be handled literally, any included timezone encoding will be lost and the date will be treaded as DateTimeKind.Local
    /// Utc formatted input will result in DateTimeKind.Utc output. Any input without TZ data will be set DateTimeKind.Unspecified
    /// This will take precedence over other flags like AlwaysUseUtc
    /// JsConfig.DateHandler = DateHandler.ISO8601 should be used when set true for consistent de/serialization.
    /// </summary>
    /// <value><c>true</c> if [skip date time conversion]; otherwise, <c>false</c>.</value>
    public static bool SkipDateTimeConversion
    {
        // obeying the use of ThreadStatic, but allowing for setting JsConfig once as is the normal case
        get => JsConfigScope.Current != null ? JsConfigScope.Current.SkipDateTimeConversion : Config.Instance.SkipDateTimeConversion;
        set => Config.AssertNotInit().SkipDateTimeConversion = value;
    }

    /// <summary>
    /// Gets or sets a value indicating if the framework should always assume <see cref="DateTime" /> is in UTC format if Kind is Unspecified.
    /// </summary>
    /// <value><c>true</c> if [assume UTC]; otherwise, <c>false</c>.</value>
    public static bool AssumeUtc
    {
        // obeying the use of ThreadStatic, but allowing for setting JsConfig once as is the normal case
        get => JsConfigScope.Current != null ? JsConfigScope.Current.AssumeUtc : Config.Instance.AssumeUtc;
        set => Config.AssertNotInit().AssumeUtc = value;
    }

    /// <summary>
    /// The has serialize function
    /// </summary>
    static internal HashSet<Type> HasSerializeFn { get; set; } = [];

    /// <summary>
    /// The has include default value
    /// </summary>
    static internal HashSet<Type> HasIncludeDefaultValue { get; set; } = [];

    /// <summary>
    /// The treat value as reference types
    /// </summary>
    public static HashSet<Type> TreatValueAsRefTypes { get; set; } = [];

    /// <summary>
    /// If set to true, Interface types will be preferred over concrete types when serializing.
    /// </summary>
    /// <value><c>true</c> if [prefer interfaces]; otherwise, <c>false</c>.</value>
    public static bool PreferInterfaces
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.PreferInterfaces : Config.Instance.PreferInterfaces;
        set => Config.AssertNotInit().PreferInterfaces = value;
    }

    /// <summary>
    /// Treats the type of as reference.
    /// </summary>
    /// <param name="valueType">Type of the value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    static internal bool TreatAsRefType(Type valueType)
    {
        return TreatValueAsRefTypes.Contains(valueType.IsGenericType ? valueType.GetGenericTypeDefinition() : valueType);
    }

    /// <summary>
    /// If set to true, Interface types will be preferred over concrete types when serializing.
    /// </summary>
    /// <value><c>true</c> if [include public fields]; otherwise, <c>false</c>.</value>
    public static bool IncludePublicFields
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.IncludePublicFields : Config.Instance.IncludePublicFields;
        set => Config.AssertNotInit().IncludePublicFields = value;
    }

    /// <summary>
    /// Sets the maximum depth to avoid circular dependencies
    /// </summary>
    /// <value>The maximum depth.</value>
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
    /// <value>The model factory.</value>
    public static EmptyCtorFactoryDelegate ModelFactory
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.ModelFactory : Config.Instance.ModelFactory;
        set => Config.AssertNotInit().ModelFactory = value;
    }

    /// <summary>
    /// Gets or sets the exclude types.
    /// </summary>
    /// <value>The exclude types.</value>
    public static HashSet<Type> ExcludeTypes
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.ExcludeTypes : Config.Instance.ExcludeTypes;
        set => Config.AssertNotInit().ExcludeTypes = value;
    }

    /// <summary>
    /// Gets or sets the exclude type names.
    /// </summary>
    /// <value>The exclude type names.</value>
    public static HashSet<string> ExcludeTypeNames
    {
        get => JsConfigScope.Current != null ? JsConfigScope.Current.ExcludeTypeNames : Config.Instance.ExcludeTypeNames;
        set => Config.AssertNotInit().ExcludeTypeNames = value;
    }

    /// <summary>
    /// Gets or sets the allow runtime type with attributes named.
    /// </summary>
    /// <value>The allow runtime type with attributes named.</value>
    public static HashSet<string> AllowRuntimeTypeWithAttributesNamed { get; set; } = [];

    /// <summary>
    /// Gets or sets the allow runtime type with interfaces named.
    /// </summary>
    /// <value>The allow runtime type with interfaces named.</value>
    public static HashSet<string> AllowRuntimeTypeWithInterfacesNamed { get; set; } = [];

    /// <summary>
    /// Gets or sets the allow runtime type in types.
    /// </summary>
    /// <value>The allow runtime type in types.</value>
    public static HashSet<string> AllowRuntimeTypeInTypes { get; set; } = [];

    /// <summary>
    /// Gets or sets the allow runtime type in types with namespaces.
    /// </summary>
    /// <value>The allow runtime type in types with namespaces.</value>
    public static HashSet<string> AllowRuntimeTypeInTypesWithNamespaces { get; set; } = [];

    /// <summary>
    /// Gets or sets the type of the allow runtime.
    /// </summary>
    /// <value>The type of the allow runtime.</value>
    public static Func<Type, bool> AllowRuntimeType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [allow runtime interfaces].
    /// </summary>
    /// <value><c>true</c> if [allow runtime interfaces]; otherwise, <c>false</c>.</value>
    public static bool AllowRuntimeInterfaces { get; set; }

    /// <summary>
    /// Resets this instance.
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
        HasSerializeFn = [];
        HasIncludeDefaultValue = [];
        TreatValueAsRefTypes = [typeof(KeyValuePair<,>)];
        __uniqueTypes = [];

        // Called when writing each string, too expensive to maintain as scoped config
        AllowRuntimeInterfaces = true;
        AllowRuntimeType = null;
        AllowRuntimeTypeWithAttributesNamed = [
            nameof(SerializableAttribute),
            nameof(DataContractAttribute),
            nameof(RuntimeSerializableAttribute)
        ];
        AllowRuntimeTypeWithInterfacesNamed = [
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
        ];
        AllowRuntimeTypeInTypesWithNamespaces = [
            "ServiceStack.Auth",
            "ServiceStack.Messaging"
        ];
        SystemJsonCompatible = false;
        AllowRuntimeTypeInTypes = [];
        PlatformExtensions.ClearRuntimeAttributes();
        ReflectionExtensions.Reset();
        JsState.Reset();
    }

    /// <summary>
    /// Resets the specified caches for type.
    /// </summary>
    /// <param name="cachesForType">Type of the caches for.</param>
    private static void Reset(Type cachesForType)
    {
        typeof(JsConfig<>).MakeGenericType(cachesForType).InvokeReset();
        typeof(TypeConfig<>).MakeGenericType(cachesForType).InvokeReset();
    }

    /// <summary>
    /// Invokes the reset.
    /// </summary>
    /// <param name="genericType">Type of the generic.</param>
    static internal void InvokeReset(this Type genericType)
    {
        var methodInfo = genericType.GetStaticMethod("Reset");
        methodInfo.Invoke(null, null);
    }

    /// <summary>
    /// The unique types
    /// </summary>
    static internal HashSet<Type> __uniqueTypes = [];
    /// <summary>
    /// The unique types count
    /// </summary>
    static internal int __uniqueTypesCount;

    /// <summary>
    /// Adds the type of the unique.
    /// </summary>
    /// <param name="type">The type.</param>
    static internal void AddUniqueType(Type type)
    {
        if (__uniqueTypes.Contains(type))
        {
            return;
        }

        HashSet<Type> newTypes, snapshot;
        do
        {
            snapshot = __uniqueTypes;
            newTypes = [..__uniqueTypes, type];
            __uniqueTypesCount = newTypes.Count;

        }

        while (!ReferenceEquals(
                   Interlocked.CompareExchange(ref __uniqueTypes, newTypes, snapshot), snapshot));
    }
}

/// <summary>
/// Class JsConfig.
/// </summary>
/// <typeparam name="T"></typeparam>
public class JsConfig<T>
{
    /// <summary>
    /// Initializes static members of the <see cref="JsConfig{T}"/> class.
    /// </summary>
    static JsConfig()
    {
        // Run the type's static constructor (which may set OnDeserialized, etc.) before we cache any information about it
        RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
    }

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    /// <returns>Config.</returns>
    static internal Config GetConfig()
    {
        var config = new Config().Populate(JsConfig.GetConfig());
        if (TextCase != TextCase.Default)
        {
            config.TextCase = TextCase;
        }

        return config;
    }

    /// <summary>
    /// Always emit type info for this type.  Takes precedence over ExcludeTypeInfo
    /// </summary>
    public static bool? IncludeTypeInfo { get; set; }

    /// <summary>
    /// Never emit type info for this type
    /// </summary>
    public static bool? ExcludeTypeInfo { get; set; }

    /// <summary>
    /// Text case to use for property names (Default = PascalCase)
    /// </summary>
    /// <value>The text case.</value>
    public static TextCase TextCase { get; set; }

    /// <summary>
    /// Define custom serialization fn for BCL Structs
    /// </summary>
    private static Func<T, string> serializeFn;

    /// <summary>
    /// Gets or sets the serialize function.
    /// </summary>
    /// <value>The serialize function.</value>
    public static Func<T, string> SerializeFn
    {
        get => serializeFn;
        set
        {
            serializeFn = value;
            if (value != null)
            {
                JsConfig.HasSerializeFn.Add(typeof(T));
            }
            else
            {
                JsConfig.HasSerializeFn.Remove(typeof(T));
            }

            ClearFnCaches();
        }
    }

    /// <summary>
    /// Whether there is a fn (raw or otherwise)
    /// </summary>
    /// <value><c>true</c> if this instance has serialize function; otherwise, <c>false</c>.</value>
    public static bool HasSerializeFn => !JsState.InSerializer<T>() && (serializeFn != null || rawSerializeFn != null);

    /// <summary>
    /// Define custom raw serialization fn
    /// </summary>
    private static Func<T, string> rawSerializeFn;

    /// <summary>
    /// Gets or sets the raw serialize function.
    /// </summary>
    /// <value>The raw serialize function.</value>
    public static Func<T, string> RawSerializeFn
    {
        get => rawSerializeFn;
        set
        {
            rawSerializeFn = value;
            if (value != null)
            {
                JsConfig.HasSerializeFn.Add(typeof(T));
            }
            else
            {
                JsConfig.HasSerializeFn.Remove(typeof(T));
            }

            ClearFnCaches();
        }
    }

    /// <summary>
    /// Gets or sets the on serializing function.
    /// </summary>
    /// <value>The on serializing function.</value>
    public static Func<T, T> OnSerializingFn {
        get;
        set {
            field = value;
            RefreshWrite();
        }
    }

    /// <summary>
    /// Gets or sets the on serialized function.
    /// </summary>
    /// <value>The on serialized function.</value>
    public static Action<T> OnSerializedFn {
        get;
        set {
            field = value;
            RefreshWrite();
        }
    }

    /// <summary>
    /// Gets or sets the de serialize function.
    /// </summary>
    /// <value>The de serialize function.</value>
    public static Func<string, T> DeSerializeFn {
        get;
        set {
            field = value;
            RefreshRead();
        }
    }

    /// <summary>
    /// Gets or sets the raw deserialize function.
    /// </summary>
    /// <value>The raw deserialize function.</value>
    public static Func<string, T> RawDeserializeFn {
        get;
        set {
            field = value;
            RefreshRead();
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance has deserialize function.
    /// </summary>
    /// <value><c>true</c> if this instance has deserialize function; otherwise, <c>false</c>.</value>
    public static bool HasDeserializeFn => !JsState.InDeserializer<T>() && (DeSerializeFn != null || RawDeserializeFn != null);

    /// <summary>
    /// Gets or sets the on deserialized function.
    /// </summary>
    /// <value>The on deserialized function.</value>
    public static Func<T, T> OnDeserializedFn {
        get;
        set {
            field = value;
            RefreshRead();
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance has deserializing function.
    /// </summary>
    /// <value><c>true</c> if this instance has deserializing function; otherwise, <c>false</c>.</value>
    public static bool HasDeserializingFn => OnDeserializingFn != null;

    /// <summary>
    /// Gets or sets the on deserializing function.
    /// </summary>
    /// <value>The on deserializing function.</value>
    public static Func<T, string, object, object> OnDeserializingFn {
        get;
        set {
            field = value;
            RefreshRead();
        }
    }

    /// <summary>
    /// Exclude specific properties of this type from being serialized
    /// </summary>
    public static string[] ExcludePropertyNames { get; set; }

    /// <summary>
    /// Writes the function.
    /// </summary>
    /// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
    /// <param name="writer">The writer.</param>
    /// <param name="obj">The object.</param>
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
    /// Parses the function.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <returns>System.Object.</returns>
    public static object ParseFn(string str)
    {
        return DeSerializeFn(str);
    }

    /// <summary>
    /// Parses the function.
    /// </summary>
    /// <param name="serializer">The serializer.</param>
    /// <param name="str">The string.</param>
    /// <returns>System.Object.</returns>
    static internal object ParseFn(ITypeSerializer serializer, string str)
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

    /// <summary>
    /// Clears the function caches.
    /// </summary>
    static internal void ClearFnCaches()
    {
        JsonWriter<T>.Reset();
        JsvWriter<T>.Reset();
    }

    /// <summary>
    /// Resets this instance.
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
    /// Refreshes the read.
    /// </summary>
    public static void RefreshRead()
    {
        JsonReader<T>.Refresh();
        JsvReader<T>.Refresh();
    }

    /// <summary>
    /// Refreshes the write.
    /// </summary>
    public static void RefreshWrite()
    {
        JsonWriter<T>.Refresh();
        JsvWriter<T>.Refresh();
    }
}

/// <summary>
/// Enum PropertyConvention
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
/// Enum DateHandler
/// </summary>
public enum DateHandler
{
    /// <summary>
    /// The timestamp offset
    /// </summary>
    TimestampOffset,
    /// <summary>
    /// The DCJS compatible
    /// </summary>
    DCJSCompatible,
    /// <summary>
    /// The is o8601
    /// </summary>
    ISO8601,
    /// <summary>
    /// The is o8601 date only
    /// </summary>
    ISO8601DateOnly,
    /// <summary>
    /// The is o8601 date time
    /// </summary>
    ISO8601DateTime,
    /// <summary>
    /// The rf C1123
    /// </summary>
    RFC1123,
    /// <summary>
    /// The unix time
    /// </summary>
    UnixTime,
    /// <summary>
    /// The unix time ms
    /// </summary>
    UnixTimeMs
}

/// <summary>
/// Enum TimeSpanHandler
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
/// Enum TextCase
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
    SnakeCase
}