// ***********************************************************************
// <copyright file="JsConfigScope.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;

using ServiceStack.Text.Common;
using ServiceStack.Text.Json;
using ServiceStack.Text.Jsv;

namespace ServiceStack.Text;

/// <summary>
/// 
/// </summary>
public sealed class JsConfigScope : Config, IDisposable
{
    private bool disposed;

    private readonly JsConfigScope parent;

#if NETCORE        
        private static AsyncLocal<JsConfigScope> head = new AsyncLocal<JsConfigScope>();
#else
    [ThreadStatic] private static JsConfigScope head;
#endif

    internal JsConfigScope()
    {
        PclExport.Instance.BeginThreadAffinity();

#if NETCORE        
            parent = head.Value;
            head.Value = this;
#else
        parent = head;
        head = this;
#endif
    }

    internal static JsConfigScope Current =>
#if NETCORE
            head.Value;
#else
        head;
#endif

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        if (!disposed)
        {
            disposed = true;
#if NETCORE        
                head.Value = parent;
#else
            head = parent;
#endif

            PclExport.Instance.EndThreadAffinity();
        }
    }
}

/// <summary>
/// 
/// </summary>
public class Config
{
    private static Config instance;
    internal static Config Instance => instance ??= new Config(Defaults);
    internal static bool HasInit;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public static Config AssertNotInit() => HasInit
                                                ? throw new NotSupportedException("JsConfig can't be mutated after JsConfig.Init(). Use BeginScope() or CreateScope() to use custom config after Init().")
                                                : Instance;

    private static string InitStackTrace;

    /// <summary>
    /// 
    /// </summary>
    public static void Init() => Init(null);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="config"></param>
    /// <exception cref="NotSupportedException"></exception>
    public static void Init(Config config)
    {
        if (HasInit && Env.StrictMode)
            throw new NotSupportedException($"JsConfig has already been initialized at: {InitStackTrace}");

        if (config != null)
            instance = config;

        HasInit = true;
        InitStackTrace = Environment.StackTrace;
    }

    internal static void Reset()
    {
        HasInit = false;
        Instance.Populate(Defaults);
    }

    /// <summary>
    /// 
    /// </summary>
    public Config()
    {
        Populate(Instance);
    }

    private Config(Config config)
    {
        if (config != null) // Defaults=null, instance=Defaults
            Populate(config);
    }

