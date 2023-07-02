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

using System.Threading;

/// <summary>
/// Class JsConfigScope. This class cannot be inherited.
/// Implements the <see cref="ServiceStack.Text.Config" />
/// Implements the <see cref="IDisposable" />
/// </summary>
/// <seealso cref="ServiceStack.Text.Config" />
/// <seealso cref="IDisposable" />
public sealed class JsConfigScope : Config, IDisposable
{
    /// <summary>
    /// The disposed
    /// </summary>
    private bool disposed;

    /// <summary>
    /// The parent
    /// </summary>
    private readonly JsConfigScope parent;

#if NET7_0_OR_GREATER        
        private static AsyncLocal<JsConfigScope> head = new AsyncLocal<JsConfigScope>();
#else
    /// <summary>
    /// The head
    /// </summary>
    [ThreadStatic] private static JsConfigScope head;
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="JsConfigScope"/> class.
    /// </summary>
    internal JsConfigScope()
    {
        PclExport.Instance.BeginThreadAffinity();

#if NET7_0_OR_GREATER        
            parent = head.Value;
            head.Value = this;
#else
        parent = head;
        head = this;
#endif
    }

    /// <summary>
    /// Gets the current.
    /// </summary>
    /// <value>The current.</value>
    internal static JsConfigScope Current =>
#if NET7_0_OR_GREATER
            head.Value;
#else
        head;
#endif

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        if (!disposed)
        {
            disposed = true;
#if NET7_0_OR_GREATER        
                head.Value = parent;
#else
            head = parent;
#endif

            PclExport.Instance.EndThreadAffinity();
        }
    }
}

/// <summary>
/// Class Config.
/// </summary>
public class Config
{
    /// <summary>
    /// The instance
    /// </summary>
    private static Config instance;
    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    internal static Config Instance => instance ??= new Config(Defaults);
    /// <summary>
    /// The has initialize
    /// </summary>
    internal static bool HasInit;

    /// <summary>
    /// Asserts the not initialize.
    /// </summary>
    /// <returns>Config.</returns>
    /// <exception cref="System.NotSupportedException">JsConfig can't be mutated after JsConfig.Init(). Use BeginScope() or CreateScope() to use custom config after Init().</exception>
    public static Config AssertNotInit() => HasInit
                                                ? throw new NotSupportedException("JsConfig can't be mutated after JsConfig.Init(). Use BeginScope() or CreateScope() to use custom config after Init().")
                                                : Instance;

    /// <summary>
    /// The initialize stack trace
    /// </summary>
    private static string InitStackTrace;

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    public static void Init() => Init(null);
    /// <summary>
    /// Initializes the specified configuration.
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <exception cref="System.NotSupportedException">JsConfig has already been initialized at: {InitStackTrace}</exception>
    public static void Init(Config config)
    {
        if (HasInit && Env.StrictMode)
            throw new NotSupportedException($"JsConfig has already been initialized at: {InitStackTrace}");

        if (config != null)
            instance = config;

        HasInit = true;
        InitStackTrace = Environment.StackTrace;
    }

