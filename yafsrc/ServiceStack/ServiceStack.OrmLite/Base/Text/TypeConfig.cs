// ***********************************************************************
// <copyright file="TypeConfig.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Linq;
using System.Reflection;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Class TypeConfig.
/// </summary>
internal class TypeConfig
{
    /// <summary>
    /// The type
    /// </summary>
    readonly internal Type Type;

    /// <summary>
    /// The enable anonymous field setters
    /// </summary>
    internal bool EnableAnonymousFieldSetters;

    /// <summary>
    /// The properties
    /// </summary>
    internal PropertyInfo[] Properties;

    /// <summary>
    /// The fields
    /// </summary>
    internal FieldInfo[] Fields;

    /// <summary>
    /// The on deserializing
    /// </summary>
    internal Func<object, string, object, object> OnDeserializing;

    /// <summary>
    /// Gets or sets a value indicating whether this instance is user type.
    /// </summary>
    /// <value><c>true</c> if this instance is user type; otherwise, <c>false</c>.</value>
    internal bool IsUserType { get; set; }

    /// <summary>
    /// The text case resolver
    /// </summary>
    internal Func<TextCase> TextCaseResolver;
    /// <summary>
    /// Gets the text case.
    /// </summary>
    /// <value>The text case.</value>
    internal TextCase? TextCase
    {
        get
        {
            var result = this.TextCaseResolver?.Invoke();
            return result is null or Text.TextCase.Default ? null : result;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeConfig" /> class.
    /// </summary>
    /// <param name="type">The type.</param>
    internal TypeConfig(Type type)
    {
        this.Type = type;
        this.EnableAnonymousFieldSetters = false;
        this.Properties = TypeConstants.EmptyPropertyInfoArray;
        this.Fields = TypeConstants.EmptyFieldInfoArray;

        JsConfig.AddUniqueType(this.Type);
    }
}

/// <summary>
/// Class TypeConfig.
/// </summary>
/// <typeparam name="T"></typeparam>
public static class TypeConfig<T>
{
    /// <summary>
    /// The configuration
    /// </summary>
    static internal TypeConfig config;

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    /// <value>The configuration.</value>
    static TypeConfig Config => config ??= Create();

    /// <summary>
    /// Gets or sets the properties.
    /// </summary>
    /// <value>The properties.</value>
    public static PropertyInfo[] Properties
    {
        get => Config.Properties;
        set => Config.Properties = value;
    }

    /// <summary>
    /// Gets or sets the fields.
    /// </summary>
    /// <value>The fields.</value>
    public static FieldInfo[] Fields
    {
        get => Config.Fields;
        set => Config.Fields = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is user type.
    /// </summary>
    /// <value><c>true</c> if this instance is user type; otherwise, <c>false</c>.</value>
    public static bool IsUserType
    {
        get => Config.IsUserType;
        set => Config.IsUserType = value;
    }

    /// <summary>
    /// Initializes static members of the <see cref="TypeConfig{T}" /> class.
    /// </summary>
    static TypeConfig()
    {
        Init();
    }

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    static internal void Init()
    {
        if (config == null)
        {
            Create();
        }
    }

    /// <summary>
    /// Gets or sets the on deserializing.
    /// </summary>
    /// <value>The on deserializing.</value>
    public static Func<object, string, object, object> OnDeserializing
    {
        get => config.OnDeserializing;
        set => config.OnDeserializing = value;
    }

    /// <summary>
    /// Creates this instance.
    /// </summary>
    /// <returns>TypeConfig.</returns>
    static TypeConfig Create()
    {
        config = new TypeConfig(typeof(T))
                     {
                         TextCaseResolver = () => JsConfig<T>.TextCase
                     };

        var excludedProperties = JsConfig<T>.ExcludePropertyNames ?? TypeConstants.EmptyStringArray;

        var properties = excludedProperties.Length > 0
                             ? config.Type.GetSerializableProperties().Where(x => !excludedProperties.Contains(x.Name))
                             : config.Type.GetSerializableProperties();
        Properties = [.. properties.Where(x => x.GetIndexParameters().Length == 0)];

        Fields = [.. config.Type.GetSerializableFields()];

        if (!JsConfig<T>.HasDeserializingFn)
        {
            OnDeserializing = ReflectionExtensions.GetOnDeserializing<T>();
        }
        else
        {
            config.OnDeserializing = (instance, memberName, value) => JsConfig<T>.OnDeserializingFn((T)instance, memberName, value);
        }

        IsUserType = !typeof(T).IsValueType && typeof(T).Namespace != "System";

        return config;
    }

    /// <summary>
    /// Resets this instance.
    /// </summary>
    public static void Reset()
    {
        config = null;
    }

    /// <summary>
    /// Gets the state.
    /// </summary>
    /// <returns>TypeConfig.</returns>
    static internal TypeConfig GetState()
    {
        return Config;
    }
}