    /// <summary>
    /// 
    /// </summary>
    public bool ConvertObjectTypesIntoStringDictionary { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool TryToParsePrimitiveTypeValues { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool TryToParseNumericType { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool TryParseIntoBestFit { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public ParseAsType ParsePrimitiveFloatingPointTypes { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public ParseAsType ParsePrimitiveIntegerTypes { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool ExcludeDefaultValues { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool IncludeNullValues { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool IncludeNullValuesInDictionaries { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool IncludeDefaultEnums { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool TreatEnumAsInteger { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool ExcludeTypeInfo { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool IncludeTypeInfo { get; set; }

    public bool Indent { get; set; }

    private string typeAttr;
    /// <summary>
    /// 
    /// </summary>
    public string TypeAttr
    {
        get => typeAttr;
        set
        {
            typeAttrSpan = null;
            jsonTypeAttrInObject = null;
            jsvTypeAttrInObject = null;
            typeAttr = value;
        }
    }

    private ReadOnlyMemory<char>? typeAttrSpan;
    /// <summary>
    /// 
    /// </summary>
    public ReadOnlyMemory<char> TypeAttrMemory => typeAttrSpan ??= TypeAttr.AsMemory();
    /// <summary>
    /// 
    /// </summary>
    public string DateTimeFormat { get; set; }
    private string jsonTypeAttrInObject;
    internal string JsonTypeAttrInObject => jsonTypeAttrInObject ??= JsonTypeSerializer.GetTypeAttrInObject(TypeAttr);
    private string jsvTypeAttrInObject;
    internal string JsvTypeAttrInObject => jsvTypeAttrInObject ??= JsvTypeSerializer.GetTypeAttrInObject(TypeAttr);

    /// <summary>
    /// 
    /// </summary>
    public Func<Type, string> TypeWriter { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Func<string, Type> TypeFinder { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Func<string, object> ParsePrimitiveFn { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public DateHandler DateHandler { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public TimeSpanHandler TimeSpanHandler { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public PropertyConvention PropertyConvention { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public TextCase TextCase { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool ThrowOnError { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool SkipDateTimeConversion { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool AlwaysUseUtc { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool AssumeUtc { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool AppendUtcOffset { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool PreferInterfaces { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool IncludePublicFields { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int MaxDepth { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public DeserializationErrorDelegate OnDeserializationError { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public EmptyCtorFactoryDelegate ModelFactory { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string[] ExcludePropertyReferences { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public HashSet<Type> ExcludeTypes { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public HashSet<string> ExcludeTypeNames { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool EscapeUnicode { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool EscapeHtmlChars { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public static Config Defaults => new(null)
                                         {
                                             ConvertObjectTypesIntoStringDictionary = false,
                                             TryToParsePrimitiveTypeValues = false,
                                             TryToParseNumericType = false,
                                             TryParseIntoBestFit = false,
                                             ParsePrimitiveFloatingPointTypes = ParseAsType.Decimal,
                                             ParsePrimitiveIntegerTypes = ParseAsType.Byte | ParseAsType.SByte | ParseAsType.Int16 | ParseAsType.UInt16 |
                                                                          ParseAsType.Int32 | ParseAsType.UInt32 | ParseAsType.Int64 | ParseAsType.UInt64,
                                             ExcludeDefaultValues = false,
                                             ExcludePropertyReferences = null,
                                             IncludeNullValues = false,
                                             IncludeNullValuesInDictionaries = false,
                                             IncludeDefaultEnums = true,
                                             TreatEnumAsInteger = false,
                                             ExcludeTypeInfo = false,
                                             IncludeTypeInfo = false,
                                             Indent = false,
                                             TypeAttr = JsWriter.TypeAttr,
                                             DateTimeFormat = null,
                                             TypeWriter = AssemblyUtils.WriteType,
                                             TypeFinder = AssemblyUtils.FindType,
                                             ParsePrimitiveFn = null,
                                             DateHandler = DateHandler.TimestampOffset,
                                             TimeSpanHandler = TimeSpanHandler.DurationFormat,
                                             TextCase = TextCase.Default,
                                             PropertyConvention = PropertyConvention.Strict,
                                             ThrowOnError = Env.StrictMode,
                                             SkipDateTimeConversion = false,
                                             AlwaysUseUtc = false,
                                             AssumeUtc = false,
                                             AppendUtcOffset = false,
                                             EscapeUnicode = false,
                                             EscapeHtmlChars = false,
                                             PreferInterfaces = false,
                                             IncludePublicFields = false,
                                             MaxDepth = 50,
                                             OnDeserializationError = null,
                                             ModelFactory = ReflectionExtensions.GetConstructorMethodToCache,
                                             ExcludeTypes = new HashSet<Type> {
                                                                                      typeof(System.IO.Stream),
                                                                                      typeof(System.Reflection.MethodBase),
                                                                                  },
                                             ExcludeTypeNames = new HashSet<string>()
                                         };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public Config Populate(Config config)
    {
        ConvertObjectTypesIntoStringDictionary = config.ConvertObjectTypesIntoStringDictionary;
        TryToParsePrimitiveTypeValues = config.TryToParsePrimitiveTypeValues;
        TryToParseNumericType = config.TryToParseNumericType;
        TryParseIntoBestFit = config.TryParseIntoBestFit;
        ParsePrimitiveFloatingPointTypes = config.ParsePrimitiveFloatingPointTypes;
        ParsePrimitiveIntegerTypes = config.ParsePrimitiveIntegerTypes;
        ExcludeDefaultValues = config.ExcludeDefaultValues;
        ExcludePropertyReferences = config.ExcludePropertyReferences;
        IncludeNullValues = config.IncludeNullValues;
        IncludeNullValuesInDictionaries = config.IncludeNullValuesInDictionaries;
        IncludeDefaultEnums = config.IncludeDefaultEnums;
        TreatEnumAsInteger = config.TreatEnumAsInteger;
        ExcludeTypeInfo = config.ExcludeTypeInfo;
        IncludeTypeInfo = config.IncludeTypeInfo;
        Indent = config.Indent;
        TypeAttr = config.TypeAttr;
        DateTimeFormat = config.DateTimeFormat;
        TypeWriter = config.TypeWriter;
        TypeFinder = config.TypeFinder;
        ParsePrimitiveFn = config.ParsePrimitiveFn;
        DateHandler = config.DateHandler;
        TimeSpanHandler = config.TimeSpanHandler;
        TextCase = config.TextCase;
        PropertyConvention = config.PropertyConvention;
        ThrowOnError = config.ThrowOnError;
        SkipDateTimeConversion = config.SkipDateTimeConversion;
        AlwaysUseUtc = config.AlwaysUseUtc;
        AssumeUtc = config.AssumeUtc;
        AppendUtcOffset = config.AppendUtcOffset;
        EscapeUnicode = config.EscapeUnicode;
        EscapeHtmlChars = config.EscapeHtmlChars;
        PreferInterfaces = config.PreferInterfaces;
        IncludePublicFields = config.IncludePublicFields;
        MaxDepth = config.MaxDepth;
        OnDeserializationError = config.OnDeserializationError;
        ModelFactory = config.ModelFactory;
        ExcludeTypes = config.ExcludeTypes;
        ExcludeTypeNames = config.ExcludeTypeNames;
        return this;
    }
}