    /// <summary>
    /// Resets this instance.
    /// </summary>
    internal static void Reset()
    {
        HasInit = false;
        Instance.Populate(Defaults);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Config"/> class.
    /// </summary>
    public Config()
    {
        Populate(Instance);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Config"/> class.
    /// </summary>
    /// <param name="config">The configuration.</param>
    private Config(Config config)
    {
        if (config != null) // Defaults=null, instance=Defaults
            Populate(config);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [convert object types into string dictionary].
    /// </summary>
    /// <value><c>true</c> if [convert object types into string dictionary]; otherwise, <c>false</c>.</value>
    public bool ConvertObjectTypesIntoStringDictionary { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [try to parse primitive type values].
    /// </summary>
    /// <value><c>true</c> if [try to parse primitive type values]; otherwise, <c>false</c>.</value>
    public bool TryToParsePrimitiveTypeValues { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [try to parse numeric type].
    /// </summary>
    /// <value><c>true</c> if [try to parse numeric type]; otherwise, <c>false</c>.</value>
    public bool TryToParseNumericType { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [try parse into best fit].
    /// </summary>
    /// <value><c>true</c> if [try parse into best fit]; otherwise, <c>false</c>.</value>
    public bool TryParseIntoBestFit { get; set; }
    /// <summary>
    /// Gets or sets the parse primitive floating point types.
    /// </summary>
    /// <value>The parse primitive floating point types.</value>
    public ParseAsType ParsePrimitiveFloatingPointTypes { get; set; }
    /// <summary>
    /// Gets or sets the parse primitive integer types.
    /// </summary>
    /// <value>The parse primitive integer types.</value>
    public ParseAsType ParsePrimitiveIntegerTypes { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [exclude default values].
    /// </summary>
    /// <value><c>true</c> if [exclude default values]; otherwise, <c>false</c>.</value>
    public bool ExcludeDefaultValues { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [include null values].
    /// </summary>
    /// <value><c>true</c> if [include null values]; otherwise, <c>false</c>.</value>
    public bool IncludeNullValues { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [include null values in dictionaries].
    /// </summary>
    /// <value><c>true</c> if [include null values in dictionaries]; otherwise, <c>false</c>.</value>
    public bool IncludeNullValuesInDictionaries { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [include default enums].
    /// </summary>
    /// <value><c>true</c> if [include default enums]; otherwise, <c>false</c>.</value>
    public bool IncludeDefaultEnums { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [treat enum as integer].
    /// </summary>
    /// <value><c>true</c> if [treat enum as integer]; otherwise, <c>false</c>.</value>
    public bool TreatEnumAsInteger { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [exclude type information].
    /// </summary>
    /// <value><c>true</c> if [exclude type information]; otherwise, <c>false</c>.</value>
    public bool ExcludeTypeInfo { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [include type information].
    /// </summary>
    /// <value><c>true</c> if [include type information]; otherwise, <c>false</c>.</value>
    public bool IncludeTypeInfo { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="Config"/> is indent.
    /// </summary>
    /// <value><c>true</c> if indent; otherwise, <c>false</c>.</value>
    public bool Indent { get; set; }

    /// <summary>
    /// The type attribute
    /// </summary>
    private string typeAttr;
    /// <summary>
    /// Gets or sets the type attribute.
    /// </summary>
    /// <value>The type attribute.</value>
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

    /// <summary>
    /// The type attribute span
    /// </summary>
    private ReadOnlyMemory<char>? typeAttrSpan;
    /// <summary>
    /// Gets the type attribute memory.
    /// </summary>
    /// <value>The type attribute memory.</value>
    public ReadOnlyMemory<char> TypeAttrMemory => typeAttrSpan ??= TypeAttr.AsMemory();
    /// <summary>
    /// Gets or sets the date time format.
    /// </summary>
    /// <value>The date time format.</value>
    public string DateTimeFormat { get; set; }
    /// <summary>
    /// The json type attribute in object
    /// </summary>
    private string jsonTypeAttrInObject;
    /// <summary>
    /// Gets the json type attribute in object.
    /// </summary>
    /// <value>The json type attribute in object.</value>
    internal string JsonTypeAttrInObject => jsonTypeAttrInObject ??= JsonTypeSerializer.GetTypeAttrInObject(TypeAttr);
    /// <summary>
    /// The JSV type attribute in object
    /// </summary>
    private string jsvTypeAttrInObject;
    /// <summary>
    /// Gets the JSV type attribute in object.
    /// </summary>
    /// <value>The JSV type attribute in object.</value>
    internal string JsvTypeAttrInObject => jsvTypeAttrInObject ??= JsvTypeSerializer.GetTypeAttrInObject(TypeAttr);

    /// <summary>
    /// Gets or sets the type writer.
    /// </summary>
    /// <value>The type writer.</value>
    public Func<Type, string> TypeWriter { get; set; }
    /// <summary>
    /// Gets or sets the type finder.
    /// </summary>
    /// <value>The type finder.</value>
    public Func<string, Type> TypeFinder { get; set; }
    /// <summary>
    /// Gets or sets the parse primitive function.
    /// </summary>
    /// <value>The parse primitive function.</value>
    public Func<string, object> ParsePrimitiveFn { get; set; }
    /// <summary>
    /// Gets or sets the date handler.
    /// </summary>
    /// <value>The date handler.</value>
    public DateHandler DateHandler { get; set; }
    /// <summary>
    /// Gets or sets the time span handler.
    /// </summary>
    /// <value>The time span handler.</value>
    public TimeSpanHandler TimeSpanHandler { get; set; }
    /// <summary>
    /// Gets or sets the property convention.
    /// </summary>
    /// <value>The property convention.</value>
    public PropertyConvention PropertyConvention { get; set; }

    /// <summary>
    /// Gets or sets the text case.
    /// </summary>
    /// <value>The text case.</value>
    public TextCase TextCase { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [throw on error].
    /// </summary>
    /// <value><c>true</c> if [throw on error]; otherwise, <c>false</c>.</value>
    public bool ThrowOnError { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [skip date time conversion].
    /// </summary>
    /// <value><c>true</c> if [skip date time conversion]; otherwise, <c>false</c>.</value>
    public bool SkipDateTimeConversion { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [always use UTC].
    /// </summary>
    /// <value><c>true</c> if [always use UTC]; otherwise, <c>false</c>.</value>
    public bool AlwaysUseUtc { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [assume UTC].
    /// </summary>
    /// <value><c>true</c> if [assume UTC]; otherwise, <c>false</c>.</value>
    public bool AssumeUtc { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [append UTC offset].
    /// </summary>
    /// <value><c>true</c> if [append UTC offset]; otherwise, <c>false</c>.</value>
    public bool AppendUtcOffset { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [prefer interfaces].
    /// </summary>
    /// <value><c>true</c> if [prefer interfaces]; otherwise, <c>false</c>.</value>
    public bool PreferInterfaces { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [include public fields].
    /// </summary>
    /// <value><c>true</c> if [include public fields]; otherwise, <c>false</c>.</value>
    public bool IncludePublicFields { get; set; }
    /// <summary>
    /// Gets or sets the maximum depth.
    /// </summary>
    /// <value>The maximum depth.</value>
    public int MaxDepth { get; set; }
    /// <summary>
    /// Gets or sets the on deserialization error.
    /// </summary>
    /// <value>The on deserialization error.</value>
    public DeserializationErrorDelegate OnDeserializationError { get; set; }
    /// <summary>
    /// Gets or sets the model factory.
    /// </summary>
    /// <value>The model factory.</value>
    public EmptyCtorFactoryDelegate ModelFactory { get; set; }
    /// <summary>
    /// Gets or sets the exclude property references.
    /// </summary>
    /// <value>The exclude property references.</value>
    public string[] ExcludePropertyReferences { get; set; }
    /// <summary>
    /// Gets or sets the exclude types.
    /// </summary>
    /// <value>The exclude types.</value>
    public HashSet<Type> ExcludeTypes { get; set; }
    /// <summary>
    /// Gets or sets the exclude type names.
    /// </summary>
    /// <value>The exclude type names.</value>
    public HashSet<string> ExcludeTypeNames { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [escape unicode].
    /// </summary>
    /// <value><c>true</c> if [escape unicode]; otherwise, <c>false</c>.</value>
    public bool EscapeUnicode { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [escape HTML chars].
    /// </summary>
    /// <value><c>true</c> if [escape HTML chars]; otherwise, <c>false</c>.</value>
    public bool EscapeHtmlChars { get; set; }

    /// <summary>
    /// Gets the defaults.
    /// </summary>
    /// <value>The defaults.</value>
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
    /// Populates the specified configuration.
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <returns>Config.</returns>
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