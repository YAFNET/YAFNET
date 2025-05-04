// ***********************************************************************
// <copyright file="WriteType.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.IO;
using System.Linq;
using System.Reflection;

using ServiceStack.OrmLite.Base.Text.Json;

namespace ServiceStack.OrmLite.Base.Text.Common;

/// <summary>
/// Class WriteType.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
static internal class WriteType<T, TSerializer>
    where TSerializer : ITypeSerializer
{
    /// <summary>
    /// The serializer
    /// </summary>
    private readonly static ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

    /// <summary>
    /// The property writers
    /// </summary>
    static internal TypePropertyWriter[] PropertyWriters;
    /// <summary>
    /// The write type information
    /// </summary>
    private readonly static WriteObjectDelegate WriteTypeInfo;

    /// <summary>
    /// Gets a value indicating whether this instance is included.
    /// </summary>
    /// <value><c>true</c> if this instance is included; otherwise, <c>false</c>.</value>
    private static bool IsIncluded =>
        JsConfig<T>.IncludeTypeInfo.GetValueOrDefault(JsConfig.IncludeTypeInfo);

    /// <summary>
    /// Gets a value indicating whether this instance is excluded.
    /// </summary>
    /// <value><c>true</c> if this instance is excluded; otherwise, <c>false</c>.</value>
    private static bool IsExcluded =>
        JsConfig<T>.ExcludeTypeInfo.GetValueOrDefault(JsConfig.ExcludeTypeInfo);

    /// <summary>
    /// Initializes static members of the <see cref="WriteType{T, TSerializer}" /> class.
    /// </summary>
    static WriteType()
    {
        if (typeof(T) == typeof(object))
        {
            Write = WriteObjectType;
        }
        else
        {
            Write = Init() ? GetWriteFn() : WriteEmptyType;
        }

        if (IsIncluded)
        {
            WriteTypeInfo = TypeInfoWriter;
        }

        if (typeof(T).IsAbstract)
        {
            WriteTypeInfo = TypeInfoWriter;
            if (!JsConfig.PreferInterfaces || !typeof(T).IsInterface)
            {
                Write = WriteAbstractProperties;
            }
        }
    }

    /// <summary>
    /// Types the information writer.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="obj">The object.</param>
    public static void TypeInfoWriter(TextWriter writer, object obj)
    {
        TryWriteTypeInfo(writer, obj);
    }

    /// <summary>
    /// Shoulds the type of the skip.
    /// </summary>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    private static bool ShouldSkipType() { return IsExcluded && !IsIncluded; }

    /// <summary>
    /// Tries the type of the write self.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    private static bool TryWriteSelfType(TextWriter writer)
    {
        if (ShouldSkipType())
        {
            return false;
        }

        var config = JsConfig.GetConfig();

        Serializer.WriteRawString(writer, config.TypeAttr);
        writer.Write(JsWriter.MapKeySeperator);
        Serializer.WriteRawString(writer, config.TypeWriter(typeof(T)));
        return true;
    }

    /// <summary>
    /// Tries the write type information.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="obj">The object.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    private static bool TryWriteTypeInfo(TextWriter writer, object obj)
    {
        if (obj == null || ShouldSkipType())
        {
            return false;
        }

        var config = JsConfig.GetConfig();

        Serializer.WriteRawString(writer, config.TypeAttr);
        writer.Write(JsWriter.MapKeySeperator);
        Serializer.WriteRawString(writer, config.TypeWriter(obj.GetType()));
        return true;
    }

    /// <summary>
    /// Gets the write.
    /// </summary>
    /// <value>The write.</value>
    public static WriteObjectDelegate Write { get; }

    /// <summary>
    /// Gets the write function.
    /// </summary>
    /// <returns>WriteObjectDelegate.</returns>
    private static WriteObjectDelegate GetWriteFn()
    {
        return WriteProperties;
    }

    /// <summary>
    /// Gets the should serialize method.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <returns>Func&lt;T, System.Boolean&gt;.</returns>
    static Func<T, bool> GetShouldSerializeMethod(MemberInfo member)
    {
        var method = member.DeclaringType.GetInstanceMethod("ShouldSerialize" + member.Name);
        return method == null || method.ReturnType != typeof(bool)
                   ? null
                   : (Func<T, bool>)method.CreateDelegate(typeof(Func<T, bool>));
    }

    /// <summary>
    /// Shoulds the serialize.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>Func&lt;T, System.String, System.Nullable&lt;System.Boolean&gt;&gt;.</returns>
    static Func<T, string, bool?> ShouldSerialize(Type type)
    {
        var method = type.GetMethodInfo("ShouldSerialize");
        return method == null || method.ReturnType != typeof(bool?)
                   ? null
                   : (Func<T, string, bool?>)method.CreateDelegate(typeof(Func<T, string, bool?>));
    }

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    private static bool Init()
    {
        if (!typeof(T).IsClass && !typeof(T).IsInterface && !JsConfig.TreatAsRefType(typeof(T)))
        {
            return false;
        }

        var propertyInfos = TypeConfig<T>.Properties;
        var fieldInfos = TypeConfig<T>.Fields;
        var propertyNamesLength = propertyInfos.Length;
        var fieldNamesLength = fieldInfos.Length;
        PropertyWriters = new TypePropertyWriter[propertyNamesLength + fieldNamesLength];

        if (propertyNamesLength + fieldNamesLength == 0 && !JsState.IsWritingDynamic)
        {
            return typeof(T).IsDto();
        }

        var shouldSerializeDynamic = ShouldSerialize(typeof(T));

        // NOTE: very limited support for DataContractSerialization (DCS)
        //	NOT supporting Serializable
        //	support for DCS is intended for (re)Name of properties and Ignore by NOT having a DataMember present
        var isDataContract = typeof(T).IsDto();
        for (var i = 0; i < propertyNamesLength; i++)
        {
            var propertyInfo = propertyInfos[i];

            string propertyName, propertyNameCLSFriendly, propertyNameLowercaseUnderscore, propertyDeclaredTypeName;
            var propertyOrder = -1;
            var propertyType = propertyInfo.PropertyType;
            var defaultValue = propertyType.GetDefaultValue();
            var propertySuppressDefaultConfig = defaultValue != null
                                                && propertyType.IsValueType
                                                && !propertyType.IsEnum
                                                && JsConfig.HasSerializeFn.Contains(propertyType)
                                                && !JsConfig.HasIncludeDefaultValue.Contains(propertyType);
            var propertySuppressDefaultAttribute = false;

            var shouldSerialize = GetShouldSerializeMethod(propertyInfo);
            if (isDataContract)
            {
                var dcsDataMember = propertyInfo.GetDataMember();
                if (dcsDataMember == null)
                {
                    continue;
                }

                propertyName = dcsDataMember.Name ?? propertyInfo.Name;
                propertyNameCLSFriendly = dcsDataMember.Name ?? propertyName.ToCamelCase();
                propertyNameLowercaseUnderscore = dcsDataMember.Name ?? propertyName.ToLowercaseUnderscore();
                propertyDeclaredTypeName = propertyType.GetDeclaringTypeName();
                propertyOrder = dcsDataMember.Order;
                propertySuppressDefaultAttribute = !dcsDataMember.EmitDefaultValue;
            }
            else
            {
                var dcsDataMember = propertyInfo.GetDataMember();
                var alias = dcsDataMember?.Name;

                propertyName = alias ?? propertyInfo.Name;
                propertyNameCLSFriendly = alias ?? propertyName.ToCamelCase();
                propertyNameLowercaseUnderscore = alias ?? propertyName.ToLowercaseUnderscore();
                propertyDeclaredTypeName = propertyInfo.GetDeclaringTypeName();
            }


            PropertyWriters[i] = new TypePropertyWriter
            (
                propertyType,
                propertyName,
                propertyDeclaredTypeName,
                propertyNameCLSFriendly,
                propertyNameLowercaseUnderscore,
                propertyOrder,
                propertySuppressDefaultConfig,
                propertySuppressDefaultAttribute,
                propertyInfo.CreateGetter<T>(),
                Serializer.GetWriteFn(propertyType),
                propertyType.GetDefaultValue(),
                shouldSerialize,
                shouldSerializeDynamic,
                propertyType.IsEnum
            );
        }

        for (var i = 0; i < fieldNamesLength; i++)
        {
            var fieldInfo = fieldInfos[i];

            string propertyName, propertyNameCLSFriendly, propertyNameLowercaseUnderscore, propertyDeclaredTypeName;
            var propertyOrder = -1;
            var propertyType = fieldInfo.FieldType;
            var defaultValue = propertyType.GetDefaultValue();
            var propertySuppressDefaultConfig = defaultValue != null
                                                && propertyType.IsValueType && !propertyType.IsEnum
                                                && JsConfig.HasSerializeFn.Contains(propertyType)
                                                && !JsConfig.HasIncludeDefaultValue.Contains(propertyType);
            var propertySuppressDefaultAttribute = false;

            var shouldSerialize = GetShouldSerializeMethod(fieldInfo);
            if (isDataContract)
            {
                var dcsDataMember = fieldInfo.GetDataMember();
                if (dcsDataMember == null)
                {
                    continue;
                }

                propertyName = dcsDataMember.Name ?? fieldInfo.Name;
                propertyNameCLSFriendly = dcsDataMember.Name ?? propertyName.ToCamelCase();
                propertyNameLowercaseUnderscore = dcsDataMember.Name ?? propertyName.ToLowercaseUnderscore();
                propertyDeclaredTypeName = fieldInfo.DeclaringType.Name;
                propertyOrder = dcsDataMember.Order;
                propertySuppressDefaultAttribute = !dcsDataMember.EmitDefaultValue;
            }
            else
            {
                var dcsDataMember = fieldInfo.GetDataMember();
                var alias = dcsDataMember?.Name;
                propertyName = alias ?? fieldInfo.Name;
                propertyNameCLSFriendly = alias ?? propertyName.ToCamelCase();
                propertyNameLowercaseUnderscore = alias ?? propertyName.ToLowercaseUnderscore();
                propertyDeclaredTypeName = fieldInfo.DeclaringType.Name;
            }

            PropertyWriters[i + propertyNamesLength] = new TypePropertyWriter
            (
                propertyType,
                propertyName,
                propertyDeclaredTypeName,
                propertyNameCLSFriendly,
                propertyNameLowercaseUnderscore,
                propertyOrder,
                propertySuppressDefaultConfig,
                propertySuppressDefaultAttribute,
                fieldInfo.CreateGetter<T>(),
                Serializer.GetWriteFn(propertyType),
                defaultValue,
                shouldSerialize,
                shouldSerializeDynamic,
                propertyType.IsEnum
            );
        }

        PropertyWriters = [.. PropertyWriters.OrderBy(x => x.propertyOrder)];
        return true;
    }

    /// <summary>
    /// Struct TypePropertyWriter
    /// </summary>
    readonly internal struct TypePropertyWriter
    {
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>System.String.</returns>
        internal string GetPropertyName(Config config)
        {
            return config.TextCase switch {
                    TextCase.CamelCase => this.propertyNameCLSFriendly,
                    TextCase.SnakeCase => this.propertyNameLowercaseUnderscore,
                    _ => this.propertyName
                };
        }

        /// <summary>
        /// The property type
        /// </summary>
        readonly internal Type PropertyType;
        /// <summary>
        /// The property name
        /// </summary>
        readonly internal string propertyName;
        /// <summary>
        /// The property order
        /// </summary>
        readonly internal int propertyOrder;
        /// <summary>
        /// The property suppress default configuration
        /// </summary>
        readonly internal bool propertySuppressDefaultConfig;
        /// <summary>
        /// The property suppress default attribute
        /// </summary>
        readonly internal bool propertySuppressDefaultAttribute;
        /// <summary>
        /// The property reference name
        /// </summary>
        readonly internal string propertyReferenceName;
        /// <summary>
        /// The property name CLS friendly
        /// </summary>
        readonly internal string propertyNameCLSFriendly;
        /// <summary>
        /// The property name lowercase underscore
        /// </summary>
        readonly internal string propertyNameLowercaseUnderscore;
        /// <summary>
        /// The getter function
        /// </summary>
        readonly internal GetMemberDelegate<T> GetterFn;
        /// <summary>
        /// The write function
        /// </summary>
        readonly internal WriteObjectDelegate WriteFn;
        /// <summary>
        /// The default value
        /// </summary>
        readonly internal object DefaultValue;
        /// <summary>
        /// The should serialize
        /// </summary>
        readonly internal Func<T, bool> shouldSerialize;
        /// <summary>
        /// The should serialize dynamic
        /// </summary>
        readonly internal Func<T, string, bool?> shouldSerializeDynamic;
        /// <summary>
        /// The is enum
        /// </summary>
        readonly internal bool isEnum;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypePropertyWriter" /> struct.
        /// </summary>
        /// <param name="propertyType">Type of the property.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyDeclaredTypeName">Name of the property declared type.</param>
        /// <param name="propertyNameCLSFriendly">The property name CLS friendly.</param>
        /// <param name="propertyNameLowercaseUnderscore">The property name lowercase underscore.</param>
        /// <param name="propertyOrder">The property order.</param>
        /// <param name="propertySuppressDefaultConfig">if set to <c>true</c> [property suppress default configuration].</param>
        /// <param name="propertySuppressDefaultAttribute">if set to <c>true</c> [property suppress default attribute].</param>
        /// <param name="getterFn">The getter function.</param>
        /// <param name="writeFn">The write function.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="shouldSerialize">The should serialize.</param>
        /// <param name="shouldSerializeDynamic">The should serialize dynamic.</param>
        /// <param name="isEnum">if set to <c>true</c> [is enum].</param>
        public TypePropertyWriter(Type propertyType, string propertyName, string propertyDeclaredTypeName, string propertyNameCLSFriendly,
                                  string propertyNameLowercaseUnderscore, int propertyOrder, bool propertySuppressDefaultConfig, bool propertySuppressDefaultAttribute,
                                  GetMemberDelegate<T> getterFn, WriteObjectDelegate writeFn, object defaultValue,
                                  Func<T, bool> shouldSerialize,
                                  Func<T, string, bool?> shouldSerializeDynamic,
                                  bool isEnum)
        {
            this.PropertyType = propertyType;
            this.propertyName = propertyName;
            this.propertyOrder = propertyOrder;
            this.propertySuppressDefaultConfig = propertySuppressDefaultConfig;
            this.propertySuppressDefaultAttribute = propertySuppressDefaultAttribute;
            this.propertyReferenceName = propertyDeclaredTypeName + "." + propertyName;
            this.propertyNameCLSFriendly = propertyNameCLSFriendly;
            this.propertyNameLowercaseUnderscore = propertyNameLowercaseUnderscore;
            this.GetterFn = getterFn;
            this.WriteFn = writeFn;
            this.DefaultValue = defaultValue;
            this.shouldSerialize = shouldSerialize;
            this.shouldSerializeDynamic = shouldSerializeDynamic;
            this.isEnum = isEnum;
        }

        /// <summary>
        /// Shoulds the write property.
        /// </summary>
        /// <param name="propertyValue">The property value.</param>
        /// <param name="config">The configuration.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool ShouldWriteProperty(object propertyValue, Config config)
        {
            var isDefaultValue = propertyValue == null || Equals(this.DefaultValue, propertyValue);
            if (isDefaultValue)
            {
                if (!this.isEnum)
                {
                    if (this.propertySuppressDefaultAttribute || config.ExcludeDefaultValues)
                    {
                        return false;
                    }

                    if (!Serializer.IncludeNullValues && (propertyValue == null || this.propertySuppressDefaultConfig))
                    {
                        return false;
                    }
                }
                else if (!config.IncludeDefaultEnums)
                {
                    return false;
                }
            }

            return true;
        }
    }

    /// <summary>
    /// Writes the type of the object.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    public static void WriteObjectType(TextWriter writer, object value)
    {
        writer.Write(JsWriter.EmptyMap);
    }

    /// <summary>
    /// Writes the empty type.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    public static void WriteEmptyType(TextWriter writer, object value)
    {
        if (WriteTypeInfo != null || JsState.IsWritingDynamic)
        {
            writer.Write(JsWriter.MapStartChar);
            if (!(JsConfig.PreferInterfaces && TryWriteSelfType(writer)))
            {
                TryWriteTypeInfo(writer, value);
            }
            writer.Write(JsWriter.MapEndChar);
            return;
        }
        writer.Write(JsWriter.EmptyMap);
    }

    /// <summary>
    /// Writes the abstract properties.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    public static void WriteAbstractProperties(TextWriter writer, object value)
    {
        if (value == null)
        {
            writer.Write(JsWriter.EmptyMap);
            return;
        }
        var valueType = value.GetType();
        if (valueType.IsAbstract)
        {
            WriteProperties(writer, value);
            return;
        }

        WriteLateboundProperties(writer, value, valueType);
    }

    /// <summary>
    /// Writes the properties.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="instance">The instance.</param>
    public static void WriteProperties(TextWriter writer, object instance)
    {
        if (instance == null)
        {
            writer.Write(JsWriter.EmptyMap);
            return;
        }

        var valueType = instance.GetType();
        if (PropertyWriters != null && valueType != typeof(T) && !typeof(T).IsAbstract)
        {
            WriteLateboundProperties(writer, instance, valueType);
            return;
        }

        if (typeof(TSerializer) == typeof(JsonTypeSerializer) && JsState.WritingKeyCount > 0)
        {
            writer.Write(JsWriter.QuoteChar);
        }

        writer.Write(JsWriter.MapStartChar);

        var i = 0;
        var writeTypeInfo = WriteTypeInfo != null || JsState.IsWritingDynamic
                                                  || (JsConfigScope.Current != null && JsConfigScope.Current.IncludeTypeInfo && !JsConfigScope.Current.ExcludeTypeInfo);
        if (writeTypeInfo)
        {
            if (JsConfig.PreferInterfaces && TryWriteSelfType(writer))
            {
                i++;
            }
            else if (TryWriteTypeInfo(writer, instance))
            {
                i++;
            }

            JsState.IsWritingDynamic = false;
        }

        if (PropertyWriters != null)
        {
            var config = JsConfig<T>.GetConfig();

            var typedInstance = (T)instance;
            var len = PropertyWriters.Length;
            for (var index = 0; index < len; index++)
            {
                var propertyWriter = PropertyWriters[index];

                if (propertyWriter.shouldSerialize?.Invoke(typedInstance) == false)
                {
                    continue;
                }

                var dontSkipDefault = false;
                if (propertyWriter.shouldSerializeDynamic != null)
                {
                    var shouldSerialize = propertyWriter.shouldSerializeDynamic(typedInstance, propertyWriter.GetPropertyName(config));
                    if (shouldSerialize.HasValue)
                    {
                        if (shouldSerialize.Value)
                        {
                            dontSkipDefault = true;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }

                var propertyValue = propertyWriter.GetterFn(typedInstance);

                if (!dontSkipDefault)
                {
                    if (!propertyWriter.ShouldWriteProperty(propertyValue, config))
                    {
                        continue;
                    }

                    if (config.ExcludePropertyReferences?.Contains(propertyWriter.propertyReferenceName) == true)
                    {
                        continue;
                    }
                }

                if (i++ > 0)
                {
                    writer.Write(JsWriter.ItemSeperator);
                }

                Serializer.WritePropertyName(writer, propertyWriter.GetPropertyName(config));
                writer.Write(JsWriter.MapKeySeperator);

                if (typeof(TSerializer) == typeof(JsonTypeSerializer))
                {
                    JsState.IsWritingValue = true;
                }

                try
                {
                    if (propertyValue == null)
                    {
                        writer.Write(JsonUtils.Null);
                    }
                    else
                    {
                        propertyWriter.WriteFn(writer, propertyValue);
                    }
                }
                finally
                {
                    if (typeof(TSerializer) == typeof(JsonTypeSerializer))
                    {
                        JsState.IsWritingValue = false;
                    }
                }
            }
        }

        writer.Write(JsWriter.MapEndChar);

        if (typeof(TSerializer) == typeof(JsonTypeSerializer) && JsState.WritingKeyCount > 0)
        {
            writer.Write(JsWriter.QuoteChar);
        }
    }

    /// <summary>
    /// Writes the latebound properties.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    /// <param name="valueType">Type of the value.</param>
    private static void WriteLateboundProperties(TextWriter writer, object value, Type valueType)
    {
        var writeFn = Serializer.GetWriteFn(valueType);
        var prevState = JsState.IsWritingDynamic;
        if (!JsConfig<T>.ExcludeTypeInfo.GetValueOrDefault())
        {
            JsState.IsWritingDynamic = true;
        }

        writeFn(writer, value);
        if (!JsConfig<T>.ExcludeTypeInfo.GetValueOrDefault())
        {
            JsState.IsWritingDynamic = prevState;
        }
    }

    /// <summary>
    /// Gets the name of the property.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="config">The configuration.</param>
    /// <returns>System.String.</returns>
    static internal string GetPropertyName(string propertyName, Config config)
    {
        return config.TextCase switch {
                TextCase.CamelCase => propertyName.ToCamelCase(),
                TextCase.SnakeCase => propertyName.ToLowercaseUnderscore(),
                _ => propertyName
            };
    }
}