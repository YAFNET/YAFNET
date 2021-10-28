// ***********************************************************************
// <copyright file="WriteType.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ServiceStack.Text.Json;
using ServiceStack.Text.Jsv;

namespace ServiceStack.Text.Common
{
    /// <summary>
    /// Class WriteType.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
    internal static class WriteType<T, TSerializer>
        where TSerializer : ITypeSerializer
    {
        /// <summary>
        /// The serializer
        /// </summary>
        private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

        /// <summary>
        /// The cache function
        /// </summary>
        private static readonly WriteObjectDelegate CacheFn;
        /// <summary>
        /// The property writers
        /// </summary>
        internal static TypePropertyWriter[] PropertyWriters;
        /// <summary>
        /// The write type information
        /// </summary>
        private static readonly WriteObjectDelegate WriteTypeInfo;

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
                CacheFn = WriteObjectType;
            }
            else
            {
                CacheFn = Init() ? GetWriteFn() : WriteEmptyType;
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
                    CacheFn = WriteAbstractProperties;
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
            if (ShouldSkipType()) return false;

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
            if (obj == null || ShouldSkipType()) return false;

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
        public static WriteObjectDelegate Write => CacheFn;

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
            if (!typeof(T).IsClass && !typeof(T).IsInterface && !JsConfig.TreatAsRefType(typeof(T))) return false;

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
                int propertyOrder = -1;
                var propertyType = propertyInfo.PropertyType;
                var defaultValue = propertyType.GetDefaultValue();
                bool propertySuppressDefaultConfig = defaultValue != null
                    && propertyType.IsValueType
                    && !propertyType.IsEnum
                    && JsConfig.HasSerializeFn.Contains(propertyType)
                    && !JsConfig.HasIncludeDefaultValue.Contains(propertyType);
                bool propertySuppressDefaultAttribute = false;

                var shouldSerialize = GetShouldSerializeMethod(propertyInfo);
                if (isDataContract)
                {
                    var dcsDataMember = propertyInfo.GetDataMember();
                    if (dcsDataMember == null) continue;

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
                int propertyOrder = -1;
                var propertyType = fieldInfo.FieldType;
                var defaultValue = propertyType.GetDefaultValue();
                bool propertySuppressDefaultConfig = defaultValue != null
                    && propertyType.IsValueType && !propertyType.IsEnum
                    && JsConfig.HasSerializeFn.Contains(propertyType)
                    && !JsConfig.HasIncludeDefaultValue.Contains(propertyType);
                bool propertySuppressDefaultAttribute = false;

                var shouldSerialize = GetShouldSerializeMethod(fieldInfo);
                if (isDataContract)
                {
                    var dcsDataMember = fieldInfo.GetDataMember();
                    if (dcsDataMember == null) continue;

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

            PropertyWriters = PropertyWriters.OrderBy(x => x.propertyOrder).ToArray();
            return true;
        }

        /// <summary>
        /// Struct TypePropertyWriter
        /// </summary>
        internal struct TypePropertyWriter
        {
            /// <summary>
            /// Gets the name of the property.
            /// </summary>
            /// <param name="config">The configuration.</param>
            /// <returns>System.String.</returns>
            internal string GetPropertyName(Config config)
            {
                return config.TextCase switch {
                    TextCase.CamelCase => propertyNameCLSFriendly,
                    TextCase.SnakeCase => propertyNameLowercaseUnderscore,
                    _ => propertyName
                };
            }

            /// <summary>
            /// The property type
            /// </summary>
            internal readonly Type PropertyType;
            /// <summary>
            /// The property name
            /// </summary>
            internal readonly string propertyName;
            /// <summary>
            /// The property order
            /// </summary>
            internal readonly int propertyOrder;
            /// <summary>
            /// The property suppress default configuration
            /// </summary>
            internal readonly bool propertySuppressDefaultConfig;
            /// <summary>
            /// The property suppress default attribute
            /// </summary>
            internal readonly bool propertySuppressDefaultAttribute;
            /// <summary>
            /// The property reference name
            /// </summary>
            internal readonly string propertyReferenceName;
            /// <summary>
            /// The property name CLS friendly
            /// </summary>
            internal readonly string propertyNameCLSFriendly;
            /// <summary>
            /// The property name lowercase underscore
            /// </summary>
            internal readonly string propertyNameLowercaseUnderscore;
            /// <summary>
            /// The getter function
            /// </summary>
            internal readonly GetMemberDelegate<T> GetterFn;
            /// <summary>
            /// The write function
            /// </summary>
            internal readonly WriteObjectDelegate WriteFn;
            /// <summary>
            /// The default value
            /// </summary>
            internal readonly object DefaultValue;
            /// <summary>
            /// The should serialize
            /// </summary>
            internal readonly Func<T, bool> shouldSerialize;
            /// <summary>
            /// The should serialize dynamic
            /// </summary>
            internal readonly Func<T, string, bool?> shouldSerializeDynamic;
            /// <summary>
            /// The is enum
            /// </summary>
            internal readonly bool isEnum;

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
                var isDefaultValue = propertyValue == null || Equals(DefaultValue, propertyValue);
                if (isDefaultValue)
                {
                    if (!isEnum)
                    {
                        if (propertySuppressDefaultAttribute || config.ExcludeDefaultValues)
                            return false;
                        if (!Serializer.IncludeNullValues && (propertyValue == null || propertySuppressDefaultConfig))
                            return false;
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
                writer.Write(JsWriter.QuoteChar);

            writer.Write(JsWriter.MapStartChar);

            var i = 0;
            if (WriteTypeInfo != null || JsState.IsWritingDynamic)
            {
                if (JsConfig.PreferInterfaces && TryWriteSelfType(writer)) i++;
                else if (TryWriteTypeInfo(writer, instance)) i++;
                JsState.IsWritingDynamic = false;
            }

            if (PropertyWriters != null)
            {
                var config = JsConfig<T>.GetConfig();

                var typedInstance = (T)instance;
                var len = PropertyWriters.Length;
                for (int index = 0; index < len; index++)
                {
                    var propertyWriter = PropertyWriters[index];

                    if (propertyWriter.shouldSerialize?.Invoke(typedInstance) == false)
                        continue;

                    var dontSkipDefault = false;
                    if (propertyWriter.shouldSerializeDynamic != null)
                    {
                        var shouldSerialize = propertyWriter.shouldSerializeDynamic(typedInstance, propertyWriter.GetPropertyName(config));
                        if (shouldSerialize.HasValue)
                        {
                            if (shouldSerialize.Value)
                                dontSkipDefault = true;
                            else
                                continue;
                        }
                    }

                    var propertyValue = propertyWriter.GetterFn(typedInstance);

                    if (!dontSkipDefault)
                    {
                        if (!propertyWriter.ShouldWriteProperty(propertyValue, config))
                            continue;

                        if (config.ExcludePropertyReferences?.Contains(propertyWriter.propertyReferenceName) == true) continue;
                    }

                    if (i++ > 0)
                        writer.Write(JsWriter.ItemSeperator);

                    Serializer.WritePropertyName(writer, propertyWriter.GetPropertyName(config));
                    writer.Write(JsWriter.MapKeySeperator);

                    if (typeof(TSerializer) == typeof(JsonTypeSerializer)) JsState.IsWritingValue = true;
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
                        if (typeof(TSerializer) == typeof(JsonTypeSerializer)) JsState.IsWritingValue = false;
                    }
                }
            }

            writer.Write(JsWriter.MapEndChar);

            if (typeof(TSerializer) == typeof(JsonTypeSerializer) && JsState.WritingKeyCount > 0)
                writer.Write(JsWriter.QuoteChar);
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
            if (!JsConfig<T>.ExcludeTypeInfo.GetValueOrDefault()) JsState.IsWritingDynamic = true;
            writeFn(writer, value);
            if (!JsConfig<T>.ExcludeTypeInfo.GetValueOrDefault()) JsState.IsWritingDynamic = false;
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="config">The configuration.</param>
        /// <returns>System.String.</returns>
        internal static string GetPropertyName(string propertyName, Config config)
        {
            return config.TextCase switch {
                TextCase.CamelCase => propertyName.ToCamelCase(),
                TextCase.SnakeCase => propertyName.ToLowercaseUnderscore(),
                _ => propertyName
            };
        }

        /// <summary>
        /// The array brackets
        /// </summary>
        private static readonly char[] ArrayBrackets = { '[', ']' };

        /// <summary>
        /// Writes the complex query string properties.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="instance">The instance.</param>
        public static void WriteComplexQueryStringProperties(string typeName, TextWriter writer, object instance)
        {
            var i = 0;
            if (PropertyWriters != null)
            {
                var config = JsConfig<T>.GetConfig();

                var typedInstance = (T)instance;
                var len = PropertyWriters.Length;
                for (var index = 0; index < len; index++)
                {
                    var propertyWriter = PropertyWriters[index];
                    if (propertyWriter.shouldSerialize != null && !propertyWriter.shouldSerialize(typedInstance))
                        continue;

                    var propertyValue = instance != null ? propertyWriter.GetterFn(typedInstance) : null;
                    if (propertyWriter.propertySuppressDefaultAttribute && Equals(propertyWriter.DefaultValue, propertyValue))
                        continue;

                    if ((propertyValue == null
                         || propertyWriter.propertySuppressDefaultConfig && Equals(propertyWriter.DefaultValue, propertyValue))
                        && !Serializer.IncludeNullValues)
                        continue;

                    if (config.ExcludePropertyReferences != null && config.ExcludePropertyReferences.Contains(propertyWriter.propertyReferenceName))
                        continue;

                    if (i++ > 0)
                        writer.Write('&');

                    var propertyValueType = propertyValue?.GetType();
                    if (propertyValueType != null &&
                        propertyValueType.IsUserType() &&
                        !propertyValueType.HasInterface(typeof(IEnumerable)))
                    {
                        //Nested Complex Type: legal_entity[dob][day]=1
                        var prefix = $"{typeName}[{propertyWriter.GetPropertyName(config)}]";
                        var props = propertyValueType.GetSerializableProperties();
                        for (int j = 0; j < props.Length; j++)
                        {
                            var pi = props[j];
                            var pValue = pi.GetValue(propertyValue, TypeConstants.EmptyObjectArray);
                            if (pValue == null && !Serializer.IncludeNullValues)
                                continue;

                            if (j > 0)
                                writer.Write('&');

                            writer.Write(prefix);
                            writer.Write('[');
                            writer.Write(GetPropertyName(pi.Name, config));
                            writer.Write("]=");

                            if (pValue == null)
                            {
                                writer.Write(JsonUtils.Null);
                            }
                            else
                            {
                                JsvWriter.GetWriteFn(pValue.GetType())(writer, pValue);
                            }
                        }
                    }
                    else
                    {
                        writer.Write(typeName);
                        writer.Write('[');
                        writer.Write(propertyWriter.GetPropertyName(config));
                        writer.Write("]=");

                        if (propertyValue == null)
                        {
                            writer.Write(JsonUtils.Null);
                        }
                        else
                        {
                            propertyWriter.WriteFn(writer, propertyValue);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Writes the query string.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="instance">The instance.</param>
        public static void WriteQueryString(TextWriter writer, object instance)
        {
            try
            {
                JsState.QueryStringMode = true;
                var config = JsConfig<T>.GetConfig();

                var i = 0;
                var typedInstance = (T)instance;
                foreach (var propertyWriter in PropertyWriters)
                {
                    var propertyValue = propertyWriter.GetterFn(typedInstance);
                    if (propertyValue == null) continue;

                    if (i++ > 0)
                        writer.Write('&');

                    var propertyType = propertyValue.GetType();
                    var strValue = propertyValue as string;
                    var isEnumerable = strValue == null
                        && !propertyType.IsValueType
                        && propertyType.HasInterface(typeof(IEnumerable));

                    if (QueryStringSerializer.ComplexTypeStrategy != null)
                    {
                        var nonEnumerableUserType = !isEnumerable && (propertyType.IsUserType() || propertyType.IsInterface);
                        if (nonEnumerableUserType || propertyType.IsOrHasGenericInterfaceTypeOf(typeof(IDictionary<,>)))
                        {
                            if (QueryStringSerializer.ComplexTypeStrategy(writer, propertyWriter.GetPropertyName(config), propertyValue))
                                continue;
                        }
                    }

                    Serializer.WritePropertyName(writer, propertyWriter.GetPropertyName(config));
                    writer.Write('=');

                    if (strValue != null)
                    {
                        writer.Write(strValue.UrlEncode());
                    }
                    else if (!isEnumerable)
                    {
                        propertyWriter.WriteFn(writer, propertyValue);
                    }
                    else
                    {
                        //Trim brackets in top-level lists in QueryStrings, e.g: ?a=[1,2,3] => ?a=1,2,3
                        using var ms = MemoryStreamFactory.GetStream();
                        var enumerableWriter = new StreamWriter(ms); //ms disposed in using 
                        propertyWriter.WriteFn(enumerableWriter, propertyValue);
                        enumerableWriter.Flush();
                        var output = ms.ReadToEnd();
                        output = output.Trim(ArrayBrackets);
                        writer.Write(output);
                    }
                }
            }
            finally
            {
                JsState.QueryStringMode = false;
            }
        }
    }
}
